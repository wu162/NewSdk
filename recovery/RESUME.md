# TextureCompiler：PC续接

分支：`recovery/texturecompiler-source`；仅PC、禁止强推、编译测试仅Actions。所有后续贴图还原优先复用公开实现并适配原版，见[公开实现入口](TEXTURE_UPSTREAM.md)。先连续实现实际消费链，稳定后集中验收；普通检查点不触发完整测试/SDK/MOD。

## 本轮完成（2026-10-30，压缩原生 DDS 候选）

- 继续独立分支 `validation/texture-public-d3dx-bc`，最新业务/探针修正检查点 `a0fb7d25`。固定 DirectXTex BC1/2/3 已接通文件编解码差异、预乘/去预乘、真实 DDS 文件入口、完整 provided 块/partial 尾层、从实际上一存储解码的 BOX 缺层、小 base 至少4/NPOT 上取2幂的公开 triangle float 适配、原生资源所有权与实际 masked/physical RGBA 消费。**候选未验收，未合入业务源码，正式 native loader 仍拒绝压缩 DDS。**
- `ce16e990` / `41010488699` 首先确认1536 XG float decode及flags0完整encode块 D/R全部相同。文件分流和量化舍入后 `bb8032e9` / `41012948892` 文件1536 decode/无抖动/抖动完整块全部相同；新增1572默认DDS中1226完整匹配/346差异，D/R每配置266136903字节相同、原版重复稳定。
- 最近完整探针 `824da4e8` / `41014720897` 为 probe-only：补 Newton 舍入后1572默认文件1249匹配/323差异，文件BC3编码另有31记录回归（重复输入入口 `bc-file-block-f20-23`）；XG块和文件decode仍全相同。显式扩展算术没有改变完整capture SHA，不能说已修复。来源、完整锚点和下一定位见 [候选专页](TEXTURE_PUBLIC_D3DX_BC.md)。
- 已写严格 `native-bc-files` 手动组件/NoOriginal逐次隔离回放，保留完整原 float/block、默认文件/VM/每层实际RGBA、EOF及拒绝所有权；**尚未通过，不运行它来冒充验收**。所有定位只选`public-d3dx-bc`、复用64旧成功组件，没有 SDK/MOD 或生产六DLL替换。
- `2904039f` / `41011903071` 的范围检查失败及超100MiB归档失败均保留：107556655字节原partial capture已从原Actions artifact无损压缩保存；finally归档已修复。新float阶段 `9f61058d` / `41015245262` 在`Original float pitch`失败，原FileEx格式协商不能当固定float输出，**其partial float字节不是业务gold**。最新已改为原CPU构造+surface view+文件实际surface-memory入口，并断言真实float格式/尺寸；当前源未验证。

## 下一轮计划

1. 先收取 `a0fb7d25` 的最后定向 Actions [41016298705](https://github.com/wu162/NewSdk/actions/runs/41016298705)（本轮收尾时仍在运行），核实真正的 sourceCommit/终态。显式CPU float入口若仍失败，修原ABI/格式/尺寸契约，不能忽略断言或将非float缓冲区冻结成gold。
2. 根据360份真正float阶段的完整结果区分解码/triangle与文件编码误差。保留1572默认DDS及旧1536 XG/1536文件块/168策略矩阵；文件BC3的31重复记录需查通用量化/solver存储规则，禁止样本分派。当前仍323默认文件差异，不能直接启用压缩加载。
3. 连贯消费固定规则全部匹配后，再集中运行`native-bc-files`完整D/R/逐次NoOriginal及必要实际消费，满足门槛后普通合入；新ABI正式进入生产包前另取适用SDK证据。未变64组件和旧SDK不重复跑。继续所有贴图优先适配公开实现；不扩展非PC。

## 已验收基础（2026-10-30，公开原生加载批次）

- 复用固定Microsoft DirectXTex triangle，接通原生TGA24/32和raw A8/X8 DDS的NPOT加载；各轴向上取2幂，DDS逐已提供层重采样，再从已上传层BOX生成缺失mip。保留X8物理未使用字节与端序、52字节header、主/secondary VM及packed tail所有权。
- 复用固定stb_image BMP，支持BI_RGB24/32的core12、INFO40、52、56、OS2-64、V4-108、V5-124，含方向、padding、真彩色表、忽略bfOffBits的原版实际像素位置及有界V5 profile。原版在BGR24展开前按源DWORD扫描alpha；适配器保留有界扫描规则，对会越过文件末尾而依赖未初始化堆字节的输入明确拒绝。来源/通知/修改前后SHA在`PublicDirectXTex`/`PublicStbBmp`。
- 业务源 `d0a31471906307690863e24784ca7384ef2371bf` / [41006229811](https://github.com/wu162/NewSdk/actions/runs/41006229811) completed/success：6860构造、14606文件（14501支持范围完整成功对照，包含5665新增NPOT与6152 BMP；105压缩DDS原版成功仍明确未支持）、另13拒绝。D/R及逐次NoOriginal全部通过；每配置634070359字节完整trace相同，四个归档原始成员SHA和12份x86 DLL指纹已独立核对，见[收据](evidence/texture/handoffs/20261030-native-public/acceptance.json)。
- 本次只补native-loader，复用63组；base保留 `34946af55168a43edd350ea12b8329833b16b8bb` / [41005120850](https://github.com/wu162/NewSdk/actions/runs/41005120850) 的完整成功收据。该首轮整体failure保留：BMP源alpha契约与宿主x86累计trace内存耗尽；修复后完整流式写盘/逐条NoOriginal读取/EOF核对，不减少样本或字节断言。当前64组件收据有效。
- 已普通合入恢复分支（合并提交 `710bae1200efe5f09067dcb90c0c8380b468fd72`），未启用生产六DLL包，未宣称完整混合模式身份或SDK集成；本轮未跑SDK/MOD。详细范围与边界见[原生文件加载](TEXTURE_NATIVE_FILE_LOAD.md)。

## 此BC分支最终交接（2026-10-30，未正式验收）

本轮后续已实现完整POT base>=4压缩DDS资源/mip和实际压缩→raw的masked/physical-endian消费。d50b04a9/41009041890已probe-only完成，D/R完整capture各89979984字节相同，原版重复稳定；固定flags0完整匹配全部1536个BC1/2/3编码块。仍有976个decode块位模式差异，以及最佳flags0/wrap下86/168完整DDS差异，绝不能算压缩支持。

此最新源码又补精度规则：字节归一化显式使用原存储float倒数并落盘，BC3 alpha palette先存右项乘积再做左项扩展乘加。后者静态数值核对全部8192个原alpha通道一致；这些新补丁未Actions验证，不能继承d50b04a9结果。正式loader和生产包未调用候选；尚无当前源码的完整NoOriginal/SDK验收。下一轮先验证此最新精度源码的完整块位模式，再区分文件FourCC与原XG格式对三色/去预乘的语义，定位全部DDS尾mip/缺层及两RGBA视图。小base>=4限制、NPOT等边界仍需完成。64旧成功组件及SDK未变，不重复跑。
