# PC原生文件加载：下一实现批次

## 目标与入口

恢复 `D3DXCreateTextureFromFileA@0x13c82 → 0x13b53`：文件解码 → 资源头/存储构造 → LockRect/复制 → 解锁与释放。当前fixture loader只在测试中调用原版，不能当作恢复实现。

- 源码：`src/TextureCompiler/Native/TextureNative{Device,Resource,Surface,Descriptor,Copy,Convert}Api.*`。
- 原调用IL：`evidence/texture/runs/40191971356-1/methods.json`，按方法读取，避免加载全文件。
- LoadImageA参数证据：`evidence/texture/native-flags-static/20261030/manifest.json`。
- 真实资源消费参考：`tests/TextureNativeCopyConsumerOracle.cpp`、`scripts/TestTextureNativeCopyConsumers.ps1`。

## 必须保留的调用契约

- 原LoadImageA复制目标为 `0x1a200006`，`flags=0x12`、`threshold=0`。
- 实际PC raw资源格式含 `0x80` 端序标记，并携带通道映射；不得按默认flags0或直接RGBA字节复制推断行为。
- 已恢复 `TextureCopySurface`，可复用数值/浮点、四端序、swizzle与identity/forced-conversion分派。资源分配、父子引用和释放须使用匹配的所有权规则。
- 新原生ABI为独立x86库，尚未进入生产六DLL包；文件加载/资源构造及完整混合模式身份仍未完成。

## 当前边界

设备type2渲染状态、type9/GPU上下文；gamma/压缩转换、packed float10、uint32数值转换、过滤/重叠等仍未支持。signed/biased one-bit、不完整端序单位及原版未初始化DXT1/bitfields域保持拒绝，不伪造确定输出。

## 执行与验收

先实现文件加载、资源构造和实际消费的连贯批次；普通检查点不触发Windows，稳定后集中验收。最终保留原版完整产物比较、D/R、NoOriginal及必要实际消费验证；当前未变成功组和SDK/MOD不重跑。正式接入生产包前再取得对应集成证据。

当前验收锚点与63组件状态统一见[RESUME](RESUME.md)，避免重复维护。详细调度规则见[EFFICIENCY](EFFICIENCY.md)和[TEXTURE_TESTING](TEXTURE_TESTING.md)。
