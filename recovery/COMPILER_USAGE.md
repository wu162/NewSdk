# NewModCompiler 使用说明

适用开发分支：`recovery/legacy-sdk-cache`（本轮状态见 [续接入口](RESUME.md)，源码依据见 [并行输入说明](docs/TOKENIZER_PARALLEL_INPUT.md)）；历史发行主线为 `compiler-main`。目标平台为 Windows/Win32、.NET Framework 4.8；本工具负责从现有 MOD 源码生成流文件或 BIG/SKUDEF，不包含旧 GUI。最终发行状态与下载链接见 [发行说明](COMPILER_RELEASE.md)。新增包版本参数的最终验收结果以该页为准。

## 后端并行与旧缓存

`--jobs auto|<正整数>` 默认 auto，表示 CPU 和当前 Win32 可用内存共同约束的 worker 上限；`--jobs 1` 保留 owner 串行对照，`--jobs 4` 允许最多四个已认证后端任务。三个 profile、plan/build 和 `newmodcompiler.json` 的 `"jobs": "auto"` 或整数均支持。修改 jobs 不改变原缓存身份。

并行资格包括 GameObject、WeaponTemplate、ArmorTemplate、W3DAnimation/W3DMesh、精确内建编码器的 NONE/XAS AudioFile（无显式 PCSampleRate）、MP3 passthrough、284 个认证 XML metadata TypeId 和四类 APT。资源不足、外部输入无法取得只读租约、APT 非 ASCII 路径，以及未认证后端均自动走原 owner 路径。Core XML 前端、验证、缓存及输出提交保持 owner 顺序。

默认 `/sc:true /bc:true /csc:true /psc:true /fsc:false /atc:false` 不变，Texture 原 SDK 缓存继续正常使用；无需关闭 SessionCache 来使用多 worker。`--sc=false --bc=false --jobs 1|4` 可作纯冷态吞吐对照。没有新增动作缓存、额外持久结果或文件哈希体系。

## 1. 安装与检查

