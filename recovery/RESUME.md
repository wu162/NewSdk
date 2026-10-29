# 热缓存与 StableSort 优化：续接入口

工作分支仍为 `recovery/legacy-sdk-cache`，禁止强推，未合入其他分支。

用户要求的前六项已实现并通过完整 Windows 验收：源码/测试 `22133880ae3cabe8be2f9f6c0c6202e6cd7f57e0`，[运行 40863166008](https://github.com/wu162/NewSdk/actions/runs/40863166008) success。Debug/Release、Core/后端/应用、真实 CLI/冷进程/取消、缓存冷热/失效/缺损修复、三种 Texture、Audio/W3X、BIG/协议、完整 mymod 原版差分及五文件发行包均通过。文件身份优先；Schema 收据跨进程复用，身份未变不重新计算内容 SHA256。实现细节见[缓存说明](docs/LEGACY_INCREMENTAL_CACHE.md)。

本提交继续处理用户新增第七项 `AssetDeclarationDocument.StableSort`：为依赖集合建立一次哈希索引，前缀中没有资产依赖当前项且当前比较键严格最大时直接追加；文化比较相等、逆向依赖和基础流间隙沿用原插入规则。基础 manifest 按 TypeId/InstanceId 建键索引，并一次压缩剩余列表，保留原顺序。

第七项代码与定向测试已准备，**尚待本提交 Windows 验收**，不能复用前六项的旧生产二进制作为通过证明。`CompilerStableSort` 对照 072beb3f 的冻结插入算法，覆盖随机 DAG/基础前缀/空及传递依赖/三种 culture/相等排序键，并检查 1024 个无依赖资产的比较次数；完整原 DLL 四流差分仍由既有生产验收承担。前期同大小/同 mtime Schema 用例的 ChangeTime 时钟刻度失败保留在 40860991872、40861431337、40862734658；22133880 已按既有文件身份测试方式分隔写入，并明确断言身份变化，未加回热路径全文件哈希。

上轮优化基线为 `84261243` / [40845943601](https://github.com/wu162/NewSdk/actions/runs/40845943601)，已有小样本同输入对照见[SDK 基准](docs/SDK_BENCHMARK.md)。本轮尚未声明端到端提速比例。
