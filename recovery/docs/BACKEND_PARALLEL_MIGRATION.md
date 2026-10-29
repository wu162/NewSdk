# 原 SDK 缓存分支后端并行迁移

2026-10-30（上海时间）。目标 `recovery/legacy-sdk-cache`，起点 `096f60db`；参考 `feature/cache-parallel` / `054e8d9a`。

实现中，尚未完成 Windows 验收。资格一次接入 W3DAnimation/W3DMesh、受控 NONE/XAS AudioFile 和 MP3 passthrough、全部 284 XML metadata TypeId（62 族）以及四个 APT 数据类型。未认证类型继续 owner 编译。

缓存判定先于后端准备。命中沿用原 Commit(location)，存在在途任务时仅排有序提交占位。Worker 只处理独立输入；owner 验证、提交和完成文档。跨文档任务在重复 include、manifest/link 和 stream 完成前汇合。失败时先 join 所有 worker，再保存已完成的部分会话，最后释放 Core 状态。

`--jobs auto|N` 默认 auto；1 为原 owner 路径。线程数不进入旧缓存身份。原 sc/bc/csc/psc 默认 true；fsc/atc 默认 false。使用 Win32 可用物理/虚拟内存与 CPU 共同准入，任务队列和 retained bytes 有界。

未引入动作缓存、动作 Key、result.bin、额外文件哈希或输入全根快照。只暂存私有 CDATA，汇合后移动到原逻辑位置。

`cache.parallel` 在 performance summary/detailed 提供真实调用/重叠峰值、prepare/wait/compute/commit、降级原因和采样内存；detailed 最多记录 512 个调用区间。off 不建立逐类型字典、计时或区间；调度所需资源采样仍启用。

待验收：Debug/Release 生产构建、全认证矩阵、旧缓存冷热/失效/取消回滚、原字节差分、完整 BIG/SKUDEF/Additional、同机 Upheaval jobs1/4/auto 对照。旧分支证据只能说明未变后端基础，不能替代本次集成验收。

## 检查点与验证中状态

- `d9de5f69`：完整生产资格、调度、CLI 与旧缓存接线，未验收检查点。
- `eb35dc6b` / Windows [40907675933](https://github.com/wu162/NewSdk/actions/runs/40907675933)：Debug/Release 生产构建通过，组件生命周期失败，独立组仍在收集；未标为通过。后续补上有效 mesh 输入、native 失败/取消 drain 用例、引用复用屏障，以及短暂内存压力结束后的资源恢复。
- Linux 静态预检：[managed.json](../evidence/refactor/backend-parallel/local-preflight/managed.json)、[fixture-schema.json](../evidence/refactor/backend-parallel/local-preflight/fixture-schema.json)。Foundation 和独立 App 实际编译成功；六份 mixed fixture 经50个可达 SDK XSD 定义校验；这不是 Windows mixed 链接或运行证据。
- 源参考认证审计 [XML_BACKEND_REENTRY.json](XML_BACKEND_REENTRY.json) 保留原来源 SHA 和范围，未改写为本轮新验收。

资源策略只在已有 wave 期间保持较低准入；队列完全汇合后可按新观测恢复，不让一次压力尖峰永久限制后续文档。任何释放异常挂在第一原异常上，所有 worker 都先 join。此处后续会补入终态、归档 digest、实际覆盖与性能表。

首批终态 failure 已核实：新增 mesh 组件样本缺必需顶点流；既有 CompilerCacheContent 反射调用未随先前 FileHashCache 可选性能参数更新。两处测试均已修正，未降低原字节断言。该批 Debug/Release 的 284×四平台 XML 矩阵全部成功，混合生产52用例、原 BuildCache/SessionCache、Texture、输入根、真实 CtrlBreak、BIG/MakeBig 差分均通过；完整 mymod 和最终五文件因组件门槛失败未执行。归档 SHA256 `b440d5f8e2cb40151190b044f0c6edba58ea587298859555c8f4da92721638e3` 已下载复算匹配，[分组摘要](../evidence/refactor/backend-parallel/40907675933-1/summary.json)。

最终候选还确保故障/取消 drain 中的每一次实际后端调用都由 owner 合并进 core.asset.compile/core.compiler 计数；没有通过的 Windows 新候选结果或性能数据前，不把这些检查点写成已验收。

### 生产惰性包装器接线修正

复查首批计量发现 `fallbackByReason.uncertified-plugin=30`、prepareMs=0、实际峰值=1：目标分支的 LazyPlugin 包装器使精确类型门禁看不到真实插件。首批生产52用例只证明字节/缓存，不能当作生产并发证明；XML组件的真实重叠也不能替代生产接线。已加入 owner-only `BuiltinCompilers.ResolveBackendPlugin`，仅在确认缓存未命中且资源允许时解包已知内建惰性包装器，再按真实实现执行同一精确资格门禁。保持热态原惰性优化，worker不访问包装器/注册表。

生产矩阵新增 actual overlap 和八个认证类型的真实 worker 线程断言（资源始终允许时）；否则拒绝验收。`15b26f4f` / Windows 40912075609 是解包修正前候选，后续终态会保留但不作为最终并发验收。没有取消或掩盖旧运行。

### 完整 owner 提交合同及最终候选

补齐两个实质异步边界：OutputManager 在复用同一 BinaryAsset 对象、替换 declaration 前，仅按发现顺序提交到该资产，后面的独立任务继续在途；缓存提交占位也参与此身份屏障。actual Commit 由捕获原文档上下文的 owner 回调执行，随后更新原缓存计数和 LastWrittenAsset；BinaryAsset 的 Commit(location) 恢复原 internal 可见性，无新的结果授权或身份链。

兼容调用计量移到 Core 真正调用 ProcessInstance 的边界，准备/路径失败不虚计 backendCalls；worker 故障/取消的实际调用均在安全 join 后合并。新增不同文档的同内容资产夹具，断言原 Texture/Hierarchy 只编译一次。旧 FileHashCache 反射测试两个构造点均匹配现有可选参数；drain 测试保留65536帧 native 分配/序列化，关闭与调度目标无关的二次方 key reduction 搜索，不削弱故障/取消/释放/性能计数断言。

`3d655274` / Windows [40912889949](https://github.com/wu162/NewSdk/actions/runs/40912889949) 已通过要求真实 worker/overlap 的混合生产矩阵，但组件门槛仍失败，终态及日志继续保留；最终候选会统一重建，不借该组代替新版完整验收。
