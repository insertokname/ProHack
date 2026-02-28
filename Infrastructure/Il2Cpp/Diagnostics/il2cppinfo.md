**!!!VIBECODED BULLSHIT AHEAD!!!**

## What You Can Reconstruct About the Original C# Code

The IL2CPP exported API is essentially a complete runtime reflection system. Here's everything you can recover:

---

### The Assembly Hierarchy

The runtime organizes everything in a tree:

```
Domain
  └── Assembly[] (each .dll in the game, e.g. Assembly-CSharp.dll, UnityEngine.dll)
        └── Image (the binary representation of that assembly)
              └── Class[] (every type defined in that assembly)
                    ├── Fields[]
                    ├── Methods[]
                    ├── Properties[]
                    ├── Events[]
                    ├── NestedTypes[]
                    └── Interfaces[]
```

Using the API you can walk this entire tree and reconstruct something very close to the original C# source structure.

---

### What You Can Recover Per-Class

```javascript
il2cpp_class_get_name(klass)        // "PlayerController"
il2cpp_class_get_namespace(klass)   // "MyGame.Controllers"
il2cpp_class_get_parent(klass)      // base class (e.g. MonoBehaviour)
il2cpp_class_get_interfaces(klass)  // implemented interfaces
il2cpp_class_get_flags(klass)       // public/private/abstract/sealed etc (CLI TypeAttributes bitmask)
il2cpp_class_is_abstract(klass)
il2cpp_class_is_enum(klass)
il2cpp_class_is_valuetype(klass)    // struct vs class
il2cpp_class_is_interface(klass)
il2cpp_class_is_generic(klass)
il2cpp_class_instance_size(klass)   // sizeof the object in memory
il2cpp_class_get_nested_types(klass)// inner classes
il2cpp_class_get_assemblyname(klass)
```

This tells you the full type hierarchy. You can reconstruct the inheritance tree of every class in the game.

---

### What You Can Recover Per-Method

```javascript
il2cpp_method_get_name(method)          // "CalculateDamage"
il2cpp_method_get_declaring_type(method)// which class owns it
il2cpp_method_get_return_type(method)   // return type as Il2CppType*
il2cpp_method_get_param_count(method)   // number of parameters
il2cpp_method_get_param(method, i)      // Il2CppType* of param i
il2cpp_method_get_param_name(method, i) // "amount", "target", etc
il2cpp_method_get_flags(method)         // public/private/static/virtual (MethodAttributes bitmask)
il2cpp_method_is_generic(method)
il2cpp_method_is_inflated(method)       // is this a generic instantiation
il2cpp_method_is_instance(method)       // instance vs static
il2cpp_method_get_token(method)         // metadata token
```

The param names survive AOT compilation because they're stored in the metadata blob. This means you can reconstruct full method signatures like:

```csharp
public float CalculateDamage(int amount, GameObject target)
```

---

### What You Can Recover Per-Field

```javascript
il2cpp_field_get_name(field)    // "health", "_secretKey"
il2cpp_field_get_type(field)    // Il2CppType* 
il2cpp_field_get_parent(field)  // owning class
il2cpp_field_get_offset(field)  // byte offset within the object — crucial for memory reading
il2cpp_field_get_flags(field)   // public/private/static/readonly
il2cpp_field_is_literal(field)  // is it a const?
```

The **offset** is particularly powerful. If you have a live object pointer you can compute exactly where any field sits in memory:

```javascript
const offset = api.field_get_offset(healthField);
const healthValue = objectPtr.add(offset).readFloat();
```

For static fields:
```javascript
il2cpp_class_get_static_field_data(klass) // returns pointer to the static data block
il2cpp_field_static_get_value(field, out) // read static field value directly
```

---

### What You Can Recover Per-Property

```javascript
il2cpp_property_get_name(prop)        // "IsAlive"
il2cpp_property_get_get_method(prop)  // the getter MethodInfo*
il2cpp_property_get_set_method(prop)  // the setter MethodInfo*
il2cpp_property_get_parent(prop)      // owning class
il2cpp_property_get_flags(prop)
```

Properties in C# are just syntactic sugar over getter/setter methods. You get both the name and can invoke the getter directly.

---

