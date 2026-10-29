# 原 SDK 与新版性能对照

`BenchmarkCompilerSdkComparison.py` 在同一台 Windows VM 上交替执行原 `BinaryAssetBuilder.exe` 和已验收的新版 Win32 Release。比较独立进程从启动到退出的端到端成本、CPU 时间、峰值工作集及缓存占用；新版还承担报告、JSONL、独立 staging 和事务发布的成本，原工具直接写出流文件。结果用于比较实际命令行使用成本。

## 输入一致性

- 两边共用同一个物理 MOD 目录和同一个 `Data/Mod.xml`，不为某一边删改 XML、改类型或缩小资源。完整 mymod、11 个原始 Upheaval Armor 文档/12 资产的子集、16 个合成 Texture 分别统计；工作量沿用已有[缓存基准](CACHE_BENCHMARK.md)。
- 两边共用相同 SDK、Schema 及资源文件。按 MOD 对应目录优先、再按 SDK 配置补充的顺序，冻结绝对 Data/Art/Audio 搜索路径；重复和不存在的候选也保留，逐项显式传给两边。新版的真实报告必须确认路径、顺序、Schema 和入口完全一致。
- 原 EXE 和所有 DLL 对照固定源树校验，复制后保持原字节及插件路由；配置仅适配 CLR4 startup，`assetbuilder` 与 LowLOD/MediumLOD 声明保留。新版直接使用原验收的精确五文件包，生产源码未重新编译。
- 缓存选项、Win32、链接、稳定排序、预编译引用、慢清理和诊断级别显式对齐。两边均单次运行，关闭驻留、GUI、监视和旧指标上报；不包含旧驻留 worker 的跨调用复用。
- 两边使用独立的会话、资产和中间目录，空缓存开始，不共享另一引擎生成的缓存。无会话缓存时每次换新中间目录，保留本模式的资产缓存供热态使用；会话模式保留本引擎、本模式的中间结果。
- 准备时保存完整共享 SDK、MOD 及原/新运行库清单。逐对检查 MOD 不变，最后确认全部输入、运行库及输入根的存在性未变。

## 样本与有效条件

6 模式为无缓存、仅资产缓存、仅会话 XML、仅会话 Deflate、两缓存 XML、两缓存 Deflate。每个引擎、每个工作量：无缓存 3 次，其他模式各 1 次冷态和 2 次热态。合计 54 对、108 个计量进程；另各 1 次相同 mymod 无缓存构建作启动预热，不计入构建成绩，但计入整组预算。

每轮改变模式及引擎执行顺序，减少固定先后顺序的偏差。单进程最多 30 秒，整组最多 180 秒；准备步骤最多 2 分钟、Windows job 最多 8 分钟。OS 页缓存未清空，“冷”只指编译器缓存为空。少量样本报告中位数和 min–max，不宣称统计显著。

每对必须成功退出且无编译错误，所有输出的成员、长度、SHA256 及原始字节一致；不同模式还必须匹配同工作量的无缓存基线。只去掉两种宿主不同的流目录前缀，不改输出字节，不能删除额外引用流来取得相同结果。原始四流/CDATAs 的首组基线保存在产物中。

原工具不提供新版同格式的持久计数，保留其完整日志和可识别的会话载入/资产缓存复制消息，不能把未观测到的计数写为 0。新版按真实报告校验热会话复用、编译次数和适用类型的资产命中。GameObject/Weapon/Armor 在原 SDK 的类型配置中未启用资产缓存，bc-only 不据此宣称发生了复用。

## 重复运行

```powershell
python recovery/scripts/PrepareCompilerCacheBenchmark.py --repo . --output "$env:RUNNER_TEMP/cb"
python recovery/scripts/PrepareCompilerSdkBenchmark.py --preparation "$env:RUNNER_TEMP/cb/preparation.json" --repo . --output "$env:RUNNER_TEMP/ob"
python recovery/scripts/BenchmarkCompilerSdkComparison.py --preparation "$env:RUNNER_TEMP/cb/preparation.json" --original-preparation "$env:RUNNER_TEMP/ob/original-preparation.json" --output recovery/evidence/refactor/sdk-benchmark/local --work-directory "$env:RUNNER_TEMP/sbw" --timeout-seconds 30 --budget-seconds 180 --hot-runs 2
```

准备器支持复用本地已下载归档，详见基础缓存基准。输出及工作目录必须不存在；原版混合工具使用短工作路径。工作流为 `.github/workflows/compiler-sdk-benchmark.yml`，显式 `[sdk-benchmark]` 提交或手动触发；不重复运行生产编译与完整 SDK 验收。
