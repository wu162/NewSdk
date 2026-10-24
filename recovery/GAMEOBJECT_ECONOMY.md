# Dock、矿石与采集完整能力组

2026-10-24：整组实现，最终完整差分验证中，尚不计入144/322已验收范围。

| 类型 | TypeId | native |
|---|---:|---:|
| DockUpdate | 3651459681 | 28 |
| OreNode | 2384596706 | 72 |
| OreMiner | 2432276463 | 20 |
| SupplyCenterDockUpdate | 3270634528 | 36 |
| SupplyWarehouseDockUpdate | 3593042897 | 36 |
| GatherSlavesUpdate | 642696730 | 20 |
| GatherEjectUpdate | 3859925230 | 172 |

原方法在evidence/gameobject-modules/economy-source，共75个精确Marshal/Token/endian/helper；固定原Tokenizer blob6e40a9ed6c9f961d294d49768d911e6a6f354619。本地从该完整blob提取，未修改云端导出工作流；GenerateEconomyFields.py校验每个方法体SHA，Windows同时核对原blob和PE枚举。六组枚举复用同一固定blob既有已验证PE表，Windows逐组再次匹配。

Dock的NumberApproachPositions是signed@8，MinDockTime@12、ObjectsInLineWeight@16（默认13s）、nullable ObjectFilter指针@20、bool@24/25/26。SupplyCenter ValueMultiplier@28为普通float，DistributedDeposit@32；SupplyWarehouse StartingBoxes@28为signed、DeleteWhenEmpty@32，均在派生字段后恢复完整Dock父层及其Token/sentinel。

OreNode的三项数量是uint@8/12/16，CashMultiplier百分比@20，四项时间@24..36；Extract/Delivery/Return/MoveBackAnimation为四个独立单枚举@40/44/48/52，未知/显式空回退233，ALL的枚举值是454；不能把ALL写成bitmask。LowNotificationPercentage@56为百分比，三个强引用@60/64/68按原顺序分配。OreMiner含uint DeliveryCapacity@8、time DeliveryTime@12、nullable ObjectStatusValidation56指针@16；ForbiddenStatus在结构@0、RequiredStatus在@28。原Token的父层是CreateModuleData，保留该实际委托而不猜测继承。

GatherSlaves的SlaveTemplate是强引用@8，Radius为float@12，Amount为uint@16。GatherEject完整172字节：Lifetime@8、signed Slots@12、ScanTime@16，HeightOffGround/EffectDistance/LiftOffGroundDistance/GatherAcceleration为float@20..32；RotationSpeed和UnitSpacing分别为inline Vector3@36/@48，Filter为完整inline ObjectFilter@60。缺失子节点保持清零，存在但空调用子结构默认值；按原schema顺序处理，不受XML子节点先后影响。

GameObjectEconomyCases.cs覆盖七型全部根字段、枚举/数字/布尔/强引用边界、六容器、完整父Token与sentinel、nullable Filter/status、inline Vector3/Filter、全部status词和掩码、重复列表容量边界、命名空间/未知属性/重复单节点/结构文本拒绝、独立默认/大小/offset/引用及分配顺序。与既有GameObject矩阵统一/Od、/O2、平台0/1/2/99、三缓冲区全部字节比较。

源码733210317aa82c8ecef155bdb80db8c5f834d396 / run40078348035和a936ac2b575259c27e58c4fb2ff08597eb0116bf / run40078557180的失败分别是独立测试RequiredStatus偏移和ALL单枚举值写错；已按原方法/PE修正，未放宽比较或拒绝。最终验证锚点待记录。SDK本组0次，原路由保留，组件不代表完整DLL/SDK验收。
