# TextureCompiler 输入和 SDK 调用契约

本页只记录当前仓库原始配置、XSD、Core 源码及原 DLL 元数据证据；不是恢复完成声明。原 DLL：`SDK/tools/BinaryAssetBuilder.TextureCompiler.dll`，SHA256 `4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e`。

## 实际入口

`SDK/tools/BinaryAssetBuilder.exe.config` 将 `TerrainTextureAtlas,Texture,OnDemandTexture` 路由到 `BinaryAssetBuilder.TextureCompiler.Plugin, BinaryAssetBuilder.TextureCompiler`，启用构建缓存。`PluginDescriptor.Initialize()` 用无参构造创建实例，并调用 `Initialize(null, platform)`；`ReInitialize()` 同样传入 null 配置。

`IAssetBuilderPlugin` 要求 `ProcessInstance(InstanceDeclaration)`、`GetAllTypesHash()`、`GetVersionNumber()`、`GetExtendedTypeInformation(uint)`，基接口还包括 Initialize/ReInitialize。`TargetPlatform` 为 Win32=0、Xbox360=1、PlayStation3=2。

`AssetDeclarationDocument.ProcessInstances()` 先取插件元数据并把 HasCustomData 写入 InstanceDeclaration；`CompileInstance()` 在为真时建立 cdata 目录和 `<asset-file-base>.cdata` 路径，然后通过 `Handle.TypeId` 查插件，直接调用 ProcessInstance。AssetBuffer 由 InstanceData、RelocationData、ImportsData 三缓冲区组成。BinaryAsset 的缓存命中、复制、提交同时处理 .cdata，因此组件验证必须检查自定义数据文件内容、缺失/零长度行为，不能只比较三个缓冲区。

`AssetDeclarationDocument.HandleFileReferenceType()` 在插件前把 FileReference 的值 trim、转小写、解析实际文件路径，再以小写实际路径替换 XML 属性，同时记录逻辑引用到 ReferencedFiles 和 DependentFiles。组件探针应模拟这个阶段；真实 SDK 测试应用逻辑 art 路径验证预处理和依赖记录。

## Schema 范围

| 类型 | XML 输入 |
| --- | --- |
| Texture | 继承 BaseAssetType；File:FileReference；Type=StandardTexture；GenerateMipMaps=true；OutputFormat=DXT1；AllowAutomaticResize=true |
| OnDemandTexture | 同上；File 标记 pipelineOnly，Type 不标记 pipelineOnly |
| TerrainTextureAtlas | AtlasSize:ushort=2048；AllowLossyCompression=true；零个或多个 Tile |
| TerrainTextureTile | TextureID:uint、BaseTexture:FileReference、NormalTexture:FileReference 均必填；零个或多个 Cell |
| TerrainTextureCell | IndexX、IndexY:byte 均必填 |

原插件元数据只把 OnDemandTexture 标记为 HasCustomData=true；Texture 和 TerrainTextureAtlas 为 false。GetVersionNumber 在 PS3 返回0，但各类型的 PS3 ProcessingHash 有独立常量，不能用统一版本getter推导全部元数据。ReInitialize 仅更新hash/platform，不虚调 Initialize。

TextureType 枚举为 StandardTexture、VolumeTexture、CubeTexture。OutputFormat 枚举为 DXT1、DXT1A、DXT3、DXT5、A8R8G8B8、X8R8G8B8、R5G6B5。源文件见 `SDK/Schemas/xsd/AssetTypeTexture.xsd` 与 `AssetTypeTerrainTextureAtlas.xsd`。

TerrainTextureAtlasRuntime 属 Xml 类型组，有 BaseTextureAtlas、NormalTextureAtlas 两个 FileReference 及 TileRuntime 列表，TileRuntime 包括 TextureID、UpperLeftX/Y、BottomRightX/Y。不可因为名字相近把这个 XML 编译器根也改路由到纹理插件。

## DLL 的可恢复边界

PE32/x86，CLR Flags=16（混合模式）。插件业务方法具有 IL：IsValidTexture、GuessProperFormat、GetNVCompressionOptions、CompressAndEmbed/3D、EmbedPC/PS3/XenonTexture、ProcessTexture、HandleTextureType、HandleOnDemandTextureType、HandleTerrainTextureAtlas、ProcessInstance 及元数据/生命周期方法。

