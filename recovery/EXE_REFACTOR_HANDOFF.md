# NewSdk 新编译器重构计划与新会话交接（修订版）

修订日期：2026-10-25（北京时间）。本版纳入两个旧插件均为混合模式的明确约束，替代前一版计划。本文是后续实施要求，不是完成报告；本次仅核对最新源码并更新文档，没有启动 SDK 编译、重构或 Actions。

## 1. 用户最新要求与执行入口

仓库：https://github.com/wu162/NewSdk

基准分支：`recovery/compiler-source`。新会话开始时读取最新远程 HEAD，记录固定 SHA，再创建独立实施分支。

用户已明确要求：

1. Mvp.Xml 恢复源码已经合入，必须纳入新编译器。
2. **已还原代码全部融入最终 EXE，不再依赖对应 DLL。** 不能把源码目录整理、减少 DLL 数量或内嵌恢复 DLL 当作完成。
3. **重构开始前，先执行一次真实 SDK 编译验证。** 最新恢复实现必须联合启用，包括新合入的 Mvp.Xml 及修改后的 Core；成功并保存证据后，才能开始目录迁移、接口重写及整合。
4. 尚未还原而仍有必要的组件继续使用，允许其作为外部 DLL。不能因此丢失纹理、音频等编译能力。
5. 当前入口为命令行，未来另建界面；不建设原驻留 BAB、Remote 服务或 GUI。
6. 用户会安排子代理加速，具体数量和运行时长以新会话指令为准。本文件给出并行边界，不自动启动代理。

本版取消上一版“已恢复 DLL 可长期保留”和“不要求已恢复实现全部进入 EXE”的宽松终点。中间验证可产生恢复 DLL，最终发布不得使用它们。若遇到无法跨越的兼容性阻塞，保存证据并明确未完成，不得擅自把用户要求降为“EXE 加一批恢复 DLL”。

不要启动或管理额外的 Work/Codex 云任务。仓库操作先读 AGENTS.md，禁止任何强推。正常提交、推送与分支操作按任务执行；是否最终合入、PR 目标及合并操作以当轮要求为准。

## 2. 最新 Mvp.Xml 状态：已经核实的事实

当前分支存在 `recovery/src/Managed/MvpXml`，并有 `recovery/MVP_XML.md` 交接。

- 完整恢复工程包含 91 个 C# 文件；原程序集 124 个类型、1,276 个方法。包含 XInclude、XPointer、XPath、EXSLT、XSLT、reader/writer 和序列化辅助，不只是当前 SDK 使用的 XInclude。
- Core 的项目已通过 ProjectReference 引用恢复的 MvpXml。
- `AssetDeclarationDocument.LoadXml()` 已直接构造 `Mvp.Xml.XInclude.XIncludingReader` 并设置 resolver。
- `XIncludingReaderWrapper` 仅保留兼容入口，LoadAssembly 是空操作；生产流程不再依赖反射发现可选 Mvp.Xml。
- 用户先前已要求只保留功能，恢复 Mvp.Xml 不签名，不要求原公钥 token；新重构不能重新引入对原签名或私钥的要求。
- 构建过程将 51 个 EXSLT 方法名从 C# 导出形式恢复为带连字符的原名；四份二进制资源保留原字节。这些不能在并入 EXE 时遗漏。
- `MvpXmlMetadata` 使用 Mono.Cecil，属于构建工具，不是发布依赖。
- 最终直接调用组件验收：源码 `a75b48a960d32e199acb2716e06e57e6deca82ce`，运行 `40189585854`。MvpXml 与 Core 的 Debug/Release 对照和 Core/Remote/EALABuild 基础回归通过；**未执行完整 MOD 编译**。

新会话须重新核实远程更新及收据，不把上述组件结果当成本次重构前 SDK 基线。

## 3. B0：重构前真实 SDK 验证——必须先完成

### 3.1 顺序和允许的准备工作

先读取 AGENTS.md、recovery/EFFICIENCY.md、recovery/RESUME.md、MVP_XML.md 及所需 SDK 脚本。固定最新代码、工具与输入。此时允许只读审计、编写必要的联合验证脚本、构建恢复实现及修复基线暴露的缺陷；**禁止提前搬迁源码或实施架构重构**。

子代理可并行审计依赖、列目录映射和研究 EXE 链接方案，但只读；不能在 SDK 尚未通过时开始各自重构。

