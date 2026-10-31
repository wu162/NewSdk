# 当前：内建 Texture 插件（Windows 验收中）

用户指定 `recovery/legacy-sdk-cache`；成果分支 `feat/texture-plugin`，
[PR #39](https://github.com/wu162/NewSdk/pull/39)。Texture/OnDemand/Atlas 直接编入 EXE，
运行目录变为 EXE、config、ANTLR 三文件。常见 PNG/TGA/BMP/JPEG/DDS 输入和
七种 schema 输出格式、mip、CDATA、图集已接入；DXT1/1A 为新增通用编码器。

实现检查点 `88ecda898`；Windows 验收源码 `20a367c0d`，
[运行41178734603](https://github.com/wu162/NewSdk/actions/runs/41178734603) 进行中。
本地托管源/测试编译、PS 语法、来源/fixture/XSD 及17项证据预检通过，
尚未取得 Windows 组件/SDK/独立 ZIP 成功结果。DXT1A mip 半透明边界另有待验收修正。

范围、原版身份对齐方法和验证入口见 [TEXTURE_PLUGIN](docs/TEXTURE_PLUGIN.md)。
下一步：跟进 Windows 编译及全部回归，集中修复失败后重新验收并归档。
保留原 SDK DLL 作隔离 oracle；不加载到生产。未经用户批准不合并，禁止强推。

其他编译器、缓存和 UI 进展见 [REFACTOR_RESUME](REFACTOR_RESUME.md)、
[README](README.md) 及 git 中此前的续接记录。
