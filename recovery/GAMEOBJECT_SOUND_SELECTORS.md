# 下一完整组：四个声音/播报选择器（尚未实现）

本轮只完成源码调查，不启用TypeId，不声明原DLL验证。提取器scripts/ExtractSoundSelectorEvidence.py及evidence/gameobject-modules/sound-selector-source-plan.json保留原blob指纹、精确签名/行号/方法摘要、lookup顺序/默认、Token记录及helper调用、endian重载，独立检查8种列表stride。生成和--check均通过；没有替代运行差分。

| 模块 | TypeId | nativeSize | 核心依赖 |
|---|---:|---:|---|
| RandomSoundSelector | 3030954484 | 28 | Chance百分比@8默认50；可空int优先级@12；Voice@16；Sound@20；Reroll bool@24 |
| ModelConditionSoundSelector | 847051638 | 16 | Override列表@8，stride132 |
| UpgradeSoundSelector | 4178664105 | 16 | Override列表@8，stride148 |
| ModelConditionEvaEventSelector | 1640547431 | 16 | Override列表@8，stride124 |

Override flags均两组inline60字节，注意Upgrade字段名是RequiredModelConditions/ConflictingModelConditions，其余是RequiredFlags/ExcludedFlags。ModelConditionSound的可空优先级@120、Voice@124、Sound@128；Upgrade再有RequiredUpgrade列表@132、ConflictingUpgrade@140。Eva条目只有EvaEventArray指针@120。

复用已验证根Audio契约，但需扩展可空结构Token及nullable int：AudioArrayVoice24（AudioEntry stride12、ObjectSpecificEntry16、NamedEntry12）；AudioArraySound8含AudioSoundEntry8（Sound可空引用@0、Type@4）；EvaEventArray8含EvaEventEntry8（EvaEvent资源@0、Type@4）。SoundOrEvaEvent8为Sound可空引用@0、EvaEvent资源@4；ObjectSpecific目标是小写typed-ID，Named名称区分大小写StringHash。部分Marshal被内联或折叠，必须查看父方法/列表helper，不得编造缺失独立重载。

按整个四选择器依赖组实现，保留原优先级→Voice→Sound→Upgrade列表分配次序、枚举fallback、单例/列表XML约束和弱升级ID语义。随后一次完整/Od、/O2、0/1/2/99三缓冲区矩阵、独立布局/Token/endian与拒绝验证。其他未支持类型继续拒绝。SDK仍待累计能力后的正式集成批次。
