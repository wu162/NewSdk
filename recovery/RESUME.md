# 编译器恢复：新一轮进行中

更新：2026-10-25，北京时间。本轮用户要求继续约1小时，不触发真实SDK编译；分支recovery/compiler-source，默认单代理，禁止强推。

## 已验收283/322，新增26类待完整矩阵

最终已验收源码 `e35541735322f0ba8987766ec362f8b65baffda1`，GameObject run `40141560682 success`，基础组件run40141560628 success。/Od、/O2各1013416 fixtures、3040248三缓冲区比较、20856拒绝、2880不变性，平台0/1/2/99；实际两模式原日志与全部步骤已核对。不可变证据evidence/gameobject-runs/40141560682-1；根快照将同步到同一树。上轮最后8类完整effects/locomotor组已验收，合同GAMEOBJECT_EFFECT_SUPPORT.md。

本轮新增完整13类对象交互组，源码、测试及166完整原方法/13张原PE表已完成，本地源检查通过，**待本检查点提交的完整Windows累计矩阵，不计入283已验收数**。合同GAMEOBJECT_OBJECT_CONTROL.md；核心GameObjectObjectControl.h/GameObjectObjectControlRuntime.h、测试GameObjectObjectControlCases.cs、ExportObjectControlMethods.py/GenerateObjectControlFields.py、object-control-source证据。包含完整Capture20/Laser44父层、ObjectEffectTuple120与UpgradeSelf强列表、Shadow60、Particle20及UpgradeMap8。CaptureAndGiveUpgrade原Token不含UnificationType，Destructible两inline Vector3直接Token；这些原始行为不能自行修正。

当前源码接受309类、13类仍拒绝，**接受数不等于验收数**。本轮又完成完整SlowDeath/Destruction五类及Geometry48/Shape32/Contact20和嵌套phase列表，合同GAMEOBJECT_SLOW_DEATH.md，原130方法/5 PE表已核对；与对象交互13类合批完整复验。前run40142578135为独立Topple默认期望把20f*.01f误写成.2f（一ULP），原恢复差分一致，已修正精确IEEE期望；失败证据保留。本轮再加入完整Contain核心8类，合同GAMEOBJECT_CONTAIN_CORE.md；其余Contain5和复杂W3D Draw8待恢复。第二次run40143597761定位四个可空引用误作inline引用，已修复Teleport Shader、WeaponScatter WeaponTemplate、Squeeze ArmTemplate/VictimShader并增加独立分配/Token测试；仍待完整复验。58 Token-only回退保留。未验证检查点不冒充完成。

已验收275中间source3c5c40a4/run40140521059及260中间source1e044cd8/run40139405380证据保留；最初失败40139056890（Steal/Leech wrapper父表）已修正并后续完整成功。最新成功前不复用旧sourceCommit冒充新代码已验证。

SDK本轮0次，原DLL和正式插件路由保持；完整GameObject/两个完整替代DLL/正式SDK及完整mymod验收尚未完成，组件结果不冒充集成。XmlCompiler另一恢复分支的并行任务不在本轮改动范围。先读AGENTS.md、EFFICIENCY.md、STAGE_DURATION.md，及时普通提交检查点，完整组统一验证，保留并发自动证据。
