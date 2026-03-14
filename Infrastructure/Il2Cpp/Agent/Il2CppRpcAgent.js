'use strict';

/**
 * Il2CppRpcAgent.js — Persistent IL2CPP game-state reader agent for ProHack.
 *
 * Lifecycle:
 *   1. Discovers stable class/field offsets by name via the IL2CPP reflection API;
 *      uses hardcoded offsets for obfuscated (mangled) fields that change name
 *      every game patch.
 *   2. Sends {cmd:"ready", ok:true} to signal that C# may start issuing reads.
 *   3. Stays resident.  Each {id:<n>, cmd:"read"} request from C# is handled
 *      immediately; the agent reads game memory in-process and replies with
 *      {id:<n>, ok:true, data:{...}} or {id:<n>, ok:false, error:"..."}.
 *
 * ── Stable fields (discovered by NAME at runtime) ───────────────────────────
 *   DSSock.Console      → ChatInput*     (SelectedMenu chain start)
 *   DSSock.OtherPoke    → gw struct      (encounter data base)
 *   DSSock.TargetPos    → Vector3        (PlayerX at +0, PlayerY at +4)
 *   ChatInput.TextList  → UITextList*
 *   UIWidget.onChange   → delegate*      (contains SelectedMenu)
 *
 * ── Obfuscated fields (HARDCODED offsets — immune to name mangling) ─────────
 *   DSSock + 0x750      → Boolean        (IsBattling)
 *   DSSock + 0x7D0      → Int32          (CurrentEncounterId)
 *   DSSock + 0x7E0      → Boolean/Int    (ShinyForm)
 *   DSSock + 0x7E4      → Int32          (EventForm)
 *
 *   These offsets are effective DSSock-relative positions. They remain stable
 *   across patches even when the obfuscator renames the fields. Only a struct
 *   layout change (fields added/removed/reordered) would break them — this is
 *   rare and detectable via verify_layout.js.
 *
 * ── Hardcoded delegate constant ─────────────────────────────────────────────
 *   DELEGATE_SELECTED_MENU_OFFSET = 0xA8
 *   Position of the SelectedMenu int inside a UIWidget.di (System.MulticastDelegate
 *   internal — stable across game patches, only changes on Unity major-version upgrade).
 *   Run Diagnostics/find_onChange_delegate_fields.js to re-verify if needed.
 *
 * ── Map screenshot ────────────────────────────────────────────────────────────
 *   {id:<n>, cmd:"map_screenshot"} → the agent reads the current map data,
 *   extracts tile-sheet textures via the GPU, composites 4 tile layers with
 *   collision/link overlays and a player marker, encodes a PNG, and sends back
 *   {id:<n>, ok:true, data:{mapName, width, height, playerX, playerY}}
 *   with the PNG bytes as a binary attachment.
 *
 *   Performance vs the standalone read_map_v2.js diagnostic:
 *     - Uses GetPixels32() for bulk pixel reads (1 call per sheet instead of
 *       ~800K individual GetPixel calls — ~40,000× fewer il2cpp_runtime_invoke).
 *     - GPU work (RenderTexture Blit + ReadPixels) runs on the main thread via
 *       a DSSock.Update Interceptor hook — one frame only.
 *
 * ── Update procedure ─────────────────────────────────────────────────────────
 *   See Infrastructure/Il2Cpp/UPDATE.md.
 *   If only obfuscated field names changed → NO code changes needed.
 *   If a stable field or class is renamed  → update NAMED_FIELDS / CLASS_NAMES.
 *   If struct layout changed (offsets shifted) → update HARDCODED_OFFSETS.
 */

// ── [1] Constants ─────────────────────────────────────────────────────────────
//
// CLASS_NAMES   – classes looked up by name at runtime (all have stable names).
// NAMED_FIELDS  – fields with stable, human-readable names, discovered by name.
// HARDCODED_OFFSETS – effective DSSock-relative offsets for fields whose names
//                    are mangled by the obfuscator every patch.  Using offsets
//                    instead of names means **no code change** is needed when
//                    the obfuscator renames them.  Only update these if the
//                    struct layout itself changes (fields added/removed/reordered).
//
// Run Diagnostics/verify_layout.js after each game patch to confirm all
// offsets still match.

const CLASS_NAMES = {
    DSSock:    'DSSock',
    ChatInput: 'ChatInput',
    UIWidget:  'UIWidget',  // onChange field is inherited by UITextList
    MapCreator: 'MapCreator',  // map data container (tiles, colliders, links, sheets)
};

const NAMED_FIELDS = {
    // DSSock instance fields (stable names)
    console:   'Console',
    otherPoke: 'OtherPoke',
    targetPos: 'TargetPos', // UnityEngine.Vector3 — x at +0, y at +4 (standard struct layout)
    mapCreator: 'MapCreator', // DSSock → MapCreator instance
    // ChatInput fields
    textList:  'TextList',
    // UIWidget fields
    onChange:  'onChange',
};

// Effective DSSock-relative offsets for obfuscated fields.
// These survive name mangling — only a struct-layout change can break them.
// Verify with: frida -p <PID> -l .\Infrastructure\Il2Cpp\Diagnostics\verify_layout.js
const HARDCODED_OFFSETS = {
    isBattling:         0x750,  // System.Boolean (was: ply → pmh → …)
    currentEncounterId: 0x7D0,  // System.Int32   (effective: OtherPoke+0x10−0x10)
    shinyForm:          0x7E0,  // System.Boolean (effective: OtherPoke+0x20−0x10)
    eventForm:          0x7E4,  // System.Int32   (effective: OtherPoke+0x24−0x10)
};

// Map screenshot offsets. We use offsets (not names) for robustness against
// obfuscator name mangling in MapCreator internals.
const MAP_HARDCODED_OFFSETS = {
    // DSSock
    mapCreator:     0x1E8, // DSSock.MapCreator

    // MapCreator
    width:          0x140,
    height:         0x144,
    tiles:          0x150,
    tiles2:         0x158,
    tiles3:         0x160,
    tiles4:         0x168,
    colliders:      0x170,
    links:          0x178,
    mapName:        0x180,
    maxTileSheets:  0x198,
    tileMaterials:  0x1A8,
};

// Offset of the SelectedMenu int within the UIWidget.di delegate object.
// This lives inside System.MulticastDelegate inherited fields (IL2CPP runtime internal).
// Stable across game patches — run Diagnostics/find_onChange_delegate_fields.js to re-verify
// after a Unity engine major-version upgrade.
const DELEGATE_SELECTED_MENU_OFFSET = 0xA8;


// ── [2]] IL2CPP API bindings ───────────────────────────────────────────────────

const _mod = Process.getModuleByName('GameAssembly.dll');

const _api = {
    domainGet:           new NativeFunction(_mod.getExportByName('il2cpp_domain_get'),                  'pointer', []),
    domainGetAssemblies: new NativeFunction(_mod.getExportByName('il2cpp_domain_get_assemblies'),       'pointer', ['pointer', 'pointer']),
    assemblyGetImage:    new NativeFunction(_mod.getExportByName('il2cpp_assembly_get_image'),          'pointer', ['pointer']),
    imageGetClassCount:  new NativeFunction(_mod.getExportByName('il2cpp_image_get_class_count'),       'uint',    ['pointer']),
    imageGetClass:       new NativeFunction(_mod.getExportByName('il2cpp_image_get_class'),             'pointer', ['pointer', 'uint']),
    classGetName:        new NativeFunction(_mod.getExportByName('il2cpp_class_get_name'),              'pointer', ['pointer']),
    classGetFields:      new NativeFunction(_mod.getExportByName('il2cpp_class_get_fields'),            'pointer', ['pointer', 'pointer']),
    fieldGetName:        new NativeFunction(_mod.getExportByName('il2cpp_field_get_name'),              'pointer', ['pointer']),
    fieldGetOffset:      new NativeFunction(_mod.getExportByName('il2cpp_field_get_offset'),            'uint',    ['pointer']),
    classGetStaticData:  new NativeFunction(_mod.getExportByName('il2cpp_class_get_static_field_data'), 'pointer', ['pointer']),
    classGetMethodFromName: new NativeFunction(_mod.getExportByName('il2cpp_class_get_method_from_name'), 'pointer', ['pointer', 'pointer', 'int']),
    runtimeInvoke:       new NativeFunction(_mod.getExportByName('il2cpp_runtime_invoke'),              'pointer', ['pointer', 'pointer', 'pointer', 'pointer']),
    objectNew:           new NativeFunction(_mod.getExportByName('il2cpp_object_new'),                  'pointer', ['pointer']),
};


// ── [3] IL2CPP helpers ────────────────────────────────────────────────────────

function _readUtf8(p) {
    try { return (p && !p.isNull()) ? p.readUtf8String() : null; } catch (_) { return null; }
}

/**
 * Find a class by simple name across all loaded IL2CPP assemblies.
 * Throws with a descriptive message if not found — caller should propagate to C#.
 */
function _findClass(name) {
    const domain   = _api.domainGet();
    const countBuf = Memory.alloc(4);
    const asmPtrs  = _api.domainGetAssemblies(domain, countBuf);
    const asmCount = countBuf.readU32();

    for (let a = 0; a < asmCount; a++) {
        const asm = asmPtrs.add(a * Process.pointerSize).readPointer();
        const img = _api.assemblyGetImage(asm);
        if (!img || img.isNull()) continue;
        const cc = _api.imageGetClassCount(img);
        for (let c = 0; c < cc; c++) {
            const klass = _api.imageGetClass(img, c);
            if (!klass || klass.isNull()) continue;
            if (_readUtf8(_api.classGetName(klass)) === name) return klass;
        }
    }
    throw new Error(`Class not found: "${name}". Update CLASS_NAMES in the agent.`);
}

/**
 * Return the byte offset of a named field within its declaring class.
 * Throws with a descriptive message if not found.
 */
function _fieldOffset(klass, fieldName) {
    const iter = Memory.alloc(Process.pointerSize);
    iter.writePointer(ptr(0));
    let f;
    while (!(f = _api.classGetFields(klass, iter)).isNull()) {
        if (_readUtf8(_api.fieldGetName(f)) === fieldName)
            return _api.fieldGetOffset(f);
    }
    throw new Error(`Field not found: "${fieldName}". Update NAMED_FIELDS in the agent.`);
}

