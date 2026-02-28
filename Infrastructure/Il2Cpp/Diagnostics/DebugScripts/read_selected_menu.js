// !!!VIBECODED BULLSHIT AHEAD!!!

(function () {
"use strict";

/**
 * read_selected_menu.js
 *
 * Reads the SelectedMenu int using IL2CPP named-field lookups wherever
 * possible, so it survives game updates.
 *
 * Resolved chain (all names, no raw module addresses):
 *
 *  DSSock.<pjm>k__BackingField   ← static field lookup by name
 *    → DSSock instance
 *       └─ .Console              ← "Console" field in DSSock (offset found by name)
 *             └─ ChatInput instance
 *                  └─ .TextList  ← "TextList" field in ChatInput (offset found by name)
 *                        └─ UITextList instance
 *                             +0xB0 → (UIWidget.onChange delegate / NGUI internal)
 *                                  +0xA8 → int  ← SelectedMenu value
 *
 * The only hardcoded offsets are the last two (0xB0, 0xA8) which are deep
 * inside Unity NGUI internals and are extremely stable across PRO updates.
 *
 * SelectedMenu values (from Domain/SelectedMenu.cs):
 *   0  = outside of fight
 *   41 = fight menu  /  no menu selected
 *   42 = can't interact
 *   46 = items menu
 *   55 = pokemon menu
 */

const mod = Process.getModuleByName("GameAssembly.dll");

// ── IL2CPP bindings ───────────────────────────────────────────────────────────
const domain_get              = new NativeFunction(mod.getExportByName("il2cpp_domain_get"),              "pointer", []);
const domain_get_assemblies   = new NativeFunction(mod.getExportByName("il2cpp_domain_get_assemblies"),   "pointer", ["pointer","pointer"]);
const assembly_get_image      = new NativeFunction(mod.getExportByName("il2cpp_assembly_get_image"),      "pointer", ["pointer"]);
const image_get_class_count   = new NativeFunction(mod.getExportByName("il2cpp_image_get_class_count"),   "uint",    ["pointer"]);
const image_get_class         = new NativeFunction(mod.getExportByName("il2cpp_image_get_class"),         "pointer", ["pointer","uint"]);
const class_get_name          = new NativeFunction(mod.getExportByName("il2cpp_class_get_name"),          "pointer", ["pointer"]);
const class_get_fields        = new NativeFunction(mod.getExportByName("il2cpp_class_get_fields"),        "pointer", ["pointer","pointer"]);
const field_get_name          = new NativeFunction(mod.getExportByName("il2cpp_field_get_name"),          "pointer", ["pointer"]);
const field_get_offset        = new NativeFunction(mod.getExportByName("il2cpp_field_get_offset"),        "uint",    ["pointer"]);
const field_get_flags         = new NativeFunction(mod.getExportByName("il2cpp_field_get_flags"),         "int",     ["pointer"]);
const field_static_get_value  = new NativeFunction(mod.getExportByName("il2cpp_field_static_get_value"),  "void",    ["pointer","pointer"]);

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

/**
 * Find a class by simple name across all assemblies.
 * Returns the Il2CppClass* or null.
 */
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

/**
 * Walk the field list of a class (does NOT follow parent chain — offsets in
 * classes.TXT are already absolute within the full object).
 * Returns { offset, isStatic } for the first field matching fieldName,
 * or null if not found.
 */
function getField(klass, fieldName) {
    const iter = Memory.alloc(Process.pointerSize);
    iter.writePointer(ptr(0));
    let f;
    while (!(f = class_get_fields(klass, iter)).isNull()) {
        if (readStr(field_get_name(f)) === fieldName) {
            const offset   = field_get_offset(f);
            const isStatic = (field_get_flags(f) & FIELD_ATTR_STATIC) !== 0;
            return { field: f, offset, isStatic };
        }
    }
    return null;
}

// ─────────────────────────────────────────────────────────────────────────────
// readSelectedMenu()
// Returns the current SelectedMenu integer, or null on failure.
// ─────────────────────────────────────────────────────────────────────────────
function readSelectedMenu() {

    // ── Step 1: DSSock class ──────────────────────────────────────────────────
    const dsKlass = findClass("DSSock");
    if (!dsKlass) throw new Error("DSSock class not found");

    // ── Step 2: DSSock.<pjm>k__BackingField → singleton instance ─────────────
    const pjmField = getField(dsKlass, "<pjm>k__BackingField");
    if (!pjmField) throw new Error("DSSock.<pjm>k__BackingField not found");
    const buf = Memory.alloc(Process.pointerSize);
    field_static_get_value(pjmField.field, buf);
    const dsInstance = buf.readPointer();
    if (!isValid(dsInstance)) throw new Error("DSSock instance is null (not logged in?)");

    // ── Step 3: DSSock.Console → ChatInput instance ───────────────────────────
    const consoleFieldInfo = getField(dsKlass, "Console");
    if (!consoleFieldInfo) throw new Error("DSSock.Console field not found");
    const chatInputPtr = dsInstance.add(consoleFieldInfo.offset).readPointer();
    if (!isValid(chatInputPtr)) throw new Error("DSSock.Console is null");

    // ── Step 4: ChatInput.TextList → UITextList instance ─────────────────────
    const chatInputKlass = findClass("ChatInput");
    if (!chatInputKlass) throw new Error("ChatInput class not found");
    const textListFieldInfo = getField(chatInputKlass, "TextList");
    if (!textListFieldInfo) throw new Error("ChatInput.TextList field not found");
    const textListPtr = chatInputPtr.add(textListFieldInfo.offset).readPointer();
    if (!isValid(textListPtr)) throw new Error("ChatInput.TextList is null");

    // ── Step 5: UITextList +0xB0 → UIWidget.onChange delegate (NGUI internal) ─
    // This is UIWidget.onChange (a delegate stored at offset 0xB0 in the object).
    // UIWidget.onChange offset is confirmed by classes.TXT – very stable NGUI field.
    const onChange = textListPtr.add(0xB0).readPointer();
    if (!isValid(onChange)) throw new Error("UITextList+0xB0 is null");

    // ── Step 6: delegate +0xA8 → SelectedMenu int ────────────────────────────
    const selectedMenu = onChange.add(0xA8).readS32();
    return selectedMenu;
}

// ─────────────────────────────────────────────────────────────────────────────
// Interpret the raw int into a human-readable state
// ─────────────────────────────────────────────────────────────────────────────
function interpretMenu(val) {
    switch (val) {
        case  0: return "OutsideOfFight";
        case 41: return "FightOrNoneMenu";
        case 42: return "CantInteract";
        case 46: return "ItemsMenu";
        case 47: return "FightSuperBoss";
        case 52: return "ItemSuperBoss";
        case 55: return "PokemonMenu";
        case 61: return "PokemonSuperBoss";
        default: return `Unknown(${val})`;
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Run once and print
// ─────────────────────────────────────────────────────────────────────────────
try {
    const val = readSelectedMenu();
    console.log(`[+] SelectedMenu = ${val}  →  ${interpretMenu(val)}`);
} catch (e) {
    console.log(`[-] Error: ${e.message}`);
}

// ─────────────────────────────────────────────────────────────────────────────
// Poll every second (comment out if you only want a one-shot read)
// ─────────────────────────────────────────────────────────────────────────────
// setInterval(function () {
//     try {
//         const val = readSelectedMenu();
//         console.log(`SelectedMenu = ${val}  (${interpretMenu(val)})`);
//     } catch (e) {
//         console.log("Error:", e.message);
//     }
// }, 1000);

})();