### 3.2 一次基线批次的具体含义

“跑一次”指集中进行一批真实 SDK 验证，不限制为只调用一个 BAB 进程。建议沿用真实 MOD 的 AptUI、全局数据、基础数据入口，依照原 EALAModStudio 构建流程提取并启动 BAB。输入需包含真实 XInclude，验证新 Mvp.Xml 的生产调用链。

在隔离临时 SDK 中执行两组，原 SDK 目录保留作对照：

| 组 | 配置 |
| --- | --- |
| 原版组 | 原 BAB、原组件，同一固定 MOD、Schema、资源和配置 |
| 当前恢复联合组 | 当前可用于编译的恢复 Core、Utility、Metrics、ExpressionEval、GameDataVerifier、MvpXml，以及已恢复的 GameObject/Weapon/Armor/XML/W3X 路由；未恢复能力仍用原组件 |

实际替换集合以代码和加载证据核实，不把“源码存在”写成“已在该批编译中执行”。Remote/旧 GUI 等非本流程模块不需要为了覆盖而人为启用。当前能力未覆盖的原类型必须明确分派并记录，禁止静默回退掩盖缺口。

若现有脚本只替换部分组件，先扩充联合配置；不能仅重跑旧脚本，便称最新所有恢复组件联合通过。确保每个恢复模块引用到联合组的正确依赖，不意外从原 SDK 路径加载另一份 Core/Utility/MvpXml。

### 3.3 输入、覆盖和结果

- 优先复用固定且曾由原 SDK 编译成功的 Upheaval 输入及依赖；完整输入可能不在目标分支，按相关交接取回固定版本，不能只复制根 XML。
- 原版必须先跑通该固定输入。使用冷缓存和隔离输出，避免旧产物或共享缓存导致假成功。
- 批次应实际调用原版纹理、音频能力，以及恢复的 XInclude。若现有 MOD 缺覆盖，增加最小合法资产与完整依赖，并先确认原版可编译。
- 对照 bin、manifest、imp、relo 的哈希；任何差异先解释和定位，不任意放宽比较规则。
- 记录源码 SHA、输入/工具/配置哈希、运行 ID、命令、输出、加载路径、实际类型分派和结果。
- 联合组经生产调用链执行：BAB → 恢复 Core → 对应编译器；MvpXml 由 LoadXml 实际调用。不能用测试宿主直接调用代替真实 SDK。
- 基线失败先修复并重复必要批次，明确失败原因和恢复范围；成功收据保存并普通推送后，才进入 R0。

原版与当前联合组都成功，关键产物一致，且实际调用/加载证据匹配，才算 B0 完成。已有历史运行不能替代这次包含最新 MvpXml/Core 的联合基线。

## 4. 最终产物的硬约束

### 4.1 什么才算融入 EXE

- 已恢复的托管实现以源码或最终合并的类型/方法进入主 EXE。
- 已恢复的原生实现以对象文件/静态库链接进主 EXE；中间静态库允许存在，不发布对应业务 DLL。
- 现有 Bridge/Plugin 层重新组织为 EXE 内部调用，不能继续依赖 Recovered.*.dll。
- Mvp.Xml 全部已恢复功能和资源纳入 EXE，不再引用或加载 Mvp.Xml.dll。
- Core、Utility、Metrics、ExpressionEval、Verifier、EALABuild 等已恢复代码必须纳入迁移清单；Remote 和旧 GUI 的已恢复源码也要列账，默认纳入可编译的内部源码集合但不启用旧宿主流程。若有代码拟排除，须列出文件、原因并取得用户对范围的明确调整，不能擅自以“未使用”删去来宣称全部融入。
- 全部融入不代表补全尚未恢复的原 DLL 功能；已知范围边界与不支持项继续明确记录。

以下均不满足要求：将恢复 DLL 嵌入 EXE 资源后 Assembly.Load；启动时解压 DLL；依赖旧 SDK 搜索路径；用单文件发布选项把多个恢复程序集包成外壳而运行时仍独立加载；将已恢复实现放到外部 worker/DLL 再由 EXE 转调。

### 4.2 允许的外部依赖

允许尚未还原且必要的原组件、其确实必要的传递依赖，以及系统/.NET/VC/DirectX 等运行环境依赖。最终不是承诺无任何 DLL，而是**已还原的业务代码不再通过依赖 DLL 工作**。

