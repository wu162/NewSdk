# Compiler source recovery — session handoff

Updated 2026-10-22. Branch: `recovery/compiler-source`. Original SDK on `main` has not been replaced or modified. This now includes the validated shared relocation runtime, complete ArmorTemplate path and WeaponTemplate scalar/resource/seven-optional-block subset, but is still **not completed recovery of both compiler DLLs**.

## User scope

Recover both `SDK/tools/BinaryAssetBuilder.XmlCompiler.dll` and `BinaryAssetBuilder.Tokenizer.dll`, including managed/C++/CLI logic. Work directly from this repository, without assumptions from earlier projects. Do not create a microtask scheduler. Commit meaningful milestones. Windows Actions are authorized. Validate using the example MOD's basic-data build (the GUI invokes BAB through `defaultscript.cs` case 5). Continue across sessions at substantial milestones to avoid context exhaustion.

## Repository-wide standing authorization — 2026-10-22

The user explicitly states: “这个仓库以后所有任务，仅仅不允许强推，其他都授权”.

For all future tasks in `wu162/NewSdk`, all task-related repository operations except force-push are authorized without repeated confirmation, including workflow configuration and automatic commits/pushes. Force-push, force-with-lease and forced API ref updates are prohibited. Read the root `AGENTS.md` for the persistent instruction. This supersedes earlier narrower authorization notes; the task scope, verification and approximately-25-minute implementation-stage rules continue unchanged.

## Stage size rule — user update, 2026-10-22

Future implementation sessions should contain about **25 minutes of effective work** (normally estimated at 20–30 minutes), not stop after a roughly 10-minute component. Bundle implementation, Windows build, original-DLL differential tests, fixes, commits and handoff into substantial stages. If planned work finishes early, continue adjacent useful work; do not idle or repeat unaffected tests to pad time. Finish critical verification already in progress and hand off a reviewable milestone. Explain any earlier handoff caused by an external blocker or context limits. This rule governs future implementation stages; a planning-only documentation edit need not take 25 minutes. The dependency-based revised plan and next session scope are in [`STAGES.md`](STAGES.md). Read both files when resuming. No microtask scheduler is introduced.

## Completed and persisted

* Mechanical C# rendering, complete IL listings, method metadata, native layouts and disassembly for both target DLLs. Core/BAB/GUI evidence is also available for integration context. ILSpy source extraction and IL extraction both returned 0 for both targets. Every nonzero-RVA IL body was read without errors.
* Tokenizer: 1,032 metadata types; 3,322 methods = 3,288 IL + 34 native. SHA256 `BA5E34E9A44509D5489086F610C3BB4502B441C1513740FB66D6B740AD3EB603`.
* XmlCompiler: 1,406 metadata types; 2,810 methods = 2,772 IL + 38 native. SHA256 `D4B78E2DA951DD458357BEE7B358E6BC9689D1987D55867254D68B3F87BBF0E6`.
* Native metadata methods are CRT/operator/runtime-support functions. Much of the C++ business logic is compiled as IL in `<Module>`; do not confuse it with C# originally written by the SDK authors.
* Buildable C# `src/Managed/CompilerCatalog.cs`: all 301 XmlCompiler and 3 Tokenizer type records, platform versions, processing hashes, unknown-type defaults.
* Buildable C++ `src/Native/DataInitializers.cpp` plus actual C++/CLI bridge `src/Bridge/PrimitiveInitializers.cpp`: time and angle initializers.
* Buildable C++ `src/Native/Relocation.h` / `.cpp` and C++/CLI `src/Bridge/RelocationBridge.cpp`: shared allocation/relocation runtime, external-reference tables, bookmark exchange, Tokenizer member lookup/fixup. Source and ABI/semantic details: `RELOCATION_RUNTIME.md`. Recovered containers use RAII/STL, not the original EASTL in-memory ABI.
* Recovered ArmorTemplate XML-to-InstanceData/RelocationData/ImportsData path, shared Node adapter and scalar parsing: `ARMOR_TEMPLATE.md`. This is a single-type component entry point, not a replacement compiler DLL.
* WeaponTemplate: 89 root attributes plus four RangeDuration blocks, two SoundOrEvaEvent blocks and WeaponAiHintInfo, native C++ implementation and C++/CLI ProcessInstance bridge. See `WEAPON_TEMPLATE.md`; this is a validated partial type, not a replacement DLL.
* Read `README.md` for build entry points and `PROVENANCE.md` for original method tokens.

