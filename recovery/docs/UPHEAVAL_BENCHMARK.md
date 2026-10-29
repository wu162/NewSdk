# 完整 Upheaval：新编译器与旧 SDK 的编译速度

2026-10-28（北京时间），[实测运行 40604851281](https://github.com/wu162/NewSdk/actions/runs/40604851281) 全部成功，工作流总耗时 31 分 42 秒。**本次完整 Upheaval clean compile/link，新编译器平均 74.002 秒，旧 SDK 平均 88.918 秒；耗时减少 16.77%，速度比为 1.202×。**

## 正式计时结果

同一 Windows runner，1 对预热后排除预热数据，10 对正式样本，交替旧→新 / 新→旧，不剔除异常值。

| 指标 | 原 SDK BAB | NewModCompiler Release |
| --- | ---: | ---: |
| 正式样本数 | 10 | 10 |
| 平均耗时（秒） | 88.917995 | 74.002477 |
| 中位数（秒） | 88.953275 | 73.998823 |
| 标准差（秒） | 0.183746 | 0.205805 |
| CV | 0.207% | 0.278% |
| p95（秒） | 89.115630 | 74.296047 |
| 最短 / 最长（秒） | 88.522359 / 89.152289 | 73.691466 / 74.426991 |
| 辅助：平均进程 CPU 秒 | 88.706250 | 73.651563 |
| 辅助：平均峰值工作集（MiB） | 168.653 | 206.417 |

每次平均节省 14.916 秒。旧/新平均 wall time 的比值为 **1.201554×**；配对 bootstrap 95% 区间 **[1.199490, 1.203620]×**（10000 次重采样，seed 162），本轮区间不包含 1。约 20.16% 的处理速度提升与 16.77% 的耗时减少使用不同分母，不混称为“耗时少 20%”。峰值工作集有所增加；旧侧取每次五个进程的最大峰值，新侧是单进程峰值，这不是整个进程树或系统内存统计。

## 工作量与有效性

- 固定原始 SDK 提交 `bacfc69fc3e71c8116d1f22b933b8b6173170ecf` 的 Upheaval tree `5551fc26a6f96b62613443e0d0fa2958c14cbce0`。完整 2723 文件、300900885 字节，194 个随包提供的 `.asset` 输入全部保留，没有精简 XML、替换资源或添加公开探针样例。
- 实际五个入口：`Data/Mod.xml`、三个 APT（TacticalHUDBuildQueuePage / TacticalHUDCommandListBox / TacticalHUDMainCommandBar）、`Data/AdditionalMaps/MapMetaData_Global.xml`。旧侧逐流启动原 BAB，新侧一次正式 CLI 编译全部入口；这体现完整 MOD 命令的实际差异，不单独归因于某个序列化器或进程启动优化。
- 两侧 Win32、相同 Schema 和搜索路径、`pc/ss/slowclean=true`；关闭生成的编译/会话缓存，每次全新输出和进程。保留预编译输入不等于复用前一次样本产生的缓存。OS 文件缓存经预热，不声称冷磁盘或热 JIT 测试。
- **22 次构建全部成功；每次 1311 个输出、77873601 字节全部身份一致**，其中五条流各 manifest/bin/imp/relo，共 20 文件，加 1291 个 CDATA。预热也接受同一门槛。SDK、MOD、两侧运行文件前后哈希未变。
- 环境：`windows-2022`，Windows Server build 20348，4 logical CPU，AMD64 Family 25 Model 1，Python 3.12.10。计时含进程启动与正式 CLI 的正常报告/事务开销，排除输入复制、目录准备和哈希校验。
- 测量仅包含编译/链接，**不包含 LoD 后处理、Additional、BIG/SKUDEF、GUI、游戏启动或增量缓存**。同一 hosted runner、单一 MOD 的结果不能外推到所有机器、MOD 或端到端发行流程；置信区间只描述本轮样本。

## 来源、收据与复跑

二进制源码为 `8b3e146cb5220e830ff6a0ce4718f47af052f166`，复用完整成功的 [40446975355](https://github.com/wu162/NewSdk/actions/runs/40446975355) 正式五文件 Release 包。已核对祖先/生产输入闭包、原始归档 SHA256、Actions 来源及 EXE-bound Texture 身份；未重建或修改生产编译器。测试源码独立记录为 `c98507a0bdc90f7a5dbdbe37353a17ecba765fc2`，后续仅增加报告提取及文档，不冒充新二进制来源。

持久保存的 [机器可读统计和全部原始计时样本](UPHEAVAL_BENCHMARK_RESULT.json) 包含运行环境、输入 Git tree/清单哈希、二进制身份、预热/正式顺序、55 个旧进程及 11 个新进程的计量、输出门槛和证据来源。统计只使用 10 对正式样本；55/11 是包含预热后的进程数。

- [原始完整证据包](https://github.com/wu162/NewSdk/actions/runs/40604851281/artifacts/12782007171)：约 98 MiB，含首对原始输出、逐样本命令/日志/产物清单、SDK/MOD 清单和 validation.json。归档 SHA256 `339d9343026d0928d22c73637ebb36808dea7d348d3792d97b195ce356580a30`。
- [轻量报告包](https://github.com/wu162/NewSdk/actions/runs/40608627111/artifacts/12782320515)：约 2 MiB，含 summary、完整 validation、输入清单和原始归档校验/提取收据；[只读提取运行](https://github.com/wu162/NewSdk/actions/runs/40608627111) 成功，编译 job 明确 skipped，没有再次编译或改变原结果。归档 SHA256 `dc4bfa78d513d290e992620d87d796dfcba254edc89ecfc361a34cad70411036`。
- Actions 证据包保留 30 天，原始完整包到期为北京时间 2026-11-27 08:22。持久 JSON 保留计量和身份，不能代替已过期的原始流字节；后续应在过期前自行归档所需完整证据。

脚本及完整方法见 [benchmark README](../benchmarks/upheaval/README.md)。工作流默认 10 对正式样本，单进程超时 20 分钟、总任务 240 分钟，不取消已有分支运行。未来正常运行自动同时上传完整包和轻量统计包；手动填写 `report_run_id` 只提取已有成功运行的报告，不重新编译。固定生产 artifact 到期或生产输入变化时，准备器会拒绝复用，需要先取得新的合格二进制来源。

早期 [40604428912](https://github.com/wu162/NewSdk/actions/runs/40604428912) 和 [40604660397](https://github.com/wu162/NewSdk/actions/runs/40604660397) 为输入预检失败，分别修正“当前主线仍跟踪 Upheaval”和“原 SDK 导入是恢复主线祖先”的假设，均未进入编译计时。失败记录保留，不混入正式样本，也不改写成成功。
