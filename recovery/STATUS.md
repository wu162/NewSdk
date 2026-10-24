# Compiler source recovery — historical session records

## 2026-10-24：三完整组新增17类型，最终矩阵通过

本轮从46推进至63/322已知Marshal类型，剩余259明确拒绝，58 Token-only保持原未知Marshal路径；根仍88属性/38子节点。完成四声音/EVA选择器、六生命周期Update、七共享Die/DieMux模块。最终source379bed90fede179cb9270a1566ce989fe48fb5df，GameObject run40019863862、基础run40019863876均success；每/Od、/O2模式130,596 fixtures、391,788三缓冲区逐字节比较、8,688拒绝、300不变性比较，平台0/1/2/99。两模式783,576次缓冲区比较。SDK0次，原路由保留，组件不冒充正式插件。

分阶段先保存再推进，原失败/Actions证据完整保留。选择器首次样例名遗漏后缀、生命周期独立float期望、Die测试宿主重名与String长度期望均修正后完整通过；源码差分成功与独立断言都保留。详情GAMEOBJECT_SOUND_SELECTORS.md、GAMEOBJECT_LIFECYCLE.md、GAMEOBJECT_DIE.md。下一组11种Upgrade基类/派生只有123个原方法证据，未启用，见GAMEOBJECT_UPGRADE_NEXT.md与RESUME.md。未完成整个GameObject和两份完整替代DLL。

实际优化：固定原blob/方法指纹、分段源读取、完整组集中验收、复用公共契约、SDK降频；不量化账号额度节省。


**Current continuation:** read [RESUME.md](RESUME.md) first. Older “next stage” paragraphs below are historical and may already be completed. The current branch/evidence source commit is authoritative.

Updated 2026-10-23. Branch: `recovery/compiler-source`. Original SDK on `main` has not been replaced or modified. This now includes the validated shared relocation runtime, complete ArmorTemplate path and WeaponTemplate scalar/resource/status/ten-optional-block/modifier-list/scatter subset, but is still **not completed recovery of both compiler DLLs**.

## User scope

Recover both `SDK/tools/BinaryAssetBuilder.XmlCompiler.dll` and `BinaryAssetBuilder.Tokenizer.dll`, including managed/C++/CLI logic. Work directly from this repository, without assumptions from earlier projects. Do not create a microtask scheduler. Commit meaningful milestones. Windows Actions are authorized. Validate using the example MOD's basic-data build (the GUI invokes BAB through `defaultscript.cs` case 5). Continue across sessions at substantial milestones to avoid context exhaustion.

## Repository-wide standing authorization — 2026-10-22

The user explicitly states: “这个仓库以后所有任务，仅仅不允许强推，其他都授权”.

For all future tasks in `wu162/NewSdk`, all task-related repository operations except force-push are authorized without repeated confirmation, including workflow configuration and automatic commits/pushes. Force-push, force-with-lease and forced API ref updates are prohibited. Read the root `AGENTS.md` for the persistent instruction. This supersedes earlier narrower authorization notes; the task scope, verification and approximately-35-minute implementation-stage rules continue unchanged.

## Stage size rule — user update, 2026-10-23

The user extended stages from about 25 to about 35 minutes on 2026-10-23. This supersedes the earlier timing rule. Future implementation sessions should contain about **35 minutes of effective work** (normally estimated at 30–40 minutes), not stop after a roughly 10-minute component. Bundle implementation, Windows build, original-DLL differential tests, fixes, commits and handoff into substantial stages. If planned work finishes early, continue adjacent useful work; do not idle or repeat unaffected tests to pad time. Finish critical verification already in progress and hand off a reviewable milestone. Explain any earlier handoff caused by an external blocker or context limits. This rule governs future implementation stages; a planning-only documentation edit need not take 35 minutes. The dependency-based revised plan and next session scope are in [`STAGES.md`](STAGES.md). Read both files when resuming. No microtask scheduler is introduced.

## Completed and persisted

* Mechanical C# rendering, complete IL listings, method metadata, native layouts and disassembly for both target DLLs. Core/BAB/GUI evidence is also available for integration context. ILSpy source extraction and IL extraction both returned 0 for both targets. Every nonzero-RVA IL body was read without errors.
* Tokenizer: 1,032 metadata types; 3,322 methods = 3,288 IL + 34 native. SHA256 `BA5E34E9A44509D5489086F610C3BB4502B441C1513740FB66D6B740AD3EB603`.
* XmlCompiler: 1,406 metadata types; 2,810 methods = 2,772 IL + 38 native. SHA256 `D4B78E2DA951DD458357BEE7B358E6BC9689D1987D55867254D68B3F87BBF0E6`.
* Native metadata methods are CRT/operator/runtime-support functions. Much of the C++ business logic is compiled as IL in `<Module>`; do not confuse it with C# originally written by the SDK authors.
* Buildable C# `src/Managed/CompilerCatalog.cs`: all 301 XmlCompiler and 3 Tokenizer type records, platform versions, processing hashes, unknown-type defaults.
* Buildable C++ `src/Native/DataInitializers.cpp` plus actual C++/CLI bridge `src/Bridge/PrimitiveInitializers.cpp`: time and angle initializers.
* Buildable C++ `src/Native/Relocation.h` / `.cpp` and C++/CLI `src/Bridge/RelocationBridge.cpp`: shared allocation/relocation runtime, external-reference tables, bookmark exchange, Tokenizer member lookup/fixup. Source and ABI/semantic details: `RELOCATION_RUNTIME.md`. Recovered containers use RAII/STL, not the original EASTL in-memory ABI.
* Recovered ArmorTemplate XML-to-InstanceData/RelocationData/ImportsData path, shared Node adapter and scalar parsing: `ARMOR_TEMPLATE.md`. A formal Armor-only SDK plugin and real MOD integration are now implemented; see the newest Armor SDK stage below. This is not a replacement of both compiler DLLs.
* WeaponTemplate: 91 root attributes (including two ObjectStatus masks), four RangeDuration blocks, two SoundOrEvaEvent blocks, WeaponAiHintInfo, three ObjectFilter blocks the IncompatibleAttributeModifier reference list and complete ScatterRadiusVsType list with 456-bit ModelCondition masks, native C++ implementation and C++/CLI ProcessInstance bridge. See `WEAPON_TEMPLATE.md`; this is a validated partial type, not a replacement DLL.
* Read `README.md` for build entry points and `PROVENANCE.md` for original method tokens.

## Actual validation

