# 剩余对象交互完整组（待Windows累计验收）

本轮新增13类，当前只是实现/测试及源检查完成，未计入验收数。固定原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`，原PE SHA256 `ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603`。`object-control-source/manifest.json`保存166完整原方法、13张原PE表；ExportObjectControlMethods.py核对精确类型闭包及稳定编号，GenerateObjectControlFields.py核对全部literal/default/Token和父层。

| 类型 | TypeId/native | 完整关键合同 |
|---|---|---|
| ToppleUpdateModuleData | 115223906/44 | 两FX强引用8/12、StumpId弱GameObject ID16；InitialVelocityPercent20=20%、InitialAccelPercent24=1%、BounceVelocityPercent28=20%、MinimumToppleSpeed32=.5；五bool36–40=false，41–43 padding |
| SuckUpObjectsUpdateModuleData | 415825860/140 | InitialDelay Time8=.5s、LiftVelocity12=1、DeathHeight16=420、Radius20=10、Duration Time24=10s；ObjectFilter inline112@28 |
| TeleportStoredObjectsUpdateModuleData | 478930205/60 | TriggerFX8/TargetFX12引用；FadeInTime16/InitialDelayTime20/DelayTime24=0s；ScaleFadeInTimeByDistance28=0；Telefrag enum32=NONE0/未知1；SpecialPowerTemplate引用36、Shader可空引用指针40；DisabledTypesToProcess bits12@44=HELD(bit3)；InstaKillObjectFilter/CollidesWithOnTeleport nullable112@48/52；GetObjectsCurrentlyAtStorePosition bool56=false |
| CaptureAndGiveUpgradeBehaviorModuleData | 513142359/32 | UnificationType enum20=SAME_PLAYER0/未知1；UpgradeMap重复结构List8@24，每项8（OwnershipAmount signed0、ProvidedUpgrade强Upgrade引用4）。先自身列表后完整Capture20。**原Token只保留Capture父记录及UpgradeMap，不含UnificationType标量**，不可自行补Token |
| WeaponScatterEffectUpdateModuleData | 1052292566/136 | WeaponTemplate可空引用指针8；ScatterSettings inline8@12：ScatterAmount signed0=0、ScatterDelay Time4=0s；Filter inline112@20；RandomScatter3D bool132=false |
| SwitchLocomotorUpdate | 1323283019/112 | LocomotorSet单枚举8无默认/未知12；UpdatePeriod12=1s、RevealShroudToAllPlayersRadius16=0、MinimumHealth百分比20=0%；ObjectStatusWhileActive inline223@24，ModelConditionWhileActive inline456@52 |
| ConvergingLaserStateModuleData | 1601476857/136 | ConvergingAngle44无默认、AngleRotation48=0d、Radius52=10；SweepFXList56/VeteranSweepFXList60引用、SweepFXTimeout64=0s、SweepWeapon68引用、ModelConditions inline456@72、Lifetime132=1s；随后完整LaserState44（String8、两强粒子列表、nullable Vector3及validation56） |
| SqueezeFlingUpdateModuleData | 1853336153/164 | ScanTime8=.2s、DelayBetweenVictimsTime12=.3s、EffectDistance16=125、GatherAcceleration20=.01、ModelLength24=100；ArmTemplate可空GameObject引用指针28、VictimShader可空引用指针32、HeightOffGround36=50；Filter inline112@40、KillFilter nullable112指针152；ParticleData List8@156，每项20：SourceBone String8@0、DestinationBone String8@8、ParticleSystem引用16 |
| DestructibleEnvironmentUpdateModuleData | 1979616656/40 | Size单枚举8=LIGHT0/未知2；LeftOffset inlineVector3@12、RightOffset inlineVector3@24；Enabled bool36=false。原Token直接指向两份inline值，不添加Vector子表 |
| MagneticEffectUpdateModuleData | 2091086747/152 | ScalableDrawModuleId弱ModuleData ID8；ScaledBoneName String8@12；Radius20=250、MaximumPullVelocity24=20、MinimumPullVelocity28=4；VictimStatusToSet为**单枚举**32=MAGNETIZED216/未知198；ObjectStatusValidation nullable56指针36先于ObjectFilter inline112@40内部数据 |
| SecondaryObjectListenerModule | 2642984081/24 | Flags bits2@8未知bit1；SelfValidation nullable56@12先分配；ObjectEffectTuple List8@16，每项**120**：inline ObjectFilter112@0，UpgradeSelf重复强Upgrade引用List8@112（不是单引用或116字节项） |
| CompositeStructureInfoModuleData | 2702353412/24 | ThingTemplate强GameObject引用8、Count uint12、BuildableDistance float16；ConnectionShadowInfo nullable60指针20，包含全部18字段/7值ShadowType单枚举（未知3）/两强Texture引用及原默认 |
| SpiderHoleUpdateModuleData | 3432253488/128 | Radius8=0、ScanFrequency12=1s；ObjectFilter inline112@16 |

除CaptureAndGiveUpgrade与ConvergingLaser外，Marshal均调用ModuleData；Token父表依原方法，SecondaryListener/Composite使用Behavior，其余Update。Capture完整父层的GrantMoney8/SlavedCaptureObject弱ID12/GrantMoneyOneTime bool16保留；不能按名称猜父类。

ShadowInfo60：Type enum0无默认；Texture4/AdditionalTexture8强引用；SizeX12/SizeY16/OffsetX20/OffsetY24 float0；OpacityStart28=0、OpacityFadeInTime32=0s、OpacityPeak36=1、OpacityFadeOutTime40=0s、OpacityEnd44=0、MaxHeight48=20、SunAngle **float**52=0；OverrideLODVisibility bool56=false、UseHouseColor57=false、IsRotatingWithObject58=true、LocalPlayerOnly59=false。缺省节点null、空节点完整初始化。

测试GameObjectObjectControlCases.cs覆盖全部根/子结构字段、父层、PE枚举及未知回退、全默认/padding/native/Token key/type/offset、结构列表8/20/120步长、强引用/弱ID/nullable/inline过滤器、nested UpgradeSelf列表、字符串对齐和schema分配顺序、0/1/2/27/28/29/65边界及拒绝/不变性。旧/Od/O2、四平台0/1/2/99、三缓冲区完整差分不变。真实SDK未运行、原DLL/正式路由未改；组件成功不能冒充正式集成。

第二次累计矩阵40143597761发现上述四个可空引用布局错误，已按原Marshal/Token修正；缺省指针0、空/非空属性分配4字节、Token指向pointee及引用后缀均独立覆盖。完整复验前不计验收。
