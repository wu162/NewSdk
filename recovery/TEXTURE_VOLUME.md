# 体积条带与 RGBAMipMappedVolumeMap 恢复

实现：`src/TextureCompiler/Managed/TextureVolume.cs`。验证宿主：`recovery/tests/TextureVolumeOracle.cs`。本组是纯 IL 的图像分片及 vector 逻辑恢复，不包含原 nvDDS 体积压缩器，也不包含 mip 滤波生成。

原证据：`recovery/evidence/texture/runs/40191971356-1/decompiled/-Module-.cs`，DLL SHA256 `4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e`。

| MethodDef | 原方法 | 行为 |
| --- | --- | --- |
| 06000003 | RGBAMipMappedVolumeMap.resize | 分配mip/slice及RGBA数组，更新尺寸 |
| 06000004 | RGBAMipMappedVolumeMap.{dtor} | 销毁嵌套图像及vector |
| 0600001D | CreateVolumeRGBAImage | 从横向正方形条带拷贝各slice |
| 0600002B | nvVector<nvImage<rgba_t>>.resize | slice vector增缩及析构 |
| 0600002F | nvVector<nvVector<nvImage<rgba_t>>>.resize | mip vector增缩及析构 |
| 06000035 | nvVector<rgba_t>.resize | 像素vector增缩，不初始化新增像素 |

## 精确条带算法

CreateVolumeRGBAImage首先执行unsigned `width % height`。余数非0时抛原BadTextureFile及原消息；检查发生在目标resize及像素读取之前。height=0因此产生DivideByZeroException，不改成另一种纹理错误。

合法条带生成 **恰好1个mip**，每slice宽高均为输入height，depth=width/height。不要求二幂，不取方根，不沿纵向切片，不反转slice顺序。输入0×H（H>0）产生1级0slice；输入W×0仍在取模阶段失败。

原拷贝循环依次为slice z、x、y：

- source索引 `y*sourceWidth + z*sourceHeight + x`。
- destination索引 `y*sourceHeight + x`。
- 每像素原样复制R/G/B/A四字节，不预乘alpha、不变换通道。

恢复版RGBA数组每项uint，在little-endian内存中对应R/G/B/A字节，所有位原样保留。

## resize与mip层级

mipCount=0时自动计算 `bitLength(max(width,height,depth))`。三维全0得到0级；显式mipCount不因尺寸较小而缩短。每一层先用当前width/height/depth分配，然后分别执行 `max(value>>1,1)`，故第一层可以包含0维度或0slice，之后维度变成至少1。resize只分配/保留存储，不计算后续层的像素内容。

width×height使用原unsigned32位乘法。Resize只更新图像维度及wrapped pixelCount；例如width=0x80000001、height=2时像素count为2，而不是抛算术溢出。恢复版保留这个可安全观察的逻辑行为。

三级vector的capacity规则相同：newCount=0保留原capacity；非0且 `newCount>capacity || newCount<=capacity>>1` 时重分配，旧capacity=0则newCapacity=newCount，否则newCapacity=newCount+(newCount>>2)。否则保留capacity。像素重分配保留旧、新allocated capacity交集的原始字节，不只保留旧logical count；因此缩小后再增长可能读回旧padding中的已写入像素。

Mip/slice vector减少logical size时析构删除的元素；再次增大时重新构造为空对象，不复活旧slice。恢复版保留这些逻辑保留/销毁语义和capacity数值，托管GC承担内存回收。Slice.Pixels是allocated backing array，PixelCount才是有效logical长度。

## 明确的安全边界

- 原像素vector新增元素没有初始化。恢复版新分配托管内存为0；不把原未初始化内存的偶然内容当作确定结果。差分只比较完整写入的strip像素和先显式填充后保留的resize字节。
- 原native vector的32位byte count/capacity溢出可能分配不足；恢复版对超过安全托管byte范围或capacity溢出的请求显式NotSupportedException，不分配缩短缓冲区后模拟原越界。
- 原strip索引的32位pointer运算不能把巨大的真实图像extent当成wrapped小数组。恢复版在合法取模检查之后，以64位验证完整source extent和slice extent，再检查输入storage。width0、height大于Int32.MaxValue仍是合法空slice路径，不因height²没有实际访问而误拒绝。
- 短/空指针source storage在原版是越界读取；恢复版显式拒绝。height0和不整倍数错误仍按原先后顺序发生，不先拿storage错误掩盖它们。
- 未定义的内存访问不在原版探针中执行；这些适配器拒绝边界有独立断言，不冒充原版定义行为差分。

## 验证宿主

宿主编译：

```
csc /nologo /unsafe /platform:x86 /r:Recovered.BinaryAssetBuilder.TextureCompiler.dll /out:TextureVolumeOracle.exe recovery/tests/TextureVolumeOracle.cs
TextureVolumeOracle.exe SDK/tools/BinaryAssetBuilder.TextureCompiler.dll
```

须使用与其他原混合模式探针相同的CLR4 legacy activation配置。原分配器经已经加载的module allocator thunks调用，malloc RVA0x4E74C、free RVA0x4E758，使用SHA256 guard，避免裸msvcr80 PInvoke引起SxS加载失败。原图像输入使用固定托管数组只读；目标由原resize分配并由原dtor释放。

矩阵包括23次连续resize（增长/减少mip与slice、清空再增长、pixel capacity边界、显式超出自然mip数、0维度和安全的uint乘法wrap），比较每层/每slice尺寸、logical count和capacity。每步先比较旧、新capacity交集中的已填像素，再对所有allocated capacity填入双方相同模式，检验后续保留行为。

条带矩阵覆盖height=0/1/2/3/4/5/7/8/16/31/32/63/64，depth=0/1/2/3/5/7，两种像素pattern；额外加入不能整除的width、0×uint.MaxValue空slice。目标预先填入非空volume，检验成功时resize和失败时不修改目标。所有成功slice逐像素比较，不抽样；错误比较类型及消息。单独断言超大extent、短storage、null storage显式拒绝和uint.MaxValue自动mip数。

本页不是成功收据，运行结论须绑定实际sourceCommit/runId，组件成功不能称为nvDDS或SDK体积纹理编译已通过。


2026-10-25第三轮补充：本页纯IL helper不变，独立TextureVolumeRawDdsEncoder/TextureVolumeMipEncoder已接入插件并取得PC/PS3真实SDK三组逐字节对照；精确范围、算法及锚点见[TEXTURE_PS3_VOLUME.md](TEXTURE_PS3_VOLUME.md)。
