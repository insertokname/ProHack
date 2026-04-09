// !!!VIBECODED BULLSHIT AHEAD!!!

'use strict';
/**
 * find_onChange_delegate_fields.js — Identify the field at UIWidget.di+0xA8.
 *
 * Currently the 0xA8 offset from the UIWidget.di delegate object to the
 * SelectedMenu int is the ONE remaining hardcoded constant in the Il2Cpp layer.
 * Run this script after a Unity engine version upgrade to verify or update it.
 *
 * Usage:
 *   frida -p <PROClient_PID> -l .\find_onChange_delegate_fields.js
 *
 * The script:
 *   1. Walks the DSSock → Console → TextList → onChange chain to reach the
 *      live UIWidget.di delegate instance.
 *   2. Dumps the 32 bytes around offset 0xA8 so you can see what value is there.
 *   3. Tries to read SelectedMenu at the current assumed offset (0xA8) and
 *      compares it to the expected range.
 *   4. Scans +/- 32 bytes around 0xA8 for any int value that looks like a
 *      valid SelectedMenu (0, 41, 42, 46, 47, 52, 55, 61).
 *
 * If the expected offset is wrong, look for the valid SelectedMenu value in
 * the scan output, note its offset, and update:
 *   Il2CppLayout.DelegateSelectedMenuOffset  in  Core/Il2CppLayout.cs
 */

const _mod  = Process.getModuleByName('GameAssembly.dll');
const VALID_MENU_VALUES = new Set([0, 41, 42, 46, 47, 52, 55, 61]);
const CURRENT_ASSUMED_OFFSET = 0xA8;

// Some builds expose these symbols in enumerateExports() but getExportByName()
// may fail for specific names. Keep a fallback map for reliable diagnostics.
const _exportsByName = Object.create(null);
for (const exp of _mod.enumerateExports()) {
    if (exp.type === 'function' && _exportsByName[exp.name] === undefined) {
        _exportsByName[exp.name] = exp.address;
    }
}

function _resolveExport(name) {
    try {
        return _mod.getExportByName(name);
    } catch (_) {
        const addr = _exportsByName[name];
        if (addr !== undefined) return addr;
        throw new Error(`Required IL2CPP export not found: "${name}"`);
    }
}

const _api = {
    domainGet:           new NativeFunction(_resolveExport('il2cpp_domain_get'),                  'pointer', []),
    domainGetAssemblies: new NativeFunction(_resolveExport('il2cpp_domain_get_assemblies'),       'pointer', ['pointer', 'pointer']),
    assemblyGetImage:    new NativeFunction(_resolveExport('il2cpp_assembly_get_image'),          'pointer', ['pointer']),
    imageGetClassCount:  new NativeFunction(_resolveExport('il2cpp_image_get_class_count'),       'uint',    ['pointer']),
    imageGetClass:       new NativeFunction(_resolveExport('il2cpp_image_get_class'),             'pointer', ['pointer', 'uint']),
    classGetName:        new NativeFunction(_resolveExport('il2cpp_class_get_name'),              'pointer', ['pointer']),
    classGetFields:      new NativeFunction(_resolveExport('il2cpp_class_get_fields'),            'pointer', ['pointer', 'pointer']),
    fieldGetName:        new NativeFunction(_resolveExport('il2cpp_field_get_name'),              'pointer', ['pointer']),
    fieldGetOffset:      new NativeFunction(_resolveExport('il2cpp_field_get_offset'),            'uint',    ['pointer']),
    classGetStaticData:  new NativeFunction(_resolveExport('il2cpp_class_get_static_field_data'), 'pointer', ['pointer']),
};

function _utf8(p) { try { return (p&&!p.isNull())?p.readUtf8String():null; } catch(_){return null;} }

function _findClass(name) {
    const cb=Memory.alloc(4), ptrs=_api.domainGetAssemblies(_api.domainGet(),cb), n=cb.readU32();
    for(let a=0;a<n;a++){
        const asm=ptrs.add(a*Process.pointerSize).readPointer(), img=_api.assemblyGetImage(asm);
        if(!img||img.isNull()) continue;
        const cc=_api.imageGetClassCount(img);
        for(let c=0;c<cc;c++){
            const k=_api.imageGetClass(img,c);
            if(k&&!k.isNull()&&_utf8(_api.classGetName(k))===name) return k;
        }
    }
    return null;
}

function _fieldOff(klass, name) {
    const iter=Memory.alloc(Process.pointerSize); iter.writePointer(ptr(0)); let f;
    while(!(f=_api.classGetFields(klass,iter)).isNull())
        if(_utf8(_api.fieldGetName(f))===name) return _api.fieldGetOffset(f);
    throw new Error('Field not found: '+name);
}

try {
    const dsKlass   = _findClass('DSSock');
    const ciKlass   = _findClass('ChatInput');
    const uiwKlass  = _findClass('UIWidget');

    const staticData   = _api.classGetStaticData(dsKlass);
    const ds           = staticData.readPointer();
    if (ds.isNull()) throw new Error('DSSock singleton is null — log in first.');

    const offsetConsole   = _fieldOff(dsKlass,  'Console');
    const offsetTextList  = _fieldOff(ciKlass,  'TextList');
    const offsetOnChange  = _fieldOff(uiwKlass, 'onChange');

    const chatInput = ds.add(offsetConsole).readPointer();
    const textList  = chatInput.add(offsetTextList).readPointer();
    const onChange  = textList.add(offsetOnChange).readPointer();

    console.log('UIWidget.di instance @ ' + onChange);

    // Current assumed offset
    const curVal = onChange.add(CURRENT_ASSUMED_OFFSET).readS32();
    const status = VALID_MENU_VALUES.has(curVal) ? '[OK]' : '[WRONG]';
    console.log(`\n${status}  offset 0x${CURRENT_ASSUMED_OFFSET.toString(16)} → SelectedMenu = ${curVal}`);
    if (!VALID_MENU_VALUES.has(curVal)) {
        console.log('      Expected one of: ' + [...VALID_MENU_VALUES].join(', '));
    }

    // Scan ±32 bytes around 0xA8
    console.log('\nScan ±0x20 bytes around 0xA8:');
    for (let off = CURRENT_ASSUMED_OFFSET - 0x20; off <= CURRENT_ASSUMED_OFFSET + 0x20; off += 4) {
        const v = onChange.add(off).readS32();
        const mark = VALID_MENU_VALUES.has(v) ? '  ← VALID SelectedMenu value' : '';
        console.log(`  +0x${off.toString(16).padStart(3,'0')}  =  ${v}${mark}`);
    }

    console.log('\nIf the current offset 0xA8 is [WRONG], find the ← VALID line above,');
    console.log('note its offset, and update Il2CppLayout.DelegateSelectedMenuOffset in Core/Il2CppLayout.cs.');

} catch(e) {
    console.log('Error: ' + e.message);
}
