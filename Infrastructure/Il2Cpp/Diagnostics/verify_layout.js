// !!!VIBECODED BULLSHIT AHEAD!!!

'use strict';
/**
 * verify_layout.js — Run this after every game update to confirm that all
 * class/field names used by Il2CppRpcAgent.js still resolve correctly.
 *
 * Usage:
 *   frida -p <PROClient_PID> -l .\verify_layout.js
 *
 * Expected output (if nothing has changed):
 *   [+] DSSock       found
 *   [+] gw           found
 *   [+] ChatInput    found
 *   [+] UIWidget     found
 *   [+] DSSock.Console       offset = 0x458
 *   [+] DSSock.OtherPoke     offset = 0x7D0
 *   [+] DSSock.ply           offset = 0x750
 *   [+] gw.oyu               offset = 0x10   → effective DS offset = 0x7D0
 *   [+] gw.oyy               offset = 0x20   → effective DS offset = 0x7E0
 *   [+] gw.oyz               offset = 0x24   → effective DS offset = 0x7E4
 *   [+] ChatInput.TextList   offset = 0x30
 *   [+] UIWidget.onChange    offset = 0xB0
 *   ... live read of all values ...
 *
 * If any line shows [-] instead of [+], update the CLASS_NAMES / FIELD_NAMES
 * constants in Il2CppRpcAgent.js and re-run until all pass.
 * Then rebuild the C# project so the new agent is embedded.
 *
 * See UPDATE.md for the full procedure.
 */

const _mod  = Process.getModuleByName('GameAssembly.dll');
const _IL2CPP_OBJECT_HEADER_SIZE = 0x10;

const _api = {
    domainGet:           new NativeFunction(_mod.getExportByName('il2cpp_domain_get'),                  'pointer', []),
    domainGetAssemblies: new NativeFunction(_mod.getExportByName('il2cpp_domain_get_assemblies'),       'pointer', ['pointer', 'pointer']),
    assemblyGetImage:    new NativeFunction(_mod.getExportByName('il2cpp_assembly_get_image'),          'pointer', ['pointer']),
    imageGetClassCount:  new NativeFunction(_mod.getExportByName('il2cpp_image_get_class_count'),       'uint',    ['pointer']),
    imageGetClass:       new NativeFunction(_mod.getExportByName('il2cpp_image_get_class'),             'pointer', ['pointer', 'uint']),
    classGetName:        new NativeFunction(_mod.getExportByName('il2cpp_class_get_name'),              'pointer', ['pointer']),
    classGetFields:      new NativeFunction(_mod.getExportByName('il2cpp_class_get_fields'),            'pointer', ['pointer', 'pointer']),
    fieldGetName:        new NativeFunction(_mod.getExportByName('il2cpp_field_get_name'),              'pointer', ['pointer']),
    fieldGetOffset:      new NativeFunction(_mod.getExportByName('il2cpp_field_get_offset'),            'uint',    ['pointer']),
    fieldStaticGet:      new NativeFunction(_mod.getExportByName('il2cpp_field_static_get_value'),      'void',    ['pointer', 'pointer']),
    classGetStaticData:  new NativeFunction(_mod.getExportByName('il2cpp_class_get_static_field_data'), 'pointer', ['pointer']),
};

function _utf8(p) { try { return (p&&!p.isNull())?p.readUtf8String():null; } catch(_){return null;} }

function _findClass(name) {
    const domain=_api.domainGet(), cb=Memory.alloc(4);
    const ptrs=_api.domainGetAssemblies(domain,cb), n=cb.readU32();
    for(let a=0;a<n;a++){
        const asm=ptrs.add(a*Process.pointerSize).readPointer();
        const img=_api.assemblyGetImage(asm);
        if(!img||img.isNull()) continue;
        const cc=_api.imageGetClassCount(img);
        for(let c=0;c<cc;c++){
            const k=_api.imageGetClass(img,c);
            if(k&&!k.isNull()&&_utf8(_api.classGetName(k))===name) return k;
        }
    }
    return null;
}

function _findField(klass, name) {
    const iter=Memory.alloc(Process.pointerSize); iter.writePointer(ptr(0));
    let f;
    while(!(f=_api.classGetFields(klass,iter)).isNull())
        if(_utf8(_api.fieldGetName(f))===name) return f;
    return null;
}

function ok(msg)  { console.log('[+] ' + msg); }
function fail(msg){ console.log('[-] ' + msg); }
function hdr(msg) { console.log('\n=== ' + msg + ' ==='); }

// ── Class checks ─────────────────────────────────────────────────────────────

