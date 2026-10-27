# GameObject 隐身探测与隐身更新

状态：本轮最终完整组件矩阵已验收通过；精确源码/运行/计数见文末。本页不代表完整GameObject、完整替代DLL或SDK集成完成。

原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；58个精确原方法及Upgrade/ObjectFilter/原FastHashFn闭包和指纹见 `evidence/gameobject-modules/stealth-vision-source/manifest.json`。`GenerateStealthVisionFields.py` 再生metadata；逻辑独立于 `GameObjectStealthVisionRuntime.h`，复用已有完整KindOf表，不新增PE表。

| 完整类型 | TypeId | native | kind | Marshal/Token父 |
|---|---:|---:|---:|---|
| StealthDetectorUpdate | 1951832746 | 156 | 145 | Upgrade / Upgrade |
| InvisibilityUpdate | 895696920 | 140 | 146 | Module / Update |

按TypeId全头去重。Detector必须保留完整Upgrade36，包括其属性和CustomAnimAndDuration/TriggeredBy/ConflictsWith依赖；自己的属性先处理，再父层，不能倒置分配顺序。

Detector：DetectionRate Time@36=0.4s、DetectionRange float@40=0.0；PingSound/LoudPingSound强引用@44/@48；IRBeaconParticleSys/IRParticleSys/IRBrightParticleSys/IRGridParticleSys强引用@52/@56/@60/@64；IRParticleSysBone String8@68无默认；ExtraRequiredKindOf/ExtraForbiddenKindOf为36字节位集@76/@112，均无默认，复用284bit/未知bit149；RequiredUpgrade强引用@148无默认；InitiallyDisabled/CanDetectWhileGarrisoned/CanDetectWhileContained为bool@152/@153/@154=false。padding@155不写。endian交换本层Time/float/String length和两位集全部9word，再按原父层处理Upgrade；引用和bool不交换。

Invisibility：InvisibilityTemplate强引用@8；UpdatePeriod Time@12=1s；RequiredNearbyObjectRange float@16=0；NamedVoiceNameToUseAsVoiceMoveToStealthyArea / NamedVoiceNameToUseAsVoiceEnterStateMoveToStealthyArea分别为nullable DWORD@20/@24；RequiresNearbyObjectFilter为inline ObjectFilter112@28。

两个超长NamedVoice字段**不是字符串、引用或lowercase资产ID**。存在即各分配4字节，包括显式空；按原FastHashFn计算大小写敏感hash，复用已验收ExactStringHash，空值hash0。两指针Token mode2只bookmark值，endian交换指向DWORD，不交换指针。固定顺序为普通属性→第一个hash→第二个hash→inline Filter。Filter缺省不Marshal，有节点才应用默认，但Token始终生成inline表(mode8)；Bridge kind146专用路径进入严格ReadDependencyFilter。

高kind属性/节点校验必须提前return，Detector只能接收完整Upgrade子节点，Invisibility只能接收指定Filter；不能让未知节点落入旧数组或泛化成接受其他模块内容。测试 `GameObjectStealthVisionCases.cs` 覆盖大小写敏感hash、空/缺省分配、KindOf/父层、schema顺序、Filter和拒绝；最终仍须完整/Od、/O2、四平台、三缓冲区差分及独立布局。


## 本轮最终整批验收

有效验收提交`284599bfd958c0f62d40ce1b72d27dd70a568bcc`；GameObject run **40094269582 success**，同提交基础run **40094269592 success**。/Od、/O2每模式**381140 fixtures、1143420三缓冲区逐字节比较、13884拒绝、1060不变性**；平台0/1/2/99，两模式合计**2286840**次缓冲区比较。实际两模式日志、validation.json、sourceCommit/runId及运行终态均已逐项核对。

本轮实际29新，164→193/322；129种其他已知Marshal继续明确拒绝，58 Token-only未知Marshal回退保留。本轮SDK0次，原路由保留。
