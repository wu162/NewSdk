# 当前命令行与旧参数范围

`compiler-main` 的结果合同，2026-10-27。这是必要旧参数核对结果；原始全量设计不代表所有 BAB 开关均已开放。未支持项必须报错，不能静默忽略。

| 用途 | 当前接口 / 处理 |
| --- | --- |
| MOD 与 SDK | `--mod <目录>`、`--sdk-root <目录>`，不依赖旧 GUI 或反复启动 BAB |
| 最终输出 | `--out`，兼容 `/od`；诊断文件用 `--log`、`--events`、`--report` |
| 内容 / 格式 | `--package-content base,apt,maps`、`--package-format loose|big` |
| 输入根 | `/dr`、`/data`、`/art`、`/audio`，保留原根搜索语义并有原工具差分 |
| 两种缓存 | `/bc:true|false`、`/sc:true|false`，默认关闭；混合 Texture 不支持会话缓存 |
| 平台 / 链接 | `/tp:Win32`、`/ls:true`；其他平台、非链接流明确拒绝 |
| 诊断级别 | `/tl:0..9`、`/el:0`；`/el:1` 未开放 |
| MOD 包版本 | `--mod-version 2.7-beta`，默认 `1.0`；对应旧脚本 GUI 的 modversion，不把 `/vf` 当作此参数 |
| SKUDEF 基名 | `--skudef-name Launcher`，默认 MOD 目录名；对应旧脚本 GUI 的 skudefname |
| 游戏版本字段 | `--game-version 1.12`，默认与旧脚本常量相同；仅控制 SKUDEF 的 mod-game 字段，不能证明其他游戏版本已兼容 |
| Additional / 中立修复 | `--include-additional=true`、`--fix-neutral-assets=true`，默认关闭，详细输入条件见 P7_PACKAGING |
| 缓存高级模式 | `/csc`、`/psc`、`/fsc`、旧缓存目录覆盖及 touch/慢清理等尚未开放；不因旧解析器认识该名称而宣称已实现 |
| 其他旧后端能力 | 自定义 Schema/中间目录、BuildConfiguration、基础补丁流、自定义后缀、非链接/稳定排序、预编译、替代输出、资产报告/字符串哈希等非默认请求明确拒绝；另行需求驱动实现 |
| 旧宿主 | GUI、驻留、监视、出错暂停、旧单文件入口不纳入此产品 |

版本/名称选项仅用于 BIG，显式用于 loose 时拒绝。MOD 版本允许 ASCII 字母、数字、点、下划线及连字符；游戏版本为点分十进制；SKUDEF 名称为单个 ASCII 文件基名。均不能注入目录或换行指令。

`newmodcompiler.json` 可配置 package-content、package-format、mod-version、game-version、skudef-name、include-additional、fix-neutral-assets 和 tp。命令行覆盖项目预设，同一来源同一参数指定冲突值时报错。所有生效值和来源进入报告及事件。

```json
{"package-content":"base,apt,maps","package-format":"big","mod-version":"2.7-beta","game-version":"1.12","skudef-name":"ExampleLauncher"}
```

上例 MOD 目录名为 Example 时，发布 `Example_2.7-beta.big` 和 `ExampleLauncher_2.7-beta.skudef`；SKUDEF 精确包含 ASCII/CRLF 的 `mod-game 1.12` 与 `add-big Example_2.7-beta.big`。APT 内部 `.version` 仍为原脚本的空格+CRLF，与 MOD 包版本不同。
