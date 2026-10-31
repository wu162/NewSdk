# 当前：内建 Texture 插件（完整验收中）

基线 `recovery/legacy-sdk-cache` / `074bf768`；分支 `feat/texture-plugin`，
[PR #39](https://github.com/wu162/NewSdk/pull/39)。Texture/OnDemand/Atlas 编入 EXE，
运行目录为 EXE、config、ANTLR 三文件。PNG/TGA/BMP/JPEG/DDS 和七种 schema
输出、mip、CDATA、图集已接入；DXT1/1A 为新增通用编码器，标准 indexed BMP
由独立调色板解码器读取。

当前生产/测试源码 `4c74bf64b`，
[运行41180859852](https://github.com/wu162/NewSdk/actions/runs/41180859852) 进行中。
当前Windows常见格式D/R各31例及12个真实CLI用例通过，原收据已下载复算入库。
Core/原三路纹理/CLI/缓存/BIG通过；完整mymod和独立三文件ZIP步骤成功，最后
Upheaval及整个运行终态/完整归档复算仍待完成，未标整体结束。原失败均留档。

范围、来源、身份对齐和失败记录见 [TEXTURE_PLUGIN](docs/TEXTURE_PLUGIN.md)。
下一步：跟进运行、集中修复全部独立失败，验收成功后归档并更新PR。
保留原SDK DLL作隔离oracle，生产不加载。未经用户批准不合并，禁止强推。
其他范围见 [REFACTOR_RESUME](REFACTOR_RESUME.md) 和此前git续接记录。