发布清单必须逐项区分：EXE 内实现、未恢复外部组件、系统运行库、Schema/资源、构建专用工具。构建工具如 Mono.Cecil 不得自动带入发布包。原 DLL 对照证据可以留在仓库，不能成为最终运行的隐式依赖。

## 5. R0：混合模式纹理/音频 DLL 与 EXE 内实现的兼容验证

### 5.1 已知事实与尚待核实的依赖

用户已明确确认：原 TextureCompiler、AudioCompiler 两个 DLL 都是混合模式。后续按此事实规划，不能再把它们当普通纯托管程序集处理。

Core 的插件契约已确认：IAssetBuilderPlugin.ProcessInstance 接收 InstanceDeclaration、返回 AssetBuffer，并提供类型元数据和版本方法。程序集身份参与类型识别；把 Core 类型放入另一个程序集后，命名空间/类名相同不代表类型相同。

两个插件各自直接引用了哪些 Core/Utility 类型、方法和原生符号，还需读取二进制清单逐项确认。特别是 Utility 的直接引用尚未逐一核实，不能把风险推测写成两个 DLL 都已确认直接依赖 Utility。

混合 DLL 同时涉及托管元数据、原生节、CLR/VTable fixup、初始化和可能的原生导入。不得默认 Mono.Cecil 等普通程序集写回工具能够保留全部信息，也不把“成功保存 DLL”当作兼容成功。

### 5.2 优先实验：主 EXE 保留 Core 程序集身份

B0 成功后，先构建最小、同架构同 CLR 的混合 EXE，将恢复 Core 类型编入其中。优先研究让最终 EXE 使用 `BinaryAssetBuilder.Core` 的程序集简单名称，并匹配旧组件实际要求的版本、culture、token 等身份条件；文件名与程序集身份的安排由实际工具链和加载实验确定，不能仅凭改 EXE 文件名认定成立。

实验目标是在不改写两个混合 DLL 的情况下，让其对 Core 的托管类型引用绑定到已加载主 EXE 中的实际类型。EXE 也可以提供托管类型，但这只是候选路线，不保证旧 CLR、混合模块初始化、按文件名加载或原生导入也自然兼容。

必须证明：

- 两个插件实际识别到主 EXE 的接口、输入和返回类型，类型转换及方法调用成功。
- 实际执行纹理和音频资产编译，检查元数据、三缓冲区、重定位、错误路径和资源释放。
- 没有加载旁路 Core.dll 或另一份同名程序集，没有由旧缓存代替编译。
- 不仅 AssemblyRef 匹配，还检查动态加载字符串、文件探测和任何原生导入。

如 Core 本身存在不能满足的签名/token 约束，记录实际原因，不假定能获得原私钥，也不通过随意关闭验证掩盖问题。不能把 MvpXml 不要求原签名的结论扩展为所有原组件都没有身份约束。

### 5.3 Utility 等额外引用必须单独解决

一个普通单程序集 EXE 只有一套程序集身份，不能通过改名同时充当 Core、Utility、Metrics 等多个程序集。若插件还直接引用这些程序集，逐项判断是否能以受控解析或窄边界适配统一到 EXE 内的实现。

AssemblyResolve 只能作为需要实验验证的机制，不能先验假设不同名称/版本/强名称的请求都能返回主 EXE 并正确执行；bindingRedirect 也不是任意程序集名称重映射。原生按 DLL 文件名导入的依赖需要另外处理，不能靠托管程序集身份解决。

### 5.4 优先路线失败后的处理

保留 Core 身份的主 EXE 无法满足全部引用时，再研究对混合 DLL 运行副本进行最小、可复现的二进制适配。必须证明原生节、fixup、初始化和调用 ABI 被保留，记录原/新文件哈希和变更范围；不直接使用普通 Cecil 重写作为默认方案。工具不支持时，不得强行保存或忽略报错。

如果适配仍不可行，优先还原真正阻塞的边界，必要时推进纹理/音频编译器恢复。不要退回发布 Core.dll、Utility.dll、外置恢复桥接 DLL 或恢复 worker，并称满足全部融入 EXE。

R0 只有真实纹理/音频编译、清理以及无恢复 DLL 的依赖审计通过后才完成；否则记为阻塞，保存结论，不开始大规模目录搬迁。上述方案目前均为计划，尚无通过证据。

