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
};

const NAMED_FIELDS = {
    // DSSock instance fields (stable names)
    console:   'Console',
    otherPoke: 'OtherPoke',
    targetPos: 'TargetPos', // UnityEngine.Vector3 — x at +0, y at +4 (standard struct layout)
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
    throw new Error(`Field not found: "${fieldName}". Update FIELD_NAMES in the agent.`);
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

    return { selectedMenu, isBattling, currentEncounterId, shinyForm, eventForm, playerX, playerY };
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
    } else {
        send({ id, ok: false, error: `Unknown command: "${req.cmd}"` });
    }
}

recv(_handleRequest);
