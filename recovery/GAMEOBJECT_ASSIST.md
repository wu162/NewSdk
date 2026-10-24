# 辅助放置与协同响应完整能力组

2026-10-24：完整实现并通过完整差分；随本轮136→155/322已验收。

| 类型 | TypeId | native |
|---|---:|---:|
| AssistedPlacementInfoBehavior | 2328709842 | 36 |
| AssistedPlacementBehavior | 2920212896 | 20 |
| AttackAssistResponseBehavior | 3962605070 | 44 |
| SpecialPowerAssistResponseBehavior | 3358372044 | 40 |

原方法位于evidence/gameobject-modules/assist-source；固定原Tokenizer blob6e40a9ed6c9f961d294d49768d911e6a6f354619。共63个精确方法；包含显式补充的HintObjectMapping endian（062.txt），不能遗漏原列表helper通过typed局部变量直接调用的该方法。GenerateAssistFields.py逐项核对bodySHA，Windows核对原blob、全部PE枚举。AssistanceHintType/WeaponSlotType取证来自a936ac2b575259c27e58c4fb2ff08597eb0116bf / run40078557180 / job120827084299日志ASSIST_ENUM_SOURCE；该失败运行只提供取证，不能冒充组件验收，最终检查仍直接与原PE对照。

Info36：AllowShrouded bool@32默认true；HintObject为20字节记录列表@8，PlacementRestriction8为nullable指针@16，SuggestedOffset为inline Vector3@20。分配顺序固定为HintObject列表及逐项依赖、PlacementRestriction及其Filter、最后inline Vector3，与XML子节点顺序无关。

HintObjectMapping20：PlayerTemplate为nullable typed-ID指针@0，显式空仍分配4字节、哈希值0；HintDisplayObject为inline weak typed-ID@4，HintOpacity普通float@8默认1.0，ObjectFilter为nullable112指针@12，HintUsesShaders bool@16默认false。新增ModuleValue::NullableAssetId仅用于该字段，分配4字节并用既有typed-ID哈希；Token书签指向被指向的4字节，endian交换值，不交换地址。PlacementRestriction8含FilterRadius float@0和nullable RestrictedObjectFilter指针@4。

AssistedPlacement20：PlacementHint为4位AssistanceHintType@8，默认NONE设置bit0；未知词使用bit3（ALLOW_SHROUDED），ALL保持全部32位。SnappingDistance float@12，RequirementObjectFilter为nullable指针@16。AttackAssist44：ObjectStatus为inline223位掩码@8，WeaponSlotID uint@36，WeaponSlotType单枚举@40未知回退5。SpecialPowerAssist40：SpecialPowerTemplate强引用@8，inline223位ObjectStatus@12。

GameObjectAssistCases.cs覆盖四型全部字段、六容器、Token/sentinel、完整HintObject列表容量边界、nullable typed-ID的缺失/空/大小写/ANSI/资源外观字符串、弱typed-ID与强引用区别、完整Restriction/Vector3/Filter、所有枚举与status词、位集未知词回退、分配顺序及三缓冲区不变性、命名空间/结构文本/未知节点属性/重复单节点拒绝。与经济组修正及全部原GameObject回归统一运行/Od、/O2、平台0/1/2/99和三缓冲区逐字节比较。

最终有效源码16a37d05276242c29eddaee4726c6ed92603dac0：GameObject run40078985685 success、基础run40078985621 success。每/Od、/O2均318592 fixtures、955776三缓冲区比较、12160拒绝、844不变性，平台0/1/2/99；两模式合计1911552次缓冲区逐字节比较。validation.json及两模式实际日志/sourceCommit/runId和终态已核对。SDK0次、原路由保留，组件成功不等于完整DLL或正式SDK插件验收。
