# W3X：首批可编译 C# 托管层已通过原版差分

更新：2026-10-25（北京时间）。分支 `recovery/w3xcompiler-source`，基点 `c66339e49e32cfe3f44aa2f9f1757f9993bbac7a`。

## 当前范围

用户本轮要求创建分支、搭还原框架和初步规划。用户进一步明确：这是新的还原项目，不接续旧交接；代码仍在 recovery，未来要合回原工作线，并可能复用/共同重构旧代码。因此新源码按现有 Native/Bridge/Managed/Plugin/tests/scripts 结构组织，项目说明在 `recovery/w3x`，不复制公共 runtime。

目标 DLL：`SDK/tools/BinaryAssetBuilder.W3XCompiler.dll`。Plugin 是该程序集中的 `BinaryAssetBuilder.W3XCompiler.Plugin` 类型；原配置路由 `W3DAnimation,W3DMesh`。

用户随后要求“规划有点谨慎，先快速把C#部分还原出来”，并提示可用ILSpy等工具。已直接复用原版ILSpy C#及原始IL，写出可编译的托管层，未重复做原版取证或先扩充规划。恢复范围：W3xCatalog平台版本/全部类型信息；Plugin初始化/再初始化、虚方法、类型分派、未知类型警告/null、Dispose/finalizer。新增 IW3xAssetCompiler 供后续 C++/CLI 接入，默认路径明确拒绝尚未恢复的动画/网格编译，不转发原 W3X DLL。

最终托管源码 `922559600a1f78fa67cc8fe6252994466c7549ce`；run `40163598580` attempt1 **success**，证据提交 `b36487894c9cf355118c53b2c92de981493cea4f`；不可变目录 `recovery/evidence/w3x/managed-runs/40163598580-1`。已核对workflow/job/所有步骤终态、validation.sourceCommit及两种模式实际PASS日志。C# `/optimize-`、`/optimize+` 各28709类型记录、201054断言、8次backend分派检查，平台0/1/2/99/-1/int.MinValue/int.MaxValue；两模式无原W3X DLL的独立元信息宿主均PASS。未修改已有共用实现，因此没有重复触发旧native全量矩阵。详情及边界见 [MANAGED.md](MANAGED.md)。

**仍未恢复 HandleAnimation/HandleMesh 的原业务算法、原指针ABI FinalizeTracker；没有真实三缓冲区差分、动画/网格通过、SDK验证或整DLL替换声明。** 本轮关闭的是用户要求优先推进的独立C#托管层，不将C++/CLI算法未实现适配器计为已恢复。

原版取证锚点继续有效：sourceCommit `3484dcb554a4b0dd6c96f8f832f5099ce4e50ba6`，run `40162934653` attempt1 success，证据提交 `1022bf4d5c42387536e0eec0c069716017d7cd97`，202类型/342方法（308 IL、34 native）。不把旧项目317/322等计数带入本项目。

## 下一组

下一组优先复用已有 shared Tracker/XmlNodeAdapter，恢复 FinalizeTracker 边界并构造原版三缓冲区捕获；随后按完整动画能力组推进，直接从 ILSpy 方法体恢复 Compress/EstimateSize/WriteOut，遇到未知边界再定向探针。不要重做已经通过的托管元信息调查，也不先扩充一轮规划。

原版证据位于 `recovery/evidence/w3x/runs/40162934653-1/`。按 [BASELINE.md](BASELINE.md) 方法地图定点读取 HandleAnimation、HandleMesh 及其压缩/顶点处理依赖；其中 GetExtendedTypeInformation 动态合同本轮已验收。相同证据/工具链未变时不重复取证。需要改托管接口时运行 TestW3xManaged；需要改共用native代码时运行全部受影响旧组件与W3X回归。

已定位 XSD `SDK/Schemas/xsd/AssetTypeW3D.xsd` 和小网格 `SDK/Art/W3/W3XViewQuad.w3x`；Shader 可研究提取已有 GameObjectDrawBeam 公共实现。初次两条基础设施失败及修正见 BASELINE，最终成功已包含修正后的源码，未跳过校验。

公共代码候选复用和合入方式见 [ARCHITECTURE.md](ARCHITECTURE.md)，验收分层见 [VALIDATION.md](VALIDATION.md)。基点遗留的 XmlCompiler/Tokenizer 文档和源码保留作参考；新项目不继续旧 GameObject 剩余 Draw 待办。后续更新本入口，避免重写旧项目整段历史。
