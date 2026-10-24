# UI chat: three-root component

2026-10-25（北京时间）。完整源码与组件差分矩阵准备，Windows待统一验证。本轮用户禁止真实SDK编译，未接SDK活跃Plugin、未改SDK配置；三根仅候选组件，不增加SDK82计数。

固定原Module blob2d2f471c8008b7c2e1f78e39752d0726da395cea、Plugin blob00bb9e5df72d07a21204f85f9a9a2f1d9557f00b、DLL SHA256 d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6。复用原方法索引与小块缓存，仅按必要方法扩展依赖，无原DLL回调。

| 根 | TypeId | TypeHash | 字节 |
| --- | ---: | ---: | ---: |
| UIQuickChatMessage | 4098431913 | 743017490 | 16 |
| UIMouseTacticalQuickChatDisplay | 2891878770 | 884927254 | 28 |
| UIMouseVoiceChat | 3746130610 | 3018771479 | 20 |

## 完整原合同

- UIQuickChatMessage：Module Marshal49745（part-0099），MessageText String<char>@4、Sound AudioEventInfo直接强引用@12。Plugin HandleType24487内联交换string length@4。Tracker109082（part-0218）Allocate(1,16)。
- QuickChatDisplay：Module Marshal48172 / Endian48205（part-0096），CloseKey HotKeySlot直接强引用@4、DelayDuration Time@8默认0s、MenuItem list@12、inline CloseMenuItem String Title@20。Plugin20863；Tracker106079（part-0212）Allocate(1,28)。
- VoiceChat：Module Marshal48275（part-0096），ToggleModeHotKeySlot@4、TalkHotKeySlot@8；ModeButtonHelp32* nullable@12、TalkButtonHelp32* nullable@16，固定顺序分配Mode再Talk，与XML顺序无关。Plugin22355仅递归两个Help做endian。Tracker107262（part-0214）Allocate(1,20)。
- InGameUISimpleHelpTemplate32：Module Marshal47525/Endian47556（part-0095），四个String<char> Title@0、Description@8、TypeDescription@16、Instructions@24。present-empty Help仍分配32；每个present-empty String仍分配NUL字节并4字节对齐。
- MenuItem32：Module Marshal48087 / Endian48127（part-0096），Title string@0，nullable ChatDestination enum*@8，ChatMessageGroup list@12，递归MenuItem list@20，nullable CloseMenuItem8*@28。严格按Title、destination、groups、递归menus、close分配；根inline close在所有菜单之后。CloseItem Marshal48061，Title string@0。Menu列表Marshal87113/Endian87155（part-0174）stride32，先一次分配所有items再逐项递归。
- ChatDestination helper74458（part-0148）present即Push4；Allies0、Everyone1，未知/空fallback0；缺属性nullptr。静态map evidence/xml-source/enums/UIMouseTacticalQuickChatDisplayChatDestination.json 行12354。
- ChatMessageGroup12列表Marshal74473（part-0148/0149）/Endian74521（part-0149），Faction enum@0默认Neutral3；ChatMessage list@4。Faction helper65951（part-0131）未知fallback0(Allies)，5项完整map在enums/FactionType.json行12416。缺Faction与未知Faction不同。
- ChatMessage4：Marshal48074（part-0096），MessageRef是TypedAssetId<UIQuickChatMessage>弱引用hash，不是Tracker强import。typed helper87067（part-0174）原CharToLower再FastHash，可复用HashAssetId；列表Marshal74385/Endian74425（part-0148）stride4。
- 标量/endian在Builder记录所有length、count、enum、hash、time，最后统一交换；pointer/ref仍由Tracker Finalize按block/value排序及index+1修正。所有非Win32平台含99使用big endian。
- 依赖HotKeySlot、AudioEventInfo只是传入已预处理引用值；不调用原编译器，不恢复其整个UI/音频模块。无Joypad依赖。

## 差分与边界

scripts/TestXmluichat.ps1在/Od、/O2各跑平台0/1/2/99。Oracle每fixture对原ProcessInstance与恢复ProcessInstance、CompileXml比较完整三数组，共6比较。覆盖所有字符串字段/空缺/对齐/1024长串/NUL/ANSI，全部强引用字段与signed-atoi溢出，时间单位、完整Destination/Faction枚举和缺省未知区别、TypedAssetId大小写/NUL/类型前缀、三级列表0/1/2/3/8/9/10/15/16/17/27/28/29/64/257增长、递归深度到32及独立ABI。

独立ABI固定156字节递归菜单实例及其12个pointer目标/target-block排序、Voice108字节四字符串帮助分配顺序，显式证明relocation顺序不按field address排序、Weak MessageRef不生成import。公共SideArray仅断言唯一/对齐/界限/哨兵，不假设单调偏移。未知/限定属性、外命名空间、未知child、结构非空文本、duplicate singleton、root/handle不符显式拒绝。沿用桥接深度32边界，33明确拒绝，不声称支持更深树。

三根原XSD分别UI/QuickChatMessage.xsd、UI/Mouse/Tactical/QuickChatDisplay.xsd、UI/Mouse/VoiceChat.xsd；均BaseAssetType不支持inheritFrom。Display schema要求CloseKey、1..9 MenuItem、root CloseMenuItem，各Title必填；底层Marshal不限制9，组件特测10和257不伪造截断。SDK本轮未运行，未放宽原XSD，schema合法样例应在未来获准SDK批次单独补齐。

最终验收以统一sourceCommit/runId及validation.json为准。Native本地仅语法检查，不代替Windows完整矩阵。主Plugin/catalog/联合smoke接线由集成代理负责，本组Types metadata在catalog/XmluichatTypes.json。
