# 下一完整组：AttachUpdate家族（仍拒绝）

2026-10-25已实现完整五类，当前待最终Windows累计矩阵验收，不计入251已验收数。`attach-next-source/manifest.json`保存78完整原方法与10张原PE表，来自只读run40134582402/source9ee04e9f350b4604a6fe7973a94a84fd89f7260d。固定原Tokenizer源码blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；PE SHA256 `ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603`。ExportAttachNextMethods.py按**精确参数类型**递归原方法及直接装饰helper，重新核对全文指纹和PE表。初始run40134524611也成功，但用类型后缀匹配，最终归档以修正后的40134582402为准。

## 五类及父层

| 类型 | TypeId | native | 原合同 |
|---|---:|---:|---|
| AttachUpdateModuleData | 3925677257 | 336 | 完整基础合同；Marshal最后调用ModuleData，Token父表为UpdateModuleData |
| StealUnitUpdateModuleData | 554078198 | 336 | 原wrapper的Marshal/Endian直接使用AttachUpdate，保留自身TypeId；**Token wrapper生成1条Attach父记录及sentinel，之后进入Attach完整表** |
| LeechTargetingAttachUpdateModuleData | 3395303590 | 336 | 同上，必须保留wrapper的Attach父表层，不能按名字猜新增字段 |
| MoneyGainAttachUpdateModuleData | 2980398469 | 348 | 全部Attach336父层；自身字段/nullable56先分配 |
| BoobyTrapUpdateModuleData | 3842124126 | 404 | 全部Attach336父层；自身Filter112先分配 |

不能套用SpecialAbility240父层；相似的名称和native大小不是继承证据。

## Attach336完整字段与原Marshal顺序

“无”表示节点存在时该属性仍无默认；整体分配零初始化。inline状态为223位/28字节，inline模型为456位/60字节。

| 输入 | 偏移 | 表示 | 默认 |
|---|---:|---|---|
| ParentStatus | 8 | inline状态 | 无 |
| ParentStatusAttached | 36 | inline状态 | 无 |
| AttachedObjectStatus | 64 | inline状态 | 无 |
| AttachedModelConditions | 92 | inline模型 | 无 |
| ForbiddenParentStatus | 152 | inline状态 | 无 |
| IgnoreForbiddenParentStatusStatus | 180 | inline状态 | 无 |
| ParentStatusToCopy | 208 | nullable28指针 | 无 |
| ParentStatusToPrefer | 212 | nullable28指针 | 无 |
| Range | 216 | float | 无 |
| CloseEnoughRange | 220 | float | 1000.0 |
| ParentOwnerAttachmentEvaEvent | 224 | 强EvaEvent引用 | 空 |
| ParentAllyAttachmentEvaEvent | 228 | 强EvaEvent引用 | 空 |
| ParentEnemyAttachmentEvaEvent | 232 | 强EvaEvent引用 | 空 |
| AttachFXList | 236 | 强FX引用 | 无 |
| DetachFXList | 240 | 强FX引用 | 无 |
| ParentOwnerDiedEvaEvent | 244 | 强EvaEvent引用 | 空 |
| ParentAllyDiedEvaEvent | 248 | 强EvaEvent引用 | 空 |
| ParentEnemyDiedEvaEvent | 252 | 强EvaEvent引用 | 空 |
| InitialAttachDelay | 256 | Time float | 无 |
| IdleScanDelay | 260 | Time float | 0.35s |
| Flags | 264 | bits27/4字节 | STICK_TO_PARENT TELEPORT USE_GEOMETRY |
| ParentDeathTypeToListenFor | 268 | **DeathType单枚举** | ALL=33 |
| NoDieIfStatusActive | 272 | inline状态 | 无 |
| ParentPositionElevation | 300 | float | 无 |
| BounceAmount | 304 | float | 0.2 |
| BounceTimeout | 308 | Time float | 0.33s |
| DamageTypesToNotLeech | 312 | nullable39位/**8字节**指针 | 无 |
| DeathTypesToNotLeech | 316 | nullable34位/**8字节**指针 | 无 |
| ForbiddenParentStatusDieDeathType | 320 | **DeathType单枚举** | NORMAL=1 |
| ObjectFilter | 324 | nullableFilter112节点指针 | 缺省null |
| ModifierToLeechFromParent | 328 | List8；每项强AttributeModifier引用4 | 空 |

最后一项是IList读取的**重复子节点**，不是属性或空格分隔的字符串列表。每项走强引用helper，不按弱GameObject ID哈希。分配顺序为两份nullable状态→nullable Damage8→nullable Death8→ObjectFilter及内部列表→强Modifier列表。ParentStatus/Attached等inline掩码的Token直接指向inline值；两个nullable状态和nullable8掩码的Token须指向值而非指针槽。

原枚举：AttachUpdateFlagsType27（未知bit22），DamageType39（未知bit38），DeathType34（位集未知bit33；单枚举未知33）；ObjectStatus223未知bit198、模型456未知bit233沿用完整公共合同。Flags默认4/5/6三个bit，值112；`ALL`位集填满实际词，不能与DeathType单枚举ALL=33混淆。精确literal均在manifest.peEnums，必须保留拼写/大小写。

## 两个派生完整自身合同

MoneyGain348：ActionType单枚举@336，无默认，唯一`ON_DETACH_PARENT_DEAD=0`，未知0；**名字含Flag但不是位集**。PurchasePricePercent float340默认100%，原行为`atof -> float -> *0.01f`。MoneyGainObjectStatusValidation nullable56@344，节点存在才Push56，内含ForbiddenStatus inline223@0、RequiredStatus inline223@28，无默认；独立Token子表。自身nullable56分配在全部Attach父层之前。

BoobyTrap404：FireWeaponConditionsOnParent inline223@336、PreventEnteringSetStatusOnParent inline223@364，无默认；AllowTriggerOnAllies bool400=false；SpecialWeapon强Weapon引用392；PreventEnteringFilter nullable112指针396，节点缺省null。自身属性/Filter先于全部Attach父层，末尾401–403 padding保零。不能只把Steal/Leech作为两个base-only类型单独启用而让共享父层缺字段。

## 实现与验收注意

现有ModuleValue::NullableBits只分配/Endian一个4字节词，**不能直接用于这里两个8字节掩码**。应使用定向完整8字节实现，或评估全部调用者后扩展公共helper，保持所有旧回归。Modifier列表可复用已有强引用列表逻辑，但必须核对节点surface、allocation/reference/Token顺序。

下一轮以五类完整依赖组实现；保持/Od、/O2、平台0/1/2/99、全部旧回归和三缓冲区逐字节差分。独立检查336/348/404、56/112及nullable8布局，节点缺省/空节点，全部枚举literal与回退，强引用/nullable掩码/过滤器嵌套列表跨分配边界，父层与派生分配顺序、每项Token、非法输入和不变性。SDK继续按更大里程碑累计。

本轮核对原008/010方法发现旧合同“没有额外空继承Token层”错误：两种alias的原wrapper各Push 12×2并写父TypeId3925677257，已按原方法修正实现、独立Token检查和此文档。证据保持原样。
