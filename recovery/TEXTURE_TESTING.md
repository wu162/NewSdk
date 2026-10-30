# TextureCompiler 验证触发规则

2026-10-26（北京时间），用户反馈“每次触发的测试个数太多了”。本规则优先于旧文档关于每次相关提交自动启动多条 Texture 工作流的安排。后续仅 Win32/PC；保留 PC 的全部输入、断言、Debug/Release、原版差分及 SDK 三模式，移除非 PC 平台轴和专属组。

## 当前触发节奏：稳定消费链一次验收（2026-10-30，优先于下文旧默认）

- 同一核心的已知能力与真实调用参数一次核对并连续实现；不按unsigned/signed/float/endian/flags逐个触发共享完整矩阵。普通检查点保存未验证代码，不附测试、SDK或plan触发标记。
- 每轮默认最多一次最终完整矩阵，源码和实际消费测试稳定后触发；第二次仅真实失败修复或明确新集成里程碑。新增/关键消费先执行，其余真实受影响组在同批完成；未变成功组复用，组内完整覆盖不减。
- 迭代早期只做解除当前阻塞的定向Actions/原版取证，不能当完整通过。当前集中入口force_groups仍叠加自动选组；没有合适独立诊断入口时，先把连贯实现做完再触发，不为每个小改动反复跑旧消费者。
- CI绑定原sourceCommit，不要求等待期间冻结共享代码。允许继续开发下一检查点；原run证据仅绑定原提交，当前变化必须在后续稳定批次取得真实受影响最终验收后才能启用或标完成。
- 正常成功先收结构化收据，完整日志/大归档只在失败、矛盾或必要审计时读取。独立模型按风险/明确门槛使用，不为每个helper默认新增；已审计相同SHA的capture及D/R相同字节不重复审计。
- 文档/交接/证据索引不触发Windows；未改变业务和测试输入时不默认追加texture-plan。选择器或依赖逻辑变化、证据矛盾、用户明确要求才运行plan。SDK/MOD仍跨批按明确里程碑触发。
- 这是执行规则，不是工作流改造；本次不改触发器、依赖、矩阵或断言，不宣称现有编排已实现独立诊断/缓存提速。正式验收的全部原版字节比较、D/R、NoOriginal与适用SDK门槛保留。

## 验证编排以功能反馈效率为先（2026-10-29）

预检前移和`priority`/`remaining`分阶段执行已实现并在Actions运行：40838359589的PPM两能力组先完整通过并上传，随后完成27个受影响旧组，整run success；40878697072补volume边界，40880308857仅补专用PPM SDK并复用49组件。保留全部原始SHA、组内断言及各组源/运行锚点；此结果不是跨批性能基准。

- 在耗时Windows矩阵前，集中核对所用原DLL、gold/archive/系数表SHA、检出行尾、路径和依赖；预检失败先定位，避免无效长等待。预检不调用未知越界原版路径，不替代真实业务差分。
- 在同一集中入口/至多一个Windows作业内，优先取得新增/修改能力组及关键实际消费的完整反馈，再完成其余选中的旧消费回归。保留每组全部Debug/Release、PC平台、完整三buffer/cdata/DDS与适用NoOriginal；不把快速反馈或预检称作全部回归完成。
- 同批个别失败时保存并复用其他完整成功组，只补失败/缺失/实际输入变化组。不能因整体failure再启动全部组；也不能为减少组数删除实际依赖或忽略共享代码变化。
- 重复原版采集优先复用具备完整输入/输出、原DLL/工具链/配置指纹和适用范围的不可变gold；恢复版仍执行全量逐字节比较和逐次隔离。新增缓存先验证失效与冷构建，不冻住未初始化/不确定原输出，不把oracle查表用于生产实现。
- .gitattributes/行尾/归档保存规则只改必要范围，先核对历史检出字节；已有原SHA保持不变。旧gold换行问题不能用更新期望SHA、文本归一化或删断言掩盖。
- 普通检查点不启动Windows；新能力组稳定后集中必要最终回归。SDK跨能力合批，完整mymod保持manual_only。长验证期间低频收证，按用户时间上限交接，不反复启动同一批次。

