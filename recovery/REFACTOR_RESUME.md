# New MOD compiler: P0–P3 source handoff

Delivery branch: `refactor/modcompiler-p0-p3`, based on `recovery/compiler-source`.
Task scope stops after P3. Up to five child agents are authorized for this task only.
Any merge requires a PR and explicit user approval; force pushes are prohibited.

The implementation contract is [NewSdk_ModCompiler_Refactor_Plan.md](NewSdk_ModCompiler_Refactor_Plan.md).
This entry describes source organization and continuation instructions. Publication of the new
acceptance summaries and artifact links is awaiting separate authorization after automatic review.
It does not replace the actual workflow receipts.

## Source and build entry points

- `src/App`: directory CLI, stream discovery/dependency-plan interfaces, sequential BuildSession, isolated StreamContext, diagnostics, JSONL and terminal report contracts.
- `src/Managed`: Core, Utility, Hash, Metrics, MvpXml, ExpressionEval, GameDataVerifier and relocated Audio sources. `build/Managed.Foundation.json` defines production inputs and resource hashes.
- `scripts/BuildCompiler.ps1`: the separate P3 production CLI/Foundation executable. Its thin `src/Hosting/CompilerEntry.cpp` contains no native asset routes. Probe and test classes are excluded from this product.
- `scripts/TestCompiler.ps1`: clean-directory product type/resource/entry/CLI checks. `TestManagedFoundation.ps1` checks managed behavior; `TestSingleExeProbe.ps1` separately checks cross-language linking and Armor differential.
- `probes/TextureRedirect`: build-time native mixed-PE writer, protected-range audit and original/roundtrip/EXE-bound component probe. It never overwrites the SDK original DLL.
- `protocol`: event/report schemas and compatibility rules. `TestApp.ps1` and `ValidateAppProtocol.py` cover the application contract.

Use Windows with .NET Framework 4.8 developer reference assemblies and VS 2022 x86 C++/CLI tools.
Roslyn is pinned by the Foundation build. Build-time C# modules use AnyCPU; the final linker and
process remain Win32/x86. The declared external managed dependency is `antlr.runtime.dll`.
The old texture DLL additionally needs its original native runtime when its probe is selected.

```powershell
./recovery/scripts/BuildCompiler.ps1 -Configuration Release -Platform Win32
# Use the returned bin/NewModCompiler.exe path:
NewModCompiler.exe --help
NewModCompiler.exe plan --mod "D:\Mods\Example" --sdk-root "D:\SDK" --out "D:\Build\Example"
```

The CLI defaults to selected `base,apt,maps` stream discovery and loose output intent. It exposes
known legacy parameters without silently claiming their backend behavior is integrated.
`build` currently returns `BACKEND_NOT_INTEGRATED` with exit 3; `doctor` also discloses that limitation.
A successful `plan` means a plan was produced, not that assets were compiled.

## Continuation boundary

P4 must add shared Native/Bridge and complete built-in asset routing to the production executable.
P5 must connect the real Core stream engine, dependency closure, cache/state cleanup and output
promotion to one process/BuildSession, then compare real MOD outputs. P6 packaging and P7 complete
postprocessing are separate work. Do not infer support for those phases from the P3 CLI skeleton.

Old independent projects remain oracle/compatibility entry points. Under `NEWSdk_MONOLITHIC`,
expression binding is direct, plugin factories are explicit, serialized Settings resolves locally,
and old GUI/Remoting sources are outside the production manifest. Historical audio receipts keep
their original source-tree identities; source migration is not permission to relabel old results.

`refactor-gates.yml` is the fixed pre-refactor baseline harness. `refactor-probes.yml` has explicit
markers for link, texture, app and full integration checks. Both new workflows are read-only.
Touched legacy audio workflows preserve their test/artifact steps and no longer write back to Git.
Do not repeat a complete SDK baseline merely for a wrapper or documentation correction.
