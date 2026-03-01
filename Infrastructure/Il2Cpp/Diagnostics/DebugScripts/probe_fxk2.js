// Probe MapCreator.fxk using NativeFunction (direct call) instead of runtime_invoke
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

// Get MapCreator instance
const dsKlass = findClass("DSSock");
const pjmField = getField(dsKlass, "<pjm>k__BackingField");
const ds = getStaticValue(pjmField);
console.log("[+] DSSock @ " + ds);

const mcFieldInfo = getField(dsKlass, "MapCreator");
const mc = ds.add(mcFieldInfo.offset).readPointer();
console.log("[+] MapCreator @ " + mc);

const mcKlass = findClass("MapCreator");

// Read qfp and MaxTileSheets
const qfpField = getField(mcKlass, "qfp");
const qfp = qfpField ? mc.add(qfpField.offset).readS32() : -1;
console.log("[+] qfp = " + qfp);

const maxSheetsField = getField(mcKlass, "MaxTileSheets");
const maxSheets = mc.add(maxSheetsField.offset).readS32();
console.log("[+] MaxTileSheets = " + maxSheets);

const overlapField = getField(mcKlass, "OverLap");
const overlap = overlapField ? mc.add(overlapField.offset).readFloat() : -1;
console.log("[+] OverLap = " + overlap);

// Get fxk method pointer (native function address)
const fxkMI = il2cpp.class_get_method_from_name(mcKlass, Memory.allocUtf8String("fxk"), 3);
if (!isValid(fxkMI)) {
    console.log("[-] fxk MethodInfo not found!");
} else {
    const fxkPtr = fxkMI.readPointer(); // MethodInfo->methodPointer at offset 0
    console.log("[+] fxk MethodInfo @ " + fxkMI);
    console.log("[+] fxk native ptr  @ " + fxkPtr);

    // Disassemble the first few instructions of fxk
    console.log("\n=== fxk disassembly (first 30 insns) ===");
    let addr = fxkPtr;
    for (let i = 0; i < 30; i++) {
        try {
            const insn = Instruction.parse(addr);
            console.log("  " + addr + ": " + insn.toString());
            addr = insn.next;
        } catch(e) {
            console.log("  " + addr + ": <parse error: " + e.message + ">");
            break;
        }
    }

    // IL2CPP native calling convention for instance methods:
    // bool fxk(MapCreator* this, uint32_t a, Vector3* b, int32_t c, MethodInfo* methodInfo)
    //
    // On x64 Windows: rcx=this, rdx=a, r8=b, r9=c, [stack]=methodInfo
    // Return: al (bool)
    //
    // Let's try calling it as a NativeFunction
    const fxkNative = new NativeFunction(fxkPtr, "bool", ["pointer", "uint32", "pointer", "int32", "pointer"]);

    const vec3Buf = Memory.alloc(16); // 12 bytes + padding

    function callFxk(tileId, cVal) {
        vec3Buf.writeFloat(0);
        vec3Buf.add(4).writeFloat(0);
        vec3Buf.add(8).writeFloat(0);

        try {
            const ret = fxkNative(mc, tileId, vec3Buf, cVal, fxkMI);
            const vx = vec3Buf.readFloat();
            const vy = vec3Buf.add(4).readFloat();
            const vz = vec3Buf.add(8).readFloat();
            return { ok: true, ret, x: vx, y: vy, z: vz };
        } catch(e) {
            return { ok: false, error: e.message };
        }
    }

    // Read some tile IDs from Tiles
    const widthField = getField(mcKlass, "Width");
    const heightField = getField(mcKlass, "Height");
    const w = mc.add(widthField.offset).readS32();
    const h = mc.add(heightField.offset).readS32();

    const tilesField = getField(mcKlass, "Tiles");
    const tilesPtr = mc.add(tilesField.offset).readPointer();
    const tilesData = tilesPtr.add(0x20).readByteArray(w * h * 4);
    const tiles = new Uint32Array(tilesData);

    // Collect unique tile IDs
    const uniqueIds = new Set();
    for (let i = 0; i < tiles.length; i++) {
        if (tiles[i] !== 0) uniqueIds.add(tiles[i]);
    }
    console.log("\n[+] Unique tile IDs in layer 1: " + uniqueIds.size);
    console.log("[+] Sample tile IDs: " + Array.from(uniqueIds).slice(0, 10).join(", "));

    console.log("\n[*] Probing fxk as NativeFunction...");

    // Try with various c values  
    const cValues = [0, 1, 64, qfp, maxSheets];
    
    const sampleIds = Array.from(uniqueIds).slice(0, 8);
    // Also try simple IDs
    sampleIds.push(1, 2, 3);

    for (const tileId of sampleIds) {
        const parts = [];
        for (const c of cValues) {
            const r = callFxk(tileId, c);
            if (r.ok) {
                parts.push("c=" + c + " → ret=" + r.ret + " vec=(" + r.x.toFixed(6) + ", " + r.y.toFixed(6) + ", " + r.z.toFixed(6) + ")");
            } else {
                parts.push("c=" + c + " → ERROR: " + r.error);
            }
        }
        console.log("  tileId=" + tileId + ":");
        for (const p of parts) console.log("    " + p);
    }

    // Also try without the MethodInfo parameter (some IL2CPP methods don't use it)
    console.log("\n[*] Trying without MethodInfo parameter (4 args)...");
    const fxkNative4 = new NativeFunction(fxkPtr, "bool", ["pointer", "uint32", "pointer", "int32"]);

    for (const tileId of [sampleIds[0], 1]) {
        vec3Buf.writeFloat(0);
        vec3Buf.add(4).writeFloat(0);
        vec3Buf.add(8).writeFloat(0);
        try {
            const ret = fxkNative4(mc, tileId, vec3Buf, 64);
            const vx = vec3Buf.readFloat();
            const vy = vec3Buf.add(4).readFloat();
            const vz = vec3Buf.add(8).readFloat();
            console.log("  tileId=" + tileId + " c=64 → ret=" + ret + " vec=(" + vx.toFixed(6) + ", " + vy.toFixed(6) + ", " + vz.toFixed(6) + ")");
        } catch(e) {
            console.log("  tileId=" + tileId + " → ERROR: " + e.message);
        }
    }
}

console.log("\n[*] Done.");
})();
