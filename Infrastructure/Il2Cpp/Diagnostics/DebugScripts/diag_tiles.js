// Diagnostic: Understand tile layout, dump a sheet PNG, and check pixel values
(function() {
"use strict";

const OUTPUT_DIR = "C:\\Users\\fekete\\Documents\\tests\\PRO-hack\\csharp2\\ProHack\\Infrastructure\\Il2Cpp\\Diagnostics\\DebugScripts";
const TILE_PX = 16;
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
    object_new:             new NativeFunction(mod.getExportByName("il2cpp_object_new"),               "pointer", ["pointer"]),
};

function readStr(p) { try { return (p && !p.isNull()) ? p.readUtf8String() : null; } catch (_) { return null; } }
function isValid(p) { if (!p || p.isNull()) return false; const v = p.toUInt32(); return v > 0x10000 && v !== 0xffffffff; }
function readIl2CppString(p) {
    try { if (!p || p.isNull()) return null; const len = p.add(0x10).readS32(); if (len <= 0 || len > 4096) return null; return p.add(0x14).readUtf16String(len); } catch (_) { return null; }
}
function findClass(name) {
    const domain = il2cpp.domain_get(); const cntBuf = Memory.alloc(4);
    const asmPtrs = il2cpp.domain_get_assemblies(domain, cntBuf); const asmCount = cntBuf.readU32();
    for (let a = 0; a < asmCount; a++) {
        try { const asm = asmPtrs.add(a * Process.pointerSize).readPointer(); const img = il2cpp.assembly_get_image(asm);
        if (!isValid(img)) continue; const cc = il2cpp.image_get_class_count(img);
        for (let c = 0; c < cc; c++) { const klass = il2cpp.image_get_class(img, c); if (!isValid(klass)) continue; if (readStr(il2cpp.class_get_name(klass)) === name) return klass; }
        } catch (_) {} } return null;
}
function getField(klass, fieldName) {
    const iter = Memory.alloc(Process.pointerSize); iter.writePointer(ptr(0)); let f;
    while (!(f = il2cpp.class_get_fields(klass, iter)).isNull()) {
        if (readStr(il2cpp.field_get_name(f)) === fieldName) return { field: f, offset: il2cpp.field_get_offset(f), isStatic: (il2cpp.field_get_flags(f) & 0x10) !== 0 };
    } return null;
}
function getStaticValue(fi) { const buf = Memory.alloc(Process.pointerSize); il2cpp.field_static_get_value(fi.field, buf); return buf.readPointer(); }
function getMethodInfo(klass, name, argc) { const mi = il2cpp.class_get_method_from_name(klass, Memory.allocUtf8String(name), argc); return isValid(mi) ? mi : null; }

// ---- Get map data ----
const dsKlass = findClass("DSSock");
const ds = getStaticValue(getField(dsKlass, "<pjm>k__BackingField"));
const mcF = getField(dsKlass, "MapCreator");
const mc = ds.add(mcF.offset).readPointer();
const mcKlass = findClass("MapCreator");

const w = mc.add(getField(mcKlass, "Width").offset).readS32();
const h = mc.add(getField(mcKlass, "Height").offset).readS32();
const mapNameStr = readIl2CppString(mc.add(getField(mcKlass, "MapName").offset).readPointer()) || "?";
const maxSheets = mc.add(getField(mcKlass, "MaxTileSheets").offset).readS32();
const qfp = mc.add(getField(mcKlass, "qfp").offset).readS32();
const overlap = mc.add(getField(mcKlass, "OverLap").offset).readFloat();

console.log("[+] Map: " + mapNameStr + " (" + w + "x" + h + ") MaxSheets=" + maxSheets + " qfp=" + qfp + " OverLap=" + overlap);

// Read tile layers
const ARRAY_HEADER = 0x20;
function readU32Array2D(arrPtr, d0, d1) {
    if (!isValid(arrPtr)) return null;
    return new Uint32Array(arrPtr.add(ARRAY_HEADER).readByteArray(d0 * d1 * 4));
}
function readU8Array2D(arrPtr, d0, d1) {
    if (!isValid(arrPtr)) return null;
    return new Uint8Array(arrPtr.add(ARRAY_HEADER).readByteArray(d0 * d1));
}

const tiles1 = readU32Array2D(mc.add(getField(mcKlass, "Tiles").offset).readPointer(), w, h);
const tiles2 = readU32Array2D(mc.add(getField(mcKlass, "Tiles2").offset).readPointer(), w, h);
const tiles3 = readU32Array2D(mc.add(getField(mcKlass, "Tiles3").offset).readPointer(), w, h);
const tiles4 = readU32Array2D(mc.add(getField(mcKlass, "Tiles4").offset).readPointer(), w, h);
const colliders = readU8Array2D(mc.add(getField(mcKlass, "Colliders").offset).readPointer(), w, h);

// --- Analyze layer coverage ---
const layers = [tiles1, tiles2, tiles3, tiles4];
const layerNames = ["Tiles1", "Tiles2", "Tiles3", "Tiles4"];
for (let li = 0; li < 4; li++) {
    const L = layers[li];
    if (!L) { console.log("[" + layerNames[li] + "] null"); continue; }
    let zero = 0, nonZero = 0;
    const ids = new Set();
    for (let i = 0; i < L.length; i++) { if (L[i] === 0) zero++; else { nonZero++; ids.add(L[i]); } }
    console.log("[" + layerNames[li] + "] " + nonZero + " filled / " + zero + " empty (" + ids.size + " unique IDs)");

    // Show min/max tile IDs
    const sorted = Array.from(ids).sort((a,b) => a-b);
    if (sorted.length > 0) {
        console.log("  min=" + sorted[0] + " max=" + sorted[sorted.length-1]);
        console.log("  first 20: " + sorted.slice(0, 20).join(", "));
    }
}

// --- Show a grid slice (center of map) ---
console.log("\n=== Tile grid at rows 10-14, cols 30-40 ===");
for (let ty = 10; ty < Math.min(15, h); ty++) {
    const row = [];
    for (let tx = 30; tx < Math.min(41, w); tx++) {
        const idx = tx * h + ty;
        const ids = [];
        for (let li = 0; li < 4; li++) {
            const v = layers[li] ? layers[li][idx] : 0;
            if (v !== 0) ids.push("L" + (li+1) + "=" + v);
        }
        const cv = colliders ? colliders[idx] : 0;
        if (cv !== 0) ids.push("C=" + cv);
        row.push(ids.length > 0 ? ids.join("|") : "---");
    }
    console.log("  y=" + ty + ": " + row.join("  "));
}

// --- Check the mesh data arrays to understand UV mapping ---
// newUV1 is List<Vector2>[MaxTileSheets] — the UV coordinates tell us the mapping
console.log("\n=== Checking mesh UV data ===");
const uvFieldNames = ["newUV1", "newUV2", "newUV3", "newUV4"];
const vertFieldNames = ["newVertices1", "newVertices2", "newVertices3", "newVertices4"];

for (let li = 0; li < 4; li++) {
    const uvField = getField(mcKlass, uvFieldNames[li]);
    if (!uvField) { console.log("[" + uvFieldNames[li] + "] field not found"); continue; }
    const uvArr = mc.add(uvField.offset).readPointer();
    if (!isValid(uvArr)) { console.log("[" + uvFieldNames[li] + "] null"); continue; }

    // This is a C# array of List<Vector2>, indexed by sheet
    const arrLen = Number(uvArr.add(0x18).readU64());
    console.log("[" + uvFieldNames[li] + "] array length = " + arrLen);

    // For each sheet, check if the List has items
    let totalUVs = 0;
    let sheetsWithUVs = [];
    for (let s = 0; s < Math.min(arrLen, maxSheets); s++) {
        const listPtr = uvArr.add(ARRAY_HEADER + s * Process.pointerSize).readPointer();
        if (!isValid(listPtr)) continue;
        // List<T> has _size at some offset. In .NET/IL2CPP: _items at 0x10, _size at 0x18
        const listSize = listPtr.add(0x18).readS32();
        if (listSize > 0) {
            sheetsWithUVs.push(s + "(" + listSize + ")");
            totalUVs += listSize;
        }
    }
    console.log("  Total UVs: " + totalUVs + " | Sheets with data: " + sheetsWithUVs.join(", "));

    // Read actual UV values from first sheet with data to understand the coordinate system
    if (sheetsWithUVs.length > 0) {
        const firstSheet = parseInt(sheetsWithUVs[0]);
        const listPtr = uvArr.add(ARRAY_HEADER + firstSheet * Process.pointerSize).readPointer();
        const listSize = listPtr.add(0x18).readS32();
        const itemsArr = listPtr.add(0x10).readPointer(); // _items is T[]

        // Read first few UV pairs (each Vector2 = 2 floats = 8 bytes)
        console.log("  First UVs from sheet " + firstSheet + " (each 4 UVs = 1 quad/tile):");
        const numToRead = Math.min(listSize, 24); // 6 tiles worth
        for (let i = 0; i < numToRead; i += 4) {
            const uvs = [];
            for (let j = 0; j < 4 && (i+j) < numToRead; j++) {
                const base = itemsArr.add(ARRAY_HEADER + (i+j) * 8);
                const u = base.readFloat();
                const v = base.add(4).readFloat();
                uvs.push("(" + u.toFixed(4) + "," + v.toFixed(4) + ")");
            }
            console.log("    tile " + (i/4) + ": " + uvs.join(" "));
        }
    }
}

// Also read corresponding vertices to understand tile positions
console.log("\n=== First few vertices from layer 1 ===");
const vertField = getField(mcKlass, "newVertices1");
if (vertField) {
    const vertArr = mc.add(vertField.offset).readPointer();
    if (isValid(vertArr)) {
        const arrLen = Number(vertArr.add(0x18).readU64());
        for (let s = 0; s < Math.min(arrLen, maxSheets); s++) {
            const listPtr = vertArr.add(ARRAY_HEADER + s * Process.pointerSize).readPointer();
            if (!isValid(listPtr)) continue;
            const listSize = listPtr.add(0x18).readS32();
            if (listSize === 0) continue;
            const itemsArr = listPtr.add(0x10).readPointer();

            console.log("  Sheet " + s + " (" + listSize + " vertices, " + (listSize/4) + " tiles):");
            const numToRead = Math.min(listSize, 8);
            for (let i = 0; i < numToRead; i += 4) {
                const verts = [];
                for (let j = 0; j < 4 && (i+j) < numToRead; j++) {
                    const base = itemsArr.add(ARRAY_HEADER + (i+j) * 12); // Vector3 = 12 bytes
                    const x = base.readFloat();
                    const y = base.add(4).readFloat();
                    const z = base.add(8).readFloat();
                    verts.push("(" + x.toFixed(2) + "," + y.toFixed(2) + "," + z.toFixed(2) + ")");
                }
                console.log("    quad " + (i/4) + ": " + verts.join(" "));
            }
            // Only show first 2 sheets with data
            break;
        }
    }
}

// --- Decode sample tiles and show which sheet/position they'd go to ---
console.log("\n=== Sample tile ID decoding ===");
const sampleIds = [7, 2361, 35525, 35526, 35533, 35534, 35541, 37600, 39438];
for (const tid of sampleIds) {
    const linear = tid - 1;
    const tilesPerRow = 64;
    const tilesPerCol = 64;
    const tilesPerSheet = tilesPerRow * tilesPerCol; // 4096
    const sheet = Math.floor(linear / tilesPerSheet);
    const local = linear % tilesPerSheet;
    const col = local % tilesPerRow;
    const row = Math.floor(local / tilesPerRow);

    // Also compute what UV the game would use
    // UV = (col * 16 / 1024, row * 16 / 1024) → (col/64, row/64)
    const u = col / tilesPerRow;
    const v = row / tilesPerCol;

    console.log("  tileId=" + tid + " → sheet=" + sheet + " col=" + col + " row=" + row +
        " pixel=(" + (col*16) + "," + (row*16) + ")" +
        " UV=(" + u.toFixed(4) + "," + v.toFixed(4) + ")");
}

console.log("\n[*] Done.");
send({ type: 'exit' });
})();
