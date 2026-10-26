# SWF 编译插件

当前目标：将已采用的SWF转换源码实现为**类似XmlCompiler的新编译器插件**，由新编译器宿主加载和调用。宿主形态不受限制；**不再规划独立EXE、CLI或SWF专用测试EXE**。

- 当前交接：[RESUME.md](RESUME.md)
- 已导入源码：[vendor/swf2apt](vendor/swf2apt)
- 来源与一致性说明：[VENDOR.md](VENDOR.md)
- 原工具差分证据：[evidence/source-consistency.json](evidence/source-consistency.json)

后续先核对XmlCompiler的实际插件契约与宿主技术栈，再建立同级插件。内部整合SwfLib/AptConverter、flasm、triangle、jpeg、zlib，使用内存输入输出，并接入宿主的类型注册、多资产收集、依赖/缓存与AssetBuffer路径。

目前原工具源码已导入并通过样例一致性验证；**新的SWF插件尚未实现或集成验证**。`single-exe/` 为已停止采用的历史实验，Native AOT/单文件/x64不再约束正式方案。已有原工具及历史测试材料仅作基线参考。
