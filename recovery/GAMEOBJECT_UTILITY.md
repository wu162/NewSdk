# 完整13类辅助模块组（待Windows验收）

固定原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`。GenerateUtilityFields.py检查完整Marshal/Endian/Token和三个多态dispatcher helper；没有未覆盖子节点或可空依赖。

| 类型 | TypeId | native | 完整字段 | Token父表 |
|---|---:|---:|---|---|
| StorePurchasedUpgradeBehaviorModuleData | 783731100 | 12 | MaxPlayerUpgrades uint@8，默认9999；strtoul base0 | Behavior2769121946 |
| AIGateUpdateModuleData | 880602396 | 16 | TriggerWidthX float@8、TriggerWidthY float@12，默认0 | Update4043318533 |
| CombinedInfoModuleData | 2018429368 | 16 | DockingBone String8@8，无默认；节点属性缺省不分配，空属性分配4字节 | Create816016269 |
| TimedOpacityUpdateModuleData | 4216826759 | 16 | Opacity float@8默认0.5，Duration Time@12默认5s | Update4043318533 |

四类Marshal末尾均调用ModuleData；Endian直接交换自身字段、TypeId、id；Token必须保留以上精确父层。String长度包含字符数、不含终止符，分配包含终止符并四字节对齐。均拒绝未知属性、所有子节点、结构文本和外来命名空间；通用TypeId/id契约复用。

GameObjectUtilityCases.cs覆盖全部属性、数值边界/时间后缀/非ASCII字符串、独立默认值/布局/Token key及type/分配及padding、容器与列表0/1/2/27/28/29/65边界及非法输入。累计矩阵保留/Od/O2、平台0/1/2/99、全部旧回归、三缓冲区比较；组件验收与正式SDK集成分开。

首批四类已通过source1e044cd8/run40139405380的/Od/O2完整矩阵（每模式952632 fixtures、2857896三缓冲区比较），计入260/322。后续九类当前待最终累计验收，不提前计数。

## 后续九类（待最终完整验收）

| 类型 | TypeId/native | 完整自身合同 |
|---|---|---|
| W3DPropDrawModuleData | 344630372/12 | Model强RenderAsset引用8；Draw父Token |
| AttributeModifierNavalModuleData | 2739727708/12 | AttributeModifier强引用8；Create父Token |
| ReplaceDrawModuleBehaviorModuleData | 3032642002/12 | ConsumeItem bool8=true；Behavior父Token；9–11 padding |
| StancesBehaviorModuleData | 3231835197/12 | StanceTemplate强引用8；Update父Token |
| SquishCollideModuleData | 3052093933/20 | CrushAnimTime8=1s、CrushKillDelay12=.5s；UseDirectionCheck bool16=true，UseVehicleStyleCrush17=false、AllowAllyCrush18=false；Collide父Token |
| AreaRestrictSpecialPowerBehaviorModuleData | 1206381257/20 | RestrictionType enum8无默认、未知1；AreaName String8@12；Behavior父Token |
| MinefieldBehaviorModuleData | 3040483501/28 | DetonationWeapon引用8、DetonatedBy bits4@12未知bit0、DetonationFX引用16、NumVirtualMines uint20均无默认；RepeatDetonateMoveThresh float24=5；Update父Token |
| LiftObjectUpdateModuleData | 3214000077/40 | CrusherModifiesVelocity bool36；LiftVelocity8、MaxElevationFromGround12；TimeIncrement16=0s、MaxTimeLifted20=1s；MaxStructureShakeVelocity24=0；AirplaneCrashWeapon引用28、SoftLandingWeapon引用32；Update父Token |
| ClusterBombUpdateModuleData | 3833989627/36 | WeaponName引用8、InitialWeaponName nullable引用4指针12；Radius16=0、NumBomblets signed20=0、MinDelay24=0s、MaxDelay28=0s、BombletFX引用32；Update父Token |

四种12字节类的Marshal字段内联在多态wrapper中，生成器核对原完整wrapper及Token/Endian；不因没有独立Marshal方法而遗漏自身字段。SpecialPowerRestrictionType精确literal为UNRESTRICTED=0、1=1、2=2，单枚举未知1。ObjectFilterRelationship仅ALLIES0/ENEMIES1/NEUTRAL2/SAME_PLAYER3，未知bit0。生成器从固定原PE核对表，不能按类名推断父类。13类均有独立完整默认/padding/Token keys/types/offsets及nullable引用/字符串分配检查。
