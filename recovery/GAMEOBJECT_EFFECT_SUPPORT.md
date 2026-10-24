# 效果与移动控制完整组：未验收检查点

2026-10-25实现8类及全部直接依赖，**当前未取得最终Windows累计矩阵成功，不能计入已验收数**。原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；PE SHA256 `ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603`。`support-source/manifest.json`保存140完整原方法及14张原PE表；ExportSupportMethods.py精确类型闭包、稳定归档编号；GenerateSupportFields.py核对完整literal/default/Token/父层和原PE。

| 类型 | TypeId/native | 完整合同 |
|---|---|---|
| LocomotorSetModuleData | 664395762/40 | KillLocomotorUpgrade bool36=false；完整Upgrade36父层含nullable Anim12及两份弱Upgrade ID列表 |
| SetLocomotorSpeedUpdateModuleData | 793065128/40 | Type enum8=INVALID，未知0；LocomotorSpeedToSet12、LocomotorDeltaPerUpdate16、OtherScaredScanRangeMultiplier20、ObjectFilterRange24均float0；UpdatePeriod Time28=1s、VisionArc Angle32=180d；ObjectFilter nullable112指针36；Update父Token |
| RestrictSpecialPowerBehaviorModuleData | 2503769947/28 | RestrictionType enum8无默认、未知1；DependentObject弱GameObject ID12；ConsiderSpecialPowerRadius bool24=true；DependentObjectRadius为重复文本子节点的float List8@16，非属性也非inline范围；Behavior父Token |
| PassiveAreaEffectBehaviorModuleData | 149050769/76 | EffectRadius8=0、MaxBeneficiaries uint12=0、HealingPercentPerSecond16百分比=0、HealingPointsPerSecond20=0、SurveyDelayFrames uint24=20；UpgradeRequired引用28；AntiCategoryMask bits15@32、AntiFX引用36、HealFX引用40、DisabledTypesToProcess bits12@44；Modifier重复强AttributeModifier引用List8@48；AllowFilter nullable112@56、ContainerFilter nullable112@60、ObjectStatusValidation nullable56@64；NonStackable bool68=false，AffectAttached69/AffectWoundedOnly70/AffectUnderAttack71/AffectContained72均false；Update父Token |
| AttributeModifierAuraUpdateModuleData | 1578938331/104 | AttributeModifierName引用36、RefreshDelay Time40、Range float44；RequiredConditions bits1@48、AntiCategory bits15@52、AntiFX引用56、MaxActiveRank signed60、RequiredObjectStatusFlags inline223@64；ObjectFilter nullable112@92；AllowPowerWhenAttacking bool96=true、InitiallyActive97=false；RunWhileDead98/AllowSelf99/AffectContainedOnly100/AffectHordeMembersOnly101/AffectAttacheeOnly102无默认，ShowDebugCircle103=false；随后完整Upgrade36父层 |
| AutoAbilityBehaviorModuleData | 1285783358/68 | MaxScanRange8=1、MinScanRange12=0、Radius16=0、IdleSeconds Time20=0s；SpecialAbilityName强SpecialPowerTemplate引用24；ForbiddenStatus inline223@28；Queries重复结构List8@56，每项Query12：EMax signed0=6、Value signed4=-1、ObjectFilter nullable112指针8；StartsActive bool64/UseStartPosition65/AdjustAttackPos66/AllowSelf67均false；Update父Token |
| EmotionTrackerUpdateModuleData | 2842040686/56 | TauntAndPointDistance8=0、HeroScanDistance12=0、TauntAndPointUpdateDelay Time16=2s、FearScanDistance20=0、ImmuneToFearLevel signed24=5；TauntAndPointExcluded/AfraidOf/AlwaysAfraidOf/PointAt四份nullable112指针28/32/36/40，依此顺序分配；AddEmotion重复强EmotionTemplate引用List8@44；IgnoreVeterancy bool52=false；Update父Token |
| FXListBehaviorModuleData | 2960202212/60 | Event重复结构List8@52，每项24；随后完整Die52父层和DieMux44依赖。Event：Index单枚举0无默认，未知7；FX强引用4，MinThreshold float8=0，MaxThreshold float12=0，Direction单枚举16=Default4/未知4，OrientToObject bool20=true，ForceUseDoFXObj21=false，22–23 padding零 |

分配顺序严格匹配原方法：Passive强Modifier列表→AllowFilter含内部数据→ContainerFilter含内部数据→validation56；Aura自身Filter→完整Upgrade；Auto Query12总列表→每项Filter含内部列表；Emotion四份Filter→强Emotion列表；FX Event24总列表→完整DieMux。所有引用、位集/单枚举、时间/角度/百分比、nullable节点与空节点分别处理。AttributeModifierCategoryType15位未知bit7；Aura Required1位未知bit0；Disabled12位未知bit0。FX事件Index14个原literal和Direction5个literal保留原大小写。

实现：GameObjectSupport.h、GameObjectSupportRuntime.h及模块分派/Bridge支持。测试：GameObjectSupportCases.cs，保留全部既有测试；覆盖所有根属性、原PE literal、父层、默认/padding、Token、结构列表/强引用/float列表、过滤器/掩码/validation与分配顺序、跨0/1/2/27/28/29/65边界、未知/重复/外来命名空间拒绝。新组仍须最终/Od、/O2、平台0/1/2/99和三缓冲区完整差分成功；本地源检查不等于Windows成功。

本轮未运行真实SDK编译，原DLL和正式插件路由未变；不能把此检查点当作正式SDK支持。续接先核对RESUME.md中的准确sourceCommit/runId和运行终态，失败须修正后取得完整矩阵，不强推、不覆盖并发证据。
