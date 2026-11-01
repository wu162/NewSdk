# SDK 集中缓存

缓存由本次 `--sdk-root` 和物理 MOD 路径决定，统一位于
`<sdk-root>/SuperCompilerCache/<ModKey>/`。`ModKey` 由清理并截短至 12 字符的 MOD
目录名，以及规范化物理 MOD 根路径 SHA-256 的前 16 字节组成。计算 ModKey 时只散列路径字符串，
不扫描或散列 MOD 文件。同一个 MOD 更换输出目录、版本或构建时间不会更换分区；
同名但物理路径不同的 MOD 各自隔离。

| 分区内路径 | 内容 |
| --- | --- |
| `session/` | 原 BAB SessionCache XML/Deflate、`.tmp/.old`、既有字符串哈希与 `.NewModCompiler/cdata` |
| `build-cache/Win32/` | 原类型、实例键的 `.asset/.cdata` |
| `intermediate/` | 按逻辑源相对路径生成的中间资产 |
| `streams/<base\|apt\|maps>/<entry>/` | 可复用四流、CDATA 和任务期间的 `backend-work` |
| `generated/fla/` | 裸 FLA 的稳定 XML 声明 |
| `temp/<BuildId>/` | 打包 LoD、APT version 和显式中立修复输入，打包后清理本 BuildId |

整个请求共享一个不可变 `CacheLayout`。SDK 配置中的 `buildCacheRoot` 不决定缓存
落点；`/bc` 未显式指定时仍采用 SDK `buildCache` 开关。`/sc`、`/bc`、`/csc`、
`/psc`、`/fsc`、`/atc`、`/ss`、`/pc`、`/slowclean` 的原行为与 BAB 格式保留。

MOD 源码在计划、编译、失败清理及打包期间只读，`newmodcompiler.json` 只读取。
MOD 位于当前 SDK 的 `Mods/` 内也适用。`--out` 及诊断文件继续必须位于 SDK/MOD
源码目录外；SDK 内允许的编译写入目标仅为当前 MOD 分区。物理路径别名、重解析点、
目录权限及 Win32 路径限制仍须通过原路径边界；失败不回退到其他缓存位置。
`plan`、`doctor` 只解析和检查布局，不创建缓存目录。

`/scd`（`/scp`）、`/bcd`（`/bcp`）、`/iod` 的命令行值和项目预设值都只允许
指向当前分区内部，否则报 `CLI_CACHE_PATH`。旧 `--out` 下遗留的会话、中间资产、
四流和 `.NewModCompiler` 不读取、不迁移、不自动删除；新布局允许首次冷构建。
需要清理缓存时，应先结束该 MOD 的构建，再清理其分区；更换 `--out` 不产生冷缓存。

BIG 直接读取本轮确认的缓存流，`published` 仅写最终 `.big/.skudef`；日志、事件和
报告位于输出根或合法诊断位置。loose（包括 development 派生格式）在 SDK 分区内编译，
只把本轮明确产出的四流/CDATA 导出到 `published/<content>/<entry>/...`，导出文件
独立于编译缓存。最终报告的 `artifacts` 列出交付路径。BIG build/plan 仍必须每次在
命令行显式提供 `--mod-version`。

同一 MOD 的分区锁覆盖完整 build：编译、Core 释放资源，直到 BIG 打包或 loose
导出结束。第二个并发 build 立即以 `CACHE_BUSY` 拒绝，不等待；不同 MOD 的分区
互不锁住。原会话文件锁与分区锁分别持有，不在 Core 释放资源时提前放开打包保护。

裸 FLA 的合成目录只在该 FLA 流中优先作为 DATA 根，保持 `DATA:<movie>.xml`
逻辑源名；普通 MOD 的 DATA 搜索顺序不变。Additional 继续直接读取当前 SDK/MOD
输入，不在源码目录生成副本；不新增 BIG 包缓存或整代发布暂存。

本次按用户要求仅进行轻量验证，具体结果见 [续接入口](../RESUME.md)。未执行大型
编译测试、完整 MOD 冷热构建、Windows 写入事件监测或真实 FLA/原版产物全字节对照；
既往缓存及 UI 验收不能作为本次路径迁移的完整生产验收。