## Actual validation

1. Original example MOD basic data: **PASS**, exit 0, newly created `mod.bin` 43,072 bytes, `mod.manifest` 1,338 bytes, `mod.imp` 64 bytes, `mod.relo` 864 bytes. Started from a fresh output directory in a temporary SDK copy. Full hashes, logs and output archive: `evidence/baseline`. Run: https://github.com/wu162/NewSdk/actions/runs/39746666619 . CLR4 startup/legacy mixed-mode activation was added only to the temporary application's configuration.
2. Windows MSVC x86 native+C++/CLI component compilation: **PASS**. Rebuilt C# catalog/hosts: **PASS**.
3. Both original plugins versus recovered catalog: **PASS**, 1,232 type-record comparisons of all public fields across Win32, Xbox360, PlayStation3 and unknown platform 99; includes unknown type IDs 0 and FFFFFFFF. `evidence/primitives/catalog-oracle.txt`.
4. Original Tokenizer global IL initializers versus rebuilt native+C++/CLI functions: **PASS**, 27 comparisons of exact float bits. Includes units, negative/scientific inputs, rounding, case sensitivity, malformed/mixed units and VC8 D exponents. `evidence/primitives/oracle.txt`, `result.json`. Run: https://github.com/wu162/NewSdk/actions/runs/39748015944 .
5. Shared runtime: **PASS**, Windows MSVC x86 C++/CLI build and 700 original/recovered comparisons (Tokenizer 364, XmlCompiler 336). All instance/relocation/import arrays match byte-for-byte for generated graphs, both endian modes, nonzero absolute offsets, duplicate-key sorts through 257 entries, and pointer/reference exchange. Tokenizer additionally validates member lookup, padding, miss behavior and cross-tracker token-member fixup. XmlCompiler has no token-member methods, so those cases are explicitly Tokenizer-only. `evidence/relocation`; run: https://github.com/wu162/NewSdk/actions/runs/39749956806 . The first run passed Tokenizer but failed because the harness attempted a Tokenizer-only function in XmlCompiler; the corrected harness scopes those tests explicitly.
6. Existing catalog/primitive regression: **PASS** again after the runtime changes, run https://github.com/wu162/NewSdk/actions/runs/39749956807 .
7. Full recovered compiler DLL build / replacing both DLLs / sample MOD compilation using replacements: **NOT IMPLEMENTED OR VALIDATED**.
8. **ArmorTemplate full single-type path: PASS**, Windows MSVC x86 native+C++/CLI compilation, `/Od` and `/O2`. Each build passes 284 original-Plugin.ProcessInstance fixtures / 852 three-buffer comparisons (1,704 comparisons across both modes). All 39 damage enum values, distinct root fields, partial defaults, missing/empty/unknown attributes, namespace filtering, malformed/negative/scientific percentages, duplicate entries and list lengths through 257 are covered. The four platforms are Win32, Xbox360, PlayStation3 and unknown 99. Independent size/member/relocation assertions also pass. `evidence/armor/oracle.txt` and `release-oracle.txt`.
9. **Shared Node adapter: PASS**, 360 direct original/recovered comparisons per DLL, 720 total. Attribute lookup, empty/default values, single-child lookup, named/all-element lists, counts/order/reset and parent position preservation are checked across empty/asset/other namespaces. See both `*-node-oracle.txt` files.
10. **Shared scalar parsers: PASS**, Tokenizer 199 + XmlCompiler 169 exact bit comparisons in each build mode, 736 total. Covers Boolean first-character rules, signed/unsigned limits, base-0 octal/hex integers, byte/ushort truncation, overflow, malformed signs, decimal/D exponents, signed zero/subnormals, overflow-to-infinity and legacy lexical rejection of hex-float/INF/NaN input. XmlCompiler does not instantiate an IValue-based ushort overload; that exact boundary is reported explicitly, with ushort IValue coverage on Tokenizer only. Corrected VC8 unsigned negative-overflow behavior and modern-CRT float grammar differences from original-DLL evidence.
11. **Time/angle regression after shared parser integration: PASS**, expanded from 27 to 41 original/recovered float-bit comparisons; catalog still passes all 1,232 record comparisons. Run https://github.com/wu162/NewSdk/actions/runs/39755982960 . No unrelated relocation/baseline re-extraction was performed.

