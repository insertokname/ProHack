// !!!VIBECODED BULLSHIT AHEAD!!!

const mod = Process.getModuleByName("GameAssembly.dll");

function bind(name, ret, args) {
    try {
        const fn = new NativeFunction(mod.getExportByName(name), ret, args);
        console.log(`[+] bound ${name}`);
        return fn;
    } catch (e) {
        console.log(`[!] FAILED to bind ${name}: ${e.message}`);
        return null;
    }
}

const domain_get = bind("il2cpp_domain_get", "pointer", []);
const domain_get_assemblies = bind("il2cpp_domain_get_assemblies", "pointer", [
    "pointer",
    "pointer",
]);
const assembly_get_image = bind("il2cpp_assembly_get_image", "pointer", [
    "pointer",
]);
const image_get_class_count = bind("il2cpp_image_get_class_count", "uint", [
    "pointer",
]);
const image_get_class = bind("il2cpp_image_get_class", "pointer", [
    "pointer",
    "uint",
]);
const class_get_name = bind("il2cpp_class_get_name", "pointer", ["pointer"]);
const class_get_fields = bind("il2cpp_class_get_fields", "pointer", [
    "pointer",
    "pointer",
]);
const class_get_static_field_data = bind(
    "il2cpp_class_get_static_field_data",
    "pointer",
    ["pointer"],
);
const field_get_name = bind("il2cpp_field_get_name", "pointer", ["pointer"]);
const field_get_offset = bind("il2cpp_field_get_offset", "uint", ["pointer"]);
const field_get_flags = bind("il2cpp_field_get_flags", "uint", ["pointer"]);
const object_get_class = bind("il2cpp_object_get_class", "pointer", [
    "pointer",
]);

const TARGET = ptr("0x2299173b000");

function readStr(p) {
    try {
        return p && !p.isNull() ? p.readUtf8String() : null;
    } catch (e) {
        return null;
    }
}

function isValid(p) {
    if (!p || p.isNull()) return false;
    const v = p.toUInt32();
    return v > 0x1000 && v !== 0xffffffff;
}

function iterFields(klass) {
    const iter = Memory.alloc(Process.pointerSize);
    iter.writePointer(ptr(0));
    const results = [];
    let f,
        guard = 0;
    while (!(f = class_get_fields(klass, iter)).isNull() && guard++ < 500)
        results.push(f);
    return results;
}

setTimeout(() => {
    const domain = domain_get();
    const countBuf = Memory.alloc(4);
    const assemblies = domain_get_assemblies(domain, countBuf);
    const asmCount = countBuf.readU32();

    let found = false;

    for (let a = 0; a < asmCount; a++) {
        const asm = assemblies.add(a * Process.pointerSize).readPointer();
        if (!isValid(asm)) continue;
        const img = assembly_get_image(asm);
        if (!isValid(img)) continue;
        const cc = image_get_class_count(img);

        for (let c = 0; c < cc; c++) {
            const klass = image_get_class(img, c);
            if (!isValid(klass)) continue;

            const staticData = class_get_static_field_data(klass);
            if (!isValid(staticData)) continue;

            iterFields(klass).forEach((field) => {
                try {
                    const flags = field_get_flags(field);
                    const isStatic = (flags & 0x10) !== 0;
                    if (!isStatic) return;

                    const offset = field_get_offset(field);
                    if (offset === 0xffffffff || offset > 0x10000) return;

                    let candidate;
                    try {
                        candidate = staticData.add(offset).readPointer();
                    } catch (e) {
                        return;
                    }
                    if (!candidate.equals(TARGET)) return;

                    const ownerName = readStr(class_get_name(klass)) ?? "?";
                    const fieldName = readStr(field_get_name(field)) ?? "?";
                    console.log(`\n[!!!] PATH FOUND`);
                    console.log(
                        `      ${ownerName}.${fieldName}  ->  DSSock @ ${candidate}`,
                    );
                    found = true;
                } catch (e) {
                    console.log(`[!] ${e.message}`);
                }
            });
        }
    }

    if (!found) console.log("[-] Not found as a direct static field.");
}, 1);
