# TextureCompiler native support reconstruction

Input: `SDK/tools/BinaryAssetBuilder.TextureCompiler.dll`, SHA256
`4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e`.
Original image base is `0x10000000`; addresses below are RVAs, not absolute addresses.
The complete original IL/native evidence is recorded by extraction run
[40191971356](https://github.com/wu162/NewSdk/actions/runs/40191971356).

Implemented independently in `src/TextureCompiler/Native/TextureSupport.h/.cpp`:

| Method | Token | RVA | Reconstructed behavior |
| --- | --- | --- | --- |
| SimpleBuffer constructor | 060000c7 | 1c20 | size=0, capacity=input, data=malloc(input), returns self |
| SimpleBuffer Reset | 060000c8 | 56b0 | sets only size=0, preserves storage and bytes |
| SimpleBuffer Append | 060000c9 | 1a00 | doubles capacity until enough, reallocates, memcpy at old size, increments size |
| SimpleBuffer scalar deleting destructor | 060000cb | 1e90 | frees non-null data, clears pointer/size/capacity; recovered caller retains object storage |
| ReverseEndianness | 060000bf | 5350 | reverses all four bytes of a uint32 |
| Direct3DCreate9 | 060000c0 | 1c00 | returns constant pointer value 1, ignoring argument |
| Direct3D.Release | 060000c1 | 1ac0 | returns constant uint32 1, takes no argument |

The recovered POD uses a native pointer followed by two uint32 fields; it has
exact original 12-byte layout on x86. Portable x64 deterministic tests exercise
behavior, not original x86 ABI. Destructor reconstruction is the native body;
its original flag=1 `operator delete(self)` behavior is intentionally kept with
the allocating caller rather than exposing cross-runtime object deletion.

Original successful input behavior is retained. Zero-capacity growth originally
loops forever; uint32 overflow and failed realloc can corrupt memory or loop.
The recovery rejects zero-capacity growth/overflow and throws on allocation
failure. These inputs are not claimed byte-identical to undefined or hanging
original behavior. The original memcpy requires non-overlapping source and
destination; self storage invalidated by realloc is outside the valid domain.

`TextureSupportTests.cpp` passed local g++ C++17 `-O0` and `-O2` with warnings
enabled, comparing 33,048 payload bytes per configuration, reset/capacity and
destruction behavior. This is deterministic portable validation, not original
Windows component acceptance.

`TextureSupportOracle.cpp` is a Windows x86 CLR4 native differential host. It
verifies original DLL SHA256 before invoking the real native bodies by fixed
RVA. It compares constructor return/fields, capacity growth, payload bytes,
source preservation, reset and repeated non-deleting destruction, 262,144
endian inputs and the two constant-return facades. Compile with:

```
cl /nologo /clr /EHsc /std:c++17 /Od recovery/tests/TextureSupportOracle.cpp src/TextureCompiler/Native/TextureSupport.cpp /Fe:TextureSupportOracle.exe
```

Run in an isolated directory containing the original SDK tools dependencies and
an exe.config enabling `useLegacyV2RuntimeActivationPolicy="true"` with CLR4.
Repeat `/Od` and `/O2`. Windows execution results are recorded by the parent
validation workflow; this document does not predeclare them successful.

## Remaining native boundary

`Direct3D.CreateDevice` RVA1c10 jumps to internal RVAe097, `D3DDevice.Release`
RVA1ad0 to df8a, `D3DResource.Release` RVA1940 to c5d2. `D3DTexture.LockRect`
RVA1950 calls db40 then **unconditionally returns 0**; `UnlockRect` RVA1980 calls
c751 then **unconditionally returns 0**. These target a statically linked
resource/device implementation, not imported system Direct3D9 COM methods. It
would change behavior to replace them blindly with a system Direct3D9 API.
The DLL imports no d3d9/d3dx DLL in its static PE import table.

This support group does not restore those internal device/resource routines,
`D3DXCreateTextureFromFileA`, NVIDIA `nvDDS.nvDXTcompress`, or XenonBundler /
Xbox texture conversion. The two constant facades must not be interpreted as
providing those larger algorithms or making a working graphics device.
