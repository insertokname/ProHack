// Diagnose collider values on the current map, focusing on water and value 5
(function() {
"use strict";

const mod = Process.getModuleByName("GameAssembly.dll");
const ARRAY_HEADER = 0x20;

const il2cpp = {
    domain_get:             new NativeFunction(mod.getExportByName("il2cpp_domain_get"),               "pointer", []),
    domain_get_assemblies:  new NativeFunction(mod.getExportByName("il2cpp_domain_get_assemblies"),    "pointer", ["pointer", "pointer"]),
    assembly_get_image:     new NativeFunction(mod.getExportByName("il2cpp_assembly_get_image"),       "pointer", ["pointer"]),
    image_get_class_count:  new NativeFunction(mod.getExportByName("il2cpp_image_get_class_count"),    "uint",    ["pointer"]),
    image_get_class:        new NativeFunction(mod.getExportByName("il2cpp_image_get_class"),          "pointer", ["pointer", "uint"]),
    class_get_name:         new NativeFunction(mod.getExportByName("il2cpp_class_get_name"),           "pointer", ["pointer"]),
    class_get_fields:       new NativeFunction(mod.getExportByName("il2cpp_class_get_fields"),         "pointer", ["pointer", "pointer"]),
    field_get_name:         new NativeFunction(mod.getExportByName("il2cpp_field_get_name"),           "pointer", ["pointer"]),
    field_get_offset:       new NativeFunction(mod.getExportByName("il2cpp_field_get_offset"),         "uint",    ["pointer"]),
    field_get_flags:        new NativeFunction(mod.getExportByName("il2cpp_field_get_flags"),          "int",     ["pointer"]),
    field_static_get_value: new NativeFunction(mod.getExportByName("il2cpp_field_static_get_value"),   "void",    ["pointer", "pointer"]),
};

function readStr(p) { try { return (p && !p.isNull()) ? p.readUtf8String() : null; } catch (_) { return null; } }
function isValid(p) { if (!p || p.isNull()) return false; const v = p.toUInt32(); return v > 0x10000 && v !== 0xffffffff; }
function readIl2CppString(p) {
    try { if (!p || p.isNull()) return null; const len = p.add(0x10).readS32();
    if (len <= 0 || len > 4096) return null; return p.add(0x14).readUtf16String(len); } catch (_) { return null; }
}
function findClass(name) {
    const domain = il2cpp.domain_get(); const cntBuf = Memory.alloc(4);
    const asmPtrs = il2cpp.domain_get_assemblies(domain, cntBuf); const asmCount = cntBuf.readU32();
    for (let a = 0; a < asmCount; a++) {
        try { const asm = asmPtrs.add(a * Process.pointerSize).readPointer(); const img = il2cpp.assembly_get_image(asm);
        if (!isValid(img)) continue; const cc = il2cpp.image_get_class_count(img);
        for (let c = 0; c < cc; c++) { const klass = il2cpp.image_get_class(img, c); if (!isValid(klass)) continue;
        if (readStr(il2cpp.class_get_name(klass)) === name) return klass; } } catch (_) {} } return null;
}
function getField(klass, fn) {
    const iter = Memory.alloc(Process.pointerSize); iter.writePointer(ptr(0)); let f;
    while (!(f = il2cpp.class_get_fields(klass, iter)).isNull()) {
        if (readStr(il2cpp.field_get_name(f)) === fn) return { field: f, offset: il2cpp.field_get_offset(f), isStatic: (il2cpp.field_get_flags(f) & 0x10) !== 0 };
    } return null;
}
function getStaticValue(fi) { const buf = Memory.alloc(Process.pointerSize); il2cpp.field_static_get_value(fi.field, buf); return buf.readPointer(); }

const dsKlass = findClass("DSSock");
const ds = getStaticValue(getField(dsKlass, "<pjm>k__BackingField"));
const mc = ds.add(getField(dsKlass, "MapCreator").offset).readPointer();
const mcKlass = findClass("MapCreator");
const w = mc.add(getField(mcKlass, "Width").offset).readS32();
const h = mc.add(getField(mcKlass, "Height").offset).readS32();
const mapNameStr = readIl2CppString(mc.add(getField(mcKlass, "MapName").offset).readPointer()) || "?";

// Read colliders
const colPtr = mc.add(getField(mcKlass, "Colliders").offset).readPointer();
const colliders = new Uint8Array(colPtr.add(ARRAY_HEADER).readByteArray(w * h));

// Read all tile layers for context
const layers = {};
for (const ln of ["Tiles", "Tiles2", "Tiles3", "Tiles4"]) {
    const lf = getField(mcKlass, ln);
    if (lf) {
        const lp = mc.add(lf.offset).readPointer();
        if (isValid(lp)) layers[ln] = new Uint32Array(lp.add(ARRAY_HEADER).readByteArray(w * h * 4));
    }
}

console.log("[+] Map: " + mapNameStr + " (" + w + "x" + h + ")");

// Count all collider values
const valCounts = {};
for (let i = 0; i < colliders.length; i++) {
    const v = colliders[i];
    if (v !== 0) valCounts[v] = (valCounts[v] || 0) + 1;
}
console.log("\n=== ALL non-zero collider values ===");
console.log(JSON.stringify(valCounts));

// For each non-standard value (not 0, 1, 6), show positions and surrounding context
console.log("\n=== Non-standard collider positions (not 0/1/6) ===");
for (let x = 0; x < w; x++) {
    for (let y = 0; y < h; y++) {
        const idx = x * h + y;
        const cv = colliders[idx];
        if (cv === 0 || cv === 1 || cv === 6) continue;
        
        // Get tile IDs at this position
        const tileIds = [];
        for (const ln of ["Tiles", "Tiles2", "Tiles3", "Tiles4"]) {
            if (layers[ln]) tileIds.push(ln + "=" + layers[ln][idx]);
        }
        
        // Get neighbors
        const nbrs = [];
        const dirs = [["U",x,y-1],["D",x,y+1],["L",x-1,y],["R",x+1,y]];
        for (const [dir, nx, ny] of dirs) {
            if (nx >= 0 && nx < w && ny >= 0 && ny < h) {
                nbrs.push(dir + ":" + colliders[nx * h + ny]);
            }
        }
        
        console.log("  (" + x + "," + y + ") cv=" + cv + " " + tileIds.join(" ") + " | nbrs: " + nbrs.join(" "));
    }
}

console.log("\n[*] Done.");
send({ type: 'exit' });
})();
