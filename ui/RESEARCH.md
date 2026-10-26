> 后续更新：已直接导入并采用 `ui/vendor/swf2apt`，固定在 `2b3cb662b7311616e462b884e5611750e5de88f4`。390个源码文件校验及原始程序集/源码版样例输出对照全部通过，见 `ui/RESUME.md`、`ui/VENDOR.md` 与 `ui/evidence/source-consistency.json`。以下保留第一轮调查历史，其“未验证/尚未导入”描述仅代表当时状态。

# SWF 直接进入 MOD 编译链

## 结论和本轮边界

可以复用现有 SWF→APT 转换源码，把写文件的边界改为内存结果，再接入资产收集/编译阶段。游戏最终仍需要 APT 格式；目标是不落地 `.apt/.const/.dat/.ru`、纹理和生成 XML 中间文件，而不是删掉 APT 格式。

用户要求本轮先搭框架，确认附件 SWF 能由原工具成功编译即可收尾。因此本轮只建立原工具源码基线、受限内存转换原型和验证宿主；没有修改生产 BAB 路由，也不宣称已经支持完整 SWF。

## 附件与来源

`CoronaUIMain.swf`：6389 字节，SHA256 `b661542cf2c8b52ef846c348f817f523f5fba63ee4baa53c365aabde30af9fa7`。CWS/zlib、SWF 7，解压总长17830，30 FPS，根时间轴1帧。四个空 Sprite ID=1/2/3/4，均0帧。完整 tag 统计（含嵌套 End）：End×5、ShowFrame×1、SetBackgroundColor×1、DoAction×1、DefineSprite×4、ExportAssets×4、DoInitAction×4；无位图、字体、形状、声音和视频。原样例保存在 `fixtures/`。

`ARSDK2_flash.7z` SHA256 `105d4739804ac479928397663fae35c7ad6609eb7f1bc015dbca3a2d5277c885`。包内 `defaultscript.cs` SHA256 `57d0addb2c0c088432feca3cafaa0a541e8da2404320f8643c410f38d75d56df`；`eaf2apt.exe` SHA256 `08cb72bf389e2ae5220084e9d712ece9da02267856ff7dff8e68914206c29197`。本轮没有把约14MB的自包含 exe 或整套运行时提交进 Git。