## 6. 工具链与链接路线

初始继续 Windows/x86 及已有 CLR/框架兼容范围。net20 是现有恢复工程的目标，不代表新 EXE 已确定采用哪个框架；用实验选定兼容运行时。现代 .NET、x64、跨平台另列任务。

候选是同一 CLR 的混合 C++/CLI EXE：托管源码编为可链接模块或纳入最终产物，原生对象静态链接；也可采用经验证等价的离线程序集/模块整合。多语言模块链接、资源与方法名后处理的可行性需要先用小样例证明。

工具链实验至少包含：

- C# 类型与 C++/CLI/原生函数在同一最终 EXE 中互调，无外置 Bridge DLL。
- Core/Utility 类型和原版插件之间的真实交接。
- MvpXml 资源查找、XInclude、XSLT 扩展函数及 51 个带连字符方法在 EXE 中工作。
- XML 项目资源的逻辑名称、按 Assembly.GetExecutingAssembly 等定位资源的代码调整正确。
- Utility 的 C++ 签名/模板类型及 ABI 在新边界中的必要部分成立；旧 DLL 构建身份修复逻辑不能不加分析地套用到 EXE。
- EXE 导入、托管 AssemblyRef、运行时已加载模块均没有恢复业务 DLL。

不得把“启用了单文件发布”当成静态链接验证。

## 7. 功能目录和迁移映射

目录不对应独立发布 DLL。主代理统一决定保留 `recovery/src` 还是迁至根 src，只迁一次；还原证据和原版对照继续留 recovery。

| 功能目录 | 主要代码 |
| --- | --- |
| Cli/ | 命令行入口、参数、退出码、结构化输出 |
| Pipeline/ | Core 的流程、会话、依赖、缓存、构建阶段；必要 EALABuild 流程 |
| Documents/ | XML/Schema、包含、继承合并、表达式、文档预处理 |
| Assets/ | Core/Utility 资产结构、ID、引用、Manifest、输出流 |
| Compilers/Common/ | 重定位、标量解析、资源封送、节点适配 |
| Compilers/GameObject/、Weapon/、Armor/ | 对应原生实现与 EXE 内适配 |
| Compilers/W3x/ | 网格、动画、原生与托管支持 |
| Compilers/Xml/ | AI、Audio 配置、UI、Effects、World 等资产编译 |
| Compilers/Validation/ | GameDataVerifier |
| Diagnostics/ | 日志、进度、Metrics |
| Compatibility/ | 未恢复 DLL 的加载和窄适配、兼容元数据，不产生恢复业务 DLL |
| ThirdParty/MvpXml/ | 全量恢复的 MvpXml 源码与资源 |
| Legacy/ | 已恢复但不启用的旧 GUI/Remote 等源码，纳入整合清单，不建设旧服务 |

功能内部可保留 Native、Bridge、Managed、Generated；GameObject 可再按 Movement、Combat、Abilities、Rendering、Economy 等拆分。同一生成器与输出由一人负责，移动代码时同步改生成器、工程、测试与脚本路径。

## 8. 组件处理清单

| 组件 | 最终要求 |
| --- | --- |
| Core、Utility、ExpressionEval、Verifier、Metrics、EALABuild、Remote | 恢复实现编入 EXE；不发布对应恢复 DLL；旧宿主行为不启用 |
| Mvp.Xml | 全量恢复源码及资源编入 EXE，修正直接调用与后处理，不再保留运行依赖 |
| 已恢复 GameObject/Weapon/Armor/XML/W3X | 原生实现静态链接，托管及桥接编入 EXE，显式注册 |
| TextureCompiler、AudioCompiler | 已确认是混合模式；尚未还原时保留，先验证主 EXE 保留 Core 身份，再处理额外引用 |
| EALA.Hash | 核实最新恢复状态；仍未恢复且必需时保留 |
| antlr.runtime | 核实最新恢复状态；当前 ExpressionEval 所需的未恢复运行库可保留 |
| antlr.astframe、CC3Tools | 审计真实直接/间接/动态依赖，不按名称删除或整批携带 |
| 原 Tokenizer/XmlCompiler/W3XCompiler | 只为明确尚未恢复且需支持的能力保留；已恢复路由不得再调用原实现；审计传递依赖 |
| 原 BAB.exe | 仅 B0 和后续对照使用，最终新入口不得依赖其执行编译 |

