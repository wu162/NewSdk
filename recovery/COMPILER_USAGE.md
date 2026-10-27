# NewModCompiler 使用说明

适用主线：`compiler-main`。目标平台为 Windows/Win32、.NET Framework 4.8；本工具负责从现有 MOD 源码生成流文件或 BIG/SKUDEF，不包含旧 GUI。最终发行状态与下载链接见 [发行说明](COMPILER_RELEASE.md)。新增包版本参数的最终验收结果以该页为准。

## 1. 安装与检查

下载 Release ZIP，完整解压到单独目录，保留其中全部五个文件。不要只复制 EXE，也不要用 SDK 的原 Texture DLL 覆盖包内已适配的文件。

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

开发构建使用 `--profile development`，默认输出 loose，适合查看流文件；高级构建使用 `--profile advanced`，保持标准默认值并通过显式参数微调。`--help-advanced` 查看兼容入口。模式只提供默认值，显式参数优先；三个模式均不改变任何缓存选项。

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
  "profile": "standard",
  "package-content": "auto",
  "mod-version": "2.7-beta",
  "game-version": "1.12",
  "skudef-name": "ExampleLauncher",
  "include-additional": true,
  "fix-neutral-assets": false,
  "tp": "Win32"
}
```

保存后通常只传 MOD 路径（需要时再覆盖版本号）：

```powershell
./NewModCompiler.exe build --mod "C:\Mods\Example" --mod-version 2.8
```

命令行覆盖项目预设，预设覆盖默认值；同一来源的冲突参数会报错。SDK 与输出路径可保存到项目，JSON 中相对路径以 MOD 目录为基准；命令行路径仍以调用目录为基准。还可保存 `package-format`、`ss`、`pc`、`slowclean`；后三者默认 true，可用 false 覆盖。Win32 与链接流属于固定行为，旧 `/tp:Win32`、`/ls:true` 调用仍接受。不要将其他旧 BAB 参数写入 JSON。

## 6. 缓存与资源根

稳定排序 `/ss`、预编译引用策略 `/pc` 和慢清理 `/slowclean` 默认开启，与旧编译脚本一致，通常不必手动填写；确有需要可分别传入 `/ss:false`、`/pc:false`、`/slowclean:false`。这些选项不依赖缓存开关。

构建资产缓存和会话缓存默认关闭，可独立启用：

```powershell
./NewModCompiler.exe build --sdk-root "C:\NewSdk\SDK" --mod "C:\Mods\Example" --out "C:\Build\Example" --package-content base /bc:true /sc:true
```

`/bc` 复用合格的资产编译结果；`/sc` 恢复文档及 intermediate/CDATA 后重新生成链接输出。缓存位于输出根的 `.cache` 下，修改输入或参数会按缓存合同失效。中途失败和取消不得确认新缓存。

含混合 Texture 的流目前不支持 `/sc:true`，改用 `/sc:false`；`/bc:true` 不表示贴图缓存已经开放。不要直接沿用旧 BAB 的缓存目录。需要清理时，在没有构建运行的情况下删除对应输出根的 `.cache` 即可，保留 `published` 可保留上一代产物。

确需改变资源搜索位置时可使用 `/dr`、`/data`、`/art`、`/audio`。默认已前置 MOD 的相应资源目录；缓存启用时要求实际解析输入位于允许的 SDK、MOD 或编译器运行目录范围。旧参数支持范围见 [CLI 兼容表](CLI_COMPATIBILITY.md)。

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
