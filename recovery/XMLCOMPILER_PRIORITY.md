# XmlCompiler 并行还原：最终交接

更新：2026-10-24（北京时间）。先读本文件，再读对应组文档；此入口描述本次独立 XmlCompiler 工作，不覆盖另一会话的 GameObject 续接。

## 分支与验收锚点

- 工作分支：`recovery/xmlcompiler-priority`；PR：https://github.com/wu162/NewSdk/pull/1 已于2026-10-24合并到 `recovery/compiler-source`，PR现为closed/merged。合并提交：`dddb9ac2804da1380a5b9b94a9035253c6e48f63`，保留两侧历史，无强推。
- 固定起点：`ba7a3d6845be5e124d08320cb2c56fda552d9fbe`。全部使用正常快进 ref 更新及保留父提交的合并，无强推。
- **实际已验收源码：`0e153b8ed50096157cc8a76d894858f946b63b43`。**
- **最终运行：`40083102908`，completed/success**，https://github.com/wu162/NewSdk/actions/runs/40083102908 。六组、联合链接及证据保存作业全部 success。
- 已逐项核对七份 validation.json 的 succeeded/sourceCommit/runId、两模式原始日志、平台 0/1/2/99 与真实正常退出。之后的交接文档更新不改变验收源码。
- 只创建过五个子代理；完成早的代理被复用。各工作分支历史已合入，FXParticle 使用复用代理的第六工作分支，不存在第六个代理。

## 十四个优先类型的实际支持面

| 类型 | 当前组件范围 | 验证组 |
| --- | --- | --- |
| AttributeModifier | 60字节根、Modifier列表、状态位集、Shader引用 | attributes |
| FXList | 5个根属性、15种已知Nugget、4组模型状态位集；两个ObjectFilter子节点仍拒绝 | effects |
| SpecialPowerTemplate | 38个根属性、过滤器、GameDependency、PlayerTech列表 | powers |
| ObjectCreationList | 完整6组typed nuggets及其向量、范围、引用和嵌套列表 | powers |
| LocomotorTemplate | 114个根属性、JetLocomotorData、BounceKickTerrainMap | locomotor |
| LogicCommandSet | Cmd引用列表及imports | attributes |
| FXParticleSystemTemplate | 根、关键帧、Wind、25种模块TypeId；部分Draw/Update/Event仍拒绝 | fxparticle |
| UpgradeTemplate | 140字节根、字符串/资源、GameDependency及ObjectFilter | attributes |
| AIPersonalityDefinition | 根、Side、18个列表、OpeningMove/Heuristic、Trouble、过滤器 | ai |
| AIStrategicStateDefinition | 根、Tactic、TeamTemplate、CreateUnits及继承heuristics | ai |
| AIMicroManagerData | 根、TargetBias、SpecialPowerDelay、RetreatBaseCriteria、IgnoreTargets | ai |
| AIBudgetStateDefinition | Budget/AccountShare及继承heuristics | ai |
| AIStateDefinition | 29种已知heuristics、加权组合及嵌套过滤器 | ai |
| AITargetingHeuristic | 根、BaseSelection、原EASTL距离排序（含signed zero） | ai |

所有成功输出保留 InstanceData、RelocationData、ImportsData。XmlCompiler 使用绝对偏移零的单 Tracker，不添加 Tokenizer 过程表。恢复代码不转发原 XmlCompiler；原 DLL 仅供测试 oracle 使用。

## 完整矩阵结果

各行均为 /Od、/O2 两模式及四个平台合计。数组比较包含相应组的直接入口一致性对照，不应全部误称为原 DLL 对照次数。

| 组 | Fixtures | 数组逐字节比较 | 显式拒绝 |
| --- | ---: | ---: | ---: |
| attributes | 9432 | 28296 | 232 |
| effects | 6712 | 40272 | 152 |
| powers | 34592 | 103776 | 1040 |
| locomotor | 33016 | 99048 | 1976 |
| ai | 15712 | 94248 | 328 |
| fxparticle | 13976 | 41928 | 168 |
| 合计（六组） | 113440 | 407568 | 3896 |

