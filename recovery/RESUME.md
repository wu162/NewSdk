# 编译器恢复：当前交接入口

更新：2026-10-25（北京时间）。分支 `recovery/compiler-source`。用户已要求开始交接，停止扩展新组。

## 已验收275/322，最后8类待验收

**最终已验收源码 `3c5c40a4bd95ff62657d4e0776afb4c94ac30a6d`，GameObject run `40140521059 success`，基础组件 run `40140521060 success`。** /Od、/O2各977064 fixtures、2931192三缓冲区比较、20432拒绝、2752不变性，平台0/1/2/99；两模式实际原始日志与全部作业步骤已核对。不可变证据 `evidence/gameobject-runs/40140521059-1`，根快照同源码。

本轮在原251基础上已验收24类：完整Attach五类、完整Utility十三类、完整Area六类。合同分别见GAMEOBJECT_ATTACH_NEXT.md、GAMEOBJECT_UTILITY.md、GAMEOBJECT_AREA.md。首批260验收source1e044cd8/run40139405380保留；失败run40139056890为Steal/Leech原wrapper遗漏Attach父Token表，已按原008/010方法修正并获后续完整矩阵成功。初始交接误写无额外父层已修订，原证据保持不变。

**最后8类源码、测试和140原方法/14原PE表已完成，仅本地源码一致性检查通过，仍待本检查点提交的Windows完整累计矩阵，不计入275。** 合同 `GAMEOBJECT_EFFECT_SUPPORT.md`；实现GameObjectSupport.h/GameObjectSupportRuntime.h；测试GameObjectSupportCases.cs；取证support-source、ExportSupportMethods.py；生成器GenerateSupportFields.py。类型：LocomotorSet、SetLocomotorSpeed、RestrictSpecialPower、PassiveAreaEffect、AttributeModifierAura、AutoAbility、EmotionTracker、FXListBehavior。包含完整Upgrade36/Die52父层、Query12/Filter112、Event24、强Modifier/Emotion列表及重复float子节点；不能跳过这些依赖或只验空模块。

当前源码接受283类、显式拒绝39类，**这不是283已验收**。准确未完成范围是最后8类待验收，加39类未实现；后者完整名单见 `GAMEOBJECT_REMAINING.md`。58 Token-only回退保留。下一步先查本检查点源码的Actions终态，失败读实际日志并修复，最后取得/Od/O2四平台三缓冲区完整成功，核对sourceCommit/runId及原始日志后才可将8类计入283；不要重复已验证且输入未变的源码矩阵。

SDK本轮0次，原DLL和正式插件路由未变。完整GameObject/两个完整替代DLL/真实SDK与完整mymod集成均未完成；组件结果不能当作正式支持。XmlCompiler优先类型入口XMLCOMPILER_PRIORITY.md，与GameObject模块计数分开，另一个恢复分支的并行工作不在本轮改动范围。

先读AGENTS.md、EFFICIENCY.md、STAGE_DURATION.md；默认单代理、完整组实现统一验证，普通提交检查点，禁止强推。已有并发自动证据提交均保留。未修改工作流权限/触发器；本轮回归由既有路径触发。本轮避免重复下载原反编译源、复用已验收父层/过滤器/引用helper及既有矩阵；无账号额度计量，不能量化节省。
