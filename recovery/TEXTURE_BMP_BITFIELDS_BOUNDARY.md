# PC BMP 扩展 bitfields 的原版栈依赖边界

2026-10-29（北京时间），`recovery/texturecompiler-source`。本记录解释为何扩展 DIB 的 BI_BITFIELDS 不能按普通 RGB565/8888 直接启用。只读原 DLL 静态证据与原版探针都不是恢复实现或 SDK 验收。

## 静态根因

固定原 DLL 为 `SDK/tools/BinaryAssetBuilder.TextureCompiler.dll`，SHA256 `4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e`，PE ImageBase `0x10000000`。下述地址全部是 RVA；反汇编显示的 VA 等于 ImageBase+RVA。

完整相关函数、原字节、文件偏移和逐函数 SHA256 见[定点静态证据](evidence/texture/bmp-bitfields-native/20261029/manifest.json)。复现工具为 `recovery/scripts/InspectTextureBmpNativeEvidence.py`，只读取/反汇编已固定 DLL，不执行 DLL 或 codec。

| 位置 | 原版行为 | 对结论的意义 |
| --- | --- | --- |
| BMP wrapper `0x167AD` | 识别 BM 后以文件 `+14` 调用 DIB parser `0x13F3D` | parser 入口确为 DIB 起点 |
| parser `0x13F61..0x13F6D` | ECX=10，EBX=EBP−0x44，`rep movsd` 复制40字节到局部 header | 扩展 DIB 的 mask 尾部没有复制进该局部对象 |
| parser `0x13F67..0x13F6F` / helper `0x13E99` | 将同一个局部地址作为源与目的；helper 只读写偏移0..0x24 | helper 只是40字节自复制，没有补读输入 mask |
| parser `0x13FC1` 起 | 非 core 支路要求 DIB≥40，并保持上述 EBX | 扩展 DIB 不走 core12 的另一套局部 header 构造 |
| parser `0x14038..0x14069` | compression=3 时要求 DIB≥52，随后读 `[EBX+0x28/+0x2C/+0x30]`；DIB≥56再读`+0x34` | 这些地址越出刚复制的40字节，落入未初始化的邻接局部栈区 |
| parser `0x14074` 起 | 先置 generic fallback format `0x1A22AAA6`，再根据上述局部 mask 匹配已知格式 | 偶然落入某个格式或某次看似稳定，不证明按输入 mask 正常解析 |

已核对 copy40 到 mask 读取之间的完整非 core 控制流：没有初始化这些邻接 mask 槽位的 memset、输入尾部复制或可补写 helper。core12 分支会在邻接区域构造另一套 header，但不是扩展 DIB 的 compression3 路径。原字节保存了整个 parser 与 helper，结论不依赖截断后的几条指令。

因此，该原版缺陷影响扩展 DIB（已取证52/56/108/124）的16/24/32-bit BI_BITFIELDS。INFO40+compression3 在读取这些槽位前已被原版拒绝；BI_RGB 绕过该 mask 分支，不受此缺陷影响。

这不能被改写成“修改输入 mask 时输出必定不变”：文件名、调用顺序、分配与进程上下文可能改变残留栈。实际差异仍需完整保存；不能用样本 ID、某次残留值或标准565公式替代原版未定义状态。

## 排除另一个疑点：输出 descriptor 是否预先清零

原 `LoadImageA` 在调用 `XGGetTextureDesc` 前使用未初始化的局部 `_XGTEXTURE_DESC`，新探针则清零60字节 descriptor。已定点核对正常 resource type3 路径：

- `XGGetTextureDesc` wrapper `0x3DF34` 调用 implementation `0x3DC51`。
- type3 分支 `0x3DDBC` 经 `0xDB60 → 0xD18D → 0xC83D` 从实际 texture header 构造 level format。
- `0x3DDDD` 明确写入输出 descriptor `+16 format`，并写入尺寸字段。

因此，在这个正常 texture 路径上，format 字段并不靠调用前 descriptor 的残留字节决定。该结论不外推未知 resource type；探针仍保留原 descriptor 字节供核对。

## 动态证据与实现边界

旧原版取证 `51189674 / 40799686293` 已有49个同输入重复不同的24/32-bit反例。其所属192个输入整体不作为确定 gold；另96个16-bit输入虽两次相等，静态证据同样显示其 mask 读取有栈依赖，不能据此启用标准565或固定“单 bit 绿色”规则。

恢复版对扩展 bitfields 全288个既有输入明确 `NotSupportedException`，组件和实际插件另验源/cdata不变及无原DLL，不把这些拒绝写成原版错误匹配。INFO40 bitfields 与 compression6 的原 BadTextureFile 匹配仍独立保留。

新 `bmp-bitfields-load` 取证入口改变一般 mask，使用两个独立进程、反向遍历顺序、两种前序调用上下文，每次保存两份原版完整 RGBA/错误、native descriptor 和已知格式的首行逻辑字节前缀。未知 native 格式不锁定/读 pixels；不根据 pitch×height 推断分配大小。native snapshot 与 LoadImageA 各自创建纹理，不宣称它们来自同一次创建。其收据始终 probe-only；具体终态和归档审计结果由[BMP 当前入口](TEXTURE_BMP_HEADERS_NEXT.md)记录。

BI_RGB 新 DIB 的实现、完整组件/实际消费及专用 SDK 是独立能力组。今后若要提供标准 bitfields 支持，需要先明确新的语义目标；现有“与原版确定产物逐字节一致”的目标下，不能擅自替原版选择未初始化栈值。