/**
 * Like _fieldOffset but returns -1 instead of throwing when the field does not exist.
 */
function _tryFieldOffset(klass, fieldName) {
    const iter = Memory.alloc(Process.pointerSize);
    iter.writePointer(ptr(0));
    let f;
    while (!(f = _api.classGetFields(klass, iter)).isNull()) {
        if (_readUtf8(_api.fieldGetName(f)) === fieldName)
            return _api.fieldGetOffset(f);
    }
    return -1;
}

/**
 * Dump all field names + offsets for a class.  Returns a plain object { name → offset }.
 */
function _listClassFields(klass) {
    const out = {};
    const iter = Memory.alloc(Process.pointerSize);
    iter.writePointer(ptr(0));
    let f;
    while (!(f = _api.classGetFields(klass, iter)).isNull()) {
        const name = _readUtf8(_api.fieldGetName(f));
        out[name] = _api.fieldGetOffset(f);
    }
    return out;
}


(function start() {
    try {
        const sz = Process.pointerSize;

        const RtlGetCurrentPeb  = new NativeFunction(Module.getExportByName('ntdll.dll',   'RtlGetCurrentPeb'),     'pointer', []);
        const GetCurrentProcess = new NativeFunction(Module.getExportByName('kernel32.dll', 'GetCurrentProcess'),    'pointer', []);
        const GetProcessId      = new NativeFunction(Module.getExportByName('kernel32.dll', 'GetProcessId'),         'uint32',  ['pointer']);
        const RtlZeroMemory     = new NativeFunction(Module.getExportByName('ntdll.dll',   'RtlZeroMemory'),         'void',    ['pointer', 'size_t']);
        const VirtualProtect    = new NativeFunction(Module.getExportByName('kernel32.dll', 'VirtualProtect'),       'int',     ['pointer', 'size_t', 'uint32', 'pointer']);
        const GetModuleInfo     = new NativeFunction(Module.getExportByName('psapi.dll',    'GetModuleInformation'), 'bool',    ['pointer', 'pointer', 'pointer', 'uint32']);

        const mi = Memory.alloc(3 * sz);
        GetModuleInfo(GetCurrentProcess(), Module.getBaseAddress('frida-agent.dll'), mi, 3 * sz);
        const agentBase = mi.add(0 * sz).readPointer();
        const agentSize = mi.add(1 * sz).readU32();

        function _removeEntry(head) {
            const flink = head.add(0 * sz).readPointer();
            const blink = head.add(1 * sz).readPointer();
            blink.add(0 * sz).writePointer(flink);
            flink.add(1 * sz).writePointer(blink);
        }

        function initList(ldr, listIndex) {
            const tail = ldr.add(listIndex * 2 * sz).add(sz).readPointer();
            let   head = tail;
            do {
                const entry   = head.sub(listIndex * 2 * sz);
                const dllBase = entry.add(6 * sz).readPointer();
                if (entry.isNull() || dllBase.isNull()) break;

                if (agentBase.equals(dllBase)) {
                    const fullName = entry.add(9 * sz);
                    const baseName = entry.add(9 * sz + 2 * sz);
                    fullName.add(0).writeU16(0); fullName.add(2).writeU16(0); fullName.add(sz).writePointer(ptr(0));
                    baseName.add(0).writeU16(0); baseName.add(2).writeU16(0); baseName.add(sz).writePointer(ptr(0));
                    _removeEntry(head);
                }
                head = head.add(sz).readPointer();
            } while (!head.equals(tail));
        }

        const ldr = RtlGetCurrentPeb().add(3 * sz).readPointer().add(4 + 4 + sz);
        initList(ldr, 0);
        initList(ldr, 1);
        initList(ldr, 2);

        const PAGE_READWRITE = 0x04;
        const oldProt = Memory.alloc(4);
        VirtualProtect(agentBase, Process.pageSize, PAGE_READWRITE, oldProt);
        RtlZeroMemory(agentBase, Process.pageSize);
        VirtualProtect(agentBase, Process.pageSize, oldProt.readU32(), oldProt);

        const hSelf = GetCurrentProcess();
        Interceptor.attach(Module.getExportByName('ntdll.dll', 'NtQueryVirtualMemory'), {
            onEnter(args) {
                this.skip = false;
                const proc = args[0];
                const addr = args[1];
                if (!proc.equals(hSelf) && Process.id !== GetProcessId(proc)) return;
                if (addr.isNull()) return;
                if (addr.compare(agentBase) >= 0 && addr.compare(agentBase.add(agentSize)) <= 0)
                    this.skip = true;
            },
            onLeave(retval) {
                if (this.skip)
                    retval.replace(ptr(0xC0000022));
            }
        });
    } catch (_) {
    }
})();


// ── [4] Layout discovery (runs once at load, result cached in _layout) ────────

function _discoverLayout() {
    const dsKlass        = _findClass(CLASS_NAMES.DSSock);
    const chatInputKlass = _findClass(CLASS_NAMES.ChatInput);
    const uiWidgetKlass  = _findClass(CLASS_NAMES.UIWidget);

    // NativePointer — read a pointer here to get the DSSock singleton instance.
    const staticDataAddr = _api.classGetStaticData(dsKlass);
    if (!staticDataAddr || staticDataAddr.isNull())
        throw new Error('DSSock static field data is null — game not fully loaded?');

    return {
        // NativePointer: dereference to get the DSSock singleton pointer.
        staticDataAddr,

        // ── Stable fields (discovered by name) ────────────────────────────────
        offsetConsole:   _fieldOffset(dsKlass,        NAMED_FIELDS.console),
        offsetTextList:  _fieldOffset(chatInputKlass, NAMED_FIELDS.textList),
        offsetOnChange:  _fieldOffset(uiWidgetKlass,  NAMED_FIELDS.onChange),
        offsetTargetPos: _fieldOffset(dsKlass,        NAMED_FIELDS.targetPos),

        // ── Obfuscated fields (hardcoded offsets — immune to name mangling) ───
        // These are effective DSSock-relative offsets. No gw class lookup needed.
        offsetPly:  HARDCODED_OFFSETS.isBattling,
        effOyu:     HARDCODED_OFFSETS.currentEncounterId,
        effOyy:     HARDCODED_OFFSETS.shinyForm,
        effOyz:     HARDCODED_OFFSETS.eventForm,
    };
}


// ── [5] In-process game state read ───────────────────────────────────────────
// Called for every {cmd:"read"} request.  Reads directly from process memory;
// no RPM syscall overhead — we are already inside the target process.

function _readGameState(layout) {
    // ── DSSock singleton ──────────────────────────────────────────────────────
    const dsPtr = layout.staticDataAddr.readPointer();
    if (dsPtr.isNull())
        throw new Error('DSSock singleton is null — game not yet loaded.');

    // ── SelectedMenu chain ────────────────────────────────────────────────────
    // DSSock.Console → ChatInput → .TextList → UITextList → .onChange → delegate + 0xA8
    const chatInputPtr = dsPtr.add(layout.offsetConsole).readPointer();
    if (chatInputPtr.isNull())
        throw new Error('ChatInput pointer is null — UI not yet initialized.');

    const textListPtr = chatInputPtr.add(layout.offsetTextList).readPointer();
    if (textListPtr.isNull())
        throw new Error('TextList pointer is null — UI not yet initialized.');

    const onChangePtr = textListPtr.add(layout.offsetOnChange).readPointer();
    if (onChangePtr.isNull())
        throw new Error('onChange delegate is null — battle UI not yet loaded.');

    const selectedMenu = onChangePtr.add(DELEGATE_SELECTED_MENU_OFFSET).readS32();

    // ── Direct DSSock reads ───────────────────────────────────────────────────
    const isBattling         = dsPtr.add(layout.offsetPly).readU8() !== 0;
    const currentEncounterId = dsPtr.add(layout.effOyu).readS32();
    const shinyForm          = dsPtr.add(layout.effOyy).readU8();  // 0 or 1
    const eventForm          = dsPtr.add(layout.effOyz).readS32();

    // TargetPos is an inline Vector3 (tile grid position of the player).
    // +0 = x (float), +4 = y (float) — standard C struct float stride, not game-specific.
    const playerX = dsPtr.add(layout.offsetTargetPos).readFloat();
    const playerY = dsPtr.add(layout.offsetTargetPos + 4).readFloat();

    let mapName = 'unknown';
    try {
        const mapLay = _discoverMapLayout();
        const mc = dsPtr.add(mapLay.offsetMapCreator).readPointer();
        if (_isValid(mc)) {
            const namePtr = mc.add(mapLay.offsetMapName).readPointer();
            mapName = _readIl2CppString(namePtr) || 'unknown';
        }
    } catch (_) {
        mapName = 'unknown';
    }

    return { selectedMenu, isBattling, currentEncounterId, shinyForm, eventForm, mapName, playerX, playerY };
}


// ── [5b] Map screenshot infrastructure ───────────────────────────────────────
// Lazy-initialized: resolved on first map_screenshot request so startup stays fast.

const _TILE_PX     = 32;  // pixels per tile in the texture
const _OUT_TILE_PX = 16;  // pixels per tile in the output (2× downscale)
const _ARRAY_HDR   = 0x20; // Il2CppArray data offset (x64)

function _isValid(p) {
    if (!p || p.isNull()) return false;
    const v = p.toUInt32();
    return v > 0x10000 && v !== 0xffffffff;
}

// Encode a Uint8Array to a base64 string without relying on browser/Node built-ins.
function _toBase64(bytes) {
    const c = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
    let s = '', l = bytes.length;
    for (let i = 0; i < l; i += 3) {
        const b0 = bytes[i], b1 = i + 1 < l ? bytes[i + 1] : 0, b2 = i + 2 < l ? bytes[i + 2] : 0;
        s += c[b0 >> 2] + c[((b0 & 3) << 4) | (b1 >> 4)];
        s += (i + 1 < l ? c[((b1 & 0xf) << 2) | (b2 >> 6)] : '=');
        s += (i + 2 < l ? c[b2 & 0x3f] : '=');
    }
    return s;
}

