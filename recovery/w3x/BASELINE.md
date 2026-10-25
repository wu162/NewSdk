# 首次原版取证基线

## 可追溯锚点

- 输入基点：`c66339e49e32cfe3f44aa2f9f1757f9993bbac7a`。
- 取证 sourceCommit：`3484dcb554a4b0dd6c96f8f832f5099ce4e50ba6`。
- Actions：`40162934653`，attempt `1`，workflow/job **success**；保存证据提交 `1022bf4d5c42387536e0eec0c069716017d7cd97`。
- 不可变目录：`recovery/evidence/w3x/runs/40162934653-1/`；清单 `extraction.json`；ILSpy `9.1.0.7988`，decompilerExit=0、ilExit=0。
- 原版 SHA-256：`bff82309b8e5e9f689112202e3613ad335148934c92dc70fe143f6e60d9ad3f1`；程序集版本 `1.0.3492.22491`，CLR `v2.0.50727`，I386，NativeEntryPoint。
- summary：202 types、342 methods，其中308 IL、34 native；这些包括基础设施和编译器生成方法，**不是342个业务恢复任务或完成率分母**。

`methods.json`/`types.json` 保存原始方法和布局清单，`plugin-methods.json` 包含 Plugin 的16个方法记录（含构造/析构等）。`plugin-contract.json` 是真实 x86 CLR4 宿主读取出的10个 public declared 方法和3个接口；输出已检查 pointerSize=4、原 DLL hash、IAssetBuilderPlugin 和程序集身份。`decompiled` 是机械 C#；完整 IL 与 native disassembly 位于 `raw-listings.zip`。所有这些仍只证明原版取证成功。

## 初步方法地图

| 合同/能力 | 原版方法 token | 读取入口 |
| --- | --- | --- |
| 初始化/再初始化 | `0x06000142` / `0x06000143` | decompiled 中 `BinaryAssetBuilder.W3XCompiler/Plugin.cs` |
| 三缓冲区导出 | `0x06000144` FinalizeTracker | 先核对 MakeRelocatable 与现有 shared Tracker |
| 动画处理 | `0x06000145` HandleAnimation | 两个 tracker、压缩设置、通道/runtime 转换 |
| 网格处理 | `0x06000146` HandleMesh | Marshal、pipeline vertex data、BuildVertexData |
| 类型分派 | `0x06000147` ProcessInstance | Mesh/Animation 两分支与未知类型返回路径 |
| hash/版本/type info | `0x06000148`–`0x0600014A` | 元信息合同，不能照搬旧插件 |
| Timecoded 压缩 | `0x0600000E`–`0x06000010` | Compress / EstimateSize / WriteOut，均为 IL |
| AdaptiveDelta 压缩 | `0x06000011`–`0x06000015` | CompressFrame / Compress / ctor / EstimateSize / WriteOut，均为 IL |
| 网格转换和布局 | `0x060000C2`–`0x060000CB`、`0x060000CD`–`0x060000CE` | W3DMeshProcessor 转换、layout、bone mapping、declaration/data，均为 IL |

34个 native 方法清单主要含 CRT/new/delete、模块启动及 EASTL RBTree 支持；PE imports 为 MSVCR80、KERNEL32、msvcm80、mscoree。不能因此跳过 native 依赖审查。程序集还声明 EALA.ProcessUtility 等引用；manifest 有引用不证明执行链实际需要它，后续构建和无原 DLL 重放应按真实调用核实。

## 已读出的静态合同，待动态 oracle 交叉验证

| 项 | 初步原版行为 |
| --- | --- |
| Mesh TypeId | `3266421346`，TypeHash `3448375452`，ProcessingHash 为 version XOR `0xCD8A089C` |
| Animation TypeId | `608742960`，TypeHash `1430609896`，ProcessingHash 为 version XOR `0xD6505CE1`；不要把 TypeHash 当成 XOR 常量 |
| 插件版本 | Win32=8、Xbox360=9、PlayStation3=11；switch default 返回0 |
| GetAllTypesHash | `1424942948` |
| 未知 TypeId | ProcessInstance 打印警告并返回 null；GetExtendedTypeInformation 仍保存传入 TypeId，未进入已知类型分支 |
| 压缩设置缺失 | HandleAnimation 走平台默认分支；此路径遇未知平台抛错。显式设置与缺失整个节点必须分别探测 |

以上来自 Plugin 方法的机械 C# 与对应 token 导航，不是恢复实现差分证明。后续还需核对完整 IL、Core 默认值、真实调用与边界，不可将“版本99返回0”推广成所有99平台输入都被接受。新恢复组件对尚未实现的已知合法能力应显式拒绝；最后完整替换的未知 TypeId 行为则须与原版合同一致。

## 初次基础设施修正

失败运行 `40162666309`：VS x86 环境影响 MetadataInventory 默认 Platform，x64 dotnet 无法加载；显式 `-p:Platform=AnyCPU` 修正。失败运行 `40162864795`：旧 csc 对带 `/` 的相对源码路径解析失败；传 Resolve-Path 的绝对路径修正。失败 artifact 和原运行保留。最终 `40162934653` 成功，不通过跳过库存读取、插件接口校验或错误检查取得成功。
