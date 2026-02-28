// !!!VIBECODED BULLSHIT AHEAD!!!

/**
 * find_selected_menu.js
 *
 * BFS through the IL2CPP managed-object graph starting from the DSSock
 * singleton.  For every field of every reachable object it checks whether
 * the field's in-memory address equals the known SelectedMenu address.
 * When found it prints the full named field-path so you can hard-code it
 * as a stable, update-resistant IL2CPP accessor.
 *
 * Usage:
 *   frida -p <pid> -l find_selected_menu.js
 *
 * Target address: the CURRENT address of the SelectedMenu int variable.
 * Update this every session ONLY until you find the named path.
 */

// ─── CONFIG ──────────────────────────────────────────────────────────────────
const TARGET_ADDR = ptr("0x1B8FDA0F738");   // ← current address of SelectedMenu
const MAX_DEPTH   = 64;                       // max pointer hops from root
const MAX_OBJECTS = 60_000;                  // safety cap on visited set

// Types to NOT recurse into (they bloat the graph or are dead-ends)
const SKIP_CLASS_PREFIXES = [
    "UILabel", "UISprite", "UIButton", "UIWidget", "UIPanel",
    "UIScrollView", "UIGrid", "UIScrollBar", "UIInput", "UIToggle",
    "UISlider", "UIPopupList", "UITextList", "UITexture", "UIProgressBar",
    "UICamera", "UIDrawCall", "UIAtlas", "UIFont",
    "TweenAlpha", "TweenColor", "TweenPosition", "TweenScale",
    "UnityEngine.", "System.Collections.", "System.Threading.",
    "Cysharp.", "HUDText", "FadeTextAfter",
];
// ─────────────────────────────────────────────────────────────────────────────

