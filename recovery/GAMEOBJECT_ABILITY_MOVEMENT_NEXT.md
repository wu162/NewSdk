# 完整SpecialAbility移动、传送、弹出乘客

2026-10-25已完整实现并通过组件矩阵；共同最终验收见RESUME.md。原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；`evidence/gameobject-modules/ability-movement-next-source/manifest.json`归档32个完整方法（四类全部Marshal/Endian/Token及多态包装、直接装饰helper闭包和Vector3三方法）。`ExportAbilityMovementMethods.py --check`从固定原源码重取逐项核对。父合同复用完整SpecialAbility240，四类都不是SpecialPower452子类；不能据TeleportToCaster的类型名字选择父类。

| 类型 | TypeId | native | 自身Token有效数 |
|---|---:|---:|---:|
| SwitchLocomotorsSpecialAbilityUpdateModuleData | 350237090 | 260 | 8 |
| EjectPassengersSpecialAbilityUpdateModuleData | 887477685 | 280 | 11 |
| TeleportSpecialAbilityUpdateModuleData | 1313225606 | 264 | 7 |
| TeleportToCasterSpecialPowerModuleData | 2474849635 | 272 | 10 |

四类各有完整SpecialAbility父Token层，表末额外sentinel；父45属性、两inline223状态、两String8和nullable Anim12必须保留。以下是原方法直接核对的导航，后续仍要核对全部方法/生成器与完整矩阵。

SwitchLocomotors完整7属性：UseUpgradedLocomotor bool256无默认（缺省零）；TriggersLanding bool257 false；TriggersFlight bool258 false；BusyForDuration Time240默认0s；LandingRange float244默认0；HordeMembersSpecialPowerTemplate强引用248；LocomotorToSet单枚举252无默认（缺省0，不是INVALID=-1）。尾padding259。枚举复用已从原PE核验的LocomotorSet19/未知12。自身先Marshal再完整父。

EjectPassengers完整9属性+1子节点：TimePerPassenger Time240默认1s；Speed float244默认10.0；Radius float248默认20.0；Height float252默认100.0；ApexHeight float256默认0.0；EjectionFX强FX引用260；EjectionBoneName String8@264；RunOffMapWhenDone bool276 false；IgnoreTargetLocation bool277 false；EjectionBoneOffset是**nullable Vector3指针272**，不是inline向量。尾padding278–279。自身骨骼文本先分配，再nullable Vector3@272的12字节，最后父两String8和Anim12。原Token对Vector3字段只Bookmark到pointee，不建立Vector3子表；仍需读归档完整Token和Endian确认。自身Vector3节点x/y/z完整解析复用既有合同，但Bridge支持/重复节点拒绝需新增。

Teleport完整6属性：BusyDuration Time240默认0s；DestinationWeapon强Weapon244；SourceWeapon强Weapon248；MaxDistance float252默认0；HeightToTeleportTo float256默认0；FadeInTime Time260默认0s。自身先于父Marshal，强引用Endian由Tracker处理。

TeleportToCaster完整9属性：Radius/MinDestinationRadius/MaxDestinationRadius/MaxRangeFromCasterSP/MaxRangeFromCasterMP为float240/244/248/252/256，均默认0；TriggerFX强FX260；TargetFX强FX264；RequireStoredObjects bool268 false；KillObjectsIfPlacedInBadSpot bool269 false；padding270–271。父仍是SpecialAbility240。

已整组实现，完整验证保留/Od、/O2、平台0/1/2/99、三缓冲区逐字节、完整旧回归、独立默认/父层/nullable向量/字符串/引用顺序/padding、拒绝与不变性。原始档案用于核验来源；Native字段/Token/运行时和Bridge已启用四类，SDK正式集成门槛保持。已有helper与枚举无需重复probe。

实现：GenerateAbilityMovementFields.py → GameObjectAbilityMovement.h/Runtime.h；GameObjectAbilityMovementCases.cs覆盖全部自身/父字段、Anim12、所有容器与模块边界、独立默认/padding/Token和字符串/向量分配。LocomotorToSet重用原PE核验LocomotorSet19/未知12，缺省仍为0。四字符String8需含终止符分配8字节，`\\1`强引用输出为2，独立断言已修正；失败run保留，不能充当成功证据。

原始manifest的“仍拒绝”为前轮证据导出时状态，保留以固定来源校验；当前验收源码`22aaf75aa50857f1675d545f7b1ed0684137ae11`、run`40129692157 success`，计数以RESUME.md为准。
