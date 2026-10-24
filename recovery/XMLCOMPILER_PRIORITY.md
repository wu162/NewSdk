# XmlCompiler：当前续接入口

更新：2026-10-24（北京时间）。目标分支 `recovery/xmlcompiler-priority`。本轮最多三个子代理并行，完整能力组统一验收；代码与工作分支父历史已全部合入本分支，禁止强推。

## 2026-10-25 本轮冻结，待完整组件验收

用户本轮要求一小时、最多三个子代理，**禁止触发SDK真实编译**。七组候选新增101入口（战术热键5、聊天3、贴花4、单音效10、PathMusic7、MusicScript21、战术UI51），联合组件候选183。现统一运行21组完整/Od、/O2四平台差分与183入口联合DLL；未经最终sourceCommit/runId证明不计入组件成功。

新增清单在catalog/XmlComponentOnlyTypes.json，所有新增条目sdkValidated=false。现有catalog/XmlRecoveredTypes.json、主Plugin/SDK路由/原SDK及SDK工作流保持不变；旧82类SDK成功仅为历史基线，新联合DLL本轮没有SDK验收。不发送SDK触发标记，不手动dispatch SDK。

## 上轮已验收结果

**原 Plugin 301 个登记入口中，已恢复并验收 82 个独立编译入口；本轮新增 30 个，上轮为 52 个。** 80 类是原 CnC3Types.xsd 全局资产；GameStatsModeData、MicrophoneSettings 仅有独立 Plugin 入口及嵌套 schema，通过组件和合法嵌套 SDK 场景覆盖，未修改原 XSD。

