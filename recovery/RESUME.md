# 编译器恢复：当前续接入口

更新：2026-10-24，北京时间。仓库`wu162/NewSdk`，分支`recovery/compiler-source`。先读AGENTS.md和EFFICIENCY.md；GitHub插件访问，禁止强推，保留并发提交及Actions证据。默认时长仍按STAGE_DURATION.md；**本轮用户指定只运行30分钟，不修改长期1小时规则**。

## 已验收231/322已知Marshal模块

本轮225→231，完整新增六种：WeaponFire/DelayedWeaponFire/Beam/ObjectStatus四种SpecialAbility派生，及OCL500/Movement488两种SpecialPower派生。完整合同/原方法来源/布局/测试分别见GAMEOBJECT_ABILITY_WEAPON.md与GAMEOBJECT_POWER_CREATION.md。

**最终共同验收源码`855277ac2ad9103536282b31ae639f678fbc1e7c`，GameObject run`40106342576 success`。** /Od、/O2各632248 fixtures、1896744三缓冲区逐字节比较、18304拒绝、1640不变性；平台0/1/2/99，两模式合计3793488缓冲区比较。实际两模式日志、原方法/生成器/原PE检查、validation.json的sourceCommit/runId/平台/成功标记、工作流全部步骤和两份产物均核对。不可变完整证据在`evidence/gameobject-runs/40106342576-1`，收尾将`evidence/gameobject-root`同步为同一个Git树。

首组独立验收源码`204765b733c01c3eac7b31bf8099342a92bd4d69`，run`40105748576 success`，两模式各615928 fixtures/1847784比较/18016拒绝/1628不变性；不可变证据仍在其run-attempt目录。基础组件最终run`40106342611 success`，此前基础`40105748512`和WeaponTemplate`40105748507`也success。后续文档/下一组只读取证不改Native/Bridge/测试/生成器，无需重复同源码矩阵。

91种其他已知Marshal仍拒绝，58 Token-only未知Marshal回退保留。后续先按精确run/sourceCommit核对不可变证据，不能盲信根快照总是最新；发布脚本只写run-attempt唯一目录，阶段收尾才同步根快照。本轮SDK0次，原路由不变；完整GameObject、正式插件与两份完整替代DLL尚未完成，组件通过不冒充SDK集成。

## 下一完整组：SpecialAbility移动/传送/弹出乘客（仍拒绝）

先读GAMEOBJECT_ABILITY_MOVEMENT_NEXT.md。SwitchLocomotors350237090/native260、EjectPassengers887477685/280、Teleport1313225606/264、TeleportToCaster2474849635/272。**四类父均SpecialAbility240**，TeleportToCaster不能据类型名继承SpecialPower452。已提取32完整原方法含直接装饰helper闭包和Vector3三方法，固定原Tokenizer blob`6e40a9ed6c9f961d294d49768d911e6a6f354619`，见`ability-movement-next-source/manifest.json`和ExportAbilityMovementMethods.py --check。本轮只取证，未启用/未计数。EjectionBoneOffset为nullable Vector3@272，String8/向量先于父String8/Anim12分配；Switch的LocomotorToSet没有INVALID默认，缺省零。

## 关键合同

SpecialAbility240为45根属性、两个inline223状态、两String8、nullable Anim12；不能混用SpecialPower452或Update24。Options31未知bit5，Conditions3未知bit0，Disabled12未知bit0；普通负词XOR，ALL覆盖完整word并忽略后词。ActiveModelCondition单枚举未知233。

WeaponFire有11完整属性，DestealthClosestObjectMatchingFilter是bool，ModelConditionsOnUnpack单枚举；Delayed必须保留WeaponFire中间层。Beam原拼写DissapateWhenTargetDead；nullable status28→自身骨骼文本→父两文本→Anim12。OCL UpgradeOCL每项4字节OCL强引用且每项独立一字段Token子表，精确bool名RegisterObjectsWithSpecialAbilityUpdate；Movement nullable model456/60字节与status223/28字节，默认bounce20.0/50.0/0.5。具体偏移和默认值见两份本轮类型文档。

完整矩阵保留/Od、/O2、平台0/1/2/99、三缓冲区逐字节、独立布局/拒绝/不变性及全部旧回归。GameObject公开ImportsData为空，不把Tracker中间引用表当公开Imports。正式启用前SDK同输入/XSD/真实调用/四产物/适用无原DLL重放及完整mymod门槛不降低。

本轮复用既有父/helper/枚举、各完整组统一验证，没有逐字段反复触发；未取得账号额度计量，不能量化节省。


## 2026-10-25 当前实现检查点（未验收）

本轮完成移动4类及相邻状态/对象操纵7类的Native/Bridge/完整测试。首轮run40128411260在Check三缓冲区差分之后的独立Eject断言失败：测试误把4字符含终止符字符串分配写成4字节（应8字节），并修正两个`\\1`强引用预期为2；实现无需改变。不能把失败run标为通过。随后补齐GiveOrRestoreUpgrade/ReconstituteState/DominateEnemy/LevelGrant/CloneStoredObjects/ReplaceSelf/AssembleStructure整组，含完整父240、nullable Filter112和typed GameObject列表、原PE核验ReplaceSelfOptions7。原方法证据52份，Inspect run40128710378成功，仅取证。下一步以本提交完整GameObject /Od、/O2和平台0/1/2/99矩阵结果验收，未通过前仍为231/322；成功后预计242/322。SDK未运行、原路由保留。按最新长期约1小时执行，本轮没有30分钟限制。
