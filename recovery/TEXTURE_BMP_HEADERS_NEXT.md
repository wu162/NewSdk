# BMP DIB / V2 / V3 / V4 / V5 取证收尾与下一步

2026-10-29（北京时间），`recovery/texturecompiler-source`。本轮仅收取中断前探针、独立审计归档并更新交接，生产BMP decoder尚未修改。仅PC、编译测试只Actions、禁止强推。

## 本轮完成：原版探针与独立归档审计

源码 `511896747c32c7863078b36ad781caae4f3763c1` / [40799686293](https://github.com/wu162/NewSdk/actions/runs/40799686293) attempt 1已核实completed/success。只执行`bmp-header-load`，41个已验收组件复用；未运行SDK或完整MOD。组收据仍为`probe-only`、`productionAccepted=false`、`sdkValidated=false`，不是BMP扩展实现验收。

2616个输入、5232次原调用，首调用2184解码/432拒绝；当前恢复入口仅288一致（216完整解码、72原错误匹配），2328不同（1968解码域、360拒绝域）。拒绝域差异包含异常类型/完整错误文本不同，不能一律解释为缺失成功支持。所有原拒绝均为`BinaryAssetBuilder.BinaryAssetBuilderException/BadTextureFile/D3DX cannot load the texture from disk.`。

[独立审计](evidence/texture/handoffs/20261029-bmp-headers/original-archive-audit.json)逐输入核对原BMP、首次/重复完整RGBA、候选RGBA及哈希、尺寸、错误和重复一致性分类，核对内外transcript及完整归档成员集。原RGBA首次和重复各1539720字节、候选152280字节；归档SHA256为`08715c9547eb6f9fb8ba1745ab3f38b954522439848caa031889c2f73deb069b`。只读归档审计不执行codec/BAB，不新增原版或恢复版验收。

| 原版观察分类 | 输入数 | 后续处理 |
| --- | ---: | --- |
| 不确定组合之外，两次解码相等 | 1992 | 后续适配候选，尚非恢复gold验收 |
| 两次均相同原拒绝 | 432 | 保留原错误边界 |
| 不确定组合内，两次恰好相等 | 143 | 与同组合不稳定输入一起隔离 |
| 不确定组合内，两次输出不同 | 49 | 保存双方完整输出，不作确定gold |

49个反例全部位于DIB52/56/108/124、24/32-bit、compression=3（BI_BITFIELDS），top/bottom两方向均有。整组192输入一起列为不确定域；其中143个重复相等不能证明该组合稳定。此次只观察同一进程内相邻两次调用，2567个`originalStable=True`不等于跨进程、跨调用上下文确定性证明。具体49个标签见审计JSON，不用标签分派生产逻辑。

792个V4/V5 metadata输入（gamma1/gamma22/sRGB/calibrated、V5 embedded-sRGB2/sRGB4/gray4）与各自同DIB、尺寸、位深、方向、pattern的plain输入，完整RGBA/hasAlpha/尺寸全部相等。该结论只覆盖当前构造的有效有界profile；不外推任意ICC、损坏结构或其他组合。

## 输入及历史失败保留

四尺寸16×8、7×5、1×1、32×16，三种像素/alpha图案；Windows DIB40/52/56/108/124×16/24/32-bit×top/bottom×compression0/3/6共1080，OS/2 core12与v2 64的24-bit bottom BI_RGB共24，V4/V5 metadata共792，合法文件gap3/17共720。bfOffBits/stride/mask/ICC相对偏移及源像素均随BMP保留。既有INFO40的BI_RGB16/24/32与原gap规则继续沿用旧验收，不重复旧4320案例。

首批278ae577 /40797426507在`h52-d24-16x8-topTrue-p0-c3-g0-plain`检测到原版两次RGBA不同而停止，failed/partial归档原样保留；该run整体仍failure，其中11个独立SDK profile通过，不改写为整体success。后续只修探针记录工具并在51189674 /40799686293补取两次完整输出和逐输入分类，没有重跑成功SDK。

独立审计复现（不编译、不运行codec）：

```sh
python3 recovery/scripts/InspectTextureBmpHeaderArchive.py \
  --run-id 40799686293 \
  --source-commit 511896747c32c7863078b36ad781caae4f3763c1 \
  --out work/bmp-header-archive-audit.json
```

## 下一轮计划与验收门槛

优先按稳定候选域整组适配DIB/core/OS2的BI_RGB、V4/V5忽略metadata效果及确定原拒绝；入口为`src/TextureCompiler/Managed/TextureBmpDecoder.cs`、`TextureImageLoader.cs`和`recovery/tests/TextureBmpHeaderFixtures.cs`。先核对原读取布局、bfOffBits/gap与alpha规则，再实现，不能把新DIB当INFO40静默回落。DIB16-bit BI_BITFIELDS的96个重复相等样例仍须核对一般mask语义；24/32-bit BI_BITFIELDS整组保持隔离，若要扩大支持，先做独立进程/调用上下文的原版定向取证。

组件须保留完整RGBA/错误/源不变、Debug/Release和逐次NoOriginal；实际Texture/OnDemandTexture、raw3/DXT3/5/透明DXT1A、POT/NPOT/volume/full mip/atlas、三buffer/full cdata/旧尾消费者按依赖取得完整差分。正式宣称接入前集中取得必要专用SDK证据；不把本探针作为组件或SDK通过。未变化的41组件/13专用SDK保留原sourceCommit/runId，不因本次收尾重跑，完整mymod仅必要集成里程碑。
