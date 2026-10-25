# Terrain资产/Atlas运行时完整Xml组件

2026-10-25，terrain组两根TerrainAssetCollection12、TerrainTextureAtlasRuntime28；只恢复原XmlCompiler注册入口，不生成自定义TextureCompiler的贴图Atlas。后者非全局schema资产。SDK未接入、未执行；最终差分以本轮汇总/evidence/xml-terrain/validation.json sourceCommit/runId为准。

固定原Module blob 2d2f471c8008b7c2e1f78e39752d0726da395cea、Plugin blob 00bb9e5df72d07a21204f85f9a9a2f1d9557f00b、原DLL SHA256 d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6。原两个Handler均在Plugin内联Marshal列表。parts/part-0049.cs.txt原行24731完整TerrainAsset36；part-0082.cs.txt41371完整Tile12及41454 Atlas Endian；part-0125.cs.txt TerrainClass enum11项/fallback3；原StructLayout12/28/36/12，原XSD AssetTypeTerrainAsset.xsd与AssetTypeTerrainTextureAtlas.xsd。

两个根均reserved@0、count/pointer@4/8。TerrainAsset每项36：id string@0/4、Texture string@8/12、BumpTexture string@16/20、Class enum@24（absent0、unknown Cliff3）、WBFolder string@28/32。字符串按原Windows ANSI/C-string，present-empty仍分配NUL4；Texture是名字字符串而非强引用。按字段顺序分配每项四string。

Atlas Tile12：TextureID uint32@0、UpperLeftX/Y与BottomRightX/Y为四uint16@4/6/8/10，unsigned解析后截low16；大端分别swap16位，不能按uint32交换。Atlas根@12..27所有额外runtime数据字段原XmlHandler未读取/写入，BaseTextureAtlas/NormalTextureAtlas两个schema属性在此入口被忽略，明确对照测试，不冒充贴图文件加载。

TestXmlterrain.ps1完整/Od、/O2 × 平台0/1/2/99，每原调用Initialize，恢复ProcessInstance/CompileXml各三数组对照。覆盖所有11class+大小写/unknown/NUL、四string完整引用形式/Unicode/NUL/4097长度、五整数及65535/65536/65537、ignored atlas属性、0..1024列表、16种string存在组合。独立ABI核大小/偏移、四string顺序/指针/空分配、Class值、Tile四uint16精确bits/endian/截断、root额外字段零、imports空及重定位。未知字段/结构、非法文本、foreign namespace、qualified属性、空声明/handle不匹配显式拒绝。Linux native语法通过不能代替Windows验收。
