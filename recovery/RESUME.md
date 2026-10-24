# 编译器恢复：当前续接入口

更新：2026-10-25，北京时间。仓库 `wu162/NewSdk`，分支 `recovery/compiler-source`。先读AGENTS.md、EFFICIENCY.md、STAGE_DURATION.md；长期阶段约1小时。GitHub插件访问，禁止强推，保留并发代码/Actions证据。本轮约36分钟完成15类与完整矩阵后因上下文长度提前交接；没有修改长期时长规则。

## 已验收246/322已知Marshal模块

本轮231→246：移动/传送4类、状态与对象操纵7类、结构路由/雷达列表2类、资源偷取/创建进入2类，均为完整能力组，包含全部父层与适用子结构。

**最终共同验收源码 `22aaf75aa50857f1675d545f7b1ed0684137ae11`，GameObject run `40129692157 success`。** /Od、/O2各 **805500 fixtures、2416500三缓冲区逐字节比较、19192拒绝、1816不变性**；平台0/1/2/99，两模式合计4833000缓冲区比较。实际两模式原始日志、原方法/生成器/原PE检查、validation.json的sourceCommit/runId/平台/成功标记、全部工作流步骤和两份产物均核对。完整不可变证据 `evidence/gameobject-runs/40129692157-1`；根快照 `evidence/gameobject-root` 已同步为同一Git树 `71eebfbd8b08cc196fa2d2f23da2432c3bdab9aa`。基础组件同源码run40129692147 success。后续只改文档/下一组取证，不改本轮Native/Bridge/测试/生成器，不需重复同源码矩阵。

本轮合同与完整字段/默认/分配/Token/测试入口：
- `GAMEOBJECT_ABILITY_MOVEMENT_NEXT.md`：SwitchLocomotors260、EjectPassengers280、Teleport264、TeleportToCaster272；32原方法证据。
- `GAMEOBJECT_ABILITY_STATE.md`：GiveOrRestoreUpgrade252、ReconstituteState260、DominateEnemy260、LevelGrant256、CloneStoredObjects264、ReplaceSelf260、AssembleStructure260；52原方法证据及原PE ReplaceSelfOptions7。
- `GAMEOBJECT_ABILITY_ROUTING.md`：ActivateModuleSpecialAbility248、RadarScanMap248，分别包含ModuleInfo12与intersection128结构列表；26原方法证据。
- `GAMEOBJECT_ABILITY_RESOURCE.md`：TiberiumThief344、CreateAndEnter504；17原方法证据。

76种其他已知Marshal仍拒绝，58 Token-only未知回退保留。SDK本轮0次，原路由不变；完整GameObject、正式插件及两份完整替代DLL仍未完成，组件通过不冒充SDK集成。原来的XmlCompiler14优先类型工作见XMLCOMPILER_PRIORITY.md，不与GameObject模块计数混算。

## 下一完整组：ToggleStatus与完整AI依赖（仍拒绝）

先读 `GAMEOBJECT_ABILITY_TOGGLE_NEXT.md`。ToggleStatus TypeId3977962844/native252，父SpecialAbility240；自身ToggleState列表每项280，内含**inline AI148**，其StateWeapon是**nullable28**，另有ModuleInfo12列表。不能只支持简单状态字段。59完整原方法归档 `ability-toggle-next-source/manifest.json`，`ExportAbilityToggleMethods.py --check` 固定原blob并重取；本轮只取证，不启用、不计数。

六个新PE枚举取证run40129634414 success，源码1412fda52b973ee7f71f4ac2acd2be6fa870f101；表见power-continuation-enums.json。节点缺省与空节点必须区分：SkirmishAiInfo是inline，但节点缺省时不运行默认初始化。完整literal、偏移、默认、单枚举/位集未知回退已写入类型文档。

## 关键合同、失败历史与验证安排

SpecialPower字样不决定父类：状态七类与TeleportToCaster都继承SpecialAbility240；CreateAndEnter继承OCL500→SpecialPower452。自身String8、nullable向量/过滤器和列表严格先于完整父层分配。Eject向量Token直接指向值，不创建Vector3子表；Radar列表为inline223/456/284共128字节，不是ObjectFilter112。GameObject公开ImportsData为空，不把Tracker中间引用表当公开Imports。

中间完整验收603a7fd/run40128943921（242类）与50b60b0/run40129220289（244类）保留各自不可变证据。失败run40128411260保留：独立测试误把4字符含终止符字符串分配写4字节（应8），并将两处`\\1`引用值误写1（应2），已修正并最终完整验收；run40129487266为C#宿主局部变量重名编译失败，22aaf75已修复。失败不当成功，未关闭或取消回归。

复用既有父/helper/枚举、各完整组统一验证；每个可复现阶段及时普通提交。原DLL未覆盖。SDK继续按更大里程碑累计；正式启用前XSD/真实调用/相同输入/四产物/适用无原DLL重放及完整mymod门槛保持。没有账号额度计量，不能量化节省。
