# 初步还原计划

## 已确认的起点

- 基点：`c66339e49e32cfe3f44aa2f9f1757f9993bbac7a`；原 DLL 位于 `SDK/tools/BinaryAssetBuilder.W3XCompiler.dll`，159744 字节，SHA-256 `bff82309b8e5e9f689112202e3613ad335148934c92dc70fe143f6e60d9ad3f1`。
- `SDK/tools/BinaryAssetBuilder.exe.config` 的原版路由为 `W3DAnimation,W3DMesh`，targetType 为 `BinaryAssetBuilder.W3XCompiler.Plugin, BinaryAssetBuilder.W3XCompiler`。配置 blob 为 `9fbb588946332d97a38d279161363b131de76cf0`。W3DHierarchy/Container 等名字出现在 DLL 中，并不因此证明由此插件直接接管。
- 原 DLL 字符串中可定位 `HandleAnimation`、`HandleMesh`、`ProcessInstance`、`W3DMeshProcessor.BuildVertexData`、`DetermineVertexDataLayout`、`ComputeBoneMappingTable`、`CompressTexCoord`、`Vector3_To_Half4` 和 `DataHandlers_Windows/Xenon/PS3`。这些是调查线索，签名、代码种类、完整依赖和算法仍须由元数据、IL、原生代码及探针确认。
- 尚无 W3X 恢复版处理逻辑、差分成功矩阵或真实 SDK 成功证明。旧 XmlCompiler/Tokenizer 的类型数和通过计数不适用于本项目。

## 已定位的输入和复用入口

`SDK/Schemas/xsd/AssetTypeW3D.xsd`（基点 blob `6fee2f746c22a4eb84af39c68a17593d62eadc78`）包含 mesh/pipeline vertex data、FXShader、AABTree、scalar/quaternion 输入通道与 timecoded/delta runtime 通道。W3DMesh、W3DAnimation 标为 W3D typeGroup；Hierarchy/Container/AnimationRuntime 等标为 Xml，不能直接扩大 W3X 插件路由。

`SDK/Art/W3/W3XViewQuad.w3x`（blob `aa7b456c17afd50a5b527b04127459f1c0f3af4f`）是可分析的小网格，含4顶点、2三角形、法线、UV、ShadeIndices 和 FXShader；依赖 `ART:w3xview.xml`。作为候选基线，SDK 验证时保留其纹理/材质依赖并重新验证，不能声称已通过。`W3XViewLight.w3x` 和 `W3XViewBoneAxis.w3x` 可用于扩展调查，不一次性加载全部 Art。

动画 XSD 明确包含 CompressionSettings 的 TimeCoded/AdaptiveDelta 开关、误差参数和 ForceReductionRate，以及每帧 BinaryMove。**XSD 默认值不一定等同于 Plugin 在缺少整个 CompressionSettings 节点时选择的默认值**；这一差异纳入原版探针。

FXShader/多态常量已有 `src/Native/GameObjectDrawBeam.h`、`GameObjectDrawBeamRuntime.h` 与 `GAMEOBJECT_DRAW_BEAM.md` 可参考。先核对 W3X 同名结构和 Marshal 的实际合同；一致时考虑提取共享 Shader 模块并运行两边回归，不让 W3X 长期依赖整套 GameObject 业务编译器。

## 阶段 A：原版基线和边界合同

运行 `ExtractW3x.ps1`：复用 MetadataInventory，固定 ILSpy 9.1.0.7988，保存方法 token/RVA/signature/IL、类型 layout、PE imports/exports、原生反汇编和插件公开合同。自动证据只写唯一 run/attempt 目录；反编译失败状态必须保留，不能将机械 C# 当可编译源代码。

下一组从 Plugin、HandleAnimation/HandleMesh 入口出发，建立完整调用图与类型清单，分清 IL 业务、native 业务、CRT 和第三方依赖。逐个查明 Initialize/ReInitialize、版本/processing hash、GetExtendedTypeInformation、未知类型/平台、异常与释放行为。按需读 Core 中的 BAB 接口和 Schemas 中实际 XSD，采集最小合法动画/网格样例。初期定向探针回答未知合同，随后合成整组验收，不逐字段启动全矩阵。

交付：原 DLL 固定指纹、可重现的原版输出/调用捕获、类型和布局合同、平台矩阵以及首个恢复版构建入口。只有精确合同确定后才创建有真实职责的类和项目引用，不用空实现充数量。

## 阶段 B：完整动画处理能力组

优先动画，因原入口和通道族线索明确、可独立构造样例；此顺序是初步判断，若取证发现网格更低依赖可调整并记录原因。覆盖根数据、scalar/quaternion 通道、帧索引与时间、压缩/runtime 转换、资源引用、所有平台及异常边界。先定位实际支持的通道/编码分支，不凭名字猜格式、容差或四元数规范化。完整能力组一起实现并跑三缓冲区逐字节差分。

交付：有明确支持/拒绝边界的动画组件、独立布局断言与完整原版差分。仅组件通过时仍标为 SDK 待验收。

## 阶段 C：完整网格处理能力组

基于真实 DataHandlers 和布局算法，实现 vertex declaration、各属性流的 defaults/格式、纹理坐标/法线/切线/颜色/权重转换、bone mapping、索引与平台布局。查清半精度浮点、归一化/舍入、饱和/溢出、重复映射、空流、非法计数和内存布局；不能用数学近似或现代库默认转换代替原版位模式。

按完整共用依赖分组（例如布局及所有顶点转换一起），统一完整矩阵。确需拆分时依赖和显式拒绝范围清楚，不把单个字段当一阶段。复用共享 tracker/解析器前增加 W3X 原版合同差分；发生公共改动时运行所有受影响的原有回归。

## 阶段 D：正式插件、SDK 与合并

元信息和受支持处理链都恢复后构建正式插件，临时 SDK 副本只改 W3DAnimation/W3DMesh 路由，原/恢复版相同输入、配置、依赖与缓存条件，记录实际调用。跨能力组累计后集中做 SDK 验证；正式宣称接管前必须通过。保留完整 mymod 里程碑验收，不以只含空模型的编译成功代表覆盖。

检查 C++/CLI/托管/原生全部必要路径是否仍调用原 DLL，完成无原 DLL 重放。将覆盖率按真实方法/分支/资产类型记录，确认剩余拒绝边界和未知项。合回原工作线前解决公共代码差异、运行双边受影响回归，保留两项目的证据和续接导航。完整替换原程序集的身份/版本/部署是最后独立门槛。

## 执行方式

框架规划阶段完成即可交接；后续实现遵循当前时长规则，以完整能力组推进。每约10–15分钟保存有意义检查点，不等待整组全通过才提交；未验证检查点不计为已恢复。GitHub 插件读取/提交，禁止强推；Actions 一般至少约2分30秒，等待时做独立工作，不高频轮询。每轮交接只写当前入口、精确证据、真实阻塞和下一能力组，避免重读整段旧交接。
