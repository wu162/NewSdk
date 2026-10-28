# PC JPEG 恢复入口

当前公共插件尚未支持 JPEG。仅准备取证，不把候选解码器或 probe-only 收据作为恢复或 SDK 验收。

原 DLL SHA256 `4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e` 包含 IJG 的 JPEG 错误文本及 `Copyright (C) 1998, Thomas G. Lane`。这只构成选择公开参考库的依据，不能证明 D3DX 的调用参数或 JPEG 输出与该库相同。

`recovery/reference/ijg6b` 保存来自 https://www.ijg.org/files/jpegsrc.v6b.tar.gz 的未修改源码子集，压缩包 SHA256 `75c3ec241e9996504fe02a9ed4d12f16b74ade713972f3db9e65ce95cd27e35d`；逐文件指纹见 PROVENANCE.json。保留原 README 授权、版权及无担保声明和 NOTICE.md。该软件部分基于 Independent JPEG Group 的工作。只在 Actions 临时目录从 jconfig.vc 复制 jconfig.h；原文件不改写。

`jpeg-load` 是 manual_only/evidence_only 组，通过统一 texture-validation.yml 手动 force_groups 显式选择。构建参考 cjpeg 生成输入，原 LoadImageA 对每份输入调用两次并核对完整 RGBA、alpha、维度、异常与源文件不变，再与六个候选模式比较：ISLOW/IFAST/FLOAT IDCT × box/fancy upsampling。候选只消费公共 Image 和已验收 NPOT resampler；不调用插件 JPEG 路由，也不是恢复验收。

完整计划语料：8种尺寸（含1/2小边、POT及NPOT）×4图案×4质量×5色彩/采样布局（灰度、444/422/420/411）×顺序/渐进=1280，另4个有界截断，共1284例。每例保存原输入和原完整 RGBA、全部可成功候选 RGBA及模式哈希。不得只看总匹配数；必须按布局/尺寸/拒绝分类定位差异，并为未知/失败保留显式边界。此时没有 JPEG 组件或 SDK 成功声明。

若候选得到完整像素证据，下一步再做正式源码 helper、独立 Debug/Release 无原 DLL 回放、公共加载/两插件完整 buffer/cdata 与专用 MOD 三模式 SDK。新 native 依赖必须同步进入实际消费者的构建/复制及依赖表；不能仅增加 P/Invoke 而漏掉原 raw/压缩/atlas 消费链。生产 SDK DLL/config 未替换，DXT1/1A 未初始化状态仍为独立边界。
