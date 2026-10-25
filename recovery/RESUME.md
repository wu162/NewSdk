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


## Upheaval ID 修复及公开源码经验已合入（2026-10-25）

从 validation/upheaval-recovered-apt-global 的固定提交 068643219fded09a05fb7b75c6507b769c430316 选择性合入：4个实现文件、6个回归测试文件、821份XSD的ID审计工具与清单、定向/SDK脚本、根因及公开用例经验文档、精简成功证据。修复嵌套AI状态与BuildDelayRange、GameDependency、ArmorSet/LocomotorSet、ObjectVisionRange及AISpecialPowerInfo身份属性边界，保留真正序列化ID和外来命名空间拒绝。源码和测试子树逐SHA与已验证来源一致，SDK工具/Schema/基础资源不变。组件40175816790、40177066058、40180066600以及SDK40178403660、40182719547沿用原sourceCommit/runId，未冒充合入提交新跑SDK。17资产SDK批次10次调用成功、20组四产物一致。源码输入与大型轨迹继续留在验证分支，本分支不引入整套Upheaval素材；运行SDK脚本前须按文档取回对应输入。详见[合入核对](UPHEAVAL_FIXES_MERGE.md)、[根因与门槛](ID_METADATA_ROOT_CAUSE.md)、[公开用例经验](PUBLIC_MOD_CASES.md)。当前请求范围已验收，无新增编译器阻塞。


## EALA.Metrics 源码还原（2026-10-25）

分支 `recovery/eala-metrics-source` 从 `7fc417546989ca94a18742b37965e9c6751a409e` 新建。完整工程位于 `recovery/src/Managed/Metrics`，13 个 C# 文件，保留原程序集身份和 net20 目标。验收源码 `2fc44710df27ea9baf4e73d183dcd7697a9a435d`，运行 [40186536173](https://github.com/wu162/NewSdk/actions/runs/40186536173) attempt1 成功，Debug/Release 各 1,197 条原版对照记录一致，恢复测试目录不含原目标 DLL。源码包括 SQL writer；仅验证离线行为，SQL 服务端与真实 SDK MOD 编译尚未验证。未替换 SDK DLL/config，合入追踪：[PR #7](https://github.com/wu162/NewSdk/pull/7)，目标 `recovery/compiler-source`。合并时目标分支新增内容仅为其他组件的验收证据，Metrics 源码、测试和原 DLL 输入未变，复用上述精确运行结果。详见 [METRICS.md](METRICS.md) 和 [验收收据](evidence/metrics/validation/40186536173-1/validation.json)。


## Mvp.Xml 独立还原分支（2026-10-25）

分支 `recovery/mvp-xml-source` 从 `260f1d2c34d52910ff27d53ef070059c4b1c9785` 新建。完整工程位于 `recovery/src/Managed/MvpXml`，91 个 C# 文件，覆盖原库 XInclude/XPointer/XPath/EXSLT/XSLT/序列化能力。按用户要求只恢复功能，输出无签名 DLL；保留简单程序集名/版本，未替换 SDK 原 DLL。构建后恢复 51 个 EXSLT 原始 CLR 方法名，避免 ILSpy 的 C# 名称转义改变 XSLT 反射入口。验收源码 `fe38f6039f77a45a77ed53da93a53ac97e110da3`，运行 [40188365985](https://github.com/wu162/NewSdk/actions/runs/40188365985) attempt1 成功：Debug/Release 各 2,110 条原版对照记录一致，包含 Core 原包装方法实际调用，恢复宿主排除原目标 DLL；详细构建、签名边界与组件覆盖见 [MVP_XML.md](MVP_XML.md)。本分支未合入 compiler-source，未执行真实 MOD SDK 编译。


### Mvp.Xml 调用处改为静态引用（2026-10-25）

用户要求 SDK 直接调用恢复函数。Core 的 `AssetDeclarationDocument.LoadXml()` 现在直接构造 `Mvp.Xml.XInclude.XIncludingReader` 并设置 XmlResolver，`DocumentProcessor.ProcessDocument()` 移除动态加载初始化，Core 项目引用 MvpXml 源码工程；旧包装类仅保留公开方法兼容，不再反射。MvpXml 使用原始二进制资源直接嵌入，保留可读 resx，从而让原有 dotnet 构建方式可用。验证同时排除原 Core/MvpXml、实际执行 LoadXml 和依赖记录，并检查产物 IL 的直接构造/setter 调用；另运行 Core/Remote/EALABuild 既有组件矩阵。验收源码 `a75b48a960d32e199acb2716e06e57e6deca82ce`，运行 [40189585854](https://github.com/wu162/NewSdk/actions/runs/40189585854) attempt1 成功：Debug/Release 各 2,111 条 MvpXml/真实 LoadXml 对照及各 2,434 条基础组件记录一致，直接调用 IL 审计通过。修改位于 `recovery/mvp-xml-source`，SDK/tools 原二进制仍作基线；尚未合入 compiler-source，未执行完整 MOD 编译。