TexturePacker 的以下方法也具有 IL，不应因 C++ 名称或指针签名把整个打包器误判为 native：

| MethodDef token | 方法 | RVA |
| --- | --- | --- |
| 06000022 | AddTexture | 4658 |
| 06000023 | TextureSort | 19c8 |
| 06000024 | PackTextures | 4808 |
| 06000025 | CopyTexture | 2c1c |
| 06000026 | MergeTextures | 3514 |

静态链接本地函数需要独立恢复或明确保留为未支持边界：nvDDS.nvDXTcompress 两个重载（060000db/RVA52950、060000dc/RVA52b60），XenonBundler.Process2DTexture/Process3DTexture/ProcessDDS（060000e1/RVAc070、060000e2/RVAc100、060000e3/RVAc190），D3DDevice/Resource/Texture 包装及 D3DXCreateTextureFromFileA。不能把复制原 DLL 的机器码或转发调用记为这些算法已还原。

DLL 诊断字符串可定位取证分支，但不是完整行为结论：非 DDS CubeTexture 提示“不支持，先用 D3D Texture tool 生成 DDS”；terrain 图像要求 Targa、256×256，纹理数量上限49；体积图像宽度需为高度的整倍数；存在空像素/压缩缓冲区上限/非二幂的粉色回退及 alpha 导致 DXT3/DXT5 自动改选诊断。具体触发条件、返回布局和文件副作用必须以 IL 和原版探针确认。

## 验证建议

1. 元数据及生命周期：三根和未知 TypeId；平台0/1/2/99；Initialize、ReInitialize、Dispose 前后行为；ProcessingHash/TypeHash/HasCustomData/Tokenized/版本逐项对照。
2. DDS 路径：标准2D、带/不带 mip、alpha、不压缩格式、cube/volume DDS；Texture 和 OnDemandTexture 都比较三缓冲区与 .cdata。文件扩展名大小写、空文件、缺失文件等验证异常边界。
3. 非 DDS：TGA 不透明、1bit alpha、恒定非零 alpha、渐变 alpha；七种 OutputFormat、GenerateMipMaps=false/true；二幂与非二幂；Volume 横排切片、无效宽高；Cube 显式拒绝。
4. Atlas：空列表、1/2/49/50 tiles；相同和不同 TextureID；Cell 顺序/重复/边界；TGA尺寸和格式拒绝；AtlasSize、压缩开关；比较生成布局和两张 atlas 的像素/压缩数据。
5. SDK 集成：准备带有效 art 依赖的最小合法 MOD，经原 Core 路由真实调用；原版/恢复版同输入比较 manifest、bin、relo、imp 和 cdata，恢复回放目录排除原 TextureCompiler DLL。未验收的 native 压缩/主机平台路径继续保留未完成状态。

现有真实资源可用作补充样例：`SDK/Mods/W3xRecovery/Art/W3xRecoveryTexture.tga`、`SDK/Art/AU/AUGunship*.tga`、`SDK/Art/Images_EP1/*.dds`。生成的小型可控样例更适合定位 mip、alpha、字节序和文件副作用差异。

## 本轮元数据恢复和验证边界

`src/TextureCompiler/Managed/TextureCatalog.cs` 和 `Plugin.Metadata.cs` 恢复上述元数据表及托管状态。Initialize 不分配原版D3D/Xenon资源或64MiB共享压缩缓冲区；Dispose不复制原版模块全局native释放，因此不能以元数据通过宣称原native生命周期等价。

`recovery/tests/TexturePluginContractOracle.cs` 输出原版全部托管签名及四平台Initialize/ReInitialize/Dispose观察；`TextureMetadataOracle.cs original.dll recovered.dll` 对照原版state-only ReInitialize与恢复版Initialize/ReInitialize的元数据及托管状态，不调用原native初始化/清理。两者均不执行纹理编译或SDK集成。实际构建及运行结果以本轮脚本生成的精确证据为准，本页本身不构成成功收据。
