# 裸DIB与BMP真彩色表

2026-10-29（北京时间）。旧独立接口已通过`d0a4547e` / `40816168489`；当前新增公共loader/`.dib`插件与BMP真彩biClrUsed接入，组件和专用SDK验收待运行，不提前计为集成完成。只支持PC，生产SDK未替换。

原固定DLL dispatcher `0x1694B`在`0x16953`把裸DIB数据直接交给`0x13F3D`，与BMP去除14字节header后的parser相同；原字节/跳表/SHA见`evidence/texture/dib-native/20261029`。`0x13FE8..0x1400D`按biClrUsed跳过4字节RGBQUAD，真彩表不映射或变色。当前BMP直接跳过该表；DIB仅补私有14字节header，不删除表、不改变DIB-relative V5 profile指针，不修改调用方数组。

支持声明的12/40/52/56/64/108/124头及BI_RGB16/24/32、已恢复的RGB555/alpha/NPOT/gap/metadata。索引色保留原BadTextureFile；不确定扩展bitfields及默认DXT1/不透明DXT1A保持拒绝。V5 profile及表/raster长度保持有界，表长度乘法使用long。

`TestTextureDib.ps1`继续使用不可变2616 header gold、已验收1944 OS2 gold及306色表变体：每配置4866输入，4578实际原raw-DIB调用和新增306实际原BMP色表调用。恢复6入口为DIB Decode/Load、公共Decode/Load、BMP Decode/公共Decode；预期17100成功、10368原错误、1728主动拒绝，另216损坏表/overflow/planes安全拒绝，均完整比较且逐次NoOriginal。

`TestTextureDibImages.ps1`新增50304插件输入/配置及同量NoOriginal：288 raw、96透明、54 atlas、78索引源各BMP/bare-DIB封装，覆盖所有头、方向、gamma/ICC、0/1/3/17色表、POT/NPOT/1x1与无效volume、raw3/DXT3/5/透明DXT1A、完整mip、三buffer/full cdata/旧尾。预期3952原错误及96不确定bitfields明确拒绝；432 atlas必须真正成功，使用原packer需要的tga路径，但保持BMP或DIB payload。

`TestTextureDibSdk.ps1`专用PcDib：498 raw、166透明、108 atlas输入，排除索引和无效volume。三模式每模式21464实际资产，其中216成功atlas；完整5-DLL来源包、安装/回滚、原Core唯一ID、四产物及全部轨迹严格比较。通过集中工作流的`[texture-dib-sdk]`仅显式选择新sdk-PcDib，并照常自动选择未验收/受影响组件；不自动启动旧SDK或完整mymod。组件稳定后再启动此专用里程碑。当前结果以RESUME和实际收据为准。
