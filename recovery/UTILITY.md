# BinaryAssetBuilder.Utility 源码还原

从 `recovery/compiler-source` 的 `7fc417546989ca94a18742b37965e9c6751a409e` 新建独立分支 `recovery/utility-source`。本次只恢复 Utility，不修改生产 SDK DLL/config 和其他编译器实现。

## 实现与原始证据

原 DLL 是 x86 .NET 2.0 C++/CLI 混合程序集，不是纯 C#。ILSpy 9.1.0.7988 全工程及原 IL 在提取运行 40186176619 保存，原始工程另归档到 `evidence/utility/ilspy-source`，不参与构建。原 DLL SHA256：`f71d8f4b10002637deb72eac68e14599e38bac07221f0ed2d1dfc751d8ffd96b`。

可重建工程为 `src/Managed/Utility/BinaryAssetBuilder.Utility.csproj`，保留原程序集名、版本 `1.0.3344.32795`、业务类型、公开接口和 x86/net20 目标：

- AssetHeader/AssetEntry/ManifestHeader：32/48/48 字节布局、全部属性、大小端转换、流读写、长度检查。
- Manifest/Asset：读取版本 >=4、引用和补丁搜索、基础流资产复用、StringHashes.xml 查找/解析、链接偏移、资产状态检查和删除。
- NameBuffer/ReferencedFileBuffer/UInt32Buffer：偏移计算、ANSI 名称、哈希去重、引用标记和大小端序列化。
- FileSystemUtils：原参数的 Win32 Unicode 调用及错误文本；AssetHandle/ReferenceHandle/ReferencedManifest 同时保留。

`std::map<int,int>` 换成托管字典，键仍然是原版的 `String.GetHashCode()`：不同字符串哈希碰撞、同名引用改变 isPatch 都复用首偏移。不得误改为按字符串去重。Manifest 的原大小端细节（包括读取时未交换 Tokenized 字段）、读取一次即使用的短流行为、写流失败后未恢复 Swap 的状态均按原 IL 保留。

C++ 模板基类名 `StructWrapper<AssetStream::...>` 及涉及内部原生结构的公开指针签名无法直接用 C# 表达。构建后工具 `tools/UtilityIdentity` 使用固定 Mono.Cecil 0.11.6 恢复原名、类型标志、原生结构可见性、方法标志和签名自定义修饰符。GetErrorString 的 uint 参数带原 C++ `modopt(IsLong)`；省略它会使已编译调用者抛 MissingMethodException，因此也显式恢复并差分核对。工具只从原 DLL 读取这些元数据，不复制或调用原方法体；业务实现均来自源码。使用 .NET SDK 8 构建会自动执行此步骤，首次构建需要 NuGet。

```powershell
dotnet build recovery/src/Managed/Utility/BinaryAssetBuilder.Utility.csproj -c Release
./recovery/scripts/TestUtility.ps1
```

原版未初始化的 AssetHeader/AssetEntry 内存、越界 cpblk、释放后指针和重复释放不是可确定的语义保证。测试先装入完整已知 buffer，或使用原版定义明确的短 Stream（内部零填充）。恢复版使用 Marshal 分配/释放并将 Dispose 后指针清零，避免重复释放；没有把原本未初始化的资产头全部改成零。Manifest 的 CRT 文件读入改用托管读入后复制到相同的非托管布局；打不开文件仍返回 false。FileSystemUtils 成功刷新卷路径未在测试中执行，避免写入真实卷。

## 验证状态

验收源码 `9c7b59036c0bc518727023331603e59bf16c48a4`，运行 [40186857933 attempt 1](https://github.com/wu162/NewSdk/actions/runs/40186857933) 成功。Debug/Release 各 **269,516 条记录**与原版完整转录 SHA256 相同：`1747257c9490ad818ba5c8a7690dd9e58bab51edc0a329b903d93b0327e2d72d`。完整记录以单份等价基准 `transcript.zip` 保存，两个配置的比较结果、重建 DLL 指纹、22 份源码/工具文件与七份真实输入指纹见 [验收收据](evidence/utility/validation/40186857933-1/validation.json)。测试使用 x86 CLR4 兼容宿主；重建 DLL 仍以 net20 为目标。

首轮 40186499964 发现两处 ILSpy 的 C++/CLI Dispose 语法，第二轮 40186662830 发现 GetErrorString 的 IsLong 二进制绑定差异；均已修复，历史失败保留，最终完整差分来自上述成功运行。未执行 EALAModStudio/BAB 完整 MOD 编译，不能把其他恢复模块的 SDK 验收算作本 DLL 的 SDK 验收。

差分在原版和恢复版独立进程、独立依赖目录执行；恢复目录从未复制原 Utility DLL。覆盖公开类型/成员清单、三种文化、名称碰撞和 ANSI/NUL、UInt32 批量写入、全部头字段/大小端/短读/异常、Manifest v3 拒绝与 v4–6、引用、补丁、hash XML、内部句柄比较，以及仓库自带七份真实 manifest 的全部资产读取。真实 manifest 读取是组件输入验证，不代表运行真实 SDK 编译。

本轮仅运行 Utility 提取和专用差分，没有重复触发未变化的 GameObject/XmlCompiler/W3X 或真实 SDK 工作流；不能量化账号额度节省。
