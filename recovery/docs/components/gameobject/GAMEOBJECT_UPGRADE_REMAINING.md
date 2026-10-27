# 其余11种命名Upgrade派生：完整组件实现

前11种公共Upgrade与派生见GAMEOBJECT_UPGRADE_NEXT.md。本组复用完整36字节Upgrade基类，所有派生先Marshal自身字段/分配，再处理公共动画/TriggeredBy/ConflictsWith。原方法、原行号和SHA256见evidence/gameobject-modules/upgrade-remaining-source/manifest.json；固定原blob 6e40a9ed6c9f961d294d49768d911e6a6f354619。

已验收源码aed221a9bec343b1d8a67dd2f071c4ddab11e5b4；GameObject run40035310226和基础run40035310221均success。每模式168432样例、505296三缓冲区比较，/Od、/O2、平台0/1/2/99。后续包含本组的最终验收以RESUME.md为准。

| 模块 | TypeId | 字节 | 自身字段/子结构 |
|---|---:|---:|---|
| SubStructureBuildUpgrade | 829862040 | 52 | ；AvailableSubStructure,StructureOffset |
| ModelConditionUpgrade | 1306352767 | 164 | AddConditionFlags@36 ModelCondition；RemoveConditionFlags@96 ModelCondition；AddTempConditionFlag@156 Enum；TempConditionTime@160 Time 默认0.0s |
| ReplaceSelfUpgrade | 1409001909 | 52 | NewObjectUnpackTime@36 Time 默认0s；Options@40 ReplaceOptions 默认DISABLE_DURING_UNPACK；ReplacementTemplate |
| SubObjectsUpgrade | 1594276481 | 44 | ；Nugget |
| ObjectCreationUpgrade | 1776471404 | 72 | UpgradeObject@36 Reference；Delay@40 Unsigned 默认0；RemoveUpgrade@44 Reference；GrantUpgrade@48 Reference；ThingToSpawn@52 AssetId；Angle@56 Angle；DestroyWhenSold@68 Boolean 默认false；FadeInTime@60 Unsigned 默认0；UseBuildingProduction@69 Boolean 默认false；Offset |
| RemoveUpgradeUpgrade | 1973457077 | 56 | UpradesGroupsToRemove@36 StringList；SuppressEvaEventForRemoval@52 Boolean 默认false；RemoveFromAllPlayerObjects@53 Boolean 默认false；UpgradeToRemove |
| StatusBitsUpgrade | 2020381784 | 92 | StatusToSet@36 ObjectStatus；StatusToClear@64 ObjectStatus |
| UnpauseSpecialPowerUpgrade | 1072475067 | 44 | SpecialPowerTemplate@36 Reference；ObeyRechageOnTrigger@40 Boolean 默认false |
| CostModifierUpgrade | 3451973623 | 68 | Percentages@36 FloatList；IsUpgradeDiscountUpgrade@64 Boolean 默认false；StartsActive@65 Boolean 默认false；Slaughter@66 Boolean 默认false；UILabel@44 String；ObjectFilter,ApplyToUpgrade |
| TechUpgrade | 4286303667 | 168 | Radius@36 Float；RadiusY@40 NullableFloat；DistanceType@44 Enum 默认CIRCULAR；FXLineBoneName@48 String；ObjectFilter |
| GeometryUpgrade | 4286905310 | 52 | ShowGeometry@36 StringList；HideGeometry@44 StringList |

## 关键差异

- ModelCondition的两个60字节inline位集、156处scalar枚举不能混用。StatusBits的两个28字节inline位集也不分配指针。
- ReplaceSelf.Options为5位标志，默认DISABLE_DURING_UNPACK(index3)，未知落index4。ReplacementTemplate是弱GameObject ID列表。
- SubObjects.Nugget为44字节结构列表。Show/HideSubObjects实际是8字节字符串头列表；Upgrade为弱ID；UpgradeTexture是8字节双强资源引用结构列表；动画可空指针在36处；三bool在40/41/42，保留尾padding。
- SubStructure.AvailableSubStructure为8字节双弱ID结构，输入UpgradeTrigger/ObjectTemplate是文本子节点；StructureOffset为72字节：60字节inline ModelCondition位集后接inline Vector3。
- ObjectCreation的Delay/FadeInTime均unsigned，不是Time；三个强引用与ThingToSpawn弱ID不同。Offset是可空12字节Vector3。
- CostModifier.Percentages实际是原始float列表，50存0x42480000，不做百分比除100。ObjectFilter可空112字节；UILabel字符串分配在filter之前，ApplyToUpgrade弱列表随后。
- Tech.ObjectFilter是inline112字节，缺失保持全零，显式空才写filter默认；其Token即使缺失也有inline表。RadiusY可空普通float，允许负值。DistanceType的2项PE枚举CIRCULAR/RECTANGULAR，未知0。
- Geometry两个属性是字符串列表，区分大小写并保留原字符串。
- 原版拼写UpradesGroupsToRemove、ObeyRechageOnTrigger必须保留。纠正拼法会被拒绝。

GenerateUpgradeRemaining.py校验原Marshal查找次序/fallback、PE枚举和完整递归Token/sentinel，生成GameObjectUpgradeRemaining.h。拒绝未知/外来命名空间/未知属性/非法嵌套/重复单例。测试覆盖0/1/2/27/28/29/65列表、全部ModelCondition名称、深嵌套纹理/动画/过滤器、独立Token/offset/分配次序。SDK未运行，仍是组件。
