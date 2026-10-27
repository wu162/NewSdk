# 编译器优化：验收与产物身份

更新：2026-10-28（北京时间）。编译、测试和微基准全部由 GitHub Actions 执行；本地只编辑、静态检查与读取证据。当前合同见 [CONTRACTS](CONTRACTS.md)，续接见 [HANDOFF](HANDOFF.md)。历史完整证据分别在[前五轮](history/VALIDATION_ROUNDS_1_5.md)和[第六轮](history/VALIDATION_ROUND_6.md)。

## 当前完整接受点

| 身份 | 值 |
| --- | --- |
| 生产 / 首次测试 / 打包源码 | `5727b05b234c130a5c26e5c465a31f8ff42baf22` |
| 完整运行 | [40593235622](https://github.com/wu162/NewSdk/actions/runs/40593235622)，attempt 1，整体 success |
| 范围 | Debug/Release 各 30 组 Core、原 DLL、三组独立原 BAB、协议、Texture、实际 CLI 与取消、全部缓存/输入根、完整 mymod、BIG 和独立五文件 ZIP |
| 完整 archive | [12776875507](https://github.com/wu162/NewSdk/actions/runs/40593235622/artifacts/12776875507)，111911680 字节，官方 SHA256 `608c56d261d27bb625bd7578b828b3e091a04609b4ddb7e5850250819276663d`；未本地下载 |
| 已核验早期 Core evidence | artifact `12774869020`，554000 字节，SHA256 `83d80d109d1a371d6006d2917b3b63684976fc9fd9727d2795d4c8115bb9ffdd`，本地下载与官方一致，source/run 及双配置各30组成功收据已读 |
| 已核验 CLI evidence | artifact `12775488351`，512168 字节，SHA256 `7b0c3fd23876534b5b256757ef2ecc2dfed8fac95bf0f8052f93215ed063ea40`，本地下载与官方一致，实际成对计量已读 |

此接受点覆盖继承阶段状态/栈安全、外部 Cache header 复用、网络 Cache 查询与本进程可修复写删、可选写失败回退和实际 CLI 成对计量；不覆盖下面的后续生产变化。构建和测试没有用旧二进制替代新生产。

## 后续候选（仍在验证）

| 生产 / 测试源码 | Actions | 范围与当前状态 |
| --- | --- | --- |
| `c3ef5344115eaeafc15979c70857cc4557e89acf` | [40594750754](https://github.com/wu162/NewSdk/actions/runs/40594750754) | cache case/junction 别名证明撤销与9行独立原 DLL 继承差分；双配置 Core、原 DLL/BAB、Texture、CLI 已通过，完整运行尚未终态 |
| `07ff6a3b13b813af8d2985569e16e89e045e0e1c` | [40595750142](https://github.com/wu162/NewSdk/actions/runs/40595750142) | APM 摘要读取器资源所有权；Foundation 和前置探针编译通过，完整运行尚未终态 |

Texture/Builtin 活跃入口 owner-thread 合同组待提交/验证。任何运行中对象不标为完整接受，不把后续工具/文档提交写成二进制来源。旧严格复用控制器仍 pin 第六轮对象；它不能覆盖本轮生产变化，待最终接受点形成后集中更新 pin。

## 新生产能力和证据边界

- 继承：显式 DFS、reference active 集与 live DOM readiness，拒绝环、避免合法深链消耗 CLR 栈；同文档 prevalidation 与跨文档 processed 来源仍按原顺序消费。4096 前向链、零 digest 注入、同处理器修复重试、SchemaInfo/default provenance、重复调用和 custom WriteTo 主错误均通过。完整不可变 AssetSemanticResult 与 Define 环境冻结尚未完成。
- Cache：实际存在查询有独立预算与终验；成功本进程写删推进预期，已有外部冲突不清除；实际开始而失败的可选写撤销该路径查询证明并报告 incomplete。真实 native DataBlob 同代 cold→warm→warm 的四输出及封存生命周期通过。Local/Output/staging 仍保持原角色。
- Header：只复用已经成功封存的 Cache 来源，不提前 pin 坏文件；无效长度仍删除并拒绝。取消继续经过真实 Commit 传播。
- 后续 alias 组不把大小写路径直接合并；相同 leaf 的候选采用有界 native parent 查询或保守证明撤销。失败/预算回退明确 incomplete，不授权强缓存；需要后续完整成功才能覆盖。
- 后续 reader 组为提前退出/摘要异常收尾，不是正常路径 APM 事件泄漏修复。Framework EndRead 原本已关闭内部等待事件；public Dispose 等待 pending read，不承诺取消 OS 读取。新增真实 SHA256 HashCore 故障与同 item 重试待 Actions。

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
