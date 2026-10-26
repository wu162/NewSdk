# P0 pre-refactor baseline

Fixed source: `87cf54e254dfb44a129da087316fd04806ed2b10`.
Fixed complete Upheaval/PublicCases input: `068643219fded09a05fb7b75c6507b769c430316`.
`inventory.json` is a static dependency/resource inventory; `dependency-metadata.json` records original PE/CLR identities, AssemblyRefs, project TypeRefs, resources and native imports. These are not Windows or SDK acceptance.

`refactor-gates.yml` invokes `RefactorBaseline.ps1` against separate clean checkouts of these exact revisions. Both implementations use the same absolute SDK/MOD/output paths, schema/data/search options, full MOD inputs and registration topology. The disposable SDK is rebuilt between sides. Original BAB is permitted only as this isolated oracle; this harness is not the new production build path.

The combined side installs recovered Core, MvpXml, ExpressionEval, verifier, Audio, XML, Tokenizer routes and W3X. Remote/EALABuild are installed for fidelity to the existing joint deployment, but installation is not a claim that BAB called their APIs. Utility/Hash/Metrics/Texture and ANTLR remain original at this stage. All actual config routes and installed DLL hashes are saved. Audio uses the same test adapter on both sides; the original audio DLL is physically removed in the recovered SDK, and its actual buffers/CustomData/call count are compared. XML/GameObject trace coverage checks registered public cases; Weapon has no trace sink and is covered by whole output comparison, not a claimed callback counter.

Successful runtime receipt requires 10 BAB invocations over cases 3/4/5 (AptUI, AdditionalMaps metadata, Mod.xml), 20 identical four-stream outputs, identical CustomData file names/sizes/bytes, actual audio callback equality, and registered public-case trace coverage. Per-run artifacts include inputs, SDK/Schema hashes, toolchain, effective configs, arguments, working directories, outputs, callback traces and `validation.json`. A failed/absent receipt keeps P0 incomplete.

## Lifecycle and semantic anchors

- `PluginDescriptor.Initialize` uses `Type.GetType` and `Activator`, populates handled type IDs, then invokes plugin `Initialize(null, platform)`; `ReInitialize` reuses the existing instance and calls `ReInitialize(null, platform)`.
- `PluginRegistry.ReInitialize` invokes every descriptor and replaces its type metadata cache. `AssetBuilderPluginsVersion` sums the `AllPlugins` mapped values plus the default plugin version. One plugin registered under multiple types appears multiple times; altering registration topology can alter cache/version semantics.
- `DocumentProcessor` contains static missing-reference, document/stream stack and timing/type statistics state. A new processor instance is not proof that per-stream state is reset after failure or cancellation.
- `Settings.Current`, hash provider state, path mapping and schema/resource configuration are existing ambient state. P2/P3 source consolidation does not establish a safe same-process multi-stream lifecycle; that requires the later runtime G3 checks.
- The current `ExpressionEvaluatorWrapper` reflectively loads `BinaryAssetBuilder.ExpressionEval`; P3 must replace this with the in-EXE service without silently disabling expressions on construction failures.
- Core embeds `BinaryAssetBuilderProject.xsd`; MvpXml embeds its original `.resources` names and requires EXSLT metadata restoration. Utility requires its existing metadata identity/signature treatment. The inventory records source bytes; G1/P3 must separately verify emitted resources and metadata.

## Parameter fidelity

The fixed G0 command matches `SDK/defaultscript.cs` cases 3/4/5: `/csc:false /ls:true /osh:false /pc:true /res:true /slowclean:true /ss:true` and explicit `art/audio/data`, `od/iod`. `/pc` means use precompiled assets, `/csc` means compressed session cache, and neither is a platform/cache master switch. G0 retains the legacy resident argument only in this isolated oracle; it does not authorize resident mode in the new CLI. Paths/effective options are preserved in every invocation receipt.

GameObject token-only items, XML non-global/abstract roots, Joypad exclusions and non-PC audio remain at their existing boundaries. This phase does not expand supported asset types or introduce fallback to original Tokenizer/XML in a new compiler.
