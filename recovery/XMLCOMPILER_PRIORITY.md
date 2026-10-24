# XmlCompiler 优先类型：当前续接入口

更新：2026-10-24（北京时间）。分支 `recovery/xmlcompiler-priority`。本入口描述 XmlCompiler 工作；GameObject 仍按其独立交接继续。

## 本轮结果与精确锚点

- 三个子代理并行实现，所有实现与测试均已保留父提交并合入本分支，未强推。
- 实际验收源码：`aac2279c99abf4c55d7f4d682b3e68bbe3a77107`。
- 六组完整组件矩阵及联合链接：运行 [40091218825](https://github.com/wu162/NewSdk/actions/runs/40091218825)，completed/success。
- 真实 SDK：运行 [40091218796](https://github.com/wu162/NewSdk/actions/runs/40091218796)，completed/success。同一 /O2 编译产物分别完成 Reduced 与 FullMymod 的原版/恢复版新鲜构建、四产物比较和两种重放；详见 [XML_PRIORITY_SDK.md](XML_PRIORITY_SDK.md)。
- 证据均已核对 succeeded、sourceCommit、runId、两模式日志及真实进程退出码。后续仅文档更新不改变验收源码。
- 旧 PR #1 已在 `dddb9ac2804da1380a5b9b94a9035253c6e48f63` 合入 `recovery/compiler-source`。本轮新增成果按用户要求留在当前分支，未再次合入另一工作分支。

## 十四类当前范围

| 类型 | 组件支持面 |
| --- | --- |
| AttributeModifier | 60 字节根、Modifier 列表、状态位集、Shader 引用 |
| FXList | 全部 5 个根属性、15 个已知 Nugget、四组模型状态位集、两类完整 ObjectFilter、未知/缺失 TypeId 的 44 字节 base fallback |
| SpecialPowerTemplate | 38 个根属性、过滤器、GameDependency、PlayerTech 列表 |
| ObjectCreationList | 六组 typed nuggets、向量、范围、引用及嵌套列表 |
| LocomotorTemplate | 114 个根属性、JetLocomotorData、BounceKickTerrainMap |
| LogicCommandSet | Cmd 引用列表及 imports |
| FXParticleSystemTemplate | 完整根、关键帧、Wind、全部 33 个已知模块、非空 Event 指针列表、原版未知模块 base fallback |
| UpgradeTemplate | 140 字节根、字符串/资源、GameDependency、ObjectFilter |
| AIPersonalityDefinition | 根、Side、18 个列表、OpeningMove/Heuristic、Trouble、过滤器 |
| AIStrategicStateDefinition | 根、Tactic、TeamTemplate、CreateUnits、继承 heuristics |
| AIMicroManagerData | 根、TargetBias、SpecialPowerDelay、RetreatBaseCriteria、IgnoreTargets |
| AIBudgetStateDefinition | Budget/AccountShare、继承 heuristics |
| AIStateDefinition | 29 种已知 heuristics、加权组合、嵌套过滤器 |
| AITargetingHeuristic | 根、BaseSelection、原 EASTL 距离排序，含 signed zero |

全部保留 InstanceData、RelocationData、ImportsData；单 Tracker 使用绝对偏移零，不加 Tokenizer 过程表。恢复实现不调用原 XmlCompiler/Tokenizer，原 DLL 仅供测试对照。新增 Particle 代码拆成 Bridge/Native 的三个 include 与两个 partial 测试文件，避免继续堆入主文件。

## 完整组件矩阵

下表合计 /Od、/O2 及平台 0/1/2/99。数组比较含相应组的直接入口对照，不应全部称为原 DLL 对照次数。

| 组 | Fixtures | 数组逐字节比较 | 显式拒绝 |
| --- | ---: | ---: | ---: |
| attributes | 9528 | 28872 | 288 |
| effects | 13040 | 78240 | 320 |
| powers | 34656 | 103968 | 1072 |
| locomotor | 33048 | 99144 | 2024 |
| ai | 15824 | 94920 | 416 |
| fxparticle | 27752 | 83256 | 784 |
| 合计 | 133848 | 488400 | 4904 |

联合 DLL 每模式另有 56 个默认根样本、168 次比较，以及 40 个本轮非空能力样本、120 次比较。两模式进程均正常退出 0。联合 smoke 不代替六组完整矩阵。

证据：`evidence/xml-<group>/validation.json` 和两模式 oracle/build 日志；联合证据在 `xml-priority-link`，DLL 在对应运行的 `recovered-xmlcompiler-priority-x86` artifact。

## 本轮修复的关键契约

- FXList 的 SecondaryObjectFilter@24 先于 SourceObjectFilter@28 分配，各 112 字节，在四个模型位集之后、disabled 列表之前。未知 Nugget 分配 44 字节并写 TypeId=0。
- Particle 新增 UpdateDefault104、UpdateRenderObject200、DrawGpu60、DrawLightning36、DrawRenderObject20、EventBase12、EventCollision28、EventLife24。Event count@156/items@160；四个前置 singleton 后分配事件指针数组及模块，最后才是 Update@164。
- Gpu Shader 与 ObjectSet RenderGroup 按原 ANSI 转换及 strlen 截断处理；Unicode、嵌入/首位 NUL、空与缺失、字符串及列表分配顺序均有原版差分。UpdateDefault 两个 damping 随机变量缺省 UNIFORM/1/1，部分覆盖保留未赋值端点。
- BAB `AssetDeclarationDocument.ValidateInstances()` 为所有具名 schema 节点添加裸 TypeId，不只多态节点。结构元数据被忽略或从 ObjectFilter 属性 map 剔除；真实 heuristic/module TypeId 分派保留。FXList/Particle 对可选根 TypeId 另检查匹配，foreign namespace 与其他未知内容仍拒绝。
- 原 XmlCompiler metadata：版本 Win32/unknown=4、PS3=7、Xbox360=6；ProcessingHash 是 TypeHash XOR version，Tokenized=false。十四类在四平台 Initialize/ReInitialize 的 800 项公开元数据比较通过，见 [XMLCOMPILER_METADATA.md](XMLCOMPILER_METADATA.md)。

## 边界与下一步

组件仍面向 BAB 预处理后的引用、默认值和 numeric polymorphic TypeId；不是完整 XSD/继承/资源解析器。未知 XML、部分不合法结构、AI 超过 96 层及非有限排序距离继续明确拒绝。Personality 原节点名是 CountAsStructureForBuildPurposesFilter。

真实 SDK 已验收十四类路由及完整 shipped mymod 的本次输入，保留全部原 Includes并追加 18 个 probe；这不等同于所有类型/所有字段的真实游戏内容覆盖。EventBase、未知 fallback 不能由合法 XSD 具体元素表达，其证据来自组件矩阵。FXList/Particle 不支持 schema 层 inheritFrom，未伪造继承样例。

SDK 原 DLL 和原配置保持不变；切换入口 `scripts/SetXmlPriorityPlugin.ps1` 可用于一次性 SDK 副本，显式注册十四类，其他类型仍用原处理器。联合 DLL 不是整个 301 类型 XmlCompiler 的完整替代，GameObject/完整 Tokenizer 及 Joypad 不在本轮新增范围。

下一步可继续其他非 Joypad 根类型的源码恢复，按完整能力组取得原 DLL 差分，再集中 SDK 验收；正式总替代仍需完整类型覆盖与所有适用集成门槛。优先复用本次 metadata、BAB TypeId 和双 profile harness，勿重复构建已验收的同源码组件来填时长。

取证仍固定原 Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`、原 DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。按组读取 XML_*.md 及 evidence/xml-source 小块；无需重下整份反编译。早期失败运行保留，不能作成功锚点。没有账号额度计量，不能量化节省比例。
