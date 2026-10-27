# TextureCompiler 当前交接

2026-10-26。分支 `recovery/texturecompiler-source`，源码位于根目录 `src/TextureCompiler`。

## 唯一目标：Win32 / PC

用户要求移除非 PC 代码与规划。PS3 专属三份 mip、自动 cubic 缩放、平台输出分支及对应测试/取证/SDK 入口已删除；Xbox360 与 unknown 平台不再支持。Initialize/ReInitialize 和公共平台入口明确拒绝非 Win32。SDK 原 DLL、共享 Core 平台枚举、原始 IL 和历史证据不变；历史 PS3 验收不属于当前能力或待办。

## PC 已有能力

- Texture / OnDemandTexture 的 DDS 原字节直通、三缓冲区和完整 cdata。
- 声明范围内的 POT TGA / PNG / BMP 解码，原版错误与拒绝行为。
- A8R8G8B8、X8R8G8B8、R5G6B5 三种 raw DDS 输出。
- PC 二维 triangle 完整 mip，体积条带、raw 与递归 mip。
- 元数据、图像规则/1644 字节压缩选项、内存 atlas packer 和已恢复 native buffer。

删除前最终 PNG/BMP SDK 锚点：`9ba02841cf320083c6ec3c571744f3c4c36e401b` / [40235430314](https://github.com/wu162/NewSdk/actions/runs/40235430314)，其中 PcNoMip / PcMips 各 Original/Recovered/NoOriginal 三组、每组 600 资产/实际 Core 调用，四产物和 buffer/cdata 相同。TGA/mip/volume 的历史精确锚点保留在各能力文档；不冒充本轮新验证。

## 仍需完成（仅 PC）

1. DXT1 / DXT1A / DXT3 / DXT5 压缩核心。已有 options、原生入口与 PC 小块样本，尚无恢复版压缩器；见 [DXT 取证](TEXTURE_DXT_ENTRYPOINTS.md)。不再调查跨平台一致性。
2. NPOT D3DX 图片重采样，目前明确拒绝。
3. JPEG、尚未声明支持的 PNG ICC/APNG、TGA metadata/RLE 边界和其他 BMP 组合，按实际 PC 输入需求独立取证。
4. 完整 TerrainTextureAtlas 解码、处理、压缩及输出入口；目前只有内存 packer。
5. 审计 PC 实际需要的原 native 初始化/释放及公共图像依赖。保留共享 helper，不为已排除平台补实现。
6. 最终合入并接入新编译器。生产 SDK DLL/config 仍未替换，完整 DLL 尚未还原。

## 本轮验证（已完成）

- PC 清理源码 `0ae0efdb3381446c116d474f45643ae225840abb`，集中运行 [40241188546](https://github.com/wu162/NewSdk/actions/runs/40241188546)：10 个受影响组件组 Debug/Release 通过；PcNoMip / PcMips 各 Original/Recovered/NoOriginal 三模式、每模式 600 资产/实际 Core 调用，四份产物和全部 buffer/cdata 相同。TGA、formats、PNG、BMP、PNG ranges 五个未变组保留原验收锚点。
- 该首轮 workflow 为 failure：基础组 native 检查缺少 `cl` 环境，不能写作整轮成功；其他成功组的精确收据仍有效。统一工作流补入 x86 MSVC 初始化，代码本身未再修改。
- 基础组复验源码 `7786834f71bcb91bace593715de0efb49cb973ab`，运行 [40241581283](https://github.com/wu162/NewSdk/actions/runs/40241581283) completed/success。Debug/Release metadata 各 628 比较、image rules 各 13,797 记录/148,802 检查、DDS 各 695 例、native 各 264,429 检查全部通过；packer 精确沿用 `ece235ac79fd7216777073ea8cc9416dd0018f47` / 40193344145。复验只选 base，15 个组件组复用，未再次启动 SDK。
- 元数据检查还验证非 Win32 Initialize/ReInitialize 明确拒绝且不改变原 PC 状态。选择器 13 项检查通过。生产 SDK 子树、原 DLL/IL 与历史证据不变。

精确收据：`evidence/texture/batch/40241188546-1/validation.json`、`evidence/texture/validation/40241581283-1/validation.json`、`evidence/texture/sdk-rasters/{PcNoMip,PcMips}/40241188546-1/validation.json`。文档收尾提交没有修改业务代码，沿用以上源码锚点，不冒充新跑验收。本轮清理已完成，剩余 PC 能力仍按上节推进。

历史格式/数值边界：[PNG/BMP](TEXTURE_PNG_BMP.md)、[TGA](TEXTURE_TGA.md)、[raw DDS](TEXTURE_RAW_DDS.md)、[triangle mip](TEXTURE_TRIANGLE_MIPS.md)、[volume](TEXTURE_VOLUME.md)。历史 PS3 文档仅留作证据索引，不产生后续工作。

## 2026-10-27 续接：DXT3 alpha 子组件

新增 PC `TextureDxt3AlphaEncoder`，仅 4×4 RGBA 的 8 字节显式 alpha，不接入尚未完整实现的 DXT3 插件路由。源码 `9f8b1361` / [40470055551](https://github.com/wu162/NewSdk/actions/runs/40470055551) success：Debug/Release 各 4,352 例、65,288 检查，与原 DLL alpha 块逐字节相同；仅运行新组、复用16组，未重跑 SDK。完整 DXT 颜色端点搜索、DXT1/1A/5 仍待恢复。细节及参考库函数映射工具见 [DXT 续接](TEXTURE_DXT_ENTRYPOINTS.md)。

同轮参考库映射及 PC 块取证源码 `70aded86` / [40470267556](https://github.com/wu162/NewSdk/actions/runs/40470267556) success：32个PC原版块样本、22个目标代码节中19个重定位屏蔽后匹配；仍为 probe-only，不能作为完整压缩器验收。候选 RVA 和下一轮调用链核对入口见 DXT 文档。
