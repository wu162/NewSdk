# CLI 编译进度

基线：`recovery/legacy-sdk-cache` / `52d44c6267385055df6da91ecdc6ee2b44f467a9`。

CLI 默认在 stderr 显示计划、数据流、缓存加载/保存、编译、打包/发布及最终结果。
交互式终端每秒刷新一次单行；阶段与子阶段切换立即刷新，完成后保留摘要。
重定向 stderr 时不启动刷新计时器，只输出阶段/数据流开始、结束、诊断与最终结果。
未知总量的资产/XML 不显示百分比或 ETA。终端较窄时优先缩短数据流名称。

数据流计数为成功完成数/计划总数。每个数据流单独累计成功提交资产、XML 解析、
文档复用次数；XML 与后端工作允许同时增长。文档复用是已有 `DocumentReuseCount`
的语义，并非缓存资产个数。成功资产利用已有 `_LastWrittenAssets` 的 `FileBase`
去重，仅在共享提交路径成功后首次登记时增加；BasePatch 无操作不计入。

Core 仅调用 `Action<int,int,int>` 整数增量回调。App 热路径只有原子计数，不输出、
不获取控制台锁、不采集逐资产时间、不新增扫描。状态使用 EventDispatcher 已有计时。
渲染器与警告/错误共用短控制台锁；警告/错误立即换行输出，进度由下一次每秒刷新恢复，
避免大量警告触发逐条重画。完成、失败、取消和释放均清理活动行。
日志 JSONL 的 `Progress` 仍只报告成功完成的数据流数，没有新增逐次刷新事件。
默认 `--performance off` 不变，进度不会启用性能采样。

可用进程环境变量 `NEWSDK_PROGRESS=off` 临时关闭全部进度状态、Core 回调与计时器，
供相同 EXE 的配对计量使用；无需新增普通命令行参数。原有警告、错误与结果仍输出。

验证入口：`.github/workflows/cli-progress.yml`。先运行现有 App 合同与确定性终端测试，
再构建 Release 生产程序及同对象验证入口，运行 Core/CLI/取消合同。完整历史 Upheaval
固定为 `bacfc69fc3e71c8116d1f22b933b8b6173170ecf:SDK/Mods/Upheaval`，树身份
`5551fc26a6f96b62613443e0d0fa2958c14cbce0`。`TestCliProgress.ps1` 在同一 Windows
环境、相同 EXE/SDK/MOD/热缓存下交替进行至少三对开/关测试，维持 `performance=off`，
比较中位数及原始发布字节。测试辅助启动器分配真实 Windows 控制台，并以独立探针核实
标准输出/错误未重定向，避免仅测到 CI 降级模式。辅助进程仅用于测试，不进入产品。

当前验证状态：实现已接通，Windows 构建与配对计量待执行。尚不能声称新增耗时不超过 1%。
