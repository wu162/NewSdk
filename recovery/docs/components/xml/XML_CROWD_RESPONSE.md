# CrowdResponse完整组件

2026-10-25，`crowd`组单根完整入口；最终差分状态由本轮汇总与`evidence/xml-crowd/validation.json` sourceCommit/runId锚定。SDK未接入、未执行；不重复另组的UnitIntro，不改GameObject代码。

固定Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`，原DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。

原证据：`evidence/xml-source/parts/part-0087.cs.txt` 原行43516 AudioArrayVoice、43526 Threshold、43556 CrowdResponse及AudioObjectSpecificVoiceEntry；`part-0086.cs.txt` 原行43475 NamedEntry原始FastHash；`part-0038.cs.txt` 原行19217 SoundOrEvaEvent；`part-0141.cs.txt`原行70868 Threshold列表、三个voice列表及Endian；`part-0163.cs.txt`原行81785 Sound nullable强引用4字节Push。原Voice枚举67项、ObjectSpecific枚举6项由`enums/ThingTemplateVoiceType.json`与`ThingTemplateObjectSpecificVoiceType.json`精确静态表恢复；FindNVPair完整原方法跨part-0119/0120，比较区分大小写，未知fallback32/0。

原StructLayout：root16、Threshold28、AudioArrayVoice24内联、AudioEntry12、ObjectSpecificEntry16、NamedEntry12、SoundOrEvaEvent8。schema为AssetTypeCrowdResponse.xsd、Includes/ObjectSound.xsd；组件不添加XSD必填/minOccurs限制。

根布局reserved@0、Weight@4 signed默认100、Threshold count/pointer@8/12。每Threshold NumUnits@0 signed默认0、AudioArrayVoice@4内联24字节。缺省voice保持零，present-empty三个count0，不增加voice本体分配。三个列表AudioEntry/ObjectSpecificEntry/NamedEntry依序Marshal（不按XML混排），count/pointer分别@0/4、8/12、16/20。

全部entry前8字节是SoundOrEvaEvent：Sound@0为nullable 4字节强引用指针，属性存在即Push，即使present-empty或文本没有反斜杠也分配零引用块；EvaEvent@4为直接强引用，默认空且不分配。AudioEntry enum@8；ObjectSpecificEntry enum@8、TargetObject弱TypedAssetId@12；NamedEntry原大小写敏感StringHash@8。TargetObject小写hash与NamedEntry raw hash不能混用。每个Threshold的三个列表及其各Sound块完成后才开始下一个Threshold。

平台0小端，1/2/99大端；Weight、NumUnits、各count、enum/hash校正，引用/指针由Tracker处理。导入和重定位按owner block分组，再按其index/target排序，不按全局index排序。

`TestXmlcrowd.ps1`运行/Od、/O2 × 四平台。每次原ProcessInstance前显式Initialize目标platform；恢复ProcessInstance与CompileXml各三个数组全部对照。Oracle覆盖全部73枚举、大小写/空/未知/NUL、所有Sound/EvaEvent引用边界、raw/weak Unicode/NUL/4097文本、signed数值、threshold与三个voice列表0..1024增长、交错XML和两threshold嵌套分配。独立ABI核root16、default100、threshold28/voice24、12/16/12项、Sound4、所有字段偏移、present-empty Sound分配、三列表分配顺序、ownerblock导入/重定位顺序。未知属性/节点、重复voice、非法文本、foreign根/qualified属性、空声明/handle不匹配显式拒绝；所有历史矩阵和SDK门槛保留。
