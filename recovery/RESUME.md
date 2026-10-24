# 编译器恢复：当前续接入口

更新：2026-10-24，北京时间。仓库`wu162/NewSdk`，分支`recovery/compiler-source`。先读AGENTS.md和EFFICIENCY.md；GitHub插件访问，禁止强推，保留并发提交及Actions证据。默认时长仍按STAGE_DURATION.md；**本轮用户指定只运行30分钟，不修改长期1小时规则**。

## 已验收229/322；另两种正在完整验证

本轮完整WeaponFire/DelayedWeaponFire/Beam/ObjectStatus四种SpecialAbility派生已验收，见GAMEOBJECT_ABILITY_WEAPON.md。源码`204765b733c01c3eac7b31bf8099342a92bd4d69`，GameObject run`40105748576 success`。/Od、/O2各615928 fixtures、1847784三缓冲区逐字节比较、18016拒绝、1628不变性，平台0/1/2/99。实际日志、validation.json identity、生成器/原方法核对与工作流产物均已核对；不可变证据在`evidence/gameobject-runs/40105748576-1`。

源码`855277ac2ad9103536282b31ae639f678fbc1e7c`追加完整OCL500/Movement488 SpecialPower派生，见GAMEOBJECT_POWER_CREATION.md；GameObject run`40106342576`验证中，未完成前两种不计数。基础组件run`40106342611 success`；前组基础run`40105748512`与WeaponTemplate run`40105748507`也success。该源码包含全部六种实现与完整旧回归，优先取得此run实际终态再收尾，不重复触发同源码矩阵。当前剩余拒绝清单源码为91种，已验收数在此run成功前仍229。

根证据快照可能仍指向前轮，请用精确run/sourceCommit和不可变证据，不盲信`evidence/gameobject-root`。Publish脚本每次写run-attempt唯一目录，最终收尾再同步根快照。SDK本轮0次，原路由保留；组件通过不代表正式GameObject插件、两份完整替代DLL或SDK通过。

## 下一完整组：SpecialAbility移动/传送/弹出乘客（仍拒绝）

先读GAMEOBJECT_ABILITY_MOVEMENT_NEXT.md。SwitchLocomotors350237090/native260、EjectPassengers887477685/280、Teleport1313225606/264、TeleportToCaster2474849635/272。**四类父均SpecialAbility240**，TeleportToCaster不能据类型名继承SpecialPower452。已提取32完整原方法含直接装饰helper闭包和Vector3三方法，固定原Tokenizer blob`6e40a9ed6c9f961d294d49768d911e6a6f354619`，见`ability-movement-next-source/manifest.json`和ExportAbilityMovementMethods.py --check。本轮只取证，未启用/未计数。EjectionBoneOffset为nullable Vector3@272，String8/向量先于父String8/Anim12分配；Switch的LocomotorToSet没有INVALID默认，缺省零。

## 关键合同

SpecialAbility240为45根属性、两个inline223状态、两String8、nullable Anim12；不能混用SpecialPower452或Update24。Options31未知bit5，Conditions3未知bit0，Disabled12未知bit0；普通负词XOR，ALL覆盖完整word并忽略后词。ActiveModelCondition单枚举未知233。

WeaponFire有11完整属性，DestealthClosestObjectMatchingFilter是bool，ModelConditionsOnUnpack单枚举；Delayed必须保留WeaponFire中间层。Beam原拼写DissapateWhenTargetDead；nullable status28→自身骨骼文本→父两文本→Anim12。OCL UpgradeOCL每项4字节OCL强引用且每项独立一字段Token子表，精确bool名RegisterObjectsWithSpecialAbilityUpdate；Movement nullable model456/60字节与status223/28字节，默认bounce20.0/50.0/0.5。具体偏移和默认值见两份本轮类型文档。

完整矩阵保留/Od、/O2、平台0/1/2/99、三缓冲区逐字节、独立布局/拒绝/不变性及全部旧回归。GameObject公开ImportsData为空，不把Tracker中间引用表当公开Imports。正式启用前SDK同输入/XSD/真实调用/四产物/适用无原DLL重放及完整mymod门槛不降低。

本轮复用既有父/helper/枚举、各完整组统一验证，没有逐字段反复触发；未取得账号额度计量，不能量化节省。
