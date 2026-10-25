# ID 元数据边界根因与验证策略

## 已完成的真实 SDK 验证

运行 [40178403660](https://github.com/wu162/NewSdk/actions/runs/40178403660)，源码 0afc6eb49d81b61b9840e21d0edd840d5f0f0de3：Upheaval 的 AptUI、全局数据、基础数据共5入口，原版与恢复版合计10次真实BAB调用全部成功；20份 bin/manifest/imp/relo 的 SHA256全部相同。SDK/tools未被修改，恢复版Core和编译插件在临时SDK中联合启用。调用来自BAB -> Core -> 插件，不用测试宿主直接调用冒充SDK。调用记录见对应 evidence/upheaval/40178403660-1，验收只绑定上述源码与范围。

## 根因不是 ID 字符串内容错误

1. 恢复时依据原Marshal/Tokenize提取“写入运行时结构的字段”，随后把这个字段集合直接当成“所有合法XML属性”白名单。两种集合并不相等。
2. 全局编译路由与XSD继承是不同关系。Xmlai曾用 XmlaiTypeId(name) 判断节点是否允许身份属性；它只登记独立根，遗漏继承BaseAssetType的嵌套StrategicState/BudgetState/BuildState。GameObject桥接层的集合/嵌套白名单也有同类缺口。
3. 原组件探针包含很多有用的默认值、非法值和低层边界样例，但部分正例省掉了嵌套类型继承而来的必填id。例如ObjectVisionRangeItem继承BaseAssetType，id是必填属性，旧AIUpdate/WeaponSet样例却只写Range和Filter。那些低层差分不能替代完整、经XSD验证的SDK输入覆盖。
4. Upheaval具有真实身份标记和继承/合并用标记，因此在多个独立读取器中逐次暴露同一个设计缺口。此前逐处修复后立刻启动完整SDK，未先汇总同类边界，是本轮重复运行的直接原因。

## 821份XSD和360份恢复源码的静态核对

可重现工具：recovery/scripts/AuditSchemaIdentityMetadata.py。清单：recovery/catalog/SchemaIdentityMetadata.json。清单记录1349个命名复杂类型、全部显式ID声明、非ID pipelineOnly属性及模式指纹；检查新类型和继承覆盖漂移，不冒充运行时正确性测试。

| ID类别 | 本轮核对结果 | 处理规则 |
| --- | --- | --- |
| 显式pipelineOnly id | 7种：ArmorTemplateSet、LocomotorSet、CrusherInfo、WeaponEffectNugget、GameDependencyType、AIBuildDelayRange、AISpecialPowerInfo | 由相应类型读取器接纳，按原版Marshal行为处理，不添加虚构运行时字段 |
| 嵌套BaseAssetType身份 | 6种类型、8个元素位置：AIStrategicState、AIBudgetState、AIBuildState、ObjectVisionRangeItem、GameStatsModeData、MicrophoneSettings | 依据XSD继承判断身份属性，不依据“是否是独立根”判断；后两种原实现已有处理 |
| 必须保留的普通ID | 12种显式声明，包括ObjectFilter、ModuleData、AIStateTactic、ParticleSysBone、ModelConditionState等 | 不做全局删除。例如ObjectFilter/模块id生成哈希，粒子骨骼与模型状态id进入字符串字段 |
| 其余显式ID声明 | BaseAssetType自身及2个测试Schema类型 | 根身份与排除的测试类型单列，不扩大已恢复范围 |

注意：pipelineOnly只说明不直接对应运行时结构成员，不表示编译器能无条件忽略这个属性。Texture.GenerateMipMaps等同样标记pipelineOnly，却影响资源生成。不得按属性名id或pipelineOnly标记全局删除节点属性。

## 汇总后发现的额外遗漏

静态审计发现LogicCommand/AISpecialPowerInfo.id仍会被Xmlprogression的Fields默认白名单拒绝；Upheaval的成功运行没有触发这个可选属性。此次统一补漏让该类型允许身份id，并把Fields布尔参数改名为allowIdentityId，避免“根节点”与“允许身份属性”继续混淆。

新正例显式包含合法Heuristic和Manager；比较有/无id的原DLL与恢复版三缓冲区，并检查输出不因该流程id变化。反例仍拒绝外来命名空间的f:id；另一正例验证ObjectFilter.id变化必须改变其已序列化哈希，防止错误的全局忽略方案。

该补漏仅启动一次受影响progression组件/Od、/O2四平台差分，没有再次触发完整SDK。其组件结果单独绑定新sourceCommit/runId；不能把0afc6eb的真实SDK结果伪称为新提交重跑结果。

## 后续执行门槛

- 先对XSD声明、继承及原Marshal字段分组，列出同一元数据类型的所有消费者，一次修复整组。
- 正例另设“真实合法输入”组，包含继承而来的必填身份属性；低层默认/拒绝样例仍保留，但不标为XSD合法SDK样例。
- 区分无命名空间id、外来f:id、编译分派TypeId、继承/流程元数据和真正序列化ID，分别验证。
- 先做静态清单和受影响组件差分。全部已定位元数据缺口汇总通过后，才集中运行一次必要SDK批次。
- 已通过且输入/依赖未变化的组保留原验收锚点。当前Upheaval三阶段SDK已经成功，不因文档、审计清单或未触发的可选属性补漏机械重跑。

## 最终收据

- 真实SDK：40178403660 / 0afc6eb49d81b61b9840e21d0edd840d5f0f0de3，10次进程成功、20份输出相同。
- 静态审计与额外补漏组件：[40180066600](https://github.com/wu162/NewSdk/actions/runs/40180066600) / 3d32896d2d2607f4819cfd81c044a240dd278e46，821份XSD清单检查通过，/Od与/O2各四平台全部通过；每模式每平台4165样例、12495原版缓冲区比较、12495直接入口比较、123拒绝测试、4189独立布局检查。
- 本地审计工具检查：规范化XML对结尾空白/换行稳定，Schema语义变化可检测，新增未分类ID会拒绝。首轮工具校验因文本指纹跨平台差异中断、组件未运行；改用规范化XML指纹后以上独立组件批次成功，历史失败保留。该工具只做分类/漂移门槛，不能替代原DLL或SDK结果。
- 新增可选AISpecialPowerInfo.id补漏只取得组件验收，不伪称新提交重跑完整SDK。Upheaval三阶段的已成功原始SDK锚点保持独立。