(function () {

const mod = Process.getModuleByName("GameAssembly.dll");

// ── IL2CPP API bindings ───────────────────────────────────────────────────────
const domain_get            = new NativeFunction(mod.getExportByName("il2cpp_domain_get"),              "pointer", []);
const domain_get_assemblies = new NativeFunction(mod.getExportByName("il2cpp_domain_get_assemblies"),   "pointer", ["pointer","pointer"]);
const assembly_get_image    = new NativeFunction(mod.getExportByName("il2cpp_assembly_get_image"),      "pointer", ["pointer"]);
const image_get_class_count = new NativeFunction(mod.getExportByName("il2cpp_image_get_class_count"),   "uint",    ["pointer"]);
const image_get_class       = new NativeFunction(mod.getExportByName("il2cpp_image_get_class"),         "pointer", ["pointer","uint"]);
const class_get_name        = new NativeFunction(mod.getExportByName("il2cpp_class_get_name"),          "pointer", ["pointer"]);
const class_get_fields      = new NativeFunction(mod.getExportByName("il2cpp_class_get_fields"),        "pointer", ["pointer","pointer"]);
const class_get_parent      = new NativeFunction(mod.getExportByName("il2cpp_class_get_parent"),        "pointer", ["pointer"]);
const field_get_name        = new NativeFunction(mod.getExportByName("il2cpp_field_get_name"),          "pointer", ["pointer"]);
const field_get_offset      = new NativeFunction(mod.getExportByName("il2cpp_field_get_offset"),        "uint",    ["pointer"]);
const field_get_type        = new NativeFunction(mod.getExportByName("il2cpp_field_get_type"),          "pointer", ["pointer"]);
const field_get_flags       = new NativeFunction(mod.getExportByName("il2cpp_field_get_flags"),         "int",     ["pointer"]);
const field_static_get_value= new NativeFunction(mod.getExportByName("il2cpp_field_static_get_value"),  "void",    ["pointer","pointer"]);
const object_get_class      = new NativeFunction(mod.getExportByName("il2cpp_object_get_class"),        "pointer", ["pointer"]);
const type_get_type         = new NativeFunction(mod.getExportByName("il2cpp_type_get_type"),           "int",     ["pointer"]);
// ─────────────────────────────────────────────────────────────────────────────

// IL2CPP type-code constants (from mono/metadata/blob.h)
const TC_STRING      = 0x0e;
const TC_CLASS       = 0x12;
const TC_ARRAY       = 0x14;
const TC_GENERICINST = 0x15;
const TC_OBJECT      = 0x1c;
const TC_SZARRAY     = 0x1d;
const FIELD_ATTR_STATIC = 0x10;

function readStr(p) {
    try { return (p && !p.isNull()) ? p.readUtf8String() : null; } catch(e) { return null; }
}

function isValid(p) {
    if (!p || p.isNull()) return false;
    const v = p.toUInt32();
    return v > 0x10000 && v !== 0xffffffff;
}

function shouldSkip(className) {
    if (!className) return true;
    for (const pfx of SKIP_CLASS_PREFIXES)
        if (className.startsWith(pfx)) return true;
    return false;
}

// Returns all INSTANCE fields of a class (walks parent chain).
// Each entry: { name, offset, typeCode }
function getInstanceFields(klass) {
    const result = [];
    const seenFields = new Set();
    let cur = klass;
    for (let depth = 0; depth < 20 && isValid(cur); depth++) {
        const iter = Memory.alloc(Process.pointerSize);
        iter.writePointer(ptr(0));
        let f;
        try {
            while (!(f = class_get_fields(cur, iter)).isNull()) {
                const fkey = f.toString();
                if (seenFields.has(fkey)) continue;
                seenFields.add(fkey);
                // skip static fields
                try { if (field_get_flags(f) & FIELD_ATTR_STATIC) continue; } catch(e) {}
                const name   = readStr(field_get_name(f)) || "?";
                const offset = field_get_offset(f);
                let typeCode = -1;
                try { typeCode = type_get_type(field_get_type(f)); } catch(e) {}
                result.push({ name, offset, typeCode });
            }
        } catch(e) {}
        try { cur = class_get_parent(cur); } catch(e) { break; }
    }
    return result;
}

// ── STEP 1 – Find DSSock class ────────────────────────────────────────────────
const domain    = domain_get();
const cntBuf    = Memory.alloc(4);
const asmPtrs   = domain_get_assemblies(domain, cntBuf);
const asmCount  = cntBuf.readU32();
let dsKlass     = null;

outer:
for (let a = 0; a < asmCount; a++) {
    const asm = asmPtrs.add(a * Process.pointerSize).readPointer();
    const img = assembly_get_image(asm);
    if (!isValid(img)) continue;
    const cc = image_get_class_count(img);
    for (let c = 0; c < cc; c++) {
        const klass = image_get_class(img, c);
        if (!isValid(klass)) continue;
        if (readStr(class_get_name(klass)) === "DSSock") { dsKlass = klass; break outer; }
    }
}

if (!dsKlass) { console.log("[-] DSSock class not found"); return; }

// ── STEP 2 – Get DSSock singleton via <pjm>k__BackingField ───────────────────
let dsInstance = null;
{
    const iter = Memory.alloc(Process.pointerSize);
    iter.writePointer(ptr(0));
    let f;
    while (!(f = class_get_fields(dsKlass, iter)).isNull()) {
        if (readStr(field_get_name(f)) === "<pjm>k__BackingField") {
            const buf = Memory.alloc(Process.pointerSize);
            field_static_get_value(f, buf);
            dsInstance = buf.readPointer();
            break;
        }
    }
}

if (!isValid(dsInstance)) { console.log("[-] DSSock singleton is null – not logged in?"); return; }
console.log("[*] DSSock singleton  @ " + dsInstance);
console.log("[*] Searching for     @ " + TARGET_ADDR + "  (SelectedMenu int)");
console.log("[*] BFS max depth=" + MAX_DEPTH + "  object cap=" + MAX_OBJECTS + "\n");

// ── STEP 3 – BFS ──────────────────────────────────────────────────────────────
const visited  = new Set();
const queue    = [];
const findings = [];

queue.push({ obj: dsInstance, path: "ds", depth: 0 });
visited.add(dsInstance.toString());

while (queue.length > 0 && visited.size < MAX_OBJECTS) {
    const { obj, path, depth } = queue.shift();
    if (depth > MAX_DEPTH) continue;

    // Get runtime class (actual type of this object)
    let klass;
    try { klass = object_get_class(obj); if (!isValid(klass)) continue; } catch(e) { continue; }

    const className = readStr(class_get_name(klass)) || "?";
    const skip      = shouldSkip(className);

    // Enumerate all instance fields
    const fields = getInstanceFields(klass);

    for (const { name, offset, typeCode } of fields) {
        try {
            const fieldAddr = obj.add(offset);

            // ★ Exact match on the target address ──────────────────────────────
            if (fieldAddr.equals(TARGET_ADDR)) {
                let val = null;
                try { val = fieldAddr.readS32(); } catch(e) {}
                const msg = `[★ FOUND] ${path}.${name}  (class ${className})  offset=0x${offset.toString(16)}  addr=${fieldAddr}  value=${val}`;
                console.log(msg);
                findings.push({ path: `${path}.${name}`, value: val, offset });
            }

            // Also flag if target is within 8 bytes of this field (struct embed)
            const diff = parseInt(TARGET_ADDR.sub(fieldAddr).toString());
            if (diff > 0 && diff < 8) {
                console.log(`[~near +${diff}] ${path}.${name}  addr=${fieldAddr}`);
            }

            // Recurse into reference-type fields (if not a skipped class)
            if (!skip) {
                const isRef = typeCode === TC_CLASS || typeCode === TC_OBJECT ||
                              typeCode === TC_STRING || typeCode === TC_ARRAY  ||
                              typeCode === TC_SZARRAY || typeCode === TC_GENERICINST;
                if (isRef) {
                    let nextPtr;
                    try { nextPtr = fieldAddr.readPointer(); } catch(e) { continue; }
                    if (!isValid(nextPtr)) continue;
                    if (visited.has(nextPtr.toString())) continue;
                    visited.add(nextPtr.toString());
                    queue.push({ obj: nextPtr, path: `${path}.${name}`, depth: depth + 1 });
                }
            }
        } catch(e) {}
    }
}

// ── SUMMARY ───────────────────────────────────────────────────────────────────
console.log(`\n[*] BFS complete.  Visited ${visited.size} objects.  Queue remaining: ${queue.length}`);

if (findings.length === 0) {
    console.log("[-] Target address NOT found in BFS traversal.");
    console.log("    Possible reasons:");
    console.log("      1. The target is deeper than MAX_DEPTH=" + MAX_DEPTH);
    console.log("      2. It is reachable only through a skipped type (UITextList, etc.)");
    console.log("      3. It is not reachable from the DSSock singleton at all.");
    console.log("\n    Try increasing MAX_DEPTH, or remove UITextList from SKIP_CLASS_PREFIXES,");
    console.log("    then re-run.  The old chain passed through DSSock.Console.TextList,");
    console.log("    so UITextList is a likely bridge even though it is a UI class.");
} else {
    console.log(`\n[+] ${findings.length} path(s) found!`);
    for (const f of findings) {
        console.log(`\n  Path : ${f.path}`);
        console.log(`  Value: ${f.value}  (0=outside fight | 41=fight | 42=can't interact | 46=items | 55=pokemon)`);
        console.log(`\n  You can now access this stably via IL2CPP field names:`);
        console.log(`    ${f.path.split('.').map((s,i) => i===0 ? s : `.${s}`).join('')}`);
    }
}

})();
