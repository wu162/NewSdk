# Transform 后的准确续接入口

地址均为原 AudioCompiler DLL 的 RVA，SHA256固定为 `a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`。这是下一轮入口清单；新增能力是否已验收以[验收索引](../evidence/audio/acceptance-current.json)的精确source/run为准。

## 当前能力边界

数值主链已经推进到 integral window调度98267 → fold/mask/weights wrapper97b00 → DCT a81db → FFT a7de3；可选post-transform谱门限855ef为本轮最后一组。97b00最终5,760 synthetic＋111实际输入、855ef2,400 synthetic均已通过原CPU/Debug/Release完整位模式及独立拒绝验证；精确锚点见各组文档/索引。仍没有完整PCM→coded frame→raw packet driver，representation28继续拒绝。

97b00真实callsite为8c0af，返回8c0b4；此处的完整capture输出不能冒充整个channel分析结果。调用者8bfb0还会：

- 在8c0e3依据backend+3c、+6c、+13c和channel索引进一步清零尾部；
- 在8c0f9把谱复制到channel+58；
- 在8c112依据core+18adc调用855ef；
- 把DCT shift写入channel+64，传播负HRESULT到channel+c0。

8c373按版本/声道分组驱动8bfb0，这些对象布局和生命周期尚未恢复；不能把几个成功数值数组API拼成完整driver。

## 优先取证：真实分析与编码分派

| 下一入口 | 已核实关系 | 下一步应交付 |
|---|---|---|
| 8c8e3 → 8c133 | 8c133先依据channel参数/版本/标志分派；直接callee含78f48、93d0d、9428d、943cf、945b8、94c03、94c72、972ed、a6f04 | 抓实际入口参数、被选target及完整变化状态，选真实可闭合分析组；不要直接给未知callee贴量化/心理声学语义 |
| 8c1b0 `call [eax+18dfc]` | 672de静态安装92c45；其他分支可直接走93d0d | 首先证明实际target，再恢复完整回调及依赖 |
| 8c280 | 8c317/8c31f/8c32f/8c337分别call core+18df8/+18df0/+18df4/+18dec | 对实际调用做native entry/return状态快照；此四种选择不是四个已恢复算法 |
| 7c06e | 在7c16e/7c23a/7c3fd调用core+18de0；另直接依赖782af | 抓实际target及coded-frame输出边界，确定它与分析/量化/码字写出的真实关系 |
| 8c40f | 直接依赖97482、975e3、97996 | 纳入相邻阶段数据流审计，不能因transform成功而跳过 |

67297安装函数给出下一轮静态候选：core+18de0默认7d4a0，backend+244非零改7c57b；+18dec初始8852b；在backend+11c==1且backend+28==0时，+18df0/+18df8/+18df4分别88678/88911/88af9，否则三者也是8852b；backend+3c==3又把这四槽全部改为8852b。这些是**静态安装证据**，还没有像98267/a81db那样的实际运行target与完整数组回放证明。优先trace上述6字节间接call点，不再重做已成功的外围managed-VEH断点。

## 初始化、编码循环、flush与bitstream仍待闭合

- 初始化：589d0 → 59a60 → 5d180。5d1d7先调用6a064配置计算；5d2d1调用659b8分配/清零0x18e50-byte core并经6592c初始化；5d2e3调用6978f。后者依赖673dc、6592c、8e274 backend工厂、65a54、68ad0、67297、65e19、67057、69094及可选675ce；成功的noise/DCT/FFT只覆盖这些依赖中的局部数值能力。默认callback安装、buffer分配、参数表、状态重置、错误释放仍需完整恢复。
- 连续输入：59070 → thiscall5d5b0 → stdcall648b6（14槽，ret38）→ 63ec2 → 62fda → 60fb7。60fb7是多阶段控制函数，包含8c373、8c40f、8c8e3及大量其他callee；不能以恢复单个transform替代它。
- flush：59070 → 5d520 → 60136（7槽，ret1c）。60136与648b6最终都可走728e3或特殊分支74f20。728e3主要搬运已有codec FIFO，另依赖70fd6/710a2；它本身不能充当尚未恢复的量化/entropy编码器。
- coded frame→raw packet：完整appender5e8f0、最终block5ec90及其生命周期尚须闭合。已经恢复的bit splice/header/next-packet/padding是[raw packet辅助组](XmaRawPacketWriter.md)，不是PCM编码决定。EE重写ee00也不能替代原frame比特流生产。

每个新组采用实际原Windows x86 CPU作为oracle，隔离恢复宿主Debug/Release完整bits/state/返回值比较；安全拒绝另计。实际native capture必须保持三个最终raw返回分配与成功未插桩基线完全相同。原始长PCM8/多block基线已经存在，后续driver/flush必须覆盖这些边界；无需重复频繁完整SDK构建。只有上述数据流与生命周期闭合后，再接公开路由并做插件端到端和里程碑SDK验收。
