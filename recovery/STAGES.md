# 后续恢复阶段与执行节奏

更新：2026-10-23。适用分支：`recovery/compiler-source`。

## 仓库级持续授权（2026-10-22）

用户明确要求：“这个仓库以后所有任务，仅仅不允许强推，其他都授权”。本仓库后续任务所需的全部仓库操作均已授权，无需重复确认；禁止强推、force-with-lease 和 API 强制更新分支。详见根目录 AGENTS.md。此前仅授权工作流自动推送的说明扩展为此仓库级授权。

## 用户明确要求：每轮约 35 分钟

用户于 2026-10-23 将每轮时长从约 25 分钟延长为约 35 分钟，覆盖此前规则；后续实现阶段按**约 35 分钟的有效工作量**组织，通常以 30–40 分钟作为估算范围。这是阶段规模和交接节奏要求，不是通过空等凑时长，也不是精确到分钟的完成保证。

* 每轮把恢复实现、Windows 编译、原 DLL 差分验证、必要修复、提交及交接合成一个有实际产出的阶段；不要把单个函数、单次编译或单个很小的组件当成独立会话。
* 如果约 10 分钟就完成原定内容，继续推进下一个相邻、可独立验证的内容，尽量达到约 35 分钟的工作量后再交接。
* 接近 35 分钟时先收尾当前可验证成果；已经启动的关键验证应取得实际结果。不要为了计时留下可直接修复的问题，也不要为了填时间重复已通过且未受影响的检查。
* 大功能允许跨多轮完成；每轮都应新增可审查的源码或解决实质阻塞，明确区分“组件通过”“单类型通过”和“完整替代 DLL 通过”。
* 每次交接记录本轮实际范围、验证结果、提交、剩余阻塞和下一轮约 35 分钟的合理范围。遇到外部阻塞或上下文容量风险可提前交接，但必须说明原因，不能把这种例外变成默认节奏。
* 保持原要求：不建立微任务调度器，不把工程切成一堆小任务；按有依赖关系的功能阶段推进，阶段成果及时提交，不强推。

本次仅调整规划和记录规则，不要求把文档修改本身延长到 35 分钟。

## 按依赖重新划分阶段

以下是工作流顺序，不代表每一行都能在一轮内完成。大阶段按自然的功能边界连续推进，每轮约 35 分钟；具体完成范围以原 DLL 证据和实际验证难度为准。

| 阶段 | 每轮优先打包的工作 | 阶段验收与交接边界 |
|---|---|---|
| 1. XML 节点适配 + ArmorTemplate 首条完整链路 | 恢复节点/属性/子节点访问及该类型所需基础解析；恢复 ArmorTemplate 布局、Marshal、Tokenize 和输出组装；建立直接对照原 ProcessInstance 的测试 | 目标是一个真实类型从 XML 到 InstanceData/RelocationData/ImportsData 的完整差分闭环。若首轮无法闭环，至少交付可编译节点适配和有证据的类型实现，保留准确失败点；不要把“读完代码”单独交接 |
| 2. WeaponTemplate 完整路径 | 复用已验证运行时与节点层，成组恢复默认值、标量、枚举、引用、可选块、列表和嵌套弹头；同步恢复 procedure table 与 endian correction | 可跨数轮，每轮覆盖一组相互关联的真实字段/结构并增加差分样例；最终验证完整 WeaponTemplate 输出，不能以只覆盖默认 XML 宣称完成 |
| 3. GameObject 与模块分派 | 恢复 GameObject 公共布局、模块分派/注册关系；按共享结构和依赖成组恢复模块，连同所需解析与 token 表一起验证 | 可跨多轮；以实际可序列化的模块组交接，维护已恢复/未恢复范围，禁止未实现分支静默成功 |
| 4. XmlCompiler 类型族恢复与插件集成 | 按实际依赖和共用布局组织 301 个类型；每轮合并基础辅助逻辑、相关类型序列化、插件分派和差分样例，避免逐类型分成微任务 | 每轮约 35 分钟，覆盖规模随复杂度调整；逐步形成可编译插件和明确类型覆盖表。仅元数据目录完成不算类型序列化完成 |
| 5. 两份替代 DLL 的整体构建和 MOD 验收 | 整合可替代插件、配置与依赖；在临时 SDK 副本替换 DLL，执行示例 MOD 基础数据编译，对照已有原版基线；补代表性类型和平台验证 | 必须使用重建 DLL 取得真实结果，记录输出差异及限制。若仍有缺口继续按约 35 分钟阶段修复，不用原 DLL 转发冒充恢复完成 |

