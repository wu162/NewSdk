# 剩余四种Create模块：完整组件实现

已验收源码38c17e13b7a28d15d61c42ecbfd2539964a72226；GameObject run40036586975、基础run40036586994均success。每模式178828 fixtures、536484三缓冲区比较、9516拒绝、336不变性；/Od、/O2和0/1/2/99完整通过。后续最终矩阵以RESUME.md为准。原方法精确签名/原行号/SHA256见evidence/gameobject-modules/create-source/manifest.json，66方法含已有EvaAnnounceClientCreate的核对，不重复计数。

| 模块 | TypeId | 字节 | 全部自身字段/依赖 |
|---|---:|---:|---|
| GrantUpgradeCreate | 79217712 | 44 | UpgradeToGrant强引用@8；ExemptStatus inline28字节ObjectStatus位集@12；GiveOnBuildComplete bool@40默认false，尾3字节padding |
| InheritUpgradeCreate | 3221293211 | 132 | Radius普通float@8默认0；ObjectFilter inline112字节@12；Upgrade弱typed-ID列表@124 |
| ExperienceLevelCreate | 3270227277 | 16 | Level signed/base0@8默认0；MPOnly bool@12默认false，尾3字节padding |
| ActivateSpecialPowerOnCreate | 3310015739 | 16 | SpecialPowerTemplate强资源引用列表@8；原Marshal内联在wrapper |

GenerateCreateFields.py核对原Marshal完整查找次序/fallback、Token原key/type/member绑定与sentinel；生成GameObjectCreate.h，接入TestGameObject.ps1。

- Grant、Inherit、Experience的Token实际走CreateModuleData→BehaviorModuleData→ModuleData；其Marshal直接落ModuleData，不能因此删除Create父层。
- Activate的Token实际走UpdateModuleData→ContainModuleData→BehaviorModuleData→ModuleData，不能按类名改成Create父链。
- Inherit过滤器缺失保持零；显式空写Rule=NONE(3)等默认。其依赖分配先于Upgrade弱ID列表，与XML顺序无关。Radius允许负数。
- Grant.ExemptStatus为inline位集，无指针分配；ALL写满7个存储字，保留位集尾部padding语义。
- Activate列表是强引用，存Import索引/原强引用解析结果；Inherit.Upgrade是弱ID小写hash，不能混用。
- 既有CreateModuleData、SupplyCenterCreateModuleData和EvaAnnounceClientCreate实现保留，未重新计入新增4类型。

差分覆盖全部223 ObjectStatus名称与位运算、signed/float/资源引用边界、0/1/2/27/28/29/65列表、inline过滤器全部属性与深列表、六容器、独立native大小/packed bool/padding/Token/父链/sentinel/分配次序，以及未知属性/非法嵌套/外来命名空间/重复单例拒绝。SDK未运行，仍为组件。
