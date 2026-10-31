# 当前：内建 Texture 插件（验收通过，PR #39 待审）

基线 `recovery/legacy-sdk-cache` / `074bf768`；分支 `feat/texture-plugin`，
[PR #39](https://github.com/wu162/NewSdk/pull/39)。Texture/OnDemand/Atlas 编入 EXE，
正式运行目录为 EXE、config、ANTLR 三文件。PNG/TGA/BMP/JPEG/DDS 和七种 schema
输出、mip、CDATA、图集已接入；DXT1/1A 为新增通用编码器，标准 indexed BMP
由调色板解码器读取。新 BC1/JPEG/GDI+ 不承诺旧编码器字节相同，未新增游戏内验收。

生产/测试源码 `4c74bf64b`，
[Windows 41180859852](https://github.com/wu162/NewSdk/actions/runs/41180859852) 整体 success：
D/R各31格式用例、12真实CLI缓存/失效/重试，原三路纹理、Core/CLI/取消、64后端、
完整缓存/SDK mymod/四流/BIG/SKUDEF、两个独立三文件ZIP及Upheaval22样本通过。
原归档、六份运行文件和两个ZIP均已下载复算；后续仅文档/收据。

范围、来源、身份对齐、原失败及成功收据见 [TEXTURE_PLUGIN](docs/TEXTURE_PLUGIN.md)。
[正式产物归档](https://github.com/wu162/NewSdk/actions/runs/41180859852/artifacts/13035436860)
的 `artifacts/final-release/41180859852-1/` 提供两个ZIP及清单。
保留原SDK DLL作隔离oracle，生产不加载。未经用户批准不合并，禁止强推。
其他范围见 [REFACTOR_RESUME](REFACTOR_RESUME.md) 和此前git续接记录。
