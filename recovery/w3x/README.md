# W3XCompiler 源码还原

本工作线从 `recovery/compiler-source` 的 `c66339e49e32cfe3f44aa2f9f1757f9993bbac7a` 创建，分支为 `recovery/w3xcompiler-source`。这是新的还原目标，不接续旧 XmlCompiler / Tokenizer / GameObject 待办；后续成果需要合回原工作线，允许复用和共同重构。

目标是恢复 `SDK/tools/BinaryAssetBuilder.W3XCompiler.dll` 的真实托管、C++/CLI 与原生业务行为，最终恢复其 BAB 插件入口。`BinaryAssetBuilder.W3XCompiler.Plugin` 是配置指定的**类型名**，所在程序集是 `BinaryAssetBuilder.W3XCompiler`，不是第二个 DLL。不以机械反编译文件、接口壳或转发原 DLL 代替源码恢复。

先读 [RESUME.md](RESUME.md)，已恢复C#范围见 [MANAGED.md](MANAGED.md)，再按需要读 [BASELINE.md](BASELINE.md)、[PLAN.md](PLAN.md)、[ARCHITECTURE.md](ARCHITECTURE.md) 和 [VALIDATION.md](VALIDATION.md)。本目录只保存项目说明、进度和规划；源码、测试、脚本及原始证据继续使用整个 recovery 的共同结构。

| 位置 | 用途 |
| --- | --- |
| `recovery/src/Native/W3x/` | W3X 布局、动画、网格处理的业务实现 |
| `recovery/src/Bridge/W3x/` | C++/CLI 与 BAB 对象/缓冲区边界 |
| `recovery/src/Managed/W3x/` | 经原版核对的元信息等托管逻辑 |
| `recovery/src/Plugin/W3x/` | 正式插件入口，待行为恢复后接入 |
| `recovery/tests/W3x*` | 原版取证、差分与独立布局/拒绝验证 |
| `recovery/scripts/*W3x*` | 取证、构建、组件验证、SDK 验证入口 |
| `recovery/evidence/w3x/runs/<runId>-<attempt>/` | 不可变原版证据及运行清单 |

取证入口是 `recovery/scripts/ExtractW3x.ps1` 和 `.github/workflows/w3xcompiler-evidence.yml`，建立原 DLL 基线与插件合同清单。以 VS2022 x86 开发者 PowerShell 运行取证脚本，需要 .NET 8、.NET Framework 4.x 和网络安装固定版本 ILSpy。本地输出目录每次唯一；脚本拒绝覆盖证据。

恢复版托管层入口 `recovery/scripts/TestW3xManaged.ps1`，使用.NET Framework csc编译 `Recovered.W3x.Managed.dll`，跑优化开/关两种模式的原版差分和无原W3X DLL元信息重放，已通过。它包含元信息、生命周期和分派，**尚不能编译动画/网格或接管SDK**；具体sourceCommit/runId见RESUME。

持续遵守根 `AGENTS.md`、`recovery/EFFICIENCY.md` 和 `STAGE_DURATION.md` 的用户授权、禁止强推、完整能力组合批、阶段检查点、SDK 降频与精确验证锚点要求。用户最新要求效率优先、先直接恢复C#；证据明确时直接实现并集中验证，不为流程扩充规划。旧类型的进度和验收数据不能计入 W3X 还原进度。
