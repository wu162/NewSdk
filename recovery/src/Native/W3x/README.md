# W3X native 实现位置

这里放原 DLL 合同确定后的 W3X 专有布局、动画与网格算法，命名空间拟 `Recovered::W3x`。当前尚无业务恢复实现；不创建返回空缓冲区的占位编译器。

分配/重定位等公共能力优先复用上级 `Relocation.*`、`ScalarParsing.*`、`ResourceMarshaling.*`；先核对 W3X 原版合同。若需重构，运行旧调用者和 W3X 的受影响回归，禁止复制公共实现形成长期分叉。整体设计见 `recovery/w3x/ARCHITECTURE.md`。
