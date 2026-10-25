# AdaptiveDelta 源码恢复与验收边界

本组恢复 `CompressedAdaptiveDelta.CompressFrame / Compress / ctor / EstimateSize / WriteOut` 的值模型及序列化逻辑。证据是 `evidence/w3x/runs/40162934653-1/decompiled/-Module-.cs` 的 1440–1876 行及同目录 `raw-listings.zip` 的 `original.il`，方法 token `0x06000011`–`0x06000015`。原对象52字节、Frame17字节、目标 `AnimationChannelDelta`44字节；恢复代码不伪装成原EASTL/指针ABI。

源码：`src/Native/W3x/AdaptiveDelta.h/.cpp`。算法保留 source sample count 小于2返回null、相邻样本最大绝对分量差/7作为scale、首帧初值、每块16个差值、块优先/分量次序、尾块重复最后值、按严格小于选择首个最优filter、4位全部完成后误差超限再重试8位、8位仍超限返回null。块数为 `(sourceSampleCount+15)>>4`，不是对 sampleCount-1 向上取整。Source frame索引/BinaryMove不被这些原方法读取，由上层 Timecoded 及动画管线处理。

`CompressAdaptiveDeltaFrame` 暴露原方法级测试接口。filter表前16个常量保留PE原始float位；后240个按 `_initTable` 的Sin/conv.r4顺序生成。double乘加明确分开，float舍入点保留。CLR x86 `conv.i4` 的溢出/NaN分支使用INT_MIN，再按低字节写出，避免C++未定义转换；这是尚待原机动态差分核实的x86行为边界。

`EstimateAdaptiveDeltaSize` 保留原IL公式：`initialCount*4 + (((initialCount*bits+7)>>3)*frameCount)`，它不是完整root大小，也不是实际packed payload长度，不能改成9/17字节每帧。`EncodeAdaptiveDelta` 提供root44、initial浮点数组、packed数据三个未重定位块。root pointer slots28/40为零，由调用方Tracker分配/登记；顺序是root `Push(44,1)`、initial `Push(4,initialCount)/Pop`、packed `Push(1,frameCount*(bits==4?9:17))/Pop`、最后Pop root。root+8记录 `(frameCount/initialCount)*16`，root+36记录实际packed字节数。4位每对先低nibble再高nibble；filter与packed bytes不随平台交换，标量/初值交换。

本地 `W3xAdaptiveDeltaTests.cpp` 在g++ `-std=c++11 -ffp-contract=off` 的O0、O2各通过14288项检查，包括固定整数差值、正负饱和、zero scale、1–4分量/2–34帧、endian字段、固定signed nibble编码、随机独立解码误差约束，实际经过4位27组、8位37组及null56组。测试不依赖`assert`，优化模式不关闭断言。这些是本地定向检查，**尚不是原DLL逐字节差分，不构成动画/插件/SDK已支持声明**。

尚待验证：Windows原DLL的256 filter输出（尤其Sin表、x86 conv.i4及float舍入）、完整Compress/WriteOut三缓冲区差分、与上层动画压缩选择/Tracker的连接。编译时不得启用fast-math或FMA重排；已有本地测试参数明确关闭float contraction。当前完整压缩接口对非有限输入/maxError、分量不一致/超过4、32位长度溢出、相邻float差溢出及无有限filter显式抛错，未将这些未验证边界默默转发给原版。原版少于2样本返回null的检查先于其他输入检查，恢复版保留这个顺序。正式路由仍保持原有未支持状态。

新增 `tests/W3xAdaptiveDeltaOracle.cpp` 是独立x86 CLR4原版动态差分宿主，按固定token调用原始 `CompressFrame(0x06000011)`、`Compress(0x06000012)` 和 `EstimateSize(0x06000014)`，无需运行SDK。计划73728个frame fixtures遍历全部256filter、4/8位、0/subnormal/normal scale、三个误差、十二种样本模式；比较17字节frame/前后canary、previous/error的float位、excess bool与canary，并检查原对象/样本不变。另324个完整压缩fixtures覆盖1–4分量、0/1/2/15/16/17/31/32/33个样本、三种曲线/误差；逐字节比较原始压缩对象字段、初值数组、全部17字节frame及estimate，包含null结果。输入手工建立已确认的EASTL布局，输出由原MSVCR80分配的小内存保留至该独立测试进程退出，避免跨CRT释放。该文件新增时尚未Windows编译/运行；只有最终运行日志及精确sourceCommit验证成功后才能计为原版差分通过。

首轮Windows组件差分发现原CLR4 x86在交换float字节后重新加载float local时，会将交换后形成的signalingNaN quiet；即使交换前是有限float也会发生。AdaptiveDelta `WriteOut` 的scale路径原IL `IL_015b/IL_015d` 与initial路径 `IL_0266/IL_0268` 均是交换后的float local重载再`stind.r4`，因此编码仅对这两类浮点字段保留此语义，整数不变。新增有限float fixture：scale位`0080807F`输出BE字节`00 80 C0 7F`，initial位`76D882FF`输出BE字节`76 D8 C2 FF`；同位整数与LE浮点保持原值。本地O0/O2各通过更新后的14292项检查；这一修改仍须随下一次完整Windows源码锚点确认。

`W3xAdaptiveDeltaOracle.cpp` 随后补齐实际原 `WriteOut(0x06000015)` 和原Tracker `MakeRelocatable(0x06000038)` 三缓冲区对照：每个成功Compress结果都在LE/BE序列化比较；另用确认的compressed52布局手工构造512份原WriteOut产物（8种scale位×8种initial位×4/8位×1/4维×LE/BE），其中有限scale/initial交换后形成sNaN/qNaN/Inf，直接检验浮点写出而不靠本地预期值。整数Type `0100807F` 同时用于证明整数交换不quiet。使用原Tracker/Chunk析构释放序列化内存，值模型侧复用shared `Relo::Tracker`；该探针尚待新的实际Windows结果。
