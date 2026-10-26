# 路径集中审计（2026-10-26 交接）

本页为代码调用链审计；除已列 Windows 证据外，不把静态复现推导标成动态验证。本轮临时恢复 CLI 候选以取得完整 Windows 证据，未通过前不代表生产入口验收。首个候选的 App Debug/Release 路径矩阵已过；Core、真实 CLI、原版对照和协议仍以作业终态及原始收据为准。

## 已修复并取得定向证据

- 测试 evidence 深目录叠加中间资产名触发 MAX_PATH：改短测试工作区，保留中文/空格和所有断言；不宣称生产长路径支持。
- Core 实际使用的 ShPath.PathCanonicalize/PathRemoveFileSpec 默认 ANSI 将中文转为问号：显式绑定 W 接口；两配置真实 Core/App 和 Unicode 回归通过。
- 旧 Framework csc 误解混合斜杠源码参数：CLI helper 改 VS Roslyn、NET48 引用及 Resolve-Path。已实际运行 Debug 14 个生产 CLI case。

## 已确认的代码缺陷（本轮修复已写入，待 Windows 动态验收）

| 位置 | 可复现输入与影响 | 下一步 |
| --- | --- | --- |
| FileNameResolver.GetDataRoot | DataPaths=C:\SDK\Data 错匹配 C:\SDK\Data2\X.xml；调用方固定 root.Length+1 截断后可产生根相对 \X，输出拼接可能越出 staging | 按分隔符边界匹配；统一安全相对路径函数并拒绝 rooted/上跳输出 |
| AssetDeclarationDocument.CurrentState / DocumentProcessor.GetExpectedOutputManifest | 根为 C:\ 或以反斜杠结束时固定 +1 多跳一字符，Mods 变 ods、Sub 变 ub | 规范根与尾部分隔符，再由共同函数取相对路径 |
| FileNameXmlResolver.PathRelativePathTo | 独立的生产 PInvoke 仍默认 ANSI，中文 file XInclude 的 SourceUri/DependentFiles 可损坏 | 显式 Unicode W；实际 XInclude 文件/依赖回归 |
| FileNameResolver art alias | art:a 无子目录且使用非通配 ArtPaths 时 Substring(0,2) 越界 | 明确拒绝空/过短输入，保留原资产搜索规则 |
| SettingsLoader.ProcessPaths | DefaultDataPaths=";" 被接受却得到空搜索集合；Canonicalize 失败的 null 继续流入下游 | 拒绝空集合/null，带原始路径报明确配置错误 |
| Options.ResolveDiagnostic / 日志 sink | junction 指向 MOD/SDK，或外部日志 hardlink 到 Mod.xml，可绕过词法 Contains；FileMode.Create 会截断实际输入 | 写入前物理路径/重解析防护；日志原子新文件替换策略避免 hardlink 截断；Windows 链接回归 |
| Options 诊断文件组合 | events=C:\out\a、report=C:\out\a\r.json 通过解析，先建目录再当文件打开，启动期失败 | 拒绝诊断文件间祖先/后代冲突及与 staging 的冲突 |

## 本轮追加修复（后续集中验证检查点）

- 日志首次 hardlink 替换在 Windows 真实矩阵暴露 File.Replace 与活动 writer 共享冲突；改同目录 MoveFileExW 保持原 writer，避免关闭重开写到输入 inode。首个候选两配置 App 矩阵通过。
- 发布后持有不共享 DELETE 的元数据句柄并核对卷/文件身份，防第二 sink 静默替换活动日志；同路径冲突、不同路径 hardlink 独立输出及发布失败清理已有矩阵，待新候选 Windows 验证。
- 诊断路径明确拒绝文件/目录类型冲突；CLI sink 故障改用真实持锁文件，目录参数拒绝和释放锁后同路径重试独立断言。
- alias XInclude 由本地绝对路径构造 URI，不拼接 file://；真实 data/art/audio XInclude 增加中文、空格、#、%23 搜索根。
- BuildSession 晚期报告/终态写入/关闭失败会晚改 exit7，原先清理阶段已结束；补撤销产物和 staging 的测试准备，不能把 plan-only close 测试当作已编译产物清理证据。

## 已区分的未验边界

- Audio 文件读写使用 Core 预处理后的绝对路径和 .NET API，未找到当前文件路径 ANSI 转换；PathMusic 字段的 Encoding.Default 是格式内容兼容问题，需原版差分，不能直接换 UTF-8。
- W3X 窄字符串用于 shader/technique/引用元数据，未找到 Native/W3X 直接打开外部文件，不能把它当已确认磁盘路径缺陷。
- 原 Texture 混合 DLL 内部路径编码尚未验证，包装层自身用绝对路径和 .NET API；不能泛称纹理全 Unicode 已支持。
- 中文仓库根下 Native 生成 UTF-8 源码而 cl 未指定源编码，是未动态验证的开发构建边界；本轮 ASCII 仓库根未触发。
- App.Paths.Contains 对正常驱动器根、UNC、同前缀兄弟目录及规范斜杠没有发现新误判；旧相对 art/data/iod 当前明确拒绝，不是静默按 CWD 执行。

## 当前生产入口证据与失败

Debug 14 个实际生产进程 case 已过，含 OS 单进程 job/no-child、冷进程分组对照、内外 MOD、真实 CTRL_BREAK/exit6/retry。独立原 BAB 对照仍严格失败：bin/imp/relo 完全相同，manifest 源文件表旧版为 ANSI 损坏的 ?? MOD 绝对路径，新版为 DATA:mod.xml。不得改写/归一化字节掩盖差异；保留 Unicode case，另增内容相同的 ASCII 原版 companion 对照并核对两种新编译器逻辑路径语义。Release CLI/CLI 协议尚未完成。

## 验证耗时优化

已加入 CLI-only 严格 Git 输入闭包复用；生产/SDK/其他 fixture 变化会拒绝复用，旧结果保留原 sourceCommit。新增运行允许清单 bin 文件归档，供后续只改工具时核验复用，避免因缺二进制被迫重复链接；尚未实现自动下载/复用执行器。外部 helper 应先廉价编译再进行长 Native 构建，具体是否已接入以 workflow 为准。不可用删断言、少平台或放宽差分来优化。
