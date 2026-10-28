# PC JPEG 恢复入口

当前已收齐原版取证，并接入JPEG候选业务路由；源码699479a9 / Actions40626161229仍在完整组件验证，尚无业务或SDK验收。不得把probe-only收据或正在运行的批次写为恢复完成。

原 DLL SHA256 `4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e` 包含 IJG 的 JPEG 错误文本及 `Copyright (C) 1998, Thomas G. Lane`。这只构成选择公开参考库的依据，不能证明 D3DX 的调用参数或 JPEG 输出与该库相同。

`recovery/reference/ijg6b` 保存来自 https://www.ijg.org/files/jpegsrc.v6b.tar.gz 的未修改源码子集，压缩包 SHA256 `75c3ec241e9996504fe02a9ed4d12f16b74ade713972f3db9e65ce95cd27e35d`；逐文件指纹见 PROVENANCE.json。保留原 README 授权、版权及无担保声明和 NOTICE.md。该软件部分基于 Independent JPEG Group 的工作。只在 Actions 临时目录从 jconfig.vc 复制 jconfig.h；原文件不改写。

`jpeg-load` 是 manual_only/evidence_only 组，通过统一 texture-validation.yml 手动 force_groups 或提交标记 [texture-jpeg-load] 显式选择。构建参考 cjpeg 生成输入，原 LoadImageA 对每份输入调用两次并核对完整 RGBA、alpha、维度、异常与源文件不变，再与六个候选模式比较：ISLOW/IFAST/FLOAT IDCT × box/fancy upsampling。候选只消费公共 Image 和已验收 NPOT resampler；不调用插件 JPEG 路由，也不是恢复验收。

完整计划语料：8种尺寸（含1/2小边、POT及NPOT）×4图案×4质量×5色彩/采样布局（灰度、444/422/420/411）×顺序/渐进=1280，另4个有界截断，共1284例。每例保存原输入和原完整 RGBA、全部可成功候选 RGBA及模式哈希。不得只看总匹配数；必须按布局/尺寸/拒绝分类定位差异，并为未知/失败保留显式边界。此时没有 JPEG 组件或 SDK 成功声明。

若候选得到完整像素证据，下一步再做正式源码 helper、独立 Debug/Release 无原 DLL 回放、公共加载/两插件完整 buffer/cdata 与专用 MOD 三模式 SDK。新 native 依赖必须同步进入实际消费者的构建/复制及依赖表；不能仅增加 P/Invoke 而漏掉原 raw/压缩/atlas 消费链。生产 SDK DLL/config 未替换，DXT1/1A 未初始化状态仍为独立边界。

## 2026-10-28 11:23 北京时间交接

用户要求先交接。本地/远程业务头699479a95c73012f5e66b74a3d7d1e43f1d8b51f，Actions40626161229 in_progress；先收取终态和各组收据，不重复启动。该run没有SDK标记。新增sdk-PcJpeg准备稿尚未执行。

原版基线3f2351f2/40625499544已经success（probe-only）：1284记录，1280成功/4BadTextureFile，2568原调用。IFAST/fancy=1274完整匹配，IFAST/box=1154，其余模式272或274。六个差异均是灰度NPOT：7×5随机q95两种扫描，17×9棋盘q30、随机q100各两种扫描；每例恰好一个RGB像素各通道差1。POT灰度和全部彩色IFAST/fancy匹配。samples.zip SHA256 b758465f88b831f5952e4700531ffe5b83a9696be6889a8b9d0defdc1eee24a1，包含原JPEG和所有完整原/candidate RGBA。

候选业务699479a9固定IFAST/fancy，native按header真实jpeg_color_space报告灰度；managed对灰度NPOT使用既有L8量化/亮度存储，RGB使用RGBA8，保留原HasAlpha=false。六个差异是否闭环须看新jpeg组D/R完整1284隔离回放，不靠理论或1274/1280的部分通过作完成。桥接只接受1/3分量、1..8192源轴与<16M像素，其他未声明JPEG域不扩大支持。

新jpeg-images完整11248例：原全部输入的raw/两根/cdata路径+80 focused输入的raw3/DXT3/5、标准/volume、有无mip与旧新cdata矩阵+32 JPEG编码atlas base/normal变体，原版全buffer/cdata及逐次NoOriginal，D/R各自native/managed构建。TestTextureJpegSdk.ps1计划3216资产（三模式每模式3200标准/volume纹理+16 atlas），仅保存准备稿，尚未运行。组件成功后再显式SDK；生产SDK不替换。

构建修复历史：40609293267因cl /TC把jpeg.lib当C输入失败；40625214348因多余_EXPORT别名失败，未采原输入；3f235移除别名、显式I386检查与实际导出记录后通过。所有失败收据保持原状态。
