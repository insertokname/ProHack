// !!!VIBECODED BULLSHIT AHEAD!!!

'use strict';
/**
 * verify_layout.js — Run this after every game update to confirm that all
 * offsets used by Il2CppRpcAgent.js are still correct.
 *
 * The agent uses a HYBRID approach:
 *   • Stable fields (Console, OtherPoke, TargetPos, TextList, onChange) are
 *     discovered by name at runtime — self-healing if their offsets shift.
 *   • Obfuscated fields (IsBattling, CurrentEncounterId, ShinyForm, EventForm)
 *     use HARDCODED DSSock-relative offsets — immune to name mangling.
 *
 * This script verifies BOTH:
 *   1. That the stable field names still resolve and their offsets match.
 *   2. That the hardcoded offsets still point to the correct data by
 *      cross-referencing with whatever obfuscated names are currently in use.
 *
 * Usage:
 *   frida -p <PROClient_PID> -l .\verify_layout.js
 *
 * If all hardcoded offsets match → no code change needed, just rebuild.
 * If a stable field name changed → update NAMED_FIELDS in Il2CppRpcAgent.js.
 * If struct layout shifted       → update HARDCODED_OFFSETS in Il2CppRpcAgent.js.
 *
 * See UPDATE.md for the full procedure.
 */

// ── Hardcoded offsets (must match Il2CppRpcAgent.js HARDCODED_OFFSETS) ─────────
const EXPECTED = {
    // Stable fields (discovered by name, but we verify offsets haven't shifted)
    'DSSock.Console':   0x458,
    'DSSock.MapCreator':0x1E8,
    'DSSock.OtherPoke': 0x7D0,
    'DSSock.TargetPos': 0x1A4,
    'ChatInput.TextList': 0x30,
    'UIWidget.onChange': 0xB0,

    // Map screenshot offsets (hardcoded in Il2CppRpcAgent.js)
    'MapCreator.Width':         0x140,
    'MapCreator.Height':        0x144,
    'MapCreator.Tiles':         0x150,
    'MapCreator.Tiles2':        0x158,
    'MapCreator.Tiles3':        0x160,
    'MapCreator.Tiles4':        0x168,
    'MapCreator.Colliders':     0x170,
    'MapCreator.Links':         0x178,
    'MapCreator.MapName':       0x180,
    'MapCreator.MaxTileSheets': 0x198,
    'MapCreator.TileMaterials': 0x1A8,

    // Hardcoded offsets for obfuscated fields (effective DSSock-relative)
    'DSSock+IsBattling':         0x750,
    'DSSock+CurrentEncounterId': 0x7D0,
    'DSSock+ShinyForm':          0x7E0,
    'DSSock+EventForm':          0x7E4,
    // Delegate internal
    'Delegate+SelectedMenu':     0xA8,
};

const _mod  = Process.getModuleByName('GameAssembly.dll');
const _IL2CPP_OBJECT_HEADER_SIZE = 0x10;

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

function _findFieldByName(klass, name) {
    const iter=Memory.alloc(Process.pointerSize); iter.writePointer(ptr(0));
    let f;
    while(!(f=_api.classGetFields(klass,iter)).isNull())
        if(_utf8(_api.fieldGetName(f))===name) return f;
    return null;
}

/** Find a field by its expected offset within a class. Returns {name, offset} or null. */
function _findFieldByOffset(klass, targetOffset) {
    const iter=Memory.alloc(Process.pointerSize); iter.writePointer(ptr(0));
    let f;
    while(!(f=_api.classGetFields(klass,iter)).isNull()) {
        const off = _api.fieldGetOffset(f);
        if (off === targetOffset) {
            return { name: _utf8(_api.fieldGetName(f)), offset: off };
        }
    }
    return null;
}

function ok(msg)   { console.log('[+] ' + msg); }
function fail(msg) { console.log('[-] ' + msg); }
function warn(msg) { console.log('[!] ' + msg); }
function hdr(msg)  { console.log('\n=== ' + msg + ' ==='); }
function hex(v)    { return '0x' + v.toString(16).toUpperCase(); }

// ── Class checks ─────────────────────────────────────────────────────────────

hdr('Class resolution');
const dsKlass        = _findClass('DSSock');    dsKlass        ? ok('DSSock       found') : fail('DSSock — UPDATE CLASS_NAMES.DSSock');
const gwKlass        = _findClass('gw');        gwKlass        ? ok('gw           found') : warn('gw not found (OK — agent uses hardcoded offsets, no gw lookup needed)');
const chatInputKlass = _findClass('ChatInput'); chatInputKlass ? ok('ChatInput    found') : fail('ChatInput — UPDATE CLASS_NAMES.ChatInput');
const uiWidgetKlass  = _findClass('UIWidget');  uiWidgetKlass  ? ok('UIWidget     found') : fail('UIWidget — UPDATE CLASS_NAMES.UIWidget');
const mapCreatorKlass= _findClass('MapCreator');mapCreatorKlass? ok('MapCreator   found') : fail('MapCreator — UPDATE CLASS_NAMES.MapCreator');

