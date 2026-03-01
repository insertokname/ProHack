// Save tile sheet 0 as PNG and correlate mesh UVs with tile IDs
(function() {
"use strict";

const OUTPUT_DIR = "C:\\Users\\fekete\\Documents\\tests\\PRO-hack\\csharp2\\ProHack\\Infrastructure\\Il2Cpp\\Diagnostics\\DebugScripts";
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
    runtime_invoke:         new NativeFunction(mod.getExportByName("il2cpp_runtime_invoke"),           "pointer", ["pointer", "pointer", "pointer", "pointer"]),
    object_new:             new NativeFunction(mod.getExportByName("il2cpp_object_new"),               "pointer", ["pointer"]),
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
function getMethodInfo(klass, name, argc) { const mi = il2cpp.class_get_method_from_name(klass, Memory.allocUtf8String(name), argc); return isValid(mi) ? mi : null; }
function invokeRetPtr(mi, inst) { const e = Memory.alloc(8); e.writePointer(ptr(0)); const r = il2cpp.runtime_invoke(mi, inst, ptr(0), e); return (!e.readPointer().isNull() || !isValid(r)) ? null : r; }
function invokeRetInt(mi, inst) { const e = Memory.alloc(8); e.writePointer(ptr(0)); const r = il2cpp.runtime_invoke(mi, inst, ptr(0), e); return (!e.readPointer().isNull() || !isValid(r)) ? -1 : r.add(0x10).readS32(); }

// Get MapCreator
const dsKlass = findClass("DSSock");
const ds = getStaticValue(getField(dsKlass, "<pjm>k__BackingField"));
const mc = ds.add(getField(dsKlass, "MapCreator").offset).readPointer();
const mcKlass = findClass("MapCreator");
const w = mc.add(getField(mcKlass, "Width").offset).readS32();
const h = mc.add(getField(mcKlass, "Height").offset).readS32();
console.log("[+] Map: " + w + "x" + h);

// Read tile arrays
const tiles2Ptr = mc.add(getField(mcKlass, "Tiles2").offset).readPointer();
const tiles2 = new Uint32Array(tiles2Ptr.add(ARRAY_HEADER).readByteArray(w * h * 4));

// === Correlate mesh vertices with UVs for Tiles2 ===
console.log("\n=== Correlating mesh vertices2 with UV2 ===");
const vertField = getField(mcKlass, "newVertices2");
const uvField = getField(mcKlass, "newUV2");

if (!vertField || !uvField) {
    console.log("[-] newVertices2 or newUV2 not found");
} else {
    const vertArr = mc.add(vertField.offset).readPointer();
    const uvArr = mc.add(uvField.offset).readPointer();
    
    if (!isValid(vertArr) || !isValid(uvArr)) {
        console.log("[-] vert or uv array is null");
    } else {
        const arrLen = Number(vertArr.add(0x18).readU64());
        console.log("[+] Array length (slots): " + arrLen);

        // Check each slot (material group)
        for (let s = 0; s < arrLen; s++) {
            const vertListPtr = vertArr.add(ARRAY_HEADER + s * Process.pointerSize).readPointer();
            if (!isValid(vertListPtr)) continue;
            const vertSize = vertListPtr.add(0x18).readS32();
            if (vertSize === 0) continue;

            const uvListPtr = uvArr.add(ARRAY_HEADER + s * Process.pointerSize).readPointer();
            if (!isValid(uvListPtr)) continue;
            const uvSize = uvListPtr.add(0x18).readS32();
            
            const vertItems = vertListPtr.add(0x10).readPointer();
            const uvItems = uvListPtr.add(0x10).readPointer();

            console.log("\n[Slot " + s + "] verts=" + vertSize + " uvs=" + uvSize + " (tiles=" + (vertSize/4) + ")");

            // Read all quads and correlate with tile IDs
            const numQuads = Math.min(vertSize / 4, 30);  // Show up to 30 tiles
            for (let q = 0; q < numQuads; q++) {
                const vi = q * 4; // 4 vertices per quad
                // Read first vertex of quad (bottom-left usually)
                const vBase = vertItems.add(ARRAY_HEADER + vi * 12); // Vector3 = 12 bytes
                const vx = vBase.readFloat();
                const vy = vBase.add(4).readFloat();
                
                // Read UV of first vertex
                const uvBase = uvItems.add(ARRAY_HEADER + vi * 8); // Vector2 = 8 bytes
                const u0 = uvBase.readFloat();
                const v0 = uvBase.add(4).readFloat();
                // Read UV of third vertex (opposite corner)
                const uvBase2 = uvItems.add(ARRAY_HEADER + (vi+2) * 8);
                const u2 = uvBase2.readFloat();
                const v2 = uvBase2.add(4).readFloat();

                // Map position from vertex (game uses x=col, y=-row typically)
                const mapX = Math.round(vx);
                const mapY = Math.round(-vy); // negate Y
                
                // Look up tile ID
                let tileId = 0;
                if (mapX >= 0 && mapX < w && mapY >= 0 && mapY < h) {
                    const idx = mapX * h + mapY;
                    tileId = tiles2[idx];
                }

                // Decode UV to grid position
                const uvCol = Math.round(Math.min(u0, u2) * 32);
                const uvRow = 31 - Math.round(Math.max(v0, v2) * 32); // flip Y: V=1 is top, row 0 is top
                
                // My formula decode
                const linear = tileId > 0 ? tileId - 1 : 0;
                const myCol = linear % 32;
                const myRow = Math.floor(linear / 32);
                const mySheet = Math.floor(linear / 1024);
                const myLocal = linear % 1024;

                console.log("  q" + q + ": vert=(" + vx.toFixed(1) + "," + vy.toFixed(1) + ") → map(" + mapX + "," + mapY + ")" +
                    " tileId=" + tileId +
                    " | UV→sheet" + s + " col=" + uvCol + " row=" + uvRow +
                    " | myFormula→sheet" + mySheet + " col=" + (myLocal % 32) + " row=" + Math.floor(myLocal / 32));
            }
        }
    }
}

// === Now hook DSSock.Update to save sheet 0 as PNG ===
const updateMI = getMethodInfo(findClass("DSSock"), "Update", 0);
const updatePtr = updateMI.readPointer();

console.log("\n[*] Hooking Update to save sheet 0 PNG...");
let hookFired = false;
const hook = Interceptor.attach(updatePtr, {
    onEnter: function() {
        if (hookFired) return;
        hookFired = true;

        try {
            // Get sheet 0 texture
            const materialKlass = findClass("Material");
            const getMainTexMI = getMethodInfo(materialKlass, "get_mainTexture", 0);
            const tex2dKlass = findClass("Texture2D");
            const getWidthMI = getMethodInfo(tex2dKlass, "get_width", 0) || getMethodInfo(findClass("Texture"), "get_width", 0);
            const getHeightMI = getMethodInfo(tex2dKlass, "get_height", 0) || getMethodInfo(findClass("Texture"), "get_height", 0);
            
            const qfnPtr = mc.add(getField(mcKlass, "qfn").offset).readPointer();
            const mat0 = qfnPtr.add(ARRAY_HEADER + 0 * Process.pointerSize).readPointer();
            const tex0 = invokeRetPtr(getMainTexMI, mat0);
            const texW = invokeRetInt(getWidthMI, tex0);
            const texH = invokeRetInt(getHeightMI, tex0);
            console.log("[+] Sheet 0 texture: " + texW + "x" + texH);

            // Make readable copy
            const rtKlass = findClass("RenderTexture");
            const gfxKlass = findClass("Graphics");
            const getTemporaryMI = getMethodInfo(rtKlass, "GetTemporary", 3);
            const setActiveMI = getMethodInfo(rtKlass, "set_active", 1);
            const releaseTemporaryMI = getMethodInfo(rtKlass, "ReleaseTemporary", 1);
            const blitMI = getMethodInfo(gfxKlass, "Blit", 2);
            const tex2dCtorMI = getMethodInfo(tex2dKlass, ".ctor", 2);
            const readPixelsMI = getMethodInfo(tex2dKlass, "ReadPixels", 3);
            const applyMI = getMethodInfo(tex2dKlass, "Apply", 0);
            const imgConvKlass = findClass("ImageConversion");
            const encodePngMI = getMethodInfo(imgConvKlass, "EncodeToPNG", 1);

            const exc = Memory.alloc(8);
            const iBuf1 = Memory.alloc(4), iBuf2 = Memory.alloc(4), iBuf3 = Memory.alloc(4);

            // GetTemporary
            iBuf1.writeS32(texW); iBuf2.writeS32(texH); iBuf3.writeS32(0);
            const a3 = Memory.alloc(3*8); a3.writePointer(iBuf1); a3.add(8).writePointer(iBuf2); a3.add(16).writePointer(iBuf3);
            exc.writePointer(ptr(0));
            const rt = il2cpp.runtime_invoke(getTemporaryMI, ptr(0), a3, exc);

            // Blit
            const a2 = Memory.alloc(16); a2.writePointer(tex0); a2.add(8).writePointer(rt);
            exc.writePointer(ptr(0));
            il2cpp.runtime_invoke(blitMI, ptr(0), a2, exc);

            // set_active(rt)
            const a1 = Memory.alloc(8); a1.writePointer(rt);
            exc.writePointer(ptr(0));
            il2cpp.runtime_invoke(setActiveMI, ptr(0), a1, exc);

            // new Texture2D(w, h)
            const newTex = il2cpp.object_new(tex2dKlass);
            iBuf1.writeS32(texW); iBuf2.writeS32(texH);
            const ca = Memory.alloc(16); ca.writePointer(iBuf1); ca.add(8).writePointer(iBuf2);
            exc.writePointer(ptr(0));
            il2cpp.runtime_invoke(tex2dCtorMI, newTex, ca, exc);

            // ReadPixels
            const rectBuf = Memory.alloc(16);
            rectBuf.writeFloat(0); rectBuf.add(4).writeFloat(0); rectBuf.add(8).writeFloat(texW); rectBuf.add(12).writeFloat(texH);
            iBuf1.writeS32(0); iBuf2.writeS32(0);
            const rpa = Memory.alloc(24); rpa.writePointer(rectBuf); rpa.add(8).writePointer(iBuf1); rpa.add(16).writePointer(iBuf2);
            exc.writePointer(ptr(0));
            il2cpp.runtime_invoke(readPixelsMI, newTex, rpa, exc);

            // Apply
            exc.writePointer(ptr(0));
            il2cpp.runtime_invoke(applyMI, newTex, ptr(0), exc);

            // set_active(null)
            a1.writePointer(ptr(0));
            exc.writePointer(ptr(0));
            il2cpp.runtime_invoke(setActiveMI, ptr(0), a1, exc);

            // ReleaseTemporary
            a1.writePointer(rt);
            exc.writePointer(ptr(0));
            il2cpp.runtime_invoke(releaseTemporaryMI, ptr(0), a1, exc);

            // EncodeToPNG
            const pngArgs = Memory.alloc(8); pngArgs.writePointer(newTex);
            exc.writePointer(ptr(0));
            const pngResult = il2cpp.runtime_invoke(encodePngMI, ptr(0), pngArgs, exc);
            
            if (isValid(pngResult) && exc.readPointer().isNull()) {
                const len = Number(pngResult.add(0x18).readU64());
                const data = pngResult.add(ARRAY_HEADER).readByteArray(len);
                const outPath = OUTPUT_DIR + "\\sheet_0.png";
                const f = new File(outPath, "wb");
                f.write(data);
                f.close();
                console.log("[+] Saved sheet 0 → " + outPath + " (" + len + " bytes)");
            } else {
                console.log("[-] EncodeToPNG failed");
            }
        } catch(e) {
            console.log("[-] Error: " + e.message);
            console.log(e.stack);
        }

        hook.detach();
        console.log("[*] Done.");
        setTimeout(function() { send({ type: 'exit' }); }, 500);
    }
});

})();
