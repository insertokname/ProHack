// !!!VIBECODED BULLSHIT AHEAD!!!

// trace_ffq.js
// Run this, then walk in-game (arrow keys) to see exactly what
// ffq() receives during real movement. Press Ctrl+C when done.

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
const class_get_method_from_name = new NativeFunction(
    mod.getExportByName("il2cpp_class_get_method_from_name"),
    "pointer",
    ["pointer", "pointer", "int"],
);

function readStr(p) {
    try {
        return p && !p.isNull() ? p.readUtf8String() : null;
    } catch {
        return null;
    }
}
function isValid(p) {
    if (!p || p.isNull()) return false;
    const v = p.toUInt32();
    return v > 0x1000 && v !== 0xffffffff;
}

// ── Find DSSock ───────────────────────────────────────────────────────────────
const domain = domain_get();
const countBuf = Memory.alloc(4);
const assemblies = domain_get_assemblies(domain, countBuf);
const asmCount = countBuf.readU32();
let dsKlass = null;

outer: for (let a = 0; a < asmCount; a++) {
    const asm = assemblies.add(a * Process.pointerSize).readPointer();
    const img = assembly_get_image(asm);
    if (!isValid(img)) continue;
    const cc = image_get_class_count(img);
    for (let c = 0; c < cc; c++) {
        const klass = image_get_class(img, c);
        if (!isValid(klass)) continue;
        if (readStr(class_get_name(klass)) === "DSSock") {
            dsKlass = klass;
            break outer;
        }
    }
}

if (!dsKlass) {
    console.log("[-] DSSock not found");
} else {
    // ── Resolve ffq and ffs ───────────────────────────────────────────────────
    const ffqInfo = class_get_method_from_name(
        dsKlass,
        Memory.allocUtf8String("ffq"),
        3,
    );
    const ffsInfo = class_get_method_from_name(
        dsKlass,
        Memory.allocUtf8String("ffs"),
        3,
    );

    if (!isValid(ffqInfo)) {
        console.log("[-] ffq() not found");
    } else {
        const ffqPtr = ffqInfo.readPointer();
        console.log(
            `[*] ffq native ptr = ${ffqPtr}  – walk in-game to see real args`,
        );
        console.log(`[*] Columns: this | vec3(x,y,z) | b | c | args[4]`);

        // Intercept ffq at native level.
        // Windows x64 ABI for a 12-byte struct (Vector3):
        //   args[0] = this (DSSock*)
        //   args[1] = Vector3* (hidden pointer, caller-allocated)
        //   args[2] = int b
        //   args[3] = int c
        //   args[4] = MethodInfo* (IL2CPP hidden param) – may be present
        Interceptor.attach(ffqPtr, {
            onEnter(args) {
                try {
                    const thiz = args[0];
                    const vecP = args[1];
                    const b = args[2].toInt32();
                    const c = args[3].toInt32();
                    const extra = args[4]; // likely MethodInfo*

                    let vx = "?",
                        vy = "?",
                        vz = "?";
                    try {
                        vx = vecP.readFloat().toFixed(3);
                        vy = vecP.add(4).readFloat().toFixed(3);
                        vz = vecP.add(8).readFloat().toFixed(3);
                    } catch (_) {}

                    // Also log TargetPos and pjs from the instance for comparison
                    let tx = "?",
                        ty = "?";
                    let px = "?",
                        py = "?";
                    try {
                        tx = thiz.add(0x1a4).readFloat().toFixed(3);
                        ty = thiz.add(0x1a8).readFloat().toFixed(3);
                        px = thiz.add(0x300).readFloat().toFixed(3);
                        py = thiz.add(0x304).readFloat().toFixed(3);
                    } catch (_) {}

                    console.log(
                        `[ffq] vec=(${vx},${vy},${vz})` +
                            `  b=${b}  c=${c}` +
                            `  extra=${extra}` +
                            `  | TargetPos=(${tx},${ty})  pjs=(${px},${py})`,
                    );
                } catch (e) {
                    console.log("[-] trace error: " + e);
                }
            },
        });
    }

    // ── Also trace ffs for comparison ─────────────────────────────────────────
    // ffs (Vector3, Vector2, Vector2) – might be the real mover
    if (isValid(ffsInfo)) {
        const ffsPtr = ffsInfo.readPointer();
        console.log(`[*] ffs native ptr = ${ffsPtr}  – also tracing`);
        Interceptor.attach(ffsPtr, {
            onEnter(args) {
                try {
                    // args[1] = Vector3*  args[2] = Vector2*  args[3] = Vector2*
                    const v3 = args[1];
                    const v2a = args[2];
                    const v2b = args[3];
                    const x3 = v3.readFloat().toFixed(3),
                        y3 = v3.add(4).readFloat().toFixed(3),
                        z3 = v3.add(8).readFloat().toFixed(3);
                    const xa = v2a.readFloat().toFixed(3),
                        ya = v2a.add(4).readFloat().toFixed(3);
                    const xb = v2b.readFloat().toFixed(3),
                        yb = v2b.add(4).readFloat().toFixed(3);
                    console.log(
                        `[ffs] vec3=(${x3},${y3},${z3})  v2a=(${xa},${ya})  v2b=(${xb},${yb})`,
                    );
                } catch (e) {
                    console.log("[-] ffs trace error: " + e);
                }
            },
        });
    }

    console.log(
        "[*] Tracing active – walk in-game now (arrow keys). Ctrl+C to stop.",
    );
}
