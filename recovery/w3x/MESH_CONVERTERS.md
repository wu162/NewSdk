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

conv.u4数值域已按原CLR4 x86行为补全：先按binary64分别round乘法与加法，截断到signed64后取低32位；NaN、Inf或变换结果不在[-2^63,2^63)时返回原integer-indefinite的低32位0。显式范围分支避免C++浮点转整数的未定义行为。Color的NaN经原conv.i4→float→conv.u4得到低byte0；NormalizedDirection的NaN分量打包为0，正负Inf仍按原比较夹紧。所有float位模式均有定义，不再把合法XSD float值仅因数值范围或NaN拒绝。结构性非法缓冲区/计数、inverse映射越界以及CopyDirect的原BE断言继续拒绝。

本地测试：`tests/W3xMeshConvertersTests.cpp`，g++ C++11 `-O0`/`-O2 -Wall -Wextra -Werror`各716断言PASS，含独立golden位模式、rounding、符号、夹紧、wrap、stride0/负stride、endian、padding/canary、inverse越界和显式拒绝。ASan/UBSan在禁用当前受限环境不支持的LeakSanitizer后同样PASS。此处是本地定向检查，**不是原 DLL 动态差分或 SDK 验收**；动态oracle由本轮统一能力组运行记录。


## 首轮原版差分发现并修复的浮点存储差异

Windows run `40165200024`、source `b6dd6bd3` 的 `/Od` 全部7组本地组件检查通过，但原版converter动态差分失败，未被当作验收成功。失败为 CopyTexCoord、BE、输入v位模式 `0x76d882ff`：double计算再round float后 `1-v` 是 `0xf6d882ff`；原版先把交换字节后的 `0xff82d8f6` 写入float局部，最后 `ldloc/stind.r4` 在CLR4 x86将该 signaling NaN quiet为 `0xffc2d8f6`。旧恢复版直接按整数写目标字节，遗漏了这一浮点存储效果。

修复后的 `StoreFloat` 显式保留原float load/store的NaN quiet位、符号与payload处理，尤其是**字节交换后的float存储**，因此该finite输入目标字节为 `f6 d8 c2 ff`。增加上述失败fixture和u分量交换后产生sNaN的独立golden断言，本地O0/O2各461断言通过。差分断言未改；修复后的完整原版矩阵已双模式通过，包含直接memcpy构造的原始sNaN用例；精确范围和成功锚点统一见 [RESUME.md](RESUME.md)，未复用失败source的状态。


## 2026-10-25 数值域收尾（待本轮完整动态矩阵）

复用 `40166287709-1/artifact.zip` 中 `/Od`、`/O2` 的728条原版独立PROBE，未重复启动相同取证。548条BlendWeight记录含234种去重输入位模式，全部符合binary64算术、signed64截断再取低位模型。对同样原输入用精确有理数模拟53bit与64bit内部精度：53bit模型0差异，64bit模型24差异，因此实现明确使用double round，不能保留x87扩展精度的半单位误差。此前尚未纳入差分的98条Color和82条Direction记录也纳入本轮正常双边比较。

本地golden现新增上述234种原版Weight输出和24项raw NaN转换断言，O0/O2各716断言通过；原有断言与canary均保留。`W3xNativeOracle` 删除旧Supported筛选，原728条输入现在直接比较原/恢复两边；另增12288项sign/exponent/mantissa结构矩阵和50项转换阈值邻近float测试，所有新样本仍检查原source不变、目标padding与完整160字节。此处实现与本地通过不能代替本轮Windows动态矩阵成功；最新sourceCommit/runId由本轮入口统一记录。