40821998297的历史失败记录（后续DIB/NPOT及SDK已闭环，当前入口见RESUME）：27组passed、18复用，DIB与dib-images完整通过；npot-rgba8唯一失败为旧weights.tsv检出哈希预检，尚未执行该组D/R比较。下一轮先修原字节恢复/规则作用域并核对真实影响，补该失败组，保留其余45项成功锚点；新PcDib SDK仍待运行。此处不将该failure改写为整体success。

## 完整 MOD 降频补充（2026-10-28）

普通组件与候选迭代不运行完整MOD。SDK验证可跨多轮能力组集中；正式启用/关键集成里程碑或实际相关集成故障时，再显式验证当前受影响profile。完整mymod/384资产扩展`sdk-PcMymod`改为`manual_only: true`，普通`[texture-sdk]`或`run_sdk=true`均不会自动选择它。需要完整MOD时，通过同一个工作流的手动入口显式填`force_groups=sdk-PcMymod`；仅运行该MOD时`run_sdk=false`即可，明确同时需要其他受影响SDK时再设置true。显式选择仍保留所有实际受影响组件的选择与完整断言，不是只运行MOD而跳过组件。

该调整不删除依赖/历史收据、不改成功判定，不缩减原样mymod、真实资产、三模式、四产物或完整轨迹/隔离检查。相关输入变化后，旧MOD成功不能证明新源码已集成；正式接入或部署前仍须完成必要验收。完整规则与交接要求见[效率规则](EFFICIENCY.md)。以下旧SDK触发说明以本补充的完整MOD例外为准。

- **普通检查点提交不启动 Windows 验证。** 整个能力批次准备好后，在阶段提交消息加 `[texture-test]`。自动验证统一由 `texture-validation.yml` 承担：一个 Linux 选择作业、至多一个 Windows 作业，选中组顺序执行，不再按组/配置创建矩阵作业。
- `[texture-sdk]` 显式纳入受影响的 PC SDK profile（原PNG/BMP PcNoMip、PcMips，以及压缩组PcDxtNoMip）；普通 `[texture-test]` 不启动 SDK。同样已成功且输入未变的 SDK profile 保留原 sourceCommit/runId，不重复运行。
- `[texture-plan]` 仅运行选择器及其单元检查并上传计划，不启动 Windows。手动入口可指定 force_groups、run_sdk 和 plan_only。
- `TextureTestGroups.json` 明列实际消费者依赖。选择器要求完整成功的 Debug/Release 证据（SDK 则要求 Original/Recovered/NoOriginal 三组成功）、有效祖先 sourceCommit 和 runId，并比较该祖先与当前版本的依赖并集；探针、失败、不完整或非祖先记录不能跳过。原 SDK 工具 DLL 变化使相关组失效；新增/修改但未登记的实现文件按保守方式选组。
- 新的 helper 或消费关系必须更新依赖表。已登记组改依赖时同时核对验收版本的旧依赖，不能靠删除路径隐藏变化。测试数量选择不改变选中组内部覆盖范围。
- 原独立 Texture 组件、入口、SDK 与取证工作流保留手动入口，已从 push 自动触发迁出，功能由新入口集中调度。保留历史运行和证据；不取消、删除失败记录。
- 每个实际组仍生成自身收据；batch/selection.json 区分本轮执行与沿用结果，沿用记录保留原 sourceCommit/runId，不写成当前源码新跑成功。失败组保存证据并继续完成本批其他已选组，最终 batch 必须失败。

当前依赖表按 TGA 入口与 PNG/BMP 入口分别登记：只改 PNG 解码分支不会重跑只使用 TGA 的旧入口；修改共用 ImageLoader 或插件入口时才扩展到其实际消费者。生产 SDK 配置及 DLL 仍不替换。

## 取证入口补充

[texture-dxt-evidence] 显式选择 dxt-symbols，用同一个集中入口采集参考库 COFF 符号；该组 manual_only/evidence_only，普通回归与 SDK 不会自动选它。收据为 probe-only，绝不能作为 DXT 恢复验收或业务组件成功证据。40236386719 实测只选此组，复用全部20个已验收组件组；13项选择器检查包含旧依赖删除、实际非祖先提交、SDK完整三模式及取证非自动触发边界。

