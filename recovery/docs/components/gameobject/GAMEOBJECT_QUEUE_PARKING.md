# QueueDock / ParkingPlaceBehavior

状态：原方法已核对，本组源码与差分测试已通过最终整批验收，精确锚点见本页末尾。本页不代表完整 GameObject、插件或 SDK 集成完成。

原始依据：Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`。精确签名、原行号、包含尾 LF 的 body SHA256、字符串 literal 与 47 个模块/必要 helper 方法见 `evidence/gameobject-modules/queue-parking-source/manifest.json` 及同目录编号文件；按签名核对，不按编号猜语义。

## 完整类型和布局

两者的 Marshal 都直接调用 ModuleData 父层；根结构 @0 为实际 TypeId、@4 为模块 id。不得据类型名推断 Token 父层。

| 类型 | TypeId | x86 native | Token 父层 |
|---|---:|---:|---|
| QueueDockModuleData | 2941001505 | 52 | CreateModuleData |
| ParkingPlaceBehaviorModuleData | 3921409083 | 56 | UpdateModuleData |

QueueDock 的五个字符串均为内联 String<char>8（length@0、数据指针@4），均有空字符串默认，因此缺省也按原方法分配一个 NUL 字节。

| 字段 | offset | 表示 | 默认 |
|---|---:|---|---|
| DockStartBoneName | 8 | String8 | 空字符串 |
| DockActionBoneName | 16 | String8 | 空字符串 |
| DockEndBoneName | 24 | String8 | 空字符串 |
| QueueBonePrefix | 32 | String8 | 空字符串 |
| TurnTableBoneName | 40 | String8 | 空字符串 |
| DockingAnimation | 48 | 单值 ModelConditionFlagType | 无 |

DockingAnimation 是单枚举，不是模型状态位集。复用完整原表；未知/显式空回退 233，ALL 为 454；缺省不调用枚举写入。

| ParkingPlace 字段 | offset | 表示 | 默认 |
|---|---:|---|---|
| NumRows | 8 | signed32 | 无 |
| NumCols | 12 | signed32 | 无 |
| ApproachHeight | 16 | float | 无 |
| DoorOpenTime | 20 | Time float | 0s |
| DoorCloseTime | 24 | Time float | 0s |
| QueueDistance | 28 | float | 100 |
| LimitedProductionAircraftSpots | 32 | signed32 | 0 |
| HelipadBoneName | 36 | String8 | HeliPark01 |
| CanParkHereFilter | 44 | nullable ObjectFilter112 | 无 |
| DefaultRallyPointOffset | 48 | nullable Vector3(12) | 无 |
| ParkInHangars | 52 | bool | 无 |
| LowersOntoParkingPlaceOnProduction | 53 | bool | false |
| CanSetRallyPoint | 54 | bool | true |
| DoorsUnpackOpen | 55 | bool | true |

“无”表示原 getter 无 fallback；缺省保留零初始化，不能人为补造默认值。Time 使用原 TimeTag 的单位解析，不是逻辑帧 Duration。

## Marshal / Token / endian 边界

ParkingPlace 按原 schema 顺序先处理全部属性，再分配 CanParkHereFilter，后分配 DefaultRallyPointOffset；即使 XML 子节点顺序反转也保留此分配顺序。Filter 复用完整 ObjectFilter 契约，包括 IncludeThing/ExcludeThing；Vector3 为可空独立 12 字节结构，不是内联坐标。

QueueDock Token 保留 Create 父层，不能以 Update 或 Module 父层代替；ParkingPlace 保留 Update→Contain→Behavior→Module 的独立 Token 父层。nullable Vector3 的原 Token helper 只 bookmark 指向的12字节值，不生成逐坐标子表；不可替换为其他类型使用的内联 Vector3 Token 布局。QueueDock 有 7 个有效 Token 条目及终止项，ParkingPlace 有 15 个有效条目及终止项；完整 name/type hash 与成员偏移见原方法证据。

Endian：QueueDock 交换五个字符串 length、DockingAnimation 与 ModuleData 两个字；不交换字符串字节或指针。ParkingPlace 交换 @8/@12/@16/@20/@24/@28/@32 及 Helipad 字符串 length@36；递归交换存在的 Filter、Vector3；bool 和指针不交换，最后交换 ModuleData 两字。

整组须保留未知字段/错误嵌套/不适用子节点的明确拒绝，不能仅因复用公共 helper 而接受其他模块的输入。验收仍要求完整 /Od、/O2、平台 0/1/2/99、三缓冲区逐字节差分、独立布局与拒绝测试；SDK 按仓库里程碑规则另行判断。


## 本轮最终整批验收

最终整批源码`922f44602683fc8a048d99fa2eb8ce8887ee0d72`；GameObject run **40089743442 success**，基础run **40089743457 success**。/Od、/O2每模式335812 fixtures、1007436三缓冲区逐字节比较、12596拒绝、892不变性，平台0/1/2/99；两模式合计2014872缓冲区比较。validation.json、实际两模式日志及sourceCommit/runId/终态已逐项核对。

9种实际新增模块与源码拆分均包含在该矩阵中，总数155→164/322，158种其他已知Marshal仍拒绝。SpawnCrate沿用既存kind103，仅强化回归，不重复计数。本轮SDK0次，原路由保留；组件成功不代表完整GameObject、正式插件或SDK集成完成。