### What You Can Recover About Types

```javascript
il2cpp_type_get_name(type)              // "System.Collections.Generic.List`1"
il2cpp_type_get_type(type)              // the Il2CppTypeEnum (int, float, class, valuetype, etc)
il2cpp_type_get_class_or_element_class  // resolve the actual class
il2cpp_type_get_attrs(type)             // attribute flags
il2cpp_type_is_byref(type)              // ref parameter?
il2cpp_type_is_static(type)
il2cpp_type_is_pointer_type(type)
il2cpp_type_get_assembly_qualified_name // "MyClass, Assembly-CSharp, Version=0.0.0.0"
```

---

### Custom Attributes

This is huge — custom attributes on classes/methods/fields often contain game-design metadata:

```javascript
il2cpp_custom_attrs_from_class(klass)    // get attr bag for a class
il2cpp_custom_attrs_from_method(method)
il2cpp_custom_attrs_from_field(field)
il2cpp_custom_attrs_has_attr(attrInfo, attrClass)  // does it have [SerializeField]?
il2cpp_custom_attrs_get_attr(attrInfo, attrClass)  // get the attribute object itself
```

If the devs used attributes like `[Obfuscated]`, `[ServerOnly]`, `[CheatProtected]` you'll find them here.

---

### Live Object Inspection

Once you have a class definition you can interact with live instances:

```javascript
il2cpp_object_get_class(obj)            // what type is this object actually
il2cpp_object_get_size(obj)             // runtime size
il2cpp_object_get_virtual_method(obj, method) // resolve virtual dispatch
il2cpp_runtime_invoke(method, obj, args, exc) // call any method on any live object
il2cpp_object_new(klass)                // allocate a new instance
il2cpp_runtime_object_init(obj)         // call constructor
il2cpp_object_unbox(obj)                // get raw value from boxed valuetype
il2cpp_value_box(klass, val)            // box a value
```

---

### GC Handles (for holding references across calls)

```javascript
il2cpp_gchandle_new(obj, pinned)        // pin an object so GC doesn't move it
il2cpp_gchandle_get_target(handle)      // get the object back
il2cpp_gchandle_free(handle)            // release the pin
```

This is important in Frida because if you hold a pointer to a managed object across async calls, the GC might move it. Pin it first.

---

### Strings

```javascript
il2cpp_string_new("hello")          // create Il2CppString*
il2cpp_string_new_wrapper("hello")  // same but takes char*
il2cpp_string_chars(str)            // pointer to UTF-16 char array
il2cpp_string_length(str)           // character count (not bytes)
```

Reading: `chars.readUtf16String(length)` — IL2CPP strings are always UTF-16 internally.

---

## mono_* vs il2cpp_* — What's the Difference?

This is where it gets interesting. Both sets of exports exist in the same DLL, and that's by design.

### Historical Reason They Both Exist

Unity originally used the **Mono runtime** (an open-source .NET implementation). IL2CPP was introduced later as an ahead-of-time compiler that converts C# IL bytecode to C++. Unity ships both APIs as exports because:

1. Some internal Unity subsystems (especially the **debugger protocol**) were written against the Mono API and were never rewritten
2. Third-party tools (profilers, debuggers like MelonLoader, BepInEx) historically targeted Mono, so Unity maintains the shim for compatibility
3. The IL2CPP runtime implements Mono's public API surface as a compatibility layer

### What This Means Practically

Looking at the offset column in your export list (the third value), notice:

```
il2cpp_class_get_name,    48, 0x19bb0
mono_class_get_name,     275, 0x19bb0   ← same offset!

il2cpp_class_get_namespace, 49, 0x7e510
mono_class_get_namespace,  276, 0x7e510  ← same offset!