[texture-dxt-blocks] 显式选择 dxt-blocks 采集原版小块样本；同样 manual_only/evidence_only，普通回归和SDK不自动执行。40237236370只运行此组，20个已验收组件组全部复用；其probe-only收据不能作为DXT实现完成证据。


## PC 唯一目标

PS3 mip/cubic/resize 与其 SDK/取证入口已删除，Xbox360 与 unknown 平台不进入业务差分。非 PC Initialize/ReInitialize 的明确拒绝由 metadata 组检查。历史 PC/PS3 收据原样保留，但选择器须对实际变更重新验收 PC 消费组。修改仅平台专属文件时不扩展到其他编译器；公共 PC 依赖按原规则选组。

`dxt3-alpha` 是自动组件组，只验证 DXT3 显式 alpha 子组件，不能作为完整 DXT 压缩器或 SDK 验收。当前已由完整块、对齐DDS和无mip插件入口消费，依赖登记在dxt-blocks-complete、dxt-dds2d、dxt-images与sdk-PcDxtNoMip；新增消费仍须同步维护依赖及完整差分。

`dxt5-alpha` 的完整门槛包括 Debug/Release 自有 native x87 helper 构建与 PE 检查、原量化器端点/索引/误差位模式、实际 DDS 的8字节alpha、归一化float组件及独立 NoOriginal重放。新增消费者必须登记 `src/TextureCompiler/Dxt/**` 及托管alpha入口依赖，并复制自有native DLL；当前已由完整块、对齐DDS和插件入口消费，sdk-PcDxtNoMip包含自有native构建/复制与三模式验收；子组件证据不能替代消费者或SDK验收。`dxt5-fast-alpha` 只接受质量68低层state；`-InspectDdsEntry` 保留原版未初始化levels的失败探针，不作成功验收。`dxt-decompile`/`dxt-colour-functions` 是手动取证组，部分导出或超时不充当业务验收。


`dxt-images` 验证真实Texture/OnDemandTexture的TGA/PNG/BMP无mip压缩入口、三缓冲区/完整cdata/旧文件尾及NoOriginal，1792输入明确区分1424成功输出与368原版拒绝。`dxt-samples` 独立验证归一化float与1/2/4小块，小尺寸无mip消费者另由dxt-dds2d/dxt-images/SDK取得完整验收，不能升级mip范围。`sdk-PcDxtNoMip` 仅在显式SDK请求时运行：178张原版可成功解码的输入、712个DXT3/5标准纹理资产，Original/Recovered/NoOriginal实际Core调用、四产物及完整轨迹比较。拒绝样例保留组件矩阵，不纳入SDK成功输入。


`dxt-dds2d` / `dxt-images`现含完整PC POT二维float mip；新增显式SDK组`sdk-PcDxtMips`。共享triangle变化必须使其raw和压缩消费者共同失效。源码7ae4af37/40524039917已通过整批9组，复用19个未变成功组。

`[texture-dxt-volume-boundaries]`显式选择`dxt-volume-boundaries`，同一集中入口采集416原版压缩volume样本（无mip/完整mip、header、完整DDS/RGBA/options/callback），manual_only/evidence_only、probe-only，不作为恢复或SDK验收。普通组件/SDK不会自动启动该探针。

`dxt-volume-dds`完整验证PC POT DXT3/5 volume的无mip/递归float mip，5568完整DDS与独立NoOriginal。`dxt-images`现含7856标准/volume入口，含原版拒绝匹配，完整三缓冲区/full cdata与NoOriginal；已实现的volume输入从拒绝矩阵转入成功对照，NPOT与默认DXT1/1A拒绝保留。`sdk-PcDxtVolumes`在显式SDK时运行186条带输入/1488资产（有/无mip合并）三模式与逐次隔离断言。新增VolumeDds/NextLevel消费者已登记依赖，aaacb3d2/40526443463整批14组通过、16组复用。

`[texture-npot-load]`仅显式选择`npot-load`原版LoadImageA取证，1012记录含POT对照，manual_only/evidence_only/probe-only。辅助恢复程序集只提供既有Image数据类型与fixture源码依赖，不调用恢复NPOT路径，不作为恢复/SDK成功证据。

## 8位NPOT最终验证与复用

