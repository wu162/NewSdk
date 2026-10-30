# PC 原生压缩 DDS：DirectXTex 候选续接

仅 PC；候选分支 `validation/texture-public-d3dx-bc`。正式分支仍不调用本候选，压缩原生加载仍明确拒绝；生产六 DLL 包与 SDK/MOD 未改。本页记录实现范围和定位入口，不能当验收收据。

## 本轮实现（2026-10-30）

- 固定 Microsoft DirectXTex `ec66b78b37389e8373a8e14ccc3cd29be94146b3`，继续适配 BC1/2/3。XG 与 D3DX 文件编解码共享公开核心，明确区分 A4 的 byte17/255 与 nibble/15、BC3 alpha 的不同存储/插值规则、文件编码预乘和解码去预乘。
- 独立 `TexturePublicBcCreateTextureFromFileA` 已接通实际 DDS 文件入口；完整提供块原样上传，partial 尾层解码后重编码；小 base 每轴至少4再上取2幂，NPOT 各已提供层先解码再用现有 DirectXTex triangle 的 float-IO 特化重采样，缺层从实际上一压缩存储解码、BOX、重编码。保留原生52字节 header、packed tail、主/secondary VM、锁定与释放。
- 文件上传默认 triangle+dither（`0x80004`，映射 RGB/A dithering）；缺层默认 BOX5 无抖动；BC1 文件阈值0.5。文件浮点编码允许过滤累积略高于1的中间值，按原编码器量化，不提前 clamp；仍拒绝非有限值并限定安全整数转换范围。XG 实际复制保留 flags0x12、端序、swizzle、rect/pitch/guard，flag2 控制去预乘。
- 颜色/alpha 的初始量化、端点、bitmap/error diffusion 与 Newton 求解阶段均已接入原舍入区间适配；尚有差异，不能把已写代码等同于正确还原。
- 新的 `TexturePublicBcDecodeResizeDdsImage` 就是 loader 的实际浮点消费 helper。最初定向检查用原 `D3DXCreateTextureFromFileExA@13b53` 请求 R32G32B32A32F，但协商输出并非已证实的固定float，现已换成显式CPU float资源与surface-memory入口，计划完整比360份解码/重采样 float，区分过滤与编码的差异；不只比较最终8位视图。
- 已写严格独立 NoOriginal 回放与 `native-bc-files` 手动正式组件入口，涵盖完整原 float/block、1572默认文件、360 float 阶段、VM/实际RGBA/所有权、EOF和9拒绝边界。**尚未通过/尚未启用**；不能在默认文件或 float 阶段仍有差异时把它标为成功。

## 已知证据与状态

- `ce16e990` / `41010488699`：XG D/R 各1536完整解码块0位模式差异、flags0全部编码块相同；完整DDS旧168例仍86差异。probe-only。
- `2b79fee9` / `41011190751`：文件编解码分流后，文件1536解码/无抖动编码全部匹配；抖动剩6灰度块，完整DDS flags0差异27/168。
- `2904039f` / `41011903071`：6灰度块已消除，但扩展检查在5x3输入因浮点范围检查停止，不能当通过。原107556655字节 partial capture 已从 Actions artifact12958057711无损压缩保存为 `failure-captures.zip`，原run failure保留；修复失败时 finally 归档，避免超100MiB单文件不能普通推送。
- `bb8032e9` / `41012948892`：1572默认文件完整跑完，1226相同/346差异；文件1536解码、无抖动/抖动编码全部相同。每配置266136903字节 capture相同，原重复稳定。
- `d521ec3e` / `41013757092` 及 `824da4e8` / `41014720897`：补 Newton RC_CHOP 后默认文件1249/1572相同，323差异；文件解码仍全部匹配，但BC3编码有31记录差异（现有随机序列重复一个输入模式，入口 `bc-file-block-f20-23`，完整alpha `ff0d8945ca7fb1a1` 对 `ff028945aa7fb181`；不能按标签特判）。两run D/R完整capture均 `2249028a4f0fa6083c44441060535afd6b79ee5fa0fd9e688a97e660f2f22c06`；显式扩展中间算术补丁没有改变该capture，不能宣称它修复了差异。
- 新 float 阶段业务检查点 `e4246eac`，运行源 `9f61058d05ec505d66ddd31e895cd1cf40f26d17` / `41015245262`；该run为failure：在9x7输入停于`Original float pitch`，不能将格式协商后的缓冲区当float。partial capture114024567字节已由finally无损归档；其中float记录不是有效float gold。最新 `a0fb7d25` 改为原CPU float构造、surface view和实际surface-memory入口1210e，明确验证格式38与尺寸；该源码等待最后一次定位运行。

上述运行只显式选 `public-d3dx-bc`，复用64未变成功组件，无 SDK/MOD；全部为定位探针，不能冒充完整组件验收。正式入口为同一集中工作流显式 `force_groups=native-bc-files`，只在相应固定业务输出完整相同时使用。

## 下一优先入口

先读最新 float 阶段结果与完整 capture。若 float 本身不同，按上游 triangle/解码的真实存储顺序适配；若 float 一致，集中修文件编码的量化/求解/平局规则。保留原回归及所有完整字节，不做样本ID或gold查表分派；不因一次候选失配放弃公开实现。当前还未取得最终 D/R + NoOriginal，不合入/启用候选；SDK只在后续正式 ABI 接入里程碑运行。

源码/来源：`src/TextureCompiler/PublicD3dxBc/`；诊断：`recovery/scripts/InspectTexturePublicBc.ps1`、`recovery/tests/TexturePublicBcDiagnostic.cpp`；严格回放：`TexturePublicBcReplay.h` / `TestTexturePublicBc.ps1`。静态原版参考：Newton alpha2851b..2885a、RGB2885a..28e46；文件解码29895/2a4f4/2a507，编码29cee/2a51a/2a532；全局b8914初值1，预乘4c901、去预乘28385。最后结果只绑定各自sourceCommit，不能继承给新源码。

最后定向运行：[41016298705](https://github.com/wu162/NewSdk/actions/runs/41016298705)，不可变源 `a0fb7d2557927cb99882b2f384962cc9ecd580ba`。本轮交接时在后台运行，未取得终态，不将其计为成功。
