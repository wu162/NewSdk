# 编译器恢复：收尾进展（2026-10-25）

分支recovery/compiler-source。当前已验收 **251/322** 已知Marshal模块，71种仍拒绝；58 Token-only回退保留。最终有效源码de8fffb065be8376c312612b9fd3d72d04f3a9d4，GameObject run40134635589 success，基础组件run40134635612 success。/Od与/O2各873680 fixtures、2621040三缓冲区比较、19512拒绝、1924不变性，平台0/1/2/99。两模式实际原始日志、全部步骤与归档validation核对完成；不可变证据evidence/gameobject-runs/40134635589-1，根快照同源码。此前40134423297失败为独立测试漏算Filter nullable28分配，已修正并取得最终成功，不掩盖失败。

本轮接着实现完整五类Attach家族，合同GAMEOBJECT_ATTACH_NEXT.md，原78方法/十PE表已固定。当前新代码**未验收，不计入251**；首轮run40139056890失败确认Steal/Leech原wrapper缺失Attach父Token表；已核对原008/010方法并修正。追加StorePurchasedUpgrade12/AIGate16/CombinedInfo16/TimedOpacity16完整辅助组；九类当前均待最终完整矩阵，不计入已验收数。下一步统一/Od/O2复验并更新sourceCommit/runId。修复原取证检查因Python set遍历随机顺序而产生的非确定性；保持既有证据编号和内容。

GiveUpgrade256/ReplaceObject260-Nugget12/ShieldSphere264/SpecialDisguise264-TerrainData8合同GAMEOBJECT_ABILITY_FINISH.md；ToggleStatus252完整AI依赖合同GAMEOBJECT_ABILITY_TOGGLE_NEXT.md。两组均包含在上述最终成功矩阵中。未改正式插件路由、未覆盖原DLL；SDK本轮0次，完整GameObject/两份完整替代DLL/SDK集成未完成，不把组件结果当作正式支持。原XmlCompiler14类见XMLCOMPILER_PRIORITY.md，不与模块计数混算。

继续先读AGENTS.md、EFFICIENCY.md、STAGE_DURATION.md；默认单代理、长期约1小时有效工作量、整组实现统一验证、约10–15分钟普通提交检查点，禁止强推。细节查对应类型合同，历史STATUS.md不必全读。