`npot-rgba8`包含1851原系数tap位模式、1012不可变原RGBA以及公共ImageLoader完整比较，Debug/Release、输入不变与逐次NoOriginal。shared helper已登记所有TGA24/32、PNG RGB/RGBA8、BMP24/32实际消费者。`dxt-images`矩阵14256（raw3/DXT3/5、POT/限定NPOT、标准/volume、有无mip、新旧cdata），保留34低位深NPOT/默认DXT1等拒绝；旧raw TGA的NPOT拒绝分支转为全buffer/cdata严格对照。显式`sdk-PcNpotRgba8`跑80输入/3200资产三模式、四产物与完整轨迹/逐次隔离。

业务a945c18c/40533831752的17组（含全部6个SDK profile）成功，整轮failure因唯一回放宿主遗漏旧TGA全零alpha规则。最终d012da8a/40536063863 success仅补npot-rgba8，复用其余31组完整成功收据，未重跑未变SDK/颜色/alpha/直接DDS。保留失败状态与原锚点；不能把probe或整轮failure描述成新整轮success，也不能因为别组宿主失败而重跑已成功类型。


`atlas-entry`验证完整公共TerrainTextureAtlas入口：旧25原gold和新23例，D/R各48、完整三buffer/DDS/cdata/错误及逐次NoOriginal。`sdk-PcAtlas`仅显式SDK时纳入，16资产三模式实际Core/四产物/完整轨迹。be520b8e/40607538234已通过，独立atlas-dds保留原锚点。

`[texture-jpeg-load]`仅显式选择原JPEG LoadImageA/六种IJG6b选项取证，manual_only/evidence_only/probe-only；不作为公共JPEG、组件或SDK验收，不自动重跑未变成功组。

`[texture-dxt1-boundary]`仅显式原版栈边界取证；3个新进程、每进程1280完整DDS，含DXT3/5稳定对照。manual_only/evidence_only/probe-only，不能作为恢复组件或SDK验收。`sdk-PcMymod`显式SDK组验证完整原样mymod与保留所有Includes的384资产全SDK图像扩展；原样0贴图调用不冒充该插件覆盖。

## 透明块DXT1A完整能力组

`dxt1a-blocks`对生产托管block/byte/float/partial接口执行D/R原版完整比较，4608块、3456份块DDS，完整源/options/双方输出与逐次NoOriginal；`dxt1a-dds`比较3648个二维/volume、有无完整mip输入，2714个完整DDS全等，934个含不透明实际块的输入明确拒绝且不调用不确定原路径；`dxt1a-images`对两根执行768个POT/partial/NPOT及标准/volume/mip输入、三buffer/full cdata/旧文件尾、原invalid-volume错误和每次NoOriginal，另保留DXT1/不透明DXT1A及混合透明/不透明块拒绝和cdata所有权检查。三组为严格组件组，成功收据必须含Debug/Release `passed=true`，不能复用evidence-only比较作为正式通过。

`sdk-PcDxt1a`只在显式SDK里程碑选择，80张TGA、320实际根资产，Original/Recovered/NoOriginal三模式、四产物与全部实际轨迹完全比较；不替代完整mymod，不改变其manual_only规则。新helper已登记所有实际消费者；SDK和未变成功组保持各自精确sourceCommit/runId。完整能力组稳定后集中验证，不为内部orientation或单个候选重复启动所有消费者。

## PNG ICC/APNG/sRGB metadata

`png-metadata`固定7920个原版gold输入，D/R执行直接/public两入口完整RGBA/原错误/源不变和逐次NoOriginal。`png-metadata-images`为D/R各34880原/恢复插件输入及同量隔离重放，含raw3/DXT3/5、透明DXT1A、标准/volume/完整mip、64个PNG编码atlas、三buffer/full cdata/旧尾。1cff9256/40780996417已整批15组通过、22组复用；不重复跑未变组。

`sdk-PcPngMetadata`仅显式SDK时选，4240实际资产、三模式四产物/完整轨迹文件集与逐次NoOriginal。metadata共享loader及native消费者已登记实际依赖；专用SDK不替代完整MOD。`png-metadata-load`/`png-metadata-order`/`jpeg-metadata-load`均manual_only/evidence_only/probe-only：前两组使用`[texture-png-metadata]`/`[texture-png-order]`，后者使用`[texture-jpeg-metadata]`。可在同一明确SDK批次顺序采集下一项探针；SDK成功与probe结论必须各自记录，比较完成不能冒充恢复验收。

