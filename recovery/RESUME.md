# TextureCompiler 当前续接（2026-10-28，北京时间）

`recovery/texturecompiler-source`，仅PC，禁止强推，编译测试只在GitHub Actions；每轮尽量约70分钟，按STAGE_DURATION.md。主入口 [TEXTURECOMPILER.md](TEXTURECOMPILER.md)。本轮从远程`d0ab7450038056d35678d34c23fe7f6334426a75`续接，已核对既有40508857046/40510009285成功证据并保留。

**本轮三组已完成：DXT3/5二维完整float mip、压缩volume无mip/递归float mip、限定RGBA8存储的NPOT加载及标准/volume raw+DXT入口。** 生产SDK DLL/config未替换，专用MOD不冒充完整mymod/全TextureCompiler。

- 二维mip：源码`7ae4af3769842084cd19cbff87c651c945f45888` / [40524039917](https://github.com/wu162/NewSdk/actions/runs/40524039917) success，Debug/Release各3072完整DDS及NoOriginal。二维每层从base生成，禁止float中途回写byte。
- volume：源码`aaacb3d25fdb99eddaa8ed4ab23701437e840ec2` / [40526443463](https://github.com/wu162/NewSdk/actions/runs/40526443463) success，各配置5568完整DDS/NoOriginal；递归先Z后XY，XY-only头层数与完整XYZ尾链保留。详见 [二维mip](TEXTURE_DXT_MIPS_NEXT.md) / [volume](TEXTURE_DXT_VOLUME_NEXT.md)。
- 8位NPOT：业务源码`a945c18c8ea02b6ad03f8a4b6172be10ba7dffa2` / [40533831752](https://github.com/wu162/NewSdk/actions/runs/40533831752) 的17组通过，包含各配置14256插件输入（12736输出/1520匹配原版拒绝）、34明确拒绝、66,334,496字节比较与完整NoOriginal；全部6个SDK profile三模式通过。此run整体failure仅因回放宿主漏TGA全零alpha规则，不能写为整轮success。

最终宿主修复源码`d012da8ae55c094220d9c506b5263b277a49042b` / [40536063863](https://github.com/wu162/NewSdk/actions/runs/40536063863) attempt1 completed/success：Debug/Release各1012原RGBA与公共加载器完整比较、1851原系数tap位模式、1,361,709检查，逐次原DLL不存在且未加载。仅补npot-rgba8，选择器复用其余31组完整成功证据，保留各原sourceCommit/runId。SDK业务源码未在宿主修复中改变。

NPOT支持true-color TGA24/32、PNG RGB/RGBA8、BI_RGB BMP24/32，源各轴<=8192且向上取2幂后<16M像素；只在加载层重采样，直接DDS API仍要求POT。PcNpotRgba8专用SDK同80输入/3200资产，Original/Recovered/NoOriginal各3200实际Core调用，四产物与完整buffer/cdata轨迹一致；NoOriginal每次调用前后检查原DLL不存在且未加载。同轮两个raw profile各600、二维DXT两个profile各712、volume profile1488资产/调用的三模式也通过。范围及SDK通用scope文字补充见 [NPOT当前交接](TEXTURE_NPOT_NEXT.md)。

**下一组**：NPOT剩余存储/量化路径（PNG灰度/GA及16位、Additional TGA灰度/调色板/16位、BMP16），先核对原D3DX源/目标像素格式、row converter与最终XGCopySurface，不能把16/5位量化简化为先解码到RGBA8再缩放。原版拒绝的PNG索引/部分GA16、BMP索引/bitfields等保持原拒绝，不把它们列成必须支持。默认DXT1/1A整块、完整TerrainTextureAtlas、生产替换/完整mymod仍待恢复或验收；未知域明确拒绝。详见 [TEXTURE_NPOT_NEXT.md](TEXTURE_NPOT_NEXT.md)。

原版取证a19bb879/40528237009（1012加载记录）和678db012/40529918922（权重/常量/抖动）仍probe-only，作为不可变基线复用，不再重跑。826ff73e/40531802530与a945c18c/40533831752的宿主失败记录保留；新功能完成由上述精确成功/复用收据组成，不篡改旧运行。

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