1. Original example MOD basic data: **PASS**, exit 0, newly created `mod.bin` 43,072 bytes, `mod.manifest` 1,338 bytes, `mod.imp` 64 bytes, `mod.relo` 864 bytes. Started from a fresh output directory in a temporary SDK copy. Full hashes, logs and output archive: `evidence/baseline`. Run: https://github.com/wu162/NewSdk/actions/runs/39746666619 . CLR4 startup/legacy mixed-mode activation was added only to the temporary application's configuration.
2. Windows MSVC x86 native+C++/CLI component compilation: **PASS**. Rebuilt C# catalog/hosts: **PASS**.
3. Both original plugins versus recovered catalog: **PASS**, 1,232 type-record comparisons of all public fields across Win32, Xbox360, PlayStation3 and unknown platform 99; includes unknown type IDs 0 and FFFFFFFF. `evidence/primitives/catalog-oracle.txt`.
4. Original Tokenizer global IL initializers versus rebuilt native+C++/CLI functions: **PASS**, 27 comparisons of exact float bits. Includes units, negative/scientific inputs, rounding, case sensitivity, malformed/mixed units and VC8 D exponents. `evidence/primitives/oracle.txt`, `result.json`. Run: https://github.com/wu162/NewSdk/actions/runs/39748015944 .
5. Shared runtime: **PASS**, Windows MSVC x86 C++/CLI build and 700 original/recovered comparisons (Tokenizer 364, XmlCompiler 336). All instance/relocation/import arrays match byte-for-byte for generated graphs, both endian modes, nonzero absolute offsets, duplicate-key sorts through 257 entries, and pointer/reference exchange. Tokenizer additionally validates member lookup, padding, miss behavior and cross-tracker token-member fixup. XmlCompiler has no token-member methods, so those cases are explicitly Tokenizer-only. `evidence/relocation`; run: https://github.com/wu162/NewSdk/actions/runs/39749956806 . The first run passed Tokenizer but failed because the harness attempted a Tokenizer-only function in XmlCompiler; the corrected harness scopes those tests explicitly.
6. Existing catalog/primitive regression: **PASS** again after the runtime changes, run https://github.com/wu162/NewSdk/actions/runs/39749956807 .
7. Full recovered compiler DLL build / replacing both DLLs: **NOT IMPLEMENTED OR VALIDATED**. The Armor-only per-type SDK plugin now has real MOD validation; see the newest Armor SDK stage below.
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

Read `RESUME.md`, root `AGENTS.md` and the type-specific source/tests needed for
the current stage. `STAGES.md` retains the substantial-stage policy. Do not load
all historical records or the full 106,000-line decompilation into context.

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

Next approximately-35-minute stage: group reference/string helper recovery with
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



## WeaponTemplate status, reference-list and ObjectFilter stage — 2026-10-22

This supersedes the resource stage's eight-field unsupported boundary. This stage
bundles multiple adjacent serializers, a shared parser, differential tests,
Windows builds and persistent evidence; the about-35-minute stage rule remains.

* Shared `src/Native/EnumBitset.h` / `.cpp`: original C-whitespace token splitting,
  sign handling, case-sensitive name matching, fallback values, OR/XOR updates
  and case-insensitive ALL semantics. The existing five Weapon root bitset
  attributes now reuse it. The full original PE ObjectStatus map (223 names) is
  included without relying on another SDK's enums.
* RequiredFiringObjectStatus / ForbiddenFiringObjectStatus: pointers at root
  offsets 136/140 allocate 28 bytes each when present, including empty text.
  Token offsets directly bookmark the mask payload; there is no nested token
  table. Seven words are endian-corrected. Unknown text selects ordinal 198;
  ALL fills all 224 stored bits and prevents later tokens from changing them,
  reproducing the original cursor bug. A minus sign XORs, rather than clears.
* Added direct `WeaponCompiler.MarshalObjectStatus(text, initialSevenWords)` and
  original global-method reflection tests: zero/patterned/all-set seeds,
  223 names, signs, toggles, C control whitespace, embedded NUL and ANSI text.
  2,070 seven-word comparisons per build mode, separate from buffer comparisons.
* IncompatibleAttributeModifier: repeated text-valued child references, root
  count/pointer at 284/288, four-byte payload elements, original list token
  records and bookmark fixup. List sizes through 257, malformed/signed/overflow
  suffixes, repeated values, interleaved children and CDATA compare exactly.
* `src/Native/ObjectFilter.h` / `.cpp`: readable complete shared 112-byte layout,
  Marshal/Tokenize/endian helpers. The three WeaponTemplate pointers at 264/268/272
  now work, with all eight attributes, 284-bit KindOf Include/Exclude masks,
  two optional ObjectStatus masks, and IncludeThing/ExcludeThing typed-ID lists.
  All 284 KindOf names and small enum maps were read from original PE data.
  Rule unknown fallback is 1; Alignment fallback is 1; Relationship fallback is
  bit 0; KindOf fallback is bit 149. Empty/default values and ALL padding bits
  preserve original behavior. Unknown/namespaced inputs remain explicitly rejected.
* Differential fixtures cover all enum names, signs, toggles, lower-case fallback,
  both root status pointers, all eight subsets of the three filters, duplicate
  blocks, list boundary lengths through 257, and all 128 subsets of seven earlier
  optional blocks combined with filters/status/resources/reference lists.
  Independent assertions check scalar defaults, bit words, data/token sizes,
  count/order, pointer targets, nested member bookmarks and relocation sentinels.
* Source milestones: `aa2ed3f6d623dc4f9d12ffa8e18ea98790faa1d4` (status masks/shared
  parser), `750313180a88f6bad25b8c3143dc6c2d9eabf978` (direct seeded parser oracle),
  `15d43c1369d906be201cb44493516fabd0072217` (modifier reference list),
  `09f5b6d71786bdc16691ec011d27b0ee8932f5ea` (complete ObjectFilter integration).
* Final source validation: https://github.com/wu162/NewSdk/actions/runs/39790728845
  completed successfully. Each MSVC x86 C++/CLI /Od and /O2 build passes **20,488
  ProcessInstance fixtures / 61,464 three-buffer comparisons / 34 unsupported-input
  checks**, plus **2,070 direct seven-word parser comparisons**. Both modes together:
  **122,928 buffer comparisons and 4,140 direct parser comparisons**. All four
  platforms include unknown 99. `evidence/weapon/validation.json` pins the exact
  source/run, with enum maps, logs/hashes and ZIP auto-persisted by the existing
  ordinary-push workflow. The optimized component remains a separate
  `recovered-weapon-component-x86` artifact, never a replacement SDK compiler DLL.
* Catalog/time-angle regression for that source also passed:
  https://github.com/wu162/NewSdk/actions/runs/39790728955 . Existing regressions
  ran with the changed source; no unrelated baseline or Armor re-extraction.
* Still unsupported: Nuggets and ScatterRadiusVsType, plus unknown/namespaced XML
  outside this documented component input domain. No GameObject serializer,
  complete replacement compiler DLL or replacement-DLL MOD build is claimed.

Next substantial stage: ModelConditionFlagType (456 bits) + ScatterRadiusVsType.
Original helpers: ScatterRadiusType Marshal at lines 15370–15410, list Marshal
53635–53676 (176 bytes per element), endian at 53831, Tokenize at 78584 (four
procedure entries), list Tokenize at 99207. Each element contains Radius at 0,
15-word RequiredModelConditions at 4, and inline ObjectFilter at 64. Reuse the
new ObjectFilter helpers. Crucial distinction: absent Filter leaves its inline
112 bytes zero, whereas present `<Filter/>` runs defaults (Rule NONE = 3).
Recover the full ModelCondition enum/fallback from original data before enabling
this list. After its verified integration, begin Nuggets shared base layout and
dispatch with explicit unsupported-derived-type rejection. Keep the about-35-minute
stage size, source/Windows/oracle/commit/handoff grouping, and no force-push rule.


## User validation strategy update — 2026-10-22

The user requests per-type SDK plugin routing to recovered DLLs once a type is fully
recovered, progressively replacing the two original compiler DLLs. This is now the
main integration path, in addition to original-DLL differential tests. See
`SDK_TYPE_INTEGRATION.md` for the verified configuration mechanism and acceptance
criteria. Prioritize ArmorTemplate formal plugin + SDK integration before the next
Weapon field group. At the time of that strategy update the components were not registered SDK plugins.
The subsequent Armor SDK stage below implements and validates that registration.


## mymod / XSD validation requirement — 2026-10-22

