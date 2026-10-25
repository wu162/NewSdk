# TextureCompiler 可复用公开实现核查（2026-10-25）

> **历史记录：自 2026-10-26 起仅以 Win32/PC 为目标。本文非 PC 能力、旧待办和旧平台矩阵不再适用；当前实现与计划以 [TEXTURECOMPILER.md](TEXTURECOMPILER.md) 为准。原 sourceCommit/runId 仅证明原验收版本，不代表本轮删除后的代码已验证。**

本轮按用户要求阅读公开源码，以下为实际适配判断；没有把仅提供头文件或静态库的项目误记为压缩核心源码。当前恢复实现仍以原DLL的IL/native差分为准，未直接引入下列第三方代码或二进制。

## 旧 NVIDIA DDS Utilities：准确 ABI 依据，核心未开源

- 官方入口：https://developer.nvidia.com/legacy-texture-tools
- 旧工具包镜像固定提交：https://github.com/Stehfyn/Legacy_DDS_Utilities/tree/f0467589621354150acf2c623cb5cad44c8f35c1
- `LIBS/inc/dds/nvdxt_options.h`、`ddsTypes.h`、`tPixel.h` 和 `LIBS/inc/dxtlib/dxtlib.h` 包含原代 `nvCompressionOptions`、`nvImage<rgba_t>`、`RGBAImage`、`RGBAMipMappedVolumeMap` 和 `nvDDS::nvDXTcompress` 重载。
- x86 4字节对齐的 options 大小1644，与原DLL布局吻合：quality@0，normalMap@24，rescale@60，mipMapGeneration@100，mipFilter@108，custom_filter_data@244，sharpenFilter@440，textureType@472，textureFormat@476，paletteSize@480，palette[256]@484，forceDXT1@1632，RGBE@1633，user_data@1640。
- **偏移0的71是kQualityProduction，不是库版本。** 原IL构造默认值400、0.3086/0.6094/0.082等也与旧头对应。不能由这些匹配断言压缩核心同版本或输出相同。
- `LIBS/lib` 提供VC7/VC8预编译 `.lib`；未找到 `nvDXTcompress` 压缩核心源码。旧包 `License.pdf` 是 NVIDIA 自有协议，不能使用现代NVTT的MIT许可替代它。

因此可用它核对原类型、枚举和默认值；不能把“下载旧静态库并链接”记为“已经还原压缩源码”。

## 新 NVTT：可编入EXE的新后端，但非旧算法等价证明

- https://github.com/castano/nvidia-texture-tools
- 许可：https://github.com/castano/nvidia-texture-tools/blob/master/LICENSE （MIT，保留通知）
- `src/nvtt/CompressorDX9.cpp`、`QuickCompressDXT.cpp`、`OptimalCompressDXT.cpp` 为真正压缩实现，接口为 `nvtt::Compressor/InputOptions/CompressionOptions`。

可作为明确选择的新压缩后端，静态链接进最终程序；其WeightedClusterFit/QuickCompress等实现和原 `nvDDS` API/选项不同。同样支持DXT1/3/5不代表DDS头、mipmap、滤波、端点选择、alpha阈值和压缩块逐字节相同。启用前应针对这些差异跑原版对照，不能静默替换。

## 解码：stb / DirectXTex 可用，但需逐像素验证

- https://github.com/nothings/stb/blob/master/stb_image.h ，许可MIT或Unlicense：https://github.com/nothings/stb/blob/master/LICENSE
- https://github.com/microsoft/DirectXTex/blob/main/DirectXTex/DirectXTexTGA.cpp
- https://github.com/microsoft/DirectXTex/blob/main/DirectXTex/DirectXTexWIC.cpp
- DirectXTex MIT许可：https://github.com/microsoft/DirectXTex/blob/main/LICENSE

原 `LoadImageA` 实际调用静态链接D3DX/XGraphics：`D3DXCreateTextureFromFileA` → `XGGetTextureDesc` → format判断hasAlpha → LockRect → `XGCopySurface`（destination=0x1A200006、pitch=width*4、flags=18）→ Unlock/Release。

stb的16位TGA最高位alpha、TGA水平方向和JPEG IDCT语义与旧库未证明相同；DirectXTex默认将全零TGA alpha改255，需要 `TGA_FLAGS_ALLOW_ALL_ZERO_ALPHA` 等选项核对。强制4通道后有alpha字节，不能直接等同于原hasAlpha。PNG gamma/palette/tRNS/16bit、JPEG gray/RGB/CMYK/progressive均应独立验收。原DLL包含IJG6b指纹，不能凭使用相同文件格式推定现代解码逐像素一致。

## Xbox360 历史调研（已排除出还原目标）

2026-10-25用户要求剔除Xbox 360；本节仅保留已完成调研的来源，不再安排Xenia/Xenon专属实现或验收任务，不作为完成阻塞。

固定提交：https://github.com/xenia-project/xenia/tree/95a5c3ee250f80c3b9d139658649d9ffb6db3eec

- `src/xenia/gpu/texture_address.h`：Tiled2D/Tiled3D坐标到块地址，可用于linear→tiled写入，不仅能用于读取。
- `src/xenia/gpu/texture_util.h/.cc`：GetPackedMipOffset/GetGuestTextureLayout，packed mip tail、pitch/slice/4KB alignment。
- BSD-3-Clause，复用须保留通知。

可减少Xenon布局重建工作，但并非 `XGCopySurface`、`XGGetTextureDesc` 或完整 `XenonBundler.ProcessDDS`。还须还原format/endian映射、DDS mip/face顺序、descriptor/header、padding及最终两块输出；像素相同不能代替完整输出字节相同。

## 相近SAGE工具与错误线索

https://github.com/jonwil/sagetools/blob/49373be5e922e8d7ae0b2caa2c74fa6c64d6cf4c/texture/TextureCompress.cpp

该GPL-3.0+项目有高度对应的纹理wrapper、alpha判断、options、volume压缩流程，但仍include `xgraphics.h` 并依赖仓库的 `d3d/lib/xgraphics.lib`。可研究业务流程，不能记为开源XG压缩/转换实现。本轮未复制该项目源码。

`ChimpsAtSea/Reach` 中相关API文件只是注释符号，未找到函数体；Microsoft `xtexconv/XboxDDSTextureLoader` 针对Xbox One，不能用来替代Xbox360布局。

## 后续入口

仅推进 Win32/PC 所需 DXT、图像读取和公共处理；当前能力及待办见 [TEXTURECOMPILER.md](TEXTURECOMPILER.md)。历史 Xbox/PS3 参考资料不产生实现或验收任务。
