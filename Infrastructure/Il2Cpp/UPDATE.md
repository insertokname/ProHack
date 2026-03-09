# UPDATE — How to update Il2CppRpcAgent after a game patch

## Step 1 — Run the layout verifier

```powershell
frida -p <PID> -l .\Infrastructure\Il2Cpp\Diagnostics\verify_layout.js
```

The script checks every class and field by name, prints the resolved offsets (with expected values), and does a live read of all five values. Look for any lines starting with `[-]`.

### All pass → done

If the output shows only `[+]` lines and the live read looks reasonable:

- **No code change needed.**
- Do a `dotnet build` and the embedded agent is already correct.

### Some fail → continue to Step 2

---

## Step 2 — Fix renamed classes or fields

Open `Infrastructure/Il2Cpp/Agent/Il2CppRpcAgent.js`.

At the very top you will find two constant blocks:

```js
const CLASS_NAMES = {
    DSSock: "DSSock",
    gw: "gw",
    ChatInput: "ChatInput",
    UIWidget: "UIWidget",
};

const FIELD_NAMES = {
    Console: "Console",
    OtherPoke: "OtherPoke",
    ply: "pmh",
    oyu: "ozd", // CurrentEncounterId
    oyy: "ozh", // ShinyForm
    oyz: "ozi", // EventForm
    TextList: "TextList",
    onChange: "onChange",
};
```

If a class or field was renamed, update the **value** string on the right-hand side. Everything else in the file uses these constants — no other changes are needed.

### How to find the new name

You need to regenerate dumps. You need a self compiled version of cheat engine that had all traces of the strings "cheat" "engine" and "ce" removed. After that you need to dissect mono and dump:

- everything into `Infrastructure\Il2Cpp\Diagnostics\Dumps\classes.txt`
- only Assembly-CSharp.txt into `Infrastructure\Il2Cpp\Diagnostics\Dumps\Assembly-Csharp.txt`
- onyl the DSSock class into `Infrastructure\Il2Cpp\Diagnostics\Dumps\DSSockInfo.txt`

Use `Assembly-CSharp.txt`:

```
grep -i "currentencountered\|shinyform\|eventform" Assembly-CSharp.txt
```

Or use the Diagnostics script which will print the name it could NOT resolve:

```
[-] Field gw.oyy not found
```

---

## Step 3 — Verify the one hardcoded offset (*could theoretically* change)

`Il2CppLayout.DelegateSelectedMenuOffset = 0xA8`

This offset only changes very rarely. If `SelectedMenu` always reads as an impossible value (e.g. −1, −2, large integer) run:

```powershell
frida -p <PID> -l .\Infrastructure\Il2Cpp\Diagnostics\find_onChange_delegate_fields.js
```

The script will:

1. Walk the chain to the `UIWidget.di` delegate instance.
2. Scan ±32 bytes around `0xA8` for values that are valid SelectedMenu integers.
3. Print the correct offset if it differs.

Then update in `Infrastructure/Il2Cpp/Core/Il2CppLayout.cs`:

```csharp
public const int DelegateSelectedMenuOffset = 0xA8;  // ← change this value
```

---

## Step 4 — Rebuild

The agent is an EmbeddedResource — it is compiled into the DLL automatically:

```powershell
dotnet build .\ProHack.sln -c Release
```

No manual copy step is required.

---

## Quick reference — expected field offsets

These are the values `verify_layout.js` checks against. If they match, no update is needed.

| Class         | Field                          | Expected offset                      |
| ------------- | ------------------------------ | ------------------------------------ |
| `DSSock`      | `Console`                      | `0x458`                              |
| `DSSock`      | `OtherPoke`                    | `0x7D0`                              |
| `DSSock`      | `pmh`                          | `0x750`                              |
| `gw`          | `ozd`                          | `0x10` → effective DS offset `0x7D0` |
| `gw`          | `ozh`                          | `0x20` → effective DS offset `0x7E0` |
| `gw`          | `ozi`                          | `0x24` → effective DS offset `0x7E4` |
| `ChatInput`   | `TextList`                     | `0x30`                               |
| `UIWidget`    | `onChange`                     | `0xB0`                               |
| `UIWidget.di` | _(MulticastDelegate internal)_ | `0xA8`                               |

---

## Adding a new readable value

1. Find the IL2CPP chain with a diagnostic script (use the `find*.js` scrips from `DebugScripts/` `verify_layout.js` as a template).
2. Add the class/field name to `CLASS_NAMES` / `FIELD_NAMES` in `Il2CppRpcAgent.js`.
3. Return the new offset from `_discoverLayout()` in the agent.
4. Add the offset property to `Il2CppLayout.cs` and parse it in `FromJson()`.
5. Read the value in `Il2CppReader.ReadCore()`.
6. Expose it from `GameStateSnapshot.cs` and `PROIl2CppManager.cs`.
7. Update `UPDATE.md` with the new field name.

---

## Misc

### "The agent sends back `ok: false`"

The agent script itself failed to find a class or field. Check the Frida log for the error string returned in `layout.error`. The class or field name changed — go to Step 2.

### "Build fails after changing Il2CppRpcAgent.js"

The `.js` file is not compiled by C#. A build failure after editing it means another C# file has an issue (e.g. you also changed `Il2CppLayout.cs`). Compile errors come from C#, not the JS.

