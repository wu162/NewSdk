# 本轮已闭合各类型，已授权启动SDK对照（2026-10-25）

用户最新指令：完成各类型还原后才启动SDK真实验证，本轮最多一小时。新增合法数值域补全、34个命名XSD类型归类和默认可加载NativePlugin。源码 `bb2de50446b1012b5107613a88a4ce3b36e42e16` / run `40167490640` attempt1 原生双模式、正式插件原版及无原DLL对照、完整原SDK XSD四文档前置检查全部success；上一轮门槛不再代表尚未还原。SDK_READY.json已绑定此真实成功源码、七个依赖树和closure指纹。下一工作流先重核同一业务代码/依赖并重跑当前源门槛，再运行三份真实SDK；尚未宣称SDK通过。

下方保留上一轮业务能力与验证锚点作为历史；最终SDK结果在本轮结束时更新。

---

# W3X：完整动画与网格组件通过双模式原版差分

更新：2026-10-25（北京时间）。目标分支 `recovery/w3xcompiler-source`。本轮最多六个子代理；用户要求正确性优先、效率优先、最多一小时，**不做SDK真实验证**。

## 当前实现

已恢复独立C#托管元信息/生命周期/分派（沿用先前通过记录，不重复调查）；新增 Timecoded、AdaptiveDelta、动画帧展开/压缩选择/三缓冲区输出、8种网格转换、布局/骨骼/顶点组件，以及完整W3DMesh root96、Triangles、bounds、AABTree、Shader与四类常量。两条XML组件桥接收BAB已完成预处理的输入。新代码在 `src/Native/W3x`、`src/Bridge/W3x`；公共Tracker/标量/资源源文件只链接复用，没有修改。

原DLL固定取证：`runs/40162934653-1`，SHA-256 `bff82309b8e5e9f689112202e3613ad335148934c92dc70fe143f6e60d9ad3f1`。能力合同见 TIMECODED.md、ADAPTIVE_DELTA.md、ANIMATION_PIPELINE.md、MESH_CONVERTERS.md、MESH_BUILDER.md、MESH_ASSET.md、MESH_XML.md、SHADER.md。

## 已取得的精确验证范围

最终验收源码 `d98844353d680f1e2cf28e26dc3df3c4d8a04eed`（业务实现与 `41075642ed79ebb06509e00047c22e81cf1248f8` 相同，仅追加捕获/无原DLL重放测试），run `40166287709` attempt1，workflow/job/全部步骤 **success**。已经核对实际双模式PASS、artifact SHA-256、validation.sourceCommit及所有逐组布尔状态。

| 每优化模式（/Od、/O2均通过） | 原版差分范围 |
| --- | --- |
| 完整动画XML→ProcessInstance三缓冲区 | 675 fixtures / 2025 buffer comparisons，平台0/1/2/99（99仅显式设置） |
| 完整网格XML→ProcessInstance三缓冲区 | 651合法形状+15单列raw边界 / 1998 buffer comparisons，平台0/1/2 |
| 网格转换 | 9947 fixtures / 1591520 bytes，728原版独立overflow/NaN probes不计支持范围 |
| AdaptiveDelta | 73728帧+324通道、884 WriteOut/Finalize serialized chunks |
| Timecoded | 3847通道、6414 WriteOut/Finalize serialized chunks |
| 孤立顶点组件 | 1296 fixtures，原BuildVertexData及Tracker三缓冲区 |

全部9组独立x86原生检查也通过，包括Shader与完整Mesh根/分配/重定位布局。每模式另在移除原W3X DLL的独立目录重放1341 fixtures，4023三缓冲区比较、540216字节，并核实原程序集未加载。两模式共2682 fixtures、8046 replay buffer comparisons；这是组件重放，不是SDK整链。不可变证据 `evidence/w3x/native-runs/40166287709-1`，原始artifact.zip SHA-256 `1deef30ca9c11bce9fb76ce700e1ee3db003cf560487a6c5f31eec90bbdac7eb`，包含四份原版binary fixture captures及其指纹。此前完整组件成功source41075642/run40165968285也保留。SDK0次。

首轮 `40165200024` / source `b6dd6bd3bddfb07f01bdb6530d98f87aa5302bfa` 发现BE交换后sNaN quiet差异；第二轮 `40165527804` / source `4085f0a0073a1be1f8e2ac622e6ffed5489980d9` 全部纯原生组通过，但XML桥因marshal_as临时句柄C2664编译失败。均已修复，最终完整矩阵通过；两次失败原始archive及逐组结果完整保留。整数换位的Mesh envelope/Shader float list保留原bits，未错误套用float-return quiet规则。

此前托管层已验收源码 `922559600a1f78fa67cc8fe6252994466c7549ce` / run `40163598580`，后续文档提交复核run `40163729737`；细节见MANAGED.md。本轮未改这些托管源，因此未重复触发旧元信息和其他编译器全量回归。

## 未完成门槛与下一步

完整组件矩阵与移除原DLL后的组件重放均已成功，不为文档变更重复运行。当前正式Plugin默认backend仍显式拒绝未集成业务；SDK配置/原DLL不替换。公开FinalizeTracker的VC8指针对象ABI、完整程序集身份替代、正式插件接入、SDK预处理/正式插件/无原DLL整链验收仍是独立门槛。共享Tracker不是原EASTL对象ABI；序列化仅支持x86。数值溢出/非有限值、非法长度等显式拒绝范围详见能力合同，不能以源码存在宣称任意输入已兼容。

本轮六个子代理按算法/编排/桥/原版oracle分工，交叉核对后整批运行；没有重复原始取证、已通过的托管元信息或其他编译器矩阵。无账户额度计量，不能量化节省。最终只保留只读组件工作流，未修改已有回归触发器、SDK路由或原DLL。
