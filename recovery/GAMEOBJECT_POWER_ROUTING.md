# 完整SpecialPowerUpdate与分派列表组（验证中）

2026-10-24，三种完整类型待Windows矩阵，不能提前计数；SDK未运行。

| 类型 | TypeId | native | kind |
|---|---:|---:|---:|
| SpecialPowerUpdateModuleData | 3115493811 | 24 | 188 |
| ActivateModuleSpecialPower | 1213608295 | 460 | 189 |
| SpecialPowerDispatchSpecialPowerModuleData | 3631725172 | 464 | 190 |

原Tokenizer固定blob6e40a9ed6c9f961d294d49768d911e6a6f354619，24个原方法与列表helper完整指纹保存`evidence/gameobject-modules/power-routing-source/manifest.json`，完整父闭包复用SpecialPower78方法。新TriggerType3由原PE取出：SELF_POSITION0/TARGET_POSITION1/TARGET_OBJECT2，未知0；只读取证run40099572191/source73f2d671506fb2cba7d5642bc0ef332debc9bfea，sourceBlob/PE SHA固定。生成器默认仍重读原PE逐表比较，导出模式仅本地检查。

SpecialPowerUpdate是独立Update24，不能误继承452父。3属性：强SpecialPowerTemplate@8、bool StartsPaused@20默认false、Disabled12@12默认HELD8；nullable SoundOrEvaEvent8@16和内层nullable强Sound4复用原完整合同，原Endian不交换声音结构/引用。Token5条加sentinel，父链Update→Contain→Behavior→Module。

ActivateModule继承完整452父。Modules是重复结构节点list8@452，元素ActivateModuleInfo16。每个元素先Deactivate bool@12默认false，再完整SpecialPowerModuleInfo12：ModuleId弱lowercase hash@0、TriggerAtTargetPosition bool@8默认true、TriggerType enum@4默认TARGET_POSITION1；9–11/13–15 padding零。端序交换每个ModuleId与TriggerType和list count，bool/padding不交换。Token仍保留ActivateModuleInfo→SpecialPowerModuleInfo独立父层与各sentinel。

SpecialPowerDispatch继承完整父；RequireAllPowersToFunction bool@460默认false，SpecialPower是重复结构节点list8@452，每项4字节强SpecialPowerTemplate属性。原Token列表每项额外分配一字段+sentinel表，并非直接bookmark值的普通强引用list；使用完整nested模式，保留name/type。端序只交换count，引用交Tracker。

三类型均严格拒绝未知属性/子节点、命名空间、结构文本、错误子节点形状、重复唯一声音。两派生类型先自身列表，再完整父的StringList/三个Filter/声音，XML节点反序输出须不变。独立golden覆盖完整native defaults/padding、结构list stride/count、弱ID/Trigger枚举/强ref index+1、每项Token父层和sentinel、Sound两层nullable；所有父字段/枚举、分派/Modules字段、0/1/2/27/28/29/65边界、原完整矩阵均保留。其他100已知Marshal仍拒绝，58 Token-only fallback不变。
