# 编译器优化：验收与产物身份

更新：2026-10-28（北京时间）。编译、测试和微基准全部由 GitHub Actions 执行；本地只编辑、静态检查与读取证据。当前合同见 [CONTRACTS](CONTRACTS.md)，续接见 [HANDOFF](HANDOFF.md)。历史完整证据分别在[前五轮](history/VALIDATION_ROUNDS_1_5.md)和[第六轮](history/VALIDATION_ROUND_6.md)。

## 当前完整接受点

| 身份 | 值 |
| --- | --- |
| 生产 / 首次测试 / 打包源码 | `71b5a0a4f55e530eccc9703eff2f3c3a397dd910` |
| 完整运行 | [40598874896](https://github.com/wu162/NewSdk/actions/runs/40598874896)，attempt 1，整体 success |
| 范围 | Debug/Release各30组、原DLL/独立原BAB、协议、Texture、实际CLI/cache/输入根、完整mymod/BIG与五文件ZIP |
| 完整 archive | [12777973896](https://github.com/wu162/NewSdk/actions/runs/40598874896/artifacts/12777973896)，112309278字节，官方SHA256 `23bc26828a430609fa27ca1555d63c11376b72ff082239707515eac5f172f569`；未本地下载 |
| 已核Core小证据 | `12777902114`，592789字节，SHA256 `07a6c485be53aec60a682dbd0486bd3ff8d08771bb21ce2eadb9424bd7bcd8e5`，本地/官方一致；双配置30/30、9继承与6reader原DLL行已读 |
| Release独立transport | [12778163405](https://github.com/wu162/NewSdk/actions/runs/40598874896/artifacts/12778163405)，1846541字节，官方SHA256 `1cbd6bb9df5d3f83c2430187f09acc5d77e1beab4d535949626265bfe5d76d12`；未本地下载 |
| Debug独立transport | [12778053643](https://github.com/wu162/NewSdk/actions/runs/40598874896/artifacts/12778053643)，2301728字节，官方SHA256 `a11d87e2656666063b5a5435d2a87b1c5a44fb7c0fa0b5af723cda058d510619`；未本地下载 |

独立artifact保存原ZIP、manifest和release.json，没有重打包内层运行包。此前`15bcd14c/40596715818` Release下载已核官方transport SHA `29b346247ad7a524f119f48053efbe17155e0d00636ed1268f3e92cff90ace81`、原ZIP1790608字节/SHA `9959a408be687dd0d2bc7441b07e901c862ff5fb691d1209c02bece98a580961`、精确五成员及Texture→EXE绑定；该历史下载不能冒充当前包本地核验。

## 本轮完整能力组

| 生产 / 首次测试源码 | Actions | 终态与范围 |
| --- | --- | --- |
| `5727b05b234c130a5c26e5c465a31f8ff42baf22` | [40593235622](https://github.com/wu162/NewSdk/actions/runs/40593235622) | **success**；live继承状态/栈安全、cache header与实际网络cache查询/自身写删、真实CLI成对计量 |
| `c3ef5344115eaeafc15979c70857cc4557e89acf` | [40594750754](https://github.com/wu162/NewSdk/actions/runs/40594750754) | **success**；case/junction别名证明撤销、9行独立原DLL继承差分 |
| `07ff6a3b13b813af8d2985569e16e89e045e0e1c` | [40595750142](https://github.com/wu162/NewSdk/actions/runs/40595750142) | **success**；APM摘要读取所有权、实际SHA256消费者故障/同item重试 |
| `15bcd14c97c475d6922cb367989a4fc04d8ba7bb` | [40596715818](https://github.com/wu162/NewSdk/actions/runs/40596715818) | **success**；Texture/Builtin owner-thread、保留原Texture transcript、24次live错误线程拒绝和关闭后合同 |
| `71b5a0a4f55e530eccc9703eff2f3c3a397dd910` | [40598874896](https://github.com/wu162/NewSdk/actions/runs/40598874896) | **success**；实际注册路由immutable能力标量，缓存只否决、session拒绝与opaque跟踪消费；6行独立原reader/digest语料 |
| `e4169602d875d213e5763a6b8fdc83973409fd61` | [40600682680](https://github.com/wu162/NewSdk/actions/runs/40600682680) | **Core失败，整批不接受**；双配置各28组成功，PhysicalInputs失败DOM根空引用、ExpressionPrograms未真正arm修改事件；修复进入下组，保留原运行 |

| `6db109aff316e21258485f49eaf361d96c790467` | [40602153873](https://github.com/wu162/NewSdk/actions/runs/40602153873) | **验证中**；修复真实NodeChanged/NodeInserted事件arm、失败DOM探针，text资源所有权、3行typed DOM原语；原DLL运行前置预检 |

前五组完整通过。第六组保留失败小证据`12778733006`，604072字节，SHA256 `f017f07615d3466a4fbd7b3f35a69f5e3838bef929303b7833fd4eee369e2e30`本地/官方一致；摘要取消/Chunk池及其余28组成功不等于整批接受。失败DOM的三文件立即exclusive、原XmlFormatting包装与同document修复重读在NRE之前已执行成功，失败发生在旧探针对已被Framework清空的DocumentElement解引用。

Define失败暴露的是未arm的fixture：原DLL52行changed=false，标准单text属性改值实际发NodeChanged。旧40 traversal中16个live-sibling修改行也未arm；它们此前不能作为回调修改覆盖证据。第七组同时监听两种事件，并在原/恢复两端强制修改实际发生，保留全量严格差分和EOF版本断言。不能把修正fixture当作放宽生产门槛；新证明待Actions。

严格复用仍pin第六轮对象；最终接受点形成后更新。运行中/失败对象不作为完整接受源。

## 新生产能力和证据边界

- 继承：显式 DFS、reference active 集与 live DOM readiness，拒绝环、避免合法深链消耗 CLR 栈；同文档 prevalidation 与跨文档 processed 来源仍按原顺序消费。4096 前向链、零 digest 注入、同处理器修复重试、SchemaInfo/default provenance、重复调用和 custom WriteTo 主错误均通过。完整不可变 AssetSemanticResult 与 Define 环境冻结尚未完成。
- Cache：实际存在查询有独立预算与终验；成功本进程写删推进预期，已有外部冲突不清除；实际开始而失败的可选写撤销该路径查询证明并报告 incomplete。真实 native DataBlob 同代 cold→warm→warm 的四输出及封存生命周期通过。Local/Output/staging 仍保持原角色。
- Header：只复用已经成功封存的 Cache 来源，不提前 pin 坏文件；无效长度仍删除并拒绝。取消继续经过真实 Commit 传播。
- 已接受 alias 组不把大小写路径直接合并；相同 leaf 的候选采用有界 native parent 查询或保守证明撤销。失败/预算回退明确 incomplete，不授权强缓存；完整生产验收已通过。
- 已接受 reader 组为提前退出/摘要异常收尾，不是正常路径 APM 事件泄漏修复。Framework EndRead 原本已关闭内部等待事件；public Dispose 等待 pending read，不承诺取消 OS 读取。真实 SHA256 HashCore 故障与同 item 重试已通过双配置完整 Actions。

## 实际 CLI 成对计量

`5727b05b / 40593235622`，同 Release EXE、同三流小 business 输入、cache 关闭、新进程/输出目录；两模式各一次不计样本的 warmup，再交错7对。全部16次的十二个输出严格字节相同。

| 中位数 ms | 默认 | 诊断性能回退 |
| --- | --- | --- |
| 进程 wall | 1255.3308 | 1234.7419 |
| 进程 CPU | 1218.75 | 1187.5 |
| controller wall | 1267 | 1255 |
| report elapsed | 1146 | 1124 |

未观察到这份小用例端到端提速，不把局部组件收益外推。CPU 有约15.625ms粒度；共享 runner、少量样本和首次 JIT 都限制推断，不宣称统计显著回归。fallback 仍保留 snapshot、绑定和生命周期正确性修复，不是旧版本整体 before/after；进程 wall 含短暂 suspended admission，controller 在 Resume 后开始，report 在必需输出完成前截断。原始样本、每 task/stage 工作量和配置 SHA 都在 `Release/business-cli-paired.json`；嵌套区间不可相加求总 wall。

## 继续和剩余门槛

成果仍在 `feature/compiler-optimization`，[PR #15](https://github.com/wu162/NewSdk/pull/15) 保持草稿，不自行合并、不强推。完整语义结果/Define 冻结、Texture 隐藏读取闭包、新强键 bundle 缓存和并行尚未完成。原字节、失败/取消/事务、冷热持久缓存、完整 mymod/BIG 和独立 ZIP 门槛均保留。
