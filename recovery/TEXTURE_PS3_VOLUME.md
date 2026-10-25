# PS3 mip 与体积 DDS 源码恢复

> **历史记录：自 2026-10-26 起仅以 Win32/PC 为目标。本文非 PC 能力、旧待办和旧平台矩阵不再适用；当前实现与计划以 [TEXTURECOMPILER.md](TEXTURECOMPILER.md) 为准。原 sourceCommit/runId 仅证明原验收版本，不代表本轮删除后的代码已验证。**

2026-10-25，分支 `recovery/texturecompiler-source`。代码在根 `src/TextureCompiler/Managed`；只恢复三种 raw 格式 A8R8G8B8、X8R8G8B8、R5G6B5，没有链接旧 nvDDS 库或回调原 TextureCompiler 业务实现。

## 已取得的精确证据

| 分组 | sourceCommit / runId | Debug 与 Release 每配置结果 |
| --- | --- | --- |
| PS3 二维 mip 核心 | dc42ea2ac004e639eaae97d46178545de884a914 / 40201952828 | 279 组，4,949,263 检查，11 个拒绝断言 |
| raw 体积编码 | 0f012b991ae28a2f86559c6d628ad36bb2d84911 / 40202402535 | 1,200 组，505,981 检查，13 个安全拒绝样本 |
| PS3 mip 实际入口 | 同上 / 40202402557 | 246 组，三 buffer 与完整 cdata 一致，含 6 个明确拒绝的 NPOT 输入 |
| 无 mip 体积实际入口 | 同上 / 40202402651 | 2,610 组，PC/PS3/unknown 99，三 buffer 与完整 cdata 一致，6 个原版错误一致 |
| 递归体积 mip 核心 | d76bd513d20546278b96960fb856bc1e1a4fb053 / 40202993770 | 2,832 组，3,211,661 检查，12 个安全拒绝样本，PC/PS3 两种 profile |

各核心差分比较完整 DDS 字节及输入存储未被修改。体积 mip 包含三种格式、非正方形维度、深度大于宽高、1 轴、随机数据和四种脉冲位置；不是像素近似比较。原 DLL 的固定 SHA256、Debug/Release 摘要及源码指纹在 `evidence/texture/stage2/{Ps3Mips,VolumeDds,VolumeMips}` 的对应不可变收据。

