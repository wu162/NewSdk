# TexturePacker IL 恢复

源码：`src/TextureCompiler/Managed/TexturePacker.cs`。输入为内存 RGBA Image，或由显式 ImageLoader 委托提供图像；默认文件解码仍未恢复。此类不依赖原 TextureCompiler DLL。下面的算法来自原 DLL 的 IL，不是通用装箱器近似。

原始证据：`recovery/evidence/texture/runs/40191971356-1/decompiled/-Module-.cs` 的 TexturePacker 相关方法，原 DLL SHA256 `4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e`。

## 布局与方法

x86 原布局：TexturePacker 大小28字节，+0 base atlas指针、+4 normal atlas指针、+8/+12/+16 EASTL vector begin/end/capacity、+24 packed byte、+25 hasNormals byte。Texture大小20字节，+0 base image、+4 normal image、+8 uint ID、+12/+14 upper-left ushort X/Y、+16/+18 bottom-right ushort X/Y。nvImage大小24字节，+0/+4 width/height、+8 pixel pointer、+12 vector size、+16 capacity、+20额外状态。RGBA像素每项4字节，内存顺序R/G/B/A。

| Token | 方法 |
| --- | --- |
| 06000022 | AddTexture |
| 06000023 | TextureSort |
| 06000024 | PackTextures |
| 06000025 | CopyTexture |
| 06000026 | MergeTextures |

AddTexture在已packed时直接返回；base文件不存在时也直接返回。检查两个路径转小写后是否 **包含** `tga`，不是检查扩展名，也不是先检查normal文件存在。base尺寸必须256×256；没有alpha的base像素alpha清零。normal文件存在则加载并把全部像素alpha设255，同时置hasNormals=true。原代码没有校验normal尺寸。

PackTextures先置packed=true，再按数量选择尺寸；超过49抛BadTextureFile但packed仍为true：

| Tile数量 | Atlas尺寸 |
| --- | --- |
| 0–1 | 512×512 |
| 2–3 | 1024×512 |
| 4–7 | 2048×512 |
| 8–9 | 1024×1024 |
| 10–21 | 2048×1024 |
| 22–49 | 2048×2048 |

先MergeTextures，后排序元数据，再释放所有tile的source images。不能把排序提前，否则XML输入顺序对应的坐标会改变。

MergeTextures将atlas像素清零；按原输入顺序从(16,16)放置tile，横向步长288；更新x后若 `x+288 > atlasWidth` 则换行至x=16、y+=288，若 `y+288 > atlasHeight` 则结束。坐标的bottom-right为upper-left加base实际宽/高，不包含边框。只有hasNormals=true才创建normal atlas，没有normal的tile区域保持全零。

CopyTexture复制 `(srcWidth+32) × (srcHeight+32)` 矩形，源坐标u=(x−16)/width、v=(y−16)/height。GetPixel先周期wrap u/v，再乘宽高并向零取整；16px边框来自对侧纹理，不是边缘像素延伸。保留原double运算和x外层/y内层遍历顺序。

TextureSort比较uint ID，不使用int比较。原EASTL quick_sort使用28项阈值、median-of-three pivot、原partition交换规则、深度上限 `2*bitLength(count)-2`、原make_heap/adjust_heap/promote_heap/sort_heap回退和两段insertion sort。恢复实现逐项移植这些步骤，保留重复ID的不稳定次序；不以Array.Sort或稳定排序替代。

## 明确边界

- 原LoadImageA文件解码和后续nvDXT压缩未由此类恢复。注入loader是显式依赖边界，默认不偷调原DLL。
- 原ctor没有初始化+25 hasNormals；没有加载normal时原始未初始化内存并非确定契约。恢复实现安全默认false；差分宿主把原结构同字节置零，仅验证该受控输入。不会把无初始化内存偶发值视为功能要求。
- 原重复PackTextures会在source已释放后再解引用；恢复版显式拒绝。原CopyTexture越界写和GetPixel负整数导致的越界不属于确定安全行为；恢复版显式拒绝。normal尺寸没有任意强改为256，但若实际复制越界则拒绝。
- 原native分配、释放及异常泄漏不复制到托管实现。成功打包后managed tile也解除source引用；atlas和坐标保持可读。

## 差分宿主

`recovery/tests/TexturePackerOracle.cs`，编译示例：

```
csc /nologo /unsafe /platform:x86 /r:Recovered.BinaryAssetBuilder.TextureCompiler.dll /out:TexturePackerOracle.exe recovery/tests/TexturePackerOracle.cs
TexturePackerOracle.exe SDK/tools/BinaryAssetBuilder.TextureCompiler.dll
```

宿主config须启用CLR4 legacy CLR2混合模式激活，与仓库其他原DLL探针相同；需要原DLL使用的VC80 CRT。宿主只引用恢复程序集，反射调用原 `<Module>` 的 TexturePacker.PackTextures 和dtor。原图像/像素/tile/vector用MSVCR80 malloc分配，使原IL释放它们时使用相同CRT，避免混用CLR分配器。无需初始化D3D。

64个样例覆盖数量0、1、2、3、4、7、8、9、10、21、22、28、29、32、49、50；每组包括反序ID、重复ID、全等uint.MaxValue、混合0/0x80000000/uint.MaxValue，以及有/无normal。比较返回/异常、packed状态、全部atlas像素、每项ID和四坐标、原source释放、打包后Add no-op。重ID结果可检出不等价排序；含坐标像素模式可检出边框clamp或提前排序。测试不冒充文件解码、native生命周期、压缩或SDK验收。

此文档不是运行成功收据；以实际运行sourceCommit、runId和测试结果为准。
