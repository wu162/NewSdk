# 最新：PPM公共四入口与实际插件已通过，待旧回归和专用SDK

`838b40b3` /40838359589的优先两组已通过：D/R各8438 gold四入口33752调用与41120实际插件/同量NoOriginal，31424成功/9664原错误/32明确拒绝，64 atlas成功。早期artifact12882330778完整D/R归档独立审计通过，SHA/完整源与三buffer/full cdata见handoffs/20261029-ppm/component-archive-audit.json；不是整批或SDK通过。其余27受影响组仍运行、19复用，CI当前源码输入不再改动。

额外竖长条带单独加入自动ppm-volume-boundaries：11520完整矩阵（5760标准正向/5760原invalid-volume），尚未编译运行，不修改已通过组的代码/fixture。新SDK前置要求全部49组件。下一步收主run终态，再显式[texture-ppm-sdk]：仅补此新边界及实际失败/变化组，接着跑新PcPpm15712资产/模式、32成功atlas、六DLL包/安装回滚/原Core唯一ID/四产物/全轨迹；不重跑未变成功组或完整MOD。业务及必要SDK全验收后才普通合入主分支。详见RESUME。

# PPM当前：独立组件通过，实际loader/plugin与完整消费验证中

- 原gold和首次成功：`7ab33356` /40836345313（probe-only，8438输入/16876原调用、6615解码/1823原错误、0不确定），独立27764文件/3315060原RGBA字节审计通过；`782f6b97` /40837034188 success，D/R各双入口16876调用、6630120完整RGBA字节及原错误全等、60安全边界/逐次NoOriginal。
- `838b40b3860106cdfd677438c33c062b98d6702f` / [40838359589](https://github.com/wu162/NewSdk/actions/runs/40838359589)正在验证公共四入口与实际`.ppm`插件：每配置41120输入/同量NoOriginal、64必须成功atlas、完整raw3/DXT3/5标准/volume/mip和旧cdata。当前四种raw尺寸均为合法POT/NPOT转POT体积条带；PGM/非法header原错误和32不透明DXT1/1A拒绝保留，额外竖长条带的非法volume拒绝是后续需补的一项边界，不冒充当前覆盖。新能力组先跑并上传texture-early归档，再跑所有真实受影响旧组；整批尚未终态。
- 已接完整六DLL包与安装/回滚严格文件集，PPM源码/通知/修改前后SHA保留；包/实际入口未提前计验收。专用`TestTexturePpmSdk.ps1`已准备：392正向raw源+16 atlas源，每模式15712资产、32成功atlas，原Core唯一ID/三模式四产物/完整轨迹/包与安装回滚。全部48组件收齐后在交接提交显式`[texture-ppm-sdk]`，不自动跑旧SDK或完整mymod。此新SDK尚未运行。
- 成果均在独立`validation/texture-public-ppm`。等待完整组件与必要SDK、独立包/产物/轨迹审计后再普通合入主贴图分支；源/测试变更只补实际受影响组，不强推，不在本机编译测试。

以下为阶段历史，当前状态以上为准。

# PPM原gold已完成；宽整数打包修复待D/R完整回放

- 原探针`7ab33356` / `40836345313`为probe-only：8438输入/16876原调用、6615解码/1823原错误、0重复不确定；候选7918匹配/520不同。完整27764文件与3315060原RGBA字节独立审计通过，归档SHA `00c9854e3f026723d378afa05c1b1241e30c23b7a37f40b718ced2885e7ee339`。全部原hasAlpha=false，NPOT向上POT；这些是原版观察而非正式PPM支持。
- 已按原字节及完整反例修复IJG行读取器保留宽整数scaled component，按原PC移位OR后转换RGBA；解决额外P6 separator字节大于maxval时的邻通道位溢出，非样本ID分派或输出表。修改前后SHA及差分状态保存在PROVENANCE.json。
- 本批只运行`public-ppm`完整8438 gold Decode/Load、D/R native/managed和逐次NoOriginal，另60个残缺binary/overflow/null安全拒绝只针对恢复版，不调用原未知域。旧成功46组件复用；生产ImageLoader/Plugin/5-DLL包尚未改变，PPM继续留独立测试分支。
- 下一步核实完整回放，随后扩展实际插件/raw/DXT/mip/volume/atlas和必要SDK，再普通合入贴图主分支。编译测试仅Actions，禁止强推。

# PPM公开源码适配进行中（独立测试分支，未正式接入）

本轮在`validation/texture-public-ppm`直接修改固定IJG6b `rdppm.c`：保留原文件及版权，另存适配副本与修改前后SHA。使用有界内存、P3/P6判别、PC整数截断、token/comment及binary separator规则；独立native DLL与候选托管程序集，不改生产ImageLoader/项目/插件。

新增8438个安全有界原LoadImageA探针，保存完整源/首次与重复RGBA/尺寸/alpha/错误及候选Decode/Load比较。候选初值中的alpha/channel/NPOT及边界仍以实际运行修正，probe-only绝非正式组件/SDK通过。不调用原版P6残缺/尺寸溢出域。通过唯一集中入口`[texture-ppm-load]`显式运行；新组priority只改变执行顺序、不减少选组或断言，SDK固定在组件之后。

下一步取得原gold，审计归档并按差分继续修改公开源码；稳定后进行完整D/R和NoOriginal回放。随后真实DDS/mip/volume/atlas/插件与必要SDK取得完整逐字节验收才普通合入`recovery/texturecompiler-source`。禁止强推，编译测试仅Actions。

# 下一独立能力：PPM（尚未实现）

2026-10-29（北京时间）。本轮主线仍是DIB/BMP色表的组件与专用SDK；本页仅保存等待期间取得的下一项静态入口，不计作功能支持、原gold或SDK验收。

固定原DLL dispatcher的format5经0x168E2/0x168EA调用`0x15240..0x154B0`。624字节完整原函数、反汇编及SHA见`evidence/texture/ppm-native/20261029`，用`InspectTexturePpmNative.py`只读复现。P3/P6签名、十进制token状态机、整数`sample*255/maxval`截断、P6 maxval<=255及单separator/CRLF跳过已可从原字节核对。原高度/宽度/maxval零值被拒绝；原尺寸乘法和残缺P6读取缺少完整保护，不对未知越界域调用原DLL。

现成公开候选已在仓库：`recovery/reference/ijg6b/rdppm.c`，固定IJG6b archive SHA `75c3ec241e9996504fe02a9ed4d12f16b74ade713972f3db9e65ce95cd27e35d`，源文件SHA `4c800ecf405e821f7d350eb6fe9ea1a336940a615c1b45d15017100bdf11e7d6`，原通知/README及PROVENANCE不改。其PPM/PGM读取器可作为公开源码路线；原生逻辑与IJG的重采样舍入、inline注释、whitespace、P6 separator/16bit范围并不必然相同，应按通用规则适配，不能跑一次未改版不一致便停止，也不能用gold表或调用原/既有decoder冒充替换。

下一轮先建独立`validation/texture-public-ppm`，取得安全有界的原LoadImageA完整源/RGBA/尺寸/hasAlpha/错误与重复确定性gold，再连续适配公开候选。重点为P3/P6、P2/P5原拒绝或其他实际行为、maxval/CRLF/空白/注释、尾部数据、POT/NPOT/partial和通道/alpha；后两项尚无运行证据，不按静态猜测定默认值。首阶段可独立Decode/Load，但组件成功仍留测试分支；正式路由及实际受影响的DDS/mip/volume/atlas/插件与必要SDK取得完整逐字节验收后，才普通合入贴图分支。旧成功输入复用，完整mymod继续降频，禁止强推，编译测试仅Actions。
