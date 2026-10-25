# W3X 托管实现位置

这里已有 `W3xCatalog.cs` 的平台版本/类型信息实现，以及供 C++/CLI 处理器接入的 `IW3xAssetCompiler.cs`。Plugin 代码在 `src/Plugin/W3x/Plugin.cs`，命名空间 `Recovered.W3x` / `Recovered.Compiler.W3x`。原版合同、差分和限制见 `recovery/w3x/MANAGED.md`；不能复制 XmlCompiler/Tokenizer 的版本、hash 或 Tokenized 标志。
