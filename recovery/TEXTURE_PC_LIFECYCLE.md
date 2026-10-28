# PC 图像管线的资源生命周期审计

本页是对现有源码与固定原 IL 的静态审计，不是新增 native ABI 等价验收。生产SDK未安装恢复插件。输入指纹、原方法token/RVA和边界见 [审计清单](TEXTURE_PC_LIFECYCLE.json)。

原插件 Initialize 在模块全局建立设备兼容对象、共享 SimpleBuffer（初始64MiB）和原封装对象；ReInitialize仅写_HashCode/_Platform，不重新分配。Dispose(bool)及终结器最终走同一个全局释放体。原体仅显式清零两个设备全局，未把已释放的共享buffer指针置空；重复释放、多个实例共享全局以及重复Initialize不能假设安全。恢复托管插件不拥有这些全局资源，Dispose保留托管虚派发/终结抑制，不复制原内存破坏行为。

| 原PC消费 | 恢复消费与所有权 | 当前结论 |
| --- | --- | --- |
| LoadImageA → D3DXCreateTextureFromFileA、LockRect/XGCopySurface、UnlockRect/Resource.Release | 公共TGA/PNG/BMP解码只分配托管数组；JPEG每次独立jpeg_decompress_struct | 已声明像素/错误域有原版差分；不需创建原设备 |
| 原共享64MiB SimpleBuffer + options[410] writer userdata | 每次直接生成DDS字节数组并EmbedDds；OnDemand FileStream在using中关闭 | 已验收组件、9专用SDK与完整mymod/384资产扩展不依赖共享native buffer |
| 原二维/体积压缩对象与栈工作区 | 公开S3 RGB/alpha函数在调用栈上分配工作数据；固定/初始化网格数据留在native模块 | 每次颜色/alpha调用受同一ColourLock保护；默认DXT1未初始化模式仍拒绝 |
| 原JPEG/image资源 | IJG6b桥接查询header与实际解码两次，成功与错误longjmp分支均调用jpeg_destroy_decompress | 输入、输出仅调用期借用；不向托管方返回native分配；原JPEG组件/隔离/SDK证据复用 |
| 原TerrainTextureAtlas的packer、tile和Relo tracker | 每次CompileAtlas局部packer、托管Image/Tile/数组与自有relocation输出 | 完整atlas及SDK已有精确验收；没有长期持有文件句柄/原tracker指针 |

当前完整生产消费只需 `Recovered.BinaryAssetBuilder.TextureCompiler.dll`、`Recovered.TexturePublicS3.dll`、`Recovered.TextureJpeg.dll`，以及SDK原Core/Utility等公共依赖。独立还原颜色/alpha API的旧辅助DLL仍是那些独立API的依赖；不能将其称作已删除，但本轮384资产SDK只复制前述生产消费所需两份native helper且NoOriginal通过。JPEG未在该384资产输入里实际调用；其解码调用证明继续绑定已验收PcJpeg 3216资产profile。

原名为Direct3DCreate9的内部函数只返回1；创建设备和资源操作来自静态链接兼容层，原PE不导入系统d3d9/d3dx。不能以名称或两个常量facade的成功验收推断原整个设备实现已恢复，也不能为实现贴图产物而盲目引入系统GPU/COM副作用。

本轮关闭的是“已声明PC托管业务消费还需要哪些原native全局资源”的依赖不确定性。没有修改业务源码，因此复用现有成功矩阵。原native指针签名、任意反射native入口、设备实现的ABI替换、异常分配/释放路径、跨AppDomain并发及完整混合模式DLL身份兼容仍未验收；不将这些边界悄悄变成新支持。
