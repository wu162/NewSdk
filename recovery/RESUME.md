# TextureCompiler：PC续接

分支：`recovery/texturecompiler-source`；仅PC、禁止强推、编译测试仅Actions。所有后续贴图还原优先复用公开实现并适配原版，见[公开实现入口](TEXTURE_UPSTREAM.md)。先连续实现实际消费链，稳定后集中验收；普通检查点不触发完整测试/SDK/MOD。

## 最新精度检查点（保存，不追加Windows）

BC3 alpha palette已改为float系数、先存右项乘积，再做左项扩展乘加的一般规则；对41007602862的全部8192个原alpha通道位模式静态数值核对一致。此新增源码尚未Actions验证。正在运行的41009041890只绑定d50b04a94db5c201bbac853a5eee0fa22c25158d，不能为本检查点背书。下一轮先读该run终态，再用当前源码集中补齐实际消费/BC块位模式/完整DDS与NoOriginal门槛；未变64组件及SDK不重跑。

## 此独立BC分支的当前检查点（未正式验收）

上一诊断165184da/41007602862已probe-only完成，D/R原始capture一致、原版重复稳定：flags0完整匹配DXT1/DXT3块编码，DXT5 alpha504差异；BC1 decode512块全等，BC2/3各511差异。现按一般规则适配：恢复BC2/3三色解码、A4经byte17/255展开、存储float倒数后去预乘、旧OptimizeAlpha梯度及pD8最后8/7系数；BC3 alpha插值精度仍待修正。新增真正压缩→raw的endian/swizzle/矩形/pitch适配，并对每个DDS mip的逻辑与物理RGBA、完整VM/锁定/释放一起诊断。完整原版/D/R/NoOriginal验收及小base/NPOT仍未完成，正式loader/生产包继续明确拒绝压缩。

本提交通过同一集中入口再次显式取证，只用于检验具体兼容修改和新实际消费者；旧64成功组保持不变并复用。请先检查分支最新public-d3dx-bc收据，不能把任务成功/部分匹配当作支持。已验收NPOT/BMP主成果如下。

## 本轮完成（2026-10-30，公开原生加载批次）

- 复用固定Microsoft DirectXTex triangle，接通原生TGA24/32和raw A8/X8 DDS的NPOT加载；各轴向上取2幂，DDS逐已提供层重采样，再从已上传层BOX生成缺失mip。保留X8物理未使用字节与端序、52字节header、主/secondary VM及packed tail所有权。
- 复用固定stb_image BMP，支持BI_RGB24/32的core12、INFO40、52、56、OS2-64、V4-108、V5-124，含方向、padding、真彩色表、忽略bfOffBits的原版实际像素位置及有界V5 profile。原版在BGR24展开前按源DWORD扫描alpha；适配器保留有界扫描规则，对会越过文件末尾而依赖未初始化堆字节的输入明确拒绝。来源/通知/修改前后SHA在`PublicDirectXTex`/`PublicStbBmp`。
- 业务源 `d0a31471906307690863e24784ca7384ef2371bf` / [41006229811](https://github.com/wu162/NewSdk/actions/runs/41006229811) completed/success：6860构造、14606文件（14501支持范围完整成功对照，包含5665新增NPOT与6152 BMP；105压缩DDS原版成功仍明确未支持）、另13拒绝。D/R及逐次NoOriginal全部通过；每配置634070359字节完整trace相同，四个归档原始成员SHA和12份x86 DLL指纹已独立核对，见[收据](evidence/texture/handoffs/20261030-native-public/acceptance.json)。
- 本次只补native-loader，复用63组；base保留 `34946af55168a43edd350ea12b8329833b16b8bb` / [41005120850](https://github.com/wu162/NewSdk/actions/runs/41005120850) 的完整成功收据。该首轮整体failure保留：BMP源alpha契约与宿主x86累计trace内存耗尽；修复后完整流式写盘/逐条NoOriginal读取/EOF核对，不减少样本或字节断言。当前64组件收据有效。
- 已普通合入恢复分支（合并提交 `710bae1200efe5f09067dcb90c0c8380b468fd72`），未启用生产六DLL包，未宣称完整混合模式身份或SDK集成；本轮未跑SDK/MOD。详细范围与边界见[原生文件加载](TEXTURE_NATIVE_FILE_LOAD.md)。

## 下一轮计划

1. 优先继续 `validation/texture-public-d3dx-bc`，源码检查点 `165184daa2a776b8b96e707ac8e3ccf0c5203932`。已接入固定DirectXTex BC1/2/3核心及独立x86 float块/图像接口；POT且base双轴>=4的DDS候选已接通完整provided块、尾层重编码、从实际前层压缩存储解码后BOX/重编码缺层和原生资源/锁定/释放。**未被正式loader或生产包调用，尚未还原完成。**
2. 本轮为该新核心准备并显式启动 `public-d3dx-bc` 定向诊断：1536块、168完整DDS，各8固定编码策略及wrap/clamp完整聚合；通过现有集中入口，不能把probe-only/任务成功或部分策略匹配当作业务验收。入口 `src/TextureCompiler/PublicD3dxBc/PATCHES.md`、`recovery/scripts/InspectTexturePublicBc.ps1`。先检查该分支最新收据/运行，核对原XGCopySurface真实flags/阈值及float精度，再修改一般端点/索引/量化规则，保留完整输出。
3. 1×1压缩DDS原生base实际4×4、levels3；源依据为 `native-loader/40999035840-1/captures.zip` 的105份完整原版资源（诊断资料，不能直接当确定性业务gold）。候选当前仍拒绝小base/NPOT，不可静默近似扩展。接通完整压缩→raw实际消费和边界后集中取得受影响D/R、NoOriginal及适用SDK，再普通合入。
4. 未变64组件和旧SDK不重复运行；新ABI正式进入生产包前另取必要SDK集成证据。更多PNG/JPEG、低位深/灰度/调色板TGA等原生加载同样先复用现成源码；不能用已有托管最终RGBA推断原生存储格式/字节。
