# MapMetaData完整XmlCompiler组件

2026-10-25，mapmetadata组单根，不生成/解析自定义MapCompiler的地图文件。SDK未接入、未执行；最终差分以本轮汇总与evidence/xml-mapmetadata/validation.json sourceCommit/runId为准。

固定Module blob 2d2f471c8008b7c2e1f78e39752d0726da395cea、Plugin blob 00bb9e5df72d07a21204f85f9a9a2f1d9557f00b、原DLL SHA256 d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6。根完整Handler在Plugin内联读取MapMetaData列表；parts/part-0087.cs.txt43601完整MetaDataObject、43670 Endian和43568 StartPositionObject；part-0037.cs.txt18556 Vector3/VectorHack小写x/y/z；part-0142.cs.txt71024 Metadata60列表；StartPosition20列表与Endian在part0141。原StructLayout12/60/20，schema AssetTypeMapMetaData.xsd与Includes/MetaDataCommon.xsd。TypeId1603703110，TypeHash1209903334。

根12 reserved@0、count/pointer@4/8，元素也叫MapMetaData。每MetaDataObject60：id弱TypedAssetId@0；Description string@4/8；DisplayName string@12/16；NumPlayers signed@20；CRC unsigned@24；FileName string@28/32；Width/Height/BorderSize signed@36/40/44；StartPosition count/pointer@48/52；IsMultiplayer/IsOfficial bool@56/57，padding58/59零。StartPosition20：Name string@0/4、Position float x/y/z内联@8/12/16。全部无根字段默认，字符串缺失无分配，present-empty分配NUL4，按原Windows ANSI/C-string；id小写hash。先分配Metadata整列表，再按项Description/DisplayName/FileName及StartPosition列表/Name的原顺序分配。数字word大端，bool不swap，pointer由Tracker处理，无imports。

TestXmlmapmetadata.ps1完整/Od、/O2 × 平台0/1/2/99，每次原Initialize目标平台，恢复ProcessInstance/CompileXml各三数组比较。全部字段字符串/weak ID/Unicode/NUL/4097、signed/CRC/float/bool完整矩阵；Metadata60与StartPosition20列表0..1024增长、两项嵌套分配、8种string存在组合。独立ABI核12/60/20、全部offset/数值bits、bool/padding、string长度/指针、嵌套分配及重定位顺序、imports空。未知字段/节点/文本、foreign根/qualified属性、重复Position、大小写坐标、空声明/handle不匹配显式拒绝。没有半实现GameMap或GameScriptList。
