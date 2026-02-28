// !!!VIBECODED BULLSHIT AHEAD!!!

(function () {
"use strict";

/**
 * find_battle_state.js
 *
 * Diagnostic tool — follows the 4 old raw pointer chains for:
 *   CurrentEncounterId, IsBattling, ShinyForm, EventForm
 *
 * For each chain it:
 *   A) Follows the old offsets step-by-step and prints the class type at each hop
 *   B) Does a backward heap scan from the final address to identify the owning
 *      managed object's class + field name
 *   C) Searches all static singletons for what directly holds the owning object
 *
 * Run, paste the output, and we'll build read_battle_state.js from the named paths.
 */

const MOD = Process.getModuleByName("GameAssembly.dll");

// ── IL2CPP bindings ───────────────────────────────────────────────────────────
const domain_get              = new NativeFunction(MOD.getExportByName("il2cpp_domain_get"),              "pointer", []);
const domain_get_assemblies   = new NativeFunction(MOD.getExportByName("il2cpp_domain_get_assemblies"),   "pointer", ["pointer","pointer"]);
const assembly_get_image      = new NativeFunction(MOD.getExportByName("il2cpp_assembly_get_image"),      "pointer", ["pointer"]);
const image_get_class_count   = new NativeFunction(MOD.getExportByName("il2cpp_image_get_class_count"),   "uint",    ["pointer"]);
const image_get_class         = new NativeFunction(MOD.getExportByName("il2cpp_image_get_class"),         "pointer", ["pointer","uint"]);
const class_get_name          = new NativeFunction(MOD.getExportByName("il2cpp_class_get_name"),          "pointer", ["pointer"]);
const class_get_namespace     = new NativeFunction(MOD.getExportByName("il2cpp_class_get_namespace"),     "pointer", ["pointer"]);
const class_get_fields        = new NativeFunction(MOD.getExportByName("il2cpp_class_get_fields"),        "pointer", ["pointer","pointer"]);
const class_get_parent        = new NativeFunction(MOD.getExportByName("il2cpp_class_get_parent"),        "pointer", ["pointer"]);
const class_get_static_field_data = new NativeFunction(MOD.getExportByName("il2cpp_class_get_static_field_data"), "pointer", ["pointer"]);
const field_get_name          = new NativeFunction(MOD.getExportByName("il2cpp_field_get_name"),          "pointer", ["pointer"]);
const field_get_offset        = new NativeFunction(MOD.getExportByName("il2cpp_field_get_offset"),        "uint",    ["pointer"]);
const field_get_flags         = new NativeFunction(MOD.getExportByName("il2cpp_field_get_flags"),         "int",     ["pointer"]);
const field_static_get_value  = new NativeFunction(MOD.getExportByName("il2cpp_field_static_get_value"),  "void",    ["pointer","pointer"]);
const object_get_class        = new NativeFunction(MOD.getExportByName("il2cpp_object_get_class"),        "pointer", ["pointer"]);
const type_get_type           = new NativeFunction(MOD.getExportByName("il2cpp_type_get_type"),           "pointer", ["pointer"]);
const field_get_type          = new NativeFunction(MOD.getExportByName("il2cpp_field_get_type"),          "pointer", ["pointer"]);

const FIELD_ATTR_STATIC = 0x10;
const REF_TYPES = new Set([0x0e, 0x12, 0x14, 0x15, 0x1c, 0x1d]);

function readStr(p)  { try { return (p && !p.isNull()) ? p.readUtf8String() : null; } catch(e) { return null; } }
function isValid(p)  { if (!p || p.isNull()) return false; const v = p.toUInt32(); return v > 0x10000 && v !== 0xffffffff; }
function safeReadS32(p) { try { return p.readS32(); } catch(e) { return null; } }
function safeReadS64(p) { try { return p.readS64().toNumber(); } catch(e) { return null; } }
function safeReadPtr(p) { try { return p.readPointer(); } catch(e) { return null; } }

function fullClassName(klass) {
    try {
        const ns = readStr(class_get_namespace(klass)) || "";
        const nm = readStr(class_get_name(klass))      || "?";
        return ns ? `${ns}.${nm}` : nm;
    } catch(e) { return null; }
}

// ── Build class registry ──────────────────────────────────────────────────────
const domain   = domain_get();
const cntBuf   = Memory.alloc(4);
const asmPtrs  = domain_get_assemblies(domain, cntBuf);
const asmCount = cntBuf.readU32();

const allClasses  = [];
const klassSet    = new Set();   // ptr string → present
const klassMap    = new Map();   // ptr string → { name, klass }

for (let a = 0; a < asmCount; a++) {
    try {
        const asm = asmPtrs.add(a * Process.pointerSize).readPointer();
        const img = assembly_get_image(asm);
        if (!isValid(img)) continue;
        const cc = image_get_class_count(img);
        for (let c = 0; c < cc; c++) {
            try {
                const klass = image_get_class(img, c);
                if (!isValid(klass)) continue;
                allClasses.push(klass);
                const ks = klass.toString();
                klassSet.add(ks);
                const n = fullClassName(klass);
                if (n) klassMap.set(ks, { name: n, klass });
            } catch(e) {}
        }
    } catch(e) {}
}
console.log(`[*] ${allClasses.length} IL2CPP classes indexed\n`);

// ── Get all fields of a class (walks parent chain) ────────────────────────────
function getAllFields(klass) {
    const result = [], seen = new Set();
    let cur = klass;
    for (let d = 0; d < 20 && isValid(cur); d++) {
        const iter = Memory.alloc(Process.pointerSize);
        iter.writePointer(ptr(0));
        let f;
        try {
            while (!(f = class_get_fields(cur, iter)).isNull()) {
                const key = f.toString();
                if (seen.has(key)) continue;
                seen.add(key);
                let flags = 0; try { flags = field_get_flags(f); } catch(e) {}
                const isStatic = (flags & FIELD_ATTR_STATIC) !== 0;
                const name     = readStr(field_get_name(f)) || "?";
                const offset   = field_get_offset(f);
                let typeCode   = -1; try { typeCode = type_get_type(field_get_type(f)); } catch(e) {}
                result.push({ f, name, offset, isStatic, typeCode });
            }
        } catch(e) {}
        try { cur = class_get_parent(cur); } catch(e) { break; }
    }
    return result;
}

// Print what class (if any) an address is a managed object of, or vtable of
function labelAddr(p) {
    try {
        const ks = p.toString();
        if (klassMap.has(ks)) return `[CLASS META: ${klassMap.get(ks).name}]`;
        const vt = safeReadPtr(p);
        if (vt && isValid(vt) && klassMap.has(vt.toString())) return `[obj of: ${klassMap.get(vt.toString()).name}]`;
        // Also try: the pointer stored at p[0] might itself be an object
    } catch(e) {}
    return "";
}

// ── STRATEGY D: Follow an old pointer chain ───────────────────────────────────
function followChain(label, rva, offsets) {
    console.log(`\n${"═".repeat(60)}`);
    console.log(`[D] Chain: ${label}`);
    console.log(`${"═".repeat(60)}`);
    let p;
    try {
        p = MOD.base.add(rva).readPointer();
        console.log(`  [0] module+0x${rva.toString(16)} → ${p}  ${labelAddr(p)}`);
    } catch(e) { console.log(`  [0] module+0x${rva.toString(16)} unreadable`); return null; }

    for (let i = 0; i < offsets.length - 1; i++) {
        const off = offsets[i];
        let next;
        try { next = p.add(off).readPointer(); } catch(e) {
            console.log(`  [${i+1}] +0x${off.toString(16)} → <unreadable>`);
            return null;
        }
        console.log(`  [${i+1}] +0x${off.toString(16)} → ${next}  ${labelAddr(next)}`);
        p = next;
    }
    const lastOff = offsets[offsets.length - 1];
    const finalAddr = p.add(lastOff);
    const s32 = safeReadS32(finalAddr);
    const s64 = safeReadS64(finalAddr);
    console.log(`  [fin] +0x${lastOff.toString(16)} (int32) = ${s32}  (int64/ptr) = ${s64}  @ ${finalAddr}`);
    return finalAddr;
}

// ── STRATEGY B: Backward heap scan to find owning object ─────────────────────
function backwardScan(targetAddr) {
    if (!targetAddr) return null;
    const SCAN_BYTES = 0x400000;  // 4 MB
    const ALIGN      = 8;
    let bestExact    = null;

    for (let back = ALIGN; back < SCAN_BYTES; back += ALIGN) {
        const candidate = targetAddr.sub(back);
        try {
            const maybeKlass = candidate.readPointer();
            if (!isValid(maybeKlass)) continue;
            const ks = maybeKlass.toString();
            if (!klassMap.has(ks)) continue;

            const { name: cn, klass } = klassMap.get(ks);
            const fields = getAllFields(klass);

            for (const { name, offset, isStatic } of fields) {
                if (isStatic) continue;
                if (offset === back) {
                    if (!bestExact) {
                        console.log(`  [★ B] Owning object @ ${candidate}`);
                        console.log(`         Class : ${cn}`);
                        console.log(`         Field : ${name}  (offset 0x${back.toString(16)})`);
                        bestExact = { candidate, cn, name, klass };
                    }
                    break;
                }
                const diff = back - offset;
                if (diff > 0 && diff < 16 && !bestExact) {
                    console.log(`  [~ B] Near match: object @ ${candidate} (${cn})`);
                    console.log(`         Closest field: ${name}  offset=0x${offset.toString(16)}  target is +${diff} inside`);
                }
            }
            if (bestExact) break;
        } catch(e) {}
    }
    if (!bestExact) console.log("  [B] No owning managed object found in 4 MB backward scan.");
    return bestExact;
}

// ── STRATEGY C: Which static holds the owner (0-2 hops) ──────────────────────
function findStaticPath(ownerObj) {
    if (!ownerObj) return;
    const ownerStr = ownerObj.candidate.toString();
    console.log(`\n  [C] Searching all statics for path to owner ${ownerStr} (${ownerObj.cn})`);
    let found = false;

    for (const klass of allClasses) {
        try {
            const staticData = class_get_static_field_data(klass);
            if (!isValid(staticData)) continue;
            const cn    = fullClassName(klass) || "?";
            const flds  = getAllFields(klass);

            for (const { name: fn, offset: fo, isStatic: fs, typeCode: tc } of flds) {
                if (!fs) continue;
                let sv;
                try { sv = staticData.add(fo).readPointer(); } catch(e) { continue; }
                if (!isValid(sv)) continue;

                // Hop 0
                if (sv.toString() === ownerStr) {
                    console.log(`  [★ C-HOP0] ${cn}.${fn}  →  owner.${ownerObj.name}`);
                    found = true; continue;
                }

                // Hop 1
                if (!REF_TYPES.has(tc)) continue;
                let k1; try { k1 = object_get_class(sv); if (!isValid(k1)) continue; } catch(e) { continue; }
                const cn1 = fullClassName(k1) || "?";
                for (const { name: f1, offset: o1, isStatic: s1 } of getAllFields(k1)) {
                    if (s1) continue;
                    let v1; try { v1 = sv.add(o1).readPointer(); } catch(e) { continue; }
                    if (!isValid(v1)) continue;
                    if (v1.toString() === ownerStr) {
                        console.log(`  [★ C-HOP1] ${cn}.${fn}  →  (${cn1}).${f1}.${ownerObj.name}`);
                        found = true;
                    }
                }
            }
        } catch(e) {}
    }
    if (!found) console.log("  [C] Owner not reachable within 1 hop of any static.");
}

// ═════════════════════════════════════════════════════════════════════════════
// Run all 4 chains
// ═════════════════════════════════════════════════════════════════════════════

// Chains from PROMemoryManager.cs:
//   CurrentEncounterId = [0x01561728, 0x20, 0x350, 0xB8, 0x0, 0x7D0]
//   IsBattling         = [0x01561728, 0x20, 0x350, 0xB8, 0x0, 0x750]
//   ShinyForm          = [0x01561AE0, 0x20, 0xB8, 0x80, 0xB0, 0x7E0]
//   EventForm          = [0x01561AE0, 0x20, 0xB8, 0x80, 0xB0, 0x7E4]
//
// Note: these chains' first hop is an RVA inside GameAssembly.dll that points
// to a C++ global (the Il2CppClass* address for some class). From there the
// remaining offsets walk through the class metadata and managed objects.

const chains = [
    { label: "CurrentEncounterId", rva: 0x01561728, offsets: [0x20, 0x350, 0xB8, 0x0, 0x7D0] },
    { label: "IsBattling",         rva: 0x01561728, offsets: [0x20, 0x350, 0xB8, 0x0, 0x750] },
    { label: "ShinyForm",          rva: 0x01561AE0, offsets: [0x20, 0xB8, 0x80, 0xB0, 0x7E0] },
    { label: "EventForm",          rva: 0x01561AE0, offsets: [0x20, 0xB8, 0x80, 0xB0, 0x7E4] },
];

for (const { label, rva, offsets } of chains) {
    const finalAddr = followChain(label, rva, offsets);
    const owner     = backwardScan(finalAddr);
    findStaticPath(owner);
}

console.log("\n[*] Done. Paste output to identify named IL2CPP paths.");
})();
