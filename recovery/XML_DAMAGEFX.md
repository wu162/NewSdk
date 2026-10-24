# DamageFX / DamageFXSettings recovery

状态：实现与完整组件差分矩阵已准备，尚未 Windows /Od、/O2 验收；SDK 待联合验收。独立恢复 XmlCompiler 的两个入口，不宣称恢复 Tokenizer 独立入口，仍接收 BAB 预处理后的 XML。

## 固定证据

原 XmlCompiler Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`，Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`，DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。用既有 evidence/xml-source 小块，未重复导出大型反编译。

- Plugin `HandleType<DamageFX>` 行3969起：root Tracker，FXGroup 列表@4，再调用列表 EndianCorrect。平台非 Win32 一律 big endian（包含99）。
- Plugin `HandleType<DamageFXSettings>` 行31355起：GroundDamageFX 是 nullable AssetReference**@4；present-empty 仍 Push(4,1)，只在字符串有反斜杠时添加 reference，最后没有额外 scalar endian。
- Plugin ProcessInstance dispatch：DamageFX 2905958645；DamageFXSettings 75989905。ExtendedTypeInformation 对应 TypeHash 分别187609259、2052546860。
- Module Tracker<DamageFX> 行91883（part-0183）：Allocate(1,12)；Tracker<DamageFXSettings> 行114997（part-0229/0230）：Allocate(1,8)。
- Module Marshal(DamageFXGroup) 行28423，EndianCorrect 行28456（part-0056）。Type@0 默认DEFAULT；Amount float@4 默认0.0；ThrottleTime@8 默认0s；十个FXList直接强引用@12..48，顺序为 Major、Minor、Rear、Side、Top，再 Veteran 同顺序。
- Module DamageFXType 枚举 Marshal 行64767：114-entry map，未知 token fallback35（ALIEN_BOMB），DEFAULT=-1。完整枚举取自 evidence/xml-source/enums/DamageFXType.json（静态原map行12334）。原strcmp按C字符串终止，包含NUL边界测试。
- Module DamageFXGroup 列表 Marshal 行64778，EndianCorrect 行64820（part-0129）：52-byte stride；一次分配全部元素再逐项 marshal，count在递归后交换。组只交换前三个32位字；references 和 pointers 由 FinalizeTracker 处理。
- schemas AssetTypeDamageFX.xsd / System/DamageFXSettings.xsd 与 CnC3Types.xsd 已核对：两根均为全局资产；FXGroup unbounded，GroundDamageFX optional。底层组件不假设 XSD 的 scalar 输入过滤。

## 实现边界

新源仅使用已验收 Tracker / ScalarParsing / DataInitializers / ResourceMarshaling。无原 DLL 回调。根允许 id、inheritFrom，节点允许 BAB 注入的裸 TypeId。未知/限定属性、外命名空间、未知子节点、组内子节点、非空文本、TypeId与根名不符均显式拒绝。Comments/whitespace保持兼容，未重做BAB引用、defaults和继承预处理。

## 测试与接线

`scripts/TestXmldamagefx.ps1` 在 /Od 与 /O2 各运行平台0/1/2/99；`tests/XmldamagefxOracle.cs` 每样本比较原 ProcessInstance 和恢复 ProcessInstance、CompileXml 三数组，共6数组比较。114个枚举及大小写/空/未知/空白、Amount完整数值边界、Time单位与非法输入、10个reference字段及nullable的全部引用边界、NUL/ANSI、0/1/2/3/27/28/29/64/257项列表，配独立偏移、缺省、bookmark增长、import index+1、relo/import哨兵与显式拒绝。测试未删减原组覆盖。

新公开组类 `Recovered.XmldamagefxCompiler` 及 `DamageFXCompiler`、`DamageFXSettingsCompiler`。组件源清单在脚本；联合DLL追加 Native/Xmldamagefx.cpp、Bridge/XmldamagefxCompiler.cpp。Plugin/catalog统一接线由集成代理完成；新 metadata 是 catalog/XmldamagefxTypes.json，合法SDK片段在 sdk/XmldamagefxProbe.fragment.xml（包含FXList依赖、两根缺省/非缺省/实际继承）。

最终是否验收必须以统一运行 sourceCommit/runId、validation.json 和实际退出码判断，不能以本实现文档代替成功结果。
