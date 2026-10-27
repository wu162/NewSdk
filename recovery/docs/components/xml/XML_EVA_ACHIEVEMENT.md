# Eva与成就组件

2026-10-25。组名 `evaachievement`；组件实现与原DLL差分入口齐备，最终验证状态和 sourceCommit/runId 以本轮汇总及 `evidence/xml-evaachievement/validation.json` 为准。SDK 未接入、未执行。已存在于 players 组的 AIPersonalityToAchievementEventMapping 不重复登记。

## 原始证据

固定 Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`，原DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。

- `evidence/xml-source/parts/part-0040.cs.txt`：原 Module 行20061 SiegeEvaEvent、20092 EvaSettings、20169 EvaEvent、20272 beacon item、20281 MiscEvaEvents 的完整 Marshal；EvaSettings/EvaEvent EndianCorrect紧邻。完整字段名包含被MSVC符号截短的 `EnemySightedMaxVoicePositionScanRange` 和 `MaxTimeBeforeResettingLastJumpTo`，核对 `static-data/fields-0017.json`。
- `parts/part-0083.cs.txt`：原行41553 MapName映射、41573 Action、41595 AchievementEvent；`part-0084.cs.txt` 原行42045 GameplayAchievementTriggerData。
- `part-0121.cs.txt`：原行60716 Siege enum fallback2；60767 weak Eva typedId列表、60896 beacon列表；AIBeacon enum fallback0。`part-0140.cs.txt` 原行70128 Achievement enum fallback3与12字节Action列表。
- `enums/SiegeEvaEventType.json`、`AchievementActionType.json`、`AIBeaconType.json`：精确静态枚举表。比较区分大小写并使用原 C-string NUL 截断；缺少无默认枚举属性留零，存在未知值用上述fallback。
- `decompiled/BinaryAssetBuilder.XmlCompiler/SageBinaryData/*.cs`：原根/项 StructLayout 大小；原 Plugin 各 HandleType 的平台分支、内联Endian和精确TypeId/TypeHash。完整入口清单在 `catalog/XmlevaachievementTypes.json`。
- 原XSD：`Includes/Eva.xsd`、`Includes/MiscEvaEvents.xsd`、`Multiplayer/SiegeEvaEvents.xsd`、`AssetTypeAchievementData.xsd`、`AssetTypeGameplayAchievementTrigger.xsd`。组件保留原 Marshal 行为，不强加XSD必填或minOccurs。

## 完整布局和语义

所有根头4字节为0。平台0/Win32为小端，1/2/99大端。强引用交给Tracker处理，不对指针或引用另做数值swap。

| 根 | 字节 | 字段 |
| --- | ---: | --- |
| SiegeEvaEvent | 16 | Seconds@4 time；EvaEvent@8强引用；EventType@12，缺省ALL=2 |
| EvaSettings | 20 | EnemySightedMaxVoicePositionScanRange@4 signed；MaxTimeToKeepJumpToEvents@8 time；MaxTimeBeforeResettingLastJumpTo@12 time；MinDistanceBetweenJumpToEvents@16 signed |
| EvaEvent | 44 | Priority@4 unsigned；TimeBetweenEvents@8默认0s；ExpirationTime@12默认2s；QuietTime@16默认0s；TimeToWaitBeforePlaying@20默认0s；SideSound列表@24/28；OtherEvaEventToBlock列表@32/36；bool AlwaysPlayFromHomeBase/CountAsJumpToLocation/SuppressOn360/SuppressOnPC@40/41/42/43，默认0/1/0/0 |
| MiscEvaEvents | 164 | 38强引用依次@4..152；AIBeaconTypeSettings列表@156/160 |
| AchievementEvent | 16 | EventId@4；Action列表@8/12，12字节项TargetCount@0/Type@4/ActionObjId@8 |
| MapNameToAchievementEventMapping | 16 | MapName string长度/指针@4/8；AchievementEventId@12 unsigned |
| GameplayAchievementTriggerData | 12 | GameplayConditionData@4强引用；AchievementEventId@8 unsigned |

SideSound为8字节项：Sound@0强引用、Side@4为小写TypedAssetId弱hash。OtherEvaEventToBlock为4字节weak hash项，读取节点文本。EvaEvent总是先分配SideSound再分配weak列表，不依XML子节点交错顺序改变分配顺序。Beacon为8字节项：BeaconType@0、PlacedEvent@4强引用。全部列表顺序保留且无XSD数量截断。MapName缺省无分配，present-empty分配NUL；长度使用C字符串字节数；大端只swap长度和整数，原字符字节不变。

38引用完整表与偏移在Native源码的Misc分支逐项列出；Oracle另保留独立名称表和全部字段ABI核对。导入按owner block、reference index排序；根38引用必须先于beacon块引用，不能按全局reference index混排。

## 测试与边界

`TestXmlevaachievement.ps1`构建 `/Od`、`/O2`，每模式平台0/1/2/99；`XmlevaachievementOracle.cs`逐实例调用原 ProcessInstance，与恢复 ProcessInstance 和 CompileXml 的三个缓冲区各比较一次。固定原DLL指纹、完整字段循环、signed/unsigned溢出和CRT前缀、time单位、全部enum/未知/大小写/NUL、bool首字符、强引用索引边界、weak/string Unicode/NUL/4097字节输入、0/1/2/3/15/16/17/27/28/29/64/257/1024项增长均覆盖。

独立ABI检查所有根缺省、时间浮点bits、字段偏移、bool不swap、两列表分配顺序/大小、38引用+beacon的owner排序、强引用index+1、字符串长度/指针/NUL和Action12字节布局。显式拒绝未知属性/节点、foreign namespace、qualified属性、结构化节点非法文本、unknown根、空声明与handle不匹配；TypeId为BAB元数据，root id/inheritFrom只接收已预处理语义。没有恢复整个BAB预处理、继承或Tokenizer。
