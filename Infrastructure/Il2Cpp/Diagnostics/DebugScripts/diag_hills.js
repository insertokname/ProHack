// Diagnose hill collider values and their neighbors to determine direction encoding
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
    class_get_method_from_name: new NativeFunction(mod.getExportByName("il2cpp_class_get_method_from_name"), "pointer", ["pointer", "pointer", "int"]),
    field_get_name:         new NativeFunction(mod.getExportByName("il2cpp_field_get_name"),           "pointer", ["pointer"]),
    field_get_offset:       new NativeFunction(mod.getExportByName("il2cpp_field_get_offset"),         "uint",    ["pointer"]),
    field_get_flags:        new NativeFunction(mod.getExportByName("il2cpp_field_get_flags"),          "int",     ["pointer"]),
    field_static_get_value: new NativeFunction(mod.getExportByName("il2cpp_field_static_get_value"),   "void",    ["pointer", "pointer"]),
};

function readStr(p) { try { return (p && !p.isNull()) ? p.readUtf8String() : null; } catch (_) { return null; } }
function isValid(p) { if (!p || p.isNull()) return false; const v = p.toUInt32(); return v > 0x10000 && v !== 0xffffffff; }
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

// Read colliders
const colPtr = mc.add(getField(mcKlass, "Colliders").offset).readPointer();
const colliders = new Uint8Array(colPtr.add(ARRAY_HEADER).readByteArray(w * h));

// Read tiles from all layers for context
const tilesPtr = mc.add(getField(mcKlass, "Tiles").offset).readPointer();
const tiles = new Uint32Array(tilesPtr.add(ARRAY_HEADER).readByteArray(w * h * 4));

console.log("[+] Map: " + w + "x" + h);
console.log("\n=== ALL non-zero collider values ===");
const valCounts = {};
for (let i = 0; i < colliders.length; i++) {
    const v = colliders[i];
    if (v !== 0) valCounts[v] = (valCounts[v] || 0) + 1;
}
console.log("Value counts: " + JSON.stringify(valCounts));

console.log("\n=== Hill tiles (collider != 0 && != 1 && != 6) ===");
for (let x = 0; x < w; x++) {
    for (let y = 0; y < h; y++) {
        const idx = x * h + y;
        const cv = colliders[idx];
        if (cv === 0 || cv === 1 || cv === 6) continue;
        
        // Get neighbors
        const neighbors = {};
        const dirs = [
            ["up",    x, y-1],
            ["down",  x, y+1],
            ["left",  x-1, y],
            ["right", x+1, y],
        ];
        for (const [dir, nx, ny] of dirs) {
            if (nx >= 0 && nx < w && ny >= 0 && ny < h) {
                const ni = nx * h + ny;
                neighbors[dir] = { col: colliders[ni], tile: tiles[ni] };
            } else {
                neighbors[dir] = { col: -1, tile: -1 };
            }
        }
        
        console.log("  (" + x + "," + y + ") cv=" + cv + " tile=" + tiles[idx] +
            " | up=" + neighbors.up.col +
            " down=" + neighbors.down.col +
            " left=" + neighbors.left.col +
            " right=" + neighbors.right.col);
    }
}

// Also check if there's a Metadata field with hill direction info
console.log("\n=== Checking Metadata field ===");
const metaField = getField(mcKlass, "Metadata");
if (metaField) {
    const metaPtr = mc.add(metaField.offset).readPointer();
    console.log("Metadata ptr: " + metaPtr + " valid=" + isValid(metaPtr));
    if (isValid(metaPtr)) {
        // Dump the Metadata class (mn) fields
        const mnKlass = findClass("mn");
        if (mnKlass) {
            console.log("mn class found, dumping fields:");
            const iter = Memory.alloc(Process.pointerSize);
            iter.writePointer(ptr(0));
            let f;
            while (!(f = il2cpp.class_get_fields(mnKlass, iter)).isNull()) {
                const name = readStr(il2cpp.field_get_name(f));
                const offset = il2cpp.field_get_offset(f);
                console.log("  " + name + " @ 0x" + offset.toString(16));
            }
        }
    }
}

console.log("\n[*] Done.");
send({ type: 'exit' });
})();
