# 编译器恢复：当前续接入口

更新：2026-10-24（北京时间）。仓库wu162/NewSdk，分支recovery/compiler-source。先读AGENTS.md、EFFICIENCY.md和本文件。旧STATUS.md只按需查历史；不要全量加载旧交接和大型反编译。

## 已验收：93/322已知Marshal模块

本阶段由63增加至93，新增30种，完成五个连贯组：11种Upgrade基类/派生、其余11种命名Upgrade派生及完整嵌套依赖、3种Production Update/exit、4种剩余Create、完整SlavedUpdate。**229种仍明确拒绝**；58 Token-only沿原未知Marshal回退路径。根范围88属性/38类子节点，六容器不代表全部模块已恢复。

有效源码 **17e4a7093436532ab843022defa278bcf744ce25**：
- GameObject run **40047664340** success；基础 run **40047664346** success。
- 远端evidence/gameobject-root/validation.json、sourceCommit/runId、两模式日志及terminal状态均核对。
- 每/Od、/O2：**181796 fixtures、545388三缓冲区逐字节比较、9544拒绝、344不变性**；平台0/1/2/99；两模式合计**1090776**次缓冲区比较。
- SDK本阶段**0次**，原路由保留。artifact recovered-gameobject-component-x86仍是组件；完整GameObject、两份完整替代DLL及正式GameObject插件尚未完成。

中间成功锚点：Upgrade基础d39c891/run40034294614；其余Upgrade aed221a/run40035310226；Production add4887/run40036066958；Create 38c17e1/run40036586975；最终Slaved即上方17e4a70。全部既有Body、ClientUpdate、声音、生命周期、Die、AI及根结构回归包含在最终矩阵内。

## 文档与关键契约

- GAMEOBJECT_UPGRADE_NEXT.md：已完成11种Upgrade基础组；Power独立Token父层，AnimState scalar枚举，Frames/Trigger unsigned；弱Upgrade ID列表不同于强引用。
- GAMEOBJECT_UPGRADE_REMAINING.md：其余11种派生，Nugget/ReplaceTexture/AvailableSubStructure/OffsetInfo，Cost可空filter与Tech inline filter，原版拼错属性与plain float Percentages。
- GAMEOBJECT_PRODUCTION.md：3种Production组；SizedList<ProductionModifier,2>原helper没有两项限制；Default出口可空Vector3、Queue出口inline Vector3；GiveUpgradeToProduced含强引用translations。
- GAMEOBJECT_CREATE.md：4种Create；Grant/Inherit/Experience使用Create父链；ActivateSpecialPowerOnCreate实际使用Update父链及强SpecialPower列表。已有EvaAnnounceClientCreate未重复计数。
- GAMEOBJECT_SLAVED.md：全部27属性+可空GuardPosOffset，39/34 scalar枚举；Range/帧数主要signed/base0，RepairWhenHealthBelowPercentage是普通整数，FadeTime是unsigned；4bool@108..111。scalar模板Token指针算术按int/uint单位4、bool单位1；GuardPosOffset直接bookmark可空Vector3数据，不生成递归子表。
- 各Generate*Fields.py核对固定原blob **6e40a9ed6c9f961d294d49768d911e6a6f354619**、原Marshal次序/fallback、PE枚举、完整Token和sentinel，全部接入TestGameObject.ps1。
- 更早GAMEOBJECT_SOUND_SELECTORS.md、GAMEOBJECT_LIFECYCLE.md、GAMEOBJECT_DIE.md等继续有效；STATUS.md保留历史。

## 下一完整模块：TechUpgradeReceiver（尚未实现）

先读 **GAMEOBJECT_TECH_RECEIVER_NEXT.md**。TypeId393460526、native180字节仍拒绝。44个原方法的精确签名/行号/SHA256已保存在evidence/gameobject-modules/tech-receiver-source/manifest.json，首次导出run40047754601 success；独立ReceiverDistanceType的PE名称表另存enum-source.json，后续导出步骤会核对保存。

与已实现TechUpgrade发送端相似，但ReceiverDistanceType是不同类型；RadarDrawModule@56是弱ModuleData ID；RadarRotateSpeed/Offset是Angle；inline ObjectFilter移至68（112字节），不能套用发送端56偏移。完整7自身属性、公共Upgrade基类及所有子依赖、Token/Endian应整组实现，再统一完整矩阵，不能仅追加3个雷达字段或据相同枚举项数假定类型相同。

## 中断记录与持续规则

本阶段因自动审批额度上限中断，Create源码38c17e1此前已推送并验收；恢复时核对实际新头、重建树并成功保存Slaved源检查点。未执行旧的失败ref更新，未强推。后续一次ref因并发Actions写证据发生API错误，也先读新头再重建树保留证据。

默认单代理；按约45分钟有效实现阶段推进，中断前后续接累计，不用空等/重复运行凑时长。完整能力组实现后统一/Od、/O2、0/1/2/99三缓冲区、独立布局/Token/sentinel、拒绝矩阵。每约10–15分钟普通推送有意义检查点，注明验证状态；不强推，不新建不必要分支。

SDK跨阶段累计降频，组件通过不冒充正式集成；适用XSD、同输入原/恢复、真实调用、四产物及适用无原DLL重放门槛不降低。Weapon/Armor及SDK输入/路由未改变，本阶段不重跑其已验收SDK。

Git命令行本环境无凭据，使用GitHub插件；API异常先查实际头和lease，不盲重试旧树。大原反编译按ExportLifecycleMethods.py导出小文件；共用sound-selector-source.yml。额度优化只减少重复读取/无依据重复运行，没有可比账号计量，不声称具体节省比例。