## 下一轮的具体起点

阶段 1 已完成；阶段 2 当前支持 WeaponTemplate 的 91 个根属性（含两个 223 位 ObjectStatus 掩码）、四个 RangeDuration、两个 SoundOrEvaEvent、WeaponAiHintInfo、三个完整 ObjectFilter 过滤器、IncompatibleAttributeModifier 引用列表和完整 ScatterRadiusVsType 列表（内嵌 ObjectFilter 与 ModelCondition 掩码）。共用 EnumBitset 已接入旧根位集和过滤器的 284 位 KindOf 掩码；非空资源引用、状态位、对象列表及可选块组合均已进入原版 ProcessInstance 差分。实际验证提交及结果以 STATUS.md 和 evidence/weapon/validation.json 为准；这仍是类型子集，不能称为完整替代 DLL。

用户于 2026-10-22 补充验证方案：每完整恢复一种类型，就通过 SDK 的 plugin 配置把该类型分派到新 DLL，逐步替换原来的两个 DLL。原 DLL 差分仍保留；真实 SDK 编译不必等全部类型完成后才开始。详见 SDK_TYPE_INTEGRATION.md。

ArmorTemplate 正式插件与 SDK 接入阶段已实现：`Recovered.Compiler.dll` 实现真实 IAssetBuilderPlugin，调用恢复的 C++/CLI 组件；mymod 新增合法样例并接入实际入口，mytank 引用新护甲。原版/恢复版隔离编译输出四个文件逐字节一致，三个实际插件调用已核对原版输出。确切最终源码及验证运行见 STATUS.md 和 evidence/armor-sdk/validation.json；使用方法见 ARMOR_SDK_INTEGRATION.md。

ModelConditionFlagType / ScatterRadiusVsType 已完成原版差分与两种优化模式验证：每模式 28,496 个 ProcessInstance 样例、85,488 次缓冲区比较，以及 4,179 次 ModelCondition 和 2,070 次 ObjectStatus 直接掩码比较。详见 STATUS.md 和 evidence/weapon/validation.json。

Nuggets 共用基类、ScalarInfo、列表/多态 Token 已恢复，且六类伤害分支通过原 DLL 差分：Damage、DOT、GrapplingHook、两种 Leech、LineDamage。每模式 41,248 个样例、123,744 次三缓冲区比较、186 次显式拒绝检查；原有位集直接差分继续通过。新增六个合法 mymod 武器样例，完整 XSD 校验与真实原版 SDK 产物中的恢复组件数据核对均通过。Armor 新插件路由回归也保持四份完整产物一致。确切源码/run 见 STATUS.md、WEAPON_NUGGETS.md 和 evidence/weapon/validation.json。

下一轮约 35 分钟：补齐 DamageAndSpawn、FireOnObjects 等剩余 Damage 派生类型，再按依赖推进 projectile / weapon-OCL / 引用类分支，复用现有共用基类和 ScalarInfo。实现与 /Od、/O2 原版 ProcessInstance 差分、合法 mymod/XSD 样例、实际原 SDK 产物核对、证据和交接合并成一个实质阶段。当前 35 个显式分派分支恢复 6 个，剩余 29 个明确拒绝；维护覆盖表，不把空 Nuggets 或少数派生类型当作整类完成。WeaponTemplate 完整前继续保留原 SDK 路由；最终切换还需处理 BAB 给根/非多态子结构注入的 TypeId 元数据。

用户已授予仓库级持续授权（仅禁止强推）；继续保持限定证据范围、pull --rebase 后普通推送及避免递归触发。每轮按约 35 分钟有效工作量推进，完成验证、提交和准确交接。
