# Mvp.Xml.dll 源码还原

目标分支：`recovery/mvp-xml-source`，从 `recovery/compiler-source` 的 `260f1d2c34d52910ff27d53ef070059c4b1c9785` 新建。

## 源码与构建

- 完整 ILSpy 9.1.0.7988 工程：`src/Managed/MvpXml`（相对本目录），91 个 C# 文件；原程序集有 124 个类型、1,276 个方法。包含 XInclude、XPointer、XPath、EXSLT、XSLT、XML reader/writer 和序列化辅助实现，不只还原 SDK 当前调用的入口。
- 目标框架 `net20`，程序集名称/版本 `Mvp.Xml, Version=2.0.2158.1055`。按用户要求只保留功能，恢复 DLL 不签名，不需要原私钥、公钥文件或强名称验证绕过。原 DLL 的强名称 token / Authenticode 签名不作为还原目标。程序集清单对照比较简单名称和版本；收据单独记录原版与恢复版的完整身份，明确公钥 token 不同。
- Windows 构建：`dotnet build recovery/src/Managed/Core/BinaryAssetBuilder.Core.csproj -c Release -p:PlatformTarget=x86` 会一同构建 MvpXml。也可单独构建 MvpXml。为保持现有 Core 的 dotnet 构建方式，工程直接嵌入原始 `.resources`，不再依赖 net20 ResGen；构建后名称修复工具使用 Windows .NET Framework 4.8。
- 构建后自动调用 `tools/MvpXmlMetadata`（相对本目录，Mono.Cecil 0.11.6，仅构建工具），将 ILSpy 的 `_002D` 方法名恢复成原始连字符。原 DLL 有 51 个这样的 EXSLT 方法定义，例如 `encode-uri`、`has-same-node`、`date-time`；它们不是可随意删除的 C# 别名。运行库不依赖 Mono.Cecil。Core 的项目引用不要求原公钥 token。
- 三份 `.resx` 保留可读文本，四份原始 `.resources` 原样嵌入（包含空的 `PerfCounterInstaller.resources`），禁用默认 resx 处理，既避免 `SR` 被误识别成文化，也允许 dotnet MSBuild 构建 net20。资源原始字节哈希见导出清单；修改资源文本时需要同步重生成相应 `.resources`。
- 原始 IL 与输入哈希、类型/方法数、51 个名称和资源清单见 `evidence/mvp-xml/export`。首次导出运行只完成源码导出，构建因 ResGen 失败；不将其当作成功构建。

## SDK 源码调用已改为直接调用

用户于本会话要求取消 SDK 调用处的反射。当前 `AssetDeclarationDocument.LoadXml()` 直接执行：

```csharp
XmlReader xmlReader = new Mvp.Xml.XInclude.XIncludingReader(_Current.XmlReader)
{
    XmlResolver = xmlResolver
};
```

`Core/BinaryAssetBuilder.Core.csproj` 以 `ProjectReference` 引用 `MvpXml/Mvp.Xml.csproj`，构建时带出恢复版 Mvp.Xml；`DocumentProcessor.ProcessDocument()` 不再调用可选库加载初始化。`XIncludingReaderWrapper` 仅保留现有公开方法供旧调用者兼容，`LoadAssembly()` 是空操作，`GetReader()` 同样直接构造 reader，无反射或加载开关。

Mvp.Xml 现在是构建依赖，依赖缺失或构造失败会明确报错，不再悄悄禁用 XInclude 并退回普通 reader。这是用户要求的有意行为改变。原 Core 的包装类私有字段 `_Lib`、`_Tracer` 和相关静态初始化器已删除；基础组件契约仅规范化这些指定私有实现差异，仍比较其他类型、公开方法和全部既有行为。

验证同时重建 Core 和 MvpXml，在恢复宿主排除两份原 DLL，并断言实际加载路径。除原包装接口对照外，测试真正执行 `AssetDeclarationDocument.LoadXml(true)`：从磁盘读取 AssetDeclaration、展开 `sdk-defs.xml`、确认 `MVP_VALUE=42` 和一个包含文件依赖。只为构造独立测试状态设置私有 CurrentState；reader 的构造、解析器、XML 加载和依赖收集都由生产 LoadXml 完成。另用 Cecil 只读检查产物 IL 必须直接调用 MvpXml 构造器和 resolver setter，生产入口不得再调用可选包装层。