此外联合 `Recovered.XmlCompilerPriority.dll` 在两模式完成14类型×4平台的112个默认根fixtures、336次数组对照，两个进程退出码均为0。该smoke仅验证组间可链接和路由，不代替各组完整矩阵。

结果位置为 `recovery/evidence/xml-<group>/validation.json` 及 Od/O2 oracle/build 日志；联合结果在 `xml-priority-link`。两份联合DLL保存在最终运行的 `recovered-xmlcompiler-priority-x86` artifact，未覆盖SDK原DLL。联合validation记录DLL指纹、全部源文件指纹及 originalDllUnchanged=true。

## 明确仍未完成的部分

1. **FXList**：SourceObjectFilter、SecondaryObjectFilter子节点及未知TypeId的原版base fallback仍显式拒绝。15个已知具体分支和根/模型位集能力已验收，不应称为完整FXList。
2. **FXParticleSystemTemplate**：非空Event及八个已知模块仍拒绝：FXParticleEventBase、FXParticleUpdateDefault、FXParticleUpdateRenderObject、FXParticleDrawGpu、FXParticleDrawLightning、FXParticleDrawRenderObject、FXParticleEventCollision、FXParticleEventLife。25个已支持模块的完整字段已验收。
3. 全部组件仍要求BAB预处理后的适用引用及数字TypeId；不在本次组件内实现XSD、继承、资源解析。未知XML/TypeId等明确拒绝边界见各组文档。AI还拒绝超过96层和实际解析成非有限值的排序距离。
4. 原编译器实际的Personality子节点名为 **CountAsStructureForBuildPurposesFilter**；截断名不带Filter被显式拒绝。该拼写来自原静态字节，不按截断MSVC符号/XSD猜测。
5. **尚未做真实SDK/MOD集成验收，也未正式替换两个原DLL或插件路由。** 此分支是已验收的优先类型组件源码，不是整个301类型XmlCompiler的完整替代DLL。

## 合并边界与后续入口

最终源树审计：基线 `recovery/src` 的52个既有源码文件SHA全部未变，新增28个Xml源码文件；无Draft文件。GameObject及Joypad类型未在本次还原，SDK原DLL保持原指纹。公共运行时、已有GameObject源码/测试/工作流不由本次修改。继续使用原主会话的GameObject交接；本PR已通过普通merge保留两边提交，后续继续禁止强推。

公共桥接通常为 `Recovered::<Type>Compiler::{CompileXml,ProcessInstance}`；六个AI根统一使用 `Recovered::XmlaiCompiler`。构建入口：
`TestXmlattributes.ps1`、`TestXmleffects.ps1`、`TestXmlpowers.ps1`、`TestXmllocomotor.ps1`、`TestXmlai.ps1`、`TestXmlfxparticle.ps1`，及 `TestXmlPriorityLink.ps1`。Windows x86 VS2022 + CLR4 legacy activation；统一工作流已包含集成分支和目标恢复分支。

下一步优先补齐上述FXList两类过滤器、未知base回退，以及FXParticle剩余模块/非空Event；再按仓库既有SDK降频规则集中做真实集成，不把当前组件成功当作SDK成功。

## 关键取证与修复

原Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`；原Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`；原DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。ExportXmlCompilerSource.py验证原blob、逐字节拼回239个小块，输出2449个实际方法体索引、PE枚举、原始静态字段及每个优先根的完整literal。证据不代表新增支持。

本轮最终修复包含：按真实TypeId区分前缀导航；Percentage及5Hz Velocity缩放；ParticleSystem InitialDelay缺省CONSTANT/-1/-1与部分覆盖；ALL特殊位集期望；CLI case临时对象作用域及managed String类型。不可变AI布局表采用原生InitOnce的进程生命周期所有权，避免CLR退出阶段注册std::map析构回调；未通过忽略退出码或强制退出掩盖异常。

早期失败运行留存，不能当成功。最终只引用上列0e153b8/run40083102908。源码/证据复用避免重复下载大型反编译文件；没有可量化的账号额度数据，不宣称节省比例。
