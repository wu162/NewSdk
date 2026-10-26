# MOD compiler refactor: P0–P3 task

Delivery branch: `refactor/modcompiler-p0-p3`, based on `87cf54e254dfb44a129da087316fd04806ed2b10`.
User authorized up to five child agents for this task. Stop after P3; no P4/P5 production native integration or full MOD execution in this task. PR required; do not merge without explicit user approval. No force pushes.

Implementation contract: [NewSdk_ModCompiler_Refactor_Plan.md](NewSdk_ModCompiler_Refactor_Plan.md).

Current checkpoint (not final acceptance):

- P0: initial source/PE/dependency/state inventory saved under `evidence/refactor`. Original SDK five invocations passed in run `40268595033`; combined restored deployment was blocked by nested tool Platform output paths. Corrected baseline harness committed at `1d260df`, rerun pending. Do not migrate production source until this gate passes.
- P1: minimal unified linker now works with **AnyCPU build-time C# modules and final x86 MSVC image**. Run `40269733403`, source `fafc306`, Debug/Release each 610 Armor/Hash transcript rows, isolated runtime without modules/old business DLLs. Earlier x86 IL modules produced LNK1302; do not change final EXE away from x86. Full Foundation resources/metadata and G2 A/B/C remain pending.
- P2: App contracts passed Debug/Release 305 assertions each in run `40269181634`, source `35f6112`. Actual 16 reports and 154 events passed JSON Schema/RFC3339 validation from that run's artifact. Evidence under `evidence/refactor/app/40269181634-1`. Default plan is stream discovery; actual engine dependency closure and output promotion remain later phases. `build` explicitly fails with exit 3 until the production backend is integrated.
- P3: Foundation manifest, IL-only metadata repair, original-byte resource checks, full host validation, and production source patch are prepared but not yet applied/accepted. Audio still lives at root `src/AudioCompiler` until G0 succeeds. `build/Managed.SourceChanges.patch` is a checkpoint, not proof of integration.

Workflows: `refactor-gates.yml` runs fixed G0 with `[refactor-baseline]`; `refactor-probes.yml` selects `[refactor-link]`, `[refactor-texture]`, or `[refactor-app]` independently. `[refactor-probes]` intentionally runs all probe jobs. All fixed baseline checkouts remain separate from new sources; evidence upload does not auto-commit.

Next: complete G0, apply tested source changes and relocate audio with all active paths updated; reuse Debug/Release Foundation modules in full G1; run G2 redirected Texture against the clean full EXE; verify final phase evidence and submit a PR. No unsupported gate is to be marked passed merely because the build starts.