il2cpp_field_get_name,   94, 0xc8d0
mono_field_get_name,    302, 0xc8d0     ← same offset!
```

**They're literally the same functions at the same addresses.** The `mono_*` exports are aliases. The ones that show `0x2d90` (a tiny stub address) are the ones that are **not implemented** — they're no-ops or stubs that exist only to satisfy the export table.

### What `0x2d90` Actually Means

That repeated stub address (`0x2d90`) appears on functions like:

```
mono_arch_clear_breakpoint,    256, 0x2d90
mono_arch_set_breakpoint,      259, 0x2d90
mono_field_set_value,          306, 0x2d90
il2cpp_gc_set_external_allocation_tracker, 124, 0x2d90
```

These all point to the same address — almost certainly a single `ret` instruction or a small "not implemented" stub. Do not rely on these.

### Which to Use

**Use `il2cpp_*` exclusively for exploration.** Here's why:

- The `il2cpp_*` naming makes your intent clear
- The `mono_*` ones that actually work are identical under the hood
- The `mono_*` ones that don't work (`0x2d90`) could silently corrupt state if called

The only reason to use `mono_*` is if you're working with a tool that was written for Mono and you're adapting it — the shim layer means it'll still work.

---

## A Complete Exploration Script

Putting it all together, here's a script that dumps the entire managed type system to the console — effectively reconstructing the class/method/field structure of the game:

```javascript
const mod = Process.getModuleByName("GameAssembly.dll");
const e = name => new NativeFunction(mod.getExportByName(name), ...({
  "il2cpp_domain_get":            ["pointer", []],
  "il2cpp_domain_get_assemblies": ["pointer", ["pointer","pointer"]],
  "il2cpp_assembly_get_image":    ["pointer", ["pointer"]],
  "il2cpp_image_get_name":        ["pointer", ["pointer"]],
  "il2cpp_image_get_class_count": ["uint",    ["pointer"]],
  "il2cpp_image_get_class":       ["pointer", ["pointer","uint"]],
  "il2cpp_class_get_name":        ["pointer", ["pointer"]],
  "il2cpp_class_get_namespace":   ["pointer", ["pointer"]],
  "il2cpp_class_get_parent":      ["pointer", ["pointer"]],
  "il2cpp_class_get_flags":       ["uint",    ["pointer"]],
  "il2cpp_class_is_valuetype":    ["bool",    ["pointer"]],
  "il2cpp_class_is_enum":         ["bool",    ["pointer"]],
  "il2cpp_class_get_methods":     ["pointer", ["pointer","pointer"]],
  "il2cpp_class_get_fields":      ["pointer", ["pointer","pointer"]],
  "il2cpp_class_get_properties":  ["pointer", ["pointer","pointer"]],
  "il2cpp_method_get_name":       ["pointer", ["pointer"]],
  "il2cpp_method_get_flags":      ["uint",    ["pointer"]],
  "il2cpp_method_get_param_count":["uint",    ["pointer"]],
  "il2cpp_method_get_param_name": ["pointer", ["pointer","uint"]],
  "il2cpp_method_get_return_type":["pointer", ["pointer"]],
  "il2cpp_field_get_name":        ["pointer", ["pointer"]],
  "il2cpp_field_get_offset":      ["uint",    ["pointer"]],
  "il2cpp_field_get_flags":       ["uint",    ["pointer"]],
  "il2cpp_property_get_name":     ["pointer", ["pointer"]],
  "il2cpp_type_get_name":         ["pointer", ["pointer"]],
}[name]));

const fn = {
  domain_get:            e("il2cpp_domain_get"),
  domain_get_assemblies: e("il2cpp_domain_get_assemblies"),
  assembly_get_image:    e("il2cpp_assembly_get_image"),
  image_get_name:        e("il2cpp_image_get_name"),
  image_get_class_count: e("il2cpp_image_get_class_count"),
  image_get_class:       e("il2cpp_image_get_class"),
  class_get_name:        e("il2cpp_class_get_name"),
  class_get_namespace:   e("il2cpp_class_get_namespace"),
  class_get_parent:      e("il2cpp_class_get_parent"),
  class_get_flags:       e("il2cpp_class_get_flags"),
  class_is_valuetype:    e("il2cpp_class_is_valuetype"),
  class_is_enum:         e("il2cpp_class_is_enum"),
  class_get_methods:     e("il2cpp_class_get_methods"),
  class_get_fields:      e("il2cpp_class_get_fields"),
  class_get_properties:  e("il2cpp_class_get_properties"),
  method_get_name:       e("il2cpp_method_get_name"),
  method_get_flags:      e("il2cpp_method_get_flags"),
  method_get_param_count:e("il2cpp_method_get_param_count"),
  method_get_param_name: e("il2cpp_method_get_param_name"),
  method_get_return_type:e("il2cpp_method_get_return_type"),
  field_get_name:        e("il2cpp_field_get_name"),
  field_get_offset:      e("il2cpp_field_get_offset"),
  field_get_flags:       e("il2cpp_field_get_flags"),
  property_get_name:     e("il2cpp_property_get_name"),
  type_get_name:         e("il2cpp_type_get_name"),
};

