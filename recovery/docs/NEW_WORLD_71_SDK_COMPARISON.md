# The New World 7.1 新旧 SDK 编译性能

[Windows 41256811133](https://github.com/wu162/NewSdk/actions/runs/41256811133) 已完成同机、同输入的完整冷编译对照，两边基础1流、APT13流、全局1流均退出0，各生成4057个文件。总墙钟1457.887→286.933秒，减少80.32%，约5.08倍。**整体状态为 outputs-differ，不能称为全输出原字节等价验收。**

| 冷编译部分 | 原 SDK 秒 | 新版秒 |
| --- | ---: | ---: |
| 基础数据 | 1416.428 | 268.522 |
| 全部13个APT | 28.868 | 9.410 |
| 全局数据 | 12.590 | 8.746 |
| 总计 | 1457.887 | 286.933 |

原版分项和总计为15个串行BAB进程的启动到退出之和；新版总计是单CLI jobs4进程启动到退出，分项由正常StreamStarted/Completed单调时钟取得，共享启动/收尾开销0.255秒只在总计内。下载、解压、哈希、原字节比较不计入。冷态指独立且最初不存在的编译器缓存/输出；OS页缓存未控制。仅1对样本，不能推断统计显著性或其他VM的绝对耗时。范围为loose编译，不含BIG/SKUDEF。

CPU总计1450.281→511.047秒；峰值工作集约1030.739→946.246 MiB；缓存1488366728→1787801316字节（增加约20.1%）。原始逐流/逐进程计时、命令、运行文件身份和完整输出清单均保留在归档。

仅 `base/mod/mod.bin` 不同，另外4056个文件长度与SHA256相同。按共同manifest定位17707个基础资产，330个不同资产全部为DXT1 Texture：12字节Texture根和128字节DDS头相同，差异仅在BC1块负载，共59846863字节。DXT3/5、raw纹理、音频及其他类型相同；全部APT/global四流与CDATA身份也相同。另对归档中全部60个链接流文件直接比较，59个原始字节相同，仅mod.bin不同。CDATA按完整长度/SHA256清单核对。该差异属于已有源码纹理的BC1编码字节边界，见 [S3说明](../src/Native/Texture/PublicS3/PATCHES.md)，没有归一化或忽略它后宣称等价，也未做游戏内或视觉等价验收。

本轮输入为ModDB完整7.1源码的用户授权裁剪版本：89条Joypad、1条八声道AudioFile及其唯一AudioEvent、3条不支持TGA的Texture，共94条声明；234张APT TGA仅清零未用色表字段，像素/编码负载不变。两引擎使用完全相同输入、物理路径、搜索根顺序与Mod自带xsd，Schema结束后还原。原始983MB归档、XML备份和前后哈希保留。源码包SHA256为 `0c59ed425b1ab55d926bfec858c6f60a0186a5d46e4cb5b295e42891a5ecdb41`，生产仍是已验收 `8050f22d` / `41244132060`，无生产修改。

热缓存失败单独保留：[41255726294](https://github.com/wu162/NewSdk/actions/runs/41255726294) 的新版冷15流成功后，热态AlliedLegionnaireInfantry→BaseInfantry继承丢失源XML。本轮显式cold-only，没有完整MOD热态或编辑速度比。原始未裁剪旧版32分钟与本轮其他VM数字不混算。

[摘要、CPU/内存/缓存与身份](../evidence/refactor/new-world-71/41256811133-1/summary.json)、[逐项CSV](../evidence/refactor/new-world-71/41256811133-1/cold-comparison.csv)、[330个BC1资产的逐项差异](../evidence/refactor/new-world-71/41256811133-1/base-asset-differences.json)、[调整清单](../evidence/refactor/new-world-71/41256811133-1/source-adjustments.json)、[原始归档](https://github.com/wu162/NewSdk/actions/runs/41256811133/artifacts/13064058982)。归档SHA256 `847331c171f1579c49b50e9634a4b7bc1b87cabfbe55aa6189fe854b14adabf3` 已下载复算。

用户随后授权修复TGA/8声道兼容性并合回性能分支再测；[PR #43](https://github.com/wu162/NewSdk/pull/43) 正在组件与小CLI验证。该后续结果尚未完成，不将此处旧二进制、94条裁剪结果冒充修复后原始媒体性能。
