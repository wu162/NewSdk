# GameObject 多态模块：公共能力组

本文件保留公共能力组的历史记录；最新已验收总数、拒绝范围及验证锚点以RESUME.md为准。历史扩展记录：46/322个已知Marshal模块已恢复；当时276个仍拒绝。Body九种XSD、ClientUpdate六种XSD和两个声音播放模块均完整支持，见GAMEOBJECT_BODY.md、GAMEOBJECT_CLIENT_UPDATES.md、GAMEOBJECT_SOUND_PLAYBACK.md。下述31公共类型结果是此前历史锚点。

状态：**完整公共能力组已实现并通过原DLL差分。** 源码`afac552209d4880e883f2d37189c896a1823e4a6`；run https://github.com/wu162/NewSdk/actions/runs/39961644190 success。/Od、/O2每模式69,744 fixtures、209,232三缓冲区比较、8,264拒绝、204不变性比较，四平台含99。基础回归run39961644145 success。SDK未运行，原路由保留。

## 已实现范围

六个容器Draws、Behaviors、AI、Body、ClientUpdates、ClientBehaviors；完整恢复原Marshal的全部31个8字节公共字段类型和未知TypeId默认分支。本轮再完成15种类型；当前其余276个已知Marshal类型显式拒绝。支持容器并不代表支持其中全部模块。

模块XML名称从AssetTypeGameObject.xsd的六个容器提取。原Marshal按数值TypeId分派，不从元素名猜测；无TypeId与未知TypeId走默认路径。测试包含同一合法元素名注入不同TypeId的直接组件探针；这些输入不冒充BAB已注入正确TypeId的SDK集成。只接受无命名空间id/TypeId；未知字段、外来命名空间、嵌套结构与结构文本继续拒绝。

## 原始来源与关键契约

