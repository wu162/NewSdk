# Joypad 独立资产编译

恢复原先排除的 11 个 Joypad 全局资产入口：`JoypadCommandBarTemplate`、
`JoypadCommandBarButtonTemplate`，以及 `UIJoypadCommandBar` 的 ButtonBuild、
ControlGroups、CoopCommunicator、HomogenousGroup、MixedGroup、PlayerPowers、
SingleUnit、Stances、TopMenu。既有 UIConfigList 内嵌 Joypad 组件继续使用原布局。

`Xmljoypad.cpp` 保留原根大小、引用提交顺序、列表分配顺序及全部 34 个按钮 Data
多态布局。包括 AutoControlGroup 的完整内联 ObjectFilter、两类 targeted power
的 nullable RGB、额外 power/radius 引用列表、多个目标的 typed id 哈希和默认值。
7 个简单设置根的引用位于 offset 4，CoopCommunicator 使用 `AICoopTemplate`。
原 TypeId、TypeHash 和引用/重定位表合同不变。

Data 包装节点仅有一个子节点时编译；未知 TypeId 分配 4 字节并写 0。未提供的
普通可选属性保留零；GroupShortcut 与 TargetCount 缺省 1，MinTargetDistance
缺省 0，target outline 缺省 HOVER_ATTACK。未知枚举按原 DLL 返回值处理，比较保留
C 字符串语义。桥接器按原 Node 契约读取无命名空间属性、剥去一个属性值前导反斜杠，
单子节点选择同命名空间的首项，命名列表使用 ea 命名空间，列表值保留完整子树文本。
非 Win32（包括未知平台 99）执行完整 scalar / ObjectFilter endian
转换，引用和 pointer endian 交给 Tracker。

原始依据位于 `recovery/evidence/xml-source`：原 Module blob
`2d2f471c8008b7c2e1f78e39752d0726da395cea`，Marshal 方法 48411–49058、列表和
Data 多态方法 74635–75822；7 个简单根另核对原 Plugin.cs 的 HandleType 内联实现。
生成器 `GenerateXmlJoypadLayouts.py` 从固定原方法提取 34 个 TypeId/分配大小、从原
struct 提取 11 个根大小，并读取 6 份原枚举映射；运行不需要原 DLL。

源码 `1f0cea03083f2219e43fa35f3dd44df73158310e` 的
[Windows 41270490335](https://github.com/wu162/NewSdk/actions/runs/41270490335) 整体 success。
组件 Od/O2 各 6,000 个样本、36,000 次三缓冲对照，覆盖平台 0/1/2/99；
1,480 个生成输入外，另有命名空间、首子节点、反斜杠及 NUL 边界。全部与原 DLL
原字节一致，direct 与 ProcessInstance 两路均检查；完整布局及拒绝结果见原收据。

生产 Debug/Release 均重新构建，通过全部 310 条内建路由元数据、重新初始化和
拒绝检查。自制小 MOD 有 43 个资产，包含 11 个 Joypad 根、全部 29 个 XSD Data
分支及 4 个支持资产；不改 SDK Schema，使用 jobs1、禁用两类缓存编译 base loose。
两配置的 manifest/bin/imp/relo 均与独立原 BAB 进程逐字节一致。运行归档已单次
下载复算 SHA256，核对 D/R 三文件、验证入口的生产 link 输入和八对原始四流。
[摘要及原始证据](../../../evidence/joypad/41270490335-1/summary.json)。

历史 82 类及 207 类目录保持原样；新增 Joypad 单独登记为 11 类。当前 XML 路由
为 300 类（278 全局、22 非全局），只排除 TestGameObject。旧 289 类 SDK 验收不
冒充新增 300 类的全局全量 SDK 验收；本轮仅增加 Joypad 组件和上述小 MOD 证明。
新类型沿用 owner 编译路径，没有新增并行/缓存矩阵、完整大型 MOD、性能或游戏内
UI 验收。最初两次脚本预检失败及修复来源保留在摘要中。
