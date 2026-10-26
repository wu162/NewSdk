# P5 生命周期、缓存与输入根

状态：P4 已验收；事务式 loose 发布及真实 CLI 发布整组已通过 Debug/Release。Win32 loose 核心 P5/G3、完整未精简 mymod 对照及 P6 最小发行包已验收；P7、完整 UI/runtime 和 Unicode Texture 仍未完成。以 `REFACTOR_RESUME.md` 和当前分支 Actions 原始收据为准；历史设计及失败保留在 git，不将旧“待实现”当作当前状态。首版仅顺序执行，Windows x86 / .NET Framework 4.8。

## 已通过的生命周期边界

- 每流新建 Settings、SchemaSet、SessionCache 和 DocumentProcessor；受控异常恢复本次拥有的递归栈、Processing、项目配置、静态 Core、hash、Tracer、metrics 和 report 状态。生产进程租约拒绝并发或重入；不可跨流裸共享 Complete 文档。
- 后端不启动 BAB/worker；生产 EXE 不含测试入口，独立验证宿主复用生产对象并分别绑定两份 EXE 身份。真实 Core、冷后端、原 BAB、App 协议和 Texture 的既有成功证据按未变化范围复用。
- 成功非空 loose 构建整代替换 `OutputRoot/published`；报告、终态或必需日志收尾失败撤销新代。空可选集合保留上一代；产物仅按精确清单发布。回滚失败保留可恢复位置，不宣称成功。
- 三份真实 Upheaval UI 的固定 106 文件/103 依赖闭包、组合与独立冷进程及当前原 BAB 四输出已通过两配置。Texture 为内联 DDS，不是 cdata；这些证据不代替完整 UI/runtime 或 SDK 验收。
- Unicode 纹理输入仍受原混合 DLL 的 LoadImageA/D3DX 限制；已有具体失败证据，不重复下载器或无依据路径排列审查。

## 本轮实际 `/bc`：定向范围已通过两配置

本节代码合同已取得两配置实际验收。生产/Core10组、App各667断言、Texture、真实CLI及输入根原版对照通过；最终缓存与Audio CDATA/W3X重放复用未变化的同一生产二进制，并分别记录二进制/测试来源。原组合运行的测试工具失败保留，修复后的定向重放成功不改写其终态。入口为 `CoreStreamCompiler`、`BuildAssetCache`、`CacheInputBoundary`；测试为 `BuildAssetCacheTests`、`SessionCacheTransactionTests`、`CompilerCacheContent`、`CompilerCacheBoundary` 和 `TestCompilerBuildCache.ps1`。

- `/bc:true` 选择真实 Core build-asset cache；每流 SessionCache 仍冷建，不读取旧 BAB session/build 命名空间。`/sc:true` 已接入并通过独立会话包两配置验收；压缩/部分/冻结 session cache 及显式旧缓存目录仍拒绝。
- 恢复的 audio/W3X 路由按 PluginRegistry 的真实 descriptor 使用缓存；混合 Texture 三路仍编译，不授权纹理缓存命中。报告统计实际资产 hit/miss，不能将恢复缓存目录或输入清单相同当作命中。
- 新命名空间包含全 SDK/MOD/runtime 内容、任务和参数，以及进程解析合同；实现版本随 EXE 内容区分。输入清单本身不授权缓存复用：缓存模式另以解析边界约束 XML、XInclude、Schema 和文件引用，根外输入拒绝。
- Audio 的 File 与 PathMusic header 均为 Schema FileReference；APT/test DataBlob 派生 FileReference。W3X 编译器直接消费预处理 XML，无输入旁读。混合 Texture 内部读取未证明完整闭包，故不复用其资产。
- FileHashItem 与 SchemaSet 增加独立 SHA-256 内容失效，保留游戏资产的原 FastHash/处理哈希语义；覆盖同长度同 mtime 修改及旧缓存无内容身份的情形。
- 编译前从已确认缓存复制资产到独立 staging；成功流只准备候选。BuildSession 在发布后、必需报告前可逆 Commit，任一后续失败 Rollback；必需渠道成功后 Complete。相同命名空间互斥，竞争明确拒绝。
- 新安装条目带 durable pending 标记，最终确认后才可复用；进程消失或回滚删除失败的遗留仍是 miss。缓存文件按完整清单和 SHA-256 检查，缺损/损坏/未登记文件不复用；回滚失败保留精确位置供恢复。

### 受影响组验收

