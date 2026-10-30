# 直接产物与原 SDK 缓存

基于 `recovery/legacy-sdk-cache` 的 `5c1843ae`，实现分支 `perf/direct-sdk-output`。本轮删除整代 Staging 与 MOD 发布事务；验收结果及同机性能计量在本页末尾更新。历史成功证据不能代替本轮改动的验证。

## 当前路径与完成条件

- `OutputRoot/published/<content>/<entry>/streams`：Core 实际四流与 CDATA 输出，也用于下次链接复用。它不是另一个待复制或整体搬走的 Staging。BIG 模式保留这些工作流文件，但最终报告只列本轮 BIG/SKUDEF。
- `OutputRoot/published/*.big`、`*.skudef`：正式 MOD 文件。BigArchiveWriter 在同一最终目录用既有 `.tmp-*` 写入，完成后 `File.Replace` 覆盖目标，无备份文件。一次 BIG 内容写入，无完整 BIG 发布复制。
- `OutputRoot/assets`，或 `/iod` 指定的中间目录：原 BAB 的逐流中间资产，供依赖、增量编译和重新链接使用。
- `session-cache-directory`，默认 `OutputRoot`：原 `BinaryAssetBuilder.SessionCache.xml[.deflate]`、原序列化的 `.tmp/.old` 和字符串哈希。原 BAB 的单缓存文件临时写入及 `.old` 保留；它们不属于 MOD 发布事务。
- `build-cache-directory`：原 BAB `BuildAssetCache` 原生类型/实例键与 Win32 资产，不引入新包缓存。保留原 `/bc`、`/atc` 行为。
- `<session-root>/.NewModCompiler/cdata`：已有的原生键 CDATA 存储，支持不同输出目录复用共享会话、重建丢失输出和导入原 BAB 缓存。仅新键创建快照，已有键不逐字节比较；缺失当前 CDATA 时按需导入，供 Core 修改的副本独立存储。
- `OutputRoot/.file-identities.json`：仅保留输入与 Schema 的文件身份和必要内容哈希，维持等长/恢复 mtime 编辑的失效能力。已移除发布收据、整代目录拓扑记录、输出 inode 身份缓存。
- `.package-lod`、`.package-versions`，以及显式中立修复需要的 `.package-neutral`：仅保存确实生成的 LoD、APT 三字节版本文件和中立修复输入，打包后删除各自目录。没有共同的每次构建暂存根，没有完整 MOD 复制。Additional 直接读取 SDK/MOD 当前文件，沿原顺序覆盖同名成员。

Core 在流开始时登记所触及的流，在四流链接全部完成时接受它们；CDATA 在实际提交后登记。本轮返回列表不依赖任何输出目录扫描。未选择的流、已移除的引用及任意旧文件不会加入新 BIG。Core 原有资产/CDAT​​A 清理仍限定于实际处理的流；失败仅移除本轮触及流的四文件，不递归清空输出根。

完整四流用原 manifest 的版本、类型哈希和流校验和，以及三文件头校验和复用。组不完整或校验和不一致时重新链接，避免只重建 IMP/RELO 而缺少 BIN 链接阶段填充的缓冲区。成功不再要求 `.linked-streams` 的第二份快照、索引和事务代际目录。

会话在 Core 成功后按原格式保存，`/psc:true` 仍允许失败/取消时保存已完成的有用工作，`/psc:false` 不在失败流的捕获路径保存。冻结、压缩选择、依赖失效、CDATA 导入与原生资产缓存保留。缓存更新不等待日志、终端事件或最终报告成功。

进程仍对编译、打包、取消和诊断错误返回错误码。已写好的 BIG/SKUDEF 不因后续日志、报告、事件失败而撤销。失败构建不会把磁盘上既存文件列成本轮成功结果；消费者应按本次报告/退出码判断成功，不能扫描旧目录推断。

## 删除清单

| 文件/类型 | 删除内容 |
| --- | --- |
| `Application/ArtifactPublisher.cs` | `ArtifactPublisher`、`ArtifactPublication`、`PublicationResult`、`PublicationLock`；Prepare/Commit/Complete/Rollback、`.publish-*`/`.previous-*`、整目录交换、备份、回滚、整树比对、CopyAndHash。 |
| `Application/LinkedOutputCache.cs` | 四流的独立快照树、`.index.json`、`.linked-candidate-*`/`.linked-previous-*`、发布事务和重复输出持久化。实际复用改为 Core 原地四流校验。 |
| `Application/BuildAssetCache.cs` | App 层未接入生产的通用事务字节存储、候选代、收据和完整文件复制/哈希。**保留** `Managed/Core/BinaryAssetBuilder.Core/BuildAssetCache.cs` 原 BAB 实现。 |
| `Application/SessionPackage.cs` | 未接入生产的会话包包装、整包恢复/复制、候选与回滚。保留真实 BAB `SessionCache`。 |
| `Application/CacheInputSnapshot.cs` | 上述未使用包装的整树指纹和扫描。 |
| `BuildSession` | `.build/<BuildId>` 的创建/销毁；正式产物 Prepare/Commit/Complete/Rollback；缓存事务参与者调用；发布/缓存撤销逻辑与相应性能阶段。 |
| `FileHashCache` | PublicationKey/PublicationReceipt、MatchesPublication/RecordPublished、通用收据入口、输出 inode 缓存，以及 FileIdentity.Stamp 和仅供已删除指纹使用的递归枚举。 |
| `LegacyCacheStore` | candidate/previous/installed/backups、Stage/Commit/ConfirmPartial/Complete/Rollback、整树 CDATA 扫描和重复内容比较。使用小型 `CacheLock` 保持原会话目录的进程排他访问，无发布生命周期。 |
| `Artifact` / `StreamContext` / `IBuildCacheCompiler` | 输出 SHA256、虚拟 BackingPath/Verified、StagingDirectory/CacheManagedBySession，以及 CommitCache/RollbackCache/CompleteCache。缓存接口仅报告实际统计。 |
| 打包与协议 | `package.hash`、输出完整文件 SHA256、`publicationUnchanged`、发布回滚状态、Additional 整份快照复制。验证器在外部采集字节身份。 |
| 测试 | 删除 ArtifactPublisherTests、LinkedOutputCacheTests、App BuildAssetCacheTests、SessionPackageTests、SessionCacheTransactionTests、SessionPublicationTests、CacheInputSnapshotTests；删除已不存在扫描器的镜像测试。其余测试按直接产物语义更新，保留原 SDK 字节对照、冷热/编辑、失败/取消与真实锁定错误。 |

