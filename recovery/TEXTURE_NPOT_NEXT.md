# PC NPOT 当前调查入口

仅PC，recovery/texturecompiler-source，禁止强推，编译测试只在GitHub Actions。当前true-color TGA24/32、PNG RGB/RGBA8、BI_RGB BMP24/32的NPOT加载及标准/volume raw+DXT、无mip/完整mip已完成下述组件/入口/专用SDK验收。生产SDK未替换；其他存储/量化路径仍拒绝。下文按阶段保留取证与失败来源，当前状态以此入口及末段完整验收为准。

原Plugin.ProcessTexture@0x06000106在PC无独立缩放分支，先调用LoadImageA@0x06000017（IL RVA0x3214），再IsValidTexture/GuessProperFormat和二维或volume压缩。LoadImageA调用D3DXCreateTextureFromFileA@0x060000E8（native RVA0x13C82），读取XGGetTextureDesc、LockRect，再以XGCopySurface目标0x1A200006/pitch=width*4转RGBA。静态来源：evidence/texture/runs/40191971356-1/{methods.json,decompiled/BinaryAssetBuilder.TextureCompiler/Plugin.cs}，原DLL SHA256固定4f559428...e9d9e。不能假定压缩器负责全部NPOT处理。

定向取证组npot-load：23组尺寸（19 NPOT与4 POT对照），TGA24/32八图案、PNG RGB/RGBA8交错/非交错四图案、BMP24/32正反向三图案，共1012记录。原LoadImageA仍使用本模块VC8 malloc/free及nvImage构造析构；核对token与thunk、输入不变、POT对照尺寸。保存原输入及加载后完整RGBA、尺寸/HasAlpha/错误，probe-only，不调用恢复NPOT。辅助恢复程序集只提供既有Image记录与fixture源码编译依赖。

收取后先核实目标尺寸、边缘/alpha/舍入及缩放所在层，必要时定位原native滤波。证据不足不启用近似算法，不以探针成功冒充支持。完整能力组需原LoadImageA差分、各实际图像解码消费者、raw/DXT二维及volume插件三缓冲区/full cdata、Debug/Release与NoOriginal、适用真实SDK三模式及四产物。支持子集必须明确界定，未知PNG16/索引/metadata等不能静默进入近似路径。

## LoadImageA取证已收取，numeric取证准备中

