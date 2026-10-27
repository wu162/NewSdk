# 阶段 A 验证记录

所有编译、测试和微基准只在 GitHub Actions 执行。本地只编辑、静态检查和读取 Actions 证据。产品来源和文档提交分开记录，不以文档分支头冒充二进制来源。

| 生产/测试源码 | Actions | 状态与范围 |
| --- | --- | --- |
| `18ae05cace7099bbd65fa6632dcfd7dd383bdded` | [40459453132](https://github.com/wu162/NewSdk/actions/runs/40459453132) | 整体 failure；完整生产、摘要、既有 Core、Texture、CLI、输入根、各缓存和 BIG 独立组通过。新 NodeJoiner 差分本身通过，但原 DLL 进入宿主触发归属检查，导致完整 mymod/最终 ZIP 未执行。失败不改写。 |
| `75601909b0e874bc24786f1d6dd3d8c64eb1aee6` | [40461583024](https://github.com/wu162/NewSdk/actions/runs/40461583024) | 整体 success：完整生产、两配置各13个 Core fixtures、原 BAB、协议、Texture、CLI、全部缓存、输入根、完整 mymod 四文件/实际 BIG 原工具差分和最终五文件 ZIP 独立解压运行验收通过。 |

首轮证据 artifact：`12714925627`，SHA256 `9672e7e862791bac1ecc472c4e0efdadf1055f3d029405b83cfb798109fae593`。风险观察来自其中 Debug/Release `CompilerCoreLifecycle/result.json`，两配置一致。

第二轮提前上传探针 artifact：`12714883356`，SHA256 `47ada239c76a49c849c84a1f91e4b71525e09dabbba710ed3defe5e931f65e38`。收据 succeeded=true；两配置 W01 严格回归全部 equivalent、未变化 override 解析0；NodeJoiner 原 DLL / 默认索引 / 禁用索引差分与归属门槛通过；schema、摘要及生产报告集成通过。

完整成功证据 artifact：`12715561263`（`refactor-core-lifecycle-40461583024-1`），SHA256 `c7e29041e529b068219e4d720e41192becbc61054e8d39f32a51bd98eb5a1dbc`。`artifacts/final-release/40461583024-1/` 含 Debug/Release ZIP；两份 manifest 的 binarySourceCommit 与 packagingSourceCommit 均为上表成功源码。后续文档提交不改变该二进制来源。

| ZIP | SHA256 |
| --- | --- |
| NewModCompiler-Win32-Debug.zip | `7a3c970475cb2f4ff3c195c8e61189d44b2446aee89f0b6dc0ce536a9bd0c0e8` |
| NewModCompiler-Win32-Release.zip | `e494ac0f42f6fc05c963e67790fa7609c9642f5d753d2283a688f70356c4f6f2` |

## 确认的问题与有意修复

这些是在公共 Core 边界确认的结果；不将其扩大为已确认的 CLI 故障。外层 CLI 使用独立流工作区与整输入缓存身份，单独验收。

| 探针 | 首轮真实结果 | 新修复及严格门槛 |
| --- | --- | --- |
| 外部 A=10，父级 `A=$A`，外部改20 | 暖恢复仍为10，冷构建为20，四文件不一致 | 按本地定义安装前的导入值验证 UsedDefines；冷暖值和四文件一致。 |
| 父级 `A=$A+1`，输入未变 | 暖输出正确，但重复解析1份文档 | 同上；未变暖路径解析0，外部改20时结果21。 |
| 两流共享弱引用，第二流新增 Tentative Upgrade | 暖流仅1资产，冷流2资产 | 每流从声明引用重建绑定和闭包；第二流包含 Upgrade，与冷流四文件一致。 |
| 保留有效 .bin，分别删除/截断 .imp/.relo | 六种损坏均恢复为空payload，原payload各8字节 | 按当前有序资产集合验证三文件 checksum 与预期长度，任一损坏重建整组；六种情况均与完整冷构建相同。 |

链接长度/头检查不等于强 payload 校验：长度和 checksum 头均保持时的任意 payload 篡改仍须后续完整 LinkPlan/CAS 凭据。绑定修复针对串行 Core，不代表已完成 W06 不可变结果或并行安全。

## 优化证据与边界

- NodeJoiner 最终80个原 SDK DLL XML/错误差分及卸载 AppDomain 后的宿主 ownership 门槛通过；该 fixture 累计 destination Attributes 访问从549,919降到13,952，禁用索引恢复549,919。此为匹配工作量，不是构建耗时。
- 首轮对4,194,335字节输入交错5次测量，OS页缓存状态 unknown。全量双摘要暖路径中位数：Debug 6.065ms 对 SHA-only 4.849ms；Release 5.754ms 对4.692ms。因额外 CPU 成本，最终策略保留可能未变输入的 SHA-only 检查。
- 首轮冷摘要模拟遗漏历史第二遍中的 SHA256，其耗时不作为最终比较数据。后续模拟已补齐，与新文件、普通变更、暖输入和同时间戳内容变化的正确性探针一起重验。
- schema 描述以当前成功加载代、声明对象和实际类型为边界；有 fallback、上下文隔离、重载失败和同时间同大小内容变化探针。尚无跨流共享 SchemaSnapshot。
- 暂无完整编译器前后 wall-time 对照，不报告端到端提速百分比。阶段计量不包含所有解析、schema 准备和发布成本；计数范围见 README。

第二轮修正后摘要微基准，同一 runner/进程内交错5次，输入4,194,335字节，OS页缓存 unknown。单位 ms，数值为中位数 [最小, 最大]；新文件指新的 FileHashItem 观察，不代表冷磁盘。

| 配置 | 历史新文件双遍 | 新策略新文件单遍 | 历史暖 SHA-only | 新策略暖 SHA-only |
| --- | --- | --- | --- | --- |
| Debug | 12.765 [12.639,15.268] | 7.113 [7.034,7.173] | 5.602 [5.557,5.703] | 5.549 [5.521,5.589] |
| Release | 11.953 [11.879,12.287] | 6.619 [6.581,7.043] | 5.399 [5.337,6.708] | 5.535 [5.428,5.694] |

逻辑摘要读取：新文件/已知时间变化从2遍减到1遍；暖输入保持1遍；同时间内容变化保留2遍安全重算。暖耗时两配置有小幅双向波动，不将5个样本解释为统计显著性、P95或全项目性能结论。

## 下一轮可用的真实成本入口

成功运行的 `evidence/refactor/sdk-mymod/40461583024-1/validation.json` 保存完整报告。完整 mymod 两配置均为1个 Core 流、11份 XML 解析、47次资产编译，逻辑摘要13次/86,747字节；同代 schema 描述854次构建、6,860次命中。该样本不足以证明跨流复用收益，后续应结合已有多流 CLI business fixture。

| 当前一次运行 | 报告总 wall ms | assetProcessing ms | validation ms | inheritance ms |
| --- | --- | --- | --- | --- |
| Debug | 845 | 202.127 | 63.706 | 15.346 |
| Release | 713 | 162.036 | 56.252 | 13.694 |

这只说明当前运行的成本，不与旧分支或其他 CI 机器相除求提速。八阶段尚未覆盖所有 schema 准备、原始 XML 解析、外层初始化与发布成本，不把差额全部归因给 schema。下一轮先补这些区间，再在多流样本评估 W03/W04 的共享边界。
