# PC DXT alpha recovery

## Scope

`TextureDxt3AlphaEncoder` emits the eight explicit-alpha bytes for a 4x4 RGBA
block. Its original acceptance remains 9f8b1361 / Actions 40470055551.

`TextureDxt5AlphaEncoder` reconstructs the default PC production-quality alpha
path: eight-level/six-level candidates, zero/one exclusion candidates, endpoint
search, integer palette/nearest-index selection, equal-endpoint canonicalization
and PC DDS alpha packing. It emits eight alpha bytes, not a colour block or a
complete DDS. `EncodeBlock` accepts 16 RGBA pixels; `EncodeAlphaBlock` accepts 16
finite normalized alpha floats without an intermediate byte conversion.
`Quantize` supports levels 6/8 and up to 512 normalized values.

No plugin DXT route is enabled. DXT1/DXT1A and DXT3/DXT5 colour compression,
complete compressed mip/volume/atlas output and real SDK integration remain
unfinished. Existing raw rejection of unsupported DXT is unchanged. SDK DLLs and
configuration remain untouched.

## Source and native dependency

The managed orchestration is in
`src/TextureCompiler/Managed/TextureDxt5AlphaEncoder.cs`. The quantizer is rebuilt
from x87 assembly source in `src/TextureCompiler/Dxt/TextureAlphaQuantizer.cpp`.
Build and copy `Recovered.TextureDxtAlpha.dll` alongside the managed assembly;
see the [native build instructions](../src/TextureCompiler/Dxt/README.md).

All runtime constants and clearing/floor/conversion helpers are owned source.
The helper has no PE import directory and no DLL entry point. It neither calls
an original DLL nor initializes a CRT that could reset the caller's x87 state.
Its code labels retain original RVAs for auditing, not as executable addresses.
The test script verifies x86, zero entry point and zero imports and records
source/binary hashes for each configuration.

A readable managed algorithm reference remains private and unused. It is not
accepted as a portable replacement: the symmetric five-point alpha vector
`[0,0.25,0.5,0.75,1]` selects endpoints `[3,255]`, while the original selects
`[0,252]`, both with error 475.875. Exact indices differ too. Operation-order
adjustments did not close this gap. The assembly backend closes the instruction-
order gap; the exact comparisons remain in the native differential suite.

## Evidence already confirmed during this stage

