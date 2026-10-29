# 热缓存性能优化：续接入口

当前工作分支 `recovery/legacy-sdk-cache`；用户要求的六项性能优化已直接在此分支完成并普通推送，未合并至其他分支。生产与测试来源均为 `84261243a2f2cc98b77399a939be0f4115a4a98d`；后续仅归档证据和整理文档。

完整 Windows 验收 [40845943601](https://github.com/wu162/NewSdk/actions/runs/40845943601) success：Debug/Release、缓存冷热/失效与失败取消、完整 mymod 原版差分、独立五文件 ZIP 均通过。已下载原归档并复算 SHA256，确认原始收据；两配置热态为零 XML 解析、零实例编译、零完整文件 SHA256 读取，`publicationUnchanged=true`，且热态会话/CDATAs 不重写。[验收摘要](evidence/refactor/hot-cache/40845943601-1/summary.json)。首轮配置回调 XmlIgnore、测试时钟粒度、直接后端/重试/缓存提交时序失败均已修复，原失败运行保留。

同机同输入配对基准 [40848391545](https://github.com/wu162/NewSdk/actions/runs/40848391545) success，复用上述精确 Release，108 对/216 个样本约 123.1 秒，所有四流/CDATAs 原始字节相同。默认 Deflate 双缓存热态各边 5 次中位数：mymod 老/新 450.6/446.1 ms，Armor 子集 426.3/417.5 ms，Texture×16 429.5/454.5 ms；前两组略快，纹理仍慢约 5.8%。其余模式、边界及原始结果见 [SDK 对照](docs/SDK_BENCHMARK.md)。不同 VM 的历史结果不用于计算本轮因果收益。

实现入口：`BuiltinCompilers`、`CoreStreamCompiler`、`SchemaSet`、`SessionCache`、`OutputManager`、`BinaryAsset`、`LinkedOutputCache`、`LegacyCacheStore`、`FileHashCache`、`ArtifactPublisher`。实现合同见[缓存说明](docs/LEGACY_INCREMENTAL_CACHE.md)。本轮六项及性能对照均完成；若继续优化，先定位纹理热态剩余开销，不能以当前少量样本推断全部 MOD 性能。禁止强推；合入其他分支仍须 PR 和用户明确批准。
