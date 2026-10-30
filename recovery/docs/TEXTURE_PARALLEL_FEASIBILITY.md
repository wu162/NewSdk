# Texture 三路并行可行性验证

基线：`recovery/legacy-sdk-cache` / `d85b8c8c0ddc688ea3614ab70a3439a7c41b7eac`。实验分支：`test/texture-parallel`。本轮只增加探针、独立测试宿主、工作流与证据；未修改生产编译器、缓存格式或并行资格表。

## 结论

**有条件可行：独立加载 DLL 的原生模块隔离方案已通过原版及新 EXE 绑定的 Debug/Release 组件验证，三种路由与混合组均实际四路同时执行、输出原字节一致。不能直接把当前共享 LegacyTextureAdapter 放进并行队列。**

原 DLL 的 Texture、OnDemandTexture、TerrainTextureAtlas 共用同一份原生 `g_Buffer`。2D/3D 的 CompressAndEmbed 先 Reset，再将这个指针写入压缩选项的 callback user data，最后从同一缓冲区生成输出。全局 g_pD3DDevice、g_pD3D 和 g_Bundler 也属于模块。多个 Plugin 对象不会生成多份模块全局存储。

普通共享实例在 Windows x86 上出现了实际错字节、长度为零异常和访问冲突。给整个调用加锁可以保持正确性，但不能提供 texture 与 texture 之间的同时编译。使用 `Assembly.LoadFile` 从不同完整路径加载相同 DLL 副本时，本轮观察到不同的 native HINSTANCE；每份模块拥有独立状态，四个 worker 可同时执行。

## 已取得的组件结果