## 验证与边界

运行 `recovery/scripts/TestMvpXml.ps1`，或查看 `.github/workflows/recover-mvp-xml.yml`。只读 Actions 权限，不会自动提交或推送。原版与 Debug/Release 恢复版使用同一份宿主源码和输入（各自引用相应 DLL 编译，避免恢复版宿主请求原强名称 token），在不同进程/目录运行；恢复版目录排除原始 Mvp.Xml.dll 和 Core.dll。

对照内容包括程序集身份、类型/字段/方法/构造器签名、资源清单与值（仅排除编译器生成的私有常量数据容器，VS2005 使用 GUID 名称、Roslyn 不带 GUID），以及三种 culture 的 XInclude 嵌套、文本、编码、fallback、循环、非法语法，XPointer 选择与拒绝，XPath 缓存/变量/命名空间/排序，EXSLT 数学/集合/字符串/日期/固定种子随机序列，迭代器复制，XSLT 扩展函数（包括 `str:encode-uri` 和 `date:leap-year` 的真实调用）与 XmlSerializer 缓存。成功路径另有断言，不能仅靠两边同样抛异常通过。

这是完整源码和组件验证任务，不修改生产 `SDK/tools/Mvp.Xml.dll` 或配置，不代表真实 MOD SDK 编译已验收。网络 HTTP、性能计数器安装、并发压力、所有公开 API 的全部输入组合尚未穷举验证。原 DLL 本身的异常和限制保持原样，例如 XInclude 的整数索引属性访问不能可靠遍历合成的 xml:base/xml:lang；常规遍历使用 MoveToFirstAttribute/MoveToNextAttribute。

历史独立库验证完成：源码 `fe38f6039f77a45a77ed53da93a53ac97e110da3`，运行 [40188365985](https://github.com/wu162/NewSdk/actions/runs/40188365985) attempt 1 成功。Debug/Release 各 2,110 条转录与原版完全一致，含成员/资源清单和功能行为。恢复程序集身份 `Mvp.Xml, Version=2.0.2158.1055, Culture=neutral, PublicKeyToken=null`。宿主断言实际加载路径为各自隔离目录，Core 包装入口、两种 XSLT 模式、连字符扩展函数、序列化缓存均必须成功，不能将异常匹配当作这些功能的成功。

收据和原版/恢复版完整转录见 [validation.json](evidence/mvp-xml/validation/40188365985-1/validation.json) 与同目录 `transcripts.zip`。交接/证据提交不改变源码、测试、构建工具或 DLL 输入，沿用该精确验收锚点，不冒充后续文档提交新运行。未重复触发其他已验证编译器类型或真实 SDK 流程；无账号额度计量，不能量化额度节省。


## Core 直接调用变更验收

最终验收源码 `a75b48a960d32e199acb2716e06e57e6deca82ce`，运行 [40189585854](https://github.com/wu162/NewSdk/actions/runs/40189585854) attempt1 成功。Debug/Release 每种配置：

- MvpXml + 重建 Core：2,111 条原版对照转录完全一致，含真实 LoadXml 的 include 内容和依赖收集；恢复宿主排除原 Core/MvpXml，断言实际加载路径。
- 产物 IL 审计：LoadXml 直接调用 XIncludingReader 构造器和 XmlResolver setter，生产路径没有可选包装层调用，程序集引用没有原强名称 token。
- Core/Remote/EALABuild 基础回归：各 2,434 条契约/组件记录与原版相同；仅规范化已明确删除的包装类私有加载字段/初始化器及既有编译器初始化差异。
- 使用常规 `dotnet build` 构建 Core 及其 MvpXml 项目引用成功，原四份二进制资源与导出哈希一致。

收据分别见 `evidence/mvp-xml/validation/40189585854-1/validation.json`、`evidence/build-infrastructure/validation/40189585854-1/validation.json`。转录原样保存在各自 transcripts.zip。后续交接/证据提交未修改源代码、测试或构建依赖，不冒充新的运行。历史 40188365985 仅是先前 MvpXml/原 Core 的独立库验收。SDK/tools 内的原始二进制保留作对照，源码调用改动在使用重建 Core/MvpXml 时生效。未执行完整 MOD 编译。
