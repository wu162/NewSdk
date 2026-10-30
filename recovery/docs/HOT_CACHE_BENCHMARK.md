# L8 热缓存性能 Benchmark

入口为 [compiler-hot-cache.yml](../../.github/workflows/compiler-hot-cache.yml)，目标基线为 `recovery/legacy-sdk-cache`。只改 Benchmark 脚本、工作流和临时输入，不修改生产编译算法。旧的短矩阵继续保留；其成绩不能替代本套单次大于 8 秒的负载。

## 运行与审批

- push/PR 只做便宜的 Python 预检，不启动编译器 Benchmark。
- 手动选择 `group=common`，Windows 整个 job 最长 35 分钟。C00 三个新进程，C01–C12 各两组 `hot_before → modified → hot_after`，合计 75 个正式进程。
- `group=full` 包含 Common，整个 Windows job 最长 60 分钟。必须同时选择 `full_approved=true`，并经 `hot-cache-full` Environment required reviewer 审批。Full 不会由 push、PR、重试 Common 或 Common 完成自动触发。
- 当前 GitHub 集成读取该 Environment 为 404，配置请求为 403 `Resource not accessible by integration`。需要具备 Environment 管理权限的仓库管理员配置 required reviewer（可选仓库所有者 `wu162`）。代码中的门槛会拒绝没有 reviewer 的环境；本轮没有运行 Full，也不能声称仓库审批已配置完成。

Common 使用已验收的 Release：源码 `cc10391be67dda57c7efa7fc06256b99e9372d9a`，运行 `41068958541`。准备脚本核对生产输入未变、原始归档 SHA256、内层 Release ZIP 和五个运行文件身份，不重新构建生产二进制。来源固定在已有验收摘要；归档失效或生产输入改变时拒绝复用。

## 负载与采样

固定历史 Upheaval tree `5551fc26a6f96b62613443e0d0fa2958c14cbce0`，保留完整基础、APT、地图与 Additional 输入，再添加 Include 可达的合法独立分区。L8 明确标记为“完整真实 MOD + 扩容混合负载”；原始未修改 Upheaval 的库存单独记录。

扩容资产不只要求字符串 ID 唯一，还复现 SDK FastHash 检查 32 位身份碰撞，确定性重试后缀，并同步继承及 W3X 引用。额外分区每档增加真实 Armor 文档和声明，不加入延迟或未 Include 的填充文件。C01/C02 同为 128 份 Armor 改动，分别分散于 16 XML、集中于 256 声明的 XML。Texture、Audio（含 XAS）、动画、继承、Define/XInclude 等均有独立输入和有效引用。

标定先预热，再运行两个完全无修改的新进程；按实际编译耗时调整共同负载，有限轮次内争取 10–14 秒。正式样本同时要求外部单调 `wallMs >8000` 和报告 `elapsedMs >8000`。启动、下载、复制、预热和后处理不能补足单次门槛。原始计时保留，`under_8s`、失败或未触发目标路径的样本不进入合格中位数。

缓存快照在原绝对路径完整恢复，避免 BAB 缓存内的绝对源路径因搬迁而失效。每个场景/重复独占一次重置后的命名空间；源文件、新增文件、SessionCache、中间结果及发布文件一起恢复，不会继承前一重复的修改结果。恢复按基线文件集和元数据重放变更，源输入的实际 changedPaths 强制回放，因此等长/保留 mtime 的修改也会重置；新增文件及空搜索目录一并清理，不反复复制未变大文件。复制在计时之外，计入所在阶段及全局预算。

Common C10 丢失 Session 元数据、保留中间结果；C11 仅删除一个最终 BIG/SKUDEF，并要求恢复后无资产后端编译。Full 的中间 CDATA、链接 `.bin`、部分/全部元数据失效分别处理，不把删无关 cache 文件当作有效覆盖。F12 没有合法、可运行的不同身份版本时记为 `not_applicable`。

F11 只对六种缓存模式各做少量热态吞吐对照（新增模式 n=1），默认模式复用本次 C00，不将无修改比较机械展开成重复三阶段；音频 jobs=1/4 各两组三阶段，auto 复用本次 C05。复用关系写入场景说明，不把同一个样本计为新的独立重复。

