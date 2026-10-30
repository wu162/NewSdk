# TextureCompiler：PC续接

分支：`recovery/texturecompiler-source`；仅PC、禁止强推、编译测试仅Actions。贴图恢复优先适配现有公开源码，连续实现消费链后集中验收；普通检查点不启动完整测试/SDK/MOD。

## 本轮完成（2026-10-30，正式BC入口与JPEG/PPM原生链）

- **BC1/2/3正式原生入口完整通过。** `validation/texture-native-bc-integration` 静态嵌入固定DirectXTex，正式NativeLoader读取小base/POT/NPOT压缩DDS、提供的尾层与缺层BOX；正式NativeCopy开放实际LoadImageA的flags0x12/threshold0/RGBA8全锁定行。内部raw入口与压缩桥分开，无原DLL/系统D3D回调，生产六DLL包未改变。
- 最终组件源码`2883373b6a4e27cb120399824244d65d5d9ae297` / [41053772714](https://github.com/wu162/NewSdk/actions/runs/41053772714)：仅补native-loader，复用63组件；D/R各16180成功文件（1679压缩，含1572完整BC语料）、6860构造、19拒绝，以及逐次NoOriginal全部通过。每份完整捕获750231011字节，SHA `b6dfff7000d2bcc260ec3a6afd784a5097f0ae144495522243592532adb434db`；完整记录/EOF与12份I386 DLL指纹已核对。见[验收收据](evidence/texture/handoffs/20261030-native-bc-integration/acceptance.json)。
- 该run整体仍为**failure**：最后普通推送被GitHub单文件100MiB上限拒绝；不是组件差分失败。原105409158字节ZIP与validation.json不改，捕获无损分为64MiB片并核对重构原SHA，保存原artifact/各片指纹和独立storage manifest；后续Save入口同样处理，避免重跑成功Windows矩阵。存储修复`7609c0b6`已普通推送；[41056117290](https://github.com/wu162/NewSdk/actions/runs/41056117290)仅Linux检查成功（11项存储、19项选择器检查，selected为空、64组件复用、Windows skipped），不改写旧run结论。
- 首轮`e5e2c979` / [41051807572](https://github.com/wu162/NewSdk/actions/runs/41051807572)另9组选中组通过，native-loader的宿主BC子矩形越界：原LockRect按raw pitch/bit单位偏移，逻辑crop4/4须编码left16/top1。已修宿主并新增每次读取的完整VM span硬检查，保留三锁定/两RGBA；生产Surface ABI未改。首轮失败与602567304字节partial capture保留。
- **下一批实现已保存但未验收。** `validation/texture-native-raster-integration` 的源码`75bcf843310724062dce8ff53ac8b5b6cfef53b6` / [41054121354](https://github.com/wu162/NewSdk/actions/runs/41054121354) **compiled-only/success**：D/R各六Native DLL与独立BC DLL均I386、两个完整宿主编译成功，未执行差分或NoOriginal。静态复用已验收IJG6b JPEG/PPM wrapper，保留JPEG彩色基础X=0、PPM基础X=FF，JPEG灰度以原L8资源保存；L8复用DirectXTex triangle并适配亮度量化、双轴整数BOX与退化轴float BOX。无新增运行decoder DLL。该实现未合入恢复分支/生产包。
- JPEG/PPM完整原生消费矩阵已准备：固定3份历史归档、13722源文件（9335原成功/4387原错误），完整HRESULT/资源/VM/各mip/三锁/两RGBA/释放和逐次NoOriginal；四参原loader矩阵仍完整保留。未跑SDK/MOD。

## 下一轮计划

1. 直接从`validation/texture-native-raster-integration` 的 `TextureNativeRaster.cpp`、`TestTextureNativeRaster.ps1`、`TextureNativeLoaderOracle.cpp` 五参模式继续。现业务和宿主均已编译；优先集中验收新native-raster与实际受影响native-loader，不重复从零调查或重跑未变消费者。
2. 集中入口：`force_groups=native-raster`、`native_bc_build_only=false`、`run_sdk=false`、`plan_only=false`（独立输入）。force仍叠加全部真实受影响组；保留13722完整输入、D/R与NoOriginal及16180/6860/19基础矩阵。compiled-only和旧最终RGBA gold不能替代原生VM/消费验收。
3. 若原生JPEG/PPM差异，先核对JPEG X0/PPM XFF、L8亮度存储、NPOT与BOX的实际层数据，按一般兼容规则修正；完整通过前继续留在独立分支。相关公开来源/通知与修改前后SHA保留。
4. 已验收BC核心保留`ac6f8af6` /41027020522与本轮正式入口收据；无输入变化不重复。新原生ABI正式进入生产包前再取适用SDK证据，普通收尾不跑完整MOD，不扩展非PC。

## 已验收基础

- 原native-public raw DDS/TGA/BMP/NPOT：`d0a31471` /41006229811，D/R及逐次NoOriginal完整通过；新BC组保留全部原输入并将105原版成功压缩边界升级为全产物成功对照。详细范围见[原生文件加载](TEXTURE_NATIVE_FILE_LOAD.md)。
- 独立公开BC：`ac6f8af6` /41027020522，1572默认DDS、360原float平面、1536 XG块、18168文件块及完整资源/消费/NoOriginal通过。见[BC来源与兼容说明](TEXTURE_PUBLIC_D3DX_BC.md)。managed JPEG/PPM及既有SDK成功仍保留各自精确历史锚点，不冒充本轮新原生ABI已集成SDK。
