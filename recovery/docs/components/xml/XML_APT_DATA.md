# APT DataBlob完整XmlCompiler组件

2026-10-25，`aptdata`包含AptAptData/AptConstData/AptDatData/AptGeometryData四根原XmlCompiler入口；这是File→DataBlob组件语义，**不是恢复自定义APTCompiler或解析APT/CONST/DAT/geometry文件格式**。SDK未接入、未执行；最终Windows差分锚点见本轮汇总与`evidence/xml-aptdata/validation.json`。

固定Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`、原DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。

原证据：三个AptApt/Const/Dat根完整Handler直接在Plugin内联读取File属性后调用DataBlob；`parts/part-0088.cs.txt`44131 Geometry额外读取signed AptID；`parts/part-0036.cs.txt`18350完整DataBlob Marshal真实执行fopen(rb)、seekend/ftell、Push(1,size)、seekstart/fread、close/Pop。不是no-op。原StructLayout前三根12、Geometry16；DataBlob为pointer@0/length@4。全部Handler非Win32仅swap length，Geometry另swap AptID。schema AssetTypeAptAssets.xsd，精确TypeId/TypeHash在catalog/XmlaptdataTypes.json。

根reserved@0、DataBlob pointer@4/length@8、Geometry AptID signed@12。File absent或fopen失败均保留零，不分配；文件成功则先设置字节长度，分配原始字节数并4字节对齐，fread原字节、不做字节序变换；短读按原方法将length清零但保留已分配块。空文件成功也执行零长度Push，产生pointer重定位，其target可等于根末尾。普通regular、seekable文件为明确支持范围；无效seek/ftell显式拒绝，避免把原危险失败路径伪装成成功。路径遵从原Windows ANSI/C-string转换及首NUL截断，不解释URI。

`TestXmlaptdata.ps1` /Od与/O2 × 平台0/1/2/99，固定原DLL，每次原ProcessInstance显式Initialize平台，恢复ProcessInstance与CompileXml各三个数组完整比较。Oracle仅在临时目录生成确定字节pattern文件，大小覆盖0/1/2/3/4/…所有4字节边界、27/28/29、255/256/257、1023/1024/1025、4095/4096/4097、65535/65536/65537；不改SDK任何文件。四根逐文件读与独立ABI核原始每字节、长度、pointer、padding、空文件零分配重定位、missing/empty/URI/Unicode错误路径、首NUL有效路径，以及Geometry完整signed边界。只有此组SideArray允许`at4,target==rootEnd,length0`这一个严格零文件合同；其余界限不放宽。

未知字段/节点、非法文本、foreign namespace、qualified File属性、非Geometry AptID、空声明/handle不匹配显式拒绝。没有SDK/Tokenizer/BAB预处理/自定义APTCompiler路由变化。动态文件的短读/并发修改和非seekable对象不冒充已通过常规文件矩阵的场景。
