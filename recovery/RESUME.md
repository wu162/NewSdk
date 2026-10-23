# 已验收85类型；继续Production完整组

第二组11种命名Upgrade已通过：source aed221a9bec343b1d8a67dd2f071c4ddab11e5b4，GameObject run40035310226 success，基础run40035310221 success。每模式168432 fixtures、505296缓冲区比较、9356拒绝、320不变性；/Od、/O2与0/1/2/99完整通过。支持85/322，237明确拒绝；SDK0次。额外边界矩阵在本地补齐，将随下一完整Production组共同验收。

ProductionUpdate与两个出口模块共63原方法已导出run40035493940；此检查点仅新增生成器和源契约，尚未启用这3类型，不得计作88。ProductionModifier的SizedList<...,2>原helper实际直接调用普通List，无2项限制；不能根据类型名猜限制。

# 当前已验收：Upgrade基类及十派生

2026-10-23续接阶段：新增11种完整Upgrade模块，已验收支持74/322，248明确拒绝；58 Token-only路径不变。最新有效源码d39c891b32b8ddc229ffc4db76fb71f5fec7fdaa，GameObject run40034294614、基础run40034294612均success；远端gameobject-root/validation.json已核对。每/Od、/O2：158216 fixtures、474648三缓冲区逐字节比较、9084拒绝、312不变性，平台0/1/2/99。SDK本轮0次。

下一组其余11种命名Upgrade派生的139个原方法已提取，见evidence/gameobject-modules/upgrade-remaining-source/manifest.json，源码导出run40034106033 success。这11种尚未实现、仍拒绝。继续整组恢复字段、嵌套依赖、Token、Endian后验收，不能把本组74计为85。

# 编译器恢复：当前续接入口

更新：2026-10-24（北京时间）。仓库wu162/NewSdk，分支recovery/compiler-source。先读AGENTS.md、EFFICIENCY.md及本文件；不要全量加载旧STATUS和大反编译文件。

## 本轮完成：三个完整组，新增17种已知Marshal类型

四声音/EVA选择器、六生命周期Update、七共享Die模块及其完整公共依赖均完成。支持范围从46增加到**63/322**已知Marshal类型；其余**259明确拒绝**，58 Token-only仍沿原未知Marshal路径。根范围仍88属性、38类子节点，六容器不代表所有模块已恢复。完整GameObject、两份完整替代DLL和正式GameObject插件尚未完成。

最终源码**379bed90fede179cb9270a1566ce989fe48fb5df**：
- GameObject run**40019863862** success。
- 基础run**40019863876** success。
- 远端validation.json与日志均核对；每/Od、/O2模式**130,596 fixtures、391,788三缓冲区逐字节比较、8,688拒绝、300不变性比较**，平台0/1/2/99；两模式合计783,576次缓冲区比较。
- SDK本轮**0次**，原路由保留。artifact recovered-gameobject-component-x86是组件，不是正式SDK插件。

本轮按约45分钟有效实现阶段推进，完成首组后保存并继续后两完整组。已及时保存未验证/验证中检查点，最终验收与检查点分离。所有提交和Actions证据保留；未新建分支、未强推。

## 类型文档与关键契约

- GAMEOBJECT_SOUND_SELECTORS.md：四选择器，完整Override和音频列表。AudioSoundEntry.Sound是内联资源引用，SoundOrEvaEvent.Sound才可空。弱Upgrade ID列表与NamedEntry精确大小写StringHash不可混用。该组独立成功source448bf2903742f7d43b2dc1e5938ef2fd4a2ca416/run40016394892，后续仍在最终完整矩阵内。
- GAMEOBJECT_LIFECYCLE.md：六Update，24根属性和ObjectStatusValidation两字段。Update Token继承ContainModuleData必须保留。Height InitialDelay是ceil逻辑帧uint；Time字段不等同。10%默认位模式0x3dcccccc，保留float乘法。独立成功source36bdb6c0bf42094010222fb7e5fa102644cce142/run40018019890。
- GAMEOBJECT_DIE.md：七个共享DieModuleData分派，inline44字节DieMux、等级字符串、可空ObjectStatus、34位死亡位集、强Upgrade资源列表与Refund过滤器。DieMux缺失保持全零，显式空才写默认。DestroyDie没有独立具体Token方法，实际布局在helper103932，保留自身父表层。字符串头长度不含终止符。最后成功锚点即上述379bed9。
- 此前九Body、六ClientUpdate、两声音播放、31公共模块、AI及根子节点均保留在最终矩阵内。对应文档按需读取。

失败记录不冒充成功：选择器首次测试节点名漏ClientBehavior；生命周期独立10%期望误写0.1f；Die首轮宿主变量重名、次轮String长度误含终止符，均已修正后取得完整最终矩阵。

## 下一完整组：Upgrade基类与十种派生（未实现）

先读**GAMEOBJECT_UPGRADE_NEXT.md**。原方法及精确签名/原行号/SHA256已保存evidence/gameobject-modules/upgrade-source/manifest.json；源导出工作流run40019496382成功。这11类型仍明确拒绝，不能把源证据报告当运行支持表。

公共Upgrade大小36：五bool，CustomAnimAndDuration可空12字节指针@8，TriggeredBy@12和ConflictsWith@20为弱typed-ID列表；派生字段先于基类分配。AnimAndDuration的AnimState@0是scalar ModelCondition枚举，Frames@4/Trigger@8均unsigned默认0，Trigger不是枚举。Delayed.DelayTime是unsigned；LevelUp两个signed；其余精确字段见下一组文档。继续核对PE枚举fallback、折叠wrapper、Token/Endian后整组实现和一次完整验收。其他大型Upgrade派生保持拒绝。

## 持续规则与工具入口

默认单代理。约45分钟有效实现阶段，完整能力组连续调查/实现，统一/Od、/O2、四平台含99、三缓冲区逐字节、独立布局/Token/sentinel与拒绝矩阵，绝不降低正确性。每约10–15分钟检查并提交/普通推送有意义成果；不因提前完成一个小组就收尾，也不空等凑时长。

操作持续授权，仅禁止任何强推；保留并发Actions证据并基于最新头快进。GitHub API更新发生异常或lease不符先查实际头，不盲重试旧树。命令行Git本环境无凭据，使用GitHub插件。大原反编译经固定blob检查的ExportSoundSelectorMethods.py/ExportLifecycleMethods.py分段导出；后者支持--die-family和--upgrade-family。workflow文件sound-selector-source.yml显示名为Export GameObject module source evidence，可复用，不为每种类型新建工作流。生成器GenerateSoundSelectorFields.py、GenerateLifecycleFields.py、GenerateDieFields.py均接入TestGameObject.ps1完整--check。

SDK跨阶段累计，组件通过不冒充正式集成；原/恢复同输入、合法XSD、实际调用、四产物及适用无原DLL重放门槛保留。本轮未改Weapon/Armor源码、SDK输入/路由，未重跑其已验收SDK。额度优化实际采用按方法读取、整组验收、复用公共契约和SDK降频；没有账号可比计量，不量化额度节省。