The user explicitly requires modifying `SDK/Mods/mymod` source as needed to
trigger each recovered type during real SDK compilation. New integration XML
must conform to `SDK/Schemas/xsd`, its `CnC3Types.xsd` entry point, the concrete
type XSD and all referenced shared definitions. Read those files before authoring
fixtures; wire new assets into the actual basic-data include/reference chain.
Compare original/recovered plugin configurations using identical modified mymod
source and fresh output/isolated caches, and prove actual recovered-plugin calls.
Do not bypass schema validation or confuse malformed direct-ProcessInstance
oracle inputs with legal SDK XML. Commit the fixtures and entry modifications.
See `SDK_TYPE_INTEGRATION.md`; the completed Armor SDK stage below includes this work.


## ArmorTemplate formal plugin and real SDK integration — 2026-10-23

* Implemented `src/Plugin/ArmorPlugin.cs` as a real IAssetBuilderPlugin in
  `Recovered.Compiler.dll`, referencing the recovered C++/CLI `Recovered.Armor.dll`.
  Initialize/ReInitialize, original platform versions, all public Armor metadata
  fields and ProcessInstance are covered. No original compiler forwarding.
* Added reversible/idempotent `scripts/SetArmorPlugin.ps1`, removing explicit
  ArmorTemplate registration before adding the selected handler. The original
  Tokenizer keeps WeaponTemplate/GameObject; XmlCompiler retains #all.
  Checked-in original compiler binaries/default configuration are unchanged.
* Added schema-valid `SDK/Mods/mymod/Data/RecoveryArmorProbe.xml`, included from
  actual Mod.xml, plus a mytank ArmorSet reference. Three assets cover defaults,
  five distinct percentages/four damage entries, and an inherited/overridden armor.
* `scripts/TestArmorSdk.ps1` builds the formal plugin and runs the 284-fixture /
  852-buffer differential oracle in both /Od and /O2, including metadata on all
  four platform IDs. The original Armor/Node/scalar regression also passed:
  https://github.com/wu162/NewSdk/actions/runs/39858755023 .
* Final Armor SDK source `1165538468f78f27edc136a6c8afa2834675be07`, run
  https://github.com/wu162/NewSdk/actions/runs/39859270614 : success. Two disposable
  SDK copies compile identical modified mymod source with fresh output and no
  shared generated cache. All complete outputs match byte-for-byte: mod.bin
  43,904 bytes; mod.manifest 1,655; mod.imp 64; mod.relo 880. Full sizes/hashes,
  configuration snapshots, logs, captured XML and buffers persist in
  `evidence/armor-sdk`. Validation pins the exact source/run.
* Exactly three real recovered SDK calls are traced. Each captured XML is replayed
  through original Tokenizer.ProcessInstance and all three buffers compare exactly.
  All three emitted instance payloads are found in final mod.bin. The inherited
  input contains the parent's four Armor children and shared percentages, with
  Default overridden to 61.25%; this is BAB's actual preprocessing, not an oracle
  approximation. Fresh-process replay with both original compiler DLLs physically
  absent also passes; neither original assembly is loaded.
* Complete CnC3Types.xsd validates the new probe and Mod.xml; three negative fixtures
  (unknown Damage, invalid Percent, missing id) are rejected. Raw direct validation
  of existing mytank.xml initially failed on its existing xai:joinAction. Corrected
  the harness to use BAB's normal preprocessing/schema pipeline for that file;
  schemas and SDK validation are not weakened. Initial failed run is retained in
  history; the final run above passes the complete integration.
* Artifact `recovered-armor-sdk-plugin-x86` contains the two new DLLs;
  `ARMOR_SDK_INTEGRATION.md` documents build/configuration/evidence usage.
  This establishes Armor-only progressive replacement, not recovery of both
  complete DLLs. WeaponTemplate is still on its original SDK handler.


## ModelCondition / ScatterRadiusVsType stage — 2026-10-23

* Continued adjacent Weapon recovery after completing Armor SDK integration.
  Added the original ModelConditionFlagType map (457 entries, INVALID=-1 and
  ordinals 0..455) to shared EnumBitset. Unknown fallback is ABOUT_TO_HIT (233);
  case-insensitive ALL fills 15 words including padding, INVALID is skipped.
  The existing sign/XOR/ALL-cursor semantics are reused without normalization.
* Implemented the full ScatterRadiusVsType list path in WeaponTemplate native
  Marshal/Tokenize/endian/output assembly plus its C++/CLI XML adapter. Element
  layout: 176 bytes, Radius at 0, 15-word mask at 4, inline ObjectFilter at 64.
  Root count/pointer 276/280. Absent Filter stays zero; present empty Filter
  sets normal defaults. Filter token tables are emitted unconditionally.
* New partial-class `tests/WeaponScatterOracle.cs` covers all enum names/signs/
  case/fallback/padding, seeded direct original-helper calls, simple and nested
  list sizes through 257, first duplicate Filter, sibling order, and 128 mixed
  optional-block/resource/status/filter/modifier configurations. Independent
  assertions cover bits, offsets, total sizes, counts and relocation sentinels.
  TestWeapon.ps1 now compiles both partial test files.
* Source `6e5d09ec43784f8fb47557e72438462b18cb7286`, run
  https://github.com/wu162/NewSdk/actions/runs/39859937000 : completed/success.
  Each MSVC x86 /Od and /O2 build passes **28,496 ProcessInstance fixtures /
  85,488 three-buffer comparisons / 39 explicit unsupported-input checks**,
  plus **2,070 direct ObjectStatus and 4,179 direct ModelCondition comparisons**.
  Both modes: 170,976 buffer comparisons, 4,140 ObjectStatus and 8,358
  ModelCondition mask comparisons. Win32/Xbox360/PlayStation3/unknown99 are covered.
  Final validation/source/run, original enum maps, logs/hashes and evidence ZIP
  are auto-persisted in evidence/weapon. The shared catalog/primitive regression
  also passed: https://github.com/wu162/NewSdk/actions/runs/39859937037 .
* No Weapon SDK routing change: Nuggets remains unsupported, with explicit
  rejection. The recovered Weapon artifact remains a partial type component;
  ArmorTemplate is the only production SDK type routed to a recovered plugin
  in the integration test. Neither original compiler DLL is fully replaced.

Next approximately-35-minute scope: recover WeaponEffectNugget common base,
polymorphic dispatch and a coherent initial derived group (DamageNugget is a
useful first member), including original ProcessInstance differential fixtures
and optimized Windows validation. Original Tokenizer -Module-.cs entry points:
base Marshal 15406 / endian 15449 / pointer dispatch 15499; DamageNugget Marshal
16961 / endian 17069; base Tokenize follows Scatter at 78607; list Tokenize 99182
and polymorphic token wrappers around 99003. Derive actual sizes/fields and
procedure hashes from original pointer arithmetic; do not assume an unrelated
SDK's layout. Reuse ModelCondition for ForbiddenTargetModelCondition and shared
ObjectStatus/ObjectFilter/resource/tracker code. Reject all unimplemented derived
branches explicitly. Do not silently accept an empty Nuggets container as full
polymorphic support. Once all Weapon branches work, register the type in the new
plugin and add legal mymod/XSD fixtures using the Armor SDK chain.

Integration detail to handle before the eventual Weapon SDK switch: real BAB
preprocessed XML includes an unqualified TypeId attribute on assets and nested
elements (visible in the committed Armor traces). Current partial Weapon adapter
rejects unknown attributes, including that annotation. Adapt this deliberate
component boundary using real BAB inputs, preserving independent rejection of
unimplemented business fields; do not claim current direct oracle XML is already
SDK-preprocessed Weapon compatibility.


