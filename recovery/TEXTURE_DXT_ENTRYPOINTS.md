# DXT 下一轮原生入口与参考库符号

2026-10-26，北京时间。此页为下一轮定向依据；DXT1/DXT1A/DXT3/DXT5 仍未恢复。不存在新压缩后端、原 DLL 回退或引入旧静态库的业务依赖。

## 不可变原版入口

原 DLL SHA256：4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e。以下均为 RVA，不是固定进程地址；实际测试按模块基址相加，先核对 SHA、x86 和 metadata token。

| 方法 | token | RVA | 现有依据 |
| --- | --- | --- | --- |
| nvDDS::nvDXTcompress，RGBAImage | 0x060000DC | 0x52B60 | tests/TextureRawDdsDifferential.cs 的已验证 Cdecl harness |
| nvDDS::nvDXTcompress，Volume | 0x060000DB | 0x52950 | tests/TextureVolumeDdsDifferential.cs 的已验证 Volume harness |
| Plugin.GetNVCompressionOptions | 0x060000FB | 0x25F0 | 原 managed IL；不要自行猜选项 |
| Plugin.CompressAndEmbed | 0x06000101 | 0x5604 | 原 managed IL |
| Plugin.CompressAndEmbed3D | 0x06000102 | 0x56B8 | 原 managed IL |

1644 字节 options、RGBAImage 六个 uint 字段和 writer 回调参数可直接沿用现有 oracle；保留 writer delegate 与源 RGBA 生命周期、callback chunk/MIPMapData 和完整 DDS。原 LoadImageA 的设备初始化要求见 PNG/BMP 交接，不能用未初始化的失败推断格式不支持。

## 已取得的参考库资料

固定仓库/提交：Stehfyn/Legacy_DDS_Utilities @ f0467589621354150acf2c623cb5cad44c8f35c1。
文件：LIBS/lib/release/nvDXTlibMT.vc8.lib，1376370 bytes。
Git blob：48d8c74d9ca04798246242c4be3bf459ddcbbdfc。
SHA256：572aa5d1d3ff26050bdc101fddacf0a4f5af360bb5e2bbe1b6bc95a615d2524b。

取证源码 74e4ba8f0ddd45c6556ac0927ce1e5e2a80fdd01，运行 [40236386719](https://github.com/wu162/NewSdk/actions/runs/40236386719) success/attempt1。脚本 InspectTextureDxtLibrary.ps1 验证原 DLL hash 与参考库 Git blob 后，使用 MSVC dumpbin 14.44.35229.0 导出 linker members/object symbols；未运行压缩器、未链接参考库进业务 DLL。

证据位于 evidence/texture/dxt-library/40236386719-1：
- validation.json：probeOnly/evidenceOnly，状态 probe-only，SDK=false。
- selected-symbols.txt：223 行相关符号。
- coff-symbols.zip：完整两份 dump；SHA256 69fa91bb5fe40a7c9553c60fe85682e790c3e31adb933d2aae325094e3f0fbd5。

已实际确认参考库中存在：
- nvDXTCompression::EncodeDXT1 / EncodeDXT1a / EncodeDXT3 / EncodeDXT5；
- CodeRGBBlock、CodeRGBBlockQuick、CodeRGBBlockExhaustive、AllSame、ExhaustiveAttempt、Search43Mult、MapAndRoundMult、RoundMult、GetAxis、GetDiameter、RmsError；
- CodeAlphaBlock、CodeAlphaBlockQuick、QuantizeAlpha；
- nvFastDXT 的 FastEncodeDXT1AllPixelsSame / ForceAlphaOne / DXT1_3 / DXT1_4 / DXT5；
- JAMA::Eigenvalue&lt;double&gt; 与 TNT::Array2D&lt;double&gt; 符号。

这些名称可缩小对象/函数匹配范围；仅凭符号存在，不能断言原 DLL 同版本、Production=71 必经哪个分支，或现代 NVTT 输出相同。参考库不是压缩核心源码。公开实现定位与版本判断仍见 TEXTURE_UPSTREAM.md。

## 建议下一批

先从 RGBAImage 的原生 harness 建立一个完整 4x4 无 mip 批次：四格式、固定单色/双色/渐变、alpha 0/255/交替/阈值附近。选项必须调用原 GetNVCompressionOptions；保存选项字节、callback 描述、DDS 头及完整 8/16 字节块，核对源数组是否被改写。该批次用新的显式取证入口，不能修改已验收 raw 编码来假装 DXT 成功。

随后将参考库中 CodeRGBBlock/CodeAlphaBlock 等对象与原 DLL 原生函数匹配，先证明版本/路径，再还原量化、端点搜索与 alpha 分支。完整恢复后统一跑颜色/alpha/尺寸/mip/PC/PS3/volume 差分，再集中做 SDK；不要按每个 endpoint 变化反复触发全套 Windows 工作流。保留现有 raw 拒绝 DXT 的断言，直到对应路由有真正实现与验收。

Xbox360/Xenon 专属路径不在本任务内。此次精确函数名搜索未找到可直接采用的核心源码；没有把库符号导出当作源码还原。
