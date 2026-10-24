# 下一完整能力组：SpecialAbility武器/光束/状态（仍拒绝）

只取证，未实现、未计数。`ExportAbilityWeaponMethods.py --check`核对固定原Tokenizer blob6e40a9ed6c9f961d294d49768d911e6a6f354619、32个精确原方法及直接装饰名helper闭包，见`evidence/gameobject-modules/ability-weapon-next-source/manifest.json`。父合同复用已恢复完整SpecialAbility240，不能继承SpecialPower452或SpecialPowerUpdate24。

| 类型 | TypeId | native | 完整继承 |
|---|---:|---:|---|
| WeaponFireSpecialAbilityUpdateModuleData | 507880339 | 272 | SpecialAbility240 |
| DelayedWeaponFireSpecialAbilityUpdateModuleData | 915350037 | 276 | WeaponFire272→SpecialAbility240 |
| BeamSpecialAbilityUpdateModuleData | 2695497697 | 272 | SpecialAbility240 |
| ObjectStatusSpecialAbilityUpdateModuleData | 127626925 | 296 | SpecialAbility240 |

四种均仍在97已知拒绝集合，不因父类完成自动启用。以下偏移用于导航，仍须核对完整原Marshal/Endian/Token与32方法指纹。

WeaponFire全部11属性：SpecialWeapon强Weapon引用@240；WhichSpecialWeapon signed@244默认0；SkipContinue bool@264 false；BusyForDuration Time@248默认0s；NeedLivingTargets@265/PlayWeaponPreFireFX@266/OffsetTargetLocationFromObject@267 bool false；WeaponSlotID signed@252默认1；WeaponSlotType单枚举@256默认PRIMARY_WEAPON；DestealthClosestObjectMatchingFilter是bool@268默认false，不是Filter；ModelConditionsOnUnpack单模型枚举@260默认INVALID=-1，不是位集。padding269–271。Delayed先FireDelay Time@272默认0s，再完整WeaponFire与完整SpecialAbility父。

Beam全部10属性：DamagePerSecond float@240无默认；JoinWithOtherBeams bool@268 false；原字段精确拼写**DissapateWhenTargetDead** bool@269 true；LineType bool@270 false；TargetAttributeModifierAdd强引用@244；TargetObjectStatusAdd nullable完整223位status28指针@248；SweepFX强FX引用@252；ReflectorExtendDistance float@256默认0；TargetSamePlayerOnly bool@271 false；PreferredTargetBone String8@260。自身nullable status与骨骼String先分配，再父的两个String和nullable Anim12；必须保留schema顺序和大小写，不能据字段名猜语义。

ObjectStatus：Status1/Status2分别为完整inline223位掩码28@240/@268，非单枚举。两字段之后Marshal完整父。新四类均继承父45属性与CustomAnim12，Token父层完整保留；Delayed必须有额外WeaponFire父层。原根Token有效条数分别12/2/11/3，均加sentinel。

无需为已有表重复probe：WeaponSlotType完整6名称/值和未知5在GameObjectWeaponSet.h的weaponSetWeaponSlotTypeEnum，GenerateWeaponSetFields默认从原PE核对；单枚举和6位mask语义要区分。ModelCondition457、ObjectStatus223和Anim12均复用已验证完整合同。新能力组仍需/Od、/O2、平台0/1/2/99、三缓冲区逐字节、完整旧回归、独立默认/Token/父层/引用与nullable/字符串分配/padding及拒绝/不变性。SDK仍按大里程碑，当前不改原路由。