每个恢复源文件须有迁移状态：进入 EXE 的模块/功能、生成来源、资源归属和测试。不能仅统计发布目录里少了几个 DLL。

## 9. 新入口与执行语义

示意接口，最终以兼容框架为准：

```csharp
BuildResult Build(BuildRequest request, IBuildObserver observer);
```

BuildRequest 包含输入、输出、SDK/Schema/资源根、平台、构建配置、阶段、缓存策略。BuildResult 返回成功/取消、诊断与产物清单；CLI 转成稳定退出码。核心不直接依赖控制台、窗口或当前工作目录。

IBuildObserver 提供结构化诊断、阶段/资产进度及安全取消检查；不能声称能强制中断原版原生 DLL。失败/取消不提交伪完整产物，明确暂存和提交机制。

恢复实现显式注册；未恢复 DLL 通过适配注册，保留 TypeId、TypeHash、版本、验证器与缓存语义。音频专用 PathMusic 路由等按实际配置和调用核实，不按文件名随意分配。新旧实现切换时隔离缓存，不能让旧缓存替代新代码执行证据。

未来 GUI 可调用同一逻辑或新 EXE 的结构化命令行；本轮不实现 GUI/Remote。无论未来调用方式如何，本轮 EXE 整合要求不变。

## 10. 实施阶段与硬门槛

| 阶段 | 工作 | 完成标准 |
| --- | --- | --- |
| B0 重构前基线 | 最新恢复组件联合启用，原版/恢复版真实 SDK 编译对照 | 固定输入、真实调用及产物一致；成功证据已保存；未通过不得重构 |
| R0 EXE 可行性 | 同 EXE 多语言链接、资源/方法名；优先验证 EXE 保留 Core 身份，单独核实 Utility 等额外引用 | 两个原混合插件实际编译与清理成功，无恢复依赖 DLL |
| R1 目录迁移与构建 | 功能归位，更新工程、生成器、资源和源码清单 | 每份恢复代码有归属、构建可复现、行为不混改 |
| R2 全量融入 | 托管与原生实现进入 EXE，消除恢复 DLL 加载 | AssemblyRef、原生导入、实际加载审计通过 |
| R3 CLI/流程集成 | 新入口、阶段调度、内置分派与未恢复组件适配 | 新 CLI 自行执行完整目标流程，无原 BAB 代执行 |
| R4 发布验收 | 干净目录、真实 MOD、依赖收集、冷/热缓存和异常边界 | 对照 B0 和原版通过；外部组件仅未恢复必要依赖 |

B0 与 R4 是两次不同时间、不同代码形态的验证，不能互相替代。构建中间恢复 DLL 或 netmodule 可用于调试，但 R2 及最终验收不能靠它们运行。阶段成果允许未完成，不能标成最终要求已满足。

## 11. 子代理协作方案

主代理统一维护基线、运行时/链接方案、公共接口、生成/发布清单、分支合并与验收。数量以当轮用户授权为准，以下四类可按人数合并或再划独立子域。

| 任务 | B0 前允许工作 | B0 成功后的所有权与交付 |
| --- | --- | --- |
| A 混合组件兼容 | 只读审计两个混合 DLL 的引用、原生导入和 fixup | 先验证主 EXE 保留 Core 身份；再处理额外引用，必要时研究混合二进制适配；提交真实资产证据，不增加外置恢复 DLL |
| B 托管和 MvpXml | 只读迁移、资源、反射/程序集名清单 | Core/Utility/表达式/验证/Metrics/MvpXml 等源码融入；资源及连字符方法正确 |
| C 原生编译器 | 只读源码与链接清单 | 原生静态链接与桥接内部化；不更改布局、算法和未定义行为边界 |
| D CLI 与流程 | 只读原流程/参数映射 | 主代理冻结接口后实施 CLI、调度、诊断和取消，保持数据预处理与验证 |

B0 的恢复组装、Action 触发和结果归档由主代理协调，可以委派一个明确所有者，避免各代理重复发起真实 SDK 运行。B0 未通过，代理不得自行进入重构。

R0 技术结论确定前，不让 B/C/D 各自选框架或设计相互不兼容的 EXE 构建。公共方案确定后用独立分支/worktree，路径和生成器明确互斥；共享脚本/最终链接/元数据后处理由主代理集中集成。

