# TextureCompiler 当前交接

2026-10-28（北京时间）。分支`recovery/texturecompiler-source`，源码根`src/TextureCompiler`。最新入口 [RESUME.md](RESUME.md)。

DXT3/5默认71颜色/alpha、完整块、POT二维/volume无mip与完整float mip、两插件入口已验收。NPOT已扩至PNG灰度/GA8/RGB/RGBA8/16、Additional TGA16/灰度8/index8调色板及BI_RGB BMP16/24/32，保留原错误与量化/通道布局。业务e9292a26/40595382578的18组及7个SDK profile通过，插件隔离宿主修复9e7d671e/40597016969完成19404输入D/R/NoOriginal；两run各自总体失败原因和精确成功组保持原记录。后续abdffd97/40598190231与daf4a749/40598863269均success，前者闭环原探针并复用34组，后者完成9418存储语料D/R回放及独立atlas指定五层DDS组件。详见[当前续接](RESUME.md)、[NPOT](TEXTURE_NPOT_NEXT.md)、[atlas](TEXTURE_ATLAS_NEXT.md)。生产SDK DLL/config未替换，完整atlas公共入口尚未接入。

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

1. DXT1 / DXT1A默认完整路径、完整压缩器和生产集成。默认71颜色核心与DXT3/5完整块、对齐整图和无mip插件路由已取得组件验收；小尺寸/float块与小尺寸无mip消费者已通过组件和SDK门槛，压缩volume现也已通过组件及专用SDK完整验收。不再调查跨平台一致性。
2. NPOT已声明源存储路径已按上文验收；未声明格式/metadata按实际输入需求独立取证，原版拒绝格式继续保留原拒绝。
3. JPEG、尚未声明支持的 PNG ICC/APNG、TGA metadata/RLE 边界和其他 BMP 组合，按实际 PC 输入需求独立取证。
4. 完整TerrainTextureAtlas插件和SDK；已具备内存packer及独立DXT5/RGB555指定五层DDS组件，接下来消费完整原插件gold恢复Marshal/加载/输出与relocation。
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

## 2026-10-27 第二阶段：Production DXT5 alpha

PC DXT5 默认 Production alpha 已独立还原：字节 RGBA 与归一化 float 输入、五候选选择、x87 量化搜索、端点和索引以及8字节PC payload。`Recovered.TextureDxtAlpha.dll` 从自有 x86 汇编源码构建，无入口、无导入；不调用原DLL。完整颜色块及插件 DXT 路由仍未实现，生产 SDK 未替换。

最终验收源码 `4bf0ca8c` / [40493971846](https://github.com/wu162/NewSdk/actions/runs/40493971846) success。Debug/Release 各：量化器2176例/408835检查（含误差位模式）；实际原DDS alpha 78144例/1172168检查；float alpha 12288例/516108检查；NoOriginal隔离重放78144例/625152比较；native x86/零入口/零导入检查通过。只选新组，18个未变组复用，未跑SDK。边界、失败反例、独立native构建和后续颜色恢复入口见 [TEXTURE_DXT_ALPHA.md](TEXTURE_DXT_ALPHA.md)。

quality68快速路径仅接受低层state子组件；原版未初始化levels，完整DDS入口失败证据保留，不作为Production验收替代。颜色源码取证在RoundMult超时处取得部分结果，不能标成完整提取成功。

## 2026-10-27 第三阶段交接

默认PC颜色状态与完整DXT3/5块已分别验收（2cee3446/40500150864、b0278705/40500898688，Debug/Release与NoOriginal）。整图无mip DDS新实现 a9cc0daf / 40501793638 尚在验证。最新范围、源码构建与后续顺序见 [TEXTURE_DXT_COLOUR.md](TEXTURE_DXT_COLOUR.md)。不再将已验收默认71颜色核心写作完全未恢复，但完整整图/插件/SDK、DXT1默认路径、mip与volume仍无完成声明。

