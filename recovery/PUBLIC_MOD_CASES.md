# 公开红警3 Mod源码用例

> 合入说明：本文的完整 Upheaval／公开 Mod 输入和大型调用轨迹保留在[固定验证源码](https://github.com/wu162/NewSdk/tree/068643219fded09a05fb7b75c6507b769c430316)；本次向 compiler-source 合入修复、回归、审计、说明和精简成功收据。完整复现 TestUpheavalSdk.ps1 前须先从该提交取回 SDK/Mods/Upheaval（含 PublicCases）。历史 sourceCommit/runId 保持原值。


## 当前批次

活动源码：`5d1a1e4ba1b80ebca401df425c6c744d4859f944`；[SDK运行40182719547](https://github.com/wu162/NewSdk/actions/runs/40182719547)，**已验收成功**。原版SDK先通过后才构建和运行恢复版；10次BAB调用全部退出0，20组bin/manifest/imp/relo逐字节相同。17资产全部出现在恢复版最终manifest，16个非Weapon资产各有1次实际恢复插件调用，Weapon使用正式恢复路由且禁用缓存。收据：`evidence/upheaval/40182719547-1/validation.json`。当前17个资产、8类，全部通过Upheaval的Mod.xml → Mod/PublicCases.xml登记，不改生产SDK DLL或编译器实现。

| 来源 | 固定上游提交 | 提取内容 | 有效资产 |
| --- | --- | --- | ---: |
| [Yuri’s Retaliation](https://github.com/CptGit/yuris-retaliation) | 7f3fa9b83acffb62d36da5c9710dad78992b3feb | ChaosDrone完整7份声明XML：车辆、武器、FX、粒子、属性修改器、建造命令和命令集 | 9 |
| [RedAlert3HelloworldMod](https://github.com/qingchen239/RedAlert3HelloworldMod) | 3dc9076f6061fa259f5b597a0b41eb2f3c2bdca0 | 完整SovietPowerPlantAdvanced.xml及对应的Soviet打包命令/命令集节点 | 3 |
| [RedAlert3_ReCoreMod](https://github.com/CruCru09/RedAlert3_ReCoreMod) | 67ff2345a928e65b8247765b25606e1e8f7f2ce5 | 完整AITeShuMapBuild.xml和SovietAIRapidTechnology.xml | 5 |

根ID分别加PublicYuri_、PublicHello_、PublicReCore_前缀，完整同步本地资产引用；模块ID保留原文。来源、原文件SHA256、适配说明、登记资产与ReCore MIT许可保存在PublicCases/manifest.json及ReCore-LICENSE.txt。Yuri与HelloWorld上游未提供许可文件；保留来源说明，不虚构许可。

## 用例价值与依赖

- Yuri：GameObject → GasAura武器 → 属性修改器与FXList → GPU粒子 → stock FXDustCloud贴图；建造命令 → 车辆 → 命令集。覆盖自身位置范围伤害、属性/染色/Lua事件nugget、圆柱体粒子发射、履带/激光绘制、炮塔、AI、声音模块和继承合并。
- HelloWorld：命令集 → Soviet打包LogicCommand → stock SpecialPower_PackReplaceSelf；建筑通过GenericPackPower XInclude与ReplaceSelfSpecialAbility替换为stock SovietAntiInfantryInfantry。保留完整建筑绘制、脚手架/平台、摧毁、能量、生产、修复/驻军等模块，覆盖AISpecialPowerInfo和多重XInclude。未导入另三座电厂和JapanMechaKing：后者只在Worldbuilder.manifest，不能冒充Static流已有依赖。
- ReCore：五个建造状态的OpeningMove、Timer、Interval、ObjectOfTypeExists组合及BuildEntry MinTime=300s；去掉根id后与Upheaval现有全部AIBuildStateDefinition无语义重复。九个GameObject依赖均精确存在于SDK Static.manifest；不引入自定义媒体或继承链。

静态扫描149处引用，报告见catalog/PublicModCaseDependencies.json。全球强引用均有本地资产或SDK预编译清单名称匹配；直接Include和XInclude文件均存在。AssociatedModule的两个名字在同一建筑内有实际Draw模块，NEWSKIN_FILL是模型内符号，不能按全局资产误判。唯一保留的未解析弱引用是Upgrade_PowerPlantTurbine；它同样存在于SDK原始SovietPowerPlantAdvanced.xml，不是本轮遗漏强依赖。保留原行为，由原版SDK验收，不为消除报告而伪造升级资产。

车辆模型/动画、装甲、移动器、OCL、肖像/按钮、激光纹理、履带、音频等沿用Static/Global/Audio资源流。所有候选都先核对引用，再执行SDK的Schema、预处理、继承、Include及链接门槛；静态报告不冒充通用依赖解析器。

适配边界：Yuri使用SDK BaseVehicle而非上游自定义覆盖；BecomeDissident和ChaosDroneFunctions仍是原字符串，不额外导入Lua游戏脚本。此处验证编译输入，未测试Lua执行、单位游戏行为或完整原Mod的可玩性。

## SDK验收要求

先在干净临时SDK中运行原版DLL的5个BAB入口（Studio case3三个AptUI、case4全局、case5基础）。任一非零退出、超时、Error/Critical或缺产物立即终止；原版全部成功后才构建并安装恢复组件、执行恢复版。两边使用完全相同的完整Upheaval加活动公开用例。

必须10次调用全部成功、20组bin/manifest/imp/relo逐字节相同，并在真实恢复插件trace中看到16个非Weapon资产。Weapon插件没有trace sink：核对其正式恢复路由、禁用构建缓存及最终manifest中的武器资产，配合完整产物比较。生产SDK文件不变，不重复未改动的chooser组件矩阵。

只运行case3/4/5，不扩展到LoD、后处理、Lua执行或BIG打包。收据记录sourceCommit、runId/attempt、originalBaselinePassed、各调用、配置/DLL/输入SHA256、输出对照与实际调用覆盖。

## 历史首批

源码`8d3fe79706c0ef1a2aa995a2fb669e74ebece444`，运行[40181842895](https://github.com/wu162/NewSdk/actions/runs/40181842895) attempt1已成功：原版先通过、10次调用退出0、20组产物一致，11资产均在恢复版manifest，10个非Weapon各有1次插件调用。收据在evidence/upheaval/40181842895-1/validation.json。

首批包含Yuri九资产及Afterdash两个过滤器对照。进一步逐项比较发现Afterdash与Upheaval已有定义相同，因此当前批次移除它们，避免把改ID当新语义覆盖；历史源码、GPL许可及验收均保留在原提交。RA3-IM调查后确认是注入/D3D叠加层项目，没有可用的SDK资产XML，未导入。历史成功只对应历史输入，不作为当前17资产已验收的依据。
