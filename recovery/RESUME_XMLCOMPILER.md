# XmlCompiler：范围内还原及真实SDK验收已完成

2026-10-25（北京时间），目标分支 `recovery/xmlcompiler-priority`。先读AGENTS.md、EFFICIENCY.md、XMLCOMPILER_PRIORITY.md及XML_SDK_RECOVERY.md。

原Plugin301项，排除TestGameObject和11个Joypad独立根后，289入口组件还原完成。当前289路由元数据在平台0/1/2/99验证；其中267个schema全局根已完成真实SDK/O2、Win32验收。22个非全局/抽象/runtime登记项不伪造全局资产，不声明独立SDK根入口调用；必要嵌套布局以实际owner证据为准。不是301入口或完整Tokenizer/非XML自定义编译器的替代。

SDK最终源码 `9b2ef85bc45e6b397fce3d275165715afa921a92`，运行[40164710673](https://github.com/wu162/NewSdk/actions/runs/40164710673)，completed/success。精确收据 [acceptance.json](evidence/xml-priority-sdk/acceptance.json)；五profiles共783次实际SDK调用，原版与无原DLL隔离恢复版重放合计4698次三缓冲区比较，实际类型并集精确267；每profile的bin/manifest/imp/relo原/恢复四产物相同。只复现EALAModStudio“建立基础数据”case5 BAB，清理阶段1/2；没有GUI、LoD或打包。

Baseline159次/A173次沿用887444e8b46ef185fdd120df5403dc8ff584840f/40162875013的当前联合DLL成功证据，共332次旧调用未重跑；当前Native/输入/metadata及委托路径不变证明和每个旧XML的identity guard齐备。最终运行仅新建B277、FullMymod162、APT12次调用的三个profile，6次原/恢复真实BAB构建。APT专项四根12次调用覆盖8个非空文件和4个空文件，236个payload字节逐字节对照；旧A的空payload不冒充文件读取能力证明。

组件锚点：GameMap完整矩阵092bb95c06647f95891c7f62b4b6fbc875c4c07a/40158416072；UIConfigList完整70内部分派4401069feaf5df91043afadaad2410293c33b67e/40159834447，均/Od、/O2、四平台。63组和289入口索引在evidence/xml-component-acceptance-20261025-289.json。SDK阶段未修改Native/Bridge，沿用已验证/O2联合DLL，不重复触发未变成功组。

历史82入口SDK清单仍保持92180f9bbd830b9dbd49a696c259e54ffe2d7721/40098695436原锚点，不能当作当前289路由DLL的验收。当前SDK状态以catalog/XmlSdkTypes.json和新收据为准。 证据XML/TSV按目录.gitattributes以CRLF checkout或直接读取Actions artifact，四产物保留二进制原字节。12个排除根只有明确扩展范围才恢复；22个非全局根若需独立SDK入口证明，应先说明真实可达生产路径。保留所有失败/修复/Actions父历史，禁止强推。
