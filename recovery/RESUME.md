# 当前检查点（2026-10-24，未验收）

已完成SpecialPowerModuleData452与RadarHack464/RadarJam460整组源码、生成器及差分测试，正在启动完整Windows矩阵。此前193种已验收，本组3种暂不计数；SDK未运行、原路由保留。先核对当前Actions实际终态及sourceCommit，再看GAMEOBJECT_SPECIAL_POWER_BASE.md。原续接记录和全部验证证据保留如下。

# 编译器恢复：当前续接入口

更新：2026-10-24（北京时间）。wu162/NewSdk，最终源码分支`recovery/compiler-source`。先读AGENTS.md、EFFICIENCY.md、STAGE_DURATION.md和本文件。用户要求按实际约1小时推进、允许3子代理；本轮19:34开始，编码/修复/验收到20:36后整理交接，没有提前按子集完成结束。仓库通过GitHub插件访问，禁止强推；保留并发提交与Actions证据。

## 已验收193/322已知Marshal模块

本轮164→193，实际新增29种，完整分组如下（类型与TypeId/native/合同见各页和原manifest）：

| 完整组 | 新增 | 文档 |
|---|---:|---|
| 经济/电力 | 4 | GAMEOBJECT_ECONOMY_POWER.md |
| 围墙/桥梁/放置 | 3 | GAMEOBJECT_STRUCTURE_WALL.md |
| 战斗响应 | 3 | GAMEOBJECT_COMBAT_RESPONSE.md |
| EMP/压制 | 2 | GAMEOBJECT_STATUS_EFFECTS.md |
| 探测/隐身 | 2 | GAMEOBJECT_STEALTH_VISION.md |
| 泰矿资源 | 3 | GAMEOBJECT_TIBERIUM.md |
| 墙枢纽/围合 | 2 | GAMEOBJECT_WALL_ENCLOSE.md |
| 资源/盟友电力 | 2 | GAMEOBJECT_RESOURCE_ALLIES.md |
| 状态/事件 | 2 | GAMEOBJECT_STATE_EVENT.md |
| 模型状态监控 | 3 | GAMEOBJECT_MODEL_CONDITION.md |
| 移动/拖拽/清理 | 3 | GAMEOBJECT_MOTION_CLEANUP.md |

有效验收提交`284599bfd958c0f62d40ce1b72d27dd70a568bcc`；GameObject run **40094269582 success**，同提交基础run **40094269592 success**。/Od、/O2每模式**381140 fixtures、1143420三缓冲区逐字节比较、13884拒绝、1060不变性**；平台0/1/2/99，两模式合计**2286840**次缓冲区比较。实际两模式日志、validation.json、sourceCommit/runId及运行终态均已逐项核对。

129种其他已知Marshal仍明确拒绝；58 Token-only未知Marshal回退保留。完整GameObject、两份完整替代DLL、正式GameObject插件尚未完成。本轮SDK0次，原路由保留；组件验收不冒充完整DLL/SDK集成。

## 本轮执行与已纠正边界

3子代理分别负责证据、源码、差分测试，主代理按完整组集成/保存检查点。首批10新df3d0fcead1eb71c0d812faf74f4f7df751980b7 / GameObject40091269128、基础40091269124均success；第二批7新c32069c1ac0c7509b851c76d6f8c00bafebccdf1 / GameObject40091859893、基础40091859870均success。最终矩阵包含此前所有回归。

第三批6429b0cda58367ecd9bd114874950c0f24473bca/run40092685797因Enclose独立Imports断言失败；781defb6ca9bbb0a7ff33afba01df79cd6ce5e70/run40093602291因ALL -DUMP_ORE独立golden失败。两处原/恢复三缓冲区均未报差异，均依据原方法修正精确期待，没有删比较/拒绝或改恢复逻辑。失败记录保留，不能把旧失败source算成成功。

