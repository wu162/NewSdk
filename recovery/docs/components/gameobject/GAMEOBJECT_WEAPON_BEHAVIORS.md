# 直接开火与伤害跟踪组（已验收）

已验收源码b57e5a5a1d11ebd3004a6f0b6f0af4bb15c86057，GameObject run40050646715 success，基础run40050646652 success。每/Od、/O2：192100 fixtures、576300三缓冲区比较、10064拒绝、400不变性，平台0/1/2/99。SDK0次。

本批新增七种：ChargeWeaponUpdate、FireWeaponUpdate、AimWeaponBehavior、LaserScatterUpdate、LinearDamageUpdate、SecondaryDamageTracker、TrackDamage。LaserUpdate此前属于已完成ClientUpdate，本批只核对原证据/回归，不重复计数。固定原blob6e40a9ed6c9f961d294d49768d911e6a6f354619。85方法精确签名/行号/SHA256见evidence/gameobject-modules/weapon-behavior-source/manifest.json；GenerateWeaponBehaviorFields.py核对原blob、逐方法、PE枚举、全部Marshal次序/default和递归Token/sentinel，接入TestGameObject.ps1。运行支持须取得本批源码的完整成功结果。

| 模块 | TypeId/native | 完整自身输入 |
|---|---|---|
| ChargeWeaponUpdate | 602268606/24 | ChargeTime Time@8默认1.0s；OverChargeTime Time@12默认0.0s；OverChargePercent percent@16默认0%；可空56字节ObjectStatusValidation@20 |
| FireWeaponUpdate | 2995535186/32 | FireInterval Time@8无fallback；AliveOnly bool@28默认false；ActiveWhenDisabled 12bit@12；16字节FireWeaponNugget列表@16；可空validation@24 |
| AimWeaponBehavior | 3828176408/24 | AimLowThreshold/AimHighThreshold/AimNearDistance/AimFarDistance四普通float@8/12/16/20，默认-0.4/0.4/0/0 |
| LaserScatterUpdate | 4198440042/16 | LaserLifetime Time@8默认1s；LaserHitGroundWidthMultiplier percent@12默认10% |
| LinearDamageUpdate | 509375610/40 | StartDistanceOffset float@8默认0；Radius float@12默认10.0；Length float@16无fallback；SweepFXList强引用@20；SweepWeapon强引用@24；Speed@28默认1.0，按逻辑帧时长换算；FollowTerrain bool@36默认true；可空validation@32 |
| SecondaryDamageTracker | 2988211815/36 | UpdateDuration Time@8默认1s；DissipateAmount uint@12默认0；MaxDamageClampScaler percent@16默认125%；可空RGBColor@20；16字节EffectInfo列表@24；可空validation@32 |
| TrackDamage | 3579087005/16 | TimeToTrack Time@8无fallback；InstantWeaponsOnly bool@12默认false |

## 关键依赖

- FireWeaponNugget：WeaponName强引用@0；FireDelay Time@4；OneShot bool@12默认false；Offset可空Vector3指针@8。先分配整块16*N列表，再依列表顺序分配各Vector3，最后顶层validation。Token的Offset直接bookmark Vector3数据，不再生成Vector3子表。
- EffectInfoType：Fraction普通float@0（不是百分比）；AttributeModifier强引用@4；OnEnterDisable、OnEnterCancelSpecialAbilities、SetUpgradedArmor bool@12/13/14默认false；Options一位EffectInfoFlagType@8默认空，DIE_ON_ENTER/未知均bit0。Token次序按原始定义：Fraction、引用、三个bool、Options，保留字节15padding。
- RGBColor：R/G/B三个普通float@0/4/8，无归一化、无缺省fallback。Secondary先Color，再EffectInfo列表，最后validation；与XML顺序无关。
- ObjectStatusValidation复用56字节：ForbiddenStatus@0、RequiredStatus@28；缺失null，显式空分配全零；Token是两个完整ObjectStatus位集。
- 原常量证据constants-source.json：LOGICFRAMES_PER_SECONDS_REAL原字段data(00 00 A0 40)=5f；SECONDS_PER_LOGICFRAME_REAL初始化为float(1.0/double(5f))。Speed先atof转float，再double(float(0.2))*double(value)转float，不能替换为任意不同精度的除法。
- 12位DisabledType和1位EffectInfoFlagType由原PE取名值；未知位均0；ALL和原XOR删除语义继续保留。全原GameObject回归包含既有位集使用者。

## 工具与验证

ExportLifecycleMethods.py原来只匹配无点号类型，现扩展到带点号的FireWeaponUpdateModuleData.FireWeaponNugget，补齐嵌套Marshal/Token及Vector3依赖。源码调查最初11模块触发150依赖上限，保留失败run40049720082；收敛到直接开火/跟踪组后导出成功，未放宽保护限制。

组件差分统一/Od、/O2和平台0/1/2/99，三缓冲区逐字节、独立默认/位集/速度/列表/指针/Token/sentinel、六容器、0/1/2/27/28/29/65列表和模块、引用与浮点边界、命名空间/重复节点/未知输入拒绝均保留。SDK本批0次，原路由未变；组件成功不代表正式插件或完整替代DLL完成。
