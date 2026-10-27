# XmlCompiler：范围内剩余还原完成

2026-10-25（北京时间），原工作分支 `recovery/xmlcompiler-priority`；通过PR [#2](https://github.com/wu162/NewSdk/pull/2) 合入 `recovery/compiler-source`。

## 当前范围

原Plugin登记301个入口，**排除12个独立根后，范围内289个入口已完成组件还原**。本轮补齐最后两根：GameMap、UIConfigList。不是301全部恢复。SDK另已完成267全局根验收，22个非全局登记边界见末节。最终组件联合检查：completed/success：源码4401069feaf5df91043afadaad2410293c33b67e，运行40159834447；289路由全部预检，UIConfigList选择性smoke两模式四平台真实退出0。

- GameMap：完整80字节根、8种列表、Environment12、WorldDict56的7类属性对、MapAnalysis28；见[XML_GAME_MAP.md](docs/components/xml/XML_GAME_MAP.md)。验收源码 `092bb95c06647f95891c7f62b4b6fbc875c4c07a`，运行[40158416072](https://github.com/wu162/NewSdk/actions/runs/40158416072)。该运行组件62组和289入口联合smoke成功，UI当时测试宿主未通过，不能把整次运行称为整体成功。
- UIConfigList：完整70个内部分派（47个专用布局、288个字段，23个保留真实尺寸的基类分支）及完整嵌套依赖；见[XML_UI_CONFIG.md](docs/components/xml/XML_UI_CONFIG.md)。验收源码 `4401069feaf5df91043afadaad2410293c33b67e`，运行[40159834447](https://github.com/wu162/NewSdk/actions/runs/40159834447)。独立ABI覆盖所有70分派、直接/嵌套字段、37条嵌套列表增长路径、全部适用枚举/位集、NUL/ANSI及拒绝边界。
- 以上两组均完成/Od、/O2、平台0/1/2/99，原ProcessInstance三缓冲区逐字节差分、直接CompileXml对照、独立ABI与显式拒绝。数组比较包含直接对照，不能全部称为独立原DLL调用。

| 本轮新增完整根 | Fixtures（两模式） | 数组比较 | 显式拒绝 |
| --- | ---: | ---: | ---: |
| GameMap | 30736 | 184416 | 592 |
| UIConfigList | 171184 | 1027104 | 2160 |

正式验收索引：[xml-component-acceptance-20261025-289.json](evidence/xml-component-acceptance-20261025-289.json)。完整301项静态登记/路由审计：[xml-coverage-20261025-289.json](evidence/xml-coverage-20261025-289.json)，静态审计不替代运行验收。此前287入口基线仍保留在0f0ab2fce098fef2d78d6a165f3f5ce86c591077/运行40142560815及其验收文件。

## 已验证组不重复触发

用户本轮新增要求：不要触发已经验证过类型的测试。工作流现依据 `SelectXmlComponentGroups.py` 对照成功验收sourceCommit与实际源码、测试及递归依赖。未变且已通过的组复用原sourceCommit/runId；缺失、失败、不完整、非祖先证据或输入变化才重新选择。包含验收时和当前依赖集合，删除文件不会造成误跳过；带数字的w3d同样参与。选择器26项临时Git仓库测试通过。

最终运行实际只选择uiconfig，复用其余62组，联合差分仅选择UIConfigList。所有选中组保留完整矩阵；联合DLL始终预检289个公开路由，但不对未选根调用原DLL。联合构建在受影响组件通过后才启动，输入未变则复用。最新选择性smoke不能冒充289个根全部重新做默认样本；完整289根smoke锚点为092bb95c/40158416072，最新代码的全部路由预检及受影响UI根smoke另有本轮精确证据。

## 原范围排除12根

`TestGameObject`、`UIJoypadCommandBarMixedGroup`、`UIJoypadCommandBarHomogenousGroup`、`UIJoypadCommandBarCoopCommunicator`、`JoypadCommandBarButtonTemplate`、`UIJoypadCommandBarStances`、`JoypadCommandBarTemplate`、`UIJoypadCommandBarTopMenu`、`UIJoypadCommandBarPlayerPowers`、`UIJoypadCommandBarButtonBuild`、`UIJoypadCommandBarControlGroups`、`UIJoypadCommandBarSingleUnit`。

11个Joypad命名独立根和TestGameObject没有计入完成。UIConfigList内部带Joypad名称的组件是该根的必要依赖，未新增这些独立根的支持。GameObject源码及另会话compiler-source分支未改。只有未来明确扩展范围时才恢复上述排除项。

## 关键合同和本轮修复

- IValue按Windows ANSI、首NUL的C字符串合同处理；字符串payload不改UTF8。UIDynamicConfig.Name使用保留大小写的FastHash；Type缺省0、显式未知2。ComponentList按数字BAB TypeId分派，未知内部ID保留原8字节基类回退/runtime0；未知根仍拒绝。
- GameMap的MapMetaData子节点原本是pipelineOnly且不写根；AssetIdListProperty保存ANSI字符串记录，不是hash/import。USP派生字符串先于基类；引用按owner block和无符号资源index排序，index=-1排在该owner末尾。首轮独立排序断言错误已修正，未修改实现迁就预期。
- UI的PercentageTag默认字符串1.0原样缩放为0.01，位0x3c23d70a；独立测试保留该单位合同，不能按普通float预期1.0。原测试宿主CS0136变量重名和此预期问题均已修复，样本未删。
- UI ObjectFilter适配层对Rule/Alignment显式截至首NUL，修复ANY加NUL尾部原2/恢复1的实际差异；共享ObjectFilter.cpp未改。其他hash、位集及字符串沿用已验证的首NUL实现。ObjectStatus223个命名值，七个word的ALL包括padding位。
- packed ColorTag仅在子节点存在时应用A255，缺失子节点保留0；RGBColor为独立float3。所有70分派保留原默认、布局、分配顺序和RuntimeTypeId，不按XML标签猜测类型、不用半根或空根计完成。
- 原先287入口的ALL不推进token、Time舍入、强引用index+1、ANSI和文件blob合同保持不变；本轮未改共享解析/重定位/helper。历史失败、修复、并发父提交和Actions产物保留；一次旧证据回写rebase冲突不作为运行成功证明。

## 当前SDK验收与边界

用户随后授权真实SDK，并限定EALAModStudio建立基础数据case5 BAB。最终源码 `9b2ef85bc45e6b397fce3d275165715afa921a92`，运行[40164710673](https://github.com/wu162/NewSdk/actions/runs/40164710673)，completed/success；收据 [acceptance.json](evidence/xml-priority-sdk/acceptance.json)，详细说明[XML_SDK_RECOVERY.md](docs/components/xml/XML_SDK_RECOVERY.md)。当前289路由metadata四平台全部对照；267个schema全局根的真实SDK调用并集通过，22个非全局/抽象/runtime登记项不宣称独立SDK根调用。原12个排除根不变。

五profiles合计783次实际调用、原版和无原DLL隔离重放4698次三缓冲区比较，各profile四产物原/恢复相同。Baseline159/A173共332次旧调用保留原锚点与完整reuse guard，不重跑；最终只新跑B277、FullMymod162、APT12三个profile。APT四根专项8个非空文件/4个空文件，236个payload字节与源文件一致。原FullMymod7个Includes完整保留。

SDK阶段仅修正插件对合法根metadata的输入边界与测试环境绝对路径一致性，没有修改Native/Bridge，没有为SDK重跑未变成功组件组。原/恢复在同一SDK绝对路径顺序编译，保留未经归一化的四产物逐字节比较；文件blob有真实非空读取证明。SDK结果仅/O2、Win32，组件/Od/O2和四平台另列，不扩大平台声明。

原82入口SDK清单仍保持92180f9bbd830b9dbd49a696c259e54ffe2d7721/40098695436历史锚点，207项新增组件账本不冒充当前SDK状态；当前状态以catalog/XmlSdkTypes.json及新收据为准。本范围组件还原及可直接声明的267全局根SDK验收已收尾；22个非全局入口若需独立生产路径证明，须先明确实际schema/自定义编译链可达性。尚非完整Tokenizer或非XML自定义APT/Audio/Map/Texture编译器替代。

固定Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`、原DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。所有失败/修复/Actions证据及并发父提交保留，后续按受影响输入选择验证，禁止强推。

## 合并历史

首批优先类型此前通过PR [#1](https://github.com/wu162/NewSdk/pull/1) 合入，合并提交dddb9ac2804da1380a5b9b94a9035253c6e48f63。本次PR #2保留两边历史并带入后续完整还原及真实SDK验收；当前GameObject与XmlCompiler各自状态见RESUME.md。
