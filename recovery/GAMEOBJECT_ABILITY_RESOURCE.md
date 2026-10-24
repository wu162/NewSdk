# 完整资源偷取与创建进入对象派生

2026-10-25。已完整验收：源码`22aaf75aa50857f1675d545f7b1ed0684137ae11`、run`40129692157 success`。最终共同组件验收见RESUME.md；SDK未运行，原路由保留。源码不能因类型名相似混用父类。

| 类型 | TypeId | native | 完整父类 |
|---|---:|---:|---|
| TiberiumThiefSpecialAbilityUpdateModuleData | 4099311701 | 344 | SpecialAbility240 |
| CreateAndEnterObjectSpecialPowerModuleData | 952407768 | 504 | OCLSpecialPower500 → SpecialPower452 → Behavior8 |

Theft自身六属性：TiberiumStolenPerUpdate signed240默认1；DelayBetweenThefts Time244默认0s；TiberiumCapacity signed248默认1；ObjectStatusWhenNotEmpty inline223/28字节252默认空；ModelConditionWhenNotEmpty inline456/60字节280无默认；PlayWeaponPreFireFX bool340默认false，padding341–343。两个掩码共22个word，不能当成单枚举/nullable。自身无额外分配，然后完整SpecialAbility两String8和nullable Anim12。

CreateAndEnter只有FXOffset子节点，nullable Vector3指针500，分配12字节；x/y/z默认0。**该向量先于全部OCL父层分配和引用处理**：向量→UpgradeOCL列表→NearestSecondaryObjectFilter及内部typed列表→Upgrade强引用列表→SpecialPower父StringList/三个inline Filter/nullable Sound。Token是完整OCL500父表+FXOffset值bookmark，不创建Vector3子表；Endian向量三个float后完整OCL父。完整继承OCL全部属性、升级/OCL列表、四个Filter及声音等父结构，Bridge对kind214明确使用原ReadDependencyFilter。

原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`，ability-resource-source/manifest.json归档17完整原方法及Vector3三方法和直接helper；ExportAbilityResourceMethods.py重取核验。GenerateAbilityResourceFields.py核验原方法指纹、完整自身属性顺序/默认、Token/native/父表/sentinel。公共OCL和SpecialAbility继续由原有生成器和回归核验。

GameObjectAbilityResourceCases.cs覆盖完整两类父字段及适用子节点、Theft正负整数/时间/完整223和456掩码、向量所有分量/空/缺省、引用编号+1、向量/OCL/Filter/Upgrade/Sound精确分配顺序和Token、继承列表及模块0/1/2/27/28/29/65、独立默认/padding/布局、拒绝与不变性；保留/Od、/O2及平台0/1/2/99完整旧回归。
