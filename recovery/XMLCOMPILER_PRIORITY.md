> 2026-10-25新一轮进行中：本轮104个新增入口源码/完整测试已准备，总287入口；第四批Windows组件完整矩阵待验收。下面183入口成功记录仍是上一轮基线，不是本轮287入口验证。SDK真实编译继续禁止，实际0次。

# XmlCompiler：当前续接入口

更新：2026-10-25（北京时间）。目标分支 `recovery/xmlcompiler-priority`。用户本轮要求约一小时、最多三个子代理，**不触发SDK真实编译**。

## 当前已验证范围

**原Plugin301个登记入口中，已有183个完整组件入口通过差分；本轮新增101个。** 本轮新增全部是“组件已验证、SDK待验证”，不能称为183类SDK已支持。原82类的历史SDK基线保留；175个组件类型在原CnC3Types.xsd为全局资产，其他8类仅有独立Plugin入口/嵌套或抽象schema。

- 最终组件/联合DLL源码：`d9908f57107f11b7ab875ac5787fce2bf23e00a9`。
- 21组完整差分与183入口联合DLL：[运行40132543491](https://github.com/wu162/NewSdk/actions/runs/40132543491)，completed/success。
- 已逐份核sourceCommit/runId、两优化模式、平台0/1/2/99、实际作业日志与联合进程真实退出码。后续仅文档提交不改变验收源码。
- 本轮SDK真实编译 **0次**。最后一次SDK运行仍为上轮40098695436，源码92180f9bbd830b9dbd49a696c259e54ffe2d7721，只证明原82类历史构建；不用于冒充本轮183入口联合产物SDK验收。
- `catalog/XmlRecoveredTypes.json`保持原82类SDK清单不变；`catalog/XmlComponentOnlyTypes.json`单列新增101类，全部sdkValidated=false，当前联合DLL的SDK状态为false。
- 主Plugin、SetXmlPriorityPlugin、TestXmlPrioritySdk、SDK Oracle/联合probe、SDK工作流、原SDK文件均未修改。精确Git SHA、运行记录与门槛见 `evidence/xml-component-acceptance-20261025.json`。
- 三个子代理工作分支与主集成检查点保留父提交历史并合入本分支，无强推。未合入另一会话的compiler-source。

## 本轮七个完整组

| 组 | 新入口 | 范围与文档 |
| --- | ---: | --- |
| tacticalhotkeys | 5 | 五根战术UI热键；XML_TACTICAL_HOTKEYS.md |
| uichat | 3 | UIQuickChatMessage、UIMouseTacticalQuickChatDisplay、UIMouseVoiceChat；XML_UI_CHAT.md |
| uidecals | 4 | UIDecalGrid、UIDecalCloud、UIDecalDestinationMarker、UIWorldBuilderDecalGrid；XML_UI_DECALS.md |
| singlesound | 10 | BaseAudioEventInfo/BaseSingleSound/AudioEvent、MusicTrack/MP3MusicTrack、DialogEvent/MP3DialogEvent、AmbientStream/MP3AmbientStream、Multisound；XML_SINGLE_SOUND.md |
| pathmusic | 7 | DynamicState/StateSet/Transition、EventRuntime/EventSet/MapRuntime、MusicPalette；XML_PATH_MUSIC.md |
| musicscript | 21 | Track、基础/昂贵条件与完整具体条件族；XML_MUSIC_SCRIPT.md |
| uitactical | 51 | 20种战术命令、26种按钮/tab及5种选择/导航/覆盖层配置；XML_UITACTICAL.md |

每组全部字段、必要依赖和原入口均已恢复，不把未知字段截断成表面入口。精确名称/TypeId/TypeHash/schemaGlobal见各组Types.json及组件专用汇总。旧82类十四组完整回归也全部通过。

## 最终完整矩阵

每行合计/Od、/O2，四平台0/1/2/99。数组比较包含适用的直接CompileXml对照，不能全部称为独立原DLL调用。没有减少旧组样本或放宽拒绝/独立ABI断言。

| 组 | Fixtures | 数组比较 | 显式拒绝 |
| --- | ---: | ---: | ---: |
| attributes | 9528 | 28872 | 288 |
| effects | 13040 | 78240 | 320 |
| powers | 34656 | 103968 | 1072 |
| locomotor | 33048 | 99144 | 2024 |
| ai | 15824 | 94920 | 416 |
| fxparticle | 27752 | 83256 | 784 |
| progression | 33280 | 199680 | 976 |
| environment | 49056 | 294336 | 2448 |
| players | 44760 | 134280 | 984 |
| damagefx | 2368 | 14208 | 160 |
| hotkeymaps | 13144 | 78864 | 296 |
| audio | 33856 | 203136 | 576 |
| uisettings | 28232 | 169392 | 2408 |
| soundmaps | 24448 | 146688 | 232 |
| tacticalhotkeys | 10464 | 62784 | 400 |
| uichat | 4072 | 24432 | 336 |
| uidecals | 11408 | 68448 | 496 |
| singlesound | 82744 | 496464 | 2672 |
| pathmusic | 15968 | 95808 | 384 |
| musicscript | 92744 | 556464 | 1016 |
| uitactical | 64952 | 389712 | 4416 |
| 合计 | 645344 | 3423096 | 22704 |

联合DLL每模式另有183×4=732默认根样本/2196数组比较，以及保留的40个非空completion样本/120比较；两模式实际退出码均0，183个精确公开ProcessInstance路由全部加载成功。联合smoke不代替21组完整矩阵。两份DLL在运行的 `recovered-xmlcompiler-priority-x86` artifact；本轮产物只有组件验收，没有SDK验收。

## 关键语义与本轮修复

- 引用输出index+1；bookmarks先按owner block，再按reference index/target block排序，不是按全局字段地址或全局target block排序。聊天Voice/递归菜单已以正确分组合同编写独立重定位断言，并保持原三数组逐字节对照。
- 聊天UI有递归32-byte Menu、FactionGroup12、weak message4、nullableDestination4/Close8、VoiceHelp32四字符串；XSD max9不等于底层Marshal截断，组件覆盖10/257项。
- UI贴花的ARGB packed ColorTag与三个float的RGBColor严格区分；present-empty packed color的alpha255只在节点存在时生效。
- AudioEvent先分配Attack/Sound/Decay，再执行BaseSingleSound的完整nullable range/slider/reference列表；六种track/dialog/ambient派生入口先处理Filename nullableReference。Multisound保留28-byte subsound及五个nullable scalars。
- PathMusicMapRuntime原XmlCompiler入口读取后丢弃Value，输出12-byte全零；这是真实注册入口的no-op，不是未知TypeId空fallback，更不是已恢复自定义MusicCompiler的PathMusicMap源资产。
- MusicScript的And/Or/Not和Track.Condition是强引用，不是内联递归多态。TrackTypeKey与SpecificTrackTypePlaying.TrackType是case-sensitive raw hash，不能用小写TypedAssetId替换。原拼写TimeSicneLastActiveOp保留。
- 新ObjectFilter适配器按原IValue C-string截断attrs/Thing文本，裸TypeId作为BAB元数据处理；没有改动共享helper并扩大到Weapon/GameObject源码范围。两路径完整KindOf/ModelCondition/ObjectStatus词汇、ALL/sign/NUL及分配边界均覆盖。
- 首轮40132183077发现LookAt三个百分比字段原getter默认100%，摘要正则漏了MSVC百分号转义而误设null。已核原静态symbol与完整原方法，三项均补100%，加入defaults与partial-attribute独立ABI。原矩阵不删样本，最终40132543491全通过。
- 单组脚本、artifact、evidence路径统一为xml-<group>；修正了single-sound等路径命名差异，未改变验收范围。新Native内部类型使用唯一命名空间，immutable registry沿用InitOnce/进程生命周期策略，正常退出码门槛保留。

## SDK边界与下一轮

本轮按用户要求没有发送SDK触发标记或手动dispatch SDK。当前183入口联合DLL整体没有SDK验收；旧82清单/路由虽未改，也不能把其旧DLL成功结果标到新联合DLL。新增101不正式提升为SDK支持，不替换原SDK配置或两原DLL。

组件仍接收BAB预处理后的引用/defaults/继承及数字TypeId，不重做全部预处理器；原DLL只供oracle。Joypad命名根、GameObject以及完整Tokenizer恢复继续排除。183/301是登记入口数量，不代表总代码复杂度已完成同一比例。

上一轮五根热键续接笔记现已实施，见XML_TACTICAL_HOTKEYS.md；不要重复恢复。下一轮可先核剩余的条件/目标比较、Eva/成就或健康条/叠层设置等完整依赖组，例如BaseConditionData/GameplayConditionData/ObjectConditionData/UpgradeConditionData，或TargetingBaseCompare/CompareList/Distance/InTurretArc/CombatChain。它们尚未恢复，必须先核完整原证据再定批；若用户继续禁止SDK，仍只做组件层。新增101的合法SDK样例/显式路由/真实覆盖与四产物、隔离重放应留给另一次明确允许SDK的批次，不能暗中触发。

固定原Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`、DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。复用evidence/xml-source索引/小块/静态数据；GitHub插件访问，禁止CLI clone重试及强推。大型原始证据拆成独立小文件/复用blob SHA，不让非构建证据上传阻塞代码检查点。

本轮复用原证据与共享helper、整组实现集中差分；没有可比账号额度数据，不量化节省。
