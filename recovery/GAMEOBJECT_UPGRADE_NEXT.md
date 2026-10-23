# 下一完整组：Upgrade基类与十种派生（仅源码调查）

当前只保存原方法证据，未启用这11个TypeId，也没有原DLL运行差分。源码固定blob6e40a9ed6c9f961d294d49768d911e6a6f354619，逐方法签名/原行号/SHA256见evidence/gameobject-modules/upgrade-source/manifest.json。Exporter现在按完整类型名索引，避免把派生类误列为基类自己的方法。

| 模块 | TypeId | nativeSize | 已核对的新增字段 |
|---|---:|---:|---|
| UpgradeModuleData | 2925571736 | 36 | 见下方公共契约 |
| PowerUpgradeModuleData | 3282447464 | 36 | 无额外Marshal字段；需保留实际Token继承层 |
| DelayedUpgradeModuleData | 1518063315 | 40 | DelayTime@36，unsigned/base0，默认0，不是Time |
| AttributeModifierUpgradeModuleData | 1956793413 | 40 | AttributeModifier@36，资源引用；Marshal内联在wrapper |
| InvisibilityUpgradeModuleData | 1220819118 | 40 | InvisibilityTemplate@36，资源引用；Marshal内联在wrapper |
| CommandSetUpgradeModuleData | 2125984281 | 40 | CommandSet@36，TypedAssetId<LogicCommandSet>；继续核对实际hash helper |
| ExperienceScalarUpgradeModuleData | 3765090183 | 40 | AddXPScalar@36，普通float默认0，不是百分比 |
| MaxHealthUpgradeModuleData | 2090477367 | 44 | AddMaxHealth@36普通float默认0.0；ChangeType@40枚举 |
| ArmorUpgradeModuleData | 2717792172 | 44 | ArmorSetFlag@36枚举；KillArmorUpgrade@40、IgnoreArmorUpgrade@41默认false |
| LevelUpUpgradeModuleData | 2541561249 | 48 | LevelsToGain@36、LevelCap@40均signed/base0默认0；DoFlash@44默认true |
| AudioLoopUpgradeModuleData | 585970648 | 48 | SoundToPlay@36资源引用；KillAfterMS@40 unsigned/base0；KillOnDeath@44 bool无fallback |

## 公共UpgradeModuleData完整契约，原Marshal行19321

大小36，继承ModuleData的前8字节。六类子输入/属性的实际分配次序：

- RequiresAllTriggers@28、RequiresAllConflictingTriggers@29、Permanent@30、WaypointMode@31，默认false；WaypointModeTerminal@32默认true。
- CustomAnimAndDuration可空指针@8，分配12字节AnimAndDuration；已核对AnimState@0是scalar ModelConditionFlagType（不是位集，属性无fallback）；Frames@4和Trigger@8均unsigned/base0默认0。原Marshal14531、Token78117、Endian14553；仍须核对scalar enum helper和Token绑定。
- TriggeredBy@12、ConflictsWith@20均inline8字节列表，元素4字节弱TypedAssetId<UpgradeTemplate>，小写HashAssetId；不能当作强资源引用。列表在AnimAndDuration之后分配。
- 最后调用ModuleData Marshal取id。派生模块先处理自身字段和分配，再调用Upgrade基类；不能只按offset排序生成分配次序。

已核对scalar枚举helper：ArmorSetType原51568，23项、fallback12；MaxHealthChangeType原55913，3项、fallback2；ModelConditionFlagType原52035，457项、fallback233。其PE名称/数值表仍须按原表生成，不能把scalar枚举误用为位集或手填猜值。

下一轮要继续读上述折叠wrapper、父方法、完整Token/Endian及enum helper，核对MaxHealthChangeType、ArmorSetType、AnimAndDuration.AnimState等PE枚举表与fallback。Power的same-size不代表Token可省略。Source-only清单不得代替运行支持表。

实施以完整11类型与公共依赖组为目标，新增完整根字段/弱列表/可空结构及边界、独立Token/sentinel、拒绝矩阵；统一/Od、/O2、0/1/2/99三缓冲区差分。其他大型Upgrade派生仍明确拒绝，不能据此声称整个Upgrade家族已完成。SDK按大里程碑累计，本组组件实现不自动触发SDK。
