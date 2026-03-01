// Probe MapCreator.fxk to understand tile ID → Vector3 mapping
(function() {
"use strict";

const mod = Process.getModuleByName("GameAssembly.dll");

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
    runtime_invoke:         new NativeFunction(mod.getExportByName("il2cpp_runtime_invoke"),           "pointer", ["pointer", "pointer", "pointer", "pointer"]),
};

function readStr(p) {
    try { return (p && !p.isNull()) ? p.readUtf8String() : null; } catch (_) { return null; }
}
function isValid(p) {
    if (!p || p.isNull()) return false;
    const v = p.toUInt32();
    return v > 0x10000 && v !== 0xffffffff;
}

function findClass(name) {
    const domain   = il2cpp.domain_get();
    const cntBuf   = Memory.alloc(4);
    const asmPtrs  = il2cpp.domain_get_assemblies(domain, cntBuf);
    const asmCount = cntBuf.readU32();
    for (let a = 0; a < asmCount; a++) {
        try {
            const asm = asmPtrs.add(a * Process.pointerSize).readPointer();
            const img = il2cpp.assembly_get_image(asm);
            if (!isValid(img)) continue;
            const cc = il2cpp.image_get_class_count(img);
            for (let c = 0; c < cc; c++) {
                const klass = il2cpp.image_get_class(img, c);
                if (!isValid(klass)) continue;
                if (readStr(il2cpp.class_get_name(klass)) === name) return klass;
            }
        } catch (_) {}
    }
    return null;
}

function getField(klass, fieldName) {
    const iter = Memory.alloc(Process.pointerSize);
    iter.writePointer(ptr(0));
    let f;
    while (!(f = il2cpp.class_get_fields(klass, iter)).isNull()) {
        if (readStr(il2cpp.field_get_name(f)) === fieldName) {
            return {
                field: f,
                offset: il2cpp.field_get_offset(f),
                isStatic: (il2cpp.field_get_flags(f) & 0x10) !== 0,
            };
        }
    }
    return null;
}

function getStaticValue(fieldInfo) {
    const buf = Memory.alloc(Process.pointerSize);
    il2cpp.field_static_get_value(fieldInfo.field, buf);
    return buf.readPointer();
}

function getMethodInfo(klass, name, argc) {
    const mi = il2cpp.class_get_method_from_name(klass, Memory.allocUtf8String(name), argc);
    return isValid(mi) ? mi : null;
}

// Get MapCreator instance
const dsKlass = findClass("DSSock");
const pjmField = getField(dsKlass, "<pjm>k__BackingField");
const ds = getStaticValue(pjmField);
console.log("[+] DSSock @ " + ds);

const mcFieldInfo = getField(dsKlass, "MapCreator");
const mc = ds.add(mcFieldInfo.offset).readPointer();
console.log("[+] MapCreator @ " + mc);

const mcKlass = findClass("MapCreator");

// Read map dimensions
const widthField = getField(mcKlass, "Width");
const heightField = getField(mcKlass, "Height");
const w = mc.add(widthField.offset).readS32();
const h = mc.add(heightField.offset).readS32();
console.log("[+] Map: " + w + "x" + h);

// Read OverLap
const overlapField = getField(mcKlass, "OverLap");
const overlap = overlapField ? mc.add(overlapField.offset).readFloat() : -1;
console.log("[+] OverLap = " + overlap);

// Read qfp
const qfpField = getField(mcKlass, "qfp");
const qfp = qfpField ? mc.add(qfpField.offset).readS32() : -1;
console.log("[+] qfp = " + qfp);

// Read MaxTileSheets
const maxSheetsField = getField(mcKlass, "MaxTileSheets");
const maxSheets = mc.add(maxSheetsField.offset).readS32();
console.log("[+] MaxTileSheets = " + maxSheets);

// Get fxk MethodInfo
const fxkMI = getMethodInfo(mcKlass, "fxk", 3);
if (!fxkMI) {
    console.log("[-] fxk method not found!");
} else {
    console.log("[+] fxk MethodInfo @ " + fxkMI);

    // Read some actual tile IDs from Tiles1
    const tilesField = getField(mcKlass, "Tiles");
    const tilesPtr = mc.add(tilesField.offset).readPointer();
    const tilesData = tilesPtr.add(0x20).readByteArray(w * h * 4);
    const tiles = new Uint32Array(tilesData);

    // Collect some unique tile IDs
    const uniqueIds = new Set();
    for (let i = 0; i < tiles.length && uniqueIds.size < 20; i++) {
        if (tiles[i] !== 0) uniqueIds.add(tiles[i]);
    }

    console.log("\n[*] Probing fxk with " + uniqueIds.size + " tile IDs...");
    console.log("    Format: fxk(tileId, &vec3, c) → bool; vec3 = (x, y, z)");
    console.log("    Trying c=0, c=1, c=64, c=" + qfp + "\n");

    // Buffers for calling fxk
    const aBuf = Memory.alloc(4);     // UInt32 tileId
    const vec3Buf = Memory.alloc(12); // Vector3& (3 floats: x, y, z)
    const cBuf = Memory.alloc(4);     // Int32 c
    const argsBuf = Memory.alloc(3 * Process.pointerSize);
    argsBuf.writePointer(aBuf);
    argsBuf.add(Process.pointerSize).writePointer(vec3Buf);
    argsBuf.add(2 * Process.pointerSize).writePointer(cBuf);
    const excBuf = Memory.alloc(Process.pointerSize);

    function callFxk(tileId, cVal) {
        aBuf.writeU32(tileId);
        vec3Buf.writeFloat(0);
        vec3Buf.add(4).writeFloat(0);
        vec3Buf.add(8).writeFloat(0);
        cBuf.writeS32(cVal);

        excBuf.writePointer(ptr(0));
        const result = il2cpp.runtime_invoke(fxkMI, mc, argsBuf, excBuf);

        if (!excBuf.readPointer().isNull()) {
            return { ok: false, error: "exception" };
        }

        const retBool = (result && !result.isNull()) ? result.add(0x10).readU8() !== 0 : false;
        const vx = vec3Buf.readFloat();
        const vy = vec3Buf.add(4).readFloat();
        const vz = vec3Buf.add(8).readFloat();

        return { ok: true, ret: retBool, x: vx, y: vy, z: vz };
    }

    const cValues = [0, 1, 64, qfp];

    for (const tileId of uniqueIds) {
        const parts = [];
        for (const c of cValues) {
            const r = callFxk(tileId, c);
            if (r.ok) {
                parts.push("c=" + c + " → ret=" + r.ret + " vec=(" + r.x.toFixed(4) + ", " + r.y.toFixed(4) + ", " + r.z.toFixed(4) + ")");
            } else {
                parts.push("c=" + c + " → EXCEPTION");
            }
        }
        console.log("  tileId=" + tileId + ":");
        for (const p of parts) console.log("    " + p);
    }

    // Also try tile ID 1 (should be the very first tile)
    console.log("\n[*] Extra probes:");
    for (const testId of [1, 2, 3, 64, 65, 4096, 4097]) {
        const r = callFxk(testId, 64);
        if (r.ok) {
            console.log("  tileId=" + testId + " c=64 → ret=" + r.ret + " vec=(" + r.x.toFixed(6) + ", " + r.y.toFixed(6) + ", " + r.z.toFixed(6) + ")");
        }
    }
}

console.log("\n[*] Done.");
})();
