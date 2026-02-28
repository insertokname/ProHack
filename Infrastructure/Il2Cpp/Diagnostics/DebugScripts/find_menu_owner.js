// !!!VIBECODED BULLSHIT AHEAD!!!

(function () {
"use strict";

/**
 * find_menu_owner.js
 *
 * Three-strategy search to find the SelectedMenu int:
 *
 *  A) Direct static field scan – is TARGET_ADDR exactly a static field address?
 *  B) Backward heap scan – which managed object's instance field lands at TARGET_ADDR?
 *  C) Path search – which static singleton transitively holds the owning object?
 *  D) Old RVA chain diagnostic – follow the old pointer chain to see what it hits now.
 */

const TARGET_ADDR = ptr("0x1B8FDA0F738");   // ← update each session until path is found
const MOD         = Process.getModuleByName("GameAssembly.dll");

// ── IL2CPP API bindings ───────────────────────────────────────────────────────
const domain_get              = new NativeFunction(MOD.getExportByName("il2cpp_domain_get"),              "pointer", []);
const domain_get_assemblies   = new NativeFunction(MOD.getExportByName("il2cpp_domain_get_assemblies"),   "pointer", ["pointer", "pointer"]);
const assembly_get_image      = new NativeFunction(MOD.getExportByName("il2cpp_assembly_get_image"),      "pointer", ["pointer"]);
const image_get_class_count   = new NativeFunction(MOD.getExportByName("il2cpp_image_get_class_count"),   "uint",    ["pointer"]);
const image_get_class         = new NativeFunction(MOD.getExportByName("il2cpp_image_get_class"),         "pointer", ["pointer", "uint"]);
const class_get_name          = new NativeFunction(MOD.getExportByName("il2cpp_class_get_name"),          "pointer", ["pointer"]);
const class_get_namespace     = new NativeFunction(MOD.getExportByName("il2cpp_class_get_namespace"),     "pointer", ["pointer"]);
const class_get_fields        = new NativeFunction(MOD.getExportByName("il2cpp_class_get_fields"),        "pointer", ["pointer", "pointer"]);
const class_get_parent        = new NativeFunction(MOD.getExportByName("il2cpp_class_get_parent"),        "pointer", ["pointer"]);
const class_get_static_field_data = new NativeFunction(MOD.getExportByName("il2cpp_class_get_static_field_data"), "pointer", ["pointer"]);
const field_get_name          = new NativeFunction(MOD.getExportByName("il2cpp_field_get_name"),          "pointer", ["pointer"]);
const field_get_offset        = new NativeFunction(MOD.getExportByName("il2cpp_field_get_offset"),        "uint",    ["pointer"]);
const field_get_type          = new NativeFunction(MOD.getExportByName("il2cpp_field_get_type"),          "pointer", ["pointer"]);
const field_get_flags         = new NativeFunction(MOD.getExportByName("il2cpp_field_get_flags"),         "int",     ["pointer"]);
const field_static_get_value  = new NativeFunction(MOD.getExportByName("il2cpp_field_static_get_value"),  "void",    ["pointer", "pointer"]);
const object_get_class        = new NativeFunction(MOD.getExportByName("il2cpp_object_get_class"),        "pointer", ["pointer"]);
const type_get_type           = new NativeFunction(MOD.getExportByName("il2cpp_type_get_type"),           "int",     ["pointer"]);

const FIELD_ATTR_STATIC = 0x10;
// IL2CPP reference type codes (types whose field values are heap pointers)
const REF_TYPES = new Set([0x0e /*STRING*/, 0x12 /*CLASS*/, 0x14 /*ARRAY*/, 0x15 /*GENERICINST*/, 0x1c /*OBJECT*/, 0x1d /*SZARRAY*/]);

function readStr(p) {
    try { return (p && !p.isNull()) ? p.readUtf8String() : null; } catch (e) { return null; }
}
function isValid(p) {
    if (!p || p.isNull()) return false;
    const v = p.toUInt32();
    return v > 0x10000 && v !== 0xffffffff;
}
function className(klass) {
    try {
        const ns = readStr(class_get_namespace(klass)) || "";
        const nm = readStr(class_get_name(klass)) || "?";
        return ns ? `${ns}.${nm}` : nm;
    } catch (e) { return null; }
}

// Returns all fields (both static and instance) of a class, walking the parent chain.
function getAllFields(klass) {
    const result = [];
    const seen   = new Set();
    let cur      = klass;
    for (let d = 0; d < 20 && isValid(cur); d++) {
        const iter = Memory.alloc(Process.pointerSize);
        iter.writePointer(ptr(0));
        let f;
        try {
            while (!(f = class_get_fields(cur, iter)).isNull()) {
                const key = f.toString();
                if (seen.has(key)) continue;
                seen.add(key);
                let flags = 0;
                try { flags = field_get_flags(f); } catch (e) {}
                const isStatic = (flags & FIELD_ATTR_STATIC) !== 0;
                const name     = readStr(field_get_name(f)) || "?";
                const offset   = field_get_offset(f);
                let typeCode   = -1;
                try { typeCode = type_get_type(field_get_type(f)); } catch (e) {}
                result.push({ name, offset, isStatic, typeCode });
            }
        } catch (e) {}
        try { cur = class_get_parent(cur); } catch (e) { break; }
    }
    return result;
}

// ── Enumerate ALL classes across all assemblies ───────────────────────────────
const domain   = domain_get();
const cntBuf   = Memory.alloc(4);
const asmPtrs  = domain_get_assemblies(domain, cntBuf);
const asmCount = cntBuf.readU32();
const allClasses = [];

for (let a = 0; a < asmCount; a++) {
    try {
        const asm = asmPtrs.add(a * Process.pointerSize).readPointer();
        const img = assembly_get_image(asm);
        if (!isValid(img)) continue;
        const cc = image_get_class_count(img);
        for (let c = 0; c < cc; c++) {
            try {
                const klass = image_get_class(img, c);
                if (isValid(klass)) allClasses.push(klass);
            } catch (e) {}
        }
    } catch (e) {}
}
console.log(`[*] Loaded ${allClasses.length} IL2CPP classes`);

// Build a Set of all known Il2CppClass* addresses for fast O(1) validation.
// A valid managed heap object has its Il2CppClass* pointer at offset 0.
const knownKlassSet = new Set();
const knownKlassMap = new Map(); // ptr string → class name
for (const klass of allClasses) {
    const ks = klass.toString();
    knownKlassSet.add(ks);
    try {
        const n = className(klass);
        if (n) knownKlassMap.set(ks, { name: n, klass });
    } catch (e) {}
}
console.log(`[*] Class pointer set built (${knownKlassSet.size} entries)`);
console.log(`[*] Target address: ${TARGET_ADDR}  (value there: ${safeReadInt(TARGET_ADDR)})\n`);

function safeReadInt(p) {
    try { return p.readS32(); } catch (e) { return "?"; }
}

// ═══════════════════════════════════════════════════════════════════════════════
// STRATEGY A: Is TARGET_ADDR directly a static field of any class?
// ═══════════════════════════════════════════════════════════════════════════════
console.log("═══ [A] Direct static field scan ═══");
let foundA = false;
for (const klass of allClasses) {
    try {
        const staticData = class_get_static_field_data(klass);
        if (!isValid(staticData)) continue;
        const fields = getAllFields(klass);
        for (const { name, offset, isStatic } of fields) {
            if (!isStatic) continue;
            try {
                const fAddr = staticData.add(offset);
                if (fAddr.equals(TARGET_ADDR)) {
                    const cn = className(klass) || "?";
                    console.log(`[★ A-HIT] ${cn}.${name}`);
                    console.log(`          staticData=${staticData}  offset=0x${offset.toString(16)}`);
                    console.log(`          value=${safeReadInt(fAddr)}`);
                    foundA = true;
                }
            } catch (e) {}
        }
    } catch (e) {}
}
if (!foundA) console.log("  → Not a direct static field.\n");

// ═══════════════════════════════════════════════════════════════════════════════
// STRATEGY B: Backward heap scan to find owning managed object
//
// IL2CPP managed objects: [Il2CppClass* klass | Il2CppObject* monitor | fields…]
// Scan backwards from TARGET_ADDR, check if candidate[0] is a known class ptr.
// ═══════════════════════════════════════════════════════════════════════════════
console.log("═══ [B] Backward heap scan for owning managed object ═══");

let ownerObj   = null;
let ownerKlass = null;
let ownerField = null;

// Scan up to 2 MB back, 8 bytes at a time (pointer alignment on 64-bit)
const SCAN_BYTES = 0x200000;
const ALIGN      = 8;

scanLoop:
for (let back = ALIGN; back < SCAN_BYTES; back += ALIGN) {
    const candidate = TARGET_ADDR.sub(back);
    try {
        const maybeKlass = candidate.readPointer();
        if (!isValid(maybeKlass)) continue;
        const ks = maybeKlass.toString();
        if (!knownKlassSet.has(ks)) continue;

        // We have a valid class pointer at candidate[0] → this is likely an object.
        const { name: cn, klass } = knownKlassMap.get(ks);
        const fields = getAllFields(klass);

        // Exact field offset match
        for (const { name, offset, isStatic } of fields) {
            if (isStatic) continue;
            if (offset === back) {
                ownerObj   = candidate;
                ownerKlass = klass;
                ownerField = name;
                console.log(`[★ B-HIT] Owning object @ ${candidate}`);
                console.log(`          Class : ${cn}`);
                console.log(`          Field : ${name}  offset=0x${back.toString(16)}`);
                console.log(`          Value : ${safeReadInt(TARGET_ADDR)}`);
                break scanLoop;
            }
        }

        // Near match (struct embedding) — report but keep scanning for exact
        for (const { name, offset, isStatic } of fields) {
            if (isStatic) continue;
            const diff = back - offset;
            if (diff >= 0 && diff < 32) {
                console.log(`[~ B-NEAR] Object @ ${candidate} (${cn})`);
                console.log(`           Closest field: ${name}  offset=0x${offset.toString(16)}`);
                console.log(`           Target is ${diff} bytes inside that field (likely struct)`);
            }
        }
    } catch (e) {}
}

if (!ownerObj) {
    console.log("  → No exact owning managed heap object found.");
    console.log("  → May be inside a static data block or native C++ memory.\n");
} else {
    // ═══════════════════════════════════════════════════════════════════════════
    // STRATEGY C: Find which static singleton holds (or transitively holds) the owner
    // ═══════════════════════════════════════════════════════════════════════════
    console.log(`\n═══ [C] Searching statics → owner (${ownerObj}) ═══`);
    const ownerStr = ownerObj.toString();
    let foundC = false;

    for (const klass of allClasses) {
        try {
            const staticData = class_get_static_field_data(klass);
            if (!isValid(staticData)) continue;
            const cn     = className(klass) || "?";
            const fields = getAllFields(klass);

            for (const { name: fname, offset, isStatic, typeCode } of fields) {
                if (!isStatic) continue;
                let sval;
                try { sval = staticData.add(offset).readPointer(); } catch (e) { continue; }
                if (!isValid(sval)) continue;

                // ── Hop 0: static directly IS the owner ──────────────────────
                if (sval.toString() === ownerStr) {
                    console.log(`[★ C-HOP0] ${cn}.${fname}  → owner`);
                    console.log(`           Frida accessor: ${cn}.${fname}.${ownerField}`);
                    foundC = true;
                    continue;
                }

                // ── Hop 1: static → obj → owner ──────────────────────────────
                if (!REF_TYPES.has(typeCode)) continue;
                let k1;
                try { k1 = object_get_class(sval); if (!isValid(k1)) continue; } catch (e) { continue; }
                const cn1    = className(k1) || "?";
                const fields1 = getAllFields(k1);

                for (const { name: f1, offset: o1, isStatic: s1, typeCode: tc1 } of fields1) {
                    if (s1) continue;
                    let v1;
                    try { v1 = sval.add(o1).readPointer(); } catch (e) { continue; }
                    if (!isValid(v1)) continue;

                    if (v1.toString() === ownerStr) {
                        console.log(`[★ C-HOP1] ${cn}.${fname}.${f1}  → owner`);
                        console.log(`           Frida accessor: ${cn}.${fname} → ${cn1}.${f1}.${ownerField}`);
                        foundC = true;
                        continue;
                    }

                    // ── Hop 2: static → obj → obj → owner ────────────────────
                    if (!REF_TYPES.has(tc1)) continue;
                    let k2;
                    try { k2 = object_get_class(v1); if (!isValid(k2)) continue; } catch (e) { continue; }
                    const cn2    = className(k2) || "?";
                    const fields2 = getAllFields(k2);

                    for (const { name: f2, offset: o2, isStatic: s2 } of fields2) {
                        if (s2) continue;
                        let v2;
                        try { v2 = v1.add(o2).readPointer(); } catch (e) { continue; }
                        if (!isValid(v2)) continue;

                        if (v2.toString() === ownerStr) {
                            console.log(`[★ C-HOP2] ${cn}.${fname} → ${cn1}.${f1} → ${cn2}.${f2}  → owner`);
                            console.log(`           Final: ...${f2}.${ownerField}`);
                            foundC = true;
                        }
                    }
                }
            }
        } catch (e) {}
    }
    if (!foundC) {
        console.log("  → Owner not found within 2 hops of any static.");
        console.log("  → Re-run with DSSock singleton added as an extra search root.");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// STRATEGY D: Follow old RVA chain to see where it leads NOW
// ═══════════════════════════════════════════════════════════════════════════════
console.log("\n═══ [D] Old RVA chain diagnostic ═══");
try {
    const oldOffsets = [0xB8, 0x0, 0x458, 0x30, 0xB0];
    let p = MOD.base.add(0x0156D958).readPointer();
    console.log(`  [0] module+0x0156D958 → ${p}`);
    tryPrintClass(p);

    for (let i = 0; i < oldOffsets.length; i++) {
        const off  = oldOffsets[i];
        let deref;
        try { deref = p.add(off).readPointer(); } catch (e) {
            console.log(`  [${i + 1}] +0x${off.toString(16)} → <unreadable>`);
            p = null; break;
        }
        console.log(`  [${i + 1}] +0x${off.toString(16)} → ${deref}`);
        tryPrintClass(deref);
        p = deref;
    }
    if (p) {
        const finalAddr = p.add(0xA8);
        const finalVal  = safeReadInt(finalAddr);
        console.log(`  [6] +0xA8 (int) = ${finalVal}  @ ${finalAddr}`);
        if (finalAddr.equals(TARGET_ADDR)) {
            console.log("  ✓ Old chain STILL leads to TARGET_ADDR!");
        } else {
            const delta = finalAddr.sub(TARGET_ADDR);
            console.log(`  ✗ Old chain leads to ${finalAddr}  (≠ target ${TARGET_ADDR}  delta=${delta})`);
        }
    }
} catch (e) {
    console.log("  Chain error:", e.message);
}

function tryPrintClass(p) {
    try {
        const ks = p.toString();
        if (knownKlassMap.has(ks)) {
            console.log(`       [vtable IS class: ${knownKlassMap.get(ks).name}]`);
            return;
        }
        const maybeKlass = p.readPointer();
        if (knownKlassMap.has(maybeKlass.toString())) {
            console.log(`       [managed obj of class: ${knownKlassMap.get(maybeKlass.toString()).name}]`);
        }
    } catch (e) {}
}

console.log("\n[*] Done.");
})();
