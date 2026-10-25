# W3X native 算法

`Timecoded`、`AdaptiveDelta` 与 `AnimationPipeline` 对应原动画压缩及帧展开/选择流程；`AnimationCompiler` 复用公共Tracker组装动画三缓冲区。`MeshConverters`、`MeshBuilder` 对应顶点转换、layout/declaration和骨骼映射，`MeshSerialization` 仅导出孤立的28字节vertex-data根，不是完整W3DMesh资产。

所有接口使用显式typed输入，并非VC8内部容器ABI。Tracker序列化只支持x86宿主。公共Relocation/ScalarParsing源文件只链接复用，不修改其既有行为。XML动画组件桥在 `src/Bridge/W3x/AnimationCompiler.cpp`，正式Plugin默认backend仍拒绝未完成集成验收的能力。

实现依据为 `evidence/w3x/runs/40162934653-1` 固定原DLL、IL和布局。独立检查和原DLL组件差分的当前状态以 `w3x/RESUME.md` 为准，不能将源码存在或本地单元检查当作整DLL/SDK验收。数值/越界拒绝边界分别记录于能力合同。
