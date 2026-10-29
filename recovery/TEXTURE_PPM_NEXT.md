# PPM公开源码适配进行中（独立测试分支，未正式接入）

本轮在`validation/texture-public-ppm`直接修改固定IJG6b `rdppm.c`：保留原文件及版权，另存适配副本与修改前后SHA。使用有界内存、P3/P6判别、PC整数截断、token/comment及binary separator规则；独立native DLL与候选托管程序集，不改生产ImageLoader/项目/插件。

新增8438个安全有界原LoadImageA探针，保存完整源/首次与重复RGBA/尺寸/alpha/错误及候选Decode/Load比较。候选初值中的alpha/channel/NPOT及边界仍以实际运行修正，probe-only绝非正式组件/SDK通过。不调用原版P6残缺/尺寸溢出域。通过唯一集中入口`[texture-ppm-load]`显式运行；新组priority只改变执行顺序、不减少选组或断言，SDK固定在组件之后。

下一步取得原gold，审计归档并按差分继续修改公开源码；稳定后进行完整D/R和NoOriginal回放。随后真实DDS/mip/volume/atlas/插件与必要SDK取得完整逐字节验收才普通合入`recovery/texturecompiler-source`。禁止强推，编译测试仅Actions。

# 下一独立能力：PPM（尚未实现）

2026-10-29（北京时间）。本轮主线仍是DIB/BMP色表的组件与专用SDK；本页仅保存等待期间取得的下一项静态入口，不计作功能支持、原gold或SDK验收。

固定原DLL dispatcher的format5经0x168E2/0x168EA调用`0x15240..0x154B0`。624字节完整原函数、反汇编及SHA见`evidence/texture/ppm-native/20261029`，用`InspectTexturePpmNative.py`只读复现。P3/P6签名、十进制token状态机、整数`sample*255/maxval`截断、P6 maxval<=255及单separator/CRLF跳过已可从原字节核对。原高度/宽度/maxval零值被拒绝；原尺寸乘法和残缺P6读取缺少完整保护，不对未知越界域调用原DLL。

现成公开候选已在仓库：`recovery/reference/ijg6b/rdppm.c`，固定IJG6b archive SHA `75c3ec241e9996504fe02a9ed4d12f16b74ade713972f3db9e65ce95cd27e35d`，源文件SHA `4c800ecf405e821f7d350eb6fe9ea1a336940a615c1b45d15017100bdf11e7d6`，原通知/README及PROVENANCE不改。其PPM/PGM读取器可作为公开源码路线；原生逻辑与IJG的重采样舍入、inline注释、whitespace、P6 separator/16bit范围并不必然相同，应按通用规则适配，不能跑一次未改版不一致便停止，也不能用gold表或调用原/既有decoder冒充替换。

下一轮先建独立`validation/texture-public-ppm`，取得安全有界的原LoadImageA完整源/RGBA/尺寸/hasAlpha/错误与重复确定性gold，再连续适配公开候选。重点为P3/P6、P2/P5原拒绝或其他实际行为、maxval/CRLF/空白/注释、尾部数据、POT/NPOT/partial和通道/alpha；后两项尚无运行证据，不按静态猜测定默认值。首阶段可独立Decode/Load，但组件成功仍留测试分支；正式路由及实际受影响的DDS/mip/volume/atlas/插件与必要SDK取得完整逐字节验收后，才普通合入贴图分支。旧成功输入复用，完整mymod继续降频，禁止强推，编译测试仅Actions。
