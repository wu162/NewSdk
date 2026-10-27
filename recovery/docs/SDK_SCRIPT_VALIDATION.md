# 2026-10-27：按旧脚本参数重跑真实 MOD 验证

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
当前 `CoreStreamCompiler.StreamSettings` 未开启 StableSort，且显式设置 UsePrecompiled=false；旧脚本则使用 `/ss:true`、`/pc:true`。`DocumentProcessor.ProcessIncludedDocuments` 根据 UsePrecompiled 决定引用流是否生成输出，这与额外三组输出对应。
**以上是日志与源码所支持的原因线索；本轮没有分别只切换 ss/pc 的因果隔离实验，也没有证明 MOD 四文件仅有排序差异或游戏语义等价。**

## 覆盖与限制

完整未精简的仓库 `SDK/Mods/mymod` 输入，选中 base/apt/maps，但该 MOD 没有 APT、AdditionalMaps 入口，因此两组明确记为无输入。本轮没有 CDATA 输出，不据此增加真实音频/纹理覆盖。

搜索顺序中的 `SDK/Static.xml`、`Global.xml`、`Audio.xml` 是仓库现有占位声明；新建旧版工作区没有预编译参考 manifest，原日志保留三条加载失败后的回退警告。这是仓库现有输入的真实运行，不是完整游戏基础数据编译，也没有补入/下载 WBData。

本轮检查原始 linked streams；不声称最终 BIG、LoD、Additional、中立资产修复、UI 或游戏运行已重新验收。SDK 差分失败后协议步骤被工作流跳过，不能把它记为通过。

## 后续

若要求新版默认行为与旧脚本相符，先处理并分别验证稳定排序及预编译引用策略，再以同一完整 MOD 重跑严格差分，之后补最终打包验证。不要继续沿用关闭这些开关的旧 oracle 证明默认脚本兼容。
本轮只修正验证工具、执行验证并记录失败，没有修改编译器生产代码。
