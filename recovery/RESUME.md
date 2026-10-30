# TextureCompiler：PC续接

分支：`recovery/texturecompiler-source`，仅PC，禁止强推，编译测试仅Actions。

## 本轮完成的实现（2026-10-30）

- 新增独立x86 `TextureNativeLoaderApi`：52字节资源头、主/secondary VM及packed-tail所有权；raw DDS A8/X8部分/完整mip与缺层生成，POT TGA24/32 raw/RLE/metadata；接通LockRect→既有Copy→UnlockRect→引用/最终释放。
- 修复原LoadImageA实际format清除`0x1c0`后flags12/threshold0的消费契约；TGA全零alpha选择X8但保留基础alpha0；BOX退化轴使用原浮点存储/向零量化，不能统一整数平均。
- 压缩DDS仍未恢复：原版会重新编码已提供的2×2/1×1尾mip，基础轴小于4也会扩展/重压缩。全部样本保留，105个原版成功压缩资源另存完整取证；恢复版明确拒绝，不能称作原版拒绝匹配或压缩验收完成。
- 业务源码 `5fc678b050dcc5aff5a8a25365a406a09ca2f4c8`，集中补验[40999035840](https://github.com/wu162/NewSdk/actions/runs/40999035840)进行中，**尚未取得该批完整成功**。之前四次失败收据原样保留。矩阵为6860构造、2789文件（2684支持范围完整对照+105明确未支持压缩边界）及另5拒绝；D/R、NoOriginal、完整header/全部VM/消费/释放门槛不变。
- 新ABI未加入生产六DLL包，完整混合模式身份未恢复；SDK/tools及生产配置本轮未改。详细范围见[原生文件加载](TEXTURE_NATIVE_FILE_LOAD.md)。

## 下一轮计划

1. 先收上述补验：只有完整D/R、NoOriginal与本批成功收据齐全，才能标为已验证；若失败只修失败/实际改变输入，不重跑未变旧组或SDK/MOD。
2. raw加载链闭环后，优先接入已有8位NPOT权重/精度，或按实际格式接入更多raster解码。保持原生格式和存储精度，不能只比最终RGBA。
3. 压缩DDS另恢复D3DX重编码/压缩转raw消费，不能用现有nvDDS压缩器近似替换；生产集成另取必要SDK证据。

旧转换验收锚点：`6af9af189a5fb9be63bd02fbab4577d71c583257` / [40965216216](https://github.com/wu162/NewSdk/actions/runs/40965216216)，63组件旧声明范围成功。旧锚点不证明新loader。当前补验选择native-loader/base，其他62组保留各自原sourceCommit/runId。具体按[执行规则](../AGENTS.md)、[测试规则](TEXTURE_TESTING.md)执行；不默认追加收尾plan或完整MOD。
