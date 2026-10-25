# ExpressionEval 与 GameDataVerifier 源码还原

2026-10-25。目标分支 `recovery/compiler-source`。两程序集均为托管 C#/.NET 2.0，使用 ILSpy 9.1.0.7988 完整导出，源码位于现有 `recovery/src` 目录下；没有遗留占位实现或未导出的方法。

## 源码

| 原程序集 | 源码工程 | 恢复内容 |
| --- | --- | --- |
| BinaryAssetBuilder.ExpressionEval.dll | [ExpressionEval](src/Managed/ExpressionEval/BinaryAssetBuilder.ExpressionEval.csproj) | ExpressionEvaluator、ExpressionValue、完整 ANTLR ExpressionLexer/ExpressionParser、token 常量、程序集元数据 |
| BinaryAssetBuilder.GameDataVerifier.dll | [GameDataVerifier](src/Managed/GameDataVerifier/BinaryAssetBuilder.GameDataVerifier.csproj) | Plugin 生命周期、GameObject 模块 ID、默认动画状态、ConditionsYes 重复、StateName 脚本引用校验、程序集元数据 |

保留原程序集名称、版本、类型命名空间及可见性；GameDataVerifier.Plugin 原本为 internal，lexer/parser 原本处于全局命名空间。保留原来的运算、异常和校验边界，不顺便更改原版行为。ExpressionEval 继续依赖 SDK 的 `antlr.runtime.dll` 和 `BinaryAssetBuilder.Core.dll`；GameDataVerifier 继续依赖 Core 和 System.Xml。

ILSpy 初始导出工程的绝对依赖路径已修正为相对 `SDK/tools` 路径，语言版本设为 C# 10，避免 C# 11+ 自动注入 RefSafetyRules/EmbeddedAttribute 兼容类型。目标框架仍为 net20。ExpressionLexer 的旧 Hashtable 构造器保留原逻辑，因此现代编译器有一条 CS0618 警告，不影响构建。

## 构建与验证

Windows + .NET SDK 8 或更新版本（首次还原 .NET Framework 引用程序集需要 NuGet）：

```powershell
dotnet build recovery/src/Managed/ExpressionEval/BinaryAssetBuilder.ExpressionEval.csproj -c Release -p:PlatformTarget=x86
dotnet build recovery/src/Managed/GameDataVerifier/BinaryAssetBuilder.GameDataVerifier.csproj -c Release -p:PlatformTarget=x86
./recovery/scripts/TestManagedSupport.ps1
```

源码验收提交：`e806b1ca26509915621d214be2492d572db78f74`。
[运行 40171170187 attempt 1](https://github.com/wu162/NewSdk/actions/runs/40171170187) 成功。
[结构化验收](evidence/managed-support/validation/40171170187-1/validation.json) 与同目录构建日志、`transcripts.zip` 已保存。

- Debug/Release 两种配置分别对照原版，每种 32,306 条记录：231 条类型/成员清单，32,075 条行为记录，两个配置的完整转录 SHA256 均与原版相同。
- 表达式覆盖整数/浮点/字符串构造及转换、全部公开一元/二元运算、解析优先级与括号、round、define 本地/外部查找和 UsedDefines、副作用、定义求值幂等、溢出、除零、错误词法/语法及异常；使用 en-US、de-DE、tr-TR 三种文化。
- 数据校验覆盖 Win32/Xbox360/PlayStation3/unknown 99，正常/重复/缺失模块 ID、大小写与土耳其文化边界、默认动画状态重复、条件换序及空白、StateName 缺失/歧义/有效引用、脚本末次调用与异常、非 GameObject 路由及空节点。
- 原版和恢复版在不同进程及目录运行。恢复目录从未复制两个目标原 DLL，只放重建的同名 DLL；Core 的 ExpressionEvaluatorWrapper 实际加载及接口调用通过。
- 原 DLL、SDK 配置、其他已恢复编译器源码未替换。这是完整源码还原及组件差分验收；没有执行完整 MOD 的真实 SDK 集成编译，也不将此前其他插件的 SDK 证据套用于本次。

原 DLL SHA256：

| 程序集 | SHA256 |
| --- | --- |
| ExpressionEval | `c2c872619b4391f5987d483597116fb33e0cbf09f3cebeeda9d0105cce18b5ac` |
| GameDataVerifier | `497f29052a32dca2002857a3dbc88232bb66a19e3dfcfecbdf048c73417847f0` |

## 原始证据与复现入口

[ILSpy 提取记录](evidence/managed-support/40170744056-1/extraction.json) 和同目录两个 `*-original.il` 保留完整原 IL。
`RecoverManagedSupport.ps1` 用于首次提取，拒绝覆盖现有恢复源码；日常构建与回归使用 `TestManagedSupport.ps1`。
专用工作流 `recovered-managed-support.yml` 按这两个工程、测试及实际 DLL 依赖变化触发，不调用 GameObject/XmlCompiler/W3X 的类型差分或真实 SDK 流程。

本轮未引入其他插件还原，也未重复调查已验收类型。初次差分识别出的现代编译器附加属性已通过工程语言版本修正，最终验收没有过滤该属性来放宽类型清单比较；历史失败运行 40171021889 保留。
