# 原 SDK 缓存恢复：续接入口

当前任务为原 SDK 完整缓存恢复及用户追加的有界性能基准。

生产矩阵、完整 mymod 原版差分和独立五文件 ZIP 已通过运行 `40814401237`；PR #19 待审查，未合并。二进制及原验收测试来源仍为 `39039961847cd1ca9cb93743fd51741a4e5c9f49`。

性能基准已通过运行 `40820325416`，源码 `c39ad93eb5df7f7b7cd78f7028155d14a5094f2b`：复用已验收 Release，54 次构建及启动诊断约 39.6 秒，未重编译生产。mymod 默认热态约减少 19% 耗时，合成纹理资产热态约减少 35%；小型 Upheaval Armor 子集会话缓存略慢。精确范围、全部数据、时间上限及保留失败见[基准说明](docs/CACHE_BENCHMARK.md)。

原 SDK 同输入对照已通过 `40823369828`，基准源码 `1c7ff21f098e972105c410eb586c4f6c8ba32d29`：54 对/108 次计量及两次预热约 91.2 秒，全部原始输出一致。新版本默认热态端到端耗时在本轮比原版多约 25%～29%，mymod 无缓存基本相当。无会话模式已隔离原版隐式 `StringHashes.xml` 复用，物理输入、Schema、搜索顺序与存在性均相同；[完整数据及边界](docs/SDK_BENCHMARK.md)。

原因分析见[报告](docs/CACHE_PERFORMANCE_ANALYSIS.md)：热态 xmlParse/instanceCompile 均为 0，publish 约 11–30.5 ms；每进程 SC 身份仍校验 826 文件，独立同算法 I/O 复现约 59.5 ms（诊断 `40826280008`，源码 `f7d69ff6f2aeefae1a32642d9854de853a95b82f`，不是生产方法直接打点）。旧工具复用未变链接输出，新宿主重新链接、三次输出哈希及事务复制。仅补分析/诊断，生产逻辑未改。

先读 [交接](REFACTOR_RESUME.md)，按其中的源码提交、验证运行和待办继续；缓存行为与代码分工见 [缓存说明](docs/LEGACY_INCREMENTAL_CACHE.md)。执行规则见 [EFFICIENCY.md](EFFICIENCY.md)。