本分支已验证的五文件 ZIP 位于 [功能验收归档40915444929](https://github.com/wu162/NewSdk/actions/runs/40915444929/artifacts/12916669037) 内 `artifacts/final-release/40915444929-1/NewModCompiler-Win32-Release.zip`（生产源码6d39f833）。本次通过 Actions 归档提供，未发布新的 Release；也可按仓库构建脚本自行编译。完整解压到单独目录，保留其中全部五个文件。不要只复制 EXE，也不要用 SDK 的原 Texture DLL 覆盖包内已适配的文件。

目标机器需要 Windows x86 兼容环境、.NET Framework 4.8、x86 VC80 CRT、与编译工具链匹配的 x86 VC 运行库以及 DirectX 9/D3DX。SDK 资源和 MOD 源码单独提供；工具 ZIP 不包含完整 SDK，也不下载游戏文件。

在 PowerShell 中进入工具目录，先执行：

```powershell
./NewModCompiler.exe --help
./NewModCompiler.exe doctor --sdk-root "C:\NewSdk\SDK"
```

`doctor` 检查 SDK 配置、Schema 和实际编译依赖，不编译资产。成功会输出 `CORE_ENVIRONMENT_OK`；失败时先按标准错误中的诊断代码处理依赖。

## 2. 准备 MOD 文件夹

`--mod` 必须指向 MOD 目录，不是单个 XML、manifest 或 babproj。默认自动识别以下入口：

| 内容 | MOD 内入口 | 行为 |
| --- | --- | --- |
| `base` | `Data/Mod.xml` | 自动模式下存在才编译；显式选择后必须存在 |
| `apt` | `Data/AptUI/*.xml` | 只发现该目录直接子文件，按稳定顺序逐流编译 |
| `global-metadata`（兼容旧名 `maps`） | `Data/AdditionalMaps/MapMetaData_*.xml` | 只发现该目录直接子文件 |

被入口引用的数据、贴图、模型、音频等资源仍须完整；选择内容不会自动修复缺失的资产依赖。APT XML 是现有 UI 编译入口，本工具不会直接将 FLA、HTML 或脚本工程转换成 APT。

输出目录必须与 SDK、MOD 输入目录分离。例如 `C:\Build\Example`，不要放在 `C:\Mods\Example` 内部。为不同 MOD 使用各自输出根。

## 3. 查看计划与编译

先看计划（只发现流，不实际编译）：

```powershell
./NewModCompiler.exe plan --sdk-root "C:\NewSdk\SDK" --mod "C:\Mods\Example" --out "C:\Build\Example"
```

编译成 BIG：

```powershell
./NewModCompiler.exe build --sdk-root "C:\NewSdk\SDK" --mod "C:\Mods\Example" --out "C:\Build\Example"
```

仅编译基础数据并输出松散流：

```powershell
./NewModCompiler.exe build --sdk-root "C:\NewSdk\SDK" --mod "C:\Mods\Example" --out "C:\Build\ExampleLoose" --package-content base --package-format loose
```

默认使用标准构建：`--package-content auto`，格式 `big`，自动包含 Additional。只有 APT 或全局元数据的 MOD 也能直接编译，无需创建空的 Mod.xml。自动发现没有任何有效编译入口时以 `PLAN_NO_INPUTS` 报错；仅有 Additional 不算编译入口。

| 构建方式 | 用法 | 默认输出 / 用途 |
| --- | --- | --- |
| 标准构建 | `build`（或 `--profile standard`） | 自动识别内容，BIG + SKUDEF，自动包含 Additional |
| 开发构建 | `--profile development` | 自动识别内容，输出 loose 流文件，便于迭代 |
| 高级构建 | `--profile advanced`、`--help-advanced` | 标准默认值，按需使用兼容开关与显式内容选择 |

模式只提供默认值，显式参数和项目设置优先；三个模式均不改变任何缓存选项。开发模式派生的 loose 输出允许保留项目的包版本/名称设置，这些设置只在以后标准打包时使用；显式 `--package-format loose` 的原有校验继续保留。

`--package-content base|apt|global-metadata` 或逗号组合仍提供原有的部分编译，旧名 `maps` 完全兼容。显式选择为空的可选组时继续保留上一代，不生成空 BIG；没有改变部分编译和完整打包机制。

成功产物位于输出根的 `published` 目录；具体流文件路径和 SHA256 从 `build-report.json` 的 `artifacts` 获取。成功非空构建替换整个已发布目录，请勿手动放入需要长期保留的文件。受控失败、取消或发布故障会回滚上一代。

## 4. 包名、版本和附加文件

```powershell
./NewModCompiler.exe build --sdk-root "C:\NewSdk\SDK" --mod "C:\Mods\Example" --out "C:\Build\Example" --package-format big --mod-version 2.7-beta --game-version 1.12 --skudef-name ExampleLauncher --include-additional=true
```

| 选项 | 默认值 / 效果 |
| --- | --- |
| `--mod-version` | `1.0`，生成 `<MOD目录名>_<版本>.big` |
| `--game-version`（高级） | 默认固定 `1.12`，仅需兼容其他版本时手动设置 |
| `--skudef-name` | MOD 目录名，生成 `<基名>_<版本>.skudef`，其中 add-big 引用真实 BIG 文件名 |
| `--include-additional=false` | BIG 默认自动包含，手动关闭时使用 false；先取 `SDK/Additional`，后由 `MOD/Additional` 同名文件覆盖，排除 `.asset` |

以上示例发布 `Example_2.7-beta.big` 和 `ExampleLauncher_2.7-beta.skudef`。版本/名称选项仅用于 BIG；MOD 版本使用 ASCII 字母/数字/点/下划线/连字符，SKUDEF 名称为单个 ASCII 文件基名。当前 BIG 也要求 MOD 目录基名为 ASCII。游戏版本字段可配置，不代表已验证其他游戏版本的实际运行。

Additional 的相对目录进入 BIG 根；它能够覆盖已编译文件及 APT 版本文件，这是保留的旧脚本行为。发生 SDK/MOD 同名覆盖或 Additional 覆盖已编译包成员时，日志、JSONL 和报告记录 `ADDITIONAL_OVERWRITE` 警告；仍按旧顺序继续打包。中立资产修复 `--fix-neutral-assets` 继续默认关闭。默认 base BIG 包含 `data/mod.*` 与低/中 LoD 流；APT、maps 和 CDATA 布局见 [打包说明](P7_PACKAGING.md)。APT 的 `.version` 空格/CRLF 标记与 MOD 包版本是两件事。

布尔值使用 `--include-additional=true`、`--include-additional=false`，或单独裸开关启用；不要写成 `--include-additional true`。

## 5. 项目预设

可在 MOD 目录放置 `newmodcompiler.json`：

```json
{
  "sdk-root": "C:/NewSdk/SDK",
  "out": "C:/Build/Example",
  "mod-version": "2.7-beta"
}
```

保存后通常只传 MOD 路径（需要时再覆盖版本号）：

```powershell
./NewModCompiler.exe build --mod "C:\Mods\Example" --mod-version 2.8
```

命令行覆盖项目预设，预设覆盖默认值；同一来源的冲突参数会报错。SDK 与输出路径可保存到项目，JSON 中相对路径以 MOD 目录为基准；命令行路径仍以调用目录为基准。还可保存 `profile`、内容选择、BIG 设置以及 `ss`、`pc`、`slowclean`（完整列表见 [参数兼容表](CLI_COMPATIBILITY.md)）；后三者默认 true，可用 false 覆盖。Win32 与链接流属于固定行为，旧 `/tp:Win32`、`/ls:true` 调用仍接受。不要将其他旧 BAB 参数写入 JSON。

## 6. 缓存与资源根

稳定排序 `/ss`、预编译引用策略 `/pc` 和慢清理 `/slowclean` 默认开启，与旧编译脚本一致，通常不必手动填写；确有需要可分别传入 `/ss:false`、`/pc:false`、`/slowclean:false`。这些选项不依赖缓存开关。

本分支恢复原 SDK 缓存默认值：会话 `/sc`、资产 `/bc`、部分会话 `/psc`、压缩 `/csc` 均默认 true；冻结 `/fsc` 和始终 touch `/atc` 默认 false。SDK 配置中的 `buildCache` 和 `buildCacheRoot` 仍生效，显式命令行优先。旧 defaultscript 显式使用 `/csc:false`，可用相同参数输出 XML：

```powershell
./NewModCompiler.exe build --sdk-root "C:\NewSdk\SDK" --mod "C:\Mods\Example" --out "C:\Build\Example" /csc:false
./NewModCompiler.exe build --sdk-root "C:\NewSdk\SDK" --mod "C:\Mods\Example" --out "C:\Build\Example" /sc:false /bc:false
```

`/sc` 按原文档、文件依赖、定义和继承复用解析与中间结果；`/bc` 按原类型/实例哈希复用资产，包括三种 Texture 路由。修改、新增、删除 MOD 输入均交由原 Core 判断，不再将整个 SDK/MOD 内容指纹作为冷失效开关。最终链接和发布仍重新执行。

`/csc:false` 使用原 `BinaryAssetBuilder.SessionCache.xml`，true 使用 `.xml.deflate`。默认会话目录为输出根，默认资产目录为输出根 `cache`，默认中间目录与旧 SDK 一样为输出根。`/scd`（别名 `/scp`）、`/bcd`（别名 `/bcp`）、`/iod` 可指定独立目录，支持读取原 SDK 格式；新宿主的 CDATA 状态保存在会话根 `.NewModCompiler/cdata`，可导入没有新侧车的旧目录。以上设置也可保存到项目预设。

`/psc:true` 允许中止后保留已完成的会话工作，失败或取消仍返回失败/取消，且不发布部分 MOD。`/psc:false` 将会话更新跟随本次发布结果确认。`/fsc:true` 读取旧会话但不更新会话文件，资产与中间文件仍按原 Core 行为处理。`/atc:true` 即使复用中间资产也更新对应网络资产缓存文件的时间。详细实现与验证状态见 [原 SDK 缓存恢复说明](docs/LEGACY_INCREMENTAL_CACHE.md)。

BIG/SKUDEF 包缓存已移除；每次构建都会从本轮编译结果重新打包，`/sc`、`/bc` 不会跳过打包。若新包与已发布文件相同，通用发布流程仍可保留现有输出；失败或取消继续回滚。

资源搜索 `/dr`、`/data`、`/art`、`/audio` 保留原根与顺序，包含配置的外部资源根。清理时可删除选定缓存目录中的会话 XML/Deflate、其 `.old`、资产文件及 `.NewModCompiler` 状态；保留 `published` 可保留上一代正式产物。

## 7. 日志、取消与自动化

默认诊断文件均在输出根：`build.log`、`build.events.jsonl`、`build-report.json`。可以使用 `--log <文件>`、`--events <文件>`、`--report <文件>` 改位置，但不得覆盖输入、published、工作区或彼此冲突。当前不支持用 `--events -` 向标准输出发送事件。

自动化应同时核对退出码、最终报告的 `status/exitCode` 和本次 `buildId`。JSONL 按 `sequence` 排序，以 `BuildFinished` 为正常终态；不要把上次文件或仅“进程消失”当作构建成功。协议细节见 [协议文档](protocol/README.md)。

| 退出码 | 含义 |
| --- | --- |
| 0 | 成功；是否产生新产物还要看报告的 publicationStatus |
| 2 | 参数或计划错误 |
| 3 | 环境或必需依赖缺失 |
| 4 | 输入、编译或打包失败 |
| 5 | 内部异常 |
| 6 | 合作式取消 |
| 7 | 必需日志、报告或输出提交失败 |

可用 Ctrl+C/Ctrl+Break 请求合作式取消；原生调用可能需要返回安全边界后才响应。强制终止或崩溃可能没有终态，调用方应识别为异常中断。工具不会自动启动游戏、安装 MOD 或写 CurrentMOD.ini。

## 8. 常见问题与已知范围

| 诊断 / 现象 | 处理 |
| --- | --- |
| `PLAN_BASE_MISSING` | 补齐 Data/Mod.xml，或只选择实际存在的 apt/maps 内容 |
| `CLI_OUTPUT_OVERLAP` | 把 --out 改到 SDK/MOD 外的独立目录 |
| `PACKAGE_IDENTITY_FORMAT` | 包版本/名称选项必须与 --package-format big 一起使用 |
| `PACKAGE_IDENTITY_INVALID` | 检查版本或文件基名是否含目录、换行或非法字符 |
| `PACKAGE_NAME_UNSUPPORTED` | 将 MOD 文件夹基名改为 ASCII 后使用 BIG |
| `PACKAGE_NEUTRAL_REFERENCE_MISSING` | 关闭可选中立修复；只有环境已提供完整参考文件时才启用 |
| `CORE_OPTION_UNSUPPORTED` | 按 CLI 兼容表移除未支持的旧选项，不能认为解析器认识名称就代表支持 |
| doctor 失败 | 保留标准错误诊断，检查 SDK 路径及 .NET/VC/DirectX/包内五文件是否完整 |

`--fix-neutral-assets=true` 默认关闭，仅限包含 base 的 BIG；需要已有完整 Worldbuilder 参考文件。WBData 下载、提取和依赖其真实数据的验收已按要求取消，不会自动为你补齐。已有验证为组件差分及生产零插入链，不能当作完整游戏中立资产验收。

LoD 明确修复原工具的表尾越界：完整保留逻辑名称/来源表，不复制越界产生的尾字节；详见 [兼容策略](P7_LOD_ORIGINAL_TAIL.md)。完整 UI/runtime、中文纹理路径、混合纹理会话缓存仍有未覆盖范围。AssetMerge 不属于新流程。

## 性能耗时分析

默认关闭，使用 `--performance summary` 查看阶段耗时，`--performance detailed` 查看内部文档/资产/哈希耗时；`--performance off` 关闭。结果聚合到构建报告的 `performance.entries`，不自动循环构建。字段口径、示例和短测试预算见 [性能打点](docs/PERFORMANCE.md)。
