# Public source reuse: exact-output gates

2026-10-28, Win32/PC only. User authorized copying available public implementations onto test branches and merging only byte-identical results. Production SDK DLL/config remain untouched. No force pushes.

## DXT / STB experiment: not eligible for merge

Test branch [validation/texture-public-codecs](https://github.com/wu162/NewSdk/tree/validation/texture-public-codecs). Build/replay source94ebf7886592843a06f6e1bc5cacaf372ae0b4ff, Actions [40627584385](https://github.com/wu162/NewSdk/actions/runs/40627584385) success means the comparison completed; it does NOT mean codecs matched. Complete results and compressed candidate artifacts remain on that test branch under recovery/evidence/texture/public-codecs/40627584385-1. Unmodified sources are pinned by30 Git blob/SHA256 records.

Both configurations checked all10,240 original DXT3/5 blocks against32 parameter combinations. Best complete16-byte matches per5120 inputs, identical counts in Debug/Release:

| Copied implementation | DXT3 | DXT5 |
| --- | ---: | ---: |
| libsquish1.15, 12 configurations | 481 | 1 |
| stb_dxt, 2 configurations; squish explicit alpha for DXT3 | 288 | 232 |
| NVTT ICBC, 18 configurations; squish alpha | 1217 | 2 |

Full artifact inspection also found different RGB565 endpoints on the same random input1024, rather than merely alternate indices for a constant color. Original DXT3 payload is `93f22c7b192dde8c4b8fd380d7222d9e`; the best ICBC variant17 produced `93f22c7b192dde8c2c86b480d7200d18`. Original RGB endpoints are36683/32979, candidate34348/32948. The compared artifact contains327,680 records per configuration, each with input RGBA and both complete blocks.

No complete format reached5120/5120. Squish DXT3 alpha alone matched5120/5120, but its color output did not; alpha-only equality is not full-DXT equality. DXT1/1A16 saved snapshots are explicitly probe-only due to the legacy undefined default-state boundary. No DXT default support is inferred from them.

STB JPEG matched272/1280 decoded images and1/4 original rejection records, after retaining the same validated downstream POT/L8/RGBA8 adapter. It is not eligible to replace the original-compatible decoder. Failed/partial copied implementations are not enabled or copied into the texture branch by this integration branch.

The first experiment run40627464821 had invalid YAML and executed no jobs. Its persistence command was changed to a YAML literal block; 40627584385 completed the actual comparison. Both run states remain unmodified.

## IJG6b: full component equality, SDK milestone pending

Existing public-source candidate699479a95c73012f5e66b74a3d7d1e43f1d8b51f / [40626161229](https://github.com/wu162/NewSdk/actions/runs/40626161229) completed/success. Debug/Release each1284 ImageLoader records (1280 complete pixel/metadata matches,4 exact original rejections),23,105 checks and888,960 RGBA bytes; the six gray NPOT cases are resolved by original L8 storage precision.

Debug/Release each11,248 real plugin cases,16 original errors,6 retained default DXT1/1A rejections,82,231,440 bytes compared, plus the same complete NoOriginal replay. Includes raw/DXT3/5, standard/volume/mips, old cdata and32 JPEG-coded atlas variations. The other11 affected component groups passed in the same run.

Clean integration test branch [validation/texture-public-jpeg](https://github.com/wu162/NewSdk/tree/validation/texture-public-jpeg) starts from texture branch05208638 and contains no failed DXT/STB candidate source. It reuses the accepted component evidence, then explicitly runs only affected SDK profiles through the existing single batched Windows job. NewPcJpeg has3216 assets per Original/Recovered/NoOriginal mode, including3200 texture combinations and16 atlas assets. SDK sourcee8835cfee56cb35376a6bc1cb18a40d189263842 / Actions40628208308 is running. It selects9 SDK profiles while reusing the accepted component groups. SDK results remain pending until their own complete receipts exist; merge this clean branch only after all required profiles and four outputs/full call traces pass.