| 矩阵 | 必须取得的证据 |
| --- | --- |
| App 缓存事务与存储 | Debug/Release；必需报告/终态/close 失败、取消、Commit/Rollback 失败、锁与重试，不留下可复用失败缓存 |
| 真实 Core 内容与边界 | 同 mtime 文档/文件/Schema 变化，旧身份失效；实际解析根内通过、根外拒绝，边界退出后恢复普通行为 |
| 生产 CLI 缓存 | 无/冷/热；真实 hit；文档/XInclude/文件/Schema/配置/实现变化；两个 MOD；失败/取消后同路径重试；原始四输出和独立冷进程/原 BAB 对照 |

未变化的 XML/非 XML 组件证据不重复全量运行。实际失败先定位后统一复验；候选完成不自动等于完整 P5/G3 或完整 SDK。

## 已通过的输入根与接着推进的会话缓存

1. `/dr`、`/data`、`/art`、`/audio` 按原 SettingsLoader 的 DataRoot 与分号搜索顺序解析；显式覆盖与 MOD 前置默认路径须区分，不能只让参数不再报错。本轮真实 Core/CLI/原 BAB 对照已通过两配置。
2. 向报告输出每流最终生效 DataRoot、data/art/audio 数组及来源；原始字符串仍保留在 effectiveOptions，不把 invocation-relative 路径预先套到旧搜索串。
3. 明确根外引用：无缓存模式按原解析契约处理；若缓存模式无法覆盖新增根，必须明确拒绝或将全部新根纳入内容合同，不能静默读取旧缓存。
4. 使用最小合法外部引用 MOD 核对路径优先级、原 BAB 原始四输出、失败和重试；同时检查最终生效根及来源报告。本轮两配置已通过；会话缓存等后续完成后集中验收完整 P5/G3/SDK，再推进 P6/P7。
5. 真实 session cache 已接入并通过生产两配置冷/热与完整事务矩阵；其证据独立于 `/bc`，两者不能互相冒认。

### 实际 `/sc`：两配置整组已通过

- 每流独立 `SessionPackage`：私有 staging 还原 `hashes/session.xml`、完整 intermediate 资产和 streams/cdata；每次 fresh SessionCache 按 `LoadCache` → `InitializeCache` → 新 processor 顺序运行，四输出由 Core 重建。
- 成功后 `SaveCache(false)`，只保存新的 session.xml、当前 intermediate/cdata，排除 `.old/.tmp` 和 linked 四输出；沿现有 Commit/Rollback/Complete 事务与发布、必需渠道共同确认。`/sc` 与 `/bc` 独立目录、锁和统计，可分别启用。
- 清单/SHA256、布局及 XML 在恢复前整体验证；结构/版本加载失败清空本包恢复的资产与 hash scope 后冷编译。坏包可逆修复：失败恢复旧坏条目，成功确认健康包，避免永久冷失效。
- 报告 `sessionPackageHits/Misses` 与实际 `documentReuse/xmlParse/instanceCompile`；包存在或载入成功不代替文档与中间资产的实际复用。失败流也报告已发生的工作。
- 三类混合 Texture 在实际处理类型检查后、保存包前明确拒绝 `/sc`，`/sc:false` 保持原编译能力；未证明完整闭包的纹理不授权会话或资产缓存复用。
- `TestCompilerSessionCache.ps1` 整组覆盖 Include/XInclude、定义/表达式、继承、多入口、同 mtime 输入/Schema/配置/实现变化、双 MOD、坏包修复、失败/取消/发布失败重试、Audio CDATA/W3X 和原 BAB 四输出；链接前先验证 fixture XSD 与替换 token。
- 首次生产链接 MSVC C1001 已由等价普通 catch 替换 App 新过滤器解决，新完整生产两配置通过。其后唯一 Core 失败为旧 Unicode fixture 未绑定 processor；修正 owner 并验证解析计数后，精确归档 host 两配置定向重验与原 BAB 基线通过，原失败终态保留。
- `/sc` 两配置各55用例/2184事件零协议错；完整未精简 mymod 原 BAB 先行与两配置生产对照（`TestCompilerSdk.ps1`）已通过。当前 mymod apt/maps 无入口，明确记录空集合；Win32 loose 核心P5/G3与P6最小包取得证据，完整UI/runtime、Unicode Texture和P7不由此推断。
- 原 Core App 协议被旧整体失败跳过的范围已对固定归档原字节补验：两配置各15报告/511事件零错，原始归档SHA与逐文件SHA在 `recovery/evidence/refactor/core-protocol-recovery`，不改写原运行终态。
