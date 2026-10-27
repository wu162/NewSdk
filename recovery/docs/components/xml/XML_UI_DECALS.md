# UI decals: four-root component

组件最终验收（2026-10-25）：源码d9908f57107f11b7ab875ac5787fce2bf23e00a9，运行40132543491 completed/success；/Od、/O2及平台0/1/2/99全部通过，183入口联合DLL也通过。用户明确禁止本轮SDK真实编译，实际0次；本组SDK仍待验收，未加入活跃82类SDK路由。精确汇总见evidence/xml-component-acceptance-20261025.json。 本组两模式合计11408 fixtures、68448数组比较、496拒绝。

2026-10-25（北京时间）。源码与完整组件差分矩阵准备；Windows验证由集成代理统一执行。本轮用户禁止真实SDK编译，不接SDK活跃路由、不改SDK82支持计数。本组不依赖RadiusDecalComponent64；原Marshal实际使用inline DecalSettings20与RenderInfo20，未修改旧共享组件。

固定原Module blob2d2f471c8008b7c2e1f78e39752d0726da395cea、Plugin blob00bb9e5df72d07a21204f85f9a9a2f1d9557f00b、DLL SHA256 d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6。按方法小块取证，没有原DLL调用。

| 根 | TypeId | TypeHash | 字节 |
| --- | ---: | ---: | ---: |
| UIDecalGrid | 2599418799 | 4073471 | 56 |
| UIDecalCloud | 2524893660 | 325319211 | 16 |
| UIDecalDestinationMarker | 1532214660 | 4106878882 | 32 |
| UIWorldBuilderDecalGrid | 1238702511 | 1469314420 | 16 |

## 原Marshal、Endian、Tracker合同

- Grid Marshal49291/Endian49355（part-0098）：WallTexture直接ref@4、WallOpacity float@8默认1.0、WallDecalColor packedARGB@12；PrimarySettings inline20@16，SecondarySettings inline20@36。Settings缺节点保持20字节零，存在节点才填Opacity默认1.0；不能无条件给两个缺失settings填默认。Plugin Handle24087，Tracker108718（part-0217）Allocate(1,56)。
- DecalSettings Marshal49210/Endian49260（part-0098）：ValidTexture ref@0、InvalidTexture ref@4、Opacity float@8默认1.0、LegalColor packed@12、InvalidColor packed@16。没有pointer/string/list。
- Cloud Marshal49200（part-0098）：BuildTexture/DefenseTexture/SpecialPowerRestrictionTexture直接ref@4/@8/@12；无额外scalar endian。Plugin23988，Tracker108627（part-0217）Allocate(1,16)。
- DestinationMarker Marshal49176（part-0098）：DefaultRenderInfo inline20@4、DestinationTypeSettings list count@24/pointer@28。Plugin23877内联交换DefaultRenderInfo.ZBias@8及RGB@12，随后完整列表Endian；Tracker108536（part-0217）Allocate(1,32)。
- RenderInfo Marshal49095（part-0098）：Model BaseRenderAssetType直接强引用@0、ZBias float@4默认0、Color RGBColor inline12@8，RGB为三个float而不是packed整数。
- DestinationTypeSettings Marshal49126/Endian49150（part-0098）：24字节项，DestinationType enum@0、RenderInfo inline20@4。列表Marshal75935/Endian75977（part-0151）一次Push24*count，再按顺序逐项Marshal；无需逐RenderInfo分配。
- DestinationType enum helper75924（part-0151），Normal0、Aggressive1、unknown/empty fallback1；缺属性保持zero。静态map在enums/DestinationType.json行12332。Native以原C字符串终止语义比较Normal，其他present token映射1。
- WorldBuilder Marshal54146（part-0108）：Texture ref@4、Opacity float@8默认1.0、Color packedARGB@12；Plugin28761内联scalar endian，Tracker112813（part-0225）Allocate(1,16)。
- PackedColor原ColorTag Marshal18499（part-0036/0037）：R/G/B默认为0，A默认255，strtoul(base0)后byte截断，输出ARGB。absent color保持零，present-empty color变0xff000000。RGBColor原Marshal18721（part-0037）：R/G/B是独立float，缺属性保持零，原helper没有A。
- 全部非Win32含99采用big endian。Native延后统一交换scalar words，直接refs/pointer的Endian/index+1及owner-block/value排序仍交给已验收Tracker。仅使用ScalarParsing与ResourceMarshaling，无Joypad/Shader/完整UI模块依赖。

## 验证与边界

scripts/TestXmluidecals.ps1在/Od、/O2各自完整平台0/1/2/99运行。tests/XmluidecalsOracle.cs每fixture原ProcessInstance对恢复ProcessInstance及CompileXml三数组，共6比较。覆盖全部11个引用字段、12个float字段、6处packedColor全部RGBA字段与溢出/hex/负值/NUL、完整Destination枚举及缺省未知区别，DestinationSettings列表0/1/2/3/15/16/17/27/28/29/64/257/1024增长，全部独立ABI/default/presence/指针/owner-block,value排序、拒绝矩阵。

ABI明确Grid56两个inline20位置；Marker32+24*count，DefaultRenderInfo floatRGB；packedARGB present-empty alpha255；Grid所有rootrefs反向唯一值imports40,20,36,16,4；Marker rootblock与listblock混合imports4,60,36。SideArray只检查对齐/界限/唯一/哨兵，不假设地址单调。原生临时x64语法检查不代替Windows验收。

Bridge公开XmluidecalsCompiler和四个完整根名Compiler；BAB裸TypeId允许，未知/限定属性、外命名空间、未知元素、结构文本、重复singleton及root/handle不符明确拒绝。组件底层缺省能力与XSD必填范围分开测试，未放宽原XSD。

原schema在UI/DecalGrid.xsd、UI/DecalCloud.xsd、UI/Decal/DestinationMarker.xsd、UI/WorldBuilderDecalGrid.xsd，四根BaseAssetType不支持inheritFrom。Grid需要三个child及对应Texture字段，Cloud三Texture必填，Marker至少一Destination设置并要求各RenderInfo/Color，World需要Texture/Color。本轮不创建或运行SDK样例，未来允许SDK批次再统一验证。Types在catalog/XmluidecalsTypes.json，主Plugin/catalog/工作流/联合smoke由集成代理负责。

最终是否通过以实际sourceCommit/runId、完整validation.json及两模式真实退出码为准。
