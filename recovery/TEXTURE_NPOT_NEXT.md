# PC NPOT 当前调查入口

仅PC，recovery/texturecompiler-source，禁止强推，编译测试只在GitHub Actions。已验收的DXT3/5二维/volume及mip锚点见RESUME.md；生产SDK未替换，当前NPOT仍明确拒绝。

原Plugin.ProcessTexture@0x06000106在PC无独立缩放分支，先调用LoadImageA@0x06000017（IL RVA0x3214），再IsValidTexture/GuessProperFormat和二维或volume压缩。LoadImageA调用D3DXCreateTextureFromFileA@0x060000E8（native RVA0x13C82），读取XGGetTextureDesc、LockRect，再以XGCopySurface目标0x1A200006/pitch=width*4转RGBA。静态来源：evidence/texture/runs/40191971356-1/{methods.json,decompiled/BinaryAssetBuilder.TextureCompiler/Plugin.cs}，原DLL SHA256固定4f559428...e9d9e。不能假定压缩器负责全部NPOT处理。

定向取证组npot-load：23组尺寸（19 NPOT与4 POT对照），TGA24/32八图案、PNG RGB/RGBA8交错/非交错四图案、BMP24/32正反向三图案，共1012记录。原LoadImageA仍使用本模块VC8 malloc/free及nvImage构造析构；核对token与thunk、输入不变、POT对照尺寸。保存原输入及加载后完整RGBA、尺寸/HasAlpha/错误，probe-only，不调用恢复NPOT。辅助恢复程序集只提供既有Image记录与fixture源码编译依赖。

收取后先核实目标尺寸、边缘/alpha/舍入及缩放所在层，必要时定位原native滤波。证据不足不启用近似算法，不以探针成功冒充支持。完整能力组需原LoadImageA差分、各实际图像解码消费者、raw/DXT二维及volume插件三缓冲区/full cdata、Debug/Release与NoOriginal、适用真实SDK三模式及四产物。支持子集必须明确界定，未知PNG16/索引/metadata等不能静默进入近似路径。

## LoadImageA取证已收取，numeric取证准备中

源码 `a19bb879704cd80ad55202760a160c8af57bbe2d` / [40528237009](https://github.com/wu162/NewSdk/actions/runs/40528237009) completed/success，1012全部解码，probe-only。POT对照尺寸不变，NPOT各轴向上取2幂。TGA3×1灰阶[0,127,255]加载为4×1 [48,79,175,207]，不能用clamp线性插值。native原清单40191971356 artifact已核对：D3DXCreateTextureFromFileA@0x13c82→0x13b53→0x13a9b→0x134b8；0x1359e/0x135c9分别向上取2幂，0x1360d默认filter=0x80004。默认0x19b3a调度到0x194fc（2D triangle），0x18ad8生成带回绕的面积积分权重；0x19704将X/Y float权重相乘存float，再每输入像素RGBA乘加存float，不能换成两pass量化近似。

定向npot-numeric继续采原0x18ad8贡献表位模式、常量/格式vtable和初始化后dither表，不重复1012加载样本。注意D3DX table allocator@0x12074/free@0x1208a使用进程堆，与nvImage的VC8 CRT malloc/free不同，不可混用。当前仍不启用恢复NPOT；量化与float精度必须完整对照后才能声明支持。
