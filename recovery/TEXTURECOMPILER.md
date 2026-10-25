# TextureCompiler 当前交接

2026-10-25。本轮从 `recovery/compiler-source` 的 `b122a54abe4405516b021e425b8fb9235d914a0e` 新建 `recovery/texturecompiler-source`。源码按用户要求全部放在根目录 [`src/TextureCompiler`](../src/TextureCompiler)，不是 `recovery/src`。测试/原始证据/交接仍在recovery。

**状态：部分能力恢复且组件验收通过，完整DLL尚未还原。** 没有替换生产SDK DLL/config、没有执行真实MOD SDK编译，也没有合入原分支。

原DLL SHA256：`4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e`。完整类型/方法/原IL/native反汇编和ILSpy工程：[`evidence/texture/runs/40191971356-1`](evidence/texture/runs/40191971356-1)，原始取证运行40191971356。反编译导出是证据，不等同于可构建、已恢复的完整业务源码。

## 已验收组件

| 能力 | 恢复实现 | 原版对照结果（Debug/Release各自） |
| --- | --- | --- |
| 三根元数据、版本、hash/platform状态 | Managed/Plugin.Metadata.cs、TextureCatalog.cs | 620项比较 |
| alpha分类、格式推断、尺寸检查、1644字节options完整初始化 | Managed/TextureImageRules.cs | 13,995条记录、474,314次检查，平台0/1/2/99，三组padding初值 |
| 内存图集排版、16px周期边框、normal alpha、原EASTL排序 | Managed/TexturePacker.cs | 64组、7,006项断言，比较全部atlas像素与坐标/重复ID次序，覆盖0..50数量边界 |
| TGA24/32 raw/RLE、方向、alpha、受限标准/DevIL扩展 | Managed/TextureTgaDecoder.cs | 1,675样例；1,421完整像素匹配、252精确拒绝匹配、2明确未支持；包含main全部3张TGA |
| SimpleBuffer、字节序及两项常量D3D facade | Native/TextureSupport.h/.cpp | 264,429次原native检查、633,920累计payload字节；/Od、/O2 |
| Texture/OnDemandTexture的DDS直通 | Managed/Plugin.Dds.cs | 2,085组、三缓冲区和完整cdata逐字节对照，PC/PS3/unknown，含9个main真实DDS |

