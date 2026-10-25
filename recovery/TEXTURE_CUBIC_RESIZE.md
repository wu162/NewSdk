# PS3 cubic 自动缩放：正式源码与 SDK 已验收

> **历史记录：自 2026-10-26 起仅以 Win32/PC 为目标。本文非 PC 能力、旧待办和旧平台矩阵不再适用；当前实现与计划以 [TEXTURECOMPILER.md](TEXTURECOMPILER.md) 为准。原 sourceCommit/runId 仅证明原验收版本，不代表本轮删除后的代码已验证。**

2026-10-25，第四轮约40分钟，单代理；Xbox360不在恢复目标内。正式恢复源码为根 `src/TextureCompiler/Managed/TextureCubicResizeEncoder.cs`，已经接入 `Plugin.Images.cs`。默认项目直接编译该类；不再存在独立实验 DLL 的验收替代正式构建。

## 能力与边界

PS3 StandardTexture 的 POT TGA 输入、AllowAutomaticResize=true、任一轴>256：两个轴同时右移，直到二者均≤256，按 B-spline cubic 生成 base image，随后从该 base 生成后续 mip，支持 A8R8G8B8 / X8R8G8B8 / R5G6B5、Texture / OnDemandTexture、有/无 mip。PS3 有 mip 保持原来的三份 payload。

NPOT D3DX resize、PNG/JPEG、DXT 和完整 atlas 入口仍未实现。右移会使任一轴为0的极端条带明确拒绝，不以clamp改写语义。生产 SDK 原 DLL/config 未替换。

## 精确验收锚点

源码 `d9e496d5907fce28d38e9bf0268e7d3488d2e626`：

| 层次 | 运行 | 结果 |
| --- | --- | --- |
| 正式 DLL 组件 Debug / Release | [40209208060](https://github.com/wu162/NewSdk/actions/runs/40209208060) | 各1854组、19066353 checks，完整DDS逐字节一致 |
| 实际 ProcessInstance Debug / Release | [40209208093](https://github.com/wu162/NewSdk/actions/runs/40209208093) | 各324组、24597016 bytes、零错误；三缓冲区及完整cdata一致 |
| 真实SDK Ps3ResizeNoMip / Ps3ResizeMips | [40209208221](https://github.com/wu162/NewSdk/actions/runs/40209208221) | 两profile各18资产，Original / Recovered / NoOriginal各18次Core实际调用；四产物和每次buffer/cdata完全一致 |

收据分别在 `evidence/texture/stage2/CubicResize/40209208060-1`、`ps3-resize-images/40209208093-1`、`sdk-ps3-resize/{Ps3ResizeNoMip,Ps3ResizeMips}/40209208221-1`。本轮曾口头误报入口612组，最终以收据324组为准。

该路由提交引起的旧回归也成功：基础40209208209、无mip入口40209208158、PC mip入口40209208069、PS3 mip入口40209208185、体积raw入口40209208124、体积mip入口40209208075。不要将后续文档提交写成新的SDK运行。

## 根因与复现

原native通用重采样器 RVA 0x53930 先乘统一double倒数，再顺序累加归一化权重，把 `1 - sum` 加回最近中心tap。旧候选漏掉最后一步，导致4×8 pattern4末级两个float bit不同，最终DDS offset169出现原版51、候选50。不是递归mip、x87扩展精度或者固定横纵顺序问题。

4→1 triangle的归一化double权重前三/四项通常是 `3FCAAAAAAAAAAAAA, 3FD2AAAAAAAAAAAA, 3FD2AAAAAAAAAAAE, 3FCAAAAAAAAAAAAA`。中心项必须包含残差。验证模型若使用Python 3.12内置sum，会因补偿求和误判；必须与原native一样显式顺序累加原权重和归一化权重。C#代码已保留显式循环，没有按样本硬编码。

测试进程探针保存GPR/flags及FXSAVE状态、恢复完整原指令；每次要求270组instrumented DDS/callback与同进程未修改原DLL基线一致。源码/运行：5c8d2631 / 40208104097，f5e4b5c1 / 40208331901，25c9b4e3 / 40208535806。最后一组记录RVA0x5f030 weighted accumulation的34次调用，确定输入来自同一个cubic base和double权重差异。探针不链接业务恢复DLL。原DLL SHA256为 `4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e`。

历史原输出探针40203552957、float探针40204552902保留；历史失败40203886032/40204116704与递归模型失败40204249089均已被本页正式验收取代。实验成功40208949578不是最终正式Debug/Release验收，以上d9e496d5运行才是。

## 同族归一化修正

后续源码 `fd7405d9b5b2b1dda3cb253f7f940ef6a5295c76` 把相同残差步骤补入 TextureTriangleMipEncoder / TextureVolumeMipEncoder，并给PC/PS3原native差分新增同一个确定的4×8输入，避免旧随机样本漏过半值边界。其验证状态及runId见[本轮交接](TEXTURE_ROUND4_HANDOFF.md)。该变更会影响普通mip，必须单独验收；不能拿本页cubic结果代替。

## 公开实现参考

本轮查阅 [NVIDIA Texture Tools FloatImage](https://github.com/castano/nvidia-texture-tools/blob/master/src/nvimage/FloatImage.cpp)、[ScummVM 2PassScale](https://github.com/scummvm/scummvm/blob/master/engines/qdengine/qdcore/util/2PassScale.h)、FreeImage Toolkit Resize 和 [TwelveMonkeys ResampleOp](https://github.com/haraldk/TwelveMonkeys/blob/master/common/common-image/src/main/java/com/twelvemonkeys/image/ResampleOp.java) 的公开实现，比较两遍滤波、B-spline及边界处理。各版本的坐标、顺序和量化不同，不能直接当旧nvDDS的等价替代。实际修正取自原DLL指令与保持输出不变的探针；没有粘贴这些项目的源码。下一轮可继续积极参考公开实现，以原版逐字节对照为验收标准。

