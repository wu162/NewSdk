# ConnectionLineManager完整FXShader组件

2026-10-25，`connections`组单根完整入口，含FXShaderMaterial与四种ShaderConstant完整依赖。最终Windows差分锚点由本轮汇总与`evidence/xml-connections/validation.json`给出，SDK未接入、未执行。不重复ShaderOverride/Rule。

固定Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`、原DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。

原证据：`parts/part-0090.cs.txt`原行45097根Marshal；`part-0041.cs.txt`原行20536 Bool、20565数字TypeId分派、20597 FXShaderMaterial、20644派生Endian、20748 Float Endian、20771 Int、20826 Texture；`part-0040.cs.txt`20327基类Name；`part-0122.cs.txt`61004..61125四派生Push/unknown基类/Endian；`part-0162.cs.txt`81450 float Value列表；`part-0164.cs.txt`82201常量指针列表。原Plugin Handler内联FXShaderMaterial的长度/count endian；原StructLayout根32/material28/base12/Bool16/Int16/Texture16/Float20；schema AssetTypeConnectionLineManager.xsd与AssetTypeW3D.xsd。TypeId105587076、TypeHash1802638191。

root32：reserved@0，FXShaderMaterial内联@4。ShaderName string@0/4（无默认、absent不分配）；TechniqueName string@8/12默认空（FXShader present即分配NUL4）；Constants count/pointer@16/20，4字节指针项列表；TechniqueIndex unsigned low8@24，padding25..27零。根缺少FXShader则整个inline28零，不创建默认TechniqueName。

常量分派严格按BAB数字TypeId，不按元素名推断：Float3741900581/20字节，Bool2750958653/16，Int2300057986/16，Texture2777716390/16。每块type tag@0、Name string@4/8（absent不分配），payload@12。Float为count/pointer@12/16和4字节float Value列表，先分配Values再Name，无XSD max4截断；Bool单字节@12无endian，Int signed@12，Texture直接强引用@12，后二者Value取单节点文本。所有常量都先处理派生payload再基类Name。常量指针列表按原顺序逐项分配；material字符串在常量列表前分配。

原unknown/missing TypeId走12字节基类，type tag固定0、仅Name字符串；此原fallback路径单独测，不作为额外已恢复入口计数，带未知payload的类型显式拒绝，避免默默丢弃未恢复数据。所有数值TypeId解析沿VC8 unsigned base0；已知id即使元素叫Any仍按数字派生合同执行。所有数值word大端校正，字符串原字节不swap，TechniqueIndex/Bool保留byte，引用/指针由Tracker处理。

`TestXmlconnections.ps1` /Od、/O2 × 平台0/1/2/99；每原ProcessInstance显式Initialize平台，恢复ProcessInstance与CompileXml各三数组逐字节比较。覆盖全部字段、四派生present-empty/absent、Name/Shader/Technique Windows ANSI/NUL/4097字符串、Float/Int/Bool/Texture完整scalar/ref边界、数字TypeId/元素名不一致、原base fallback、float Values0..1024及常量列表0..1024增长。独立ABI核32/28/12/16/20、四type tag、Float先Values后Name、两material strings、TechniqueIndex low8/padding、Bool byte、强引用index+1、ownerblock/target重定位顺序及imports。未知字段/结构、重复singleton、非法文本、foreign根/qualified属性、空声明/handle不匹配显式拒绝。未扩展BAB预处理/Tokenizer/SDK路由。
