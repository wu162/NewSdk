# PPM续接：实际公共加载与插件已通过，剩余回归/SDK待收齐

PPM成果保存在独立`validation/texture-public-ppm`，尚未合入`recovery/texturecompiler-source`。仅PC，原SDK树未替换，禁止强推，编译测试仅Actions。

## 本轮完成

- 固定复用IJG6b `rdppm.c`，未改原参考文件；适配副本、上游通知/README、archive SHA及修改前后SHA见`src/TextureCompiler/Ppm/PROVENANCE.json`。改为有界内存、原P3/P6 token/comment/separator规则、整数截断和宽整数移位OR，保留原溢出到相邻通道的确定行为；不用样本ID/输出表或原/既有decoder回调。
- 原gold：`7ab33356` /40836345313为probe-only，8438输入/16876原调用、6615解码/1823原错误、0重复不确定，完整27764文件审计通过。SHA `00c9854e3f026723d378afa05c1b1241e30c23b7a37f40b718ced2885e7ee339`；原hasAlpha=false、NPOT向上POT。首版7918匹配/520差异已按通用打包规则修复。
- 独立组件`782f6b97` /40837034188 success：D/R双入口及NoOriginal全量相同。接公共ImageLoader/`.ppm`后，`838b40b3860106cdfd677438c33c062b98d6702f` / [40838359589](https://github.com/wu162/NewSdk/actions/runs/40838359589)的两优先组已passed：D/R各8438 gold四入口33752调用、13260240完整RGBA字节及60候选安全拒绝；每配置41120实际插件输入/同量NoOriginal（31424成功/9664原错误/32不透明DXT1/1A明确拒绝），64 atlas必须成功，完整三buffer/full cdata/旧尾全等。
- 早期artifact12882330778已恢复并保存收据；完整D/R插件归档审计见`evidence/texture/handoffs/20261029-ppm/component-archive-audit.json`，每配置222892952预期缓冲区字节。两新组于07:56:22–08:02:00 UTC先完整执行，08:02:04上传完成，随后才继续27受影响旧组；19旧组复用。此观察不是整个批次通过或跨批耗时基准。

## 当前待验收

- 40838359589的27旧回归仍运行；到本轮交接不宣称整体success。接入用Managed/Ppm/Jpeg/PublicS3/Colour/Dxt业务文件此后未变，成功组按实际依赖保留锚点。
- 六DLL来源包/安装回滚已实现，尚待SDK真实执行。额外`ppm-volume-boundaries`组已准备11520完整矩阵：同144个POT/NPOT竖长源，5760标准成功正对照及5760原invalid-volume错误；没有修改本批已通过组的源码或fixture，补充组尚未编译运行。
- `TestTexturePpmSdk.ps1`已准备：392正向raw源+16 atlas源，三模式每模式15712资产/32成功atlas，完整六DLL包、安装回滚、原Core唯一ID、四产物/全轨迹/逐次NoOriginal。SDK前置要求全部49组件；缺失/失败即在构包/SDK调用前停止。

## 下一轮直接执行

1. 在`validation/texture-public-ppm`收取40838359589终态及完整证据；保留每个成功组原sourceCommit/runId，只修失败、缺失或实际输入变化组，不重跑未变成功矩阵。
2. 更新该分支RESUME并在提交消息显式加`[texture-ppm-sdk]`。统一入口自动补新volume边界及实际受影响失败组，组件全部成功后仅跑新sdk-PcPpm；不自动启动旧SDK或完整mymod。不要在主贴图分支触发尚不存在的PPM组。
3. SDK完成后运行`InspectTexturePpmSdkPackage.py --run-id <run> --source-commit <sdk-source> --out recovery/evidence/texture/handoffs/20261029-ppm/sdk-package-audit.json`，核对全部产物/轨迹、六DLL/来源/通知、安装回滚、ID及49组件实际依赖/输入关联。新脚本已准备但尚未在真实PPM SDK归档执行。
4. 必要完整组件/SDK及独立审计全部通过后再普通合入`recovery/texturecompiler-source`。默认DXT1/不透明DXT1A继续拒绝；不因收尾重跑完整MOD。