function _readIl2CppString(p) {
    try {
        if (!p || p.isNull()) return null;
        const len = p.add(0x10).readS32();
        if (len <= 0 || len > 4096) return null;
        return p.add(0x14).readUtf16String(len);
    } catch (_) { return null; }
}

function _getMethodInfo(klass, name, argc) {
    const mi = _api.classGetMethodFromName(klass, Memory.allocUtf8String(name), argc);
    return (_isValid(mi)) ? mi : null;
}

function _invokeRet(mi, inst, args) {
    if (!mi || (typeof mi === 'object' && mi.isNull && mi.isNull()))
        throw new Error('_invokeRet called with null MethodInfo — method not found');
    const exc = Memory.alloc(Process.pointerSize);
    exc.writePointer(ptr(0));
    const r = _api.runtimeInvoke(mi, inst || ptr(0), args || ptr(0), exc);
    if (!exc.readPointer().isNull()) return null;
    return r;
}

function _invokeRetPtr(mi, inst) { return _invokeRet(mi, inst); }

function _invokeRetInt(mi, inst) {
    const r = _invokeRet(mi, inst);
    return (_isValid(r)) ? r.add(0x10).readS32() : -1;
}

function _invokeRetBool(mi, inst) {
    const r = _invokeRet(mi, inst);
    return (_isValid(r)) ? r.add(0x10).readU8() !== 0 : false;
}

// ── Map layout (lazy, cached) ─────────────────────────────────────────────────

let _mapLayout = null;  // discovered once on first screenshot request

// Cache of raw sheet pixel data, keyed by sheet index string.
// Populated on the first screenshot of a map; cleared automatically when the map changes.
// Subsequent screenshots of the same map skip ALL GPU readbacks entirely.
const _sheetPixelCache  = new Map();

// Cached byte offset of the pixel data pointer inside the native Texture2D object.
// After ReadPixels(), Unity stores the RGBA32 buffer in its C++ ImageData struct:
//   struct ImageData { uint8_t* data; size_t size; };
// We locate it by scanning the first 512 bytes of the native object on the first call
// and cache the offset so subsequent sheets pay zero scan cost.
let _nativeTexDataOff = -1;
let   _sheetCacheMapKey = null;

function _discoverMapLayout() {
    if (_mapLayout) return _mapLayout;

    const mcKlass = _findClass(CLASS_NAMES.MapCreator);
    const dsKlass = _findClass(CLASS_NAMES.DSSock);

    // Dump all MapCreator fields once for debugging.
    const allFields = _listClassFields(mcKlass);
    send({ cmd: 'debug_fields', className: 'MapCreator', fields: allFields });

    // Try to discover the map origin fields.
    // Common names: StartX/StartY, startX/startY, X/Y, MinX/MinY, OriginX/OriginY
    const originNames = ['StartX', 'startX', 'X', 'MinX', 'OriginX'];
    const originNamesY = ['StartY', 'startY', 'Y', 'MinY', 'OriginY'];
    let offStartX = -1, offStartY = -1;
    for (const n of originNames) { offStartX = _tryFieldOffset(mcKlass, n); if (offStartX >= 0) break; }
    for (const n of originNamesY) { offStartY = _tryFieldOffset(mcKlass, n); if (offStartY >= 0) break; }
    send({ cmd: 'debug_origin', offStartX, offStartY, fieldDump: Object.keys(allFields).join(', ') });

    _mapLayout = {
        // DSSock → MapCreator pointer offset (name-discovered for sanity).
        // If this name ever changes, fallback to hardcoded offset.
        offsetMapCreator: (() => {
            try { return _fieldOffset(dsKlass, NAMED_FIELDS.mapCreator); }
            catch (_) { return MAP_HARDCODED_OFFSETS.mapCreator; }
        })(),

        // MapCreator field offsets — hardcoded for reliability against obfuscation.
        offsetWidth:         MAP_HARDCODED_OFFSETS.width,
        offsetHeight:        MAP_HARDCODED_OFFSETS.height,
        offsetMapName:       MAP_HARDCODED_OFFSETS.mapName,
        offsetTiles:         MAP_HARDCODED_OFFSETS.tiles,
        offsetTiles2:        MAP_HARDCODED_OFFSETS.tiles2,
        offsetTiles3:        MAP_HARDCODED_OFFSETS.tiles3,
        offsetTiles4:        MAP_HARDCODED_OFFSETS.tiles4,
        offsetColliders:     MAP_HARDCODED_OFFSETS.colliders,
        offsetLinks:         MAP_HARDCODED_OFFSETS.links,
        offsetMaxTileSheets: MAP_HARDCODED_OFFSETS.maxTileSheets,
        offsetTileMaterials: MAP_HARDCODED_OFFSETS.tileMaterials,

        // Map origin (world-coord of tile [0,0]).  -1 if the field wasn't found.
        offsetStartX: offStartX,
        offsetStartY: offStartY,
    };
    return _mapLayout;
}

// ── Unity method handles (lazy, cached) ───────────────────────────────────────

let _unityMethods = null;

function _resolveUnityMethods() {
    if (_unityMethods) return _unityMethods;

    const matKlass   = _findClass('Material');
    const tex2dKlass = _findClass('Texture2D');
    const texKlass   = _findClass('Texture');
    const rtKlass    = _findClass('RenderTexture');
    const gfxKlass   = _findClass('Graphics');

    let widthMI  = _getMethodInfo(tex2dKlass, 'get_width', 0)  || _getMethodInfo(texKlass, 'get_width', 0);
    let heightMI = _getMethodInfo(tex2dKlass, 'get_height', 0) || _getMethodInfo(texKlass, 'get_height', 0);

    // Pixel-read strategy — probe all known overload counts so we never miss
    // a stripped-down or non-standard IL2CPP build.
    // Priority: GetRawTextureData > GetPixels32 > GetPixels (bulk) > GetPixel (per-pixel)

    // GetRawTextureData() → byte[] — raw native pixels, zero IL2CPP overhead.
    // Available in Unity 2018.1+ (not generic version).  0-arg only.
    const getRawTexDataMI   = _getMethodInfo(tex2dKlass, 'GetRawTextureData', 0);

    // GetPixels32([miplevel]) → Color32[] (4 bytes/pixel, r g b a), zero float conversion.
    let getPixels32MI = null, getPixels32Args = -1;
    for (let _na = 0; _na <= 3 && !getPixels32MI; _na++) {
        const mi = _getMethodInfo(tex2dKlass, 'GetPixels32', _na);
        if (mi) { getPixels32MI = mi; getPixels32Args = _na; }
    }

    // GetPixels([miplevel]) → Color[] (4 floats = 16 bytes/pixel), needs float→byte.
    // Also check 4-arg block overload: GetPixels(x, y, w, h) — same Color[] result.
    let getPixelsMI_new = null, getPixelsArgs = -1;
    for (let _na = 0; _na <= 5 && !getPixelsMI_new; _na++) {
        const mi = _getMethodInfo(tex2dKlass, 'GetPixels', _na);
        if (mi) { getPixelsMI_new = mi; getPixelsArgs = _na; }
    }

    let getPixelMI = _getMethodInfo(tex2dKlass, 'GetPixel', 2);

    let pixelReadMode = 'GetPixel'; // final fallback
    if      (getRawTexDataMI) pixelReadMode = 'GetRawTextureData'; // raw bytes, zero overhead
    else if (getPixels32MI)   pixelReadMode = 'GetPixels32';       // Color32[], zero conversion
    else if (getPixelsMI_new) pixelReadMode = 'GetPixels';         // Color[], needs float→byte
    else if (getPixelMI)      pixelReadMode = 'NativeMemory';      // direct native scan, then GetPixel fallback

    // Apply: try 0-arg, fall back to 1-arg or 2-arg
    let applyMI = _getMethodInfo(tex2dKlass, 'Apply', 0)
               || _getMethodInfo(tex2dKlass, 'Apply', 1)
               || _getMethodInfo(tex2dKlass, 'Apply', 2);

    // RenderTexture.GetTemporary: try 3-arg, fall back to 2-arg
    let getTemporaryMI = _getMethodInfo(rtKlass, 'GetTemporary', 3)
                      || _getMethodInfo(rtKlass, 'GetTemporary', 2);

    // ReadPixels: try 3-arg, fall back to 4-arg
    let readPixelsMI = _getMethodInfo(tex2dKlass, 'ReadPixels', 3)
                    || _getMethodInfo(tex2dKlass, 'ReadPixels', 4);

    const m = {
        tex2dKlass,
        getMainTexMI:        _getMethodInfo(matKlass, 'get_mainTexture', 0),
        getIsReadMI:         _getMethodInfo(tex2dKlass, 'get_isReadable', 0),
        widthMI,
        heightMI,
        getRawTexDataMI,
        getPixelsMI: getPixelsMI_new, getPixelsArgs,
        getPixelMI,
        getPixels32MI, getPixels32Args,
        pixelReadMode,
        getTemporaryMI,
        setActiveMI:         _getMethodInfo(rtKlass, 'set_active', 1),
        releaseTemporaryMI:  _getMethodInfo(rtKlass, 'ReleaseTemporary', 1),
        blitMI:              _getMethodInfo(gfxKlass, 'Blit', 2),
        tex2dCtorMI:         _getMethodInfo(tex2dKlass, '.ctor', 2),
        readPixelsMI,
        applyMI,
    };

    // Validate all critical methods — throw with the specific name that failed
    const required = [
        ['Material.get_mainTexture', m.getMainTexMI],
        ['Texture.get_width',        m.widthMI],
        ['Texture.get_height',       m.heightMI],
        ['RenderTexture.GetTemporary', m.getTemporaryMI],
        ['RenderTexture.set_active',   m.setActiveMI],
        ['Graphics.Blit',            m.blitMI],
        ['Texture2D..ctor',          m.tex2dCtorMI],
        ['Texture2D.ReadPixels',     m.readPixelsMI],
        ['Texture2D.Apply',          m.applyMI],
    ];
    const missing = required.filter(function(e) { return !e[1]; }).map(function(e) { return e[0]; });
    if (missing.length > 0)
        throw new Error('Unity methods not found: ' + missing.join(', '));

    _unityMethods = m;
    return _unityMethods;
}

