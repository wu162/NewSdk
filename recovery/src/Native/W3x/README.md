# W3X native 算法

`Timecoded`、`AdaptiveDelta`、`AnimationPipeline` 和 `AnimationCompiler` 实现动画压缩、帧展开/格式选择及三缓冲区输出。`MeshConverters`、`MeshBuilder`、`MeshSerialization` 实现顶点转换/布局/骨骼及孤立顶点组件；`MeshAsset` 和 `Shader` 实现完整网格根、三角形、包围体、AABTree与材质/常量序列化。

所有接口使用显式typed输入，并非VC8内部容器ABI。Tracker序列化只支持x86宿主。公共Relocation/ScalarParsing/ResourceMarshaling只链接复用，没有修改。XML组件桥位于 `src/Bridge/W3x`，正式Plugin默认backend仍拒绝未完成集成验收的能力。

实现依据为 `evidence/w3x/runs/40162934653-1` 固定原DLL、IL和布局。各能力合同及 `w3x/RESUME.md` 记录实际验证锚点和拒绝范围；源码存在或本地检查不等于整DLL/SDK验收。
