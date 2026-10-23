# 编译器恢复：当前续接入口

更新：2026-10-24（北京时间）。仓库wu162/NewSdk，分支recovery/compiler-source。先读AGENTS.md和EFFICIENCY.md。

## 本轮进行中：共享 Die 家族

七种Die模块、完整inline DieMuxData、强升级资源列表、建筑字符串列表与Refund ObjectFilter源码和测试已准备；当前是**验证中检查点**，不能提前声明这七种已验收。GenerateDieFields.py派生Token/字段，DestroyDie使用折叠的实际多态helper，保留自身继承层。完成当前提交的完整矩阵再更新状态，SDK0次。

## 当前已验证范围：56/322已知Marshal类型

六种生命周期Update已通过，源码36bdb6c0bf42094010222fb7e5fa102644cce142；GameObject run40018019890、基础run40018019764均success。每/Od、/O2模式112,084 fixtures、336,252三缓冲区比较、8,576拒绝、292不变性比较；平台0/1/2/99。合计672,504次缓冲区比较；SDK未运行，原路由保留。

本轮更早四声音/EVA选择器：source448bf2903742f7d43b2dc1e5938ef2fd4a2ca416/run40016394892及基础40016394889 success；每模式105,348 fixtures、316,044比较。详情GAMEOBJECT_SOUND_SELECTORS.md。AudioSoundEntry.Sound内联，SoundOrEvaEvent.Sound才可空。

六Update：DeletionUpdate、DieOnProducerDeathUpdate、InvalidTargetsDeathUpdate、HeightDieUpdate、LoseHealthUpdate、LifetimeUpdate；24根属性及ObjectStatusValidation两字段。Update Token实际继承ContainModuleData；InvalidTargets可空28字节ObjectStatus，LoseHealth可空56字节ObjectStatusValidation，Lifetime可空有符号int。Height InitialDelay按ceil逻辑帧uint；Time不等同。默认10%位模式0x3dcccccc（float乘0.01f），不是直接0.1f。原方法在lifecycle-source/manifest.json，生成器GenerateLifecycleFields.py。

此前九Body、六ClientUpdate、两声音播放、31公共模块、AI及全部根子节点均在完整矩阵内。根范围88属性、38类子节点；未完成整个GameObject、两份完整替代DLL或正式GameObject插件。此前56范围尚有266类型明确拒绝，58 Token-only仍原未知Marshal路径。当前Die组只有通过完整矩阵后才可将计数增至63。

## 持续规则

约45分钟有效实现阶段；每约10–15分钟检查并保存有意义成果。完成一组后继续下一依赖满足组，不空等凑时长。默认单代理，按完整组连续实现并统一/Od、/O2、四平台含99、三缓冲区逐字节、独立布局/Token/sentinel和拒绝矩阵，不降低正确性。

仓库操作持续授权，只禁止强推；保留并发Actions证据，基于最新头普通快进更新现有分支，不新建无必要分支。命令行Git本环境无凭据，使用GitHub插件；大原反编译用固定blob检查的源方法导出工作流分段读取。源证据报告不是运行支持清单，最终以validation.json的sourceCommit/runId和矩阵终态为准。

SDK跨阶段累计，组件成功不冒充正式集成。原/恢复同输入、合法XSD、实际调用、四产物比较和适用无原DLL重放门槛保留。Weapon/Armor本轮未改源码、SDK输入或路由，不复跑已验收SDK；其旧锚点见相应文档。
