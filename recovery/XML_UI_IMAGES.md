# UI图像和绘字配置完整组件

2026-10-25，`uiimages`组包含ImageSequence、UnitTypeIcon、DrawGroupInfo。最终Windows差分锚点由本轮汇总与`evidence/xml-uiimages/validation.json`给出；SDK未接入、未执行。

固定Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`、原DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。

原证据：原Plugin `HandleType<ImageSequence>`直接内联Marshal（未单独生成Module根方法），`parts/part-0143.cs.txt`原行71969 Image强引用文本列表；`part-0088.cs.txt`原行44098 UnitTypeIcon完整Marshal；`part-0092.cs.txt`原行46360 DrawGroupInfo完整Marshal及46459 Endian；Vector2与ColorTag沿既有完整原证据/共享scalar语义。原StructLayout分别12/12/52，三个原Plugin Handler均非Win32大端；TypeId/TypeHash在`catalog/XmluiimagesTypes.json`。XSD AssetTypeImageSequence.xsd、AssetTypeUnitTypeIcon.xsd、AssetTypeDrawGroupInfo.xsd。

ImageSequence12：reserved@0，Image count/pointer@4/8，列表每项4字节强引用，读取Image节点文本。允许空列表、不限制数量，强引用index+1及owner/index导入排序。

UnitTypeIcon12：Image强引用@4、Offset nullableVector2指针@8；absent无分配，present-empty分配8字节float x/y默认0.0；非Win32逐float校正。

DrawGroupInfo52：FontName string长度/指针@4/8默认Arial（含NUL、4字节对齐，因此默认总60字节）；FontSize unsigned@12默认12；DropShadowOffsetX/Y signed@16/20默认-1；PixelOffetXInteger/YInteger signed@24/28、PixelOffetXReal/YReal float@32/36（保留原拼写Offet）；TextColor/TextDropShadowColor packedARGB@40/44，absent0、present-empty alpha255；IsBold/UsePlayerColor/PixelOffsetAsPercentage bool@48/49/50默认0/1/0，padding51为0。字体字符串按原Windows ANSI转换后C-string截断（原Value构造调用Marshal.StringToHGlobalAnsi，Module行118062），present-empty仍分配NUL；只校正数值长度，原字符字节不交换。

`TestXmluiimages.ps1` /Od、/O2 × 平台0/1/2/99；每次原ProcessInstance显式Initialize，恢复ProcessInstance与CompileXml各三个缓冲区完整对照。覆盖全部字段、strong refs、float/CRT、signed/unsigned、bool、packedARGB全部通道、字符串Windows ANSI/NUL/各4字节边界至4097、Image列表0..1024增长。独立ABI核默认值、12/12/52大小、present/absent/empty、全部字段偏移、float bits、bool/padding、指针及导入排序、通过独立Marshal.StringToHGlobalAnsi核字符串字节/长度/NUL/padding。未知字段/节点、foreign namespace、qualified属性、重复singleton、非法文本、空声明/handle不匹配显式拒绝；不恢复BAB预处理、Tokenizer或SDK正式路由。
