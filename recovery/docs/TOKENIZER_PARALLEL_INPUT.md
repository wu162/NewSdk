# Tokenizer 并行与借用后端输入

2026-10-30，`recovery/legacy-sdk-cache`。本轮基于 fbf51cee；实现与测试已提交后由生命周期工作流统一重建验证。旧实现的二进制与验收见 [历史记录](BACKEND_PARALLEL_MIGRATION.md)，不能代替本轮结果。

## 开放的三种类型

`PreparedBackendWork.Eligibility` 仅允许精确内建插件及其唯一 TypeId：GameObject 2486173485、WeaponTemplate 2496977262、ArmorTemplate 980180622。未知实现、错误 TypeId 与原有未认证路径保持 owner 屏障。

三种 Bridge 的 `ProcessInstance` 只读 `Node`、`Document.NamespaceManager`、`Handle.TypeId` 和初始化后不变的 platform。`CompileXml` 创建局部 map/vector/输入结构；Native 的 `CompileGameObject`、`CompileWeapon`、`CompileArmor` 使用局部 Relo::Tracker，常量字段/模块/枚举表没有运行期写入。CRT 标量解析不改变全局 locale。输出数组属于每次调用。三个插件可选 trace 的 GUID 文件名独立，共享 calls.tsv 追加由各自 TraceGate 同步；正常编译没有增加全局锁。

## 可以删除的复制及源码依据

| 删除项 | 可以直接删除的依据 |
| --- | --- |
| `CopyBackendXml` 全树及属性复制、私有 XmlDocument/NameTable | `DocumentProcessor.ProcessDocumentContents` 先完成 ProcessExpressions/ProcessOverrides/Validate，再调用 ProcessInstances；认证 Bridge 不修改 DOM。直接借用还保留原默认属性的 Specified/SchemaInfo 及祖先 XML 上下文。 |
| namespace 字典、prefix 列表和新 XmlNamespaceManager | 绑定在 AssetDeclarationDocument.Load 时构造，认证后端只读取；OpenDocument 在重开 pending 文档前 Flush。XPath 位置和查询对象仍为每次调用局部值。 |
| InstanceHandle 和三种引用列表/引用句柄的复制 | 后端只用 TypeId，引用插槽已由前端写入 XML；不遍历目标资产、读取引用缓存或注册 HashProvider 状态。OutputManager.GetBinaryAsset 在替换同资产声明前 FlushAsset。 |
| 普通任务的新 InstanceDeclaration/CurrentState | worker 结束后 owner 才 Verify/Commit/RecordCommit；其中清除 XmlNode 的行为在 GetResult 之后。无 CDATA 后端无需写声明。 |
| CDATA 声明的语义重新初始化 | 仅浅拷贝 CurrentState，隔离 CustomDataPath；所有只读引用共用。绕过 XmlNode setter，避免重新计算句柄及移除 inheritFrom。 |
| 调度器的一次重复整树估算、纯内存任务的 List<FileStream> | 准备不再分配 DOM，先按队列容量等待，在 TryPrepare 内估算一次，随后用实际工作集估算控制总 retained。没有外部文件就不建立租约集合。 |

文档 MakeComplete/MakeCacheable/Reset 已通过 DeferCompletion 延后到该文档所有序列提交；引用 stream 和 manifest/link 前 Flush。同一资产及同文档复用都保留现有屏障。失败/取消先 join 所有 native 调用，再释放 Core/插件和输入租约。借用契约不允许在 standalone TryPrepare 与 Execute 之间任意修改原输入；旧的防御性 owner mutation 测试已改为实际生产生命周期和借用身份检查。

## 必须保留的隔离和转换

- CDATA 私有路径及提交时移动：Audio 会写入文件，不能与原声明/目标路径共享写状态。
- 每次调用的 XPath cursor、Native 字符串和树、Relo::Tracker 及三个托管输出数组：对应 C++ ABI/拥有期和后端写入，无法通过删除托管准备阶段的复制一并删除。
- Audio/APT 外部输入只读租约：后端按原文件路径重新打开；取消和失败的 join 前不能释放。
- 内存工作集估算：DOM 不复制仍有 Bridge/Native 的输入展开和输出分配。原 DOM 字符/节点估算保留作工作集界限，三种 tokenized 类型加入四倍模块/过程树余量；W3X 帧/通道及外部输入倍率保留。

## 集中验证范围

新增生产夹具的24个 tokenizer 资产及其重复文档已通过原 SDK aggregate Schema；缓存/jobs 矩阵要求每类只编译8次，并确认三类都实际进入 worker。组件用同一 DOM/NameTable 的36次 tokenizer 调用覆盖四平台0/1/2/99，比较三缓冲、DOM 未修改及 trace 索引24条/类型；原 XML284×四平台矩阵继续执行。原缓存热态零准备、部分失败/重试、真实 CtrlBreak/native drain、mymod 原 BAB 四流/BIG/SKUDEF 与完整 Upheaval 同 VM jobs1/4/auto 均由既有生命周期工作流统一执行。最终运行来源和真实通过范围回填 RESUME。
