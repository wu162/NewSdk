# W3X 正式插件位置

这里已有 `Recovered.Compiler.W3x.Plugin` 的托管元信息、生命周期和分派实现，避免现有 Armor `Recovered.Compiler.Plugin` 重名。动画/网格业务通过 IW3xAssetCompiler 接入，尚待实现；默认构造路径明确拒绝两种编译请求。不是可直接接管 SDK 的完整正式插件。

没有原版转发器，没有修改正式 SDK 配置。托管恢复范围见 `recovery/w3x/MANAGED.md`；元信息、真实组件差分、实际 SDK 接管和完整 DLL 替换分别验收，见 `recovery/w3x/VALIDATION.md`。
