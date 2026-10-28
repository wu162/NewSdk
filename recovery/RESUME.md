# TextureCompiler 公开源码适配续接（2026-10-28，本轮进行中）

目标贴图分支 `recovery/texturecompiler-source`；独立实验 `validation/texture-public-codecs`。仅PC、禁止强推、编译测试仅GitHub Actions。用户澄清：**修改拿来的源码，使产物一致**；不只是原样比较后放弃。测试分支修改和原版gold全量比较持续推进，完整相同后还需消费/SDK门槛才合入；生产SDK DLL/config未替换。

- IJG公开源码、JPEG组件及真实SDK验收已在 `ce37532f192f6b2275adcd044cddce5232aa3c1e` 合入贴图分支。40626161229/40628208308均success；9 SDK profiles每模式13104资产/实际调用。后面旧状态不能覆盖这项完成。
- DXT已实际修改复制的libsquish/ICBC/AMD源码。AMD第二轮1ce890cf /40631302274的D/R DXT5 alpha已5120/5120；RGB仍只有374/5120完整块，不能启用为完整DXT后端。
- 更接近的公开S3源码已复制并修复强制四色契约：81268e60 /40631683152 D/R完整DXT3、DXT5各4838/5120、alpha5120/5120。剩余252灰度单色、8其他单RGB（其中3灰度）、22随机。02557975 /40631926236的x87比较没有改善，别重复跑相同配置。
- 新兼容补丁把既有还原源码的固定灰度算法表接入S3（不是gold样本输出表，不回调恢复/原编码函数），并比较原16/8与0/0、1/1、2/2迭代停止规则。最新矩阵待收取；先读远程分支头、run sourceCommit和收据，保留Actions回写，不重跑未变成功组。
- 所有不完整候选和完整实验产物仍在测试分支，贴图分支只集中已验收成果、当前规则与交接。详见[适配结果](TEXTURE_PUBLIC_REUSE_RESULTS.md)，实验分支文档TEXTURE_PUBLIC_CODECS.md、源码PROVENANCE.json/PATCHES.md。灰度首轮40632042322因PowerShell参数拆字符导致link失败，非业务匹配结论；a61e2b58已修复重提。

本页以下保留已完成JPEG与历史锚点；当前DXT适配优先以上述最新状态为准。

# TextureCompiler 当前续接（2026-10-28 11:54 北京时间）

目标 `recovery/texturecompiler-source`，仅PC，禁止强推，编译/测试只在GitHub Actions，每轮尽量约70分钟。用户新增要求：直接复用网上可取得的现成源码，在独立测试分支比较产物，完整一致才合入；不按许可类型筛选公开候选，保留上游文件/通知/来源。本轮已执行该流程，详见[公开源码比较与验收](TEXTURE_PUBLIC_REUSE_RESULTS.md)。生产SDK DLL/config未替换。

## 已完成：现成IJG6b源码 + 原版精确适配

