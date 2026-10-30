# TextureCompiler：PC续接

分支：`recovery/texturecompiler-source`；仅PC、禁止强推、编译测试仅Actions。所有贴图还原优先复用公开实现并适配原版。连续实现实际消费链，稳定后集中验收；普通检查点不触发完整测试/SDK/MOD。

## 本轮完成（2026-10-30，压缩 DDS 数值兼容修正）

- 在 `validation/texture-public-d3dx-bc` 继续适配固定 DirectXTex BC1/2/3：修正原surface-memory ABI的源指针/格式次序和源尺寸；文件量化使用原预舍入倒数、转整数前float存储；端点还原/调色板插值保留原存储顺序；最终float行饱和，固定系数表避免首次调用舍入模式改变常量。没有样本ID分派、gold查表或原版后端回调。
- `598b9d10854d4c184e8a1e372a2068228c307949` / `41024955646` 完成真正360份原float对照（80差异）；`f1b3865db42201594ef9ef688275ccf1c8da6fb2` / `41025554191` 消除31条BC3回归，默认DDS差异323→7。
- `6b12ff3c457a93a1a466eb09a4d9d2fab508af94` / [41026301123](https://github.com/wu162/NewSdk/actions/runs/41026301123) **probe-only**：1572默认DDS、360完整float平面、1536 XG块、18168文件块（含16632实际过滤块）在固定plain/dither策略全部逐字节匹配；D/R各296270063字节捕获SHA `b664deb00c0448bc77c7a7ef9b307aa3ece83581b560330f0647884f1811b13e`，原版重复稳定。旧168策略扫描仍为定位资料，不要求任意候选策略等于默认文件策略。
- 本批已验收独立BC源码 `ac6f8af69177ddeda86eb0b9abe09ed9798f7e2c` 再固定RGB/解码系数位模式，并将原版重复稳定性列为硬断言。本轮一次正式 `native-bc-files` [41027020522](https://github.com/wu162/NewSdk/actions/runs/41027020522) **completed/success**：完整D/R及逐次NoOriginal全部通过，捕获仍为同一296270063字节/SHA；完整归档、全部原/重复/固定候选记录、EOF和两份I386 DLL指纹已核对，见[验收收据](evidence/texture/handoffs/20261030-native-bc/acceptance.json)。仅补新组，复用64组件，未跑SDK/MOD。
- 恢复分支的正式native-loader仍拒绝压缩DDS，候选ABI尚未进入生产六DLL包。详细来源、修正与剩余门槛见[压缩DDS专页](TEXTURE_PUBLIC_D3DX_BC.md)。旧失败/partial capture及原锚点保留，不作为float gold。

## 当前继续实现：正式原生入口接入（未验证）

独立分支 `validation/texture-native-bc-integration` 从已验收恢复头989a89c2继续，已将公开BC静态嵌入正式NativeLoader/NativeCopy：文件入口调用同一DDS资源/mip消费，Copy仅开放已比较的LoadImageA flags0x12/threshold0/RGBA8完整锁定行路径。内部raw转换与压缩桥分开，避免递归，保留raw宿主编译参数和BC的scalar IA32参数。无运行时原版/系统D3D依赖。

已准备正式入口的完整矩阵：原14606文件全部保留，105压缩边界改为成功全字节对照，原两个缺层真实文件新增成功对照，另加1572完整BC文件；目标16180文件（1679压缩）、6860构造、19拒绝，三锁定视图/两RGBA/VM/所有权/NoOriginal不减。实际共享BuildNativeCopy的10组登记新增BC/私有头依赖。**这批源码、计数和接入尚未运行最终差分，不得标为支持或合入恢复分支。**

该独立分支的集中工作流新增显式手动 `native_bc_build_only=true`，仅编译D/R六Native DLL、独立BC DLL及两个完整宿主，并检查依赖选择边界；收据另存native-bc-build/compilation.json，status compiled-only，不能用于复用组件成功。普通测试/SDK选择规则不变，也不新增自动入口。编译源码 `d7766971738a12f31e2ec67d01bf9c1b5c2a2f28` / [41029403480](https://github.com/wu162/NewSdk/actions/runs/41029403480) **completed/success**：D/R各六Native DLL+独立BC DLL均I386，两个验证宿主均编译成功，19项依赖选择边界检查通过，plan/validate均skipped。[编译收据](https://github.com/wu162/NewSdk/blob/01926aa11a6d0d5351334450f71fafe6117281f1/recovery/evidence/texture/native-bc-build/41029403480-1/compilation.json) status=compiled-only，differentialExecuted/noOriginalExecuted均false。首个96994fea的41029238956仅因YAML scalar失败、未编译，失败保留；上述修正run不冒充最终差分。源码与证据保存于独立分支，尚未合入恢复分支或生产包。

## 下一轮计划

### 当前执行检查点（2026-10-30，JPEG/PPM原生链，尚未验证）

- BC正式接入的集中验收已启动：[41051807572](https://github.com/wu162/NewSdk/actions/runs/41051807572)，固定源码`e5e2c97981f4369331807cb17f76165d8509eab5`，显式native-bc-files并保留全部自动受影响组；结果待收，不以旧compiled-only替代。
- 继续实现保存在独立`validation/texture-native-raster-integration`：静态采用已验收固定IJG6b JPEG/PPM wrapper，接通原尺寸BGRA/X8和JPEG灰度L8；JPEG基础X=0、PPM基础X=FF分别保留。L8复用固定DirectXTex triangle并适配原luminance存储，双轴BOX整数路径与退化轴float路径分开。无新增运行decoder DLL或原DLL回调。
- 此检查点仅实现，原生资源/VM/mip/实际Copy消费矩阵仍在准备，尚未编译或差分，不合入恢复分支、不启用生产包。下一步补齐固定JPEG/PPM全语料原生消费、D/R与逐次NoOriginal，再按实际影响集中验收；先收BC批次结果，失败只修真实原因。

1. 接入批次已通过构建，下一轮直接从 `validation/texture-native-bc-integration` 的 `TestTextureNativeLoader.ps1` / `TextureNativeLoaderOracle.cpp` 继续。集中入口显式 `force_groups=native-bc-files, native_bc_build_only=false, run_sdk=false, plan_only=false`（各为独立输入）；保留自动叠加的所有受影响原生组，不把force_groups当只跑指定组。执行16180文件/6860构造/19拒绝、完整D/R及逐次NoOriginal，并补齐共享NativeCopy的真实消费；compiled-only不能跳过任何组件门槛。
2. 本批独立BC组件已闭环；保留 `ac6f8af6` / `41027020522` 的精确成功收据，未变输入不重复验收。已普通合入恢复分支 `989a89c2b9f8bb6a6cb845904f10e6815142d239`，未启用生产包。
3. 接入实现继续保持独立分支；有构建或差分失败只修真实原因，全部必要消费者通过前不合入/启用。
4. 新ABI正式进入生产包前，再在明确集成里程碑取得适用SDK证据。普通收尾不跑SDK/MOD；不扩展非PC。

## 已验收基础（2026-10-30，公开原生加载批次）

- 复用固定Microsoft DirectXTex triangle，接通原生TGA24/32和raw A8/X8 DDS的NPOT加载；各轴向上取2幂，DDS逐已提供层重采样，再从已上传层BOX生成缺失mip。保留X8物理未使用字节与端序、52字节header、主/secondary VM及packed tail所有权。
- 复用固定stb_image BMP，支持BI_RGB24/32的core12、INFO40、52、56、OS2-64、V4-108、V5-124，含方向、padding、真彩色表、忽略bfOffBits的原版实际像素位置及有界V5 profile。原版在BGR24展开前按源DWORD扫描alpha；适配器保留有界扫描规则，对会越过文件末尾而依赖未初始化堆字节的输入明确拒绝。来源/通知/修改前后SHA在`PublicDirectXTex`/`PublicStbBmp`。
- 业务源 `d0a31471906307690863e24784ca7384ef2371bf` / [41006229811](https://github.com/wu162/NewSdk/actions/runs/41006229811) completed/success：6860构造、14606文件（14501支持范围完整成功对照，包含5665新增NPOT与6152 BMP；105压缩DDS原版成功仍明确未支持）、另13拒绝。D/R及逐次NoOriginal全部通过；每配置634070359字节完整trace相同，四个归档原始成员SHA和12份x86 DLL指纹已独立核对，见[收据](evidence/texture/handoffs/20261030-native-public/acceptance.json)。
- 本次只补native-loader，复用63组；base保留 `34946af55168a43edd350ea12b8329833b16b8bb` / [41005120850](https://github.com/wu162/NewSdk/actions/runs/41005120850) 的完整成功收据。该首轮整体failure保留：BMP源alpha契约与宿主x86累计trace内存耗尽；修复后完整流式写盘/逐条NoOriginal读取/EOF核对，不减少样本或字节断言。当前64组件收据有效。
- 已普通合入恢复分支（合并提交 `710bae1200efe5f09067dcb90c0c8380b468fd72`），未启用生产六DLL包，未宣称完整混合模式身份或SDK集成；本轮未跑SDK/MOD。详细范围与边界见[原生文件加载](TEXTURE_NATIVE_FILE_LOAD.md)。
