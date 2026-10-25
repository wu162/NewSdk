# TextureCompiler 当前交接

2026-10-25。分支 `recovery/texturecompiler-source`；源码全部在根目录 [src/TextureCompiler](../src/TextureCompiler)。原DLL SHA256 `4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e`。原始IL、metadata/native清单仍在 `evidence/texture/runs/40191971356-1`，不把反编译导出冒充完整可构建还原。

**DLL尚未完整还原；TGA到三种raw DDS已支持二维Win32完整/PS3限三级mip，以及PC/PS3体积raw与递归mip，均取得专用MOD真实SDK验收；PS3 POT TGA >256自动cubic缩放也已取得组件/入口/真实SDK验收。** 生产SDK DLL/config未替换，源码仍在独立分支。

## 第五轮当前入口

PNG/BMP 解码与实际入口组件已完成；最终含 RGB16 的四 profile SDK 批次待收取。精确格式边界、gamma/RGB16/BMP 原版语义、组件锚点及 SDK 状态见 [第五轮交接](TEXTURE_PNG_BMP.md)。新的测试触发规则见 [TEXTURE_TESTING.md](TEXTURE_TESTING.md)，后续不再每次提交启动多条 Texture 工作流。

## 已验收范围

| 能力 | 精确证据 |
| --- | --- |
| 元数据、image rules/options、内存图集packer、native buffer、DDS直通 | 前轮40194142827；不变metadata/image/packer分组精确复用40193344145，原收据与源码锚点保留 |
| TGA24/32、RLE、方向/alpha、受限标准/DevIL扩展 | 源码0cd42405，40195290786；Debug/Release各1675：1421像素、252拒绝、2明确未支持 |
| 真彩16、gray8、8bit索引/15-32bit palette及原拒绝边界 | 源码52e8d6d4，40198138775；各4534：2606像素、1928拒绝、零差异。旧1675未重跑 |
| Volume条带切片、逐级尺寸、三级vector容量/保留RGBA | 源码3b7eac91，40197368504；各290组/389991检查/381195像素；后续Volume核对输入后复用 |
| A8R8G8B8、X8R8G8B8、R5G6B5无mip DDS | 源码3b7eac91，40197368508；各135组/48157检查 |
| Win32 raw triangle mip全链 | 源码f38ec9ac，40199020130；279组含随机/极端轴/impulse，11拒绝，完整DDS逐字节差分 |
| 实际ProcessInstance图像入口 | 45b36054→aa2b2d80，40198277912/40199691250；三buffer及完整cdata，PC/PS3/unknown无mip；NPOT重采样明确排除 |
| 实际ProcessInstance mip入口 | 源码aa2b2d80，40199691351；原Initialize后的Win32环境与恢复版逐字节对照 |

精确sourceCommit、Debug/Release摘要和输入指纹，以 `evidence/texture/stage2/{Formats,Volume,Raw,Mips}`、`images`、`mip-images` 下对应runId收据为准；不能将文档合并提交写为新跑成功。原已成功且未变的分组使用祖先/输入核对后复用，不重新调用原DLL测试该组。

## 真实SDK验收

专用MOD使用main固定提交 `bacfc69fc3e71c8116d1f22b933b8b6173170ecf` 的3张真实TGA，每图×3种raw格式×Texture/OnDemandTexture，共18资产。另9个main真实DDS继续用于直通差分。原路径、blob/SHA256在 [fixture manifest](tests/fixtures/texture-main/manifest.json)，输入未重编码。

