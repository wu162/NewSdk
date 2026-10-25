# W3X component bridge

`AnimationCompiler.cpp` 将 BAB 已完成预处理的XML转换为typed动画输入，调用独立原生编译器并复制三缓冲区到AssetBuffer。复用XmlNodeAdapter和标量解析；资源引用使用BAB已解析的反斜杠十进制导入索引，通道分派读取预处理TypeId，不能以节点名推测。

该桥用于组件级原DLL差分，不注册或替换SDK正式插件，也不实现原指针ABI的公开FinalizeTracker。未完成的完整Mesh XML/envelope保持未启用。验收锚点见 `recovery/w3x/RESUME.md`。
