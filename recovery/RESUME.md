# GameObject 与 XmlCompiler 当前续接入口

2026-10-25（北京时间）。目标分支 `recovery/compiler-source`。本文件整合PR [#2](https://github.com/wu162/NewSdk/pull/2) 两边状态；禁止强推。先读AGENTS.md与EFFICIENCY.md，再按子系统读取详细交接。

- **GameObject**：322/322已知Marshal模块已完成，58个Token-only保持原未知Marshal回退。验收源码 `f8726d624b0a74ea0927705bca352027cbf76e30`；组件运行40162947222、基础40162947146、真实SDK40162947209均成功。详细当前状态及不可变证据见[RESUME_GAMEOBJECT.md](RESUME_GAMEOBJECT.md)和GAMEOBJECT_DRAW_MODEL.md。
- **XmlCompiler**：原301登记项排除TestGameObject和11个Joypad独立根后，289入口组件还原及元数据核对完成；其中267个XSD全局类型取得真实SDK根调用证明，22个非全局项不冒充独立SDK根。SDK源码 `9b2ef85bc45e6b397fce3d275165715afa921a92`、运行40164710673成功；5个MOD profile四产物相同，783次实际调用、4698次原版/隔离重放缓冲区比较。详细状态见[RESUME_XMLCOMPILER.md](RESUME_XMLCOMPILER.md)、[XMLCOMPILER_PRIORITY.md](XMLCOMPILER_PRIORITY.md)及[SDK验收收据](evidence/xml-priority-sdk/acceptance.json)。

两边SDK均执行EALAModStudio defaultscript.cs case5基础数据BAB流程，只在临时SDK副本启用相应恢复插件；各自验收锚点与范围分别保留，不将它们改写成一次双插件同时启用的联合SDK运行。本次合并不修改已验收Native/Bridge/共享helper及原SDK DLL/config，不重复触发未变成功类型的编译测试。

当前已授权还原范围内没有新增待实现或待收取验收。后续变更按实际影响选择测试；缺失、失败、输入变化的组仍须取得完整必要验收。原XmlCompiler排除12根、22非全局项的边界不因合并扩大，其他自定义编译器及完整Tokenizer不在本次完成声明内。

证据冲突处理：XmlCompiler当前共享证据保留priority分支已验收版本；compiler-source同名旧证据原样归档在 `evidence/xml-merge-baselines/compiler-source-833c9177`，GameObject不可变运行档案不变。XML/TSV按证据目录.gitattributes检出以核对运行SHA256；源码与运行锚点不得交叉套用。
