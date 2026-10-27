# 四种声音／播报选择器（组件已验证）
  
源码448bf2903742f7d43b2dc1e5938ef2fd4a2ca416；GameObject run40016394892、基础run40016394889均success。每/Od、/O2模式105,348 fixtures、316,044三缓冲区逐字节比较、8,516拒绝、284不变性比较；四平台0/1/2/99。两模式合计632,088次缓冲区比较。SDK未运行，原路由保留；不是正式GameObject插件。

| 模块 | TypeId | nativeSize |
|---|---:|---:|
| RandomSoundSelectorClientBehavior | 3030954484 | 28 |
| ModelConditionSoundSelectorClientBehavior | 847051638 | 16 |
| UpgradeSoundSelectorClientBehavior | 4178664105 | 16 |
| ModelConditionEvaEventSelectorClientBehavior | 1640547431 | 16 |

支持四模块全部属性与子节点、三Override stride132/148/124、AudioArrayVoice三种列表、AudioArraySound、EvaEventArray、可空有符号优先级和弱Upgrade ID列表。两inline60字节ModelCondition mask、优先级→Voice→Sound→Upgrade分配次序、scalar enum fallback、NamedEntry大小写敏感StringHash和TargetObject/Upgrade小写typed-ID均保留。**AudioSoundEntry.Sound是内联资源引用；SoundOrEvaEvent.Sound才是可空指针**，更正此前交接描述。

实现GameObject.cpp、桥接GameObjectCompiler.cpp；GenerateSoundSelectorFields.py从固定原blob 6e40a9ed6c9f961d294d49768d911e6a6f354619派生18个Token布局及四分派选择，生成GameObjectSoundSelectors.h。检查集成进TestGameObject.ps1；此前原矩阵全部保留。新增所有嵌套合法枚举、fallback、ModelCondition全表、资源边界、空/多项列表、分配顺序、typed-ID和NamedEntry大小写、不支持输入及独立布局/Token/sentinel断言。

原始方法逐方法核对证据在evidence/gameobject-modules/sound-selector-methods/；ExportSoundSelectorMethods.py核对原blob和body SHA256，覆盖折叠的具体重载、父方法、列表及共享helper。source-plan.json仍是此前调查计划，不能单独代替运行验收。首次run40016123624因测试节点漏ClientBehavior后缀而失败，已按XSD修正；不能把该失败列为通过，最终锚点为上述修正后完整矩阵。

本组已保存50/322已知Marshal类型支持状态，其他272明确拒绝，58 Token-only继续原未知Marshal路径。完整GameObject和两份完整替代DLL仍未完成。

后续六种生命周期Update及共享Die组已完成组件验收，见GAMEOBJECT_LIFECYCLE.md、GAMEOBJECT_DIE.md。当前下一组为Upgrade基类及十种派生，只有源码证据、未启用；续接以RESUME.md为准。