function readStr(ptr) {
  if (!ptr || ptr.isNull()) return "<null>";
  try { return ptr.readUtf8String(); } catch(e) { return "<unreadable>"; }
}

function iterateIter(iterFn, klass) {
  const iter = Memory.alloc(Process.pointerSize);
  iter.writePointer(ptr(0));
  const results = [];
  let item;
  while (!(item = iterFn(klass, iter)).isNull()) results.push(item);
  return results;
}

setTimeout(() => {
  const domain = fn.domain_get();
  const countBuf = Memory.alloc(4);
  const assemblies = fn.domain_get_assemblies(domain, countBuf);
  const count = countBuf.readU32();

  for (let a = 0; a < count; a++) {
    const assembly = assemblies.add(a * Process.pointerSize).readPointer();
    const image = fn.assembly_get_image(assembly);
    if (image.isNull()) continue;

    const imageName = readStr(fn.image_get_name(image));
    // filter to game assembly only, skip Unity internals
    if (!imageName.includes("Assembly-CSharp")) continue;

    const classCount = fn.image_get_class_count(image);
    console.log(`\n=== ${imageName} (${classCount} classes) ===`);

    for (let c = 0; c < classCount; c++) {
      const klass = fn.image_get_class(image, c);
      if (klass.isNull()) continue;

      const ns    = readStr(fn.class_get_namespace(klass));
      const name  = readStr(fn.class_get_name(klass));
      const isVal = fn.class_is_valuetype(klass);
      const isEnum= fn.class_is_enum(klass);
      const keyword = isEnum ? "enum" : isVal ? "struct" : "class";

      const parent = fn.class_get_parent(klass);
      const parentName = parent.isNull() ? "" : ` : ${readStr(fn.class_get_name(parent))}`;

      console.log(`\n  ${keyword} ${ns ? ns+"." : ""}${name}${parentName}`);

      // Fields
      iterateIter(fn.class_get_fields, klass).forEach(field => {
        const fname  = readStr(fn.field_get_name(field));
        const offset = fn.field_get_offset(field);
        const flags  = fn.field_get_flags(field);
        const isStatic = (flags & 0x10) !== 0;
        console.log(`    [field] ${isStatic?"static ":""}${fname} @offset=0x${offset.toString(16)}`);
      });

      // Methods
      iterateIter(fn.class_get_methods, klass).forEach(method => {
        const mname  = readStr(fn.method_get_name(method));
        const flags  = fn.method_get_flags(method);
        const pcount = fn.method_get_param_count(method);
        const retType= fn.method_get_return_type(method);
        const retName= readStr(fn.type_get_name(retType));
        const isStatic = (flags & 0x10) !== 0;

        const params = [];
        for (let p = 0; p < pcount; p++) {
          const pname = readStr(fn.method_get_param_name(method, p));
          params.push(pname);
        }
        console.log(`    [method] ${isStatic?"static ":""}${retName} ${mname}(${params.join(", ")})`);
      });

      // Properties
      iterateIter(fn.class_get_properties, klass).forEach(prop => {
        const pname = readStr(fn.property_get_name(prop));
        console.log(`    [prop]   ${pname}`);
      });
    }
  }
}, 3000);
```

This will print something resembling the original C# source structure:

```
=== Assembly-CSharp (147 classes) ===

  class MyGame.FlagManager : MonoBehaviour
    [field] _secretKey @offset=0x18
    [field] static _instance @offset=0x0
    [method] static Void Awake()
    [method] String get_flag()
    [method] static String XorDecrypt(String ciphertext, String key)
    [prop]   Instance
```

At that point you know the exact method to invoke, the fields to read, and the memory offsets to inspect directly — even without any symbols in the binary.