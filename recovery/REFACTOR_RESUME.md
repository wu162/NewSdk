# 新 MOD 编译器交接：下一轮 P4

工作与交付分支：`refactor/modcompiler-p0-p3`。目标分支：`recovery/compiler-source`。
草稿 PR：#13，未经用户明确批准不得合入；禁止任何强推。
实施方案：[NewSdk_ModCompiler_Refactor_Plan.md](NewSdk_ModCompiler_Refactor_Plan.md)。

本页交接源码组织、构建约束和续接范围，不公布新的运行编号、指纹或产物链接。
新增验收摘要的公开提交仍等待单独授权；原始资料保留在本分支已有 Actions 产物中。
下一轮先读取实际收据并确认对应源码，复用适用证据，不因这里未发布摘要而重跑全部验证。

## 当前代码入口

- `src/App`：MOD 目录 CLI、内容选择、流入口发现、可注入依赖计划、顺序 BuildSession、独立 StreamContext、日志／JSONL／终态报告。
- `src/Managed`：Core、Utility、Hash、Metrics、MvpXml、ExpressionEval、GameDataVerifier、Audio；音频已迁到 `src/Managed/Audio`。
- `build/Managed.Foundation.json`：统一管理源码和资源清单；排除旧 GUI 和重复 AssemblyInfo。
- `scripts/BuildCompiler.ps1`：独立生产 CLI／Foundation EXE；入口是 `src/Hosting/CompilerEntry.cpp`，不编入探针和测试类。
- `scripts/TestCompiler.ps1`：生产 EXE 的干净目录、类型、资源、CLI 与明确失败行为检查。
- `scripts/TestManagedFoundation.ps1`、`TestSingleExeProbe.ps1`：分别检查管理实现和跨语言统一链接。
- `probes/TextureRedirect`、`scripts/TestTextureRedirect.ps1`：混合贴图 DLL 原生写入、PE 审计及原版／往返／EXE 绑定探针；不覆盖 SDK 原件。
- `protocol`、`scripts/TestApp.ps1`、`ValidateAppProtocol.py`：信息协议及应用契约检查。
- `scripts/ValidateAudioSourceTree.py`：只有项目 HintPath 的准确目录迁移可以等价复用旧树身份；源码、文件集合、模式或其他项目内容变化均拒绝。历史收据不改写。

## 必须保留的构建和运行约束

1. Windows x86、.NET Framework 4.8、固定 Roslyn，使用 VS 2022 的 x86 C++/CLI 工具链。
2. 构建期 C# `.netmodule` 使用 AnyCPU；最终链接仍为 `/MACHINE:X86`。不能改写 unsafe 标记伪造安全模块。
3. Foundation 不反向依赖上层 Bridge。Bridge 从 Foundation 模块导入类型，Application 再引用 Bridge 对象文件；由 MSVC 最终统一链接。
4. 已恢复业务实现进入物理 EXE。不能以嵌入并解包 DLL、保留一套恢复插件 DLL、隐藏搜索路径等方式代替。
5. Core Schema 和 MvpXml 资源保持逻辑名称与准确字节；Utility 签名修饰符和 EXSLT CLR 名称在纯 IL 中间模块修复，最终混合 EXE 只读检查。
6. 贴图原 DLL 的共享字符串、元数据 token、原生段、导入和重定位都要保护。保留原件，只对发行副本操作；不得无证据退回 Core 转发 DLL。
7. 正式产品保留必要的 ANTLR 依赖；旧 GUI、Remoting、驻留 BAB 和旧程序集名反射发现不进入生产链。
8. 构建期工具显式使用 AnyCPU，隔离 MSVC 的 Platform 环境；不能影响最终编译器的 x86 约束。
9. 必需日志写入及关闭失败均为失败；事件、报告和进程退出码需交叉核对。保持单终态、单次资源关闭。

## 当前限制

`plan` 生成入口发现／显式依赖计划，不代表已解析真实 Core Include 和引用闭包。
生产 `build` 当前明确返回 `BACKEND_NOT_INTEGRATED`（退出码 3），`doctor` 同样说明后端限制。
完整 Native 生产路由、真实 Core 执行器、缓存失效与输出提交尚需后续阶段；不得将探针成功等同于新入口完成真实 MOD 构建。

## 下一轮只推进 P4

- 从现有有效构建入口整理明确 Native／Bridge 源码清单，共享实现只编译一次，不递归无区别纳入 Draft 或重复实现。
- 将 Tokenizer 三根、已声明 XML 路由和 W3X Native／Bridge 与 Application 包装统一链接进生产 EXE。
- 内建注册使用明确类型／工厂，检查重复或缺失路由；不回退原 Tokenizer、XmlCompiler 或恢复业务 DLL。
- 接入贴图兼容边界时保持 EXE 类型身份、补丁验证及依赖允许清单，保留错误和生命周期行为。
- 以实际受影响依赖组集中执行完整差分、路由及干净目录检查；未变更且证据有效的组复用原收据。
- 完成 P4 的实现、必要验证和交接后结束；不自动进入 P5–P7，不发起完整 MOD 新入口验收来冒充 P4 组件验证。

P5 再接通真实 MOD 的单进程 Core 流执行、依赖闭包、缓存／失败／取消清理和输出提交，并作真实 MOD 对照。
P6 发布清理、P7 完整后处理保持独立，不提前承诺 BIG／后处理能力。

## 工作组织

上一轮的子代理额度不自动延续。新一轮数量由用户临时指定；未指定则单代理推进。
所有成果继续集中在本工作分支；临时分支成果收回。按有意义检查点保存，保留并发更改，不强推。
工作流保持只读仓库权限，保留验证和产物上传，不恢复自动提交／推送步骤。
公开验收资料未获授权前，不通过文档、PR 正文或其他位置绕过该限制；交接源码不需要重复完整 SDK 基线。
