# W3X C++/CLI 边界

这里放经合同确认的 BAB 输入适配、native 调用和 AssetBuffer 导出，命名空间拟 `Recovered.W3x`。原版方法签名、所有权与异常边界取证后再建立可编译处理入口；当前没有恢复版支持声明。

优先复用上级 XmlNodeAdapter 与已核实的共享桥接能力。桥不负责隐藏原 DLL 转发；最后必须能移除原 W3X DLL 进行重放。
