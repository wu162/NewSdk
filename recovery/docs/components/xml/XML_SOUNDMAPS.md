# Sound maps: three-root group

最终验收（2026-10-24）：组件/Od、/O2四平台及联合DLL运行40099095351成功（源码e7580311f36811265e6ed5af5a184ef623f31284）；SDK双profile和无原DLL隔离重放运行40098695436成功（源码92180f9bbd830b9dbd49a696c259e54ffe2d7721）。两锚点的编译器与SDK输入相同，后续仅UI Oracle断言修改，详见XML_PRIORITY_SDK.md及evidence/xml-priority-acceptance.json。 本组两模式合计24448 fixtures、146688数组比较、232显式拒绝。

状态：完整组件与联合SDK已验收，精确锚点见下。

## 原证据与范围

固定原 Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`、DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。复用evidence/xml-source中的原方法小块，源码与独立QA未重下整份反编译。

| 根 | TypeId | TypeHash | 根大小 |
| --- | ---: | ---: | ---: |
| AnimationSoundSettings | 1302035577 | 3170370516 | 8 |
| MiscAudio | 2070603733 | 1484343325 | 384 |
| LargeGroupAudioMap | 534008255 | 1045439046 | 48 |

- MiscAudio：Module Marshal行18619，parts/part-0037和0038，完整95个AudioEventInfo强引用@4..380；Fields.inc逐项保留原schema名称与offset，无重排。独立QA匹配全部95个原symbol和表项，没有遗漏或额外字段。Tracker part-0177 Allocate(1,384)；Plugin对应HandleType/dispatch/ExtendedTypeInformation由实现代理核对，直接references仅由Tracker Finalize交换。
- AnimationSoundSettings：Module Marshal行19239（part-0038），MinMicrophoneDistanceToDirty@4是strtol(base0) signed32；缺省零。Tracker part-0177 Allocate(1,8)，big endian交换@4。
- LargeGroupAudioMap：Module Marshal行43794、EndianCorrect行43843（part-0087）。nullable ModelCondition[15]@4/@8，ObjectStatus[7]@12/@16，均按存在顺序分配，present-empty依然分配。Size uint32@20默认1000，HandOffModeTime float@24默认1s，MaximumAudioSpeed float@28默认100经速度单位换算。速度保留原float字面量0.2f提升double后乘已round的float，再转float。
- LargeGroup StartThreshold/StopThreshold@40/@42是strtoul(base0)后ushort截断，各自交换16位，IgnoreStealthedUnits@44默认true；padding@45..47保持零。Sound list header@32（count/pointer）。Tracker part-0197 Allocate(1,48)。
- SoundKeyPair24（Module part-0087行43777）：Name TypedAssetId hash@0（ASCII lower），Sound AudioEventInfo强引用@4；Key原始StringHash列表header@8，DuckInfo列表header@16。先分配全部24字节items，每项先分配keys再ducks，与XML交错次序无关。列表Marshal/Endian在part-0142。
- DuckInfo12（part-0087行43756）：AudioMap强引用@0，SoundRef TypedAssetId lower hash@4，VolumeMultiplier percentage@8。列表Marshal/Endian在part-0142，scalar交换@4/@8。
- Key为RAW大小写保留FastHash，不是TypedAssetId lower。原StringHashTag值helper part-0130，列表helper part-0142；FastHashInternal原算法行17607（part-0035），单字符A/a分别3701560732/1214652683。TypedAssetId SoundKeyPair原helper part-0172，先原CharToLower再FastHash；保留NUL/ANSI规则。
- Nullable bitsets使用既有完整EnumBitset帮助函数，unknown fallback、负号XOR、ALL符号/截断后续token和padding均保留。平台非Win32含99全部big endian。
- XSD CnC3Types全局三根已核。MiscAudio/AnimationSoundSettings是BaseAssetType无inheritFrom；LargeGroupAudioMap为BaseInheritableAsset，Sound与Key、两threshold在schema必需。组件支持底层Marshal缺省/空输入，但SDK用合法schema输入。

## 独立QA、测试与SDK

QA纠正RawHash移动指针错误const及TypeId常量namespace不可见两处语法错误，原生本地语法检查通过（x64临时副本，仅去掉x86 static_assert；不是Windows验收）。实现只使用独立恢复公共helper，不调用原DLL。

tests/XmlsoundmapsOracle.cs配scripts/TestXmlsoundmaps.ps1在/Od和/O2各自跑四平台0/1/2/99。每样本原ProcessInstance对恢复ProcessInstance、CompileXml三数组共6比较；95refs每字段完整引用/overflow/NUL边界、全部ModelCondition/ObjectStatus词和4个nullable字段、所有scalar/单位/bool/ushort溢出、Raw/Typed hash大小写与ANSI、Sound/Key/Duck 0/1/2/3/27/28/29/64/257增长、跨项分配顺序、独立偏移/import index+1/padding/哨兵和显式拒绝均完整覆盖。未知属性/元素、限定属性、外命名空间、非空结构文本及Root/Handle不符显式拒绝；Key允许text但不允许元素子节点。

catalog/XmlsoundmapsTypes.json供统一接线。sdk/XmlsoundmapsProbe.fragment.xml覆盖三根default/nondefault及LargeGroup合法真实继承、95ref全部非空、四种flags、Sound/Key/Duck多层列表。音频引用RadarOn/RadarOff来自SDK原MiscAudio.xml，复用Audio.xml reference，不新增未恢复AudioEvent资产；Duck引用本组base map，避免本地强引用循环。联合SDK仍须完成XSD/同输入原恢复/四产物/隔离重放，不把组件证明当SDK成功。

最终验收以实际sourceCommit/runId、validation.json、两模式退出码和SDK证据为准，不以本准备文档宣称成功。

