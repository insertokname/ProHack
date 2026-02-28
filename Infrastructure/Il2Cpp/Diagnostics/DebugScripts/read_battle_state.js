// !!!VIBECODED BULLSHIT AHEAD!!!

(function () {
"use strict";

/**
 * read_battle_state.js
 *
 * Reads CurrentEncounterId, IsBattling, ShinyForm, EventForm
 * using IL2CPP named-field lookups — no hardcoded module addresses.
 *
 * Confirmed by find_battle_state.js: all 4 live in the DSSock singleton.
 *
 *   DSSock.<pjm>k__BackingField  → ds  (static singleton, found by name)
 *     ds.OtherPoke  (offset resolved by name) → CurrentEncounterId  int32
 *     ds.ply        (offset resolved by name) → IsBattling          int32 ≠ 0
 *     ds + 0x7E0    (raw offset from ds obj)  → ShinyForm           int32
 *     ds + 0x7E4    (raw offset from ds obj)  → EventForm           int32
 *
 * The two raw offsets (0x7E0, 0x7E4) are relative to the DSSock instance
 * which is itself found dynamically — far more stable than module-relative
 * pointer chains.
 */

const mod = Process.getModuleByName("GameAssembly.dll");

// ── IL2CPP bindings ───────────────────────────────────────────────────────────
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
    try { return (p && !p.isNull()) ? p.readUtf8String() : null; } catch(e) { return null; }
}
function isValid(p) {
    if (!p || p.isNull()) return false;
    const v = p.toUInt32();
    return v > 0x10000 && v !== 0xffffffff;
}

function findClass(targetName) {
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
                if (readStr(class_get_name(klass)) === targetName) return klass;
            }
        } catch(e) {}
    }
    return null;
}

// Returns { field, offset } for the named field, or null.
function getField(klass, fieldName) {
    const iter = Memory.alloc(Process.pointerSize);
    iter.writePointer(ptr(0));
    let f;
    while (!(f = class_get_fields(klass, iter)).isNull()) {
        if (readStr(field_get_name(f)) === fieldName) {
            return { field: f, offset: field_get_offset(f),
                     isStatic: (field_get_flags(f) & FIELD_ATTR_STATIC) !== 0 };
        }
    }
    return null;
}

// ── One-time setup: resolve class + field offsets ─────────────────────────────
// We cache these once — they never change within a session.
let _dsKlass          = null;
let _pjmField         = null;
let _offsetOtherPoke  = null;   // CurrentEncounterId
let _offsetPly        = null;   // IsBattling

function setup() {
    _dsKlass = findClass("DSSock");
    if (!_dsKlass) throw new Error("DSSock class not found");

    // Static singleton field
    _pjmField = getField(_dsKlass, "<pjm>k__BackingField");
    if (!_pjmField) throw new Error("DSSock.<pjm>k__BackingField not found");

    // Instance field offsets (resolved by name, confirmed by find_battle_state.js)
    const otherPoke = getField(_dsKlass, "OtherPoke");
    if (!otherPoke) throw new Error("DSSock.OtherPoke not found");
    _offsetOtherPoke = otherPoke.offset;

    const ply = getField(_dsKlass, "ply");
    if (!ply) throw new Error("DSSock.ply not found");
    _offsetPly = ply.offset;

    console.log(`[setup] DSSock class    @ ${_dsKlass}`);
    console.log(`[setup] OtherPoke offset = 0x${_offsetOtherPoke.toString(16)}  (CurrentEncounterId)`);
    console.log(`[setup] ply offset       = 0x${_offsetPly.toString(16)}  (IsBattling)`);
    console.log(`[setup] ShinyForm offset = 0x7E0  (raw from ds instance, stable)`);
    console.log(`[setup] EventForm offset = 0x7E4  (raw from ds instance, stable)\n`);
}

// ── Per-read: get the DSSock instance and read all 4 values ──────────────────
function readBattleState() {
    // Re-fetch the singleton every call — it could theoretically change on logout/login
    const buf = Memory.alloc(Process.pointerSize);
    field_static_get_value(_pjmField.field, buf);
    const ds = buf.readPointer();
    if (!isValid(ds)) throw new Error("DSSock instance is null (not logged in?)");

    const currentEncounterId = ds.add(_offsetOtherPoke).readS32();
    const isBattlingRaw      = ds.add(_offsetPly).readS32();
    const shinyForm          = ds.add(0x7E0).readS32();
    const eventForm          = ds.add(0x7E4).readS32();

    return {
        currentEncounterId,
        isBattling: isBattlingRaw !== 0,
        isBattlingRaw,
        shinyForm,
        eventForm,
        isSpecial: shinyForm !== 0 || eventForm !== 0,
    };
}

function printState(s) {
    const special = s.isSpecial
        ? `  ★ SPECIAL (shiny=${s.shinyForm} event=${s.eventForm})`
        : `  (not special)`;
    console.log(
        `encounterId=${s.currentEncounterId}  battling=${s.isBattling}` +
        `  shiny=${s.shinyForm}  event=${s.eventForm}${s.isSpecial ? "  ★ SPECIAL" : ""}`
    );
}

// ── Run ───────────────────────────────────────────────────────────────────────
try {
    setup();
} catch(e) {
    console.log(`[-] Setup failed: ${e.message}`);
    return;
}

// One-shot read
try {
    const state = readBattleState();
    console.log("[+] Battle state:");
    console.log(`    CurrentEncounterId : ${state.currentEncounterId}`);
    console.log(`    IsBattling         : ${state.isBattling}  (raw=${state.isBattlingRaw})`);
    console.log(`    ShinyForm          : ${state.shinyForm}`);
    console.log(`    EventForm          : ${state.eventForm}`);
    console.log(`    IsSpecial          : ${state.isSpecial}`);
} catch(e) {
    console.log(`[-] Read failed: ${e.message}`);
}

// Uncomment to poll every 500 ms (useful for watching battle state change live)
// setInterval(function () {
//     try { printState(readBattleState()); }
//     catch(e) { console.log("Error:", e.message); }
// }, 500);

})();
