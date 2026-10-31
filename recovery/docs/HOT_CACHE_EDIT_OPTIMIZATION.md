# 热缓存修改后构建优化

本轮续接 [PR #38](https://github.com/wu162/NewSdk/pull/38)，起点 `d358b2e1`，已验收生产基线仍为 `e2dba3c3`。禁止强推，未经用户明确批准不合并。历史完整证据继续引用[上一轮报告](COMPILER_COLD_HOT_OPTIMIZATION.md)，不重复生成。

## 当前检查点：实现已接入，集中验收运行中

集中源码 `8e8cf766df1016c6095158b62ea18f48a5c37cfe`，[Windows 41174952443](https://github.com/wu162/NewSdk/actions/runs/41174952443)。后续文档提交不代表新二进制。修改仅涉及五份 Managed Core 文件，SDK、原生/Bridge 插件、生成代码和构建输入未改；此前有效的独立插件组件证据沿用，Core 集成与真实 SDK 另取本次新结果。

- `instance` Include 的文件内容依赖与 XInclude/实际素材依赖分开。父文档仍正常处理和完整验证，子文档按实际父实例的最终 XML/类型/处理哈希、物理来源、实际祖先闭包及文件绑定和内容判断继承失效；物理 Include 路径变化仍重载。旧 v18 会话没有来源标记的继承文档回退重解析一次，不删除模糊旧依赖；新写 v19，让旧二进制拒绝窄依赖格式。
- 请求内按需保存成功父实例的原验证节点，独立于跨流清理的 CurrentState，不持久化。保留 PSVI、默认属性、引用处理阶段；先检查依赖和 Define，再恢复只读继承源。为避免 CloneNode 丢失类型信息，节点仍持有原 OwnerDocument，因此可能保留整份父 DOM；尚无内存收益结论。
- 引用处理改为等序 DOM 遍历，缓存编译 Schema 的引用元数据和显式强引用类型对；同次文档验证复用已解析文件，引用哈希缓冲首次写入时才分配，保留原 capacity 哈希字节。

首轮 D/R 产品构建、12 个既有 Core fixture 和热缓存扩展、FLA、64 worker、Texture、CLI、cache/root/payload/session/BIG 独立组通过，运行整体 failed 保留：三项新探针误用了请求生命周期、Armor 的 BuildCache 资格和 xsi:type 的既有拒绝行为；旧性能探针还要求早已删除的 identity.sha256 阶段。仅修测试，生产输入与十个运行文件保持 `8e8cf766`。首次归档身份及原失败结果见[首轮收据](../evidence/refactor/hot-edit/first-41174952443/archive.json)。

[修复轮 41176937044](https://github.com/wu162/NewSdk/actions/runs/41176937044) 的父快照、引用探针 D/R 均通过；Include 无关兄弟已证明 1 次解析和独立冷控制原字节一致，随后热态探针仍因跨请求复用 DocumentProcessor 留下遍历需求而失败。测试已改成生产采用的每请求新处理器；[41177598230](https://github.com/wu162/NewSdk/actions/runs/41177598230) 仅补 Include 探针、复用已经通过的两项探针，并补此前未执行的 SDK/包/完整 MOD 门槛。整体和编辑性能仍待收口，不能标为完成。

父快照 Release 微型诊断：[原结果](../evidence/refactor/hot-edit/repair-41176937044/Release/CompilerInheritanceSnapshot.json)。同输入、相同三份第一流中间资产，控制从序列化会话读入，候选保留请求内原节点；Armor 不走 BuildCache，两边仅用相同普通中间产物隔离父前端成本。

| 工作 | 无快照控制 | 请求内快照 |
| --- | ---: | ---: |
| XML 解析 / 继承重载 / 后端编译 | 3 / 1 / 1 | 2 / 0 / 1 |
| 父解析 / XSD / 继承合并 / 引用 ms | 0.0909 / 0.0478 / 0.0208 / 0.0044 | 全 0 |
| 单次 detailed wall ms | 17.295 | 13.5745 |

仅 n=1 机制诊断，不作为稳定吞吐或完整 MOD 收益。候选保留 1 份 OwnerDocument（4 节点、15 属性、228 XML 字符），这些是该微型输入的持有对象数量，不是托管内存字节；未测完整 MOD 快照内存开销。`core.xml.inheritance.frontend` 仅覆盖 PrevalidatedLoad 分支，Shallow Load 的父成本按父文档调用帧单独记录，不能把前一计时当全部继承重载成本。只有父依赖/类型/输出证据仍成立，才跳过取 DOM 的额外前端；缺少中间产物时仍按原路径加载。

实例级前端仍按整份修改 XML 处理；本轮不恢复全图 Define 指纹、全树 SHA 扫描或 BIG/SKUDEF 包缓存。无修改纯热态收益继续与日常编辑收益分列。

## 实例级前端复用研究：本轮未实现

现有会话不保存 XML。最小请求内下一步可保存成功的“表达式和继承合并后、XSD 前”实例阶段，跳过未变实例的表达式和合并，仍让整份文档通过完整 XSD。面向独立 CLI 编辑，还需持久原始实例身份和可重放结果；宜先限无继承实例，保存所有实际 Define 读值和表达式写回补丁。现有 `ExpressionEvaluator.LookupDefine` 只登记来自其他文档的使用值，不能直接充当实例级读集；未复现多代残留问题，本轮不据此改动原集合。

继承实例还需要合并阶段树或重放记录。最终父节点含绝对文件路径、强引用索引和 TypeId，不能拿它重新执行早期前端；若要复用引用阶段，还需重放字符串哈希登记及 strong/weak/file 元数据，继续清理每流引用索引和 CDATA 路径。现有 SDK 未检出 key/unique/keyref 不能证明可配置 Schema 没有跨实例约束，因此本轮不跳过完整 XSD。大文档中“只改一个实例”仍会处理未变兄弟，这是后续明确范围。
