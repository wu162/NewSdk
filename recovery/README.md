# RA3 compiler source recovery

This branch recovers the actual `SDK/tools/BinaryAssetBuilder.Tokenizer.dll` and `BinaryAssetBuilder.XmlCompiler.dll` shipped in this repository. The original DLLs are not replaced.

## What is usable now

* `src/Managed/CompilerCatalog.cs`: buildable C# reconstruction of both plugins' version/type metadata logic, including all 301 XmlCompiler types and 3 Tokenizer types.
* `src/Native/DataInitializers.cpp`: readable C++ implementations of the original time and angle initializers. No original DLL forwarding is used.
* `src/Bridge/PrimitiveInitializers.cpp`: buildable C++/CLI bridge to those native implementations.
* `src/Native/Relocation.h` / `.cpp` and `src/Bridge/RelocationBridge.cpp`: reconstructed allocation/relocation runtime, reference tables, bookmark exchange and Tokenizer member-offset fixup; see `RELOCATION_RUNTIME.md`.
* `src/Native/ArmorTemplate.*`, `ScalarParsing.*`, `src/Bridge/XmlNodeAdapter.h` and `ArmorCompiler.cpp`: complete ArmorTemplate XML-to-buffer reconstruction plus shared XML/scalar parsing. See `ARMOR_TEMPLATE.md`.
* `src/Native/WeaponTemplate.*` and `src/Bridge/WeaponCompiler.cpp`: 91 root attributes, four RangeDuration blocks, two SoundOrEvaEvent blocks WeaponAiHintInfo, three ObjectFilter blocks and the IncompatibleAttributeModifier reference list with complete buffers for that supported subset. See `WEAPON_TEMPLATE.md`; ScatterRadiusVsType now includes the complete inline ObjectFilter and 456-bit ModelCondition masks; Nuggets remains unfinished. Shared EnumBitset helpers include the two 223-bit ObjectStatus masks. ResourceMarshaling provides shared string, reference and typed-ID helpers.
* `src/Plugin/ArmorPlugin.cs`: formal ArmorTemplate-only SDK plugin in `Recovered.Compiler.dll`; configuration switching, legal mymod probes and actual MOD output comparison are described in `ARMOR_SDK_INTEGRATION.md`.
* `tests`: differential tests which load the original DLLs and compare the recovered components' results, including float bit patterns. See `evidence/primitives` for the actual results.

These components are **not yet complete replacement compiler DLLs**. ArmorTemplate now has a working component `ProcessInstance` entry point; its node layer, native layout and token tables are reconstructed. The Armor-only formal plugin now compiles mymod through the real SDK; full WeaponTemplate, GameObject, XmlCompiler serializers and whole-DLL replacement remain unfinished. The recovered relocation component is compiled and differentially validated against both originals; it is used by the Armor-only plugin, while complete replacement of both DLLs remains unfinished.

## Evidence versus recovered source

`decompiled` is ILSpy's mechanical C# rendering, not the original source language and not a buildable substitute for a C++/CLI project. In particular, empty native structs retain only their total size; pointer arithmetic in function bodies is needed to recover their fields. Generated `.csproj` files in that directory are evidence, not the supported build entry point.

`evidence/<assembly>/methods.json` records every method token/RVA, signature, code kind and available IL bytes. `types.json` records layouts and fields present in metadata. `summary.json` pins the assembly/hash. `logs` contains compressed complete IL/native listings. Source generation is pinned to ILSpy 9.1.0.7988.

Native metadata methods in the two targets are CRT/operator/runtime startup calls, not a large unidentified game-compiler subsystem. Most of the C++/CLI business logic was compiled to IL. This makes direct semantic recovery possible without importing an unrelated game's implementation.

## Reproduce on Windows

Use an x86 VS2022 developer PowerShell with C++/CLI support and .NET Framework 4.x:

```powershell
./recovery/scripts/TestPrimitives.ps1
./recovery/scripts/TestRelocation.ps1
./recovery/scripts/TestArmor.ps1
./recovery/scripts/TestArmorSdk.ps1
./recovery/scripts/TestWeapon.ps1
./recovery/scripts/Baseline.ps1
```

The scripts write build intermediates under `RUNNER_TEMP`; on a developer machine, set that variable to a disposable directory. The Actions workflows provide the same environment. The baseline uses a temporary SDK copy and invokes exactly the basic-data BAB command from `SDK/defaultscript.cs` case 5. Its only runtime compatibility change is adding CLR4 legacy mixed-mode activation to that temporary copy's application configuration.

## Known boundaries

The current tests validate the two rebuilt components, not an entire recovered MOD compiler. Primitive parsing comparisons include valid units, rounding, case sensitivity and malformed/mixed-unit cases. A differential failure exposed VC8 CRT's acceptance of D exponents, which current UCRT `atof` does not accept; the reconstructed initializer preserves that observed behavior. Shared scalar tests additionally cover integer limits, overflow and malformed signs, hex/octal integers, decimal float boundaries, and lexical differences from the current CRT. Old VC8 treats hex-float, INF and NaN text as zero; negative unsigned overflow saturates its magnitude before negation. The time/angle initializers now reuse that corrected numeric parser. Exotic legacy CRT inputs have not been exhaustively characterized.

Read `STATUS.md` before continuing, then work from the committed evidence and differential harness. No microtask scheduler is required.
