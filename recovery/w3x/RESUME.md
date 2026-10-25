# W3X 新项目续接入口

更新：2026-10-25（北京时间）。分支 `recovery/w3xcompiler-source`，基点 `c66339e49e32cfe3f44aa2f9f1757f9993bbac7a`。

## 当前范围

用户本轮要求创建分支、搭还原框架和初步规划。用户进一步明确：这是新的还原项目，不接续旧交接；代码仍在 recovery，未来要合回原工作线，并可能复用/共同重构旧代码。因此新源码按现有 Native/Bridge/Managed/Plugin/tests/scripts 结构组织，项目说明在 `recovery/w3x`，不复制公共 runtime。

目标 DLL：`SDK/tools/BinaryAssetBuilder.W3XCompiler.dll`。Plugin 是该程序集中的 `BinaryAssetBuilder.W3XCompiler.Plugin` 类型；原配置路由 `W3DAnimation,W3DMesh`。

已保存取证脚本、x86 原版合同宿主和独立 Windows 工作流，复用现有 MetadataInventory。最终取证 sourceCommit `3484dcb554a4b0dd6c96f8f832f5099ce4e50ba6`，run `40162934653` attempt1 **success**，保存证据提交 `1022bf4d5c42387536e0eec0c069716017d7cd97`。已核对 job/所有步骤终态、两条PASS日志和不可变 `extraction.json`：202类型/342方法（308 IL、34 native），C#及IL退出0，x86真实宿主10个公开方法合同通过。**恢复版业务实现0，组件差分未运行，SDK未验证，原版路由未改。** 不把旧项目317/322等计数带入本项目。

## 下一组

原版证据位于 `recovery/evidence/w3x/runs/40162934653-1/`。先读 [BASELINE.md](BASELINE.md) 的方法地图、静态合同和平台/压缩默认值注意点，再定点读该目录 plugin-contract、plugin-methods、summary，以及 decompiled/IL 中 Plugin、HandleAnimation、HandleMesh 的实际调用和依赖。按 [PLAN.md](PLAN.md) 阶段A完成动态元信息合同、布局、原版三缓冲区输出探针，再启动完整动画能力组。相同证据/工具链未变时不重复取证。

已定位 XSD `SDK/Schemas/xsd/AssetTypeW3D.xsd` 和小网格 `SDK/Art/W3/W3XViewQuad.w3x`；Shader 可研究提取已有 GameObjectDrawBeam 公共实现。初次两条基础设施失败及修正见 BASELINE，最终成功已包含修正后的源码，未跳过校验。

公共代码候选复用和合入方式见 [ARCHITECTURE.md](ARCHITECTURE.md)，验收分层见 [VALIDATION.md](VALIDATION.md)。基点遗留的 XmlCompiler/Tokenizer 文档和源码保留作参考；新项目不继续旧 GameObject 剩余 Draw 待办。后续更新本入口，避免重写旧项目整段历史。
