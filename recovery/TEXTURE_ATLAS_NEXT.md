# PC TerrainTextureAtlas 后续入口

公共TerrainTextureAtlas恢复入口尚未接入，生产SDK未替换。现已具备验收过的内存TexturePacker、独立DXT5/RGB555固定五层DDS组件，以及完整原插件25例gold；这些不冒充完整atlas恢复版或SDK验收。继续同一recovery/texturecompiler-source分支，禁止强推，所有构建与测试只在GitHub Actions。

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

完整atlas探针首轮dfbeaee3/40599421956整体failure只在probe宿主误假定count=0必有Runtime数据，两个受影响raw SDK profile三模式各600调用已通过、保留收据。current.txt与原IL_00f3..00ff已证实：声明Tile列表为空时，原HandleTerrainTextureAtlas直接跳过packer与FinalizeTracker，返回InstanceData/RelocationData/ImportsData均null；不能把这一行为等同于声明非空但所有基础文件不存在。宿主现明确核对该null契约，仍保留全部25×2原调用和完整非空Runtime/DDS断言，只补此probe，不重跑已通过SDK。

## 最终原插件gold与下一组的精确布局

源码056937b13fa39431ec9171b0d990e6a5c15a013c / [40599993769](https://github.com/wu162/NewSdk/actions/runs/40599993769) attempt1 completed/success：atlas-load为25例、19成功/6原错误、50实际原调用、468检查，重复三buffer确定性、源图像/XML/cdata不变。samples.zip含可移植input XML、原图像及input-files.tsv、完整instance/relo/imports；这是probe-only，尚未对恢复atlas入口差分。该轮只跑atlas-load，36个完整成功组（含7个SDK profile）按原sourceCommit/runId复用。

静态读取这份Actions产物所得布局（后续实现必须全量gold差分）：

- 声明Tile列表为空：三个AssetBuffer字段均null。声明非空但所有base缺失：仍生成512×512基础DDS，runtime effective count=0，ImportsData为空数组；不能统一返回null或空数组。
- Runtime首部28字节：+0为0，+4有效tile数，+8 tile数组指针，+12/+16 base DDS指针/长度，+20/+24 normal DDS指针/长度。块顺序为root、base DDS、有normal时的normal DDS、最后12字节×有效tile数的数组；每块沿用4字节对齐。
- 有normal的relocations顺序是12、20、8、0xffffffff；无normal为12、8、0xffffffff。即便有效tile数0，+8仍指向最终buffer末尾（一越界末指针），并保留relocation 8，不能改成null。missing-base例：root=(0,0,349340,28,349312,0,0)，relo=(12,8,0xffffffff)。
- count-1（有normal）root=(0,1,1047836,28,349312,349340,698496)，两个DDS都是512×512；count-49两个DDS均2048×2048。normal-128被原入口接受，仍拷入完整atlas canvas。非空成功的ImportsData为空数组。
- count-1与AtlasSize=0/65535、AllowLossyCompression=false的三个metadata变体完整buffer哈希相同；不按XSD名称发明原版没有的输出变化。输入含重复ID和Cell，排序与坐标应复用已验收packer，再按实际gold核对。
- six errors均为BadTextureFile：base/normal扩展名必须含tga；base加载后须256×256；原拒绝的gray16用于base/normal时仍为D3DX cannot load；50 tile报Maximum number of textures exceeded (50). Limit is 49. 缺失base先跳过，缺失normal的错误扩展名先报targa要求。NPOT 255×255 base先被LoadImageA取整到256×256后可接受，24位base也已捕获。

下一轮直接在此gold上完成Plugin.Atlas路由/Marshal及Runtime序列化，恢复版全25例原buffer/cdata、源/错误/空字段对照与NoOriginal；扩展真实文件/边界矩阵并取得专用atlas MOD三模式SDK。原DDS组件的小形状160例不代替512..2048实际atlas图谱的最终差分。保留所有失败与probe-only来源，不重新执行已完成原取证。
