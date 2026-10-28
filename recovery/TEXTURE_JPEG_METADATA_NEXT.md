# JPEG 色彩空间与 APP metadata 取证入口

2026-10-29（北京时间），`recovery/texturecompiler-source`，仅PC。原版探针已完成：源码`7ff6dc66e8d847718fcaf4926b7eecdce46c05ac` / [40783041481](https://github.com/wu162/NewSdk/actions/runs/40783041481) completed/success，4000输入/8000原调用，1440解码、2560 CMYK/YCCK原拒绝，当前公开IJG入口4000完整匹配、0差异。收据probe-only、productionAccepted=false，不能冒充恢复/插件/SDK验收；下一完整批次准备好，见下节。

- `recovery/tests/TextureJpegColourFixtures.c`用已固定IJG6b公开API编码RGB、CMYK、YCCK，8种POT/NPOT尺寸、4质量、顺序/渐进、4像素图案；原样/移除Adobe APP14/transform 0、1、2共3840输入。源码只构造测试输入，没有替换生产decoder，也没有修改上游IJG文件。
- 从既有不可变JPEG gold `3f2351f2d0b0adccf416ba6dda7e97d49b5c92f6` /40625499544选16张原版成功的YCbCr 4:2:0输入（8尺寸×顺序/渐进、pattern3/quality75），分别附加Exif orientation 1/3/6/8、4种固定来源ICC、XMP、COM，共160输入。
- `TextureJpegMetadataProbe.cs`固定原DLL SHA/token/allocator、Initialize PC和TGA正对照；每输入原调用两次，完整尺寸/hasAlpha/RGBA/错误须一致，输入不变。另保存当前公共IJG生产入口的完整候选RGBA/错误；完整相同/不同均如实计数。
- 集中入口`InspectTextureJpegMetadata.ps1`，组`jpeg-metadata-load`为manual_only/evidence_only。标记`[texture-jpeg-metadata]`或手动force_groups显式采集，总4000输入、8000目标原调用；收据probe-only，`productionAccepted=false`。普通组件/SDK不自动选它。

先完成PNG metadata组件/必要专用SDK，再安排这组原版取证；不为准备工具并行启动额外Windows。取证后按sourceCommit/runId、完整产物与错误定位。若4通道路径原版也拒绝，保留相同BadTextureFile并记录明确边界；若原版可成功，则继续改公开IJG适配的一般颜色转换/标记规则，不以样本ID或gold输出表替代算法。

即使新语料对当前公共loader全部相同，也仍需独立Debug/Release NoOriginal重放、相关插件/atlas消费和适用SDK门槛，才能扩大声明范围。已有PNG/JPEG/DXT成功且输入未变的组保留原锚点；默认不跑完整MOD，生产SDK DLL/config不替换。

## 最终完整能力组（待验收）

原输入/双方完整RGBA/原错误在`evidence/texture/jpeg-metadata-load/40783041481-1`；samples.zip SHA256 `ef7313c2061dcc28b580e8447c9dc8e01a8dfe7b08025fd19bab8ffe5ee5acca`。两次原调用逐字节重复结果一致。CMYK/YCCK保持同一BadTextureFile，Exif orientation与ICC/XMP/COM仅按本批观察不影响解码，不做查看器式旋转/色彩管理。

- `jpeg-metadata`：D/R分别固定4000原gold输入，直接/public两入口共8000调用，完整RGBA/hasAlpha/尺寸/原错误，源数组与文件不变，每次调用前后原DLL不存在且未加载。自有IJG native也分别Debug/Release构建，保留原sourceCommit/runId/归档SHA与祖先检查。
- `jpeg-metadata-images`：D/R各32080完整插件输入及同量NoOriginal重放。全部160 APP metadata输入+240个RGB/CMYK/YCCK/APP14代表输入（8尺寸、顺序/渐进、quality75/pattern3、5 APP14变体）×两根/raw3+DXT3/5/标准+volume/有无mip/新旧cdata=32000；灰度渐进/真彩顺序两份256×256图×10 metadata×base/normal×新旧尾=80 atlas输入。完整三buffer/cdata和原拒绝；拒绝不冒充输出成功。
- `sdk-PcJpegMetadata`：只取240原版成功的不可变输入（160 metadata+80 RGB/APP14）形成9600真实纹理资产，20张灰度/真彩metadata atlas形成40资产，共9640。三模式完整四产物、全部实际轨迹文件集/每buffer/cdata及逐次NoOriginal，原拒绝只留组件矩阵。

没有修改生产JPEG/IJG/native或其他业务源码，也没有引入候选替换；是在已经验收的同一公开IJG6b入口上扩充原版取证和组件/消费/SDK覆盖。新组及其实际consumer/helper/gold依赖已登记，旧37组件和11 SDK在输入未变时沿用各自成功锚点。完整MOD不默认运行。
