# Road / ShaderOverride / ShaderOverrideRule

2026-10-25（北京时间）。新增3个完整原XmlCompiler登记入口，仅组件源码，等待完整Windows差分；SDK未集成/未验证。

| 类型 | TypeId | TypeHash | 根大小 | 全局schema |
| --- | ---: | ---: | ---: | --- |
| Road | 3516764358 | 738011492 | 24 | 是 |
| ShaderOverride | 3166846828 | 1029381398 | 16 | 是 |
| ShaderOverrideRule | 3648907125 | 279003514 | 16 | 否，独立Plugin入口且为嵌套schema |

固定原Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`、DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。原Module方法21526/21546/21582、43057、61507/61519/61561，已核完整Marshal/Endian及nullable TypedAssetId helper；Plugin三个HandleType及GetExtendedTypeInformation登记、Tracker根大小、CnC3Types全局声明与AssetTypeShaderOverride.xsd交叉核实。

Road根保留word0，Texture强引用@4、NormalTexture强引用@8，RoadWidth/RoadWidthInTexture/Priority三个float@12/16/20。Priority是float，不得与ShaderOverride的unsigned Priority混用。

ShaderOverride保留word0，unsigned Priority@4默认1，Rule count@8/pointer@12，项16字节。规则保持XML列表顺序；先一次分配完整Rule数组，然后逐项处理其三个字段。

ShaderOverrideRule首word是IfOriginalShaderIs nullable weak hash指针，不是保留word。只要属性存在，即使空也分配4字节weak hash；ReplaceShaderName直接case-folded weak hash@4；ReplaceTechniqueName为string length@8/pointer@12，存在空字符串仍分配终止NUL，缺失保持zero。Rule先nullable弱引用，再inline弱hash，再string分配。弱引用不产生imports。两个弱hash、string长度和Priority/float按平台swap，指针统一Tracker重定位。

新Xmlmaterialrules Native/Bridge/Oracle/Test脚本/catalog复用既有helpers，无共享改动；拒绝未知字段、非asset/qualified节点、结构文本、错误root/TypeId。不把原独立ShaderOverrideRule入口误计为未知fallback。

完整矩阵保持/Od、/O2、0/1/2/99、原ProcessInstance三数组+直接CompileXml三数组；覆盖全部字段、CRT signed/unsigned/float区别、Unicode/NUL/强弱引用、字符串0..65535、Rule列表0..1024、nullable空与缺失、独立24字节Road和64字节Shader嵌套ABI、根Rule指针@0、owner/target block重定位顺序及拒绝边界。联合非空样本另作smoke，不代替完整组矩阵。
