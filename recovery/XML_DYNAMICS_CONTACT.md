# DynamicsContactTagMapping完整组件

2026-10-25，`dynamics`单根组件；最终Windows差分以本轮汇总与`evidence/xml-dynamics/validation.json` sourceCommit/runId为准，SDK未接入、未执行。

固定Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`，原DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。

原证据：根Marshal直接内联原Plugin HandleType<DynamicsContactTagMapping>（读取Mapping列表、非Win32大端）；`parts/part-0107.cs.txt`原行53608 ContactTagMapping完整Marshal/53625 Endian；`part-0153.cs.txt`原行76833 DynamicsContactTag enum、76844 Mapping16列表与Endian；`enums/DynamicsContactTag.json`8项精确静态表；原StructLayout12与16，XSD System/DynamicsContactTagMapping.xsd。TypeId1044178106、TypeHash898281629。

根12字节：reserved@0、Mapping count/pointer@4/8。每项16：ContactA/ContactB enum@0/4（缺失0，present unknown7；NONE0/GROUND1/WATER2/VEHICLE3/PROP4/WALL5/TREE6/DEBRIS7，大小写敏感、C-string截断），MinForce float@8默认1.2f（0x3f99999a），FX直接强引用@12。允许空列表、不截断数量，保持XML顺序；非Win32逐enum/float/count校正，强引用和指针由Tracker处理。

`TestXmldynamics.ps1` /Od、/O2 × 四平台0/1/2/99，每原调用显式Initialize；恢复ProcessInstance/CompileXml各三个数组完整对照。包含两字段全部enum、unknown/大小写/空/NUL、float CRT精度/溢出/D指数/Unicode、强引用边界、Mapping0..1024增长。独立ABI核12/16、缺失enum0和force1.2、全部偏移/floatbits、FX index+1和导入index排序、单列表重定位；未知字段/节点、非法文本、foreign namespace、qualified属性、空声明/handle不匹配显式拒绝。没有改SDK/Tokenizer/BAB预处理。
