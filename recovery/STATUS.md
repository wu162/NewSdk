# Compiler source recovery — session handoff

Updated 2026-10-22. Branch: `recovery/compiler-source`. Original SDK on `main` has not been replaced or modified. This is a substantial initial recovery milestone, **not completed recovery of both compiler DLLs**.

## User scope

Recover both `SDK/tools/BinaryAssetBuilder.XmlCompiler.dll` and `BinaryAssetBuilder.Tokenizer.dll`, including managed/C++/CLI logic. Work directly from this repository, without assumptions from earlier projects. Do not create a microtask scheduler. Commit meaningful milestones. Windows Actions are authorized. Validate using the example MOD's basic-data build (the GUI invokes BAB through `defaultscript.cs` case 5). Continue across sessions at substantial milestones to avoid context exhaustion.

## Completed and persisted

* Mechanical C# rendering, complete IL listings, method metadata, native layouts and disassembly for both target DLLs. Core/BAB/GUI evidence is also available for integration context. ILSpy source extraction and IL extraction both returned 0 for both targets. Every nonzero-RVA IL body was read without errors.
* Tokenizer: 1,032 metadata types; 3,322 methods = 3,288 IL + 34 native. SHA256 `BA5E34E9A44509D5489086F610C3BB4502B441C1513740FB66D6B740AD3EB603`.
* XmlCompiler: 1,406 metadata types; 2,810 methods = 2,772 IL + 38 native. SHA256 `D4B78E2DA951DD458357BEE7B358E6BC9689D1987D55867254D68B3F87BBF0E6`.
* Native metadata methods are CRT/operator/runtime-support functions. Much of the C++ business logic is compiled as IL in `<Module>`; do not confuse it with C# originally written by the SDK authors.
* Buildable C# `src/Managed/CompilerCatalog.cs`: all 301 XmlCompiler and 3 Tokenizer type records, platform versions, processing hashes, unknown-type defaults.
* Buildable C++ `src/Native/DataInitializers.cpp` plus actual C++/CLI bridge `src/Bridge/PrimitiveInitializers.cpp`: time and angle initializers.
* Read `README.md` for build entry points and `PROVENANCE.md` for original method tokens.

## Actual validation

1. Original example MOD basic data: **PASS**, exit 0, newly created `mod.bin` 43,072 bytes, `mod.manifest` 1,338 bytes, `mod.imp` 64 bytes, `mod.relo` 864 bytes. Started from a fresh output directory in a temporary SDK copy. Full hashes, logs and output archive: `evidence/baseline`. Run: https://github.com/wu162/NewSdk/actions/runs/39746666619 . CLR4 startup/legacy mixed-mode activation was added only to the temporary application's configuration.
2. Windows MSVC x86 native+C++/CLI component compilation: **PASS**. Rebuilt C# catalog/hosts: **PASS**.
3. Both original plugins versus recovered catalog: **PASS**, 1,232 type-record comparisons of all public fields across Win32, Xbox360, PlayStation3 and unknown platform 99; includes unknown type IDs 0 and FFFFFFFF. `evidence/primitives/catalog-oracle.txt`.
4. Original Tokenizer global IL initializers versus rebuilt native+C++/CLI functions: **PASS**, 27 comparisons of exact float bits. Includes units, negative/scientific inputs, rounding, case sensitivity, malformed/mixed units and VC8 D exponents. `evidence/primitives/oracle.txt`, `result.json`. Run: https://github.com/wu162/NewSdk/actions/runs/39748015944 .
5. Full recovered compiler DLL build / replacing both DLLs / sample MOD compilation using replacements: **NOT IMPLEMENTED OR VALIDATED**.

## Evidence entry points for the next substantial stage

* `decompiled/BinaryAssetBuilder.Tokenizer/BinaryAssetBuilder.Tokenizer/Plugin.cs`: ProcessInstance dispatches WeaponTemplate 2496977262, GameObject 2486173485, ArmorTemplate 980180622. Generic HandleType follows Node -> XML Marshal -> Tokenize procedure table -> endian correction -> MakeRelocatable(token table) -> FixupMemberOffsets -> MakeRelocatable(data, table size) -> buffer copies. Tokenizer ImportsData is empty in that path.
* `decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs`: about 106,000 lines of global implementation. WeaponTemplate Marshal starts near 17,530; WeaponTemplate Tokenize near 79,756. These are navigational hints; use method names/token inventory if line numbers change.
* `decompiled/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder.XmlCompiler/Plugin.cs`: large generated per-type wrappers and ProcessInstance dispatch. `<Module>` carries the shared marshaler/runtime code. Mechanical native structs often have size but no fields; recover field offsets from pointer arithmetic, not by trusting empty structs.
* Relo.Tracker global methods provide Allocate, Push, Pop, AddReference, MakeRelocatable, GetMemberOffset, BookmarkMemberOffset, FixupMemberOffsets. Restore the common relocation/Node/marshaling runtime next, then bring generated type serializers into readable C++/CLI/native source and integrate actual Plugin.ProcessInstance entry points. Keep source and tests together; do not silently use original compiler forwarding or pretend unresolved stubs work.
* Original Relo data blocks align allocations to 4 bytes. Procedure table entries occupy 12 bytes. Tokenizer WeaponTemplate has 105 slots including sentinel; ArmorTemplate 7; GameObject 127. WeaponTemplate original structure size is 328 bytes. Preserve emitted layout, order, type/name hashes, and relocation semantics.
* Expand oracle fixtures beyond the example MOD when integrating serializers: its successful baseline alone does not prove WeaponTemplate/ArmorTemplate coverage or every supported XmlCompiler type.

## Practical environment notes

Local scratch currently has no authenticated GitHub clone and no .NET installation; GitHub connector accesses the private repository, and Actions handles Windows builds/decompilation. Downloadable Actions evidence archives can be brought into scratch with the connector's artifact download URL. Keep using the user's selected GitHub plugin.

Actions extraction uses pinned ILSpy 9.1.0.7988 and net8 metadata reader without loading mixed-mode originals. The x86 developer environment sets `Platform`; MetadataInventory explicitly sets AnyCPU and an output directory. C++/CLI requires `/EHa`, not `/EHsc`. Old Framework csc requires resolved absolute source paths here. Concurrent result writers use `git pull --rebase` before ordinary pushes; never force-push. Early failed extraction/build runs were corrected; use the successful runs/evidence above.

The oracle exposed old VC8 `atof` parsing `2d1` as 20. Current UCRT does not, so the recovered initializer normalizes D exponent notation after original unit removal. This detail is tested; exotic legacy CRT numeric syntax has not been exhaustively characterized.

## Resume instruction

Read this file and `README.md`, inspect current branch/head and actual committed evidence, then continue the shared runtime and real serializer reconstruction. Do not redo extraction, recreate the baseline or import conclusions from another recovery repository. Do not claim the two full compiler DLLs have been restored at this stage.
