# NewSdk Mod Tools

在 **Windows 本地 VS Code 1.96 或更新版本**中配置和编译 NewSdk MOD。支持工作区信任检查；远程窗口、WSL、容器和其他操作系统不属于 P0 范围。

从仓库 `main` 开发；编译器命令和协议参考 `recovery/legacy-sdk-cache` 的 `680c4f0c4439bd7435addebfc57a1a16c3c320e3`。VSIX 不包含编译器，也不从源码生成编译器。

## 使用

1. 安装 `NewSdk-Mod-Tools-P0.vsix`，在 VS Code 中打开 MOD 目录。
2. 点击左侧 NewSdk 图标，进入配置。选择已有 `NewModCompiler.exe`，或输入已打包编译器的 HTTPS ZIP 下载链接。需要认证时，认证信息保存在 VS Code SecretStorage；链接不预设。
3. 选择 SDK 和输出目录，填写 MOD 版本。输出目录必须与 MOD 和 SDK 完全分离。全局路径保存在用户设置，项目输出和版本保存在扩展存储。
4. 执行“检查编译环境”和“查看编译计划”，然后点击“编译 MOD”。原始输出显示在任务终端；工作台显示阶段、数据流数量、问题及本轮报告确认的产物。

`newmodcompiler.json` 只读。插件不创建 `.vscode`，不修改 MOD 源文件，不搬运或清理 SDK 的 `SuperCompilerCache`。编译使用单个编译器进程，参数使用数组传递；默认 `auto` 内容发现、BIG/SKUDEF 和 Additional，保留编译器自己的 jobs 和缓存设置。

同一物理 MOD 在多个 VS Code 窗口中也只能运行一个任务。项目锁保存在当前用户的系统临时目录；只有确认宿主和编译器进程均已退出时才回收旧锁。计划记录使用 `plan.events.jsonl`、`plan-report.json`、`plan.log`，不会覆盖最近一次构建的记录。

停止按钮使用 VS Code 任务终止 API，显示为**强制中断**。只有编译器的完整事件与报告明确返回 `cancelled/6` 才显示正常取消。成功必须同时有进程退出码 0、本轮 `BuildFinished`、同一 `buildId` 的成功报告。缺少报告、崩溃和旧报告都不会显示为成功。

没有真实位置的诊断保留在扩展问题列表，不伪造 Problems 面板行号。进度不显示资产百分比或 ETA。产物来自本轮报告，不扫描目录认领旧文件。

## 开发与验证

```powershell
cd vscode-extension
npm ci
npm test
npm run package
npm run test:host # Windows；启动真实 VS Code 扩展宿主
```

Windows 工作流 `.github/workflows/vscode-p0.yml` 执行协议、配置、ZIP 边界和真实扩展宿主测试，打包 VSIX，并上传测试结果。宿主测试使用单独编译的协议假编译器检查计划、错误、缺失报告、停止、重新编译、只读源目录和跨进程配置持久化；假编译器不是生产编译器。

真实编译器由仓库 Secrets 提供：

| Secret | 含义 |
| --- | --- |
| `NEWSDK_COMPILER_URL` | 直接下载编译器 ZIP 的 HTTPS 链接；未配置时明确跳过真实测试 |
| `NEWSDK_COMPILER_SHA256` | 可选 SHA-256；未提供时记录下载文件的实际哈希 |
| `NEWSDK_COMPILER_AUTHORIZATION` | 可选 HTTP `Authorization` 值；仅用于下载，不写进日志或源码 |

常规提交在提供 ZIP 后执行一个小型 Armor 数据流的真实编译冒烟测试。手动运行 `full_acceptance` 会增加 base、APT 入口、global-metadata 入口和完整内容发现组合；这些路由测试使用小型 Armor XML，**不等于真实 APT 业务内容验收**。测试不会重建生产编译器，也不重复完整 Upheaval 基准。

完整 P0 验收仍需已提供编译器包上的真实 APT 业务用例、正常 Ctrl+Break 取消等方案要求的证据。未提供编译器包、或这些门槛未通过时，不能将本插件标记为 P0 全部完成。下载来源、SHA-256、真实事件、报告和日志随 Actions 产物保存。
