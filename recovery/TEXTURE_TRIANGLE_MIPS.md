# Win32 raw triangle mip恢复

> **历史记录：自 2026-10-26 起仅以 Win32/PC 为目标。本文非 PC 能力、旧待办和旧平台矩阵不再适用；当前实现与计划以 [TEXTURECOMPILER.md](TEXTURECOMPILER.md) 为准。原 sourceCommit/runId 仅证明原验收版本，不代表本轮删除后的代码已验证。**

源码src/TextureCompiler/Managed/TextureTriangleMipEncoder.cs；无mip RawEncoder API不变。输入为RGBA，输出A8R8G8B8/X8R8G8B8/R5G6B5旧DDS完整mip链，仅Win32默认options profile；不等于DXT压缩或PS3限三级语义。

精确规则：每级直接从base独立重采样；triangle越界裁剪后归一化，权重乘统一倒数；中间图较小的轴先，tie先vertical；每tap double乘加后存float32；按原量化舍入。不得用递归缩图、clamp、先舍入乘积或逐项权重除法替换。

独立组件：sourceCommit f38ec9ac8173d7918f701550cfb51b915e5965b7，运行40199020130，Debug/Release各279组、5038592检查，11拒绝，含随机、极端轴、impulse及完整DDS字节比较。收据在evidence/texture/stage2/Mips/40199020130-1。数值探针40198443312只在测试进程instrumentation，原270组DDS不变，不链接任何恢复业务代码。

实际初始化环境：源码aa2b2d80，ProcessInstance运行40199691351逐buffer/cdata通过；真实SDK mip运行40199691305的原/恢复/移除原目标DLL三组18资产全部产物/轨迹一致。该workflow最后gitpush并发冲突失败，编译与比较步骤成功；仅收据与原Actions artifact链接持久化，见TEXTURECOMPILER.md与sdk-mips的PERSISTENCE.md。不能将此workflow描述为整体绿色。

PS3、Xenon、NPOT重采样及DXT仍不在本helper验收范围。SDK的原native初始化可能影响FPU，因此独立组件成功不能代替真实SDK，现已分别保存两种证据。

2026-10-25第三轮补充：独立TexturePs3MipEncoder及TextureVolumeMipEncoder已完成组件与专用MOD真实SDK验收，分别保留PS3固定三份payload和体积递归先Z后XY算法；本二维helper自身的base-relative契约不变。见[TEXTURE_PS3_VOLUME.md](TEXTURE_PS3_VOLUME.md)。NPOT及PS3自动resize仍未支持。

