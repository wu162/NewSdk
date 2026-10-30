# PCSampleRate 并行验收证据

生产、二进制和测试来源 `8faba4e9f7efe470b95fdc7f2be43ed069e3a670`；[Windows运行41028084011](https://github.com/wu162/NewSdk/actions/runs/41028084011)整体success。后续归档提交不改变生产或测试输入。

`summary.json` 保存新增worker、原版音频、缓存/jobs、发布包和完整MOD范围；`api-*.json` 保存GitHub终态及artifact元数据。原ZIP为artifact `12966923428`，250607352字节，已下载复算SHA256与GitHub digest相同：`bd9981b61f677b122e6956f2d3da8a7e8eea99476ebf6c9ee9888ad01aa9cc02`。

`receipts.zip` 按原始字节保存选定的18份JSON收据及完整MOD测量说明，解压后为 `receipts/evidence/...` 和 `receipts/artifacts/...`。`receipt-index.json` 列出各收据SHA256、源归档身份、bundle哈希和两配置五文件包的实际重算身份；两份内层ZIP各五个文件均与生产bin及发布收据相同。全量输出/日志仍在上述原始artifact中，本目录不重复保存250MB归档。

完整Upheaval的1289个AudioFile均无显式PCSampleRate，因此其22样本只证明整体串并行回归，不能量化新增资格的性能收益。新增能力由每配置32个WAV/AIFF worker用例（均峰值4）和64个生产缓存/jobs用例证明。
