# PC DXT3/5 压缩volume（已验收）

仅PC，分支recovery/texturecompiler-source；禁止强推，编译测试只用GitHub Actions。二维完整float mip已经7ae4af37/40524039917验收，不重复未变成功组。当前PC POT压缩volume无mip/完整float mip已完成组件、两插件入口和专用SDK验收；生产SDK未替换。

原版定向入口：`dxt-volume-boundaries`，416样本，13种XYZ尺寸×DXT3/5×有/无mip×8图案。复用既有Volume原生结构（每slice 24字节、level/volume各12字节），GetNVCompressionOptions并设置textureType@uint[118]=2，固定原DLL SHA256，核对token0x060000DB与RVA0x52950。收取header flags/linearSize/depth、mipCount、caps/caps2、完整payload和每级slice callback；结果只能标probe-only。

后续完整能力组：压缩volume无mip与递归float mip一起实现；复用TextureVolume的条带重排和既有TextureVolumeMipEncoder先Z平均、后XY triangle的float32 stores，不能套用二维从base分别生成每层的策略，也不能回写byte。新增helper与消费者同步登记依赖，共享raw volume消费者完整回归。原版压缩头/线性尺寸/XY-only header层数与XYZ payload层数须按新证据核实，不能仅套用raw头。

门槛：Debug/Release完整DDS逐字节、所有1/2小边/深度长条/方体/多块与完整尾链，源输入不变、独立布局与拒绝；全矩阵NoOriginal重放。插件VolumeTexture接入后两入口三缓冲区/full cdata及原版拒绝，实际SDK Original/Recovered/NoOriginal三模式、四产物和完整轨迹，NoOriginal每次Core调用前后断言原DLL不存在且未加载。NPOT、默认DXT1/1A仍明确拒绝；专用MOD不冒充完整mymod。

## 原版布局证据

取证源码 `6d065a9edb5101d8a1387f7b508649e9d851adc7` / [40525480986](https://github.com/wu162/NewSdk/actions/runs/40525480986) completed/success，416样本probe-only，原DLL SHA256固定4f559428...e9d9e。完整DDS/RGBA/options与callback在`evidence/texture/dxt-volume-boundaries/40525480986-1`。

无mip flags=0x881007、mipCount=0、caps=0x1000；有mip flags=0x8a1007、caps=0x401008，mipCount只按XY。caps2=0x200000、depth写第0级深度。linearSize是第0级单slice的ceil(width/4)*ceil(height/4)*16。payload按level/z/y-block/x-block拼接，XYZ各max(1,/2)直到全1，callback每slice为size:level,width,height,z,depth。1×1×8有mip头count=1，但实际4层15个slice/240字节payload，DDS总长368。

TextureDxtVolumeDdsEncoder消费既有float小块与从VolumeMipEncoder提取的NextLevel；保持Z平均后XY、每tapfloat32存储，无byte中转。条带输入通过TextureVolume原重排，预检正POT、精确RGBA存储及小于16M像素，原宽非高倍数的BadTextureFile错误保留。Texture/OnDemandTexture接入VolumeTexture。

## 完整能力组验收

源码 `aaacb3d25fdb99eddaa8ed4ab23701437e840ec2` / [40526443463](https://github.com/wu162/NewSdk/actions/runs/40526443463) attempt1 completed/success。整批14受影响组执行、16成功组复用，不重测未变二维DDS/triangle和颜色/alpha/float小块。取证不能替代此业务验收。

- Debug/Release各5568完整volume DDS，2,310,019检查；两格式×有/无mip×87种XYZ尺寸×16图案，覆盖1/2小边、1×1×64等极端轴、16³/32×8×4/8×32×4/4×4×32，均匀/双色/渐变/半步alpha/随机。完整头/payload逐字节、独立逐slice callback/linearSize/XY头层数与XYZ尾链、源输入不变，30明确拒绝。完整5568 NoOriginal重放，各2,207,232字节比较通过。
- Debug/Release各7856标准/volume实际入口输入（6336成功输出、1520原版拒绝匹配，其中48为宽非高倍数的合法POT图像），18明确拒绝、38,159,776字节比较；三缓冲区、源文件、新旧完整cdata一致，同量NoOriginal通过。
- PcDxtVolumes专用SDK186条带图像：TGA侧长1/2/4/8、深度1/2/4/8、八图案共128输入，40 PNG和18 BMP；两入口/两格式/有无mip共1488资产。Original/Recovered/NoOriginal各1488实际Core调用，四产物和完整轨迹一致。NoOriginal每次调用前后确认原DLL不存在且未加载，自有NativeColour/NativeAlpha同时部署。
- 原PcDxtNoMip/PcDxtMips各712资产三模式与PcNoMip/PcMips raw SDK各600资产三模式通过。共享VolumeMipEncoder影响的raw volume完整矩阵与所有图像消费者、base元数据也通过。

收据位于`evidence/texture/{dxt-volume-dds,dxt-images,sdk-dxt-volumes,batch}/40526443463-1/validation.json`。生产SDK DLL/config未替换，不宣称完整mymod/全TextureCompiler完成。下一组 [NPOT](TEXTURE_NPOT_NEXT.md)；默认DXT1/1A仍拒绝。
