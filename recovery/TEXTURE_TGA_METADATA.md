# TGA header / extension / footer metadata

2026-10-29（北京时间），`recovery/texturecompiler-source`。当前为原版取证阶段，尚无本批恢复/消费/SDK验收。仅PC、单代理、编译测试仅Actions、禁止强推，生产SDK未替换。

## 本批完整取证组

`TextureTgaMetadataFixtures.cs`按TGA 2.0的18-byte image header、495-byte标准extension、绝对auxiliary偏移、10-byte developer entry和26-byte TRUEVISION footer构造有效源文件。构造器为自有测试代码，没有更换生产codec或复制上游实现。已接受的507-byte DevIL特殊布局和旧RLE异常以TEXTURE_TGA.md为准，不重复旧实验。

- 10格式：truecolor24、truecolor32（descriptor alpha8/0）、truecolor16（alpha1/0）、gray8，以及index8配palette15/16/24/32。
- 尺寸8×4、16×8条带、7×5 NPOT、1×1；raw与逐行有界RLE、方向16/32；三种图案含全零alpha与恒定alpha127。RLE包不跨行。
- 26个metadata profile：无尾/空footer、文本/时间/软件、alpha属性0..4、gamma 1/1、1/2、22/10、0/1、1/0、aspect、key color、两个developer tag、scanline offsets、identity/invert color-correction table、2×2 postage stamp、全部字段、developer/tables在extension前、chained extension/footer，以及非零header origin和origin+all。
- 合计12480个源输入、24960次目标原调用；原输出完整RGBA/尺寸/hasAlpha或完整type/message/ErrorCode逐次重复一致，源文件不得改变。固定原DLL SHA/token/allocator thunk，Initialize PC并先做TGA正对照。

入口`InspectTextureTgaMetadata.ps1`，集中组`tga-metadata-load`为manual_only/evidence_only，标记`[texture-tga-metadata]`显式运行。原探针收据probe-only，绝不冒充恢复或SDK验收。

## 当前源码边界与后续

`TextureTgaDecoder.ValidateMetadataTail`只接受有界的标准零渲染字段/chained footer和特定DevIL布局；非零gamma/alpha/developer/table拒绝。`TextureTgaAdditionalFormats.Decode`在像素结束后遇任何尾部metadata均拒绝；两类decoder也拒绝非零header origin。

先查原版对这些metadata的实际影响，再用完整gold驱动一般结构规则，保留旧RLE跨行拒绝/两项原成功异常的显式未支持边界。共享helper必须登记所有真实消费者；整组完成后集中运行D/R完整差分、逐次NoOriginal与实际插件三buffer/full cdata/atlas/mips/volume，正式宣称集成前完成必要专用SDK。完整MOD继续降频。
