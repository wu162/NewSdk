# SWF 编译插件 · 续接入口

## 最新目标（2026-10-26，覆盖此前单EXE方案）

用户明确要求：**把SWF编译能力作为一个新的编译器插件，类似XmlCompiler；没有独立EXE，也不保留独立CLI或SWF专用测试EXE的规划。**

插件由新编译器宿主加载和调用。宿主最终是EXE、库或其他形式由宿主项目决定，SWF插件不作限制。按现有XmlCompiler的实际插件契约组织生命周期、类型注册、版本/处理哈希及编译入口；不能将“可复用Core + 独立CLI”当作本项目最终方案。

主线不再采用Native AOT单EXE、独立命令行、固定win-x64或单文件发布约束。目标框架、位数、托管/原生装配方式应与新编译器及现有插件机制对齐，先核实实际接口再选型。

分支继续使用 `feature/swf-direct-compiler`，成果在 `ui/` 集中。**本次只更新交接/说明，没有实现或构建新的SWF插件。**

## 当前实际状态

- 已导入并采用原项目源码：`ui/vendor/swf2apt/`，来源 `lanyizi/swf2apt@2b3cb662b7311616e462b884e5611750e5de88f4`，导入提交 `662a74cddcf17539386eb6a775df9f107bc9a2c7`。保留原结构/许可，仅未复制未使用的预编译zlib/crc_i386.obj。
- 原工具与源码的一致性已验证，证据可以继续复用。插件注册、加载、SWF内存转换和SDK集成尚未实现/验证。
- `ui/single-exe/` 是**停止采用的历史实验**，不再是重构入口或验收目标。原生接口、内存资源模型、预检代码可按插件需要评估复用；不能直接把EXE项目改成Library便宣称已实现XmlCompiler式插件。
- 旧memory原型、字符串补丁和导入项目原Program仅保留作历史/原工具差分参考，不继续规划成独立产品或新建独立SWF测试入口。
- 本次不删除历史代码、不修改构建工作流、不触发编译。后续代码重构时同步整理过时的single-exe项目/工作流，保留可追溯历史证据。

## 插件应承担的职责

1. 实现与XmlCompiler同级的新SWF编译插件，复用宿主现有的配置、注册、初始化、版本、诊断及编译调用机制。准确方法名/接口以实际源码为准。
2. 内部复用并改造已导入的SwfLib/AptConverter、flasm、triangle、jpeg、zlib，将SWF解析、RA3优化及所需资源生成留在宿主进程内，不调用辅助EXE。
3. 编译结果从内存进入现有资产构建，不落地apt/const/dat/ru/TGA/XML中间文件。游戏资产继续使用既有APT类型及原布局。
4. 遵循宿主的单资产返回契约和多资产收集流程。已调查的 `IAssetBuilderPlugin.ProcessInstance` 返回单个 `AssetBuffer`（InstanceData/RelocationData/ImportsData）；一个SWF产生的多个APT/Geometry/Texture资产需在宿主支持的收集/展开阶段登记，不能假设插件入口能直接返回整个资产组，也不擅自改全部插件接口。
5. 纳入真实SWF依赖追踪、内容hash、RA3/优化选项、转换算法版本和缓存失效。输入缺失、未支持tag和原生错误必须明确报告，不能静默忽略或返回空成功。

## 下一轮从这里开始

1. 定点读取当前新编译器与XmlCompiler的实际插件接口、注册配置、生命周期、TypeId/TypeHash、ProcessingHash及编译返回路径，确认宿主目标框架/位数。沿用已有证据，不加载recovery历史任务清单。
2. 按上述真实契约设计并建立同级SWF插件工程/入口及注册方式；如果内部需要模块拆分，它们属于插件实现细节。正式构建目标是插件，不新增独立EXE/CLI。
3. 从已采用源码改造字节输入/内存结果边界，移除目录清理、文件hash缓存和临时输出；flasm保留RA3模式、迭代与错误语义，改进程内调用。全局状态先隔离，不直接宣称并发安全。
4. 接入现有APT类型/AssetBuffer与宿主多资产收集；可评估让生成声明的File指向真实原SWF，保留文件依赖。现有resolver不支持mem://，不能只换虚拟路径冒充集成。
5. triangle/JPEG/zlib作为插件内部原生实现装配；明确内存所有权，C++异常、exit和跨托管栈longjmp不得逃逸。System.Drawing、几何和纹理管线按实际目标框架处理，不受旧AOT限制驱动。
6. 使用现有宿主、组件测试及原工具差分验证，不新增独立SWF测试CLI。先覆盖提供样例的普通/优化模式，再补视觉样例；最终比较manifest/bin/relo/imp并验证插件加载、注册及实际调用后，才可启用生产配置。
7. 正式方案确定后整理旧single-exe代码/工作流；只迁移确有价值且与插件契约兼容的部分。插件支持范围、组件成功、SDK/游戏验收分别记录。

## 有效基线证据

- 原源码一致性：[40288873546](https://github.com/wu162/NewSdk/actions/runs/40288873546)，success，记录 `ui/evidence/source-consistency.json`。附件PDB与原程序集GUID相符，390/390非生成C#源文件SHA256一致。
- 原始程序集/原flasm与源码版统一在.NET8 x86下，样例CoronaUIMain.swf的普通RA3模式全部5个文件、优化模式全部8个文件逐字节相同。优化APT/CONST/DAT=10121/9787/48字节；未优化=25861/25179/48。
- 未复现完整.NET5原生宿主；样例没有几何、图片及音视频。原工具一致性不等于新插件已支持这些功能或已集成SDK。
- 用户批准上传的最小原工具包：`ui/reference/original-managed-tools.zip`。校验清单：`ui/reference/uploaded-tool-source-checksums.json`。
- 历史单EXE壳：[40291703530](https://github.com/wu162/NewSdk/actions/runs/40291703530)，提交 `58a987b80beba690b3eb7cc0468abeae50c28158`，记录 `ui/evidence/single-exe-framework.json`。只证明当时壳的构建/预检；转换能力为0，不能作为插件实现或集成证据，不再推进该路线。
- 原样导入的PDB校验只适用于基线。主动改源后保留原工具与原始证据，按实际改动做差分，不修改原工具指纹来制造一致。

## 必要路径与运行约束

`ui/AGENTS.md`、本文件为当前规则；`ui/RESEARCH.md` 为历史调查和接口线索，`ui/VENDOR.md` 为导入说明；二者旧阶段描述须服从本次插件方向。

复用运行经验：GitHub插件操作仓库；及时普通提交/推送；不高频轮询Actions；不重复已验证且依赖未变的类型/完整SDK测试。保持原APT的32位相对偏移，不让宿主指针宽度改变资产布局。禁止强推，合入须PR并经用户明确批准。本轮仅文档更新，无需重新构建。
