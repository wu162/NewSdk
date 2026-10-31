# 当前：Texture 原身份与并行编译（Windows 全量验证中）

分支 `feat/texture-plugin`，[PR #39](https://github.com/wu162/NewSdk/pull/39)，
基线 `recovery/legacy-sdk-cache` / `074bf768`。用户要求保持原 DLL 身份：版本
`4202`，Texture/OnDemand/Atlas ProcessingHash 分别为 `2673862832`、
`4165508788`、`1182597302`，已恢复；原 DLL 对照不再使用身份替换包装。

三路 Texture 已纳入现有有界并行调度：输入只读租约和头部预算、私有 CDATA、
S3 可变状态 TLS、工作线程消息由 owner 顺序回放。无法准备或预算不足沿用 owner。
生产/测试源码 `8fe12ca9b`，
[Windows 41185772818](https://github.com/wu162/NewSdk/actions/runs/41185772818) 运行中：
Debug/Release 构建、Core/worker/失败回收、64并行缓存矩阵、原三路完整转录、
真实 CLI/取消及常见格式通过。D/R 均实测4个贴图任务重叠，jobs1/4/auto输出相同，
31个格式用例原始结果与上轮全等，16个贴图CLI用例通过；已下载复算独立
[common收据](evidence/texture/plugin-validation/41185772818-1/common-summary.json)。
完整缓存/SDK/ZIP/Upheaval 仍在执行，暂不标本轮全量验收通过。

此前生产源码 `4c74bf64b` 的
[Windows 41180859852](https://github.com/wu162/NewSdk/actions/runs/41180859852) 已整体通过，
只代表之前串行和新身份版本；历史证据保留，不覆盖。本轮完成后在
[TEXTURE_PLUGIN](docs/TEXTURE_PLUGIN.md) 记录新 sourceCommit/runId/产物。
运行目录仍为 EXE、config、ANTLR 三文件；七种 schema 输出和已知 BC1 字节差异
范围不变。保持原身份意味着既有缓存可复用，切换编码实现需使用干净或独立目录
来验证新字节；不自动清理缓存。未经用户批准不合并，禁止强推。
