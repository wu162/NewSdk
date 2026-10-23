# 完整声音播放能力组

AnimationSoundClientBehavior与ModelConditionAudioLoopClientBehavior已实现。源码85a1e180c6bdb506733ec0711e9ad7c8512095c4，run39968193677及基础回归39968193825均success；最终异常精确性修订以RESUME最终锚点为准。

| 类型 | TypeId | nativeSize | 完整结构 |
|---|---:|---:|---|
| AnimationSoundClientBehavior | 670005533 | 20 | 可空MaxUpdateRangeCap@8，Sound列表@12，stride24 |
| ModelConditionAudioLoopClientBehavior | 1514068980 | 16 | ModelConditionSound列表@8，stride124 |

Animation Sound条目：Sound资源@0、可空60字节RequiredMC@4/ExcludedMC@8、W3DAnimation小写typed-ID@12、Frame有符号整数列表头@16。Frame使用原C whitespace规则；动画名不是区分大小写StringHash。

ModelConditionSound条目：可空Sound引用对象@0、inline RequiredFlags60字节@4、ExcludedFlags60字节@64。复用原ModelCondition契约及padding；不能以可空mask替代inline布局。

原PositiveNonZeroReal helper68649实际接受0，拒绝负数，异常BinaryAssetBuilder.BinaryAssetBuilderException / XmlFormattingError / `Value should be >= 0`。最终修订直接比较异常类型、消息及公开错误码属性。继承Token、nullable/list Token、endian和原分配顺序均保留。

完整组每模式91,528 fixtures、274,584三缓冲区比较、8,444拒绝、272不变性比较，/Od与/O2两模式合计549,168次缓冲区比较，平台0/1/2/99。覆盖全部ModelCondition枚举、0/空/缺失/负值、所有字段及长列表；SDK未运行。