The complete Armor/Node/scalar validation run is https://github.com/wu162/NewSdk/actions/runs/39756201609 , verified `success`. Validated source: `33e479960aeb3bfb5b42f1aaab9e8bcac214dbc6`; `evidence/armor/validation.json` pins that source commit and records aggregate success. The run publishes the optimized `Recovered.Armor.dll` as artifact `recovered-armor-component-x86` and the oracle logs separately. Neither SDK compiler DLL is included in or replaced by that component.


## Evidence entry points for the next substantial stage

* `decompiled/BinaryAssetBuilder.Tokenizer/BinaryAssetBuilder.Tokenizer/Plugin.cs`: ProcessInstance dispatches WeaponTemplate 2496977262, GameObject 2486173485, ArmorTemplate 980180622. Generic HandleType follows Node -> XML Marshal -> Tokenize procedure table -> endian correction -> MakeRelocatable(token table) -> FixupMemberOffsets -> MakeRelocatable(data, table size) -> buffer copies. Tokenizer ImportsData is empty in that path.
* `decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs`: about 106,000 lines of global implementation. WeaponTemplate Marshal starts near 17,530; WeaponTemplate Tokenize near 79,756. These are navigational hints; use method names/token inventory if line numbers change.
* `decompiled/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder.XmlCompiler/Plugin.cs`: large generated per-type wrappers and ProcessInstance dispatch. `<Module>` carries the shared marshaler/runtime code. Mechanical native structs often have size but no fields; recover field offsets from pointer arithmetic, not by trusting empty structs.
* Relo.Tracker global methods provide Allocate, Push, Pop, AddReference, MakeRelocatable, GetMemberOffset, BookmarkMemberOffset, FixupMemberOffsets. The relocation runtime is now reconstructed and validated: reuse it. The shared Node/XML adapter and ArmorTemplate path are now reconstructed. Reuse them; bring remaining generated type serializers into readable C++/CLI/native source, and integrate actual Plugin.ProcessInstance entry points. Keep source and tests together; do not silently use original compiler forwarding or pretend unresolved stubs work.
* Original Relo data blocks align allocations to 4 bytes. Procedure table entries occupy 12 bytes. Tokenizer WeaponTemplate has 105 slots including sentinel; ArmorTemplate 7; GameObject 127. WeaponTemplate original structure size is 328 bytes. Preserve emitted layout, order, type/name hashes, and relocation semantics.
* Expand oracle fixtures beyond the example MOD when integrating serializers: its successful baseline alone does not prove WeaponTemplate/ArmorTemplate coverage or every supported XmlCompiler type.

## Practical environment notes

Local scratch currently has no authenticated GitHub clone and no .NET installation; GitHub connector accesses the private repository, and Actions handles Windows builds/decompilation. Downloadable Actions evidence archives can be brought into scratch with the connector's artifact download URL. Keep using the user's selected GitHub plugin.

Actions extraction uses pinned ILSpy 9.1.0.7988 and net8 metadata reader without loading mixed-mode originals. The x86 developer environment sets `Platform`; MetadataInventory explicitly sets AnyCPU and an output directory. C++/CLI requires `/EHa`, not `/EHsc`. Old Framework csc requires resolved absolute source paths here. Concurrent result writers use `git pull --rebase` before ordinary pushes; never force-push. Early failed extraction/build runs were corrected; use the successful runs/evidence above.

