# PC DXT3/5 压缩volume下一组

仅PC，分支recovery/texturecompiler-source；禁止强推，编译测试只用GitHub Actions。二维完整float mip已经7ae4af37/40524039917验收，不重复未变成功组。当前压缩volume仍拒绝，生产SDK未替换。

原版定向入口：`dxt-volume-boundaries`，416样本，13种XYZ尺寸×DXT3/5×有/无mip×8图案。复用既有Volume原生结构（每slice 24字节、level/volume各12字节），GetNVCompressionOptions并设置textureType@uint[118]=2，固定原DLL SHA256，核对token0x060000DB与RVA0x52950。收取header flags/linearSize/depth、mipCount、caps/caps2、完整payload和每级slice callback；结果只能标probe-only。

后续完整能力组：压缩volume无mip与递归float mip一起实现；复用TextureVolume的条带重排和既有TextureVolumeMipEncoder先Z平均、后XY triangle的float32 stores，不能套用二维从base分别生成每层的策略，也不能回写byte。新增helper与消费者同步登记依赖，共享raw volume消费者完整回归。原版压缩头/线性尺寸/XY-only header层数与XYZ payload层数须按新证据核实，不能仅套用raw头。

门槛：Debug/Release完整DDS逐字节、所有1/2小边/深度长条/方体/多块与完整尾链，源输入不变、独立布局与拒绝；全矩阵NoOriginal重放。插件VolumeTexture接入后两入口三缓冲区/full cdata及原版拒绝，实际SDK Original/Recovered/NoOriginal三模式、四产物和完整轨迹，NoOriginal每次Core调用前后断言原DLL不存在且未加载。NPOT、默认DXT1/1A仍明确拒绝；专用MOD不冒充完整mymod。
