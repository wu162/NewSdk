# 默认构建与参数设计验收

日期：2026-10-27（北京时间）。本轮基于 `compiler-main` 的 `933cf4a21e72808e03ffdd3b69bcd851a929775d`，成果分支为 `feature/compiler-build-defaults`，PR 为 [#14](https://github.com/wu162/NewSdk/pull/14)，尚未合并。

生产及测试源码：`8b3e146cb5220e830ff6a0ce4718f47af052f166`。
验收运行：[40446975355](https://github.com/wu162/NewSdk/actions/runs/40446975355)，状态：**已通过（success）**。后续仅文档提交不改变二进制或测试来源。

Debug/Release 的 `SDK_RESULT` 均为 succeeded=true、bigSucceeded=true、原始输出 differences=0。原始证据与最终包在 [artifact 12708969379](https://github.com/wu162/NewSdk/actions/runs/40446975355/artifacts/12708969379)；归档 SHA256：`4354fd09d9390e4224152fc48b2ca9ff302cde5dcda01f244d2e3c74c4ad12e9`。

## 实现范围

- 标准构建默认自动发现 `Data/Mod.xml`、`Data/AptUI/*.xml`、`Data/AdditionalMaps/MapMetaData_*.xml`，生成 BIG + SKUDEF。APT/元数据单独存在也可编译；自动发现没有编译入口时以 `PLAN_NO_INPUTS` 失败。
- BIG 默认包含 Additional，先 SDK、后 MOD，覆盖已编译包成员的时点、大小写规则及 `.asset` 排除沿用旧脚本；以 `ADDITIONAL_OVERWRITE` 输出覆盖警告。`--include-additional=false` 关闭。
- `standard`/`development`/`advanced` 仅提供默认值，CLI > 项目 > 默认。开发默认 loose，可以沿用包身份；显式 loose 继续原校验。SDK/输出路径可保存到项目，相对项目路径基于 MOD；旧数值版本写法保留。
- 对外使用 `global-metadata`，接受 `maps` 别名；内部任务 ID、目录、selectedContent 协议保留 maps。
- `/ss`、`/pc`、`/slowclean` 默认 true；Win32/链接行为不新增普通设置。中立修复默认 false，游戏版本默认 1.12。
- 显式内容选择、空可选组保留旧发布行为，未调整部分编译/完整打包机制。缓存参数、默认值和实现未改；原流级测试仅补充显式 loose，防止新打包默认值改变原测试范围。未引入 AssetMerge 或 WBData 工作。

## 验证范围

上述运行全部门槛已通过，原始日志、收据、产物及 SHA256 保存在对应 Actions artifact：

- Debug/Release 应用合同：自动发现、无输入拒绝、显式部分选择、项目路径与优先级、模式、版本、旧别名、Additional 覆盖警告、失败/取消/发布回滚。
- 完整生产构建、Core/原 BAB 基线、Texture、真实 CLI 与协议；原构建缓存/会话缓存、资源根、CDATA 回归保持原断言。
- `TestCompilerBig.ps1`：保留显式关闭 Additional 的旧基线；新增无选择/格式参数的标准构建，与原 LoD→CopyFolders→MakeBig 对照；开发默认 loose 与显式 loose 流字节比较。独立 ZIP 使用同组测试。
- `TestCompilerSdk.ps1`：完整、未精简的仓库 mymod，Debug/Release 对原 BAB 比较原始流/CDATAs；另以新默认命令生成 BIG，对照原 CopyFolders、LoD 和 MakeBig 的全部成员及 SKUDEF。

BIG 比较指精确成员名称及成员原始内容，不将此扩大为所有 BIG 容器索引/排列字节必须相同。没有新增完整 UI/runtime、游戏内运行或真实中立资产插入的验收。

## 保留的失败历史

- [40445955355](https://github.com/wu162/NewSdk/actions/runs/40445955355)：旧路径测试隐含 loose 默认，改为显式 loose 后修复；新增报告断言同时改为读取既有 effectiveOptions。
- [40446211014](https://github.com/wu162/NewSdk/actions/runs/40446211014)、[40446435367](https://github.com/wu162/NewSdk/actions/runs/40446435367)：部署探针的中文 MOD 目录同样隐含 loose，已为原测试入口显式指定 loose，未放宽 BIG 原有名称边界或路径断言。
