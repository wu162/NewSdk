# GameObject 战斗响应、掩体与友军通知

状态：本轮最终完整组件矩阵已验收通过；精确源码/运行/计数见文末。本页不代表完整GameObject、完整替代DLL或SDK集成完成。

原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`，52个精确原方法与完整ObjectFilter/状态验证/位集/引用helper见 `evidence/gameobject-modules/combat-response-source/manifest.json`。`GenerateCombatResponseFields.py` 核对指纹、字段顺序和Token再生；逻辑独立于 `GameObjectCombatResponseRuntime.h`。

| 完整类型 | TypeId | native | kind |
|---|---:|---:|---:|
| FindCoverBehavior | 3199719331 | 16 | 140 |
| NotifyWhenAttackingBehavior | 50199012 | 40 | 141 |
| HitReactionBehavior | 1506879888 | 48 | 142 |

按TypeId全头去重；三者Marshal直接ModuleData，Token父均Update，不能按Behavior名称改成Behavior父。

FindCover：AttributeModifierInCover强引用@8、CoverScanRange float@12，均无默认。

Notify：NotifyRadius float@8、AssistResponseModuleId弱ModuleData typed-ID@12无默认（lowercase后FastHash，无强引用登记）；SleepTime Time@16=0s；MaxFriendsToNotify signed@20=100；WeaponSlotID unsigned@24=1；WeaponSlotType单枚举@28=PRIMARY_WEAPON（完整既有6项表，未知5）；AcceptableFriends nullable ObjectFilter112@32；HasMobileFriends/IgnoreDisabledStatus bool@36/@37=false。先根属性，再过滤器分配；Filter Token mode7，Bridge走严格ReadDependencyFilter。

HitReaction：Light/Medium/HeavyDamageHitReactionLifeTimer为Time@8/@12/@16，均0s；对应Threshold是float@20/@24/@28，默认0，不能除100；RequiredDeathTypes为双word Death位集@32无默认（未知bit33、ALL两个word全置1）；ObjectStatusValidation为nullable56@40，ForbiddenStatus@0、RequiredStatus@28；RotateToSourceWhenDamaged/RotateToSourceIgnoreIdleCheck/GoIdleWhenDamaged为bool@44/@45/@46=false，padding@47保持零。状态验证Token为nullable嵌套表，默认缺省不分配，有节点再依原helper应用内容。

endian交换Time/float/弱ID/单枚举/位集全部word；递归交换存在的Filter和状态验证；强引用、bool、指针保持原模块合同，最终引用平台字节由TrackerFinalize负责。高kind的属性/递归节点判断必须提前返回，不能落入旧87+字段数组。

测试 `GameObjectCombatResponseCases.cs` 覆盖符号/数值边界、完整WeaponSlot/Death词表、弱ID与强引用区分、过滤器/验证缺省与显式空、拒绝及既有回归。最终仍须/Od、/O2、平台0/1/2/99、三缓冲区逐字节与独立布局成功。


## 本轮最终整批验收

有效验收提交`284599bfd958c0f62d40ce1b72d27dd70a568bcc`；GameObject run **40094269582 success**，同提交基础run **40094269592 success**。/Od、/O2每模式**381140 fixtures、1143420三缓冲区逐字节比较、13884拒绝、1060不变性**；平台0/1/2/99，两模式合计**2286840**次缓冲区比较。实际两模式日志、validation.json、sourceCommit/runId及运行终态均已逐项核对。

本轮实际29新，164→193/322；129种其他已知Marshal继续明确拒绝，58 Token-only未知Marshal回退保留。本轮SDK0次，原路由保留。
