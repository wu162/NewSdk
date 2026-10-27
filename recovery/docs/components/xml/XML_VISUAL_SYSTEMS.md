# Breeze / Model / AutoParticle 系统设置

2026-10-25（北京时间）。完整恢复三个原Plugin入口，组件待验收、SDK未集成/未验收。本轮没有真实SDK编译。

| 根 | TypeId | TypeHash | 根大小 |
| --- | ---: | ---: | ---: |
| BreezeInfo | 1626415288 | 890675481 | 28 |
| ModelSettings | 1968811301 | 2503358751 | 24 |
| AutoParticleSystemSettings | 3278684478 | 1247287143 | 124 |

证据为固定Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`；原Module方法行52060–52153(Breeze)、52153–52372(AutoParticle)、52765–52840(Model)，对应evidence/xml-source/method-index及parts；原Plugin独立HandleType、Tracker构造根大小、GetExtendedTypeInformation三项登记均核实。三者均在CnC3Types.xsd全局声明，System/BreezeInfo.xsd、ModelSettings.xsd、AutoParticleSettings.xsd作字段交叉检查。

Breeze根的LoadAfter强引用@4；Direction@8默认1.0472、Intensity@12默认0.027488125、Lean@16默认0.0157075、Randomness@20默认0.2；BreezePeriod Time@24默认10s。原方法按这五个标量后LoadAfter顺序执行。

Model的TimeOfDay/Weather两个bool在20/21，字节不swap；AnimationSharingCap signed@8、FrameTolerance float@12、DrasticThreshold float@16；LoadAfter强引用@4最后处理。无额外默认，padding保持zero。

AutoParticle根ParticleScale float@4无默认、MaxParticleCount signed@8默认2500；FireSmall/FireMedium/FireLarge/SmokeSmall/SmokeMedium/SmokeLarge/Aflame分别内联16字节，自12开始。每项ParticlePrefix为8字节string（长度@0/指针@4）、ParticleSystemId为case-folded弱hash@8、ParticleMax signed@12。节点不存在保持zero；前缀存在且空仍分配终止NUL。按七个固定字段顺序分配，不按XML child输入顺序；全部节点缺省/空并不制造新列表。字符串计数、weak hash及signed值endian纠正，字符串指针与强引用由Tracker纠正。

仅新文件Xmlvisualsystems Native/Bridge/Oracle/Test脚本/catalog，不修改共享helper。显式拒绝未知属性/子节点、重复单节点、结构文本、限定属性、非asset命名空间、root/TypeId不匹配；TypeId元数据可透过嵌套节点。

完整Oracle测试覆盖全部字段、默认及partial配置、原ProcessInstance/直接CompileXml六数组、/Od和/O2、0/1/2/99、浮点/整数/bool/Time/引用/Unicode/NUL/弱hash边界，七个inline每个字符串0..4096增长及逆序XML全字段混合。独立ABI包括三个根布局、两个bool字节、全部Breeze默认bit、AutoParticle124根+两字符串132总长、empty-vs-missing prefix、relocation与imports。Time1500ms独立bit为0x3fc00001，遵守原binary32单位比例乘法舍入，不近似为1.5。

本地只作native语法检查，去除x86静态断言的临时副本不构成ABI或差分证明；最终以Windows完整矩阵及其sourceCommit/runId为准。
