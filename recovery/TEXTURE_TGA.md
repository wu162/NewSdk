# TGA decoder subset and original-load oracle

当前更新（2026-10-28，北京时间）：既有TGA24/32及Additional真色16、灰度8、index8配palette15/16/24/32的已声明NPOT存储路径已完成公共加载、两入口raw/DXT标准/volume/有无mip与新2560资产三模式SDK；原alpha、方向/RLE和原拒绝规则保留。 业务e9292a26/40595382578、完整入口隔离9e7d671e/40597016969及最终存储回放daf4a749/40598863269的精确收据与总体失败说明见[NPOT当前交接](TEXTURE_NPOT_NEXT.md)。下文保留历史阶段范围。

> **历史记录：自 2026-10-26 起仅以 Win32/PC 为目标。本文非 PC 能力、旧待办和旧平台矩阵不再适用；当前实现与计划以 [TEXTURECOMPILER.md](TEXTURECOMPILER.md) 为准。原 sourceCommit/runId 仅证明原验收版本，不代表本轮删除后的代码已验证。**

`src/TextureCompiler/Managed/TextureTgaDecoder.cs` is an independently written CPU decoder. It remains opt-in through `TexturePacker(TextureTgaDecoder.Load)`; it does not install itself as the default loader or call the original DLL. SDK and compression acceptance are separate.

## Current behavior

Supports true-color types 2 and 10 (raw/RLE), 24-bit BGR and 32-bit BGRA storage, four horizontal/vertical direction combinations, image IDs and row-contained packets. Returned images have a top-left origin and little-endian packed RGBA words. RLE packets crossing a scanline are explicitly rejected with the original `BinaryAssetBuilderException`, `BadTextureFile`, and message `D3DX cannot load the texture from disk.`

Original probe `40194142937-1` established that all-zero 32-bit alpha becomes 255 with `HasAlpha=false`; other 32-bit alpha, including entirely opaque alpha 255, remains unchanged with `HasAlpha=true`. 24-bit pixels synthesize alpha 255 and report false. Descriptor attribute counts 0 and 8 follow this tested rule.

Still rejects mapped/grayscale/15-/16-bit TGA, interleaving, nonzero palette/origin declarations, unsupported attribute counts, developer directories, external rendering tables, unsupported rendering metadata, unexplained bytes, zero dimensions, and decoded images at or above 64 MiB. Invalid headers/IDs/pixels/packets are rejected explicitly; original behavior outside the tested subset is not claimed.

## Bounded metadata support

After the decoded pixel stream, accepts only these fully bounded structures:

- Exact 26-byte empty footer: extension/developer offsets zero, full 18-byte `TRUEVISION-XFILE.\0` signature.
- One or more 495-byte standard extension + footer groups. Extension size must be 495; rendering fields at offsets 470–494 must all be zero. Developer pointer must be zero; extension pointer must be zero or point to that group's extension start. Full footer NUL is required. Author/comment/timestamp/job/software fields are ignored as non-rendering metadata.
- Legacy 507-byte DevIL extension dialect despite its declared size 495: the extra 12 bytes after the timestamp are zero; software-ID field at 438 is the exact padded `Developer's Image Library (DevIL)` format identifier; rendering/key-color/table fields 482–505 are zero; attribute type at 506 is 3. Its footer follows the same pointer rules. A terminal missing footer NUL is additionally permitted only for this bounded dialect, as observed in the real sample.

This is structural parsing, never a file-name/hash exception. Arbitrary extension pointers, tables, gamma corrections and trailing payloads are not silently ignored.

