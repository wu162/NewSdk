# PC 默认 DXT1 / DXT1A 未初始化边界

原 DLL 固定SHA256 `4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e`。

取证源码 `85690b1f1112de3937c52fae2e97eff16cb5d785`，运行 [40658031884](https://github.com/wu162/NewSdk/actions/runs/40658031884)，attempt1，completed/success。状态严格为 **probe-only**；只选手动 `dxt1-boundary`，复用32个未变组件组。不是恢复编码器验收。

取证不修改原二进制或其内存代码。自有C++辅助函数先在一个已经返回的128KiB栈帧内写入0、1、2、0x7fffffff或0xffffffff，然后正常调用不可变原RGBA nvDXTcompress@RVA0x52B60。选项来自原 GetNVCompressionOptions，PC默认71/4×4/无mip；同一源RGBA和选项，每种填充值重复两次，三个独立新进程。四格式×8 RGB图案×4 alpha图案×5填充值×2重复=每进程1280、总3840份完整原DDS；输入始终不变，回调及完整源/选项/DDS保留。

每个新进程均有相同25类输入产生两种不同完整DDS：

| 格式 | alpha模式 | 变化RGB图案数 |
| --- | --- | --- |
| DXT1 | 全0、全255、127/128交替、0..255渐变 | 各5，共20 |
| DXT1A | 全255 | 5 |
| DXT1A | 全0、127/128交替、0..255渐变 | 0 |
| DXT3/5对照 | 上述全部alpha | 0 |

变化的RGB图案为固定非灰单色、双色、灰度渐变及两种固定种子随机；全黑、全白、灰单色在本矩阵未变化，不推断全部类似输入稳定。这个受控调用环境反例足以排除“默认DXT1输出只由RGBA+options决定”的假设；不能用某次偶然栈值当作合法确定性默认值。

这与静态证据一致：原RGBA包装层在默认DXT1未写颜色state+4；DXT1A遇透明样本才写0，DXT3/5写1。CodeRGBBlockNoMode@0x83CD0按该字段分派强制三色、强制四色或两者比较。即使三色数学编码器完成，也不能冒充默认完整DXT1/1A已等价。

可推进的独立范围是显式初始化三色state，以及已确认有透明样本的DXT1A块。必须继续用完整原块/float/partial/DDS/mip/volume/入口/SDK门槛验证后才启用对应消费者。全透明、透明阈值、输出模式与端点/索引规范化仍需逐项核对。无原版回调、样本ID分派或gold表后端。

证据：`evidence/texture/dxt1-boundary/40658031884-1/{validation.json,samples.zip}`。`[texture-dxt1-boundary]`显式采集，普通组件/SDK不自动运行。后续宿主提交6bd4ade3将每次options复制和三进程摘要单独保存；该收据仍绑定实际运行85690b1f，不改写源码锚点。
