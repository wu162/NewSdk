# 历史交接快照（非当前状态）

以下为旧 REFACTOR_RESUME.md 原文，保留技术约束与调查记录。**不要在启动时加载。** 后续提交已实现验证宿主及 fixture 接线；其中“尚未实现”“未接工作流”等描述已过时。当前状态只看 REFACTOR_RESUME.md，验证结论以对应源码的 Actions 收据为准。

---

# 新 MOD 编译器续接：P4 验证宿主待修复

工作与交付分支：`refactor/modcompiler-p0-p3`；目标：`recovery/compiler-source`。
现有草稿 PR #13；未经用户明确批准不得合入，禁止强推。
实施方案：[NewSdk_ModCompiler_Refactor_Plan.md](NewSdk_ModCompiler_Refactor_Plan.md)。

2026-10-26 本轮按用户“先交接”停止。**P4 未完成验收，不能宣称新入口已可构建真实 MOD。**
本页只交接源码、状态及续接动作；沿用已有约束，不公开新增运行编号、指纹或产物链接。原始收据与日志保留在本分支 Actions 产物中，续接先读实际收据，不因缺少公开摘要重跑基线。

## 本轮已保存的实现

- `build/Compiler.Native.json`：明确的 Native、Bridge、Application 包装源码清单，公共实现只编译一次。
- `scripts/BuildCompilerNative.ps1`：87 个 Native 编译单元；68 个 Bridge 生成副本汇入单一 `CompilerBridges.cpp`，生成一个 Bridge 对象。只在生成副本中把 Core 导入改为 Foundation 模块并固定 include 路径，原恢复算法未改。
- `scripts/BuildCompiler.ps1`：Foundation → Native/Bridge → Application → 生产 EXE；C# 只导入这个统一 Bridge 对象。明确的 Plugin 包装和 W3X 辅助源码进入 Application，使用 `NEWSdk_MONOLITHIC`。
- `src/App/Application/BuiltinCompilers.cs`、`BuiltinXmlRoutes.cs`：6 个显式工厂、299 路由；XML 267 个全局／22 个非全局登记项边界不变。缺失、重复、未知及非 Win32 明确拒绝；`CreateWithTexture` 显式增加三条贴图路由。
- `src/App/Compatibility/LegacyTextureAdapter.cs`、`scripts/PrepareLegacyTexture.ps1`：从固定原件生成发行副本，检查 EXE 身份、补丁清单、文件哈希和受保护 PE 区域；限定 Win32、三条路由和单次释放。不覆盖 SDK 原件。
- `tests/CompilerRoutes.cs`、`CompilerDeployment.cs`、`scripts/TestCompiler.ps1`：路由、类型、资源、干净目录及生产 CLI 检查，**动态检查目前受下述 CLR 限制阻塞**。
- `.github/workflows/refactor-native.yml`：显式 `[refactor-native]` 或 dispatch 的 P4 验证，仓库只读权限、保留日志／收据上传，不运行完整 SDK。

## 已实际观察到的状态与阻塞

1. 首轮各 C++ 编译单元编译成功，但 C# 同时导入多个 Bridge 对象产生 `CS0101`（重复 std／C++ 元数据）。已通过上面的单一 Bridge 编译单元修复。
2. 后续运行中，Foundation Debug／Release 构建成功；**Debug 生产 EXE 已完成统一链接、只读元数据检查和贴图副本静态审计**。
3. 随后外部 `CompilerDeployment.exe` 加载生产 EXE 时失败：
   `System.IO.FileLoadException / 0x80131019: Attempt to load an unverifiable executable with fixups (IAT ... or a TLS section.)`。
   这是验证宿主外部加载混合 EXE 的限制；没有证明生产 CLI 本身失败，也没有证明它已通过本轮运行检查。
4. 测试在 Debug 动态检查处停止；**Release 生产 EXE、后续差分及贴图适配器运行均未因此通过**。保留失败收据，不盲目重试原脚本。
5. 以下外部 fixture 脚本同样需要调整加载方式，不能直接运行后宣称已验收。

## 下一步：修正验证宿主，再集中运行

尚未实现／提交的方案：

