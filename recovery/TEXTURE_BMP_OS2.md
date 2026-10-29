# OS/2 BMP 完整 BI_RGB 真彩能力组

2026-10-29（北京时间），`recovery/texturecompiler-source`。本批为功能实现，验收状态以 RESUME 与实际 run 收据为准；未通过前不宣称支持完成。

固定原 DLL SHA256 `4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e`。原 parser 的完整字节在 `evidence/texture/bmp-bitfields-native/20261029/DibParser.bin`，反汇编在同目录 `DibParser.asm`。

- `0x13F89..0x13FBC`：core12 转成公共局部 header，宽高取 signed WORD；planes/depth 直接复制，compression/额外字段置零。恢复版限制16384维度，未引入大于32767的unsigned宽高行为。
- `0x13FC1..0x14032`：非core接受至少40字节header，负高度取绝对值，compression0进入公共BI_RGB分派。
- `0x1431E..0x14348`：仅16/24/32位进入直接颜色格式；索引1/4/8位走原失败，与输入调色板是否完整无关。BMP BI_RGB从14+DIB取像素，bfOffBits/gap规则保持。
- 原core12与OS2-64没有24位专属限制；原共享RGB555/BGRA32、全零alpha与NPOT存储规则可直接复用。源码只解除该限制，未实现标准bitfields替代原未初始化mask，也不接受OS/2压缩。

`TextureBmpOs2Fixtures.cs`独立构造core的3-byte与OS2-64的4-byte调色板；真彩/索引、八形状、方向、三图案、三gap及调色板reserved0/255共1944输入。直接差分两入口与独立进程逐次NoOriginal比较完整原RGBA、尺寸、hasAlpha、完整错误，另36坏结构安全拒绝。坏结构不作原gold声明。

`TextureBmpOs2PipelineOracle.cs`复用既有完整三buffer/cdata/旧尾与DDS布局断言：25920 raw3/DXT3/5、576透明DXT1A、108正向atlas、1944索引拒绝，共28548插件输入。含两根、标准/volume/无mip与完整mip、POT/NPOT/partial、非法volume原错误。每配置重新捕获原输出并在无原DLL进程逐次重放；atlas必须成功，不允许同错误冒充成功覆盖。SDK单独累计，不随本组自动跑完整MOD。

本轮修正旧BMP atlas测试覆盖：原`TexturePacker.AddTexture`检查的是两条路径中是否含`tga`，不是文件magic。旧BMP组件atlas的双方相同错误只是拒绝对照；旧专用SDK因此失败。现用合法`.tga`路径保存完全相同BMP内容，并对组件atlas增加成功断言。恢复loader仍按文件magic派发，没有将BMP转换为TGA像素或更换源图。

索引色原gold为`69e6485e / 40810778444`的`bmp-indexed-header-load`：2025索引输入全部稳定BadTextureFile，96真彩控制成功，4243原调用。独立归档审计见`evidence/texture/handoffs/20261029-bmp-os2/indexed-archive-audit.json`，2121源文件1014648字节、首次/重复RGBA各24704字节；仍probe-only，不能当作恢复成功锚点。

专用SDK与原BMP headers批次合并：356不可变旧源+189新OS2源，各40资产；64旧透明源+27新OS2透明源，各8资产；50旧atlas源+21新OS2源，各2资产，共22670资产/模式、142正向atlas。原模式/恢复模式/逐次NoOriginal使用完全相同哈希检查后的ID和XML。新OS2源只加原24-bit bottom范围之外的7个DIB/depth/direction变体，未为每种输入另开SDK。完整MOD不重跑。

实际SDK使用`BuildTextureCompiler.ps1`产出的5-DLL包；包内带来源与源码/产物指纹，可用`InstallTextureCompiler.ps1`修改匹配SDK副本的3条贴图根路由，并用`RestoreTextureCompiler.ps1`恢复6个文件原状态。回滚先核验所有当前/备份指纹，拒绝覆盖安装后的新改动。SDK只在临时副本验证安装/回滚/再次安装和最终实际3模式对照；生产SDK树保持原样。构建成功与验收成功分开记录。
