// !!!VIBECODED BULLSHIT AHEAD!!!

const mod = Process.getModuleByName("GameAssembly.dll");

// IL2CPP API bindings
const domain_get = new NativeFunction(
    mod.getExportByName("il2cpp_domain_get"),
    "pointer", [],
);
const domain_get_assemblies = new NativeFunction(
    mod.getExportByName("il2cpp_domain_get_assemblies"),
    "pointer", ["pointer", "pointer"],
);
const assembly_get_image = new NativeFunction(
    mod.getExportByName("il2cpp_assembly_get_image"),
    "pointer", ["pointer"],
);
const image_get_class_count = new NativeFunction(
    mod.getExportByName("il2cpp_image_get_class_count"),
    "uint", ["pointer"],
);
const image_get_class = new NativeFunction(
    mod.getExportByName("il2cpp_image_get_class"),
    "pointer", ["pointer", "uint"],
);
const class_get_name = new NativeFunction(
    mod.getExportByName("il2cpp_class_get_name"),
    "pointer", ["pointer"],
);
const class_get_fields = new NativeFunction(
    mod.getExportByName("il2cpp_class_get_fields"),
    "pointer", ["pointer", "pointer"],
);
const field_get_name = new NativeFunction(
    mod.getExportByName("il2cpp_field_get_name"),
    "pointer", ["pointer"],
);
const field_static_get_value = new NativeFunction(
    mod.getExportByName("il2cpp_field_static_get_value"),
    "void", ["pointer", "pointer"],
);

function readStr(p) {
    try {
        return p && !p.isNull() ? p.readUtf8String() : null;
    } catch (e) {
        return null;
    }
}

// Read an IL2CPP System.String object (UTF-16, length at +0x10, chars at +0x14)
function readIl2CppString(p) {
    try {
        if (!p || p.isNull()) return null;
        const len = p.add(0x10).readS32();
        if (len <= 0 || len > 2048) return "(invalid string)";
        return p.add(0x14).readUtf16String(len);
    } catch (e) {
        return null;
    }
}

function isValid(p) {
    if (!p || p.isNull()) return false;
    const v = p.toUInt32();
    return v > 0x1000 && v !== 0xffffffff;
}

// ── Step 1: find the DSSock class ────────────────────────────────────────────
const domain = domain_get();
const countBuf = Memory.alloc(4);
const assemblies = domain_get_assemblies(domain, countBuf);
const asmCount = countBuf.readU32();

let dsKlass = null;

outer:
for (let a = 0; a < asmCount; a++) {
    const asm = assemblies.add(a * Process.pointerSize).readPointer();
    const img = assembly_get_image(asm);
    if (!isValid(img)) continue;

    const cc = image_get_class_count(img);
    for (let c = 0; c < cc; c++) {
        const klass = image_get_class(img, c);
        if (!isValid(klass)) continue;
        if (readStr(class_get_name(klass)) === "DSSock") {
            dsKlass = klass;
            console.log("[*] Found DSSock class @ " + klass);
            break outer;
        }
    }
}

if (!dsKlass) {
    console.log("[-] DSSock class not found");
} else {
    // ── Step 2: read DSSock.<pjm>k__BackingField (the singleton) ─────────────
    const iter = Memory.alloc(Process.pointerSize);
    iter.writePointer(ptr(0));

    let dsInstance = null;
    let field;
    while (!(field = class_get_fields(dsKlass, iter)).isNull()) {
        if (readStr(field_get_name(field)) === "<pjm>k__BackingField") {
            const buf = Memory.alloc(Process.pointerSize);
            field_static_get_value(field, buf);
            dsInstance = buf.readPointer();
            console.log("[+] DSSock instance = " + dsInstance);
            break;
        }
    }

    if (!dsInstance || dsInstance.isNull()) {
        console.log("[-] DSSock instance is null – not logged in yet?");
    } else {
        // ── Step 3: read Items field ──────────────────────────────────────────
        // DSSockInfo.txt: 170 : Items (type: System.Collections.Generic.List<DSSock.hd>)
        const listPtr = dsInstance.add(0x170).readPointer();
        if (!isValid(listPtr)) {
            console.log("[-] Items list is null");
        } else {
            // List<T> layout (IL2CPP):
            //   +0x10 : _items  → T[] backing array
            //   +0x18 : _size   → int count
            const arrayPtr = listPtr.add(0x10).readPointer();
            const count    = listPtr.add(0x18).readS32();

            console.log(`[+] Items count: ${count}`);

            if (!isValid(arrayPtr) || count <= 0) {
                console.log("[-] Items array is empty or invalid");
            } else {
                // IL2CPP array elements start at +0x20 (16-byte object header +
                // 8-byte bounds ptr + 4-byte length + 4-byte padding = 0x20)
                for (let i = 0; i < count; i++) {
                    const hdPtr = arrayPtr.add(0x20 + i * Process.pointerSize).readPointer();
                    if (!isValid(hdPtr)) {
                        console.log(`  [${i}] (null entry)`);
                        continue;
                    }

                    // DSSock.hd fields (DSSockInfo.txt):
                    //   0x10 : pau  (System.String)  – item identifier / name
                    //   0x18 : pav  (System.String)  – secondary string
                    //   0x20 : paw  (System.Int32)   – quantity / count
                    //   0x24 : pax  (lj)             – enum/type id (read as int32)
                    //   0x28 : pay  (System.Int32)   – extra int (e.g. slot / flags)
                    const pau = readIl2CppString(hdPtr.add(0x10).readPointer());
                    const pav = readIl2CppString(hdPtr.add(0x18).readPointer());
                    const paw = hdPtr.add(0x20).readS32();
                    const pax = hdPtr.add(0x24).readS32();
                    const pay = hdPtr.add(0x28).readS32();

                    console.log(
                        `  [${i}] pau="${pau}" pav="${pav}" paw=${paw} pax=${pax} pay=${pay}`
                    );
                }
            }
        }
    }
}
