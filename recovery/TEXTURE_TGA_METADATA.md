# TGA header / extension / footer metadata

2026-10-29（北京时间），`recovery/texturecompiler-source`。原版取证及本批恢复/完整消费已验收，专用SDK集中验收准备好。仅PC、单代理、编译测试仅Actions、禁止强推，生产SDK未替换。

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

## 原版证据与已通过的完整组件

原版源码`63dcfd2b527284ff153e07e7badc997a3d2bfb49` / [40793184986](https://github.com/wu162/NewSdk/actions/runs/40793184986) completed/success：12480全部解码，24960原调用逐字节重复一致；480上下文×26 profile的完整输出/尺寸/hasAlpha与none基线全部相同。原gold在`evidence/texture/tga-metadata-load/40793184986-1`，samples.zip SHA256 `e55d7f704fd1d592593843e7444fa231ff05a3824ea3895a08ba50bb4b149993`。此收据仍probe-only。

恢复实现只校验有界的495-byte extension、developer directory/tag data、色彩表、扫描线偏移与postage stamp区域及footer/chained结构，不应用gamma、premultiply或alpha属性，不使用坐标标签。元数据引用必须在像素末端之后且在所属footer前；区域必须完整覆盖尾部，非法/悬空/溢出/损坏结构明确拒绝。旧24/32-bit DevIL限定路径和cross-row RLE拒绝保留，Additional格式共享标准TGA2 parser；不是任意未知尾数据全部忽略。

完整验收组准备好：`tga-metadata`对12480原gold在D/R各执行直接/公共两入口24960次逐次NoOriginal调用，并执行24种损坏结构×两存储格式×两入口=96次明确拒绝；`tga-metadata-images`各47248插件输入及同量NoOriginal，包括43200 raw3/DXT3/5、3840恒alpha63的透明DXT1A、208 atlas，涵盖10源格式/26 metadata、POT/NPOT/partial/二维volume/full mip、新旧cdata、三buffer/full DDS。生产SDK还待后续必要里程碑，不把检查点或探针写成已完成。

最终组件源码`a928f9819a483f94256c3e16a450f7dd1034107a` / [40795326615](https://github.com/wu162/NewSdk/actions/runs/40795326615) completed/success：19完整组通过，22成功组复用。D/R各12480 gold双入口24960调用、1904740检查、5616000 RGBA字节和96个损坏结构明确拒绝；47248完整插件输入/同量NoOriginal各432710096字节、0错误。损坏结构拒绝是明确未支持边界，不冒充原拒绝对照；原gold本批12480全部成功。全部原gold已由[独立审计](evidence/texture/handoffs/20261029-tga-metadata/original-archive-audit.json)逐文件核对。

专用SDK `sdk-PcTgaMetadata` 为8152真实资产：170份原gold输入形成6800 raw/DXT3/5资产，156恒alpha63源形成1248透明DXT1A资产，52份atlas输入形成104 base/normal资产。原/恢复/逐次NoOriginal、四产物与完整实际轨迹；不替代完整MOD。SDK源码278ae5774226853ad5ac5df5de9db67c935755bf /40797426507的11 profile全部passed：三模式每模式21576实际资产/调用、75459完整轨迹文件，四产物/完整轨迹独立逐文件核对通过；其中PcTgaMetadata各8152资产/调用、28481轨迹。41组件和两个纯PNG/JPEG metadata SDK复用祖先锚点。当前13专用SDK每模式共35456资产/调用、124013轨迹，分组锚点见[验收汇总](evidence/texture/handoffs/20261029-tga-metadata/acceptance.json)。整体workflow为failure，原因仅BMP原版探针检测重复不确定性，不能称整批success；SDK独立passed收据有效。生产SDK未修改，完整MOD未跑。
