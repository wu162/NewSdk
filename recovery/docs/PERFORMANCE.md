# 可关闭的构建性能打点

默认 `--performance off`。开启后仍执行正常构建，不自动循环、不更改缓存策略或产物；性能结果位于原有 `build-report.json` 的可选 `performance` 字段。普通日志/JSONL 不逐次记录打点。

```powershell
NewModCompiler.exe build --sdk-root D:\SDK --mod D:\Mods\MyMod --out D:\Build\MyMod --performance summary
NewModCompiler.exe build --sdk-root D:\SDK --mod D:\Mods\MyMod --out D:\Build\MyMod --performance detailed
NewModCompiler.exe build --sdk-root D:\SDK --mod D:\Mods\MyMod --out D:\Build\MyMod --performance off

# 查看自身耗时最大的部分；streamId 为空的是整个请求的阶段。
$r = Get-Content D:\Build\MyMod\build-report.json -Raw | ConvertFrom-Json
$r.performance.entries | Sort-Object selfMs -Descending | Select-Object -First 25 name,streamId,count,totalMs,selfMs,maxMs
```

也可在 MOD 的 `newmodcompiler.json` 设置 `"performance": "detailed"`，命令行优先，可用 `--performance off` 覆盖。`plan` 也支持打点。独立 Core 宿主默认没有回调，需要时可提供 `Settings.MeasurePerformance`；CLI 只在 detailed 档安装回调，结束后恢复原 Settings。

| 档位 | 粒度 |
| --- | --- |
| off | 无新增性能报告；不读取性能时钟、不分配计时 scope、不建立计时字典。保留既有构建阶段事件与缓存计数。 |
| summary | plan/compile/package/publish；身份收据加载/保存；后端初始化/销毁；每流设置、提供程序、Schema、字符串哈希、缓存准备/读取/保存（序列化和 staging 分开）、Core 主处理、产物采集、清理；BIG/LoD/Additional/中立资产修复及发布准备/提交。 |
| detailed | summary 加 Schema 读取解析/编译/依赖整理、文档打开/递归包含、XML 解析/表达式/覆盖/校验、资产处理/编译/验证/提交、按资产类型汇总的编译器调用、输出引用解析/manifest/link/清理、真实目录枚举及 SHA256、Core 一次读取 FastHash+SHA256。 |

`streamId` 对应构建计划的流 ID。名称固定到阶段/资产类型，不含文件名、实例名，重复调用只累加计数，不生成每文件事件。热态没有执行的操作不生成条目，例如复用成功时可能没有 `core.compiler.Texture`；结合原有 `cache.xmlParse`、`instanceCompile`、`fileHashReads` 判断。

- `count`：进入并完成计时范围的次数，包含异常/取消退出的尝试。
- `totalMs`：累计墙钟耗时，包含嵌套打点。
- `selfMs`：累计墙钟耗时扣除直接子打点，便于定位同一层级内部成本。
- `maxMs`：最慢单次调用。平均可用 `totalMs / count` 计算。

采用单调 `Stopwatch.GetTimestamp()` 与其 `frequency`，以 double 毫秒保留亚毫秒信息。收集器由请求独占，跟随现有单线程同步 Core 调用；没有全局性能开关/共享结果，不污染下一次构建。失败和取消会关闭 scope 并把已完成的测量写入失败报告。

不同层级的 `totalMs` 有重叠，不可相加；递归文档/包含也会把子调用时间计入父调用。`selfMs` 只扣掉已开启的子打点，因此 summary 与 detailed 的 self 值不能直接当作同一口径比较。计时本身的管理开销可能计入外层 self；detailed 用于定位，关闭后测最终吞吐。

报告标记 `boundary: before-report-write`：范围截至最终报告写入前，不包括 CLI 参数解析、诊断通道打开、报告序列化/写出、终端事件/日志关闭与最终事务清理。它不是完整进程耗时；完整耗时需由调用方计量。不要为测报告写入而再写一遍报告。

## 有界验证

`TestCompilerPerformance.py` 使用临时 MOD：两个 16×16 Texture 和一个 ArmorTemplate、一个 include；三档各冷态一次/热态两次，交替顺序，校验所有流/CDATAs 原始字节相等，并和独立原版 SDK 比较；再验证三档 BIG/SKUDEF 相等以及无效 XML 的失败计时。每进程默认 10 秒，测量阶段总预算 60 秒，不运行完整 Upheaval。Windows 工作流重新构建 Release 生产组合；构建时间独立于测量预算。

App 合同同时覆盖默认关闭、CLI 值校验、嵌套/递归自身耗时、跨流汇总、异常与取消。Linux 的 Foundation/独立 App 编译只作廉价预检，不能代替 Windows 混合 EXE 运行验收。具体结果以 `RESUME.md` 和对应运行的 `validation.json` 为准。

## 本轮短测结果

[Windows 40886567000](https://github.com/wu162/NewSdk/actions/runs/40886567000) success，生产与测试来源 `239949602a8cc361fd6fb4f3bdb226e84abc6723`。Release 生产组合重建和全部 App 合同通过；测量实际 8.02 秒、14 个进程，包括三档冷态/热态、独立原版、BIG/SKUDEF 和失败 XML。全部指定字节比较通过；[原始摘要与代表报告](../evidence/refactor/performance/40886567000-1/summary.json)，完整样本/日志保留在 digest 已复算的 Actions artifact。

| 档位 | 热态进程耗时中位数 ms | 样本数 |
| --- | ---: | ---: |
| off | 445.85 | 2 |
| summary | 449.10 | 2 |
| detailed | 454.11 | 2 |

同 VM 小样本，OS 页缓存不受控，不能用这些差值精确估计打点开销。热态指编译器缓存热，仍是每次新进程：各档热态零 XML 解析、零资产编译、零完整文件哈希读取；detailed 中 Schema 读取解析 127–138ms、Schema 编译 23–26ms、依赖整理 14–16ms。这些条目解释了未重新编译资产时仍存在的初始化成本，不代表完整 MOD 的瓶颈分布。
