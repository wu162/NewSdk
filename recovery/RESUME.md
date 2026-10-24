# XmlCompiler 分支续接

更新：2026-10-24（北京时间）。本入口适用于 `recovery/xmlcompiler-priority`；先读AGENTS.md、EFFICIENCY.md，再读 [XMLCOMPILER_PRIORITY.md](XMLCOMPILER_PRIORITY.md)。

已验收82/301个原Plugin独立入口，本轮新增30个；80类全局资产，GameStatsModeData/MicrophoneSettings仅嵌套schema。十四组完整/Od、/O2、平台0/1/2/99矩阵与联合DLL运行40099095351成功，组件源码e7580311f36811265e6ed5af5a184ef623f31284。

SDK源码92180f9bbd830b9dbd49a696c259e54ffe2d7721，运行40098695436成功：159资产，Reduced159调用/477比较，FullMymod162调用/486比较，每profile原版和隔离重放均通过、四产物相同。之后仅UI测试断言改动；所有SDK输入/编译器源码树与SDK Oracle的Git SHA相同，精确复用依据见 evidence/xml-priority-acceptance.json。

下一轮完整能力组：五个战术UI热键根，已只读取证，尚未实现，不计入82。先读 [XML_TACTICAL_HOTKEYS_NEXT.md](XML_TACTICAL_HOTKEYS_NEXT.md)，不要重复调查已核共享helper；未知内容仍须原证据/必要探针核实。

仅GitHub插件访问仓库，禁止强推；保留并发提交与Actions证据，每10–15分钟保存有意义检查点。按用户授权的一小时工作窗口推进完整组，集中差分/SDK验收，不删样本、不以组件成功冒充SDK成功。

Joypad命名根继续排除；本轮不恢复GameObject或完整Tokenizer，不替换原SDK配置/DLL。GameObject的独立进度在recovery/compiler-source分支，本入口不替代该分支交接。
