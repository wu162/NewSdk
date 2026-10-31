> 参数设计更新（2026-10-27）：标准构建默认 BIG/SKUDEF、自动发现输入并包含 Additional；开发模式 `--profile development` 默认 loose，高级模式 `--profile advanced` 使用标准默认值。显式选项与项目预设优先于模式默认值。`global-metadata` 是新名称，`maps` 为兼容别名；内部任务 ID/报告/输出目录保持 `maps`。`--help-advanced` 查看高级入口。本分支已接回原 SDK 缓存默认值与高级能力，验证状态见 docs/LEGACY_INCREMENTAL_CACHE.md。项目还可保存 SDK/输出路径、模式及 ss/pc/slowclean。最新操作见 [使用说明](COMPILER_USAGE.md)。

# 当前命令行与旧参数范围

`compiler-main` 的结果合同，2026-10-27。这是必要旧参数核对结果；原始全量设计不代表所有 BAB 开关均已开放。未支持项必须报错，不能静默忽略。

| 用途 | 当前接口 / 处理 |
| --- | --- |
| MOD 与 SDK | `--mod <目录>`、`--sdk-root <目录>`，不依赖旧 GUI 或反复启动 BAB |
| 最终输出 | `--out`，兼容 `/od`；诊断文件用 `--log`、`--events`、`--report` |
| 内容 / 格式 | `--package-content base,apt,maps`、`--package-format loose|big` |
| 输入根 | `/dr`、`/data`、`/art`、`/audio`，保留原根搜索语义并有原工具差分 |
| 旧脚本输出策略 | `/ss:true|false`、`/pc:true|false`、`/slowclean:true|false` 均默认 true，独立于缓存；分别控制稳定排序、引用流预编译策略和慢清理 |
| 两种缓存 | `/bc:true|false`、`/sc:true|false`，默认开启；三种 Texture 路由支持资产与会话缓存 |
| 平台 / 链接 | `/tp:Win32`、`/ls:true`；其他平台、非链接流明确拒绝 |
| 诊断级别 | `/tl:0..9`、`/el:0`；`/el:1` 未开放 |
| MOD 包版本 | `--mod-version 2.7-beta`，默认 `1.0`；对应旧脚本 GUI 的 modversion，不把 `/vf` 当作此参数 |
| SKUDEF 基名 | `--skudef-name Launcher`，默认 MOD 目录名；对应旧脚本 GUI 的 skudefname |
| 游戏版本字段 | `--game-version 1.12`，默认与旧脚本常量相同；仅控制 SKUDEF 的 mod-game 字段，不能证明其他游戏版本已兼容 |
| Additional / 中立修复 | Additional 在 BIG 中默认开启，可用 `--include-additional=false` 关闭；`--fix-neutral-assets=true` 仍为显式启用，默认关闭，详细输入条件见 P7_PACKAGING |
| 缓存高级模式 | `/csc`、`/psc` 默认 true；`/fsc`、`/atc` 默认 false；`/scd`/`/scp`、`/bcd`/`/bcp`、`/iod` 支持原目录和格式 |
| 其他旧后端能力 | 自定义 Schema、BuildConfiguration、基础补丁流、自定义后缀、非链接、替代输出、资产报告/字符串哈希等非默认请求明确拒绝；另行需求驱动实现 |
| 旧宿主 | GUI、驻留、监视、出错暂停、旧单文件入口不纳入此产品 |

`/pc:true` 使用原 Core 的引用策略：优先尝试预编译引用，缺失时沿原逻辑处理引用而不额外发布它们。它不代表自动导入游戏数据或 WBData；当前私有工作区里不存在的参考 manifest 不会凭空生成。`/pc:false` 可恢复编译并输出引用流的旧选项行为；含多个输出流的 BIG 仍受现有打包合同约束。

版本/名称选项仅用于 BIG，显式 `--package-format loose` 时仍拒绝。开发模式自动派生 loose 时允许保留这些设置供标准打包使用，不将它们写入流。MOD 版本允许 ASCII 字母、数字、点、下划线及连字符；游戏版本为点分十进制；SKUDEF 名称为单个 ASCII 文件基名。均不能注入目录或换行指令。

`newmodcompiler.json` 可配置 package-content、package-format、mod-version、game-version、skudef-name、include-additional、fix-neutral-assets 、tp、profile、sdk-root、out、ss、pc 和 slowclean。命令行覆盖项目预设，同一来源同一参数指定冲突值时报错。所有生效值和来源进入报告及事件。

```json
{"package-content":"base,apt,maps","package-format":"big","mod-version":"2.7-beta","game-version":"1.12","skudef-name":"ExampleLauncher"}
```

上例 MOD 目录名为 Example 时，发布 `Example_2.7-beta.big` 和 `ExampleLauncher_2.7-beta.skudef`；SKUDEF 精确包含 ASCII/CRLF 的 `mod-game 1.12` 与 `add-big Example_2.7-beta.big`。APT 内部 `.version` 仍为原脚本的空格+CRLF，与 MOD 包版本不同。
