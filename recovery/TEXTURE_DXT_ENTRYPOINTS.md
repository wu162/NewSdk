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

## 第五轮末段追加：64 个原版块样本

源码 23eaca15a66c665e111fe8e9ec486775e2eb709f，运行 [40237236370](https://github.com/wu162/NewSdk/actions/runs/40237236370) success/attempt1。仅选择 dxt-blocks，20 个组件组全部复用。使用原 2D native 入口、GetNVCompressionOptions 与 null rect，PC/PS3 × 四格式 × 八组 4x4 无 mip 输入，共64份完整DDS、1644字节options与源RGBA，全部借用现有原生harness契约；未执行恢复版压缩器。

证据位于 evidence/texture/dxt-blocks/40237236370-1：validation.json、transcript.tsv、samples.zip；压缩包SHA256 4d7067c4791cfe5c22325d974eee71ff2c2302af5b0c7c40ca9e8d17c947fdad。DXT1/DXT1A为136字节DDS，DXT3/DXT5为144字节；回调分别为4+124+8/16，payload描述为0,4,4,0,1。64例输入调用前后hash均相同。

**待解释的原版差异**：32个同输入/格式的PC与PS3配对中，31对完整DDS相同；D3DFMT_DXT1、pattern5 的8色不透明棋盘不同，源RGBA相同。PC SHA256为745bbfb70f3f7f73f20ea10e59d5ed2e9a2c753a48bfb68934fca8c6fb4c710a，PS3为3c550f55d4e16973bfacb60b5368fc8f9f45baa8780cdf08727111044bf70d7a。尚未将差异归因于平台、options或内部状态；不能擅自视为允许像素近似。

下一轮先直接解包已有 samples.zip 比较该对 options、DDS头与8字节块，无须重跑本批。若现有数据不足以解释，再用同输入同options的重复调用/独立进程定向查明；不要先泛化“两个平台完全同码”或“压缩器非确定性”。随后再用已有64例作为实现起点；它们仍只是原版资料，不是恢复版差分通过。
