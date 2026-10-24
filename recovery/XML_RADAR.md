# Radar完整组件组

2026-10-25，组`radar`包含RadarTerrainImage与UIRadarObjectsLayer。最终Windows差分状态以本轮汇总及`evidence/xml-radar/validation.json` sourceCommit/runId为准；SDK未接入、未执行。

固定原Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`、原DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。

原证据：`evidence/xml-source/parts/part-0100.cs.txt` 原行50389 RadarTerrainImage Marshal及Endian（延续part0101）；`part-0038.cs.txt`原行19010 ICoord2D；`part-0099.cs.txt` 原行49759 SpecialIcon、49823 UIRadarObjectsLayer及完整Endian；`part-0152.cs.txt`原行76002 SpecialIcon列表124字节步长、76136 MapBorder enum（未知fallback2）；`enums/MapBorder.json`精确9项。原StructLayout root40、root24、icon124。原Plugin非Win32全部大端，精确TypeId/TypeHash见catalog/XmlradarTypes.json。schema AssetTypeRadarTerrainImage.xsd与UI/Radar/ObjectsLayer.xsd。

RadarTerrainImage40：reserved@0、MapBorder@4默认Orange0（Invalid=-1，未知Blue2）、Texture强引用@8、ImageSize ICoord2D@12、ImagePosition@20、TextureSize@28（均x/y signed）、Rotated单字节bool@36默认0，37..39 padding0。坐标缺失保持0，present空也默认0；全部结构内联，无子块。

UIRadarObjectsLayer24：reserved@0、MinUnitDotSize unsigned@4默认3、MinStructureDotSize@8默认5、NeutralColor packedARGB@12，缺失0、present-empty alpha255；SpecialIcon count/pointer@16/20。每个icon124：Image强引用@0、ImageOffset float x/y@4/8、ObjectFilter112内联@12。缺少filter整个inline0，present-empty写RuleNONE3且不分配filter自身；status/thing列表复用完整已验证ObjectFilter。列表一次分配全部124字节项，再按项顺序分配各filter子块。filter适配器剥除TypeId元数据，attrs与Thing文本按原C-string截断，共享helper未改。所有数字大端校正，reference/pointer交给Tracker。

`TestXmlradar.ps1` /Od与/O2 × 平台0/1/2/99；每原ProcessInstance前显式Initialize平台，恢复ProcessInstance与CompileXml各三数组完整对照。覆盖MapBorder全部9值及大小写/未知/NUL、三坐标全部字段、两unsigned、bool、两强引用、float2、packedARGB全通道、filter全部KindOf/ObjectStatus词汇及sign/ALL/NUL/Unicode、0..1024 icon/Thing增长与多filter分配。独立ABI核40/24/124、默认值、byte bool/padding、坐标偏移、packedARGB、inlinefilter和28/28/4/4子块、重定位与导入owner排序。未知字段/节点、重复singleton、非法文本、foreign根/qualified属性、空声明与handle不匹配均显式拒绝。未恢复整套BAB预处理、Tokenizer或SDK正式路由。
