// !!!VIBECODED BULLSHIT AHEAD!!!


// inseraction_moveright.js — move player one tile right (no hardcoded offsets)
// All field offsets resolved at runtime via il2cpp_field_get_offset.
// Only the non-obfuscated field/method names are used.
// Usage: frida -p <PID> -l .\inseraction_moveright.js

const mod = Process.getModuleByName("GameAssembly.dll");

// IL2CPP API
const il2cpp = {
    domain_get:                 new NativeFunction(mod.getExportByName("il2cpp_domain_get"), "pointer", []),
    domain_get_assemblies:      new NativeFunction(mod.getExportByName("il2cpp_domain_get_assemblies"), "pointer", ["pointer", "pointer"]),
    assembly_get_image:         new NativeFunction(mod.getExportByName("il2cpp_assembly_get_image"), "pointer", ["pointer"]),
    image_get_class_count:      new NativeFunction(mod.getExportByName("il2cpp_image_get_class_count"), "uint", ["pointer"]),
    image_get_class:            new NativeFunction(mod.getExportByName("il2cpp_image_get_class"), "pointer", ["pointer", "uint"]),
    class_get_name:             new NativeFunction(mod.getExportByName("il2cpp_class_get_name"), "pointer", ["pointer"]),
    class_get_fields:           new NativeFunction(mod.getExportByName("il2cpp_class_get_fields"), "pointer", ["pointer", "pointer"]),
    class_get_method_from_name: new NativeFunction(mod.getExportByName("il2cpp_class_get_method_from_name"), "pointer", ["pointer", "pointer", "int"]),
    field_get_name:             new NativeFunction(mod.getExportByName("il2cpp_field_get_name"), "pointer", ["pointer"]),
    field_get_offset:           new NativeFunction(mod.getExportByName("il2cpp_field_get_offset"), "uint", ["pointer"]),
    field_static_get_value:     new NativeFunction(mod.getExportByName("il2cpp_field_static_get_value"), "void", ["pointer", "pointer"]),
    string_new:                 new NativeFunction(mod.getExportByName("il2cpp_string_new"), "pointer", ["pointer"]),
};

function str(p) { try { return p && !p.isNull() ? p.readUtf8String() : null; } catch { return null; } }
function ok(p)  { return p && !p.isNull() && p.toUInt32() > 0x1000; }

function findClass(name) {
    const domain = il2cpp.domain_get();
    const buf = Memory.alloc(4);
    const asms = il2cpp.domain_get_assemblies(domain, buf);
    for (let i = 0, n = buf.readU32(); i < n; i++) {
        const img = il2cpp.assembly_get_image(asms.add(i * Process.pointerSize).readPointer());
        if (!ok(img)) continue;
        for (let j = 0, cc = il2cpp.image_get_class_count(img); j < cc; j++) {
            const k = il2cpp.image_get_class(img, j);
            if (ok(k) && str(il2cpp.class_get_name(k)) === name) return k;
        }
    }
    return null;
}

// Iterate all fields, return map of name → { field, offset }
function getFields(klass) {
    const result = {};
    const iter = Memory.alloc(Process.pointerSize); iter.writePointer(ptr(0));
    let f;
    while (!(f = il2cpp.class_get_fields(klass, iter)).isNull()) {
        const name = str(il2cpp.field_get_name(f));
        if (name) result[name] = { field: f, offset: il2cpp.field_get_offset(f) };
    }
    return result;
}

// Find the DSSock singleton static field (type DSSock, ends with k__BackingField)
function getSingleton(klass) {
    const iter = Memory.alloc(Process.pointerSize); iter.writePointer(ptr(0));
    let f;
    while (!(f = il2cpp.class_get_fields(klass, iter)).isNull()) {
        const name = str(il2cpp.field_get_name(f));
        if (name && name.endsWith("k__BackingField")) {
            const buf = Memory.alloc(Process.pointerSize);
            il2cpp.field_static_get_value(f, buf);
            const val = buf.readPointer();
            if (ok(val)) return val;
        }
    }
    return null;
}

function getMethod(klass, name, argc) {
    const m = il2cpp.class_get_method_from_name(klass, Memory.allocUtf8String(name), argc);
    return ok(m) ? m.readPointer() : null;
}

// Find the follower-movement method: (String, Int32) → void, identified by signature
function findFollowerMethod(klass, fields) {
    // Iterate all methods with 2 params, call each candidate during a test?
    // For now, search for the method right before the one that takes (Vector3, Int32, Int32)
    // Both are adjacent in the vtable. We find ffq-equivalent by signature, then ffp is the one before it.
    // 
    // Simpler: just iterate fields and methods. The follower method is the (String, Int32)->void
    // that sits near movement methods. We can't distinguish by signature alone, so we use
    // the name-based approach but fall back to null if not found.
    //
    // Since method names DO get re-mangled, we store the current name and the user updates it
    // per UPDATING3.md instructions. This is the ONE thing that needs manual update.
    return null; // handled by caller with explicit name
}

// ── Setup ──────────────────────────────────────────────────────────────────────
const dsKlass = findClass("DSSock");
if (!dsKlass) throw "DSSock not found";

const ds = getSingleton(dsKlass);
if (!ds) throw "DSSock singleton not found";

const fields = getFields(dsKlass);

const offStanding = fields["Standing"]?.offset;
const offDir      = fields["PlayerDir"]?.offset;
const offTarget   = fields["TargetPos"]?.offset;  // Vector3: x at +0, y at +4, z at +8

if (offStanding == null || offDir == null || offTarget == null)
    throw `Missing fields: Standing=${offStanding} PlayerDir=${offDir} TargetPos=${offTarget}`;

console.log(`[+] DSSock @ ${ds}`);
console.log(`[+] Standing @ 0x${offStanding.toString(16)}, PlayerDir @ 0x${offDir.toString(16)}, TargetPos @ 0x${offTarget.toString(16)}`);

const updatePtr = getMethod(dsKlass, "Update", 0);
if (!updatePtr) throw "Update method not found";

// ── Follower method ────────────────────────────────────────────────────────────
// This is the ONLY obfuscated name in the script. See UPDATING3.md for how to find it.
const FOLLOWER_METHOD = "ffp";   // ← update this after re-mangle
const FOLLOWER_ARGC   = 2;

const followerPtr = getMethod(dsKlass, FOLLOWER_METHOD, FOLLOWER_ARGC);
const follower = followerPtr ? new NativeFunction(followerPtr, "void", ["pointer", "pointer", "int32"]) : null;
const rStr = il2cpp.string_new(Memory.allocUtf8String("r"));

if (follower) console.log(`[+] Follower method "${FOLLOWER_METHOD}" found`);
else          console.log("[!] Follower method not found — player will move but follower won't follow");

// ── Move right on next standing frame ──────────────────────────────────────────
const hook = Interceptor.attach(updatePtr, {
    onEnter() {
        if (ds.add(offStanding).readU8() !== 1) return;
        hook.detach();

        const x = ds.add(offTarget).readFloat();
        ds.add(offTarget).writeFloat(x + 1);   // TargetPos.x += 1
        ds.add(offStanding).writeU8(0);         // start walking
        ds.add(offDir).writeU8(4);              // face right
        if (follower) follower(ds, rStr, 4);    // follower follows

        console.log(`[+] Moved right: TP.x ${x} → ${x + 1}`);
    }
});

console.log("[*] Will move right on next standing frame...");
