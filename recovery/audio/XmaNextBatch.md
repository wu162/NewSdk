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
| 默认table-backed complex FFT a7de3 | [XmaComplexFft](XmaComplexFft.md) | 03d98ce / 40235031462，order0..15双向192组/6拒绝 |
| table-backed DCT-IV a81db | [XmaDct4](XmaDct4.md) | d3f9600 / 40236037500，360＋111真实重放/9拒绝 |
| 完整97b00 signal wrapper | [XmaSignalTransform](XmaSignalTransform.md) | f683edd / 40236818983，5760＋111真实重放/13拒绝 |
| 可选855ef谱衰减 | [XmaSpectralGate](XmaSpectralGate.md) | 5bc9328 / 40237077516，2400组/11拒绝 |
| raw packet构造/bitsplice | [XmaRawPacketWriter](XmaRawPacketWriter.md) | d3f9600 / 40236037500，1106组；93ba818 / 40236621021，92实际header/next观察 |

除原整数packet历史行外，本轮各候选均以实际原Windows x86 CPU、Debug/Release完整位模式比较；安全拒绝另计。精确完整sourceCommit/runId和路径见[验收索引](../evidence/audio/acceptance-current.json)，不能把缩写或旧运行改写为后续提交的新验收。

## 下一轮直接任务

从[XmaAfterTransform](XmaAfterTransform.md)继续原channel分析/量化/entropy/初始化与状态驱动。完整signal函数返回点为8c0b4，之后caller额外mask/copy、8c133等分析分派不属于已恢复97b00；855ef只在独立范围验收。

完整5e8f0帧appender、跨packet分割/continuation、输出FIFO与码率时钟仍缺。raw packet helpers虽有1106组件样本和92真实状态，后者仅header63/next29；padding0、实际bitsplice state未覆盖。不能把格式写入函数当作coded-frame生成器。

默认backend的DCT a81db、FFT a7de3已经通过，不再从这里重复起步。超出table域的CRT sin/cos和generic reader/8float窗口仍拒绝，不能用系统数学库替代而放松位比较。

## 可信原版输入/取证工具

- `cd66d9a2c821bc41f1f975aac8ac0a5bac6a6499` / 40232525056：直接调用589d0，8个mono/stereo、257/4096帧、RAM/stream案例各重复两次，保存ee00前raw与两个附属分配、descriptor、float及原e9b0转换后的signed16。三返回分配长度单位均为byte，用原free IAT ab1a8释放。
- `579c95661d90f067ab414c4133f033c2571b96e1` / 40233712043：来自固定main真实PCM8及长stereo的8例，16次原编码；121个packet、4个多block案例，输出重复逐byte相同。适合后续长块边界组，不重做短impulse作为全部覆盖。
- 同579c956 / 40233712043：纯native journal在97d97记录111次actual target，唯一为98267；全三返回分配不变。
- `de20ba24dc6941c56ee27ade3c897ece6a22b17e` / 40234805270：111个真实window快照，保存原32参数、系数位、short偏移及有界旧/新source，三返回分配不变。scheduler原CPU/Debug/Release都重放同111项，并交叉核对原128byte参数。
- 旧managed-VEH外围探针40232992739失败，不能作可用基线；不要重新采用有回调的断点方案。新native工具对特定实际site的成功也不冒充旧外围3site全部动态验证。

原始格式/方法调查：[XmaAudit](XmaAudit.md)、[PublicXmaReferences](PublicXmaReferences.md)、[wrapper反汇编](xma-wrapper-disassembly.txt)。原 `589d0 -> 59a60 -> 5d180 -> 659b8/6978f` 及编码循环 `59070 -> 5d5b0 -> 648b6`、flush `5d520 -> 60136` 是实际大codec；小工厂5a6d0仅18byte facade，绝非全部编码器。

- `d3f96009e64b44c4d9482421cc5922b3309320bc` / 40236037500：111个实际DCT输入/输出，360＋111原CPU/两配置逐位通过。
- `f683edd76b9619b23a6242b3dd5ddd450fcdb51b` / 40236818983：111个实际97b00整wrapper输入/输出，5760＋111原CPU/两配置逐位通过；不包含caller后续mask/855ef。

## 验证约束

用户要求不要频繁触发完整SDK真实验证。现在以独立数值/结构组、定向原版取证和真实输入重放推进；SDK留到完整管线接入的更大里程碑且显式启动。未变组复用既有精确锚点，原DLL/输入/相关依赖改变的组必须重新验证。运行时不得读/加载原AudioCompiler作为恢复后端；测试中的原版oracle与恢复宿主分开。
