# XmlCompiler分支续接

2026-10-25（北京时间）。目标recovery/xmlcompiler-priority。先读AGENTS.md、EFFICIENCY.md，再读[XMLCOMPILER_PRIORITY.md](XMLCOMPILER_PRIORITY.md)。

组件已验证287/301入口，本轮新增104（40组）；完整61组/Od、/O2、平台0/1/2/99及287入口联合DLL运行40142560815成功，源码0f0ab2fce098fef2d78d6a165f3f5ce86c591077。合计1716864 fixtures、9852216数组比较（含适用CompileXml对照）、34264拒绝；联合两模式真实退出0。

本轮SDK真实编译0次。组件专用catalog现205类，全部SDK pending；历史82类SDK清单/主Plugin/SDK工作流/原SDK未改。旧SDK40098695436只对应92180f9，不能当新287入口DLL的SDK验收。精确证明evidence/xml-component-acceptance-20261025-287.json。

下一轮完整组：UIConfigList70分派（XML_UI_CONFIG_NEXT.md）或GameMap80根及WorldDict/MapAnalysis（XML_GAME_MAP_NEXT.md），两者都没有半根实现/登记。另11个Joypad根及TestGameObject属原排除项，合计剩14，不能称301完成。非完整Tokenizer/自定义编译器替代；若仍禁SDK继续组件层。

保留所有失败/修复/并发父提交与Actions证据。每10–15分钟保存检查点，GitHub插件正常推送，禁止强推；不重做已验证组，不删完整原DLL差分/独立ABI/拒绝边界。详见主交接的ANSI、ALL控制流、Time舍入、文件blob与原真实no-op边界。

## 当前进行中检查点

GameMap与UIConfigList完整根/依赖、完整差分测试、289入口联合路由已编写，本地C++语法及生成器重现/静态登记审计通过，Windows矩阵待最终验收。正式验收仍为上面的287入口，不能把289路由登记视为验收通过。见XML_GAME_MAP.md、XML_UI_CONFIG.md。本轮SDK真实编译0次，原排除12个独立根不变。
