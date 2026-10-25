# Keyboard mapping asset group

最终验收（2026-10-24）：组件/Od、/O2四平台及联合DLL运行40099095351成功（源码e7580311f36811265e6ed5af5a184ef623f31284）；SDK双profile和无原DLL隔离重放运行40098695436成功（源码92180f9bbd830b9dbd49a696c259e54ffe2d7721）。两锚点的编译器与SDK输入相同，后续仅UI Oracle断言修改，详见XML_PRIORITY_SDK.md及evidence/xml-priority-acceptance.json。 本组两模式合计13144 fixtures、78864数组比较、296显式拒绝。

状态：完整组件与联合SDK已验收，精确锚点见下。

## 原版证据与 ABI

固定 Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`、DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。重用 evidence/xml-source 小块，不下载整份原反编译。

| 根 | TypeId | TypeHash | 大小 |
| --- | ---: | ---: | ---: |
| GeneralHotKeySlots | 417176643 | 655411104 | 12 |
| DefaultHotKeys | 3741098742 | 1229544125 | 12 |
| DebugHotKeys | 3359870461 | 73026796 | 12 |
| MappableKey | 1350608344 | 2226564801 | 12 |

- Plugin HandleType<GeneralHotKeySlots> 行15880：TakeScreenShot 直接HotKeySlot强引用@4，ReloadRapidIterationFeature@8，均无额外 scalar endian。
- Plugin HandleType<DefaultHotKeys> 行12792 / DebugHotKeys 行12892：调用 Module 的整根Marshal，随后列表EndianCorrect。Module Marshal(DefaultHotKeys) 行44236 / DebugHotKeys 行44264（part-0088）：选Map singleton，其HotKey列表写入root@4（count@4, pointer@8）。
- Module Marshal(HotKeyDef) 行44221：12字节项，Slot强引用@0，Key(MappableKey)强引用@4，Modifiers BitFlags<3>@8。列表Marshal 行71686、EndianCorrect 行71728（part-0143）：一次分配12*count，按输入顺序；仅交换Modifiers word及root count，引用/指针由FinalizeTracker处理。
- Module Modifiers helper 行86635（part-0173）：CTRL=0、ALT=1、SHIFT=2；未知/大小写不符fallback=1(ALT)。负号执行XOR，不是清零。ALL大小写不敏感、忽略符号、填满32位，且原helper不再前进token指针，后续token无效。复用已验证MarshalEnumBitset，保留padding bits和NUL/isspace规则。enum静态字节证据在enums/HotKeyModifier.json行12342。
- Plugin HandleType<MappableKey> 行12575：KeyDef String<char>@4，存在空值仍分配NUL字符串，缺省保持zero pair；big endian内联交换length@4，pointer@8由Tracker交换。
- Tracker构造原方法 Module行99163 / 99345 / 99436 / 101984（part-0198、0203/0204），根均Allocate(1,12)。
- 每根Plugin都用平台!=Win32决定big endian，unknown99同样交换；dispatch与ExtendedTypeInformation已分别核对。
- CnC3Types.xsd允许四根全局资产。GeneralHotKeySlots继承BaseAssetType无inheritFrom；其他三根BaseInheritableAsset。SDK Default/Debug要求Map存在、HotKey要求Slot和Key；组件仍保留原Marshal对缺省/缺属性的底层能力，两层测试不混同。

## 实现与测试

Native/Xmlhotkeymaps.cpp只调用独立恢复的Tracker、ResourceMarshaling和EnumBitset，无原DLL回调。Bridge/XmlhotkeymapsCompiler.cpp公开XmlhotkeymapsCompiler和四个根名Compiler。未知元素/属性、限定属性、外命名空间、非空文本、重复Map、HotKey子节点和root/handle不符显式拒绝；裸TypeId接受BAB注入。输入引用、继承和schema默认值仍由BAB提供。

scripts/TestXmlhotkeymaps.ps1：/Od、/O2各自四平台0/1/2/99完整运行。tests/XmlhotkeymapsOracle.cs每样本原ProcessInstance与恢复ProcessInstance、CompileXml三数组各比较，共6数组。两种Map全部Modifiers样本及两两组合、全部引用字段、NUL/ANSI/overflow引用、字符串缺省/空/多字节/对齐/长串、0/1/2/3/27/28/29/64/257项完整列表与bookmark增长、独立ABI及显式拒绝均覆盖。不替换旧能力组矩阵。

catalog/XmlhotkeymapsTypes.json供统一catalog/plugin接线。sdk/XmlhotkeymapsProbe.fragment.xml使用uisettings的RecoveryUiHotKey强引用，包含全部四根schema合法default/nondefault、两个Map真实继承、MappableKey继承覆写。原SDK配置不改；联合SDK负责最终XSD/原恢复同输入/产物/无原DLL重放。

最终源码验收以实际sourceCommit/runId、validation.json、两模式退出码和联合SDK结果为准，本文件不构成成功声明。
