# GameMap尚未实现：完整依赖入口

2026-10-25。本轮只调查，**GameMap仍未恢复，不登记为已支持，不拿空根样本计完成**。MapMetaData、Terrain和GameScriptList各自完整组件另有源码及矩阵；不能代替这根。SDK未编译。

固定Module blob 2d2f471c8008b7c2e1f78e39752d0726da395cea、Plugin blob 00bb9e5df72d07a21204f85f9a9a2f1d9557f00b。原根Marshal/Endian在parts/part-0119.cs.txt原行59745，schema AssetTypeGameMap.xsd；Plugin完整Handler确实调用根Marshal，非no-op。建议下一整批以全部子结构一起实现，不按字段拆小运行。

根80字节：reserved0；Waypoint MapObject28列表@4/8；Structure/Unit/Prop MapObjectUSP56列表@12/16、20/24、28/32；Audio/Unknown MapObject28列表@36/40、44/48；Road4引用列表@52/56；EnvironmentData nullable12@60；WorldDict nullable56@64；PostEffect4引用列表@68/72；MapAnalysis nullable28@76。严格按原上述次序分配。schema中的MapMetaData pipelineOnly子节点原Marshal不读取，需要明确语义对照，不可拿已恢复MapMetaData组件强行写进这根。

MapObject28完整方法59570：id小写TypedAssetId@0、ThingTemplate强引用@4、MapObjectFlagType8 bitflags@8、Angle原SageReal float@12（不是AngleTag）、Position/VectorHack小写float3@16。USP56完整方法59646：Faction/Team/EventList三string和Health浮点先处理派生，再Marshal基类，必须按原方法核三string分配顺序/偏移。Dictionary56原方法59720：BoolProperty12、IntProperty12、RealProperty12、StringProperty16、UnicodeStringProperty16、AssetIdProperty12、AssetIdListProperty16七列表按固定顺序@0/8/16/24/32/40/48；UnicodeStringProperty原来也走StringPropertyPair，不能猜成UTF16。

后续精确方法（原全量索引method-index.txt可直接按类型定位）：

| 子结构 | Marshal原行 | 大小 |
| --- | ---: | ---: |
| StringPropertyPair | 59361（part0118） | 16 |
| MapAnalysisType | 59380（part0118） | 28 |
| RealPropertyPair | 59473（part0118） | 12 |
| BoolPropertyPair | 59493（part0118） | 12 |
| IntPropertyPair | 59507（part0119） | 12 |
| EnvironmentObject | 59527（part0119） | 12 |
| AssetIdListPropertyPair | 59537（part0119） | 16 |
| AssetIdPropertyPair | 59556（part0119） | 12 |
| MapObject | 59570（part0119） | 28 |
| MapObjectUSP | 59646（part0119） | 56 |
| Dictionary | 59720（part0119） | 56 |

RoadObject/PostEffectObject是4字节引用，部分Marshal内联在列表泛型 helper；需继续核精确字段/id键、引用类型、AssetIdList的空白分词和平台Endian。MapAnalysis包括5个signed计数+WaterAreaType枚举列表。必须保留完整uint32/float/bool/padding、Windows ANSI字符串/NUL、ALL/sign bitset、全部枚举/引用/各列表增长、三缓冲区/Od/O2/平台0/1/2/99、独立分配ABI与拒绝边界，不用抽样替代。此文档为调查交接，不是实现或验证证明。
