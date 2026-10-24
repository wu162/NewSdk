# 编译器恢复：当前续接入口

更新：2026-10-25，北京时间。仓库 `wu162/NewSdk`，分支 `recovery/compiler-source`。先读AGENTS.md、EFFICIENCY.md和STAGE_DURATION.md，最新长期阶段约1小时；本轮无30分钟限制。GitHub插件访问，禁止强推，保留并发代码/Actions证据。

## 已验收242/322；最终15类能力批次等待完整结果

原231基础上，移动4类和状态/对象操纵7类已完整验收。源码 `603a7fd0bd11e31b3c2774e3e97839fe5fa7622e`，run `40128943921 success`：/Od、/O2各752680 fixtures、2258040三缓冲区逐字节比较、18948拒绝、1776不变性；平台0/1/2/99。validation/sourceCommit/runId、两模式原日志、全部步骤和两份产物已核对，唯一证据目录 `evidence/gameobject-runs/40128943921-1`。基础组件run40128943883 success。具体合同见GAMEOBJECT_ABILITY_MOVEMENT_NEXT.md、GAMEOBJECT_ABILITY_STATE.md。

额外ActivateAbility/RadarScanMap两类结构列表源码 `50b60b0b184fbfc2fad06bb8c2aadf6f7b0dc62c`，run `40129220289` 仍需核对实际终态。随后完整TiberiumThief344/CreateAndEnter504已实现；其首次run40129487266在C#宿主局部变量重名编译阶段失败，修复源码 `22aaf75aa50857f1675d545f7b1ed0684137ae11`，最终run `40129692157` 验证中。**下一步优先取得最终完整矩阵结果并集中修复，不要把待验收4类提前计数。** 全部通过后预计246/322，剩76已知Marshal拒绝和58 Token-only未知回退。两类合同分别见GAMEOBJECT_ABILITY_ROUTING.md、GAMEOBJECT_ABILITY_RESOURCE.md。

首轮run40128411260在已通过原/恢复Check之后的独立Eject断言失败：4字符字符串含终止符分配应8字节，`\\1`强引用值应2；测试已集中修正并在603a7fd完整验收。失败/中间结果保留，不标为成功。原231基础最终验收仍是855277ac2ad9103536282b31ae639f678fbc1e7c/run40106342576。

本轮SDK0次、原路由不变。完整GameObject、正式插件及两份完整替代DLL仍未完成；组件通过不冒充SDK集成。SDK按更大里程碑累计，正式启用前的XSD/真实调用/相同输入/四产物/适用无原DLL重放与完整mymod验收保持。

## 下一完整组：ToggleStatus与完整AI依赖（仍拒绝）

先读 `GAMEOBJECT_ABILITY_TOGGLE_NEXT.md`。ToggleStatus TypeId3977962844/native252，父SpecialAbility240；自身ToggleState列表每项280，内含**inline AI148**，其StateWeapon是**nullable28**，另有ModuleInfo12列表。不能只支持简单切换状态。59完整原方法见 `ability-toggle-next-source/manifest.json`，`ExportAbilityToggleMethods.py --check` 固定原blob并重取；本轮只取证，不启用、不计数。

六个新PE枚举取证run40129634414 success，源码1412fda52b973ee7f71f4ac2acd2be6fa870f101；表见power-continuation-enums.json。特别核对SkirmishAiInfo节点缺省与空节点：inline存储但节点缺省时不运行默认初始化。完整literal/偏移/默认和边界导航见类型文档。

## 本轮关键合同与工作方式

15类分别是移动4、状态7、结构列表2、资源/创建2；已有公共父/helper/枚举复用，不逐字段重复跑矩阵。SpecialPower字样不决定父类：状态七类与TeleportToCaster都继承SpecialAbility240；CreateAndEnter才继承OCL500→SpecialPower452。String8、nullable向量/过滤器和列表严格按原方法顺序分配。

GameObject公开ImportsData为空，不把Tracker中间引用表当公开Imports。所有新类型保留两模式/四平台/三公开缓冲区/完整旧回归/独立布局/拒绝和不变性。发布脚本只写run-attempt唯一目录；最终收尾将根快照同步为最终成功证据同一个Git树，不能把当前根快照视作最新验证。没有账号额度计量，不能量化节省。
