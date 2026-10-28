# 缓存与并行专项：当前交接

更新：2026-10-29（北京时间）。工作分支 `feature/cache-parallel` 从 `compiler-main` 分出；[draft PR #17](https://github.com/wu162/NewSdk/pull/17) 目标为 `compiler-main`，未经用户批准不合入，禁止强推。

## 云端任务入口

后续在 Codex 云端执行，复用已有仓库检出和终端；先检查工作区/HEAD，再按 [云端执行规则](EFFICIENCY.md#codex-cloud) 核对成果分支。不要要求ChatGPT GitHub插件，不默认重新clone。云端完成编辑与静态检查，编译/运行测试仍走Windows Actions。任务临时分支不改变成果归属；受限时交付差异并注明未推送或待CI状态，不覆盖已有修改。

## 范围与读取顺序

读完 `AGENTS.md` 后先读本页及 [执行规则](EFFICIENCY.md)，随后按本次任务读取计划章节；首次接手或调整整体方向时再通读计划，日常不加载全部计划和历史记录。

以 [CACHE_PARALLEL_PLAN](docs/CACHE_PARALLEL_PLAN.md) 为目标与验收合同：A 资产动作缓存、B 消除重复 I/O、C 跨文档有界并行。具体已支持能力见 [CACHE_PARALLEL](docs/CACHE_PARALLEL.md)，精确来源见 [证据 JSON](docs/CACHE_PARALLEL_EVIDENCE.json)。计划中的要求不能当作已完成状态；历史里程碑中的“下一步”不覆盖本页。

不继续旧重构收尾、发行功能扩展、其他组件分支归集或全局研究。旧 session、原字节差分、SDK/mymod、BIG/CDATA、取消与回滚、独立 ZIP 仍保留为适用回归合同。

## 当前有效状态

- A/C 已有生产接入与完整回归证据。XML 289 路由中284条 metadata 与4条 APT 已获 worker 准入资格，TestTexture及未知新增类型保持 owner；资格不等于每类型复杂字段或双同类重叠均已实测。W3X/Audio 的具体资格与限制以能力表为准，Texture、Audio 重采样/PathMusic边界不扩大。
- 最新完整生产验收：`e86958de` / [40690172191](https://github.com/wu162/NewSdk/actions/runs/40690172191) success。B 组已加入阶段计时、共享每 Capture 路径分类、移动已验证私有结果以消除再次复制；完整 Debug/Release、Core、缓存、旧 session、SDK/mymod、BIG 与独立 ZIP 已通过。精确归档与来源见 JSON 的 `sharedInventoryAndPrivateMoveMilestone`。
- 最新小 mymod 计量：[40693590489](https://github.com/wu162/NewSdk/actions/runs/40693590489) success；binarySource=`e86958de`、testSource=`e5809d10`，复用成功二进制，未重编。完整7文件/982257字节 MOD、完整 SDK；Debug/Release 热中位2226/2217ms，无缓存824/760ms。热与 Additional-only 均47hit/0miss/0backend，四输出原字节相同。Release首热盘点1360.599ms，其中受控读1007.240ms，私有恢复15.806ms、动作Key3.864ms；计时有嵌套，受控读不是纯磁盘/hash。
- 完整 Upheaval 的历史同二进制计量：无缓存 jobs1→4 为88.160s→57.733s，1311输出原字节相同；热缓存中位73.609s仍慢于无缓存并行。原 [40682631514](https://github.com/wu162/NewSdk/actions/runs/40682631514) case成功但整体failure（末尾协议目录检查错误）；轻量归档重放 [40685364366](https://github.com/wu162/NewSdk/actions/runs/40685364366) success，不改写原终态。详见 [Upheaval记录](docs/UPHEAVAL_CACHE_PARALLEL.md)。此计量不是最新B组二进制的性能验收。

默认 `bc/sc=false`、jobs=1；专项未替换主线发行。热缓存净收益尚未解决，不能用零后端调用或跨runner耗时差宣称已加速。

## 下一步：依赖清单与分层持久缓存

按 [B1执行方案](docs/HOT_CACHE_OPTIMIZATION.md) 推进。当前已接入第一阶段生产读取观察、Core负候选、后端租约摘要、清单复验与独立二进制持久事务，具体 [READ_CLOSURE](docs/READ_CLOSURE.md)。全根保护仍保留，`canAuthorizeReuse=false`，尚无文档/stream复用或新性能验收。

- 首批 `60d17fb2` / [40783157399](https://github.com/wu162/NewSdk/actions/runs/40783157399) failure：新增畸形记录测试错误假设InvalidDataException属于IOException，未进入native构建；失败保留。
- 修正与解码/取消边界 `66524fb2` / [40783469606](https://github.com/wu162/NewSdk/actions/runs/40783469606) App Debug/Release success；不代表后续lease记录或完整生产验收。
- 后续lease接入、预算状态修复、空间计量与完整回归正在集中验收，终态和精确源码稍后补录。

1. 第一阶段观察记录接入后，先取得完整生产回归与原始归档；再认证纯metadata读取闭包（配置/工具/选项绑定、逐任务读取资格及precompiled/目录决策仍缺）。只有认证完成才能替换全根保护。继续记录并验证完整实际读取闭包（含resolver候选/不存在/目录决策），在已认证路径替换全根盘点；未知读取保留保守路径。目标允许无关根成员变化，影响语义/输出的变化仍须失效；旧session合同不改。
2. 先读B1“与当前后端优化的衔接”：恢复态必须保留强Key、引用/注册副作用、三缓冲/CDATA、独占输入、owner提交与MachineResources预算，不能扩大XML/W3X/Audio/Texture资格。随后复用文档准备态，再实现完整stream无变化快路；保留现有ActionCache和有界执行器，不复制第二套前端。枚举/并行鲜读降为局部优化，不先做完微调才推进结构改造。
3. 缓存采用版本化二进制索引与可定位数据块的设计方向：字符串去重、按需解码、分块可选压缩、小记录有界聚合；旧格式隔离，容量、完整性与代际事务必须闭合。具体压缩/分片阈值用同数据集对照确定。
4. 成组Actions验证失效、原字节、取消/异常和回滚；同runner同jobs交替无缓存/热缓存至少3对。同时报告各层体积、文件数、热读取/解码、内存峰值、单改重写和事务峰值空间。先小mymod，稳定后低频完整Upheaval。

当前Release小mymod仍为热中位2217ms、无缓存760ms；首热盘点1361ms来自另一次阶段记录，不能混合相减。必须分别报告开销下降与热缓存净收益，不把零后端调用或压缩率当作解决。

## 执行与交接

遵循 [EFFICIENCY](EFFICIENCY.md)：生产成组实现与验证，编译/测试全部 Actions；仅测试变化优先复用匹配二进制并重跑受影响测试，分别记录 binary/test 来源；仅文档不构建。已有有效验收不重复运行，失败终态与严格断言保留。

每轮更新本页的最新有效来源、真实阻塞和下一步，细节放能力文档/证据 JSON，历史留 git。不要累加旧发行和组件恢复待办。
