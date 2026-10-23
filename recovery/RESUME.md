# 编译器恢复：当前续接入口

更新：2026-10-23。仓库`wu162/NewSdk`，分支`recovery/compiler-source`。先读AGENTS.md、EFFICIENCY.md和本文件；历史STATUS.md不是当前计划，不全量加载大型反编译或source-inventory。

## 当前实现检查点（2026-10-23，ClientUpdate验证中）

本轮13:48:08Z开始有效工作，按约45分钟推进。Body完整组已通过：source d0b96f2a57aeb7dac4dc2d58683b6ed8a93d8c8f/run39965730762 success；每模式81,864 fixtures、245,592三缓冲区比较、8,344拒绝、260不变性比较，四平台含99。

继续完成ClientUpdate六种XSD类型全部字段和LargeGroupAudio Key列表（区分大小写FastHash）、ushort UnitWeight；扩展原Token树和字段白名单，其他278个已知类型继续拒绝。**ClientUpdate待本提交完整矩阵，尚不声明通过。** 原有Body/AI/公共模块回归全部保留。SDK未运行、保持原路由。下方锚点仍是前序历史，最终验收后重写本入口。

## 持续规则

- 目标完整恢复XmlCompiler/Tokenizer的C#和C++/CLI；两份完整替代DLL尚未完成。
- 仓库操作持续授权，只禁止任何强推；保留并发代码及Actions证据提交，普通快进更新现有分支。本轮未新建分支。
- 约45分钟有效实现阶段；先完成一组后保存并推进下一完整组。每约10–15分钟检查未保存成果；检查点明确未验证/验证中/已验证。不能空等或重复分析凑时长。
- 尽快完成整组，关键公共契约集中调查、连续实现、统一完整矩阵；避免每个字段/同源码重复触发Windows。正确性优先，不降低/Od、/O2、四平台含99、三缓冲区逐字节比较、独立布局/Token/sentinel及拒绝测试。
- 默认单代理；原DLL证据须核对实际方法，不能以旧总结或nativeSize代替语义。工具结果先筛选，大型源码按方法展开。
- 真实SDK可跨多阶段累计；普通组件组不触发SDK。路由/输入链/正式启用前仍须适用SDK验收，不能用组件成功冒充集成成功。可在临时副本精简无关MOD输入，原/恢复版输入相同，保留必要依赖及完整XSD。详见EFFICIENCY.md。
- Weapon `[sdk-probe]` / `[sdk-plugin]`、Armor `[sdk-armor]`或手动批次入口控制真实SDK。没有SDK标记的组件批次不频繁跑SDK。

## 当前GameObject：88根属性、38类根子节点（模块容器有限范围）

本轮连续完成三个完整能力组：

1. **SkirmishAIInformation完整依赖组**：88字节根、172字节AIContextualPower列表、两组20字节AIBaseSelectionCriteria、28字节SpecialPowerWeapon、两处ObjectFilter；64字段及所有枚举/位集、可空值/引用、列表、Token/endian。
2. **ReplaceModule/InheritableModule完整XSD空结构**：各分配1字节，Tracker按4字节对齐；root@496/@500；原Token不生成子表/目标bookmark，offset仍0。未知内容拒绝。
3. **多态模块公共能力组**：Draws、Behaviors、AI、Body、ClientUpdates、ClientBehaviors六容器；原Marshal全部31个8字节公共字段类型及未知TypeId默认路径。**其余291个已知Marshal类型仍显式拒绝，不是全部模块已恢复。**

关键边界：DamageType的UNDEFINED=14、unknown fallback=38；可空bool为1字节分配，其余AI可空值/引用对象4字节；ALL保留word padding。模块id为小写typed-ID哈希；按数值TypeId分派，不从元素名猜测。未知模块分配8字节、输出TypeId=0、Token仅12字节单记录包装，无公共子表和包装sentinel。31个具体类型的Token继承树按原方法提取，不按推测继承生成。

原Marshal/endian各322个显式模块分支，Token380个；58个Token-only不能自行加到Marshal。已保存完整源码清单、两处endian父类别名和31个公共类型Token树。

