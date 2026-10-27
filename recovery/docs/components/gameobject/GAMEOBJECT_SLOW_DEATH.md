# 完整SlowDeath与建筑销毁组（已通过Windows验收）

固定Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`、原PE SHA256 `ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603`。slow-death-source保存130完整原方法及5张PE表，ExportSlowDeathMethods.py/GenerateSlowDeathFields.py固定全文指纹、枚举、literal/default/Token及关键列表步长；本地检查不能代替Windows验收。

| 类型 | TypeId/native | 完整能力 |
|---|---|---|
| SlowDeathBehaviorModuleData | 2557908884/116 | SinkRate8为FrameSpeed（float经原逻辑帧秒数double乘再转float），ProbabilityModifier signed12=10、ModifierBonusPerOverkillPercent16百分比=0；SinkDelay20/Variance24/DestructionDelay28/Variance32/DecayBeginTime36均Time默认0s；FlingForce40/Variance44=0，FlingPitch48/Variance52为Angle无默认；DeathFlags nullable60指针56、DeathTypes **同为nullable模型456位/60字节**指针68，DeathObjectStatusBits nullable223位/28字节指针72；ShadowWhenDead bool112=false、Fade113=false、FadeTime60/FadeDelay64=0s；四份phase列表FX76/OCL84/Weapon92/Sound100各List8；DieMuxData nullable44指针108最后分配；Update父Token |
| ClearanceTestingSlowDeathBehaviorModuleData | 245525561/140 | ClearanceMaxHeight116/MaxHeightFraction120/MinHeight124/MinHeightFraction128 float0；ClearanceGeometry nullable48指针132及全部依赖，ClearanceGeometryOffset nullableVector12指针136，均先于完整SlowDeath116父层 |
| HelicopterSlowDeathBehaviorModuleData | 7192255/208 | 自身全部22字段：float116/120/124(default1)/128/132，Time136，float140/144，Time148/152，BladeBone String8@156，OCL/FX引用164..188，DelayFromGroundToFinalDeath **float**192、FinalRubbleObject弱ID196、MaxBraking float200=99999.9、DeathSound强音频引用204。自身字符串先于完整SlowDeath父层 |
| ShipSlowDeathBehaviorModuleData | 3972432445/116 | Marshal/Endian直接使用完整SlowDeath116；**原Token wrapper有独立1条SlowDeath父记录和sentinel**，保留自身TypeId |
| BuildingDestructionModuleData | 227975924/72 | AssociatedModule弱ModuleData ID52、MaxDebrisCloudsPerSecond float56=2、FillerMesh弱SubObject ID60=NEWSKIN_FILL（原默认hash0x82df7106）；Item List8@64，每项20：BonePrefix String8@0、FX引用8、CreationObject弱GameObject ID12、FlingVelocity float16=1；自身Item列表先于完整Die52父层 |

phase每项12：Type单枚举0，未知FINAL2；FX/OCL/Weapon内含同名重复文本子节点的强引用List8@4。Sound用**List属性**分词成ReloString8列表@4。四种项Token均有SlowDeathBaseType父层，再指向内层列表；Sound内层Token步长8，强引用步长4。枚举INITIAL0/MIDPOINT1/FINAL2/HIT_GROUND3。OCL/Weapon/Sound的SizedList4原Marshal包装器仅转调普通列表，**不强加4项限制**；测试保留0/1/2/3/4/5/27/28/29/65边界。

GeometryInfo48与GameObject根的raw Geometry28是不同合同，不可复用猜测布局：IsSmall bool44=false，BoundingCircleRadius0/BoundingSphereRadius4/XWidth8/YDepth12 float0；Shapes结构List8@16每项32；RotationAnchorOffset nullableVector2指针24，Center/HighestContactPoint/InnermostContactPoint nullableVector3指针28/32/36；ContactPoints是**单个nullable ContactPoint20**指针40。分配顺序为Shapes及每项Name/Offset→四向量→ContactPoints及Label。Shape32：Type单枚举0（未知BOX1）、Height4/MajorRadius8/MinorRadius12=0、Name String8@16、Offset nullableVec12指针24、BActive bool28=false。Contact20：Label String8@0、Pos inlineVec3@8。向量Token直接指向值、不添加子表。

主实现GameObjectSlowDeath.h/GameObjectSlowDeathRuntime.h；GameObjectSlowDeathCases.cs覆盖全部父层/根属性/子结构字段、原PE literal与未知回退、默认/native/padding/Token、phase内外列表/字符串/强引用/nullable模型状态/几何多层分配顺序及不变性和非法输入。完整/Od/O2、平台0/1/2/99、旧回归及三缓冲区逐字节比较保留。SDK未运行，原DLL和正式路由未改。

前一对象交互run40142578135失败来自独立Topple百分比默认期望：原版与恢复版一致，`20.0f * 0.01f`为0x3e4ccccc，不能用字面量0.2f的0x3e4ccccd；已固定精确IEEE值，不改恢复逻辑、不弱化差分，合并本整组后统一完整复验。

最终验收source9c138e8f182c4dac7c2732c42133e0a789925c01，run40145009538 success；/Od、/O2各1212528 fixtures、3637584三缓冲区比较、23056拒绝、3976不变性，平台0/1/2/99。基础40145009445 success。原失败记录保留；SDK0次。
