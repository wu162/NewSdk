# TextureCompiler：PC续接

分支：`recovery/texturecompiler-source`；仅PC、禁止强推、编译测试仅Actions。所有贴图还原优先复用公开实现并适配原版。连续实现实际消费链，稳定后集中验收；普通检查点不触发完整测试/SDK/MOD。

## 本轮完成（2026-10-30，压缩 DDS 数值兼容修正）

- 在 `validation/texture-public-d3dx-bc` 继续适配固定 DirectXTex BC1/2/3：修正原surface-memory ABI的源指针/格式次序和源尺寸；文件量化使用原预舍入倒数、转整数前float存储；端点还原/调色板插值保留原存储顺序；最终float行饱和，固定系数表避免首次调用舍入模式改变常量。没有样本ID分派、gold查表或原版后端回调。
- `598b9d10854d4c184e8a1e372a2068228c307949` / `41024955646` 完成真正360份原float对照（80差异）；`f1b3865db42201594ef9ef688275ccf1c8da6fb2` / `41025554191` 消除31条BC3回归，默认DDS差异323→7。
- `6b12ff3c457a93a1a466eb09a4d9d2fab508af94` / [41026301123](https://github.com/wu162/NewSdk/actions/runs/41026301123) **probe-only**：1572默认DDS、360完整float平面、1536 XG块、18168文件块（含16632实际过滤块）在固定plain/dither策略全部逐字节匹配；D/R各296270063字节捕获SHA `b664deb00c0448bc77c7a7ef9b307aa3ece83581b560330f0647884f1811b13e`，原版重复稳定。旧168策略扫描仍为定位资料，不要求任意候选策略等于默认文件策略。
- 当前实现 `ac6f8af69177ddeda86eb0b9abe09ed9798f7e2c` 再固定RGB/解码系数位模式，并将原版重复稳定性列为硬断言。本轮一次正式 `native-bc-files` 完整D/R/逐次NoOriginal验收 [41027020522](https://github.com/wu162/NewSdk/actions/runs/41027020522) **运行中，未取得终态**；不能用上面的probe或旧提交成功替代当前源验收。未变64组件、SDK/MOD不重跑。
- 正式native-loader仍拒绝压缩DDS，候选ABI尚未进入生产六DLL包。详细来源、修正与剩余门槛见[压缩DDS专页](TEXTURE_PUBLIC_D3DX_BC.md)。旧失败/partial capture及原锚点保留，不作为float gold。

## 下一轮计划

1. 先收取 `ac6f8af6` / `41027020522` 的正式终态，核对完整D/R、NoOriginal、原始归档SHA和选择器实际选组。失败只补真实失败/变化输入；成功后普通合入候选，不强推。
2. 组件闭环后，在独立实现分支接入正式native-loader及实际压缩CopySurface消费，优先复用这批公开BC核心；更新旧105个压缩未支持边界为原版成功完整对照，并保留原raw/BMP/TGA矩阵。共享native-copy的真实消费者需要最终受影响回归，不能照搬旧收据。
3. 新ABI正式进入生产包前，再在明确集成里程碑取得适用SDK证据。普通收尾不跑SDK/MOD；不扩展非PC。

## 已验收基础（2026-10-30，公开原生加载批次）

- 复用固定Microsoft DirectXTex triangle，接通原生TGA24/32和raw A8/X8 DDS的NPOT加载；各轴向上取2幂，DDS逐已提供层重采样，再从已上传层BOX生成缺失mip。保留X8物理未使用字节与端序、52字节header、主/secondary VM及packed tail所有权。
- 复用固定stb_image BMP，支持BI_RGB24/32的core12、INFO40、52、56、OS2-64、V4-108、V5-124，含方向、padding、真彩色表、忽略bfOffBits的原版实际像素位置及有界V5 profile。原版在BGR24展开前按源DWORD扫描alpha；适配器保留有界扫描规则，对会越过文件末尾而依赖未初始化堆字节的输入明确拒绝。来源/通知/修改前后SHA在`PublicDirectXTex`/`PublicStbBmp`。
- 业务源 `d0a31471906307690863e24784ca7384ef2371bf` / [41006229811](https://github.com/wu162/NewSdk/actions/runs/41006229811) completed/success：6860构造、14606文件（14501支持范围完整成功对照，包含5665新增NPOT与6152 BMP；105压缩DDS原版成功仍明确未支持）、另13拒绝。D/R及逐次NoOriginal全部通过；每配置634070359字节完整trace相同，四个归档原始成员SHA和12份x86 DLL指纹已独立核对，见[收据](evidence/texture/handoffs/20261030-native-public/acceptance.json)。
- 本次只补native-loader，复用63组；base保留 `34946af55168a43edd350ea12b8329833b16b8bb` / [41005120850](https://github.com/wu162/NewSdk/actions/runs/41005120850) 的完整成功收据。该首轮整体failure保留：BMP源alpha契约与宿主x86累计trace内存耗尽；修复后完整流式写盘/逐条NoOriginal读取/EOF核对，不减少样本或字节断言。当前64组件收据有效。
- 已普通合入恢复分支（合并提交 `710bae1200efe5f09067dcb90c0c8380b468fd72`），未启用生产六DLL包，未宣称完整混合模式身份或SDK集成；本轮未跑SDK/MOD。详细范围与边界见[原生文件加载](TEXTURE_NATIVE_FILE_LOAD.md)。
