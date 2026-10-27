# GameObject 墙枢纽与围合

状态：本轮最终完整组件矩阵已验收通过；精确源码/运行/计数见文末。本页不代表完整GameObject、完整替代DLL或SDK集成完成。

原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；39个精确原方法、wrapper、弱ID/列表/String/ObjectFilter闭包与body指纹见 `evidence/gameobject-modules/wall-enclose-source/manifest.json`。`GenerateWallEncloseFields.py` 再生metadata；逻辑独立于 `GameObjectWallEncloseRuntime.h`，无需新PE表。

| 完整类型 | TypeId | native | kind |
|---|---:|---:|---:|
| WallHubBehavior | 4093160954 | 56 | 150 |
| EncloseBehavior | 2864072215 | 140 | 151 |

按TypeId全元数据去重。两者Marshal直接ModuleData，Token父均Update，不按Behavior名字推断。

WallHub：HubCapTemplateId/DefaultSegmentTemplateId/CliffCapTemplateId/ShoreCapTemplateId/BorderCapTemplateId/ElevatedSegmentTemplateId为弱GameObject typed-ID@8/@12/@16/@20/@24/@28，无默认；BuilderRadius/MaxBuildoutDistance float@32/@36=0；Options unsigned@40=0，原方法是纯整数而非位集/枚举；StaggeredBuildFactor signed@44=0；SegmentPattern弱GameObject ID列表头@48。根属性后一次分配4*count列表，逐项lowercase后hash；无强引用登记。Token列表mode3/stride4，endian交换六弱ID、普通字段、每列表ID及count，不交换指针。

Enclose：EnclosureHitFX强引用@8；ScalableDrawModuleId弱ModuleData typed-ID@12；ScaledBoneName String8@16，均无默认；EnclosureRadius float@24=300；Filter为inline ObjectFilter112@28。字符串缺省不分配，显式空分配NUL；Filter缺省不Marshal，有节点才应用默认，Token无论有无Filter都生成完整inline表(mode8)。原schema先字符串等属性，再Filter；endian交换弱ID/String length/radius和完整Filter，强引用最终平台字节由TrackerFinalize处理。

实际Bridge kind151 Filter走ReadDependencyFilter；高kind属性/NodeKind/NodeAttribute均提前返回，避免进入旧字段数组。WallHub只接收SegmentPattern文本项，Enclose只接收Filter及其原合法内容；重复/未知/外来namespace仍拒绝。

测试 `GameObjectWallEncloseCases.cs` 验证原字段全部能力、Options整数、signed/unsigned/弱强ID区分、列表/字符串边界、inline Filter空与缺省及拒绝，保留完整旧回归。最终须/Od、/O2、平台0/1/2/99、三缓冲区逐字节差分、独立布局及不变性，组件成功不代表SDK或完整GameObject完成。


## 本轮最终整批验收

有效验收提交`284599bfd958c0f62d40ce1b72d27dd70a568bcc`；GameObject run **40094269582 success**，同提交基础run **40094269592 success**。/Od、/O2每模式**381140 fixtures、1143420三缓冲区逐字节比较、13884拒绝、1060不变性**；平台0/1/2/99，两模式合计**2286840**次缓冲区比较。实际两模式日志、validation.json、sourceCommit/runId及运行终态均已逐项核对。

本轮实际29新，164→193/322；129种其他已知Marshal继续明确拒绝，58 Token-only未知Marshal回退保留。本轮SDK0次，原路由保留。
