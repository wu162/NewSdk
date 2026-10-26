# SWF 直接编译 · 续接入口

## 当前状态：已采用项目源码

分支 `feature/swf-direct-compiler`。用户要求直接采用现有项目，并允许上传工具用于一致性对照。本轮已完成源码拷入及样例验证；不再以临时checkout和字符串补丁作为正式实现路线。

正式源码：`ui/vendor/swf2apt/`，来自 `lanyizi/swf2apt@2b3cb662b7311616e462b884e5611750e5de88f4`。保留原目录及许可证；仅未复制上游未使用的预编译 `zlib/crc_i386.obj`，原生工具从源码构建。源码导入提交：`662a74cddcf17539386eb6a775df9f107bc9a2c7`。

## 已验证证据

- Actions：[40288873546](https://github.com/wu162/NewSdk/actions/runs/40288873546)，最终success；验证框架提交 `a9adf26ed130f17b7ee18b5fa7efcda3efaea918`，实际编译源码固定在上述上游提交，验证后原样写入导入提交。
- 记录：`ui/evidence/source-consistency.json`，artifact `ui-source-consistency`。
- 先核对附件PDB与EXE内原始程序集的CodeView GUID，均匹配；再核对编译时源码SHA256：eaf2apt 33/33、SwfLib 357/357，共390/390一致。每个项目另有2个构建自动生成文件不参与源码比对。
- 最初候选60b15a67只有FillStyleFactory.cs的报错信息不同，改用其父提交2b3cb662后全部一致；不是改动校验值掩盖差异。
- 直接执行从附件EXE提取、未经修改的eaf2apt/SwfLib程序集及原flasm，与从导入源码构建的原Program入口/原生flasm比较，样例 `CoronaUIMain.swf` 的普通RA3和 `--ra3 --optimize` 两种模式均成功。
- 在相同输入路径逐字节比较所有生成文件：普通模式5个（apt/const/dat/hash/xml）；优化模式8个（另有eaf/flm/tac），**全部一致**。
- 优化模式APT=10121字节、CONST=9787、DAT=48；未优化APT=25861、CONST=25179、DAT=48。

边界：原始托管程序集和源码版统一运行于 **.NET8 x86**；没有复现附件完整自包含EXE的.NET5原生宿主。当前证明的是此样例输出一致、原始托管源码对应；不是所有SWF功能或跨运行时游戏行为已经验证。样例没有视觉、字体和音视频，后续扩展需相应样例。SDK集成和游戏加载未验证。

## 使用与文件

- `ui/README.md`：直接从已导入源码构建CLI。
- `ui/VENDOR.md`：来源、基线工具、许可和使用决定。
- `ui/verification/DirectCli.xml`：构建适配器；直接编译原源码与原Program，未修改转换算法。目标net8.0-windows/x86。
- `ui/verification/verify_source.py`：PDB源码指纹、原始/源码CLI对照和输出指纹。
- `ui/reference/uploaded-tool-source-checksums.json`：源文件及PDB/程序集校验。
- `ui/reference/original-managed-tools.zip`：用户明确批准上传的最小基线包（原始程序集、flasm、jpeg及校验清单）；不含整套运行时。
- `.github/workflows/ui-source-parity.yml`：验证后保存源码，已获根AGENTS.md中工作流普通提交/推送授权。禁止强推或自行合入。
- `ui/RESEARCH.md`：历史调查与BAB接入设计，按需读。
- 旧 `prepare_memory_backend.py`、`MemoryAssets.cs` 和memory工作流只是第一轮受限原型；其证据仍有效，但后续正式内存实现应直接基于导入源码，不再扩展字符串替换路线。

## 下一阶段

1. 将已验证的源码接口改为输入字节/输出bundle。先内存化flasm的RA3优化流程，保留迭代及错误语义；原工具基线仍用于输出差分。
2. 前端把SWF展开成既有AptAptData/AptConstData/AptDatData，专用插件从bundle返回AssetBuffer。Core单插件只返回一个资产，不能直接返回整个资产组。File可引用真实原SWF以保留依赖/hash。
3. 获得精简BAB对照，比较manifest/bin/relo/imp后再启用生产路由。随后覆盖geometry、纹理、字体和非空Sprite。
4. PDB逐文件相同检查只针对本次原样导入基线。主动改源后应保留这份原始证据，并按明确改动更新验收入口；不篡改原工具指纹来制造一致。

继续遵守：独立ui交接，不加载recovery历史任务；复用有效证据，不重复未变化类型或完整SDK验证；及时保存成果；合入必须PR并经用户明确批准。本轮已普通推送，未合入。
