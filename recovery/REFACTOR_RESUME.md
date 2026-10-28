# 缓存与并行专项：当前交接

更新：2026-10-28（北京时间）。工作分支 `feature/cache-parallel` 从 `compiler-main` 分出；[draft PR #17](https://github.com/wu162/NewSdk/pull/17) 目标为 `compiler-main`，未经用户批准不合入，禁止强推。

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

## 下一步：计划 B 组输入闭包成本

本次按需读：[B组](docs/CACHE_PARALLEL_PLAN.md#plan-b)、[输入一致性](docs/CACHE_PARALLEL_PLAN.md#plan-input-consistency)、[验收](docs/CACHE_PARALLEL_PLAN.md#plan-validation) 和 [计量](docs/CACHE_PARALLEL_PLAN.md#plan-performance)；涉及store/发布时补读 [事务](docs/CACHE_PARALLEL_PLAN.md#plan-transactions)。

1. 从 `CacheInputSnapshot.cs`、`CoreStreamCompiler.cs` 及既有计量定位同一构建内重复盘点/受控读，成组消除可证明重复的工作，保留必要读取；不再扩大 XML 资格或围绕私有恢复做微调。
2. 共享必须有稳定字节版本证明；保留鲜读、文件身份核验、MOD/Additional保护、首尾检查与发布前保护。不能用 mtime/size 替代内容身份，也不把前后扫描当严格快照。
3. 同组准备等长mtime修改、替换/租约释放、输入变化拒绝、取消/异常与事务回归；生产闭包变化后集中重建受影响矩阵，按适用里程碑验证 SDK/独立 ZIP。
4. 先用已有计量与小夹具验证改动；大型 MOD 仅在专用测试分支低频集中运行。完整运行期间推进独立工作，有余量继续计划内依赖满足的工作。

小 MOD 的“所有情况下热缓存必须更快”不是新增验收门槛，也不另起长期调优项目；其阶段计量用于定位计划 B 的重复成本。后续 Parse/Prepare、链接/封装缓存仅在计划第9节的计量条件成立时推进。

## 执行与交接

遵循 [EFFICIENCY](EFFICIENCY.md)：生产成组实现与验证，编译/测试全部 Actions；仅测试变化优先复用匹配二进制并重跑受影响测试，分别记录 binary/test 来源；仅文档不构建。已有有效验收不重复运行，失败终态与严格断言保留。

每轮更新本页的最新有效来源、真实阻塞和下一步，细节放能力文档/证据 JSON，历史留 git。不要累加旧发行和组件恢复待办。
