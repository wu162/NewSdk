# ProjectilePath完整组件

2026-10-25。组名 `projectilepath`，单根完整入口；最终Windows验证以本轮汇总和 `evidence/xml-projectilepath/validation.json` 的sourceCommit/runId为准。SDK未集成、未执行。

固定原Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`，原DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。

原证据：`evidence/xml-source/parts/part-0100.cs.txt` 原行50351完整根Marshal；`part-0037.cs.txt` 原行18584 Vector4 Marshal，x/y/z/w逐字段默认0.0；`part-0152.cs.txt` 原行76069 Vector4列表16字节步长/Push、76111 Endian；`part-0040.cs.txt` 原行20496 Vector4 EndianCorrect；原Plugin `HandleType<ProjectilePath>` 对非Win32全部大端，先校正nullable scale再校正point list。原StructLayout16，XSD `AssetTypeProjectilePath.xsd`。精确TypeId1710207315、TypeHash955128953在组件catalog中。

根布局：reserved@0；ComponentScale nullable16字节Vector4指针@4；Point count@8和pointer@12。scale缺失不分配，present-empty分配16字节全0；所有Point各16字节浮点x/y/z/w。总是先分配scale、再Point列表，即使XML中Point先出现。原Marshal允许0点（不强加XSD minOccurs1）；不截断列表。四坐标使用已验证VC8 ParseFloat行为，大端逐float和列表count校正，pointer交给Tracker。

`TestXmlprojectilepath.ps1` `/Od`和`/O2` × 平台0/1/2/99，原ProcessInstance与恢复ProcessInstance/CompileXml的三数组各完整对照。Oracle覆盖两个节点全部4字段、CRT/D指数/溢出/精度边界/NUL/Unicode、0..1024列表、scale有/无与交错顺序、partial向量。独立ABI核root16/scale16/point16、全部float bits、无scale时point直接@16、存在scale时point@32、计数、精确大小与重定位顺序。显式拒绝重复scale、unknown字段/子节点、大小写错误X、foreign namespace、qualified属性、非法文本、空声明、handle不匹配。root id/inheritFrom与TypeId接收既有BAB预处理后的元数据，不扩展到Tokenizer/SDK。
