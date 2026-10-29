# 独立裸 DIB 加载接口

2026-10-29（北京时间）。`TextureDibDecoder.IsDib/Decode/Load`是独立接口，本轮未改变公共`TextureImageLoader`或插件文件扩展名路由。验收终态以RESUME及dib组收据为准，不能将接口组件通过称为DIB XML/SDK接入完成。

原固定DLL的native dispatcher `0x16804..0x16A9F`依次尝试9种格式，最后的format6分支`0x1694B`在`0x16953`把原数据地址/大小直接传给`0x13F3D`，正是BMP wrapper跳过14字节后调用的DIB parser。完整函数字节、跳表和SHA在`evidence/texture/dib-native/20261029`；复现用`InspectTextureDibNative.py`，仅静态读取，不执行DLL。

接口用私有14字节BMP envelope复用已有BI_RGB、RGB555、alpha、NPOT、错误、V4/V5 metadata及有界V5 profile处理。DIB-relative profile指针无需平移；调用方数组不修改。不引入标准bitfields替代原未初始化mask语义。已声明DIB revision为12/40/52/56/64/108/124；原parser 0x13FE8..0x1400D对真彩同样按biClrUsed跳过4-byte RGBQUAD；新接口在私有envelope移除该表并将biClrUsed清零，V5 profile指针同步减去表长，保持实际像素/原ICC相对位置。306个新表长1/3/17变体覆盖INFO/V2/V3/OS2/V4/V5真彩、方向及gamma/ICC。BMP公共decoder本身的非零真彩biClrUsed仍是下一独立扩展，不能把新DIB私有适配算到BMP路由。

`TestTextureDib.ps1`使用既有2616 BMP header gold及当前OS2直接语料1944输入，去掉真实BMP file header，并新增306个真彩色表变体，形成4866裸DIB输入。每配置实际调用4578次原LoadImageA（另1初始化正对照），完整比较原raw-DIB与原BMP的RGBA/尺寸/alpha/错误，再验恢复数组/文件两入口；288不确定bitfields不调用原版，明确拒绝。独立无原DLL进程每次调用前后检查隔离，并比较完整输入/输出/错误及完整transcript。

预期每配置两入口5700成功解码、3456原错误匹配、576主动拒绝；拒绝与成功分开计数。复用旧gold并不等于只假定raw-DIB与BMP相同：非拒绝域均新增实际原raw-DIB调用证明。新接口未被现有插件消费，依赖表单独登记，未变成功组不重跑。

下一步在接口验收后接入`TextureImageLoader.Decode`的DIB识别及`Plugin.Images.cs`的`.dib`文件路由，再集中取得受影响完整插件/raw/DXT/mip/volume/atlas/NoOriginal和必要SDK证据。正式接入前保留当前插件拒绝，完整MOD仍按集成里程碑降频。
