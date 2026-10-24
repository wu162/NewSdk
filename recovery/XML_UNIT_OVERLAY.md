# UnitOverlayIconSettings：组件恢复

2026-10-25（北京时间）。本组新增一个原Plugin登记入口，TypeId=2421413379、TypeHash=3893526655。组件差分待验证，SDK未集成、未验证，本轮禁止SDK真实编译。

证据固定于原Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`，原DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。原方法可在 `evidence/xml-source/method-index.txt` 定位；原Module完整本地反编译行44444–44644、65951、72014–72191、99800，原Plugin HandleType行13311、登记信息39614。schema `SDK/Schemas/xsd/AssetTypeUnitOverlayIconSettings.xsd` 及 `CnC3Types.xsd` 全局资产声明已交叉核实。

## 完整字段与布局

根104字节，保留word0；13个强引用位于4..52，依原Marshal顺序：AmmoPipImage、AmmoPipFrameImage、ContainPipImage、ContainPipFrameImage、GarrisonPipImage、UnitTypeIconBackgroundImage、TargetIndicatorImage、TargetIndicatorGlowImage、PowerIconImageSequence、RepairIconImageSequence、InfiltratedIconImageSequence、HealUnitIconImageSequence、RepairVehicleIconImageSequence。

8个百分比在56、64、68、72、76、80、84、88，依次为AmmoPipScale、ContainPipScale、GarrisonPipScale、UnitTypeIconScale、StatusIconScale、VeterancyIconScale、TargetIndicatorScale、HealingIconScale。原getter全部默认100%，逐个核对MSVC百分号静态符号，不能误读为无默认。AmmoPipRange为signed int@60；HealingIconDisplayTime为Time@92，默认3s。

FactionVeterancy列表count@96、pointer@100、项12字节；Faction@0为完整五值FactionType，case-sensitive C-string查表，未知/缺失0、RANDOM=-1。RankIcon列表count@4、pointer@8、项8字节，signed Rank@0与强Image引用@4。根列表先一次分配全部Faction项，再按Faction顺序分别分配各Rank列表；不按XML全局字段地址重排。标量、列表计数与enum进行endian纠正；强引用/指针交由Tracker统一处理。imports按owner block、引用index排序；relocations按owner block、target block排序。

## 实现与验收

`src/Native/Xmlunitoverlay.{h,cpp}` 与 `src/Bridge/XmlunitoverlayCompiler.cpp` 仅复用既有ScalarParsing/DataInitializers/ResourceMarshaling/Relocation，不修改共享helper。显式拒绝未知字段/节点、非asset命名空间、限定属性、结构文本和root/TypeId不匹配。TypeId元数据允许在嵌套节点出现。

`tests/XmlunitoverlayOracle.cs` / `scripts/TestXmlunitoverlay.ps1` 保留原ProcessInstance三数组、直接CompileXml三数组比较，/Od和/O2、平台0/1/2/99，13根引用和Rank引用全部边界、8百分比、signed整数、Time、五枚举/未知/NUL、两层列表0..1024增长、独立152字节嵌套ABI和逆序引用书签、默认值及partial-attribute默认值。联合DLL默认根smoke另由TestXmlPriorityLink纳入，不代替本组完整矩阵。
