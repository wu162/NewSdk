# AI Update八模块：恢复前交接记录

**已完成：最终源码c7788592d20f1c4fbdc63f422b647595b840253d / run40077851777 success。当前实现、未知词bit4纠正及完整验证见GAMEOBJECT_AI_UPDATE.md和RESUME.md。以下保留恢复前调查，不代表当前拒绝范围。**

原证据已导出：源码bae88f824dd018ba5a46ba390ab5f095cc046d07 / run40072829359 success。recovery/evidence/gameobject-modules/ai-update-source共96个精确方法及enum-source.json；固定原Tokenizer blob6e40a9ed6c9f961d294d49768d911e6a6f354619，按manifest.signature/bodySha定位，不沿用旧文件编号。该成功只是原方法导出，不是组件/SDK支持。

| 完整组 | TypeId | native |
|---|---:|---:|
| AIUpdate | 148619278 | 76 |
| JetAIUpdate | 1149685576 | 100 |
| HordeAIUpdate | 1440184067 | 76 |
| SupplyTruckAIUpdate | 2595752269 | 116 |
| TransportAIUpdate | 3197949694 | 76 |
| AnimalAIUpdate | 3444991387 | 116 |
| WanderAIUpdate | 3625168682 | 144 |
| DeployStyleAIUpdate | 3996180436 | 92 |

八种TypeId仍在186个已知Marshal拒绝清单内，待完整实现及最终矩阵成功后才启用/计数。ConstructionYardAIUpdate的8字节公共实现此前已支持，不重复计数；Horde/Transport虽复用完整AIUpdate76字节Marshal，其Token仍有独立父层，不能降格为8字节base-only或省掉Token层。

主AIUpdate字段从实际Marshal003/Token等核对：StateMachine、AttackPriority、AutoAcquireEnemiesWhenIdle、StopChaseDistance、StandGround、Min/MaxCowerTime、BurningDeathTime、ComboLocoAttackDistance、SpecialContactPoints、FadeOnPortals、IdleTargetTime、MaxCombineChildren、SpawnOffsetRadius、DefaultLocomotorSet，以及nullable UnitAITargetChooserData。**Chooser指针@68、StandGround bool@72**（以实际方法为准，不能看native76猜最后word是指针）。

UnitAITargetChooserData包装Marshal076内联完整40字节：先TargetingCompareList强引用@36，默认DefaultTargetingCompareList，再BaseAITargetChooserData36；区别于TurretChooser44。Token051需同样恢复父层与sentinel。Base及ObjectVisionRangeItem/ObjectFilter与已验收WeaponSet共享原方法：先核对该组manifest方法bodySHA等价，再复用对应完整Marshal/Token/endian实现，不能因已有同名类型就假设所有调用相同。现有MarshalWeaponSetNode索引8/9与GameObjectWeaponSet.h可作实现复用入口，仍在GameObject.cpp内部。

派生组：Jet含ParkingOffset及OutOfAmmoEvaEvent等；SupplyTruck含仓库/采集延迟、范围、树木/矿石/路径行为；Animal含Flee/Wander半径/距离/比例/计时与AfraidOfCastles；Wander在完整AIUpdate上有AttackAll/EntryCondition/Selectable/WanderDistance；DeployStyle含Pack/Unpack时间、炮塔收回/展开条件和DeployedAttributeModifierName。所有字段类型、缺省、nullable/inline、列表、资源顺序、枚举及Endian均须逐原方法确认，以上不是手写字段规范。

下一步：先读本文件及对应manifest，读取完整Marshal/Token/endian与未知helper，核对原PE枚举；按完整八类型及全部必要依赖一次恢复，测试随代码准备。覆盖六容器、全字段与枚举/fallback、缺失/显式空、强引用/typed ID、nullable字符串/Chooser、原BaseChooser全部VisionRange/Filter依赖、继承Token/sentinel、拒绝与XML次序不变性；统一完整GameObject /Od、/O2四平台0/1/2/99和三缓冲区逐字节差分、适用共享回归。及时普通推送未验证检查点，不把取证当实现；SDK继续按大里程碑集中验证，原路由保留。

本轮收尾补充：weapon-state-source与ai-update-source的BaseChooser、ObjectVisionRangeItem、ObjectFilter共8个同签名Marshal/Token/endian方法bodySHA已逐项比对相同。可复用这部分已验收实现；新UnitChooser包装、各AI具体字段和派生Token仍须独立恢复/验证。

