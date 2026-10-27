# 2026-10-27：旧脚本参数差分与修复

## 修复与最终验收（2026-10-27）

**已修复并通过。** [完整运行 40440661815](https://github.com/wu162/NewSdk/actions/runs/40440661815) 为 success；生产二进制、测试和发行源码均为 `57354fbc013369eac1ad4c6e3346134d81a68a6c`。本轮 Debug/Release 均重新构建，后续说明更新仅改文档。

- `/ss`、`/pc`、`/slowclean` 默认 true，与旧编译脚本一致；各自接受 true/false，缓存仍默认关闭。默认值及命令行来源进入报告。原 BAB 组件 oracle 同步相同默认策略。
- 完整未精简仓库 `mymod`：两配置都恰好输出 MOD 四文件，manifest/bin/imp/relo 逐字节一致，差异数为 0；此前 static/global/audio 三组额外输出已消除。
- 真实 MOD BIG：两配置各 12 个成员（主流四文件＋原 LoD 八输出），与旧 BAB 输出经原 LoD/MakeBig 打包的成员名称和原始内容逐字节一致；SKUDEF 文件名及 ASCII/CRLF 内容与旧脚本默认版本一致。此门槛比较成员，不要求不同工具的归档遍历顺序或整个 BIG 文件哈希相同。
- 应用、Core、原 BAB、Texture、真实 CLI/取消、资源根、资产缓存、音频 CDATA/W3X、会话缓存、BIG/Additional/中立零插入及协议、最终五文件 ZIP 独立运行均已通过。
- [原始日志和产物](https://github.com/wu162/NewSdk/actions/runs/40440661815/artifacts/12707135831)，归档 SHA256 `511afe2ebb3117b5c79e0c9adad34bc0a735b4f16c3386a08dd754ba131d61dd`；完整收据为 `evidence/refactor/sdk-mymod/40440661815-1/validation.json`，[精简收据](../evidence/refactor/sdk-mymod/40440661815-1/summary.json)。

原失败运行和收据保持 failure。新的成功来自本轮修复后的新构建，不是改写或放宽旧断言。完整 mymod 仍没有 APT/地图输入，这两组的实际覆盖来自独立生产用例；完整游戏数据、WBData 和游戏内运行不由本轮证明。预编译引用缺少参考时的回退警告继续保留，不自动下载或制造参考输入。

## 原失败记录（修复前）

**结论：未通过新旧原始输出一致性验收。** 旧 BAB、新版 Debug、新版 Release 三个编译进程均成功退出，SDK 输入未改变；两配置都出现相同差异。不能把此前“关闭旧选项的基线通过”推广为与默认旧编译脚本一致。

- [Actions 运行 40439297653](https://github.com/wu162/NewSdk/actions/runs/40439297653)（failure，保留原终态）。
- 二进制、测试源码均为 `c700bc4a870252e6b23bcd2d580c66711e92d466`；两配置本轮重新编译，没有复用历史 EXE。
- [原始产物及日志](https://github.com/wu162/NewSdk/actions/runs/40439297653/artifacts/12705393039)：ZIP SHA256 `101826990703f53fc21b4bb3c15a2beb650e3fc2020bd3bcd91512041df7a7ee`。
- [精简收据](../evidence/refactor/sdk-script/40439297653-1/summary.json)；完整 `validation.json`、输入指纹、原配置、请求、进程控制器记录和两边原始输出保存在上述归档中。精简收据不替代完整原始证据。

## 基线

依据 `SDK/defaultscript.cs` 第 3/4/5 步的实际 BAB 调用，不依据旧参数表猜测。三处均启用：
`/ls:true /pc:true /res:true /slowclean:true /ss:true`，并保留 `/osh:false` 等原设置。
按用户要求单独关闭缓存：`/bc:false /sc:false /psc:false /csc:false /fsc:false /atc:false`。

旧 BAB 保留原配置中的 LowLOD/MediumLOD 表；资源搜索顺序保持 SDK 当前目录→MOD对应目录→Mods→共享 Art/Audio/SageXml。两边传入同样的绝对搜索路径，输出/中间目录改为新建隔离目录，防止污染输入或复用残留。实际全部参数见收据的 processes/arguments。

新版使用当前实现，显式选择 loose，以直接比较编译输出；未为这次验证修改生产行为或强行传入其拒绝的 `/ss:true`、`/pc:true`。

## 原始差异

Debug 与 Release 的新输出彼此相同；它们对旧版的差异一致：

| 文件 | 旧版/新版字节数 | 首个不同偏移（从 0 起） |
| --- | ---: | ---: |
| mods/mymod/data/mod.manifest | 4995 / 4995 | 4 |
| mods/mymod/data/mod.bin | 209548 / 209548 | 0 |
| mods/mymod/data/mod.imp | 64 / 64 | 0 |
| mods/mymod/data/mod.relo | 4656 / 4656 | 0 |

另外，新版多出 `static`、`global`、`audio` 各四文件，共 12 个文件；旧版只有 MOD 四文件，新版共 16 个。没有删除额外输出或重排/归一化来取得通过。

旧版 stdout 明确记录 `Stable sorting assets` 及 schema/asset dependency sorting。
失败版本的 `CoreStreamCompiler.StreamSettings` 未开启 StableSort，且显式设置 UsePrecompiled=false；旧脚本则使用 `/ss:true`、`/pc:true`。`DocumentProcessor.ProcessIncludedDocuments` 根据 UsePrecompiled 决定引用流是否生成输出，这与额外三组输出对应。
**以上是日志与源码所支持的原因线索；本轮没有分别只切换 ss/pc 的因果隔离实验，也没有证明 MOD 四文件仅有排序差异或游戏语义等价。**

## 覆盖与限制

完整未精简的仓库 `SDK/Mods/mymod` 输入，选中 base/apt/maps，但该 MOD 没有 APT、AdditionalMaps 入口，因此两组明确记为无输入。本轮没有 CDATA 输出，不据此增加真实音频/纹理覆盖。

搜索顺序中的 `SDK/Static.xml`、`Global.xml`、`Audio.xml` 是仓库现有占位声明；新建旧版工作区没有预编译参考 manifest，原日志保留三条加载失败后的回退警告。这是仓库现有输入的真实运行，不是完整游戏基础数据编译，也没有补入/下载 WBData。

本轮检查原始 linked streams；不声称最终 BIG、LoD、Additional、中立资产修复、UI 或游戏运行已重新验收。SDK 差分失败后协议步骤被工作流跳过，不能把它记为通过。

## 历史轮次范围

上轮只修正验证工具、执行验证并记录失败，没有修改生产代码；本轮已按页首说明修复生产默认策略，并通过同一完整 MOD 原始差分和最终打包。继续保留严格旧脚本 oracle，不恢复关闭这些开关的旧基线。