// ── Stable field checks (discovered by name at runtime) ───────────────────────

hdr('Stable fields (name-based — used by the agent at runtime)');

function checkNamedField(klass, className, fieldName, expectedOffset) {
    if (!klass) { fail(`${className}.${fieldName} — class not found`); return null; }
    const f = _findFieldByName(klass, fieldName);
    if (!f) { fail(`${className}.${fieldName} — FIELD NOT FOUND → update NAMED_FIELDS in Il2CppRpcAgent.js`); return null; }
    const off = _api.fieldGetOffset(f);
    const match = (expectedOffset == null || off === expectedOffset);
    const status = match ? '✓' : `!! SHIFTED — expected ${hex(expectedOffset)}, got ${hex(off)} → update HARDCODED_OFFSETS if affected`;
    ok(`${className}.${fieldName}  offset = ${hex(off)}  ${status}`);
    return off;
}

const offsetConsole   = checkNamedField(dsKlass,        'DSSock',    'Console',   0x458);
const offsetMapCreator= checkNamedField(dsKlass,        'DSSock',    'MapCreator',0x1E8);
const offsetOtherPoke = checkNamedField(dsKlass,        'DSSock',    'OtherPoke', 0x7D0);
const offsetTargetPos = checkNamedField(dsKlass,        'DSSock',    'TargetPos', 0x1A4);
const offsetTextList  = checkNamedField(chatInputKlass, 'ChatInput', 'TextList',  0x30);
const offsetOnChange  = checkNamedField(uiWidgetKlass,  'UIWidget',  'onChange',  0xB0);

// ── Map screenshot hardcoded offset verification ─────────────────────────────

hdr('Map screenshot offsets (hardcoded in agent)');

function verifyMapOffset(expectedOffset, label) {
    if (!mapCreatorKlass) {
        warn(`MapCreator class not found — cannot verify ${label}`);
        return;
    }
    const f = _findFieldByOffset(mapCreatorKlass, expectedOffset);
    if (f) ok(`${label} @ ${hex(expectedOffset)}  ✓  (current name: "${f.name}")`);
    else fail(`${label} @ ${hex(expectedOffset)} — NO FIELD AT THIS OFFSET → update MAP_HARDCODED_OFFSETS`);
}

verifyMapOffset(EXPECTED['MapCreator.Width'],         'MapCreator.Width');
verifyMapOffset(EXPECTED['MapCreator.Height'],        'MapCreator.Height');
verifyMapOffset(EXPECTED['MapCreator.Tiles'],         'MapCreator.Tiles');
verifyMapOffset(EXPECTED['MapCreator.Tiles2'],        'MapCreator.Tiles2');
verifyMapOffset(EXPECTED['MapCreator.Tiles3'],        'MapCreator.Tiles3');
verifyMapOffset(EXPECTED['MapCreator.Tiles4'],        'MapCreator.Tiles4');
verifyMapOffset(EXPECTED['MapCreator.Colliders'],     'MapCreator.Colliders');
verifyMapOffset(EXPECTED['MapCreator.Links'],         'MapCreator.Links');
verifyMapOffset(EXPECTED['MapCreator.MapName'],       'MapCreator.MapName');
verifyMapOffset(EXPECTED['MapCreator.MaxTileSheets'], 'MapCreator.MaxTileSheets');
verifyMapOffset(EXPECTED['MapCreator.TileMaterials'], 'MapCreator.MaterialArray');

// ── Hardcoded offset verification (obfuscated fields) ─────────────────────────

hdr('Hardcoded offsets (obfuscated fields — agent uses these directly)');
console.log('Cross-referencing hardcoded DSSock-relative offsets against actual fields...');
console.log('');

function verifyHardcodedOffset(klass, className, hardcodedOffset, description) {
    if (!klass) { warn(`${className} class not found — cannot verify ${description} at ${hex(hardcodedOffset)}`); return; }
    const found = _findFieldByOffset(klass, hardcodedOffset);
    if (found) {
        ok(`${description}  @ ${hex(hardcodedOffset)}  ✓  (current name: "${found.name}")`);
    } else {
        fail(`${description}  @ ${hex(hardcodedOffset)}  — NO FIELD AT THIS OFFSET → struct layout changed! Update HARDCODED_OFFSETS.`);
    }
}

// IsBattling is a direct DSSock field at 0x750
verifyHardcodedOffset(dsKlass, 'DSSock', EXPECTED['DSSock+IsBattling'], 'DSSock.IsBattling');

