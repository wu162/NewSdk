# 恢复与维护资料分类

日常使用见 [新编译器使用说明](../COMPILER_USAGE.md)，当前开发状态见 [交接入口](../REFACTOR_RESUME.md)。本目录中的历史恢复记录须按原 sourceCommit/runId 理解，不替换当前能力合同。

## 当前编译器性能优化

[当前短交接](compiler-optimization/HANDOFF.md) 保存最新接受点与下一步；[方案索引](compiler-optimization/README.md) 按问题导航，[实现合同](compiler-optimization/CONTRACTS.md) 说明边界与开关，[当前验证](compiler-optimization/VALIDATION.md) 区分源码/二进制/测试和原 ZIP。历轮失败与产物身份集中在 [前五轮归档](compiler-optimization/history/VALIDATION_ROUNDS_1_5.md)，完整用户方案只保留 PLAN 一份。

## 类型恢复记录

| 分类 | 入口 |
| --- | --- |
| GameObject 模块、属性与依赖 | [GameObject](components/gameobject/README.md) |
| XML 资产类型 | [XML](components/xml/README.md) |
| Weapon 结构与处理链 | [Weapon](components/weapon/README.md) |
| Armor 与 SDK 集成 | [Armor](components/armor/README.md) |

## 基础库与编译链参考

- [Audio](../AUDIO_COMPILER.md)、[Hash](../HASH.md)、[Utility](../UTILITY.md)、[Metrics](../METRICS.md)、[MvpXml](../MVP_XML.md)
- [Managed support](../MANAGED_SUPPORT.md)、[Relocation runtime](../RELOCATION_RUNTIME.md)、[源码来源](../PROVENANCE.md)
- [XML 元数据](../XMLCOMPILER_METADATA.md)、[优先级](../XMLCOMPILER_PRIORITY.md)、[SDK 类型集成](../SDK_TYPE_INTEGRATION.md)
- [构建基础设施](../BUILD_INFRASTRUCTURE.md)、[公开 MOD 用例](../PUBLIC_MOD_CASES.md)、[Upheaval](../UPHEAVAL_SDK.md)

## 历史设计与交接

- [历史重构方案](../NewSdk_ModCompiler_Refactor_Plan.md)：实施设计，当前状态以前述入口为准。
- [旧 EALA MOD Studio 流程](../EALAMODSTUDIO_BUILD_FLOW.md)：用于对照原脚本，不表示新工具仍执行全部旧步骤。
- [历史状态流水](history/STATUS.md)、[重构交接归档](history/REFACTOR_HANDOFF_ARCHIVE_20261026.md)
- [早期 recovery README](history/RECOVERY_README_INITIAL.md)、[结果主线建立前的 RESUME](history/RESUME_BEFORE_COMPILER_MAIN.md)
- [旧 Audio 交接](../RESUME_AUDIO.md)、[旧 GameObject 交接](../RESUME_GAMEOBJECT.md)、[旧 XML 交接](../RESUME_XMLCOMPILER.md)、[旧阶段规划](../STAGES.md)

根目录保留当前产品文档与少量稳定参考入口，类型记录按分类收纳；源码、脚本、原始验证文件不随本次文档整理移动。