hdr('Class resolution');
const dsKlass        = _findClass('DSSock');    dsKlass        ? ok('DSSock')    : fail('DSSock — UPDATE CLASS_NAMES.DSSock');
const gwKlass        = _findClass('gw');        gwKlass        ? ok('gw')        : fail('gw — UPDATE CLASS_NAMES.gw');
const chatInputKlass = _findClass('ChatInput'); chatInputKlass ? ok('ChatInput') : fail('ChatInput — UPDATE CLASS_NAMES.ChatInput');
const uiWidgetKlass  = _findClass('UIWidget');  uiWidgetKlass  ? ok('UIWidget')  : fail('UIWidget — UPDATE CLASS_NAMES.UIWidget');

// ── Field offset checks ───────────────────────────────────────────────────────

hdr('Field offsets');

function checkField(klass, className, fieldName, expectedOffset) {
    if (!klass) { fail(`${className}.${fieldName} — class not found`); return null; }
    const f = _findField(klass, fieldName);
    if (!f) { fail(`${className}.${fieldName} — FIELD NOT FOUND, UPDATE FIELD_NAMES`); return null; }
    const off = _api.fieldGetOffset(f);
    const match = (expectedOffset == null || off === expectedOffset);
    ok(`${className}.${fieldName}  offset = 0x${off.toString(16).toUpperCase()}` +
       (expectedOffset != null ? (match ? '  ✓' : `  !! expected 0x${expectedOffset.toString(16).toUpperCase()}`) : ''));
    return off;
}

const offsetConsole   = checkField(dsKlass,        'DSSock',    'Console',   0x458);
const offsetOtherPoke = checkField(dsKlass,        'DSSock',    'OtherPoke', 0x7D0);
const offsetPly       = checkField(dsKlass,        'DSSock',    'ply',       0x750);
const gwOyuOff        = checkField(gwKlass,        'gw',        'oyu',       0x10);
const gwOyyOff        = checkField(gwKlass,        'gw',        'oyy',       0x20);
const gwOyzOff        = checkField(gwKlass,        'gw',        'oyz',       0x24);
const offsetTextList  = checkField(chatInputKlass, 'ChatInput', 'TextList',  0x30);
const offsetOnChange  = checkField(uiWidgetKlass,  'UIWidget',  'onChange',  0xB0);

if (gwOyuOff != null && offsetOtherPoke != null) {
    const eff = offsetOtherPoke + gwOyuOff - _IL2CPP_OBJECT_HEADER_SIZE;
    ok(`gw.oyu → effective DS offset = 0x${eff.toString(16).toUpperCase()} (expect 0x7D0)`);
}
if (gwOyyOff != null && offsetOtherPoke != null) {
    const eff = offsetOtherPoke + gwOyyOff - _IL2CPP_OBJECT_HEADER_SIZE;
    ok(`gw.oyy → effective DS offset = 0x${eff.toString(16).toUpperCase()} (expect 0x7E0)`);
}
if (gwOyzOff != null && offsetOtherPoke != null) {
    const eff = offsetOtherPoke + gwOyzOff - _IL2CPP_OBJECT_HEADER_SIZE;
    ok(`gw.oyz → effective DS offset = 0x${eff.toString(16).toUpperCase()} (expect 0x7E4)`);
}

// ── Live value read ───────────────────────────────────────────────────────────

hdr('Live value read (must be in-game / logged in)');

try {
    if (!dsKlass || !offsetConsole || !offsetOtherPoke || !offsetPly ||
        !gwOyuOff || !gwOyyOff || !gwOyzOff || !offsetTextList || !offsetOnChange) {
        console.log('[!] Skipping live read — one or more lookups failed above.');
    } else {
        // Read DSSock singleton
        const staticData = _api.classGetStaticData(dsKlass);
        const ds = staticData.readPointer();
        if (ds.isNull()) { console.log('[!] DSSock singleton is null — not logged in.'); }
        else {
            ok('DSSock singleton @ ' + ds);

            const encId    = ds.add(offsetOtherPoke + gwOyuOff  - _IL2CPP_OBJECT_HEADER_SIZE).readS32();
            const ply      = ds.add(offsetPly).readS32();
            const shiny    = ds.add(offsetOtherPoke + gwOyyOff  - _IL2CPP_OBJECT_HEADER_SIZE).readS32();
            const event_   = ds.add(offsetOtherPoke + gwOyzOff  - _IL2CPP_OBJECT_HEADER_SIZE).readS32();

            const chatInput = ds.add(offsetConsole).readPointer();
            const textList  = chatInput.add(offsetTextList).readPointer();
            const onChange  = textList.add(offsetOnChange).readPointer();
            const selMenu   = onChange.add(0xA8).readS32();

            console.log(`  CurrentEncounterId = ${encId}`);
            console.log(`  IsBattling (ply)   = ${ply !== 0} (raw ${ply})`);
            console.log(`  ShinyForm  (oyy)   = ${shiny}`);
            console.log(`  EventForm  (oyz)   = ${event_}`);
            console.log(`  SelectedMenu       = ${selMenu}`);
            console.log('');
            console.log('[+] All values read successfully — layout is valid!');
        }
    }
} catch(e) {
    console.log('[-] Live read failed: ' + e.message);
}
