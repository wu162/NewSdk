# SWF 直接编译 · 续接入口

本项目独立于 recovery 旧交接。分支 `feature/swf-direct-compiler`，起点 `87cf54e254dfb44a129da087316fd04806ed2b10`（recovery/compiler-source）。

用户本轮收尾条件：框架搭好，提供的 CoronaUIMain.swf 能被原工具成功编译后即可先结束；完整 MOD 编译器内存接入属于后续目标。

当前检查点：已解开附件并定位原工具源码 `lanyizi/swf2apt@60b15a67a4cbd931f181af8c6c758bd7a8441ebc`（上游 jonwil/swf2apt）。已准备固定源码差分宿主与仅支持样例能力的无文件内存后端，**Windows 构建/运行尚待确认**，未集成 SDK、未做优化器等价验证。

先查 `.github/workflows/ui-memory.yml` 对本分支的最新运行；不得用旧提交的成功冒充当前代码通过。工作流只验证 ui，不重测 recovery 既有类型。最终以成功 runId/sourceCommit、ui-memory-evidence 内 JSON 为准。

必要路径：`ui/RESEARCH.md`（研究结论，待本轮补齐）、`ui/tools/prepare_memory_backend.py`（生成可审查的内存后端）、`ui/MemoryAssets.cs`（能力边界）、`ui/tests`（原源码工具与内存输出差分）、`ui/fixtures/CoronaUIMain.swf`（原附件）。

后续方向：先补齐原脚本 `--ra3 --optimize` 与上传工具的基线，再把 SWF 展开成既有 AptAptData/AptConstData/AptDatData 声明并由专用插件返回内存 AssetBuffer。不是把 swf 原始字节直接打包为 APT，也不是转换后删除临时文件。
