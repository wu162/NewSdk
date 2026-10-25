# Timecoded 动画通道源码

实现：`src/Native/W3x/Timecoded.h/.cpp`。依据原版取证 `40162934653-1/decompiled/-Module-.cs`，`CompressedTimecoded.Compress/EstimateSize/WriteOut`（token `0x0600000E`–`0x06000010`）以及 `FindUselessPacket`、`FindUselessPacketQ`、`Slerp` 和四元数乘法。

采用便携 typed-value 接口，不复刻 EASTL 对象 ABI。Compress 保留类型、pivot 和带 binaryMove 的源帧，按完整误差阶段再按强制缩减目标运行原删除规则。目标为 `max(1,CLR_x86_conv_u4(float(sourceCount)*double(forceReductionRate)))`；type3 用旋转误差，type4 用可见性误差，其余用位移误差。二帧 scalar 原方法实际比较第二帧值减自身，非 binary 第二帧可删除；多帧 scalar 实际以端点插值减最后端点，未擅自改为“中间帧误差”。旋转保持原 Slerp、共轭乘积 W、绝对值和 acos 路径的 float/double 运算边界。

Encode 输出根36字节（typeId1624821609、usage+4、帧数+8、分量数+12、pivot+16、timecodeCount+20、指针+24、valueCount+28、指针+32）。接入 shared Tracker 时依次 Push 根(size36)、值(size4)、时间码(size2)，指针由 Tracker 记录。时间码截断 uint16 并按 binaryMove OR 0x8000；所有标量及payload按目标端序编码。Estimate 仅为 `(4*width+2)*retainedCount`，不计根或对齐。

BE float payload 保留原 `EndianToPlatform(float)` 返回路径的 x87 行为：有限源值交换字节后若形成 signaling NaN，再作为 float 返回/存储会设置 quiet 位。例如源 bits `F6D882FF` 输出 `F6 D8 C2 FF`，不是简单按字节交换得到的 `F6 D8 82 FF`。只在实际调用该 float helper 的值数组路径处理，不修改整型根字段；本批网格原版差分首先发现此公共运行时行为，Timecoded 原版 oracle 已加入7种有限位模式样例交叉验证。

已通过本地 GCC C++11 `-O0`、`-O2` source-contract 断言，入口 `tests/W3xTimecodedTests.cpp`。本地检查之外，原版Compress/Estimate/WriteOut三缓冲区差分、完整动画组件与无原DLL组件重放已通过；最终锚点统一见 [RESUME.md](RESUME.md)，不声明SDK验收。

## 合法边界补充（待本批原版矩阵验收）

XSD 的误差及ForceReductionRate均为无范围限制xs:float。解除原先针对负/极大rate及非finite源值的额外拒绝。原IL conv.u4使用CLR4 x86规则：binary64乘法结果在signed64范围内截断，再取低32位；非finite或越过signed64范围产生整数不定值，其低32位为0，再执行原min1。该规则复用已在网格548条原probe中验证的运行时结论，TimecodedOracle另加175条同CLR4x86 DynamicMethod `conv.u4`直接比较，并对原Compress扩充385个rate/error组合与840个source边界组合。本批新增边界是否通过，以后续统一validation锚点为准，不能沿用此前矩阵声明。

`INF`/`NaN`字面经原VC8 atof得到0，不应仅以typed非finite测试宣称XML可达；但合法数值文本如`1e39`经double atof再转float可得到+Inf。原Timecoded的IEEE比较自然处理±Inf/NaN，不像AdaptiveDelta会由无最佳filter进入非法表读取。序列化的原float加载还会先quiet源sNaN，再按平台换序；sNaN用于typed合同防护，并不声明XML文本能直接产生任意sNaN载荷。

保留拒绝：不一致宽度、零宽度、超过4分量、Orientation非4分量及序列化长度溢出均为typed模型/宿主表示边界。空通道Estimate返回0；原WriteOut对空源解引用第一帧，恢复版抛错，不模拟内存错误。索引不重排、不限制15位，保留原截断行为。XML的FirstFrame与NumFrames都是unsignedInt而无关系约束；有Frame却越过NumFrames的输入虽能通过XSD，原HandleAnimation直接写数组越界，恢复版必须明确拒绝。ChannelScalar/Orientation虽XSD允许，但原HandleAnimation明确抛unknownusage；Quaternion五种usage均允许。无channel的NumFrames0不涉及此越界，应正常生成runtime根。
