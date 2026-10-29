# 新 SDK 大中小 MOD 耗时：Windows 测量完成

工作分支 `recovery/legacy-sdk-cache`，禁止强推，未合入其他分支。生产逻辑未改，本轮新增测量脚本/工作流及分析归档。

来源 `629951986c1776a5fda5ad4a68bbc7b7da3a688d` / [Windows 40890949858](https://github.com/wu162/NewSdk/actions/runs/40890949858) success。Release 一次构建，默认缓存、detailed、自动全部内容、BIG/SKUDEF/Additional；三组各冷一次/热两次，9 进程，实际测量/校验 87.63 秒。冷/热最终输出一致。

小：完整 mymod，冷 2.459s/热中位 0.587s；中：96 个真实 Upheaval Texture 子集（不是独立完整中型 MOD），1.166s/0.653s；大：完整历史 Upheaval（2,723 文件、287MiB、五流），75.286s/2.864s。大型冷态编译 3,354 资产；动画 27.11s、音频 17.71s、XML 校验 13.91s 合占约78%。热态零 XML 解析/资产编译，缓存加载 0.773s、Core 复用/引用处理 1.100s、打包 0.333s。小型第一笔冷测有启动/等待成本，三档不是纯规模伸缩实验。

所有样本零错误；小型 82/大型 1,859 条默认外部引用等警告保留，不宣称游戏内验收，也未本轮做完整大型原版差分。[完整分析与图表](docs/MOD_SCALE_TIMINGS.md)、[来源/样本/摘要](evidence/refactor/mod-scale/40890949858-1/summary.json)、[CSV](evidence/refactor/mod-scale/40890949858-1/samples.csv)。原 artifact ZIP 已下载复算 SHA256 匹配，包含运行时二进制及原始报告/事件。

性能打点默认 off；`--performance summary|detailed` 写入 `build-report.json` 的可选 performance.entries。[使用及计时边界](docs/PERFORMANCE.md)。前轮打点生产/测试 `23994960` / [40886567000](https://github.com/wu162/NewSdk/actions/runs/40886567000) 的 8 秒小输入原版字节验证仍保留。