旧运行留下的 `.build`、`.linked-streams`、`.publish-*` 等目录不会被新代码创建或复用，也不会为清理它们扫描整个输出根。

## I/O 变化与计量方法

设 BIG 大小为 B、Additional 有效输入总量为 A。以下是生产路径的结构性次数；文件重命名不计作内容写入或复制。原版这里指修改前的新编译器基线 `5c1843ae`，不是 EA 原 BAB。

| 工作 | 修改前 | 修改后 |
| --- | --- | --- |
| 正常 BIG 内容生成 | 1 次 | 1 次，最终目录内临时写入后替换 |
| 完整 BIG 发布复制 | 新产物/变化时 1 次，未变热态 0 次 | 0 次 |
| 完整 BIG SHA256 | 每次新打包 1 次（首次检查旧输出可能再读旧 BIG） | 0 次 |
| Additional 完整输入快照 | 每次 1 次，读 A、写 A | 0 次 |
| 四流缓存持久化 | 新结果另建快照；硬链接失败会完整复制 | 0 份额外快照，原地复用 |
| 发布目录事务耗时 | Prepare/Commit/收据/清理 | 该阶段删除，无发布扫描/复制/事务耗时 |

不含初次旧输出检查时，变化 BIG 消除至少 2B+A 的附加逻辑读取和 B+A 的附加内容写入；未变热态消除 B+A 的附加读取和 A 的附加写入。Core 原有输入读取、资产缓存、BIG 从成员读入写出、诊断文件 I/O 仍有其成本。不能把公式当作物理磁盘或完整进程的实测收益。

`MeasureDirectOutput.py` 在同 VM 重建基线/当前 Release，使用完全相同的混合资产输入与可选 16 MiB Additional；无缓存、冷、热各三对，交替执行顺序。控制器比较完整 BIG/SKUDEF 字节，子进程外计算身份；Win32 `GetProcessIoCounters` 记录含运行时加载和诊断的逻辑进程读写量。OS 页缓存未受控，不宣称物理磁盘写入或完整 MOD 的普遍加速。

## 本轮验证

首个实现检查点 `a1f5a059`：本地五项热缓存控制器预检通过，Windows Debug/Release App 合同通过；完整产品构建及生产验证仍在进行。后续删除未用输入快照并简化输入身份接口，须使用最终源码重新验收，不能复用此检查点的生产结论。

第二轮生产/计量来源 `f6c3c009`，[41046820725](https://github.com/wu162/NewSdk/actions/runs/41046820725)：Core、两类原缓存、Audio/W3X 载荷及完整 mymod 原 SDK 差分两配置通过；CLI 浅克隆 fixture、BIG 旧路径断言和资产缓存恢复 CDATA 的旧发布时间戳断言失败，均已保留并修正测试。完整归档已下载复算，见[原收据与独立通过范围](../evidence/refactor/direct-output/41046820725-1/summary.json)。

同 VM 三对中位数（Release；逻辑进程 I/O，不是物理磁盘；生产源为 f6c3c009，后续仅删除不使用的元数据辅助方法）：

| 输入 | 缓存 | 完整进程 ms：前→后 | 逻辑读 MiB：前→后 | 逻辑写 MiB：前→后 | 发布阶段 ms：前→后 |
| --- | --- | --- | --- | --- | --- |
| 混合资产 | 关闭 | 1075.73→973.54 | 2.98→2.27 | 1.17→0.91 | 19.88→0.00 |
| 混合资产 | 冷 | 1037.40→1039.67 | 3.14→2.42 | 1.35→1.09 | 18.11→0.00 |
| 混合资产 | 热 | 722.14→668.63 | 2.35→2.07 | 0.52→0.32 | 14.05→0.00 |
| 混合资产 + 16 MiB Additional | 关闭 | 1073.48→1018.88 | 66.98→18.27 | 49.17→16.91 | 26.59→0.00 |
| 混合资产 + 16 MiB Additional | 冷 | 1098.70→1028.40 | 67.14→18.42 | 49.35→17.09 | 25.80→0.00 |
| 混合资产 + 16 MiB Additional | 热 | 744.25→694.20 | 50.35→18.07 | 32.52→16.32 | 26.09→0.00 |

36 次均比较完整 BIG/SKUDEF 字节，并从原报告复核了关闭/冷/热的真实缓存计数。小输入冷态完整耗时 1037.40→1039.67 ms（约 +0.22%），如实保留；不能仅凭删除发布阶段推断所有输入都会整体加速。加入 16 MiB Additional 的冷态逻辑读减少约 72.56%、逻辑写减少约 65.37%；它是该合成场景，不代表完整 MOD 的通用比例。完整计量见[原始 JSON](../evidence/refactor/direct-output/41046820725-1/measurement.json)。
