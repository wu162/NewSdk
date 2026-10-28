# Upheaval 缓存为何比老SDK慢：成本定位

**已定位主要的新增成本：每条流反复哈希整个输入库，以及命中后仍重建再丢弃候选缓存。** 缓存实际命中1617项，数据没有显示热建因为这些条目失效而退化。Texture未启用BC、XML/其他类型继续处理，是剩余工作；不能把全部差距归给贴图。

本轮是单台Windows VM的定向诊断，使用原正式Release执行完整构建，并用未修改的同源C#方法进行CLR4/x86、opt+受控重放。**不是整进程CPU采样火焰图，不能把重放中位数机械相加/相减当成精确归因或承诺可节省时间。** 它解释已有双VM基准，不替换原正式均值或置信区间。

## 同机完整构建与逐流事件

来源[40778834315](https://github.com/wu162/NewSdk/actions/runs/40778834315)，测试源`d30f36aa13a2839eba5728c99b6e8c5b499fff7b`，原生产源码`8b3e146cb5220e830ff6a0ce4718f47af052f166`/原验收包40446975355。环境为Windows2022、Intel Family6 Model106、4 logical CPU、x86 CLR4.0.30319.42000。每种完整构建观察一次，全部与原完整Upheaval指纹一致，SDK/MOD/运行文件及确认缓存前后均未变。

| 新版模式 | 完整wall秒 | CPU秒 | compile阶段秒 | publish阶段秒 |
| --- | ---: | ---: | ---: | ---: |
| 关闭BC | 59.905 | 54.562 | 55.298 | 1.483 |
| BC冷建 | 75.166 | 73.203 | 73.141 | 1.436 |
| BC热建 | 49.148 | 46.906 | 46.820 | 1.523 |

额外时间集中在compile阶段，发布约1.5秒且变化很小，计划阶段也只有数毫秒。不要把这一新VM的绝对时间与原双VM旧SDK均值相减。

| 入口（事件间隔秒） | 关闭BC | BC冷建 | BC热建 |
| --- | ---: | ---: | ---: |
| base/Mod | 54.107 | 60.783 | 33.722 |
| apt/TacticalHUDBuildQueuePage | 0.459 | 3.279 | 3.218 |
| apt/TacticalHUDCommandListBox | 0.201 | 2.976 | 3.147 |
| apt/TacticalHUDMainCommandBar | 0.274 | 3.157 | 3.289 |
| maps/MapMetaData_Global | 0.252 | 2.942 | 3.437 |

本例只有base流真正恢复出资产缓存，三个APT和地图流没有缓存载荷，却也承担同样的全输入扫描。地图元数据从0.252秒增至3.437秒，直接显示很短的入口被固定校验成本主导。

## 1. 每流前后做全库哈希，是第一优先级

[CoreStreamCompiler.cs:454](https://github.com/wu162/NewSdk/blob/8b3e146cb5220e830ff6a0ce4718f47af052f166/recovery/src/App/Application/CoreStreamCompiler.cs#L454)在每条流前调用Capture；[520行](https://github.com/wu162/NewSdk/blob/8b3e146cb5220e830ff6a0ce4718f47af052f166/recovery/src/App/Application/CoreStreamCompiler.cs#L520)的MatchesCurrent又调用一次完整Capture。扫描不是mtime检查：[CacheInputSnapshot.cs:120](https://github.com/wu162/NewSdk/blob/8b3e146cb5220e830ff6a0ce4718f47af052f166/recovery/src/App/Application/CacheInputSnapshot.cs#L120)逐文件读取内容并计算SHA256。

SDK14715文件、MOD2723文件、运行包5文件，合计17443文件。五条流×前后两次=10次完整遍历，每次完整MOD构建累计174430次文件内容哈希，读取4,319,264,120字节（约4.32GB，另有目录/路径/清单操作）。相同SDK、MOD、运行文件被重复验证，冷建和热建都付这个成本。

同源原函数对五流前后校验重放三次为13.119、13.338、13.192秒，中位数**13.192秒**、CPU中位数**12.563秒**。本机完整冷建相对关闭BC增加15.260秒，重复扫描的量级足以解释新增成本的大部分；这是同机受控重放的量级证据，不是严格的采样百分比分摊。

## 2. 热命中后仍复制、哈希、重建候选，最后丢弃

[BuildAssetCache.PrepareStream](https://github.com/wu162/NewSdk/blob/8b3e146cb5220e830ff6a0ce4718f47af052f166/recovery/src/App/Application/BuildAssetCache.cs#L47)先校验存储载荷，再复制到临时目录并再次校验。编译后[StageStream](https://github.com/wu162/NewSdk/blob/8b3e146cb5220e830ff6a0ce4718f47af052f166/recovery/src/App/Application/BuildAssetCache.cs#L69)不区分全命中，仍复制并哈希出全新候选；[Commit:115](https://github.com/wu162/NewSdk/blob/8b3e146cb5220e830ff6a0ce4718f47af052f166/recovery/src/App/Application/BuildAssetCache.cs#L115)遇到已有条目直接跳过，Complete再删除候选。

本例实际载荷2906文件、54,443,140字节（索引另计）。重放使用原CLI的请求、任务及真实命名空间，确实命中原缓存；冷写在隔离目录完成并验证可恢复，原确认缓存哈希不变。三次重放各操作的五流合计中位数：

| 操作 | wall秒 | CPU秒 |
| --- | ---: | ---: |
| 热恢复：校验→复制→再校验 | 1.666523 | 1.328125 |
| 热命中后仍复制/哈希候选 | 1.090887 | 0.843750 |
| 发现条目存在、跳过安装 | 0.000303 | 0.000000 |
| 删除未安装候选 | 0.118318 | 0.109375 |
| 冷建复制/哈希候选 | 1.148905 | 0.796875 |
| 冷建安装候选 | 0.002542 | 0.000000 |
| 冷建完成清理 | 0.000275 | 0.000000 |

热建仅“重建候选再丢弃”就约1.21秒。它是确认的重复工作，但比全输入扫描小；不能把文件复制当成这次最大的瓶颈。冷建将工作缓存复制/哈希成持久候选约1.15秒；这不包含插件首次生成工作缓存的开销，不能当成冷建所有写入成本。

## 3. 命中覆盖有限，剩余1737次确实还在编译

将完整构建日志中的`Compiling Type:Instance`逐项分类，总数与原报告计数一致：冷建3354次，热建1737次。热建少掉的1617次恰好是AudioFile1289、W3DAnimation194、W3DMesh134。XML仍解析925次。

热建剩余包括AudioEvent304、FXParticleSystemTemplate214、GameObject121、WeaponTemplate108等，以及Texture115和OnDemandTexture2。**1737次不是1737张贴图。** [CoreStreamCompiler.cs:442](https://github.com/wu162/NewSdk/blob/8b3e146cb5220e830ff6a0ce4718f47af052f166/recovery/src/App/Application/CoreStreamCompiler.cs#L442)明确关闭Texture/OnDemandTexture/TerrainTextureAtlas的BC，因为混合D3DX读取的依赖闭包尚未证明。115+2也吻合旧SDK热建总缓存复制比新版多117项的差额；本轮没有逐类耗时消融，不能给贴图差距硬分配秒数。

BC不是文档/会话缓存，不能跳过全部XML和链接；这一点本身不全是新旧差异。准确结论是：全局校验和缓存搬运吞掉了大块命中收益，新版还有少于旧SDK的贴图缓存覆盖。

## 4. 单文件迭代还存在更粗的失效粒度

CacheInputSnapshot把整SDK/MOD/runtime内容写入同一输入指纹，并作为资产缓存命名空间的一部分。修改一张DDS也会使同流原本可用的音频、动画和网格缓存失效。先前已验证的XML/DDS/WAV修改前1617命中，首次修改后0命中/1617未命中、耗时为旧版3.6～3.8倍，与该代码路径一致。参见[正式扩展报告](UPHEAVAL_EXPANDED.md)；它是另一个迭代性能问题，不能误称本次无修改热建没有命中。

## 优化顺序与约束

1. 优先消除多流重复全库扫描：评估构建会话级共享输入清单/统一提交前验证，继续保留输入变化拒绝、解析边界、最终确认及回滚。不能通过关闭哈希或只看mtime换速度。
2. 原命名空间完整性已验证、全命中且无新增资产时，评估不再重建候选；保留真正必要的命中载荷完整性验证。不要复刻旧版损坏缓存传播错误字节的行为。
3. 为单文件迭代建立经过审计的资产依赖键，避免把一个资源变化放大成全局namespace失效；不能直接删除全库指纹而不补依赖覆盖证明。
4. Texture缓存另做D3DX实际读取路径、工具链/依赖身份审计后再开放；不能只将UseBuildCache改成true。也不能用含Texture的完整SC绕过现有限制。

本轮只定位、未改变生产编译行为。以上是有证据的优化目标，不承诺未经实测的修复后耗时。

## 证据与复跑

[持久计量](../benchmarks/upheaval/results/cache-cost-40778834315.json)保存三次原始重放、CPU、逐流/阶段事件耗时、日志类型计数、源码/运行文件哈希、确认缓存与输入不变门槛。生产EXE SHA256仍为`fc2e99a1a6f65e058759bda187e5ca3bb084c2269fa0fa07b26bca8ce289cf0d`。helper仅编译原Model/CacheInputSnapshot/BuildAssetCache/ArtifactPublisher/Diagnostics源码及逐字提取的两段缓存契约函数，使用实际CLI请求和计划；原正式EXE始终用于完整构建。

- [upheaval-cache-cost-evidence-40778834315-1](https://github.com/wu162/NewSdk/actions/runs/40778834315/artifacts/12857115321)，SHA256 `80a828dd12eb04b4855b534f39da681fb5d2ab57194e1d15fe29b802f240484f`，已下载核对。
- [upheaval-cache-cost-summary-40778834315-1](https://github.com/wu162/NewSdk/actions/runs/40778834315/artifacts/12856910646)，SHA256 `ecd5e8885f594f01573562354d5f5cd3db495c6dce0f54ea1464cad0d86f19e9`，已下载核对。

首个尝试[40778055207](https://github.com/wu162/NewSdk/actions/runs/40778055207)在CLR拒绝把混合PE EXE当普通程序集加载时提前失败，尚无构建计时；失败保留，不计入数据。随后改用同源独立helper，没有修改正式EXE或放宽缓存命中/字节门槛。

复跑选择工作流`suite=cache-cost`或提交标记`[upheaval-cache-cost]`，只运行一个定向诊断job，其他正式benchmark组不重复。可复跑入口为`cache_cost.py`和`CacheCostProbe.cs`；完整SDK/MOD仍固定原历史tree，生产包继续走原身份核验。单VM诊断数值不能替代正式双VM统计，也不能视为完整调用栈采样归因。
