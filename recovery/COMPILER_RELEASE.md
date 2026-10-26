# 编译器最小发行包

`recovery/scripts/PublishCompiler.ps1` 为 P6 的发布入口。它要求已有成功的 Debug/Release 真实生产 CLI 验收收据；构建成功本身不能发布。已取得 Debug/Release Windows 运行及独立解压证据；当前接受版本与后续候选以 `REFACTOR_RESUME.md` 和对应 Actions 区分。

```powershell
./recovery/scripts/PublishCompiler.ps1 `
  -CompilerDirectory recovery/artifacts/compiler-tests/<build-id> `
  -AcceptanceReceipt recovery/evidence/refactor/cli/<run-id>/validation.json `
  -OutputDirectory recovery/artifacts/releases/<release-id>
```

需要 Windows、PowerShell 7、Python、现有 `TextureRedirect/requirements.txt` 和 `app-protocol-requirements.txt` 依赖，以及构建外部 CLI 控制器所需的 VS Roslyn/.NET Framework 4.8 targeting pack。发布脚本不重建生产 EXE、不重新修补 Texture，也不修改 SDK。调用者提供的 CLI 收据必须有两个成功配置、完整二进制源码提交和五文件实际运行哈希；若其使用历史二进制，源码来源保留为 `binarySourceCommit`，不冒充发布脚本当前提交的新构建。

每个 ZIP 根目录只有以下五个文件：

| 文件 | 用途 |
| --- | --- |
| `NewModCompiler.exe` | 包含已恢复业务代码的 Win32 编译器 |
| `NewModCompiler.exe.config` | .NET Framework 4.8 混合运行配置 |
| `antlr.runtime.dll` | 固定版本的必要解析运行库 |
| `BinaryAssetBuilder.TextureCompiler.dll` | 直接引用该 EXE 的兼容 Texture 实现 |
| `BinaryAssetBuilder.TextureCompiler.dll.patch.json` | 原版、补丁、EXE 身份及受保护 PE 审计清单 |

旧业务 DLL、测试宿主、补丁工具、控制器、对象文件和临时 SDK 均不入包。每配置的 `.manifest.json` 放在 ZIP 外，记录五文件长度/SHA256、ZIP 哈希、生产构建与 CLI 收据来源、工具链、补丁清单及运行依赖说明；`release.json` 是整轮终态。运行依赖复用现有 G2 的导入保护审计和运行证据（见 `recovery/probes/TextureRedirect/README.md`），不扩展新的依赖审计。目标机器仍须具备 Windows x86 兼容环境、.NET Framework 4.8、原混合 Texture 所需 x86 VC80 CRT、与构建工具链匹配的 x86 VC 运行库，以及既有 DirectX 9/D3DX 环境。这些系统组件不随包复制；hosted runner 上通过不能推断裸 Windows 已具备它们。

发布前从 ZIP 解压至独立短目录，以现有单进程控制器分别运行原 bin 和解压 bin 的真实 `doctor` 与小型 Armor MOD。逐字节比较四个链接输出，检查进程、JSONL 生命周期、报告、精确产物与运行文件不变，最后用现有协议 Schema 校验器验收两个配置。验证证据保存在 `evidence/`，不进入 ZIP。仅两配置全部通过后生成正式 ZIP 文件名和清单；失败保留错误收据与候选 ZIP，不把候选标为正式发行物。

这个门槛证明五文件包的基本部署与构建能力。完整 P5/G3/SDK、完整 UI/runtime、Unicode Texture、BIG/skudef 和 P7 均需各自证据，不能由本脚本的小型 MOD 推断完成。

`PrepareCompilerRelease.py` 固定复用已成功的三字节 APT 版本修正生产归档；重新核对完整生命周期的成功步骤、归档 SHA256、两配置收据、协议原始文件与五文件运行身份。它还直接解析 Debug/Release 的 APT、APT+maps 生产 BIG 和原 MakeBig BIG，共八份原始归档，要求所有 APT `.version` 精确为 `20 0d 0a`；两字节旧候选不能通过。独立 HashFix/LoD 准备文件不属于生产链接输入，允许保留；生产源码、SDK、构建脚本或依赖改变仍拒绝复用。准备成功仅表示可运行 ZIP 部署验收，正式包仍必须通过后续发布步骤。

完整 mymod 证据继续保留原二进制及测试来源；本次候选只对已声明的受影响组取得新验收，不能把历史完整 MOD 结果改标为当前二进制重测。后续 base 后处理候选需要自己的生产验收与重新固定来源；不得使用该 APT/maps 归档发布未验证的 base 能力。

解压后的典型调用（SDK 与 MOD 单独提供）：

```powershell
./NewModCompiler.exe doctor --sdk-root C:\NewSdk\SDK
./NewModCompiler.exe build --sdk-root C:\NewSdk\SDK --mod C:\Mods\Example --out C:\Build\Example --package-content base,apt,maps
```
