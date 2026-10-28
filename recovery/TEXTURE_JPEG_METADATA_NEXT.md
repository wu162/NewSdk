# JPEG 色彩空间与 APP metadata 取证入口

2026-10-29（北京时间），`recovery/texturecompiler-source`，仅PC。当前为**取证工具准备稿，尚无运行/恢复/插件/SDK验收结论**；不得以工具存在或比较任务success宣称新JPEG域支持。

- `recovery/tests/TextureJpegColourFixtures.c`用已固定IJG6b公开API编码RGB、CMYK、YCCK，8种POT/NPOT尺寸、4质量、顺序/渐进、4像素图案；原样/移除Adobe APP14/transform 0、1、2共3840输入。源码只构造测试输入，没有替换生产decoder，也没有修改上游IJG文件。
- 从既有不可变JPEG gold `3f2351f2d0b0adccf416ba6dda7e97d49b5c92f6` /40625499544选16张原版成功的YCbCr 4:2:0输入（8尺寸×顺序/渐进、pattern3/quality75），分别附加Exif orientation 1/3/6/8、4种固定来源ICC、XMP、COM，共160输入。
- `TextureJpegMetadataProbe.cs`固定原DLL SHA/token/allocator、Initialize PC和TGA正对照；每输入原调用两次，完整尺寸/hasAlpha/RGBA/错误须一致，输入不变。另保存当前公共IJG生产入口的完整候选RGBA/错误；完整相同/不同均如实计数。
- 集中入口`InspectTextureJpegMetadata.ps1`，组`jpeg-metadata-load`为manual_only/evidence_only。标记`[texture-jpeg-metadata]`或手动force_groups显式采集，总4000输入、8000目标原调用；收据probe-only，`productionAccepted=false`。普通组件/SDK不自动选它。

先完成PNG metadata组件/必要专用SDK，再安排这组原版取证；不为准备工具并行启动额外Windows。取证后按sourceCommit/runId、完整产物与错误定位。若4通道路径原版也拒绝，保留相同BadTextureFile并记录明确边界；若原版可成功，则继续改公开IJG适配的一般颜色转换/标记规则，不以样本ID或gold输出表替代算法。

即使新语料对当前公共loader全部相同，也仍需独立Debug/Release NoOriginal重放、相关插件/atlas消费和适用SDK门槛，才能扩大声明范围。已有PNG/JPEG/DXT成功且输入未变的组保留原锚点；默认不跑完整MOD，生产SDK DLL/config不替换。
