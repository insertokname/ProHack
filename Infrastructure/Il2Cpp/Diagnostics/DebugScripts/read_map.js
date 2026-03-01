// !!!VIBECODED BULLSHIT AHEAD!!!

(function () {
"use strict";

/**
 * read_map.js
 *
 * Reads the currently loaded map from memory and writes a PNG image of it.
 *
 * Resolved chain (all names, no raw module addresses):
 *
 *   DSSock.<pjm>k__BackingField   ← static singleton
 *     └─ .MapCreator              ← MapCreator instance
 *          ├─ .Width   (Int32)     – map width in tiles
 *          ├─ .Height  (Int32)     – map height in tiles
 *          ├─ .MapName (String)    – display name of the map
 *          ├─ .Tiles   (UInt32[,]) – base terrain layer
 *          ├─ .Tiles2  (UInt32[,]) – overlay layer 2
 *          ├─ .Tiles3  (UInt32[,]) – overlay layer 3
 *          ├─ .Tiles4  (UInt32[,]) – overlay layer 4
 *          ├─ .Colliders (Byte[,]) – collision data (0 = walkable)
 *          └─ .Links   (Bool[,])   – warp / link tiles
 *
 *   DSSock.TargetPos (Vector3)     – player tile position
 *
 * Output: a PNG file colour-coded by tile ID, with collision / link overlays.
 */

// ── Configuration ─────────────────────────────────────────────────────────────
const OUTPUT_DIR  = "C:\\Users\\fekete\\Documents\\tests\\PRO-hack\\csharp2\\ProHack\\Infrastructure\\Il2Cpp\\Diagnostics\\DebugScripts";
const TILE_PX     = 4;                   // pixels per map tile (increase for larger image)

// ── IL2CPP bindings ───────────────────────────────────────────────────────────
const mod = Process.getModuleByName("GameAssembly.dll");

const domain_get             = new NativeFunction(mod.getExportByName("il2cpp_domain_get"),             "pointer", []);
const domain_get_assemblies  = new NativeFunction(mod.getExportByName("il2cpp_domain_get_assemblies"),  "pointer", ["pointer","pointer"]);
const assembly_get_image     = new NativeFunction(mod.getExportByName("il2cpp_assembly_get_image"),     "pointer", ["pointer"]);
const image_get_class_count  = new NativeFunction(mod.getExportByName("il2cpp_image_get_class_count"),  "uint",    ["pointer"]);
const image_get_class        = new NativeFunction(mod.getExportByName("il2cpp_image_get_class"),        "pointer", ["pointer","uint"]);
const class_get_name         = new NativeFunction(mod.getExportByName("il2cpp_class_get_name"),         "pointer", ["pointer"]);
const class_get_fields       = new NativeFunction(mod.getExportByName("il2cpp_class_get_fields"),       "pointer", ["pointer","pointer"]);
const field_get_name         = new NativeFunction(mod.getExportByName("il2cpp_field_get_name"),         "pointer", ["pointer"]);
const field_get_offset       = new NativeFunction(mod.getExportByName("il2cpp_field_get_offset"),       "uint",    ["pointer"]);
const field_get_flags        = new NativeFunction(mod.getExportByName("il2cpp_field_get_flags"),        "int",     ["pointer"]);
const field_static_get_value = new NativeFunction(mod.getExportByName("il2cpp_field_static_get_value"), "void",    ["pointer","pointer"]);

const FIELD_ATTR_STATIC = 0x10;

// ── Helpers ───────────────────────────────────────────────────────────────────
function readStr(p) {
    try { return (p && !p.isNull()) ? p.readUtf8String() : null; } catch (e) { return null; }
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
    } catch (e) { return null; }
}

function findClass(name) {
    const domain   = domain_get();
    const cntBuf   = Memory.alloc(4);
    const asmPtrs  = domain_get_assemblies(domain, cntBuf);
    const asmCount = cntBuf.readU32();
    for (let a = 0; a < asmCount; a++) {
        try {
            const asm = asmPtrs.add(a * Process.pointerSize).readPointer();
            const img = assembly_get_image(asm);
            if (!isValid(img)) continue;
            const cc = image_get_class_count(img);
            for (let c = 0; c < cc; c++) {
                const klass = image_get_class(img, c);
                if (!isValid(klass)) continue;
                if (readStr(class_get_name(klass)) === name) return klass;
            }
        } catch (e) {}
    }
    return null;
}

function getField(klass, fieldName) {
    const iter = Memory.alloc(Process.pointerSize);
    iter.writePointer(ptr(0));
    let f;
    while (!(f = class_get_fields(klass, iter)).isNull()) {
        if (readStr(field_get_name(f)) === fieldName) {
            return {
                field:    f,
                offset:   field_get_offset(f),
                isStatic: (field_get_flags(f) & FIELD_ATTR_STATIC) !== 0,
            };
        }
    }
    return null;
}

function getStaticValue(fieldInfo) {
    const buf = Memory.alloc(Process.pointerSize);
    field_static_get_value(fieldInfo.field, buf);
    return buf.readPointer();
}

function readInstancePtr(instance, fieldInfo) {
    return instance.add(fieldInfo.offset).readPointer();
}

function readInstanceS32(instance, fieldInfo) {
    return instance.add(fieldInfo.offset).readS32();
}

function readInstanceFloat(instance, fieldInfo) {
    return instance.add(fieldInfo.offset).readFloat();
}

// ── IL2CPP 2D array reader ────────────────────────────────────────────────────
// Il2CppArray layout on x64:
//   +0x00  klass*              (8)
//   +0x08  monitor*            (8)
//   +0x10  Il2CppArrayBounds*  (8)    — non-null for multi-dim arrays
//   +0x18  max_length          (8)    — total element count
//   +0x20  data[]                     — raw element data (row-major)
//
// Il2CppArrayBounds (each dim):
//   length       (il2cpp_array_size_t, 4 bytes on most builds)
//   lower_bound  (int32_t, 4 bytes)

const ARRAY_DATA_OFFSET = 0x20;

/**
 * Read a 2D UInt32[,] array.  Returns { dim0, dim1, data: Uint32Array }.
 * data is stored row-major: element [i,j] = data[i * dim1 + j].
 */
function readUint32Array2D(arrayPtr, expectedDim0, expectedDim1) {
    if (!isValid(arrayPtr)) return null;
    const total = expectedDim0 * expectedDim1;
    const raw   = arrayPtr.add(ARRAY_DATA_OFFSET).readByteArray(total * 4);
    return {
        dim0: expectedDim0,
        dim1: expectedDim1,
        data: new Uint32Array(raw),
    };
}

/**
 * Read a 2D Byte[,] array.
 */
function readByteArray2D(arrayPtr, expectedDim0, expectedDim1) {
    if (!isValid(arrayPtr)) return null;
    const total = expectedDim0 * expectedDim1;
    const raw   = arrayPtr.add(ARRAY_DATA_OFFSET).readByteArray(total);
    return {
        dim0: expectedDim0,
        dim1: expectedDim1,
        data: new Uint8Array(raw),
    };
}

// ── Map data reader ───────────────────────────────────────────────────────────
function readMapData() {
    // 1. Find DSSock singleton
    const dsKlass = findClass("DSSock");
    if (!dsKlass) throw new Error("DSSock class not found");

    const pjmField = getField(dsKlass, "<pjm>k__BackingField");
    if (!pjmField) throw new Error("DSSock.<pjm>k__BackingField not found");

    const ds = getStaticValue(pjmField);
    if (!isValid(ds)) throw new Error("DSSock instance is null (not logged in?)");
    console.log("[+] DSSock instance @ " + ds);

    // 2. Read player position (TargetPos is a Vector3 value-type embedded in the object)
    const tpField = getField(dsKlass, "TargetPos");
    if (!tpField) throw new Error("DSSock.TargetPos not found");
    const playerX = ds.add(tpField.offset).readFloat();         // x
    const playerY = ds.add(tpField.offset + 4).readFloat();     // y (stored as z in Unity 3D, but PRO treats it as y)

    // 3. Navigate to MapCreator
    const mcFieldInfo = getField(dsKlass, "MapCreator");
    if (!mcFieldInfo) throw new Error("DSSock.MapCreator field not found");
    const mc = readInstancePtr(ds, mcFieldInfo);
    if (!isValid(mc)) throw new Error("MapCreator is null (no map loaded?)");
    console.log("[+] MapCreator @ " + mc);

    // 4. Find MapCreator class for named-field resolution
    const mcKlass = findClass("MapCreator");
    if (!mcKlass) throw new Error("MapCreator class not found");

    // 5. Read scalar fields
    const widthField  = getField(mcKlass, "Width");
    const heightField = getField(mcKlass, "Height");
    if (!widthField || !heightField) throw new Error("Width/Height fields not found");

    const width  = readInstanceS32(mc, widthField);
    const height = readInstanceS32(mc, heightField);
    console.log(`[+] Map size: ${width} x ${height}`);

    // MapName
    const mapNameField = getField(mcKlass, "MapName");
    let mapName = "(unknown)";
    if (mapNameField) {
        const strPtr = readInstancePtr(mc, mapNameField);
        mapName = readIl2CppString(strPtr) || "(unreadable)";
    }
    console.log(`[+] Map name: ${mapName}`);

    // Outside flag
    const outsideField = getField(mcKlass, "Outside");
    let outside = false;
    if (outsideField) {
        outside = mc.add(outsideField.offset).readU8() !== 0;
    }
    console.log(`[+] Outside: ${outside}`);

    // Region
    const regionField = getField(mcKlass, "Region");
    let region = -1;
    if (regionField) {
        region = readInstanceS32(mc, regionField);
    }
    console.log(`[+] Region: ${region}`);

    // 6. Read tile layers
    const tilesField  = getField(mcKlass, "Tiles");
    const tiles2Field = getField(mcKlass, "Tiles2");
    const tiles3Field = getField(mcKlass, "Tiles3");
    const tiles4Field = getField(mcKlass, "Tiles4");

    const tilesPtr  = tilesField  ? readInstancePtr(mc, tilesField)  : ptr(0);
    const tiles2Ptr = tiles2Field ? readInstancePtr(mc, tiles2Field) : ptr(0);
    const tiles3Ptr = tiles3Field ? readInstancePtr(mc, tiles3Field) : ptr(0);
    const tiles4Ptr = tiles4Field ? readInstancePtr(mc, tiles4Field) : ptr(0);

    const tiles  = readUint32Array2D(tilesPtr,  width, height);
    const tiles2 = readUint32Array2D(tiles2Ptr, width, height);
    const tiles3 = readUint32Array2D(tiles3Ptr, width, height);
    const tiles4 = readUint32Array2D(tiles4Ptr, width, height);

    console.log(`[+] Tiles layers read: L1=${!!tiles} L2=${!!tiles2} L3=${!!tiles3} L4=${!!tiles4}`);

    // 7. Read colliders
    const collidersField = getField(mcKlass, "Colliders");
    const collidersPtr   = collidersField ? readInstancePtr(mc, collidersField) : ptr(0);
    const colliders      = readByteArray2D(collidersPtr, width, height);
    console.log(`[+] Colliders read: ${!!colliders}`);

    // 8. Read links
    const linksField = getField(mcKlass, "Links");
    const linksPtr   = linksField ? readInstancePtr(mc, linksField) : ptr(0);
    const links      = readByteArray2D(linksPtr, width, height);   // Boolean[,] stored as bytes
    console.log(`[+] Links read: ${!!links}`);

    return {
        width, height, mapName, outside, region,
        tiles, tiles2, tiles3, tiles4,
        colliders, links,
        playerX, playerY,
    };
}

// ── CRC32 (for PNG chunks) ────────────────────────────────────────────────────
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

// ── Adler-32 (for zlib wrapper) ───────────────────────────────────────────────
function adler32(data, start, len) {
    let a = 1, b = 0;
    for (let i = start; i < start + len; i++) {
        a = (a + data[i]) % 65521;
        b = (b + a) % 65521;
    }
    return ((b << 16) | a) >>> 0;
}

// ── Minimal PNG encoder (uncompressed DEFLATE) ────────────────────────────────
function encodePNG(imgW, imgH, pixels) {
    // pixels is a Uint8Array of length imgW * imgH * 3  (RGB, row-major, top-to-bottom)

    // ── Build raw scanline data (filter byte 0 + row bytes) ──────────────────
    const rowBytes   = imgW * 3;
    const scanline   = rowBytes + 1;          // +1 for filter byte
    const rawLen     = imgH * scanline;
    const raw        = new Uint8Array(rawLen);
    for (let y = 0; y < imgH; y++) {
        raw[y * scanline] = 0;                // filter = None
        raw.set(pixels.subarray(y * rowBytes, y * rowBytes + rowBytes), y * scanline + 1);
    }

    // ── Wrap in zlib stored (uncompressed) DEFLATE ───────────────────────────
    // zlib header: CMF=0x78 FLG=0x01  (deflate, wbits=15, level=0, FCHECK ok)
    // Then one or more stored blocks, then Adler-32.
    const MAX_BLOCK = 65535;
    const numBlocks = Math.ceil(rawLen / MAX_BLOCK) || 1;
    const zlibLen   = 2 + numBlocks * 5 + rawLen + 4;   // header + block-headers + data + adler
    const zlib      = new Uint8Array(zlibLen);
    let zp = 0;
    zlib[zp++] = 0x78;
    zlib[zp++] = 0x01;

    let remaining = rawLen;
    let srcOff    = 0;
    for (let b = 0; b < numBlocks; b++) {
        const blockLen = Math.min(remaining, MAX_BLOCK);
        const isLast   = (b === numBlocks - 1) ? 1 : 0;
        zlib[zp++] = isLast;                              // BFINAL + BTYPE=00 (stored)
        zlib[zp++] = blockLen & 0xFF;
        zlib[zp++] = (blockLen >> 8) & 0xFF;
        zlib[zp++] = (~blockLen) & 0xFF;
        zlib[zp++] = (~blockLen >> 8) & 0xFF;
        zlib.set(raw.subarray(srcOff, srcOff + blockLen), zp);
        zp       += blockLen;
        srcOff   += blockLen;
        remaining -= blockLen;
    }

    const adl = adler32(raw, 0, rawLen);
    zlib[zp++] = (adl >> 24) & 0xFF;
    zlib[zp++] = (adl >> 16) & 0xFF;
    zlib[zp++] = (adl >>  8) & 0xFF;
    zlib[zp++] =  adl        & 0xFF;

    // ── Assemble PNG file ────────────────────────────────────────────────────
    const SIG  = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    const ihdr = makeIHDR(imgW, imgH);
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

function makeIHDR(w, h) {
    const data = new Uint8Array(13);
    data[0]  = (w >> 24) & 0xFF; data[1]  = (w >> 16) & 0xFF;
    data[2]  = (w >>  8) & 0xFF; data[3]  =  w        & 0xFF;
    data[4]  = (h >> 24) & 0xFF; data[5]  = (h >> 16) & 0xFF;
    data[6]  = (h >>  8) & 0xFF; data[7]  =  h        & 0xFF;
    data[8]  = 8;    // bit depth
    data[9]  = 2;    // colour type: RGB
    data[10] = 0;    // compression method
    data[11] = 0;    // filter method
    data[12] = 0;    // interlace method
    return makeChunk("IHDR", data);
}

function makeChunk(type, data) {
    const len   = data.length;
    const chunk = new Uint8Array(4 + 4 + len + 4);          // length + type + data + crc
    chunk[0] = (len >> 24) & 0xFF; chunk[1] = (len >> 16) & 0xFF;
    chunk[2] = (len >>  8) & 0xFF; chunk[3] =  len        & 0xFF;
    chunk[4] = type.charCodeAt(0); chunk[5] = type.charCodeAt(1);
    chunk[6] = type.charCodeAt(2); chunk[7] = type.charCodeAt(3);
    chunk.set(data, 8);
    const c = crc32(chunk, 4, 4 + len);                     // CRC covers type + data
    chunk[8 + len]     = (c >> 24) & 0xFF;
    chunk[8 + len + 1] = (c >> 16) & 0xFF;
    chunk[8 + len + 2] = (c >>  8) & 0xFF;
    chunk[8 + len + 3] =  c        & 0xFF;
    return chunk;
}

// ── Tile-ID → colour ─────────────────────────────────────────────────────────
// Uses a golden-ratio-based hue spread for maximal colour separation.
const GOLDEN_RATIO_CONJUGATE = 0.618033988749895;

function tileIdToRgb(id) {
    if (id === 0) return [30, 30, 30];                       // empty / no tile → dark grey
    const hue = ((id * GOLDEN_RATIO_CONJUGATE) % 1.0);
    const sat = 0.55;
    const lum = 0.50;
    return hslToRgb(hue, sat, lum);
}

function hslToRgb(h, s, l) {
    let r, g, b;
    if (s === 0) { r = g = b = l; }
    else {
        const q = l < 0.5 ? l * (1 + s) : l + s - l * s;
        const p = 2 * l - q;
        r = hue2rgb(p, q, h + 1/3);
        g = hue2rgb(p, q, h);
        b = hue2rgb(p, q, h - 1/3);
    }
    return [Math.round(r * 255), Math.round(g * 255), Math.round(b * 255)];
}

function hue2rgb(p, q, t) {
    if (t < 0) t += 1;
    if (t > 1) t -= 1;
    if (t < 1/6) return p + (q - p) * 6 * t;
    if (t < 1/2) return q;
    if (t < 2/3) return p + (q - p) * (2/3 - t) * 6;
    return p;
}

// ── Blend / overlay helpers ───────────────────────────────────────────────────
function blendOver(base, overlay) {
    // simple 50/50 blend
    return [
        (base[0] + overlay[0]) >> 1,
        (base[1] + overlay[1]) >> 1,
        (base[2] + overlay[2]) >> 1,
    ];
}

function darken(rgb, factor) {
    return [
        Math.round(rgb[0] * factor),
        Math.round(rgb[1] * factor),
        Math.round(rgb[2] * factor),
    ];
}

// ── Render map to pixel buffer ────────────────────────────────────────────────
function renderMap(m) {
    const imgW = m.width  * TILE_PX;
    const imgH = m.height * TILE_PX;
    const pixels = new Uint8Array(imgW * imgH * 3);

    for (let ty = 0; ty < m.height; ty++) {
        for (let tx = 0; tx < m.width; tx++) {
            const idx = tx * m.height + ty;   // C# [width, height] row-major: outer=x, inner=y

            // ── Composite tile colour from layers ────────────────────────────
            let colour = [30, 30, 30];        // default background

            if (m.tiles  && m.tiles.data[idx]  !== 0)
                colour = tileIdToRgb(m.tiles.data[idx]);
            if (m.tiles2 && m.tiles2.data[idx] !== 0)
                colour = blendOver(colour, tileIdToRgb(m.tiles2.data[idx]));
            if (m.tiles3 && m.tiles3.data[idx] !== 0)
                colour = blendOver(colour, tileIdToRgb(m.tiles3.data[idx]));
            if (m.tiles4 && m.tiles4.data[idx] !== 0)
                colour = blendOver(colour, tileIdToRgb(m.tiles4.data[idx]));

            // ── Collision overlay ─────────────────────────────────────────────
            if (m.colliders && m.colliders.data[idx] !== 0) {
                // blocked tile → reddish tint + darken
                colour = blendOver(darken(colour, 0.5), [180, 40, 40]);
            }

            // ── Link overlay ──────────────────────────────────────────────────
            if (m.links && m.links.data[idx] !== 0) {
                colour = [60, 120, 255];      // bright blue for warps
            }

            // ── Player marker (3×3 tile bright yellow cross) ──────────────────
            const px = Math.round(m.playerX);
            const py = Math.round(m.playerY);
            const dx = Math.abs(tx - px);
            const dy = Math.abs(ty - py);
            if (dx + dy <= 1) {
                colour = [255, 255, 0];       // bright yellow
            }

            // ── Write TILE_PX × TILE_PX block ────────────────────────────────
            for (let py2 = 0; py2 < TILE_PX; py2++) {
                for (let px2 = 0; px2 < TILE_PX; px2++) {
                    const ix = tx * TILE_PX + px2;
                    const iy = ty * TILE_PX + py2;
                    const off = (iy * imgW + ix) * 3;
                    pixels[off]     = colour[0];
                    pixels[off + 1] = colour[1];
                    pixels[off + 2] = colour[2];
                }
            }
        }
    }

    return { imgW, imgH, pixels };
}

// ── Main ──────────────────────────────────────────────────────────────────────
try {
    console.log("── read_map.js ──────────────────────────────────────────────");

    const mapData = readMapData();

    if (mapData.width <= 0 || mapData.height <= 0) {
        console.log("[-] Map dimensions invalid — is a map loaded?");
    } else {
        console.log("[*] Rendering image...");
        const { imgW, imgH, pixels } = renderMap(mapData);

        console.log(`[*] Encoding PNG (${imgW}×${imgH})...`);
        const png = encodePNG(imgW, imgH, pixels);

        // Sanitise map name for filename
        const safeName = mapData.mapName
            .replace(/[^a-zA-Z0-9_\- ]/g, "")
            .replace(/\s+/g, "_")
            .substring(0, 60) || "map";
        const outPath = OUTPUT_DIR + "\\" + safeName + ".png";

        const f = new File(outPath, "wb");
        f.write(png.buffer);
        f.close();

        console.log(`[+] Saved ${png.length} bytes → ${outPath}`);
        console.log(`[+] Map : ${mapData.mapName}`);
        console.log(`[+] Size: ${mapData.width}×${mapData.height} tiles  (${imgW}×${imgH} px)`);
        console.log(`[+] Player pos: (${mapData.playerX}, ${mapData.playerY})`);
    }
} catch (e) {
    console.log("[-] Error: " + e.message);
    console.log(e.stack);
}

})();