- 最终组件/联合 DLL 源码：`e7580311f36811265e6ed5af5a184ef623f31284`。
- 十四组完整差分与联合 DLL：[运行40099095351](https://github.com/wu162/NewSdk/actions/runs/40099095351)，completed/success。
- 真实 SDK：`92180f9bbd830b9dbd49a696c259e54ffe2d7721`，[运行40098695436](https://github.com/wu162/NewSdk/actions/runs/40098695436)，completed/success。
- SDK 成功后仅修正 UI Oracle 的独立排序断言。两锚点的整个 recovery/src、recovery/sdk、recovery/scripts、SDK、.github 子树以及 XmlPrioritySdkOracle.cs 的 Git SHA 完全相同，因此复用同一 SDK 验收；精确 SHA 和各项门槛见 `evidence/xml-priority-acceptance.json`。
- 已核最终 validation.json 的 sourceCommit/runId、实际作业日志、两优化模式与平台0/1/2/99、联合进程真实退出码及 SDK 两套隔离重放。之后的纯文档提交不改变验收源码。
- 三个工作分支与集成检查点均保留父提交并合入本分支，无强推；没有合入另一会话的 compiler-source。

## 本轮新增完整能力组

| 组 | 入口数 | 完整范围 |
| --- | ---: | --- |
| damagefx | 2 | DamageFX、DamageFXSettings |
| hotkeymaps | 4 | GeneralHotKeySlots、DefaultHotKeys、DebugHotKeys、MappableKey |
| audio | 7 | AudioEventLimitGroup、AudioLOD、AudioSettings、Video、VideoEventList、MicrophoneSettings、BootupDisplaySequence |
| uisettings | 14 | HotKeySlot、MouseCursor、BasicMouseCursors、UIMouseDragSelectionFeedback、UIFloatingTextDisplay、UIMessageDisplay、UIMilitaryCaptionDisplay、UISubtitleDisplay、UITimerDisplay、DynamicGameLOD、GameLODPreset、StaticGameLOD、UICoopTinting、RadiusCursor |
| soundmaps | 3 | AnimationSoundSettings、MiscAudio、LargeGroupAudioMap |

前52入口的九组完整能力继续回归：原十四优先类，加 progression12、environment12、players14。精确 TypeId/TypeHash/schemaGlobal 与分组清单见 `catalog/XmlRecoveredTypes.json`。新五组详细布局与拒绝边界见 XML_DAMAGEFX.md、XML_HOTKEYMAPS.md、XML_AUDIO.md、XML_UISETTINGS.md、XML_SOUNDMAPS.md。

## 最终完整差分矩阵

每行合计 /Od、/O2，均覆盖平台0/1/2/99。数组数包含适用的直接 CompileXml 对照，不能全部称作独立原 DLL 调用次数。没有减少旧组样本或放宽断言。

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
| 合计 | 362992 | 1728984 | 12984 |

联合 DLL 每模式另有82×4=328默认根样本/984数组比较，以及40个非空completion样本/120次比较。/Od、/O2实际进程退出码均为0，82个精确公开 ProcessInstance 路由全部加载成功；联合 smoke 不替代组矩阵。DLL 在运行的 `recovered-xmlcompiler-priority-x86` artifact。

## SDK 验收

159个固定源资产覆盖80个全局类型，包括缺省、非缺省、原版允许的实际继承、本地强引用、多层列表和嵌套结构。Reduced 实际159次恢复插件调用，原版与隔离恢复版分别477次三数组比较；FullMymod 保留全部7个原 Includes，162次调用、分别486次比较。两profile的 bin/manifest/imp/relo 四产物均逐字节相同，imports非空；同一/O2 DLL与插件复用，两原DLL指纹保持不变。隔离重放目录物理排除原 XmlCompiler/Tokenizer DLL，并检查进程未加载这两个程序集。82入口metadata在四平台 Initialize/ReInitialize 对照4608项，另8个未知类型拒绝。详见 XML_PRIORITY_SDK.md。

## 本轮关键合同与修复

- DamageFX列表stride52，Type缺省DEFAULT=-1，未知fallback35；GroundDamageFX为nullable reference**，present-empty仍分配4字节。
- Hotkey Modifiers保留原BitFlags的unknown ALT、负号XOR、ALL截断行为；MappableKey present-empty KeyDef仍分配字符串。
- MicrophoneSettings、FontDesc等具名结构兼容BAB注入的裸TypeId；mic的id在原schema必填，不能因嵌套而忽略。
- UI浮动速度、Radius速度和LargeGroupAudioMap MaximumAudioSpeed按已核实原常量0.2f缩放；不能按常见游戏帧率猜测。
- MiscAudio的95个direct reference字段逐项核原调用与偏移；LargeGroupAudioMap为48字节根，SoundKeyPair stride24/DuckInfo stride12，完整位集、raw StringHash与小写TypedAssetId分开处理。
- 原EnumLookup/TimeTag/Tokenize按C字符串在NUL终止；新增DOM构造NUL、ANSI等边界差分，恢复代码不使用原DLL。
- 引用输出是index+1；Tracker bookmark按源block和value排序，不是按字段地址排序。原版QuickSort同键行为继续由完整三数组差分覆盖；独立ABI检查保留精确字段集合、排序合同、边界和哨兵。
- SDK原XSD不允许继承的根不造inheritFrom样例。FXList必须有NuggetList；合法继承样例仍先满足原XSD必填项。两次独立UI断言错误和首轮schema样例错误已修复，历史失败运行保留，不能将其冒充成功。
- 新Native内部类型使用唯一命名空间，动态不可变表保留InitOnce/进程生命周期所有权，未放宽正常退出码门槛。

## 边界与下一轮

这是82个独立入口的部分替代，仍非完整301入口或完整Tokenizer。Joypad命名根继续排除；BootupDisplaySequence根不是Joypad命名根。GameObject不在本轮源码修改范围。编译器仍接收BAB预处理后的defaults、继承、资源引用与数字TypeId；未知/不支持内容明确拒绝，不重做整个BAB预处理器。原SDK配置和原DLL未替换，切换脚本只针对指定SDK副本，其他类型仍走原处理器。

下一完整能力组已只读取证：五个战术UI热键根，依赖已恢复HotKeySlot，无Joypad依赖。见 `XML_TACTICAL_HOTKEYS_NEXT.md` 的精确方法、布局、枚举fallback、47引用字段与SDK必填规则；**尚未实现或验收，不计入82**。继续前核远程分支头，按完整组补源码/测试后统一差分，达到集成里程碑再SDK验收。

固定原Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`、原DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。复用 evidence/xml-source 的索引、小块和静态字节。Plugin超过1MB，contents接口可能返回空content；需要全文时可用GitHub插件fetch blob API，先本地筛选再展示，勿输出整份大文件。CLI无法克隆本仓库，继续使用GitHub插件。

本轮复用原源索引、共享helper和已证明的SDK输入，集中修复后完整验收；未量化账号额度节省。
