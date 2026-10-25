# 当前分支项目导航：W3XCompiler

本分支 `recovery/w3xcompiler-source` 从 `c66339e49e32cfe3f44aa2f9f1757f9993bbac7a` 创建，用于新的 W3XCompiler 还原项目。**当前工作入口：[w3x/RESUME.md](w3x/RESUME.md)**，总体规划：[w3x/PLAN.md](w3x/PLAN.md)。不接续下方旧 GameObject/XmlCompiler/Tokenizer 待办。代码仍使用 recovery 的共同源码/测试/脚本体系，未来合回原工作线并复用或共同重构。

下方保留基点的另一项目交接供参考；其验收数据不属于 W3X。将来合并时保留两个项目入口并按用户当轮目标选择。

---

# 编译器恢复：317/322已完成累计验收与交接

更新：2026-10-25，北京时间。分支recovery/compiler-source，单代理，禁止强推，本轮SDK0次。

## 已验收317/322，剩余5类模型Draw

最终完整成功source `ffd4e0aa440dbb6a8742b283bf0b7c455a50115f`，GameObject run `40160094730 success`，基础组件run40160094713 success。/Od、/O2各 **1281576 fixtures、3844728三缓冲区比较、23848拒绝、4072不变性**，平台0/1/2/99；两模式合计7689456三缓冲区比较。已核对实际两条PASS原日志、作业/运行终态及不可变validation的sourceCommit/runId/modes/platforms。不可变目录evidence/gameobject-runs/40160094730-1；共享gameobject-root同步完全相同Git树。此前309类source9c138e8/run40145009538与全部失败/中间证据保留。

本轮新增完整8类：Engineer88、Infiltrator128、Transport332、HordeTransport336、Horde500；W3DLaser112、Tracer112、Stream76及完整FXShader28/四种多态常量（Bool16、Int16、Texture16、Float20）/Color4f16/nullable ParticleSystemList8。合同GAMEOBJECT_CONTAIN_REMAINING.md、GAMEOBJECT_DRAW_BEAM.md；源码生成器、完整runtime和C#累计测试已保存并全部验收。58 Token-only回退保留，正式DLL与插件路由保留。

重要已核实语义：Engineer/Infiltrator Marshal直接ModuleData而Token保留Contain父层；Tracer/Stream Marshal直接ModuleData而Token仍含Draw父层；HordeTransport独立bool332；KindOf任意未知位149；TextureIndex是signed int并保留CRT溢出；Shader常量未知类型保存type0且Token无嵌套表。原方法283+197指纹、父层调用与标量Token类型交叉检查均保留。

失败与修正保留：40157547072独立语音哈希期望；40158022383测试表名重复前缀；40158330433仅/Od通过（1269184 fixtures）后/O2被15分钟上限取消；40159265840/40159330006/40159478438新旧beam符号冲突与漏改引用；40159602595 Tracer少24字节Draw父表；40159867519 TextureIndex错作unsigned。最终生成符号统一drawBeam、测试DrawBeam；累计作业现30分钟，新组前置后仍跑全部历史回归。最终成功不依赖修改/跳过失败断言。

validation.scope历史尾句仍写13个拒绝，是旧说明；实际UnsupportedModuleTypes已核对只有5个且完整回归按5个执行。下一实现批次顺带更正该尾句，不为纯标签重复启动相同完整矩阵。

## 下一轮

剩余完整组：ScriptedModel216、Spotlight224、Tank228、Dynamics252、Truck396，继续显式拒绝。先读GAMEOBJECT_DRAW_MODEL_NEXT.md及GAMEOBJECT_REMAINING.md；draw-source已有283原方法、17张PE表，显式保留全局ModuleData**分派的source-inventory边界。ModelConditionState196、AnimationState140、Animation60、ParticleSysBone36等须完整实现，不能只启用Scripted根属性。共享Shader实现可复用，完整组统一验证。

完整GameObject、两个完整替代DLL、正式SDK及完整mymod仍是独立门槛；组件成功不得冒充SDK成功。XmlCompiler另一恢复分支不在本轮范围。先读AGENTS.md、EFFICIENCY.md、STAGE_DURATION.md；约1小时阶段、10–15分钟检查点、GitHub插件普通提交推送与保留并发证据继续适用。自动证据只写不可变目录，共享根快照须在完整成功后手动同步并核实锚点。

本轮约69分钟，后段用于修正差分和收取最终完整矩阵，没有扩充收尾范围。避免重复clone、全量历史/反编译上下文与逐字段SDK运行；无可比额度计量，不能量化节省。
