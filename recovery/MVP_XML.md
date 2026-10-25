# Mvp.Xml.dll 源码还原

目标分支：`recovery/mvp-xml-source`，从 `recovery/compiler-source` 的 `260f1d2c34d52910ff27d53ef070059c4b1c9785` 新建。

## 源码与构建

- 完整 ILSpy 9.1.0.7988 工程：`src/Managed/MvpXml`（相对本目录），91 个 C# 文件；原程序集有 124 个类型、1,276 个方法。包含 XInclude、XPointer、XPath、EXSLT、XSLT、XML reader/writer 和序列化辅助实现，不只还原 SDK 当前调用的入口。
- 目标框架 `net20`，程序集名称/版本 `Mvp.Xml, Version=2.0.2158.1055`。按用户要求只保留功能，恢复 DLL 不签名，不需要原私钥、公钥文件或强名称验证绕过。原 DLL 的强名称 token / Authenticode 签名不作为还原目标。程序集清单对照比较简单名称和版本；收据单独记录原版与恢复版的完整身份，明确公钥 token 不同。
- Windows VS2022 完整 MSBuild：`MSBuild.exe recovery/src/Managed/MvpXml/Mvp.Xml.csproj /restore /p:Configuration=Release`。旧 net20 resx 需要完整 .NET Framework ResGen，不能直接用 .NET Core MSBuild 取代。
- 构建后自动调用 `tools/MvpXmlMetadata`（相对本目录，Mono.Cecil 0.11.6，仅构建工具），将 ILSpy 的 `_002D` 方法名恢复成原始连字符。原 DLL 有 51 个这样的 EXSLT 方法定义，例如 `encode-uri`、`has-same-node`、`date-time`；它们不是可随意删除的 C# 别名。运行库不依赖 Mono.Cecil。SDK 的 Core 通过简单程序集名反射加载，不要求原公钥 token。
- 三份 `.resx` 保留可读资源，显式设置 `WithCulture=false`，避免新 MSBuild 把名称中的 `SR` 当作塞尔维亚语文化并误生成卫星程序集；ILSpy 未导出的原空 `PerfCounterInstaller.resources` 从 DLL 原样取回。组件对照比较资源名和全部资源键值，而非依赖 ResGen 二进制排版相同。
- 原始 IL 与输入哈希、类型/方法数、51 个名称和资源清单见 `evidence/mvp-xml/export`。首次导出运行只完成源码导出，构建因 ResGen 失败；不将其当作成功构建。

## SDK 实际调用

`DocumentProcessor` → `XIncludingReaderWrapper.LoadAssembly()` → `Assembly.Load("Mvp.Xml")`；`AssetDeclarationDocument` → `XIncludingReaderWrapper.GetReader(XmlReader, FileNameXmlResolver)` → 反射构造 `Mvp.Xml.XInclude.XIncludingReader` 并设置 `XmlResolver`。

对照宿主直接调用原始 Core 的这两个包装方法，使用覆写资源获取的 `FileNameXmlResolver` 子类；需断言返回非空并实际展开汉字资产内容，不能把 Core 返回 null 或静默关闭 include 算作通过。XML include 使用自定义 resolver，原版文本 include 直接走 WebRequest，因此测试同时建立共同的临时磁盘素材。所有输入为本地生成数据，无远程网络依赖。

## 验证与边界

运行 `recovery/scripts/TestMvpXml.ps1`，或查看 `.github/workflows/recover-mvp-xml.yml`。只读 Actions 权限，不会自动提交或推送。原版与 Debug/Release 恢复版使用同一份宿主源码和输入（各自引用相应 DLL 编译，避免恢复版宿主请求原强名称 token），在不同进程/目录运行；恢复版目录排除原始目标 DLL。

对照内容包括程序集身份、类型/字段/方法/构造器签名、资源清单与值（仅排除编译器生成的私有常量数据容器，VS2005 使用 GUID 名称、Roslyn 不带 GUID），以及三种 culture 的 XInclude 嵌套、文本、编码、fallback、循环、非法语法，XPointer 选择与拒绝，XPath 缓存/变量/命名空间/排序，EXSLT 数学/集合/字符串/日期/固定种子随机序列，迭代器复制，XSLT 扩展函数（包括 `str:encode-uri` 和 `date:leap-year` 的真实调用）与 XmlSerializer 缓存。成功路径另有断言，不能仅靠两边同样抛异常通过。

这是完整源码和组件验证任务，不修改生产 `SDK/tools/Mvp.Xml.dll` 或配置，不代表真实 MOD SDK 编译已验收。网络 HTTP、性能计数器安装、并发压力、所有公开 API 的全部输入组合尚未穷举验证。原 DLL 本身的异常和限制保持原样，例如 XInclude 的整数索引属性访问不能可靠遍历合成的 xml:base/xml:lang；常规遍历使用 MoveToFirstAttribute/MoveToNextAttribute。

验证完成：源码 `fe38f6039f77a45a77ed53da93a53ac97e110da3`，运行 [40188365985](https://github.com/wu162/NewSdk/actions/runs/40188365985) attempt 1 成功。Debug/Release 各 2,110 条转录与原版完全一致，含成员/资源清单和功能行为。恢复程序集身份 `Mvp.Xml, Version=2.0.2158.1055, Culture=neutral, PublicKeyToken=null`。宿主断言实际加载路径为各自隔离目录，Core 包装入口、两种 XSLT 模式、连字符扩展函数、序列化缓存均必须成功，不能将异常匹配当作这些功能的成功。

收据和原版/恢复版完整转录见 [validation.json](evidence/mvp-xml/validation/40188365985-1/validation.json) 与同目录 `transcripts.zip`。交接/证据提交不改变源码、测试、构建工具或 DLL 输入，沿用该精确验收锚点，不冒充后续文档提交新运行。未重复触发其他已验证编译器类型或真实 SDK 流程；无账号额度计量，不能量化额度节省。