- 组件源码 `699479a95c73012f5e66b74a3d7d1e43f1d8b51f` / [40626161229](https://github.com/wu162/NewSdk/actions/runs/40626161229) completed/success：13受影响组件组通过。jpeg D/R各1284记录（1280完整RGBA/元数据相同、4原错误），23105检查、888960字节、逐次NoOriginal；六个灰度NPOT差异已由原L8存储规则完全修复。jpeg-images各11248入口（16原错误、6既有DXT1/1A拒绝）、82,231,440字节比较与同量NoOriginal，含raw/DXT3/5、二维/volume/mips、旧cdata和32 JPEG编码atlas变体。
- 干净集成测试分支 `validation/texture-public-jpeg` 从贴图分支05208638创建，没有不匹配的DXT/STB实验源码。SDK验收源码 `e8835cfee56cb35376a6bc1cb18a40d189263842` / [40628208308](https://github.com/wu162/NewSdk/actions/runs/40628208308) completed/success：9个SDK profile全部通过、32组件组复用。PcJpeg Original/Recovered/NoOriginal各3216资产/3216实际Core调用，四产物和完整实际轨迹相同；NoOriginal逐次确认原DLL不存在且未加载。全9profile每模式合计13104资产/调用。PcJpeg mod.bin=12,181,924字节。SDK已完成，不再按旧“准备稿/运行中”重复触发。
- 自有C桥接`src/TextureCompiler/Jpeg/TextureJpeg.c`直接静态链接固定IJG6b公开源码，IFAST/fancy，灰度L8/RGB RGBA8 NPOT；公共ImageLoader和.jpg/.jpeg路由已验收。原型原gold `3f2351f2/40625499544` 1284/2568调用保留probe-only。源库`recovery/reference/ijg6b`里最初说明的“probe-only”是采集阶段历史范围，当前消费/验收以上述成功锚点为准；不改写已固定源文件/指纹。CMYK/未声明metadata等域不扩大支持，专用MOD不冒充完整mymod。

## 已测试但禁止作为等价后端合入：现成DXT/STB

- 分支 `validation/texture-public-codecs`，实际比较源码94ebf7886592843a06f6e1bc5cacaf372ae0b4ff / [40627584385](https://github.com/wu162/NewSdk/actions/runs/40627584385) success表示比较完成，收据status=comparison-completed、productionAccepted=false。复制30个未改上游源码/通知，Git blob/SHA256逐项核对；libsquish12配置、stb_dxt2配置、NVTT ICBC18配置，共32配置。
- D/R各对全部10240原DXT3/5块比较（327680候选块），完整源RGBA与原/候选块归档。最佳完整匹配/5120：libsquish DXT3=481、DXT5=1；stb=288/232；ICBC=1217/2。**没有一个完整格式达到5120/5120**，实际随机样本RGB565端点也不同；不能只改DDS头或常量索引宣称等价。Squish DXT3 alpha单独5120相同不代表颜色/完整DXT相同。
- STB JPEG完整匹配272/1280，原拒绝匹配1/4，不等价；不启用。16个DXT1/1A旧快照仅probe-only，原默认未初始化状态门槛保留。失败/不匹配实验代码与产物留在上述测试分支；贴图分支只接收已完整验收的IJG6b结果与说明。

## 继续工作的约束与入口

先fetch远程头并保留Actions证据；上述验证源码均应为合入后祖先，输入未变时复用原sourceCommit/runId，文档/合并提交不冒充新跑SDK。已验收全部PC atlas仍保留be520b8e/40607538234的48例/16资产原锚点，JPEG共享loader后的atlas组件与SDK已分别在40626161229/40628208308补验。

余下：原默认DXT1/1A未初始化状态、未声明图像/metadata、必要PC native边界和完整mymod/生产部署。不要再把JPEG、已声明NPOT或完整atlas列为未恢复。新公开实现可继续在测试分支跑完整原版差分；没有字节等价证据不得替换当前压缩器。历史失败保持真实状态，不重跑本轮未变成功组。

以下为前序历史，旧“JPEG待验收/atlas未接入”等阶段描述由本页最新成功锚点覆盖。

# TextureCompiler 当前续接（2026-10-28，北京时间）

`recovery/texturecompiler-source`，仅Win32/PC，禁止强推，编译测试只在GitHub Actions；每轮尽量约70分钟，按STAGE_DURATION.md。Git身份wu162 <2997261975@qq.com>。主入口[TEXTURECOMPILER.md](TEXTURECOMPILER.md)。本轮从4857ff70续接，先核对远程头与40536063863及其复用收据；所有源码、取证、验收继续集中本分支。

**本轮已恢复余下已声明的NPOT源存储路径，并通过公共解码、完整插件矩阵与专用SDK；atlas仅完成独立固定五层DDS组件，公共atlas入口仍未接入。** 生产SDK DLL/config未替换，专用MOD不冒充完整mymod/完整TextureCompiler。

- NPOT业务源码`e9292a26149bf8add555a883cb95739a65fc69a9` / [40595382578](https://github.com/wu162/NewSdk/actions/runs/40595382578)有18组通过，包括全部7个SDK profile；整run因dxt-images隔离宿主仍检查旧14256条而failure，保留。Debug原版差分19404已全部相同后才触发旧条数断言，业务源码无需修复。
- 宿主修复`9e7d671e46b3abb41e3e8ab22abd1c759e77b9c9` / [40597016969](https://github.com/wu162/NewSdk/actions/runs/40597016969)：dxt-images Debug/Release各19404（17884输出/1520匹配原拒绝）、6个默认DXT1/1A明确拒绝、88,895,384字节比较及同量NoOriginal，逐次原DLL不存在且未加载。该run整体failure仅因新original-only stress快照探针误把Pitch*Height当可读分配范围，不能写整run success。
- stress快照改为有界首行大小原始前缀后，`abdffd9724ddab6a9c71e0e92d132aea7241dc8b` / [40598190231](https://github.com/wu162/NewSdk/actions/runs/40598190231) completed/success：只补该probe，复用34个完整成功组。原4986基线不可变，padding从不作为恢复比较契约。
- 最终较大比例回放与独立atlas DDS源码`daf4a749752786d5aea56b8de62fc47f1e77383d` / [40598863269](https://github.com/wu162/NewSdk/actions/runs/40598863269) completed/success。旧4986（4446解码/540拒绝）各配置188999检查；新4432（3952解码/480拒绝）各配置4893795检查，两份完整原RGBA、旧语料原错误域及新语料原异常type/message/ErrorCode、源不变及逐次无原DLL隔离；不把probe-only当恢复验收。

NPOT现支持PNG灰度1/2/4/8及16（16+tRNS仍原拒绝）、GA8、RGB/RGBA8/16；TGA既有24/32及Additional真色16、灰度8、index8配palette15/16/24/32（已声明RLE/方向）；BI_RGB BMP16/24/32。保留原alpha规则、PNG RGB16旧行布局和RGBA16通道排列。源各轴1..8192，向上取2幂后<16M像素；仅图像加载层重采样，直接DDS编码API仍要求POT。PNG索引/GA16/gray16+tRNS、BMP索引/bitfields、TGA gray16/index16保持原错误，不作为缺失支持。

新PcNpotStorage专用SDK为64输入×两根×raw3/DXT3/5×标准/volume×有无mip=2560资产；Original/Recovered/NoOriginal各2560真实Core调用，四产物哈希与完整buffer/cdata轨迹相同，NoOriginal逐次检查原DLL不存在且未加载。原PcNoMip/PcMips各600、PcDxtNoMip/PcDxtMips各712、PcDxtVolumes1488、PcNpotRgba8 3200三模式同批通过。各收据保留原sourceCommit/runId，不因总体宿主失败重跑成功SDK。

atlas DDS独立组件Debug/Release各160完整DXT5/RGB555原DDS、12个拒绝、57984检查/54304字节对照通过，逐次无原DLL。原+104=5是指定五层，不是过滤核；短自然链重复1×1 payload，header mipCount仍min(自然层数,5)。原格式9为RGB555；原probe误名filter的不可变记录由SCOPE.md解释，不重写。此组件没有接入公共TerrainTextureAtlas，也未作为该入口SDK验收。详见[atlas后续](TEXTURE_ATLAS_NEXT.md)。

最新完整核对源码`056937b13fa39431ec9171b0d990e6a5c15a013c` / [40599993769](https://github.com/wu162/NewSdk/actions/runs/40599993769) completed/success：只采atlas原入口25例（19成功/6错误）、50调用、468检查，复用36个完整成功组/7个SDK profile；该原入口仍probe-only。新增独立DDS程序集后，两个raw SDK profile已在`dfbeaee30c482816a927d4f42229a0ceafd598e8` /40599421956三模式各600调用补验通过，其整run因gold宿主对空声明列表误读而failure，后续仅修复probe；不重复SDK。原gold证实空声明列表返回三字段null，而声明非空但全base缺失仍产生DDS与count=0/末指针relocation；布局与可移植原输入见[atlas后续](TEXTURE_ATLAS_NEXT.md)。

**下一整组**：在完整原atlas入口gold上实现Tile/File Marshal、已恢复TexturePacker与独立DDS组件消费、Runtime tracker/relocation，再跑完整原版差分/NoOriginal和专用MOD三模式SDK。默认DXT1/1A的未初始化状态、JPEG/未声明metadata、完整生产部署/mymod保持独立门槛；不恢复PS3/Xbox路径。既有二维DXT mip `7ae4af37` /40524039917与volume `aaacb3d2` /40526443463成功锚点继续复用，算法分别base-relative与递归Z后XY，不能混用。详细原始/失败记录与范围见[NPOT当前交接](TEXTURE_NPOT_NEXT.md)。

以下保留历史子系统与阶段记录，不以旧的“下一轮”覆盖上述当前入口。

# GameObject、XmlCompiler 与 W3X 当前续接入口

2026-10-25（北京时间）。目标分支 `recovery/compiler-source`。本文件保留PR [#2](https://github.com/wu162/NewSdk/pull/2) 的GameObject/XmlCompiler状态，并加入W3X还原成果；禁止强推。先读AGENTS.md与EFFICIENCY.md，再按子系统读取详细交接。

- **GameObject**：322/322已知Marshal模块已完成，58个Token-only保持原未知Marshal回退。验收源码 `f8726d624b0a74ea0927705bca352027cbf76e30`；组件运行40162947222、基础40162947146、真实SDK40162947209均成功。详细当前状态及不可变证据见[RESUME_GAMEOBJECT.md](RESUME_GAMEOBJECT.md)和GAMEOBJECT_DRAW_MODEL.md。
- **XmlCompiler**：原301登记项排除TestGameObject和11个Joypad独立根后，289入口组件还原及元数据核对完成；其中267个XSD全局类型取得真实SDK根调用证明，22个非全局项不冒充独立SDK根。SDK源码 `9b2ef85bc45e6b397fce3d275165715afa921a92`、运行40164710673成功；5个MOD profile四产物相同，783次实际调用、4698次原版/隔离重放缓冲区比较。详细状态见[RESUME_XMLCOMPILER.md](RESUME_XMLCOMPILER.md)、[XMLCOMPILER_PRIORITY.md](XMLCOMPILER_PRIORITY.md)及[SDK验收收据](evidence/xml-priority-sdk/acceptance.json)。

- **W3XCompiler**：W3DAnimation/W3DMesh定义行为恢复及专用MOD真实SDK三组对照已通过。验收源码 `ceeef68df4c032309154214f6746322e0a65e2ec`，运行40169489711 attempt1成功；原版、恢复版、移除原W3X DLL的恢复版四产物逐字节相同，8动画/4网格各36次实际三缓冲区比较。详细范围、未定义行为拒绝边界及不可变证据见[w3x/RESUME.md](w3x/RESUME.md)与[w3x/SDK_ACCEPTANCE.md](w3x/SDK_ACCEPTANCE.md)。

三组子系统SDK验收分别执行EALAModStudio defaultscript.cs case5基础数据BAB流程，只在临时SDK副本启用相应恢复插件；各自验收锚点与范围分别保留，不将它们改写成一次多插件同时启用的联合SDK运行。本次合并不修改已验收Native/Bridge/共享helper及原SDK DLL/config，不重复触发未变成功类型的编译测试。

当前已授权还原范围内没有新增待实现或待收取验收。后续变更按实际影响选择测试；缺失、失败、输入变化的组仍须取得完整必要验收。原XmlCompiler排除12根、22非全局项的边界不因合并扩大，其余自定义编译器及完整Tokenizer不在本次完成声明内。

证据冲突处理：XmlCompiler当前共享证据保留priority分支已验收版本；compiler-source同名旧证据原样归档在 `evidence/xml-merge-baselines/compiler-source-833c9177`，GameObject不可变运行档案不变。XML/TSV按证据目录.gitattributes检出以核对运行SHA256；源码与运行锚点不得交叉套用。

W3X合并核对：唯一重叠编辑为本导航文档；W3X源码/测试/工作流/MOD/证据保持其已验收内容，共享Relocation、ScalarParsing、ResourceMarshaling、XmlNodeAdapter及SDK/tools、SDK/Schemas与两分支相同，见[w3x/MERGE_VALIDATION.json](w3x/MERGE_VALIDATION.json)。不把历史验收写为合并提交新运行；没有重复触发未变化的已验收类型。W3X的SDK_READY仍精确绑定原验收的整树，合并后包含其他子系统新内容，后续若显式再启动SDK须先重新核对并认证新整树，不能直接篡改指纹冒充已验证。


## 托管辅助程序集（2026-10-25）

**ExpressionEval 与 GameDataVerifier 完整源码已还原到 `src/Managed/ExpressionEval`、`src/Managed/GameDataVerifier`。** ILSpy 9.1.0.7988 全工程导出，保留原命名空间/程序集身份及 net20 目标，无占位实现。验收源码 `e806b1ca26509915621d214be2492d572db78f74`，运行 [40171170187](https://github.com/wu162/NewSdk/actions/runs/40171170187) 成功：Debug/Release 各 32,306 条原版对照记录一致，恢复目录中不存在两个目标原 DLL；Core 表达式动态加载通过。详见 [MANAGED_SUPPORT.md](MANAGED_SUPPORT.md) 与 [验收收据](evidence/managed-support/validation/40171170187-1/validation.json)。仅完成源码及组件验收，未替换生产 SDK DLL/config、未执行完整 MOD 的真实 SDK 集成编译。


## Core / Remote / EALABuild 独立还原分支（2026-10-25）

分支 `recovery/core-remote-ealabuild` 从 `c61aa003be605a7169fba6305311f2c96d88896c` 拉出，三个完整 ILSpy 工程已放入 `src/Managed/Core`、`src/Managed/Remote`、`src/Managed/EALABuild`，共 98 个 C# 文件。验收源码 `939080072f73d2eff92c563fcd04ab9dbe056aa5`，运行 40171944143 attempt1 成功：三个工程 Debug/Release 构建通过，原版/恢复版类型成员、资源及组件样本转录相同。两个已审计的现代编译器初始化元数据差异详见 [BUILD_INFRASTRUCTURE.md](BUILD_INFRASTRUCTURE.md)。未替换 SDK DLL/config，未执行完整 MOD 的真实 SDK 集成编译，未合入 compiler-source；既有其他编译器验收锚点保持不变。


## Upheaval ID 修复及公开源码经验已合入（2026-10-25）

从 validation/upheaval-recovered-apt-global 的固定提交 068643219fded09a05fb7b75c6507b769c430316 选择性合入：4个实现文件、6个回归测试文件、821份XSD的ID审计工具与清单、定向/SDK脚本、根因及公开用例经验文档、精简成功证据。修复嵌套AI状态与BuildDelayRange、GameDependency、ArmorSet/LocomotorSet、ObjectVisionRange及AISpecialPowerInfo身份属性边界，保留真正序列化ID和外来命名空间拒绝。源码和测试子树逐SHA与已验证来源一致，SDK工具/Schema/基础资源不变。组件40175816790、40177066058、40180066600以及SDK40178403660、40182719547沿用原sourceCommit/runId，未冒充合入提交新跑SDK。17资产SDK批次10次调用成功、20组四产物一致。源码输入与大型轨迹继续留在验证分支，本分支不引入整套Upheaval素材；运行SDK脚本前须按文档取回对应输入。详见[合入核对](UPHEAVAL_FIXES_MERGE.md)、[根因与门槛](ID_METADATA_ROOT_CAUSE.md)、[公开用例经验](PUBLIC_MOD_CASES.md)。当前请求范围已验收，无新增编译器阻塞。


## EALA.Metrics 源码还原（2026-10-25）

分支 `recovery/eala-metrics-source` 从 `7fc417546989ca94a18742b37965e9c6751a409e` 新建。完整工程位于 `recovery/src/Managed/Metrics`，13 个 C# 文件，保留原程序集身份和 net20 目标。验收源码 `2fc44710df27ea9baf4e73d183dcd7697a9a435d`，运行 [40186536173](https://github.com/wu162/NewSdk/actions/runs/40186536173) attempt1 成功，Debug/Release 各 1,197 条原版对照记录一致，恢复测试目录不含原目标 DLL。源码包括 SQL writer；仅验证离线行为，SQL 服务端与真实 SDK MOD 编译尚未验证。未替换 SDK DLL/config，合入追踪：[PR #7](https://github.com/wu162/NewSdk/pull/7)，目标 `recovery/compiler-source`。合并时目标分支新增内容仅为其他组件的验收证据，Metrics 源码、测试和原 DLL 输入未变，复用上述精确运行结果。详见 [METRICS.md](METRICS.md) 和 [验收收据](evidence/metrics/validation/40186536173-1/validation.json)。

## Utility 源码还原（2026-10-25）

分支 `recovery/utility-source` 从本分支 `7fc417546989ca94a18742b37965e9c6751a409e` 新建。原 C++/CLI Utility 的完整业务源码已重建到 `src/Managed/Utility`；原始 ILSpy 工程与 IL 保留在 `evidence/utility`。构建后恢复原模板类型身份及 C++ 签名修饰符（GetErrorString 的 IsLong），不调用原 DLL 业务实现。验收源码 `9c7b59036c0bc518727023331603e59bf16c48a4`，运行 [40186857933](https://github.com/wu162/NewSdk/actions/runs/40186857933) 成功：Debug/Release 各269,516条原版对照完全相同，恢复运行目录无原Utility DLL，七份真实manifest全资产读取通过。组件已验收，未替换SDK DLL/config、未跑完整MOD编译；通过 [PR #8](https://github.com/wu162/NewSdk/pull/8) 合入compiler-source，保留同期Metrics与其他组件验收证据；原版越界/未初始化内存/重复释放不属确定行为承诺，卷刷新成功路径未实际操作。详见 [UTILITY.md](UTILITY.md) 和 [验收收据](evidence/utility/validation/40186857933-1/validation.json)。



## Mvp.Xml 独立还原分支（2026-10-25）

分支 `recovery/mvp-xml-source` 从 `260f1d2c34d52910ff27d53ef070059c4b1c9785` 新建。完整工程位于 `recovery/src/Managed/MvpXml`，91 个 C# 文件，覆盖原库 XInclude/XPointer/XPath/EXSLT/XSLT/序列化能力。按用户要求只恢复功能，输出无签名 DLL；保留简单程序集名/版本，未替换 SDK 原 DLL。构建后恢复 51 个 EXSLT 原始 CLR 方法名，避免 ILSpy 的 C# 名称转义改变 XSLT 反射入口。验收源码 `fe38f6039f77a45a77ed53da93a53ac97e110da3`，运行 [40188365985](https://github.com/wu162/NewSdk/actions/runs/40188365985) attempt1 成功：Debug/Release 各 2,110 条原版对照记录一致，包含 Core 原包装方法实际调用，恢复宿主排除原目标 DLL；详细构建、签名边界与组件覆盖见 [MVP_XML.md](MVP_XML.md)。合入追踪：[PR #9](https://github.com/wu162/NewSdk/pull/9)，目标 `recovery/compiler-source`；未执行真实 MOD SDK 编译。


### Mvp.Xml 调用处改为静态引用（2026-10-25）

用户要求 SDK 直接调用恢复函数。Core 的 `AssetDeclarationDocument.LoadXml()` 现在直接构造 `Mvp.Xml.XInclude.XIncludingReader` 并设置 XmlResolver，`DocumentProcessor.ProcessDocument()` 移除动态加载初始化，Core 项目引用 MvpXml 源码工程；旧包装类仅保留公开方法兼容，不再反射。MvpXml 使用原始二进制资源直接嵌入，保留可读 resx，从而让原有 dotnet 构建方式可用。验证同时排除原 Core/MvpXml、实际执行 LoadXml 和依赖记录，并检查产物 IL 的直接构造/setter 调用；另运行 Core/Remote/EALABuild 既有组件矩阵。验收源码 `a75b48a960d32e199acb2716e06e57e6deca82ce`，运行 [40189585854](https://github.com/wu162/NewSdk/actions/runs/40189585854) attempt1 成功：Debug/Release 各 2,111 条 MvpXml/真实 LoadXml 对照及各 2,434 条基础组件记录一致，直接调用 IL 审计通过。修改位于 `recovery/mvp-xml-source`，SDK/tools 原二进制仍作基线；合入追踪：[PR #9](https://github.com/wu162/NewSdk/pull/9)，目标 `recovery/compiler-source`；未执行完整 MOD 编译。

合并核对：目标分支 `4e86bf87e9339a5b5662f5aa6a27debe8970fe79` 在共同基线后新增 Utility 源码/脚本/文档/证据及 primitives 验证收据；与 Mvp.Xml 分支唯一重叠是本续接文档。Utility 全部子树和 primitives 收据按目标 SHA 原样保留，Core/MvpXml/测试/构建工具/SDK 原 DLL 与验收源码保持不变，复用运行 40189585854 的精确结果，不将文档合并冒充新验证。

## TextureCompiler：后续仅 Win32/PC（2026-10-26）

用户要求移除非 PC 代码和规划。当前范围、剩余任务及本轮验证状态统一见 [TEXTURECOMPILER.md](TEXTURECOMPILER.md)，测试策略见 [TEXTURE_TESTING.md](TEXTURE_TESTING.md)。PS3/Xbox360 不再是实现或验收目标；历史锚点保留，不冒充清理后验收。源码在根 `src/TextureCompiler`，生产 SDK DLL/config 未替换，禁止强推。

PC 清理验证已完成：源码 0ae0efdb / 40241188546 的其余 10 组件组及两个 PC SDK profile 通过，每 SDK 模式 600 资产/调用；该 workflow 因 base 缺少 MSVC failure。补齐工具链后 7786834f / 40241581283 的 base Debug/Release/native 通过，整轮 success，只复验 base、未重跑 SDK。完整锚点与收据见当前 Texture 交接。

Texture 2026-10-27 续接：`recovery/texturecompiler-source` 已新增 DXT3 显式 alpha 子组件（未接插件），`9f8b1361` / 40470055551 Debug/Release 各4352例、65288检查成功；`70aded86` / 40470267556 的参考库代码映射与32个PC块取证成功（probe-only，19/22目标代码节匹配）。下一步按 TEXTURE_DXT_ENTRYPOINTS.md 的候选 RVA 核对实际调用链并恢复 DXT5 alpha/颜色搜索，不重复旧探针、不把子组件或静态匹配写为完整DXT验收。生产 SDK 未替换。

Texture 2026-10-27 第二阶段当前入口：[TEXTURE_DXT_ALPHA.md](TEXTURE_DXT_ALPHA.md)。分支仍为 `recovery/texturecompiler-source`，仅PC。Production DXT5 alpha 字节/float与独立x87源码后端最终源码 `4bf0ca8c` / 40493971846 success；Debug/Release各2176量化器、78144实际DDS alpha、12288 float、78144 NoOriginal重放通过，native无入口无导入；原SDK与插件DXT拒绝边界不变。颜色块、完整压缩器与SDK集成仍待做。不要再用未接受的私有managed参考替代x87后端，也不要重跑已通过alpha；下一步按颜色取证当前状态推进RoundMult/AllSame/CodeRGBBlock完整能力组。

## 2026-10-27 19:19（北京时间）认证中断时的本地交接

本轮最后成功普通推送为 `6417e32ad3bae6aa28b386a72ed35df1da0cd505`。随后 GitHub API 返回401 Bad credentials，原生Git只读访问同样无法取得认证；环境仍在线，尚无可用的凭据刷新工具。未新增secret要求，未强推，未改写失败记录。此交接提交尚未推送；恢复连接后先读取远程分支头，保留Actions可能已写回的证据，再rebase并普通推送本地文档。

已验证成果：Production DXT5 alpha最终源码 `4bf0ca8ccb74a6f670cd7ecb954483dc795fae9f` / Actions `40493971846` completed/success，Debug/Release量化器、78144实际DDS alpha、12288 float、78144 NoOriginal及native ABI全部通过。源码/测试输入后续未变，SDK未修改，完整颜色压缩器未完成。详细范围见 TEXTURE_DXT_ALPHA.md，不重跑未变成功组。

唯一待收取：源码 `c4bb4db72440bf23827656a643022ea67c7bbb1d` / Actions `40494632567`，显式 `dxt-colour-functions`，只补取 CodeRGBBlockExhaustive@0x833F0、CodeRGBBlockNoMode@0x83CD0、FastEncodeDXT1_3@0x8BAA0、FastEncodeDXT1_4@0x8C1F0。最后可读状态为运行中；当前最终状态与远端是否已自动回写未知，不标作成功、不重复启动。预期输出在 `evidence/texture/dxt-decompile/40494632567-1`。此前All取证40490888440在RoundMult超时、只收取13函数，仍为failed/partial；本次补取即使成功也不意味着RoundMult已完成。

上节认证中断已解决：已保留远端 Actions 证据提交351880d4，将本地交接rebase后普通推送。40494632567现核实completed/success，四份颜色函数导出已回写并拉取，收据status=probe-only；不属于颜色实现验收。剩余RoundMult导出/颜色恢复继续按TEXTURE_DXT_ALPHA.md推进，勿重复补取这四函数。

