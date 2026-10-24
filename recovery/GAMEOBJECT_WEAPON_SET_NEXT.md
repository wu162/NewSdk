# 下一完整组：WeaponSetUpdate（仍明确拒绝）

TypeId1125589673，native40字节；四个列表@8/16/24/32，分别WeaponSlotHardpoint（stride28）、WeaponSlotTurret（124）、WeaponSlotHierarchicalTurret（128）、WeaponSlotContained（28）。完整原证据weapon-state-source的143方法包含本模块及三个反应开火/DamageState模块，后四个已验收见GAMEOBJECT_REACTIVE.md；不要误以为这意味着WeaponSetUpdate已支持。

## 已解决的排序未知项

weapon-slot-sort-source固定原blob6e40a9ed6c9f961d294d49768d911e6a6f354619，共23个精确方法（最终导出源码b2d59f1d1b250d42958be40f9e796fa5da85704a/run40070108593 success）；按manifest.signature定位，不沿用旧文件编号。包含EASTL quick_sort/impl、median、partition、bounded/simple insertion、完整heap fallback，以及SortedListHelper.Sort/Comparer、Tracker.ExchangePointers/AlterBookmark。方法body现按平衡大括号截断，避免下一方法前的MarshalAs属性污染SHA，适用于void及非void。

原Sort先建WeaponData指针排列，按Ordering signed整数比较；28为插入排序阈值，大于28时先introsort，再插入排序前28，最后simple insertion尾段。重复key次序必须保持原EASTL实际行为，不能换std::sort或stable_sort。

原版探针源码72b573545f0d8b88f6057d0e245bdb6a5d419f72/run40069703938 success。weapon-slot-sort-probe/permutations.tsv保留720组：四槽位、平台0/1/2/99、长度0/1/2/27/28/29/65/129/257、五种排序/重复模式，包含原索引排列、各三缓冲区长度与SHA256。原版断言每条72字节WeaponData的Template/Upgrade两个强引用及bool保持配对。原版TSV是定向语义调查，不是恢复组件或SDK验证。

已新增Native/WeaponSlotOrder.h/.cpp，返回原始记录的目标排列；本地-O0（ASan/UBSan，当前ptrace环境不支持LSan，关闭其泄漏检查）及-O2均匹配720原版排列，另1024有符号键性质检查。TestWeaponSlotOrder.ps1与weapon-slot-order.yml复用既有TSV验证Windows x86 /Od、/O2，不重复跑同一原版探针。helper验证结果另存weapon-slot-order/validation.json，必须核对实际sourceCommit/runId/终态；最终源码e5ba9e6858c0bbb20016ec65f8beeba72aafb5f2、run40070276350 success；两模式均720原版排列+1024有符号键检查，终态、validation.json和两模式日志已核对。首次两模式已通过但并发普通推送拒绝导致工作流失败，已补有限pull/rebase/push重试，以上为修复后的成功锚点。此helper尚未接入GameObject。

## 仍需实现的完整依赖

排序助手只给排列。原SortedListHelper.Sort使用Tracker.ExchangePointers后才交换72字节数据。ExchangePointers仅允许a/b处于当前栈顶block，按该block elementSize范围调整三组bookmarks；unsigned类型经AlterBookmark同时涉及源/目标，另两组要调整成员槽。不要仅memcpy记录或提前重排XML，否则引用记录/重定位顺序可能不同。下一步核对23方法中的AlterBookmark及Native/Relocation接口后完整恢复，补原/恢复三缓冲区差分，再接入WeaponSetUpdate。

Hardpoint：TypeId@0保持分配时0（Marshal不依据XML TypeId分派）；ID unsigned@4；InterleavedStyle enum@8；WeaponChoiceCriteria enum@12；Sorted Weapon list@16；AllowInterleavedFiring/AutoChooseWeapon bool@24/25。WeaponData stride72：Ordering enum@0，Template/Upgrade强引用@4/8，两段ObjectStatus@12/40，bool@68/69，末尾padding。不要把xas:isPolymorphic当作指针多态列表。

Turret在完整Hardpoint上inline TurretAIData@28（96字节）；HierarchicalTurret再ParentID@124；Contained增加父Token层但Marshal完整Hardpoint。Turret Marshal先TurretSettings，再Hardpoint；显式空与缺失TurretSettings不同。TurretAI所有20项包括普通float转速、Angle/Percent/Time、ControlledWeaponSlots6位、ObjectStatus、可空TurretAITargetChooser@88、bool@92–94。Chooser44字节，先自身TargetingCompareList强引用/三个bool，再完整BaseAITargetChooser36字节；base含时间、sympathy范围、4位DynamicTargetFlag、六bool以及ObjectVisionRange list@20。ObjectVisionRangeItem stride120，Range percent@4（不是@0）与inline ObjectFilter@8。全部枚举以PE enum-source.json为准，默认及offset按原方法逐一验证。

完整接入时必须覆盖四槽/全属性/全部依赖、每层Token/sentinel、缺失/显式空、强引用/各种掩码、排序重复key与阈值/交换、所有容器、拒绝/不变性，并统一取得完整GameObject /Od、/O2四平台三缓冲区与适用共享回归。保留本模块拒绝直到完整实现及验收；SDK原路由保留，正式集成门槛不降低。