// ── GPU → readable texture copy ───────────────────────────────────────────────

function _makeReadableCopy(srcTex, w, h) {
    const m   = _resolveUnityMethods();
    const ib1 = Memory.alloc(4), ib2 = Memory.alloc(4), ib3 = Memory.alloc(4);
    const a3  = Memory.alloc(3 * Process.pointerSize);
    const a2  = Memory.alloc(2 * Process.pointerSize);
    const a1  = Memory.alloc(Process.pointerSize);

    // RenderTexture.GetTemporary(w, h, 0)
    ib1.writeS32(w); ib2.writeS32(h); ib3.writeS32(0);
    a3.writePointer(ib1); a3.add(Process.pointerSize).writePointer(ib2); a3.add(2 * Process.pointerSize).writePointer(ib3);
    const rt = _invokeRet(m.getTemporaryMI, ptr(0), a3);
    if (!_isValid(rt)) throw new Error('GetTemporary failed');

    // Graphics.Blit(src, rt)
    a2.writePointer(srcTex); a2.add(Process.pointerSize).writePointer(rt);
    _invokeRet(m.blitMI, ptr(0), a2);

    // RenderTexture.set_active(rt)
    a1.writePointer(rt);
    _invokeRet(m.setActiveMI, ptr(0), a1);

    // new Texture2D(w, h)
    const newTex = _api.objectNew(m.tex2dKlass);
    if (!_isValid(newTex)) throw new Error('object_new(Texture2D) failed');
    ib1.writeS32(w); ib2.writeS32(h);
    const ca = Memory.alloc(2 * Process.pointerSize);
    ca.writePointer(ib1); ca.add(Process.pointerSize).writePointer(ib2);
    _invokeRet(m.tex2dCtorMI, newTex, ca);

    // newTex.ReadPixels(Rect(0,0,w,h), 0, 0)
    const rectBuf = Memory.alloc(16);
    rectBuf.writeFloat(0); rectBuf.add(4).writeFloat(0);
    rectBuf.add(8).writeFloat(w); rectBuf.add(12).writeFloat(h);
    ib1.writeS32(0); ib2.writeS32(0);
    const rpa = Memory.alloc(3 * Process.pointerSize);
    rpa.writePointer(rectBuf); rpa.add(Process.pointerSize).writePointer(ib1); rpa.add(2 * Process.pointerSize).writePointer(ib2);
    _invokeRet(m.readPixelsMI, newTex, rpa);

    // Note: Apply() (CPU→GPU re-upload) is intentionally omitted here.
    // ReadPixels already populated the CPU-side pixel buffer, which is all
    // GetPixels32 needs.  Re-uploading wastes one GPU round-trip per sheet.

    // Cleanup RT
    a1.writePointer(ptr(0));
    _invokeRet(m.setActiveMI, ptr(0), a1);
    if (m.releaseTemporaryMI) { a1.writePointer(rt); _invokeRet(m.releaseTemporaryMI, ptr(0), a1); }

    return newTex;
}

// ── Bulk pixel read (strategy auto-selected) ─────────────────────────────────
// Returns Uint8Array(w * h * 4) in RGBA bottom-to-top order (Unity convention).

function _readPixelsBulk(tex, w, h) {
    const m = _resolveUnityMethods();
    if (m.pixelReadMode === 'GetPixels32')  return _readViaGetPixels32(m, tex, w, h);
    if (m.pixelReadMode === 'GetPixels')    return _readViaGetPixels(m, tex, w, h);
    if (m.pixelReadMode === 'NativeMemory') return _readViaNativeMemory(m, tex, w, h);
    return _readViaGetPixel(m, tex, w, h);
}

// Strategy: direct native memory scan — bypasses all stripped Unity bulk-read APIs.
//
// After ReadPixels() into a Texture2D(w, h) (default RGBA32 format), Unity's C++ runtime
// stores the pixel bytes in a native ImageData struct:
//   struct ImageData { uint8_t* data;  size_t size; }  (16 bytes on x64)
// This struct lives somewhere in the first ~512 bytes of the native Texture2D object,
// which is pointed to by `m_CachedPtr` at managed-object offset 0x10.
//
// Cost once the offset is cached: 2 GetPixel calls (≈22µs) + 1 readByteArray ≋2ms/sheet.
function _readViaNativeMemory(m, tex, w, h) {
    // ─ Get 2 reference pixels for buffer validation (2 IL2CPP calls) ────────────
    const xBuf = Memory.alloc(4), yBuf = Memory.alloc(4);
    const gpArgs = Memory.alloc(2 * Process.pointerSize);
    gpArgs.writePointer(xBuf); gpArgs.add(Process.pointerSize).writePointer(yBuf);
    const exc = Memory.alloc(Process.pointerSize);

    function _gpBytes(px, py) {
        try {
            xBuf.writeS32(px); yBuf.writeS32(py);
            exc.writePointer(ptr(0));
            const c = _api.runtimeInvoke(m.getPixelMI, tex, gpArgs, exc);
            if (!exc.readPointer().isNull() || !_isValid(c)) return null;
            return [
                Math.round(Math.min(1, Math.max(0, c.add(0x10).readFloat())) * 255) & 0xFF,
                Math.round(Math.min(1, Math.max(0, c.add(0x14).readFloat())) * 255) & 0xFF,
                Math.round(Math.min(1, Math.max(0, c.add(0x18).readFloat())) * 255) & 0xFF,
                Math.round(Math.min(1, Math.max(0, c.add(0x1C).readFloat())) * 255) & 0xFF,
            ];
        } catch(_) { return null; }
    }
    const ref0 = _gpBytes(0, 0);
    const ref1 = _gpBytes(1, 0);

    function _matches(buf) {
        const tol = 1;
        if (ref0) for (let i = 0; i < 4; i++) if (Math.abs(buf[i]     - ref0[i]) > tol) return false;
        if (ref1) for (let i = 0; i < 4; i++) if (Math.abs(buf[4 + i] - ref1[i]) > tol) return false;
        return true;
    }

    // ─ Navigate to native Texture2D C++ object via m_CachedPtr @ managed+0x10 ────
    const nativeTex = tex.add(0x10).readPointer();
    if (!_isValid(nativeTex)) throw new Error('m_CachedPtr is null');
    const bufSize = w * h * 4;

    // ─ Try cached offset first ───────────────────────────────────────────
    if (_nativeTexDataOff >= 0) {
        try {
            const dataPtr = nativeTex.add(_nativeTexDataOff).readPointer();
            if (_isValid(dataPtr)) {
                const buf = new Uint8Array(dataPtr.readByteArray(bufSize));
                if (_matches(buf)) return buf;
            }
        } catch(_) {}
        _nativeTexDataOff = -1; // stale — re-scan
    }

    // ─ Scan: find ImageData { uint8_t* data; size_t size; } ────────────────────
    // size_t is 8 bytes on x64; check lo/hi 32-bit halves separately to avoid
    // any issues with Frida UInt64 boxing.
    for (let off = 0x08; off <= 0x200; off += 8) {
        try {
            const sizeLo = nativeTex.add(off + 8).readU32();
            const sizeHi = nativeTex.add(off + 12).readU32();
            if (sizeLo !== bufSize || sizeHi !== 0) continue;

            const dataPtr = nativeTex.add(off).readPointer();
            if (!_isValid(dataPtr)) continue;

            const buf = new Uint8Array(dataPtr.readByteArray(bufSize));
            if (_matches(buf)) {
                _nativeTexDataOff = off; // cache for all subsequent sheets
                return buf;
            }
        } catch(_) {}
    }

    // ─ Scan failed: fall back to per-pixel GetPixel ──────────────────────────
    return _readViaGetPixel(m, tex, w, h);
}

// Strategy 0: GetPixels32 → Color32[] (4 bytes/pixel, r g b a) — fastest, zero conversion.
function _readViaGetPixels32(m, tex, w, h) {
    // IMPORTANT: always call with the exact arg count we discovered.
    // Calling a 1-arg method with NULL params causes a Windows SEH access violation
    // (~700 ms per occurrence) before IL2CPP can even enter the method body.
    let result;
    if (m.getPixels32Args === 1) {
        const mipBuf = Memory.alloc(4);
        mipBuf.writeS32(0);
        const args = Memory.alloc(Process.pointerSize);
        args.writePointer(mipBuf);
        result = _invokeRet(m.getPixels32MI, tex, args);
    } else {
        result = _invokeRet(m.getPixels32MI, tex, ptr(0)); // 0-arg: NULL args is correct
    }
    if (!_isValid(result)) throw new Error('GetPixels32 returned null');
    const len = Number(result.add(0x18).readU64());
    if (len !== w * h) throw new Error('GetPixels32 length mismatch: ' + len + ' vs ' + (w * h));
    // Color32[] data: 4 bytes per element (r, g, b, a) — already in the right format.
    return new Uint8Array(result.add(_ARRAY_HDR).readByteArray(len * 4));
}

// Strategy 1: GetPixels → Color[] (4 floats = 16 bytes per pixel), convert to RGBA bytes
function _readViaGetPixels(m, tex, w, h) {
    let result;
    if (m.getPixelsArgs === 1) {
        const mipBuf = Memory.alloc(4);
        mipBuf.writeS32(0);
        const args = Memory.alloc(Process.pointerSize);
        args.writePointer(mipBuf);
        result = _invokeRet(m.getPixelsMI, tex, args);
    } else {
        result = _invokeRet(m.getPixelsMI, tex, ptr(0));
    }
    if (!_isValid(result)) throw new Error('GetPixels returned null');

    const len = Number(result.add(0x18).readU64());
    if (len !== w * h) throw new Error('GetPixels length mismatch: ' + len + ' vs ' + (w * h));

    // Color[] — each element is 4 floats (r, g, b, a) = 16 bytes
    const floatData = result.add(_ARRAY_HDR).readByteArray(len * 16);
    const floats = new Float32Array(floatData);
    const out = new Uint8Array(len * 4);
    for (let i = 0; i < len; i++) {
        out[i * 4]     = Math.round(Math.max(0, Math.min(1, floats[i * 4]))     * 255);
        out[i * 4 + 1] = Math.round(Math.max(0, Math.min(1, floats[i * 4 + 1])) * 255);
        out[i * 4 + 2] = Math.round(Math.max(0, Math.min(1, floats[i * 4 + 2])) * 255);
        out[i * 4 + 3] = Math.round(Math.max(0, Math.min(1, floats[i * 4 + 3])) * 255);
    }
    return out;
}

