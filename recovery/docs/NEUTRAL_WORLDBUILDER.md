# 中立与 Worldbuilder 资产修复

本分支从用户指定的 `recovery/legacy-sdk-cache` / `680c4f0c` 开始，带入
`release/compiler-2026.11.01-harness` / `feadec6e` 的修改；不会修改该发布分支。

## 使用

BIG 包含 base 时使用 `--fix-neutral-assets=true`，或在 MOD 的
`newmodcompiler.json` 中加入 `"fix-neutral-assets": true`。
也可修改 `NewModCompiler.exe.config` 已有的 `appSettings`：

```xml
<add key="fix-neutral-assets" value="true"/>
```

优先级：CLI > MOD 项目 JSON > EXE 配置 > false。报告中的来源分别为
`commandLine`、`project`、`applicationConfig`、`default`。出厂 EXE 配置为 false。
只需配置一个位置；CLI `--fix-neutral-assets=false` 可临时关闭。
BIG 仍要求每次在 CLI 指定 `--mod-version`。

```powershell
./NewModCompiler.exe build --sdk-root C:\NewSdk\SDK --mod C:\Mods\Example --out C:\Build\Example --mod-version 1.0 --fix-neutral-assets=true
```

修复顺序为 HashFix、递归 ModAssetResolver、LoD，然后 MakeBig。操作仅改变打包私有副本；
编译缓存、MOD 源码及官方参考输入不被修改。未开启时沿用原来的打包路径。
依赖参考丢失时使用 `PACKAGE_NEUTRAL_REFERENCE_MISSING`。

## 官方参考文件

来源：[CNCNZ WorldBuilder Mod SDK 1.12](https://files.cncnz.com/red_alert_3/modding/CCRA3WorldBuilder_ModSDKv112.zip)。
包内 InstallShield 安装程序的 `WBData_12.big` 包含完整四流，15931 个资产。
官方 manifest 与仓库原有 `SDK/builtmods/sagexml/Worldbuilder.manifest` 完全相同。
压缩只改变存储方式，不删资产、重写资产表或替换原始 payload。

文件位于 `SDK/builtmods/sagexml/`：

- `Worldbuilder.manifest`：官方原始资产表，SHA256 `1f621187352425571d72a0761fa56ca11cba525b12220cca27c94345080eeedd`。
- `Worldbuilder.imp`、`Worldbuilder.relo`：官方原始字节。
- `Worldbuilder.bin.parts/index.json` 与156个 `block-*.deflate`：完整 `.bin` 的8 MiB独立 raw Deflate 块，共498047426字节。
- `Worldbuilder.provenance.json`：ZIP、安装程序、BIG和原四流的长度与SHA256，以及完整还原核验。

原始 `.bin` 长1308421704字节，超过 GitHub 普通文件限制。块读取保持原始偏移，
只解压当前资产涉及的块，缓存当前块；无需先展开完整1.3GB。
索引的块SHA256对应解压后的字节。运行热路径不重复哈希；导入和完整还原负责校验。
完整还原SHA256：`682b7c654c589a61e90eae565860497e5847e02bcee4ab303d6161ecc7280e36`。

SDK原有 `builtmods/Static.manifest`、`Global.manifest`、`Audio.manifest` 仍需存在。
用户已有 `builtmods/Worldbuilder.*` 完整原始四流时优先采用该组；否则采用 sagexml 组。
同一组的普通 `.bin` 优先于 `.bin.parts`，不跨目录混合不完整输入。

从解出的官方BIG重新导入：

```powershell
python recovery/scripts/ImportWorldbuilderReferences.py import --big C:\Extracted\WBData_12.big --output C:\CleanSDK\builtmods\sagexml
```

运行新编译器不需要展开。仅原版工具对照或第三方工具需要原始 `.bin` 时：

```powershell
python recovery/scripts/ImportWorldbuilderReferences.py unpack --parts SDK/builtmods/sagexml/Worldbuilder.bin.parts --output-bin C:\OracleSDK\BuiltMods\Worldbuilder.bin
```

## 验证范围

`recovery/fixtures/neutral-worldbuilder` 是两个资产的小型 MOD：覆盖 `AM_Box01`
触发真实官方身份修正，`ObjectCreationList` 引用 Worldbuilder 专属 `AM_Bench`
触发模型等递归补入。没有人为改参考哈希或伪造 payload。

`neutral-worldbuilder.yml` 构建 Debug/Release，执行 App、真实 Core/CLI及原版
HashFix/Resolver/LoD组件差分；随后验证全局配置、项目配置和CLI覆盖、关闭开关、
缺参考失败与恢复重试，比较官方原工具 HashFix→Resolver→LoD→MakeBig 的全部BIG成员原始字节。
正式运行结果以本页后续收据链接为准；脚本存在不表示验收已经通过。
本轮不代表完整大型MOD或游戏内验收。
