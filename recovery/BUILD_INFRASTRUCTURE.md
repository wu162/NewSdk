# Core、Remote 与 EALABuild 源码还原

2026-10-25，从 `recovery/compiler-source` 的 `c61aa003be605a7169fba6305311f2c96d88896c` 新建 `recovery/core-remote-ealabuild`。

| 原 DLL | 工程 | C# 文件数 | 依赖 |
| --- | --- | ---: | --- |
| SDK/tools/BinaryAssetBuilder.Core.dll | [Core](src/Managed/Core/BinaryAssetBuilder.Core.csproj) | 78 | BinaryAssetBuilder.Utility、EALA.Hash、EALA.Metrics 及 .NET 2.0 框架 |
| SDK/tools/BinaryAssetBuilder.Remote.dll | [Remote](src/Managed/Remote/BinaryAssetBuilder.Remote.csproj) | 5 | .NET 2.0 框架 |
| SDK/EALABuild.dll | [EALABuild](src/Managed/EALABuild/EALABuild.csproj) | 15 | .NET 2.0 框架、Windows Forms、Drawing、XML |

ILSpy 9.1.0.7988 完整工程导出；保留原程序集名称、版本、命名空间和实现。Core 包括文档处理、插件加载、会话缓存、资产输出、继承合并、命令行、项目模型及原 GUI；Remote 本来就是通信契约和状态/常量；EALABuild 包括构建辅助、路径、步骤参数、脚本/GUI 接口及 BAB 项目/UI 代理生成。Core 的 `BinaryAssetBuilderProject.xsd` 保留为同名嵌入资源。

工程目标保持 net20，语言版本 C# 10，引用路径相对于仓库。Core/EALABuild 显式引用 System.Windows.Forms 和 System.Drawing，修正 ILSpy 的旧框架 WindowsDesktop 工程在现代 SDK 中未自动加入这些引用的问题。没有改动原 SDK DLL/config，没有接入已验收的 GameObject、XML 或 W3X 路由。

## 构建和验证

Windows + .NET SDK 8，首次还原 net20 引用程序集需要 NuGet：

```powershell
dotnet build recovery/src/Managed/Core/BinaryAssetBuilder.Core.csproj -c Release -p:PlatformTarget=x86
dotnet build recovery/src/Managed/Remote/BinaryAssetBuilder.Remote.csproj -c Release -p:PlatformTarget=x86
dotnet build recovery/src/Managed/EALABuild/EALABuild.csproj -c Release -p:PlatformTarget=x86
./recovery/scripts/TestBuildInfrastructure.ps1
```

`RecoverBuildInfrastructure.ps1` 首次导出源码；已有工程时不覆盖源码。原 DLL SHA256 与压缩完整 IL 保存于 `evidence/build-infrastructure/<runId>-<attempt>`。`TestBuildInfrastructure.ps1` 在独立进程和目录中对照原版与 Debug/Release 重建版，恢复版目录排除三个目标原 DLL。检查程序集身份、全部非编译器生成类型/字段/方法/构造器、枚举常量和嵌入资源；行为样本覆盖 Core 标识/定义/XML/集合边界以及 EALABuild 路径、参数构造和属性。

这些是完整源码导出、构建及组件检查；不等同于 Core 全部构建路径已完成差分，也不等同于真实 SDK MOD 编译验收。未执行驻留 BAB 跨进程通信或 GUI 自动化。没有重复启动此前已验收编译器类型的测试。

## 原 DLL 指纹

- Core: `b36d5015f97532457d64072034f9f5c629a4c060a2d34c49c4f89fd6e6f79e47`
- Remote: `126b4ab6c2c879b5f8ed7bb50e49a73e898fb6e082b7727d6424713cd8902135`
- EALABuild: `5e46cfdad438d8fb6e2b683035ab79811c1e696bd673fd1af0ccd68558c7bb7f`
