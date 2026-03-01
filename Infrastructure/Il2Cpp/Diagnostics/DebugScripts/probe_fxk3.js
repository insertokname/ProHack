// Deep disassembly of fxk branches + inspect mesh UV data for tile decoding
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
    const domain = il2cpp.domain_get();
    const cntBuf = Memory.alloc(4);
    const asmPtrs = il2cpp.domain_get_assemblies(domain, cntBuf);
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
            return { field: f, offset: il2cpp.field_get_offset(f) };
        }
    }
    return null;
}

function getStaticValue(fi) {
    const buf = Memory.alloc(Process.pointerSize);
    il2cpp.field_static_get_value(fi.field, buf);
    return buf.readPointer();
}

// Disassemble a range of code
function disasm(startAddr, count) {
    let addr = startAddr;
    const lines = [];
    for (let i = 0; i < count; i++) {
        try {
            const insn = Instruction.parse(addr);
            lines.push("  " + addr + ": " + insn.toString());
            addr = insn.next;
        } catch(e) {
            lines.push("  " + addr + ": <parse error>");
            break;
        }
    }
    return lines;
}

// Get fxk code pointer
const mcKlass = findClass("MapCreator");
const fxkMI = il2cpp.class_get_method_from_name(mcKlass, Memory.allocUtf8String("fxk"), 3);
const fxkPtr = fxkMI.readPointer();
console.log("[+] fxk @ " + fxkPtr);

// Disassemble main entry (0-30)
console.log("\n=== fxk entry (first 30 insns) ===");
disasm(fxkPtr, 30).forEach(l => console.log(l));

// The branch for tileId <= 0x125f9 goes to fxkPtr + offset
// 0x1802fa3ec from output, fxkPtr was 0x1802f9e40
// offset = 0x1802fa3ec - 0x1802f9e40 = 0x5AC
const branchTarget1 = fxkPtr.add(0x5AC);
console.log("\n=== Branch target for tileId <= 0x125f9 (at fxk+0x5AC) ===");
disasm(branchTarget1, 60).forEach(l => console.log(l));

// Also look at what happens after the big switch falls through (default/common case)
// The function seems to have a common return path. Let me look for it.
// Let me also look around the area just after the first few comparisons
const area2 = fxkPtr.add(0x9F); // after the jbe to 0x5AC
console.log("\n=== fxk after first branch check (fxk+0x9F) ===");
disasm(fxkPtr.add(0x9F), 40).forEach(l => console.log(l));

// Also check the biu method (mesh builder) to understand tile→UV mapping
const biuMI = il2cpp.class_get_method_from_name(mcKlass, Memory.allocUtf8String("biu"), -1);
if (isValid(biuMI)) {
    const biuPtr = biuMI.readPointer();
    console.log("\n=== biu @ " + biuPtr + " (first 40 insns) ===");
    disasm(biuPtr, 40).forEach(l => console.log(l));
}

// Check: Are there other methods that might decode tiles?
// List all MapCreator methods
console.log("\n=== MapCreator methods ===");
const methodNames = ["fxk", "biu", "Update", "Start", "Awake", "BuildMesh", "CreateMap", "SetTile", "GetTile", "InitMap"];
for (const name of methodNames) {
    for (let argc = -1; argc <= 8; argc++) {
        const mi = il2cpp.class_get_method_from_name(mcKlass, Memory.allocUtf8String(name), argc);
        if (isValid(mi)) {
            const fp = mi.readPointer();
            console.log("  " + name + "(" + (argc === -1 ? "?" : argc) + ") → " + fp);
        }
    }
}

// Now let's check the actual Tiles array bounds and sample data
const dsKlass = findClass("DSSock");
const pjmField = getField(dsKlass, "<pjm>k__BackingField");
const ds = getStaticValue(pjmField);
const mcFieldInfo = getField(dsKlass, "MapCreator");
const mc = ds.add(mcFieldInfo.offset).readPointer();

const widthField = getField(mcKlass, "Width");
const heightField = getField(mcKlass, "Height");
const w = mc.add(widthField.offset).readS32();
const h = mc.add(heightField.offset).readS32();
console.log("\n[+] Map: " + w + "x" + h);

// Read the Tiles array bounds info
const tilesField = getField(mcKlass, "Tiles");
const tilesPtr = mc.add(tilesField.offset).readPointer();
console.log("[+] Tiles array @ " + tilesPtr);

// Read bounds pointer
const boundsPtr = tilesPtr.add(0x10).readPointer();
console.log("[+] Bounds ptr @ " + boundsPtr);
if (isValid(boundsPtr)) {
    // Il2CppArrayBounds: each dimension has { length: size_t, lower_bound: int32 }
    // On x64: length is 8 bytes, lower_bound is 4 bytes, total 16 bytes per dimension? Or 12?
    // Actually IL2CPP uses: struct { il2cpp_array_size_t length; il2cpp_array_lower_bound_t lower_bound; }
    // On x64 with 64-bit size: 8+4=12 bytes per dimension, but likely padded to 16
    const d0len = Number(boundsPtr.readU64());
    const d0lb = boundsPtr.add(8).readS32();
    const d1len = Number(boundsPtr.add(16).readU64());
    const d1lb = boundsPtr.add(24).readS32();
    console.log("[+] Dim0: length=" + d0len + " lower_bound=" + d0lb);
    console.log("[+] Dim1: length=" + d1len + " lower_bound=" + d1lb);
} else {
    console.log("[!] Bounds pointer is null (1D array?)");
}

// max_length (total elements)
const maxLen = Number(tilesPtr.add(0x18).readU64());
console.log("[+] max_length = " + maxLen);

// Read ALL unique tile IDs from all layers
const layers = ["Tiles", "Tiles2", "Tiles3", "Tiles4"];
for (const layerName of layers) {
    const lf = getField(mcKlass, layerName);
    if (!lf) { console.log("[!] " + layerName + " not found"); continue; }
    const lp = mc.add(lf.offset).readPointer();
    if (!isValid(lp)) { console.log("[!] " + layerName + " is null"); continue; }
    const len = Number(lp.add(0x18).readU64());
    const data = new Uint32Array(lp.add(0x20).readByteArray(len * 4));
    
    const unique = new Set();
    for (let i = 0; i < data.length; i++) {
        if (data[i] !== 0) unique.add(data[i]);
    }
    const sorted = Array.from(unique).sort((a, b) => a - b);
    console.log("[+] " + layerName + ": " + unique.size + " unique IDs, range [" + 
        sorted[0] + " .. " + sorted[sorted.length-1] + "]");
    console.log("    IDs: " + sorted.join(", "));
}

console.log("\n[*] Done.");
})();