源码来源：[`lanyizi/swf2apt`](https://github.com/lanyizi/swf2apt/tree/60b15a67a4cbd931f181af8c6c758bd7a8441ebc)，源自 [`jonwil/swf2apt`](https://github.com/jonwil/swf2apt)。附件 PDB/EXE 出现 Lanyi 本地 ARSDK2 工程路径及 eaf2apt/SwfLib 名称，支持将此 fork 作为候选来源；**没有证明此提交与上传 exe 二进制一致**。固定 fork 使用 net6.0，上传 exe 内含 net5.0 构建路径。本轮宿主采用 net8.0-windows 编译原源码算法，这是源码版基线，不能冒称上传二进制已运行。

## 原脚本的实际链条

包内 `defaultscript.cs` 第863–864、1129–1150行：

1. 调用 `ARModExtraTools/swf2apt/compiled/eaf2apt.exe <swf> --ra3 --optimize`。
2. 对子目录 SWF 生成外层 AssetDeclaration XML，Include 转换器生成的 XML。
3. BAB 逐个编译 `aptui/*.xml`，输出 manifest 等 MOD 资产流；最后生成 version 标记。

`aptXmlGenerator.cmd` 只生成 Include 包装，不负责编译 SWF。脚本没有严格串联所有命令的成功状态，不能用流程走到底或存在旧产物证明本次转换成功。本轮使用全新临时目录、严格退出/异常检查及输出指纹。

## 源码中真正需要拆开的边界

以下路径均固定在 `lanyizi/swf2apt@60b15a67a4cbd931f181af8c6c758bd7a8441ebc`：

| 来源 | 当前行为 | 内存化方向 |
|---|---|---|
| [AptConverter.cs](https://github.com/lanyizi/swf2apt/blob/60b15a67a4cbd931f181af8c6c758bd7a8441ebc/eaf2apt/AptConverter.cs) `ConvertToApt` | hash缓存、删建输出目录、flasm循环优化、序列化APT/CONST、DAT/XML输出 | 输入字节+明确选项，输出 bundle；缓存由上层按内容与编译选项控制 |
| [AptOutput.cs](https://github.com/lanyizi/swf2apt/blob/60b15a67a4cbd931f181af8c6c758bd7a8441ebc/eaf2apt/AptOutput.cs) `GetBuffer` | 内部原本就是 MemoryStream，最后 ToArray | APT/CONST 字节可直接返回，无需重新实现序列化算法 |
| [AptCharacterShape.cs](https://github.com/lanyizi/swf2apt/blob/60b15a67a4cbd931f181af8c6c758bd7a8441ebc/eaf2apt/Characters/AptCharacterShape.cs) | triangle 子进程读写 poly/node/ele/neigh，最终 StreamWriter 写RU | 三角化改库调用，几何结果进入内存 writer |
| [AptCharacterBitmap.cs](https://github.com/lanyizi/swf2apt/blob/60b15a67a4cbd931f181af8c6c758bd7a8441ebc/eaf2apt/Characters/AptCharacterBitmap.cs) 和 PackTextures | 构造期间即写TGA/JPEG；后续从路径读写、缩放 | 像素/编码字节常驻 bundle，纹理编译器增加内存输入 |
| [flasm.cpp](https://github.com/lanyizi/swf2apt/blob/60b15a67a4cbd931f181af8c6c758bd7a8441ebc/flasm/flasm.cpp) 与 RunFlasm | 外部工具、eaf及result文件、迭代至返回值稳定 | 暴露内存优化API；保留RA3模式、迭代与失败语义，不把关闭优化冒充等价 |

本轮生成的 `MemoryAptConverter` 只接受 SWF7、上述七种 tag、空 Sprite；不执行 flasm。保留原序列化主体，仅替换输入和最终输出边界。验证比较相同的 `optimize=false` 源码基线，另行运行原工具 `--ra3 --optimize` 验证样例可编译。两者没有被宣称语义或字节等价。上游对部分不支持tag会忽略，因此能力检查必须在调用转换器之前执行。

## 接入现有编译器

以 NewSdk 起点 `87cf54e254dfb44a129da087316fd04806ed2b10` 的源码为依据：

- [IAssetBuilderPlugin.ProcessInstance](https://github.com/wu162/NewSdk/blob/87cf54e254dfb44a129da087316fd04806ed2b10/recovery/src/Managed/Core/BinaryAssetBuilder.Core/IAssetBuilderPlugin.cs) 返回单个 AssetBuffer；[AssetBuffer](https://github.com/wu162/NewSdk/blob/87cf54e254dfb44a129da087316fd04806ed2b10/recovery/src/Managed/Core/BinaryAssetBuilder.Core/AssetBuffer.cs) 为 InstanceData/RelocationData/ImportsData。一个新 Swf 插件不能凭此接口直接返回一组独立资产。
- [AssetDeclarationDocument](https://github.com/wu162/NewSdk/blob/87cf54e254dfb44a129da087316fd04806ed2b10/recovery/src/Managed/Core/BinaryAssetBuilder.Core/AssetDeclarationDocument.cs) 的 CompileInstance 把单个结果赋给 asset.Buffer。适合在 LoadXml 建立DOM后、GatherUnvalidatedInstances之前展开 SWF，且覆盖 PrevalidatedLoad 的重载路径。
- [AssetTypeAptAssets.xsd](https://github.com/wu162/NewSdk/blob/87cf54e254dfb44a129da087316fd04806ed2b10/SDK/Schemas/xsd/AssetTypeAptAssets.xsd) 已定义四种 APT 根；File 为 DataBlob，Geometry 另有 AptID。优先复用这些游戏资产类型及已有 ID 命名，不向游戏流写入新 Swf TypeId。
- [DocumentProcessor.ProcessDocument](https://github.com/wu162/NewSdk/blob/87cf54e254dfb44a129da087316fd04806ed2b10/recovery/src/Managed/Core/BinaryAssetBuilder.Core/DocumentProcessor.cs) 当前要求真实输入文件；[FileNameXmlResolver](https://github.com/wu162/NewSdk/blob/87cf54e254dfb44a129da087316fd04806ed2b10/recovery/src/Managed/Core/BinaryAssetBuilder.Core/FileNameXmlResolver.cs) 不支持 mem://。不能只生成虚拟路径便宣称接入成功。

建议下一阶段：前端把单SWF展开成多个现有APT声明，File指向真实原SWF；专用APT插件根据类型和AptID取 bundle 中的字节，返回正确布局及重定位数组。按原SWF路径+内容hash+RA3/优化选项+转换器版本缓存一次bundle，保留原文件依赖追踪。旧 `.apt/.const/.dat/.ru` 输入仍走原路径。此设计尚需真实SDK验证。

此前恢复的四种 APT DataBlob 根可复用其布局证据及三数组测试方法，但它们目前读取File文件，不等于 SWF 解析器或内存入口。本轮不改 recovery 既有恢复源码及状态文件。

## 验收与续接

第一阶段门槛：固定样例、原源码工具 `ra3=true/optimize=true` 成功产生 apt/const/dat/xml；同时确认受限内存后端与 no-opt 原源码基线三payload逐字节一致，并且转换工作目录无产物。保留 runId、sourceCommit 和输出哈希。

后续先做内存 flasm 与样例优化差分；再做APT三资产SDK对照；扩展到视觉UI时补齐 geometry、纹理及依赖/缓存。最终应比较 `.manifest/.bin/.relo/.imp` 并确认游戏可加载。组件成功不能替代 SDK 或游戏验收。

许可证：上游 eaf2apt 的 COPYING 为 GPLv3；修改及派生源按 GPL-3.0-or-later 标记并保留 `ui/COPYING`。构建时固定源码拉取；没有复制未审计的大型第三方库进入生产代码。SwfLib和原生辅助工具在正式合并前仍须逐组件核对各自许可。
