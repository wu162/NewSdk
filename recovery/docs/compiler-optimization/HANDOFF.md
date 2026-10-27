# 编译器优化：当前交接

更新：2026-10-27，北京时间；本轮15:56开始，按用户最新要求推进接近90分钟，使用2个子代理。
成果分支：`feature/compiler-optimization`，起点 `compiler-main@e7396d3`。
执行约定与方案读取索引：[README.md](README.md)。

## 当前状态：既有W02/W04已验收，W03正内容子组验证中

- W02：增加 rawParse、schemaRead/Compile/Metadata/ContentCheck、逻辑 schema 读取、outerStages 与顶层 applicationWork；不可变快照保留实际调用/抛异常次数，明确 beforeInitialReport 边界和嵌套区间，未宣称覆盖全部 wall 成本。
- W04：每 build 单 entry MetadataCache，仅保存自有继承/依赖值，存储/恢复均深拷贝。三流真实 CLI 双配置均从3次表遍历降为1次构建+2次恢复；每流仍完整读取815个schema文件/1,515,884字节并Compile自己的Framework schema、绑定helpers及整文档验证。提供 DisableSharedSchemaMetadata 回退，尚无跨流Framework schema或引用描述共享。
- 既有 W01 三项真实 Core 风险修复、W03 单遍双摘要、W08 NodeJoiner 局部索引及同代引用描述均保留并回归。每流重绑仍不等于 W06 不可变结果或并行完成。

## 本轮进行中

W03已接入有界PhysicalInputSnapshot：首次正内容单遍封存SHA256+原1MiB FastHash；文件只读pin与父目录防替换pin保留到发布/cache/必需输出完成，发布前强内容和身份复核；XML/XInclude/schema读同内容，跨流/配置共享。预算、不支持路径、输出工作区及opaque Texture单独bypass，保留/bc /sc全根复核；预编译reference manifest与隐藏读取未证明完整闭包，不宣称W03全部完成。
源码 `0df7c92f4fb54d238b3dae588787f56bdb9cc230` / [40472198342](https://github.com/wu162/NewSdk/actions/runs/40472198342) 完整验证中。首次checkpoint21cd74f9的CLI测试收据重复PowerShell属性已普通修复，原预检失败[40472165263](https://github.com/wu162/NewSdk/actions/runs/40472165263)保留。
W05 resolver候选影子日志源码b88240bc；专用fixture在40472999771双配置独立通过（204检查/6组），不授权缓存。W03原run同时发现取消包装、旧逐流计量假设、same-backend修复重试pin和新增fixture漏XML using；dd6745f8已保存取消/重试/仪表修复检查点，尚需新验收。W08基准manifest有序索引及原DLL三方用例正在集成，所有未验收项不标完成。

## 最新验证身份

所有编译、测试和微基准仅在 Actions；本地只编辑、静态检查及读取证据。
构建/测试/打包源码 `4a128696c2de04b5151c4dc9f1ac3c35661e7407` / [40466010643](https://github.com/wu162/NewSdk/actions/runs/40466010643) 整体 success：Debug/Release生产、各14个Core fixtures、原BAB、协议、Texture、实际CLI、全部缓存、输入根、完整mymod/BIG与五文件ZIP独立解压验收均通过。后续仅文档，不改二进制来源。
提前 artifact `12716528944` 与完整 artifact `12716723779` 均下载核对SHA256；完整包含两配置ZIP及准确source/run manifest。哈希、微基准和范围见 [VALIDATION.md](VALIDATION.md)。
本轮前两次运行40465129025/40465759765因新增测试误要求零输入可选组存在backendCompile指标而失败，原终态保留；已按真实调用数（含0）修复并完整重验，未放宽生产拒绝或字节断言。

## 测量与下一步

共享前真实三流每配置重复schema逻辑读2445次/4,547,652字节，外层schema准备合计约668/686ms。W04后读/Compile次数不变，元数据命中区间约1ms；三次微基准波动明显、单流还需保存成本，不能宣称完整构建提速。OS页缓存unknown，不跨CI机器相除比较时间。
下一组优先推进 W03 构建级实际消费输入封存及变化拒绝，再接 W05 影子依赖。不能仅加path→digest字典：Reset强内容复核、同时间同长度变更、XML/Include/XInclude/payload消费同版本、cacheoff发布前强复核，以及快照跨Dispose/发布/Complete生命周期须成组处理。未证明消费闭包的类型保守回退；保留现有/bc /sc全根复核。
W05影子依赖、W06不可变结果和W09—10并行尚未完成；完整mymod仍仅1流，不能单独证明跨流收益。
后续成果仍集中本分支；[PR #15](https://github.com/wu162/NewSdk/pull/15) 保持草稿，不自行合入。后续每轮90分钟并推进接近90分钟、提前完成一组后继续后续任务；通常2个子代理（最多3个）；只改文档不重编译。