**Full 的当前完整配置在阶段预算内不可达：**F01–F10 展开为 28 个独立条件（含四边深度继承及实际 APT 入口改名）、各两组三阶段，共 168 个正式进程；仅按严格 >8 秒的下限就需要 >1344 秒（22.4 分钟），超过其 960 秒预算，尚未计快照及重编较慢的情况。报告预先列出这个下限；若用户单独批准 Full，仍可在硬预算内取得部分性能数据，但不能声称完成整组。实现没有擅自省略变体或降低默认重复数来制造通过。

## 时限与输出

从 checkout 前记录整个 Windows job 时间；准备、标定、快照恢复、采样、诊断均计入。Common 第 32 分钟、Full 第 56 分钟停止启动进程。进程 timeout 取场景上限、阶段余量和全局余量的最小值，超时终止并落盘。Common 的负载/快照、标定、正式采样、诊断分别限制 7/4/18/2 分钟；已消耗的阶段时间不会因进入下一轮而重置。外部 job 的 35/60 分钟限制仍是最终硬限。

输出 `measurements.csv`、`scenarios.csv`、`phases.csv`、中文 `report.md`，另有 `benchmark.json` 和每进程的 request/report/stdout/stderr。无成绩也写出 CSV 表头。记录实际改动数量、缓存计数、实际计划 streamId、工作负载指纹、输入大小、发布大小、机器和 EXE 身份。

正式吞吐固定 `--performance off`；少量 `detailed` 独立归因，不混入正式中位数。off 没有的阶段/失效计数保持空白；嵌套或并行阶段不求和为 wall。所有必选场景与规定样本完成且全部满足门槛才可 `passed`，否则记录 `partial`、`budget_exhausted` 或 `under_8s`。

本套只交付性能数据和正常退出/路径执行的最少检查，不做游戏运行、功能回归或原 SDK 字节差分。跨 VM 的历史毫秒不用于宣称回归；不同版本必须在同机、同指纹、同缓存条件下重新配对，不能只给更快版本增加负载。

## 验证记录

本地控制器/负载预检及 actionlint 通过。新增分区用完整 SDK XSD 做了廉价合法性检查（在内存中适配 Windows 路径及原 Schema QName 尾空格，没有修改 SDK）；这不等于 Windows 编译成功。

[Windows/Linux 预检 41135332298](https://github.com/wu162/NewSdk/actions/runs/41135332298) 两个平台各 33 项通过（源码 `873271d3`），包括差量恢复、同 mtime 输入和新增搜索根清理；[收据](../evidence/refactor/l8-hot-cache/preflight-41135332298.json)。中文报告读取显式 UTF-8；每个源变更按真实调用合同登记，不依赖 NTFS 两次快速写入一定取得不同时间戳。

首轮 Common [41131918949](https://github.com/wu162/NewSdk/actions/runs/41131918949) 在预热阶段暴露生成 ID 的 FastHash 碰撞；第二轮 [41132655545](https://github.com/wu162/NewSdk/actions/runs/41132655545) 暴露直接 XInclude Defines 的全局重复定义。这两轮均未产生正式成绩。已修为真实哈希分配和共同定义文档，后续实际状态以对应运行的报告为准。

第三轮 [41133537865](https://github.com/wu162/NewSdk/actions/runs/41133537865)（源码 `e667ea70`，仍使用完整目录复制恢复）成功标定 13 个扩容分区：4545 份输入文档、111997 个输入声明、301.54 MiB、5 个实际流。两个标定热态 elapsed 为 11931/11856 ms，wall 为 12001.56/11923.84 ms，双门槛均满足。完成 C00–C06，保留 40 个合格正式样本；第 41 个正式样本 C07/modified 在 18 分钟采样阶段余量耗尽时终止。总计 21.16 分钟，状态为 `budget_exhausted`，未把必选场景缺失记为通过。[原始数据与收据](../evidence/refactor/l8-hot-cache/41133537865-1)。新版差量恢复的 [Common 41135980179](https://github.com/wu162/NewSdk/actions/runs/41135980179) 另行验证，不能把旧运行当作新版性能成绩。
