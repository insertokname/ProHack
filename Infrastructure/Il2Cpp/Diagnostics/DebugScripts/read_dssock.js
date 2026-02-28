// !!!VIBECODED BULLSHIT AHEAD!!!

const mod = Process.getModuleByName("GameAssembly.dll");

const domain_get = new NativeFunction(
    mod.getExportByName("il2cpp_domain_get"),
    "pointer",
    [],
);
const domain_get_assemblies = new NativeFunction(
    mod.getExportByName("il2cpp_domain_get_assemblies"),
    "pointer",
    ["pointer", "pointer"],
);
const assembly_get_image = new NativeFunction(
    mod.getExportByName("il2cpp_assembly_get_image"),
    "pointer",
    ["pointer"],
);
const image_get_class_count = new NativeFunction(
    mod.getExportByName("il2cpp_image_get_class_count"),
    "uint",
    ["pointer"],
);
const image_get_class = new NativeFunction(
    mod.getExportByName("il2cpp_image_get_class"),
    "pointer",
    ["pointer", "uint"],
);
const class_get_name = new NativeFunction(
    mod.getExportByName("il2cpp_class_get_name"),
    "pointer",
    ["pointer"],
);
const class_get_static_field_data = new NativeFunction(
    mod.getExportByName("il2cpp_class_get_static_field_data"),
    "pointer",
    ["pointer"],
);
const class_get_fields = new NativeFunction(
    mod.getExportByName("il2cpp_class_get_fields"),
    "pointer",
    ["pointer", "pointer"],
);
const field_get_name = new NativeFunction(
    mod.getExportByName("il2cpp_field_get_name"),
    "pointer",
    ["pointer"],
);
const field_get_offset = new NativeFunction(
    mod.getExportByName("il2cpp_field_get_offset"),
    "uint",
    ["pointer"],
);
const field_static_get_value = new NativeFunction(
    mod.getExportByName("il2cpp_field_static_get_value"),
    "void",
    ["pointer", "pointer"],
);
const runtime_invoke    = new NativeFunction(mod.getExportByName("il2cpp_runtime_invoke"),        "pointer", ["pointer", "pointer", "pointer", "pointer"]);


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

const TARGETS = ["<pjm>k__BackingField"];

const domain = domain_get();
const countBuf = Memory.alloc(4);
const assemblies = domain_get_assemblies(domain, countBuf);
const asmCount = countBuf.readU32();

for (let a = 0; a < asmCount; a++) {
    const asm = assemblies.add(a * Process.pointerSize).readPointer();
    const img = assembly_get_image(asm);
    if (!isValid(img)) continue;

    const cc = image_get_class_count(img);
    for (let c = 0; c < cc; c++) {
        const klass = image_get_class(img, c);
        if (!isValid(klass)) continue;
        if (readStr(class_get_name(klass)) !== "DSSock") continue;

        console.log("[*] Found DSSock class");
        const staticData = class_get_static_field_data(klass);

        // iterate fields
        const iter = Memory.alloc(Process.pointerSize);
        iter.writePointer(ptr(0));
        let field;
        while (!(field = class_get_fields(klass, iter)).isNull()) {
            const fname = readStr(field_get_name(field));
            if (!TARGETS.includes(fname)) continue;

            // read via static_get_value into a pointer-sized buffer
            const buf = Memory.alloc(Process.pointerSize);
            field_static_get_value(field, buf);
            const val = buf.readPointer();

            console.log(`\n[+] DSSock.${fname}`);
            console.log(`    raw value : ${val}`);
            // try reading as utf8 string too in case it's a plain char*
            try {
                console.log(`    as utf8   : ${val.readUtf8String()}`);
            } catch (e) {}
            // try reading as utf16 (Il2CppString)
            try {
                const len = val.add(Process.pointerSize * 2).readU32();
                if (len > 0 && len < 512) {
                    const chars = val.add(Process.pointerSize * 2 + 4);
                    console.log(
                        `    as Il2CppString: ${chars.readUtf16String(len)}`,
                    );
                }
            } catch (e) {}
        }
    }
}
