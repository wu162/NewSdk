# 编译器恢复：当前续接入口

更新：2026-10-24（北京时间）。仓库wu162/NewSdk，分支recovery/compiler-source。先读AGENTS.md、EFFICIENCY.md和本文件；旧STATUS仅按需查，不全量加载历史。

## 本轮用户提示（后续继续遵守）

当前环境命令行无法克隆仓库，使用 GitHub 插件；Actions 一般需要2分30秒以上，等待时推进独立工作，不高频轮询；直接写代码还原更快时省略不必要步骤，效率优先，但严格守住正确性底线。细则及用户原话见 [EFFICIENCY.md](EFFICIENCY.md) 的“用户执行提示（2026-10-24）”。

## 已验收112/322已知Marshal模块

本轮由93增至112，新增19种，完成四个完整能力组：TechUpgradeReceiver（1）；雷达/可见性/Capture（8）；直接开火/伤害跟踪（7）；LaserState/SweepingLaserState/StreamState（3）。对应GAMEOBJECT_TECH_RECEIVER.md、GAMEOBJECT_VISION.md、GAMEOBJECT_WEAPON_BEHAVIORS.md、GAMEOBJECT_BEAM_STATES.md。原已支持的Beacon、RadarMarker、LaserUpdate不重复计数。

最终有效源码 **5d4e332b53eb8492f7731cf7f388de55fdc7672e**：
- GameObject run **40051190340** success；基础 run **40051190379** success。
- 远端validation.json的sourceCommit/runId、两模式oracle日志和终态已核对。
- 每/Od、/O2：**194088 fixtures、582264三缓冲区逐字节比较、10216拒绝、416不变性**；平台0/1/2/99。两模式合计1164528次缓冲区比较。
- **210种已知Marshal仍明确拒绝**；58 Token-only沿原未知Marshal回退。全部既有Body、ClientUpdate、Upgrade、声音、生命周期、Die、AI等回归保留。
- 根范围88属性/38类子节点；完整GameObject、两份完整替代DLL及正式GameObject插件未完成。component artifact仍是组件，不冒充完整替代DLL。
- SDK本轮**0次**，原路由保留。没有改变SDK输入/路由；不重跑既有Weapon/Armor SDK。额外既有WeaponTemplate组件回归run40050646649亦success（源码b57e5a5a）。

中间成功锚点：Receiver fa250611/run40048856796；Vision 7cd68875/run40049625447；开火/跟踪b57e5a5a/run40050646715。最终矩阵包含全部中间组。

## 关键契约

- ReceiverDistanceType独立Token类型873624354；RadarDrawModule弱ID、两Angle；inline ObjectFilter@68；完整Upgrade父链和依赖分配顺序。
- UnitVision UpdateRate是signed整数，SelectiveVisibility同名字段是可空Time；AllowedPlayers可空Relationship位集。UnifiedShroud Marshal与Token父层路径不同，不能压平Token。
- FireWeaponNugget与EffectInfo各16字节；Nugget Offset Token直接bookmark可空Vector3数据。EffectInfo Fraction普通float，三个bool@12/13/14；Options最后Token。Color三个普通float不归一化。
- LinearDamage Speed原5Hz逻辑帧时长0.2f：先atof转float，再double(float(0.2))*double(value)转float；原字段与初始化公式见constants-source.json。
- LaserState粒子系统列表是强引用，Endian只交换count，不能按弱ID交换元素；Sweeping先自身强引用，再完整父层，保留额外父Token。
- 各Generate*Fields.py在正式TestGameObject.ps1校验原blob/PE/方法/Token；本地--exported-evidence只是开发检查，CI没有使用它。

## 本阶段新增与当前检查点

Projectile/Dynamics/Freefall/EnableDynamicsCollide及完整Movement四分支、Volume五类shape/Quaternion已验收，112→116/322；206种已知Marshal仍拒绝。见GAMEOBJECT_MOTION.md。源码c77679fc2d0cc53326317bfa6d93c39c707bd767；GameObject run40053555512 success、基础run40053555499 success。每/Od、/O2：202832 fixtures、608496三缓冲区比较、10616拒绝、424不变性，平台0/1/2/99；实际validation.json及两模式日志已核对。下文旧112锚点为此前阶段，不作为新代码的验收。

继续实现Physics/BezierProjectileBehavior/Missile完整组（GAMEOBJECT_FLIGHT.md），49原方法导出2479c32473931bc1707ac7e0a372e0504b6ca727/run40053432952 success。三个模块全部属性、两段KindOf位集、完整Bezier父Token和测试已准备，等待当前检查点完整矩阵；未经终态成功不计入116。Motion首轮独立断言误认为ALL后继续处理词，按原helper不前移指针的实际行为修正，仅断言修改；保留逐字节比较，最终上述矩阵已通过。SDK本阶段0次，原路由保留。

## 持续规则

默认单代理，约45分钟有效实现阶段；每约10–15分钟普通推送有意义检查点，未验证不标完成。整组实现及测试准备好后统一受影响完整矩阵；不空等、重复调查或重复相同输入运行凑时长；不强推，不新建无必要分支。并发Actions会写证据，更新ref前读新头重建树保留其内容。

Git命令行本环境无凭据，使用GitHub插件。巨大原文件fetch_file行范围本环境返回空，使用ExportLifecycleMethods.py+sound-selector-source.yml有界导出；按signature/bodySha复用相同原方法。调查曾触发150依赖保护（run40049720082），随后收敛完整能力组，未放宽保护。SDK跨阶段累计降频；正式启用前XSD、同输入原/恢复、真实调用、四产物及适用无原DLL重放门槛不降低。
