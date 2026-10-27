# PC DXT3/5 压缩volume下一组

仅PC，分支recovery/texturecompiler-source；禁止强推，编译测试只用GitHub Actions。二维完整float mip已经7ae4af37/40524039917验收，不重复未变成功组。当前压缩volume仍拒绝，生产SDK未替换。

原版定向入口：`dxt-volume-boundaries`，416样本，13种XYZ尺寸×DXT3/5×有/无mip×8图案。复用既有Volume原生结构（每slice 24字节、level/volume各12字节），GetNVCompressionOptions并设置textureType@uint[118]=2，固定原DLL SHA256，核对token0x060000DB与RVA0x52950。收取header flags/linearSize/depth、mipCount、caps/caps2、完整payload和每级slice callback；结果只能标probe-only。

后续完整能力组：压缩volume无mip与递归float mip一起实现；复用TextureVolume的条带重排和既有TextureVolumeMipEncoder先Z平均、后XY triangle的float32 stores，不能套用二维从base分别生成每层的策略，也不能回写byte。新增helper与消费者同步登记依赖，共享raw volume消费者完整回归。原版压缩头/线性尺寸/XY-only header层数与XYZ payload层数须按新证据核实，不能仅套用raw头。

门槛：Debug/Release完整DDS逐字节、所有1/2小边/深度长条/方体/多块与完整尾链，源输入不变、独立布局与拒绝；全矩阵NoOriginal重放。插件VolumeTexture接入后两入口三缓冲区/full cdata及原版拒绝，实际SDK Original/Recovered/NoOriginal三模式、四产物和完整轨迹，NoOriginal每次Core调用前后断言原DLL不存在且未加载。NPOT、默认DXT1/1A仍明确拒绝；专用MOD不冒充完整mymod。

## 原版布局与本轮检查点（待验收）

取证源码 `6d065a9edb5101d8a1387f7b508649e9d851adc7` / [40525480986](https://github.com/wu162/NewSdk/actions/runs/40525480986) completed/success，416样本probe-only，原DLL SHA256固定4f559428...e9d9e。完整DDS/RGBA/options与callback在`evidence/texture/dxt-volume-boundaries/40525480986-1`。

无mip flags=0x881007、mipCount=0、caps=0x1000；有mip flags=0x8a1007、caps=0x401008，mipCount只按XY。caps2=0x200000、depth写第0级深度。linearSize是第0级单slice的ceil(width/4)*ceil(height/4)*16。payload按level/z/y-block/x-block拼接，XYZ各max(1,/2)直到全1，callback每slice为size:level,width,height,z,depth。1×1×8有mip头count=1，但实际4层15个slice/240字节payload，DDS总长368。

新TextureDxtVolumeDdsEncoder消费既有float小块与从VolumeMipEncoder提取的NextLevel；保持Z平均后XY、每tapfloat32存储，无byte中转。两插件入口接入VolumeTexture；全部代码/测试准备完后集中验证。组件目标每配置5568完整DDS、7856标准/volume插件输入及全量NoOriginal重放；新PcDxtVolumes专用SDK186合法TGA/PNG/BMP条带图像，有/无mip合计1488资产/实际调用，三模式与逐次隔离断言。依赖变化的raw volume/图像与原SDK profile一并选择。当前均待验证，不能将检查点写为完成。
