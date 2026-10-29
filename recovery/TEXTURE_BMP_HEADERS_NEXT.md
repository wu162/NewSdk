> 最新收尾：40804823544整体failure。10既有SDK passed；新PcBmpHeaders在原Core发现BmpHeader_12000/BmpHeader_11012的ID hash冲突7c4181ec，未取得专用SDK验收。下一步先生成并检查无冲突ID，再只补此SDK。bitfields探针已完成且独立归档审计通过，仍probe-only。完整状态见RESUME首节。

# BMP DIB / V2 / V3 / V4 / V5 取证收尾与下一步

2026-10-29（北京时间），`recovery/texturecompiler-source`。上次已完成中断探针收尾；本轮继续BI_RGB实现及完整消费验证。仅PC、编译测试只Actions、禁止强推。

## 当前实现检查点（验证中）

`9dc5a38911b821178feea03ac90ac036c0e3ce5d` / [40803450961](https://github.com/wu162/NewSdk/actions/runs/40803450961)已completed/success：14个受影响完整组通过、29个成功组复用；当前43组件有效，SDK待本轮集中验收。实现覆盖DIB12/40/52/56/64/108/124的已声明BI_RGB，V4/V5忽略gamma/ICC渲染效果并检查V5 profile范围，compression6与INFO40 bitfields匹配原BadTextureFile。扩展bitfields288输入均NotSupported：192个24/32位不确定输入和96个16位未解语义输入，不冒充原错误匹配。

组件`bmp-headers`全2616行双入口：1896原解码、432原拒绝、288明确拒绝，另106坏结构边界；Debug/Release各775880检查、2673360字节完整对照。插件`bmp-header-images`每配置37160条，其中36008原版差分与1152主动拒绝；完整NoOriginal回放，含200 atlas；各配置5960原错误匹配/1152主动拒绝，原比较成功输出30048条，完整比较216030576字节。组件覆盖全部原pattern；插件按固定profile/shape/pattern组合选择，不宣称所有交叉组合都重测。

[完整布局审计](evidence/texture/handoffs/20261029-bmp-headers/bi-rgb-layout-audit.json)已只读核对1896 BI_RGB的源像素区域、尺寸/hasAlpha及1336680完整RGBA字节，全部等于对应INFO40（含NPOT与792 metadata）。复现脚本`InspectTextureBmpLayoutArchive.py`不执行codec，只是实现依据，不能替代Actions验收。

专用`PcBmpHeaders`准备356个原gold源×40消费、64个透明源×8消费及50个atlas源×2消费，共14852实际资产/模式；包含Windows各DIB的32bit零/混合/不透明alpha。此SDK验证中（源码6fb39059 / [40804823544](https://github.com/wu162/NewSdk/actions/runs/40804823544)，尚无通过声明），不自动启动完整mymod。另准备`bmp-bitfields-load`原版探针，2进程×4320输入、12正对照、25946次原Load及17304份native snapshot；native只读已知格式的首行逻辑字节前缀、不据pitch×height推断分配大小；未知格式不读pixels。native snapshot与Load来自分别创建的纹理，不能称为同一个Load实例的存储。先收实际descriptor/已知格式首行逻辑字节前缀/两次RGBA，再判断一般mask语义，probe-only不启用生产bitfields。

## 已完成：原版探针与独立归档审计

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

## 扩展 bitfields 的静态根因（不启用生产）

已定点核对固定原DLL：DIB parser `0x13F3D` 只复制40字节局部header，helper `0x13E99`仅40字节自复制；compression3且DIB≥52随后却从局部header的+40/+44/+48/+52读mask，落入未初始化的邻接栈区，而非输入mask字段。影响16/24/32位，说明96个16位的两次相等也不能作为标准565支持依据。正常texture type3的XGGetTextureDesc明确写出format，排除了该路径输出descriptor预清零与否的疑点。完整8函数字节/SHA/RVA及复现见[根因与边界](TEXTURE_BMP_BITFIELDS_BOUNDARY.md)。此为静态证据，不冒充新probe或恢复验收；动态probe仍按真实终态记录。

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