- 原Plugin.cs blob641761fc1420ce2ab6a9a75aa443dd35c970bda8，GameObject HandleType line716明确公开ImportsData=new byte[0]；不能把中间Tracker offset/sentinel表当公有返回。Enclose精确断言Imports长度0，FX24/String长度4/指针at+140/尾at+148/radius25均保留。
- MonitorConditionActionType唯一DUMP_ORE=0，普通负词为XOR；原ALL分支跳过token指针推进，后续词不再处理。ALL -DUMP_ORE和ALL -DUMP_ORE -DUMP_ORE都为FFFFFFFF，不能错误地对ALL后的负词执行XOR。原036 bodySHA56463e826838b3912fb33ff1ae1ea1dd11878a090edfcef4804f6cd039f1eaa6；详见MODEL_CONDITION.md。
- Invisibility两nullable NamedVoice是大小写敏感ExactStringHash，不能用小写HashAssetId或资源引用。EMP颜色是lowercase r/g/b三个float，Tiberium RadarColor是uppercase RGBA字节截断ARGB packed，两者不可混用。Model INVALID=-1忽略；ALL覆盖完整15word。
- Terrain UpgradeMustBePresent是真StringList，IncomeInterval是unsigned非Time；Energy复用完整Capture20。Grapple Bounce默认20f*0.01f为3E4CCCCC而非直接0.2f的3E4CCCCD；ObjectStatusValidation56→Filter112按schema分配。所有父Token、nullable/inline Filter、引用最终平台字节均保留。

## 源码组织与自动回归

主cpp保持拆分：当前68469字符/1018行。FieldsRuntime、FamiliesRuntime、DispatchRuntime、Support承载通用职责；本轮新增能力组运行代码分别放EconomyPower/StructureWall/CombatResponse/StatusEffects/StealthVision/Tiberium/WallEnclose/ResourceAllies/MotionCleanup的独立Runtime.h，原位置include、定义顺序/同一编译单元不变，机械展开字节等价已复核。StateEvent/ModelCondition纯字段组直接通用分派，无需不存在的Runtime文件。

11组共29新ID互不重复且均在原322集合；32个FieldSet count/边界、200根Token记录模式/stride已独立静态复核。高kind133–161在输入校验入口早返回，不能掉入旧宽索引。新增Filter Bridge入口均走原严格读取器。GameObject工作流增加tests/GameObject*.cs路径，保证新Cases单文件修正也自动触发完整矩阵，未改权限、自动证据步骤或关闭旧回归。

## 下一完整组：SpecialPower公共父与两Radar子类（仍拒绝）

先读`GAMEOBJECT_SPECIAL_POWER_BASE_NEXT.md`。SpecialPowerModuleData3472757011/native452、RadarHack3201673786/464、RadarJam3870511801/460，均按全TypeId元数据去重。固定原Tokenizer blob6e40a9ed6c9f961d294d49768d911e6a6f354619的78个精确原方法与必要helper闭包已提取/指纹核对，在special-power-base-source。本轮只取证，未实现/未计数。

WeatherType7、DisabledType12、PartitionManagerDistTestType5完整原PE表已取并在最终成功运行复读；enum-source.json保存所有名称/值、原PE/source指纹与原始/复核运行。无需再猜表或为相同证据单跑probe。仍须核父452（不是460）、3份inlineFilter112@96/@208/@320、StringList@32、nullable SoundOrEvaEvent8@432及内层ref4、严格schema分配顺序、父Behavior Token与两个子类完整继承。具体38属性/回退/原helper见NEXT，不能以本摘要代替原方法。

## 持续规则

约10–15分钟保存有意义检查点，明确未验证/验证中；按完整能力组连续实现并统一完整差分。保留/Od、/O2、四平台含99、三缓冲区逐字节、独立布局与拒绝边界，不降样本、不放宽断言。Actions通常至少约2分30秒，查实际终态、sourceCommit/runId再验收；相同源码/工具链证据不重复跑。

SDK按大里程碑集中运行；正式启用前XSD、同输入原/恢复、真实调用、四产物、适用无原DLL重放与完整mymod门槛不降低。历史STATUS按需读。本轮复用公共实现/原PE表、合并完整能力组验证，未做额度计量，无法量化节省。
