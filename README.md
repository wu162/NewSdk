# NewSdk — NewModCompiler

`compiler-main` 是 2026-10-27 起的重构结果与后续开发主线。保留旧分支历史，不将本次成果合入 `recovery/compiler-source`；旧 PR #13 关闭且不合并。此处的开发主线约定不等同于 GitHub 仓库网页默认分支设置。

## 获取工具

最终工具由 [Core lifecycle 工作流](https://github.com/wu162/NewSdk/actions/workflows/refactor-core-lifecycle.yml?query=branch%3Acompiler-main) 在 Windows 构建。仅成功通过生产与独立 ZIP 验收后，产物中的 `final-release` 目录才有正式 Debug/Release ZIP 和哈希清单；通常使用 Release。运行编号及固定下载入口见 [发行说明](recovery/COMPILER_RELEASE.md)。当前新分支最终构建正在准备，不能把旧包称为新分支重编译结果。

每个工具 ZIP 解压后只有五个运行文件：EXE、EXE.config、antlr.runtime.dll、TextureCompiler.dll 和其补丁身份清单。SDK、MOD 源码和系统运行库单独准备。要求 Windows x86 兼容环境、.NET Framework 4.8、x86 VC80/构建工具链 VC 运行库及 DirectX 9/D3DX；先运行 doctor 检查环境。

## 使用

```powershell
./NewModCompiler.exe doctor --sdk-root C:\NewSdk\SDK
./NewModCompiler.exe plan --sdk-root C:\NewSdk\SDK --mod C:\Mods\Example --out C:\Build\Example --package-content base,apt,maps --package-format big
./NewModCompiler.exe build --sdk-root C:\NewSdk\SDK --mod C:\Mods\Example --out C:\Build\Example --package-content base,apt,maps --package-format big
```

`--mod` 指向 MOD 文件夹。默认选择 `base,apt,maps`，默认 `--package-format loose`；BIG 为显式选择。成功构建的最终文件位于输出根下 `published`，默认 BIG/SKUDEF 名称为 `<MOD目录名>_1.0`。构建同时输出日志、JSONL 事件及最终报告，适合后续界面调用；不启动旧 GUI 或 BAB 子进程。

- `--mod-version 2.7-beta`、`--game-version 1.12`、`--skudef-name ExampleLauncher`：设置 BIG/SKUDEF 身份，默认值保持旧脚本行为。
- `/bc:true`：启用构建资产缓存，默认关闭。
- `/sc:true`：启用会话缓存，默认关闭；含混合 Texture 的流明确拒绝此选项。
- `--include-additional=true`：BIG 中先复制 SDK/Additional，再由 MOD/Additional 覆盖；默认关闭，排除 `.asset`。
- `--fix-neutral-assets=true`：仅 base BIG 可用的显式中立修复；默认关闭，要求已有完整 Worldbuilder 参考四文件，缺失时报错。

布尔值使用等号，或单独裸开关启用；不要写成 `--include-additional true`。详细包布局见 [P7 打包合同](recovery/P7_PACKAGING.md)。

## 已完成与范围

核心编译、单进程 base/apt/maps、日志/协议、构建与会话缓存、默认 BIG/SKUDEF、Additional、纯 C# MakeBig 入 EXE 和五文件包已有验收。中立修复已覆盖组件差分及生产零插入链，完整游戏插入不在已验证范围内。原工具的特定 LoD 来源表尾字节差异见 [说明](recovery/P7_LOD_ORIGINAL_TAIL.md)。

AssetMerge 已按用户要求排除。WBData 下载、提取、读取器与依赖真实 WBData 的验收已取消，均不作为本次重构完成条件。完整 UI/runtime、中文纹理路径、混合纹理会话缓存仍有未覆盖范围。

版本与必要旧参数已形成[明确合同](recovery/CLI_COMPATIBILITY.md)，LoD 对原工具越界采用完整表复制的确定性修复，专项与最终生产验收正在准备；不把这些未完成项目表述为全功能兼容。当前不支持的参数明确拒绝。压缩/部分/冻结会话缓存、旧缓存目录覆盖、预编译及替代输出/资产报告等选项尚未开放；并非每个旧 BAB 参数都已支持。开发续接见 [REFACTOR_RESUME](recovery/REFACTOR_RESUME.md)，历史实施设计见 [重构方案](recovery/NewSdk_ModCompiler_Refactor_Plan.md)。
