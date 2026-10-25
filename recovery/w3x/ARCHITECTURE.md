# 与现有 recovery 共用的实现框架

## 组织原则

新项目单独推进，代码结构共用；未来合并时保留双方证据和源码，不用目录搬家或覆盖旧实现解决冲突。W3X 特有代码使用 `src/Native/W3x` 等子目录，公共代码继续只有一份。新 native 命名空间优先 `Recovered::W3x`，managed/bridge 优先 `Recovered.W3x`；正式插件使用独立全名（拟 `Recovered.Compiler.W3x.Plugin`），避免现有 Armor 的 `Recovered.Compiler.Plugin` 冲突。最终程序集输出名和 ABI 在合同确认后确定，不提前冒充原版程序集身份。

## 复用决策

| 现有代码 | 初步安排 | 合入前需要证明 |
| --- | --- | --- |
| `MetadataInventory` | 现在直接复用，只读 PE/CLI，不修改公共工具 | 新流程通过；旧目标输出语义不变 |
| `src/Native/Relocation.*` | 首选共用，禁止先复制一套 W3xTracker | 分配顺序、对齐、指针/引用表排序、absolute offset、endian 与 W3X 原版一致 |
| `src/Native/ScalarParsing.*`、`DataInitializers.*` | 候选共用 | W3X 实际调用路径、词法/溢出/浮点位模式一致 |
| `src/Native/ResourceMarshaling.*`、枚举/位集辅助 | 按原版使用情况共用 | 类型 hash、导入项、空值和未知枚举处理一致 |
| `src/Bridge/XmlNodeAdapter.h` | 候选共用 | W3X 使用的命名空间、节点值、默认值和遍历顺序合同一致 |
| `src/Native/GameObjectDrawBeam*.h` 的 Shader/常量能力 | 候选提取为共享 Shader 模块 | W3X 同名布局、Marshal、多态默认分派和导入顺序一致；两边完整回归 |
| 已有 ProcessInstance/插件/SDK 测试脚本 | 借鉴构建、三缓冲区捕获、临时 SDK 和证据模式 | 不继承 Armor/Weapon 的 TypeId、版本号、Tokenized 标记和路由范围 |

若公共逻辑需要扩展，优先增加小而明确的共享接口，将 W3X 特有语义留在调用侧；确属同一合同再合并实现。先列调用者和受影响测试，再改公共文件。不能为了让 W3X 通过而改变旧类型语义，也不能只运行 W3X 回归。

## 待原版合同确认后的层次

1. **Managed metadata**：Initialize/ReInitialize、版本、类型和 processing hash 等插件元信息；具体公开签名、平台行为、未知类型行为来自原 DLL。
2. **Bridge**：BAB `InstanceDeclaration` 到已验证 XML/资源适配器；native 结果到 `AssetBuffer`。负责受控错误传播和对象生命周期，不把业务转换全部堆在桥内。
3. **Native W3X**：动画帧/通道转换与编码；网格 layout/declaration、顶点/索引/骨骼数据；原版表现出的重排、量化和平台差异。
4. **Shared runtime**：共同使用且经过 W3X 差分确认的解析、分配、重定位、引用、大小端能力。
5. **Plugin**：只路由验收范围；不支持的合法输入显式拒绝，不能默默交给原版或返回空 AssetBuffer。

这是待实现模块边界，不是已恢复 API。当前不生成伪造的可替换 DLL、不注册空插件，不改 `SDK/tools` 原 DLL 与正式配置。

## 合并回原工作线

保留本分支从指定提交开始的祖先关系，使用普通合并/推送，禁止强推。公共重构单独描述兼容性、调用者及双边回归证据；W3X 私有实现可独立合入。合并前比较原工作线新公共改动，重新验证受影响组件，不把基点的旧通过记录直接套用到重构后的代码。根续接入口是两个项目的导航；合并时保留两条工作线，不将 W3X 的状态覆盖成 XmlCompiler 的完成比例。
