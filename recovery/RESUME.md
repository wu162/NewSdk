# 编译器恢复：当前续接入口

更新：2026-10-24（北京时间）。wu162/NewSdk，分支 recovery/compiler-source。先读AGENTS.md、EFFICIENCY.md、本文件；历史STATUS按需，不全量加载大型原反编译文件。

## 已验收136/322已知Marshal模块

本轮123→136，新增13种完整模块及全部必要依赖：

| 完整组 | 新增 | 文档 |
|---|---:|---|
| WeaponSetUpdate四槽/Sorted WeaponData/完整炮塔与目标选择器 | 1 | GAMEOBJECT_WEAPON_SET.md |
| AutoHealBehavior/RepairUpdate/InitiateRepairDie/HealCrateCollide | 4 | GAMEOBJECT_HEALING.md |
| CrateCollide/Money/Unit/Veterancy/Attribute/Random/Salvage/Spawn | 8 | GAMEOBJECT_CRATE.md |

最终有效组件源码 **1ef49469cd7a9f290b76952c4040cd2df0a2a147**：GameObject run **40072750732 success**，基础run **40072750706 success**。每/Od、/O2：**275032 fixtures、825096三缓冲区逐字节比较、11352拒绝、712不变性**，平台0/1/2/99；两模式合计**1650192**次缓冲区比较。validation.json、两模式实际日志、sourceCommit/runId及实际终态已核对。中间WeaponSet源码45625616880eb075147618813786d27794ee38c2/run40071300506、治疗/维修源码3b2cb3d2687d06b92417c3af56f6656b45e7cc63/run40072240199均success；最终矩阵包含全部。既有额外WeaponTemplate回归run40071300487及基础run40071300482/40072240244均success。

**186种已知Marshal仍明确拒绝**，包含下一组八种AI Update；58 Token-only保持未知Marshal回退。完整GameObject、完整两份替代DLL和正式GameObject插件未完成。SDK本轮**0次**，原路由保留；组件验收不冒充SDK/完整DLL验收。

关键纠正：原Tracker.ExchangePointers/AlterBookmark只按当前栈顶block的elementSize调整三组bookmark的offset，**不修改value**；既有Tracker实现已符合原方法，未据旧“源/目标”措辞改坏它。WeaponData排序保留XML引用生成次序，按原EASTL排列逐次先交换bookmark再交换72字节记录；不能提前排序XML。复用原720排列证据与已验收helper，不重跑相同排序探针。

治疗组首轮因旧拒绝数组末尾AutoHeal ID漏删而失败；已修正清单（不是放宽比较/改Marshal）并取得修复后完整矩阵成功。四类型/Crate/选择器的缺失与显式空、inline/nullable及引用分配顺序均保留，细节见三组文档。

## 下一完整组：AI Update八模块（未支持）

先读 **GAMEOBJECT_AI_UPDATE_NEXT.md**。原ai-update-source **96精确方法及PE枚举**已导出：源码**bae88f824dd018ba5a46ba390ab5f095cc046d07**，run**40072829359 success**。这是取证结果，未计支持，也未接入组件。

完整组AIUpdate76、Jet100、Horde76、SupplyTruck116、Transport76、Animal116、Wander144、DeployStyle92。Horde/Transport复用AIUpdate76 Marshal但有独立Token父层；ConstructionYardAIUpdate8此前已支持，不重复计数。主AIUpdate的nullable UnitAITargetChooserData指针**@68**、StandGround bool**@72**；UnitChooser完整40字节，先TargetingCompareList强引用@36（默认DefaultTargetingCompareList），再BaseChooser36。Base/VisionRange/Filter可在核对原方法指纹等价后复用WeaponSet完整依赖；不得把UnitChooser40误用为TurretChooser44。

按manifest.signature/bodySha定位，先查完整Marshal/Token/endian与未知helper，再连续实现整组和测试，统一受影响完整矩阵。尚未恢复类型继续拒绝。顺带在下一次相关代码批次修正GenerateReactiveFields.py旧阶段docstring“WeaponSet remains refused”（目前只是一条过时注释，运行状态以本文件/验证证据为准），避免仅为注释另触发Windows矩阵。

## 执行规则与用户提示

默认单代理，约45分钟有效实现；每约10–15分钟普通推送有意义检查点，未验证不标完成。整组实现/测试准备好后统一完整回归；不空等、重复调查或重复同输入验证凑时长。禁止强推，不新建无必要分支；并发更新先读新头重建树，保留Actions证据。

用户提示已在EFFICIENCY.md：命令行无法克隆，使用GitHub插件；Actions一般需2分30秒以上，以实际终态为准；直接写代码更快时省略无助正确性的步骤，严格守住正确性底线。SDK跨阶段攒够能力再集中验证；正式启用前XSD、同输入原/恢复、真实调用、四产物和适用无原DLL重放门槛不降低。完整mymod里程碑保留。

本轮复用已验证Tracker/资源/解析/位集/Upgrade/DieMux/Filter和原排序证据，按三个完整能力组推进；未反复clone、未整库下载、未重复SDK运行。没有账号额度计量，不能量化节省。
