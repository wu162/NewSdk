# 下一完整能力组：SpecialPower公共父与Radar子类（仍拒绝）

仅取证与只读PE准备，未实现、未计入本轮支持数量。固定原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`。78个精确原方法/wrapper/helper已提取并逐签名、含尾LF的body SHA256核对，见 `evidence/gameobject-modules/special-power-base-source/manifest.json`。不按文件编号、名称子串或native大小猜行为。

## 已按TypeId去重的下一组三种

| 完整类型 | TypeId | native |
|---|---:|---:|
| SpecialPowerModuleData | 3472757011 | 452 |
| RadarHackSpecialPowerModuleData | 3201673786 | 464 |
| RadarJamSpecialPowerModuleData | 3870511801 | 460 |

三ID已对当前全ModuleChoice头及本轮待验收类型去重，均仍拒绝。实际父是452字节，不是460；父Marshal约23.6KB，三个inline ObjectFilter112占336字节。两Radar子类复用整份父合同，不需要另加新枚举。不要把名称同尾的SpecialPowerUpdateModuleData、SpecialAbilityUpdateModuleData或OCLSpecialPowerModuleData当成这个父。

## 公共父的复杂度和关键闭包

父Marshal直接ModuleData，Token父Behavior；父有39个有效Token条目加终止项。完整根属性、默认、字段顺序、偏移、Token hash和endian均已在原方法取证，下一轮应一次恢复全部父能力，再恢复两简单子类，不按字段拆小批。

关键布局：强SpecialPowerTemplate@8、AntiFX@12、AttributeModifier@16、AttributeModifierRange float@20、AttributeModifierFX@24；RequirementsFilterMPSkirmish是StringList8@32（不是Filter节点）；InitiateFX/TriggerFX强引用@40/@44；SetModelCondition为单枚举@48，不能替换模型位集；SetModelConditionTime为Time@52；OnTriggerRechargeSpecialPower为弱SpecialPowerTemplate typed-ID@64；LeechRechargePercentPlayerTechTrigger/LeechRechargePercentFromSpecialPower实际为强PlayerTech/SpecialPower引用@76/@80，不能据Percent名称当float；DisabledTypesToIgnore为12位存储word@84；CanAffectObjectFilter是强ObjectFilterAsset引用@88，默认资源名GenericSpecialPowerObjectFilter，不能当inline filter；ObjectFilterDistType单枚举@92。

三个真正inline Filter依schema顺序为AttributeModifierAffects@96、RequirementsFilterMP@208、RequirementsFilterStrategic@320；缺省节点不Marshal，Token仍生成内联Filter表。存在时复用完整ObjectFilter，包括其IncludeThing/ExcludeThing动态列表，不能只恢复位集。

InitiateSound是nullable SoundOrEvaEvent8指针@432。存在时分配8字节；Sound属性再分配独立nullable强音效引用4字节@0，EvaEvent为inline强引用@4、默认空。Sound Token bookmark实际4字节值，Eva直接bookmark字段，父Token对存在的SoundOrEvaEvent生成完整两字段表；没有额外的声音枚举需求。原SoundOrEvaEvent没有独立Endian overload；父内联端序路径与强引用/指针合同必须逐项保留，不能凭结构尺寸推断交换所有word。bool集中于父尾部，padding不得改写。所有上述helper及原三表相关Marshal方法已包括在78方法闭包。

RadarHack新增unsigned NumFalseReturns@452默认0、float Radius@456默认0.0、Time HackDuration@460默认0s；RadarJam新增float JamRadius@452默认0.0、Time JamDuration@456默认0s。两者先处理自身字段再Marshal父，Token也保留独立SpecialPower父层；不得只实现本层标量而计为支持。

## 原PE枚举与最先验证项

只读 `InspectSpecialPowerEnums.py` 已在run **40093602291**（源码`781defb6ca9bbb0a7ff33afba01df79cd6ce5e70`）取得完整三表，输出 `SPECIAL_POWER_ENUM_SOURCE`；原证据是 `recovery/evidence/gameobject-root/special-power-enums.txt`，blob `da2bfef57693403072d80dd767a95516b7db00a5`。完整7/12/5名称和值已保存 `evidence/gameobject-modules/special-power-base-source/enum-source.json`，sourceBlob及原PE SHA256 `ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603` 已核对。该运行后续独立golden失败不影响已成功完成的只读原PE取证；不能将其当作SpecialPower实现/组件验收成功。三模块仍未实现且拒绝，78方法指纹不变。

| 原PE表 | entry count | 原未知回退 |
|---|---:|---|
| WeatherType | 7 | 单枚举1 |
| DisabledType | 12 | bit index0；ALL整word置FFFFFFFF，负词XOR |
| PartitionManagerDistTestType | 5 | 单枚举0 |

完整PE表补充：Weather NORMAL=0、CLOUDY=1、RAINY=2、CLOUDYRAINY=3、SUNNY=4、SNOWY=5、INVALID=6；Disabled DEFAULT=0、USER_PARALYZED=1、EMP=2、HELD=3、PARALYZED=4、UNMANNED=5、UNDERPOWERED=6、FREEFALL=7、TEMPORARILY_BUSY=8、SCRIPT_DISABLED=9、SCRIPT_UNDERPOWERED=10、INFILTRATED=11，故父默认HELD为mask8；Dist CENTER_2D=0、CIRCLE=1、CENTER_3D=2、SPHERE=3、EXTENTS_3D=4。名称和值只取自原PE，不能因相似名称重排。Disabled位集原ALL分支同样要按原helper核实token不前进边界，不能仅凭普通负词XOR推导ALL后的文本。

SetModelCondition继续复用完整457名称原表（含INVALID=-1，单枚举与456位集语义不同）。优先验证父默认、布尔尾部/零padding、3个inline Filter缺省/显式空及固定schema分配、StringList、强弱引用区别、两层nullable声音分配/Token、枚举完整词表及正负位词，再验证两个Radar子类与父字段组合。

GameObject是tokenized输出：原Plugin.cs blob `641761fc1420ce2ab6a9a75aa443dd35c970bda8` 的HandleType<GameObject> line716明确将ImportsData设为空。内部强引用仍按TrackerFinalize写入index+1，但不能把通用Tracker的Import offset/sentinel列表当作GameObject最终输出。保留实例、重定位、imports三缓冲区完整差分与独立Length0断言。

下一轮完整父+两子类统一/Od、/O2、平台0/1/2/99、三缓冲区逐字节、布局/拒绝/不变性，继续拆独立metadata/Runtime/输入校验块，不堆回GameObject.cpp。本轮不冒险启动此新父实现；无新支持或SDK声明。HordeDispatch等原Marshal/endian/Token别名不因恢复父而自动计数，必须按独立TypeId原分派与完整输入合同另验。


## 最终成功运行复核

三表WeatherType7/DisabledType12/PartitionManagerDistTestType5已在最终成功source`284599bfd958c0f62d40ce1b72d27dd70a568bcc` / GameObject40094269582中再次完整读取，原sourceBlob与PE SHA一致；完整表保存enum-source.json及root/special-power-enums.txt。SpecialPower452/RadarHack464/RadarJam460仍未实现、仍拒绝，不计193。下一轮复用此表，但生成器默认检查仍应从相同原PE逐表重取比较。
