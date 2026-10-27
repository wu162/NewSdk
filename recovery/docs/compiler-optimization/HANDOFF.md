# 编译器优化：当前交接

更新：2026-10-27，北京时间；第五轮19:39开始，按用户要求持续推进约110分钟，通常2个子代理（本轮2个）。全部编译、测试与组件微基准仅Actions；本地只编辑、静态检查及核验下载证据。成果集中`feature/compiler-optimization`；不强推、不自行合并，[PR #15](https://github.com/wu162/NewSdk/pull/15)保持草稿。
方案与边界：[README.md](README.md)；完整方案只存PLAN一份；准确计量与保留失败：[VALIDATION.md](VALIDATION.md)。

## 最新接受点

- 生产/首次完整测试/打包源码 **`b934445adb4ae900215ff980cb902f67407e2d5e` / [40507039217](https://github.com/wu162/NewSdk/actions/runs/40507039217) 整体success**。双配置各27个Core fixtures、原DLL、三组独立原BAB、协议、Texture、实际CLI、全部缓存/输入根、完整mymod/BIG与独立五文件ZIP全部通过。
- 当前严格复用pin已更新为上述b934/40507039217；测试/工具源码`e4a3adc3823c20a4a3858440702b3e9cfbf06b22` / [40510206136](https://github.com/wu162/NewSdk/actions/runs/40510206136) 严格复用/原ZIP导出验收已整体success，双配置27/27与Ubuntu/Windows各26项工具合同通过。追加最后trace将SchemaSet置null的公共排序局部合同；生产未变，不重建生产。只允许精确27 fixture、明确工具/小样例/Markdown路径，生产/SDK/recipe变化仍必须完整重建。
- 已下载核验本轮27组targeted artifact `12734969954` 与原BAB小证据 `12736365897` 的outer SHA/source/run。六份原BAB收据确认Debug/Release下mod-a/mod-b、Upheaval、98资产链共8次独立进程比较，四输出逐字节相同且输入未变。
- 完整生产archive `12736019142`（108739539字节、GitHub SHA256 `416e58b56c3b377adc5380f671e55f6558bde0c24e921506502a41cca3786dd9`）已由严格复用Actions按digest/size下载核验；本地未下载此完整成功archive，只下载核验原ZIP小artifact的outer/inner SHA、精确五成员及source/run。小包：[Release](https://github.com/wu162/NewSdk/actions/runs/40510206136/artifacts/12736174698) / [Debug](https://github.com/wu162/NewSdk/actions/runs/40510206136/artifacts/12737615880)，不重打包，身份详见VALIDATION。

## 本轮新增可用组

- W06：文档LastState不再复用可变声明wrapper；保存/复制/恢复隔离SelfInstances与Define/include/output记录，保留标量、顺序、Handle别名、原路径及来源引用。恢复Define.Document不写穿其它文档，旧LastOutputAssets列表shape合同在fallback保留。派生声明/InstanceSet与派生metadata图分别整图保守回退；`DisableDocumentSnapshots`恢复旧生命周期。
- W06复制开销：只共享positive ownership证明成立的私有saved声明值（实际exact值复制或XML新建）。wrapper/文档容器仍独立，Current恢复仍复制；未知BinaryFormatter来源无证明。动态旧Handle开关先detach再暴露，防止兄弟缓存写穿。`DisableSharedDocumentDeclarations`单独回退深复制，不恢复旧文档共享错误。未冻结DOM/SchemaInfo/整个Define环境，不称完整AssetSemanticResult或并行安全。
- W08 NodeJoiner：仅id匹配索引，Name始终原线性；实测Prefix setter原/新DLL都不发DOM事件。相关外部结构/id变化后本次merge永久legacy，成功/异常均解除订阅；保留自己的精确增删更新、重复首项、原错误与512热点属性读取门槛。
- W08 StableSort：仅本次同步排序、根私有bindings和exact图/标准字典比较器/文化可使用完整(TypeId,InstanceId) membership索引；小集/预算/派生/custom/legacy或回调变化保守回退。资格检查使用实际comparer已捕获的同一map，保留最后trace替换SchemaSet的旧排序行为。闭包构造、int XOR碰撞错误、schema初次排序及插入顺序不变；`DisableStableSortMembershipIndex`可回退，coreWork新增实际计量，scope不逃逸。
- 真实小组：从固定Upheaval源06864321逐字节取4份完整原XML，加3份明确synthetic Include/Define/跨文档继承适配器，共7资产，无Art/Texture/Audio闭包；新增96项Experience强依赖链+2项weak拉入Upgrade。前置原字节/闭包检查位于长编译之前，Core冷/暖/持久会话与默认/回退四输出、独立原BAB均通过，不替代完整Upheaval或游戏内验收。
- W03事务补测：precompiled/递归patch/StringHashes候选变化在3流全部成功后拒绝整次发布、释放pins/清理及新代重试。显式fixture-only Core配置及consumer注入，未新增CLI业务选项，APT/maps用Armor验证事务编排。

## 实际计量边界

- 32声明×20次公开缓存传递，7轮交错：保留21文档时private saved图共享32、深复制672。Debug中位0.1055/0.6153ms，Release0.1052/0.6211ms；不计XML/反射检查，不外推RAM峰值或完整CLI提速。
- 真实98资产链每根80 builds/4440 entries/4600 indexed+346 small-linear；关闭后0 indexed/4946 linear。64声明实际public StableSort组件样本Debug中位0.4455/0.4708ms、Release0.4318/0.4604ms，含入场/建索引/反射invoke，7轮，无速度断言/P95/统计显著性承诺。
- schema仍逐流读取/Compile，构建内自有元数据共享与此前输入封存/表达式程序/传输缓冲边界保留；缓存默认关闭，Texture限制及原字节/失败门槛不变，未放开并行。

## 保留失败与下一轮

1. 所有本轮失败及原收据在VALIDATION。包括fixture重复tl、匿名payload转换、混合路径查询、裸registry缺生产元数据view、动态schema误声明const；均修真实原因，未删断言换通过。BinaryAsset直接pin尝试因阻断坏缓存修复/删除/重试已完整撤回，只保留原修复回归，不能称.asset/.cdata读取闭包完成。
2. W06后续优先研究完整prevalidation/processed XML与Define环境的结果边界；先公共Core探针，保留SchemaInfo/默认值/阶段/错误与冷热持久四输出，不简单CloneNode或legacy XML hash授权强缓存。
3. W03仍有BinaryAsset可变物化读取、Texture隐藏资源及其它查询闭包；网络cache要先解决原坏输入修复合同，不能在信任前直接pin。W07完整bundle缓存与整个W09—10插件/状态/资源并行合同未完成，不跳过Core、不开放worker。
4. 仅工具/测试/文档变化复用已验证生产对象；生产变化统一重建受影响矩阵，10—15分钟保存普通推送检查点。后续合入仍先PR并经用户明确批准。
5. 第五轮效率复盘为中等：约105分钟、2个子代理，7次完整运行中前5次主要失败于fixture准备。下一轮按EFFICIENCY先集中核对参数/payload/规范路径/生产元数据view/编译条件，能提前做的检查放到Actions长编译前，已知同一错误修复后再发完整候选；两个子代理优先各推进完整实现组，减少可避免的验证返工。组件计量不代表完整CLI已明显提速。
