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
const class_get_method_from_name = new NativeFunction(
    mod.getExportByName("il2cpp_class_get_method_from_name"),
    "pointer", ["pointer", "pointer", "int"],
);
const runtime_invoke = new NativeFunction(
    mod.getExportByName("il2cpp_runtime_invoke"),
    "pointer", ["pointer", "pointer", "pointer", "pointer"],
);

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
    // static field offset 0x0, type DSSock (reference → pointer)
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
        // ── Step 3: resolve Quit() and invoke it ─────────────────────────────
        // Quit() has 0 parameters (see DSSockInfo.txt: Quit ():System.Void)
        const methodName = Memory.allocUtf8String("Quit");
        const quitMethod = class_get_method_from_name(dsKlass, methodName, 0);

        if (!isValid(quitMethod)) {
            console.log("[-] Quit() method not found on DSSock");
        } else {
            console.log("[+] Quit() MethodInfo = " + quitMethod);

            const exc = Memory.alloc(Process.pointerSize);
            exc.writePointer(ptr(0));

            // il2cpp_runtime_invoke(method, instance, params=NULL, &exception)
            runtime_invoke(quitMethod, dsInstance, ptr(0), exc);

            const thrown = exc.readPointer();
            if (!thrown.isNull()) {
                console.log("[-] Exception thrown @ " + thrown);
            } else {
                console.log("[+] Quit() invoked successfully");
            }
        }
    }
}
