# 完整区域伤害与目标选择组（已通过完整Windows矩阵）

原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`，PE SHA256 `ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603`。`area-source/manifest.json`保存94个完整原方法及12张PE表；ExportAreaMethods.py按精确参数类型递归直接依赖，保持既有归档编号、不受set遍历顺序影响；GenerateAreaFields.py检查完整literal/default/Token/父层和原PE。

| 类型 | TypeId/native | 完整能力与关键布局 |
|---|---|---|
| LineSegmentDamageUpdateModuleData | 14531459/88 | NodeLifetime Time8、DamageInterval Time12、Distance float16、Damage float20；DamageType enum24=UNDEFINED14、DeathType enum28=NORMAL1、DamageFXType enum32=UNDEFINED112；ObjectStatusForbiddenForDamage inline223@36；MinDistanceTolerance64=10、MaxValidHeightOffGround68=0；FXParticleSystemTemplate引用72、NodeFXList引用76；ObjectFilter nullable112指针80、ObjectStatusValidation nullable56指针84；Update父Token |
| StrafeAreaUpdateModuleData | 163451004/56 | WeaponName引用8、SecondaryWeaponName nullable引用4指针12；Radius16/MinRadius20/PreattackDistance24/SweepFrequency28/SweepAmplitude32/DivingFloor36/InitialSweepPhase40/TargetJitterOffset44均float默认0；ShotsToFirePerFrame uint48=1；Options bits4@52；Update父Token |
| BombardAreaUpdateModuleData | 1230044432/44 | UsePlayerStore bool40=false；IncomingAngle8=0d、WeaponName引用12、MaxShots signed16=1；ObjectStatusValidation nullable56指针20；TimeDelayRange inline8@24、RadiusSpreadRange inline8@32，各Low/High@0/4无默认；41–43 padding；Update父Token |
| StrafeAreaTargetChooserModuleData | 3599948941/24 | Radius float8=0、MaxAttacksPerTarget signed12=-1、OverflowOptions bits2@16；ObjectFilter nullable112指针20；**Behavior父Token** |
| CircularDamageUpdateModuleData | 2191489142/144 | Flags bits1@8；MaxRadius float12=100、ArcDistance float16=10；SweepFXList引用20、SweepWeapon引用24、FXTimeout Time28=.25s；ObjectFilter **inline112@32**；Update父Token |
| DamageFieldUpdateModuleData | 4088922837/44 | Radius **signed**32=0、RequiredUpgrade强引用36；ObjectFilter nullable112指针40先分配；随后完整FireWeaponUpdate32父层（FireInterval/AliveOnly/ActiveWhenDisabled、FireWeaponNugget16列表与nullable Vector3@8、ObjectStatusValidation nullable56@24）；不能仅支持半个父层 |

前五类Marshal最后调用ModuleData，但Token父表如上；DamageField则Marshal/Endian/Token均复用完整FireWeaponUpdate32。nullable Filter和内部Status28/弱ID列表先于后续验证结构或父层分配。Circular的Filter缺省保持零、空节点运行默认；Bombard两个range节点缺省或空节点均保零且不分配，节点顺序不影响schema顺序。

精确未知回退：DamageType单枚举38，DeathType33，DamageFXType35（不能误用UNDEFINED112）；StrafeAreaUpdateFlagType未知bit3，TargetChooser未知bit1，Circular未知bit0。所有literal从原PE导出；完整ALL/负号XOR语义保留。TypeId及资源仍要求适用BAB预处理后的输入。

GameObjectAreaCases.cs覆盖全部根字段、全部原PE literal及未知边界、完整父层属性/列表/向量、过滤器KindOf/Status及列表跨0/1/2/27/28/29/65边界、validation56和inline范围、独立size/default/padding/Token keys/types/offsets及分配顺序、强引用与nullable引用值、非法属性/节点/重复/外来命名空间及顺序不变性。原全部回归、/Od/O2、平台0/1/2/99、三缓冲区逐字节对照不变。SDK未运行、原DLL与插件路由不变；组件通过不冒充正式集成。

最终组件验收source3c5c40a4bd95ff62657d4e0776afb4c94ac30a6d/run40140521059 success；/Od、/O2各977064 fixtures、2931192三缓冲区比较、20432拒绝、2752不变性，四平台0/1/2/99；两模式原始日志及作业步骤已核对。SDK未运行。
