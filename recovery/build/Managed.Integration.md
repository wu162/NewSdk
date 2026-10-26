# Managed Foundation integration

G0 run `40269297741` completed the SDK comparison before these production changes.
Production bindings compile under `NEWSdk_MONOLITHIC`; existing library projects do not
define it and remain available for the isolated legacy oracle. Windows full Foundation
validation is tracked separately; do not infer success from the G0 baseline.

`BuildManagedFoundation.ps1` compiles the manifest's full source closure with Roslyn 4.0.1, C# 10,
.NET Framework 4.8 reference assemblies, AnyCPU build-time IL module, deterministic output, and only `antlr.runtime` as a
business AssemblyRef. It excludes duplicate AssemblyInfo and old GUI classes. Core's ShPath UI
helper is conditionally excluded in the production build. EALABuild/Remote are not inputs.
No decompiled directory or original implementation DLL is compiled into the module. Final MSVC
link and test host remain strictly x86. G1 minimal run `40269733403` at `fafc306` proved
AnyCPU intermediate modules avoid LNK1302 without altering unsafe attributes or IL; Debug and
Release each matched 610 oracle rows. This is not evidence for the full Foundation combination.

The module has no resources. `resources.json` contains absolute paths, logical names and SHA-256;
the final MSVC linker must embed these exact five resources. Core schema uses a canonical CRLF
copy matching original DLL SHA-256, independent of checkout autocrlf; all four MvpXml binary
resources are copied verbatim and fixed SHA-256 verified. No input module may be shipped.
`ManagedMetadata` only rewrites a pure IL build-time netmodule and restores Utility C++ type names,
original public/family flags, signature modifiers, and 51 EXSLT CLR names. Original Utility is read
as a contract oracle, never copied into the output. The tool rejects unexpected AssemblyRefs and
never writes the final mixed executable.

The conditional production source directly constructs the expression evaluator, resolves the one known
Settings serializedType into the current assembly, and requires explicit factories for production
PluginDescriptor initialization. `BuiltinManagedPlugins` exposes the recovered audio and verifier
factories; native compiler registration belongs to P4. Legacy targetType strings cannot silently
load old business code in this production build. Unknown serialized types are rejected.

`TestManagedFoundation.ps1` builds Debug/Release modules and C# test-only multi-file assembly hosts,
using no old business DLL. It exercises resources, aliases, Utility storage, expression arithmetic,
hash binding, audio initialization/reinitialization/disposal, and verifier creation. This is **not**
the single physical executable proof. `ManagedFoundationValidation.Run(true)` reads an independent `Foundation.types.txt` inventory
emitted from the repaired module; this test data file must accompany the clean G1 validation
EXE. Every business type from the inventory must resolve into the entry EXE.
`ManagedFoundationValidation.Run(true)` is the additional
G1 post-link test, asserting all implementation types belong to the entry executable and that only
one module remains. `ManagedMetadata verify-linked <exe> <original Utility DLL>` performs read-only final
metadata verification of all Utility flags and custom modifiers; it never writes the mixed PE.
Existing full component differentials remain separate required evidence for
behavior affected by changed implementation, ABI or registration. A module test does not replace
real SDK pipeline acceptance.

Audio source is now `recovery/src/Managed/Audio`; its standalone project retains the isolated
Core oracle reference at the corrected relative path. Active script inputs, workflow path
filters and future source-tree receipts follow the new path. Historical sourceCommit/tree
hashes and historical evidence files retain their old path and have not been rewritten.
