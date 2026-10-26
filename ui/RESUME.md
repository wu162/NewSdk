# SWF 编译 · 续接入口

## 当前状态：单 EXE 重构框架已搭好

分支 `feature/swf-direct-compiler`。用户最新要求：将已采用的工具源码重构为仅生成一个EXE来编译SWF，本轮先打框架。长期仍保留内存结果接入MOD编译器、不生成APT中间文件的目标。

新框架：`ui/single-exe/`，Windows x64 / .NET8 Native AOT。静态C++桥实际链接进EXE，未来将flasm、triangle、jpeg、zlib迁入同一进程。**当前只完成壳、输入检查、模块契约及发布验证，SWF→APT算法尚未迁入；compile明确以退出码4失败，不生成产物。不能把框架编译成功说成SWF编译完成。**

## 本轮验收

- 源码提交：`58a987b80beba690b3eb7cc0468abeae50c28158`。
- Actions：[40291703530](https://github.com/wu162/NewSdk/actions/runs/40291703530)，success；job `121468046787`。
- 机器记录：`ui/evidence/single-exe-framework.json`。
- 单文件产物：`SwfCompiler.exe`，2536448字节，SHA256 `2ffc705f50efa27fc94cfa41e69ab65c67b73e041a93f63e327601842ffbe8a2`。
- [EXE artifact](https://github.com/wu162/NewSdk/actions/runs/40291703530/artifacts/12636298624)，名称SwfCompiler-framework；证据artifact ui-single-exe-evidence。
- 隔离目录仅放EXE和SWF输入、PATH仅保留System32：帮助/版本、静态原生ABI=1、能力位=0、CWS/FWS结构检查、参数/坏输入拒绝、compile未就绪明确失败、无额外产物均通过。检查PE无CLR目录，发布目录无DLL/runtimeconfig/deps文件。
- 首次运行40291333356已生成EXE，但坏输入退出码分类不正确；已修正InvalidDataException分支并取得上述完整成功结果，未放宽断言。

## 直接读取这些文件

- `ui/single-exe/README.md`：模块边界、限制及迁移顺序。
- `ui/single-exe/Publish.ps1`：一键构建静态库并发布EXE。
- `src/SwfCompiler.csproj`：Native AOT + DirectPInvoke + NativeLibrary；IL2026/IL3050为错误。
- `src/Compiler.cs`：字节输入/CompilerOptions → CompilationResult资源组，ISwfBackend尚未实现；无路径与文件写入依赖。
- `src/SwfInspector.cs`：有限长/有限嵌套的FWS/CWS结构预检，不是SwfLib语义解析器；ZWS未实现。
- `native/include/swfc_native.h`、`native/bridge.cpp`：仅真实实现版本/能力查询，不提供假成功转换函数。
- `tests/smoke.py`、`.github/workflows/ui-single-exe.yml`：单文件分发/隔离运行验收。上述src/native/tests相对路径均位于ui/single-exe。
- `ui/vendor/swf2apt/`：已采用原源码；本轮没有改它，继续作为正确性基线。

## 原源码基线（复用，不重跑）

采用 `lanyizi/swf2apt@2b3cb662b7311616e462b884e5611750e5de88f4`，导入提交 `662a74cddcf17539386eb6a775df9f107bc9a2c7`。保留原目录、许可；未复制未使用的预编译zlib/crc_i386.obj。详情 `ui/VENDOR.md`。

[40288873546](https://github.com/wu162/NewSdk/actions/runs/40288873546)已通过，记录 `ui/evidence/source-consistency.json`：附件PDB与原程序集GUID相符；390/390非生成C#源文件SHA256相同。原始程序集和源码版统一在.NET8 x86下，样例普通RA3模式全部5个文件、优化模式全部8个文件逐字节一致。优化APT/CONST/DAT=10121/9787/48字节，未优化=25861/25179/48。未复现完整.NET5原生宿主，样例没有几何/图片/音视频，SDK/游戏验收未做。

用户批准上传工具，最小原工具包在 `ui/reference/original-managed-tools.zip`。新框架是x64宿主，APT仍必须保留原32位相对偏移，不能用宿主指针宽度替换文件布局。旧memory原型及字符串补丁只作历史参考，正式实现从导入源码改造。

## 下一阶段

1. 将原flasm拆为内存优化API，静态链接进原生桥。保留RA3选项、迭代和错误语义，移除eaf/result/flm/tac文件依赖；全局状态先串行隔离，不直接宣称并发安全。
2. 将AptConverter/SwfLib迁入ISwfBackend，保留原序列化算法，改字节输入/内存资源输出；补普通/优化模式完整样例差分后再接通compile。同步处理AOT/裁剪问题，不屏蔽诊断。最终输出writer也必须明确接入后才能返回成功。
3. triangle/JPEG/zlib改为同架构静态库，固定C ABI和内存所有权；C++异常、exit、跨托管栈longjmp不得逃逸。替换System.Drawing及文件纹理路径，并补视觉样例差分。
4. 前端展开已有APT资产，接入单资产AssetBuffer路由及最终MOD流；取得manifest/bin/relo/imp对照后再改生产配置。当前接口无路径依赖，为这一步保留入口。
5. 原样导入时的PDB源码校验只适用于基线。主动改源后保留基线证据与原工具，按实际迁移范围做差分，不改原工具指纹来制造相同。

独立ui交接，不加载recovery旧任务；复用有效证据，不重测未变化XmlCompiler类型/完整SDK。禁止强推；合入须PR并经用户明确批准。本轮普通推送、未合入。框架验收后仅更新文档与证据，不重复同源码Windows测试。
