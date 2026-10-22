# Compiler source recovery

Target: the two original DLLs in `SDK/tools` in this repository. No source or conclusions from older recovery projects are assumed.

## Current state

The first Windows evidence extraction is being prepared. No replacement compiler is built or validated yet.

`recovery/decompiled` will contain mechanically decompiled C# evidence, including invalid representations of C++/CLI constructs. It is **not** claimed to be buildable recovered source. `recovery/evidence` will retain per-method tokens, RVAs, code kinds, IL bytes, type layouts, references and original hashes. IL and native disassembly archives are retained under `recovery/logs` and in Actions artifacts.

The original configuration routes `GameObject`, `WeaponTemplate`, and `ArmorTemplate` to `BinaryAssetBuilder.Tokenizer.Plugin`; other XML types fall through to `BinaryAssetBuilder.XmlCompiler.Plugin`. This is the starting integration contract, not a guess from a different game SDK.

## Next substantial milestone

Inspect extracted Plugin entry points, compiler dispatch, allocator/relocation ABI, native methods and EALAModStudio's exact basic-data command. Reconstruct readable managed and C++/CLI projects from that evidence. Preserve native code gaps explicitly; decompiler stubs do not constitute recovered native source. Build on Windows x86, then compare the example mod's basic-data results against the unmodified original tools.

## Validation

None yet. Do not report extraction success as source compilation or mod compilation success.

## Resume

Use branch `recovery/compiler-source`. Read this file, the latest workflow and extraction summaries. Continue directly from actual committed evidence; do not impose a microtask schedule.
