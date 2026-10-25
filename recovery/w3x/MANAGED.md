# 第一批可编译 C# 恢复

用户要求先快速恢复 C#，因此直接基于固定 ILSpy C# 和原始 IL 实现托管部分，再集中差分。IL 是编译结果，不能据此断言原始语言为 C#；本批将适合独立托管实现的行为重写成可编译 C#，不把机械反编译输出视为完整恢复版。

## 源码与边界

| 源码 | 已实现行为 | 原版依据 |
| --- | --- | --- |
| `src/Managed/W3x/W3xCatalog.cs` | 平台版本、两种类型的完整 ExtendedTypeInformation、未知类型默认值、all-types hash | Plugin `0x06000148`–`0x0600014A` 和 Core ExtendedTypeInformation |
| `src/Plugin/W3x/Plugin.cs` | 构造状态、Initialize/ReInitialize、虚方法语义、ProcessInstance 分派和未知类型警告/null、Dispose/finalizer 行为 | Plugin `0x0600013F`、`142`、`143`、`147`、`14B`–`14E` |
| `src/Managed/W3x/IW3xAssetCompiler.cs` | 后续 native/C++/CLI 处理器接入接口 | 新建共享边界，不冒充原版 ABI |

`HandleAnimation`/`HandleMesh` 当前是 backend 适配器，**不是原版两方法业务算法的恢复**；无 backend 时抛 NotSupportedException，不转发原 DLL。`FinalizeTracker(Tracker*, EndianHelper*, AssetBuffer)` 的原生指针 ABI 仍待现有 shared Tracker 与桥接层恢复。本批没有宣称原公开 ABI 完整兼容、真实三缓冲区通过、动画/网格可用或 SDK 接管成功。

没有新增生产代码对原 W3X DLL 的引用。`Recovered.W3x.Managed.dll` 只编译以上3个源文件，直接引用现有 BAB Core，保留可合回/与旧组件共用的目录。正式配置和 SDK/tools 原版文件不修改。

## 容易被简化错的合同

- 默认平台 Win32；未知平台版本0，不能照搬 Tokenizer 版本规则。
- Animation 的 TypeHash 是 `1430609896`，processing seed 是 `0xD6505CE1`；二者不同。
- 未知 TypeId 不调用虚拟 GetVersionNumber；已知 TypeId 应调用，保留派生类覆盖效果。
- ReInitialize 虚调用 Initialize，不直接复制赋值或忽略派生类。
- Initialize 使用 `HashProvider.GetTypeHash(GetType())`；此算法包含类型名和模块 MVID，恢复程序集内的私有 hash 自然不同。复现算法，不硬编码原程序集的私有值。此字段不用于上述公开返回值。
- ProcessInstance 依据 declaration.Handle.TypeId 分派；未知类型输出与原版相同警告后返回 null。空/未初始化 declaration 的原有错误行为保留。
- Dispose 原版无业务资源清理和 disposed 状态门禁。重复 Dispose 后仍能初始化及读取元信息；不能擅自加入 ObjectDisposedException。
- Tokenized、UseBuildCache 等未设置字段保留 Core 默认值，不借用旧插件标记。

## 验证入口

`recovery/scripts/TestW3xManaged.ps1`，工作流 `Recovered W3X managed`。同一批跑 C# `/optimize-`、`/optimize+`，每种包含7个平台值（0/1/2/99/-1/int.MinValue/int.MaxValue）、4096个确定性散列ID及已知/边界ID、全部7个公开字段、对象独立性、虚方法和生命周期差分。

Known dispatch 的原版调用目标由实际原 DLL 方法 IL 交叉检查，backend 的输入/平台/结果身份与异常传播由测试处理器检查；这不是原版动画/网格算法的差分。无原 DLL 进程只验收托管元信息层。完整 native 算法仍须 /Od、/O2、真实三缓冲区差分与 SDK 门槛。

最终源码 `922559600a1f78fa67cc8fe6252994466c7549ce`，run `40163598580` attempt1 success。两种模式各28709类型记录、201054断言、8次backend分派检查，无原W3X DLL元信息重放均PASS；已核对实际日志、终态与sourceCommit。

运行输出保存在 `recovery/evidence/w3x/managed-runs/40163598580-1`，清单明确 `nativeHandlersValidated=false`、`sdkValidated=false`。成功结果只能对应实际源码提交；后续本批源码无变更时复用此锚点，不因文档提交重复跑Windows。