The oracle exposed old VC8 `atof` parsing `2d1` as 20. Current UCRT does not, so the recovered initializer normalizes D exponent notation after original unit removal. This detail is tested; exotic legacy CRT numeric syntax has not been exhaustively characterized.

## Resume instruction

Read this file, STAGES.md, README.md and WEAPON_TEMPLATE.md, then inspect branch/head.
Continue WeaponTemplate from the resource/seven-block implementation described below.
Do not repeat Armor/XML/runtime reconstruction or restart WeaponTemplate defaults.
Next approximately-25-minute scope: recover the two 223-bit ObjectStatus masks
and shared enum/bitset handling with original ProcessInstance differential tests;
then filters/simple lists as dependencies and time allow. Remaining eight fields
are explicitly unsupported. A complete replacement plugin and rebuilt-DLL MOD
run are still outstanding. Read the newest validation.json for exact source/run.

## Continuation details from this stage

* Recovered runtime source milestone: `a8a13107b144fc0f4aca2bd59bb2a93a1158be49`; corrected/validated code: `724f6e5d035b3bf0b697708ea201ca3ecbca8246`. Actions commits following those record the exact evidence. This documentation commit is based on the latest branch head to retain all evidence.
* `TestRelocation.ps1` is the build/test entry point. C++ native and bridge filenames are distinct to avoid `/Fo` object collisions. The reflection oracle uses 88-byte original Tracker and 24-byte Chunk layouts and calls each original in a separate x86 process. Pop is inlined in the original and reproduced by moving the original stack end back four bytes in the oracle only.
* Most important serialization details: sort by `(owning block, value)`, preserve equal-key EASTL ordering, imports encode `reference + 1`, independent four-byte allocation padding, no sentinel for an empty table. ExchangePointers changes bookmarks only; callers still move bytes.
* Current runtime boundary: valid serializer inputs on x86. Invalid pointers, underflow, OOM and allocation-overflow behavior are not oracle-validated; unresolved member fixup throws explicitly. There is no original-DLL forwarding and no replacement of SDK binaries.



## Armor/XML stage continuation — 2026-10-22

This substantial stage bundled XML adaptation, complete ArmorTemplate layout/Marshal/Tokenize/EndianCorrect/output assembly, 284 fixtures, direct Node tests against both originals, shared scalar parsers, legacy CRT fixes, optimized Windows builds and persistent evidence. Source milestones: `ee8835c` (initial type path), `af7d05d` (first full Armor/Node validation), `5313395` (shared scalar extraction), `a443f6d` (legacy float grammar and optimized build checks), `507e5dd` (time/angle reuse), `33e4799` (final validated oracle coverage). Follow-up Actions commits preserve full results; this handoff is committed on top of those results, without force-pushing.

Early failures were corrected: incompatible `/clr /fp:strict` options, the System/Windows IServiceProvider header-order collision, modern CRT unsigned negative-overflow and float lexical differences, and an oracle incorrectly assuming XmlCompiler had Tokenizer's IValue ushort instantiation. The final `/Od` and `/O2` native/C++/CLI builds and all scoped oracles pass. Read `ARMOR_TEMPLATE.md` for field offsets, procedure-table details, original tokens and component usage.

Historical next scope (now completed by the stage below): begin WeaponTemplate using existing XML/scalar/relocation layers; recover a coherent layout/default/enum/scalar field group and its differential host, then optional/list/nugget structures as dependencies allow. WeaponTemplate is 328 bytes with 105 main token-table slots, but has many nested structures. Do not silently zero unimplemented branches, forward to original compiler DLLs, or label default-only fixtures as full coverage. A complete replacement plugin and MOD compilation using rebuilt DLLs are still outstanding.



## WeaponTemplate scalar/duration stage — 2026-10-22

* Implemented `src/Native/WeaponTemplate.*`, `src/Bridge/WeaponCompiler.cpp`,
  `tests/WeaponOracle.cs`, `scripts/TestWeapon.ps1` and direct-PE enum extraction.
  All 104 named procedure entries plus sentinel are reconstructed. 77 root
  attributes work; 36 Boolean bytes retain their byte layout. All nine enum maps
  and defaults are read from the original DLL/evidence, not another SDK version.