## Nugget common runtime and first six branches — historical checkpoint, 2026-10-23

This checkpoint is superseded by the nine-branch completion below; its counts
and source/run describe the earlier six-branch milestone only.

* Added `src/Native/WeaponNugget.h` / `.cpp` and C++/CLI Nugget readers.
  Original TypeId-driven dispatch, common 40-byte base, upgrade typed-ID lists,
  pointer masks, SpecialObjectFilter and 124-byte ScalarInfo list are recovered.
  The base allocations run after derived/ScalarInfo allocations, matching original
  byte and relocation ordering. WeaponTemplate delegates list Marshal/Tokenize/
  endian conversion; its 256/260 count/pointer and polymorphic token frames work.
* Six of the original **35 explicit dispatch branches** are now reconstructed:
  DamageNugget (152 bytes), DOTNugget (164), GrapplingHookNugget (152),
  LeechPercentMaxHealthDamageNugget (156), LeechHealthDamageNugget (156),
  LineDamageNugget (176). Common and derived fields, nested filters/masks/upgrades/
  ScalarInfo, references, defaults, inheritance frames and endian conversion are
  included. All other 29 branches explicitly reject; zero/unknown TypeIds reject.
  Empty Nuggets now works structurally but is not claimed as full Nugget support.
* First three-branch source `b0de16e4bd6eb7ca423fbcafed9a73774c361031`,
  run https://github.com/wu162/NewSdk/actions/runs/39861485833 : success. Each
  /Od and /O2 passed 35,060 fixtures / 105,180 buffer comparisons / 70 rejections.
  Extended six-branch source `ccfa45b4485f85365fcdd86170a22ae6c3484432`,
  run https://github.com/wu162/NewSdk/actions/runs/39861894014 : success.
* Final source including real SDK/XSD probes:
  `0bfc640f44503d4a12192b7070581bf29a9688e3`, run
  https://github.com/wu162/NewSdk/actions/runs/39862386470 : completed/success.
  Each MSVC x86 /Od and /O2 passes **41,248 original ProcessInstance fixtures /
  123,744 three-buffer comparisons / 186 unsupported-input checks**, plus the
  existing 2,070 ObjectStatus and 4,179 ModelCondition direct parser comparisons.
  Both modes total 247,488 buffer comparisons, 4,140 ObjectStatus and 8,358
  ModelCondition comparisons. Four platform IDs include unknown 99.
* `tests/WeaponNuggetOracle.cs` is the third partial oracle file. Fixtures include
  every new enum value/field, legacy numeric/unit/Boolean grammar, reference
  suffix limits, list sizes through 257, absent/empty/duplicate filters, pointer
  combinations, all 29 rejected dispatch branches, TypeId-driven element-name
  independence and 128 combinations with prior Weapon fields/optionals/scatter.
  Independent assertions check data/token sizes, tags, pointer-array order,
  scalar defaults, derived bytes/padding and relocation lengths/endpoints.
* Added schema-valid `SDK/Mods/mymod/Data/RecoveryWeaponNuggetProbe.xml` and its
  actual Mod.xml Include. All six weapons have non-default data, common filters/
  masks/upgrades and ScalarInfo; DOT required fields and ScalarInfo.Filter are
  supplied. `scripts/TestWeaponSdkProbe.ps1` validates raw XML with complete
  CnC3Types.xsd, builds actual mymod using fresh original-SDK output, and finds
  each reconstructed InstanceData payload byte-for-byte in final mod.bin.
  Payload lengths: Damage 3,096; DOT 3,168; Grappling 3,120; each leech 3,136;
  Line 3,216. Logs, all four SDK outputs and component inputs/buffers are inside
  the persisted weapon oracle ZIP's nugget-sdk-output.zip.
* The six-asset SDK probe retains **original WeaponTemplate routing**. It compares
  recovered component payloads with actual original SDK output; it is not an SDK
  build through a recovered Weapon plugin. Expected-component input adds the
  known polymorphic TypeIds to raw XML and does not claim full BAB preprocessing.
  All three buffers are checked separately by original ProcessInstance oracles.
* Armor recovered-plugin integration reran with the expanded mymod and passed:
  https://github.com/wu162/NewSdk/actions/runs/39862386602 . Original/recovered
  Armor routing produces identical full MOD outputs: bin 62,776 bytes, manifest
  2,225, imp 64, relo 1,168. All three real recovered Armor calls, schema negative
  checks, reversible routing and original-free replay remain passing. Primitive/
  catalog regression also passed: run 39862386479.
* Read `WEAPON_NUGGETS.md` for layout, source entry points, accepted input boundary
  and the full 35-branch coverage table. Current validation/source/run is pinned
  in evidence/weapon/validation.json, including componentSucceeded and
  sdkProbeSucceeded with an explicit SDK scope. No SDK compiler DLL is replaced.

## Complete DamageNugget inheritance family — 2026-10-23

