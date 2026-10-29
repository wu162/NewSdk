# 新 SDK 大中小 MOD 测量：脚本准备完成，Windows 待运行

工作分支 `recovery/legacy-sdk-cache`，禁止强推。生产代码未改；沿用已经通过短验证的性能打点版本。本轮新增 Release 单次构建及三档输入测量，结果尚未取得。

小：完整当前 mymod；中：历史 Upheaval 中 96 个无继承/跨资产依赖的真实 Texture 子集及其原 ART 文件（不是独立中型完整 MOD）；大：完整历史 Upheaval，来源 `bacfc69f`、树 `5551fc26`，2,723 文件/约 287MiB。均使用默认缓存、自动发现全部内容、BIG/SKUDEF/Additional，冷一次/热两次，detailed，进程 180 秒/测量总预算 900 秒；失败组不重试热态，其余组继续并保留失败报告。

测量脚本 `MeasureCompilerModScale.py` 本地准备预检通过，包含输入清单和来源、实际规模、逐次阶段/编译器计数及输出一致性。Windows 工作流 `compiler-mod-scale.yml` 待运行；不能把历史组件 Upheaval 通过冒充本次新 SDK 大型 MOD 已通过。

前序性能打点：默认 `--performance off`；`summary`/`detailed` 写入 `build-report.json` 的可选 `performance.entries`。源码 `23994960` / [Windows 40886567000](https://github.com/wu162/NewSdk/actions/runs/40886567000) success，Release 生产和 App 合同通过，14 次小输入短测实际 8.02 秒。[说明](docs/PERFORMANCE.md)、[归档](evidence/refactor/performance/40886567000-1/summary.json)。
