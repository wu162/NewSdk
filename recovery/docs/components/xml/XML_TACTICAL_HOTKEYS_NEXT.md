# 已实施：五根战术UI热键原始续接笔记

2026-10-24原始取证，已于2026-10-25完成组件实现及完整差分（源码d9908f5，运行40132543491）。**SDK尚未运行，仍未加入活跃SDK路由**。详见XML_TACTICAL_HOTKEYS.md；以下保留原取证锚点，不再是待恢复任务。唯一资产依赖HotKeySlot已恢复，无Joypad依赖。继续时先核远程头及XMLCOMPILER_PRIORITY.md，再按这些锚点定点读原方法，不把本笔记当成功证据。

固定原Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`，Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`。以下行号为完整原Module或Plugin；parts在 evidence/xml-source/parts，索引为 evidence/xml-source/method-index.txt。

| 根 | TypeId | TypeHash | 根字节 |
| --- | ---: | ---: | ---: |
| UIMouseTacticalSelectionDetailsButtonHotKeys | 1296329066 | 1187295208 | 12 |
| UIMouseTacticalMainCommandBarHotKeys | 2591692061 | 1944261033 | 24 |
| UIMouseTacticalPlayerPowerHotKeys | 2283998413 | 2326182616 | 12 |
| UIMouseTacticalMiscHotKeys | 2014061824 | 3544626863 | 200 |
| UIMouseHUDElementHotKeyGate | 2738273474 | 3442548366 | 12 |

## 完整布局与取证锚点

- Selection：UnitAbilitySlot文本引用list，count@4/pointer@8，每项4字节。Plugin HandleType行21200内联Marshal/count endian；Tracker106352，part-0212。
- PlayerPower：相同布局，元素名Slot。Plugin20651；Tracker105897，part-0211。
- MainCommandBar：SelectionRefinementPageSlot directref@4；BuildQueuePage list@8，8字节项QueueType enum@0/Slot ref@4；ButtonSlot文本引用list@16，每项4字节。先分配整块BuildQueuePage，再ButtonSlot，不随XML交错顺序改变。根Marshal47973、子项47964（part-0095）；list Marshal/Endian74190/74275、ref list74230（part-0148）；Plugin20205内联ButtonSlot count endian；Tracker105533（part-0211）。
- Misc：47个directrefs@4..188；StanceSettings list count@192/pointer@196，8字节项Stance enum@0/Slot ref@4。根Marshal47992（part-0095/0096），子项47983（part-0095），list Marshal/Endian74310/74350（part-0148）；Plugin20315；Tracker105624（part-0211）。
- HUDGate：ElementHotKeyMapping list@4，每项8字节ElementId enum@0/HotKeySlot ref@4。子项47516（part-0095），list Marshal/Endian73663/73703（part-0147）；Plugin16313内联根Marshal；Tracker102348（part-0204）。

三种enum均区分缺属性保持零与present但未知：

| enum | unknown fallback | 原helper/表 |
| --- | --- | --- |
| MouseHUDFixedElementId | 21 / AggressiveStanceButton | 73652，part-0147；enums/MouseHUDFixedElementId.json，36项 |
| ProductionQueueType | 4 / AIRCRAFT | 73738，part-0147；enums/ProductionQueueType.json，8项 |
| Stance | 1 / AGGRESSIVE | 70305，part-0140；enums/Stance.json，4项 |

三个8字节list的Endian只交换每项enum@0及count；引用和pointer交Tracker。HotKeySlot directref helper68311（part-0136）仍是找反斜杠、atoi suffix、AddReference；复用已验收ResourceMarshaling即可。输出index+1，bookmarks按block/value排序，不能按field address猜顺序。所有Plugin非Win32均big endian，包含99。

Misc字段顺序对应offset=4+4*i：

```text
AllCheer, AttackMove, CameraReset, ChatWithAllies, ChatWithBuddies,
ChatWithEveryone, CommunicateWithAlliesMode, CreateFormation, CycleHarvesters,
DeleteBeacon, ForceAttack, ForceAttackMove, ForceMove, GoToNextBuildQueue,
GoToPriorBuildQueue, GoToNextSubGroup, GoToPriorSubGroup, OpenAdvancedCommands,
OpenLoadGameScreen, OpenMessenger, OpenPauseScreen, OpenPlayerTechStore,
OpenSaveGameScreen, OpenStatusScreen, PlaceRallyPoint, PlanningMode,
PreferSelection, QuickChatMenu, ReverseMove, Scatter, SelectAll,
SelectMatchingUnits, SellMode, ShowAllHealthBars, Stop, TelestratorErase,
TelestratorNextColor, TelestratorNextLineWidth, TelestratorPriorColor,
TelestratorToggle, ToggleFastForwardMode, ToggleHUD, TogglePowerMode,
ToggleRepairMode, ViewHomeBase, ViewLastEvaEvent, WaypointMode
```

## SDK与实施门槛

五根在原CnC3Types.xsd均为全局资产，继承BaseAssetType，**不支持inheritFrom**；根list均可空。XSD在SDK/Schemas/xsd/UI/Mouse/下：

- Tactical/SelectionDetails/ButtonHotKeys.xsd
- Tactical/MainCommandBar/HotKeys.xsd
- Tactical/PlayerPower/HotKeys.xsd
- Tactical/MiscHotKeys.xsd
- HUDElementHotKeyGate.xsd

合法非空样例中Gate项需ElementId/HotKeySlot，BuildQueuePage需QueueType，StanceSettings需Stance；引用可复用RecoveryUiHotKey。仍需逐项schema验证，不放宽原约束。

以下为已完成组件实现时的要求；未来SDK批次仍须另行获准真实编译。按完整五根一批补Native/Bridge、全部enum与47字段引用边界、缺省/未知/NUL/ANSI、list增长与跨list分配顺序、独立ABI和拒绝矩阵；完成后统一/Od、/O2、四平台三数组原DLL对照及联合SDK验收。同步Plugin/catalog/联合根数量/固定SDK覆盖清单，不用联合smoke替代完整组矩阵。