源码 `a19bb879704cd80ad55202760a160c8af57bbe2d` / [40528237009](https://github.com/wu162/NewSdk/actions/runs/40528237009) completed/success，1012全部解码，probe-only。POT对照尺寸不变，NPOT各轴向上取2幂。TGA3×1灰阶[0,127,255]加载为4×1 [48,79,175,207]，不能用clamp线性插值。native原清单40191971356 artifact已核对：D3DXCreateTextureFromFileA@0x13c82→0x13b53→0x13a9b→0x134b8；0x1359e/0x135c9分别向上取2幂，0x1360d默认filter=0x80004。默认0x19b3a调度到0x194fc（2D triangle），0x18ad8生成带回绕的面积积分权重；0x19704将X/Y float权重相乘存float，再每输入像素RGBA乘加存float，不能换成两pass量化近似。

定向npot-numeric继续采原0x18ad8贡献表位模式、常量/格式vtable和初始化后dither表，不重复1012加载样本。注意D3DX table allocator@0x12074/free@0x1208a使用进程堆，与nvImage的VC8 CRT malloc/free不同，不可混用。当前仍不启用恢复NPOT；量化与float精度必须完整对照后才能声明支持。

## numeric已收取，隔离RGBA8组件验证中

numeric源码`678db0127a2ea3620700b44e46093dd6dc2050d1` / 40529918922 completed/success、probe-only。固定权重阈值float=1e-5；字节归一化用double存储的float倒数0.0039215688593685627。原RGBA8行读取@0x20bb4，写出@0x1cfd0（vtable@0x99df8）；写出设置x87 RC=toward-zero，先sample*255存float，再加抖动存float，最后截断整数，恢复调用者CW。抖动4×4取(2k+1)/32、每行蛇形遍历；动态表@0xb9048与无抖动表@0xb8fc8已收取。

新增独立TextureNpotRgba8Resampler与npot-rgba8组件组，**待验证，不接ImageLoader或插件**。按原0x18ad8 float存储、回绕积分权重和0x194fc source y/x、dest y/x顺序恢复，量化用float位模式前驱模拟正值向零存储，不改调用者FPU状态。初步域为RGBA8、正尺寸<=8192、向上取2幂后<16M像素；PNG16、BMP16/TGA16等源/目标低位深格式不可直接套用此8位量化。

完整组件门槛：Debug/Release逐位比较numeric原表全部tap与1012个不可变原LoadImageA完整RGBA，原输入不变、POT对照、每次调用前后原DLL不存在且未加载。原样本ZIP、权重TSV及收据sourceCommit/runId/祖先/hash均检查；探针仍是原版基线，只有新组件全部比较通过才算组件验收，不能据此宣称插件/SDK NPOT支持。

首轮隔离组件源码826ff73e/40531802530失败于回放宿主BMP元数据，未完成全矩阵，不作验收。宿主错误使用bfOffBits并假定BMP24无alpha；现按既有已验收TextureBmpDecoder的D3DX契约（强制offset54、BMP24 HasAlpha=true、BMP32按实际alpha字节检测）修复，保持所有原尺寸/alpha/完整RGBA断言，原样本与native表不变，待完整Debug/Release复验。恢复滤波源码未因此变更。

## RGBA8组件已验收，8位入口集中验证中

最终组件源码`e267d829b781681d4c846ced9bbfa872d8ee3ef5` / [40532546015](https://github.com/wu162/NewSdk/actions/runs/40532546015) completed/success。Debug/Release各1012完整原RGBA、1851系数tap精确位模式、814468检查通过，每次调用前后原DLL不存在且未加载。原826ff73e/40531802530失败保留，未放宽alpha或像素断言；修复的是BMP回放宿主，滤波源码未变。

下一整组已准备：TGA24/32、PNG RGB/RGBA8、BI_RGB BMP24/32的NPOT解码接入同一helper；只有原始8位存储/目标8位量化域，PNG其他位深/颜色和BMP16/Additional TGA16等NPOT仍拒绝，POT原能力不变。公共解码分派加入同1012不可变原RGBA全量对照；隔离回放手工读取原TGA fixture，避免启用后的TGA Decode预先重采样而遮蔽组件测试。

原Image/Mip/Volume/VolumeMip TGA入口中NPOT“拒绝后跳过”改成实际原版三缓冲区/full cdata对照，不减少样本。dxt-images保留旧7856并新增6400：80个已捕获NPOT源输入（四种合法加载后volume形状×十个存储/方向profile×两图案），两根/五输出raw+DXT/有无mip/标准+volume/新旧cdata，完整NoOriginal重放；34明确拒绝保留低位深NPOT和默认DXT1/1A。新PcNpotRgba8专用SDK同80输入、3200资产三模式与逐次隔离；选择器同步全部实际8位decoder/helper消费者，包括PNG/BMP/TGA组件及raw/DXT/volume/SDK回归。所有新集成仍待本轮完整Actions终态。

集成批次源码a945c18c/40533831752整体failure，17组（含全部6个SDK profile）实际通过，唯一npot-rgba8失败于独立TGA fixture提取遗漏旧“全零alpha转opaque/HasAlpha=false”规则（tga-d32-1×1-p3）。现仅修复测试宿主，生产滤波/解码器/SDK输入不变，保留所有完整断言与失败记录；通过组继续保留原sourceCommit/runId，最终只补该组件并请求选择器核实/复用其余成功组。不得将40533831752改写为整轮success。

## 最终8位NPOT集成验收（2026-10-28，北京时间）

业务源码`a945c18c8ea02b6ad03f8a4b6172be10ba7dffa2` / 40533831752的17个实际组通过；整轮因唯一npot-rgba8宿主TGA全零alpha规则遗漏而failure，保留。最终仅修复宿主，源码`d012da8ae55c094220d9c506b5263b277a49042b` / [40536063863](https://github.com/wu162/NewSdk/actions/runs/40536063863) attempt1 completed/success：只运行npot-rgba8，完整复用其余31成功组。各复用收据保留原sourceCommit/runId，不把复用标为新跑。

- Debug/Release各1012完整RGBA、公共TextureImageLoader同量比较、1851原系数tap位模式、1,361,709检查；TGA/BMP两种既有canonicalization均在独立fixture读取中正确保持，源像素不变，逐次无原DLL/未加载。
- 两入口每配置14256输入（12736成功输出、1520原版拒绝匹配）、34明确拒绝、66,334,496字节比较及同量NoOriginal；包含80 NPOT源×raw3/DXT3/5×标准/volume×有无mip×新旧cdata。旧raw TGA NPOT“拒绝后跳过”全部转为严格原版buffer/cdata对照；相关TGA/PNG/BMP/范围与图像回归均成功。
- 新PcNpotRgba8专用MOD：80输入（3×1、3×2、5×3、7×5四形状×十个存储/方向profile×0/2图案），五输出格式×两入口×标准/volume×有无mip=3200资产。Original/Recovered/NoOriginal各3200实际Core调用，四产物和完整轨迹相同；NoOriginal每次调用前后断言原DLL不存在且未加载。收据`evidence/texture/sdk-npot-rgba8/40533831752-1/validation.json`。
- 原PcNoMip/PcMips各600、PcDxtNoMip/PcDxtMips各712、PcDxtVolumes各1488资产/调用三模式均通过；新共享helper消费关系已登记。颜色/alpha/直接DDS等未变成功组复用，不额外重测。

边界：源各轴1..8192、向上取2幂后小于16M像素，POT保持原路径。仅上述RGBA8原始存储/目标8位量化；PNG非RGB/RGBA8、BMP16、Additional TGA16/灰度/调色板等NPOT保持明确拒绝，默认DXT1/1A仍未恢复。直接Raw/DXT DDS编码API仍要求POT；NPOT发生在图像加载层。生产SDK DLL/config与完整mymod均未替换或宣称通过。

SDK收据的通用scope字符串沿用复用脚本的旧POT/DXT措辞；实际PcNpotRgba8范围由profile、80份fixtureFiles、input-Mod.xml的3200资产、三模式/四产物/完整轨迹及本段共同界定。补充见同目录SCOPE.md，不改写原始validation.json或来源锚点。

## 下一完整能力组

恢复余下NPOT源存储/目标量化：先读取原D3DX格式descriptor/格式vtable与row getter/setter，再核对过滤前的原始样本精度、向零存储/抖动以及XGCopySurface最终RGBA转换。RGBA8的已验收贡献表/遍历契约可复用，但PNG RGB16存在旧descriptor/行布局异常，BMP/TGA16可能先按5/6/1位量化，不能用解码后8位值近似。源格式变化应按连贯量化组统一完整原版差分、拒绝、公共加载器与两入口、NoOriginal以及适用SDK；不要拆成反复小批触发。

已拒绝的原版PNG索引/部分GA16、BMP索引/bitfields等保留原错误，不把原版拒绝误列成缺失支持；ICC/APNG及未声明TGA/BMP metadata仍按既有边界。优先实际PC能力，默认DXT1/1A未初始化字段、完整atlas和生产集成为其他独立门槛。

## 2026-10-28 下一轮存储完整组调查中

上一轮已闭环并在4857ff70保存交接。本轮新npot-storage原版定向取证共4986输入：PNG低/高位深与灰度/GA/RGB/RGBA、Additional TGA16/灰度/调色板、BMP16及既有原版拒绝域，9种NPOT/POT对照形状、PNG交错/metadata/图案和TGA RLE/方向/alpha。除最终LoadImageA RGBA外，记录原D3DX创建后的format/width/height、LockRect pitch及完整原存储。锁定行可能含未定义padding，只作取证，不把raw padding当恢复契约。两次创建/加载的成功/拒绝须一致，原输入不变；probe-only，不重复上一轮RGBA8/颜色/压缩/SDK成功组。
