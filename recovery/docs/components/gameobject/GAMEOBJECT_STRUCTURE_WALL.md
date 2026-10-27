# GameObject 围墙、桥梁与结构放置

状态：本轮最终完整组件矩阵已验收通过；精确源码/运行/计数见文末。本页不代表完整GameObject、完整替代DLL或SDK集成完成。

原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；60个原Marshal/Token/endian/wrapper及必要Shape/ObjectFilter/Vector/helper方法和指纹见 `evidence/gameobject-modules/structure-wall-source/manifest.json`。`GenerateStructureWallFields.py` 对照原方法及既有完整几何枚举证据再生metadata；Runtime独立为 `GameObjectStructureWallRuntime.h`。

| 完整类型 | TypeId | native | kind | Token父 |
|---|---:|---:|---:|---|
| WallBuilderBehavior | 2509263754 | 216 | 137 | Behavior |
| BridgeBuilder | 2734068631 | 52 | 138 | Update |
| StructurePlacementBehavior | 3922835435 | 56 | 139 | Behavior |

三者已按TypeId全表去重；Marshal均直接ModuleData。

Wall：MaxNumSegments signed@8=5；SecondaryModelConditions为60字节位集@12=WALL_SEGMENT（bit422）；SecondaryObjectStatus为28字节位集@72=WALL_SEGMENT（bit203，不能误用KindOf索引）；ConnectingObjectFilter为inline112@100；StartInSecondaryModel bool@212=false。Filter缺省不Marshal但Token始终有完整内联表(mode8)，有节点才应用其默认值。Bridge将kind137 Filter交给严格ReadDependencyFilter，不放宽过滤器属性/子节点。

Bridge按schema次序处理：EndCap强引用@8；EndCap2 nullable引用@12；EndCapLen float@16；CenterPiece强引用@20；CenterPieceLen float@24；CenterPieceB nullable引用@28；CenterPieceBLen nullable float@32（允许负值）；Width float@36；GateHouse强引用@40；GateHouse2 nullable引用@44；DamageNeighborSegementsAtHealthPercent Percent@48默认66.66666（保留原拼写Segements）。其余无默认；存在的nullable字段各分配4字节，包括显式空。四个nullable Token都bookmark实际值(mode2)，引用字节由TrackerFinalize处理，nullable float值须endian交换。

Placement仅有RestrictedAreaShape子节点，内联Shape48@8。缺省保持Shape零；有节点才依次写Type@0=SPHERE、ContactPointGeneration@4=NONE、Name String8@8无默认、Active/UsedForHealthBox bool@44/@45=true、Major/MinorRadius float@16/@20=1.0、Other String8@24无默认、Height float@32=1.0、FrontAngle Angle@36无默认，最后nullable Offset Vector3@40。Shape采用完整GeometryType与ContactPointGeneration表，未知分别1/3；不是MotionShape。缺省与存在时都生成Shape Token表；Offset原Token仅bookmark指向值(mode2)，不构造坐标子表。

endian按原方法交换位集全部存储word、普通字段/字符串length、存在的nullable float与Shape Offset；递归处理Filter，bool/指针不交换。测试 `GameObjectStructureWallCases.cs` 保留旧矩阵并验证缺省/显式空、全部字段、单枚举/位集、schema顺序、内联表和拒绝边界。最终须完整/Od、/O2、四平台、三缓冲区差分及独立布局。


## 本轮最终整批验收

有效验收提交`284599bfd958c0f62d40ce1b72d27dd70a568bcc`；GameObject run **40094269582 success**，同提交基础run **40094269592 success**。/Od、/O2每模式**381140 fixtures、1143420三缓冲区逐字节比较、13884拒绝、1060不变性**；平台0/1/2/99，两模式合计**2286840**次缓冲区比较。实际两模式日志、validation.json、sourceCommit/runId及运行终态均已逐项核对。

本轮实际29新，164→193/322；129种其他已知Marshal继续明确拒绝，58 Token-only未知Marshal回退保留。本轮SDK0次，原路由保留。
