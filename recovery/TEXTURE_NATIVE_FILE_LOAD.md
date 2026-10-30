# PC原生资源与文件加载

入口：`src/TextureCompiler/Native/TextureNativeLoaderApi.{h,cpp,def}`；独立x86 `Recovered.TextureNativeLoader.dll`，由`BuildTextureNativeLoader.ps1`构建。依赖已恢复Resource/Surface/Copy ABI，实际消费联合核对Device/Descriptor。**未进入生产六DLL包，不代表完整混合模式身份或SDK集成。**

## 当前实现

- `TextureCreateCpuTexture(width,height,levels,format)`：普通linear单面二维资源，POT/NPOT、levels0完整链或受限层数。52字节GlobalAlloc对象和VirtualAlloc主/secondary存储、四端序标签、small mip packed-tail、引用与最终释放遵守原契约；tiled/sub-byte/无效尺寸或层数明确拒绝。
- `TextureCreateTextureFromFileA(device,path,output)`：非渲染type1设备；ANSI路径、只读/共享读/OPEN_EXISTING/FILE_FLAG_RANDOM_ACCESS、有界读取256MiB。成功转交资源所有权，失败不转交部分资源；缺文件HRESULT88760b59。
- raw A8/X8 DDS已有部分/完整mip、缺层BOX；支持POT/NPOT，NPOT各轴向上取2幂，逐已提供层分别重采样。未提供层从实际已上传的前层生成，不把文件中的旧源尺寸当目标尺寸。
- TGA24/32 raw/RLE、四原点方向、ID、已声明TGA2 metadata/旧DevIL扩展，支持POT/NPOT。全零alpha32选择X8但POT基础alpha0字节保留；24位基础alphaFF；生成/重采样的X8未使用字节0。
- BMP BI_RGB24/32：core12、INFO40、52、56、OS2-64、V4-108、V5-124，正/负高度、行padding、真彩色biClrUsed RGBQUAD、文件gap及有界V5 post-raster profile。原版从真实DIB后加色表处读取像素，忽略bfOffBits实际偏移和色彩metadata；外层仍检查其边界。裸DIB、低位深、bitfields与压缩BMP未接通。
- BMP原版RVA143d2..14421在BGR24展开前以每X四字节扫描byte3，决定A8/X8；会读取行padding/下一行或有界尾数据。不能用stb合成alphaFF或最终RGBA判断descriptor。适配器只做原文件内相同扫描；若全零扫描继续将越过EOF，原版依赖未初始化堆字节，该域明确拒绝。BMP24基础仍存alphaFF，BMP32基础保留源alpha；确定性BMP24语料显式提供零尾扫描存储，保留全部形状/字段/断言，并另验越界拒绝。静态依据见`evidence/texture/native-public-static/20261030/`。
- 原LoadImageA随后对format与`0xfffffe3f`相与，目标`1a200006`、flags12、threshold0；矩阵同时核对该逻辑视图和完整端序物理视图。上传/消费保留LockRect→Copy→Unlock→引用→最终VM释放。
- BOX双轴均>=2为整数快速路径；退化轴重复采样并依序float32累加、quarter存储，再按x87向零缩放/半偏置。不能统一为整数平均或ties-to-even。

## 实际复用的公开源码

`PublicDirectXTex`固定 `microsoft/DirectXTex@ec66b78b37389e8373a8e14ccc3cd29be94146b3`，实际采用CreateTriangleFilter与ResizeTriangleFilter的核、回绕、scatter、active-row计数/复用和完成行写出；适配原x87浮点存储、归一化、蛇形有序抖动及X8 packing。不是仅阅读后另写替代算法。

`PublicStbBmp`固定 `nothings/stb@003501f6d7a8668ce8b6d875e34dd97ad0d5d5b5`，实际使用stb像素/通道/方向/padding解码；宿主归一化DIB容器以适配原像素起点及metadata契约，通用开关保留全零alpha物理字节。上游完整文件、通知、补丁说明和修改前后SHA均保留，Windows检出用-text固定字节，验收预检逐一核对。禁止回调原/既有恢复编码器或样本查表。

## 当前验收与保存

业务源 `d0a31471906307690863e24784ca7384ef2371bf` / [41006229811](https://github.com/wu162/NewSdk/actions/runs/41006229811) completed/success。D/R各6860构造、14606文件（14501支持成功、105原版成功压缩边界），另13拒绝，独立NoOriginal全量回放；5665新增NPOT和6152 BMP。完整52字节header、全部主/secondary VM和padding/tail空隙、每层descriptor/layout/三锁定视图、两完整RGBA消费视图、AddRef/Release/最终VM释放全部逐字节比较。

每配置完整trace634070359字节，SHA256 `53565024fd9fb747a029a8f0f0ae0e2c2927de55a5b20ffd255d42695822b68d`。流式捕获/逐条回放仍核对完整label/length/payload和最终EOF，避免x86累计全部trace导致内存耗尽。归档四原始成员SHA与12份I386 DLL指纹已独立审计，见`evidence/texture/handoffs/20261030-native-public/acceptance.json`。

本次补native-loader、复用63组；base保留34946af5/41005120850完整成功结果，该首轮整体failure保留。普通合并710bae12进入恢复分支；生产六DLL/SDK输入未改，无SDK/MOD或收尾plan。上一原生raw加载验收5fc678b0/40999035840及失败记录原样保留，不把旧范围升级为本轮证据。

## 未恢复与下一入口

正式loader仍拒绝所有压缩DDS，包括已有2x2/1x1尾层重新编码、小base扩展、缺层重压缩；105原版成功资源另存完整诊断，绝不能算匹配拒绝或压缩验收。其1x1输入原生base是4x4、levels3，不能直接复制DDS头/基础块推定成功。

下一独立候选 `validation/texture-public-d3dx-bc` 已有公开DirectXTex BC1/2/3核心、float块/图像ABI和POT base双轴>=4的完整owned DDS资源/mip消费；原flags/精度/端点/索引、partial打包与实际压缩→raw消费尚待适配和完整验收，不被正式loader/生产包调用。定向诊断入口`InspectTexturePublicBc.ps1`只取证，不等于组件/NoOriginal/SDK完成。

更多DDS布局、PNG/JPEG/PPM、低位深/灰度/调色板TGA、GPU/渲染/体积/数组/cube原生文件加载仍未接通。继续优先复用公开源码，稳定后按真实影响集中验收；新ABI进入生产包前另取适用SDK证据，未变组及完整MOD不按每轮重复跑。最新状态见[RESUME](RESUME.md)。
