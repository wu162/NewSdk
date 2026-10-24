# 剩余模块清单（收尾检查点）

此表以当前实现分派为准：已知Marshal总322类，当前源码接受283类、仍显式拒绝39类；**接受数不等于验收数**。最后8类待Windows完整矩阵，上一完整组终态见RESUME.md。58 Token-only未知回退继续保留。

| TypeId | 类型 | native |
|---:|---|---:|
| 7192255 | HelicopterSlowDeathBehaviorModuleData | 208 |
| 115223906 | ToppleUpdateModuleData | 44 |
| 227975924 | BuildingDestructionModuleData | 72 |
| 245525561 | ClearanceTestingSlowDeathBehaviorModuleData | 140 |
| 267870432 | EngineerContainModuleData | 88 |
| 415825860 | SuckUpObjectsUpdateModuleData | 140 |
| 478930205 | TeleportStoredObjectsUpdateModuleData | 60 |
| 494913911 | HordeGarrisonContainModuleData | 176 |
| 513142359 | CaptureAndGiveUpgradeBehaviorModuleData | 32 |
| 803073101 | W3DLaserDrawModuleData | 112 |
| 1052292566 | WeaponScatterEffectUpdateModuleData | 136 |
| 1083087423 | HordeTransportContainModuleData | 336 |
| 1121390653 | W3DSpotlightDrawModuleData | 224 |
| 1197014736 | HordeContainModuleData | 500 |
| 1322199627 | W3DScriptedModelDrawModuleData | 216 |
| 1323283019 | SwitchLocomotorUpdate | 112 |
| 1346840366 | InfiltratorContainModuleData | 128 |
| 1409388128 | W3DTracerDrawModuleData | 112 |
| 1468802863 | TransportContainModuleData | 332 |
| 1601476857 | ConvergingLaserStateModuleData | 136 |
| 1610744897 | W3DStreamDrawModuleData | 76 |
| 1853336153 | SqueezeFlingUpdateModuleData | 164 |
| 1979616656 | DestructibleEnvironmentUpdateModuleData | 40 |
| 2091086747 | MagneticEffectUpdateModuleData | 152 |
| 2323717438 | W3DTankDrawModuleData | 228 |
| 2354114775 | ContestableGarrisonContainModuleData | 220 |
| 2395449223 | TunnelTeleportContainModuleData | 184 |
| 2557908884 | SlowDeathBehaviorModuleData | 116 |
| 2615586235 | W3DDynamicsDrawModuleData | 252 |
| 2642984081 | SecondaryObjectListenerModule | 24 |
| 2702353412 | CompositeStructureInfoModuleData | 24 |
| 2756454809 | W3DTruckDrawModuleData | 396 |
| 3028557483 | TunnelContainModuleData | 184 |
| 3135487101 | GarrisonContainModuleData | 160 |
| 3247734492 | SlaughterHordeContainModuleData | 324 |
| 3432253488 | SpiderHoleUpdateModuleData | 128 |
| 3620482613 | OpenContainModuleData | 148 |
| 3834721291 | HealContainModuleData | 180 |
| 3972432445 | ShipSlowDeathBehaviorModuleData | 116 |

后续以完整依赖族推进：Contain家族、SlowDeath/Destruction家族、W3D复杂Draw家族及剩余对象交互。先精确原方法/PE及父层闭包，再成组实现和完整差分；不能按类型名或native大小猜父类。正式GameObject、301类型XmlCompiler完整替代、真实SDK/MOD集成仍是独立门槛；本清单不能用作整体项目完成率。