- 无mip SDK：源码c733a1b5，运行 [40199309214](https://github.com/wu162/NewSdk/actions/runs/40199309214)成功，收据 `evidence/texture/sdk/40199309214-1/validation.json`。
- 完整mip SDK：源码aa2b2d80，运行 [40199691305](https://github.com/wu162/NewSdk/actions/runs/40199691305) 的编译与比较步骤成功；最后证据推送并发ref冲突导致workflow failure。原始成功收据在 [sdk-mips/40199691305-1](evidence/texture/sdk-mips/40199691305-1/PERSISTENCE.md)，完整轨迹保持原Actions artifact，没有冒充整个workflow绿色。

两个profile均为原版/恢复版/恢复版移除原TextureCompiler.dll三组真实BAB编译，各组18个Core实际调用，四份stream文件、逐资产三buffer及完整cdata相同。使用EALAModStudio case5对应BAB参数，SDK原配置只在临时副本替换Texture路由；trace wrapper没有Main/手工驱动插件入口。移除原目标DLL的SDK组证明实际恢复路径无原Texture DLL回退。此验收只覆盖专用18资产MOD，不冒充整个Upheaval或其他编译器联合验收。

## 当前入口与边界

`TextureImageLoader`分派已验证的TGA、PNG和BMP解码组；`Plugin.Images.cs`接到真正ProcessInstance；`TextureRawDdsEncoder`保留无mip API及显式true拒绝，二维mip由独立`TextureTriangleMipEncoder`/`TexturePs3MipEncoder`提供，体积编码由`TextureVolumeRawDdsEncoder`/`TextureVolumeMipEncoder`提供，插件按平台、Type和GenerateMipMaps选择。DDS输入仍逐字节直通；OnDemandTexture另写cdata，Standard/Volume/Cube枚举0/1/2，保留旧大小/endian/已有文件尾部行为。

NPOT真实图片会被旧D3DX自动重采样，不能把原始TGA像素尺寸直接套IsValidTexture并产出空pink数据。恢复插件对NPOT明确拒绝，直到重采样被精确恢复；未假装完成该路径。PS3 mip固定三份payload已实现并验收；POT TGA >256的自动cubic resize已接入并验收，极端条带右移导致零轴时明确拒绝。两个跨行RLE/footer原版误读像素边界保留明确未支持。

## 第三轮新增：PS3 mip、体积 DDS 与体积 mip

详见 [TEXTURE_PS3_VOLUME.md](TEXTURE_PS3_VOLUME.md)。二维PS3核心279组、体积raw核心1200组、体积mip核心2832组均Debug/Release完整DDS字节对照；实际volume入口各2610组，PC/PS3/unknown99三buffer及完整cdata一致。源码仍全部在根src/TextureCompiler。

- PS3二维mip、PC/PS3无mipvolume SDK：源码31f45e04b20ce7a307906009cbc7dd16c16e1e1a，运行40202575350成功，3个profile各三组18资产。
- PC/PS3体积mip SDK：源码75f302ef802499fb319c77a5da7e0f454a30ca68，运行40203176308成功，2个profile各三组18资产。
- 五个profile分别比较原版/恢复版/移除原目标DLL恢复版四份stream产物和全部实际调用buffer/cdata；通过BAB显式选择PC/PS3。只证明专用MOD范围，生产SDK/tools和config不替换，没有合入其他分支。
- PS3二维固定写3份payload，小图仍重复末级，头级数为min(XY自然数,3)。体积mip递归先合并Z再滤波XY，头自然mip数不考虑深度；不能照搬二维base-relative算法或现代通用DDS头。
- 最终75f302ef实际入口/受影响回归见专门文档；不变旧核心及成功SDK保留原锚点，不重跑。并发证据普通push已添加rebase与有限重试，禁止强推。

## 下一轮

1. DXT1/DXT1A/DXT3/DXT5压缩核心仍缺，不把raw DDS或options恢复称为DXT恢复。
2. Xbox360/XenonBundler专用恢复按用户要求剔除；共享PC/PS3 XGraphics调用及原native生命周期仍需审计；Volume raw/mip已接入并取得PC/PS3 SDK验收。
3. 完整TerrainTextureAtlas入口/压缩未接；当前仅内存packer验收。
4. PNG/BMP 本轮声明范围见 TEXTURE_PNG_BMP.md；JPEG、剩余 TGA metadata 和 NPOT resize 继续独立取证，未支持路径保持明确拒绝，禁止隐藏原 DLL 回退。
5. PS3自动cubic resize已正式验收，数值根因及精确锚点见[TEXTURE_CUBIC_RESIZE.md](TEXTURE_CUBIC_RESIZE.md)；普通二维/体积mip残差修正状态见[第四轮交接](TEXTURE_ROUND4_HANDOFF.md)。NPOT D3DX resize仍未支持。

详见 [RAW_DDS](TEXTURE_RAW_DDS.md)、[TRIANGLE_MIPS](TEXTURE_TRIANGLE_MIPS.md)、[TGA_FORMATS](TEXTURE_TGA_FORMATS.md)、[VOLUME](TEXTURE_VOLUME.md)、[公开实现核查](TEXTURE_UPSTREAM.md)。Mip数值探针40198443312仅测试进程instrumentation，270原DDS输出不变；不链接恢复业务代码。Windows x86/CLR4 legacy activation/.NET Framework4.8；禁止强推。

