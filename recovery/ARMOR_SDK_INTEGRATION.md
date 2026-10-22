# ArmorTemplate SDK plugin integration

`src/Plugin/ArmorPlugin.cs` implements the real `IAssetBuilderPlugin` interface.
The assembly is `Recovered.Compiler.dll`; it calls the reconstructed native /
C++/CLI `Recovered.Armor.dll`, never the original Tokenizer. It is registered
only for ArmorTemplate. WeaponTemplate and GameObject remain on the original
Tokenizer, and the other types remain on the original XmlCompiler.

The plugin retains the original ArmorTemplate type ID (980180622), type hash
(1679479598), platform version and processing hash. `GetAllTypesHash` retains the
original metadata constant; that value does not assert support for other types.
ProcessInstance rejects unsupported type IDs through the component's check.
Initialize and ReInitialize both set the active target platform.

## Reproduce

In an x86 VS2022 developer PowerShell with C++/CLI and .NET Framework 4.x:

```powershell
$env:RUNNER_TEMP = 'C:/Temp/ra3-recovery'
./recovery/scripts/TestArmorSdk.ps1
```

This builds the component and formal plugin using `/Od` and `/O2`; both run the
existing original-ProcessInstance differential oracle through the plugin.
The oracle also compares every public Armor type metadata field and platform
version across Win32, Xbox360, PlayStation3 and unknown platform 99.

`TestArmorSdk.ps1` then creates two separate disposable SDKs. The original and
recovered passes use identical committed mymod source, newly created output,
no shared generated caches and the same BAB basic-data flags as defaultscript
case 5. Temporary CLR4 legacy mixed-mode activation is the same in both passes.
The checked-in SDK compiler binaries and default plugin configuration are not
replaced by the test.

`SDK/Mods/mymod/Data/RecoveryArmorProbe.xml` is included from `Mod.xml`. The
existing mytank ArmorSet references `RecoveryArmorProbe_NonDefault`, ensuring a
real GameObject references the newly compiled type. The probe contains an empty
armor, distinct non-default percentages and four different damage entries, and
an inherited armor that exercises BAB's preprocessing before plugin invocation.
The original sample content is otherwise retained.

The new probe and Mod.xml are validated directly against the complete
`SDK/Schemas/xsd/CnC3Types.xsd` with relative includes/imports. Existing mytank.xml
contains an `xai:joinAction` directive; BAB processes that directive before its
own schema validation. Raw validation of that existing directive against the
asset-only schema is not equivalent to BAB's input pipeline. Neither schemas
nor SDK validation settings are weakened for the test. Three negative fixtures
(unknown Damage, invalid Percent, missing id) must fail schema validation.

## Configuration switching

After obtaining the two plugin DLLs from the successful integration artifact,
use a disposable SDK first:

```powershell
./recovery/scripts/SetArmorPlugin.ps1 -SdkRoot C:/Temp/SDK -PluginDirectory C:/Temp/Plugin -Mode Recovered
# Restore the original dispatch:
./recovery/scripts/SetArmorPlugin.ps1 -SdkRoot C:/Temp/SDK -PluginDirectory C:/Temp/Plugin -Mode Original
```

The switch removes ArmorTemplate from existing explicit registrations before
adding its selected handler. Repeated switching must never register the type
twice: original PluginRegistry uses Dictionary.Add. The recovered registration
sets `useBuildCache=false`; the default XmlCompiler `#all` handler is retained.
The recovered mode copies both new DLLs to tools. Original mode restores routing
without deleting those unregistered files.

## Evidence and boundaries

`RA3_RECOVERED_ARMOR_TRACE` optionally enables per-call traces from the actual
plugin: preprocessed XML and the three produced buffers, with a call list.
Without that environment variable, the plugin writes no trace files.
`ArmorSdkOracle.cs` loads the original Tokenizer only inside the test executable,
replays each captured XML and checks all three buffers. It requires calls for
all three probes and locates each probe's instance bytes in final mod.bin.
The SDK script also tests reversible/idempotent routing and replays the verified
trace in a fresh process with both original compiler DLLs physically absent. It
checks that neither original compiler assembly is loaded.
The script additionally requires all four complete MOD outputs (`bin`,
`manifest`, `imp`, `relo`) to match byte-for-byte and records hashes/sizes.
A successful BAB exit without traced probe calls is not accepted.

Actual source/run and success are pinned by `evidence/armor-sdk/validation.json`.
Consult that file before claiming a particular revision passed. Workflow artifacts
provide the new DLL pair separately from the evidence. This integration covers
ArmorTemplate only; full WeaponTemplate, GameObject, the XmlCompiler serializers
and complete replacement of the two original DLLs remain outstanding.
