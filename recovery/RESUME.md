# 原 SDK 缓存恢复：续接入口

当前任务为原 SDK 完整缓存恢复及用户追加的有界性能基准。

生产矩阵、完整 mymod 原版差分和独立五文件 ZIP 已通过运行 `40814401237`；PR #19 待审查，未合并。二进制及原验收测试来源仍为 `39039961847cd1ca9cb93743fd51741a4e5c9f49`。

性能基准已通过运行 `40820325416`，源码 `c39ad93eb5df7f7b7cd78f7028155d14a5094f2b`：复用已验收 Release，54 次构建及启动诊断约 39.6 秒，未重编译生产。mymod 默认热态约减少 19% 耗时，合成纹理资产热态约减少 35%；小型 Upheaval Armor 子集会话缓存略慢。精确范围、全部数据、时间上限及保留失败见[基准说明](docs/CACHE_BENCHMARK.md)。

先读 [交接](REFACTOR_RESUME.md)，按其中的源码提交、验证运行和待办继续；缓存行为与代码分工见 [缓存说明](docs/LEGACY_INCREMENTAL_CACHE.md)。执行规则见 [EFFICIENCY.md](EFFICIENCY.md)。