源Tokenizer -Module-.cs blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`。提取器`scripts/ExtractGameObjectModuleEvidence.py`核对该指纹，逐项保留原方法签名和行号，生成`evidence/gameobject-modules/source-inventory.json`。该文件是源码清单，不是运行成功证据。

| 原方法 | 行号 | 契约 |
|---|---:|---|
| ModuleData公共Marshal | 14411 | TypeId@0由外层写入；id@4为typed-ID |
| ModuleData id哈希 | 67672 | ASCII/原ANSI编码的小写化FastHash，复用HashAssetId |
| Marshal多态分派 | 47214 | 322个显式TypeId；缺失按0；strtoul语义 |
| endian多态分派 | 48200 | 同322个ID |
| Token多态分派 | 95684 | 380个显式ID，含58个仅Token分支 |
| 未知Marshal | 62881 | Push8字节；只写id；输出TypeId清零 |
| ModuleData公共Token | 78028 | TypeId/id两个bookmark和sentinel |
| 根模块容器包装 | 66079–66153 | 四种可空8字节列表头；AI列表内联@472；Body单对象 |
| Draw列表Marshal示例 | 75056 | 4字节指针数组，然后按XML顺序分配各模块 |
| Draw列表Token示例 | 104852 | count记录及item记录，每item再进入多态包装 |

- 原Marshal/endian共322个ID，Token共380个ID。**不能把58个仅Token分支自动加进Marshal。** 输入它们的TypeId时原Marshal仍输出TypeId=0，Token因此只生成单记录包装，不会访问对应具体Token方法。
- 两个endian分支直接复用父类型方法：HordeDispatchSpecialPower→SpecialPower；LeechTargetingAttachUpdate→AttachUpdate。源清单分别保留名字；不能以名称不同误报缺失或自行新增字段。
- 31个公共类型的Token不是同一张表。提取器逐方法递归记录继承表、name/type hash、成员偏移及sentinel；实现按这些树生成Token，包含原UpdateModuleData→ContainModuleData的实际Token委托。不要用推测的C++继承替换原调用。
- 未知模块分配8字节，输出TypeId=0；Token仅12字节包装记录(name=2,type=0,offset=0)，**没有公共ModuleData子表，也没有包装sentinel**。
- 结构根分配顺序：SkirmishAIInformation→Draws→Behaviors→AI→Body→ClientUpdates→ClientBehaviors→Geometry。四个可空列表的“缺失”和“存在但空”不同；AI始终有inline计数Token。Body原只处理恰好一个子元素；组件允许空Body但拒绝多个，未宣称原版忽略多个的非XSD输入为支持表面。
- 所有公共模块数据为TypeId/id两word。endian交换数据两word和列表count；指针由Tracker最终重定位。共享relocation/scalar/resource/bitset实现未改。

## 全部31个公共类型

| 类型 | TypeId | 原Marshal包装行 |
|---|---:|---:|
| ConstructionYardAIUpdateModuleData | 12431267 | 61033 |
| DamageModuleData | 39586775 | 60646 |
| TrackIncomingWeaponFireBehaviorModuleData | 367637472 | 61684 |
| DrawModuleData | 402003752 | 61419 |
| ObjectDefectionHelperModuleData | 413409423 | 62460 |
| ProductionUpdateInfoModuleData | 456357763 | 62161 |
| ContainModuleData | 687874333 | 60298 |
| InactiveBodyModuleData | 690179694 | 61872 |
| DamagerModuleData | 712593807 | 61294 |
| ObjectHelperModuleData | 701158544 | 62640 |
| RadarBehaviorModuleData | 777285518 | 61778 |
| CreateModuleData | 816016269 | 60680 |
| SubStructureBehaviorModuleData | 857632184 | 62110 |
| ParabolicCurveUpdateModuleData | 1193738269 | 62353 |
| ClientUpdateModuleData | 1286434524 | 61159 |
| StoreTemplateIDBehaviorModuleData | 1354235932 | 60133 |
| ShieldBodyUpdateModuleData | 1681533635 | 61518 |
| EnergyDisablingBehaviorModuleData | 1683093719 | 61637 |
| SubStructureStorageBehaviorModuleData | 1689321498 | 60982 |
| CollideModuleData | 1894105347 | 60856 |
| ClientBehaviorModuleData | 2072794994 | 62701 |
| ObjectSMCHelperModuleData | 2138239554 | 60875 |
| SupplyCenterCreateModuleData | 2541127534 | 62503 |
| FiringTrackerModuleData | 2552695760 | 60179 |
| BehaviorModuleData | 2769121946 | 62601 |
| BeamTargetModuleData | 3375962491 | 62318 |
| DestroyModuleData | 3415251966 | 60834 |
| AttributeModifierPoolUpdateModuleData | 3861049429 | 62188 |
| UpdateModuleData | 4043318533 | 60287 |
| BodyModuleData | 4103182530 | 60341 |
| PropBodyModuleData | 4287119435 | 60845 |

## 验证范围

在原完整GameObject矩阵上增加六容器×31类型×id边界、全部58个Token-only回退输入、缺失/零/未知/unsigned溢出TypeId；其余291个已知类型逐项拒绝。列表0/1/2/27/28/29/65项覆盖原排序阈值；混合AI/Geometry/空Module标记和XML顺序不变性。独立检查默认8字节模块、id大小写归一、12字节包装、具体TypeId及非零具体Token绑定；保留/Od、/O2、0/1/2/99和三缓冲区逐字节比较。

## 下一完整能力组

优先完成Body XSD的其余七种类型（ActiveBody、ShieldBody、ImmortalBody、HighlanderBody、ManagerBody、ManagedActiveBody、ManagedImmortalBody）；InactiveBody和PropBody已经包含在公共组。共享ActiveBody字段和相关Enum/列表/资源一次调查、一次实现、一次完整矩阵。仍须读取实际Marshal/endian/Token和未知helper，不能把nativeSize或类型名称当作字段语义。

Body下一组调查见GAMEOBJECT_BODY.md；不是已恢复声明。

