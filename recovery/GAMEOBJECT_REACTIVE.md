# DamageState / 反应开火完整组（组件完整矩阵通过）

已实现DamageStateList（4161248261/16）、FireWeaponWhenDamagedBehavior（144160085/80）、FireWeaponCollide（2924124080/80）、FireWeaponWhenDeadBehavior（2429465756/144），完整矩阵已通过，119→123/322；199种已知Marshal仍明确拒绝。源码3968ee7499a924ef068f8ef0132d62927c2bd168，GameObject run40069379207 success，基础run40069379228 success。每/Od、/O2：220032 fixtures、660096三缓冲区比较、10820拒绝、460不变性，平台0/1/2/99；validation.json与两模式原始日志、实际终态已核对。固定原blob 6e40a9ed6c9f961d294d49768d911e6a6f354619；weapon-state-source导出143方法：63c451f78ef02913e2abe9f454c6dfcecffbabd7/run40068448236 success。

DamageState Data为96字节list元素，percent@0/4、完整ObjectStatus28字节@8、完整ModelCondition60字节@36。WhenDamaged继承完整Upgrade（36字节），DamageTypes普通unsigned解析@36，DamageAmount普通float@40，八个强引用@44–72，InitiallyActive bool@76。

Collide Flags五位（NONE是bit0，不是0），未知bit2；WhereToFire默认TARGET_OBJECT=0，未知2；两个inline ObjectStatus@16/44和FireOnce bool@76。

WhenDead保留完整Upgrade父层与依赖；Time@36、DeathWeapon强引用@40、完整KindOf36字节@44、WeaponOffset可空Vector3@80、inline DieMux44字节@84、WeaponFireProbability list@128、五bool@136–140及填充。缺失DieMuxData保持全0，显式空才套用DieMux默认。Probability stride12：DeathType完整8字节，ChancePercentage是unsigned（不除100）。分配按Vector→DieMux依赖→Probability→Upgrade依赖，Token父层在先。

GenerateReactiveFields.py正式检查全部143方法原blob/body SHA、PE枚举、原始属性fallback、Token/父链/列表stride/sentinel，复用已验收Upgrade/DieMux/向量/位集语义。ReactiveFamily覆盖全部字段边界、所有容器、完整列表与模块0/1/2/27/28/29/65、独立size/default/布局/Token/掩码/分配、拒绝与顺序不变性。SDK未运行，原路由保留。

## WeaponSetUpdate不在本组验收

同一源码调查覆盖WeaponSetUpdate完整四种槽位、TurretAI/目标选择器；仍明确拒绝，不能只支持空槽。发现SortedListHelper.Sort会按Ordering比较72字节WeaponData并调用Tracker.ExchangePointers同步指针，再交换结构体；不能以提前排序XML或仅memcpy替代。详见GAMEOBJECT_WEAPON_SET_NEXT.md。weapon-slot-sort-source已扩展到23个排序/ExchangePointers/AlterBookmark方法；原版720组排序与引用配对探针已成功。C++排序排列助手已还原，未接入WeaponSetUpdate；完整Tracker交换和四槽位/炮塔接入仍待恢复。

首轮空Flags独立断言误将无token当未知token，原/恢复字节一致；已按原Tokenizer空串行为修正为0，未知词仍独立断言bit2。最终完整矩阵锚点如上，不以失败运行充数。
