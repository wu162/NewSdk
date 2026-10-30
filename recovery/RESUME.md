# TextureCompiler：PC续接

分支：`recovery/texturecompiler-source`，仅PC，禁止强推，编译测试仅Actions。

## D3DX BC 下一批候选（待定向诊断，不是验收）

`validation/texture-public-d3dx-bc` 已接通固定DirectXTex BC1/2/3原源码、标量x86的float块/图像编解码，以及POT base>=4的完整原生DDS资源消费：provided完整块复制、小尾重编码、从实际前层压缩存储解码后BOX生成缺层。新模块未被正式loader/生产包调用，flags/阈值/partial打包仍待原版适配。

本检查点通过已有集中入口显式触发 `public-d3dx-bc` 定向原版诊断（1536块、168完整原生DDS，各8固定编码策略和wrap/clamp完整聚合，不按样本选择策略）；它只解除未知参数/精度差异，不提供业务验收或NoOriginal声明。入口 `PublicD3dxBc/PATCHES.md`、`InspectTexturePublicBc.ps1`。正式启用前仍需修正一般规则并取得完整受影响D/R/NoOriginal及适用SDK。已验收loader源d0a3147/41006229811随普通合并保留，本轮主成果见恢复分支。

## 本轮候选检查点（2026-10-30）

`validation/texture-public-native-npot`：已接入固定 DirectXTex triangle 源码适配，原生 TGA24/32 与 raw A8/X8 DDS 的 NPOT/逐已提供层重采样及缺层 BOX 已实现；接入固定stb_image BMP24/32的七种BI_RGB DIB修订及原版offset/alpha规则。完整矩阵为6860构造、14606文件（含5665个NPOT和6152个BMP新增输入、105压缩未支持边界）、另13拒绝；保留旧矩阵与全部原产物字节比较。首轮34946af5/41005120850失败：BMP24原版按源DWORD扫描alpha（不是解码后alpha）及宿主x86累计trace内存耗尽。已按RVA143d2..14421修正源扫描/越界拒绝，确定性BMP24语料显式提供尾部扫描字节，全部形状/字段/断言保留；gold改流式完整写盘/逐条NoOriginal/EOF核对。尚未合入恢复主分支、未进入生产包；下一步补验失败组，未变成功组/SDK复用。

## 后续默认路线（2026-10-30 用户明确要求）

**所有贴图还原优先复用网上已有实现并适配原版行为，不能默认从零重写。** 先读[公开实现入口](TEXTURE_UPSTREAM.md)：DirectXTex/Wine优先用于D3DX编解码、加载/转换及过滤/mip，已有格式先复用仓库已接入源码。原样不匹配继续修改一般规则；只有具体未覆盖/不适合的部分才记录原因并补写。独立候选分支、来源/固定提交/通知、完整逐字节与实际消费/D/R/NoOriginal/适用SDK门槛保持。

本次仅更新执行文档，业务源码、测试、验收锚点和生产包未改，未下载新候选或启动编译/Actions/SDK/MOD。下方保存上一实现批次的已验证成果。

## 上一实现批次成果（2026-10-30）

- 新增独立x86 `TextureNativeLoaderApi`：52字节资源头、主/secondary VM及packed-tail所有权；raw DDS A8/X8部分/完整mip与缺层生成，POT TGA24/32 raw/RLE/metadata；接通LockRect→既有Copy→UnlockRect→引用/最终释放。
- 修复原LoadImageA实际format清除`0x1c0`后flags12/threshold0的消费契约；TGA全零alpha选择X8但保留基础alpha0；BOX退化轴使用原浮点存储/向零量化，不能统一整数平均。
- 压缩DDS仍未恢复：原版会重新编码已提供的2×2/1×1尾mip，基础轴小于4也会扩展/重压缩。全部样本保留，105个原版成功压缩资源另存完整取证；恢复版明确拒绝，不能称作原版拒绝匹配或压缩验收完成。
- 业务源码 `5fc678b050dcc5aff5a8a25365a406a09ca2f4c8`，集中验收[40999035840](https://github.com/wu162/NewSdk/actions/runs/40999035840)已completed/success，native-loader/base两组通过、62组复用，当前64组件收据有效。之前四次失败收据原样保留。矩阵为6860构造、2789文件（2684支持范围完整对照+105明确未支持压缩边界）及另5拒绝；Debug/Release及NoOriginal全部通过，每配置275197311字节完整trace相同；归档成员、原始SHA及12份x86 DLL指纹已核对，见[收据索引](evidence/texture/handoffs/20261030-native-loader/acceptance.json)。
- 新ABI未加入生产六DLL包，完整混合模式身份未恢复；SDK/tools及生产配置本轮未改。详细范围见[原生文件加载](TEXTURE_NATIVE_FILE_LOAD.md)。

## 下一轮计划

1. 下一实现轮先按公开实现路线选择原生NPOT/加载和D3DX重编码的候选，以DirectXTex/Wine为优先入口；复用已验收`TextureNpotRgba8Resampler`作为原版契约依据，适配权重/浮点存储、完整资源/mip与实际消费。注意X8的NPOT未使用字节不能由最终RGBA255反推。
2. 后续更多raster及压缩DDS同样先接入现成源码，再适配原生格式、存储精度和D3DX重编码/压缩转raw消费；不能直接用nvDDS近似替换，也不能把公开实现首次不匹配当作结束复用路线的理由。
3. 新ABI正式接入生产包前另取必要SDK集成证据；新能力稳定后集中验收，普通检查点不重跑未变旧组或SDK/MOD。

旧转换验收锚点：`6af9af189a5fb9be63bd02fbab4577d71c583257` / [40965216216](https://github.com/wu162/NewSdk/actions/runs/40965216216)，63组件旧声明范围成功。旧锚点不证明新loader。本轮选择native-loader/base，其他62组保留各自原sourceCommit/runId。业务/测试输入在验收后未改，后续仅更新收据与文档。具体按[执行规则](../AGENTS.md)、[测试规则](TEXTURE_TESTING.md)执行；不默认追加收尾plan或完整MOD。
