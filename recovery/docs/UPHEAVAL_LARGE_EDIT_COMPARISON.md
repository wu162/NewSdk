# 完整 Upheaval 大幅编辑的热缓存比较

当前为计量实现检查点，尚未取得运行结果。[完整A/B运行41181143063](https://github.com/wu162/NewSdk/actions/runs/41181143063)，计量源 `a4682714`。生产代码未变，不重建二进制：优化前 `e2dba3c3` / 41169755141，优化后 `8e8cf766` / 41178094135；各自验收来源与运行文件身份按[上轮报告](HOT_CACHE_EDIT_OPTIMIZATION.md)复用。分支仍为 `perf/compiler-cold-hot-cache` / [PR #38](https://github.com/wu162/NewSdk/pull/38)，未经用户批准不合并，禁止强推。

## 输入和场景

完整历史 Upheaval：源提交 `bacfc69fc3e71c8116d1f22b933b8b6173170ecf`，树 `5551fc26a6f96b62613443e0d0fa2958c14cbce0`。所有原始 MOD 输入保留，不裁剪，不加大规模 Armor。每个场景独立从完整 MOD 准备，修改不在场景间累积。

| 组 | 常见开发情况 | 修改范围 |
| --- | --- | --- |
| A | GameObject批量平衡 | 27文件、32实例、121个根属性/生命/移动等字段 |
| A | Weapon/模块批改 | 20文件、82实例、157个射程/速度/DamageNugget等字段 |
| A | 公共父有效字段 | 真实SDK BaseSoundEffect父字段；发布闭包301个AudioEvent，Reverb/Zoom有效影响301、MinRange影响294 |
| A | 公共父无关兄弟 | 同文件BaseQuickChatDialog，本MOD实际继承闭包不使用 |
| B | 公共Define批改 | SDK父库原生Define，实际语义使用26/10/0次，第三项未使用 |
| B | 资产增删、重命名和引用更新 | 20个有语义引用的原生Weapon重命名；明确补充的8资产文件真删除、8资产新文件真添加，更新root Include及所有完整标识符引用 |
| B | 外部素材批改 | 实际输入闭包中64 WAV与16 DDS，保留合法头/大小 |
| B | W3X动画批改 | 32个真实动画文档FrameRate |

原MOD没有自己的Define。三个父/Define场景在预热前将真实SDK父文件精确复制至MOD的DATA搜索根，不写仓库SDK；这是明确的库本地覆盖，不冒充原MOD原生文件。生命周期场景的8个可删除克隆在预热前明确补充，其余原MOD仍完整保留。每个修改的实际文件、实例、字段、引用/继承影响及same-length/same-mtime适用性由输入与mutation收据记录。

## 计量方法

使用现有注册入口 compiler-cache-benchmark 的 upheaval-edits 模式。A/B两组可并行使用不同VM；每个场景的基线/候选始终同VM、同输入、固定jobs4，不能跨组比较绝对时长。

每场景只为每个实现构建一次完整预热代，随后保存不可变的完整输出/会话/中间状态。三组off配对和单独一组detailed归因前，恢复输入与缓存到同一绝对路径，并先执行未计入编辑耗时的restore-check，要求0解析/0编译；允许输入恢复后的ChangeTime引起必要文件哈希读取。再测hot-before、modified、hot-after，hot均须0解析/0编译。

缓存快照复制、输入编辑和发布比较在进程计时外。编译器仍按原行为每次正常打包BIG/SKUDEF，没有新增生产包缓存。每次新进程，OS页缓存未控；off与detailed分开报告。比较所有实际published文件原字节，包括BIG/SKUDEF/Additional；同修改代两实现与各trial必须一致，有效修改必须改变实际发布，无关兄弟必须保持原发布。

独立场景集中收集失败；任何失败或不足3个off配对的场景不标完成。保留wall/report耗时、XML解析、继承重载、后端编译、相关阶段、原始较慢样本和来源身份。生产闭包与已验收产物不变，适用的既有生产/SDK验收复用；本轮只新增完整真实负载的编辑性能证据。

本地九项廉价预检及完整MOD八场景prepare/mutate/XML解析/原字节与mtime恢复通过，不能替代真实编译。`core.xml.inheritance.frontend`与`dependency.inheritance.sourceIdentityBuilds`是候选新增归因项，基线没有该指标，不能将缺失视作零。继承重载的双侧比较使用已有`cache.reload.inheritanceTarget`。计量源便利字段误写`core.output.sort`，已改正为`core.output.order`；最终归因从原始report的正确entry读取，不因此重跑。