// Strategy 2: GetPixel(x, y) per-pixel — slowest but guaranteed to work
function _readViaGetPixel(m, tex, w, h) {
    const out = new Uint8Array(w * h * 4);
    const xBuf = Memory.alloc(4), yBuf = Memory.alloc(4);
    const args = Memory.alloc(2 * Process.pointerSize);
    args.writePointer(xBuf);
    args.add(Process.pointerSize).writePointer(yBuf);
    const exc = Memory.alloc(Process.pointerSize);

    for (let y = 0; y < h; y++) {
        yBuf.writeS32(y);
        for (let x = 0; x < w; x++) {
            xBuf.writeS32(x);
            exc.writePointer(ptr(0));
            const r = _api.runtimeInvoke(m.getPixelMI, tex, args, exc);
            if (exc.readPointer().isNull() && _isValid(r)) {
                const off = (y * w + x) * 4;
                out[off]     = Math.round(Math.max(0, Math.min(1, r.add(0x10).readFloat())) * 255);
                out[off + 1] = Math.round(Math.max(0, Math.min(1, r.add(0x14).readFloat())) * 255);
                out[off + 2] = Math.round(Math.max(0, Math.min(1, r.add(0x18).readFloat())) * 255);
                out[off + 3] = Math.round(Math.max(0, Math.min(1, r.add(0x1C).readFloat())) * 255);
            }
        }
    }
    return out;
}

// ── Read map data (non-GPU, runs on any thread) ──────────────────────────────

function _readMapData(layout, mapLay) {
    const dsPtr = layout.staticDataAddr.readPointer();
    if (dsPtr.isNull()) throw new Error('DSSock singleton is null');

    const mc = dsPtr.add(mapLay.offsetMapCreator).readPointer();
    if (!_isValid(mc)) throw new Error('MapCreator is null — no map loaded?');

    const width  = mc.add(mapLay.offsetWidth).readS32();
    const height = mc.add(mapLay.offsetHeight).readS32();
    if (width <= 0 || height <= 0) throw new Error('Invalid map dimensions: ' + width + 'x' + height);

    const namePtr = mc.add(mapLay.offsetMapName).readPointer();
    const mapName = _readIl2CppString(namePtr) || 'unknown';

    const total = width * height;

    function readU32Arr(off) {
        const p = mc.add(off).readPointer();
        if (!_isValid(p)) return null;
        return new Uint32Array(p.add(_ARRAY_HDR).readByteArray(total * 4));
    }
    function readU8Arr(off) {
        const p = mc.add(off).readPointer();
        if (!_isValid(p)) return null;
        return new Uint8Array(p.add(_ARRAY_HDR).readByteArray(total));
    }

    const playerX = dsPtr.add(layout.offsetTargetPos).readFloat();
    const playerY = dsPtr.add(layout.offsetTargetPos + 4).readFloat();
    const maxSheets = mc.add(mapLay.offsetMaxTileSheets).readS32();
    const tileMaterialsPtr = mc.add(mapLay.offsetTileMaterials).readPointer();

    // Map origin: world-coordinate of tile [0,0].
    const startX = mapLay.offsetStartX >= 0 ? mc.add(mapLay.offsetStartX).readS32() : 0;
    const startY = mapLay.offsetStartY >= 0 ? mc.add(mapLay.offsetStartY).readS32() : 0;

    return {
        width, height, mapName, playerX, playerY, startX, startY, maxSheets, tileMaterialsPtr,
        tiles:     readU32Arr(mapLay.offsetTiles),
        tiles2:    readU32Arr(mapLay.offsetTiles2),
        tiles3:    readU32Arr(mapLay.offsetTiles3),
        tiles4:    readU32Arr(mapLay.offsetTiles4),
        colliders: readU8Arr(mapLay.offsetColliders),
        links:     readU8Arr(mapLay.offsetLinks),
    };
}

// ── Tile ID decoder (8×8 block layout) ────────────────────────────────────────

function _decodeTileId(tileId, sheetCount) {
    if (tileId === 0) return null;
    const block = Math.floor(tileId / 64);
    const sheet = Math.floor(block / 16);
    if (sheet < 0 || sheet >= sheetCount) return null;
    const bws = block % 16;
    const bx  = Math.floor(bws / 4), by = bws % 4;
    const wb  = tileId % 64;
    const wx  = wb % 8, wy = Math.floor(wb / 8);
    return { sheet, col: bx * 8 + wx, row: by * 8 + wy };
}

// ── Read tile sheets + bulk pixel data (GPU — MUST run on main thread) ────────

function _readSheetPixels(map) {
    const m = _resolveUnityMethods();
    const tileMaterialsPtr = map.tileMaterialsPtr;
    if (!_isValid(tileMaterialsPtr)) throw new Error('Tile material array is null');

    const arrLen     = Number(tileMaterialsPtr.add(0x18).readU64());
    const sheetCount = Math.min(arrLen, map.maxSheets > 0 ? map.maxSheets : arrLen);

    // 1. Collect unique tile IDs and determine which sheets are needed
    const uniqueIds = new Set();
    const layers = [map.tiles, map.tiles2, map.tiles3, map.tiles4];
    for (const layer of layers) {
        if (!layer) continue;
        for (let i = 0; i < layer.length; i++) if (layer[i] !== 0) uniqueIds.add(layer[i]);
    }

    const neededSheets = new Set();
    for (const tid of uniqueIds) {
        const loc = _decodeTileId(tid, sheetCount);
        if (loc) neededSheets.add(loc.sheet);
    }

    // 2. For each needed sheet: get texture → make readable copy → GetPixels32
    const sheetData = new Array(sheetCount).fill(null); // { pixels: Uint8Array, w, h }

    for (const si of neededSheets) {
        const mat = tileMaterialsPtr.add(_ARRAY_HDR + si * Process.pointerSize).readPointer();
        if (!_isValid(mat)) continue;

        const tex = _invokeRetPtr(m.getMainTexMI, mat);
        if (!_isValid(tex)) continue;

        const w = _invokeRetInt(m.widthMI, tex);
        const h = _invokeRetInt(m.heightMI, tex);
        if (w <= 0 || h <= 0) continue;

        const readable = _invokeRetBool(m.getIsReadMI, tex);
        let readTex = tex;
        if (!readable) {
            readTex = _makeReadableCopy(tex, w, h);
        }

        const pixels = _readPixelsBulk(readTex, w, h);
        sheetData[si] = { pixels, w, h };
    }

    return { sheetData, sheetCount, uniqueIds };
}

// ── Build tile cache from bulk sheet data ─────────────────────────────────────

function _buildTileCache(sheetInfo) {
    const { sheetData, sheetCount, uniqueIds } = sheetInfo;
    // Output tiles are always _OUT_TILE_PX×_OUT_TILE_PX (16×16).
    // Each sheet entry carries `tileSize`: the pixel size of one tile in the *source* sheet.
    //   • Full-size readback  → tileSize = _TILE_PX (32) → step = 2 (sample every other pixel)
    //   • Half-size readback  → tileSize = _TILE_PX>>1 (16) → step = 1 (copy directly)
    // The opaque flag lets _renderMap use TypedArray.set (native memcpy) on solid tiles.
    const outBytes = _OUT_TILE_PX * _OUT_TILE_PX * 4;
    const cache    = new Map();

    for (const tileId of uniqueIds) {
        const loc = _decodeTileId(tileId, sheetCount);
        if (!loc || !sheetData[loc.sheet]) continue;

        const sd        = sheetData[loc.sheet];
        const srcTilePx = sd.tileSize !== undefined ? sd.tileSize : _TILE_PX;
        const step      = srcTilePx / _OUT_TILE_PX; // 1 for half-size, 2 for full-size
        const pixels    = new Uint8Array(outBytes);
        const srcW      = sd.w, srcH = sd.h;
        const srcPx     = loc.col * srcTilePx;
        const srcPy     = loc.row * srcTilePx;

        let opaque = true;
        for (let ty = 0; ty < _OUT_TILE_PX; ty++) {
            const srcRow = (srcH - 1) - (srcPy + ty * step);
            if (srcRow < 0 || srcRow >= srcH) { opaque = false; continue; }
            for (let tx = 0; tx < _OUT_TILE_PX; tx++) {
                const srcOff = (srcRow * srcW + srcPx + tx * step) * 4;
                const dstOff = (ty * _OUT_TILE_PX + tx) * 4;
                pixels[dstOff]     = sd.pixels[srcOff];
                pixels[dstOff + 1] = sd.pixels[srcOff + 1];
                pixels[dstOff + 2] = sd.pixels[srcOff + 2];
                pixels[dstOff + 3] = sd.pixels[srcOff + 3];
                if (pixels[dstOff + 3] < 255) opaque = false;
            }
        }
        cache.set(tileId, { pixels, opaque });
    }
    return cache;
}

// ── Composite map with sprites + overlays ─────────────────────────────────────

