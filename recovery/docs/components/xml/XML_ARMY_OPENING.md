# ArmyDefinition / SkirmishOpeningMove

2026-10-25（北京时间）。完整恢复两根XmlCompiler登记入口；组件差分待验收，SDK pending，不修改GameObject实现。

| 类型 | TypeId | TypeHash | 根大小 |
| --- | ---: | ---: | ---: |
| ArmyDefinition | 3587539190 | 213154423 | 64 |
| SkirmishOpeningMove | 3971904488 | 1878382208 | 20 |

固定Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`，DLL SHA256沿用组件原件。原Module方法56842/56871/56913、58424及紧随的Endian、77338（AIBankAccountType）、78626/78668列表；原Plugin两个HandleType/GetExtendedTypeInformation及Tracker大小核实。两者均在CnC3Types全局声明，schema SkirmishAI/AssetTypeArmyDefinition.xsd与AssetTyeSkirmishOpeningMove.xsd交叉检查。

Army：word0保留，Side string length@4/pointer@8先分配；11个GameObject弱hash依次在12..52：MCVTemplate、GathererTemplate、ExpanderTemplate、EnergyProducer、PrimaryResourceNodeCollector、PrimaryResourceThingToBuild、SecondaryResourceNodeCollector、VehicleRepairFacility、NavalRepairFacility、AircraftRepairFacility、WallPiece。TechStructure列表count@56/pointer@60，4字节弱hash项，保持输入顺序。弱hash按既有C-string/lowercase合同，不产生imports；引用GameObject不等于恢复GameObject源码。

Opening：word0保留、Side string@4先分配；Order count@12/pointer@16，项20字节。每项Time float@0默认0s，Build string length@4/pointer@8默认空（缺失仍分配NUL），Account enum@12默认SLUSH_FUND=5，未知值按原lookup fallback DEFENSE=3，Deposit unsigned@16默认0。完整6值：DEFENSE3/INVESTMENT0/PRODUCTION2/SLUSH_FUND5/SPECIAL_OPERATIONS1/TECHNOLOGY4。先整个Order数组后逐项Build string，不套用schema长度限制。Side缺失保持空指针，与present empty分配NUL严格区分。

所有长度/count/hash/enum/float按平台endian纠正，指针交Tracker修正。新增Xmlarmyopening Native/Bridge/Oracle/Test脚本/catalog复用helpers，无共享修改。

测试覆盖原ProcessInstance+直接CompileXml六数组、/Od/O2、0/1/2/99，11根弱hash/TechStructure完整Unicode/NUL/大小写与反斜线输入、Side/Build string、Time units/CRT/unsigned溢出、全部enum/未知/C-string、Side字符串0..65535、两列表0..1024、拒绝/metadata、独立76字节Army和72字节Opening混合ABI、默认Build NUL分配及owner/target block顺序。Time1500ms精确位为3fc00001，未误用十进制近似。最终通过必须核对应sourceCommit/runId，不以本地native语法代替。
