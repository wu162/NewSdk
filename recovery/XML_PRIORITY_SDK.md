# XmlCompiler SDK：最终验收

实际源码 `71cbdd7bd27478d0bcb9dc3dbd2f22deac888dd7`；[SDK运行 40095358016](https://github.com/wu162/NewSdk/actions/runs/40095358016) 与[组件运行 40095358086](https://github.com/wu162/NewSdk/actions/runs/40095358086)均 completed/success。

52 个原 Plugin 独立入口的 metadata 在平台0/1/2/99的 Initialize/ReInitialize 全部对照，共2928项公开字段/属性/版本/哈希比较及8项未知类型拒绝。版本 Win32/unknown=4、PS3=7、Xbox360=6；ProcessingHash=TypeHash XOR version，Tokenized=false。精确表见 catalog/XmlRecoveredTypes.json。

原 CnC3Types.xsd 允许其中51类全局资产。GameStatsModeData 无全局element，保留独立组件全矩阵与 MultiplayerSettings 嵌套 SDK 覆盖，不伪造全局注册。94个合法源码资产在 sdk/XmlPriorityAllProbe.xml；三个 XSD 负例均被拒绝。

| Profile | 实际调用 | 每种重放的三数组比较 | bin字节 | manifest字节 | imp字节 | relo字节 |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Reduced | 94 | 282 | 12756 | 11437 | 372 | 928 |
| FullMymod | 97 | 291 | 222300 | 16334 | 432 | 5580 |

每profile新建原版/恢复版SDK副本、清缓存并新输出；四份产物逐字节一致、imports非空。FullMymod逐项保留原Mod.xml七个Includes，再追加probe。所有额外实际调用也须完整三数组比较并证明进入最终mod.bin。原版重放后，再用物理排除两个原编译器DLL的独立进程重放恢复插件，逐次确认未加载原编译器程序集。仓库原DLL指纹不变。

同一 /O2 编译产物复用于两profile：联合DLL SHA25615569016e15adaf7f9ed7eac5e0f4733fff19e280610f4c685156e2e600f4355，插件 SHA256168cd3d6c20e2770b953ce4dfcc60016cbe1e0bdd50ee6b6d5307da10552a3158。SDK是Win32验收；/Od、/O2及其余平台另有九组完整组件矩阵。

入口 scripts/TestXmlPrioritySdk.ps1：先编译独立oracle并完整XSD校验，再构建联合DLL/插件，核对metadata，运行两profile和两种重放。schema oracle集中收集所有错误，避免逐字段重复构建。scripts/SetXmlPriorityPlugin.ps1反复切换后核对唯一显式注册及禁buildcache，保留原#all。Original/Recovered使用相同注册拓扑，避免AssetBuilderPluginsVersion求和差异。原SDK目录不被Test脚本改写。

证据根目录 evidence/xml-priority-sdk/validation.json记录sourceCommit/runId；schema/metadata在根目录；Reduced与FullMymod分别保存输入/config、四产物、trace、replay/isolated JSON和日志。succeeded、原DLL不变、两profile与隔离标记均已读回。工作流artifact为xml-priority-sdk-evidence；显式[sdk-xml-priority]或workflow_dispatch触发，普通字段变动仍保留组件CI。

样例覆盖新科技/命令/成长/字体、天气水体和映射、ShadowMap/Orders及nullable引用、玩家AI配置、编队多层列表、相同输入内strong refs及合法inheritFrom。底层不合法XML、非有限或深度拒绝及不具全局XSD表达的入口由组件验证，不能把94样例称作所有字段的实际游戏内容覆盖。完整mymod仍由原SDK处理其他未恢复类型；隔离重放不等于整个SDK无需原DLL构建。

此前3733fbc发现匿名命名空间CLR重名、inline endian误判和测试断言问题；d465844进一步查明ShaderOverride nullable引用及AIData schema必需项。均已修复，失败证据保留，最终只引用本页顶部源码和两个成功运行。