## JPEG RGB/APP metadata 与 CMYK/YCCK 拒绝

`jpeg-metadata`固定4000原gold，D/R各直接/public两入口8000调用、完整RGBA/原错误/源不变与逐次NoOriginal；自有IJG native分别D/R构建。`jpeg-metadata-images`各32080完整输入（19280输出/12800原拒绝）和同量NoOriginal，含80 metadata atlas、raw3/DXT3/5、标准/volume/full mip和旧cdata/三buffer。`sdk-PcJpegMetadata`显式SDK组为9640真实资产、三模式四产物/33721完整轨迹文件集及逐次NoOriginal。dba6d305/40785653416已整批通过，仅执行2新组件+1新SDK、复用旧37组件+11 SDK。原探针仍probe-only，生产JPEG源码未改；相关helper/消费者/gold依赖已登记，未变成功组不重复跑。当前39组件/12专用SDK有效；完整MOD规则不变。

## 2026-10-29 TGA metadata SDK 里程碑

组件a928f981/40795326615的19完整组通过、22复用。SDK278ae577/40797426507的11 profile全部passed，41组件与两个纯PNG/JPEG metadata SDK复用；每模式21576实际资产/调用、75459完整轨迹文件及四产物独立全字节核对。当前41组件/13专用SDK有效，原锚点见handoffs/20261029-tga-metadata/acceptance.json。整个40797426507仍为failure：附带BMP原版探针在重复相同输入时产生不同像素，不能混作SDK失败或整体success；仅修改取证记录工具并显式重跑bmp-header-load，不重跑未变成功SDK。不稳定原输出留作不确定边界，不可当确定gold或恢复验收。

BMP探针补取51189674/40799686293已completed/success，仅执行`bmp-header-load`并复用41组件，未启动SDK/MOD。独立归档审计及重复不确定域见[当前BMP入口](TEXTURE_BMP_HEADERS_NEXT.md)；该success仅证明取证完成，`probe-only`/`productionAccepted=false`不变，不可用作跳过BMP新增实现验收的证据。

## DIB与BMP真彩表消费

`dib`扩展为裸DIB/BMP六入口和306个实际原BMP表调用；`dib-images`覆盖50304完整插件输入/配置及同量NoOriginal，432 atlas为必须成功的正向断言。原已通过独立DIB锚点仍是40816168489，新范围不可直接复用旧收据。

`[texture-dib-sdk]`在同一texture-validation.yml显式选择`sdk-PcDib`，保留正常受影响组件选择，REQUEST_SDK不因此开启全部旧SDK。专用21464资产/模式，完整包/安装回滚/唯一ID/三模式四产物/全部轨迹和逐次隔离；不触发完整mymod。新公共DIB helper已登记所有loader消费者，不能靠缩小依赖省略必要回归。

专用DIB SDK脚本在包构建前重新核对当前组件收据：允许本批刚写出的成功组与未变祖先，任一缺失/失败组仍被选择则立即停止，不开始SDK。保存component-readiness.json及源/运行锚点供独立审计。RESUME.md已列入集中入口的path监听，因此可在收齐组件后的交接提交显式附`[texture-dib-sdk]`；普通交接无标记仍不启动计划或Windows，不以文档提交的状态冒充组件/SDK成功。

## PPM公共源码与六DLL SDK里程碑

`public-ppm`/`ppm-images`在838b40b3/40838359589完整通过；`ppm-volume-boundaries`在b999b667/40878697072完成D/R各11520输入与NoOriginal，该run因后续SDK回滚白名单失败，不能改写整体状态。回滚修复后bdecb870/40880308857仅执行`sdk-PcPpm`，49成功组件复用；三模式各15712资产、32成功atlas、四产物/54977完整轨迹和六DLL包/安装回滚/来源均已独立审计。精确范围、普通合并输入证明见TEXTURE_PPM_NEXT.md，不自动运行旧SDK或完整mymod。
