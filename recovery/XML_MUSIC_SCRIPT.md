# MusicScript condition / track family — component only

完整21个原XmlCompiler独立入口：MusicScriptConditionNuggetBase、MusicScriptConditionExpensiveNuggetBase、18个具体条件/逻辑节点及MusicScriptTrack（精确列表与TypeId/TypeHash见catalog/XmlmusicscriptTypes.json）。两Base是原schema抽象、无全局声明，schemaGlobal=false，其余19global。本轮仅组件恢复，不运行SDK、不改活跃82入口插件路由，不称SDK已支持。

## 原证据和完整布局

Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`；Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`。原Module方法集中29580..30322（evidence/xml-source/parts/part-0059/0060），原Plugin各HandleType内联补足简单节点和Endian；原Tracker构造92156..94066，逐个核Allocate大小。字段表在XmlmusicscriptFields.inc，均保留首word0，不按schema外观推测省略字段。

- Base4无数据；ExpensiveBase8含TimeBetweenConditionChecks@4缺省5s。LocalPlayerIsObserver8含CountDeadPlayersAsObservers byte4；AnyTrackPlaying4无数据；TimeFromStartOfLevel8含Timeout Time@4。
- UnitsFarFromBase20：ExpTime4，signed MinUnitsToPass8、real MinDistanceFromBase12、4位Relationship16，unknown fallback0。LocalPlayerKillRatio20：ExpTime4、Time8、real Ratio12、ComparisonOperator RatioOp16（unknown2）。
- TrackPlayedCount12：Track为小写TypedAssetId@4、signed Count8。SpecificTrackTypePlaying8：TrackType是原case-sensitive StringHash@4，**不小写**。PathMusicStateCurrentlyActive8含小写TrackedState@4；PathMusicStateLastActive20含小写TrackedState4、StateChange8（unknown0）、TimeSinceLastActive12、原拼写**TimeSicneLastActiveOp** ComparisonOperator16（unknown2）。
- EvaEventPlayedRecently12：direct EvaEvent strongRef4、Timeout Time8缺省1s。Not8：singleton Condition节点文本的direct strongRef@4；Or/And12：Condition count/pointer@4/@8，stride4 strongRef。所有逻辑关系是资产引用，**不是递归内联多态**，不调用原编译器或假装编译目标资产。
- ScoredKillCount20：signedCount4、Time8、nullable ObjectFilterAsset reference**@12、bools CountOnlyKillsAgainstTheLocalPlayer16/CountOnlyKillsByTheLocalPlayer17。HitBySuperWeapon24：小写TrackedWeapon4、Time8、signedCount12、nullable TargetFilter reference**16、CountOnlyHitsAgainstTheLocalPlayer byte20。
- EngagementCount28：ExpTime4，signedCount8、Time12、ScreenStatus16（unknown1）、nullable TargetFilter reference**20，bools CountOnlyEngagementsAgainstTheLocalPlayer24/CountOnlyEngagementsByTheLocalPlayer25。nullable引用存在属性就分配4，即使空文本；不误分配整个ObjectFilter。
- ObjectsOfTypeExist136：ExpTime4、signedCount8缺省1、inline15-word RequiredModelConditions@12及ExcludedModelConditions@72（原456-bit，unknown233，ALL保留padding），nullable **ObjectFilter112结构**指针@132。ObjectsNearEvaEvent24：ExpTime4、directEvaEventRef8、signedCount12缺省1、realDistance16、nullable ObjectFilter112结构指针@20。两条路径完整复用已有MarshalObjectFilter/EndianObjectFilter，保留status指针及Thing列表的原分配次序。
- MusicScriptTrack36：TrackTypeKey raw StringHash4（缺省空），signedLevel8/Priority12，nullable Condition reference**16，directBaseAudioEventInfo Track reference20，Timeout列表count/pointer24/28（stride8：**signed**Weight0缺省1000、Duration Time4），bool ConditionsAreLatch32缺省false、FadeInTrack33缺省false、FadeOutLowerLevelTrack34缺省true。Condition块先于Timeout数组分配。

原Plugin只有Win32走小端，其余包括99走大端；全部根/inline数组/指针payload和继承base按原Endian路径处理。原Track和SpecificTrackType使用FastHash原始大小写/ANSI有符号字节语义；TypedAssetId才小写。RawHash复用此前Soundmaps已核原算法的独立代码，未修改公共helper。

## Filter和C字符串边界

Filter只允许原ObjectFilter的id、Rule、Relationship、Alignment、Include、Exclude、StatusBitFlags、StatusBitFlagsExclude与IncludeThing/ExcludeThing列表；裸TypeId仅供BAB结构注入，不传给公共helper。调用前将每个attribute value按std::string(v.c_str())截断，Thing文本同样截断，避免旧公共Enum helper的std::string比较把NUL后尾部误作枚举内容；不修改共享helper扩大Weapon/GameObject回归范围。Filter对象和其status/Thing payload由原Tracker管理，不搬运另一Chunk字节。

## 验证

TestXmlmusicscript.ps1编本组+Relo/Scalar/Resource/EnumBitset/ObjectFilter，三份C# Oracle/Cases/FilterCases。完整/Od、/O2，平台0/1/2/99，原ProcessInstance三数组逐字节和CompileXml另三数组。全部21根缺省独立字节ABI；所有字段/原枚举值/未知回退/nullable/ref/原CRT和TimeTag/ANSI/DOM NUL；所有457 ModelCondition枚举（两个inline字段）、284 KindOf（两Filter路径的Include/Exclude）、223 ObjectStatus（两路径status正/排除）、Rule/Relationship/Alignment及NUL后缀/leading NUL；两Filter完整Thing/status分配，长度跨28/29/30/31/32/65/129；逻辑/Timeout长列表；每根128轮完整组合及显式拒绝。

本地g++ -Wall/-Wextra/-Werror语法及三C#词法结构检查通过。Windows完整矩阵仍待主代理统一执行，证据目录evidence/xml-musicscript/validation.json与两优化日志；最终须核sourceCommit/runId和实际终态。此未验证检查点不计入已验收数量，无任何真实SDK编译、SDK输入或正式路由变更。

