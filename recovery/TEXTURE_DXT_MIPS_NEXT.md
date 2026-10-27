# PC DXT3/5 压缩 mip（已验收）

PC POT 完整二维压缩 mip 已完成组件及专用MOD SDK验收，见末段精确锚点。当前生产 SDK 未替换；NPOT / 默认 DXT1/1A 仍拒绝；PC压缩volume已由aaacb3d2/40526443463验收。最新组件、无 mip 插件与 SDK 状态以 [当前 DXT 交接](TEXTURE_DXT_COLOUR.md) 为准。继续在 `recovery/texturecompiler-source`，禁止强推，编译测试只用 GitHub Actions。

## 已可复用的实现与证据

- `TextureDxtSampleBlockEncoder.EncodeFloatBlock`：有限归一化 RGBA float，宽高各为 1/2/4，完整16字节PC DXT3/5块；保留原颜色索引复制/空洞，alpha不复制边缘。源码 `9d487859` / 40506678502，Debug/Release各4608原float dispatcher对照、2304实际byte DDS交叉比较及NoOriginal通过。与旧块编码器共用同一ColourLock保护惰性LUT；不得绕开bridge质量71边界。
- `TextureTriangleMipEncoder`：已验收 raw 二维 triangle mip。它从原始归一化图像分别生成每个目标层，不递归使用上一层；根据两个候选中间图的面积决定横/纵pass先后，面积相等先纵向。归一化系数使用一个倒数与最近tap校正。不要混用volume的递归策略。
- 原版整图边界：源码 `3319f50c` / 40507887013，`evidence/texture/dxt-image-boundaries/40507887013-1/{validation.json,transcript.tsv,samples.zip}`。416个DXT3/5小尺寸与对齐、无mip与完整mip样本，保留DDS/RGBA/options/callback。此收据probe-only，不代替恢复验收。
- 小尺寸无mip整图/插件源码 `81e3e76d` / 40508857046 已通过1280整图、1792入口与NoOriginal及712资产三模式SDK；SDK宿主补强 `73cae641` / 40510009285 也通过，NoOriginal每次Core调用前后断言原程序集未加载。不要把取证或文档提交冒充新业务验收。

## 原版 DDS 与逐级布局

仅PC DXT3/5、POT尺寸。目标尺寸每级为 `max(1, width/2)` / `max(1, height/2)`，直到1×1；层数包含初级和尾级，1×1也写mipCount=1。

- 有mip：flags `0x000a1007`、caps `0x00401008`、caps2=0。无mip：flags `0x00081007`、caps `0x1000`、mipCount=0。
- 两格式每块16字节。每级payload为 `ceil(width/4) * ceil(height/4) * 16`；DDS头的linearSize只写第0级payload，不写整个链总长度。
- 128字节DDS头之后按层顺序连接payload，每级块按y再x排列。callback逐级携带 `level,width,height,0,1`。例如2×16完整链payload为64+32+16+16+16，mipCount=5，DDS总长272。
- 默认无mip小块颜色复制规则已经由SampleBlockEncoder封装。不能对全部RGBA统一做clamp-to-edge；alpha越界索引/nibble为0，与颜色不同。

## 完整能力组实现与门槛

1. 将已有二维triangle计算提取为明确的float层接口，供raw与压缩消费，保留旧raw的计算次序与舍入。若选择复制算法，需说明维护理由并验证两处一致；不要为方便先把float层量化回byte。
2. 逐级将float RGB/alpha原样送入 `EncodeFloatBlock`；完整4×4浮点层也不能调用byte接口。组装上述DDS头和层串接，校验长度、合法POT与源存储、正尺寸和原安全像素上限。volume与DXT1/1A继续明确拒绝。
3. **raw最终byte相同不等于中间float位模式相同。** 既有triangle证据可以复用契约，不能据此跳过压缩结果差分；若端点/索引出现差异，定位原生float层/累加舍入，不靠容差、byte回写或近似像素掩盖。
4. 批次覆盖所有1/2小边、长条、方图、多块图与完整尾链，均匀/双色/渐变/半步alpha与随机。Debug/Release完整DDS逐字节、每级payload定位、源输入不变、独立布局/拒绝及NoOriginal完整链重放均需通过。对共享triangle helper有实际影响的raw/mip消费者同步完整回归。
5. 插件 `GenerateMipMaps=true` 路由接入时更新全部实际消费者依赖、自有native构建/复制及SDK矩阵。集中 Original / Recovered / NoOriginal 实际Core调用，四产物、三缓冲区和完整cdata比较；同输入的未变成功组沿用原sourceCommit/runId，未通过或依赖变化组不得跳过。

无签名托管程序集与自有NativeColour/NativeAlpha DLL须一同部署在临时SDK tools目录；原TextureCompiler DLL在NoOriginal模式必须不存在且未加载。正式生产替换、完整mymod或全TextureCompiler完成仍是后续门槛，本批专用MOD不能冒充完整生产验收。


## 2026-10-27 完整能力组验收

源码 `7ae4af3769842084cd19cbff87c651c945f45888` / [40524039917](https://github.com/wu162/NewSdk/actions/runs/40524039917) attempt1 completed/success。依赖选择执行9组、复用19组成功证据，不重测未变颜色/alpha/小块。实现为 TextureTriangleMipEncoder.Normalize/ResizeLevel 内部float层、TextureDxtDdsEncoder完整float链；raw计算次序不变。插件原先已传递GenerateMipMaps，现消费完整能力，不另改路由。

- Debug/Release各3072整图，1,023,327检查；完整DDS头/payload逐字节、每层callback/布局、源输入不变及26个明确拒绝输入通过。24尺寸包含1/2小边、1×32、32×1、2×64、64×2、非方图与多块尾链，32图案覆盖均匀、双色、渐变、半步alpha与随机。相同3072完整链NoOriginal重放，各974,848字节比较通过。
- 两实际入口各配置3584输入（2848成功输出/736原版拒绝匹配），18明确拒绝，16,506,816字节比较；包含TGA/PNG/BMP、显式DXT3/5与原DXT1 alpha推断、三缓冲区和新旧完整cdata；同量NoOriginal通过。
- PcDxtNoMip与新PcDxtMips各三模式、每模式178输入图像/712资产和实际Core调用，四产物及完整轨迹一致。NoOriginal每次Core调用前后断言原DLL不存在且未加载，自有NativeColour/NativeAlpha一起部署并检查PE边界。
- 共享triangle影响的mips、mip-images、raster-images全部通过；PcNoMip/PcMips raw SDK各600资产三模式通过。原版与恢复版使用相同专用MOD输入，生产SDK未替换。

收据：`evidence/texture/{dxt-dds2d,dxt-images,sdk-dxt,sdk-dxt-mips,batch}/40524039917-1/validation.json`。原float小块等未变子组件保留原sourceCommit/runId。压缩volume后续也已验收，见 [TEXTURE_DXT_VOLUME_NEXT.md](TEXTURE_DXT_VOLUME_NEXT.md)，其递归Z后XY策略不能套用本二维base-relative策略。