组件成功收据：[`evidence/texture/validation/40194142827-1/validation.json`](evidence/texture/validation/40194142827-1/validation.json)，源码 `765cf26f418e4fe503603c9c7515ea16c8fc9e67`，运行 [40194142827](https://github.com/wu162/NewSdk/actions/runs/40194142827) attempt1成功。

元数据/image/packer的Debug及Release成功来自源码 `ece235ac79fd7216777073ea8cc9416dd0018f47`、运行40193344145；该运行整体因当时DDS枚举错误及native编译参数失败，不能写成整体成功。其每组成功日志/源指纹/原收据保存在 [`evidence/texture/validation/40193344145-1`](evidence/texture/validation/40193344145-1)。40194142827在核对祖先、原DLL/Core/Utility、项目、各组实现和oracle均未变化后精确复用这六组成功，另实际重跑修正后的DDS/native，不冒充所有组在新提交再次执行。

DDS恢复宿主不含原TextureCompiler DLL。图像规则/packing/native oracle是同进程原版差分，测试中加载原DLL，但恢复业务代码本身不加载或转发给它。只有OnDemandTexture写cdata；普通Texture与Atlas的metadata不设置HasCustomData。DDS不重新压缩，原版即把非空.dds完整保留；大小、四字节padding和PS3/unknown混合endianness保留原行为。OnDemandTexture的Type枚举为Standard=0、Volume=1、Cube/未知=2，必须与原真实输出核对，不能按猜测互换Standard/Cube。

## 真实main样本

来源main固定提交 `bacfc69fc3e71c8116d1f22b933b8b6173170ecf`。main的SDK/Mods发现115 DDS、30 TGA；选择各DDS FourCC/mipmap-count/caps2组合及TGA type/depth/descriptor/footer组合的最小代表，共9 DDS+3 TGA，4,023,271字节。保留原始blob，没有重编码/裁剪。

精确原路径、blob SHA、SHA256、字节数：[`tests/fixtures/texture-main/manifest.json`](tests/fixtures/texture-main/manifest.json)。不因文件名含`_vol`就断言其为volume；本轮DDS按实际头部字段分类。DDS样本已加入成功差分；TGA另有原像素probe和独立验收状态，不能混同。

## TGA阶段

独立源码 `Managed/TextureTgaDecoder.cs` 与 `tests/TextureTgaOracle.cs`。最终验收源码 `0cd42405f5b757c72383040d2f1ff820c31d29bf`，运行 [40195290786](https://github.com/wu162/NewSdk/actions/runs/40195290786) attempt1成功，收据位于 [`evidence/texture/tga/40195290786-1/validation.json`](evidence/texture/tga/40195290786-1/validation.json)。Debug/Release各1,675个完整保留样例：1,421组完整RGBA/hasAlpha匹配、252组原版拒绝的类型/消息/ErrorCode匹配、2个原版误读footer像素的异常边界明确未支持，不计入等价解码成功；mismatch=0。main三张真实TGA全部取得完整像素一致结果。

恢复24/32位真彩raw与按行RLE、方向/ID、全零alpha规则，以及有明确渲染字段约束的标准495字节扩展链和507字节DevIL方言。最后一种能解释真实AptUI文件，不能泛化为任意TGA扩展均支持。没有把原版跨行run读取footer作为像素的异常行为复制成正常支持；这2项显式边界及其他未支持格式继续拒绝。可通过 `new TexturePacker(TextureTgaDecoder.Load)` 显式使用，不替换默认codec，也不等同于完整TextureCompiler入口或压缩能力。

历史probe运行40194142937（源码765cf26f）有430初始差异/252原版失败/2真实extension未支持，只证明取证完成；初次受限验收40194716161（源码806db41）仍有4明确未支持。最终阶段新增64个metadata矩阵并支持两个真实扩展文件，取得上述独立最终成功；历史证据未改写成新成功。详情与DevIL固定源码链接见 [`TEXTURE_TGA.md`](TEXTURE_TGA.md)。

## 未还原范围与下一步

- 原LoadImageA的完整D3DX/XGraphics图像解码/格式转换，尤其PNG/JPEG、其他TGA类型及扩展metadata。
- `nvDDS::nvDXTcompress` 两重载、滤波/mipmap/resize及volume压缩本机核心。options已恢复不等于压缩器已恢复。
- XenonBundler完整2D/3D/DDS转换、XG surface/header/descriptor和纹理布局。
- 原Initialize/Dispose中的共享D3D/Xenon/64MiB buffer生命周期。托管插件仅恢复状态，不声称这些副作用已完成。
- TerrainTextureAtlas完整入口与codec接线；当前只恢复内存packer，不把它宣称为完整atlas编译。
- 未支持的Plugin业务入口明确抛NotSupportedException；不存在隐藏原DLL回退。正式SDK接线前须做完整组件与必要真实MOD编译，不把本轮组件成功冒充SDK验收。

网上实现调查见 [`TEXTURE_UPSTREAM.md`](TEXTURE_UPSTREAM.md)：旧NVIDIA头与ABI吻合但核心仍为.lib；MIT新NVTT可做不同后端，不能默认为原压缩块等价；Xenia BSD源码可复用360地址/布局，不是完整XG API。没有直接复制这些项目的源码或二进制。

## 运行与续接

Windows x86 + VS2022 /clr + .NET Framework4.8；`dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c Release`。`TestTexture.ps1`运行相关组件，按可追溯输入检查复用已成功未变组；TGA使用独立`TestTextureTga.ps1`。测试原版CLR2 mixed assembly时必须CLR4 legacy activation。CRT内存必须经原模块已解析malloc/free thunk，不能裸PInvoke msvcr80或用另一CRT释放。

本轮隔离了原零长度DDS异常路径的FileStream泄漏，测试不复用那个未关闭文件；恢复实现不复制异常资源泄漏。native /clr宿主需要/EHa，不能/EHsc。下轮不要重新触发未变化的已验收组；只对新增codec/接线及真实输入变化取得必要新结果。禁止强推。
