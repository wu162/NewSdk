# W3X：动画与完整网格源码已写入，组件差分收敛中

更新：2026-10-25（北京时间）。目标分支 `recovery/w3xcompiler-source`。本轮最多六个子代理；用户要求正确性优先、效率优先、最多一小时，**不做SDK真实验证**。

## 当前实现

已恢复独立C#托管元信息/生命周期/分派（沿用先前通过记录，不重复调查）；新增 Timecoded、AdaptiveDelta、动画帧展开/压缩选择/三缓冲区输出、8种网格转换、布局/骨骼/顶点组件，以及完整W3DMesh root96、Triangles、bounds、AABTree、Shader与四类常量。两条XML组件桥接收BAB已完成预处理的输入。新代码在 `src/Native/W3x`、`src/Bridge/W3x`；公共Tracker/标量/资源源文件只链接复用，没有修改。

原DLL固定取证：`runs/40162934653-1`，SHA-256 `bff82309b8e5e9f689112202e3613ad335148934c92dc70fe143f6e60d9ad3f1`。能力合同见 TIMECODED.md、ADAPTIVE_DELTA.md、ANIMATION_PIPELINE.md、MESH_CONVERTERS.md、MESH_BUILDER.md、MESH_ASSET.md、MESH_XML.md、SHADER.md。

## 已取得的精确验证范围

源码 `4085f0a0073a1be1f8e2ac622e6ffed5489980d9`，run `40165527804` attempt1：**整体failure**（动画XML桥C++/CLI编译C2664），但 `/Od`、`/O2`全部7组独立原生检查与4组真实原DLL差分均PASS：converter9937 fixtures/1589920bytes；delta73728帧+324通道、884 serialized chunks；vertex-data1296 fixtures；Timecoded3847通道、6414 serialized chunks。日志、逐组validation、完整原始artifact.zip保存在 `evidence/w3x/native-runs/40165527804-1`。这是纯原生能力的有效证据，不能标为完整动画XML通过。

首轮run `40165200024`/source `b6dd6bd3bddfb07f01bdb6530d98f87aa5302bfa`发现BE浮点换位后sNaN quiet差异，已按原CLR行为修复并经第二轮差分验证；失败证据完整保留。整数换位的Mesh envelope/Shader float list保留原bits，不错误套用float-return quiet规则。

当前已修复XML桥 `marshal_as`临时句柄编译问题，并加入完整Mesh XML桥、完整网格原ProcessInstance三缓冲区矩阵，以及raw sNaN载入用例。**这一扩展批次尚待Windows新运行结果，不宣称已经验收。** 执行入口 `scripts/TestW3xNative.ps1` / workflow `recovered-w3x-native.yml`；所有独立组两模式均尝试，任何失败仍使整体失败。工作流只读并上传artifact，由会话保存唯一目录证据。

此前托管层已验收源码 `922559600a1f78fa67cc8fe6252994466c7549ce` / run `40163598580`，后续文档提交复核run `40163729737`；细节见MANAGED.md。本轮未改这些托管源，因此未重复触发旧元信息和其他编译器全量回归。

## 未完成门槛与下一步

先收敛当前Windows完整矩阵并核对实际sourceCommit/runId、所有原PASS日志与终态，再保存成功证据。当前正式Plugin默认backend仍显式拒绝未集成业务；SDK配置/原DLL不替换。公开FinalizeTracker的VC8指针对象ABI、完整程序集身份替代、正式插件接入、SDK/无原DLL整链验收仍是独立门槛。共享Tracker不是原EASTL对象ABI；序列化仅支持x86。数值溢出/非有限值、非法长度等显式拒绝范围详见能力合同，不能以源码存在宣称任意输入已兼容。