前三条新路径的真实 SDK：验收源码 `31f45e04b20ce7a307906009cbc7dd16c16e1e1a`，运行 [40202575350](https://github.com/wu162/NewSdk/actions/runs/40202575350) 成功，profile 为 Ps3Mips、VolumePc、VolumePs3。每 profile 原版/恢复版/移除原目标 DLL 的恢复版各18资产、18次真实 Core 调用，四份 stream 产物及每个实际调用的三 buffer/cdata 逐字节相同。收据在 `evidence/texture/sdk-new-paths/<profile>/40202575350-1/validation.json`。

PS3Mips 使用 main 固定提交的3张真实 TGA，关闭自动 resize；两个 volume profile 使用确定生成的16×16×{1,4,16}条带、三种格式、Texture/OnDemandTexture，合法 XML 由真实 BAB/XSD 处理。运行显式 `/tp:Win32` 或 `/tp:PlayStation3`，没有通过 wrapper 篡改实际平台。SDK/tools 与生产 config 未替换，临时 SDK 副本路由恢复插件。

体积 mip 最终入口及真实 SDK：验收源码 `75f302ef802499fb319c77a5da7e0f454a30ca68`，实际入口 [40203176310](https://github.com/wu162/NewSdk/actions/runs/40203176310) 成功，Debug/Release各2,610组；SDK [40203176308](https://github.com/wu162/NewSdk/actions/runs/40203176308) 成功，VolumeMipPc/VolumeMipPs3两profile各原版/恢复版/无原目标DLL三组18资产。四份stream文件、每次实际Core调用的三buffer及完整cdata一致，见 `evidence/texture/sdk-volume-mips/<profile>/40203176308-1/validation.json`。输入仍为16×16×{1,4,16}条带与三种raw格式，但XML显式GenerateMipMaps=true；没有用无mip结果替代mip验收。

最终路由源码75f302ef的受影响回归：二维无mip40203176244、Win32 mip40203176247、PS3 mip40203176263、无mipvolume40203176302、基础组件40203176257均成功。只重跑受路由编辑影响的组件矩阵；不变的核心差分和上一批三profile SDK保留原sourceCommit/runId。

## 二维 PS3 的三份 payload

`TexturePs3MipEncoder` 复用已验收的 Win32 base-relative triangle 数值算法，原 options[100]=30、options[104]=3。输出固定三份层数据；即使1×1已到末级，也重复写出末级像素。DDS 头 mipCount 却为 `min(自然XY级数, 3)`，故1×1头为1而 payload 有3份，2×2头为2而仍有3份。原始探针 [40201763838](https://github.com/wu162/NewSdk/actions/runs/40201763838) 是原版证据，不是恢复验收；前两次失败保留在 Actions，不伪装成功。

DDS 头与像素仍为 little endian；PS3 的外层资产长度/重定位由原已恢复 EmbedDds 路径处理。PS3 POT TGA 大于256且允许自动 resize 的 StandardTexture 已在第四轮正式恢复；组件、实际入口与两个真实SDK profile通过，见 [cubic验收](TEXTURE_CUBIC_RESIZE.md)。NPOT 原D3DX resize和右移导致零轴的极端条带仍明确拒绝。

## 体积条带与 raw 头

`TextureVolumeRawDdsEncoder.EncodeStrip` 使用已经验收的 `TextureVolume.CreateVolumeRGBAImage`，按横向正方形条带切 z，再按 z/y/x 序输出。`Encode` 接受紧密 RGBA 体积数据，不生成 mip，原先的 true 拒绝 API 保留。所有实际尺寸须正二幂，RGBA 总像素数小于16,777,216，短/过长 storage、超范围和未支持格式均显式拒绝。

原无 mip 体积头 flags=0x00881007，caps=0x1000，caps2=0x00200000；depth 写入 offset24。offset20 是单张 base slice 的输出大小，不是整个体积大小。即使 depth=1 也保留 volume 标记。不能用现代 DDS writer 的通用头替代。

## 递归体积 mip

`TextureVolumeMipEncoder` 独立保留原算法，与二维每级从 base 重采样不同：

1. RGBA 字节先按 double/255 转 float32，后续级间保持 float，不提前量化成字节。
2. 每级先平均相邻两张 z slice；深度1直接保留。
3. 对平均后的每张 slice 执行 XY triangle。较小中间图的轴先，tie 为 vertical；边缘裁剪并归一化，每 tap 乘加后存 float32，继承二维已核实的权重倒数语义。
4. 该 float 体积成为下一级的输入。写 DDS 时才按各 raw 格式量化；base 级继续用原 raw 编码。

Win32/非PS3生成 `bitLength(max(width,height,depth))` 份 payload；PS3固定3份，三维都到1后继续写相同级。头 flags=0x008a1007，caps=0x00401008；mipCount **只按XY自然级数计算**，PS3再取不大于3，即使 depth 更长也不修正原头。原版探针 [40202575400](https://github.com/wu162/NewSdk/actions/runs/40202575400) 含120组 PC/PS3、三格式完整 DDS/callback 顺序，不调用恢复业务实现；它本身不算组件验收。

## 回归与剩余范围

路由增加后，0f012b99 的 Win32 mip 40202402521、无 mip image 40202402540、基础组件40202402542均成功；后者按原指纹复用未变 metadata/image rules/packer 的40193344145证据。未把复用写成重新调用原 DLL。

证据推送新增 `SaveTextureEvidence.ps1`，并发普通 push 被拒绝时先 rebase 再有限重试；禁止强推。40202111197的比较步骤已成功，但最后证据 push 冲突导致整个workflow failure；最终0f012b99完整重跑与持久化成功，不将旧失败改写为绿色。当前不变成功组不另开相同 SDK 复验。

目标仅为PC/PS3，Xbox 360/Xenon专属转换已按用户要求剔除。仍缺 DXT1/DXT1A/DXT3/DXT5、PC/PS3所需公共图像处理/完整 native 生命周期与完整 TerrainTextureAtlas，PNG/JPEG、剩余 TGA metadata、NPOT resize仍未验收；未支持输入不隐藏回退原目标 DLL。

## 第四轮补充（2026-10-25）

本页31f45e04/75f302ef验收为历史锚点。普通二维/体积mip归一化残差修正后的源码是fd7405d9b5b2b1dda3cb253f7f940ef6a5295c76；PC/PS3核心与实际入口全部通过，真实SDK40209761463的PcMips/Ps3Mips/VolumeMipPc/VolumeMipPs3四profile全部成功。每profile18资产的Original/Recovered/NoOriginal四产物与真实调用buffer/cdata完全相同。精确新收据和复用指纹见[第四轮交接](TEXTURE_ROUND4_HANDOFF.md)。未变volume raw保留本页原验收，不冒充本轮新跑。

