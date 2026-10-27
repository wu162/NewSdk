# 雷达、可见性与Capture完整组（已验收）

已验收源码7cd68875d9fda24b8650676b4794c246dd4f873e；GameObject run40049625447、基础run40049625277均success。每/Od、/O2：187612 fixtures、562836三缓冲区比较、9792拒绝、384不变性，平台0/1/2/99。SDK未运行。

固定原blob6e40a9ed6c9f961d294d49768d911e6a6f354619；evidence/gameobject-modules/vision-source/manifest.json导出87方法，含八新模块、两个原已完成ClientUpdate与全部共享依赖。enum-source.json从原PE取UnifiedControlType（ALLIED_PLAYERS=1/SAME_PLAYER=0）及RelationshipType（ENEMIES=0/NEUTRAL=1/ALLIES=2）。GenerateVisionFields.py逐方法SHA256并核对原blob/PE、字段/fallback和完整递归Token/sentinel；运行结果以validation.json对应sourceCommit为准，未成功前不计入支持范围。

| 模块 | TypeId/native | 全部输入与布局 |
|---|---|---|
| CaptureBehavior | 4173638251/20 | GrantMoney uint@8默认0，GrantMoneyOneTime bool@16默认false，SlavedCaptureObject弱GameObject ID@12 |
| UnifiedControlBehavior | 3179117790/24 | UnificationType scalar@20默认SAME_PLAYER、未知1，然后完整Capture |
| RadarUpdate | 1768048089/12 | RadarExtendTime普通float@8默认0（不是TimeTag） |
| UnitVisionBehavior | 4070192848/12 | UpdateRate signed/base0@8默认1（不是时间） |
| SelectiveVisibilityBehavior | 1632936356/16 | UpdateRate可空TimeTag@8，AllowedPlayers可空3bit RelationshipType@12；依此顺序分配，各4字节，缺失null/显式空分配0 |
| InvisibilityBehavior | 88803780/12 | InvisibilityTemplate强引用@8 |
| InvisibilityBroadcastUpdate | 2984624090/44 | InvisibilityTemplate强引用@8，AffectsSelf bool@40默认true，UpdatePeriod TimeTag@12默认1s，BroadcastRange普通float@16默认0，ScalableDrawModuleId弱ModuleData ID@20，ScaledBoneName String头@24；BroadcastObjectFilter可空112字节@32，ObjectStatusValidation可空56字节@36 |
| UnifiedShroudRevealBehavior | 2339238473/24 | Marshal直接UnificationType+Capture，Token额外保留UnifiedControl父层；不能压平Token继承 |

Capture/visibility原Marshal常直接调用ModuleData，但Token保留Behavior、Update、Contain等各父层。InvisibilityBroadcast分配固定顺序为字符串内容、filter及其IncludeThing/ExcludeThing依赖、validation（ForbiddenStatus@0与RequiredStatus@28）。不跟随XML顺序。filter节点复用ReadDependencyFilter的完整校验并保持原BroadcastObjectFilter名称。

Relationship位集未知映射bit2、ALL填充整32位、删除按原XOR语义；标量UnifiedControl未知值为1，与默认0不同。所有强引用继续产生既有translations，弱ID先小写FastHash，不混淆两者。

新增公共ModuleValue::NullableTime和NullableBits，各仅为新字段使用；既有完整GameObject回归不删减。新增测试覆盖全部字段、数值/单位边界、枚举/位集、大小写/Unicode/引用、缺失/显式空、父链及Token类型、独立布局/分配次序、六容器、0/1/2/27/28/29/65模块和列表以及未知/重复/外来命名空间拒绝。SDK未运行，原路由未改变。
