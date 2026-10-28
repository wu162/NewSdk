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

## 下一步：解决热缓存额外开销

按需读计划 [B1热缓存执行方案](docs/HOT_CACHE_OPTIMIZATION.md)，涉及输入/事务变化再读其关联合同。当前为待实现方案，没有新的生产验证或收益。

1. **第一批先减重复操作：**优化目录属性获取、稳定文件分类/摘要复用及root内容与stream描述组织；现有build级租约已经存在，不重复实现。保留每stream首尾保护、MOD/Additional鲜读和512句柄预算。
2. **仍由受控读主导时实施第二批：**有界并行必要鲜读，reader独占SHA/缓冲/handle，owner维护租约与确定顺序归并；先比较1/2/4 reader，不将非线程安全的现有租约字典直接并行化。
3. 按现有Actions入口成组验证，先小mymod同runner、同jobs交替无缓存/热缓存至少3对；稳定后低频复测完整Upheaval。原字节、失效、取消/异常、租约释放、回滚与旧session合同保留。
4. 分别报告“开销下降”和“热缓存快于无缓存”。若仍倒挂，如实保留差距；不继续恢复微调、扩大XML资格或用自动关闭缓存掩盖问题。全根保护替换为实际读取闭包涉及行为合同变更，需单独说明并经用户确认，不能借优化删除检查。

最新问题定位：无缓存不执行缓存路径的两次全根盘点；47hit/0backend仍承担盘点、文件打开/身份与鲜读成本。首热1361ms盘点和三热2217ms中位不能混作同次数据相减。目标是降低这些成本，不能保证所有小MOD都有缓存净收益。

## 执行与交接

遵循 [EFFICIENCY](EFFICIENCY.md)：生产成组实现与验证，编译/测试全部 Actions；仅测试变化优先复用匹配二进制并重跑受影响测试，分别记录 binary/test 来源；仅文档不构建。已有有效验收不重复运行，失败终态与严格断言保留。

每轮更新本页的最新有效来源、真实阻塞和下一步，细节放能力文档/证据 JSON，历史留 git。不要累加旧发行和组件恢复待办。