The 507-byte layout is confirmed by [DevIL's writer at pinned commit 6f3d5e9bc5d173f56060fc8e14bed35c8edcfdcc](https://github.com/DentonW/DevIL/blob/6f3d5e9bc5d173f56060fc8e14bed35c8edcfdcc/DevIL/src-IL/src/il_targa.cpp#L774): it writes six extra UShorts after the normal timestamp, shifting the software and final fields by 12 bytes while declaring 495. That current writer includes the footer NUL. The missing NUL is a separate observed property of this repository's older sample, not a claim about every DevIL writer. Only format-layout evidence was consulted; this decoder/parser implementation does not copy that project's implementation.

## Original evidence and real images

Original DLL SHA256 `4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e`; extraction `40191971356-1`. Original module method `06000017` calls the statically linked D3DX loader, inspects `XGGetTextureDesc`, derives alpha from the loaded format, converts locked level-0 data through `XGCopySurface` (destination format `0x1A200006`, pitch `width*4`), then unlocks/releases. The 24-byte x86 nvImage ctor/dtor are methods `0600002C` / `0600002D`.

The main-branch real inputs remain pinned by `recovery/tests/fixtures/texture-main/manifest.json`:

- `map_mp_2_feasel10_ep1_art.tga`: 512×512, raw24, exact empty footer.
- `apt_tacticalhudbuildqueuepage_1.tga`: 1024×512, raw32, ID length 1; two consecutive standard 495-byte extension/footer groups, the first unreferenced and the second pointing to itself, all rendering fields zero.
- `apt_tacticalhudmaincommandbar_1.tga`: 512×256, raw32; 507-byte legacy DevIL extension at 524306, then a 25-byte footer missing its terminal NUL. At actual shifted offsets, rendering fields are zero and attribute type is 3. Reading this dialect as a normal 495-byte record incorrectly interprets software-ID/version bytes as gamma metadata.

Before extending support, an independent local raw-BGRA/header-direction decode was compared against the **entire saved original pixel arrays** in probe `40194142937-1`; all three real images matched exactly, and both 32-bit images reported alpha. This was evidence for implementing the bounded structures, not a substitute for the subsequent Windows differential.

## Oracle, exact scope, and receipts

`recovery/tests/TextureTgaOracle.cs` guards the original SHA, checks native allocator thunk signatures, and uses the original module's VC8 malloc/free RVAs `4E74C` / `4E758`, real Initialize, nvImage ctor/dtor, and LoadImageA. It compares complete pixel arrays/dimensions/alpha and full exception type/message/ErrorCode for rejected inputs.

The full current matrix contains **1,675 fixtures**: all previous 1,608 generated cases, 64 added standard/chained/legacy-DevIL metadata cases (24/32 × raw/RLE × four directions × four metadata dialects), and the same three real files. No old fixtures were removed. Expected classification is **1,421 complete pixel/alpha matches**, **252 original rejection matches**, and **2 explicitly unsupported original-success cases**. Without the real directory, totals are 1,672 / 1,418 / 252 / 2. Strict hard assertions enforce the exact accounting.

The two remaining unsupported cases are `empty-footer-24-rleTrue-direction16` and `empty-footer-24-rleTrue-direction32`: their illegal cross-row runs make the legacy D3DX row parser consume the `TRUEVISION` footer as pixels (`FF525545`/`FF564953`). They retain complete original output in the transcript and must be rejected by the recovered decoder. They do not count as equivalent decodes. The other 252 cross-row cases are exact negative comparisons, not ignored failures.

Historical immutable receipts retain their actual scopes:

- Original probe `40194142937-1`, source `765cf26f418e4fe503603c9c7515ea16c8fc9e67`: 1,611 inputs, non-acceptance probe.
- Strict partial acceptance `40194716161`, source `806db41...`: before metadata extension support, 1,355 pixel/alpha matches + 252 negative matches + 4 declared unsupported (including the two real metadata images). It is not relabeled as acceptance of the later metadata changes.
- Current metadata extension revision: Windows differential pending until its own receipt is collected.

Build x86 CLR4 `/unsafe /r:Recovered.BinaryAssetBuilder.TextureCompiler.dll`, use legacy-v2 activation, and keep recovered/support DLLs beside the host. Run `TextureTgaOracle.exe original.dll transcript.txt fixtures [--probe|--strict] [real-directory]`. Probe mode collects differences without acceptance. Strict mode asserts both equivalent behavior and declared unsupported boundaries, with separate counters; it does not call all inputs successful image decodes.


## Final immutable acceptance

Source commit `0cd42405f5b757c72383040d2f1ff820c31d29bf`, run [40195290786](https://github.com/wu162/NewSdk/actions/runs/40195290786) attempt1 passed. The exact receipt is [evidence/texture/tga/40195290786-1/validation.json](evidence/texture/tga/40195290786-1/validation.json), with source/input hashes and full transcript archive. Debug and Release each checked all 1,675 fixtures: 1,421 complete-pixel/alpha decoding matches, 252 exact original rejection matches, 2 explicit unsupported original-success anomalies, zero mismatches. All three pinned real main-branch TGA files are included in the decoding matches. The two unsupported inputs are not counted as equivalent decoding passes. No compression or full SDK acceptance is implied.