// lowRes=true: skip GPU tile drawing; render only collision/link/player overlays on a gray background.
// Used by the low-res fast-path that avoids all GPU work and completes in <100 ms.
function _renderMap(map, tileCache, lowRes) {
    const w = map.width, h = map.height;
    const imgW = w * _OUT_TILE_PX, imgH = h * _OUT_TILE_PX;
    const px = new Uint8Array(imgW * imgH * 4);
    // Background: dark gray for normal mode (tile sprites paint over it);
    // medium gray for low-res mode (visible as the walkable-floor colour).
    const bg = lowRes ? 80 : 20;
    for (let i = 0; i < px.length; i += 4) { px[i] = bg; px[i+1] = bg; px[i+2] = bg; px[i+3] = 255; }

    if (!lowRes) {
    const layers = [map.tiles, map.tiles2, map.tiles3, map.tiles4];

    // ── Tile layers ───────────────────────────────────────────────────────────
    // Cache entries are { pixels: Uint8Array(OUT_TILE_PX²×4), opaque: bool }.
    // The downscale (32→16) was pre-baked in _buildTileCache, so no Math.floor here.
    // Opaque tiles copy whole rows with TypedArray.set (native memcpy — one call per row).
    // Only semi-transparent overlay tiles fall back to the per-pixel alpha loop.
    for (let li = 0; li < 4; li++) {
        const layer = layers[li];
        if (!layer) continue;
        for (let ty = 0; ty < h; ty++) {
            for (let tx = 0; tx < w; tx++) {
                const tid = layer[tx * h + ty];
                if (tid === 0) continue;
                const entry = tileCache.get(tid);
                if (!entry) continue;
                const tp      = entry.pixels;
                const dstBase = ((ty * _OUT_TILE_PX) * imgW + tx * _OUT_TILE_PX) * 4;
                if (entry.opaque) {
                    // Fast path: copy each 16-pixel row with a single native memcpy.
                    for (let py = 0; py < _OUT_TILE_PX; py++) {
                        const so = py * _OUT_TILE_PX * 4;
                        px.set(tp.subarray(so, so + _OUT_TILE_PX * 4), dstBase + py * imgW * 4);
                    }
                } else {
                    // Slow path: per-pixel alpha blend for semi-transparent overlay tiles.
                    for (let py = 0; py < _OUT_TILE_PX; py++) {
                        const rowSrc = py * _OUT_TILE_PX * 4;
                        const rowDst = dstBase + py * imgW * 4;
                        for (let ppx = 0; ppx < _OUT_TILE_PX; ppx++) {
                            const so = rowSrc + ppx * 4;
                            const sa = tp[so + 3];
                            if (sa === 0) continue;
                            const dO = rowDst + ppx * 4;
                            if (sa === 255) {
                                px[dO] = tp[so]; px[dO+1] = tp[so+1]; px[dO+2] = tp[so+2]; px[dO+3] = 255;
                            } else {
                                const a = sa / 255, ia = 1 - a;
                                px[dO]   = Math.round(tp[so]   * a + px[dO]   * ia);
                                px[dO+1] = Math.round(tp[so+1] * a + px[dO+1] * ia);
                                px[dO+2] = Math.round(tp[so+2] * a + px[dO+2] * ia);
                                px[dO+3] = 255;
                            }
                        }
                    }
                }
            }
        }
    }
    } // end if (!lowRes) — tile sprites omitted in low-res mode

    // ── Collision overlays ────────────────────────────────────────────────────
    if (map.colliders) {
        // Arrow masks for ledges (16×16)
        function mkArrDown() {
            const p = [];
            for (let y = 2; y <= 9; y++) { p.push([7,y]); p.push([8,y]); }
            for (let x = 4; x <= 11; x++) p.push([x,10]);
            for (let x = 5; x <= 10; x++) p.push([x,11]);
            for (let x = 6; x <= 9; x++)  p.push([x,12]);
            p.push([7,13]); p.push([8,13]);
            return p;
        }
        const arrDown  = mkArrDown();
        const arrUp    = arrDown.map(function(p){return [p[0],15-p[1]];});
        const arrLeft  = arrDown.map(function(p){return [15-p[1],p[0]];});
        const arrRight = arrDown.map(function(p){return [p[1],p[0]];});
        const arrMasks = { 2: arrDown, 3: arrRight, 4: arrLeft };

        function blendPx(dO, r, g, b, alpha) {
            if (dO < 0 || dO >= px.length) return;
            const a = alpha / 255, ia = 1 - a;
            px[dO]   = Math.round(r * a + px[dO]   * ia);
            px[dO+1] = Math.round(g * a + px[dO+1] * ia);
            px[dO+2] = Math.round(b * a + px[dO+2] * ia);
            px[dO+3] = 255;
        }

        for (let ty = 0; ty < h; ty++) {
            for (let tx = 0; tx < w; tx++) {
                const cv = map.colliders[tx * h + ty];
                if (cv === 0) continue;
                const bx = tx * _OUT_TILE_PX, by = ty * _OUT_TILE_PX;
                if (cv === 1) {
                    // Wall — red tint
                    for (let py = 0; py < _OUT_TILE_PX; py++) for (let ppx = 0; ppx < _OUT_TILE_PX; ppx++) {
                        const o = ((by+py)*imgW+bx+ppx)*4;
                        px[o] = Math.min(255, Math.round(px[o]*0.7+180*0.3));
                        px[o+1] = Math.round(px[o+1]*0.7); px[o+2] = Math.round(px[o+2]*0.7);
                    }
                } else if (cv === 6) {
                    // Grass — green tint
                    for (let py = 0; py < _OUT_TILE_PX; py++) for (let ppx = 0; ppx < _OUT_TILE_PX; ppx++) {
                        const o = ((by+py)*imgW+bx+ppx)*4;
                        px[o] = Math.round(px[o]*0.8);
                        px[o+1] = Math.min(255, Math.round(px[o+1]*0.8+100*0.2));
                        px[o+2] = Math.round(px[o+2]*0.8);
                    }
                } else if (cv === 5) {
                    // Water — blue tint
                    for (let py = 0; py < _OUT_TILE_PX; py++) for (let ppx = 0; ppx < _OUT_TILE_PX; ppx++) {
                        const o = ((by+py)*imgW+bx+ppx)*4;
                        px[o] = Math.round(px[o]*0.75);
                        px[o+1] = Math.round(px[o+1]*0.75+80*0.25);
                        px[o+2] = Math.min(255, Math.round(px[o+2]*0.75+200*0.25));
                    }
                } else if (arrMasks[cv]) {
                    // Ledge — orange tint + directional arrow
                    for (let py = 0; py < _OUT_TILE_PX; py++) for (let ppx = 0; ppx < _OUT_TILE_PX; ppx++) {
                        const o = ((by+py)*imgW+bx+ppx)*4;
                        px[o] = Math.min(255, Math.round(px[o]*0.75+200*0.25));
                        px[o+1] = Math.min(255, Math.round(px[o+1]*0.75+120*0.25));
                        px[o+2] = Math.round(px[o+2]*0.75);
                    }
                    const mask = arrMasks[cv];
                    const fillSet = new Set(); const outSet = new Set();
                    for (let mi = 0; mi < mask.length; mi++) fillSet.add(mask[mi][0]+','+mask[mi][1]);
                    for (let mi = 0; mi < mask.length; mi++) {
                        for (let dy = -1; dy <= 1; dy++) for (let dx = -1; dx <= 1; dx++) {
                            if (!dx && !dy) continue;
                            const k = (mask[mi][0]+dx)+','+(mask[mi][1]+dy);
                            if (!fillSet.has(k)) outSet.add(k);
                        }
                    }
                    for (const k of outSet) {
                        const p = k.split(','); const ox = parseInt(p[0]), oy = parseInt(p[1]);
                        if (ox >= 0 && ox < _OUT_TILE_PX && oy >= 0 && oy < _OUT_TILE_PX)
                            blendPx(((by+oy)*imgW+bx+ox)*4, 60, 30, 0, 200);
                    }
                    for (let mi = 0; mi < mask.length; mi++)
                        blendPx(((by+mask[mi][1])*imgW+bx+mask[mi][0])*4, 255, 180, 0, 230);
                } else {
                    // Unknown — orange diamond
                    for (let py = 0; py < _OUT_TILE_PX; py++) for (let ppx = 0; ppx < _OUT_TILE_PX; ppx++) {
                        const o = ((by+py)*imgW+bx+ppx)*4;
                        px[o] = Math.min(255, Math.round(px[o]*0.8+180*0.2));
                        px[o+1] = Math.min(255, Math.round(px[o+1]*0.8+100*0.2));
                        px[o+2] = Math.round(px[o+2]*0.8);
                    }
                    const cx = 7, cy = 7;
                    const dp = [[cx,cy-3],[cx-1,cy-2],[cx,cy-2],[cx+1,cy-2],
                        [cx-2,cy-1],[cx-1,cy-1],[cx,cy-1],[cx+1,cy-1],[cx+2,cy-1],
                        [cx-3,cy],[cx-2,cy],[cx-1,cy],[cx,cy],[cx+1,cy],[cx+2,cy],[cx+3,cy],
                        [cx-2,cy+1],[cx-1,cy+1],[cx,cy+1],[cx+1,cy+1],[cx+2,cy+1],
                        [cx-1,cy+2],[cx,cy+2],[cx+1,cy+2],[cx,cy+3]];
                    for (let di = 0; di < dp.length; di++)
                        blendPx(((by+dp[di][1])*imgW+bx+dp[di][0])*4, 255, 140, 0, 220);
                }
            }
        }
    }

    // ── Link overlay (blue border) ────────────────────────────────────────────
    if (map.links) {
        for (let ty = 0; ty < h; ty++) for (let tx = 0; tx < w; tx++) {
            if (map.links[tx * h + ty] === 0) continue;
            const bx = tx * _OUT_TILE_PX, by = ty * _OUT_TILE_PX;
            for (let py = 0; py < _OUT_TILE_PX; py++) for (let ppx = 0; ppx < _OUT_TILE_PX; ppx++) {
                if (ppx >= 2 && ppx < _OUT_TILE_PX-2 && py >= 2 && py < _OUT_TILE_PX-2) continue;
                const o = ((by+py)*imgW+bx+ppx)*4;
                px[o]   = Math.round(px[o]*0.4+60*0.6);
                px[o+1] = Math.round(px[o+1]*0.4+140*0.6);
                px[o+2] = Math.round(px[o+2]*0.4+255*0.6);
            }
        }
    }

    // ── Player marker (yellow cross) ──────────────────────────────────────────
    const pmx = Math.round(map.playerX), pmy = Math.round(map.playerY);
    const mTiles = [[pmx,pmy],[pmx-1,pmy],[pmx+1,pmy],[pmx,pmy-1],[pmx,pmy+1]];
    for (const [mtx, mty] of mTiles) {
        if (mtx < 0 || mty < 0 || mtx >= w || mty >= h) continue;
        const bx = mtx * _OUT_TILE_PX, by = mty * _OUT_TILE_PX;
        for (let py = 0; py < _OUT_TILE_PX; py++) for (let ppx = 0; ppx < _OUT_TILE_PX; ppx++) {
            if (ppx > 1 && ppx < _OUT_TILE_PX-2 && py > 1 && py < _OUT_TILE_PX-2) continue;
            const o = ((by+py)*imgW+bx+ppx)*4;
            if (o >= 0 && o+3 < px.length) { px[o]=255; px[o+1]=255; px[o+2]=0; px[o+3]=255; }
        }
    }

    return { imgW, imgH, pixels: px };
}

