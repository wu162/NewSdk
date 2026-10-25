# EXE integration current checkpoint

Start: 2026-10-25 19:59 Asia/Shanghai. Current implementation branch: `refactor/compiler-exe-integration`.
Baseline source: `b122a54abe4405516b021e425b8fb9235d914a0e`. Scope/gates: [handoff](EXE_REFACTOR_HANDOFF.md).

B0 preparation in progress; no architectural refactor or directory moves have started. The new joint baseline restores full pinned Upheaval input `068643219fded09a05fb7b75c6507b769c430316`, tree `d48cdbd6fec7b09aa11cf6a767fa6f59476fa6f6`, into the runner only. Original BAB must pass before building/running the joint recovered group. Eight explicit managed targets are installed; all recovered native bridges compile against the same recovered Core. Cache is disabled, outputs isolated; twenty four-output hashes must match. Production Core tracing is environment gated and records actual plugin/verifier calls, XML/XInclude and assembly path/hash. EALABuild/Remote remain inventory-only for this extracted BAB flow.

New workflow: `.github/workflows/joint-sdk-baseline.yml`; receipt: `recovery/evidence/joint-baseline/<run>-<attempt>/validation.json`. B0 is NOT complete until terminal success, real coverage and exact runtime hashes are confirmed. R0/R1/R2/R3/R4 not started.

Read-only original binary audit: both TextureCompiler and AudioCompiler are x86 CLR2 mixed modules with native VTable fixups; Audio has TLS. Both request unsigned Core 1.0.0.0. Both reference eleven Core business types; neither has a Utility AssemblyRef/TypeRef/native import. Hash/Metrics/ProcessUtility AssemblyRefs contain no TypeRefs. This supports trying a Core-identity EXE, but does not prove runtime compatibility. Never rewrite these mixed DLLs with ordinary Cecil.
