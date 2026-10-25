# TextureCompiler 第四轮交接

> **历史记录：自 2026-10-26 起仅以 Win32/PC 为目标。本文非 PC 能力、旧待办和旧平台矩阵不再适用；当前实现与计划以 [TEXTURECOMPILER.md](TEXTURECOMPILER.md) 为准。原 sourceCommit/runId 仅证明原验收版本，不代表本轮删除后的代码已验证。**

2026-10-25 15:29 UTC开始，约40分钟。单代理；未启动子代理。工作分支 `recovery/texturecompiler-source`；禁止force push。按用户要求积极参考公开实现，Xbox360/Xenon专用实现及验收已剔除，PC/PS3共享依赖保留。

## 已完成

1. PS3 POT TGA >256自动cubic resize已恢复、接入默认Managed项目及实际ProcessInstance；原实验文件移到Managed，不再另编Experimental DLL。两轴同时右移至≤256，任一轴归零明确拒绝；支持三种raw格式、Texture/OnDemandTexture、无mip/PS3三份mip payload。
2. 最末级float差异根因为原native通用resampler归一化后把 `1 - sequentialSum` 加回中心tap。对原DLL的状态保持探针确定了double权重；不是递归mip。正式组件与实际入口逐字节完全一致。
3. 同族普通二维/体积mip也补上该残差步骤。新增固定4×8输入的三格式原native回归，防止只依赖旧随机矩阵。补齐PS3工作流对Triangle helper的传递路径依赖；PS3组件证据提交改为有限重试的普通push，未强推。
4. 新增自动缩放真实SDK两个profile及普通mip残差真实SDK四profile。所有测试仅临时SDK/config副本；生产SDK/tools DLL/config未替换。原DLL只作为测试基准，恢复业务代码不回调原实现。

## 验收源码与运行

自动缩放源码 `d9e496d5907fce28d38e9bf0268e7d3488d2e626`：

| 层次 | runId | Debug / Release 或 SDK结果 |
| --- | --- | --- |
| 正式cubic组件 | 40209208060 | 各1854组、19066353 checks，完整DDS一致 |
| 自动缩放实际插件入口 | 40209208093 | 各324组、24597016 bytes，三buffer/full cdata一致 |
| 自动缩放真实SDK | 40209208221 | Ps3ResizeNoMip、Ps3ResizeMips各18资产，三种运行四产物/调用buffer/cdata完全一致 |

普通mip残差修正源码 `fd7405d9b5b2b1dda3cb253f7f940ef6a5295c76`：

| 层次 | runId | Debug / Release 或 SDK结果 |
| --- | --- | --- |
| Win32完整mip核心 | 40209761398 | 各282组、5039424 checks |
| PS3三份payload核心 | 40209761399 | 各282组、4950085 checks |
| PC/PS3体积mip核心 | 40209761393 | 各2832组、3211661 checks |
| Win32 mip实际入口 | 40209761414 | 各174组；2个原版错误、6个明确未支持重采样样本按既有断言保留 |
| PS3 mip实际入口 | 40209761391 | 各246组；2个原版错误、6个明确未支持样本按既有断言保留 |
| 体积mip实际入口 | 40209761409 | 各2610组；6个原版错误边界按既有断言保留 |
| 残差修正真实SDK | 40209761463 | PcMips、Ps3Mips、VolumeMipPc、VolumeMipPs3四profile全部成功 |

所有上表workflow均 completed/success，attempt1。真实SDK每profile都是Original / Recovered / NoOriginal三组，各18资产/18次实际Core成功调用；mod.bin/manifest/imp/relo及每次三个buffer和完整cdata逐字节相同。未启用原DLL的NoOriginal运行也通过。入口摘要的errors是测试的原版拒绝边界数量，不是新失败；不可删除这些断言冒充覆盖。

收据路径：

- `evidence/texture/stage2/{CubicResize,Mips,Ps3Mips,VolumeMips}/<runId>-1/validation.json`
- `evidence/texture/{ps3-resize-images,mip-images,ps3-mip-images,volume-mip-images}/<runId>-1/validation.json`
- `evidence/texture/sdk-ps3-resize/{Ps3ResizeNoMip,Ps3ResizeMips}/40209208221-1/validation.json`
- `evidence/texture/sdk-mips/40209761463-1/validation.json`
- `evidence/texture/sdk-new-paths/Ps3Mips/40209761463-1/validation.json`
- `evidence/texture/sdk-volume-mips/{VolumeMipPc,VolumeMipPs3}/40209761463-1/validation.json`

源码指纹核对在 `evidence/texture/handoffs/20261025-stage4/managed-inputs.json`：Managed共17份输入，d9→fd仅TriangleMipEncoder/VolumeMipEncoder两个blob改变；cubic类、Plugin.Images、raw/TGA及csproj均相同。因此cubic验收继续精确引用d9运行，普通mip用fd新运行，未变raw及旧子系统复用各自原锚点。文档提交不是新的SDK验收源码。

## 后续入口

本页只保留第四轮历史结果。后续仅 PC，当前任务与验收状态统一见 [TEXTURECOMPILER.md](TEXTURECOMPILER.md)；不再维护 PS3 cubic 或其验证。
