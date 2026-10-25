# W3X：完整类型与真实SDK对照已通过

更新：2026-10-25（北京时间）。目标分支 `recovery/w3xcompiler-source`。本轮单代理，禁止强推。

## 当前结论与精确锚点

W3DAnimation/W3DMesh 已完成所列定义行为的源码恢复，并通过完整组件、正式可加载插件与真实BAB编译对照。最终验收源码 `ceeef68df4c032309154214f6746322e0a65e2ec`，SDK run `40169489711` attempt1，job `121097257454`，运行及全部步骤 success。已从下载artifact独立核对 sourceCommit/runId、组件/插件/SDK三个成功状态及三组四份输出逐字节相等，而非仅看工作流绿灯。

真实SDK按 EALAModStudio 使用的 `SDK/defaultscript.cs` case5 启动BAB基本数据编译。专用 `SDK/Mods/W3xRecovery` 包含8个动画、4个网格、两骨骼Hierarchy及纹理依赖；全量原SDK CnC3Types.xsd四文档验证通过。三个全新SDK副本分别使用原插件、恢复插件、恢复插件且物理移除原W3X DLL，均编译成功。

| 三组完全相同的产物 | 字节数 | SHA-256 |
| --- | ---: | --- |
| mod.bin | 10796 | f62716f91820e3307d3928473df973a02809f349d1428be28d518773bdee5193 |
| mod.manifest | 1439 | f548b09c30af61740781e28df509e49ac20d05ccb7f2f8a002d50bb28886bc2d |
| mod.imp | 76 | 6284f1cf8f0c2795e875dfcb79f0f4090185f9ced270a16bdde7fb29f20b75f3 |
| mod.relo | 788 | b657ddac1120c1a45fcedc26aa202e32721380a386968b5e9b7ffafd795c3831 |

恢复版与无原DLL版各捕获12次实际ProcessInstance调用，覆盖全部8动画/4网格；各36次三缓冲区比较，实例数据均出现在最终mod.bin中。恢复版轨迹由原插件重放逐字节对照；无原DLL轨迹由独立恢复版重放，检查原程序集未加载。SDK当前只验证Win32，组件平台矩阵保留0/1/2及动画99。

同一SDK运行先重跑并通过/Od、/O2完整组件矩阵和正式插件差分/无原DLL重放：每模式675动画、666网格原捕获；正式插件1341 fixtures，8052三缓冲区差分比较，独立无原DLL1341 fixtures/4029比较。完整数值域和34个命名XSD类型归类见 `W3xTypeClosure.json`，未定义行为安全拒绝边界仍保留。

## 本轮修复与证据

前一SDK run `40168310561` / source `34dfd46bf509cdf3e3e35767d310d61d5cabcf90` 的Original阶段报纹理文件不存在，未进入恢复版。经原Core.FileNameResolver.ResolvePath与HandleFileReferenceType确认，无别名File按声明XML所在目录解析。修复为 `../Art/W3xRecoveryTexture.tga`，并在完整XSD前置宿主检查文档相对纹理存在性；source `6930e9fb09bfa47ed77835f3670c8ae67135be4c` / prerequisite run `40169265269` 全部成功后才更新SDK_READY七树指纹并启动最终SDK。没有修改恢复算法或放宽断言。

最终不可变证据：`recovery/evidence/w3x/sdk-runs/40169489711-1`。`acceptance-evidence.zip`保存实际SDK的四种产物、预处理XML/三缓冲区轨迹、命令/配置/输入与工具指纹、日志和组件/插件验证JSON；execution.json记录归档指纹及原artifact来源。原artifact SHA-256 `3505be53de52f6bffb620505563a0602cad2e7a5b4c288257fa29ed66d63337e`。前次失败与修复后前置报告分别保存在 `sdk-runs/40168310561-1`、`prerequisite-runs/40169265269-1`。

## 剩余范围

本轮目标（所列W3X类型恢复及专用MOD真实SDK验证）已完成，无待修复失败。SDK配置和原DLL没有正式替换；可加载恢复身份为 `Recovered.Compiler.W3x.NativePlugin, Recovered.W3x.Plugin`，构建入口 `BuildW3xPlugin.ps1`。完整程序集同名身份/任意VC8原生指针对象ABI替换、完整mymod及游戏运行时验收不在本次证明范围，不能由专用MOD成功推断。原DLL取证SHA-256为 `bff82309b8e5e9f689112202e3613ad335148934c92dc70fe143f6e60d9ad3f1`，原始证据 `runs/40162934653-1`；历史组件记录见 `STANDALONE_REPLAY.md`、`NATIVE_VALIDATION.md`。

若继续合并或正式部署，先核对目标分支共享源码差异并执行受影响门槛；当前未合入其他分支。不要为本次纯证据/交接提交重复触发相同完整矩阵。未启动或管理Work/Codex任务。无账户额度计量，不能量化节省。
