# PC DXT alpha components

`TextureAlphaQuantizer.cpp` builds `Recovered.TextureDxtAlpha.dll` for Win32/x86.
It reconstructs the native quantizer at RVA 0x7D500 in the fixed SDK oracle, with
owned constants, stack probing, clearing, floor and integer conversion helpers.
The source preserves x87 instruction order; it does not execute an original DLL
or ship copied machine-code blobs. It has no import table or DLL entry point,
so loading it cannot initialize a CRT or change the caller's floating-point mode.

From an x86 MSVC developer shell at the repository root (output directory must exist):

```powershell
cl /nologo /LD /MT /fp:strict /GS- /O2 /Fo"<output>/quantizer.obj" src/TextureCompiler/Dxt/TextureAlphaQuantizer.cpp /link /NODEFAULTLIB /NOENTRY /OUT:"<output>/Recovered.TextureDxtAlpha.dll" /IMPLIB:"<output>/quantizer.lib"
dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c Release -o <output> --nologo
```

For this repository, run compilation and tests through GitHub Actions only:
`texture-validation.yml`, `force_groups=dxt5-alpha`. The script builds both
configurations, validates the native PE, compares native error bits/endpoints/
indices and actual DDS alpha bytes, and replays the complete byte corpus in a
separate process without the original TextureCompiler DLL.

`TextureDxt5AlphaEncoder` calls this helper for production-quality alpha search.
`EncodeBlock` consumes 16 RGBA pixels; `EncodeAlphaBlock` consumes 16 normalized
alpha floats without converting them through bytes. Both produce only the eight
alpha bytes. They do not produce colour blocks or complete DDS files and are not
connected to the plugin's unsupported DXT route. Future integration must include
native-helper build/copy steps and the affected consumer dependencies in the test
catalog. Existing SDK DLL/config are untouched.

The private managed algorithm reference is not the production implementation:
it chooses the opposite equal-error orientation for the symmetric five-point
case. Preserve that failing evidence and exact comparisons when attempting a
portable managed implementation; do not substitute an error tolerance.

`TextureDxt5FastAlphaEncoder` is a separate quality-68 low-level state component.
The original fast function does not initialize its `levels` field, while the
DDS writer reads it. Its packed internal state is not accepted as a complete
DDS alpha implementation. The original entry discrepancy remains reproducible
with `TestTextureDxt5FastAlpha.ps1 -InspectDdsEntry` in Actions; it is inspection,
not a passing component gate or a replacement for the default production path.