建议合并顺序：B0 成功收据 → R0 工具链及兼容结论 → 功能迁移 → 托管/原生 EXE 整合 → CLI 流程 → 发布验收。各子代理交付 SHA、文件列表、命令和结果、待验证项、残留依赖及冲突点。禁止强推，定期普通提交和推送检查点。

## 12. 验证要求与完成定义

### 12.1 有针对性的组件检查

复用既有组件差分，按实际修改选择受影响组；避免目录改动机械触发全部昂贵 SDK 回归。重点新增真实新边界验证：EXE 内调用、旧 DLL 类型引用、原生内存交接、资源定位、MvpXml XSLT 反射方法名、错误/取消/缺依赖、路径带空格及非项目工作目录启动。

MvpXml 全量源码融入不能只验 XInclude；至少延续原有 XPointer、XPath、EXSLT、XSLT 扩展和序列化缓存代表性组件覆盖。原始资源字节、逻辑名称和 EXE 所在程序集查找逻辑均检查。

### 12.2 最终发布包审计

- 从最终发布包中物理移除所有恢复业务 DLL，并使原 SDK/tools 目录无法被隐式搜索到。
- 在干净目录用新 EXE 执行真实编译，Schema/资源通过显式路径或已规定布局提供。
- 审计 EXE AssemblyRef、原生导入与运行中加载模块，检查无恢复 Core/Utility/MvpXml/Recovered.* 等依赖。
- 明确允许的未恢复 DLL 及其必要传递依赖，记录原版或构建补丁版的来源/哈希。
- 不依赖 Assembly.Load 恢复 DLL 字节、解压缓存、旁路 worker 或旧 BAB.exe。
- 编译结果与 B0 同输入原/恢复结果比较，记录新源码 SHA 与独立验收收据。

### 12.3 原有经验继续适用

XSD 继承身份属性、pipelineOnly 元数据与序列化 ID 分开处理，不能全局删除 id。资产用例必须带依赖和合法 Schema 输入。历史验收只绑定其源码/输入/范围，不拼成一次联合成功，不改旧收据冒充新运行。

## 13. 新会话交付清单

- [ ] 最新远程 HEAD 与固定输入/工具清单。
- [ ] **B0：包含最新 MvpXml/Core 的真实 SDK 联合对照成功收据，先于重构。**
- [ ] EXE 运行时、多语言链接、保留 Core 身份及额外依赖处置的 R0 证据；两个混合 DLL 必须真实编译通过。
- [ ] 全部恢复源码和资源到 EXE 的迁移映射，包含未启用的旧代码处置。
- [ ] 按功能整理的源码、可重现生成器和最终 EXE 构建。
- [ ] 无恢复 DLL 的发布包，明确保留的未恢复组件。
- [ ] 新 CLI 实际完成目标阶段，调用记录与产物对照通过。
- [ ] 最终 AssemblyRef/导入/实际加载审计。
- [ ] 续接文档列出已完成、未完成、未验证、阻塞和剩余外部 DLL，不把中间形态标成最终完成。

## 14. 关键阅读入口

- `AGENTS.md`、`recovery/EFFICIENCY.md`、`recovery/RESUME.md`。
- `recovery/MVP_XML.md`；对应 `40189585854` 的 mvp-xml 与 build-infrastructure 验收收据。
- `recovery/src/Managed/Core/BinaryAssetBuilder.Core/AssetDeclarationDocument.cs`、`XIncludingReaderWrapper.cs`；Core 和 MvpXml 工程。
- `recovery/EALAMODSTUDIO_BUILD_FLOW.md`、`SDK/tools/BinaryAssetBuilder.exe.config`。
- `recovery/BUILD_INFRASTRUCTURE.md`、`MANAGED_SUPPORT.md`、`UTILITY.md`、`METRICS.md`。
- GameObject、XML、W3X 各自续接与能力边界。
- `recovery/ID_METADATA_ROOT_CAUSE.md`、`PUBLIC_MOD_CASES.md`、`UPHEAVAL_FIXES_MERGE.md`。
- 现有真实 SDK/Upheaval 脚本和 `.github/workflows`：开始执行时核实最新路径、参数、替换集合和触发条件。

本文中的链接和路径来自本次核对，新会话仍需以最新远程文件为准。B0 是后续执行的首个任务，本次文档修订没有代为执行。