* Four real RangeDuration blocks now work: PreAttackDelay, FiringDuration,
  CoolDownDelayBetweenShots and ClipReloadTime. Each adds eight data bytes,
  a 36-byte token table and a real pointer relocation. Original field allocation
  order and first-duplicate-child selection are preserved.
* First scalar-only source milestone: `accd5abd48810cb83dc1c685bd4b217288417760`,
  run https://github.com/wu162/NewSdk/actions/runs/39760497709 : both modes passed
  3,008 fixtures / 9,024 three-buffer comparisons, plus 29 rejection checks.
* Duration implementation: `0c77c9845c46877fbe24c94b72d4db6d0abc1de7`,
  run https://github.com/wu162/NewSdk/actions/runs/39761012085 : both modes passed
  3,128 fixtures / 9,384 buffer comparisons, plus 29 rejection checks per mode.
  Four platforms include unknown platform 99. Independent assertions cover
  root and nested offsets, sizes, duration allocation order and relocation sentinel.
* Final validation source and artifact run: `5e29fb4263547cdce546883b2c763cfda9434805`,
  https://github.com/wu162/NewSdk/actions/runs/39761190464 . Verified completed/success. Both modes pass 3,128 fixtures / 9,384 buffer
  comparisons each (18,768 total), plus 29 rejections each. Final result is pinned
  in `evidence/weapon/validation.json`; complete logs are persisted in the evidence
  ZIP. The optimized component is published separately as
  `recovered-weapon-component-x86`; it is not named as or used to replace a SDK DLL.
* Catalog/time-angle regression triggered by these source commits also passed:
  https://github.com/wu162/NewSdk/actions/runs/39761012116 . The existing workflow
  persists its result through ordinary commits; retain those concurrent commits.
* User authorization update, 2026-10-22: the user explicitly permits workflow auto-push configuration. Weapon validation now has contents-write permission and automatically commits its scoped evidence files to recovery/compiler-source after an ordinary pull --rebase. No force-push is used. Evidence-only commits do not match its push path filters, preventing a self-trigger loop. Earlier automatic approval rejection applied before this explicit authorization and is no longer a pending permission requirement.
* Known limits: 23 remaining non-scalar fields must be absent; all other child
  blocks, root reference/string attributes (including explicit empty values),
  unknown/namespaced attributes are rejected explicitly. Scalar/root success
  does not establish full WeaponTemplate coverage. GameObject, XmlCompiler type
  serializers and full replacement-DLL MOD compilation remain unfinished.

Next approximately-25-minute stage: group reference/string helper recovery with
WeaponTemplate's audio/FX/weapon reference attributes and nonempty differential
fixtures, then bring in additional optional structures if time permits. Preserve
the verified scalar/duration behavior and expand the original-ProcessInstance
oracle instead of replacing it with recovered-only tests.



## WeaponTemplate resource and optional-block stage — 2026-10-22

This supersedes the earlier scalar/duration stage's supported-scope limits.

* Added shared native `ResourceMarshaling.h` / `.cpp`: padded ANSI strings,
  prepared-reference suffix parsing and typed-asset-ID FastHash. No original
  DLL forwarding. Original Node.Value uses StringToHGlobalAnsi; the bridge's
  ANSI conversion is exercised with accented and Chinese input on the runner.
* Expanded root support from 77 to 89 attributes: PreferredTargetBone; three
  pointer audio references; four inline FX references; two inline reload audio
  references; ProjectileStreamName typed ID; PreAttackWeapon pointer reference.
  Explicit-empty strings and pointer references allocate real payloads. Inline
  references without a backslash remain zero. Decimal indexes include malformed
  suffixes, signed limits/overflow, negative values and modulo-2^32 +1 encoding.
* Added OverrideVoiceAttackSound / OverrideVoiceEnterStateAttackSound (8-byte
  blocks, optional nested audio reference, inline EvaEvent) and WeaponAiHintInfo
  (12-byte block, two Boolean bytes, optional float pointer and inline warhead
  reference). Reused existing four RangeDuration blocks. Full layouts and token
  constants are in WEAPON_TEMPLATE.md and readable source.
