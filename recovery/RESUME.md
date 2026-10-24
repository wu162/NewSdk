# 当前验证中：完整SlavedUpdate

2026-10-24中断续接：已完成SlavedUpdate全部27根属性、可空GuardPosOffset、39项Damage/34项Death scalar枚举、完整Update父链与29 Token记录。源生成run40047463132 success。完整/Od、/O2、0/1/2/99三缓冲区/独立布局/拒绝矩阵正在运行；93支持/229拒绝为候选数，验收前有效92。SDK0次。

# 编译器恢复：当前续接入口

更新：2026-10-24（北京时间）。仓库wu162/NewSdk，分支recovery/compiler-source。先读AGENTS.md、EFFICIENCY.md和本文件；旧STATUS.md按需查历史，不全量加载大型反编译。

## 已验收：92/322已知Marshal模块

本阶段从63增加至92，新增29种：完整11种Upgrade基类/派生组、其余11种命名Upgrade派生及完整嵌套依赖、3种Production Update/exit、4种剩余Create模块。230种仍明确拒绝；58 Token-only仍沿原未知Marshal回退路径。根范围88属性/38类子节点，六容器不代表全部模块已恢复。

有效源码 **38c17e13b7a28d15d61c42ecbfd2539964a72226**：
- GameObject run **40036586975** success；基础 run **40036586994** success。
- 远端evidence/gameobject-root/validation.json已核对sourceCommit/runId及双模式结果。
- 每/Od、/O2：**178828 fixtures、536484三缓冲区逐字节比较、9516拒绝、336不变性**；平台0/1/2/99；两模式合计1072968次缓冲区比较。
- SDK本阶段**0次**，原路由保留。artifact recovered-gameobject-component-x86仍是组件；完整GameObject、两份完整替代DLL与正式GameObject插件尚未完成。

四组中间成功锚点：Upgrade基础d39c891/run40034294614；其余Upgrade aed221a/run40035310226；Production add4887/run40036066958；最终Create即上方38c17e1。全部既有Body、ClientUpdate、声音、生命周期、Die、AI及根结构回归均包含最终矩阵。

## 类型文档和源码证据

- GAMEOBJECT_UPGRADE_NEXT.md：已完成的11种Upgrade基础组；Power保留独立Token父层；AnimState是scalar枚举，Frames/Trigger均unsigned；弱Upgrade ID列表不能当强引用。
- GAMEOBJECT_UPGRADE_REMAINING.md：其余11种派生，Nugget/ReplaceTexture/AvailableSubStructure/OffsetInfo，Cost可空filter与Tech inline filter，原版拼错属性与plain float Percentages。
- GAMEOBJECT_PRODUCTION.md：3种Production组，SizedList<ProductionModifier,2>原helper没有两项限制；Default出口可空Vector3，Queue出口inline Vector3；GiveUpgradeToProduced含强引用translation列表。
- Create源证据evidence/gameobject-modules/create-source/manifest.json共66原方法（含已支持EvaAnnounceClientCreate的核对，不重复计数）。Grant/Inherit/Experience使用Create Token父链；ActivateSpecialPowerOnCreate实际使用Update父链，SpecialPowerTemplate是强引用列表。详细文档本阶段继续补齐。
- 各Generate*Fields.py均从固定原blob 6e40a9ed6c9f961d294d49768d911e6a6f354619核对字段查找次序/fallback/PE枚举/完整递归Token和sentinel，并接入TestGameObject.ps1。
- 更早类型文档GAMEOBJECT_SOUND_SELECTORS.md、GAMEOBJECT_LIFECYCLE.md、GAMEOBJECT_DIE.md等继续有效；STATUS保存历史。

## 当前继续项：完整SlavedUpdate组

SlavedUpdateModuleData（3370197599，112字节）仍拒绝；先通过ExportLifecycleMethods.py --slaved-family导出实际Marshal/Endian/Token及全部依赖，再整组实现。源导出检查点不得计为运行支持。

本阶段曾因自动审批额度上限暂停：38c17e1 Create实现已成功推送和验收，后续Slaved导出更新ref未执行；恢复后必须基于实际新头构建树，保留并发Actions证据，不能直接推旧树。

## 持续规则

默认单代理；约45分钟有效实现阶段，本阶段因额度中断续接，不用空等凑时长。完整能力组实现后统一/Od、/O2、0/1/2/99三缓冲区、独立布局/Token/sentinel与拒绝矩阵。每约10–15分钟保存有意义检查点，注明验证状态；不强推，不新建不必要分支。

SDK跨阶段累计降频，组件通过不冒充正式集成；适用XSD、同输入原/恢复、真实调用、四产物与无原DLL重放门槛不降低。Weapon/Armor及SDK输入/路由未改变，本阶段不重跑其已验收SDK。

Git命令行本环境无凭据，使用GitHub插件；API异常先查实际头与lease，不盲重试旧树。大原反编译按ExportLifecycleMethods.py导出小文件读取；共用sound-selector-source.yml工作流。额度优化只减少重复读取/重复运行，没有可比账号计量，不声称具体节省比例。
