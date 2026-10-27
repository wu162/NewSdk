# PC TerrainTextureAtlas 后续入口

本组尚未恢复，生产SDK未替换。已验收的TexturePacker仅为内存packer；不能据此声明完整atlas加载、压缩、二进制输出或SDK验收。继续同一recovery/texturecompiler-source分支，禁止强推，所有构建与测试只在GitHub Actions。

## 静态原版依据与当前缺口

不可变原DLL SHA256为4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e。原始IL的Plugin.HandleTerrainTextureAtlas在RVA 0x69f4，1475字节；Atlas marshaler在RVA 0x696c，124字节；TexturePacker构造入口在RVA 0x4a70，120字节。原始取证来自既有raw-listings，不以现代格式惯例代替原始调用链。

- XSD为SDK/Schemas/xsd/AssetTypeTerrainTextureAtlas.xsd，Tile含TextureID、BaseTexture、NormalTexture及可选Cell列表；AtlasSize默认2048、AllowLossyCompression默认true。原marshal源结构16字节：tile列表在+4、AtlasSize在+12的ushort、AllowLossyCompression在+14。是否实际影响PC输出须按业务读值/原版差分核对，不能仅依据XSD名字设计行为。
- 原packer会加载256×256基础图，根据有效tile数选atlas尺寸，复制periodic padding后排序。现有TexturePacker已恢复内存路径和部分文件边界；完整插件需接TextureImageLoader并对原版完整buffer、路径、缺失图与错误边界核对。
- PC分支明确新建原nvCompressionOptions，再写+476格式与+104指定mip层数：基础atlas分别为3和5；有normal atlas时分别为9和5。原IL在IL_0338..0391。原DDS确认格式9为RGB555；需适配指定五层payload/header，而非新增过滤核。
- 原TerrainTextureAtlasRuntime声明28字节；输出tile为12字节，TextureID及四个ushort坐标按原排序填入；两个TextureFileData位于runtime +12/+20。必须取得实际原buffer与relocation收据后再实现Tracker分配/对齐/顺序，不能仅按静态声明宣称ABI已验收。
- 原PC分支使用原默认mip生成设置配合指定五层；不是已验证的二维默认完整triangle链。本次原DDS已证明+104是层数覆盖，不是过滤核。继续复用已验收triangle的float精度、tap/归一化/最近tap校正与pass次序。PS3/Xbox专属分支均不属于当前任务。

## 已完成的原版前置探针（probe-only）

TextureAtlasPrerequisitesProbe.cs / InspectTextureAtlasPrerequisites.ps1为manual_only、evidence_only的atlas-prerequisites组：10种POT形状（含1/2小边与长条）×8图案×格式3/7/9×层数覆盖0/5=480样本，每份原调用两次核对完整DDS、callback与options确定性，记录RGBA、完整DDS、1644字节选项、DDS字段、header格式跳转表和相关静态数据。7/0是原raw/完整链对照；3/9和5来自上述真实PC atlas调用。该probe不调用恢复编码器，不冒充完整atlas或SDK。

集中workflow标记[texture-next-probes]与较大尺寸NPOT存储探针一起执行；普通检查点不启动Windows。40597016969中已完成；不因解释更正重跑相同原样本。后续门槛是完整指定五层浮点mip与格式9输出、实际atlas原插件全buffer差分/NoOriginal、适用专用MOD三模式SDK；默认DXT1/1A与生产部署保持独立边界。

## 40597016969 原版取证后的语义更正

480样本/960次原调用通过，原始收据仍probe-only。旧探针把+104变量、TSV列名和文件后缀命名为filter，这是解释错误；实际写入值没有错误，原DDS/options/callback记录保持不可变。+104=0生成完整自然链，+104=5恰好写出五个payload。DDS mipCount是min(自然层数,5)，自然链不足五层仍重复写1×1尾层；例如1×1的mipCount=1却有五个payload，32×16则mipCount=5、五个payload。原格式9是RGB555（16bpp、PF flags0x40、mask7c00/03e0/001f、无alpha），并非压缩或signed normal格式。补充范围见该原证据目录SCOPE.md，不改写原JSON/TSV。

候选TextureAtlasDdsEncoder仅提供PC DXT5/RGB555指定五层的独立DDS组件，直接消费已验收triangle float与SampleBlockEncoder；公共atlas插件路由未接入。新隔离回放选完整160条格式3/9、+104=5记录，Debug/Release对完整DDS、五级布局/原callback、12个不安全输入拒绝及每次原DLL缺席/未加载核对；其余320条原控制记录不冒充新组件样本。现待Actions；完整512..2048图谱、文件/Tile Marshal、最终Runtime buffer/relo与SDK仍是下一整组。

## 独立DDS组件已通过，完整入口取证准备

源码daf4a749752786d5aea56b8de62fc47f1e77383d / 40598863269已completed/success，atlas-dds完整Debug/Release通过（精确检查数见收据），原DLL逐次不存在且未加载；公共TerrainTextureAtlas仍拒绝。旧NPOT4986与新较大比例4432隔离回放同批通过，未扩大atlas业务入口。

准备atlas-load original-only组：25个完整ProcessInstance用例，每个两次，共50调用；12个0/1/2/3/4/7/8/9/10/21/22/49 tile阈值、AtlasSize/AllowLossy变体、256/128 normal与缺失normal、重复TextureID及Cell输入、base文件缺失/扩展名/尺寸/原解码拒绝、NPOT和24-bit源、50 tile上限。读取完整Runtime/两DDS/三buffer、XML与原输入，核对重复确定性、source/XML/cdata不变；19预期成功、6既有原错误。此probe仍不调用恢复atlas插件，不冒充SDK。临时目录使用可移植相对图像名，不把运行器绝对路径嵌入记录。
