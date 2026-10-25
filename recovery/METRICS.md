# EALA.Metrics.dll 源码还原

从 `recovery/compiler-source` 的 `7fc417546989ca94a18742b37965e9c6751a409e` 新建 `recovery/eala-metrics-source`。完整工程位于 `recovery/src/Managed/Metrics/EALA.Metrics.csproj`，共 13 个 C# 文件（12 个类型文件与 AssemblyInfo），保留 `EALA.Metrics, Version=1.0.0.0`、原命名空间、成员和 net20 目标。项目使用 C# 10，避免现代编译器额外生成 ref-safety 兼容类型。

原件：`SDK/tools/EALA.Metrics.dll`，SHA256 `bcf488c809a052fe920de95ec10975693ffa27f8d3e892a1cff510c4a118fa1c`。ILSpy 固定版本 `9.1.0.7988` 全工程导出；原始 IL 及首次 Debug/Release 构建记录位于 `evidence/metrics/40186363315-1`。未手工改写指标逻辑，没有占位实现。后续仅整理项目的 LangVersion 与构建产物忽略规则。

## 功能与调用

- `MetricDescriptor`、`MetricType`、`Metric`：指标元信息、样本数组、时间戳和显示格式。`ListMetric` 保留可修改 ArrayList 引用。
- `MetricManager`：描述符缓存、监听器、会话开关和七种样本类型转换。描述符名称使用当前区域设置 `ToLower()`，不是 invariant；`SubmitInternal` 会改写调用方 samples[0]。这些原行为均保留。
- `MetricTimer`：四个构造入口，Dispose 时提交耗时及可选用户数据；原实现重复 Dispose 会重复提交，未擅自改成幂等。
- `MetricApplicationData`：Windows 用户、机器、进程路径，三个属性可读写。
- `ConsoleMetricsListener`、`ReportMetrics`：控制台输出。前者虽存储传入的 TextWriter，Close 实际使用 Console.WriteLine，且不清空缓冲区；保留原行为。
- `SqlMetricsListener`、`SqlMetricsWriter`：配置读取、SQL Server 存储过程会话、描述符及指标写入，完整保留 System.Configuration/System.Data 依赖。

BAB 的 `BinaryAssetBuilder._Descriptors` 注册 22 个指标；Build 流程打开/关闭 MetricManager 会话，Core 等代码提交指标；BAB 的 `FileMetricsListener` 和 `OutputMetricsListner` 实现此 DLL 的 IMetricsListener。还原工程可独立构建，尚未替换 SDK/tools 原 DLL。

## 验证与边界

入口：`recovery/scripts/TestMetrics.ps1`，宿主 `recovery/tests/MetricsOracle.cs`。原版和 Debug/Release 恢复版在不同目录、不同进程运行，恢复宿主不放入原 EALA.Metrics.dll。比较程序集身份、完整类型/成员清单（包括私有成员和枚举常量）以及离线行为转录。

矩阵包含 en-US/de-DE/tr-TR，全部 MetricType 和未知枚举值，空值/空数组/错误类型/溢出及大小边界；指标格式化、描述符大小写缓存、提交转换与调用方数组别名、禁用/重开/非法会话、注册异常、四个计时器构造入口及重复 Dispose、监听器输出和列表引用、应用数据属性、SQL 缺失配置及未打开关闭异常、离线描述符去重。时间戳先断言构造时间范围，再固定私有时间戳以比较文本；计时器比较非负 double、附加数据及提交次数，不把两进程真实耗时当成确定值。

SQL 服务端及存储过程执行未验证；没有可用测试数据库，不能将离线验证冒充数据库写入验收。未执行完整 MOD 的真实 SDK 集成编译，未改 SDK DLL/config、其他编译器或现有成功测试的触发范围。正式替换 SDK 前仍应验证适用集成路径。

验收源码 `2fc44710df27ea9baf4e73d183dcd7697a9a435d`，运行 [40186536173](https://github.com/wu162/NewSdk/actions/runs/40186536173) attempt 1 成功。Debug/Release 各 1,197 条对照记录（含身份/类型成员清单与行为结果）与原版完全一致，三份转录 SHA256 均为 `20ae1300c28739641b99f4d5293dd543aab267a68944e7a21f604dffca440de4`。收据：[validation.json](evidence/metrics/validation/40186536173-1/validation.json)，完整转录：[transcripts.zip](evidence/metrics/validation/40186536173-1/transcripts.zip)。

本轮只运行新增 Metrics 工程的导出/构建和完整离线差分；未重复运行已成功验收的其他编译器。后续只追加文档，复用上述精确源码验收，不冒充文档提交的新构建结果。
