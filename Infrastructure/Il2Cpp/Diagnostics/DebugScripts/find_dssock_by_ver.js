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
const field_get_flags = new NativeFunction(
    mod.getExportByName("il2cpp_field_get_flags"),
    "uint",
    ["pointer"],
);
const field_get_value_object = new NativeFunction(
    mod.getExportByName("il2cpp_field_get_value_object"),
    "pointer",
    ["pointer", "pointer"],
);
const string_chars = new NativeFunction(
    mod.getExportByName("il2cpp_string_chars"),
    "pointer",
    ["pointer"],
);
const string_length = new NativeFunction(
    mod.getExportByName("il2cpp_string_length"),
    "int",
    ["pointer"],
);

function readStr(p) {
    try {
        return p && !p.isNull() ? p.readUtf8String() : null;
    } catch (e) {
        return null;
    }
}
function readIl2CppString(p) {
    try {
        if (!p || p.isNull()) return null;
        const len = string_length(p);
        if (len <= 0 || len > 1024) return null;
        return string_chars(p).readUtf16String(len);
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
    // Step 1: find DSSock class pointer
    let dsSockClass = null;
    const domain = domain_get();
    const countBuf = Memory.alloc(4);
    const assemblies = domain_get_assemblies(domain, countBuf);
    const asmCount = countBuf.readU32();

    outer: for (let a = 0; a < asmCount; a++) {
        const img = assembly_get_image(
            assemblies.add(a * Process.pointerSize).readPointer(),
        );
        if (!isValid(img)) continue;
        const cc = image_get_class_count(img);
        for (let c = 0; c < cc; c++) {
            const klass = image_get_class(img, c);
            if (!isValid(klass)) continue;
            if (readStr(class_get_name(klass)) === "DSSock") {
                dsSockClass = klass;
                break outer;
            }
        }
    }

    if (!dsSockClass) {
        console.log("[-] DSSock class not found");
        return;
    }
    console.log(`[*] DSSock class @ ${dsSockClass}`);

    // Step 2: find the Version field descriptor
    const fields = iterFields(dsSockClass);
    const versionField = fields.find(
        (f) => readStr(field_get_name(f)) === "Version",
    );
    if (!versionField) {
        console.log("[-] Version field not found");
        return;
    }

    // Step 3: scan all writable memory regions for pointers to dsSockClass
    // Every IL2CPP object's first word IS its class pointer — so any address
    // whose first 8 bytes == dsSockClass is a candidate DSSock instance
    const needle = dsSockClass.toMatchPattern(); // turns ptr into a byte pattern

    console.log("[*] Scanning memory for DSSock instances...");

    Process.enumerateRanges("rw-").forEach((range) => {
        // skip small or obviously non-heap regions
        if (range.size < 0x1000) return;

        try {
            const matches = Memory.scanSync(range.base, range.size, needle);
            matches.forEach((match) => {
                const candidate = match.address;

                // candidate is a pointer TO the class ptr, meaning the object IS at candidate
                // verify it looks like a real object by checking the Version field
                try {
                    const valObj = field_get_value_object(
                        versionField,
                        candidate,
                    );
                    if (!isValid(valObj)) return;
                    const str = readIl2CppString(valObj);
                    if (str === "Valentine 26 v2") {
                        console.log(`[+] FOUND DSSock instance @ ${candidate}`);
                    }
                } catch (e) {
                    /* not a valid object, skip */
                }
            });
        } catch (e) {
            /* unreadable range, skip */
        }
    });

    console.log("[*] Scan complete");
}, 3000);
