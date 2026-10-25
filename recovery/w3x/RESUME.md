# W3X：完整动画与网格组件通过双模式原版差分

更新：2026-10-25（北京时间）。目标分支 `recovery/w3xcompiler-source`。本轮最多六个子代理；用户要求正确性优先、效率优先、最多一小时，**不做SDK真实验证**。

## 当前实现

已恢复独立C#托管元信息/生命周期/分派（沿用先前通过记录，不重复调查）；新增 Timecoded、AdaptiveDelta、动画帧展开/压缩选择/三缓冲区输出、8种网格转换、布局/骨骼/顶点组件，以及完整W3DMesh root96、Triangles、bounds、AABTree、Shader与四类常量。两条XML组件桥接收BAB已完成预处理的输入。新代码在 `src/Native/W3x`、`src/Bridge/W3x`；公共Tracker/标量/资源源文件只链接复用，没有修改。

原DLL固定取证：`runs/40162934653-1`，SHA-256 `bff82309b8e5e9f689112202e3613ad335148934c92dc70fe143f6e60d9ad3f1`。能力合同见 TIMECODED.md、ADAPTIVE_DELTA.md、ANIMATION_PIPELINE.md、MESH_CONVERTERS.md、MESH_BUILDER.md、MESH_ASSET.md、MESH_XML.md、SHADER.md。

## 已取得的精确验证范围

最终业务源码 `41075642ed79ebb06509e00047c22e81cf1248f8`，run `40165968285` attempt1，workflow/job/全部步骤 **success**。已经核对实际双模式PASS、artifact SHA-256、validation.sourceCommit及所有逐组布尔状态。

| 每优化模式（/Od、/O2均通过） | 原版差分范围 |
| --- | --- |
| 完整动画XML→ProcessInstance三缓冲区 | 675 fixtures / 2025 buffer comparisons，平台0/1/2/99（99仅显式设置） |
| 完整网格XML→ProcessInstance三缓冲区 | 651合法形状+15单列raw边界 / 1998 buffer comparisons，平台0/1/2 |
| 网格转换 | 9947 fixtures / 1591520 bytes，728原版独立overflow/NaN probes不计支持范围 |
| AdaptiveDelta | 73728帧+324通道、884 WriteOut/Finalize serialized chunks |
| Timecoded | 3847通道、6414 WriteOut/Finalize serialized chunks |
| 孤立顶点组件 | 1296 fixtures，原BuildVertexData及Tracker三缓冲区 |

全部9组独立x86原生检查也通过，包括Shader与完整Mesh根/分配/重定位布局。不可变证据 `evidence/w3x/native-runs/40165968285-1`，原始artifact.zip SHA-256 `6b84397380ae0e57089b10e0c651f4ac60070b61d1419a0f8607213a3bf98257`。该结果属于组件验收，SDK0次。

首轮 `40165200024` / source `b6dd6bd3bddfb07f01bdb6530d98f87aa5302bfa` 发现BE交换后sNaN quiet差异；第二轮 `40165527804` / source `4085f0a0073a1be1f8e2ac622e6ffed5489980d9` 全部纯原生组通过，但XML桥因marshal_as临时句柄C2664编译失败。均已修复，最终完整矩阵通过；两次失败原始archive及逐组结果完整保留。整数换位的Mesh envelope/Shader float list保留原bits，未错误套用float-return quiet规则。

此前托管层已验收源码 `922559600a1f78fa67cc8fe6252994466c7549ce` / run `40163598580`，后续文档提交复核run `40163729737`；细节见MANAGED.md。本轮未改这些托管源，因此未重复触发旧元信息和其他编译器全量回归。

## 未完成门槛与下一步

当前正在补充移除原W3X DLL后的完整组件fixture重放；尚未宣称该额外门槛已经通过。完整组件矩阵已成功，不为文档变更重复运行。当前正式Plugin默认backend仍显式拒绝未集成业务；SDK配置/原DLL不替换。公开FinalizeTracker的VC8指针对象ABI、完整程序集身份替代、正式插件接入、SDK/无原DLL整链验收仍是独立门槛。共享Tracker不是原EASTL对象ABI；序列化仅支持x86。数值溢出/非有限值、非法长度等显式拒绝范围详见能力合同，不能以源码存在宣称任意输入已兼容。
