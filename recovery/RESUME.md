# Tokenizer 并行与准备阶段去拷贝：实现完成，Windows 验证待运行

成果分支仍为 `recovery/legacy-sdk-cache`，本轮基于 `fbf51cee`。直接开放精确内建 GameObject、WeaponTemplate、ArmorTemplate 的 worker 资格；未改变类型/处理哈希、旧缓存格式或 jobs 身份。三种插件可选 trace 的共享索引写入已同步。

根据 Core 生命周期和实际后端读取闭包，删除 XML DOM、NameTable、namespace bindings、句柄和引用集合的深拷贝；无 CDATA 的任务直接使用原声明，CDATA 任务只浅拷贝声明状态以隔离输出路径。删除调度器重复的整树估算和无外部输入任务的租约列表分配。原生字符串/输入树、Tracker、输出三缓冲及外部文件租约继续保留，原因见 [本轮源码依据](docs/TOKENIZER_PARALLEL_INPUT.md)。

本地已通过 Python 语法、新增生产 fixture 的原 SDK Schema 预检及 diff 检查。四平台 worker/trace、284 XML 路由、Debug/Release 生产缓存/jobs 矩阵、失败取消、原 BAB/mymod 和完整 Upheaval 性能由 `[refactor-core-lifecycle]` 统一重建验证；当前尚未取得本轮 Windows 结果，不能复用旧二进制冒充通过。

上一轮已验收生产源码 `6d39f833`、功能运行 [40915444929](https://github.com/wu162/NewSdk/actions/runs/40915444929) 与性能 [40920804580](https://github.com/wu162/NewSdk/actions/runs/40920804580) 只适用于旧实现。原验收与计量保留于 [历史迁移记录](docs/BACKEND_PARALLEL_MIGRATION.md)。禁止强推；未合并其它主线，未发布 Release。
