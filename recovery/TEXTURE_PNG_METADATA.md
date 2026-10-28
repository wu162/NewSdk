# PC PNG ICC/APNG 默认图与 sRGB/gAMA

2026-10-29（北京时间），`recovery/texturecompiler-source`。仅PC、禁止强推、编译和恢复验收仅Actions。生产SDK DLL/config未替换，完整mymod不默认重测。

## 当前状态

原版取证与不可变gold、恢复组件及完整插件消费已通过。源码`1cff9256c8af926121cd47f44b82b34200ff24e1` / [40780996417](https://github.com/wu162/NewSdk/actions/runs/40780996417) completed/success：15个实际受影响组全部通过、22个未变成功组复用。Debug/Release各7920 gold输入的双入口共15840调用、1094726检查、3153920 RGBA字节；34880插件输入与同量NoOriginal均0错误、各232844236字节比较。专用SDK即将集中验证，未取得成功收据前不宣称集成完成。

## 已核对的原版契约

- `iCCP`不做色彩管理，不因RGB/灰度profile、ICC版本、不同RGB色域而改变输出。APNG的`acTL/fcTL/fdAT`不参与帧合成，读取`IDAT`的PNG默认图；默认图不属于动画序列时也如此。后续帧在测试中与默认图不同，包含子矩形、offset、blend/dispose和三帧序列。
- 本批合法chunk framing/CRC内，空/短/重复/后置iCCP、不支持的ICC压缩方法或坏profile zlib、空APNG控制块/帧数据、错误sequence及越界后续帧均不影响默认图。恢复版直接跳过这些ancillary payload，不解压ICC、不执行动画。**这不是APNG动画播放或ICC色彩管理实现。**
- 有效`sRGB`：在首个IDAT前，长度1，rendering intent 0..3。它优先于前后任意本批有效`gAMA`，像素等同sRGB默认gamma；有效声明之后的无效重复sRGB不撤销它。错误长度、intent 4和IDAT后sRGB忽略，保留有效gAMA。gamma的数值转换和已恢复PNG16源存储规则不变。
- chunk长度/CRC、IDAT拼接、zlib/Adler、关键chunk规则继续执行既有检查。本批没有把CRC损坏或所有畸形PNG升级为原版等价范围；原版会拒绝的索引PNG和GA16仍保留BadTextureFile，其他原拒绝边界不因metadata扩展而放开。

实现只修改`src/TextureCompiler/Managed/TexturePngDecoder.cs`的metadata分派及sRGB优先级，无原DLL回调、无gold输出表、无样本ID分派。现有浮点/NPOT、raw/DXT/mip/volume和atlas继续使用同一公共加载链。

## 原版gold和来源

| gold | sourceCommit / runId | 输入 / 原调用 | 完整RGBA / 原错误 |
| --- | --- | --- | --- |
| ICC/APNG、gamma交互 | `c812b8094a032dba0a29475d90e563cef211116e` / [40779895712](https://github.com/wu162/NewSdk/actions/runs/40779895712) | 4680 / 9360 | 3120 / 1560 |
| sRGB顺序、intent、重复/后置 | `2cab493118bb33992a762ee191af392d10fe30eb` / [40780410178](https://github.com/wu162/NewSdk/actions/runs/40780410178) | 3240 / 6480 | 2160 / 1080 |

两run completed/success，收据均为**probe-only**，不是恢复验收。每输入连续原调用两次逐字节核对完整RGBA和descriptor/hasAlpha，原异常type/message/ErrorCode也须一致，源文件不得改变；先初始化PC插件并通过TGA正对照，固定原DLL SHA/token/allocator thunk。

两份gold各覆盖180个独立上下文：15种合法PNG颜色/位深组合 × POT 8×4、NPOT 7×5、volume条带16×8 × linear/Adam7 × 两种像素图案。第一份每上下文26种metadata，第二份18种。原输入、完整RGBA和完整错误记录在：

- `evidence/texture/png-metadata-load/40779895712-1/{samples.zip,transcript.tsv,validation.json}`；归档SHA256 `5e698ae274086468e3efa5520847fa9127acc4de62c1b96060a5ac0cbab0cac6`。
- `evidence/texture/png-metadata-order/40780410178-1/{samples.zip,transcript.tsv,validation.json}`；归档SHA256 `e804e2d5099f73c6be676a87153b02806fc0237827dc0da657c48503cdbd964f`。

5份ICC取自[Compact-ICC-Profiles](https://github.com/saucecontrol/Compact-ICC-Profiles/tree/bdd84663061bc4ae95ca70decff54f581e27f702)固定提交`bdd84663061bc4ae95ca70decff54f581e27f702`：sRGB v2 micro、sRGB v4、AdobeCompat v2、sGrey v2 micro、sGrey v4。只将原字节base64保存，解码后指纹与上游数据相同；完整CC0通知和PROVENANCE保存在`reference/compact-icc/`。它们是测试素材，没有引入新的生产codec或色彩管理库。

[独立归档核对](evidence/texture/handoffs/20261029-png-metadata/original-archive-audit.json)由`recovery/scripts/InspectTexturePngMetadataArchive.py`读取归档生成，不执行生产codec/DLL。核对所有源/输出SHA、全量RGBA字节、完整错误、metadata默认图规则及sRGB优先级；两个gold各有54个上下文实际发生gamma差异，其余相同上下文不冒充gamma敏感覆盖。

## 恢复验收入口

- `png-metadata`：`TestTexturePngMetadata.ps1`固定两份gold的sourceCommit/runId/归档SHA/总数/成功拒绝数，要求原源码是祖先；Debug/Release分别对全部7920输入执行直接decoder与公共ImageLoader两入口，共15840次调用。逐像素完整比较RGBA、尺寸/hasAlpha和原错误，数组与源文件不变，每次调用前后检查原DLL不存在且未加载。
- `png-metadata-images`：`TestTexturePngMetadataImages.ps1`，D/R各34880输入及同量NoOriginal重放。32640个raw3/DXT3/5输入、2176个全透明DXT1A输入、64个PNG编码atlas输入；两根、标准/volume、无mip/完整mip、POT/NPOT、linear/Adam7、新旧cdata。完整三buffer、全cdata/旧文件尾与源字节相同，并作独立DDS/atlas布局检查。默认DXT1/不透明DXT1A范围不扩大。
- 原受影响组通过集中选择器统一回归；成功且输入未变的组保留原sourceCommit/runId，gold探针不作为可复用恢复证据。原版取证入口`png-metadata-load`/`png-metadata-order`为manual_only/evidence_only。
- SDK专用组`sdk-PcPngMetadata`覆盖4240实际资产，当前待验收：88张不可变RGBA8 POT/NPOT输入产生3520 raw/DXT3/5资产，88张全透明metadata输入产生704 DXT1A资产，8张PNG编码TGA路径产生16 atlas base/normal资产。三模式实际Core调用、四产物、全轨迹文件集/完整buffer/cdata与逐次NoOriginal；不替代完整MOD。

## 下一步和效率

本轮最终组件已取得完整成功证据，接着集中必要SDK并独立逐文件核对实际归档。此后PNG metadata本批闭环，转向未声明JPEG CMYK或TGA metadata/RLE、其他BMP组合的实际输入取证。原默认DXT1/不透明DXT1A的未初始化边界继续拒绝。

本轮先用两个独立原版探针查清完整metadata规则，再合批所有恢复/消费矩阵，避免按单个ICC或APNG子项重复启动全部消费者。未重跑既有成功且输入未变的组件，完整mymod仍manual_only。无法量化账号额度节省。
