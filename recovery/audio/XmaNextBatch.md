# EA-XMA 下一批恢复

更新：2026-10-26（北京时间）。**编码器尚未恢复，representation28 仍拒绝。**
原输入 `SDK/tools/BinaryAssetBuilder.AudioCompiler.dll`，SHA256
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`。
本文地址均为 RVA。

## 已完成：不要重复作为待实现项

`XmaPacketBits.cs` / `XmaPacketScanner.cs` 已恢复下列闭合整数组：

| 函数 | 范围 [start,end) | 指令数 |
|---|---|---:|
| count writer | `e890..e946` | 59 |
| bit accumulator | `ea80..eae0` | 40 |
| frame/continuation reader | `eae0..eb70` | 53 |
| packet scan/count emission | `ef60..f0fa` | 121 |

共832字节/273指令。源码 `96f72d037b840ad09590688e8d4ec1f998446d8d`，
[40209977895](https://github.com/wu162/NewSdk/actions/runs/40209977895) attempt1：
原 CPU、Debug/Release 各1025组一致，6安全拒绝，含8份真实 EA 输出块。
**这些块经过外层处理，不是原始 codec packet；没有编码器接入。**

维护边界：count 阈值为 `c0/30c0/c30c0/30c30c0`，最终五字节编码以 ff 起始，
不是 LEB128；packet 步长800字节、可用位3fe0、初始偏移15位、跨边界分割3fd1。
原 bit accumulator 对正位数无条件读3字节，截断输入按候选安全策略处理。

## 下一步：先建立原始 codec oracle

1. 在真实 Windows x86 CPU 调用 **`0x589d0`**，复用 `f150..f1e5` 构造的
   0x3c 字节 stream descriptor，以及 `f234..f275` 的输出指针布局。
2. 在 **`0xee00` 重写之前**抓取返回分配：保存 descriptor、planar signed16 源字节、
   11个参数、HRESULT、每个输出长度/数组及 FPU 设置。先固定 mono，再 stereo。
3. 核实 allocator 所有权：原 wrapper 在 `f30b..f320` 用 CRT free IAT
   `0xab1a8` 释放三个返回分配；不要用未经确认的其他 allocator 释放。
4. 沿实际 `59a60 -> 5d180 -> 659b8/6978f` 及编码循环 `59070` 跟踪；
   记录间接站点 `5a4cd/5a529/5a58d` 的真实目标后，再确定首个数值核心完整组。
5. 恢复输出封装/packet 重写 `ee00..ef54`（340字节/102指令、无直接 call），
   先查清输入中的外层 SIMEX 对象状态；完成编码与插件后取得差分和真实 SDK 验收。

顶层真实 CPU oracle 不要求先还原全部 codec。通用 Unicorn codec oracle 尚未建立，
还缺 CRT/compiler thunk、security cookie/stack probe、全局状态、间接目标及数值调用审计；
不能沿用重采样模拟器的成功声明。

## 已确认调用图：不要找错入口

| 位置 | 已确认含义 |
|---|---|
| `f275 -> 589d0` | 11栈参数 adapter，ret2c；创建对象、调用 vtable+4、经58a70释放 |
| `5a6d0..5a70f` | 63字节/23指令的小工厂，只分配18字节 facade；vtable首选VA100c5040 |
| vtable+0 / +4 / +8 | 5a710（另一项 RIFF/parser 操作，进58a90）/ **59a60（编码）** / 59840（删除析构） |
| `59a60..5a6c3` | 3171字节/954指令；15栈槽含显式对象指针，ret3c；28直接目标及3间接调用 |
| `5a141 -> 5d180..5d397` | 535字节/171指令初始化；继续调用6a064、69e95、659b8、6978f |
| `659b8 -> 6592c` | 分配/清零18e50字节（101968字节）codec状态，数值核心并未恢复 |
| `5b160` | 另一个28字节状态对象，vtable c5320：5b630/5b300/5b390/5b1e0，调用slot+c |
| 外层SIMEX vtable ac870 | ecc0/f400/14250/eb70/eba0/b880，与上述 codec 对象不同 |

外层 float→signed16 的 `e9b0` 使用 FISTP 舍入，不能直接复用 PCM 的截断。
此调用清单仅是已审计局部，未界定整个 codec 的工作量或可移植性。

## 输入与详细证据

- [XmaAudit](XmaAudit.md)、[wrapper反汇编](xma-wrapper-disassembly.txt)。
- 原版8份 Xenon XMA 样本：源码 `1c0457599f2a78082a283aade83a19e3f60a430e`，
  运行40199000936，`../evidence/audio/plugin-validation/40199000936-1`；
  mono/stereo、257/4096帧、RAM/stream、quality75、22050Hz，全部产生原版输出。
  这是 original-only 证据，不是恢复版验收。
- [PublicXmaReferences](PublicXmaReferences.md)：minidx9/FFmpeg/vgmstream 精确来源及 EA 差异。
- [整理前详细审计](https://github.com/wu162/NewSdk/blob/36880397ce00f18690edf21e78bc68b39736eac4/recovery/audio/XmaNextBatch.md)：完整直接目标清单、ABI与取证方法，按需读取。
