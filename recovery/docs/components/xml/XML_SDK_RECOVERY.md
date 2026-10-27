# XmlCompiler：267全局根真实SDK验收完成

2026-10-25。源码 `9b2ef85bc45e6b397fce3d275165715afa921a92`，运行[40164710673](https://github.com/wu162/NewSdk/actions/runs/40164710673)，completed/success；汇总收据 [acceptance.json](../../../evidence/xml-priority-sdk/acceptance.json)。原Plugin301入口中，排除12根后289入口组件还原完成；当前289个SDK登记项的公开metadata对照也已完成。真实SDK实际根调用覆盖其中267个schema全局类型；22个非全局/抽象/runtime项未冒充独立SDK根。

## 实际执行范围

按用户要求只复现EALAModStudio“建立基础数据”：从 `SDK/defaultscript.cs` 的 `executeModBuildStep` case5提取同一BAB调用，使用一次性SDK副本并清理阶段1/2。原/恢复两次使用完全相同的SDK绝对路径、MOD输入和注册拓扑，关闭缓存。不启动GUI，不执行LoD、修复、资产合并、BIG打包或安装。

独立MOD均位于 `SDK/Mods`，入口是 `Data/Mod.xml`。源XML经原CnC3Types.xsd验收；BAB继续负责defaults、继承、引用、FileReference绝对路径及数字多态TypeId预处理。B的UIConfigList使用全部70个真实组件标签，不手写TypeId；场景包括非空字段、嵌套依赖、列表增长及有效枚举/位集。FullMymod完整保留原7个Includes并追加159个必需探针。

| Profile | 实际SDK调用 | 实际根类型 | 原版+隔离重放数组比较 | 最终运行方式 |
| --- | ---: | ---: | ---: | --- |
| XmlCompilerRecoveryBaseline | 159 | 80 | 954 | 复用完整成功收据与identity guard |
| XmlCompilerRecoveryA | 173 | 59 | 1038 | 复用完整成功收据与identity guard |
| XmlCompilerRecoveryB | 277 | 130 | 1662 | 本次原/恢复真实BAB构建 |
| FullMymod | 162 | 80 | 972 | 本次原/恢复真实BAB构建 |
| XmlCompilerRecoveryApt | 12 | 4 | 72 | 本次原/恢复真实BAB构建与payload专项 |

合计783次实际调用、4698次原版/隔离重放缓冲区比较；类型存在profile间重叠，精确并集267，不能把表内类型数直接相加。每profile的 `mod.bin`、`mod.manifest`、`mod.imp`、`mod.relo` 原/恢复四产物逐字节相同。前四profile具有非空imports；APT纯文件blob不导入资产，其最终imp为4字节，另由每asset空imports和文件payload专项验证，未要求制造无意义的引用。

## APT文件能力及复用证明

APT四根为AptAptData、AptConstData、AptDatData、AptGeometryData。专项12次调用覆盖8个非空文件和4个空文件，非空长度46/13字节、空长度0；236个payload字节与源文件逐字节相同。检查完整根大小、指针/count、padding、唯一blob重定位、无imports及payload实际进入最终mod.bin，收据在 `evidence/xml-priority-sdk/XmlCompilerRecoveryApt/apt-payloads.json`。旧A样例的相对路径被BAB再次拼接目录，payload实际为空，因此只保留其原范围证据，不拿它证明真实文件读取；专项使用声明XML目录下的 `fixture.bin`。

Baseline/A共332次旧调用未重新编译。其验收锚点是887444e8b46ef185fdd120df5403dc8ff584840f/40162875013，同一当前Native DLL，MOD输入和Native/Bridge不变，metadata及ProcessInstance委托路径有源码证明。当前插件逐个检查旧预处理XML，确认不会进入新增metadata归一分支，原InstanceDeclaration、XmlNode及XML内容身份不变，ProcessInstance/Native调用均0。旧receipt缺source/run时仅从该profile成功replay.json取锚点，未改写旧收据。完整原四产物和两种重放保留；最终新运行仅B/Full/APT三个profile、6次真实BAB构建。

## 修复与最终证据

SDK边界发现合法根metadata `buildRule`/`typeHashCode` 未被BAB移除。原Marshal不读取这些字段，SDK插件仅在进入恢复组件前克隆并去除这些根metadata，保留原声明与严格组件边界；原XSD和Native字段未删，B仍保留130个buildRule样例及1个合法typeHashCode样例。

第二次差异是测试工作目录：TestTexture.CustomData会保存BAB规范化后的绝对路径。原/恢复副本名分别含-original/-recovered，两个资产产生80个差异字节，全部属于路径与其长度。改为同一绝对SDK路径先后运行两种模式后，最终原始四产物完全相同；没有在比较时归一化、忽略或遮蔽差异，也未因此修改Native。

当前Native联合DLL SHA256：`170c2218a27339bc8b262c8c6a528090ade1a23a3cfa6c0855294239c68b82ae`。当前SDK Plugin SHA256：`8ffef757d19b154241db740e551fcff58c95b253c0bab0a4835dc58288c7a106`。两原DLL保持原样，原XmlCompiler SHA256仍为 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。metadata在0/1/2/99的Initialize/ReInitialize均对照，共16200项公开字段/属性/版本/哈希比较、56次未知/排除入口拒绝。

`SummarizeXmlSdkAcceptance.py` 从五profile收据、实际输出文件、trace、schema和两种replay验证生成汇总，复用profile另核identity guard，精确核对267类型并集及APT专项。证据与运行锚点逐profile保留，不把旧82入口SDK或静态路由登记当作新DLL运行证明。 XML/TSV哈希按运行时原始字节核对；证据目录的.gitattributes固定CRLF，四份二进制产物标为-text。优先读取Actions artifact或按属性checkout；git archive中的规范化LF不能直接代替原运行字节做哈希。此次仅恢复证据读取规则，无需额外SDK编译。

## 保持的边界

289是组件入口及SDK metadata登记数；267是本次真实SDK全局根调用数；22是非全局登记数；12是原范围排除数。22项的必要嵌套布局仅按实际owner证据说明，未伪造全局XSD声明或独立SDK根调用。TestGameObject及11个Joypad独立根继续排除；UIConfigList内部Joypad组件属于该完整根的依赖。

历史82入口及其旧DLL的92180f9/40098695436基线在 `XML_PRIORITY_SDK.md` 单列保留。SDK真实验收仅/O2、Win32；组件/Od、/O2及四平台矩阵另有精确验收索引。此结果不宣称完整Tokenizer或Audio/Map/Texture等非XML自定义编译器已还原。生产配置修改仅限显式SDK副本，不覆盖仓库原DLL；后续变更仍按受影响输入选择验证，禁止强推。
