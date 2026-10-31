# NewSdk — NewModCompiler

`compiler-main` 是 2026-10-27 起的重构结果与后续开发主线。保留旧分支历史，不将本次成果合入 `recovery/compiler-source`；旧 PR #13 关闭且不合并。此处的开发主线约定不等同于 GitHub 仓库网页默认分支设置。

## 获取工具

最终工具由 [Core lifecycle 工作流](https://github.com/wu162/NewSdk/actions/workflows/refactor-core-lifecycle.yml?query=branch%3Acompiler-main) 在 Windows 构建。仅成功通过生产与独立 ZIP 验收后，产物中的 `final-release` 目录才有正式 Debug/Release ZIP 和哈希清单；通常使用 Release。运行编号及固定下载入口见 [发行说明](recovery/COMPILER_RELEASE.md)。本次新主线最终构建与独立ZIP验收已成功：[运行40429363386](https://github.com/wu162/NewSdk/actions/runs/40429363386)。二进制来自新增版本/LoD收尾后的源码，文档整理随后提交，不改变运行文件。

本分支将 Texture 编译器源码内建到 EXE，新构建的工具 ZIP 只有三个运行文件：NewModCompiler.exe、NewModCompiler.exe.config、antlr.runtime.dll。SDK、MOD 源码和系统运行库单独准备。要求 Windows x86 兼容环境、.NET Framework 4.8 及与构建工具链匹配的 x86 VC 运行库；Texture 不再依赖原 DLL、VC80 或 DirectX 9/D3DX。先运行 doctor 检查环境。上面的历史验收包仍保留五文件布局，新实现的验收状态见 [续接入口](recovery/RESUME.md)。

## 使用

完整操作步骤见 [新编译器使用说明](recovery/COMPILER_USAGE.md)，开发和历史资料见 [文档入口](recovery/README.md)。

本分支内建 [FLA/AS UI 编译插件](recovery/docs/FLA_UI_COMPILER.md)：可自动发现 `Data/AptUI/` 下的二进制 Flash MX 2004/8 FLA，与对应 AS 直接编译为 MOD 四流，也可由 `AptAptData` XML 声明与其他资产混合编译。

```powershell
./NewModCompiler.exe doctor --sdk-root C:\NewSdk\SDK
./NewModCompiler.exe plan --sdk-root C:\NewSdk\SDK --mod C:\Mods\Example --out C:\Build\Example
./NewModCompiler.exe build --sdk-root C:\NewSdk\SDK --mod C:\Mods\Example --out C:\Build\Example
```

`--mod` 指向 MOD 文件夹。默认自动发现 base、APT UI、全局元数据，输出 BIG + SKUDEF；开发模式 `--profile development` 默认 loose。`--package-content global-metadata` 兼容旧名 `maps`。成功构建的最终文件位于输出根下 `published`，默认 BIG/SKUDEF 名称为 `<MOD目录名>_1.0`。构建同时输出日志、JSONL 事件及最终报告，适合后续界面调用；不启动旧 GUI 或 BAB 子进程。

- `--mod-version 2.7-beta`、`--game-version 1.12`、`--skudef-name ExampleLauncher`：设置 BIG/SKUDEF 身份，默认值保持旧脚本行为。
- `/bc:true|false`：构建资产缓存默认开启，保留原 SDK 类型/实例哈希布局。
- `/sc:true|false`：会话缓存默认开启，三种 Texture 路由可复用；`/csc`、`/psc` 默认 true，`/fsc`、`/atc` 默认 false。
- `--include-additional=false`：BIG 默认自动包含 SDK/Additional，再由 MOD/Additional 覆盖；使用 false 可关闭，排除 `.asset`，覆盖冲突记录警告。
- `--fix-neutral-assets=true`：仅 base BIG 可用的显式中立修复；默认关闭，要求已有完整 Worldbuilder 参考四文件，缺失时报错。

布尔值使用等号，或单独裸开关启用；不要写成 `--include-additional true`。详细包布局见 [P7 打包合同](recovery/P7_PACKAGING.md)。

## 已完成与范围

核心编译、单进程 base/apt/maps、日志/协议、构建与会话缓存、默认 BIG/SKUDEF、Additional、纯 C# MakeBig 入 EXE 和五文件包已有验收。中立修复已覆盖组件差分及生产零插入链，完整游戏插入不在已验证范围内。原工具的特定 LoD 来源表尾字节差异见 [说明](recovery/P7_LOD_ORIGINAL_TAIL.md)。

AssetMerge 已按用户要求排除。WBData 下载、提取、读取器与依赖真实 WBData 的验收已取消，均不作为本次重构完成条件。完整 UI/runtime、中文纹理路径仍有未覆盖范围；原 SDK 缓存完整恢复的当前验证状态见 [缓存说明](recovery/docs/LEGACY_INCREMENTAL_CACHE.md)。

版本与必要旧参数已形成[明确合同](recovery/CLI_COMPATIBILITY.md)，LoD 对原工具越界采用完整表复制的确定性修复，专项与最终生产/ZIP验收已通过；不把这些未完成项目表述为全功能兼容。当前不支持的参数明确拒绝。压缩/部分/冻结会话、touch、原缓存目录和 `/iod` 已接入本分支，完整生产缓存矩阵、mymod 原版差分及独立 ZIP 门槛已通过[运行 40814401237](https://github.com/wu162/NewSdk/actions/runs/40814401237)，准确覆盖边界见缓存说明；额外预编译及替代输出/资产报告等选项尚未开放；并非每个旧 BAB 参数都已支持。开发续接见 [REFACTOR_RESUME](recovery/REFACTOR_RESUME.md)，历史实施设计见 [重构方案](recovery/NewSdk_ModCompiler_Refactor_Plan.md)。
