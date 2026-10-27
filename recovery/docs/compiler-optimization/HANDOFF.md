# 编译器优化：当前短交接

更新：2026-10-27（北京时间）。成果集中 `feature/compiler-optimization`；[PR #15](https://github.com/wu162/NewSdk/pull/15) 保持草稿，不强推、不自行合并。通常 2 个子代理、最多 3 个，编译/测试/微基准全部 Actions。导航：[README](README.md)；实现边界：[CONTRACTS](CONTRACTS.md)；完整证据：[VALIDATION](VALIDATION.md)。

2026-10-28 续接检查点：当前分支有待新 Actions 验证的继承执行状态/栈安全、外部 cache header 游标与网络 cache 查询终验、真实 CLI 交错计量准备。下列 `64bf/40524897974` 仍是最近完整接受点，不覆盖新生产修改；完整语义结果、Define 环境冻结、Texture 隐藏闭包和强缓存命中均未因此完成。

## 最新接受点与下载

- **生产、首次完整验收及打包：`64bfcc80cec7860e69f0ffbc5cb0e04c69baec87` / [40524897974](https://github.com/wu162/NewSdk/actions/runs/40524897974)，整体 success。** 双配置各 30 fixtures、原 DLL/三组独立原 BAB、协议、Texture、实际 CLI、全部缓存/输入根、完整 mymod/BIG 和独立五文件 ZIP 通过。
- 当前严格复用pin为上述64bf/40524897974；工具/测试 **`2bd197f64b5ef1bd9476ac4198e1c3934ed13897` / [40528794946](https://github.com/wu162/NewSdk/actions/runs/40528794946)整体success**，双配置30/30与独立原BAB再验通过。生产对象未重建。
- 原包下载：[Release](https://github.com/wu162/NewSdk/actions/runs/40528794946/artifacts/12746065549) / [Debug](https://github.com/wu162/NewSdk/actions/runs/40528794946/artifacts/12746445163)。ZIP 不重打包；binarySource、packagingSource、testSource 和精确五成员 SHA 分别记录在 VALIDATION，文档头不是二进制来源。

## 本轮完成

| 能力 | 当前可用范围 |
| --- | --- |
| W03 Cache 物化 | header 身份/声明长度合格后 tee 原 copy chunks，规范 1 MiB legacy 摘要与 SHA 同流；完整成功后才 pin 并核对 native identity。后续来源读复用封存游标，取消、坏输入删除/修复/重试和失败回滚保留。Local 永久原路径，App staging cache 继续排除。 |
| 缓存触碰 | 只对已捕获来源用 identity 复核的 attribute-only handle 更新 access/write/creation；内容写和删除仍被 pins 拒绝。未捕获/只读来源保持原 FileInfo 路径。 |
| W08 表达式遍历 | exact Framework DOM 使用 live sibling 遍历，单 text 属性读值一次；custom DOM/getter、错误顺序、ordinal/Define 轨迹保留。40 组原 DLL DOM corpus 与真实四输出覆盖。 |
| legacy XML hash | 生产维持原 writer；947 行原 DLL 确定性语料保留。缓冲与 ASCII 尝试因 CPU 收益不稳定已撤回，回归和计量记录在 VALIDATION。 |
| W08 符号查找 | 输出绑定内以完整 ID 索引实际 legacy table keys；保留 location/skip/派生查找顺序与全部影子事件。custom/预算及外部边界永久回退，scope 不逃逸。 |
| W08 schema 前序查询 | 单次插入内的有界标量集合，下一次插入重建以观察 schema/DOM 回调；owner 报告实际 index/linear 工作。 |
| W06 saved metadata | positive ownership 才共享私有 Define/include/output/UsedDefine 图，Current 深复制、动态 legacy detach、派生/未知序列化回退。当前 `CacheFromDocument` 无生产调用，不报告 CLI 收益。 |

开关与预算详见 CONTRACTS。保留 Win32/.NET4.8、ABI、legacy 摘要/输出顺序、缓存默认关闭和 Texture 会话缓存拒绝；未开放并行。

## 计量与保留失败

同run交错7轮的Release公共组件中位数（优化/回退）：表达式100次遍历9.9147/13.5792ms；宽schema完整merge4.8350/31.3033ms；8192次符号查询1.6288/1.9668ms。公开saved metadata传递0.0052/0.3770ms只作API组件计量。真实外部Cache三流：tee1次/1048692字节、额外asset读取0、游标命中2、属性触碰3/失败0；最终小证据artifact12743682844已核SHA和30/30来源。

这是组件和逻辑工作计量，未取得完整 CLI 前后耗时对照，不报告端到端提速。原 DLL 原始 ANSI wrapper 的多 pair 收缩/未初始化读取不作为确定性 Unicode oracle；具体安全语料边界在 CONTRACTS。

初版 hash 缓冲的 CPU 回归、TouchCache 在有效 pin 下失败、owner probe 预检/初始化错误及相应 Actions 原终态均保留在 VALIDATION；没有删断言换通过。本轮使用2个子代理，10个完整候选中4个success/6个failure；原准备错误、真实缓存交互修复与哈希撤回均见VALIDATION。

## 下一轮

1. 优先推进完整 prevalidation/processed XML 与 Define 环境的语义结果边界，先证明 SchemaInfo、默认值、阶段、来源和错误；不能用 CloneNode 或 legacy XML hash 直接授权强缓存。
2. 完成 BinaryAsset 其它可变读取、网络 cache 查询与 Texture 隐藏资源闭包。当前 `completeClosure=false`，同流封存只覆盖已验证 Cache copy，不覆盖 App staging 或全部物化读取。
3. W07 新完整 bundle 缓存、W09–10 插件/状态/资源合同与并行仍未完成；保留 Core、原字节、冷热/持久缓存、失败取消/事务、完整 mymod/BIG 和独立 ZIP 门槛。
4. 后续测试/工具/文档变化优先严格复用当前接受对象；生产变化重新成组构建。继续普通推送到本分支，合入须用户批准。详细历史只在验证归档和 Git 保存，不向入口重复追加历轮流水。
