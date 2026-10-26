# P7：内建 BIG/SKUDEF

状态：apt/maps 格式/成员/回滚主组已通过两配置，但收尾原Windows命令实证发现APT版本标记差异：旧脚本产生 `20 0d 0a`（空格+CRLF），旧候选只写 `0d 0a`。发行包刷新已被明确阻止。源码、App断言与CLI oracle已改为实测三字节，尚未重编译/验收；不能把旧主组成功冒认为这项修正版已通过。P5/P6已验收版本保持独立，base后处理仍未完成。

```powershell
./NewModCompiler.exe build --sdk-root C:\NewSdk\SDK --mod C:\Mods\Example --out C:\Build\Example --package-content apt,maps --package-format big
```

`apt`、`maps` 可单独或组合选择；`big` 必须显式排除 `base`。默认内容含base，因此仅添加 `--package-format big` 会明确拒绝。base 依赖的 LoDStreamBuilder 尚未恢复；不能把未后处理的 raw base 流打包后当作完整游戏 MOD。

成功非空构建的 `published` 目录只发布 `<MOD目录名>_1.0.big` 与同名 `.skudef`。SKUDEF 使用原脚本默认的 ASCII/CRLF：`mod-game 1.12` 和 `add-big <包名>`。当前版本/游戏版本为该固定默认，包名及成员限定受支持ASCII；没有启用未经原工具及游戏读取合同确认的Unicode编码。

| 本次产物 | BIG 内路径 |
| --- | --- |
| APT四输出 | `data/aptui/<流名>.manifest/.bin/.imp/.relo` |
| APT CustomData | `data/aptui/<流名>/cdata/<文件>.cdata` |
| APT派生版本 | `data/aptui/<流名>.version`，原Windows命令实测三个字节空格+CRLF（`20 0d 0a`） |
| MapMetaData四输出 | `data/additionalmaps/<流名>.manifest/.bin/.imp/.relo` |
| maps CustomData | `data/additionalmaps/<流名>/cdata/<文件>.cdata` |

成员只来自本次已确认的流产物清单，APT版本由本次有效task精确派生；不扫描陈旧输出、intermediate、MOD Additional或预编译assets。空可选集合保留上一代，不生成空BIG。完整precompiled AssetMerge、HashFix→ModAssetResolver→LoD、额外文件打包尚需各自接入，不由此推断完成。

BIG4遵循原MakeBig端序、NUL路径、`L219`目录尾和连续原始payload；路径小写反斜线、case重复/遍历/歧义路径及32位长度溢出拒绝。Archive与SKUDEF在本次staging生成，再沿已有发布/缓存/必需渠道事务确认；取消和失败不替换上一代。生产不启动MakeBig或编译worker，外部原工具仅用于隔离验证。

验证入口为 `TestCompilerBig.ps1` 与 `ValidateCompilerBig.py`：真实单选/组合、完整CDATA、APT版本、SKUDEF原始字节、陈旧代替换、取消/发布失败重试，以及同清单原SDK MakeBig的结构/完整成员与内容对照。原工具的目录遍历顺序不稳定，不能错误要求两份archive整体字节顺序相同；每个成员内容仍逐字节核验。格式/路径/内容证据不等于实际启动游戏的运行验收。

下一组先重建并验收三字节版本修正，再按新的真实成功归档重新pin发布准备器及其原命令期望；旧二字节候选保持禁止刷新。然后优先恢复base必需LoD并核验旧脚本顺序，再扩展必要AssetMerge/额外文件及版本参数；不启动游戏、安装MOD或写CurrentMOD.ini。正式发行包必须在当前候选终验后刷新，旧P6包不自动包含本能力。
