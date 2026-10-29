# BMP DIB / V2 / V3 / V4 / V5 后续取证

2026-10-29（北京时间），`recovery/texturecompiler-source`。当前只有工具准备稿，尚无本批原版/恢复/消费/SDK结论。仅PC、编译测试只Actions、禁止强推。

既有BI_RGB INFO40的16/24/32、POT/NPOT、原gap读取规则及索引/BI_BITFIELDS拒绝已验收，不重复旧4320案例。新显式原版组`bmp-header-load`取2616个输入，先区分支持、原拒绝与当前恢复入口差异。

- 四尺寸16×8、7×5、1×1、32×16，三种像素/alpha图案。
- Windows DIB 40/52/56/108/124 ×16/24/32 bit×top/bottom×compression 0/3/6，共1080；含明确的unsupported compression/bitdepth组合，不预判成功。
- OS/2 core12与OS/2 v2 64只取24-bit bottom BI_RGB，共24；不构造巨大unsigned负高度。
- V4/V5 calibrated/gamma/sRGB及V5固定CC0来源嵌入ICC，共792；另有合法文件gap3/17，共720。
- mask在对应DIB内部或外部占用完整区域，文件大小/bfOffBits/stride/原始像素/ICC相对偏移有界。测试构造器调用已有BMP fixture写原像素，未修改生产BMP decoder或引入codec。

`TextureBmpHeaderProbe.cs`固定原DLL/allocator/token、Initialize PC和TGA正对照；每输入原调用两次并逐字节比较RGBA/descriptor/hasAlpha或完整原错误，保存当前ImageLoader候选的完整RGBA/错误。探针即使comparison全部相同仍为probe-only、productionAccepted=false，不能作为恢复/插件/SDK验收。

集中入口`InspectTextureBmpHeaders.ps1`，`bmp-header-load`为manual_only/evidence_only，标记`[texture-bmp-headers]`显式选择。先完成本轮TGA组件，再在同一必要SDK Windows批次顺序采此探针，减少重复安装/拉取；原完整MOD不自动选择。根据真实结果继续整组适配，不能把未知DIB当INFO40默默回落，也不把原拒绝当缺失成功支持。

首批278ae577 /40797426507在`h52-d24-16x8-topTrue-p0-c3-g0-plain`检测到原版同输入两次RGBA不一致而停止，保留failed/partial归档；同批11 SDK全部通过，不受此失败影响。探针随后改为保存两次完整RGBA/尺寸/hasAlpha/错误并逐输入标记`originalStable`，不稳定输入独立分类、绝不作为确定gold或恢复验收。只显式重跑bmp-header-load，不重跑已通过SDK；生产BMP仍未修改。
