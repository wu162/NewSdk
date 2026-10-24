# 编译器恢复：收尾进展（2026-10-25）

分支recovery/compiler-source。当前已验收 **260/322** 已知Marshal模块，62种仍拒绝；58 Token-only回退保留。最终有效源码1e044cd8ed645d4607206ca5a16948f620a3778e，GameObject run40139405380 success，基础组件run40139405419 success。/Od与/O2各952632 fixtures、2857896三缓冲区比较、19932拒绝、2352不变性，平台0/1/2/99；实际两模式原日志及全部步骤已核对。不可变证据evidence/gameobject-runs/40139405380-1。前一基线251/source de8fffb/run40134635589及失败run40139056890均保留。

本轮接着实现完整五类Attach家族，合同GAMEOBJECT_ATTACH_NEXT.md，原78方法/十PE表已固定。Attach及首批四类Utility已验收，新追加15类未验收；首轮run40139056890失败确认Steal/Leech原wrapper缺失Attach父Token表；已核对原008/010方法并修正。追加完整13类辅助组（GAMEOBJECT_UTILITY.md）及完整6类区域伤害/目标选择组（GAMEOBJECT_AREA.md）；本轮已验收9类；后续9类Utility和6类Area仍待最终完整矩阵，不计入260已验收数。下一步统一/Od/O2复验并更新sourceCommit/runId。修复原取证检查因Python set遍历随机顺序而产生的非确定性；保持既有证据编号和内容。

GiveUpgrade256/ReplaceObject260-Nugget12/ShieldSphere264/SpecialDisguise264-TerrainData8合同GAMEOBJECT_ABILITY_FINISH.md；ToggleStatus252完整AI依赖合同GAMEOBJECT_ABILITY_TOGGLE_NEXT.md。两组均包含在上述最终成功矩阵中。未改正式插件路由、未覆盖原DLL；SDK本轮0次，完整GameObject/两份完整替代DLL/SDK集成未完成，不把组件结果当作正式支持。原XmlCompiler14类见XMLCOMPILER_PRIORITY.md，不与模块计数混算。

继续先读AGENTS.md、EFFICIENCY.md、STAGE_DURATION.md；默认单代理、长期约1小时有效工作量、整组实现统一验证、约10–15分钟普通提交检查点，禁止强推。细节查对应类型合同，历史STATUS.md不必全读。
