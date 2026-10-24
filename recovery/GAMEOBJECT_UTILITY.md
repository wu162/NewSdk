# 完整辅助模块组（待Windows验收）

固定原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`。GenerateUtilityFields.py检查完整Marshal/Endian/Token和三个多态dispatcher helper；没有未覆盖子节点或可空依赖。

| 类型 | TypeId | native | 完整字段 | Token父表 |
|---|---:|---:|---|---|
| StorePurchasedUpgradeBehaviorModuleData | 783731100 | 12 | MaxPlayerUpgrades uint@8，默认9999；strtoul base0 | Behavior2769121946 |
| AIGateUpdateModuleData | 880602396 | 16 | TriggerWidthX float@8、TriggerWidthY float@12，默认0 | Update4043318533 |
| CombinedInfoModuleData | 2018429368 | 16 | DockingBone String8@8，无默认；节点属性缺省不分配，空属性分配4字节 | Create816016269 |
| TimedOpacityUpdateModuleData | 4216826759 | 16 | Opacity float@8默认0.5，Duration Time@12默认5s | Update4043318533 |

四类Marshal末尾均调用ModuleData；Endian直接交换自身字段、TypeId、id；Token必须保留以上精确父层。String长度包含字符数、不含终止符，分配包含终止符并四字节对齐。均拒绝未知属性、所有子节点、结构文本和外来命名空间；通用TypeId/id契约复用。

GameObjectUtilityCases.cs覆盖全部属性、数值边界/时间后缀/非ASCII字符串、独立默认值/布局/Token key及type/分配及padding、容器与列表0/1/2/27/28/29/65边界及非法输入。累计矩阵保留/Od/O2、平台0/1/2/99、全部旧回归、三缓冲区比较；组件验收与正式SDK集成分开。
