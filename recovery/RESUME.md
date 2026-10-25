# GameObject、XmlCompiler 与 W3X 当前续接入口

2026-10-25（北京时间）。目标分支 `recovery/compiler-source`。本文件保留PR [#2](https://github.com/wu162/NewSdk/pull/2) 的GameObject/XmlCompiler状态，并加入W3X还原成果；禁止强推。先读AGENTS.md与EFFICIENCY.md，再按子系统读取详细交接。

- **GameObject**：322/322已知Marshal模块已完成，58个Token-only保持原未知Marshal回退。验收源码 `f8726d624b0a74ea0927705bca352027cbf76e30`；组件运行40162947222、基础40162947146、真实SDK40162947209均成功。详细当前状态及不可变证据见[RESUME_GAMEOBJECT.md](RESUME_GAMEOBJECT.md)和GAMEOBJECT_DRAW_MODEL.md。
- **XmlCompiler**：原301登记项排除TestGameObject和11个Joypad独立根后，289入口组件还原及元数据核对完成；其中267个XSD全局类型取得真实SDK根调用证明，22个非全局项不冒充独立SDK根。SDK源码 `9b2ef85bc45e6b397fce3d275165715afa921a92`、运行40164710673成功；5个MOD profile四产物相同，783次实际调用、4698次原版/隔离重放缓冲区比较。详细状态见[RESUME_XMLCOMPILER.md](RESUME_XMLCOMPILER.md)、[XMLCOMPILER_PRIORITY.md](XMLCOMPILER_PRIORITY.md)及[SDK验收收据](evidence/xml-priority-sdk/acceptance.json)。

- **W3XCompiler**：W3DAnimation/W3DMesh定义行为恢复及专用MOD真实SDK三组对照已通过。验收源码 `ceeef68df4c032309154214f6746322e0a65e2ec`，运行40169489711 attempt1成功；原版、恢复版、移除原W3X DLL的恢复版四产物逐字节相同，8动画/4网格各36次实际三缓冲区比较。详细范围、未定义行为拒绝边界及不可变证据见[w3x/RESUME.md](w3x/RESUME.md)与[w3x/SDK_ACCEPTANCE.md](w3x/SDK_ACCEPTANCE.md)。

三组子系统SDK验收分别执行EALAModStudio defaultscript.cs case5基础数据BAB流程，只在临时SDK副本启用相应恢复插件；各自验收锚点与范围分别保留，不将它们改写成一次多插件同时启用的联合SDK运行。本次合并不修改已验收Native/Bridge/共享helper及原SDK DLL/config，不重复触发未变成功类型的编译测试。

当前已授权还原范围内没有新增待实现或待收取验收。后续变更按实际影响选择测试；缺失、失败、输入变化的组仍须取得完整必要验收。原XmlCompiler排除12根、22非全局项的边界不因合并扩大，其余自定义编译器及完整Tokenizer不在本次完成声明内。

证据冲突处理：XmlCompiler当前共享证据保留priority分支已验收版本；compiler-source同名旧证据原样归档在 `evidence/xml-merge-baselines/compiler-source-833c9177`，GameObject不可变运行档案不变。XML/TSV按证据目录.gitattributes检出以核对运行SHA256；源码与运行锚点不得交叉套用。

W3X合并核对：唯一重叠编辑为本导航文档；W3X源码/测试/工作流/MOD/证据保持其已验收内容，共享Relocation、ScalarParsing、ResourceMarshaling、XmlNodeAdapter及SDK/tools、SDK/Schemas与两分支相同，见[w3x/MERGE_VALIDATION.json](w3x/MERGE_VALIDATION.json)。不把历史验收写为合并提交新运行；没有重复触发未变化的已验收类型。W3X的SDK_READY仍精确绑定原验收的整树，合并后包含其他子系统新内容，后续若显式再启动SDK须先重新核对并认证新整树，不能直接篡改指纹冒充已验证。


## 托管辅助程序集（2026-10-25）

**ExpressionEval 与 GameDataVerifier 完整源码已还原到 `src/Managed/ExpressionEval`、`src/Managed/GameDataVerifier`。** ILSpy 9.1.0.7988 全工程导出，保留原命名空间/程序集身份及 net20 目标，无占位实现。验收源码 `e806b1ca26509915621d214be2492d572db78f74`，运行 [40171170187](https://github.com/wu162/NewSdk/actions/runs/40171170187) 成功：Debug/Release 各 32,306 条原版对照记录一致，恢复目录中不存在两个目标原 DLL；Core 表达式动态加载通过。详见 [MANAGED_SUPPORT.md](MANAGED_SUPPORT.md) 与 [验收收据](evidence/managed-support/validation/40171170187-1/validation.json)。仅完成源码及组件验收，未替换生产 SDK DLL/config、未执行完整 MOD 的真实 SDK 集成编译。


## Core / Remote / EALABuild 独立还原分支（2026-10-25）

分支 `recovery/core-remote-ealabuild` 从 `c61aa003be605a7169fba6305311f2c96d88896c` 拉出，三个完整 ILSpy 工程已放入 `src/Managed/Core`、`src/Managed/Remote`、`src/Managed/EALABuild`，共 98 个 C# 文件。验收源码 `939080072f73d2eff92c563fcd04ab9dbe056aa5`，运行 40171944143 attempt1 成功：三个工程 Debug/Release 构建通过，原版/恢复版类型成员、资源及组件样本转录相同。两个已审计的现代编译器初始化元数据差异详见 [BUILD_INFRASTRUCTURE.md](BUILD_INFRASTRUCTURE.md)。未替换 SDK DLL/config，未执行完整 MOD 的真实 SDK 集成编译，未合入 compiler-source；既有其他编译器验收锚点保持不变。


## Upheaval 联合真实 SDK 验证与 ID 根因审计（验证分支）

本节仅说明 validation/upheaval-recovered-apt-global 分支成果，未合入 recovery/compiler-source。main 的真实 Upheaval 源码保持原样；恢复版 Core 与 XML/GameObject/Weapon/Armor/W3X 等组件在临时 SDK 联合接入。运行40178403660、源码0afc6eb49d81b61b9840e21d0edd840d5f0f0de3：Studio case3 AptUI、case4全局、case5基础数据全部成功，原版/恢复版共10次调用、20份四产物SHA256相同；未运行后处理与打包。XML记录1379次、GameObject121次、W3X Mesh134次/Animation194次实际调用。详见[UPHEAVAL_SDK.md](UPHEAVAL_SDK.md)。

ID反复失败根因：把Marshal字段集合当作全部XML合法属性集合，并用全局根登记推断身份属性，漏掉嵌套XSD继承及pipelineOnly元数据；部分低层正例又省略嵌套必填id。已按全部821份XSD、22种显式ID类型与6种嵌套身份类型整理可重现审计门槛，保留ObjectFilter/ModuleData/AIStateTactic等真实二进制ID。详见[ID_METADATA_ROOT_CAUSE.md](ID_METADATA_ROOT_CAUSE.md)。

静态汇总另发现未被Upheaval触发的AISpecialPowerInfo.id遗漏，已补齐并通过独立组件运行40180066600、源码3d32896d2d2607f4819cfd81c044a240dd278e46：/Od、/O2四平台，每模式每平台4165样例、12495原DLL三缓冲区比较及12495直接入口比较。全SDK未机械重跑，两个验收锚点不得交叉冒用。当前请求范围已完成；后续先汇总同组Schema/Marshal元数据边界、统一组件验证，再集中必要SDK批次。


## 公开 Mod 用例扩展（2026-10-25）

仅 validation/upheaval-recovered-apt-global：已把 Yuri’s Retaliation 的9个新资产与 Afterdash 的2个同义过滤器对照注册进 Upheaval（共11资产、8类）。本地ID前缀与引用同步改名，58处静态资源引用有本地/SDK清单匹配；原版SDK负责最终Schema/继承/链接验收。源码8d3fe79706c0ef1a2aa995a2fb669e74ebece444，运行40181842895 attempt1成功：原版先通过才开始恢复版，10次BAB调用退出0，20组四产物一致，11资产全部出现在恢复版manifest，10个非Weapon资产各有1次真实插件调用。未修改编译器实现、未重复未变chooser组件矩阵。详见[PUBLIC_MOD_CASES.md](PUBLIC_MOD_CASES.md)及[evidence/upheaval/40181842895-1/validation.json](evidence/upheaval/40181842895-1/validation.json)。仅case3/4/5编译验证；不包含Lua运行、游戏行为及BIG打包。当前请求已验收，无新增阻塞。
