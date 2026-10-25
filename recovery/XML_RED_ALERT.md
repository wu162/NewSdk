# RedAlertTemplate完整组件

2026-10-25，`redalert`单根92字节雷达威胁设置，最终Windows差分状态由本轮汇总与`evidence/xml-redalert/validation.json` sourceCommit/runId锚定；SDK未接入、未执行。

固定Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`、原DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。原证据`parts/part-0090.cs.txt`45164完整Marshal、45319 Endian（跨0091），`part-0145.cs.txt`72595 nullable AttributeModifierRef；原StructLayout92，原Plugin非Win32大端，XSD AssetTypeRedAlert.xsd。全部截断MSVC符号名已核`static-data/fields-0025.json`真实完整文本。TypeId4141306833、TypeHash133543122。

root reserved@0；unsigned OrangeAreaThreatThreshold/RedAreaThreatThreshold/MaxAllowedThreat@4/8/12默认0；float KillThreatMultiplier/DeathThreatMultiplier@16/20默认1；unsigned Yellow/Orange/RedAreaThreatDecayPerSecond@24/28/32默认5；unsigned RedAlertBuildingDamagePerSecond/RedAlertUpdatedBuildingDamagePerSecond@36/40默认0；time RedAlertBuildingDamageIncreaseTime@44默认0s；直接Eva引用EvaEventToPlayOnRedAlertInitiateOwner/Ally/Enemy@48/52/56默认空；float Yellow/Orange/RedAlertTechPointMultiplier@60/64/68默认1；nullable AttributeModifier引用YellowAreaEffect/OrangeAreaEffect/RedAreaEffect/RedAlertEffect@72/76/80/84，各present即分配4字节（含empty）；bool DisableNonRepairability/RedAlertDisableStructureBuilding/RedAlertShowAllUnitsAndStructures@88/89/90默认0/1/1，padding91为0。

四effect总按Yellow/Orange/Red/RedAlert顺序分配，不按XML顺序；强引用index+1、ownerblock导入顺序由Tracker处理。全部数值word大端，bool不swap，引用/指针由Tracker处理。

`TestXmlredalert.ps1` /Od与/O2 × 平台0/1/2/99，每原ProcessInstance前显式Initialize目标平台，恢复ProcessInstance与CompileXml各三数组完整对照。全8 unsigned/5 float/time/3 bool/3 directref/4 nullable引用字段矩阵，CRT/溢出/D指数/单位/NUL/Unicode/ref索引边界、全部16种effect存在组合。独立ABI核92根、全部defaults与offsets、浮点bits、bool/padding、四effect逆XML固定分配顺序、present-empty4字节、rootindex排序后分组effect导入与重定位。未知属性/节点、foreign namespace、qualified属性、重复effect、非法结构、空声明和handle不匹配显式拒绝。未改变Tokenizer/BAB预处理/SDK路由。
