# TextureCompiler 第五轮：PNG/BMP 与集中验证

> **历史记录：自 2026-10-26 起仅以 Win32/PC 为目标。本文非 PC 能力、旧待办和旧平台矩阵不再适用；当前实现与计划以 [TEXTURECOMPILER.md](TEXTURECOMPILER.md) 为准。原 sourceCommit/runId 仅证明原验收版本，不代表本轮删除后的代码已验证。**

2026-10-26（北京时间）。分支 recovery/texturecompiler-source；本轮按用户要求单代理推进，不启动子代理，禁止强推。当前组件及包含 RGB16 的最终四 profile SDK 验收均已完成。生产 SDK 的 DLL/config 未替换，源码未合入其他分支。

## 成功解码与原版拒绝范围

新增代码为根 src/TextureCompiler/Managed/TexturePngDecoder.cs、TextureBmpDecoder.cs；通过 TextureImageLoader 和真正 Plugin.ProcessInstance 接入。只使用恢复代码及 .NET 自带 DeflateStream，不调用原 TextureCompiler 或 System.Drawing 解码业务输入；System.Drawing 仅用来生成独立的测试 PNG。

| 格式 | 已恢复行为 |
| --- | --- |
| PNG 灰度 | 1/2/4/8/16 位；低位深展开、16 位量化；灰度 tRNS 的像素与 hasAlpha 分开遵循原版 |
| PNG 真彩 | RGB 8/16 位，保留旧 D3DX 的 RGB16 异常存储布局及 tRNS 填充值 |
| PNG 带 alpha | 灰度 alpha 8 位、RGBA 8/16 位；灰度 alpha 8 位 hasAlpha=false 但保留像素 alpha，RGBA hasAlpha=true |
| PNG 基础处理 | 五种行滤波、分片/空 IDAT、stored 与正常 Deflate、多块 zlib、Adam7、CRC/Adler、gamma、已测 sBIT/sRGB/text/pHYs |
| 原版 PNG 拒绝 | 调色板各位深、灰度 alpha 16 位、灰度 16 位带 tRNS：保持 BadTextureFile 及原消息 |
| BMP 成功输入 | BITMAPINFOHEADER 40 字节、BI_RGB 16/24/32 位，正/负高度、行填充及文件 gap |
| 原版 BMP 拒绝 | 索引 1/4/8 位，以及本轮 16/32 位 BI_BITFIELDS 三组掩码矩阵：保持 BadTextureFile，不能称为成功支持位掩码解码 |

成功输入仅包含 POT 尺寸。ICC/APNG、JPEG、NPOT D3DX 重采样、其他 BMP DIB/压缩/掩码及未声明的组合不在本轮完成范围。PNG/BMP 编译继续只支持 A8R8G8B8、X8R8G8B8、R5G6B5 三种 raw 输出；DXT 压缩仍缺失。既有 TGA 与 DDS 路由保留。

## 关键原版语义

- RGB16：原版 D3DX 返回 X8R8G8B8 descriptor，却将 little-endian B16/G16/R16/A16 行按 width*4 字节写入。输出只使用每行前半源像素并拆成两像素；恢复版有意保留此行为。没有 tRNS 时填充字为 FF00；有 tRNS 时按透明键使用 0000/FFFF，影响奇数输出像素的低字节。原生 descriptor/LockRect 取证运行 40232650159，12 张安全输入，收据和 probe.log 位于 evidence/texture/png-rgb16/40232650159-1。
- Gamma：先把文件 gamma 与屏幕 2.2 分别量化为 float，再提升为 double 做乘积、指数与 pow；全部用 double 会在极少的 16 位边界差 1。该可复现模型经 14 个 gamma 值及完整 0..65535 样本范围验证，包含阈值两侧。没有用放宽误差或近似像素比较验收。
- BMP：16 位颜色按比例最近量化；24 位原 hasAlpha=true；32 位扫描 alpha，全部为零时补 255 且 hasAlpha=false。BI_RGB 原版从 DIB 后立即取像素，忽略 bfOffBits gap；按原行为保留。
- 原 LoadImageA 探针必须先 Initialize，最后 Dispose，并先通过 TGA 正对照。首两轮无 Initialize 的运行 40231257963、40231508998 属于无效宿主调查，不能作为 PNG 格式拒绝证据。

## 已取得的组件验收

| 分组 | sourceCommit | runId | Debug / Release 各自结果 |
| --- | --- | --- | --- |
| PNG 核心 | 14b22c15ba0b45351b55089681c72b913bc1e521 | 40234341155 | 5882 例：3898 像素匹配、1984 原版拒绝匹配、0 未支持、0 差异 |
| PNG 完整采样 | 同上 | 40234341155 | 98 组、3684352 像素，differentCases=0 |
| BMP 核心及 bitfields 拒绝 | e20a40cc175e3b15029b106b07f78d9e21990a2f | 40235000007 | 4320 例：1080 像素匹配、3240 原版拒绝匹配、0 差异 |
| 最终实际插件入口 | 同上 | 40235000007 | 9216 例：4416 原版错误边界、0 未支持；19891368 bytes 三 buffer/full cdata 精确比较 |
| cubic 当前依赖基线补齐 | 274b29061d340295e274a74e416159f085f335d2 | 40233890070 | cubic 分组自身通过；所在 batch 因当时 PNG 差异失败，不能把整个运行称为成功 |