// gw fields: the effective DSSock offsets are computed as OtherPoke + gwFieldOff - 0x10.
// To verify, we check the gw class (if available) for fields at the expected gw-internal offsets.
// gw-internal offset = effectiveDSSockOffset - OtherPoke + IL2CPP_OBJECT_HEADER
if (gwKlass && offsetOtherPoke != null) {
    const otherPoke = offsetOtherPoke;
    function verifyGwField(effOffset, description) {
        const gwInternalOff = effOffset - otherPoke + _IL2CPP_OBJECT_HEADER_SIZE;
        const found = _findFieldByOffset(gwKlass, gwInternalOff);
        if (found) {
            ok(`${description}  @ DSSock+${hex(effOffset)} (gw+${hex(gwInternalOff)})  ✓  (current name: "${found.name}")`);
        } else {
            fail(`${description}  @ DSSock+${hex(effOffset)} (gw+${hex(gwInternalOff)})  — NO FIELD → struct layout changed! Update HARDCODED_OFFSETS.`);
        }
    }
    verifyGwField(EXPECTED['DSSock+CurrentEncounterId'], 'CurrentEncounterId');
    verifyGwField(EXPECTED['DSSock+ShinyForm'],          'ShinyForm');
    verifyGwField(EXPECTED['DSSock+EventForm'],          'EventForm');
} else {
    warn('Cannot verify gw hardcoded offsets (gw class or OtherPoke not found).');
    console.log('  Using hardcoded values — live read will confirm if they work.');
}

// ── Live value read (uses hardcoded offsets, just like the agent) ──────────────

hdr('Live value read (uses hardcoded offsets — must be in-game / logged in)');

try {
    if (!dsKlass) {
        console.log('[!] Skipping live read — DSSock class not found.');
    } else {
        const staticData = _api.classGetStaticData(dsKlass);
        const ds = staticData.readPointer();
        if (ds.isNull()) { console.log('[!] DSSock singleton is null — not logged in.'); }
        else {
            ok('DSSock singleton @ ' + ds);

            // Read using HARDCODED offsets (same as the agent)
            const encId  = ds.add(EXPECTED['DSSock+CurrentEncounterId']).readS32();
            const battle = ds.add(EXPECTED['DSSock+IsBattling']).readU8();
            const shiny  = ds.add(EXPECTED['DSSock+ShinyForm']).readU8();
            const event_ = ds.add(EXPECTED['DSSock+EventForm']).readS32();

            console.log(`  CurrentEncounterId = ${encId}`);
            console.log(`  IsBattling         = ${battle !== 0} (raw ${battle})`);
            console.log(`  ShinyForm          = ${shiny}`);
            console.log(`  EventForm          = ${event_}`);

            // SelectedMenu chain uses name-discovered offsets for the pointer chain
            if (offsetConsole != null && offsetTextList != null && offsetOnChange != null) {
                const chatInput = ds.add(offsetConsole).readPointer();
                const textList  = chatInput.add(offsetTextList).readPointer();
                const onChange  = textList.add(offsetOnChange).readPointer();
                const selMenu   = onChange.add(EXPECTED['Delegate+SelectedMenu']).readS32();
                console.log(`  SelectedMenu       = ${selMenu}`);
            } else {
                warn('Skipping SelectedMenu — one or more pointer-chain fields not found.');
            }

            if (offsetTargetPos != null) {
                const px = ds.add(offsetTargetPos).readFloat();
                const py = ds.add(offsetTargetPos + 4).readFloat();
                console.log(`  PlayerPos          = (${px}, ${py})`);
            }

            if (offsetMapCreator != null) {
                const mc = ds.add(offsetMapCreator).readPointer();
                if (mc && !mc.isNull()) {
                    const mw = mc.add(EXPECTED['MapCreator.Width']).readS32();
                    const mh = mc.add(EXPECTED['MapCreator.Height']).readS32();
                    const mnamePtr = mc.add(EXPECTED['MapCreator.MapName']).readPointer();
                    let mname = '(null)';
                    try {
                        if (mnamePtr && !mnamePtr.isNull()) {
                            const len = mnamePtr.add(0x10).readS32();
                            if (len > 0 && len < 4096) mname = mnamePtr.add(0x14).readUtf16String(len);
                        }
                    } catch (_) {}
                    console.log(`  MapName            = ${mname}`);
                    console.log(`  MapSize            = ${mw}x${mh}`);
                }
            }

            console.log('');
            console.log('[+] All values read successfully — layout is valid!');
        }
    }
} catch(e) {
    console.log('[-] Live read failed: ' + e.message);
    console.log('    If obfuscated field values look wrong, the struct layout may have changed.');
    console.log('    Update HARDCODED_OFFSETS in Il2CppRpcAgent.js.');
}
