# EALA.Hash 源码恢复

工作分支：`recovery/eala-hash-source`，基于 `recovery/compiler-source` 的 `9632a329`。
源码：`recovery/src/Managed/Hash`，.NET 2.0 纯 C#，无原 Hash DLL/VC++ 运行库依赖。
保留 `EALA.Hash.FastHash` 可构造类、六个 public static 重载和原程序集版本 `1.0.3344.32674`。

## 原版语义

原始方法 IL 与 DLL SHA256 见 `evidence/hash/original-methods.il.txt`。
FastHashFn 使用小端 16 位读取、uint 溢出；尾部的单字节是 **signed char**。
无种子的字符串使用 UTF-16 Length 为种子；byte[] 使用整个数组长度为种子，含显式 length 重载。
字符串先 Marshal.StringToHGlobalAnsi，再以原 string.Length 为字节数：保留 ANSI/DBCS 行为，不能改成 UTF-8 或 UTF-16 哈希。
非空数组指定 length=0 返回 0，即使种子非零；空数组仍在 pin data[0] 时抛 IndexOutOfRangeException；null 抛 NullReferenceException。
原版负长度/超过数组长度会越界读取内存，不存在稳定可兼容结果。恢复版明确抛 ArgumentOutOfRangeException；不会对原版执行这些不安全探针。

## 验证

`recovery/scripts/TestHash.ps1`：独立原版进程，与没有原 Hash DLL 的 Debug/Release 进程比较完整转录。
覆盖全部六个重载、公开签名/构造器、六种种子、长度0至各余数、全部65536种两字节组合、全部256种有符号尾字节、随机/固定数据至65536字节、ANSI/中文/土耳其文/代理项/嵌入NUL、三种Culture、null/空数组异常。
恢复侧另检验非法长度拒绝。Windows Actions：`.github/workflows/recovered-hash.yml`。
当前：源码与测试已准备，Windows原版差分待验收。

未替换 `SDK/tools`，未改变 Core 的项目引用；本次不宣称真实 SDK 集成验证。合入通过 PR，由用户批准。
