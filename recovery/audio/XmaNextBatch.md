# EA-XMA 当前续接点

更新：2026-10-26（北京时间）。**编码器仍未完整恢复，representation28继续拒绝。**
原DLL SHA256：`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`。
地址均为RVA；不要把函数工厂、公开decoder或下面的已完成辅助组当成完整编码器。

## 已完成：不再重复调查/实现

| 组 | 源码/证据入口 | 验收 |
|---|---|---|
| 整数count/bit/frame/scan | XmaPacketBits / XmaPacketScanner | 96f72d0 / 40209977895，1025组 |
| float→signed16预转换 | [XmaPcmConversion](XmaPcmConversion.md) | 5d56d52 / 40232917060，124例/142193样本，两配置 |
| raw packet→EA输出重写ee00 | XmaOutputRewriter | 同上，801例含8原始raw到历史插件闭环 |
| 双状态PRNG/noise | [XmaCodecCore](XmaCodecCore.md) | 同上，6360组 |
| integral window/copy六叶 | [XmaPcmWindow](XmaPcmWindow.md) | 579c956 / 40233712043，6888组 |
| 98267 integral旧/新块调度 | [XmaPcmWindowScheduler](XmaPcmWindowScheduler.md) | de20ba2 / 40234805270，3240 synthetic＋111真实输入回放、15拒绝 |
| EA/WAVE/BE seek元数据 | [XmaCodecMetadata](XmaCodecMetadata.md) | 1ebd94a / 40233897932，384 synthetic；5495da2 / 40234514000，8真实state及原codec分配闭环 |
| 默认table-backed complex FFT a7de3 | XmaComplexFft | 03d98ce / 40235031462，order0..15双向/6pattern，共192组/6拒绝 |

除原整数packet历史行外，本轮各候选均以实际原Windows x86 CPU、Debug/Release完整位模式比较；安全拒绝另计。精确完整sourceCommit/runId和路径见[验收索引](../evidence/audio/acceptance-current.json)，不能把缩写或旧运行改写为后续提交的新验收。

## 当前第一任务

`97b00 -> [backend+1f0]` 的默认DCT样式核是 `a81db`，其FFT回调由 `[backend+204]` 提供，默认构造指向 `a7de3`。完整table-backed DCT组（N64/128/256/512/1024/2048）正在实现/验证；以新收据判断完成，不能因FFT已通过就宣称DCT通过。

下一步按完整数据流组推进：

1. DCT本身：原9槽ABI、66个原binary32系数、pre/post rotation各FSTP落点、原FFT回调及scale/输出参数，取得完整原CPU/两配置差分。
2. 实际DCT输入与输出捕获：从真实编码callsite截取，三份最终codec返回分配与未插桩原版一致，捕获数据再在隔离恢复宿主重放；不以synthetic输入冒充真实路径。
3. 后续分析/量化/bitstream编码核心、初始化/状态更新和完整raw packet生命周期；仅在这些完整闭合后才能启用编码路由并取得插件差分及里程碑SDK验收。

超出table域时若原版走CRT sin/cos，当前对应组件仍明确拒绝；不猜数值、不用系统数学库替代而放松逐位比较。通用generic reader、8/float window分支未恢复，不把integral61/78/94的结果扩大到这些表示。

## 可信原版输入/取证工具

- `cd66d9a2c821bc41f1f975aac8ac0a5bac6a6499` / 40232525056：直接调用589d0，8个mono/stereo、257/4096帧、RAM/stream案例各重复两次，保存ee00前raw与两个附属分配、descriptor、float及原e9b0转换后的signed16。三返回分配长度单位均为byte，用原free IAT ab1a8释放。
- `579c95661d90f067ab414c4133f033c2571b96e1` / 40233712043：来自固定main真实PCM8及长stereo的8例，16次原编码；121个packet、4个多block案例，输出重复逐byte相同。适合后续长块边界组，不重做短impulse作为全部覆盖。
- 同579c956 / 40233712043：纯native journal在97d97记录111次actual target，唯一为98267；全三返回分配不变。
- `de20ba24dc6941c56ee27ade3c897ece6a22b17e` / 40234805270：111个真实window快照，保存原32参数、系数位、short偏移及有界旧/新source，三返回分配不变。scheduler原CPU/Debug/Release都重放同111项，并交叉核对原128byte参数。
- 旧managed-VEH外围探针40232992739失败，不能作可用基线；不要重新采用有回调的断点方案。新native工具对特定实际site的成功也不冒充旧外围3site全部动态验证。

原始格式/方法调查：[XmaAudit](XmaAudit.md)、[PublicXmaReferences](PublicXmaReferences.md)、[wrapper反汇编](xma-wrapper-disassembly.txt)。原 `589d0 -> 59a60 -> 5d180 -> 659b8/6978f` 及编码循环 `59070 -> 5d5b0 -> 648b6`、flush `5d520 -> 60136` 是实际大codec；小工厂5a6d0仅18byte facade，绝非全部编码器。

## 验证约束

用户要求不要频繁触发完整SDK真实验证。现在以独立数值/结构组、定向原版取证和真实输入重放推进；SDK留到完整管线接入的更大里程碑且显式启动。未变组复用既有精确锚点，原DLL/输入/相关依赖改变的组必须重新验证。运行时不得读/加载原AudioCompiler作为恢复后端；测试中的原版oracle与恢复宿主分开。