- 新增 `BuildCompilerValidationHost.ps1 -ProductBuildDirectory <build/Debug> -OutputDirectory <build/Debug/validation>`。
- 复用生产构建中完全相同的 Foundation／Application 模块、Native／Bridge 对象及资源；仅另加测试专用 C++/CLI 入口，在独立目录输出同程序集名的 `validation/bin/NewModCompiler.exe`。不要重编或替换业务实现。
- 测试入口接收纯托管 fixture EXE 路径与参数，通过 `Assembly.LoadFrom` 加载该 fixture 并调用其 EntryPoint；此时业务混合 EXE 本身已是进程入口，不再从外部装载它。
- 验证宿主收据至少包含 sourceCommit、configuration、productExeSha256、exeSha256、sameProductionObjects、每个输入对象／模块哈希及入口信息；检查生产 EXE 前后哈希不变。
- `TestCompiler.ps1` 在隔离 obj 前生成宿主；类型／路由及组件差分由宿主执行。真正的生产 CLI 仍直接运行原生产 EXE，生产产物仍做静态元数据／资源检查。收据必须区分两者，不能把测试宿主 SHA 当作生产 SHA。
- 原生产 EXE **不得加入测试入口或测试类**；不得改写 TLS／IAT、unsafe 标记，或放回恢复业务 DLL 绕过限制。
- 同步调整下列 fixture 脚本，核对生产与验证宿主两份收据，在验证宿主进程内运行 fixture；开始／结束检查 Core、Bridge、Plugin 同属入口单模块 EXE，无旧业务程序集。以上接线尚未完成。

## 已保存但尚未运行通过的组件验证

| 入口 | 准备范围及限制 |
| --- | --- |
| `TestCompilerDifferential.ps1` | 复用 SingleExe Probe 的 Armor／Hash 四平台、三缓冲区、布局与拒绝样本；目前仍按外部 fixture 方式执行，需接新宿主 |
| `TestCompilerGameObject.ps1` + `GameObjectTranscript.cs` | 原46文件完整矩阵，独立原版录制／D-R重放、布局／拒绝／异常与完整计数；需接新宿主 |
| `TestCompilerWeapon.ps1` + `WeaponTranscript.cs` | 原8文件完整矩阵、正式包装、注释输入、直接位集解析与拒绝；需接新宿主 |
| `TestW3xProduct.ps1` | 复用已有可信完整原版 capture，重放动画／网格及元数据；需接新宿主，并在 workflow 接入旧 artifact 下载与祖先检查所需历史 |
| `TestLegacyTextureAdapter.ps1` | 已有独立测试专用统一入口；原件对照、失败后复用、单次关闭、损坏部署拒绝；新适配器测试本轮尚未执行 |
| `PrepareCompilerXmlSmoke.py`、`TestCompilerXmlSmoke.ps1` | 已保存未验证准备，未接工作流；保持289路由默认样本及 completion 的四平台联合 payload 检查，仍需改宿主。**这只是联合 smoke，不是各 XML 组完整边界／拒绝矩阵** |

GameObject、Weapon 的历史默认测试路径保持原有语义；新增条件编译转录仅用于隔离原版与统一 EXE 类型身份。
W3X 旧 capture 已核实原始收据、转录哈希、祖先关系及相关原始输入一致；原始编号仍可从既有 W3X 证据入口与 Actions 读取。新执行脚本继续严格校验，不能无条件复用。
XML 各受影响组完整差分仍待逐组安全适配；不能用联合 smoke 冒充全部 P4 验收。

## 必须保留的约束

- Windows x86、.NET Framework 4.8、固定 Roslyn、VS2022 x86 C++/CLI；构建期 C# 模块与工具使用 AnyCPU，最终链接 `/MACHINE:X86`。
- Foundation 不反向依赖 Bridge；所有恢复业务实现进入物理 EXE，不能嵌入后解包 DLL 或依赖隐藏搜索路径。
- Core Schema／MvpXml 资源名与字节、Utility 签名修饰符、EXSLT 名称修复保留；最终混合 EXE 只读检查。
- 贴图保留固定原件、元数据 token／共享字符串、原生段、导入与重定位；不得无证据退回 Core 转发 DLL。
- 保留必要 ANTLR；旧 GUI、Remoting、驻留 BAB、旧程序集名反射发现不进生产链。
- `plan` 仍仅入口发现／显式依赖计划；生产 `build` 仍明确 `BACKEND_NOT_INTEGRATED`，退出码3。真实 Core 执行、Include 闭包、缓存、失败／取消清理、输出提交属于后续 P5；P6／P7不冒进。
- 复用已核对的 P0–P3／贴图历史证据；本轮没有重复完整 SDK 基线。链接变化后的新组合必须另验，历史成功不能改标为当前代码成功。
- 所有成果保持本分支。用户已要求本轮停止；下一轮代理数量由用户重新指定。本轮没有合入 PR，也没有触发新的完整 SDK 验证。
