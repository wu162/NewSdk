# DamageState / 反应开火完整组（实现检查点，待完整矩阵）

已实现DamageStateList（4161248261/16）、FireWeaponWhenDamagedBehavior（144160085/80）、FireWeaponCollide（2924124080/80）、FireWeaponWhenDeadBehavior（2429465756/144），等待完整矩阵，不计入当前已验收119。固定原blob 6e40a9ed6c9f961d294d49768d911e6a6f354619；weapon-state-source导出143方法：63c451f78ef02913e2abe9f454c6dfcecffbabd7/run40068448236 success。

DamageState Data为96字节list元素，percent@0/4、完整ObjectStatus28字节@8、完整ModelCondition60字节@36。WhenDamaged继承完整Upgrade（36字节），DamageTypes普通unsigned解析@36，DamageAmount普通float@40，八个强引用@44–72，InitiallyActive bool@76。

Collide Flags五位（NONE是bit0，不是0），未知bit2；WhereToFire默认TARGET_OBJECT=0，未知2；两个inline ObjectStatus@16/44和FireOnce bool@76。

WhenDead保留完整Upgrade父层与依赖；Time@36、DeathWeapon强引用@40、完整KindOf36字节@44、WeaponOffset可空Vector3@80、inline DieMux44字节@84、WeaponFireProbability list@128、五bool@136–140及填充。缺失DieMuxData保持全0，显式空才套用DieMux默认。Probability stride12：DeathType完整8字节，ChancePercentage是unsigned（不除100）。分配按Vector→DieMux依赖→Probability→Upgrade依赖，Token父层在先。

GenerateReactiveFields.py正式检查全部143方法原blob/body SHA、PE枚举、原始属性fallback、Token/父链/列表stride/sentinel，复用已验收Upgrade/DieMux/向量/位集语义。ReactiveFamily覆盖全部字段边界、所有容器、完整列表与模块0/1/2/27/28/29/65、独立size/default/布局/Token/掩码/分配、拒绝与顺序不变性。SDK未运行，原路由保留。

## WeaponSetUpdate不在本组验收

同一源码调查覆盖WeaponSetUpdate完整四种槽位、TurretAI/目标选择器；仍明确拒绝，不能只支持空槽。发现SortedListHelper.Sort会按Ordering比较72字节WeaponData并调用Tracker.ExchangePointers同步指针，再交换结构体；不能以提前排序XML或仅memcpy替代。独立weapon-slot-sort-source已导出Sort与Comparer（9734f903715abe9192a7dda3bf8448088e33cc2b/run40068810956 success）；继续必须跟进eastl quick_sort与Tracker.ExchangePointers（含重复Ordering、强引用记录与重定位），再实现整个WeaponSetUpdate依赖组。
