# AI Update 八模块恢复

2026-10-24：完整实现并通过原DLL完整差分，136→144/322已验收。

| 类型 | TypeId | native |
|---|---:|---:|
| AIUpdate | 148619278 | 76 |
| JetAIUpdate | 1149685576 | 100 |
| HordeAIUpdate | 1440184067 | 76 |
| SupplyTruckAIUpdate | 2595752269 | 116 |
| TransportAIUpdate | 3197949694 | 76 |
| AnimalAIUpdate | 3444991387 | 116 |
| WanderAIUpdate | 3625168682 | 144 |
| DeployStyleAIUpdate | 3996180436 | 92 |

原证据为ai-update-source的96个方法及PE枚举，固定原Tokenizer blob6e40a9ed6c9f961d294d49768d911e6a6f354619，导出源码bae88f824dd018ba5a46ba390ab5f095cc046d07 / run40072829359 success。GenerateAIUpdateFields.py核对全部96方法bodySHA，正式Windows检查另核对原源码blob和PE枚举。BaseChooser/ObjectVisionRangeItem/ObjectFilter的8个方法指纹与已验收weapon-state-source一致，复用其完整实现。

AI主体76字节：AttackPriority为内联ReloString@8，SpecialContactPoints为StringList@40；UnitAITargetChooserData是nullable指针@68，StandGround/FadeOnPortals是bool@72/@73。UnitChooser分配40字节，先TargetingCompareList强引用@36（默认DefaultTargetingCompareList），再完整BaseChooser36及ObjectVisionRange/内联ObjectFilter依赖。原AI endian只调用BaseChooser，不交换引用索引；不能用TurretChooser44替换。

Jet/SupplyTruck/Animal/Wander/DeployStyle先Marshal派生字段再AI主体，保留资源引用分配顺序。Animal六个距离/比例字段是signed integer，InitialFleeBlindlyRadius为float；Jet OutOfAmmoDamagePerSecond是百分比；Wander EntryCondition是完整456位模型状态、WanderDistance是uint、bool@140/@141；Deploy的UnpackTime@76先于PackTime@80。Horde和Transport复用完整AI76 Marshal，但分别保留独立Token父层和sentinel。未知StateMachine枚举回退0，LocomotorSet回退12；AutoAcquireEnemies是5位bitset，未知词使用bit4（ATTACK_BUILDINGS），ALL保留全部32位。

GameObjectAIUpdateCases.cs覆盖全部根字段、六容器、枚举/位集/fallback、数字边界、缺失/显式空、强引用及typed ID、字符串与StringList、nullable UnitChooser、BaseChooser全部字段和VisionRange/Filter依赖、重复列表容量边界、派生Token父层/sentinel、独立native/default/offset/分配顺序与拒绝边界；和既有完整矩阵统一运行/Od、/O2及平台0/1/2/99，比较三缓冲区全部字节。

检查点cfe3027e1d91f53ee91a72d242159699fa36e0a1保存未验证生成器。最终源码c7788592d20f1c4fbdc63f422b647595b840253d：GameObject run40077851777 success，基础run40077851766 success。每/Od、/O2：300060 fixtures、900180三缓冲区比较、11896拒绝、808不变性比较，平台0/1/2/99；两模式共1800360次缓冲区比较。validation.json及两模式实际日志/sourceCommit/runId已核对。首轮XML测试容器误用和第二轮AutoAcquire未知词bit4差异已修正，保留原失败证据，未放宽拒绝或比较。

SDK本批0次，原路由保留；组件验证不代表完整DLL或SDK正式插件验收。
