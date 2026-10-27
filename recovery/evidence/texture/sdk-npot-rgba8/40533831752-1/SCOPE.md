# PcNpotRgba8 实际SDK范围补充

源码a945c18c8ea02b6ad03f8a4b6172be10ba7dffa2，运行40533831752 attempt1。原validation.json的通用scope字符串沿用复用脚本旧POT/DXT文字；不改写原始收据。

实际profile=PcNpotRgba8：80个NPOT源图像，true-color TGA24/32、PNG RGB/RGBA8（含Adam7）、BI_RGB BMP24/32（正反方向），源3×1/3×2/5×3/7×5与0/2图案。加载后向上取2幂。A8R8G8B8/X8R8G8B8/R5G6B5/DXT3/DXT5五输出×Texture/OnDemandTexture×Standard/Volume×有无mip，共3200资产。

Original/Recovered/NoOriginal每模式3200个唯一资产及实际Core调用，四产物和完整三缓冲区/cdata轨迹一致；NoOriginal每次调用前后断言原DLL不存在且未加载。以validation.json的profile/fixtureFiles/modes、input-Mod.xml、配置与traces.zip为原始依据。该SDK组passed，但当次workflow整体failure因另一个组件的回放宿主规则遗漏。

最终宿主修复d012da8ae55c094220d9c506b5263b277a49042b/40536063863 success，只补npot-rgba8并复用本SDK原sourceCommit/runId；SDK生产源码、输入、配置和比较逻辑未在该修复中改变。最终闭环不将40533831752改写为整轮success。

仅限定8位存储路径和专用MOD验收，生产SDK未替换，不宣称完整mymod/全TextureCompiler。其他位深/原版拒绝边界见recovery/TEXTURE_NPOT_NEXT.md。
