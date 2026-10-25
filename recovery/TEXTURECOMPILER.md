# TextureCompiler 当前交接

2026-10-26。分支 `recovery/texturecompiler-source`，源码位于根目录 `src/TextureCompiler`。

## 唯一目标：Win32 / PC

用户要求移除非 PC 代码与规划。PS3 专属三份 mip、自动 cubic 缩放、平台输出分支及对应测试/取证/SDK 入口已删除；Xbox360 与 unknown 平台不再支持。Initialize/ReInitialize 和公共平台入口明确拒绝非 Win32。SDK 原 DLL、共享 Core 平台枚举、原始 IL 和历史证据不变；历史 PS3 验收不属于当前能力或待办。

## PC 已有能力

- Texture / OnDemandTexture 的 DDS 原字节直通、三缓冲区和完整 cdata。
- 声明范围内的 POT TGA / PNG / BMP 解码，原版错误与拒绝行为。
- A8R8G8B8、X8R8G8B8、R5G6B5 三种 raw DDS 输出。
- PC 二维 triangle 完整 mip，体积条带、raw 与递归 mip。
- 元数据、图像规则/1644 字节压缩选项、内存 atlas packer 和已恢复 native buffer。

删除前最终 PNG/BMP SDK 锚点：`9ba02841cf320083c6ec3c571744f3c4c36e401b` / [40235430314](https://github.com/wu162/NewSdk/actions/runs/40235430314)，其中 PcNoMip / PcMips 各 Original/Recovered/NoOriginal 三组、每组 600 资产/实际 Core 调用，四产物和 buffer/cdata 相同。TGA/mip/volume 的历史精确锚点保留在各能力文档；不冒充本轮新验证。

## 仍需完成（仅 PC）

1. DXT1 / DXT1A / DXT3 / DXT5 压缩核心。已有 options、原生入口与 PC 小块样本，尚无恢复版压缩器；见 [DXT 取证](TEXTURE_DXT_ENTRYPOINTS.md)。不再调查跨平台一致性。
2. NPOT D3DX 图片重采样，目前明确拒绝。
3. JPEG、尚未声明支持的 PNG ICC/APNG、TGA metadata/RLE 边界和其他 BMP 组合，按实际 PC 输入需求独立取证。
4. 完整 TerrainTextureAtlas 解码、处理、压缩及输出入口；目前只有内存 packer。
5. 审计 PC 实际需要的原 native 初始化/释放及公共图像依赖。保留共享 helper，不为已排除平台补实现。
6. 最终合入并接入新编译器。生产 SDK DLL/config 仍未替换，完整 DLL 尚未还原。

## 本轮验证

PC 专属清理的受影响组件及两个 PC SDK profile 待集中运行；本地静态检查和选择器结果见后续提交。不得把旧 PC/PS3 收据当作清理后源码的新验收。按 [TEXTURE_TESTING.md](TEXTURE_TESTING.md) 的单入口、至多一个 Windows 作业顺序运行；未变解码组保留原锚点，不频繁启动完整 SDK。

历史格式/数值边界：[PNG/BMP](TEXTURE_PNG_BMP.md)、[TGA](TEXTURE_TGA.md)、[raw DDS](TEXTURE_RAW_DDS.md)、[triangle mip](TEXTURE_TRIANGLE_MIPS.md)、[volume](TEXTURE_VOLUME.md)。历史 PS3 文档仅留作证据索引，不产生后续工作。
