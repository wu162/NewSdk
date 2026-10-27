# PC TextureCompiler 颜色恢复交接（2026-10-27 20:11 北京时间）

分支 `recovery/texturecompiler-source`，用户本轮要求继续两小时，现按“先交接”停止新增实现。禁止强推；编译/测试全部只在 GitHub Actions。生产 SDK DLL/config 与插件 DXT 拒绝路由尚未修改。

## 已验收：默认 Production=71 颜色状态

源码 `2cee34462377beca4b16926eaf25d093daa02c2a` / [40500150864](https://github.com/wu162/NewSdk/actions/runs/40500150864) attempt1 completed/success。

- Debug/Release 各 2,019 例，29,107,932 检查：比较原版与恢复版完整 0x3850 字节初始化工作块。
- 同量独立 NoOriginal 重放，各 29,105,904 字节比较；原 DLL 不存在且未加载。
- 覆盖格式0/1/2/3、模式字段0/1/2、自动及固定3/4色调用、样本数1/2/3/4/8/16/32与常量128/256/512容量边界，灰度/单色/双色/渐变/随机与两套正权重。
- normalization area @+0x3010 必须为正；测试使用16。早期未验证工作稿的零面积不能作为正常候选选择证明。
- Native DLL 为 I386、无 PE 入口、无导入，所有数学/清零/复制/舍入 helper 为自有实现；不调用原 DLL。

核心源码：`src/TextureCompiler/Colour/TextureColour.asm`、`TextureColourBridge.cpp`、`SOURCE_MAP.json`。生成器：`recovery/scripts/GenerateTextureColourSource.py`（分析工具 capstone==5.0.6），helper 模板 `TextureColourHelpers.inc`。正常构建直接使用已提交源码，不在运行时读取原 DLL 或解释机器码。

调用图取证源码 `7367b839` / [40496512115](https://github.com/wu162/NewSdk/actions/runs/40496512115) success/probe-only，共52函数；默认71路径排除72穷举闭包后约30函数。Ghidra 的 RoundMult 函数体不完整：生成器根据两张各511项跳转表恢复29,398条指令、102,292代码字节，检查连续且无重叠，再生成指向本模块符号的跳转表。整条默认路径共36,629条指令。源中静态数据只复制实际原常量/预计算灰度表与插值系数，检查数据区不存在原绝对地址重定位；动态LUT从零初始化。

ABI/边界：仅 Win32 SSE2、默认质量71；bridge 在进入状态机前拒绝72及其他质量，不声称穷举恢复。其不可达72目标为显式失败指令，不能绕过bridge直接调用内部标签。原子化API还检查块尺寸、格式、计数、归一化RGB和正权重。公开块编码器序列化调用，保护原样恢复的惰性查表初始化。

首轮40499270083失败于MASM隐含x87操作数；40499698874失败于大小写/PROC局部标签。已按原操作码展开两操作数并使用全局标签修复，失败记录保留，最终状态不依赖这些失败批次。

## 已验收：完整 DXT3 / DXT5 4×4 块

源码 `b02787056a655a9710117e05e4b771740d3b67fe` / [40500898688](https://github.com/wu162/NewSdk/actions/runs/40500898688) attempt1 completed/success。

`src/TextureCompiler/Managed/TextureDxtBlockEncoder.cs` 将自有颜色与已验收 alpha 组合，支持默认PC DXT3/5，返回完整16字节压缩块。Debug/Release 各10,240例、235,528检查，原版实际DDS的完整16字节payload相同；同量NoOriginal各163,840比较通过，源RGBA未被改写。包括灰度全256值、单色、双色、渐变、alpha边界、随机RGBA。NativeColour与NativeAlpha均由本次源码独立编译；源码测试正常不依赖原压缩库。

仍不等于整图、完整插件或真实SDK验收。DXT1/1A仅有上述显式初始化状态级对照，不承诺默认整块输出：原RGBA包装层对部分模式未显式初始化+0x4字段，DXT1A遇透明像素才写0、DXT3/5固定写1。此为静态依据，仍需独立取证与定义边界；不能假设所有历史PC/PS3小块差异已因此定因。

Alpha原锚点保留：`4bf0ca8c` / 40493971846，包括字节/float量化器及NoOriginal；其实现未在本轮修改。完整细节见 [TEXTURE_DXT_ALPHA.md](TEXTURE_DXT_ALPHA.md)。

## 已验收：二维无 mip DDS

源码 `1a260825052acfe7f90582bf8483318b4f627e21` / [40502726713](https://github.com/wu162/NewSdk/actions/runs/40502726713) attempt1 completed/success，Debug/Release 及 NoOriginal 完整矩阵通过。

前次 `a9cc0daf` / 40501793638 失败于测试宿主查找 `EncodeBlock`，整图实际入口为 `Encode`；未进入编码器。现已修复并增加入口及尺寸/格式/mip拒绝断言，原失败证据保留。

- 新 `TextureDxtDdsEncoder.cs`：只支持 DXT3/5、POT且宽高至少4、无mip，逐4×4块编码并构建完整128字节DDS头。输入上限沿用小于16M像素；小尺寸/NPOT/mip/DXT1/1A明确拒绝。
- `TestTextureDxtDds.ps1` / `TextureDxtDdsDifferential.cs` / `TextureDxtDdsNoOriginal.cs`：目标每配置768整图（两格式×12尺寸×32图案），包括4×8、8×4等非正方形图与32×32多块，完整DDS逐字节及NoOriginal重放。
- 依赖表已登记 `dxt-dds2d`；此次运行只选该组，未重跑未变的颜色状态/块/alpha成功组。未触发SDK。

## 当前推进：插件 DXT3/5 无 mip 入口

`Plugin.Images.cs` 的 StandardTexture 分支接入自有 DXT DDS 编码器；覆盖显式DXT3/5及原 GuessProperFormat 将带alpha的DXT1转换为DXT3/5的路径。原始DXT1/1A、压缩mip、volume与小于4尺寸仍明确拒绝。生产SDK未替换。

新 `dxt-images` 组件组：Debug/Release各1280输入，含TGA七种尺寸与八类图案、PNG完整已声明格式/位深/交错/元数据组合、BMP位深/方向/bitfields，两个实际ProcessInstance入口与新旧cdata文件；逐字节比较三缓冲区与完整cdata，另做独立NoOriginal重放和未支持输入拒绝。源码 `6d9197ae3013bf046496e928fbf1c84939d8b16c` / [40505763208](https://github.com/wu162/NewSdk/actions/runs/40505763208) attempt1 success。Debug/Release各1280输入（912成功输出、368原版拒绝行为匹配），22明确拒绝检查，6,016,960字节比较；同量NoOriginal成功。此前40503241547及40503774342分别发现旧cdata测试断言与全零TGA alpha被D3DX转为不透明后的DXT1测试域错误，均保留失败记录；未改写已验收解码规则。

`dxt-samples` 已验收：源码 `9d487859ea0cd6da90a10d1e59e4e1300b7c04dd` / [40506678502](https://github.com/wu162/NewSdk/actions/runs/40506678502) attempt1 success；同批完整块、对齐DDS与插件入口回归全部成功。独立 `TextureDxtSampleBlockEncoder` 接受1/2/4×1/2/4字节或归一化float块，直接复用自有颜色核心与alpha量化器；按原0x5c790包装层的字面映射处理颜色索引空洞，DXT5越界alpha索引为0、DXT3越界nibble为0，绝不以统一复制边缘近似。原4×4编码器仅将内部ColourLock开放给同程序集新组件，仍共用同一把锁。每配置4608原版float dispatcher对照、2304实际原byte DDS交叉对照及NoOriginal重放，未接插件/压缩mip。Debug/Release各4608原float对照/376847检查、2304实际原byte DDS交叉比较及4608 NoOriginal/73728字节比较通过。

SDK集中运行 `40507166585`（源码 `3e884145`）的原两个raw profile三模式各600资产通过，压缩profile在Original模式被BMP bitfields拒绝。现移出6张已由组件拒绝矩阵覆盖的bitfields输入，`sdk-PcDxtNoMip` 改为114合法图像/456资产，待复验；不删减组件断言，不将原版拒绝输入冒充SDK成功。生产SDK仍未替换。

手动原版取证组 `dxt-image-boundaries`：416个小尺寸/对齐二维DXT3/5、无mip/完整mip样本，保存DDS、RGBA、options、header与callback framing；仅用于后续整图/压缩mip契约，不是恢复版验收。

## 下一轮顺序

1. fetch当前远程头，核实最新dxt-images运行的head_sha、conclusion及收据，保留机器人提交，再继续源码。不要假设文档提交是新的验收源码。
2. 完成实际 Texture/OnDemandTexture 插件入口差分；收取dxt-images及因Plugin.Images修改失效的现有入口组完整回归。新组包含native DLL构建/复制和独立重放。
3. 在更大里程碑集中做 Original / Recovered / NoOriginal 真SDK。现阶段不能替换生产SDK或声称压缩图像SDK通过。
4. 小于4的尾级块、浮点RGB块、triangle压缩mip、volume压缩与DXT1/1A是后续独立能力组。原包装层对小尺寸颜色索引有复制规则，而alpha越界像素不是直接复制边缘；勿直接把普通4×4块复制填充当作已恢复mip。

后续正常构建：x86 MSVC shell 中先 `ml /c /coff /safeseh` 编译 Colour ASM，再以 `/LD /MT /fp:strict /GS-` 编译bridge，链接 `/NODEFAULTLIB /NOENTRY`，与自有Alpha helper及托管DLL放在同目录。实际测试调用仓库相应Actions组，勿在本地编译测试。


## 2026-10-27 本轮末段：小尺寸整图/插件批次（待验收）

对齐纹理SDK验收：源码 `3319f50c1cbc227efe6f68d7470249f9576c49d3` / [40507887013](https://github.com/wu162/NewSdk/actions/runs/40507887013) success；Original/Recovered/NoOriginal各456资产与实际Core调用，四产物及完整三缓冲区/cdata轨迹一致。前次40507166585两个原raw profile三模式各600资产成功，压缩profile失败于原版BMP bitfields解码；6个拒绝输入保留组件矩阵。40507672026为并发发布未落地前误启动的plan-only失败（未知新取证组），未运行Windows，不作业务失败或验收。

同批 `dxt-image-boundaries` 416原版样本取证完成，证明small无mip header flags=0x81007、mipCount=0、caps=0x1000，linearSize/payload为ceil(width/4)*ceil(height/4)*16。完整DDS/RGBA/options/callback留在samples.zip；有mip样本flags=0xa1007，mipCount包含1x1尾级，不能以取证宣称已恢复mip。

`TextureDxtDdsEncoder` 新批次据此支持宽或高1/2的POT无mip图像，部分块使用已验收SampleBlockEncoder；插件通过原路由消费。组件矩阵扩为每配置1280整图、1792插件输入与独立NoOriginal；SDK扩为178合法图像/712资产（三模式），与两个受影响raw profile统一验证。消费者依赖均增加SampleBlockEncoder。源码待Actions验收，压缩mip/volume、NPOT、默认DXT1/1A仍拒绝，生产SDK未替换。
