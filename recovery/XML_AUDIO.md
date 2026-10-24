# Audio/video 完整能力组（待统一验收）

目标分支 recovery/xmlcompiler-priority。组件包括 AudioEventLimitGroup、AudioLOD、AudioSettings、Video、VideoEventList，以及原 Plugin 的独立 MicrophoneSettings 入口。前五者在原 CnC3Types.xsd 中有全局资产；MicrophoneSettings 只有嵌套 schema，SDK 必须经 AudioSettings 的三个 mic 节点覆盖，不能新增全局 XSD 声明。

## 原版证据

Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`，Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`；均为原分支已固定证据。Module 每个方法可通过 evidence/xml-source/method-index.txt 及 parts/part-NNNN.cs.txt 重现。Plugin 原文件在 decompiled/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder.XmlCompiler/Plugin.cs，超过 Contents API 的1MB限制时用 Git blob API 获取。

| 类型/依赖 | Module Marshal 行号 | Tracker 根大小 | Plugin HandleType 行号 |
| --- | ---: | ---: | ---: |
| AudioEventLimitGroup | 19444 | 8 | 662 |
| AudioLOD | 51111 | 44 | 26591 |
| AudioSettings | 53162 | 384 | 28010 |
| Video | 44697 | 28 | 13512 |
| VideoEventList | 45069 | 12 | 13612 |
| MicrophoneSettings | 52975 | 48 | 27910 |

- 根 Tracker 构造器分别在88971/110902/112176/99982/100073/112085；所有保留根首字0。Plugin 统一仅 Win32 小端，0以外平台包含99走大端。Limit 没有独立 EndianCorrect 方法，但原 HandleType 内联交换@4，已保留。
- AudioSettings 根45个属性及全部嵌套依赖；bool@380/@381，两个RGBA@176/@192，slider列表@208、元素8；三个48字节mic@216/@264/@312；20字节compressor@360。Absent节点保持全0，present-empty运行原Marshal缺省；RGBA是四个float（R/G/B/A），不是打包颜色。
- MicrophoneSettings自身继承BaseAssetType，首4字节保留，11个float@4..44；四个百分比与其他real区分。VolumeCompressionSettings float@0/4/8/12及bool@16；Attack/Release按原TimeTag转换。
- AudioLOD count/pointer@32/@36，ProcessorRequirement元素8（enum@0、signed MinMHz@4），三个bool@40/@41/@42。Enum unknown回退AudioLOD=1、ProcessorType=3。源枚举表在 evidence/xml-source/enums；AudioPriority=5、AudioVolumeSlider=3、TextJustification=2、VideoSoundOverride=0，匹配区分大小写。
- Video Filename/Comment为8字节String@4/@12，Comment缺省No Comment，Volume@20百分比、SoundOverride@24。缺省仅Comment仍产生字符串分配/重定位，缺省实例40字节。
- VideoEventList count/pointer@4/@8，列表元素4字节指针；分派仅数字TypeId，不靠标签名。MovieEvent=3074724042（16字节）、TitleCardEvent=866596483（24字节）、未知/缺失TypeId分配12字节基类并写0。Base两个nullable FadeData指针@4/@8，元素28（signed FrameCount@0、两个inline Color3f@4/@16）。Movie引用@12先于base；Title display-time@12和TextInstance列表@16先于base。每个TextInstance44字节，TextId为原Utils.Tokenize按空白分割的String列表，保留重复项，Font是String；TextColor为小写r/g/b的Color3f。
- Video派生/enum及列表方法在72419..72598/86754；TextInstance Marshal44942、Endian45016；FadeData44766；Color3f18951、RGBA18780；时间原方法18375；String列表85247及Tokenize18262。保存的字段和枚举清单与这些证据对应，不调用原DLL实现。

## 接线和验证

新增 Native/Xmlaudio.cpp、Xmlaudio.h、XmlaudioFields.inc、XmlaudioEnums.inc；Bridge/XmlaudioCompiler.cpp；tests/XmlaudioOracle.cs、XmlaudioCases.cs；scripts/TestXmlaudio.ps1。Catalog为 catalog/XmlaudioTypes.json；SDK合法源fragment为 sdk/XmlaudioProbe.fragment.xml（11个资产，真实inheritFrom、本地MovieAsset引用和两层列表，MicrophoneSettings经嵌套覆盖）。

Windows完整验收命令 `recovery/scripts/TestXmlaudio.ps1`，包含/Od和/O2，平台0/1/2/99，ProcessInstance原DLL三数组逐字节及CompileXml直接对照；全部根缺省独立字节布局，所有scalar边界/枚举合法和未知值，长列表跨过28/29/30的排序边界，nullable缺省，256组复合样本及拒绝边界。不削减其他组矩阵。证据会落在 evidence/xml-audio/validation.json 及两模式日志；本提交仅本地g++语法通过，未取得Windows或SDK成功，不标已验收。

桥接保留未限定TypeId（结构节点允许BAB注入，event节点保留数字分派），foreign namespace/qualified属性、未知内容、重复singleton、异常文本明确拒绝；未知VideoEvent仅支持原基类字段。原DLL仅用于oracle，SDK预处理/继承/引用仍交给BAB。