此前29类子节点及全部根属性保持覆盖。StringList仅接受XSD合法文本（原结果为空列表），非空Item拒绝。非多态无命名空间TypeId注解允许；外来命名空间、未知字段/结构、重复单例和结构文本拒绝。共享relocation/scalar/resource/bitset/ObjectFilter实现未改。

**仍不是完整GameObject编译器；没有正式GameObject插件。本轮真实SDK运行0次，保持原路由。**

## 最终精确验证锚点

源码：`afac552209d4880e883f2d37189c896a1823e4a6`。
GameObject：https://github.com/wu162/NewSdk/actions/runs/39961644190 ，success。
基础回归：https://github.com/wu162/NewSdk/actions/runs/39961644145 ，success，同源码。

每种/Od、/O2：**69,744 fixtures、209,232三缓冲区比较、8,264拒绝检查、204独立不变性比较**；两模式合计**418,464次缓冲区比较**，Win32/Xbox360/PS3/99。终态、sourceCommit/runId、两模式摘要和platforms已核对`evidence/gameobject-root/validation.json`。组件artifact为`recovered-gameobject-component-x86`，保留原日志及Actions证据提交。

前序独立组历史：AI source986bb898dacfbed2d6f92074f3e49f9aef421719/run39959718372；空Module标记source1d07ec2aee4ec6aa0193f8866fbd4ed76e7497d0/run39960173975。最终矩阵包含全部以上范围，无需对未变源码重复运行。

源码：`src/Native/GameObject.{h,cpp}`、`GameObjectFields.h`、`src/Bridge/GameObjectCompiler.cpp`。生成器`GenerateGameObjectFields.py`调用`ExtractGameObjectModuleEvidence.py`，检查原blob和源派生元数据一致性。测试仍在GameObject{Oracle,Cases,KindOf}.cs及TestGameObject.ps1。

类型细节：GAMEOBJECT.md、GAMEOBJECT_AI.md、GAMEOBJECT_MODULES.md。`evidence/gameobject-modules/source-inventory.json`约500KB，仅按TypeId/类型筛选读取。

## 下一完整能力组：Body的其余七种类型

先读**GAMEOBJECT_BODY.md**。一次完成ActiveBody、ShieldBody、ImmortalBody、HighlanderBody、ManagerBody、ManagedActiveBody、ManagedImmortalBody全部字段/继承Token及DamageRangeModifier依赖；InactiveBody/PropBody已支持。该文档是本轮源码调查，**不是这七类已实现或原DLL差分结果**。

重点：DodgePercent不乘0.01，另外两个MaxHealth百分比及DamageRangeModifier的Min/Max会乘；ShieldArmor可空引用先分配，之后才调用ActiveBody分配GrabObject/列表。五个76字节派生类Token链不同。当前模块分配固定8字节，必须扩展size/字段/桥接白名单，不能仅增加TypeId。保持其他未恢复类型显式拒绝。

整组实现和测试准备后统一完整矩阵；GameObject能力累计后再集中准备正式插件、合法mymod样例及四产物/实际三缓冲区/无原DLL重放，不因普通字段逐个跑SDK。

## 未变化的Weapon/Armor已集成状态

Weapon：91根属性、35/35显式Nugget、默认TypeId路径及正式插件已完成。源码22c12f53c2f3c886cdb63d212ab10bc57cf715e0；run39896743313 success。每模式66,152 fixtures、793,824缓冲区比较、1,002拒绝，另有ObjectStatus/ModelCondition直接比较；38合法mymod武器、四份完整产物一致、实际调用及无原DLL重放通过。正式插件Recovered.Weapon.Plugin/Recovered.Weapon.dll，默认检入SDK路由仍原版。见WEAPON_TEMPLATE.md、WEAPON_NUGGETS.md、WEAPON_INPUT_PIPELINE.md。

Armor正式插件/真实SDK四产物/实际调用/无原DLL重放此前通过；最近mymod输入后的回归run39896743435 success。本轮未修改Weapon/Armor源码、SDK输入或路由，未重复对应已完成SDK批次。

本轮按完整AI→空Module标记→31类型公共模块三组推进，公共组一次完整差分通过；首轮AI仅独立断言混淆UNDEFINED与unknown，修正后完整验收。阶段及时保存，未逐字段重复全矩阵。没有账号额度计量，不能量化额度节省。
