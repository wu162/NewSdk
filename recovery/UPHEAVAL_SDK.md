# Upheaval 恢复组件联合 SDK 验证

## 结果：三个阶段成功

[运行40178403660](https://github.com/wu162/NewSdk/actions/runs/40178403660)，源码0afc6eb49d81b61b9840e21d0edd840d5f0f0de3。调用SDK/defaultscript.cs对应case3 AptUI（3个XML）、case4全局数据（MapMetaData_Global.xml）、case5基础数据（Mod.xml）。原版与恢复版5入口各执行一次：10次退出码均为0，20份bin/manifest/imp/relo的SHA256全部相同。未执行后续LoD、合并、修复、复制、BIG打包或安装步骤。

Upheaval原样取自main bacfc69fc3e71c8116d1f22b933b8b6173170ecf，目录树5551fc26a6f96b62613443e0d0fa2958c14cbce0；验证分支从recovery/compiler-source ddf7ff6dac82b8f3e5331899d493e3847f46efb3建立。原MOD与仓库SDK/tools保持不变。

验证收据、配置、DLL指纹、输入指纹、日志、输出和调用记录位于recovery/evidence/upheaval/40178403660-1/validation.json及同目录。恢复版Core/Remote/ExpressionEval/GameDataVerifier/EALABuild被放入临时SDK，XML/GameObject/Weapon/Armor/W3X按配置接入。未还原的其他类型仍使用原SDK插件。

真实调用是BinaryAssetBuilder.exe -> 恢复版Core -> 已注册插件。XML插件实际记录1379次调用（39种根类型），GameObject记录121次调用；W3X trace记录见同目录trace/w3x。此处的EALABuild仅在SDK副本中替换，入口为提取Studio默认脚本的BAB调用，不宣称运行GUI调度；注册某DLL也不意味着本MOD覆盖该DLL所有能力。

## 本轮发现与修复

联合接入时单插件脚本的Tokenizer存在性检查不适合全部替换，改为三条路由原子注册。随后修复XSD合法的嵌套身份属性边界：AI Personality的三个State和BuildDelayRange、LocomotorSet、GameDependency（XML与GameObject）、ArmorSet、共享ObjectVisionRange。各次原DLL定向差分与历史失败证据保留。

[根因审计](ID_METADATA_ROOT_CAUSE.md)区分XML合法属性、流程元数据与真正的序列化字段。新增全821份XSD审计清单和漂移检查。静态审计额外发现AISpecialPowerInfo.id可选属性遗漏，已汇总到独立组件补漏；其差分状态以recovery/evidence/xml-progression/validation.json为准，不将上述SDK成功锚点改写为新提交重跑。

## 运行策略

真实SDK已通过，不因文档或未被Upheaval触发的可选属性补漏机械重跑。后续先审计整组元数据及所有使用者、检查合法正例与拒绝边界，再做必要组件矩阵，最后集中一次必要SDK。用户观察到完整运行起步约7分钟：新完整作业先留约7分钟，再每3–4分钟检查；只在失败后读取对应日志。
