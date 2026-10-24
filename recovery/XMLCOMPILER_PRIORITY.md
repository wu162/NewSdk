# XmlCompiler：当前续接入口

更新：2026-10-24（北京时间）。目标分支 `recovery/xmlcompiler-priority`。本轮按约一小时推进，最多三个子代理并行；实现、修复和下一能力组连续进行，最后冻结代码统一验收。

## 当前结果

**已恢复原 Plugin 301 个登记入口中的 52 个独立编译入口，本轮新增 38 个。** 其中 51 类在原 CnC3Types.xsd 允许全局资产；GameStatsModeData 仅有原 Plugin 独立入口和嵌套 schema，独立能力由组件证明，SDK 经 MultiplayerSettings 的嵌套场景覆盖，未修改原 XSD。

- 实际验收源码：`71cbdd7bd27478d0bcb9dc3dbd2f22deac888dd7`。
- 九组完整差分、联合 DLL：[运行 40095358086](https://github.com/wu162/NewSdk/actions/runs/40095358086)，completed/success。
- 原版/恢复版真实 SDK 双 profile：[运行 40095358016](https://github.com/wu162/NewSdk/actions/runs/40095358016)，completed/success。
- 已逐份核对 succeeded/sourceCommit/runId、两优化模式与四平台、真实退出码和 SDK 两套重放。之后仅文档提交不改变验收源码。
- 全部工作分支保留父提交并合入本分支，无强推。未把本轮新增再合入另一会话的 compiler-source；GameObject 仍按其独立交接继续。

## 新增能力组

| 组 | 新增入口数 | 完整范围 |
| --- | ---: | --- |
| progression | 12 | LogicCommand、PlayerTech、PlayerTechRank、PlayerTechUpgradeBinding、ExperienceLevelTemplate、PlayerTechStoreTemplate、PurchasePlayerTechButtonTemplate、ButtonSingleStateData、UICommandButton、FontDefaultSetting、FontTranslation、FontSubstitution |
| environment | 12 | Weather、WaterTransparency、WaterSetting、Environment、LightingSettings、WaterTextureList、TerrainParticleMapping、BounceKickTerrainMapping、ShadowMap、OrdersManagerSettings、TerrainVisualSettings、CameraSettings |
| players | 14 | AIData、UnitAISettings、PlayerTemplate、MultiplayerSettings、AIBuildStateDefinition、AIStrategicStateName、MultiplayerColor、AIPersonalityToAchievementEventMapping、FormationAssistantUnitDefinition、FormationAssistantTemplate、FormationSelectionTemplate、FormationAssistant、GameStatsModeData、SkirmishSettings |

原十四类及其完整能力保持回归：AttributeModifier、FXList、SpecialPowerTemplate、ObjectCreationList、LocomotorTemplate、LogicCommandSet、FXParticleSystemTemplate、UpgradeTemplate 及六个 AI 根。FXList 双过滤器和 unknown base fallback、Particle 全 33 模块/Event 都保留。

精确 TypeId/TypeHash 与 schemaGlobal 标记见 `catalog/XmlRecoveredTypes.json`；详细布局、方法和拒绝边界分别在 XML_PROGRESSION.md、XML_ENVIRONMENT.md、XML_PLAYERS.md、XML_FORMATIONS.md、XML_SKIRMISH.md 及原 XML_*.md。

## 完整差分矩阵

每行合计 /Od、/O2，平台 0/1/2/99。数组计数包含相应组件的直接 CompileXml 对照，不能全部称为原 DLL 对照次数。

| 组 | Fixtures | 数组比较 | 显式拒绝 |
| --- | ---: | ---: | ---: |
| attributes | 9528 | 28872 | 288 |
| effects | 13040 | 78240 | 320 |
| powers | 34656 | 103968 | 1072 |
| locomotor | 33048 | 99144 | 2024 |
| ai | 15824 | 94920 | 416 |
| fxparticle | 27752 | 83256 | 784 |
| progression | 33280 | 199680 | 976 |
| environment | 49056 | 294336 | 2448 |
| players | 44760 | 134280 | 984 |
| 合计 | 260944 | 1116696 | 9312 |

联合 DLL 每模式另有 52×4=208 默认根样本/624 次比较，以及 40 个非空样本/120 次比较。两模式实际进程退出码都为 0。联合 smoke 不代替九组完整矩阵。证据在 evidence/xml-<group>/validation.json 与两模式日志；联合产物在运行的 recovered-xmlcompiler-priority-x86 artifact。

## SDK 与关键修复

94 个合法资产强制覆盖 51 个全局类型及缺省、非缺省、真实继承、本地强引用、多层列表。Reduced 实际 94 次调用，原版与隔离恢复版各 282 次三数组比较；FullMymod 保留全部七个原 Includes，再追加相同 probe，97 次调用、各 291 次比较。两 profile 的 bin/manifest/imp/relo 四产物均逐字节一致，imports 非空；复用同一 /O2 DLL，两个原 DLL 指纹保持不变。隔离目录物理排除两原 DLL，并核对无原编译器程序集加载。详见 [XML_PRIORITY_SDK.md](XML_PRIORITY_SDK.md)。

- 成长组 PlayerTech/Binding 的根 scalar/count endian 转换在原 Plugin 中内联，不能因为没有独立 EndianCorrect 方法而省略。最初错误已通过原差分纠正。
- OrdersManagerSettings 的 ShaderOverride 是 nullable AssetReference**；present-empty 仍分配四字节，两个 shader 块先于四个 ShadowInfo。不是普通字符串或直接引用。
- 新组件内部类型置于唯一原生命名空间，避免 C++/CLI 抹除匿名 TU 身份后与旧 AI 的 Layout 发生 LNK2022。动态不可变表使用原生 InitOnce/进程生命周期所有权，正常退出码门槛不放宽。
- AIBuildStateDefinition 先分配 BuildEntry，再通过新增原生共享 helper 在同一 Tracker 内处理既有 heuristic；未做字节块移植。旧六 AI 全矩阵仍通过。
- AIData SDK 必须有 LowLodTree/LowLodTreeNoGrab/LowLodTreeNoHarvest 三个 AssetId 字符串。所有类型都兼容 BAB 对具名结构节点注入的裸 TypeId，实际多态分派保留。
- FontSubstitution 的 XSD 列表限制和底层 Marshal 能力分开测试；底层超出十项仍按原 DLL 输出，不靠 schema 推测 native 截断。

## 边界与后续

恢复编译器不调用原 XmlCompiler/Tokenizer；原 DLL 只用于 oracle。仍接受 BAB 预处理后的引用/defaults/继承和数字多态 TypeId，不在组件内重做整个预处理器。未知内容和文档列出的不支持边界继续明确拒绝。SDK 原配置和原 DLL 未替换，切换脚本用于指定 SDK 副本，其他类型仍由原处理器负责。

52 个独立入口不是全部 301 入口或完整 Tokenizer 的总替代。Joypad 根仍排除；GameObject 不在本轮修改范围。下一轮继续其他非 Joypad 完整能力组，先核原 Plugin dispatch/Tracker/Marshal/Endian 与完整依赖，再合批差分。可优先调查 DamageFX/DamageFXSettings、其他 UI/字体/音视频或设置类；未取证前不因小型外观假设简单。

取证固定原 Module blob2d2f471c8008b7c2e1f78e39752d0726da395cea、Plugin blob00bb9e5df72d07a21204f85f9a9a2f1d9557f00b、原 DLL SHA256 d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6。复用 evidence/xml-source 的方法索引、小块和静态字节，不必重下整份反编译。历史失败保留在提交和 Actions；只以上述最终 source/run 为验收。按完整组统一运行并复用公共 helper，未量化账号额度节省。
