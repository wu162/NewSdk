# JPEG 色彩空间与 APP metadata 取证入口

2026-10-29（北京时间），`recovery/texturecompiler-source`，仅PC。原版探针已完成：源码`7ff6dc66e8d847718fcaf4926b7eecdce46c05ac` / [40783041481](https://github.com/wu162/NewSdk/actions/runs/40783041481) completed/success，4000输入/8000原调用，1440解码、2560 CMYK/YCCK原拒绝，当前公开IJG入口4000完整匹配、0差异。原探针收据probe-only、productionAccepted=false保持原义；后续完整组件/消费/SDK已在dba6d305 / [40785653416](https://github.com/wu162/NewSdk/actions/runs/40785653416) completed/success，见下节。

- `recovery/tests/TextureJpegColourFixtures.c`用已固定IJG6b公开API编码RGB、CMYK、YCCK，8种POT/NPOT尺寸、4质量、顺序/渐进、4像素图案；原样/移除Adobe APP14/transform 0、1、2共3840输入。源码只构造测试输入，没有替换生产decoder，也没有修改上游IJG文件。
- 从既有不可变JPEG gold `3f2351f2d0b0adccf416ba6dda7e97d49b5c92f6` /40625499544选16张原版成功的YCbCr 4:2:0输入（8尺寸×顺序/渐进、pattern3/quality75），分别附加Exif orientation 1/3/6/8、4种固定来源ICC、XMP、COM，共160输入。
- `TextureJpegMetadataProbe.cs`固定原DLL SHA/token/allocator、Initialize PC和TGA正对照；每输入原调用两次，完整尺寸/hasAlpha/RGBA/错误须一致，输入不变。另保存当前公共IJG生产入口的完整候选RGBA/错误；完整相同/不同均如实计数。
- 集中入口`InspectTextureJpegMetadata.ps1`，组`jpeg-metadata-load`为manual_only/evidence_only。标记`[texture-jpeg-metadata]`或手动force_groups显式采集，总4000输入、8000目标原调用；收据probe-only，`productionAccepted=false`。普通组件/SDK不自动选它。

本轮先完成PNG metadata，再把JPEG原版取证合入同一SDK Windows批次顺序采集，随后统一跑完整新能力组。4通道CMYK/YCCK全部按原版BadTextureFile拒绝，不作为缺失支持；RGB的APP14解释由原已验收的IJG6b一般规则处理，没有样本ID分派或gold表。

原探针全匹配之后，已补齐独立Debug/Release NoOriginal重放、相关插件/atlas消费和专用SDK。旧PNG/JPEG/DXT成功组保留原锚点，不默认跑完整MOD，生产SDK DLL/config未替换。

## 最终完整能力组（已验收）

原输入/双方完整RGBA/原错误在`evidence/texture/jpeg-metadata-load/40783041481-1`；samples.zip SHA256 `ef7313c2061dcc28b580e8447c9dc8e01a8dfe7b08025fd19bab8ffe5ee5acca`。两次原调用逐字节重复结果一致。CMYK/YCCK保持同一BadTextureFile，Exif orientation与ICC/XMP/COM仅按本批观察不影响解码，不做查看器式旋转/色彩管理。

- `jpeg-metadata`：D/R分别固定4000原gold输入，直接/public两入口共8000调用，完整RGBA/hasAlpha/尺寸/原错误，源数组与文件不变，每次调用前后原DLL不存在且未加载。自有IJG native也分别Debug/Release构建，保留原sourceCommit/runId/归档SHA与祖先检查。
- `jpeg-metadata-images`：D/R各32080完整插件输入及同量NoOriginal重放。全部160 APP metadata输入+240个RGB/CMYK/YCCK/APP14代表输入（8尺寸、顺序/渐进、quality75/pattern3、5 APP14变体）×两根/raw3+DXT3/5/标准+volume/有无mip/新旧cdata=32000；灰度渐进/真彩顺序两份256×256图×10 metadata×base/normal×新旧尾=80 atlas输入。完整三buffer/cdata和原拒绝；拒绝不冒充输出成功。
- `sdk-PcJpegMetadata`：只取240原版成功的不可变输入（160 metadata+80 RGB/APP14）形成9600真实纹理资产，20张灰度/真彩metadata atlas形成40资产，共9640。三模式完整四产物、全部实际轨迹文件集/每buffer/cdata及逐次NoOriginal，原拒绝只留组件矩阵。

没有修改生产JPEG/IJG/native或其他业务源码，也没有引入候选替换；是在已经验收的同一公开IJG6b入口上扩充原版取证和组件/消费/SDK覆盖。新组及其实际consumer/helper/gold依赖已登记，旧37组件和11 SDK在输入未变时沿用各自成功锚点。完整MOD不默认运行。

## 最终收据、独立核对与下一轮

最终源码`dba6d3050bf33db791144e00f54129f769226714` / [40785653416](https://github.com/wu162/NewSdk/actions/runs/40785653416) completed/success，只执行上述2新组件+1新SDK，旧37组件/11 SDK输入未变直接复用。D/R各4000 gold双入口8000调用、2880成功解码/5120原拒绝调用、649163检查、2000160字节RGBA；插件各32080输入（19280输出/12800原拒绝）及同量逐次NoOriginal、每模式224710304字节比较。所有原拒绝匹配完整type/message/ErrorCode，未算作输出成功。

PcJpegMetadata Original/Recovered/NoOriginal各9640实际资产/实际Core调用，四产物及每模式33721完整轨迹文件逐文件全等；每次NoOriginal检查原DLL不存在且未加载。详见[SDK归档审计](evidence/texture/handoffs/20261029-jpeg-metadata/sdk-archive-audit.json)。业务/native源码与PNG组件1cff9256完全相同，本批是已有公开源码的覆盖扩展，没有引入替换候选或重写上游文件。

[原始归档独立核对](evidence/texture/handoffs/20261029-jpeg-metadata/original-archive-audit.json)验证全部原输入、原/候选RGBA与错误、160个APP metadata完整输出与旧原gold逐字节相同。256个RGB上下文全部观察到Adobe transform 1/2改变像素解释；native/no-Adobe/transform0相同、transform1与2相同，不能笼统称所有JPEG APP都忽略。Exif orientation 1/3/6/8、4种ICC、XMP和COM在本批不改变原像素。

本批已闭环。下一轮按RESUME优先TGA有效非零extension/gamma/alpha/developer/table与其他实际BMP组合；CMYK/YCCK原拒绝边界不再反复当作缺失解码能力。完整MOD继续降频，当前12 SDK覆盖每模式27304资产与95532轨迹文件，分别保留7ff6dc66/40783041481的旧11组和dba6d305/40785653416的新1组锚点，不把复用改写为新跑。
