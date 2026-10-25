# Cubic 自动缩放：实验续接，尚未验收

2026-10-25。正式插件仍拒绝 PS3 StandardTexture >256 且 AllowAutomaticResize=true 的路径；NPOT D3DX resize 也仍拒绝。以下候选不属于已支持能力，不能用前面的PS3/volume成功SDK收据代替本组验收。

## 代码与隔离

候选在根 `src/TextureCompiler/Experiments/TextureCubicResizeEncoder.cs`，位于 Managed 项目目录之外。默认 csproj 不包含此文件，Plugin.Images.cs 没有调用它。`TestTextureCubicResize.ps1` 先构建正式恢复 DLL，再用 csc 单独编译 Experimental.Texture.Cubic.dll，差分只反射加载这个实验 DLL。候选保留最接近原版的二维 base-relative 后续 mip 模型；递归模型已经证伪，不能再误当正确实现。

默认 Managed 目录的15个C#文件及1个csproj与最终SDK验收源码 `75f302ef802499fb319c77a5da7e0f454a30ca68` 全部逐Git blob SHA一致。隔离提交 `1e19ecb45b6100503c1eb0c69c43698c01c84626` 的核对收据见 `evidence/texture/handoffs/20261025-stage3/default-managed-inputs.json`。无需因实验目录/文档变化重复调用已验收类型的原DLL。

## 原版证据

- 原 `Plugin.ProcessTexture` 的PS3 StandardTexture分支：宽或高大于256时两个轴同时右移，直到二者都≤256；写 rescale type@60=5、filter@64=4、float width/height@68/72。枚举来自固定公开旧头，4为B-spline cubic，5为PreScale；不把rescale 5误当mip filter 5。
- 原版输出探针：源码 `147c85a806c566288b4ff94a46933525ed2f83e8`，运行 [40203552957](https://github.com/wu162/NewSdk/actions/runs/40203552957) 成功，279组原版三格式、HalfAxesAtLeast1、无mip完整DDS。收据和transcript.zip在 `evidence/texture/cubic-resize-probe/40203552957-1`。这只是原版证据，不是恢复成功。
- 原版浮点探针：源码 `424f0f830415b1749eabd1db2e6023d602d282a9`，运行 [40204552902](https://github.com/wu162/NewSdk/actions/runs/40204552902) 成功，270组。先在未打补丁的原native函数生成同进程基线，再安装测试进程FloatToUnsigned RVA0x5bc90的探针；全部DDS字节和callback元数据必须与基线一致才记录float bits。保留原SHA256、指令/重定位常量guard、完整x87/SSE及寄存器状态保存、finally恢复并逐字节检查恢复。只用于取证，未链接到恢复业务DLL。原浮点记录在 `evidence/texture/cubic-float-probe/40204552902-1/transcript.zip`。

## 当前阻塞与已证伪假设

直接B-spline cubic的输出base像素与探针匹配，但完整后续mip数值仍未闭合。最近完整组件矩阵包含2x/4x/8x缩放、mip=false/true、随机/脉冲、最高1024轴及11个拒绝样本；整个组**没有取得Debug/Release通过**。

1. 原 base-relative 候选在运行40203886032/40204116704失败：PS3 mip=true、shift=1、RGBA 4×8、pattern4，DDS offset169原版51、候选50。
2. 改递归在源码3883ac9e、运行40204249089失败：8×4、pattern2、offset168原版132、候选131。该模型数学权重也不同，已退回更接近的base-relative候选；不要仅为修复第一个样本而保留递归法。
3. float探针对45组A8R8G8B8 mip=true的本地模型核对：base floats逐bit匹配；base-relative的后续float最接近，只有4×8 pattern4末级两个float bit不同。该局部数学核对不能冒充全部Windows矩阵验收。逐项除法代替统一倒数、固定水平/垂直顺序、末级固定单轴优先均没有解决全部float记录；不做按样本硬编码修正。

下一轮从浮点记录直接核对这个4×8样本末级的归一化权重、累加精度与中间值生命周期，再跑最终整组矩阵。新增实验候选应把Experiments源文件也纳入显式输入指纹。组件闭合后才接Plugin，并运行准备好的实际入口/PS3真实SDK新profile；本轮没有接入，没有启动缩放SDK验收。两个轴右移可能得到0的极端条带不在当前安全原版探针域，后续须先明确拒绝/原错误边界，不能通过clamp悄悄更改原语义。

已验收二维PS3 mip、体积raw/mip及五个真实SDK profile不变，详见 [TEXTURE_PS3_VOLUME.md](TEXTURE_PS3_VOLUME.md)。本页实验失败必须与这些成功分组分别报告。
