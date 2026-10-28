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

## IJG6b: full component and SDK equality

Existing public-source candidate699479a95c73012f5e66b74a3d7d1e43f1d8b51f / [40626161229](https://github.com/wu162/NewSdk/actions/runs/40626161229) completed/success. Debug/Release each1284 ImageLoader records (1280 complete pixel/metadata matches,4 exact original rejections),23,105 checks and888,960 RGBA bytes; the six gray NPOT cases are resolved by original L8 storage precision.

Debug/Release each11,248 real plugin cases,16 original errors,6 retained default DXT1/1A rejections,82,231,440 bytes compared, plus the same complete NoOriginal replay. Includes raw/DXT3/5, standard/volume/mips, old cdata and32 JPEG-coded atlas variations. The other11 affected component groups passed in the same run.

Clean integration test branch [validation/texture-public-jpeg](https://github.com/wu162/NewSdk/tree/validation/texture-public-jpeg) starts from texture branch05208638 and contains no failed DXT/STB candidate source. It reuses the accepted component evidence, then explicitly runs only affected SDK profiles through the existing single batched Windows job. NewPcJpeg has3216 assets per Original/Recovered/NoOriginal mode, including3200 texture combinations and16 atlas assets. SDK sourcee8835cfee56cb35376a6bc1cb18a40d189263842 / [40628208308](https://github.com/wu162/NewSdk/actions/runs/40628208308) completed/success. All9 selected SDK profiles passed;32 accepted component groups were reused. NewPcJpeg has3216 assets and3216 actual Core calls in each ofOriginal/Recovered/NoOriginal, with all four outputs and full trace files identical. NoOriginal checks the original DLL absent/unloaded during every call. Aggregate assets/calls per mode across9 profiles are13,104. The source/runtime inputs have not changed after this acceptance. This clean branch is eligible for ordinary merge; the failed public DXT/STB branch remains separate.

JPEG SDK four-output hashes are recorded in sdk-jpeg/40628208308-1/validation.json: mod.bin12,181,924 bytes, SHA256 bd67f637e67ef7fe3b875ab671bea6542e47ebaf28318df398ac904a407697c1. Complete traces.zip SHA25666235fd393668f504e2616faef30e80f46878665cd5c6bbab3b513cd87cbf9bd was checked against the saved artifact. This is a dedicated MOD, not complete mymod/production deployment.


## Follow-up: modify the copied code until the legacy contract matches

The user clarified that stock mismatches should drive changes to the copied
source. Compatibility work is active on validation/texture-public-codecs;
failed stock candidates are retained as baselines, not a reason to stop.

ATI/AMD legacy CPU code was copied from GPUOpen-Tools/compressonator at
095a143ad8cfbf9ed5afa4ea63c735bfd0fd04dd. Baseline source8520760e /
[40629834780](https://github.com/wu162/NewSdk/actions/runs/40629834780)
completed the full Debug/Release12-variant matrix: best DXT3=369/5120,
DXT5=308/5120, DXT5 alpha=2400/5120. No integration eligibility.

First compatibility sourcee1ecde44 /
[40630771403](https://github.com/wu162/NewSdk/actions/runs/40630771403)
completed Debug/Release42-variant matrices,430080 complete block records per
configuration. Actual copied-source changes: libsquish equal565/constant-alpha
canonical form, ICBC equal565, AMD five alpha candidate classes/low-first ties,
normalized-float RGB input. Best results, same in both configurations:

| Patched copied source | DXT3 full/5120 | DXT5 full/5120 | DXT5 alpha/5120 |
| --- | ---: | ---: | ---: |
| libsquish | 483 | 1 | 269 |
| ICBC + squish alpha | 1217 | 258 | 269 |
| ATI/AMD | 374 | 311 | 2771 |

These improvements still do not establish complete equality. Upstream identities
and patch hashes are separately preserved in the test branch PROVENANCE.json and
PATCHES.md. No lookup of expected fixture payloads or original/recovered encoder
callback is used. The next AMD iteration replaces its scalar grid search with a
constrained fit/local-move solver adapted from the readable reference; scalar/x87
ties remain an explicit acceptance issue.

A closer public S3 scalar algorithm family was also found in
Team-Resurgent/RXDK-Libs@b7c9f2b3a24ece1aec850dca52155a4f8633686c,
libs/libxgraphics/dxtc/{s3_quant.cpp,S3_quant.h}. Only shared CPU algorithms are
built on PC; no non-PC API or platform route is reintroduced. Source36637170 /
[40631487946](https://github.com/wu162/NewSdk/actions/runs/40631487946)
compiled but failed after2 saved records: allSame chose a three-colour BC1
midpoint for a forced-four-colour DXT3/5 caller. Source81268e60 patches that
contract in the copied allSame search. This is a general format rule, not a
fixture-specific output change. The failed partial run is retained as failure.


Second AMD adaptation1ce890cf /
[40631302274](https://github.com/wu162/NewSdk/actions/runs/40631302274)
completed successfully: D/R all12 variants match the complete5120 DXT5 alpha
payloads. RGB still limits complete blocks to374/5120; no full-format eligibility.
This byte-domain result does not replace the broader normalized-float quantizer
or consumer/SDK gates. Existing recovered x87 helper remains the accepted backend.

S3 forced-four-colour source81268e60 /
[40631683152](https://github.com/wu162/NewSdk/actions/runs/40631683152)
completed both configurations: variant0 (normalized-float pixels, float channel
weights) matches4838/5120 complete DXT3 and DXT5 blocks, alpha5120/5120.
Variant2 with double channel weights has the same count; double source pixels
are worse. Full artifacts show remaining mismatches:252/256 first grayscale
blocks,8/256 other single-RGB blocks (three also gray),0/256 two-color,
0/256 constant-alpha gradient,22/4096 random.21/22 random candidates have lower
decoded squared-channel-weighted error than legacy output, so merely improving
compression quality cannot establish output compatibility.

Source02557975 /
[40631926236](https://github.com/wu162/NewSdk/actions/runs/40631926236)
compared scalar IA32/x87 compilation. All4 variants retain the same complete
counts and saved payloads as SSE2; architecture alone did not fix the differences.

Next source adaptation uses the existing fixed256 grayscale optimization records
from owned TextureColour.asm D_000B9608+8192 / original RVA0xBB608. These are
original codec constants, with source/data fingerprints, not the differential
corpus outputs. The result is described as a hybrid S3/legacy compatibility
implementation. It does not call original DLL or recovered encoder functions.
Explicit source policies compare upstream16/8 and lower0/0,1/1,2/2 search budgets,
all four input/weight precisions, all10240 original blocks, Debug/Release. No
fixture-based policy selection is allowed. These newer results are pending.

Grayscale-source run40632042322 failed native linking because the new PowerShell
architecture argument was a single string splatted into characters; it did not
run the matrix. The script now explicitly keeps an array (a61e2b58). Preserve
failed runs and do not describe them as codec-output mismatches or acceptance.


Grayscale and stopping-policy sourcea61e2b586c03f3ad58f0c6c1ba88679e52a9a822 /
[40632315333](https://github.com/wu162/NewSdk/actions/runs/40632315333)
completed the full16-variant matrix: D/R each163840 full original/candidate
records, best complete DXT3=DXT5=5093/5120 (99.47%), all alpha5120/5120.
All grayscale/two-color/constant-alpha-gradient cases now match. Five single-RGB
and22 random fixtures remain. Budgets0/0 regress4 and fix none;1/1 and2/2 do not
improve, so upstream16/8 is restored. The original stopping hypothesis is not
supported by this matrix. Run40632288601 shares the earlier argument-splat link
failure and is retained as failed, not as a codec comparison.

Remaining variant0 fixtures:263,281,337,356,462;1361,1445,1690,1937,2132,2144,
2405,2487,2527,2758,2878,2944,3010,3618,3651,3787,3910,4001,4268,4299,4838,5115.
These are diagnosis identities only; no runtime fixture dispatch is implemented.
New e5fbd08a tests independent single-colour/general-ramp floor interpolation
against baseline, all4 precision variants per policy, same complete corpus.
The variant policy meaning changed explicitly at that source; earlier receipts
must keep their original source-specific interpretation. Its result is pending.


## Final collected rounding result and next step

Sourcee5fbd08a8275af071c08b65a3fd579653d915cd7 /
[40632587996](https://github.com/wu162/NewSdk/actions/runs/40632587996)
completed/success. D/R each16 policies/precisions,163840 records;
raw artifact SHA2569ecff36fad03558b2199fc68839e5f4db2b7d44ff9d30e6f78655accd569298b
is identical between configurations. Baseline variant0 and2 remain best at
5093/5120 complete DXT3 and DXT5, alpha5120/5120. Single-colour floor gives4892,
general-ramp floor1648, both1447 at float source/weight precision. Single-colour
floor fixes none of the27 and regresses201; general floor fixes fixture1690
but regresses3446. These broad floor substitutions are rejected. The accepted
experimental baseline keeps rounded interpolation and upstream16/8 iteration
budgets, with the explicit fixed grayscale adaptation. All alternatives remain
isolated as experimental policies and are never selected per fixture.

No candidate is eligible for full-format integration: all receipts keep
productionAccepted=false/sdkValidated=false. No
public DXT candidate was merged into production TextureCompiler. All new builds
and replays ran in GitHub Actions. No run remains pending at this handoff.

Continue on the copied S3 source, instrumenting its single-colour selection and
final ramp/search stages for the27 fixed outliers. Reuse already saved colour
state evidence under colour-state/40500150864-1 and complete block artifacts;
inspect its receipt/scope first, rather than recollecting the same original
inputs. Compare general rules/precision and stage choices. Do not repeat the
rejected x87, lower-budget or broad-floor matrices unchanged. Do not substitute
visually better blocks or use runtime outlier-ID/payload lookup. Once complete
byte equality is obtained, preserve full float, image/mip/volume/atlas/plugin
and dedicated SDK Original/Recovered/NoOriginal acceptance before merge.

## S3 fully accepted after source adaptation

Final clean integration source705af33817d97c1a10557914e9dd80c1f3d31efe / Actions40649825971 passed8 complete affected component groups in Debug/Release with NoOriginal;24 unchanged groups reused. SDK source867e7d38a17f82f42aa4c2f12ff387fca2ca78c0 / Actions40650835181 passed all9 profiles:13104 assets and actual Core calls per mode, four outputs and complete traces identical across Original/Recovered/NoOriginal. All32 component groups were reused in SDK validation. Full source/input domain, consumer counts and independently checked hashes are in TEXTURE_PUBLIC_S3.md and TEXTURE_PUBLIC_S3_ACCEPTANCE.json.

This closes all27 original full-block differences and the downstream filtered/partial quick-index snapshot mismatch. Only adapted S3 source and explicit legacy gray constants are included in the clean integration; other failed public candidates remain isolated. Source fingerprints and original notices are retained. Production SDK DLL/config and complete mymod deployment remain unchanged/separate.
