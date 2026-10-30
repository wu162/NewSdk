# PC原生资源与文件加载

入口：`src/TextureCompiler/Native/TextureNativeLoaderApi.{h,cpp,def}`；独立x86 `Recovered.TextureNativeLoader.dll`，通过`BuildTextureNativeLoader.ps1`构建。依赖已恢复Resource/Surface/Copy ABI；设备和描述符由实际消费者联合使用。**未进入生产六DLL包，也不代表完整混合模式身份恢复。**

## 当前实现

- `TextureCreateCpuTexture(width,height,levels,format)`：普通、linear、单面二维资源；POT/NPOT尺寸，levels0为完整链，也可指定受限层数。52字节GlobalAlloc对象和VirtualAlloc主/secondary存储遵守原header/引用/释放协议；small mip在原packed-tail布局中存放，四端序标签保留。tiled、sub-byte、无效尺寸/层数明确拒绝。
- `TextureCreateTextureFromFileA(device,path,output)`：非渲染type1设备；CreateFileA的ANSI路径、只读/共享读/OPEN_EXISTING/FILE_FLAG_RANDOM_ACCESS，有界读取不超过256MiB。成功资源由调用者使用`TextureResourceRelease`释放，失败不转交部分资源；原版缺文件HRESULT为88760b59。
- 文件范围：A8/X8 raw32 DDS已有部分或完整mip链，缺层按原BOX补齐；POT TGA24/32 raw/RLE、ID、四个原点方向、已声明TGA2 metadata/旧DevIL扩展。
- 上传调用已有LockRect/TextureCopySurface/UnlockRect，实际消费者继续核对AddRef/Release及最终VM释放。CPU上传保留文件小端字节；原LoadImageA随后对描述符format与`0xfffffe3f`相与、再以目标`1a200006`、flags`12`、threshold0复制。带端序标签的物理视图另外比较。
- 全零alpha的32位TGA选择X8，基础存储仍保留alpha0；24位TGA基础alphaFF；生成mip的X8未使用字节0。
- BOX双轴均≥2时为整数快速路径；任一轴为1时为归一化float行接口，重复退化轴、三次顺序float32加法/乘四分之一，再经x87向零的缩放与半偏置存储。不可统一为整数平均或一般ties-to-even。

## 验收入口与边界

`native-loader`组（`TestTextureNativeLoader.ps1`、`TextureNativeLoaderOracle.cpp`）保留6860个构造、2789个文件输入（2684个支持范围成功对照、105个原版成功但需压缩重编码的明确未支持边界）、另5个明确拒绝，并检查固定总数；完整52字节header、全部主/secondary VM字节（含padding与tail间隙）、所有mip布局/描述符、每层三个锁定视图、raw两个完整RGBA视图、引用及最终释放；Debug/Release、独立NoOriginal和完整capture SHA。原版fingerprint为`4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e`。

本组尚在失败修复后的集中补验，当前source/run与是否通过统一看[RESUME](RESUME.md)，不可引用历史失败作完成。静态证据在`evidence/texture/native-loader-static/20261030/`，脚本只读取/反汇编原DLL，不执行编译器。

未恢复：NPOT文件重采样、全部压缩DDS文件加载（包含已提供的2×2/1×1尾mip重新编码、基础轴小于4的扩展/重压缩、缺失mip重压缩）、更多DDS布局、PNG/JPEG/BMP/DIB/PPM与低位深/调色板TGA、GPU/渲染/体积/数组/cube文件路径。构造NPOT资源不等于恢复NPOT文件加载。12个真实代表输入全部保留：raw DDS与3个TGA共4个进入完整成功差分；8个压缩DDS明确拒绝。矩阵中的105个原版成功压缩资源完整存储另存独立归档，绝不把恢复版拒绝称为原版拒绝匹配。该归档为后续重编码取证，不认证压缩业务完成，也不把其可能的不确定字节冻成业务gold。

下一批优先接入已独立验收的8位NPOT权重/精度或实际消费所需raster格式，保留原生格式和存储精度；压缩缺层不能套用nvDDS压缩输出冒充D3DX一致。正式接入生产包前另取必要SDK集成证据，普通检查点和每轮收尾不触发完整MOD。
