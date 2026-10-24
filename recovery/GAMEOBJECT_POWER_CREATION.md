# 完整OCL与Movement SpecialPower派生

2026-10-24实现并验收，完整共同源码`855277ac2ad9103536282b31ae639f678fbc1e7c`、GameObject run`40106342576 success`；精确验收锚点见RESUME.md，完整矩阵通过前不计数。SDK未运行、原路由保留。

| 类型 | TypeId | native | 完整父类 |
|---|---:|---:|---|
| OCLSpecialPowerModuleData | 2308556452 | 500 | SpecialPower452 |
| MovementSpecialPowerModuleData | 1729257771 | 488 | SpecialPower452 |

原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`。`power-creation-source/manifest.json`归档28个完整方法及直接装饰helper闭包；`ExportPowerCreationMethods.py --check`固定原源码并重取比对。`GeneratePowerCreationFields.py --check`检查完整属性顺序/默认值、Token/父层/sentinel、原方法指纹，并从SHA256 `ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603`原PE核对OCLCreateLocType15和SPMovementType3。LocomotorSet19/未知12复用原PE核验AIUpdate合同。生成`GameObjectPowerCreation.h`；运行时在`GameObjectPowerCreationRuntime.h`。

OCL自身完整属性：OCL强引用452；CreateLocation单枚举456，无默认、未知5；CreateLocationOffset float460无默认；MaxCreateCount uint464默认0；NumberToSpawn uint468默认1；DestinationOCL强引用472；精确布尔名 **RegisterObjectsWithSpecialAbilityUpdate**@496默认false。尾padding497–499为0。`UpgradeOCL` List8@476每项仅**4字节OCL强引用**，不是按名称猜出的Upgrade/OCL八字节结构；每项Token有OCL字段及sentinel。`NearestSecondaryObjectFilter` nullable Filter112指针484；`Upgrade`强Upgrade文本子节点List8@488每项4字节、直接Token值。自身Token有效11条加sentinel，父为完整SpecialPower452。

OCL分配顺序：UpgradeOCL列表→nullable Filter112及其内部弱模板列表→Upgrade强引用列表→完整父（StringList、三个inlineFilter、nullable Sound8/强Sound4）。原Endian只交换两个list count及Filter，不交换OCL/Upgrade强引用，也不交换父声音强引用。Bridge为自身和继承Filter使用原有完整ReadDependencyFilter验证，UpgradeOCL为可重复结构节点（仅OCL属性），Upgrade为可重复纯文本叶。

Movement自身完整9属性：MovementType单枚举452默认NONE（未知0）；Duration Time456默认0s；ModelCondition nullable **456位mask60**指针460，使用457名称表、未知233、INVALID=-1和>=456忽略；ObjectStatus nullable223位mask28指针464、未知198；LocomotorSet单枚举468默认INVALID=-1（未知12）；BusyDurationOnExit Time472默认0s；CollisionBounceHeight float476默认**20.0**、CollisionBounceDistance float480默认**50.0**、CollisionBounceSpeedScalar float484默认0.5。自身Token有效10条加sentinel，两个mask Token指向值而非指针槽。mask60→mask28→完整SpecialPower父分配；完整15/7 word Endian，ALL保留末word padding且不继续处理后词，负普通词XOR。

`GameObjectPowerCreationCases.cs`覆盖自身所有属性/默认/边界和全部父属性/枚举、全部继承Filter/声音、nullable456/223、OCL结构/强引用列表0/1/2/27/28/29/65、独立native布局/父表/子表/sentinel/引用index+1/分配顺序/非法输入与不变性，以及所有容器和重复模块跨分配边界。完整旧回归、/Od、/O2、平台0/1/2/99、三公开缓冲区逐字节差分继续保留。组件成功不等于正式插件、完整替代DLL或SDK通过。
