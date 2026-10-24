# Tactical UI hotkeys: five-root component

2026-10-25（北京时间）。源码和完整差分矩阵准备；Windows验证待集成代理统一运行。本轮用户禁止真实SDK编译，因此不改SDK活跃路由、不增加SDK支持82计数。本组五根只标组件候选。

原证据完整锚点见 XML_TACTICAL_HOTKEYS_NEXT.md。固定 Module blob2d2f471c8008b7c2e1f78e39752d0726da395cea、Plugin blob00bb9e5df72d07a21204f85f9a9a2f1d9557f00b，原DLL SHA256 d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6。本轮复用上轮已完整核对的Marshal/Endian/Tracker/dispatch/schema取证与缓存，未重下大型反编译。

Native/Xmltacticalhotkeys.cpp完整覆盖Selection12、Main24、Player12、Misc200、Gate12五根。47个Misc directref原offset@4..188，Stance list@192；Main SelectionRefinementPageSlot@4、BuildQueue list@8、ButtonSlot list@16并保持schema分配顺序。两个纯reference列表元素4字节，三个enum/ref列表元素8字节。enum属性缺省保持零，present未知分别Queue4、Stance1、ElementId21；完整原枚举map在Fields.inc。所有引用由已验收ResourceMarshaling处理，Tracker负责pointer/ref endian与block/value排序。无原DLL回调、无Joypad/完整UI组件依赖；唯一资产依赖HotKeySlot已独立恢复。

Bridge公开XmltacticalhotkeysCompiler与五个完整根名Compiler，接受BAB注入裸TypeId；仍依赖BAB预处理引用/defaults，未知属性/子节点、限定属性/外命名空间、结构非空文本、叶子元素子节点、root/handle不符明确拒绝。五根schema均BaseAssetType不允许inheritFrom，组件接受元数据兼容既有桥接约定；本轮不放宽原XSD、不触发SDK。

测试入口scripts/TestXmltacticalhotkeys.ps1在/Od、/O2各平台0/1/2/99跑原ProcessInstance对恢复ProcessInstance和CompileXml三数组，共每样本6比较。Oracle覆盖47字段每个引用边界（signed atoi溢出、空/缺失/NUL/ANSI/多分隔符）、三个完整enum/大小写/空/缺属性/未知与NUL、所有列表0/1/2/3/15/16/17/27/28/29/64/257/1024增长、Main不等长列表和交错XML分配顺序、全部独立偏移/大小/引用index+1及拒绝边界。

独立排序门槛：通用SideArray只核对齐、界限、唯一与哨兵，不假设field address递增；ABI用降序唯一引用值分别核root block在前、block内部value升序。Misc47rootrefs要求imports188→4，再按Stance list引用值排序；Main要求imports4,36,28,44,48,40。重复/容量边界完整三数组仍逐字节对照原DLL，不用简化排序替代Oracle。

本地仅原生C++语法检查（临时x64副本去掉x86 static_assert），不代替Windows/Od/O2四平台验收。最终状态须以实际sourceCommit/runId和完整validation.json为准。Types元数据在catalog/XmltacticalhotkeysTypes.json，主Plugin/catalog/联合smoke由集成代理接线；本组未修改共享工作流/SDK配置。