[原版实验 41071099626](https://github.com/wu162/NewSdk/actions/runs/41071099626)，源码 `101538e463fea6b1f2335a6b46c89dab98c0faeb`：

| 调用组 | 共享 DLL，无锁 | 共享 DLL，全局锁 | 四份独立原生模块 |
| --- | --- | --- | --- |
| Texture | 实际输出错字节 | 通过，峰值 1 | 通过，峰值 4 |
| OnDemandTexture | 访问冲突，退出码 -1073741819 | 通过，峰值 1 | 通过，峰值 4 |
| TerrainTextureAtlas | 实际输出错字节 | 通过，峰值 1 | 通过，峰值 4 |
| 混合三路 | 实际输出错字节 | 通过，峰值 1 | 通过，峰值 4 |

每组先在同一进程串行生成四份不同内容的参考输出，再让四个独立线程同时起跑，运行三轮。逐字节比较 InstanceData、RelocationData、ImportsData 与 CDATA 的内容和存在性；每个隔离组 12 次调用全部相同，保留实际线程区间和原生模块地址。后续 EXE 验证源码还增加每轮 ReInitialize、运行/Dispose 前后内存记录及一次性收集同轮全部字节差异；其验收结果见下节，不冒充本表来源。

这份运行整体为 **failure**，因为当时控制器将预期的共享重入拒绝也判作工作流失败。上表隔离与加锁进程均 exit 0、结果 passed；无锁失败原样保留。后续控制器分别报告共享重入状态与隔离方案的成败，不把无锁失败变成通过。

第一份完整三路对照 [41070763951](https://github.com/wu162/NewSdk/actions/runs/41070763951) / `39b39b49` 也失败：Texture 的 InstanceData 变化、OnDemandTexture 的 CDATA 变化、TerrainTextureAtlas 抛出 `Texture data has length 0`，混合组错字节；四个加锁组通过。最初 [41070562887](https://github.com/wu162/NewSdk/actions/runs/41070562887) 是 Framework csc 输入路径未规范化造成的**探针构建失败**，不是纹理结论；修正路径后才取得上述结果。

## 最终新 EXE 绑定验证

[41071696527](https://github.com/wu162/NewSdk/actions/runs/41071696527) 整体及全部步骤 **success**，测试/宿主源码为 `0dc6027b0eac86f79bc6e72f6f8bfff12e1e402c`；后续归档和文档提交没有修改该运行的构建或测试输入。

- 原版、Debug EXE、Release EXE，分别覆盖三种路由及混合组，每组全局锁/独立模块两种模式；24 个正向进程全部 exit 0。加锁真实峰值 1，独立模块真实峰值 4。
- 每个隔离进程都验证四份 native HINSTANCE 不同，四个 worker × 三轮，共 **144 次隔离调用**与串行输出相同；另外 144 次加锁调用也全部相同。每轮工作前执行 ReInitialize，所有工作结束后执行 Dispose。
- 两个配置从当前生产 Foundation 源码编译，Core 类型实际属于最终 NewModCompiler.exe；DLL 仍使用既有 Core 引用重定向，两份保护 PE 审核均 passed。编译时 netmodule 移走后运行，未借原 Core DLL 伪装新 EXE 绑定。
- 工作流校验各宿主的原版输出 SHA256 清单。外层 ZIP 下载及 SHA256 复算后，本地进一步将所有正向进程的串行参考和三轮结果直接与原版 locked 的 `.bin` 文件比较，合计 **1,272 个已生成的缓冲区文件逐字节相等**（包含零长度缓冲区，其中 Debug/Release 跨运行时对照为 848 个），同时要求文件存在集合一致；不是仅比较长度或摘要。
- 本次无锁负向仍为四组拒绝：Texture/Atlas/混合组错字节；OnDemandTexture 卡住，120 秒后由隔离控制器终止。结合前两轮的 CDATA 错写、长度为零异常和访问冲突，不能将某次偶然不崩溃解释为可重入。

[原始收据、内存快照与来源摘要](../evidence/texture-parallel/41071696527-1/summary.json)。最终归档 artifact `12986600064`，大小 69,948,281 字节；已实际下载并复算 SHA256：`9330ecc9b9104fb3822dc42aaf6c9d9fc30cfeaf234a13192d6d1f3abd62ef32`。

新增四个模块后，三种运行环境各组 private bytes 采样增加 **264.33–264.70 MiB**。Release 的四组执行后采样为 355.96–370.99 MiB，五个插件 Dispose 后为 29.49–45.00 MiB。这是进程快照，包含 DLL/运行库和仍存活的托管参考输出，不是连续内存峰值，也不把 Dispose 解释为程序集已卸载。原生构造器的 64 MiB 初始分配另有反汇编证明。

## 内存、生命周期和生产接入边界

- 原生 SimpleBuffer 构造参数为 67,108,864 字节，即每份模块初始 64 MiB；Append 容量不足时按倍数扩容。该成本独立于输入 XML 大小，不能直接沿用当前只估算 XML 的准入。
- 实验有一个生成参考的默认模块，加上四个隔离模块；因此同时存在五份原生状态。正式方案应复用有界实例池并明确计算常驻空间、解码图像、压缩工作区和输出，不能无限加载副本或仅用输入文件压缩大小估算。
- 每个模块同一时刻只允许一个插件调用；Initialize/ReInitialize/Dispose 必须与该模块的 in-flight 工作互斥。生产任务还要接入现有的输入文件租约、私有 CDATA 路径、有序提交及失败/取消 join。
- .NET Framework 的 LoadFile 程序集不能单独卸载。Dispose 释放原生资源不等于卸载程序集；池需复用已加载模块并处理副本文件生命周期。
- 不改变会话/资产缓存的内容身份。热缓存命中应继续跳过纹理实例的加载、准备和编译。

以上是接入条件，**本分支没有实现或宣布生产 worker 认证**。这里的同时执行证明不是完整 MOD 构建加速报告，也没有受控的端到端性能对照。

## 输入和覆盖

生成的四份 256×256 BGRA TGA 内容不同。Texture/OnDemandTexture 为 StandardTexture、DXT5、GenerateMipMaps=true、禁止自动缩放；TerrainTextureAtlas 为 2048 atlas、一个 256 tile、Base/Normal 源及 AllowLossyCompression=false。这是能直接触发共享缓冲区竞争的输入；并非每种 DDS、Cube/Volume、自动格式、尺寸、中文路径或错误组合都已覆盖。

组件成功不等同于生产 CLI、缓存/jobs 矩阵、取消/失败排空、完整 mymod/Upheaval 或游戏内验收。原生产源码未变，已有生产验收没有作为本轮全量重测来宣称。

## 复现与证据

Windows 2022/.NET Framework 4.8 x86：

```powershell
./recovery/scripts/TestTextureParallel.ps1 -OutputDirectory <新证据目录>
./recovery/scripts/BuildTextureParallelHost.ps1 -OutputDirectory <新宿主目录>
./recovery/scripts/TestTextureParallel.ps1 -HostDirectory <新宿主目录>/Release/bin -OutputDirectory <新证据目录>
```

构建宿主需要 MSVC x86、.NET SDK 8、原 SDK 工具和固定的 PE 审核依赖。统一入口为 [Texture native reentry experiment](../../.github/workflows/texture-parallel-probe.yml)。新宿主使用当前源码 Foundation，Core 类型属于最终 EXE；原 DLL 只按既有构建流程改变 Core 引用，继续执行保护 PE 审核。编译时 netmodule 移到隔离目录后再运行。

静态依据：[native-state.json](../evidence/texture-parallel/static/native-state.json)、[构造器](../evidence/texture-parallel/static/buffer-constructor.txt)、[Append](../evidence/texture-parallel/static/buffer-append.txt)。可用 `dnfile==0.18.0`、`dncil==1.0.2` 运行 [AuditReentry.py](../probes/TextureRedirect/AuditReentry.py) 复现 CIL 与字段位置。

三份成功取得有效组件结果的归档均已实际下载并复算 GitHub artifact SHA256；前两次整体失败、最终一次整体成功，范围分别如上：

| 运行 | artifact | 外层 ZIP SHA256 |
| --- | --- | --- |
| 41070763951 | 12984887761 | 2f81011e5da1423eb33dee9b80ced09ca9607a3eea925a31bb67aa34728c1e43 |
| 41071099626 | 12983814828 | 21593d0fc4c7e88a971d02781bb431b11296926411c3bb45ca6449e96d1f77e5 |
| 41071696527 | 12986600064 | 9330ecc9b9104fb3822dc42aaf6c9d9fc30cfeaf234a13192d6d1f3abd62ef32 |

仓库保留原始 result/validation JSON、stdout/stderr 与来源；完整原字节保留在各运行 artifact，避免再次将大量重复二进制入库。
