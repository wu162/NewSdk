# WeaponSetUpdate 完整组件恢复

状态：完整四槽及必要依赖已验收，含后补组合场景。最终源码1ef49469cd7a9f290b76952c4040cd2df0a2a147 / GameObject run40072750732 success；每/Od、/O2：275032 fixtures、825096三缓冲区逐字节比较、11352拒绝、712不变性，四平台0/1/2/99。validation.json、两模式实际日志和终态已核对，SDK未运行，原路由保留。首轮WeaponSet源码45625616880eb075147618813786d27794ee38c2/run40071300506 success保留为历史锚点。

TypeId1125589673，native40；四列表@8/16/24/32，Hardpoint28、Turret124、Hierarchical128、Contained28。WeaponData72，Ordering@0 signed比较，Template/Upgrade强引用@4/8，ObjectStatus@12/40各28字节，bool@68/69，padding@70/71；排序保持字段/引用配对。Hardpoint TypeId@0保持分配时0，ID unsigned@4，bool@24/25；不按子XML TypeId分派，不把列表视为指针多态。

Turret先inline TurretSettings@28，再完整Hardpoint；Hierarchical先ParentID@124再Turret；Contained复用完整Hardpoint Marshal但多一层Token父表。TurretSettings缺失时inline96字节全0，显式空时应用默认，不能合并。Turret所有20项含普通float转速/idle scan角、Angle/Percent/Time、ControlledWeaponSlots6位、ObjectStatus、nullable Chooser@88、bool@92–94。Chooser44先自身强引用/三bool，再Base36。Base含Time、sympathy范围、4位DynamicTargetFlag、六bool、ObjectVisionRange列表@20；item120，Range percent@4，inline ObjectFilter@8。Filter缺失与显式空保持原版区别。

原证据weapon-state-source固定blob6e40a9ed6c9f961d294d49768d911e6a6f354619。GenerateWeaponSetFields.py核对67个相关精确方法的签名/bodySha，CI另核对原blob及PE枚举。生成字段表、全部Token父层/nullable/list/sentinel。没有手猜Token hash或使用xas:isPolymorphic替代真实布局。

原EASTL排序排列复用WeaponSlotOrder：28阈值、introsort/heap fallback、first28插入排序与尾部simple insertion，重复key次序不得换为std::sort/stable_sort。原720排列取证run40069703938及helper/Od/O2 run40070276350保留；未重复相同探针。运行时SortWeaponData保持原XML引用生成次序，建立排列和逆映射；每次调用Tracker.ExchangePointers后交换72字节记录。原Tracker.ExchangePointers/AlterBookmark（weapon-slot-sort-source000/001）只移动offset，三组bookmark的value不变；既有实现逐条核对已正确，不为旧交接歧义改坏它。

GameObjectWeaponSetCases.cs覆盖六容器、四槽、全属性、枚举与fallback/大小写、完整ObjectStatus、布尔/数值/引用边界，所有nullable/inline空与缺失，列表0/1/2/27/28/29/65/129/257、五模式排序、重复key、引用/bool/掩码配对、独立尺寸/偏移/默认/sentinel、拒绝及XML不变性。后补组合覆盖TurretSettings/Chooser/VisionRange/Filter与乱序WeaponData同时出现的分配顺序与强引用配对。完整矩阵的成功以RESUME.md及validation.json最新sourceCommit/runId为准。

本模块只是组件支持，完整GameObject插件、完整DLL和真实SDK集成未宣称完成。
