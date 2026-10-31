# 内建 Texture 插件

实现分支 `feat/texture-plugin` 从用户指定的 `recovery/legacy-sdk-cache` 建立，
[PR #39](https://github.com/wu162/NewSdk/pull/39)；参考
`recovery/texturecompiler-source` 的 `b30dffac5aeb027cf37a6dac0cd47beafe9fbea0`。

`Texture`、`OnDemandTexture`、`TerrainTextureAtlas` 直接编入 EXE。
运行目录只有 `NewModCompiler.exe`、`NewModCompiler.exe.config` 和
`antlr.runtime.dll`，不再生成或加载原纹理 DLL、补丁清单或独立纹理 codec DLL。
原 `SDK/tools` 二进制保留为隔离对照输入。纹理处理仍在 owner 线程执行；S3
源码有共享状态，未新增 worker 资格。

| 输入 | 处理 |
| --- | --- |
| DDS / DDS 大写扩展名 | 原字节直通，保留已有 cube / volume DDS |
| TGA | true-color、灰度、索引和 RLE |
| PNG / BMP / DIB | 恢复版解码器优先；其他标准变体经 Windows GDI+ |
| JPEG | Windows GDI+ |

输出支持 SDK schema 中全部七个格式：`DXT1`、`DXT1A`、`DXT3`、`DXT5`、
`A8R8G8B8`、`X8R8G8B8`、`R5G6B5`，支持 mip 链、水平切片 volume、
OnDemand CDATA 及原 atlas 布局。NPOT 按恢复版规则向上调整为 2 的幂；
保留默认 DXT1 的 alpha 格式推断。栅格 cube 保留原拒绝行为，使用预制 DDS。
GIF/TIFF 首帧和 PPM 为额外输入能力，尚未纳入本轮主要验收矩阵。

DXT3/5 链接参考分支的公开 S3 源码及其兼容适配；DXT1/1A 使用新增通用 BC1
编码器。新 BC1 和 JPEG/GDI+ 扩展输入不承诺与旧 nvdxt/IJG 编码字节相同。
恢复版 DDS/raw/mip/图集/CDATA 布局继续接受原版差分；具体结果以验收收据为准。
来源、源文件指纹及原代码修改记录见
[源码说明](../src/Plugin/Texture/README.md) 和相邻 `SOURCE.json`。

插件版本改为 `4300`，三个 ProcessingHash 分别对原值异或 `0x54580001`，
使旧纹理缓存失效；运行时 TypeHash 和资产布局保持原合同。完整流对照的隔离
原版进程通过测试专用 wrapper 对齐这两个有意改变的身份字段，仍由固定原 DLL
执行 `ProcessInstance`，四流和 CDATA 原字节比较不作归一化。wrapper、catalog、
原 DLL 及 Core 的身份均记录在独立收据中，未写入 SDK 或生产产品。

## 验证入口

- `TestCompilerTexture.ps1`：三种路由的原版三缓冲/CDATA/错误对照，失败后复用。
- `TestTextureCommonFormats.ps1`：常见输入、独立 DDS/BC 解码、全部 mip 层、
  中文路径、DDS 直通和真实七资产 CLI 冷热/单图失效/独立重建/缺图重试。
- Core lifecycle 工作流：Debug/Release、新生产对象、CLI/取消、缓存、完整
  mymod 原版差分和独立三文件 ZIP 验收。

当前验收运行：[41178734603](https://github.com/wu162/NewSdk/actions/runs/41178734603)，
源码 `20a367c0d`，运行中。DXT1A mip 的半透明边界另有待验收修正；未将构建预检
表述为组件、完整 SDK 或游戏内验证通过。
