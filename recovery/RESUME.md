# TextureCompiler：PC续接

分支：`recovery/texturecompiler-source`；仅PC、禁止强推、编译测试仅Actions。所有后续贴图还原优先复用公开实现并适配原版，见[公开实现入口](TEXTURE_UPSTREAM.md)。先连续实现实际消费链，稳定后集中验收；普通检查点不触发完整测试/SDK/MOD。

## 本轮完成（2026-10-30，公开原生加载批次）

- 复用固定Microsoft DirectXTex triangle，接通原生TGA24/32和raw A8/X8 DDS的NPOT加载；各轴向上取2幂，DDS逐已提供层重采样，再从已上传层BOX生成缺失mip。保留X8物理未使用字节与端序、52字节header、主/secondary VM及packed tail所有权。
- 复用固定stb_image BMP，支持BI_RGB24/32的core12、INFO40、52、56、OS2-64、V4-108、V5-124，含方向、padding、真彩色表、忽略bfOffBits的原版实际像素位置及有界V5 profile。原版在BGR24展开前按源DWORD扫描alpha；适配器保留有界扫描规则，对会越过文件末尾而依赖未初始化堆字节的输入明确拒绝。来源/通知/修改前后SHA在`PublicDirectXTex`/`PublicStbBmp`。
- 业务源 `d0a31471906307690863e24784ca7384ef2371bf` / [41006229811](https://github.com/wu162/NewSdk/actions/runs/41006229811) completed/success：6860构造、14606文件（14501支持范围完整成功对照，包含5665新增NPOT与6152 BMP；105压缩DDS原版成功仍明确未支持）、另13拒绝。D/R及逐次NoOriginal全部通过；每配置634070359字节完整trace相同，四个归档原始成员SHA和12份x86 DLL指纹已独立核对，见[收据](evidence/texture/handoffs/20261030-native-public/acceptance.json)。
- 本次只补native-loader，复用63组；base保留 `34946af55168a43edd350ea12b8329833b16b8bb` / [41005120850](https://github.com/wu162/NewSdk/actions/runs/41005120850) 的完整成功收据。该首轮整体failure保留：BMP源alpha契约与宿主x86累计trace内存耗尽；修复后完整流式写盘/逐条NoOriginal读取/EOF核对，不减少样本或字节断言。当前64组件收据有效。
- 已普通合入恢复分支（合并提交 `710bae1200efe5f09067dcb90c0c8380b468fd72`），未启用生产六DLL包，未宣称完整混合模式身份或SDK集成；本轮未跑SDK/MOD。详细范围与边界见[原生文件加载](TEXTURE_NATIVE_FILE_LOAD.md)。

## 下一轮计划

1. 优先继续 `validation/texture-public-d3dx-bc`，最新源码检查点 `ce16e990192672b46ed8f6bb4870ed791d009e64`。已接入固定DirectXTex BC1/2/3核心及独立x86 float块/图像接口；POT且base双轴>=4的DDS候选已接通完整provided块、尾层重编码、从实际前层压缩存储解码后BOX/重编码缺层和原生资源/锁定/释放。**未被正式loader或生产包调用，尚未还原完成。**
2. 本轮通过现有集中入口完成 `public-d3dx-bc` 定向诊断，均只选该组并复用64组件。最新已运行源 `d50b04a94db5c201bbac853a5eee0fa22c25158d` / [41009041890](https://github.com/wu162/NewSdk/actions/runs/41009041890) 为probe-only：D/R完整capture各89979984字节相同、原版重复稳定；固定flags0完整匹配全部1536个BC1/2/3编码块，但976个decode块位模式和最佳策略86/168完整DDS（含每层逻辑/物理RGBA、VM及所有权）仍不同。不能把这些任务成功或部分匹配当业务验收。最新源码随后补写显式原存储float字节倒数及BC3 alpha右项乘积存储规则；对旧原capture8192个alpha通道数值核对一致，但**新补丁尚未Actions验证**，不继承d50b04a9结果。入口 `src/TextureCompiler/PublicD3dxBc/PATCHES.md`、`recovery/scripts/InspectTexturePublicBc.ps1`。下一轮先验证最新源码的完整块位模式，再区分文件FourCC与XG原生格式对三色/去预乘的语义，定位全部DDS尾层/缺层及RGBA视图差异；保留完整输出并取得最终D/R/NoOriginal，当前未启用压缩加载。
3. 1×1压缩DDS原生base实际4×4、levels3；源依据为 `native-loader/40999035840-1/captures.zip` 的105份完整原版资源（诊断资料，不能直接当确定性业务gold）。候选当前仍拒绝小base/NPOT，不可静默近似扩展。接通完整压缩→raw实际消费和边界后集中取得受影响D/R、NoOriginal及适用SDK，再普通合入。
4. 未变64组件和旧SDK不重复运行；新ABI正式进入生产包前另取必要SDK集成证据。更多PNG/JPEG、低位深/灰度/调色板TGA等原生加载同样先复用现成源码；不能用已有托管最终RGBA推断原生存储格式/字节。
