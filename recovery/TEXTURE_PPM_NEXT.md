# PPM：公开源码适配、实际插件与专用SDK已闭环

仅PC，生产SDK树未替换，禁止强推，编译测试仅Actions。PPM成果在`validation/texture-public-ppm`已通过以下完整门槛并普通合入`recovery/texturecompiler-source`；各组保留实际sourceCommit/runId，合并不冒充新跑SDK。

## 本轮完成

- 固定复用IJG6b `rdppm.c`，原文件/通知、archive SHA及修改前后SHA见`src/TextureCompiler/Ppm/PROVENANCE.json`。适配有界内存、原P3/P6 token/comment/separator、整数截断和宽整数移位OR；不用样本ID/输出表或原/既有decoder回调。来源文件的candidate描述保留适配时状态，当前验收范围以本页及不可变运行收据为准。
- 原gold `7ab33356` /40836345313仍为probe-only：8438输入/16876原调用、6615解码/1823原错误、0重复不确定；完整27764文件独立审计通过。独立组件`782f6b97` /40837034188已完成D/R双入口及NoOriginal。
- 公共加载与实际插件`838b40b3860106cdfd677438c33c062b98d6702f` / [40838359589](https://github.com/wu162/NewSdk/actions/runs/40838359589)已completed/success：29选中完整组passed、19旧组复用。D/R各8438 gold四入口33752调用；41120实际插件输入/同量NoOriginal，31424成功/9664原错误/32不透明DXT1/1A明确拒绝，64成功atlas。完整源/三buffer/full cdata/旧尾独立审计见`evidence/texture/handoffs/20261029-ppm/component-archive-audit.json`。
- 补充volume边界`b999b6677be0010562850e4077be5b7180932913` / [40878697072](https://github.com/wu162/NewSdk/actions/runs/40878697072) D/R各11520完整输入及同量NoOriginal通过：5760标准成功、5760原invalid-volume错误，P3/P6竖长POT/NPOT、raw3/DXT3/5与完整mip。两份完整归档原字节一致，48715200预期buffer字节/配置已独立核对，见`volume-boundary-archive-audit.json`。该run整体仍为failure：SDK随后在新六DLL包回滚时被旧五DLL白名单拒绝；Original虽已15712调用成功，不冒充三模式SDK成功。
- 功能修复：`RestoreTextureCompiler.ps1`精确接受六运行DLL+config共七文件，计数由白名单导出，继续先预检所有安装后文件和备份哈希，再完整恢复原状态；没有放宽修改保护或允许任意文件。
- 专用SDK最终源码`bdecb8707c5bfa7095e5596d9d5c8fea963fdd38` / [40880308857](https://github.com/wu162/NewSdk/actions/runs/40880308857)已completed/success，仅补sdk-PcPpm，49组件全部复用。Original/Recovered/NoOriginal各15712真实资产/调用、32成功atlas、四产物和54977完整轨迹文件逐字节全等；408源图（392 raw+16 atlas）、139源码指纹、六DLL来源/通知/实际包、安装回滚、15712唯一原Core ID及49组件依赖关联全部独立审计通过，见`evidence/texture/handoffs/20261029-ppm/sdk-package-audit.json`。

## 可复现入口与下一轮计划

```text
python recovery/scripts/InspectTexturePpmVolumeArchive.py 40878697072 b999b6677be0010562850e4077be5b7180932913
python recovery/scripts/InspectTexturePpmSdkPackage.py --run-id 40880308857 --source-commit bdecb8707c5bfa7095e5596d9d5c8fea963fdd38 --out recovery/evidence/texture/handoffs/20261029-ppm/sdk-package-audit.json
```

当前PPM范围已闭环，无遗留实现或验收任务；合并输入一致性证明见`evidence/texture/handoffs/20261029-ppm/merge-validation.json`，不重复未变成功组件/SDK。后续新增PC实际输入需求按完整能力组推进，来源适配仍先独立测试分支、完整逐字节组件/消费与必要SDK通过后才合入；不自动运行旧SDK或完整mymod。默认DXT1/全不透明DXT1A与不确定扩展bitfields继续明确拒绝，原PGM等错误保持相同。

合并提交`05797db560a507f7ac8dd25460a495617471060f` / [40881954710](https://github.com/wu162/NewSdk/actions/runs/40881954710)已completed/success；Linux选择/无损存储检查通过，49组件复用、selected为空、Windows skipped。完整计划artifact12899894197及其原SHA已保存到同一handoff目录，不把计划检查说成新跑组件或SDK。
