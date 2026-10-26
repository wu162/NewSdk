# G2 Texture direct-to-EXE experiment

This is an isolated build-time experiment, not the deployed texture adapter.
Original `SDK/tools/BinaryAssetBuilder.TextureCompiler.dll` is never overwritten.

`Writer` pins dnlib 4.5.0 and calls **NativeWrite**, preserving metadata row IDs and
heap offsets as far as dnlib supports. It accepts only the audited original
SHA-256. `roundtrip` changes no intended semantics; `redirect` changes only the
Core AssemblyRef to a supplied EXE's actual assembly identity. The shared
`BinaryAssetBuilder.Core` namespace string is never overwritten. The writer
checks TypeRef/MemberRef token/name invariants and requires public EXE members
with matching dnlib signature text for direct Core MemberRefs. This static check
does **not** replace layout/enum/interface/runtime ABI acceptance.

On Windows x86 with .NET Framework/VC80 CRT, .NET SDK 8 and Python:

```powershell
python -m pip install -r recovery/probes/TextureRedirect/requirements.txt
./recovery/scripts/TestTextureRedirect.ps1
# C additionally requires a complete Core-owning final EXE exposing
# --texture-probe -> TextureProbe.Run(args.Skip(1).ToArray()).
./recovery/scripts/TestTextureRedirect.ps1 -TargetExe <isolated-final-exe> -RequireComplete
```

A and B run three actual Win32 ProcessInstance routes, each success → missing
input → success in one plugin/process, comparing all three buffers and CustomData
byte for byte. Native and managed load paths are recorded. An invalid original
fixture fails A before any writer result can be accepted. Metadata lookup,
Initialize/ReInitialize and Dispose are exercised. A/B still use the original
Core oracle and cannot establish C. No stubs are used to pretend Core is in EXE.

`AuditPe.py` compares x86/entry points, imports, resources, TLS, IAT, relocations,
VTableFixups, managed-native header, metadata tokens/names/signature blobs and
native MethodDef RVA ranges. The original metadata and CLR header are excluded
from native-range byte comparison, since they are intentionally writer-owned.
Unlisted native functions are covered by gaps between known MethodDefs but are
not independently disassembled; that remaining limit is explicit in the receipt.
Any protected-range change fails. No automatic fallback patches or forwarding
assemblies are generated.

The initial fixture is a deterministic uncompressed 512² BGRA TGA. It must first
be proven valid in A; it is not an existing SDK acceptance fixture. This component
probe does not exercise XSD/preprocessing and cannot replace the real SDK input
and routing portion of G2. G2 is complete only after all these requirements and
C pass on the actual final EXE. `-RequireComplete` fails if C cannot run; the default
A/B-only mode records `partial-A-B-only`, never `passed`.
