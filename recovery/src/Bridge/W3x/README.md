# W3X component bridges

`AnimationCompiler.cpp` 与 `MeshCompiler.cpp` 将 BAB 已完成预处理的XML转换为typed输入，调用原生编译器并复制三缓冲区到AssetBuffer。两者复用XmlNodeAdapter和标量解析；资源引用使用BAB已解析的反斜杠十进制导入索引，多态分派读取预处理TypeId，不以节点名推测。

桥用于组件级原DLL差分，不注册或替换SDK正式插件，也不实现原指针对象ABI的公开FinalizeTracker。当前实际验收与下一步骤见 `recovery/w3x/RESUME.md`。