- Byte alpha + quantizer + NoOriginal acceptance before the float API:
  source `97ad2829b8ca9f17d86657072c2da5a0c6621840`, Actions
  [40493322953](https://github.com/wu162/NewSdk/actions/runs/40493322953), success.
  Debug/Release each: 2,176 quantizer cases / 408,835 exact checks; 78,144 actual
  original DDS alpha cases / 1,172,168 checks; 78,144 isolated NoOriginal replays /
  625,152 byte comparisons. This is a component run, not a real SDK MOD build.
- Quantizer acceptance checks original endpoints, indices and double error bit
  patterns for levels 6/8, counts 0 through representative sizes up to 512,
  constant/binary/gradient/random/near-threshold values.
- Actual DDS acceptance covers every byte-alpha pair, all uniform values,
  per-position variation, gradients and random colours/alpha. The complete DDS
  is captured by the original harness; only its eight alpha payload bytes are
  claimed as recovered. Colour bytes are not accepted as restored.
- NoOriginal is a separate executable directory with the original TextureCompiler
  DLL absent; it replays all original records and checks the loaded assemblies.
- Early managed-only quantizer failures (40490402314, 40490888440, 40491978889),
  native build failures (40492606645, Release in 40492930678), and the float API's
  first build failure (40493606249) remain as immutable diagnostic evidence.
  They are not relabeled as successful runs.

The latest complete native/float/ABI batch is recorded below after verification.
Do not apply the earlier byte-only acceptance to an unverified changed input.

## Fast-quality limitation

`TextureDxt5FastAlphaEncoder` restores the quality-68 native **state** path only.
Its complete low-level matrix has 71,168 cases / 1,921,543 checks per configuration.
It is not a complete DDS alpha encoder. The original fast function leaves
`levels` unwritten, while the DDS writer reads that field. Entry inspection
failed at fixture 0 (equal endpoints) in 40488941255, and at fixture 256 (a
nonuniform block after constant blocks) in 40489972525 after a trial equal-
endpoint adjustment. The trial was reverted; no arbitrary DDS policy is claimed.

The failing entry assertions remain in `TextureDxt5FastAlphaEntryDifferential.cs`,
accessible through `TestTextureDxt5FastAlpha.ps1 -InspectDdsEntry` in Actions.
That switch marks the run probe-only and is not an automatic component gate.
The normal group accepts only the explicitly bounded low-level state component.

## Colour recovery handoff

Pinned Ghidra 11.4.2 successfully exported seven alpha/dispatch functions in
40488941255 (the overall batch failed its separate fast-entry check). The later
18-function request in 40490888440 exported 13 functions, then timed out while
decompiling `RoundMult@0x61460`; that extraction is failed/partial. Its 13 C files
and full log are retained in `evidence/texture/dxt-decompile/40490888440-1`.

Useful next inputs are `CodeRGBBlock.c`, `AllSame.c`, `MapAndRoundMult.c`,
`Search43Mult.c` and `CodeRGBBlockQuick.c`. `AlphaDdsWriter.c` independently records
the packing rules used in this stage. The remaining requested colour exports
were not reached after the timeout; do not treat them as collected. Next stage
should address that targeted extraction and implement a coherent colour group,
then validate complete colour/alpha/size/mip/volume outputs before enabling DXT
in the plugin or scheduling a compressed-texture SDK milestone.

## Latest complete acceptance: native + byte + float + NoOriginal + ABI

Source `4bf0ca8ccb74a6f670cd7ecb954483dc795fae9f`, Actions
[40493971846](https://github.com/wu162/NewSdk/actions/runs/40493971846), attempt1,
completed/success. Each of Debug and Release passed:

| Layer | Cases | Checks | Accepted scope |
| --- | ---: | ---: | --- |
| Quantizer | 2,176 | 408,835 | Native endpoints, indices, exact double error bits |
| Actual original DDS | 78,144 | 1,172,168 | Eight production alpha payload bytes |
| Normalized float alpha | 12,288 | 516,108 | Native CodeAlphaBlock state plus PC alpha packing |
| Isolated NoOriginal replay | 78,144 | 625,152 | Original DLL absent and not loaded |

Float cases include denormals/near-zero/near-one, constant/gradient/random values,
zero/one exclusions and all input rejection assertions. Float-state comparison
is a component test, not a full float-image DDS or compressed mip SDK run.

Both native outputs are I386, PE entry point 0, import-directory RVA 0. The
receipt records the exact CPP and README input hashes and Debug/Release native
binary hashes. This batch selected only `dxt5-alpha`; 18 unchanged successful
component groups reused their own ancestor sourceCommit/runId. No SDK was run.

Receipt: `evidence/texture/dxt5-alpha/40493971846-1/validation.json`.
Selection: `evidence/texture/batch/40493971846-1/selection.json`.
The later documentation and evidence-only extraction changes do not modify
these accepted implementation/test inputs and are not claimed as new runtime
acceptance.

The later source fix was a C# local-name collision in the float API, not a
comparison waiver. The final run includes the float API and corrected build,
and all exact assertions remain enabled.

## Concrete colour audit notes for the next stage

From the preserved `CodeRGBBlock.c`/`AllSame.c` and native assembly (static facts,
not a validated colour harness): the RGB block count is at +0x8, triples of
normalized doubles start at +0x10, three auxiliary/clamped doubles are at
+0x3018/+0x3020/+0x3028, selected level count is at +0x3034, six endpoint channel
integers start at +0x3038, and per-sample indices start at +0x3050. The +0x4 field
selects behaviour but its meaning is not yet established; do not guess its name
or initialization. CodeRGBBlock mutates its internal sample triples when clamping.
A future managed adapter should distinguish that owned working block from the
caller's original RGBA storage.

AllSame has two important data dependencies beyond the visible function body:
it selects records from RVA 0xB9608 or 0xBB608 (32-byte records) for its grayscale
path, and lazily constructs a 6,912-byte 3-channel endpoint lookup at RVA 0xC0160
using 5/6/5 expansion and coefficients at RVA 0xBD608 onward. The initialized flag
is at RVA 0xC1C60. These are analysis leads; no table regeneration or colour
output has been accepted. Do not label a copied sample list as a restored table
builder. Prove the initialization, tie-breaking and actual caller options first.

The decompiler reports misleading calling conventions/extraout registers in some
AllSame/rounding output. Use the COFF signature and original assembly for ABI and
floating-point evaluation order. This stage's five-point quantizer failure shows
why a mathematically equivalent decompiler expression is insufficient evidence.

### Remaining four colour exports collected

After authentication recovered, Actions
[40494632567](https://github.com/wu162/NewSdk/actions/runs/40494632567) was confirmed
completed/success for source `c4bb4db72440bf23827656a643022ea67c7bbb1d`.
All four requested C exports and their hashes are in
`evidence/texture/dxt-decompile/40494632567-1`; receipt status is probe-only.
Combined with the earlier 13 exports, this provides 17 of the 18 requested
functions. RoundMult remains unresolved; no colour implementation acceptance is
claimed. Do not rerun the four-function extraction unchanged.
