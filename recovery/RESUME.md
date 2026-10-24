# 本轮30分钟检查点（2026-10-24）

六种完整类型代码/测试已提交，Windows完整矩阵验证中，验收计数仍225/322。首批WeaponFire/DelayedWeaponFire/Beam/ObjectStatus见204765b，run40105748576；随后完整OCL500/Movement488包含28原方法闭包、PE枚举检查、列表/nullable/父层/拒绝/不变性测试。以后先查最新源码完整run终态及不可变证据，不能把待验收6种计数。全部旧回归和/Od、/O2、平台0/1/2/99保留，SDK未运行、原路由保留。本轮用户指定仅30分钟，不修改长期1小时规则。

# 编译器恢复：当前续接入口

更新：2026-10-24，北京时间。仓库wu162/NewSdk，源码分支recovery/compiler-source。先读AGENTS.md、EFFICIENCY.md、STAGE_DURATION.md。约1小时有效阶段、每10–15分钟保存有意义检查点；GitHub插件访问，禁止强推；保留并发提交与Actions证据。完整组实现后统一完整矩阵，SDK按大里程碑。

## 已验收225/322已知Marshal模块

本轮193→225，完整新增32种：SpecialPower452/RadarHack/RadarJam 3种；SpecialPower完整派生23种；SpecialPowerUpdate/ActivateModule/SpecialPowerDispatch 3种；SpecialAbility240/Capture/OCLMonitor 3种。类型/TypeId/native/完整合同分别见GAMEOBJECT_SPECIAL_POWER_BASE.md、GAMEOBJECT_SPECIAL_POWER_DERIVED.md、GAMEOBJECT_POWER_ROUTING.md、GAMEOBJECT_SPECIAL_ABILITY.md。

最终共同验收源码 **b6a1516c1b3dc73a069bf229534dacbb02eef8f3**，GameObject run **40101248219 success**。/Od、/O2各 **566456 fixtures / 1699368三缓冲区逐字节比较 / 17812拒绝 / 1580不变性**；平台0/1/2/99，两模式合计3398736缓冲区比较。两模式实际日志、原PE/生成器检查、validation.json identity与工作流归档终态均核对。Native/Bridge/测试/生成器与a125源码相同，基础run40100525692 success。

不可变完整证据在`evidence/gameobject-runs/40101248219-1`；`evidence/gameobject-root`最终快照指向相同Git树。新的发布脚本只写run-attempt唯一目录，根快照由阶段收尾同步；**以后先核对RESUME的精确run/sourceCommit与不可变证据，不能盲信根快照已是最新运行**。

97种其他已知Marshal仍拒绝，58 Token-only未知Marshal回退保留。本轮SDK0次，原路由不变。完整GameObject、正式GameObject插件与两份完整替代DLL尚未完成；组件通过不冒充DLL/SDK集成。

## 关键边界与故障记录

SpecialPower父452，三inline Filter112@96/208/320、StringList8@32、nullable Sound8@432内层nullable强Sound4；原Endian不交换声音/强引用。11种仅继承父的类型各保留独立SpecialPower Token层。RadarSpy/Freeze TargetFilter是StringList，PlayerUpgrade/Grant的Upgrade为强引用文本子节点list。87c026/run40096821430失败因生成器误把Token槽ptr+8当native偏移；已优先native指针、父层0并静态检查全部字段偏移序列，60c7cf/run40099429813 success，失败证据保留。

PowerUpdate只有24并继承Update，不能当452父。ActivateModule每项16，先Deactivate再ModuleInfo12，弱ModuleId、TriggerType单枚举与两个bool；Dispatch每项4强引用，但每项另有一字段+sentinel子表。此组a0d1f09/run40100044990及基础40100044953 success，219→222。

SpecialAbility240不是上述两父的子类：45完整根属性、两个inline223状态、两String8及最后nullable Anim12。Options31未知bit5，Conditions3未知bit0，Disabled12未知bit0；普通负词XOR、ALL整word覆盖并忽略后续；ActiveModelCondition单枚举未知233。保留schema分配顺序和零padding。

a125/run40100525642两模式组件步骤均通过，但旧共享证据文件rebase冲突导致终态failure。现PublishGameObjectEvidence.ps1使用新的detached发布工作树、run-attempt不可变目录、最多3次普通pull/rebase/push；不强推、不改测试checkout输出。最终40101248219差分/实际发布/产物均success。本地还模拟两发布者竞争，原输出和两份档案都保留。工作流超时10→15分钟适配完整矩阵，未减少平台/样本/断言，新增生成器单文件变化也自动回归。

## 下一完整组：SpecialAbility武器/光束/状态（仍拒绝）

先读`GAMEOBJECT_ABILITY_WEAPON_NEXT.md`。WeaponFire507880339/native272、DelayedWeaponFire915350037/276、Beam2695497697/272、ObjectStatus127626925/296，均仍拒绝、未计数。固定原Tokenizer blob6e40a9ed6c9f961d294d49768d911e6a6f354619的32完整方法及直接装饰helper闭包已提取/指纹逐项核对，见ability-weapon-next-source/manifest.json和ExportAbilityWeaponMethods.py --check。父240已验收；Delayed必须额外继承完整WeaponFire272。WeaponSlotType6、ModelCondition457、ObjectStatus223、Anim12均可复用原PE验证合同；不要据字段名猜Filter/位集。

只读取证73f2d671506fb2cba7d5642bc0ef332debc9bfea/run40099572191 success，power-continuation-enums.json保存TriggerType3、ConditionsType3、SpecialAbilityUpdateOptionsType31、SPMovementType3、OCLCreateLocType15；原PE SHA256 ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603。生成器默认从原PE重取比较，无需重复probe。完整OCLSpecialPower/Movement可作为随后能力组。

原分派全TypeId清单source-inventory.json定位精确方法。大原源码用fetch_blob取得并局部提取；fetch_file对>1MB文件行范围可能返回空，不能当方法不存在。后续保留/Od、/O2、四平台含99、三缓冲区逐字节、完整旧回归和独立布局/拒绝/不变性。GameObject公开ImportsData为空，不能把Tracker中间引用表当公开Imports。正式启用前SDK同输入/XSD/真实调用/四产物/适用无原DLL重放与完整mymod门槛不降低。

本轮复用完整父/helper/PE表、合并完整能力组验收；除定位真实差异及修复证据发布外未人为重复同源码矩阵。未取得账号额度计量，不能量化节省。
