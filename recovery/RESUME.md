# 可关闭性能打点：实现并通过 Windows 短验证

工作分支 `recovery/legacy-sdk-cache`，禁止强推，未合入其他分支。

新增 `--performance off|summary|detailed`，默认 off。结果聚合到构建报告可选 `performance.entries`，按阶段、流、资产类型记录 count/totalMs/selfMs/maxMs；关闭时不读性能时钟、不分配 scope、不建立计时字典。summary 覆盖初始化、Schema、缓存、Core、打包/发布；detailed 覆盖文档/XML、资产编译/验证/提交、哈希与链接，Schema 再分读取解析/编译/依赖整理。[使用及计时边界](docs/PERFORMANCE.md)。

生产/测试来源 `239949602a8cc361fd6fb4f3bdb226e84abc6723`；[Windows 40886567000](https://github.com/wu162/NewSdk/actions/runs/40886567000) success：Release 生产组合重建、全部独立 App 合同通过。实际测量 8.02 秒/14 次（总预算 60 秒、单进程 10 秒），三档各冷一次/热两次，小型两 Texture/一 Armor include 流与原版逐字节相等，三档 BIG/SKUDEF 相同，无效 XML 保留失败计时。[归档摘要和代表报告](evidence/refactor/performance/40886567000-1/summary.json)；原 ZIP 摘要已复算匹配。

热态零 XML 解析、零资产编译、零完整文件哈希读取；详细档 Schema 读取解析 127–138ms，编译 23–26ms，依赖整理 14–16ms，已能定位热态主要成本。off/summary/detailed 热态进程中位数 445.85/449.10/454.11ms，n=2、OS 页缓存不受控，不能当作精确打点开销估计；这是小输入 Release 验证，不冒充完整 MOD/Debug 验收。

首轮 [40885632072](https://github.com/wu162/NewSdk/actions/runs/40885632072) 保留 failure 原终态：生产和 App 合同通过、三档冷态输出相同，但漏测真实 internal Commit 入口；已移动打点并在上述运行重验。首轮 Schema 244ms 推动本轮拆分粒度。

前序四项优化及完整 Debug/Release/mymod 验收仍为 `639a5ff7` / [40878062262](https://github.com/wu162/NewSdk/actions/runs/40878062262)，新旧 SDK 短比较为 `0c6ea99f` / [40880915003](https://github.com/wu162/NewSdk/actions/runs/40880915003)，来源/范围见 [增量缓存说明](docs/LEGACY_INCREMENTAL_CACHE.md)。本次未重新执行这些历史完整矩阵。
