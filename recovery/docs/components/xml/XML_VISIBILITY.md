# Invisibility完整组件组

2026-10-25。`visibility`组新增 InvisibilityTemplate 与 InvisibilityManagerSettings。最终差分状态由本轮汇总及 `evidence/xml-visibility/validation.json` 的 sourceCommit/runId锚定。SDK未接入、未执行。

## 原DLL证据

固定Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`；原DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。

- `evidence/xml-source/parts/part-0052.cs.txt` 原行26429完整InvisibilityTemplate Marshal、26491 EndianCorrect（延续part-0053）；同part原ObjectFilter Marshal行26334。`part-0105.cs.txt` 原行52827 Manager内联ObjectFilter。
- `parts/part-0127.cs.txt` InvisibilityType enum未知fallback CAMOUFLAGE=1，weakUpgrade列表；`parts/part-0167.cs.txt` Options5 bitset，未知fallback bit0、ALL符号无关且填整个uint、不继续后续token，负号为XOR，与既有MarshalEnumBitset合同相同。
- `static-data/fields-0019.json` 原静态完整默认字符串（MSVC符号名截短不得当内容）：ForbiddenConditions 15个A/B/C武器状态、ForbiddenStatus 4个状态，精确文本见Native与Oracle独立默认列表。
- `enums/InvisibilityType.json`、`InvisibilityNuggetOptions.json`、`ModelConditionFlagType.json`、`ObjectStatusType.json`、`KindOfType.json`与ObjectFilter枚举；原 StructLayout分别240与116字节。
- 原Plugin精确HandleType对所有非Win32平台大端，Manager对`root+4`的ObjectFilter做EndianCorrect。TypeId/TypeHash见 `catalog/XmlvisibilityTypes.json`。
- schema：`AssetTypeInvisibilityTemplate.xsd`和`System/InvisibilityManagerSettings.xsd`。不将XSD必填约束添加到原组件Marshal。

## 全字段布局

InvisibilityTemplate头4字节保留零。ForbiddenConditions@4和ForbiddenConditionExceptions@64各15×uint；ForbiddenStatus@124共7×uint；CamouflageLevel@152 unsigned；InvisibilityType@156（INVALID=-1、STEALTH=0默认、CAMOUFLAGE=1及未知）；Options@160一个uint；EnteringStealthFX@164、LeavingStealthFX@168强引用；HintDetectableStates@172七uint默认IS_ATTACKING；NoStealthForAttackWindow@200 time默认0s；RequiredStatus@204七uint；IgnoreTreeCheckUpgrade列表count/pointer@232/236、4字节弱TypedAssetId项。ForbiddenConditionExceptions缺省空；ForbiddenConditions与ForbiddenStatus用完整原静态默认；RequiredStatus/Options absent留零。所有模型/状态/Options值均复用已验证EnumBitset逻辑，弱ID复用大小写归一hash，强引用复用Tracker索引+1。

InvisibilityManagerSettings为116字节：root+4内联112字节ObjectFilter。节点缺失则112字节保持零；present-empty写Rule=NONE3/Alignment=NONE0，不分配ObjectFilter自身。仅StatusBitFlags、StatusBitFlagsExclude present时各分配28字节；随后IncludeThing、ExcludeThing分别分配4字节weak列表。适配器只剥离TypeId元数据，全部属性与Thing文本按原IValue C-string截断；共享ObjectFilter实现和Weapon/GameObject代码未改。

大端仅swap数值word和weak ID；强引用/指针由Tracker处理。Template列表按原分配顺序，Manager共享filter子块顺序必须保留。没有Tokenize/BAB预处理/SDK正式路由变化。

## 完整测试

`TestXmlvisibility.ps1` `/Od`、`/O2` × 平台0/1/2/99，固定原DLL逐实例ProcessInstance与恢复ProcessInstance、CompileXml各三个缓冲区逐字节比较。每个ModelCondition、ObjectStatus、KindOf词条与符号/NUL均有覆盖；全部Options、InvisibilityType、filter Rule/Alignment/Relationship、unknown/ALL/负号/空/Unicode、整数/time/ref边界、weak文本和0..1024项增长均覆盖。

独立ABI核每个原默认bit、所有根/项大小和偏移、全部模型/状态词条的独立位索引、Options mask、两强引用index+1及导入排序、weak等价/空值、Manager absent/present-empty区别、内联filter以及28/28/4/4子块分配、计数和重定位顺序。显式拒绝未知属性/节点、重复filter、非法文本、foreign namespace、qualified属性、weak节点结构、unknown根、空声明及handle不匹配。最终Windows差分仍是验收门槛，Linux语法检查不能代替。
