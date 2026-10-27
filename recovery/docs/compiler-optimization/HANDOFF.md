# 编译器优化：当前交接

更新：2026-10-27，北京时间；第三轮15:56开始，按用户要求推进接近90分钟，2个子代理。
成果分支：`feature/compiler-optimization`；不强推、不自行合并，PR #15保持草稿。
每轮90分钟，提前完成一组时继续后续已授权任务；编译/测试/微基准仅用Actions。
方案与实际边界：[README.md](README.md)；准确哈希、失败与测量：[VALIDATION.md](VALIDATION.md)。

## 最新状态：硬链接别名热修已推，完整重验运行中

- W03正内容：有界PhysicalInputSnapshot封存字节+SHA256+原1MiB FastHash，XML/XInclude/schema读同内容，已登记native文件资源由文件/父目录pin保护原路径。provider Dispose后发布前强内容/身份复核，pins保留至事务及必需sink完成。预算/路径/opaque回退明确；仍保留/bc /sc全根复核，未全封闭precompiled/reference manifest、Texture隐藏读取和相关查询。
- 失败Compile结束整个封存generation，原same-backend修复重试恢复，新代强比Initialize配置SHA。取消在schema/XInclude原样传播，不转ReadingSchema/xi:fallback；真实CLI/BIG CTRL_BREAK exit6已通过。
- W05子组：有界resolver有序候选正/负影子证据，保留旧每次File.Exists、顺序、选择与错误；不额外探测、不复用、不跳工作，canAuthorizeReuse=false。Define/继承/符号消费边尚未接入。
- W08子组：base manifest有序重复队列索引+Fenwick即时秩移除，patched首项、重复、异常部分状态保留；实际trace回调后永久legacy。小组/派生/非法/别名列表回退，计量不含List2内部搬移。W01旧修复、W02计量、W04自有元数据等均保留并回归。

## 既有验收身份及新补测

生产/全套测试/打包源码 `68f38abb1ff649857ff786aa8a6bcf657553b8da` / [40476134103](https://github.com/wu162/NewSdk/actions/runs/40476134103) 整体success：Debug/Release生产、各17个Core fixtures、原DLL/原BAB、协议、Texture、实际CLI、全部缓存/输入根、完整mymod/BIG及独立五文件ZIP均通过。
Targeted artifact `12720638360`、完整artifact `12720508892` 已下载校验SHA和source/run；ZIP与manifest一致。先前40472165263/40472198342/40472999771失败原样保留，原因与修复见VALIDATION，不降低断言换通过。

补测test源码 `55f1a519c8a140f5c2988c138f15bc8854d68905` / [40479104375](https://github.com/wu162/NewSdk/actions/runs/40479104375) 严格复用68：双配置16/17，W08新增派生Equals边界40例/147检查通过；硬链接alias可以unlink的真实失败保留。第一次补测4a67f452/40478217177因两个fixture编译问题失败，均未改生产断言换通过。

**当前生产源码 `c65761fed6eed5ad71512e1cea2271eb49a4163f` / [完整重建40479845571](https://github.com/wu162/NewSdk/actions/runs/40479845571) 仍在运行，不能标通过。** 热修保留每个alias路径自己的handle，共享内容/SHA不变；新增activeAliasPins/activeReadHandles，旧activeFilePins为unique内容owner数。68的第一inode handle只能阻写，不能阻另一hardlink名称删除，故68完整成功不覆盖该边界，也不能用于接受当前热修。
下一轮首项：跟进40479845571到终态，核对hardlink unlink必须被阻断、同内容不同inode不误合并、全部17个fixtures与CLI/缓存/完整mymod/BIG/ZIP；失败集中修复。生产已变，固定68的targeted复用必须拒绝当前head，勿绕过closure检查。

## 已测收益与剩余工作

真实三流每配置822次正内容capture/1,529,741字节、1633次sealed hits，发布前822次复核；bypass/cleanupFailures均0。schema仍逻辑消费2445次并Compile3次，后续消费来自封存字节；不是物理磁盘计数。W04仍1次元数据构建+2次恢复。
W08组件512倒序组旧candidate/removal扫描各131,328次变为0，512次索引查找+5121次秩操作；默认/tl3回调路径保守，不能据此报告完整CLI/排序提速。business的48次直接相对路径resolve原本没有candidate Exists，负查询另由专用用例验证。
下一轮优先接入尚未封闭的precompiled/reference内容及resolver候选一致性，再扩展W05实际Define/继承/符号消费轨迹；影子证据未放行精确缓存跳过。W06不可变结果、W09—10并行尚未完成；完整mymod仍仅1流，不泛称全游戏/G3/中文Texture新验收。
固定targeted复用归档仅适用于68f38abb之后明确允许的测试/文档变更；新生产输入变动必须重新构建验收，不能沿用归档。
