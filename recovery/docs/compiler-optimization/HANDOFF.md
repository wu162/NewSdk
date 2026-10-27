# 编译器优化：当前交接

更新：2026-10-27，北京时间；第四轮17:39开始，用户要求每轮最多120分钟、尽量跑满持续推进，通常2个子代理（本轮2个）；效率优先，正确性是底线。
成果集中`feature/compiler-optimization`；不强推、不自行合并，[PR #15](https://github.com/wu162/NewSdk/pull/15)保持草稿。全部编译/测试/微基准仅用GitHub Actions。
方案与边界：[README.md](README.md)；准确哈希、计量与保留失败：[VALIDATION.md](VALIDATION.md)。

## 最新：全部生产验收、严格复用及原ZIP导出成功

- 生产/首次验收测试/打包源码 `60abfeff415e8b1c5bc0b26bf1457f13754c6d75` / [完整运行40491888271](https://github.com/wu162/NewSdk/actions/runs/40491888271) 整体success。双配置各22个Core fixtures、原DLL/原BAB、协议、Texture、实际CLI、全部缓存/输入根、完整mymod/BIG及独立五文件ZIP全部通过。
- 最新工具/探针源码 `443dc88152606a5a41475ff0a5820fc958e1a003` / [40494473681](https://github.com/wu162/NewSdk/actions/runs/40494473681) 整体success：仅允许的测试/工具/文档变更，精确复用上述60的production/validation对象，双配置22 fixtures及26组控制器合同通过。binarySource=60，testSource=443，sourceRun=40491888271；后续文档头不冒充二进制来源。
- 已下载核验本轮targeted/CLI/复用证据，以及原Debug/Release ZIP的outer/inner SHA、source/run、manifest和精确五运行文件。小包原ZIP字节不重打包：[Release artifact](https://github.com/wu162/NewSdk/actions/runs/40494473681/artifacts/12730207255) / [Debug artifact](https://github.com/wu162/NewSdk/actions/runs/40494473681/artifacts/12730631821)。完整原archive `12729367281` 已由Actions严格校验，不冒称本地下载了该106MB包。

## 本轮可用增量

- W03：两个Core入口及Utility的root/递归patch manifest、StringHashes消费封存字节；FileReference原路径pins保留。原实际resolver/manifest发现/asset存在查询的布尔结果登记并在内容复核后复查，代内冲突或终验变化拒绝发布。8192路径/1Mi字符预算、unsupported/output根incomplete保留，不作原子快照或完整查询闭包承诺。终验SHA共用64KiB buffer，每个read/EOF保留原取消token；成功完整文件才计reads/bytes。
- W05：有界scalar/string轨迹保存实际Define绑定/值/来源/epoch/read-site、同文档prevalidation或跨文档processed继承、精确/派生符号候选和选择/歧义。canAuthorizeReuse=false，不额外查询、不缓存绑定/值，不以legacy XML hash放行强缓存。
- W06子组：root私有有序绑定/闭包供manifest、排序和资产报告消费，成功Prepare才发布只读标量快照。声明Handle保存/恢复两侧复制exact-base值图，保留存储32bit ids、重复顺序、null及跨字段Handle别名；派生Handle/list整图legacy，并保留运行时provenance跨恢复/再次保存，XML不改。DisableStreamBindingResults/DisableDeclarationHandleSnapshots可回退；未冻结整个LastState/SelfInstances/DOM/Define。
- W08：每DP弱生命周期有界exact表达式程序；首遍原parser实际求值，完整成功后缓存语法，暖路重新读取当前Define和执行原Value/文化转换；失败/超限不缓存。固定en-US只读文化复用可独立回退。264行原SDK DLL、四文化五开关组合、失败前缀、所有预算及真实XML四输出通过。
- W09子组：每DP懒分配并复用copy 1MiB/link 64KiB，原copy锁/短尾块/错误与临时文件合同不变，legacy静态fallback也按需分配。Source相同的default/fallback严格字节差分及大DataBlob三流/session/失败取消重试通过；backend单租约仍在，没有开放并行。

实际业务三流两配置：base三次相同Define为parse1/build1/hit2，APT/maps无表达式仍0；原BAB及独立冷组四输出相等。各流copyAlloc0/linkAlloc1（64KiB），只记owner分配，不外推完整内存峰值。schema仍逐流读取/Compile，构建内自有元数据仍1 build+2 restore。
7轮交错组件微基准见VALIDATION：每500次program共享文化中位约1.05ms、parser共享文化约40ms；只是组件样本，不报告完整CLI提速/P95/统计显著性。

## 下一轮优先与约束

1. W06下一完整组优先处理AssetDeclarationDocument.CacheFromDocument在other._Last存在时仍共享LastState/SelfInstances的生命周期，尤其后续MakeComplete/再次序列化；先用公共Core最小探针确认，不把当前Handle子组外推为完整不可变AssetSemanticResult。保持默认值/SchemaInfo、顺序、错误及冷热/持久session四输出。Prepare只读快照不是事务接受凭据，后续manifest/link失败仍可保留诊断快照；null仅保留原in-memory行为。
2. 补完整BuildSession真实多流precompiled/patch/StringHashes候选变化→整次发布拒绝/回滚/pins释放用例（当前manifest以真实Core/直接Verify为主）。BinaryAsset缓存/物化读取、Texture隐藏资源与其它集合查询仍保守；保留/bc /sc全根校验、Texture缓存拒绝和原字节/失败门槛。W07完整bundle缓存与整个W09—10状态/插件/内存并行合同未完成，不提前放开worker或精确缓存跳过。
3. 严格targeted复用现固定60abfeff/40491888271，精确22个探针与工具/Markdown白名单；源码/SDK/recipe、删除/rename、脏/含ignored的未跟踪生产输入拒绝。新生产变化必须完整重建，新成功后再更新pin；旧68不再是当前复用入口。

本轮失败与修复留在VALIDATION：新增Utility精确签名审计、manifest测试配置、协议Source标签、不同AssetHeader padding输入，以及derived-list恢复后回退来源丢失。未删断言换通过。完整mymod仍仅1流，不泛称全游戏/G3/中文Texture或游戏内新验收。
