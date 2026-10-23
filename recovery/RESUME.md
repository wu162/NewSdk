# 本轮进行中：四种声音／播报选择器

已实现四种选择器及完整嵌套列表、可空优先级和 Token/Endian；新增差分与独立布局测试。当前为**未验证检查点**，等待本提交触发的完整 Windows 矩阵；不得据此宣布新增四类型已完成。原方法经 source blob 指纹及 SHA256 核对，分段证据在 sound-selector-methods/。SDK 未运行，原路由保留。下方此前成功锚点仍只适用于旧源码。

# 编译器恢复：当前续接入口

更新：2026-10-23。仓库wu162/NewSdk，分支recovery/compiler-source。先读AGENTS.md、EFFICIENCY.md及本文件。不要全量加载大型反编译或source-inventory。

## 本轮结果与当前验证

本轮13:48:08Z开始有效工作，按约45分钟推进；完成剩余七种Body（至此九种XSD全部）、六种ClientUpdate（XSD全部）、两个完整声音播放模块。公共31种加新增15种，共**46/322已知Marshal类型**；其余**276个明确拒绝**，58个Token-only仍走原未知Marshal路径。支持六容器不代表所有模块已恢复。根范围仍88属性、38类根子节点；未完成整个GameObject、两份完整替代DLL或正式GameObject插件。

最后运行源码83a7e5261da45fdf1d35d14728db42b76c040747；GameObject run39969902743、基础run39969902873（均success，已核对远端validation.json及日志）。该修订补齐负数MaxUpdateRangeCap原异常类型BinaryAssetBuilder.BinaryAssetBuilderException、XmlFormattingError及消息，并直接与原版对照。此前修订92846ab构建失败（缺少异常命名空间），不能列为通过；已修复。

完整播放组此前源码85a1e180c6bdb506733ec0711e9ad7c8512095c4/run39968193677、基础39968193825均success。每/Od、/O2模式91,528 fixtures、274,584三缓冲区比较、8,444拒绝、272不变性比较；两模式合计549,168次缓冲区比较，平台0/1/2/99。最终修订已通过相同完整矩阵，作为本轮最终验证锚点。

本轮SDK运行0次，原路由保留。组件artifact为recovered-gameobject-component-x86，非正式SDK插件。Weapon/Armor未变，先前验收不冒充本轮SDK。

## 持续规则

- 目标完整恢复XmlCompiler/Tokenizer的C#和C++/CLI；两份完整替代DLL尚未完成。
- 仓库操作持续授权，只禁止任何强推；保留并发代码及Actions证据提交，普通快进更新现有分支。本轮未新建分支。
- 约45分钟有效实现阶段；先完成一组后保存并推进下一完整组。每约10–15分钟检查未保存成果；检查点明确未验证/验证中/已验证。不能空等或重复分析凑时长。
- 尽快完成整组，关键公共契约集中调查、连续实现、统一完整矩阵；避免每个字段/同源码重复触发Windows。正确性优先，不降低/Od、/O2、四平台含99、三缓冲区逐字节比较、独立布局/Token/sentinel及拒绝测试。
- 默认单代理；原DLL证据须核对实际方法，不能以旧总结或nativeSize代替语义。工具结果先筛选，大型源码按方法展开。
- 真实SDK可跨多阶段累计；普通组件组不触发SDK。路由/输入链/正式启用前仍须适用SDK验收，不能用组件成功冒充集成成功。可在临时副本精简无关MOD输入，原/恢复版输入相同，保留必要依赖及完整XSD。详见EFFICIENCY.md。
- Weapon `[sdk-probe]` / `[sdk-plugin]`、Armor `[sdk-armor]`或手动批次入口控制真实SDK。没有SDK标记的组件批次不频繁跑SDK。

## 已完成组与关键契约

- Body：GAMEOBJECT_BODY.md。独立组source d0b96f2a57aeb7dac4dc2d58683b6ed8a93d8c8f/run39965730762 success。Active76、Shield108；其余五种派生76，Token继承不同。默认百分比literal为66.666666/33.333333，DodgePercent不缩放。ShieldArmor可空引用先于Active字符串/列表分配。
- ClientUpdate：GAMEOBJECT_CLIENT_UPDATES.md。source76e1e34c12b4728ab194acffbde1a26bef52febd/run39966783977 success。25属性及LargeGroupAudio完整Key列表；Key区分大小写，UnitWeight ushort；LaserLifetime普通float。
- 两个声音播放模块：GAMEOBJECT_SOUND_PLAYBACK.md。AnimationSound含可空非负float、可空ModelCondition两mask、Sound资源、动画小写typed-ID及Frame有符号整数列表；AudioLoop含可空Sound和两inline60字节ModelCondition。原PositiveNonZeroReal实际上接受0。
- 以前完成的AI、空ReplaceModule/InheritableModule、31个公共8字节模块、所有根属性和此前根子节点继续在矩阵内。共享scalar/resource/relocation/bitset/ObjectFilter契约保留。

实现src/Native/GameObject.{h,cpp}、GameObjectFields.h、src/Bridge/GameObjectCompiler.cpp；测试GameObject{Oracle,Cases,KindOf}.cs及TestGameObject.ps1。GenerateGameObjectFields.py调用ExtractGameObjectModuleEvidence.py验证原blob/字段/Token图。evidence/gameobject-root/validation.json保存准确sourceCommit/runId/modes/platforms；最终请先检查该文件。

## 下一完整组：四种声音/播报选择器

先读GAMEOBJECT_SOUND_SELECTORS.md。本轮仅调查并保存scripts/ExtractSoundSelectorEvidence.py及evidence/gameobject-modules/sound-selector-source-plan.json；生成与--check通过，**没有启用运行支持，也没有原DLL差分**。四种：RandomSoundSelector、ModelConditionSoundSelector、UpgradeSoundSelector、ModelConditionEvaEventSelector。调查覆盖12个嵌套契约、8个独立列表stride。

整组复用完整根Audio数组契约，新增nullable int、可空嵌套结构及Override列表Token；字段名、枚举fallback、typed-ID与StringHash差异、原分配次序必须逐方法核对。部分Marshal折叠/内联在父方法或列表helper里，不能因缺少独立重载省略。统一完整Windows矩阵后保存，继续下一完整组；按EFFICIENCY累计SDK，禁止降正确性换速度。

## 未变化的Weapon/Armor已集成状态

Weapon：91根属性、35/35显式Nugget、默认TypeId路径及正式插件已完成。源码22c12f53c2f3c886cdb63d212ab10bc57cf715e0；run39896743313 success。每模式66,152 fixtures、793,824缓冲区比较、1,002拒绝，另有ObjectStatus/ModelCondition直接比较；38合法mymod武器、四份完整产物一致、实际调用及无原DLL重放通过。正式插件Recovered.Weapon.Plugin/Recovered.Weapon.dll，默认检入SDK路由仍原版。见WEAPON_TEMPLATE.md、WEAPON_NUGGETS.md、WEAPON_INPUT_PIPELINE.md。

Armor正式插件/真实SDK四产物/实际调用/无原DLL重放此前通过；最近mymod输入后的回归run39896743435 success。本轮未修改Weapon/Armor源码、SDK输入或路由，未重复对应已完成SDK批次。