* Validated final source: `f552f2195ebac59858a9c8d6db92194fa8756881`.
  [Weapon run 39863931585](https://github.com/wu162/NewSdk/actions/runs/39863931585)
  completed/success. Added DamageAndSpawn (220 bytes), FireOnObjects (164)
  and DamageContained (272) to the six above. The base plus all eight XSD
  DamageNugget-derived members are now recovered: **9/35 explicit branches**.
  The other **26** branches and unknown/zero TypeIds reject explicitly.
* Each MSVC x86 /Od and /O2 passed **47,512 original ProcessInstance fixtures,
  142,536 three-buffer comparisons and 190 rejection checks**, plus 2,070
  direct ObjectStatus and 4,179 ModelCondition comparisons. Both modes total
  285,072 buffer comparisons. Independent assertions cover derived sizes,
  token frames, defaults, pointer/relocation order and wrong-derived children.
* Spawn preserves the inline model mask and typed GameObject ID; Fire preserves
  the prepared Weapon reference and optional filter allocated before its parent.
  Contained preserves MaxUnitsToDamage, WindowBlastFX and unconditional inline
  DamageObjectFilter marshaling/tokenization. All reuse the verified common base.
* All **nine** legal mymod weapons passed the complete CnC3Types.xsd and actual
  original SDK build. Each recovered InstanceData matches its payload in mod.bin:
  Damage 3,096; DOT 3,168; Grappling 3,120; each leech 3,136; Line 3,216;
  Spawn 3,224; Fire 3,464; Contained 3,476 bytes. Evidence contains SDK logs,
  four outputs and expected inputs/component buffers in nugget-sdk-output.zip.
* Weapon SDK routing remains **original**. This is actual original-SDK payload
  matching, not a recovered Weapon-plugin build. Direct original-DLL tests
  separately compare InstanceData, RelocationData and ImportsData. Expected
  component XML adds only known polymorphic TypeIds; full BAB annotations on
  root/non-polymorphic children remain an eventual integration boundary.
* [Armor SDK run 39863931613](https://github.com/wu162/NewSdk/actions/runs/39863931613)
  completed/success with all nine probes present. Original/recovered Armor
  routes produce identical bin 72,940, manifest 2,480, imp 64 and relo 1,328 bytes.
  [Primitive/catalog regression 39863931573](https://github.com/wu162/NewSdk/actions/runs/39863931573)
  also completed/success. Exact current source/run is pinned by the committed
  evidence/weapon and evidence/armor-sdk validation.json files.

Next approximately-35-minute scope: recover the ProjectileNugget /
ScatterProjectileNugget / WeaponOCLNugget reference family, then adjacent
non-Damage branches as dependencies permit. Original Marshal / Tokenize hints:
Projectile 16865 / 79393, ScatterProjectile 15757 / 78814, WeaponOCL 16192 / 79074.
Verify method names. Reuse common helpers but preserve each original layout:
non-Damage branches with a 40-byte WeaponEffectNugget base must not be cast to the
current 152-byte Damage prefix. Group implementation, Windows /Od-/O2 original
ProcessInstance comparisons, legal mymod/XSD fixtures, actual SDK payload checks,
commits and evidence into one substantial stage. Do not redo the nine completed
Damage-family branches. Maintain the full coverage table in WEAPON_NUGGETS.md.
WeaponTemplate stays on the original handler until all remaining branches,
unknown-polymorphic fallback and BAB metadata annotations are recovered. Full
recovery/replacement of both target compiler DLLs is still outstanding.


## Current recovery, failed-run repair and 32-branch completion — 2026-10-23

The previous handoff still described nine branches, while the branch had advanced
through the 24-branch milestone and an unvalidated 30-branch source. This session
read the actual branch head and failed Actions run rather than repeating that work.

* Failure source `ae766a17c3bb11a0b5b4ee37591960569f038b19`, run 39890803205:
  empty SecondaryDamageNugget differed at instance byte 1332. Its
  PercentDamageToContained procedure-name constant was mistyped as 4033350053;
  original IL writes signed -261607243 = **4033360053u**. Fix `69dd1af3cf43a7f273316762dcd58e544b1b4815`
  adds an independent key/type/member assertion. Run 39891592759 passed both modes:
  54,744 fixtures / 164,232 three-buffer comparisons / 802 rejections per mode.
* SDK batch source `83ec946e687569fb883bda0798ceb6de46a93cc4`,
  https://github.com/wu162/NewSdk/actions/runs/39891922522 : completed/success.
  Added 12 legal probes for the activation/crater and scalar/string branches,
  plus a mixed 12-Nugget asset. All **31** weapons passed complete CnC3Types.xsd
  and original SDK basic-data compilation; all 31 recovered InstanceData payloads
  occur byte-for-byte in actual mod.bin. Schema negatives reject missing Amount,
  missing AttachModuleId and invalid DamageType. Weapon SDK routing stays original.
  This covers 30 branches, not the subsequent two additions. Preserved separately
  in evidence/weapon/sdk-batch-validation.json and sdk-batch-logs.zip.
* Armor SDK integration run https://github.com/wu162/NewSdk/actions/runs/39891922459
  completed/success on the same batch source and expanded mymod. All four complete
  original/recovered outputs are byte-identical; three traced calls, inheritance,
  original-free replay, negative schema cases and reversible routing pass.
* Added ParalyzeNugget (60 bytes) and InformationWarfareNugget (52) using their
  real common 40-byte base, original per-member procedure keys, scalar units,
  references and endian correction. Original PE extraction now includes
  ParalyzeEffectType and InfoWarEffect. Unknown Paralyze names map to EMP (0),
  default NONE is 4; InfoWar default NONE sets bit zero, unknown names select
  bit zero, ALL sets the full 32-bit word including padding. Existing sign/XOR/
  ALL-cursor behavior is preserved and tested through original ProcessInstance.
* Latest component source `bd474c8cfec499be91ade4af040279e86010c5cf`,
  https://github.com/wu162/NewSdk/actions/runs/39892423711 : completed/success.
  Each MSVC x86 /Od and /O2 passes **57,288 fixtures / 171,864 three-buffer
  comparisons / 850 rejection checks**, plus 2,070 direct ObjectStatus and
  4,179 direct ModelCondition comparisons. Both modes total **343,728 buffer
  comparisons**, four platforms including unknown 99. New WeaponEffectOracle.cs
  covers each enum value, independent token/default/mask assertions, malformed
  numbers/units/references, signed mask combinations, seeded randomized masks,
  list lengths through 257 and mixed inheritance/allocation paths.
* Catalog/time-angle regression https://github.com/wu162/NewSdk/actions/runs/39892423774
  completed/success. No extra SDK compilation was triggered for those two new
  branches; SDK batch scope is deliberately pinned to its earlier source.
* Added short RESUME.md, made it the root AGENTS.md continuation entry, updated
  STAGES/type/coverage docs and marked old STATUS next-stage paragraphs historical.
  This reduces repeated context loading. Tools/model remained usable this turn;
  account/model usage quotas cannot be reset through repository changes.

Current explicit Nugget coverage: **32/35**. Remaining: MetaImpact (96 bytes),
TintObjects (72), AttributeModifier (64). Their exact Marshal/Tokenize navigation
and dependencies are in RESUME.md. Full Weapon preprocessing/unknown-TypeId
behavior and formal plugin routing remain after those branches. Full recovery of
both compiler DLLs, GameObject and remaining XmlCompiler serializers is unfinished.



## 2026-10-23 — Final three explicit Weapon Nugget branches

Implementation source: `0d4c9932dd4c7bec1af88f37a527033afad80b9c`.
Recovered MetaImpact (96 bytes, optional KillObjectFilter), TintObjects
(72 bytes, inline RGBColor), AttributeModifier (64 bytes, two modifier references,
optional DamageFXType, angle, AntiCategories and AntiFX). Explicit dispatcher
coverage is now 35/35; unknown/missing/zero TypeId and unsupported inputs remain
explicitly rejected. Complete DLL replacement and formal Weapon plugin routing
are still incomplete.

Original methods/defaults/offsets and PE enum provenance are recorded in
WEAPON_NUGGETS.md; new differential and independent assertions are in
WeaponFinalNuggetOracle.cs. Prior tests remain enabled. The three formerly
unrecovered TypeId rejection fixtures now reject truly unknown IDs instead;
newly supported types receive positive differential fixtures.

Windows run https://github.com/wu162/NewSdk/actions/runs/39894482358 completed
success. validation.json sourceCommit/runId and both mode summaries were checked.
Each /Od and /O2: 64,392 ProcessInstance fixtures, 193,176 buffer comparisons,
1,002 explicit rejections, 2,070 direct ObjectStatus comparisons and 4,179 direct
ModelCondition comparisons, all four platforms including unknown 99. Combined
buffer comparisons: 386,352. Primitive regression run 39894482472 also succeeded.

No new real SDK build was requested or run. The separate SDK batch at source
83ec946e687569fb883bda0798ceb6de46a93cc4 still validates only the first 30 branches;
all five added since that batch remain outside SDK integration evidence.
Next: unknown-TypeId fallback, BAB TypeId/preprocessing boundary, then formal
Weapon plugin routing and one full schema-valid SDK batch. See RESUME.md.


## 2026-10-23 — Weapon formal SDK integration and GameObject baseline

Implemented unknown/missing/zero Nugget dispatch from original PE evidence: 40-byte base allocation, zero output tag, single 12-byte polymorphic token with offset zero, base-only endian. Added tolerance for BAB's non-polymorphic TypeId annotations and a formal Weapon plugin with original metadata. First checkpoint's independent layout expectation incorrectly used EXTENTS_3D=0; corrected to the original 4 before successful complete runs. No comparisons/assertions were weakened.

Verified source 22c12f53c2f3c886cdb63d212ab10bc57cf715e0; run 39896743313 success. Every /Od and /O2: 66,152 fixtures, 793,824 buffer comparisons, 1,002 refusals, 2,070 ObjectStatus and 4,179 ModelCondition direct comparisons, four targets including 99. Synthetic BAB annotations and formal metadata/entry run across the whole matrix. Full modes combined: 1,587,648 buffer comparisons.

One formal SDK Win32 batch: 38 schema-valid weapons covering all 35 Nugget branches, all-35 mixed list and inheritance; three invalid schema probes rejected; reversible/idempotent plugin routing; original/recovered mod.bin, mod.manifest, mod.imp and mod.relo all byte-identical; all 38 actual plugin calls replayed against original three buffers and again without the original Tokenizer/XmlCompiler DLLs. Independent evidence formal-sdk.json/formal-sdk-output.zip is separate from the prior 31-probe original-route SDK batch. Default checked-in config retains original routing; SetWeaponPlugin.ps1 switches only WeaponTemplate. Primitive run 39896743345 and Armor SDK run 39896743435 also success for this source.

Proceeded to the next authorized dependency group instead of stopping after Weapon: established original GameObject characterization (no recovered GameObject implementation). Verified source 46b0ec5020b3911ec0b0403a3d60f63426590673; run 39897995324 success. 196 fixtures and 588 deterministic three-buffer comparisons on four targets; 16 scalar bindings, 592-byte native root. Original source extraction maps 126 fields plus sentinel, including 23 member bookmarks, 101 helper entries, two null-only Tokenize stubs at ReplaceModule@496 and InheritableModule@500. Complete source/runtime join verified all field names and hashes, eight default empty-list tables, root offset 1620, InstanceData 2212 and empty relocation/import buffers. Evidence in evidence/gameobject/source-layout.json and original-baseline.zip. Next stage: recover a coherent scalar/string/default-helper subset, never use a frozen buffer as recovered logic.

RESUME.md now names exact current anchors and the next ~35-minute implementation stage. Efficiency actions: fetched original module once, inspected targeted methods, reused original evidence and already built Weapon assemblies for the SDK batch, did not repeat SDK builds per field; account quota savings cannot be measured here.

## 2026-10-23 — GameObject complete root attributes and coherent child groups

Completed the planned duration + first child group, saved it, and continued the already-authorized dependent groups instead of ending after the first subset. GameObject now supports **88/88 direct root attributes and 16 non-polymorphic child types**, not a full GameObject compiler or SDK plugin.

Groups: duration plus DisplayName/DisplayNameTransformed/Buildable/ThingClass/DisplayColor; three repeated leaf lists plus Flammability/LiveCameraOffset; complete ArmorSet/LocomotorSet structure lists; four visual leaves (both FormationPreview decals, ShadowInfo, VisionInfo). Source-derived PE enum maps and child Token metadata retain defaults, allocation/schema order, nested records, endian correction, reference behavior and refusals. All shared runtime modules remain unchanged.

Final component source `a115ef671b3c4a7a18a11bc720418b4c68e789f4`; successful run https://github.com/wu162/NewSdk/actions/runs/39924338693 . Each /Od and /O2: 23,240 fixtures, 69,720 three-buffer comparisons, 224 refusals and 16 separate invariance comparisons; two modes total **139,440 buffer comparisons**, Win32/Xbox360/PS3/99. Evidence sourceCommit/runId and component artifact were verified. Duration sweeps cover adjacent floats at frame boundaries and powers of two; all child enums/23 ArmorSet bits, mixed allocations, nested tokens and independent layouts pass.

Primitive regression https://github.com/wu162/NewSdk/actions/runs/39924338637 has success at attempt 2, same head SHA: 41 original/recovered float-bit cases and 1,232 type records pass. Attempt 1's tests passed but its evidence push raced the GameObject evidence commit (remote ref changed). Only the failed job was rerun; ordinary pull/rebase/push succeeded. No force update and no workflow trigger changes.

The first group exposed a new independent assertion's incorrect decimal expectation: 200.00002 rounds to one frame, while 200.00004 produces two. The original/recovered differential already agreed. Corrected the independent assertion and obtained full successful matrices, retaining the failed run in history. Invariance comparisons are reported separately from the actual three-buffer count.

Actual SDK runs: **zero**. Original SDK route remains; full GameObject and the two complete replacement DLLs remain unfinished. Next coherent work: pointer StringList wrappers and the AutoResolve family. The pointer StringList helper selects child elements rather than using Browser's whitespace overload; inspect real BAB-preprocessed representation before implementation. Remaining unsupported blocks/modules still fail closed. RESUME.md and GAMEOBJECT.md contain the current scope and exact anchors.

Checkpoints were ordinary updates on work branches, then coherent prepared groups were fast-forwarded into recovery/compiler-source; concurrent evidence commits were preserved. No per-field SDK runs, no repeated Weapon investigation, and no account quota savings claim.


## 2026-10-23 — complete GameObject dependency/list families (23 child types)

Implemented seven additional child types, keeping all 88 root attributes and the earlier 16 child types: AutoResolveArmor, AutoResolveWeapon, AutoResolveInfo, BuildFadeInOnCreateList, BuildVariations, ObjectResourceInfo and GameDependency. Complete nested dependencies include upgrade lists, MoneyTransaction/BuildCost, nullable ModelCondition/ObjectStatus masks, three typed-ID lists and the existing full ObjectFilter. Shared parsing/relocation/resource/enum-bitset/ObjectFilter implementations are unchanged.

Pointer StringList text behavior was not guessed: original helper 66048 selects element children via Node.GetChildNodes(null), while actual StringList XSD permits text only. BAB source retains text and injects TypeId. The component matrix validates actual XSD text through .NET and compares original output, which contains an allocated empty list. Synthetic original-only Item children demonstrate the nonempty helper path but are XSD-invalid and explicitly refused by the recovered component. No real GameObject SDK run was performed; see GAMEOBJECT.md for exact source/input-chain evidence and supported surface.

Final source 074d267b2835be89ade3944c1cbe6d2f94b3793a; component run https://github.com/wu162/NewSdk/actions/runs/39928834132 succeeded. Each /Od and /O2: 34,052 fixtures, 102,156 three-buffer comparisons, 492 refusals and 124 invariance comparisons; both modes total 204,312 buffer comparisons. All four platforms (Win32, Xbox360, PS3, 99) remain. GameDependency includes all 457 ModelCondition names and 223 ObjectStatus names, signed/XOR/ALL semantics, null/empty allocations, lists and nested filter coverage. Original source hash and generated metadata checks passed. Matching primitive regression https://github.com/wu162/NewSdk/actions/runs/39928834050 succeeded. Final evidence sourceCommit/runId and the component DLL artifact were checked.

Coherent preceding groups passed at sources 8650678284fd171f0dc076732bf12d0944e2d1ef (AutoResolve, run 39926787685) and 93d16a98c24cde4ac9d2e00d26c6d76c96ff7015 (pointer/resource lists, run 39927770965). Incomplete checkpoints were saved on recovery/work/gameobject-autoresolve, recovery/work/gameobject-stringlists and recovery/work/gameobject-dependency; complete prepared groups were ordinary fast-forward updates to the target branch, retaining concurrent Actions evidence. No force push, no per-field full matrix, no SDK runs. The local Linux host lacks x86 development headers/C++ CLI, so Windows CI supplies authoritative builds and differential validation; local source-derived metadata consistency passed.

TestGameObject.ps1 now links the existing ObjectFilter.cpp. Workflow change adds only the ObjectFilter path filter; permissions and automatic evidence pushes are unchanged. Automatic review initially rejected saving the workflow due to an authorization concern. Re-read remote AGENTS.md and original workflow to establish explicit user authorization and unchanged privileges; retry was approved. No bypass or new privilege was introduced.

GameObject remains an incomplete component, not a formal SDK plugin or either complete replacement DLL. Next coherent group: CrusherInfo/ExtraCrushLevels and ProjectedBuildabilityInfo/masks/AllowedObjectFilter. SkirmishAIInformation, Geometry, audio arrays, polymorphic module groups and unresolved Replace/Inheritable Token paths remain. RESUME.md is the current continuation entry. Actual quota reduction cannot be quantified; avoided repeated Weapon investigation and all real SDK runs, while preserving full affected matrices.


## 2026-10-23 — complete GameObject Crusher/Projection, Geometry and audio/EVA groups (29 child types)

User extended this and future implementation rounds to approximately45 minutes; AGENTS/EFFICIENCY/RESUME/STAGES/STAGE_DURATION now agree, preserving ordinary pushes/no force-push and checkpoint/validation rules. Existing branch updated directly, no new branch. An external pause before user continuation is not counted as effective work.

Six root child classes added in three coherent batches: complete CrusherInfo/ExtraCrushLevels and repeatedProjectedBuildabilityInfo; rawGeometry/PlacementShape/Shape/ContactPoint with vectors; allAudioArrayVoice branches plusSound/EVA arrays. Original defaults/layouts/enum fallbacks/nullable allocations/Token/endian and unknown-input refusals restored. NamedVoice hashes preserve case, while TargetObject typedIDs fold case; no shared implementation changed.

Final source d27f0b932b4855d708cf0f09095c0b50026251e4, component https://github.com/wu162/NewSdk/actions/runs/39956084622 success; baseline https://github.com/wu162/NewSdk/actions/runs/39956084413 success. Per/Od,/O2:44,980fixtures,134,940buffer comparisons,796refusals,136invariance checks; four targets0/1/2/99;269,880total comparisons. Read terminal states and validation.json, matching sourceCommit/runId/modes. Evidence bot commits retained. SDK zero runs/original route; no formalGameObject plugin or wholeDLL replacement claimed.

Prior group anchors:4a8989d232432330a230cdcb58f0e7bbd7e6462e/run39935810403 (25 child types),140963c2f2be6c1091ce506c5130205b77fe072b/run39955059001 (26 child types); final matrix covers all29. Source-derived generator fingerprint/check retained. Local native syntax check only supplements Windows x86 build/differential; it is not a replacement for layout validation.

Prepared GAMEOBJECT_AI.md as source-only next-group map:88-byteSkirmishAIInformation,172-byteAISpecialPowerInfo list, two20-byteBaseSelection objects,28-byteAISpecialPowerWeaponData and sharedObjectFilter. Not implemented/not differentially validated. Nullablefloat/bool/asset-reference and enum-bitset semantics need full next-batch probes. AIBaseType has11names for10bits, with PRIMARY_TYPE_MAX and CAPTURED sharing2; do not infer bitCount from nameCount. See RESUME for current continuation.


## 2026-10-23：完整AI、空Module标记与31类型公共模块组

在recovery/compiler-source连续完成三组，未新建分支：SkirmishAIInformation完整64字段/特技列表/两组base criteria/武器/ObjectFilter；ReplaceModule/InheritableModule两个完整XSD空结构；六模块容器和全部31个8字节公共模块类型、未知TypeId默认路径。其余291个已知Marshal类型显式拒绝，仍不是完整GameObject编译器，没有正式GameObject插件。

最终源码afac552209d4880e883f2d37189c896a1823e4a6；GameObject https://github.com/wu162/NewSdk/actions/runs/39961644190 success；基础回归 https://github.com/wu162/NewSdk/actions/runs/39961644145 success，同源码。每种/Od、/O2为69,744 fixtures、209,232三缓冲区逐字节比较、8,264拒绝、204不变性比较；含Win32/Xbox360/PS3/99；两模式418,464缓冲区比较。读取终态及validation.json核对sourceCommit/runId/modes/platforms。Actions证据提交保留。真实SDK0次，原路由保留；共享scalar/resource/relocation/bitset/ObjectFilter未改。

原分派提取器核对322个Marshal/endian、380个Token和58个Token-only，保留两处endian别名及全部31公共类型原Token树；生成器检查完整原blob、64个AI字段名称/默认/Token偏移。数据和源码清单不是额外模块运行成功声明。工作流只追加新提取器到既有路径过滤，没有关闭回归或减样本。

AI首轮仅独立测试把UNDEFINED误写为unknown fallback（应分别14/38），保留失败run39959381460，修正并扩展独立检查后完整通过run39959718372；空标记组run39960173975通过；公共模块整组一次完整矩阵通过。未逐字段反复触发Windows，未触发SDK。没有账号额度计量，不能量化节省。

RESUME已重写为当前入口；下一完整组为Body的其余七类型，GAMEOBJECT_BODY.md整理实际方法/布局/分配与Token差异，仍未实现/差分。注意ShieldArmor先分配后ActiveBody基类，DodgePercent不是归一百分比，76字节派生类Token链不同；不能只把新TypeId加到固定8字节choices。


## 2026-10-23：Body / ClientUpdate / 声音播放完整组

本轮有效工作从13:48:08Z开始，至14:33Z左右完成实现、矩阵、后续源码调查和交接，按约45分钟推进。首个完整组及时保存，再推进两个完整组和下一选择器源码证据；并未以首组完成结束。全程普通快进更新，保留Actions证据，无强推，无SDK运行。

- Body其余七种全部实现，至此九种XSD Body全覆盖；source d0b96f2a57aeb7dac4dc2d58683b6ed8a93d8c8f/run39965730762 success。精确默认66.666666/33.333333、ShieldArmor先分配、完整12字节DamageRangeModifier及各派生Token图。
- 六种ClientUpdate及25属性全部实现，含区分大小写Key列表、ushort UnitWeight、普通float LaserLifetime；source76e1e34c12b4728ab194acffbde1a26bef52febd/run39966783977 success。
- AnimationSoundClientBehavior / ModelConditionAudioLoopClientBehavior完整依赖组实现：可空非负range、可空/inline ModelCondition、资源、动画typed-ID、Frame signed列表、Token/endian。source85a1e180c6bdb506733ec0711e9ad7c8512095c4/run39968193677 success。
- 终审补齐原XmlFormattingError异常；92846ab初次构建因命名空间不完整失败，修复源码83a7e5261da45fdf1d35d14728db42b76c040747，最终GameObject https://github.com/wu162/NewSdk/actions/runs/39969902743 和基础 https://github.com/wu162/NewSdk/actions/runs/39969902873 均success。远端validation.json源码/runId/modes/platforms及日志已核对。每/Od、/O2模式91,528 fixtures、274,584三缓冲区比较、8,444拒绝、272独立不变性比较；两模式合计549,168缓冲区比较，平台0/1/2/99。
- 总46/322已知模块类型，276个仍明确拒绝；58个Token-only沿用原未知Marshal路径。根88属性/38子节点，仍非完整GameObject/正式插件，原SDK路由保留。
- 下一完整组四个声音/播报选择器仅源码调查：ExtractSoundSelectorEvidence.py、sound-selector-source-plan.json，12嵌套契约、8独立列表stride，生成及--check成功，不启用TypeId、不冒充DLL验证。见GAMEOBJECT_SOUND_SELECTORS.md。

准确当前入口RESUME.md；Body文档已从计划更新为完成并修正literal精度；新增ClientUpdate和声音播放契约说明。最终交接提交只含文档及源证据调查，不改已验证运行源码或测试。


## 2026-10-24：四完整能力组，93→112已验收

本轮新增19种：TechUpgradeReceiver（1）、雷达/可见性/Capture（8）、直接开火/伤害跟踪（7）、LaserState/SweepingLaserState/StreamState（3）。Beacon/RadarMarker/LaserUpdate为此前已支持类型，不重复计数。最终源码5d4e332b53eb8492f7731cf7f388de55fdc7672e，GameObject run40051190340及基础run40051190379均success；validation.json/sourceCommit/runId、双模式oracle日志与终态已核对。每/Od、/O2：194088 fixtures、582264三缓冲区逐字节比较、10216拒绝、416不变性，平台0/1/2/99。两模式合计1164528次缓冲区比较。210已知Marshal继续明确拒绝；58 Token-only原fallback不变。SDK本轮0次，原路由与输入未改变；component artifact不等于完整GameObject插件或两份完整替代DLL。

中间验收锚点：fa250611/run40048856796（Receiver）、7cd68875/run40049625447（Vision）、b57e5a5a/run40050646715（开火/跟踪）；最终回归包含全部。既有额外WeaponTemplate组件回归run40050646649 success。原字段/enum/Token/Endian生成器全部保留原blob/PE校验，正式CI没有使用本地exported-evidence检查模式。关键契约分别见四份GAMEOBJECT_*类型文档。

源码调查首次11模块触发150依赖上限（run40049720082），收敛到完整直接开火/跟踪组后恢复，未放宽保护。新增点号类型、SinglePolymorphic/PolymorphicType及typed-pointer/typed-local callee证据导出支持。下一组Projectile/Dynamics/Freefall/EnableDynamicsCollide原97方法已导出，sourceCommit76ea42b0ab846d3f594887a72dfa1146aad28f58、run40051656143 success；含完整Movement分派/typed wrapper/具体Marshal与Dynamics Volume五类shape依赖，见GAMEOBJECT_MOTION_NEXT.md，尚未实现、尚未计支持。期间并发Actions提交均保留；ref lease变化先读新头重建树普通推送，没有强推。

本轮按完整能力组统一矩阵，复用已验证公共实现及相同原方法证据，未重复触发SDK；没有可比账号计量，不量化额度节省。


## 2026-10-24：WeaponSet及治疗/Crate三完整组，123→136已验收

新增13种完整Marshal模块：WeaponSetUpdate1、治疗/维修4、Crate碰撞/生成8；具体范围/布局见GAMEOBJECT_WEAPON_SET.md、GAMEOBJECT_HEALING.md、GAMEOBJECT_CRATE.md。最终源码1ef49469cd7a9f290b76952c4040cd2df0a2a147，GameObject run40072750732、基础run40072750706均success；每/Od、/O2：275032 fixtures、825096三缓冲区逐字节比较、11352拒绝、712不变性，平台0/1/2/99。两模式合计1650192次缓冲区比较，validation.json、两模式日志、sourceCommit/runId及终态已核对。186已知Marshal仍拒绝，58 Token-only fallback不变；组件不等于完整GameObject/完整替代DLL/SDK插件。SDK本轮0次，原路由保留。

WeaponSet源码45625616880eb075147618813786d27794ee38c2/run40071300506 success；完整四槽/WeaponData排序/炮塔/Chooser/VisionRange/inline Filter已恢复。原Tracker ExchangePointers与AlterBookmark逐条核对：只移动offset，value不变；既有实现正确，纠正旧交接歧义。排序用既有EASTL排列及逆映射，先交换bookmark再交换72字节数据，未提前重排XML。重复key/阈值/引用与bool配对及依赖组合分配均覆盖；复用原720排序探针和既有/Od/O2 helper证据，未重复相同探针。

治疗/维修源码3b2cb3d2687d06b92417c3af56f6656b45e7cc63/run40072240199 success，覆盖完整Upgrade、nullable validation/DieMux/Filter和inline Repair percent/audio；首轮仅拒绝清单末尾AutoHeal ID维护遗漏导致失败，修复后完整矩阵通过。Crate八模块包含基类及所有现存Crate碰撞分派和Spawn，继承强引用顺序保留；Spawn CrateList是重复文本子节点的强引用列表，不是属性字符串，先列表后BlockingUnits。未修改共享Tracker/资源/标量/位集实现。额外WeaponTemplate run40071300487和基础run40071300482/40072240244 success。

下一完整AI Update八模块仅取证：源码bae88f824dd018ba5a46ba390ab5f095cc046d07/run40072829359 success，96精确方法/PE枚举，见GAMEOBJECT_AI_UPDATE_NEXT.md。UnitChooser40与TurretChooser44区分，AIUpdate pointer@68/StandGround bool@72；该组尚未接入，仍拒绝。约45分钟单代理有效实现、10–15分钟检查点、完整组统一差分、SDK降频与正确性门槛均保留。所有提交普通推送，保留并发Actions证据，没有强推/无必要新分支。优化只避免重复上下文/调查/SDK探针，不声称可量化账号额度下降。


## 2026-10-24 XmlCompiler 五组扩展：82入口最终验收

目标recovery/xmlcompiler-priority，本轮新增30入口（damagefx2/hotkeymaps4/audio7/uisettings14/soundmaps3），总82个原Plugin入口/80全局schema类型。组件e7580311f36811265e6ed5af5a184ef623f31284，完整14组与联合DLL运行40099095351成功；362992 fixtures、1728984数组比较（含适用CompileXml对照）、12984显式拒绝，均/Od、/O2、四平台0/1/2/99。

SDK92180f9bbd830b9dbd49a696c259e54ffe2d7721，运行40098695436成功；159固定资产，Reduced159/FullMymod162实际调用，各原版与隔离恢复版477/486数组比较，四产物一致，完整7 Includes保留，两原DLL不变。SDK后仅UI Oracle断言修改，src/sdk/scripts/原SDK/.github及SDK Oracle Git对象完全相同，复用有精确证据。历史失败40098375480/40098375483/40098695441保留，最终以成功锚点为准。

源码分支父历史全部合并，无强推。GameObject与Joypad命名根继续排除；不是完整301入口/Tokenizer替代。精确矩阵、限制和下一轮五根战术UI热键证据见XMLCOMPILER_PRIORITY.md、XML_PRIORITY_SDK.md、XML_TACTICAL_HOTKEYS_NEXT.md及evidence/xml-priority-acceptance.json。


## 2026-10-25：101新增组件，183入口差分通过；SDK0

用户要求一小时/最多3子代理/本轮禁止SDK真实编译。七组新增101：tacticalhotkeys5、uichat3、uidecals4、singlesound10、pathmusic7、musicscript21、uitactical51。最终源码d9908f57107f11b7ab875ac5787fce2bf23e00a9，运行40132543491 completed/success，21完整组/Od/O2/四平台与183入口联合DLL全过。645344 fixtures、3423096数组比较（含direct）、22704拒绝；联合每模式732/2196与40/120 completion，实际退出0。

首轮40132183077真实LookAt三个100%默认值漏失已按原静态symbol修正，加defaults/partial独立ABI；历史失败保留。SDK真实编译0次，SDK工作流最近仍40098695436。主Plugin/SDK配置/scripts/oracle/probe/SDK工作流及原82SDK清单未改，保护SHA见evidence/xml-component-acceptance-20261025.json。新增101全部sdkValidated=false，原82历史SDK证据不冒充当前183联合DLL验收；原SDK未替换。下一轮按XMLCOMPILER_PRIORITY.md选择新的完整组，或仅在明确获准SDK时批量集成新增能力。
