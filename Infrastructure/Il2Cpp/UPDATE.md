# UPDATE — How to update Il2CppRpcAgent after a game patch

## Architecture

The agent uses a **hybrid offset strategy** to survive obfuscator name mangling:

- **Stable fields** (`Console`, `OtherPoke`, `TargetPos`, `TextList`, `onChange`)
  are discovered **by name** at runtime via the IL2CPP reflection API. These have
  human-readable names that never change.

- **Obfuscated fields** (`IsBattling`, `CurrentEncounterId`, `ShinyForm`, `EventForm`)
  use **hardcoded DSSock-relative offsets** in `HARDCODED_OFFSETS`. These offsets
  are stable across patches even when the obfuscator renames the fields — so
  **no code change is needed** when only names are mangled.

---

## Step 1 — Run the layout verifier

```powershell
frida -p <PID> -l .\Infrastructure\Il2Cpp\Diagnostics\verify_layout.js
```

The script:
1. Verifies that stable class/field names still resolve correctly.
2. Cross-references hardcoded offsets against actual fields to confirm the
   struct layout hasn't shifted.
3. Does a live read of all values using the same offsets as the agent.

### All `[+]` lines → done

If the output shows only `[+]` lines and the live read looks reasonable:

- **No code change needed.**
- Just rebuild: `dotnet build .\ProHack.sln -c Release`

### `[-]` on a stable field → Step 2

A stable field name was renamed (rare). Update `NAMED_FIELDS` in the agent.

### `[-]` on a hardcoded offset → Step 3

The struct layout changed (fields added/removed/reordered). Update `HARDCODED_OFFSETS`.

---

## Step 2 — Fix renamed stable fields (rare)

Open `Infrastructure/Il2Cpp/Agent/Il2CppRpcAgent.js`.

Update the value in `CLASS_NAMES` or `NAMED_FIELDS`:

```js
const CLASS_NAMES = {
    DSSock:    "DSSock",
    ChatInput: "ChatInput",
    UIWidget:  "UIWidget",
};

const NAMED_FIELDS = {
    console:   "Console",
    otherPoke: "OtherPoke",
    targetPos: "TargetPos",
    textList:  "TextList",
    onChange:  "onChange",
};
```

These field names are human-readable and have been stable across every observed
game patch. If one does change, update the string value on the right-hand side.

---

## Step 3 — Fix shifted struct layout (very rare)

If `verify_layout.js` reports `[-]` on a hardcoded offset, the struct layout
itself changed (fields were added, removed, or reordered).

### How to find new offsets

Regenerate dumps. You need a self compiled version of cheat engine that had all traces of the strings "cheat" "engine" and "ce" removed. After that you need to dissect mono and dump:

- everything into `Infrastructure\Il2Cpp\Diagnostics\Dumps\classes.txt`
- only Assembly-CSharp.txt into `Infrastructure\Il2Cpp\Diagnostics\Dumps\Assembly-Csharp.txt`
- only the DSSock class into `Infrastructure\Il2Cpp\Diagnostics\Dumps\DSSockInfo.txt`

Then look up the new offsets in the dumps and update `HARDCODED_OFFSETS`:

```js
const HARDCODED_OFFSETS = {
    isBattling:         0x750,  // System.Boolean — direct DSSock field
    currentEncounterId: 0x7D0,  // System.Int32   — OtherPoke + gw.field - 0x10
    shinyForm:          0x7E0,  // System.Boolean — OtherPoke + gw.field - 0x10
    eventForm:          0x7E4,  // System.Int32   — OtherPoke + gw.field - 0x10
};
```

Formula for gw (value-type) fields:
```
effective DSSock offset = OtherPoke offset + gw field offset − 0x10 (IL2CPP object header)
```

---

## Step 4 — Verify the delegate offset (*could theoretically* change)

`DELEGATE_SELECTED_MENU_OFFSET = 0xA8`

This offset only changes very rarely (Unity major-version upgrade). If
`SelectedMenu` always reads as an impossible value (e.g. −1, −2, large integer) run:

```powershell
frida -p <PID> -l .\Infrastructure\Il2Cpp\Diagnostics\find_onChange_delegate_fields.js
```

The script will:

1. Walk the chain to the `UIWidget.di` delegate instance.
2. Scan ±32 bytes around `0xA8` for values that are valid SelectedMenu integers.
3. Print the correct offset if it differs.

Then update `DELEGATE_SELECTED_MENU_OFFSET` in `Il2CppRpcAgent.js`.

---

## Step 5 — Rebuild

The agent is an EmbeddedResource — it is compiled into the DLL automatically:

```powershell
dotnet build .\ProHack.sln -c Release
```

No manual copy step is required.

---

## Quick reference — expected offsets

### Stable fields (discovered by name)

| Class       | Field      | Expected offset |
|-------------|------------|-----------------|
| `DSSock`    | `Console`  | `0x458`         |
| `DSSock`    | `OtherPoke`| `0x7D0`         |
| `DSSock`    | `TargetPos`| `0x1A4`         |
| `ChatInput` | `TextList` | `0x30`          |
| `UIWidget`  | `onChange`  | `0xB0`          |

### Hardcoded offsets (obfuscated fields — immune to name mangling)

| Value              | DSSock-relative offset | Notes                              |
|--------------------|------------------------|------------------------------------|
| IsBattling         | `0x750`                | Direct DSSock field (System.Boolean)|
| CurrentEncounterId | `0x7D0`                | Effective: OtherPoke+0x10−0x10     |
| ShinyForm          | `0x7E0`                | Effective: OtherPoke+0x20−0x10     |
| EventForm          | `0x7E4`                | Effective: OtherPoke+0x24−0x10     |
| SelectedMenu       | `0xA8`                 | Inside UIWidget.di delegate        |

---

## Adding a new readable value

1. Find the IL2CPP chain with a diagnostic script (use `verify_layout.js` as a template).
2. If the field has a stable name → add to `NAMED_FIELDS` and discover it in `_discoverLayout()`.
   If the field has an obfuscated name → add to `HARDCODED_OFFSETS` and use the offset directly.
3. Read the value in `_readGameState()` in the agent.
4. Expose the value from `GameStateSnapshot.cs` and `PROIl2CppManager.cs`.
5. Update this document with the new field.

---

## Misc

### "The agent sends back `ok: false`"

The agent script itself failed to find a class or field. Check the Frida log for
the error string returned in `layout.error`. A stable field name changed — go to Step 2.

**Note:** Obfuscated field renames can no longer cause this error since those
fields use hardcoded offsets. This error now only occurs if a stable class or
field name (DSSock, Console, OtherPoke, etc.) is renamed.

### "Build fails after changing Il2CppRpcAgent.js"

The `.js` file is not compiled by C#. A build failure after editing it means
another C# file has an issue. Compile errors come from C#, not the JS.

