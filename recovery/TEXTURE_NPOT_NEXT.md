# PC NPOT 当前调查入口

仅PC，recovery/texturecompiler-source，禁止强推，编译测试只在GitHub Actions。已验收的DXT3/5二维/volume及mip锚点见RESUME.md；生产SDK未替换，当前NPOT仍明确拒绝。

原Plugin.ProcessTexture@0x06000106在PC无独立缩放分支，先调用LoadImageA@0x06000017（IL RVA0x3214），再IsValidTexture/GuessProperFormat和二维或volume压缩。LoadImageA调用D3DXCreateTextureFromFileA@0x060000E8（native RVA0x13C82），读取XGGetTextureDesc、LockRect，再以XGCopySurface目标0x1A200006/pitch=width*4转RGBA。静态来源：evidence/texture/runs/40191971356-1/{methods.json,decompiled/BinaryAssetBuilder.TextureCompiler/Plugin.cs}，原DLL SHA256固定4f559428...e9d9e。不能假定压缩器负责全部NPOT处理。

定向取证组npot-load：23组尺寸（19 NPOT与4 POT对照），TGA24/32八图案、PNG RGB/RGBA8交错/非交错四图案、BMP24/32正反向三图案，共1012记录。原LoadImageA仍使用本模块VC8 malloc/free及nvImage构造析构；核对token与thunk、输入不变、POT对照尺寸。保存原输入及加载后完整RGBA、尺寸/HasAlpha/错误，probe-only，不调用恢复NPOT。辅助恢复程序集只提供既有Image记录与fixture源码编译依赖。

收取后先核实目标尺寸、边缘/alpha/舍入及缩放所在层，必要时定位原native滤波。证据不足不启用近似算法，不以探针成功冒充支持。完整能力组需原LoadImageA差分、各实际图像解码消费者、raw/DXT二维及volume插件三缓冲区/full cdata、Debug/Release与NoOriginal、适用真实SDK三模式及四产物。支持子集必须明确界定，未知PNG16/索引/metadata等不能静默进入近似路径。