入口矩阵覆盖 PC/PS3、Texture/OnDemandTexture、Standard/Volume、无 mip/有 mip、三种 raw 输出和已存在 cdata 尾部。错误边界计数是断言通过的原版错误输入，不能删去或混成像素成功数。当前 PNG/BMP 和入口最终运行均 completed/success，attempt1；cubic 单组结果保留其 batch 失败状态。

旧 TGA 入口、mip/volume/PS3 resize 的受影响回归在 f6276dda 的 40232336956/40232336933/40232336846/40232336829/40232336926/40232337041 完成；未改 helper 仍精确沿用第四轮或对应旧验收。不要将沿用结果说成当前提交新测。

## 真实 SDK 状态

初始不含 RGB16 的专用 MOD 已在源码 8a70300bf84ae60a5bb6ef00e6322c70fd27d91a、运行 40232650147 四个 profile 全部成功。PcNoMip/PcMips/Ps3NoMip/Ps3Mips 各 Original/Recovered/NoOriginal 三组、每组 552 资产及 552 次实际 Core 调用，四产物和实际三个 buffer/full cdata 完全一致。

最终 SDK 验收源码 9ba02841cf320083c6ec3c571744f3c4c36e401b，运行 [40235430314](https://github.com/wu162/NewSdk/actions/runs/40235430314) completed/success，attempt1。PcNoMip/PcMips/Ps3NoMip/Ps3Mips 四 profile 均通过；每 profile 是 50 张成功输入 × Standard/Volume × Texture/OnDemandTexture × 三 raw 格式，共 600 资产。每组 Original/Recovered/NoOriginal 均为 600 次实际 Core 成功调用，合计 7200 次；每 profile 四产物及全部实际三个 buffer/full cdata 一致。bitfields 原版拒绝样本只留在组件/入口矩阵，未加入成功 MOD。

精简验收收据与 19 个 Managed 输入指纹见 [acceptance.json](evidence/texture/handoffs/20261026-stage5/acceptance.json)；原始收据为 evidence/texture/sdk-rasters/{profile}/40235430314-1/validation.json，完整实际调用位于同目录 traces.zip。19 个 SDK 构建输入跨四 profile 一致，最终文档/取证提交与该 SDK 源码逐 Git blob 相同。SDK 整个子树仍为 1f2d71a20ce8bf7d9343c7a2246c10275abe28f9，与本轮起始树一致，证明生产 SDK 未被修改。

SDK 使用 EALAModStudio case5 的 BAB 基础数据流程，仅在临时 SDK/MOD 副本改路由；NoOriginal 移除原目标 DLL。完整调用轨迹压缩为 traces.zip 保存，减少数千个小文件；比较在压缩前逐文件完成，收据保留压缩包 SHA256。

## 触发规则与下一轮

用户反馈“每次触发的测试个数太多了”后已改为 [TEXTURE_TESTING.md](TEXTURE_TESTING.md)。普通检查点不启动 Windows；完整组提交加 [texture-test]，SDK 里程碑加 [texture-sdk]。仅一个自动入口、至多一个 Windows 作业；依赖选择器核对完整成功祖先证据。选择器最新 13 项边界检查通过（40236386719）；40233600075 为早期计划验证，未启动 Windows。PNG 新修改只运行 png/raster-images/png-ranges，BMP 更新只运行 bmp/raster-images，其余成功组保留原锚点。

下一轮先读本页及 TEXTURECOMPILER.md。DXT 原生入口与旧参考库 223 行相关 COFF 符号已采集，见 [TEXTURE_DXT_ENTRYPOINTS.md](TEXTURE_DXT_ENTRYPOINTS.md)，仅属取证，不是 DXT 恢复完成。继续 DXT1/DXT1A/DXT3/DXT5 核心、JPEG/NPOT、剩余 TGA metadata、完整 atlas 入口或 PC 实际需要的 native 生命周期。后续 PS3/Xbox360 的实现与验收全部排除。公开参考为 W3C PNG 规范、Microsoft BITMAPINFOHEADER；旧压缩器复用核查见 TEXTURE_UPSTREAM.md。原版差分始终是行为基准。

本轮未量化账号额度节省；实际减少的是未变成功组重跑、多个 Windows 安装环境和 SDK 中间批次。运行环境末段变为不可用后，通过 GitHub 接口继续收取 Actions 和保存文档；远程源码与证据已持续提交。

本轮末段运行环境失效后仍通过 GitHub/Actions 完成 SDK 收取与旧库符号采集。最终 SDK 和取证阶段每次仅一个 Windows 作业，取证运行明确只选 dxt-symbols，20 个已成功组件组全部复用。无源代码/行为变更后不再启动 Windows 重测。

第五轮最后追加的原版 DXT 块取证 40237236370 成功：64例、输入未改写、只选 dxt-blocks、20个组件组复用。完整DDS/options/RGBA已留存；一对DXT1棋盘PC/PS3输出差异仅作历史观察，不再调查，详见 DXT 入口文档。未宣称 DXT 已恢复，不为此重跑 PNG/BMP/SDK。

