# ProductionUpdate与两个出口：完整组件实现

原方法及精确签名/原行号/SHA256：evidence/gameobject-modules/production-source/manifest.json，63方法，固定原blob 6e40a9ed6c9f961d294d49768d911e6a6f354619。GenerateProductionFields.py从原源码及PE生成字段、枚举与Token。生成检查run40035737553通过；运行验收锚点见RESUME.md。

| 模块 | TypeId | 字节 | 主要布局 |
|---|---:|---:|---|
| ProductionUpdate | 1858556985 | 84 | 17根属性、QuantityModifier@60、ProductionModifier@68、GiveUpgradeToProduced指针@76、4bool@80..83 |
| DefaultProductionExit | 1785063229 | 16 | UnitCreatePoint可空Vector3指针@8、NaturalRallyPoint可空指针@12 |
| QueueProductionExitUpdate | 2422471532 | 48 | 7根属性，UnitCreatePoint inline Vector3@20、NaturalRallyPoint inline@32、4bool@44..47 |

## 原始契约

- ProductionUpdate.MaxQueueEntries unsigned默认99，NumDoorAnimations signed/base0默认0。Type是8项ProductionQueueType，默认INVALID=-1，未知4(AIRCRAFT)。DisabledTypesToProcess为12位inline标志，未知index0；ALL仍写满32位存储字。
- DoorOpening/WaitOpen/Close、ProductionTimeDelayScalar及NewObjectFadeInTime为Time；ConstructionCompleteDuration、UnitInvulnerableTime与SpecialPrepModelconditionTime则是unsigned，不能因名称Time就转时间。
- QuantityModifier为8字节：Name弱GameObject ID@0、Count signed@4，无fallback。
- ProductionModifier为124字节：RequiredUpgrade强引用@0，两个普通float CostMultiplier@4/TimeMultiplier@8默认1.0，ModifierFilter inline112@12；过滤器缺失保持零，显式空才写默认。
- SizedList<ProductionModifier,2>原Marshal方法56462仅转调普通List；没有两项上限。已覆盖3/27/28/29/65项，不自行增加限制。
- GiveUpgradeToProduced是可空8字节结构，内含UpgradeTranslate列表；每项UpgradeTranslation8字节，RequiredUpgrade/ProvidedUpgrade两强引用。显式空容器仍分配8字节，缺失为null。
- 原分配顺序为QuantityModifier、ProductionModifier及其inline filter依赖、GiveUpgradeToProduced及translations；XML颠倒顺序不改变结果。
- Default出口两Vector3仅显式子节点时分配，UnitCreatePoint始终先于NaturalRallyPoint；Queue出口直接写入对象内联坐标，不增加分配。Queue.ClearAlliesFromDestination默认true，其余3bool false。
- 三个Token均保留UpdateModuleData→ContainModuleData→BehaviorModuleData→ModuleData继承层；Default/Queue实际Marshal最后仅调用ModuleData，但不能据此删Token父层。

完整差分包括/Od、/O2，平台0/1/2/99，Instance/Relocation/Imports三缓冲区逐字节，全部枚举、数值/时间单位/引用边界、全部列表扩容、nested filter、独立defaults/offset/Token/sentinel及严格拒绝。SDK路由不变，组件不冒充完整GameObject或正式替代DLL。
