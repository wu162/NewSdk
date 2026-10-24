# Single-sound family — component-only recovery

本批完整十入口：BaseAudioEventInfo、BaseSingleSound、AudioEvent、MusicTrack、MP3MusicTrack、DialogEvent、MP3DialogEvent、AmbientStream、MP3AmbientStream、Multisound。BaseAudioEventInfo和BaseSingleSound是原schema抽象类型，无全局资产声明，仍有原Plugin独立入口；前者也没有任何Marshal字段。Catalog分别标schemaGlobal=false。此批未接入正式SDK路由，未运行真实SDK编译，不能称SDK已支持。

## 固定原版证据

原Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`；Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`。复用 evidence/xml-source/method-index.txt、parts/part-NNNN.cs.txt和enums/*.json，并核原Plugin独立HandleType、ProcessInstance分派和GetExtendedTypeInformation。精确十个TypeId/TypeHash见catalog/XmlsinglesoundTypes.json。

| 根/依赖 | Marshal 行 | Tracker 构造行 | 布局 |
| --- | ---: | ---: | --- |
| BaseAudioEventInfo | 原Plugin无Marshal调用 | 89062 | 4字节保留首字 |
| BaseSingleSound | 19472 | 89153 | 128字节，完整根属性和全部依赖 |
| AudioEvent | 20050 | 89244 | 152字节，3个weighted列表先于BaseSingleSound |
| MusicTrack | 19951 | 89335 | 132字节，nullable Filename reference**@128先于base |
| MP3MusicTrack | 19918 | 115634 | 同上，原MP3音频引用类型 |
| DialogEvent | 20017 | 89426 | 同MusicTrack |
| MP3DialogEvent | 19984 | 115725 | 同MP3MusicTrack |
| AmbientStream | 19852 | 89517 | 同MusicTrack |
| MP3AmbientStream | 19885 | 115816 | 同MP3MusicTrack |
| Multisound | 19458 | 89608 | 16字节，Control@4和Subsound列表@8 |

原Plugin对Win32为小端，其他平台含99走大端。BaseSingleSound Endian19661；AudioEvent由Plugin显式校正3个weighted列表再校正base；Filename reference仅由Tracker处理引用/指针；Multisound Control大端交换内联于Plugin，不能因没有独立Endian函数漏掉。

## 完整数据合同

- BaseSingleSound全部19个inline根字段定义在XmlsinglesoundFields.inc：float/scaled percentage@4..28，signed Limit@32，Priority@36，8位Type bitset@40、9位Control bitset@44，real MinRange/MaxRange@48/@52，percentage@56..76。根首字保留0。
- Priority未知回退5；AudioVolumeSlider未知回退3；Type bits未知置bit5，Control bits未知置bit7，Multisound bits未知置bit0。每个原enum表、正负号、负号XOR、ALL大小写与padding32位全置，以及ALL停止处理后续token，复用已验证公共MarshalEnumBitset并取得本组完整原差分。
- SubmixSlider present属性先分配4字节enum@80；即使空字符串也分配并按未知回退3。随后依次分配PitchShift@84、PerFilePitchShift@88（各nullable RealRange8），Delay@92、InitialDelay@96（各IntRange8），VolumeSliderMultiplier列表@100（stride8 enum+real），MinRangeShift@108、MaxRangeShift@112（各RealRange8），LimitGroup列表@116（stride4引用），NonInterruptibleTime@124（TimeRange8）。Absent不分配，present-empty仍分配8字节全0。Range Low/High无缺省；IntRange是signed原CRT解析，TimeRange使用原TimeTag，real不误套百分比。
- 原Range Marshal/Endian为Int19031/19052、Time19074/19097、Real19119/19140；nullable helpers60409/60419/60514。Time转换单位查找、数值和float缩放遵守原C字符串在NUL终止。
- AudioEvent Attack/Sound/Decay count/pointer分别@128/@132、@136/@140、@144/@148，每个AudioFileRefWithWeight stride12：strong引用@0、unsigned Weight@4缺省1000、percentage Volume@8缺省100。原19268，list60630/Endian60672。三列表先处理，随后才处理继承base，保持Tracker block/import/relo排序合同。
- 六个track/dialog/ambient根仅多一个nullable strong reference** Filename@128。Filename节点存在就分配4字节，即使文本为空；之后base的所有属性/列表/nullable依赖完整保留。普通AudioFile与AudioFileMP3Passthrough引用helper均采用BAB给定index后缀，不重新按名称寻址或读取原DLL。
- Multisound Subsound列表stride28：strong BaseAudioEventInfo引用@0，unsigned Weight@4缺省1000，nullable float* PitchShiftLow@8、PitchShiftHigh@12，nullable percentage* Volume@16、PlayPercent@20、VolumeShift@24。存在属性依序各分配4字节，空字符串仍分配并解析0。原Marshal19330、Endian19361、list60316/60358；Control Marshal19458，2位位集helper81798。引用/weight先于五个nullable指针，保持精确分配次序。
- 原裸TypeId可由BAB注入任何结构节点，当前各结构明确允许；动态多态不在本组范围。foreign namespace/qualified属性、未知字段/节点、异常文本及duplicate singleton继续显式拒绝。此组件只接收BAB预处理后的defaults/继承/资源索引，不实现整个BAB或Tokenizer。

## 验证入口与当前状态

新增Native/Bridge Xmlsinglesound、2个小型字段/枚举inc、Oracle/Cases/BoundaryCases，以及TestXmlsinglesound.ps1。只编译本组与原共享Relo/Scalar/Resource/EnumBitset，不改变旧组件或SDK配置。

组件矩阵/Od、/O2，平台0/1/2/99，每个原ProcessInstance对照Instance/Relocation/Imports三数组，另CompileXml直接3数组对照。全部根缺省独立逐字节布局；所有inline/nullable/range字段边界、全部枚举/位集、全部列表/nullable缺失与present-empty、六Filename引用路径、长列表跨28/29/30/31/32/65排序边界、128轮完整继承图及Multisound、独立stride/指针/enumfallback/ALLpadding断言和拒绝矩阵。DOM构造embedded NUL，覆盖C字符串边界；另含ANSI/非ASCII后缀、空白、旧CRT浮点/整数行为。

本地g++语法检查、C#词法结构检查通过；Windows完整矩阵待主代理统一运行。证据将写evidence/xml-single-sound/validation.json与两优化日志，最终应核实际sourceCommit/runId及成功终态。当前不计入已验收82，未验证检查点不冒充完成。

