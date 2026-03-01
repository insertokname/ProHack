// !!!VIBECODED BULLSHIT AHEAD!!!

(function () {
"use strict";

/**
 * read_map_v2.js
 *
 * Reads the currently loaded map and renders it using the ACTUAL in-game tile
 * sprites extracted from the tile-sheet textures in memory.
 *
 * How it works:
 *   1. Reads map data from DSSock → MapCreator (Width, Height, Tiles 1-4, etc.)
 *   2. Reads the tile-sheet Material[] (MapCreator.qfn) and for each material
 *      gets its mainTexture (Texture2D).
 *   3. Determines the tile-ID → (sheet, column, row) mapping.
 *   4. For every unique tile ID actually used on the map, reads the 16×16 pixel
 *      region from the correct texture via Texture2D.GetPixelImpl_Injected.
 *   5. Composites all 4 tile layers (with alpha blending) plus collision/link
hi *      overlays (with directional arrows on ledges/hills) and a player marker,
 *      then writes a full-colour PNG.
 *
 * Uses member names from the DSSock / Assembly-CSharp dumps wherever possible.
 * Offsets are only used for IL2CPP internal structures (object header, array layout).
 *
 * Output: <MapName>_v2.png in the script directory.
 */

// ── Configuration ─────────────────────────────────────────────────────────────
const OUTPUT_DIR  = "C:/Users/fekete/Documents/tests/PRO-hack/csharp2/ProHack/Infrastructure/Il2Cpp/Diagnostics/DebugScripts/read_map_output";
const TILE_PX     = 32;   // pixels per tile in the texture (confirmed via mesh UV data)
const OUT_TILE_PX = 16;   // pixels per tile in the output image (downscale 2×)
const SAVE_SHEETS = false; // also save each tile-sheet texture as a separate PNG

// ── IL2CPP API bindings ───────────────────────────────────────────────────────
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
    string_new:             new NativeFunction(mod.getExportByName("il2cpp_string_new"),               "pointer", ["pointer"]),
    object_new:             new NativeFunction(mod.getExportByName("il2cpp_object_new"),               "pointer", ["pointer"]),
};

const FIELD_ATTR_STATIC = 0x10;

// ── Helpers ───────────────────────────────────────────────────────────────────
function readStr(p) {
    try { return (p && !p.isNull()) ? p.readUtf8String() : null; } catch (_) { return null; }
}

function isValid(p) {
    if (!p || p.isNull()) return false;
    const v = p.toUInt32();
    return v > 0x10000 && v !== 0xffffffff;
}

function readIl2CppString(p) {
    try {
        if (!p || p.isNull()) return null;
        const len = p.add(0x10).readS32();
        if (len <= 0 || len > 4096) return "(invalid string)";
        return p.add(0x14).readUtf16String(len);
    } catch (_) { return null; }
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
                field:    f,
                offset:   il2cpp.field_get_offset(f),
                isStatic: (il2cpp.field_get_flags(f) & FIELD_ATTR_STATIC) !== 0,
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

function readInstancePtr(instance, fieldInfo) {
    return instance.add(fieldInfo.offset).readPointer();
}

function readInstanceS32(instance, fieldInfo) {
    return instance.add(fieldInfo.offset).readS32();
}

/**
 * Find a method on a class by name and argument count, return the raw function pointer.
 */
function getMethodPtr(klass, name, argc) {
    const mi = il2cpp.class_get_method_from_name(klass, Memory.allocUtf8String(name), argc);
    if (!isValid(mi)) return null;
    return mi.readPointer(); // MethodInfo->methodPointer is at offset 0
}

/**
 * Find a MethodInfo pointer (for use with il2cpp_runtime_invoke).
 */
function getMethodInfo(klass, name, argc) {
    const mi = il2cpp.class_get_method_from_name(klass, Memory.allocUtf8String(name), argc);
    return isValid(mi) ? mi : null;
}

// ── IL2CPP array helpers ──────────────────────────────────────────────────────
// Il2CppArray layout (x64):
//   +0x00 klass*      (8)
//   +0x08 monitor*    (8)
//   +0x10 bounds*     (8)   — non-null for multi-dim
//   +0x18 max_length  (8)   — total element count
//   +0x20 data[]             — element data

const ARRAY_HEADER = 0x20;

function readArrayLength(arrPtr) {
    if (!isValid(arrPtr)) return 0;
    // max_length is size_t (8 bytes on x64)
    return Number(arrPtr.add(0x18).readU64());
}

function readUint32Array2D(arrayPtr, dim0, dim1) {
    if (!isValid(arrayPtr)) return null;
    const total = dim0 * dim1;
    const raw   = arrayPtr.add(ARRAY_HEADER).readByteArray(total * 4);
    return { dim0, dim1, data: new Uint32Array(raw) };
}

function readByteArray2D(arrayPtr, dim0, dim1) {
    if (!isValid(arrayPtr)) return null;
    const total = dim0 * dim1;
    const raw   = arrayPtr.add(ARRAY_HEADER).readByteArray(total);
    return { dim0, dim1, data: new Uint8Array(raw) };
}

// ── Map data reader ───────────────────────────────────────────────────────────
function readMapData() {
    const dsKlass = findClass("DSSock");
    if (!dsKlass) throw new Error("DSSock class not found");

    const pjmField = getField(dsKlass, "<pjm>k__BackingField");
    if (!pjmField) throw new Error("DSSock.<pjm>k__BackingField not found");

    const ds = getStaticValue(pjmField);
    if (!isValid(ds)) throw new Error("DSSock instance is null (not logged in?)");
    console.log("[+] DSSock instance @ " + ds);

    // Player position
    const tpField = getField(dsKlass, "TargetPos");
    if (!tpField) throw new Error("DSSock.TargetPos not found");
    const playerX = ds.add(tpField.offset).readFloat();
    const playerY = ds.add(tpField.offset + 4).readFloat();

    // MapCreator
    const mcFieldInfo = getField(dsKlass, "MapCreator");
    if (!mcFieldInfo) throw new Error("DSSock.MapCreator field not found");
    const mc = readInstancePtr(ds, mcFieldInfo);
    if (!isValid(mc)) throw new Error("MapCreator is null (no map loaded?)");
    console.log("[+] MapCreator @ " + mc);

    const mcKlass = findClass("MapCreator");
    if (!mcKlass) throw new Error("MapCreator class not found");

    // Scalar fields
    const widthField  = getField(mcKlass, "Width");
    const heightField = getField(mcKlass, "Height");
    if (!widthField || !heightField) throw new Error("Width/Height fields not found");
    const width  = readInstanceS32(mc, widthField);
    const height = readInstanceS32(mc, heightField);
    console.log("[+] Map size: " + width + " x " + height);

    const mapNameField = getField(mcKlass, "MapName");
    let mapName = "(unknown)";
    if (mapNameField) {
        const strPtr = readInstancePtr(mc, mapNameField);
        mapName = readIl2CppString(strPtr) || "(unreadable)";
    }
    console.log("[+] Map name: " + mapName);

    const outsideField = getField(mcKlass, "Outside");
    const outside = outsideField ? mc.add(outsideField.offset).readU8() !== 0 : false;

    const regionField = getField(mcKlass, "Region");
    const region = regionField ? readInstanceS32(mc, regionField) : -1;

    // Tile layers
    const tilesField  = getField(mcKlass, "Tiles");
    const tiles2Field = getField(mcKlass, "Tiles2");
    const tiles3Field = getField(mcKlass, "Tiles3");
    const tiles4Field = getField(mcKlass, "Tiles4");

    const tiles  = readUint32Array2D(tilesField  ? readInstancePtr(mc, tilesField)  : ptr(0), width, height);
    const tiles2 = readUint32Array2D(tiles2Field ? readInstancePtr(mc, tiles2Field) : ptr(0), width, height);
    const tiles3 = readUint32Array2D(tiles3Field ? readInstancePtr(mc, tiles3Field) : ptr(0), width, height);
    const tiles4 = readUint32Array2D(tiles4Field ? readInstancePtr(mc, tiles4Field) : ptr(0), width, height);

    console.log("[+] Tiles: L1=" + !!tiles + " L2=" + !!tiles2 + " L3=" + !!tiles3 + " L4=" + !!tiles4);

    // Colliders & Links
    const collidersField = getField(mcKlass, "Colliders");
    const linksField     = getField(mcKlass, "Links");
    const colliders = readByteArray2D(collidersField ? readInstancePtr(mc, collidersField) : ptr(0), width, height);
    const links     = readByteArray2D(linksField     ? readInstancePtr(mc, linksField)     : ptr(0), width, height);

    // MaxTileSheets & Material[]
    const maxSheetsField = getField(mcKlass, "MaxTileSheets");
    const maxSheets = maxSheetsField ? readInstanceS32(mc, maxSheetsField) : 0;
    console.log("[+] MaxTileSheets: " + maxSheets);

    // qfp = total tiles per sheet (e.g. 5632 = 88*64 means 64 tiles/row in each 1024px sheet)
    const qfpField = getField(mcKlass, "qfp");
    const qfp = qfpField ? readInstanceS32(mc, qfpField) : 0;
    console.log("[+] qfp (tiles per mega-row): " + qfp);

    // qfn = Material[] (per-sheet materials)
    const qfnField = getField(mcKlass, "qfn");
    const qfnPtr   = qfnField ? readInstancePtr(mc, qfnField) : ptr(0);

    // TileMaterial (base material, used for sheet 0 if qfn is missing)
    const tileMaterialField = getField(mcKlass, "TileMaterial");
    const tileMaterialPtr   = tileMaterialField ? readInstancePtr(mc, tileMaterialField) : ptr(0);

    return {
        mc, mcKlass,
        width, height, mapName, outside, region,
        tiles, tiles2, tiles3, tiles4,
        colliders, links,
        playerX, playerY,
        maxSheets, qfp, qfnPtr, tileMaterialPtr,
    };
}

// ── Texture reading infrastructure ────────────────────────────────────────────

/**
 * Resolve all the Unity methods we need for texture reading.
 * Returns an object with callable NativeFunctions.
 */
function resolveTextureMethods() {
    // Material class → get_mainTexture()
    const materialKlass = findClass("Material");
    if (!materialKlass) throw new Error("Material class not found");

    const getMainTexMI = getMethodInfo(materialKlass, "get_mainTexture", 0);
    if (!getMainTexMI) throw new Error("Material.get_mainTexture method not found");

    // Texture2D class → GetPixel, get_width, get_height, get_isReadable
    const tex2dKlass = findClass("Texture2D");
    if (!tex2dKlass) throw new Error("Texture2D class not found");

    const getPixelMI     = getMethodInfo(tex2dKlass, "GetPixel", 2);
    const getIsReadMI    = getMethodInfo(tex2dKlass, "get_isReadable", 0);
    const getWidthMI     = getMethodInfo(tex2dKlass, "get_width", 0);
    const getHeightMI    = getMethodInfo(tex2dKlass, "get_height", 0);

    // Try to get get_width/get_height via Texture base if not found on Texture2D
    let widthMI  = getWidthMI;
    let heightMI = getHeightMI;
    if (!widthMI || !heightMI) {
        const texKlass = findClass("Texture");
        if (texKlass) {
            if (!widthMI)  widthMI  = getMethodInfo(texKlass, "get_width", 0);
            if (!heightMI) heightMI = getMethodInfo(texKlass, "get_height", 0);
        }
    }
    if (!widthMI)  throw new Error("Texture.get_width not found");
    if (!heightMI) throw new Error("Texture.get_height not found");

    // ImageConversion → EncodeToPNG (static, 1 arg)
    const imgConvKlass = findClass("ImageConversion");
    const encodePngMI  = imgConvKlass ? getMethodInfo(imgConvKlass, "EncodeToPNG", 1) : null;

    return {
        getMainTexMI,
        getPixelMI,
        getIsReadMI,
        widthMI,
        heightMI,
        encodePngMI,
    };
}

/**
 * Call a method that returns a managed pointer (e.g. get_mainTexture).
 */
function invokeReturnPtr(methodInfo, instance) {
    const exc = Memory.alloc(Process.pointerSize);
    exc.writePointer(ptr(0));
    const result = il2cpp.runtime_invoke(methodInfo, instance, ptr(0), exc);
    if (!exc.readPointer().isNull()) {
        console.log("[!] Exception during invoke");
        return null;
    }
    return result;
}

/**
 * Call a method that returns an int (boxed).
 * IL2CPP runtime_invoke returns boxed value types; int data starts at offset 0x10.
 */
function invokeReturnInt(methodInfo, instance) {
    const exc = Memory.alloc(Process.pointerSize);
    exc.writePointer(ptr(0));
    const result = il2cpp.runtime_invoke(methodInfo, instance, ptr(0), exc);
    if (!exc.readPointer().isNull() || !isValid(result)) return -1;
    return result.add(0x10).readS32();
}

/**
 * Call a method that returns a bool (boxed).
 */
function invokeReturnBool(methodInfo, instance) {
    const exc = Memory.alloc(Process.pointerSize);
    exc.writePointer(ptr(0));
    const result = il2cpp.runtime_invoke(methodInfo, instance, ptr(0), exc);
    if (!exc.readPointer().isNull() || !isValid(result)) return false;
    return result.add(0x10).readU8() !== 0;
}

/**
 * Call GetPixel(x, y) on a Texture2D and return [r, g, b, a] in 0-255 range.
 */
function invokeGetPixel(methodInfo, texInstance, x, y) {
    const args = Memory.alloc(2 * Process.pointerSize);
    const xBuf = Memory.alloc(4);
    const yBuf = Memory.alloc(4);
    xBuf.writeS32(x);
    yBuf.writeS32(y);
    args.writePointer(xBuf);
    args.add(Process.pointerSize).writePointer(yBuf);

    const exc = Memory.alloc(Process.pointerSize);
    exc.writePointer(ptr(0));
    const result = il2cpp.runtime_invoke(methodInfo, texInstance, args, exc);

    if (!exc.readPointer().isNull() || !isValid(result)) {
        return [0, 0, 0, 0]; // error → transparent black
    }

    // Boxed Color struct: 4 floats at offset 0x10 (R, G, B, A in [0,1])
    const r = result.add(0x10).readFloat();
    const g = result.add(0x14).readFloat();
    const b = result.add(0x18).readFloat();
    const a = result.add(0x1C).readFloat();

    return [
        Math.round(Math.max(0, Math.min(1, r)) * 255),
        Math.round(Math.max(0, Math.min(1, g)) * 255),
        Math.round(Math.max(0, Math.min(1, b)) * 255),
        Math.round(Math.max(0, Math.min(1, a)) * 255),
    ];
}

/**
 * Call EncodeToPNG(Texture2D) — static method, returns byte[].
 */
function invokeEncodeToPNG(methodInfo, texInstance) {
    const args = Memory.alloc(Process.pointerSize);
    args.writePointer(texInstance);

    const exc = Memory.alloc(Process.pointerSize);
    exc.writePointer(ptr(0));
    const result = il2cpp.runtime_invoke(methodInfo, ptr(0), args, exc);

    if (!exc.readPointer().isNull() || !isValid(result)) {
        console.log("[!] EncodeToPNG failed");
        return null;
    }

    // result is a managed byte[] — read length and data
    const len  = Number(result.add(0x18).readU64());
    if (len <= 0 || len > 50 * 1024 * 1024) return null; // sanity: max 50 MB
    const data = result.add(ARRAY_HEADER).readByteArray(len);
    return new Uint8Array(data);
}

// ── GPU → readable texture pipeline ───────────────────────────────────────────

// Cached method handles for the RenderTexture→Blit→ReadPixels pipeline
let _readablePipeline = null;

function getReadablePipeline() {
    if (_readablePipeline) return _readablePipeline;

    const rtKlass   = findClass("RenderTexture");
    const gfxKlass  = findClass("Graphics");
    const tex2dKlass = findClass("Texture2D");

    if (!rtKlass)   throw new Error("RenderTexture class not found");
    if (!gfxKlass)  throw new Error("Graphics class not found");
    if (!tex2dKlass) throw new Error("Texture2D class not found");

    const getTemporaryMI     = getMethodInfo(rtKlass,   "GetTemporary",     3);
    const setActiveMI        = getMethodInfo(rtKlass,   "set_active",       1);
    const releaseTemporaryMI = getMethodInfo(rtKlass,   "ReleaseTemporary", 1);
    const blitMI             = getMethodInfo(gfxKlass,  "Blit",             2);
    const tex2dCtorMI        = getMethodInfo(tex2dKlass, ".ctor",           2);
    const readPixelsMI       = getMethodInfo(tex2dKlass, "ReadPixels",      3);
    const applyMI            = getMethodInfo(tex2dKlass, "Apply",           0);

    if (!getTemporaryMI) throw new Error("RenderTexture.GetTemporary(3) not found");
    if (!setActiveMI)    throw new Error("RenderTexture.set_active not found");
    if (!blitMI)         throw new Error("Graphics.Blit(2) not found");
    if (!tex2dCtorMI)    throw new Error("Texture2D..ctor(2) not found");
    if (!readPixelsMI)   throw new Error("Texture2D.ReadPixels(3) not found");
    if (!applyMI)        throw new Error("Texture2D.Apply(0) not found");

    _readablePipeline = {
        tex2dKlass,
        getTemporaryMI, setActiveMI, releaseTemporaryMI,
        blitMI, tex2dCtorMI, readPixelsMI, applyMI,
    };
    return _readablePipeline;
}

/**
 * Create a CPU-readable copy of a GPU-only texture.
 *
 * Pipeline: RenderTexture.GetTemporary → Graphics.Blit → set_active →
 *           new Texture2D → ReadPixels → Apply → cleanup RT
 *
 * Returns a NEW Texture2D that is readable (GetPixel / EncodeToPNG work on it).
 */
function makeReadableCopy(sourceTexture, w, h) {
    const p   = getReadablePipeline();
    const exc = Memory.alloc(Process.pointerSize);

    // Reusable int buffers
    const iBuf1 = Memory.alloc(4);
    const iBuf2 = Memory.alloc(4);
    const iBuf3 = Memory.alloc(4);

    // ── 1. RenderTexture.GetTemporary(w, h, 0)  [static] ─────────────────
    iBuf1.writeS32(w);
    iBuf2.writeS32(h);
    iBuf3.writeS32(0);  // depthBuffer = 0
    const a3 = Memory.alloc(3 * Process.pointerSize);
    a3.writePointer(iBuf1);
    a3.add(Process.pointerSize).writePointer(iBuf2);
    a3.add(2 * Process.pointerSize).writePointer(iBuf3);

    exc.writePointer(ptr(0));
    const rt = il2cpp.runtime_invoke(p.getTemporaryMI, ptr(0), a3, exc);
    if (!isValid(rt) || !exc.readPointer().isNull())
        throw new Error("RenderTexture.GetTemporary failed");

    // ── 2. Graphics.Blit(source, rt)  [static, ref-type args] ────────────
    const a2 = Memory.alloc(2 * Process.pointerSize);
    a2.writePointer(sourceTexture);
    a2.add(Process.pointerSize).writePointer(rt);

    exc.writePointer(ptr(0));
    il2cpp.runtime_invoke(p.blitMI, ptr(0), a2, exc);

    // ── 3. RenderTexture.set_active(rt)  [static] ────────────────────────
    const a1 = Memory.alloc(Process.pointerSize);
    a1.writePointer(rt);
    exc.writePointer(ptr(0));
    il2cpp.runtime_invoke(p.setActiveMI, ptr(0), a1, exc);

    // ── 4. new Texture2D(w, h) ───────────────────────────────────────────
    const newTex = il2cpp.object_new(p.tex2dKlass);
    if (!isValid(newTex)) throw new Error("il2cpp_object_new(Texture2D) failed");

    iBuf1.writeS32(w);
    iBuf2.writeS32(h);
    const ca = Memory.alloc(2 * Process.pointerSize);
    ca.writePointer(iBuf1);
    ca.add(Process.pointerSize).writePointer(iBuf2);

    exc.writePointer(ptr(0));
    il2cpp.runtime_invoke(p.tex2dCtorMI, newTex, ca, exc);
    if (!exc.readPointer().isNull())
        throw new Error("Texture2D..ctor exception");

    // ── 5. newTex.ReadPixels(Rect(0,0,w,h), 0, 0) ───────────────────────
    const rectBuf = Memory.alloc(16);
    rectBuf.writeFloat(0);           // x
    rectBuf.add(4).writeFloat(0);    // y
    rectBuf.add(8).writeFloat(w);    // width
    rectBuf.add(12).writeFloat(h);   // height

    iBuf1.writeS32(0); // destX
    iBuf2.writeS32(0); // destY
    const rpa = Memory.alloc(3 * Process.pointerSize);
    rpa.writePointer(rectBuf);
    rpa.add(Process.pointerSize).writePointer(iBuf1);
    rpa.add(2 * Process.pointerSize).writePointer(iBuf2);

    exc.writePointer(ptr(0));
    il2cpp.runtime_invoke(p.readPixelsMI, newTex, rpa, exc);
    if (!exc.readPointer().isNull())
        console.log("[!] ReadPixels exception (may still work)");

    // ── 6. newTex.Apply() ────────────────────────────────────────────────
    exc.writePointer(ptr(0));
    il2cpp.runtime_invoke(p.applyMI, newTex, ptr(0), exc);

    // ── 7. RenderTexture.set_active(null)  [restore] ─────────────────────
    a1.writePointer(ptr(0));
    exc.writePointer(ptr(0));
    il2cpp.runtime_invoke(p.setActiveMI, ptr(0), a1, exc);

    // ── 8. RenderTexture.ReleaseTemporary(rt) ────────────────────────────
    if (p.releaseTemporaryMI) {
        a1.writePointer(rt);
        exc.writePointer(ptr(0));
        il2cpp.runtime_invoke(p.releaseTemporaryMI, ptr(0), a1, exc);
    }

    return newTex;
}

// ── Tile sheet reader ─────────────────────────────────────────────────────────

/**
 * Read all tile-sheet textures from the MapCreator Material array.
 * Returns an array of { material, texture, width, height, isReadable }.
 */
function readTileSheets(mapData, methods) {
    const sheets = [];
    const { qfnPtr, maxSheets } = mapData;

    if (!isValid(qfnPtr)) {
        console.log("[!] qfn (Material[]) is null — trying TileMaterial fallback");
        if (isValid(mapData.tileMaterialPtr)) {
            const tex = invokeReturnPtr(methods.getMainTexMI, mapData.tileMaterialPtr);
            if (isValid(tex)) {
                const w = invokeReturnInt(methods.widthMI, tex);
                const h = invokeReturnInt(methods.heightMI, tex);
                const readable = methods.getIsReadMI ? invokeReturnBool(methods.getIsReadMI, tex) : false;
                sheets.push({ material: mapData.tileMaterialPtr, texture: tex, width: w, height: h, isReadable: readable });
                console.log("[+] Sheet 0 (fallback): " + w + "x" + h + " readable=" + readable);
            }
        }
        return sheets;
    }

    const arrLen = readArrayLength(qfnPtr);
    const sheetCount = Math.min(arrLen, maxSheets > 0 ? maxSheets : arrLen);
    console.log("[+] Material[] length=" + arrLen + ", using " + sheetCount + " sheets");

    for (let i = 0; i < sheetCount; i++) {
        const mat = qfnPtr.add(ARRAY_HEADER + i * Process.pointerSize).readPointer();
        if (!isValid(mat)) {
            console.log("[!] Sheet " + i + ": material is null");
            sheets.push(null);
            continue;
        }

        const tex = invokeReturnPtr(methods.getMainTexMI, mat);
        if (!isValid(tex)) {
            console.log("[!] Sheet " + i + ": mainTexture is null");
            sheets.push(null);
            continue;
        }

        const w = invokeReturnInt(methods.widthMI, tex);
        const h = invokeReturnInt(methods.heightMI, tex);
        const readable = methods.getIsReadMI ? invokeReturnBool(methods.getIsReadMI, tex) : false;

        sheets.push({ material: mat, texture: tex, width: w, height: h, isReadable: readable });
        console.log("[+] Sheet " + i + ": " + w + "x" + h + " readable=" + readable);
    }

    return sheets;
}

// ── Tile ID → (sheet, pixelX, pixelY) decoder ─────────────────────────────────

/**
 * Given a tile ID, compute which sheet texture to sample and the top-left
 * pixel coordinates within that texture.
 *
 * PRO tile encoding (mega-texture layout):
 *   qfp = MaxTileSheets × tilesPerRow  (e.g. 88×64 = 5632)
 *   This means all sheets are treated as one tall virtual strip of
 *   'tilesPerRow' columns wide.
 *
 *   tileID 0 = empty / transparent
 *   tileID > 0:
 *     linear      = tileID - 1       (0-based)
 *     tilesPerRow = sheetWidth / TILE_PX  (e.g. 1024/16 = 64)
 *     tilesPerCol = sheetHeight / TILE_PX (e.g. 1024/16 = 64)
 *     tilesPerSheet = tilesPerRow × tilesPerCol  (e.g. 4096)
 *     sheet  = floor(linear / tilesPerSheet)
 *   Tiles are laid out in 8×8 BLOCKS within each 32×32 tile grid:
 *     block  = floor(tileId / 64)
 *     sheet  = floor(block / 16)         (16 blocks per 1024-tile sheet)
 *     bws    = block % 16                (block-within-sheet)
 *     bx     = floor(bws / 4)            (block column 0-3)
 *     by     = bws % 4                   (block row 0-3)
 *     wb     = tileId % 64               (position within 8×8 block)
 *     wx     = wb % 8,  wy = floor(wb/8)
 *     col    = bx*8 + wx
 *     row    = by*8 + wy - 1   (wrap negatives via modulo)
 *     pixelX = col × TILE_PX
 *     pixelY = row × TILE_PX
 */
function decodeTileId(tileId, sheets, qfp) {
    if (tileId === 0) return null; // empty tile

    const block = Math.floor(tileId / 64);
    const sheet = Math.floor(block / 16);        // 16 blocks of 64 = 1024 tiles per sheet
    const bws   = block % 16;                    // block index within sheet (0-15)
    const bx    = Math.floor(bws / 4);           // block column (0-3)
    const by    = bws % 4;                       // block row (0-3)

    const wb    = tileId % 64;                   // position within 8×8 block
    const wx    = wb % 8;
    const wy    = Math.floor(wb / 8);

    const col   = bx * 8 + wx;
    const row   = by * 8 + wy;  // 0-31 (by∈0-3, wy∈0-7 → max 31)

    if (sheet < 0 || sheet >= sheets.length || !sheets[sheet]) return null;

    return {
        sheet,
        pixelX: col * TILE_PX,
        pixelY: row * TILE_PX,
    };
}

// ── Tile pixel cache ──────────────────────────────────────────────────────────

/**
 * Build a cache mapping unique tile IDs → RGBA pixel arrays (TILE_PX × TILE_PX × 4).
 * Only reads pixels for tiles actually used on the map.
 */
function buildTileCache(mapData, sheets, methods) {
    // 1. Collect all unique tile IDs across all layers
    const uniqueIds = new Set();
    const layers = [mapData.tiles, mapData.tiles2, mapData.tiles3, mapData.tiles4];
    for (const layer of layers) {
        if (!layer) continue;
        for (let i = 0; i < layer.data.length; i++) {
            const id = layer.data[i];
            if (id !== 0) uniqueIds.add(id);
        }
    }

    console.log("[+] Unique tile IDs to extract: " + uniqueIds.size);

    // 2. Group by sheet for efficient access
    const bySheet = new Map(); // sheet index → [{ tileId, pixelX, pixelY }]
    let failCount = 0;

    for (const tileId of uniqueIds) {
        const loc = decodeTileId(tileId, sheets, mapData.qfp);
        if (!loc) {
            failCount++;
            continue;
        }
        if (!bySheet.has(loc.sheet)) bySheet.set(loc.sheet, []);
        bySheet.get(loc.sheet).push({ tileId, pixelX: loc.pixelX, pixelY: loc.pixelY });
    }

    if (failCount > 0) {
        console.log("[!] " + failCount + " tile IDs could not be decoded (out of range)");
    }

    // 3. Read pixels for each tile
    const cache = new Map(); // tileId → Uint8Array(TILE_PX * TILE_PX * 4) RGBA
    const tileBytes = TILE_PX * TILE_PX * 4;
    let totalRead = 0;
    let totalTiles = 0;
    for (const [s, entries] of bySheet) totalTiles += entries.length;

    // Pre-allocate arg buffers for GetPixel (reuse across calls for efficiency)
    const xBuf = Memory.alloc(4);
    const yBuf = Memory.alloc(4);
    const argsBuf = Memory.alloc(2 * Process.pointerSize);
    argsBuf.writePointer(xBuf);
    argsBuf.add(Process.pointerSize).writePointer(yBuf);
    const excBuf = Memory.alloc(Process.pointerSize);

    const tempTextures = [];  // readable copies to clean up later

    for (const [sheetIdx, entries] of bySheet) {
        const sheet = sheets[sheetIdx];
        if (!sheet) {
            console.log("[!] Sheet " + sheetIdx + ": null — tiles will be blank");
            for (const e of entries) cache.set(e.tileId, new Uint8Array(tileBytes));
            continue;
        }

        // If the texture is not CPU-readable, create a readable copy via RT blit
        let tex = sheet.texture;
        if (!sheet.isReadable) {
            console.log("[*] Sheet " + sheetIdx + ": GPU-only, creating readable copy...");
            try {
                tex = makeReadableCopy(sheet.texture, sheet.width, sheet.height);
                tempTextures.push(tex);
                console.log("[+] Sheet " + sheetIdx + ": readable copy OK");
            } catch (e) {
                console.log("[!] Sheet " + sheetIdx + ": readable copy FAILED — " + e.message);
                for (const e2 of entries) cache.set(e2.tileId, new Uint8Array(tileBytes));
                continue;
            }
        }

        const texH = sheet.height;
        const getPixelMI = methods.getPixelMI;

        console.log("[*] Reading " + entries.length + " tiles from sheet " + sheetIdx + "...");

        for (const entry of entries) {
            const pixels = new Uint8Array(tileBytes);

            for (let py = 0; py < TILE_PX; py++) {
                for (let px = 0; px < TILE_PX; px++) {
                    // Unity GetPixel uses bottom-up Y. We want top-down for our image.
                    // So the "top" row of the tile in texture space is at
                    // texY = texH - 1 - (entry.pixelY + 0)   (for the top-most pixel row)
                    // and the "bottom" row at texH - 1 - (entry.pixelY + TILE_PX - 1)
                    //
                    // We read row py (0 = top of tile in output) from:
                    //   texY = texH - 1 - entry.pixelY - py
                    //
                    // Wait, actually — the tile's pixelY is the row index in the
                    // spritesheet counting from top.  Unity GetPixel Y=0 is the
                    // texture bottom.  So we need:
                    //   texY = (texH - 1) - (entry.pixelY + py)
                    const texX = entry.pixelX + px;
                    const texY = (texH - 1) - (entry.pixelY + py);

                    xBuf.writeS32(texX);
                    yBuf.writeS32(texY);
                    excBuf.writePointer(ptr(0));

                    const result = il2cpp.runtime_invoke(getPixelMI, tex, argsBuf, excBuf);

                    const off = (py * TILE_PX + px) * 4;
                    if (isValid(result) && excBuf.readPointer().isNull()) {
                        const r = result.add(0x10).readFloat();
                        const g = result.add(0x14).readFloat();
                        const b = result.add(0x18).readFloat();
                        const a = result.add(0x1C).readFloat();
                        pixels[off]     = Math.round(Math.max(0, Math.min(1, r)) * 255);
                        pixels[off + 1] = Math.round(Math.max(0, Math.min(1, g)) * 255);
                        pixels[off + 2] = Math.round(Math.max(0, Math.min(1, b)) * 255);
                        pixels[off + 3] = Math.round(Math.max(0, Math.min(1, a)) * 255);
                    }
                    // else: stays [0,0,0,0] (transparent black)
                }
            }

            cache.set(entry.tileId, pixels);
            totalRead++;

            if (totalRead % 100 === 0) {
                console.log("    ... " + totalRead + "/" + totalTiles + " tiles read");
            }
        }
    }

    console.log("[+] Tile cache built: " + cache.size + " tiles (" + totalRead + " read)");
    return cache;
}

// ── Save tile sheets as PNGs (optional) ───────────────────────────────────────

function saveTileSheetPngs(sheets, methods, mapName) {
    if (!methods.encodePngMI) {
        console.log("[!] ImageConversion.EncodeToPNG not found — skipping sheet export");
        return;
    }

    for (let i = 0; i < sheets.length; i++) {
        const sheet = sheets[i];
        if (!sheet || !sheet.isReadable) continue;

        console.log("[*] Encoding sheet " + i + " to PNG...");
        const pngBytes = invokeEncodeToPNG(methods.encodePngMI, sheet.texture);
        if (!pngBytes) {
            console.log("[!] Failed to encode sheet " + i);
            continue;
        }

        const safeName = mapName.replace(/[^a-zA-Z0-9_\- ]/g, "").replace(/\s+/g, "_").substring(0, 40) || "map";
        const outPath = OUTPUT_DIR + "\\tiles_runtime\\sheet_" + safeName + "_" + i + ".png";
        const f = new File(outPath, "wb");
        f.write(pngBytes.buffer);
        f.close();
        console.log("[+] Saved sheet " + i + " → " + outPath + " (" + pngBytes.length + " bytes)");
    }
}

// ── CRC32 ─────────────────────────────────────────────────────────────────────
function makeCrcTable() {
    const t = new Uint32Array(256);
    for (let n = 0; n < 256; n++) {
        let c = n;
        for (let k = 0; k < 8; k++) c = (c & 1) ? (0xEDB88320 ^ (c >>> 1)) : (c >>> 1);
        t[n] = c;
    }
    return t;
}
const CRC_TABLE = makeCrcTable();

function crc32(bytes, start, len) {
    let crc = 0xFFFFFFFF;
    for (let i = start; i < start + len; i++) crc = CRC_TABLE[(crc ^ bytes[i]) & 0xFF] ^ (crc >>> 8);
    return (crc ^ 0xFFFFFFFF) >>> 0;
}

// ── Adler-32 ──────────────────────────────────────────────────────────────────
function adler32(data, start, len) {
    let a = 1, b = 0;
    for (let i = start; i < start + len; i++) {
        a = (a + data[i]) % 65521;
        b = (b + a) % 65521;
    }
    return ((b << 16) | a) >>> 0;
}

// ── Minimal PNG encoder (RGBA, uncompressed DEFLATE) ──────────────────────────
function encodePNG_RGBA(imgW, imgH, pixels) {
    // pixels: Uint8Array of length imgW * imgH * 4 (RGBA, row-major, top-to-bottom)
    const rowBytes = imgW * 4;
    const scanline = rowBytes + 1; // +1 for filter byte
    const rawLen   = imgH * scanline;
    const raw      = new Uint8Array(rawLen);
    for (let y = 0; y < imgH; y++) {
        raw[y * scanline] = 0; // filter = None
        raw.set(pixels.subarray(y * rowBytes, y * rowBytes + rowBytes), y * scanline + 1);
    }

    // zlib stored blocks
    const MAX_BLOCK = 65535;
    const numBlocks = Math.ceil(rawLen / MAX_BLOCK) || 1;
    const zlibLen   = 2 + numBlocks * 5 + rawLen + 4;
    const zlib      = new Uint8Array(zlibLen);
    let zp = 0;
    zlib[zp++] = 0x78;
    zlib[zp++] = 0x01;
    let remaining = rawLen, srcOff = 0;
    for (let b = 0; b < numBlocks; b++) {
        const blockLen = Math.min(remaining, MAX_BLOCK);
        const isLast   = (b === numBlocks - 1) ? 1 : 0;
        zlib[zp++] = isLast;
        zlib[zp++] = blockLen & 0xFF;
        zlib[zp++] = (blockLen >> 8) & 0xFF;
        zlib[zp++] = (~blockLen) & 0xFF;
        zlib[zp++] = (~blockLen >> 8) & 0xFF;
        zlib.set(raw.subarray(srcOff, srcOff + blockLen), zp);
        zp += blockLen; srcOff += blockLen; remaining -= blockLen;
    }
    const adl = adler32(raw, 0, rawLen);
    zlib[zp++] = (adl >> 24) & 0xFF;
    zlib[zp++] = (adl >> 16) & 0xFF;
    zlib[zp++] = (adl >>  8) & 0xFF;
    zlib[zp++] =  adl        & 0xFF;

    const SIG  = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    const ihdr = makeIHDR_RGBA(imgW, imgH);
    const idat = makeChunk("IDAT", zlib);
    const iend = makeChunk("IEND", new Uint8Array(0));
    const total = SIG.length + ihdr.length + idat.length + iend.length;
    const png   = new Uint8Array(total);
    let off = 0;
    png.set(SIG, off);   off += SIG.length;
    png.set(ihdr, off);  off += ihdr.length;
    png.set(idat, off);  off += idat.length;
    png.set(iend, off);
    return png;
}

function makeIHDR_RGBA(w, h) {
    const data = new Uint8Array(13);
    data[0]  = (w >> 24) & 0xFF; data[1]  = (w >> 16) & 0xFF;
    data[2]  = (w >>  8) & 0xFF; data[3]  =  w        & 0xFF;
    data[4]  = (h >> 24) & 0xFF; data[5]  = (h >> 16) & 0xFF;
    data[6]  = (h >>  8) & 0xFF; data[7]  =  h        & 0xFF;
    data[8]  = 8;    // bit depth
    data[9]  = 6;    // colour type: RGBA
    data[10] = 0;    // compression
    data[11] = 0;    // filter
    data[12] = 0;    // interlace
    return makeChunk("IHDR", data);
}

function makeChunk(type, data) {
    const len   = data.length;
    const chunk = new Uint8Array(4 + 4 + len + 4);
    chunk[0] = (len >> 24) & 0xFF; chunk[1] = (len >> 16) & 0xFF;
    chunk[2] = (len >>  8) & 0xFF; chunk[3] =  len        & 0xFF;
    chunk[4] = type.charCodeAt(0); chunk[5] = type.charCodeAt(1);
    chunk[6] = type.charCodeAt(2); chunk[7] = type.charCodeAt(3);
    chunk.set(data, 8);
    const c = crc32(chunk, 4, 4 + len);
    chunk[8 + len]     = (c >> 24) & 0xFF;
    chunk[8 + len + 1] = (c >> 16) & 0xFF;
    chunk[8 + len + 2] = (c >>  8) & 0xFF;
    chunk[8 + len + 3] =  c        & 0xFF;
    return chunk;
}

// ── Render map using actual sprites ───────────────────────────────────────────

function renderMapWithSprites(mapData, tileCache) {
    const { width, height } = mapData;
    const imgW = width  * OUT_TILE_PX;
    const imgH = height * OUT_TILE_PX;
    const pixels = new Uint8Array(imgW * imgH * 4);

    // Fill with a neutral background
    for (let i = 0; i < pixels.length; i += 4) {
        pixels[i]     = 20;  // R
        pixels[i + 1] = 20;  // G
        pixels[i + 2] = 20;  // B
        pixels[i + 3] = 255; // A (opaque)
    }

    // Scale factor: texture tiles are TILE_PX, output tiles are OUT_TILE_PX
    const scale = TILE_PX / OUT_TILE_PX; // e.g. 32/16 = 2

    const layers = [mapData.tiles, mapData.tiles2, mapData.tiles3, mapData.tiles4];

    for (let layerIdx = 0; layerIdx < layers.length; layerIdx++) {
        const layer = layers[layerIdx];
        if (!layer) continue;

        for (let ty = 0; ty < height; ty++) {
            for (let tx = 0; tx < width; tx++) {
                const idx = tx * height + ty; // C# [width, height] row-major
                const tileId = layer.data[idx];
                if (tileId === 0) continue;

                const tilePixels = tileCache.get(tileId);
                if (!tilePixels) continue;

                // Blit the tile (with alpha compositing), downscaling from TILE_PX to OUT_TILE_PX
                for (let py = 0; py < OUT_TILE_PX; py++) {
                    for (let px = 0; px < OUT_TILE_PX; px++) {
                        // Nearest-neighbour sample from the source tile
                        const srcX = Math.floor(px * scale);
                        const srcY = Math.floor(py * scale);
                        const srcOff = (srcY * TILE_PX + srcX) * 4;
                        const sr = tilePixels[srcOff];
                        const sg = tilePixels[srcOff + 1];
                        const sb = tilePixels[srcOff + 2];
                        const sa = tilePixels[srcOff + 3];

                        if (sa === 0) continue; // fully transparent

                        const destX = tx * OUT_TILE_PX + px;
                        const destY = ty * OUT_TILE_PX + py;
                        const dstOff = (destY * imgW + destX) * 4;

                        if (sa === 255) {
                            // Fully opaque — overwrite
                            pixels[dstOff]     = sr;
                            pixels[dstOff + 1] = sg;
                            pixels[dstOff + 2] = sb;
                            pixels[dstOff + 3] = 255;
                        } else {
                            // Alpha blend
                            const a  = sa / 255;
                            const ia = 1 - a;
                            pixels[dstOff]     = Math.round(sr * a + pixels[dstOff]     * ia);
                            pixels[dstOff + 1] = Math.round(sg * a + pixels[dstOff + 1] * ia);
                            pixels[dstOff + 2] = Math.round(sb * a + pixels[dstOff + 2] * ia);
                            pixels[dstOff + 3] = 255;
                        }
                    }
                }
            }
        }
    }

    // ── Collision overlay ──────────────────────────────────────────────────────
    // Value 1 = solid wall (red), value 6 = grass/encounter (green)
    // Values 2-5 = directional ledges/hills (arrow overlay)
    //   2 = jump down (↓), 3 = jump left (→), 4 = jump right (←), 5 = jump up (↑)
    // Other non-zero values = unknown (orange diamond marker)
    if (mapData.colliders) {
        // Log unique collider values for diagnostics
        const colVals = new Map(); // value → count
        for (let i = 0; i < mapData.colliders.data.length; i++) {
            const v = mapData.colliders.data[i];
            if (v !== 0) colVals.set(v, (colVals.get(v) || 0) + 1);
        }
        const valStr = Array.from(colVals.entries())
            .sort((a, b) => a[0] - b[0])
            .map(([v, c]) => v + "(×" + c + ")")
            .join(", ");
        console.log("[+] Unique collider values: " + valStr);

        // ── Arrow pixel masks for 16×16 tiles ──
        // Each arrow is a set of [x,y] pairs (0-indexed within the tile)
        // Arrow body + arrowhead pointing in the given direction
        function makeArrowDown() {
            const pts = [];
            // Shaft: 2px wide vertical line from y=2 to y=9
            for (let y = 2; y <= 9; y++) { pts.push([7, y]); pts.push([8, y]); }
            // Arrowhead
            for (let x = 4;  x <= 11; x++) pts.push([x, 10]);
            for (let x = 5;  x <= 10; x++) pts.push([x, 11]);
            for (let x = 6;  x <= 9;  x++) pts.push([x, 12]);
            for (let x = 7;  x <= 8;  x++) pts.push([x, 13]);
            return pts;
        }
        function makeArrowUp() {
            // Flip down arrow vertically
            return makeArrowDown().map(function(p) { return [p[0], 15 - p[1]]; });
        }
        function makeArrowLeft() {
            // Rotate down arrow: swap x↔y and flip
            return makeArrowDown().map(function(p) { return [15 - p[1], p[0]]; });
        }
        function makeArrowRight() {
            // Rotate down arrow: swap x↔y
            return makeArrowDown().map(function(p) { return [p[1], p[0]]; });
        }

        // Pre-build arrow masks per collider value
        // 2 = ledge down, 3 = ledge right, 4 = ledge left
        const arrowMasks = {
            2: makeArrowDown(),
            3: makeArrowRight(),
            4: makeArrowLeft(),
        };

        // Helper: draw a single pixel with blending
        function drawPixel(px, py, r, g, b, alpha) {
            if (px < 0 || py < 0 || px >= imgW || py >= imgH) return;
            const off = (py * imgW + px) * 4;
            const a  = alpha / 255;
            const ia = 1 - a;
            pixels[off]     = Math.round(r * a + pixels[off]     * ia);
            pixels[off + 1] = Math.round(g * a + pixels[off + 1] * ia);
            pixels[off + 2] = Math.round(b * a + pixels[off + 2] * ia);
            pixels[off + 3] = 255;
        }

        for (let ty = 0; ty < height; ty++) {
            for (let tx = 0; tx < width; tx++) {
                const idx = tx * height + ty;
                const cv  = mapData.colliders.data[idx];
                if (cv === 0) continue;

                if (cv === 1) {
                    // Solid wall — 30% red tint
                    for (let py = 0; py < OUT_TILE_PX; py++) {
                        for (let px = 0; px < OUT_TILE_PX; px++) {
                            const destX = tx * OUT_TILE_PX + px;
                            const destY = ty * OUT_TILE_PX + py;
                            const off   = (destY * imgW + destX) * 4;
                            pixels[off]     = Math.min(255, Math.round(pixels[off]     * 0.7 + 180 * 0.3));
                            pixels[off + 1] = Math.round(pixels[off + 1] * 0.7);
                            pixels[off + 2] = Math.round(pixels[off + 2] * 0.7);
                        }
                    }
                } else if (cv === 6) {
                    // Grass / encounter zone — 20% green tint
                    for (let py = 0; py < OUT_TILE_PX; py++) {
                        for (let px = 0; px < OUT_TILE_PX; px++) {
                            const destX = tx * OUT_TILE_PX + px;
                            const destY = ty * OUT_TILE_PX + py;
                            const off   = (destY * imgW + destX) * 4;
                            pixels[off]     = Math.round(pixels[off]     * 0.8);
                            pixels[off + 1] = Math.min(255, Math.round(pixels[off + 1] * 0.8 + 100 * 0.2));
                            pixels[off + 2] = Math.round(pixels[off + 2] * 0.8);
                        }
                    }
                } else if (cv === 5) {
                    // Water / surfable — 25% blue tint
                    for (let py = 0; py < OUT_TILE_PX; py++) {
                        for (let px = 0; px < OUT_TILE_PX; px++) {
                            const destX = tx * OUT_TILE_PX + px;
                            const destY = ty * OUT_TILE_PX + py;
                            const off   = (destY * imgW + destX) * 4;
                            pixels[off]     = Math.round(pixels[off]     * 0.75);
                            pixels[off + 1] = Math.round(pixels[off + 1] * 0.75 + 80 * 0.25);
                            pixels[off + 2] = Math.min(255, Math.round(pixels[off + 2] * 0.75 + 200 * 0.25));
                        }
                    }
                } else if (arrowMasks[cv]) {
                    // ── Directional ledge/hill — draw arrow ──
                    // 1. Light orange tint on the whole tile
                    for (let py = 0; py < OUT_TILE_PX; py++) {
                        for (let px = 0; px < OUT_TILE_PX; px++) {
                            const destX = tx * OUT_TILE_PX + px;
                            const destY = ty * OUT_TILE_PX + py;
                            const off   = (destY * imgW + destX) * 4;
                            pixels[off]     = Math.min(255, Math.round(pixels[off]     * 0.75 + 200 * 0.25));
                            pixels[off + 1] = Math.min(255, Math.round(pixels[off + 1] * 0.75 + 120 * 0.25));
                            pixels[off + 2] = Math.round(pixels[off + 2] * 0.75);
                        }
                    }
                    // 2. Draw arrow: dark outline then bright fill
                    const mask = arrowMasks[cv];
                    const baseX = tx * OUT_TILE_PX;
                    const baseY = ty * OUT_TILE_PX;
                    // Outline (dark brown, 1px border around each arrow pixel)
                    const outlineSet = new Set();
                    const fillSet = new Set();
                    for (let m = 0; m < mask.length; m++) {
                        fillSet.add(mask[m][0] + "," + mask[m][1]);
                    }
                    for (let m = 0; m < mask.length; m++) {
                        const ax = mask[m][0], ay = mask[m][1];
                        for (let dy = -1; dy <= 1; dy++) {
                            for (let dx = -1; dx <= 1; dx++) {
                                if (dx === 0 && dy === 0) continue;
                                const ox = ax + dx, oy = ay + dy;
                                if (ox >= 0 && ox < OUT_TILE_PX && oy >= 0 && oy < OUT_TILE_PX) {
                                    const key = ox + "," + oy;
                                    if (!fillSet.has(key)) outlineSet.add(key);
                                }
                            }
                        }
                    }
                    // Draw outline pixels (dark brown)
                    for (const key of outlineSet) {
                        const parts = key.split(",");
                        drawPixel(baseX + parseInt(parts[0]), baseY + parseInt(parts[1]), 60, 30, 0, 200);
                    }
                    // Draw fill pixels (bright orange-yellow)
                    for (let m = 0; m < mask.length; m++) {
                        drawPixel(baseX + mask[m][0], baseY + mask[m][1], 255, 180, 0, 230);
                    }
                } else {
                    // ── Unknown collider — small orange diamond marker ──
                    const baseX = tx * OUT_TILE_PX;
                    const baseY = ty * OUT_TILE_PX;
                    // Light tint
                    for (let py = 0; py < OUT_TILE_PX; py++) {
                        for (let px = 0; px < OUT_TILE_PX; px++) {
                            const off = ((baseY + py) * imgW + baseX + px) * 4;
                            pixels[off]     = Math.min(255, Math.round(pixels[off]     * 0.8 + 180 * 0.2));
                            pixels[off + 1] = Math.min(255, Math.round(pixels[off + 1] * 0.8 + 100 * 0.2));
                            pixels[off + 2] = Math.round(pixels[off + 2] * 0.8);
                        }
                    }
                    // Diamond shape in center with the collider value
                    var cx = 7, cy = 7;
                    var diamondPts = [[cx,cy-3],[cx-1,cy-2],[cx,cy-2],[cx+1,cy-2],
                        [cx-2,cy-1],[cx-1,cy-1],[cx,cy-1],[cx+1,cy-1],[cx+2,cy-1],
                        [cx-3,cy],[cx-2,cy],[cx-1,cy],[cx,cy],[cx+1,cy],[cx+2,cy],[cx+3,cy],
                        [cx-2,cy+1],[cx-1,cy+1],[cx,cy+1],[cx+1,cy+1],[cx+2,cy+1],
                        [cx-1,cy+2],[cx,cy+2],[cx+1,cy+2],[cx,cy+3]];
                    for (var di = 0; di < diamondPts.length; di++) {
                        drawPixel(baseX + diamondPts[di][0], baseY + diamondPts[di][1], 255, 140, 0, 220);
                    }
                }
            }
        }
    }

    // ── Link overlay (semi-transparent blue border) ───────────────────────────
    if (mapData.links) {
        for (let ty = 0; ty < height; ty++) {
            for (let tx = 0; tx < width; tx++) {
                const idx = tx * height + ty;
                if (mapData.links.data[idx] === 0) continue;

                // Draw a 2px blue border around the tile
                for (let py = 0; py < OUT_TILE_PX; py++) {
                    for (let px = 0; px < OUT_TILE_PX; px++) {
                        if (px >= 2 && px < OUT_TILE_PX - 2 && py >= 2 && py < OUT_TILE_PX - 2) continue;
                        const destX = tx * OUT_TILE_PX + px;
                        const destY = ty * OUT_TILE_PX + py;
                        const off   = (destY * imgW + destX) * 4;
                        pixels[off]     = Math.round(pixels[off]     * 0.4 + 60  * 0.6);
                        pixels[off + 1] = Math.round(pixels[off + 1] * 0.4 + 140 * 0.6);
                        pixels[off + 2] = Math.round(pixels[off + 2] * 0.4 + 255 * 0.6);
                    }
                }
            }
        }
    }

    // ── Player marker (bright yellow 3×3 tile cross) ──────────────────────────
    const px0 = Math.round(mapData.playerX);
    const py0 = Math.round(mapData.playerY);
    const markerTiles = [
        [px0, py0], [px0 - 1, py0], [px0 + 1, py0], [px0, py0 - 1], [px0, py0 + 1],
    ];
    for (const [mtx, mty] of markerTiles) {
        if (mtx < 0 || mty < 0 || mtx >= width || mty >= height) continue;
        // Draw a diamond/cross outline
        for (let py = 0; py < OUT_TILE_PX; py++) {
            for (let px = 0; px < OUT_TILE_PX; px++) {
                // Only draw edge pixels for a hollow marker
                if (px > 1 && px < OUT_TILE_PX - 2 && py > 1 && py < OUT_TILE_PX - 2) continue;
                const destX = mtx * OUT_TILE_PX + px;
                const destY = mty * OUT_TILE_PX + py;
                if (destX < 0 || destY < 0 || destX >= imgW || destY >= imgH) continue;
                const off = (destY * imgW + destX) * 4;
                pixels[off]     = 255; // R
                pixels[off + 1] = 255; // G
                pixels[off + 2] = 0;   // B
                pixels[off + 3] = 255; // A
            }
        }
    }

    return { imgW, imgH, pixels };
}

// ── Main ──────────────────────────────────────────────────────────────────────
//
// GPU methods (RenderTexture, Graphics.Blit, ReadPixels) MUST run on Unity's
// main thread. Frida's script body runs on an injected thread, so we:
//   1. Do all non-GPU work here (read map data, enumerate sheets).
//   2. Hook DSSock.Update() — which runs every frame on the main thread.
//   3. On the next frame the hook fires, does all GPU work, renders, saves PNG.
//   4. Hook detaches itself.
//
try {
    console.log("── read_map_v2.js ───────────────────────────────────────────");
    console.log("[*] Phase 1: Reading map data...");

    const mapData = readMapData();

    if (mapData.width <= 0 || mapData.height <= 0) {
        console.log("[-] Map dimensions invalid — is a map loaded?");
    } else {
        console.log("[*] Phase 2: Resolving texture methods...");
        const methods = resolveTextureMethods();
        console.log("[+] Methods resolved");

        console.log("[*] Phase 3: Reading tile sheets...");
        const sheets = readTileSheets(mapData, methods);

        if (sheets.length === 0) {
            console.log("[-] No tile sheets found — cannot render");
        } else {
            // ── Find DSSock.Update for main-thread hook ───────────────────
            const dsSockKlass = findClass("DSSock");
            if (!dsSockKlass) throw new Error("DSSock class not found for hook");
            const updateMI = getMethodInfo(dsSockKlass, "Update", 0);
            if (!updateMI) throw new Error("DSSock.Update method not found");
            const updatePtr = updateMI.readPointer(); // native function pointer

            console.log("[*] Waiting for next frame (hooking DSSock.Update @ " + updatePtr + ")...");

            let hookFired = false;
            const hook = Interceptor.attach(updatePtr, {
                onEnter: function () {
                    if (hookFired) return;
                    hookFired = true;

                    try {
                        console.log("[*] ─── Main-thread callback entered ───");

                        // Phase 3b: save tile-sheet PNGs (optional)
                        if (SAVE_SHEETS) {
                            console.log("[*] Phase 3b: Saving tile sheet PNGs...");
                            saveTileSheetPngs(sheets, methods, mapData.mapName);
                        }

                        // Phase 4: build tile cache (GPU → readable copies here)
                        console.log("[*] Phase 4: Building tile cache (reading pixels)...");
                        const tileCache = buildTileCache(mapData, sheets, methods);

                        // Phase 5: render
                        console.log("[*] Phase 5: Rendering map with sprites...");
                        const { imgW, imgH, pixels } = renderMapWithSprites(mapData, tileCache);

                        // Phase 6: encode & save
                        console.log("[*] Phase 6: Encoding PNG (" + imgW + "×" + imgH + ")...");
                        const png = encodePNG_RGBA(imgW, imgH, pixels);

                        const safeName = mapData.mapName
                            .replace(/[^a-zA-Z0-9_\- ]/g, "")
                            .replace(/\s+/g, "_")
                            .substring(0, 60) || "map";
                        const outPath = OUTPUT_DIR + "\\" + safeName + "_v2.png";

                        const f = new File(outPath, "wb");
                        f.write(png.buffer);
                        f.close();

                        console.log("[+] ════════════════════════════════════════════════════");
                        console.log("[+] Saved " + png.length + " bytes → " + outPath);
                        console.log("[+] Map : " + mapData.mapName);
                        console.log("[+] Size: " + mapData.width + "×" + mapData.height + " tiles  (" + imgW + "×" + imgH + " px)");
                        console.log("[+] Player pos: (" + mapData.playerX + ", " + mapData.playerY + ")");
                        console.log("[+] Tile sheets: " + sheets.filter(Boolean).length);
                        console.log("[+] Unique tiles rendered: " + tileCache.size);
                        console.log("[+] ════════════════════════════════════════════════════");
                    } catch (e) {
                        console.log("[-] Error in main-thread callback: " + e.message);
                        console.log(e.stack);
                    }

                    hook.detach();
                    console.log("[*] Hook detached. Auto-exiting Frida...");

                    // Auto-exit: send a special message that tells Frida CLI to quit
                    setTimeout(function() {
                        send({ type: 'exit' });
                    }, 500);
                },
            });
        }
    }
} catch (e) {
    console.log("[-] Error: " + e.message);
    console.log(e.stack);
}

})();
