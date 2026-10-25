# Upheaval 联合 SDK 验证

从 recovery/compiler-source ddf7ff6dac82b8f3e5331899d493e3847f46efb3 建立独立验证分支。Upheaval 原样取自 main bacfc69fc3e71c8116d1f22b933b8b6173170ecf，目录树 5551fc26a6f96b62613443e0d0fa2958c14cbce0。

仅执行 SDK/defaultscript.cs case3（3个 AptUI XML）、case4（MapMetaData_Global.xml）和 case5（Mod.xml）。全新输出目录，沿用默认 BAB 参数与完整输入、资源及 XSD。不执行后续 LoD、合并、修复、复制或打包。恢复组件在一次性 SDK 副本中联合启用，仓库 SDK/tools 保持原样。

构建 XML、GameObject、Weapon、Armor、W3X 插件和 Core、Remote、ExpressionEval、GameDataVerifier、EALABuild。原版与恢复版分别执行同一5入口，核对退出码、错误日志和20份 bin/manifest/imp/relo 哈希。EALABuild 在临时 SDK 中替换保存，但本次为提取 BAB 调用的无界面入口，不宣称运行 GUI 调度或每个已注册插件均有实际调用。实际覆盖以 trace 和输入为准；没有调用的组件不能宣称获得新增 SDK 验收。尚未还原的其他插件保留原版。

状态：待 Actions 验证，不能视为通过。日志、配置、DLL 指纹、输入 SHA256、产物及 validation.json 保存至 recovery/evidence/upheaval/<runId>-<attempt> 并上传 artifact。没有重复运行既有组件全量矩阵。

## 2026-10-25 续接

运行 [40174918364](https://github.com/wu162/NewSdk/actions/runs/40174918364)，源码 ca604c23a68e76709ff3ace38175f248312f8c31：所有恢复组件构建成功；原版 SDK 的3个 AptUI、全局数据、Mod.xml 五次调用均 exit=0，20份产物齐全。恢复版在编译前的 SetArmorPlugin 注册阶段失败（清空原 Tokenizer 最后一个类型后仍要求其存在），没有执行恢复版 BAB，不能声明联合验证通过。原始失败证据保留。

修复提交 324c1e08da9c02c523680101348bb038bd991ac3：本轮专用脚本原子替换 GameObject/WeaponTemplate/ArmorTemplate 三条路由，同时检查重复类型和最终目标；不修改既有单插件脚本或编译器实现。新运行 [40175382847](https://github.com/wu162/NewSdk/actions/runs/40175382847) 正在验证。继续时先检查此运行结果与对应 validation.json；失败按日志定位，不将旧原版成功当作恢复版成功。

## 第二次运行与 AI 子节点修复（待验证）

运行40175382847、源码324c1e08da9c02c523680101348bb038bd991ac3：原版5入口成功；恢复版3个AptUI和全局数据成功，16份产物与原版SHA256一致。恢复版基础数据处理BasePersonality时拒绝嵌套StrategicState的id。错误堆栈为 BAB.DoBuildData -> Core.DocumentProcessor -> Core.AssetDeclarationDocument.CompileInstance -> Recovered.XmlPriority.Plugin -> Recovered.XmlaiCompiler，确认实际SDK调度。

XSD SkirmishAI/AssetTypeAIPersonalityDefinition.xsd 中 AIStrategicState、AIBudgetState、AIBuildState 继承 BaseAssetType（含id），AIBuildDelayRange显式声明pipelineOnly id。恢复代码仅允许独立根id，误拒绝上述嵌套合法属性。本次仅为这四种结构接纳id，不将其他未知属性全部放行；已有AIStateTactic的id二进制Hash字段保持原处理。新增对应原DLL差分样例，先运行受影响AI组/Od、/O2和四平台矩阵，通过后再执行联合SDK。修复状态待实际运行证实。

## AI 验证通过与 LocomotorSet 元数据修复

运行40175816790、源码d859b61cd7bb66d9beca7c6a8d0a83bb8bc7a103：AI组/Od与/O2、平台0/1/2/99全部成功（各平台1985样例、11907缓冲区比较、52拒绝测试）；Upheaval恢复版全部AIPersonalityDefinition已编译成功。随后在GameObject:BaseMagneticSatelliteEffect的LocomotorSet上拒绝id。

AssetTypeGameObject.xsd的LocomotorSet显式声明id为xas:pipelineOnly；桥接层仅为该节点忽略此属性，其余未知属性仍拒绝，并在错误中增加节点/属性名称。提交edec51f3ab6c8b829fb2cfdcf59162479666aae7加入原DLL差分样例和仅运行Armor/Locomotor集合读取的测试入口，不执行未受影响模块族。运行40176438018先跑受影响组/Od、/O2四平台，再运行真实SDK。AI成功证据沿用40175816790，未冒充为新源码重测。

## GameDependency 元数据修复与监控

运行40176438018、源码edec51f3ab6c8b829fb2cfdcf59162479666aae7：Armor/Locomotor定向组/Od、/O2四平台通过（每模式累计668样例、2004三缓冲区比较、24拒绝测试）。真实编译越过此前GameObject，后在SpecialPowerChronoSwapSelect/GameDependency拒绝id。

Includes/GameDependency.xsd显式将GameDependencyType.id标为pipelineOnly。提交9fa6d8dc9f62bc4453b88891e16d0e3cfcb3ca5f修正XML powers与Tokenizer GameObject的两处同类型读取，分别加入原DLL差分样例；保留其他属性拒绝。运行40177066058先执行受影响的GameDependency组与powers组矩阵，再执行联合真实SDK。已经通过且未变化的AI和Locomotor组沿用既有锚点，不重复全量运行。仍未声明基础数据完成。

用户补充：测试通常起步约5分钟。后续监控新运行先留约5分钟，再约3分钟检查一次；失败后检查终态、日志与新证据，继续修复。

## GameDependency 通过；ArmorSet 的同类边界

运行40177066058、源码9fa6d8dc9f62bc4453b88891e16d0e3cfcb3ca5f：GameDependency定向组及Xmlpowers组原DLL矩阵成功。真实SDK进入结构/模型数据，已记录W3DMesh调用；在BaseCivilianStructureNoGarrison的ArmorSet.id被拒绝。未完成基础数据，不能把部分调用视作完整集成通过。

核对AssetTypeGameObject.xsd全部pipelineOnly声明：仅ArmorTemplateSet.id、LocomotorSet.id、CrusherInfo.id。CrusherInfo已有处理，LocomotorSet上轮已修复，本轮补齐ArmorSet并加入原DLL三缓冲区样例。提交2ba2c5feb2d23a4c0854d41797dd8bb869d98f51；运行40177659712仅重跑受影响集合组后继续完整SDK，复用前轮已通过且未变化的AI和powers/依赖组证据。
