# GameScriptList完整XmlCompiler组件

2026-10-25，gamescripts单根完整注册入口，不恢复自定义ScriptCompiler或解释执行游戏脚本。SDK未接入、未执行；最终Windows验收由本轮汇总与evidence/xml-gamescripts/validation.json sourceCommit/runId锚定。

固定Module blob 2d2f471c8008b7c2e1f78e39752d0726da395cea、Plugin blob 00bb9e5df72d07a21204f85f9a9a2f1d9557f00b、原DLL SHA256 d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6。原parts/part-0087.cs.txt43909参数数字TypeId分派/Endian、43978 Action raw FastHash；part-0088.cs.txt44023 Script/44037 Group/44067 root；part-0142.cs.txt71313两种参数精确Push、71442 Action列表；part-0143.cs.txt71560递归Group列表/71602 Script列表及Endian；part-0173.cs.txt86503参数指针列表与Group typedId helper。原StructLayout8/20/12/12/8/12，schema AssetTypeGameScriptList.xsd，原Plugin HandleType非Win32大端。根TypeId3737094024、TypeHash2724763994。

root8：reserved@0、ScriptSet nullable Group20 pointer@4。Group20 id低写TypedAssetId@0、Script count/pointer@4/8、ScriptGroup count/pointer@12/16；总先分配Scripts及其内部数据，再分配递归Groups，不按XML交错顺序。Script12 id低写typedId@0、Action列表@4/8。Action12 Type大小写敏感raw StringHash@0、ParameterList count/pointer@4/8。ParameterList每项4字节指针，按数字TypeId分派，元素名不决定类型：ObjectType58964679分配8，tag@0/directObject强引用@4；ObjectTypeList524038440分配12，tag@0/Object引用list count/pointer@4/8，项4字节文本强引用。unknown或missing TypeId原fallback为4字节tag0基类，不计额外入口；未知payload显式拒绝。所有数字word大端，pointer/import由Tracker处理。Bridge保持既有32层XML深度边界。

TestXmlgamescripts.ps1 /Od与/O2 × 平台0/1/2/99，每原ProcessInstance显式Initialize平台，恢复ProcessInstance/CompileXml各三个数组完整比较。全部id/raw hash Unicode/NUL/4097，两个参数完整强引用边界、数字TypeId格式/错元素名/原fallback，Script/Group/Action/Parameter/Object列表分别0..1024增长、递归0/1/2/3/7/15/24层。独立ABI核8/20/12/12/8/12/4、mixedcase弱id与raw hash区别、完整嵌套字段偏移、TypeId/引用index+1、先Script后Group分配与ownerblock导入/重定位顺序。未知字段/文本/结构、foreign根/qualified属性、重复singleton、空声明/handle不匹配显式拒绝。完整字段和依赖均实现，没有截断半个root。