// ── PNG encoder (uncompressed DEFLATE, minimal) ──────────────────────────────

function _encodePNG(imgW, imgH, pixels) {
    const crcT = new Uint32Array(256);
    for (let n = 0; n < 256; n++) { let c = n; for (let k = 0; k < 8; k++) c = (c&1) ? (0xEDB88320^(c>>>1)) : (c>>>1); crcT[n] = c; }
    function crc32(b, s, l) { let c = 0xFFFFFFFF; for (let i = s; i < s+l; i++) c = crcT[(c^b[i])&0xFF]^(c>>>8); return (c^0xFFFFFFFF)>>>0; }
    function adler32(d, s, l) { let a=1, b=0; for (let i=s;i<s+l;i++){a=(a+d[i])%65521;b=(b+a)%65521;} return ((b<<16)|a)>>>0; }

    const rowB = imgW * 4, scanline = rowB + 1, rawLen = imgH * scanline;
    const raw = new Uint8Array(rawLen);
    for (let y = 0; y < imgH; y++) { raw[y*scanline] = 0; raw.set(pixels.subarray(y*rowB, y*rowB+rowB), y*scanline+1); }

    const MAX_BLK = 65535, numBlk = Math.ceil(rawLen/MAX_BLK)||1;
    const zlibLen = 2 + numBlk*5 + rawLen + 4, zlib = new Uint8Array(zlibLen);
    let zp = 0; zlib[zp++]=0x78; zlib[zp++]=0x01;
    let rem = rawLen, so = 0;
    for (let b = 0; b < numBlk; b++) {
        const bl = Math.min(rem, MAX_BLK), last = (b===numBlk-1)?1:0;
        zlib[zp++]=last; zlib[zp++]=bl&0xFF; zlib[zp++]=(bl>>8)&0xFF;
        zlib[zp++]=(~bl)&0xFF; zlib[zp++]=(~bl>>8)&0xFF;
        zlib.set(raw.subarray(so,so+bl),zp); zp+=bl; so+=bl; rem-=bl;
    }
    const adl = adler32(raw,0,rawLen);
    zlib[zp++]=(adl>>24)&0xFF; zlib[zp++]=(adl>>16)&0xFF; zlib[zp++]=(adl>>8)&0xFF; zlib[zp++]=adl&0xFF;

    function mkChunk(type, data) {
        const l = data.length, ch = new Uint8Array(4+4+l+4);
        ch[0]=(l>>24)&0xFF; ch[1]=(l>>16)&0xFF; ch[2]=(l>>8)&0xFF; ch[3]=l&0xFF;
        ch[4]=type.charCodeAt(0); ch[5]=type.charCodeAt(1); ch[6]=type.charCodeAt(2); ch[7]=type.charCodeAt(3);
        ch.set(data,8); const c = crc32(ch,4,4+l);
        ch[8+l]=(c>>24)&0xFF; ch[8+l+1]=(c>>16)&0xFF; ch[8+l+2]=(c>>8)&0xFF; ch[8+l+3]=c&0xFF;
        return ch;
    }
    const ihdrD = new Uint8Array(13);
    ihdrD[0]=(imgW>>24)&0xFF; ihdrD[1]=(imgW>>16)&0xFF; ihdrD[2]=(imgW>>8)&0xFF; ihdrD[3]=imgW&0xFF;
    ihdrD[4]=(imgH>>24)&0xFF; ihdrD[5]=(imgH>>16)&0xFF; ihdrD[6]=(imgH>>8)&0xFF; ihdrD[7]=imgH&0xFF;
    ihdrD[8]=8; ihdrD[9]=6; ihdrD[10]=0; ihdrD[11]=0; ihdrD[12]=0;

    const SIG = [0x89,0x50,0x4E,0x47,0x0D,0x0A,0x1A,0x0A];
    const ihdr = mkChunk('IHDR', ihdrD), idat = mkChunk('IDAT', zlib), iend = mkChunk('IEND', new Uint8Array(0));
    const total = SIG.length + ihdr.length + idat.length + iend.length;
    const png = new Uint8Array(total);
    let off = 0; png.set(SIG,off); off+=SIG.length; png.set(ihdr,off); off+=ihdr.length;
    png.set(idat,off); off+=idat.length; png.set(iend,off);
    return png;
}

// ── DSSock.Update hook for main-thread GPU work ──────────────────────────────

let _pendingScreenshot = null; // { id: requestId } or null
let _updateHook = null;
let _mfState    = null; // multi-frame screenshot state (null when idle)

// ── Multi-frame screenshot hook (batched GPU) ─────────────────────────────────
//
//  The dominant cost of a fresh screenshot is GPU pipeline stalls:
//  ReadPixels() forces the GPU command queue to drain completely before it
//  returns.  The old approach did one Blit + one ReadPixels per sheet, paying
//  that flush penalty N times (N = number of unique tile sheets on the map).
//
//  New approach — 4 Update frames:
//
//    Frame A  (phase 'init')  Read map tile data from CPU memory.
//    Frame B  (phase 'blit')  Queue ALL Graphics.Blit() calls to the GPU in one
//                             shot, each targeting a HALF-SIZE RenderTexture.
//                             No ReadPixels — GPU runs asynchronously.
//                             Half-size = 4× fewer pixels (256×256 vs 512×512),
//                             GPU hardware bilinear downscale is free, and the
//                             output tiles are already at _OUT_TILE_PX (16 px).
//    Frame C  (phase 'read')  Call ReadPixels() for every RenderTexture.
//                             The FIRST call flushes the GPU for ALL queued Blits.
//                             Every subsequent call is instant — GPU is done.
//                             Net result: N sheets → exactly 1 GPU stall.
//    Frame D  (phase 'build') _buildTileCache + _renderMap + send.
//
//  Cache hit: cached sheets are consumed in Frame B (no GPU); if all sheets are
//  cached, Frame C is a no-op and the pipeline completes in 3 frames total.

