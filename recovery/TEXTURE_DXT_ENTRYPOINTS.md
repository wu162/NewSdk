# DXT 下一轮原生入口与参考库符号

> **历史记录：自 2026-10-26 起仅以 Win32/PC 为目标。本文非 PC 能力、旧待办和旧平台矩阵不再适用；当前实现与计划以 [TEXTURECOMPILER.md](TEXTURECOMPILER.md) 为准。原 sourceCommit/runId 仅证明原验收版本，不代表本轮删除后的代码已验证。**

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

随后将参考库中 CodeRGBBlock/CodeAlphaBlock 等对象与原 DLL 原生函数匹配，先证明版本/路径，再还原量化、端点搜索与 alpha 分支。完整恢复后统一跑颜色/alpha/尺寸/mip/Win32/volume 差分，再集中做 SDK；不要按每个 endpoint 变化反复触发全套 Windows 工作流。保留现有 raw 拒绝 DXT 的断言，直到对应路由有真正实现与验收。

Xbox360/Xenon 专属路径不在本任务内。此次精确函数名搜索未找到可直接采用的核心源码；没有把库符号导出当作源码还原。

## 第五轮末段追加：64 个原版块样本

源码 23eaca15a66c665e111fe8e9ec486775e2eb709f，运行 [40237236370](https://github.com/wu162/NewSdk/actions/runs/40237236370) success/attempt1。仅选择 dxt-blocks，20 个组件组全部复用。使用原 2D native 入口、GetNVCompressionOptions 与 null rect，PC/PS3 × 四格式 × 八组 4x4 无 mip 输入，共64份完整DDS、1644字节options与源RGBA，全部借用现有原生harness契约；未执行恢复版压缩器。

证据位于 evidence/texture/dxt-blocks/40237236370-1：validation.json、transcript.tsv、samples.zip；压缩包SHA256 4d7067c4791cfe5c22325d974eee71ff2c2302af5b0c7c40ca9e8d17c947fdad。DXT1/DXT1A为136字节DDS，DXT3/DXT5为144字节；回调分别为4+124+8/16，payload描述为0,4,4,0,1。64例输入调用前后hash均相同。

**历史跨平台观察（不再调查、不构成 PC 完成阻塞）**：32个同输入/格式的PC与PS3配对中，31对完整DDS相同；D3DFMT_DXT1、pattern5 的8色不透明棋盘不同，源RGBA相同。PC SHA256为745bbfb70f3f7f73f20ea10e59d5ed2e9a2c753a48bfb68934fca8c6fb4c710a，PS3为3c550f55d4e16973bfacb60b5368fc8f9f45baa8780cdf08727111044bf70d7a。尚未将差异归因于平台、options或内部状态；不能擅自视为允许像素近似。

后续仅还原 Win32 压缩器。优先使用已有 PC 样本及其 options、DDS 头和压缩块；若需新取证，仅运行 PC。上述跨平台差异作为历史观察保留，不再是完成阻塞或验证任务。64 例仍只是原版资料，不是恢复版差分通过。

## 2026-10-27：PC DXT3 显式 alpha 子组件

`src/TextureCompiler/Managed/TextureDxt3AlphaEncoder.cs` 独立实现 4×4 RGBA → 8 字节显式 alpha：每通道 `(a * 15 + 127) / 255`，按行、低 nibble 在前打包。严格拒绝 null 与非 64 字节输入，不改写源数组。未实现颜色块、DXT1/1A/5，也未接入 Plugin 的压缩路由；现有 raw 对 DXT 的拒绝保持不变。

验收源码 `9f8b1361148a6b76fec1b5564b3d055ba4ee62e7`，集中 Actions [40470055551](https://github.com/wu162/NewSdk/actions/runs/40470055551) attempt1 success。Debug/Release 各 4,352 例、65,288 检查：全部 256 个均匀 alpha，以及每个像素位置的全部 256 值与混合相邻 alpha；非均匀 RGB。每例调用原 GetNVCompressionOptions 与 Win32 原生 DXT3 压缩，对完整 DDS 的前 8 字节 payload 逐字节比较，并检查输入未修改。8 字节颜色块只保留完整 DDS hash，不作为已恢复内容。

收据 `evidence/texture/dxt3-alpha/40470055551-1/validation.json` 与压缩转录。选择器 13 项检查通过，只运行 dxt3-alpha，16 个原成功组沿用各自 sourceCommit/runId；未执行 SDK。此证据仅证明 alpha 子组件，不升级为完整 DXT3/DXT 压缩器验收。

PC 清理后取证 harness 已只生成四格式×八图案=32例；修正 `InspectTextureDxtBlocks.ps1` 遗留的 64 例期望。历史 64 例收据不变。

新增 `MatchTextureDxtCode.py`，由显式 dxt-symbols 入口在 Actions 执行。核对固定原 DLL/参考库 SHA256，解析 x86 COFF archive 的目标函数，屏蔽 DIR32/REL32 操作数字节，以至少16字节无重定位锚点定位，再比较剩余全部代码字节；输出符号、COFF成员偏移、长度、mask 后比较字节数及候选原 DLL RVA。短锚点不匹配，未知重定位明确报错。匹配不能证明引用目标、实际可达路径、库版本或压缩输出等价；零匹配也不证明函数不存在。

### 参考库与原 DLL 映射实测

源码 `70aded868821631a4bfc1e633b717d03da0c3529` / [40470267556](https://github.com/wu162/NewSdk/actions/runs/40470267556) attempt1 success，仅选择 dxt-symbols、dxt-blocks，17 个组件组复用。PC 32例/132检查成功；参考库22个目标代码节有19个 relocation-masked 匹配。完整结果在 `evidence/texture/dxt-library/40470267556-1/code-matches.json`，每节记录实际比较长度。COFF 节可大于单个函数（例如 RoundMult 所在节106380字节），不要把节长当作函数体长度。

| 候选符号 | 原 DLL RVA |
| --- | --- |
| EncodeDXT1 / EncodeDXT1a / EncodeDXT3 / EncodeDXT5 | 0x57D30 / 0x57E10 / 0x57F00 / 0x57FF0 |
| Search43Mult / RoundMult | 0x603D0 / 0x61460 |
| AllSame / MapAndRoundMult | 0x7B3F0 / 0x7BB80 |
| CodeRGBBlockQuick | 0x7BDF0 |
| CodeRGBBlock（带 int 参数） | 0x7C420 |
| QuantizeAlpha / CodeAlphaBlock | 0x7D500 / 0x7E8F0 |
| CodeRGBBlockExhaustive / CodeRGBBlock（无 int 参数） | 0x833F0 / 0x83CD0 |
| FastEncodeDXT1AllPixelsSame / FastEncodeDXT5 | 0x8B380 / 0x8B570 |
| FastEncodeDXT1_3 / FastEncodeDXT1_4 | 0x8BAA0 / 0x8C1F0 |

CodeAlphaBlockQuick（14字节，未达到16字节锚点门槛）、FastEncodeDXT1ForceAlphaOne、RmsError 未匹配；不能视为原 DLL 中不存在。匹配尚未核对重定位的目标地址与调用可达性，不断言整个库版本相同。

下一轮可直接从上述 RVA 定向反汇编，先核对 EncodeDXT3/5 到 alpha block 的实际调用链与对象布局，再还原 QuantizeAlpha/CodeAlphaBlock 的端点搜索、误差度量和索引打包。颜色端点搜索沿 CodeRGBBlock/AllSame 路径推进。保留完整 DXT 拒绝边界；新子组件接入插件之前必须补颜色、alpha、尺寸、mip、volume 与真实 SDK 验收，不以19个静态节匹配替代行为差分。

## 2026-10-27 第二阶段检查点（验证中）

新增 `TextureDxt5FastAlphaEncoder`，仅原 nvFastDXT 质量值68的alpha路径：端点float量化、比值显式float落地及索引映射。未接插件，不代替默认Production=71的QuantizeAlpha搜索。低层原生差分源码 `ef2d69f5fd14561ab960f3d78fd7607da5abd3a6` / Actions `40488693788` success：Debug/Release 各71,168例、1,921,543检查，覆盖全部65,536对alpha值、逐位置256值、渐变与随机块。后加实际原DDS入口4352例差分仍在验证；不能用本次低层验收替代新入口结果。

定向原版反编译入口 `dxt-decompile` 使用官方Ghidra11.4.2，SHA256 `795a02076af16257bd6f3f4736c4fc152ce9ff1f95df35cd47e2adc086e037a6`。首轮40488564968因临时项目目录未建立失败，原日志保留；修复后重跑中。仅取证，不加载进业务DLL。

原DLL实际调用依据：RVA 0x5C50C / 0x8A77F 调用 CodeAlphaBlock@0x7E8F0；其入口检查 block+0x818==68 时调用 FastEncodeDXT5@0x8B570，其他质量在0x7EBB1调用QuantizeAlpha@0x7D500。block布局：count@0、float alpha[512]@4、保留zero/one@0x804/0x808、levels@0x80c、endpoints@0x810/0x814、indices@0x81c。包装层随后按levels与端点顺序做规范化与索引重映射，不能把低层输出布局未经核对直接当作DDS。


## 第二阶段结果入口

Production DXT5 alpha、独立 x87 后端、精确量化器反例、quality68 未初始化 levels 边界和颜色取证部分结果见 [TEXTURE_DXT_ALPHA.md](TEXTURE_DXT_ALPHA.md)。上面的“验证中”章节为历史检查点，不是最新验收声明。
