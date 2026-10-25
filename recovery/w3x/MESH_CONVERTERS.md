# W3X 顶点转换能力组

源码：`src/Native/W3x/MeshConverters.h/.cpp`。这是便携算法接口，不是原 `ConverterData` 的24字节 ABI；调用方提供已验证的源/目标范围和 inverse bone mapping。源数据为主机 float/ushort；目标数值按指定 endian 序列化。支持正/零/负 pitch（缓冲区必须覆盖所有访问）、未对齐地址。源/目标不应部分重叠。负计数与空非零缓冲区是恢复接口的防御性拒绝，不声称与原版未定义内存访问等价。

证据锚点：`evidence/w3x/runs/40162934653-1`，原 DLL SHA256 见 `BASELINE.md`。对照 `decompiled/-Module-.cs` 和 `raw-listings.zip/original.il`：

| 方法 | token | 原版语义 |
| --- | --- | --- |
| CopyTexCoord | 0x060000C2 | u保持，v以double计算1-v再round float，单独交换每个float |
| CompressTexCoord | 0x060000C3 | 同上并按原整数算法转换half |
| Vector4_To_UByte4N | 0x060000C4 | [0,1]夹紧、float(值*255)、float(+0.5)、截断，整32位交换 |
| Vector3_To_Half4 | 0x060000C5 | xyz转换half，第四分量为0，每个half交换 |
| ConvertBlendIndex | 0x060000C6 | ushort索引检查inverse表边界，取低8位；无endian交换 |
| ConvertBlendWeight | 0x060000C7 | double(值)*255+0.5后conv.u4低8位，不饱和；无endian交换 |
| CompressNormalizedDirection | 0x060000C8 | 分量夹[-1,1]，xy乘1023、z乘511分别round float；截断，11/11/10打包，整32位交换 |
| CopyDirect<Vector3> | 0x060000D2 | 逐元素原样拷12字节；bigEndian在计数检查前直接assert |

half算法保留原 `shr.un` 的32位计数掩码、tie-to-even、过大/Inf/NaN映射sign\|0x7fff。不能用标准IEEE binary16替代：例如源位模式0x08800000（2^-110）输出0x0400，而标准转换会变为0。零仍正确输出0，负零保留符号。

为避免C++浮点转unsigned的未定义行为，有限范围conv.u4使用先截断int64再取uint32。原CLR x86转换的部分溢出已采集为独立PROBE，但未将完整溢出域纳入恢复支持：本接口显式拒绝BlendWeight变换结果不在[-2^31,2^32)或非有限值，以及color/normal的NaN。颜色/方向的正负Inf会被夹紧；half和直接拷贝仍接受非有限值。**这些拒绝是尚未恢复边界，不代表原版拒绝。** 不应将该能力组写为所有合法XSD float输入均已支持。

本地测试：`tests/W3xMeshConvertersTests.cpp`，g++ C++11 `-O0`/`-O2 -Wall -Wextra -Werror`各461断言PASS，含独立golden位模式、rounding、符号、夹紧、wrap、stride0/负stride、endian、padding/canary、inverse越界和显式拒绝。ASan/UBSan在禁用当前受限环境不支持的LeakSanitizer后同样PASS。此处是本地定向检查，**不是原 DLL 动态差分或 SDK 验收**；动态oracle由本轮统一能力组运行记录。


## 首轮原版差分发现并修复的浮点存储差异

Windows run `40165200024`、source `b6dd6bd3` 的 `/Od` 全部7组本地组件检查通过，但原版converter动态差分失败，未被当作验收成功。失败为 CopyTexCoord、BE、输入v位模式 `0x76d882ff`：double计算再round float后 `1-v` 是 `0xf6d882ff`；原版先把交换字节后的 `0xff82d8f6` 写入float局部，最后 `ldloc/stind.r4` 在CLR4 x86将该 signaling NaN quiet为 `0xffc2d8f6`。旧恢复版直接按整数写目标字节，遗漏了这一浮点存储效果。

修复后的 `StoreFloat` 显式保留原float load/store的NaN quiet位、符号与payload处理，尤其是**字节交换后的float存储**，因此该finite输入目标字节为 `f6 d8 c2 ff`。增加上述失败fixture和u分量交换后产生sNaN的独立golden断言，本地O0/O2各461断言通过。差分断言未改；修复后的完整原版矩阵已双模式通过，包含直接memcpy构造的原始sNaN用例；精确范围和成功锚点统一见 [RESUME.md](RESUME.md)，未复用失败source的状态。
