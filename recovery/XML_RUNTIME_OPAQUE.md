# 两个明确的XmlCompiler no-op登记入口

2026-10-25，`runtimeopaque`仅恢复原Plugin已登记的BaseRenderAssetType与AudioFileRuntime入口语义。**不是恢复W3D渲染资产编译器或自定义AudioCompiler，不代表音频文件解码、声音头生成或完整W3D支持。** 两者均非CnC3Types.xsd全局资产（BaseRender是abstract schema）；不把unknown TypeId fallback作为新入口。SDK未接入、未执行。最终差分锚点由本轮汇总与`evidence/xml-runtimeopaque/validation.json`给出。

固定Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`、原DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。

`evidence/xml-runtimeopaque/original-contract.json`逐根保存完整原Handler方法的源路径/起止行/UTF-8/LF规范化方法SHA256和完整核心Marshal→Finalize代码；原完整方法保留在decompiled原Plugin，非摘要替代取证。

- BaseRenderAssetType：原Plugin完整HandleType只构建Node、4字节root Tracker、设置EndianHelper后直接FinalizeTracker，无Marshal调用、无字段getter、无子分配。StructLayout4，schema AssetTypeW3D.xsd abstract BaseAsset扩展、无字段。精确TypeId2989459401、TypeHash3866869572。
- AudioFileRuntime：原Plugin完整HandleType构建32字节root，只调用节点Value getter取得C-string后丢弃，没有读取或写入六个schema属性，也未调用任何音频解析方法；随后调用完整AudioFileRuntime EndianCorrect。`parts/part-0038.cs.txt`原行19290仅swap@4/12/16/24中的零，不分配、不写数据。原StructLayout32，Tracker构造见`part-0177.cs.txt`88516，BaseRender Tracker见`part-0180.cs.txt`90063。schema AssetTypeAudio.xsd六属性SubtitleStringName/NumberOfChannels/NumberOfSamples/SampleRate/HeaderData/HeaderDataSize在这个Xml入口都被忽略。精确TypeId584092973、TypeHash2941751938。

因此两个准确登记TypeId的输出分别4/32字节全零，Imports/Relocations空，平台0/1/2/99均相同；Audio六个schema属性与根文本变化也不改变输出。适配器接受这些已证实字段，未知新字段或结构显式拒绝，不假装提供未恢复功能。

`TestXmlruntimeopaque.ps1` /Od、/O2 × 四平台，原ProcessInstance每次Initialize平台，恢复ProcessInstance/CompileXml各三数组完整比较。全六schema属性分别覆盖全部数值/引用/Unicode/NUL/4097长度边界，64种属性存在组合、任意根文本与metadata；独立ABI严格逐字节核全0、精确长度、无侧数组。未知属性/子节点、qualified属性、foreign namespace、空声明/handle不匹配显式拒绝，保留已知no-op语义与未知fallback的区别。APT真实File/DataBlob另在aptdata完整恢复，绝不归入no-op。
