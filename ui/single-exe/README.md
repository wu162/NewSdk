> **已停止采用：仅供历史参考。** 用户最终确定改为类似XmlCompiler的新编译器插件，取消独立EXE、CLI及SWF专用测试EXE规划。当前规则和下一步以 [../RESUME.md](../RESUME.md) 为准。以下构建命令、发布目标和迁移设想记录的是已被替代的单EXE实验，不再作为实施指令或正式验收目标。

# 历史单 EXE 重构实验

当前阶段只搭框架：**可构建并独立运行的Native AOT EXE、静态原生桥、SWF结构检查和内存编译契约**。原SWF→APT转换、优化、三角化、JPEG实现尚未迁入；`compile`明确返回退出码4，不生成假产物。

## 构建与运行

Windows x64，安装.NET8 SDK、VS2022 C++ v143和Windows SDK。运行：

```powershell
./ui/single-exe/Publish.ps1
./ui/.work/single-exe-publish/SwfCompiler.exe --help
./ui/.work/single-exe-publish/SwfCompiler.exe inspect ./ui/fixtures/CoronaUIMain.swf
```

发布入口先构建 `SwfCompiler.Native.lib`，再用Native AOT将C#与该静态库链接为 `SwfCompiler.exe`。分发只需要EXE；PDB若由工具链产生，属于可选调试文件，不作为运行依赖。发布目录不应出现工具DLL、辅助EXE或.NET运行时文件。

初始目标为win-x64；游戏APT格式仍应按原32位相对偏移序列化，不能跟随编译进程指针宽度改变。后续若需要额外宿主平台，单独验证。

退出码：0为检查/帮助成功，2为参数错误，3为输入/结构不支持，4为转换后端未接入，1为框架/ABI等内部错误。

## 模块边界

| 位置 | 职责 | 当前状态 |
|---|---|---|
| `src/Program.cs` | 参数、输入文件、命令及诊断 | 已实现框架；compile未启用 |
| `src/SwfInspector.cs` | 有大小/嵌套限制的FWS/CWS结构检查 | 已实现；不代表支持相应tag语义 |
| `src/Compiler.cs` | 输入字节+选项→内存资源组；不依赖文件路径 | 契约已建；ISwfBackend尚无转换实现 |
| `src/NativeBridge.cs` | C ABI入口、版本与已迁移能力 | ABI v1；能力位为0 |
| `native/` | 静态C/C++库，后续容纳flasm/triangle/jpeg/zlib | 当前只实现真实版本/能力查询 |
| `tests/smoke.py` | 单EXE隔离运行、输入检查及拒绝假编译成功 | 按Windows Actions实际结果验收 |

`CompilationResult`按资源类型、名称和可选AptID保存字节。原生调用返回的临时分配不能逃逸到结果中，后端负责拷贝/释放，结果有效期不依赖输入或某次原生调用。最终资产流写入由CLI/BAB上层负责；本轮不预设新容器格式。

## 当时的迁移设想（现已失效）

1. 直接基于 `../vendor/swf2apt/` 已采用源码，将flasm命令入口拆成内存优化API。保留RA3模式、迭代与错误行为；移除对eaf/result/flm/tac文件的依赖。全局状态先串行隔离，再评估可重入，不立即宣称并发安全。
2. 将AptConverter/SwfLib接入ISwfBackend。复用原算法，替换文件/目录/hash边界；样例的普通/优化模式先与原始工具逐字节比较，再启用compile。AOT反射/裁剪问题必须实修，不屏蔽IL2026/IL3050警告。
3. triangle、JPEG、zlib按目标架构构建静态库并接入同一C ABI。几何用内存数组，图像用明确像素布局；C++异常、exit和跨托管栈longjmp不得逃出接口。JPEG失败处理需在原生层收敛为诊断。
4. 替换System.Drawing与文件纹理管线，保持颜色、透明度、缩放和几何输出的差分；新增视觉样例后再宣称支持图形UI。
5. 接入MOD资产收集与AssetBuffer输出。保留旧工具和 `../evidence/source-consistency.json` 作为原始基线，不把当前壳的构建成功当成转换或SDK成功。

原生ABI下一阶段增加功能时使用固定宽度状态码、调用者/库各自明确的内存所有权及长度。接口头当前只声明已实现的版本/能力函数，不提供返回假成功的转换占位实现。

技术依据：[Native AOT部署](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)、[DirectPInvoke与NativeLibrary静态链接](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/interop)。

本目录新增代码按GPL-3.0-only提供，许可证文本见 `../COPYING`；导入代码保留各自原声明。