* First resource source: `40e7c3556457f32510c3c3fec9fe65e84443481e`, run
  https://github.com/wu162/NewSdk/actions/runs/39764631667 : success. Each of
  /Od and /O2 passed 4,740 fixtures / 14,220 three-buffer comparisons and 17
  unsupported-input checks. Catalog/time-angle regression also passed in run
  https://github.com/wu162/NewSdk/actions/runs/39764631778 .
* Optional-block source `381b9287c87db5a619b17bc600e7b323bb47adc7` compiled,
  then differential testing exposed a truncated mangled XML field name. The
  original raw string is **UseAsWarheadForDamageCalculations** (plural). Its
  mangled identifier stops before the final s. Corrected native/bridge/oracle
  names using the raw-data bytes, not another game's SDK.
* Final source: `bef3220408380d609d7a145c4c40171caba63ee8`. Final validation:
  https://github.com/wu162/NewSdk/actions/runs/39787342909 , verified completed /
  success. Each /Od and /O2 build passed **5,492 fixtures / 16,476 three-buffer
  comparisons / 26 rejection checks**; 32,952 buffer comparisons across both
  modes. `evidence/weapon/validation.json` pins the exact source/run and scope;
  oracle logs/hashes are auto-persisted in the evidence ZIP. The optimized
  `Recovered.Weapon.dll` remains a separate component artifact, not a replacement
  SDK compiler DLL. Catalog/time-angle regression also passed:
  https://github.com/wu162/NewSdk/actions/runs/39787342799 .
* The oracle additionally exercises all 128 subsets of the seven optional
  blocks, with nonempty root string/audio/FX/weapon references and reverse XML
  order. Independent assertions check exact data size, pointer targets, nested
  reference values, token member targets, float bits and relocation order/sentinel.
  Original and recovered InstanceData, RelocationData and ImportsData are compared
  byte-for-byte on Win32, Xbox360, PlayStation3 and unknown platform 99.
* Important boundary: Tokenizer intentionally returns an empty ImportsData even
  when its internal tracker records references. The recovered code follows this
  original Plugin.ProcessInstance behavior; do not claim nonempty public imports.
* Still unsupported, with explicit rejection: RequiredFiringObjectStatus,
  ForbiddenFiringObjectStatus, Nuggets, SurpriseAttackObjectFilter,
  CombinedAttackObjectFilter, HitStoredObjectFilter, ScatterRadiusVsType and
  IncompatibleAttributeModifier. Other unknown/namespaced input remains rejected.
  There is still no complete recovered WeaponTemplate or replacement compiler DLL.
* Automatic approval review rejected an optional workflow edit to capture extra
  helper excerpts because the file includes contents-write and auto-push settings.
  That edit was dropped; existing authorized workflow configuration is retained.
  Source/test changes proceeded using the existing build/evidence workflow.
  Follow-up: the user explicitly approved this exact extra workflow change.
  Implemented in `013e5ee468f864e4856c602d8ad8b481290ee16b`: 11 named,
  line-numbered original helper excerpts in `resource-helpers.txt`, included by
  the existing evidence ZIP/hash collection. Permissions and auto-push logic
  are unchanged. Run https://github.com/wu162/NewSdk/actions/runs/39788084474
  completed successfully; both modes again pass 5,492 fixtures / 16,476 buffer
  comparisons / 26 rejection checks each.

Next stage: ObjectStatus masks (223 bits). Relevant original Tokenizer global
methods: IValue pointer Marshal near 51212 (28-byte allocation), inline Marshal
near 51592, seven-word EndianCorrect near 51672, text bitset parser near 67448,
and pointer Tokenize near 98079 (member offset, not a nested procedure table).
Extract full enum maps from original PE data as with previous enum recovery.
Do not assume mangled names preserve full XML attribute strings. Reuse all
validated resource/node/scalar/relocation code. Time permitting, bring in the
related ObjectFilter helpers, then lists/Nuggets in later substantial stages.