function _setupUpdateHook() {
    if (_updateHook) return;

    const dsKlass  = _findClass(CLASS_NAMES.DSSock);
    const updateMI = _getMethodInfo(dsKlass, 'Update', 0);
    if (!updateMI) throw new Error('DSSock.Update not found — cannot hook main thread');
    const updatePtr = updateMI.readPointer();

    _updateHook = Interceptor.attach(updatePtr, {
        onEnter: function () {

            // ── Phase 'init': read map data, build sheet list ─────────────────
            if (_pendingScreenshot && !_mfState) {
                const req = _pendingScreenshot;
                _pendingScreenshot = null;
                const _t0 = Date.now();
                try {
                    const mapLay = _discoverMapLayout();
                    const map    = _readMapData(_layout, mapLay);
                    const tileMaterialsPtr = map.tileMaterialsPtr;
                    if (!_isValid(tileMaterialsPtr)) throw new Error('Tile material array is null');
                    const arrLen     = Number(tileMaterialsPtr.add(0x18).readU64());
                    const sheetCount = Math.min(arrLen, map.maxSheets > 0 ? map.maxSheets : arrLen);

                    const uniqueIds = new Set();
                    [map.tiles, map.tiles2, map.tiles3, map.tiles4].forEach(function (l) {
                        if (!l) return;
                        for (let i = 0; i < l.length; i++) if (l[i] !== 0) uniqueIds.add(l[i]);
                    });
                    const neededSheets = new Set();
                    for (const tid of uniqueIds) {
                        const loc = _decodeTileId(tid, sheetCount);
                        if (loc) neededSheets.add(loc.sheet);
                    }

                    _mfState = {
                        id: req.id, map, tileMaterialsPtr,
                        mode: req.mode || 'normal',
                        neededSheets: Array.from(neededSheets),
                        sheetData:    new Array(sheetCount).fill(null),
                        sheetCount, uniqueIds,
                        phase:      'blit',
                        pendingRTs: [],   // { rt, newTex, hw, hh, si, tileSize }
                        timing: {
                            initMs:        Date.now() - _t0,
                            neededSheets:  neededSheets.size,
                            pixelReadMode: _resolveUnityMethods().pixelReadMode,
                            gp32Args:      _resolveUnityMethods().getPixels32Args,
                            gpArgs:        _resolveUnityMethods().getPixelsArgs,
                            blitMs:        0, blitNew: 0, blitCached: 0,
                            readFirstMs:   0, readRestMs: 0, readCount: 0,
                            sumSetActiveMs: 0, sumReadPixelsMs: 0,
                            sumGP32Ms: 0, sumReleaseMs: 0,
                            buildTileMs:   0, renderMs: 0, buildMs: 0,
                        },
                    };
                } catch (e) {
                    send({ id: req.id, ok: false, error: e.message });
                }
                return;
            }

            if (!_mfState) return;
            const state = _mfState;

            // ── Phase 'blit': queue ALL GPU Blits in one frame ────────────────
            // Cache hits consumed immediately. Uncached: GetTemporary(half-size)
            // + Blit — GPU command queued async, no stall. ReadPixels deferred.
            if (state.phase === 'blit') {
                const _tBlit0 = Date.now();
                const m = _resolveUnityMethods();
                for (const si of state.neededSheets) {
                    const cacheKey = '' + si;
                    if (_sheetPixelCache.has(cacheKey)) {
                        state.sheetData[si] = _sheetPixelCache.get(cacheKey);
                        continue;
                    }
                    try {
                        const mat = state.tileMaterialsPtr.add(_ARRAY_HDR + si * Process.pointerSize).readPointer();
                        if (!_isValid(mat)) continue;
                        const tex = _invokeRetPtr(m.getMainTexMI, mat);
                        if (!_isValid(tex)) continue;
                        const w = _invokeRetInt(m.widthMI, tex);
                        const h = _invokeRetInt(m.heightMI, tex);
                        if (w <= 0 || h <= 0) continue;

                        if (_invokeRetBool(m.getIsReadMI, tex)) {
                            // CPU-readable: no GPU work at all.
                            const pixels = _readPixelsBulk(tex, w, h);
                            const entry  = { pixels, w, h, tileSize: _TILE_PX };
                            state.sheetData[si] = entry;
                            _sheetPixelCache.set(cacheKey, entry);
                            continue;
                        }

                        // Blit to half-size RT (GPU queued async — NO stall here).
                        // hw×hh = (w/2)×(h/2): 4× fewer pixels to transfer later;
                        // GPU bilinear downscale is free, tiles land at _OUT_TILE_PX.
                        const hw = Math.max(1, w >> 1), hh = Math.max(1, h >> 1);
                        const ib1 = Memory.alloc(4), ib2 = Memory.alloc(4), ib3 = Memory.alloc(4);
                        ib1.writeS32(hw); ib2.writeS32(hh); ib3.writeS32(0);
                        const a3 = Memory.alloc(3 * Process.pointerSize);
                        a3.writePointer(ib1); a3.add(Process.pointerSize).writePointer(ib2); a3.add(2 * Process.pointerSize).writePointer(ib3);
                        const rt = _invokeRet(m.getTemporaryMI, ptr(0), a3);
                        if (!_isValid(rt)) continue;

                        const a2 = Memory.alloc(2 * Process.pointerSize);
                        a2.writePointer(tex); a2.add(Process.pointerSize).writePointer(rt);
                        _invokeRet(m.blitMI, ptr(0), a2);  // ← GPU command queued, does NOT stall

                        // Pre-alloc destination Texture2D (CPU only, zero GPU cost).
                        const newTex = _api.objectNew(m.tex2dKlass);
                        if (!_isValid(newTex)) {
                            if (m.releaseTemporaryMI) { const aa = Memory.alloc(Process.pointerSize); aa.writePointer(rt); _invokeRet(m.releaseTemporaryMI, ptr(0), aa); }
                            continue;
                        }
                        const ca = Memory.alloc(2 * Process.pointerSize);
                        ca.writePointer(ib1); ca.add(Process.pointerSize).writePointer(ib2);
                        _invokeRet(m.tex2dCtorMI, newTex, ca);

                        state.pendingRTs.push({ rt, newTex, hw, hh, si, tileSize: _TILE_PX >> 1 });
                    } catch (_) { /* skip failed sheet */ }
                }
                state.timing.blitMs     = Date.now() - _tBlit0;
                state.timing.blitNew     = state.pendingRTs.length;
                state.timing.blitCached  = state.neededSheets.length - state.pendingRTs.length;
                state.phase = 'read';
                return; // yield — GPU processes all queued Blits during this frame
            }

            // ── Phase 'read': ReadPixels from all RTs (1 GPU stall total) ────
            // First set_active + ReadPixels flushes the pipeline for ALL Blits.
            // Every subsequent ReadPixels is instant — GPU is already done.
            if (state.phase === 'read') {
                const m  = _resolveUnityMethods();
                const a1 = Memory.alloc(Process.pointerSize);
                const _tRead0 = Date.now();
                let   _tAfterFirst = 0;
                let   _sumSetActiveMs = 0, _sumReadPixelsMs = 0,
                      _sumGetPixels32Ms = 0, _sumReleaseMs = 0;
                for (const prt of state.pendingRTs) {
                    const { rt, newTex, hw, hh, si, tileSize } = prt;
                    try {
                        let _t;
                        _t = Date.now();
                        a1.writePointer(rt);
                        _invokeRet(m.setActiveMI, ptr(0), a1);  // 1st call → GPU flush for ALL Blits
                        _sumSetActiveMs += Date.now() - _t;

                        _t = Date.now();
                        const ib1 = Memory.alloc(4), ib2 = Memory.alloc(4);
                        ib1.writeS32(0); ib2.writeS32(0);
                        const rectBuf = Memory.alloc(16);
                        rectBuf.writeFloat(0); rectBuf.add(4).writeFloat(0);
                        rectBuf.add(8).writeFloat(hw); rectBuf.add(12).writeFloat(hh);
                        const rpa = Memory.alloc(3 * Process.pointerSize);
                        rpa.writePointer(rectBuf); rpa.add(Process.pointerSize).writePointer(ib1); rpa.add(2 * Process.pointerSize).writePointer(ib2);
                        _invokeRet(m.readPixelsMI, newTex, rpa);
                        _sumReadPixelsMs += Date.now() - _t;
                        if (!_tAfterFirst) _tAfterFirst = Date.now(); // ← marks end of GPU stall

                        _t = Date.now();
                        const pixels = _readPixelsBulk(newTex, hw, hh);
                        _sumGetPixels32Ms += Date.now() - _t;

                        const entry  = { pixels, w: hw, h: hh, tileSize };
                        state.sheetData[si] = entry;
                        _sheetPixelCache.set('' + si, entry);
                    } catch (_) { /* composite with what we have */ }
                    // Always release RT.
                    try {
                        const _t = Date.now();
                        a1.writePointer(ptr(0));
                        _invokeRet(m.setActiveMI, ptr(0), a1);
                        if (m.releaseTemporaryMI) { a1.writePointer(rt); _invokeRet(m.releaseTemporaryMI, ptr(0), a1); }
                        _sumReleaseMs += Date.now() - _t;
                    } catch (_) {}
                }
                const _tReadDone = Date.now();
                state.timing.readFirstMs     = _tAfterFirst ? _tAfterFirst - _tRead0 : 0;
                state.timing.readRestMs      = _tAfterFirst ? _tReadDone - _tAfterFirst : _tReadDone - _tRead0;
                state.timing.readCount       = state.pendingRTs.length;
                state.timing.sumSetActiveMs  = _sumSetActiveMs;
                state.timing.sumReadPixelsMs = _sumReadPixelsMs;
                state.timing.sumGP32Ms       = _sumGetPixels32Ms;
                state.timing.sumReleaseMs    = _sumReleaseMs;
                state.phase = 'build';
                return;
            }

            // ── Phase 'build': composite and send raw RGBA ────────────────────
            let _tB;
            try {
                _tB = Date.now();
                const cache  = _buildTileCache({ sheetData: state.sheetData, sheetCount: state.sheetCount, uniqueIds: state.uniqueIds });
                state.timing.buildTileMs = Date.now() - _tB;
                _tB = Date.now();
                const render = _renderMap(state.map, cache);
                state.timing.renderMs    = Date.now() - _tB;
                state.timing.buildMs     = state.timing.buildTileMs + state.timing.renderMs;
                send({
                    id: state.id, ok: true,
                    data: {
                        mapName:   state.map.mapName,
                        width:     state.map.width,
                        height:    state.map.height,
                        imgWidth:  render.imgW,
                        imgHeight: render.imgH,
                        playerX:   state.map.playerX,
                        playerY:   state.map.playerY,
                        startX:    state.map.startX,
                        startY:    state.map.startY,
                        colliders: state.map.colliders ? _toBase64(state.map.colliders) : null,
                        links:     state.map.links     ? _toBase64(state.map.links)     : null,
                        timing:    state.timing,
                    },
                }, render.pixels.buffer);
            } catch (e) {
                send({ id: state.id, ok: false, error: e.message });
            }
            _mfState = null;
        }
    });
}


// ── [6] Startup ───────────────────────────────────────────────────────────────
// Discover layout immediately on load, before entering the request loop.
// If discovery fails, C# will know via the ok:false ready signal and can
// retry (e.g. call LoadGameAsync again after the game finishes loading).

let _layout = null;

try {
    _layout = _discoverLayout();
    send({ cmd: 'ready', ok: true });
} catch (e) {
    send({ cmd: 'ready', ok: false, error: e.message });
}


// ── [7] Persistent request loop ───────────────────────────────────────────────
// recv() fires once per call; the handler re-registers itself immediately so
// no incoming request is ever missed.  C# can issue concurrent reads — each
// response carries the same `id` as the request for correlation.

function _handleRequest(req) {
    recv(_handleRequest); // re-register first, before any async work

    if (!req || typeof req !== 'object') return;

    const id = req.id;

    if (req.cmd === 'read') {
        if (!_layout) {
            send({ id, ok: false, error: 'Layout not initialized — discovery failed at startup. Check the ready message error.' });
            return;
        }
        try {
            const data = _readGameState(_layout);
            send({ id, ok: true, data });
        } catch (e) {
            send({ id, ok: false, error: e.message });
        }
    } else if (req.cmd === 'map_screenshot' || req.cmd === 'screenshot') {
        if (!_layout) {
            send({ id, ok: false, error: 'Layout not initialized.' });
            return;
        }
        const _ssMode = req.mode === 'low_res' ? 'low_res' : 'normal';
        if (_ssMode === 'low_res') {
            // Low-res fast path: no GPU work — runs entirely on the Frida message thread.
            // Reads map data from CPU memory, renders gray background + collision/link/player
            // overlays (identical to normal mode except tile sprites are omitted).
            // Typically completes in <100 ms regardless of how many tile sheets the map has.
            try {
                const mapLay = _discoverMapLayout();
                const map    = _readMapData(_layout, mapLay);
                const render = _renderMap(map, null, true);
                send({
                    id, ok: true,
                    data: {
                        mapName:   map.mapName,
                        width:     map.width,
                        height:    map.height,
                        imgWidth:  render.imgW,
                        imgHeight: render.imgH,
                        playerX:   map.playerX,
                        playerY:   map.playerY,
                        startX:    map.startX,
                        startY:    map.startY,
                        colliders: map.colliders ? _toBase64(map.colliders) : null,
                        links:     map.links     ? _toBase64(map.links)     : null,
                    },
                }, render.pixels.buffer);
            } catch (e) {
                send({ id, ok: false, error: e.message });
            }
        } else {
            // Normal mode: full GPU texture pipeline via DSSock.Update hook.
            try {
                _setupUpdateHook();
                _pendingScreenshot = { id, mode: 'normal' };
                // Response will be sent from the DSSock.Update hook on the next frame.
            } catch (e) {
                send({ id, ok: false, error: e.message });
            }
        }
    } else {
        send({ id, ok: false, error: `Unknown command: "${req.cmd}"` });
    }
}

recv(_handleRequest);
