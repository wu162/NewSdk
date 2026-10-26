# P5 生命周期审计与最小实施组

状态：2026-10-26 P4 已验收。文档/配置清理、Hash/Tracer 作用域及 Metrics/AssetReport 资源恢复已编写，Windows 验证待完成。
本页保存设计依据；具体实现以源码及 Core lifecycle 作业为准，不代表生产 `build` 已接入或 SDK/G3 已通过。
路径均相对 `recovery`；首版仅顺序执行，不能并行修改旧静态状态。

## 第一组：真实 Core 异常清理（可独立于后端实现）

- `src/Managed/Core/BinaryAssetBuilder.Core/DocumentProcessor.cs` 的 `ProcessDocumentInternal` 拥有一次递归调用。
  它 push 静态 `_CurrentDocumentStack`/`_CurrentStreamStack`，add 实例 `_DocumentStack`，设置 document.Processing。
  当前这些状态仅在成功路径清理，异常后同 cache 的 document 会被误判为循环引用。
  用 finally 对称恢复本次调用拥有的帧和 Processing；记录进入深度，嵌套调用不能清空父帧。
- 将 `ProcessDocumentContents` 的 `_CurrentStreamStack.Pop()` 和 `_DocumentStack.RemoveAt(...)` 清理所有权
  移回调用方；同步删除原成功路径清理，避免双 pop。MakeComplete/MakeCacheable/SaveDocumentToCache 仍只在成功后执行。
- `ProcessProjectDocument` 切换 `Settings.Current` 后只在成功路径恢复，`_CompilingProject` 和默认配置表也会残留。
  每配置 finally 恢复 Settings；外层 finally 恢复项目标志/表；reader 使用 using，不能以资源清理掩盖原编译异常。
- 本组不接入 CLI，不改 HashProvider/Tracer/Metrics，不复用失败后的生产缓存；先在真实 Core 宿主验证递归与配置清理。
  要测试同 processor/cache 的底层清理，但生产后端仍应在失败后丢弃该流 cache，不能据此宣称脏 cache 可安全复用。

## 后续组：作用域、资源及后端

| 状态所有者 / 函数 | 已查明风险 | 建议边界 |
| --- | --- | --- |
| DocumentProcessor 静态 `_MissingReferences`、计时/类型耗时和两份栈 | 新 processor 仍共享；ResetTimers 只复位计时 | 顺序租约拒绝重入；建立明确构建/流边界，恢复或复位全部拥有状态 |
| `BinaryAssetBuilder/Settings.cs` Current / Clone | Clone 是 MemberwiseClone，描述符和数组共享 | 每流新 Settings，finally 恢复原引用；不修改共享描述符 |
| SettingsLoader.PostProcessSettings / ProcessPaths | ProcessPaths 隐式读取 Current.DataRoot | 安装该流 Current 后才后处理；后续改为显式传 DataRoot |
| HashProvider.InitializeStringHashes / FinalizeStringHashes | 全局字典、目录、文件名无恢复；初始化可能半完成 | 局部构造后安装的 scoped state，finally 恢复原状态；字符串表按流归属 |
| Tracer.TraceWrite / SetTraceLevel | 全局委托、mask；无 mask 读取接口 | 保存并恢复委托和 mask；日志异常不能中断其余清理 |
| MetricManager.AddListener / OpenSession / CloseSession | 无 Remove；Open/Close 任一 listener 抛错会留下半开资源或 session | 对称注册；失败逆序关闭已打开 listener；Close 尽力关全部并 finally 标关闭 |
| AssetReport.InitAssetRecordWriter / Close | 静态 writer/路径/记录表；Close 自身抛错也会残留 | 无条件 Dispose 并清字段；流失败后 staging 必须立即可删 |

建议顺序：第一组异常清理 → 静态状态/日志/metrics/report/hash 作用域 → IStreamCompiler 真实后端 → 缓存 → G3/SDK。
`decompiled/BinaryAssetBuilder/BinaryAssetBuilder/BinaryAssetBuilder.cs:DoBuildData` 可参考初始化顺序，
不能照搬其进程退出假设，也不能永久设置 Current.SingleFile=true / BuildCache=false。
后端每流新建 Settings、SchemaSet、SessionCache、DocumentProcessor；只读共享资源需另有验证依据。
先关闭持久 session/build cache；缓存后续需新命名空间和失效验收，不能无条件读旧 BAB 缓存。

## 真实 Core 同进程测试入口与矩阵

- 参考 `scripts/TestCompilerFoundation.ps1` 的独立 fixture 模式，引用生产对象 validation host，
  由 `$run.exe` 装载 fixture；生产 EXE 不加测试入口，不放回 Core 等业务 DLL。
  Windows x86 / Framework 4.8，Debug/Release，收据绑定 sourceCommit、fixture、生产/宿主两份 EXE 身份。
- fixture 使用真实 Settings/SchemaSet/SessionCache/DocumentProcessor；SessionCache.InitializeCache(new List<string>())
  建立冷缓存，processor.Cache/SchemaSet 显式赋值；具体 schema 与插件 fixture 需按实现时实际依赖确定。
  成功样例必须走真实 Core include、XSD、实例与输出链，不能用 stub 代替后端验收。

| 同进程序列 | 必须断言 |
| --- | --- |
| 成功 A → 成功 B，同类型/ID、不同值和根目录 | B 四份产物与独立冷进程一致，无 A 字符串表、报告、路径残留 |
| 深层 include 失败 → 修复输入 → 同路径重试 | Processing 与三份栈恢复，真实失败类型准确，无伪循环引用；另保留合法循环拒绝 |
| babproj 第二配置失败 → 普通 XML 成功 | 原 Settings 引用/配置/postfix/路径、项目标志和默认配置表恢复 |
| 报告或 StringHashes 输出中途失败 → 重试 | staging 立即可删除/重命名，writer 关闭，重试输出与冷进程一致 |
| 预置 Tracer/metrics → 连续 session，注入 listener Open/Close 失败 | 原回调恢复、无重复输出，已打开资源全部关闭，后续 session 可打开 |
| 流边界取消 → 下一 session 成功 | 无半产物公布、全局状态恢复；再以反序执行排除固定顺序依赖 |

第一组先实现第二、第三行并覆盖递归早退和正常成功；后续各作用域就绪后再扩展其他行。
底层测试可反射检查私有状态，输出正确性与拒绝行为仍须走公开真实 Core 调用。
完整 P5 还需计划规定的无/冷/热缓存、依赖变化、连续 MOD、取消/失败后重试及真实 SDK/G3 输出对照。
