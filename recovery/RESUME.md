# 编译器恢复：当前续接入口

更新：2026-10-23。仓库 `wu162/NewSdk`，分支 `recovery/compiler-source`。
先读本文件及 AGENTS.md / EFFICIENCY.md，再按本轮范围读取源码与证据。STATUS.md 保存历史，旧章节的“下一轮”不是当前计划。

## 持续规则

- 目标是完整恢复 XmlCompiler / Tokenizer 的 C# 与 C++/CLI；两份完整替代 DLL 尚未完成。
- 仓库操作持续授权，禁止任何强推。保留并发 Actions 证据提交；API 更新也只能普通快进。
- 实现阶段按约 35 分钟有效工作量；先完成子集就保存，再继续依赖满足的工作。首个可复现成果及时提交/推送，每约 10–15 分钟检查未保存成果。禁止空等或重复分析凑时长。
- 正确性优先：保持原 DLL 核对、/Od 与 /O2、四平台含 99、三缓冲区比较、独立布局和拒绝测试。只减少重复读取/重复调查，不减测、不放宽断言。默认单代理。
- 输出先筛选，大型反编译按方法展开。通常一次约 150 行或 8 KB；这是展示目标而非证据上限。不能把写规则当作已证明额度降低。
- 真实 SDK 可跨多个实现阶段集中验证，不要求每轮执行；可在临时副本中注释/移除无关 MOD 输入，保留依赖，原/恢复版使用相同输入。精简覆盖须记录，关键里程碑保留完整 mymod 验收，详见 EFFICIENCY.md。Armor 仅显式 [sdk-armor] 或手动批次运行。真实 SDK 按类型/字段批次集中验证。Weapon `[sdk-probe]` 为原路由 payload 批次，`[sdk-plugin]` 为正式恢复插件对照；普通修复只跑组件。输入链/路由变化后必须集成验证。
- mymod 源码须符合 SDK/Schemas/xsd 完整 XSD 并接入 Mod.xml。保留未支持输入的显式拒绝。

## 当前已完成：Armor 与 Weapon 逐类型正式接入验证

ArmorTemplate：此前正式插件、真实 SDK 配置路由、四份完整产物比较、实际调用及无原 DLL 重放已通过。本轮 mymod 变更后的 Armor 回归也通过。

WeaponTemplate：91 根属性、可选块、三种 ObjectFilter、状态位、ScatterRadiusVsType、共用 Nugget/ScalarInfo、全部 **35/35 显式 Nugget 分支**，以及 missing/zero/unknown TypeId 默认路径已恢复。

本轮关键细节：未知对象分配 40 字节，仅 Marshal 共用基类，输出 TypeId 清零；Tokenize 只有 12 字节包装记录且子表偏移为零，不能生成基类 Token 子表；endian 仅处理共用基类。所有非多态节点允许 BAB 注入的无命名空间 TypeId，Nugget 项仍按它分派。未知字段/结构及外来命名空间继续拒绝。

正式插件：`Recovered.Weapon.Plugin, Recovered.WeaponPlugin`，依赖 `Recovered.Weapon.dll`，不调用原编译器。`scripts/SetWeaponPlugin.ps1` 可逆、幂等切换 WeaponTemplate。默认检入的 SDK 配置仍是原路由；已在隔离 SDK 副本中实际完成恢复插件路由编译。其他类型仍需原插件。

## Weapon 精确验证锚点

源码：`22c12f53c2f3c886cdb63d212ab10bc57cf715e0`。
运行：https://github.com/wu162/NewSdk/actions/runs/39896743313 ，success。
每种 /Od、/O2：66,152 个 fixture、793,824 次缓冲区比较、1,002 次拒绝检查、2,070 次 ObjectStatus 和 4,179 次 ModelCondition 直接比较；含 Win32、Xbox360、PS3、99。两模式合计 **1,587,648 次缓冲区比较**。整套矩阵同时覆盖非多态 TypeId 注解和正式插件元数据/入口。

同次正式 SDK 批次：38 个合法 mymod 武器，覆盖全部 35 分支、完整 35 项混合列表和继承版本；完整 XSD 及三个非法样例拒绝通过；原/恢复版 mod.bin、mod.manifest、mod.imp、mod.relo 全部逐字节一致；38 个实际调用的三缓冲区原版重放通过，移除两份原编译器 DLL 后重放也通过。实际 SDK 为 Win32；其他平台由组件矩阵验证。

证据：`evidence/weapon/validation.json`、`formal-sdk.json`、`formal-sdk-output.zip`、`oracle-logs.zip`，sourceCommit/runId 和两模式摘要已核对。正式构建两份程序集在该 run 的 recovered-weapon-component-x86 artifact。
基础回归：https://github.com/wu162/NewSdk/actions/runs/39896743345 。
Armor SDK 回归：https://github.com/wu162/NewSdk/actions/runs/39896743435 ，success。

旧 `sdk-batch-validation.json` 是此前原路由 31 样例批次的独立历史证据，不等同于新的正式路由结果。详见 WEAPON_TEMPLATE.md / WEAPON_NUGGETS.md / WEAPON_INPUT_PIPELINE.md。

## GameObject 当前：根组件已恢复，尚未接入 SDK

已实现独立 x86 C++/CLI 组件 `Recovered.GameObject.dll`，支持 GameObject XSD 直接声明的 **87/88 根属性**：标量/字符串、六枚举、七 typed ID、三资源引用、Browser 字符串列表，以及两项 284-bit KindOf 位集。还不是完整 GameObject 编译器，也没有 GameObject 正式插件。SDK 继续使用原路由。

仅剩根属性 `EvaEventSecondDamageFarFromFirstTimeoutMS` 覆盖未恢复；**所有 XML 子节点仍显式拒绝**。省略的时长默认已按原 scale 0.004999999888241291f 及两个 float 舍入点恢复为 155。不要误用 31000 或假定 30 FPS。

源码：`7b958b9df5c31f880465d6fd2df8eec6cb73fa28`。
完整组件验证：https://github.com/wu162/NewSdk/actions/runs/39920441194 ，success。
/Od 与 /O2 各 12,452 fixtures、37,356 次三缓冲区比较、36 次拒绝检查；两模式合计 **74,712 次比较**。覆盖 Win32、Xbox360、PS3、99；全部 284 个 KindOf 名称在两个字段分别测试无符号/加号/减号，含 ALL、XOR、未知回退；六枚举全值、资源哈希/索引边界、Browser 与字符串/资源/位集混用、独立布局均通过。
对应基础回归：https://github.com/wu162/NewSdk/actions/runs/39920441265 。
证据：`recovery/evidence/gameobject-root/validation.json` 及两模式 oracle 日志，sourceCommit/runId 已核对；该 run 的 `recovered-gameobject-component-x86` artifact 包含组件 DLL。
本轮无真实 SDK 验证，不将组件成功当作 SDK 接入成功。共享 relocation/scalar/resource/enum-bitset 源码未修改。

默认布局来自原逻辑重建：592 字节根、126 字段加结束记录、90 成员书签、28 空偏移、8 空列表子表。默认 root=1620、InstanceData=2212；非空 Browser 每项令 Token 前缀增长 12 字节，不能固定根偏移。Buildable@428、ThingClass@432、DisplayColor@452 即使没有 XML 子节点也有成员书签；ReplaceModule@496 / InheritableModule@500 的原 Tokenize 非空路径仍缺失，不得擅自补成普通子表。

入口：`recovery/GAMEOBJECT.md`、`src/Native/GameObject.{h,cpp}`、`src/Bridge/GameObjectCompiler.cpp`、`scripts/GenerateGameObjectFields.py`、`scripts/TestGameObject.ps1`、`tests/GameObject{Oracle,Cases,KindOf}.cs`。
生成器从原 Marshal/Tokenize 和 PE 枚举表生成可读元数据，不嵌入原实例缓冲区。源码指纹需先规范化 Windows checkout CRLF；所有 126 个默认 Token 的键/偏移/分类已与独立原版运行时清单核对。

原版基线仍单独保留在 `evidence/gameobject`：196 样例、588 次重复三缓冲区比较；源码 `46b0ec5020b3911ec0b0403a3d60f63426590673`、run 39897995324 success。不要混淆基线与恢复组件证据。

## 下一轮约 35 分钟

1. 核对上述最终组件/基础回归终态和 sourceCommit；只读本入口、GAMEOBJECT.md 与目标源码，不重扫 Weapon 或大型反编译全文。
2. 先刻画根 +388 的 duration 覆盖：原 helper 55021、scale 9619；调查小数帧边界、负数、指数、溢出与 float-to-uint，保留两个 float 舍入点，不从默认 155 推断全部行为。
3. 再恢复一组相连子节点：Buildable@428、ThingClass@432、DisplayColor@452；DisplayColor 是 Color4 节点/ColorTag 路径，不能按 uint 文本处理。时间允许再接 396/400 两个可选字符串指针。逐一核对缺省、分配、Token/endian 及拒绝边界。
4. 保留 /Od、/O2 四平台完整三缓冲区差分和独立布局/拒绝门槛。其他子块与模块继续显式拒绝；若修改共享行为，评估并验证全部受影响使用者。
5. 等形成更完整的字段/子节点批次再集中做合法 mymod SDK 验证。正式启用 GameObject 恢复插件前须完成真实路由对照与无原 DLL 重放。

本轮复用已有共享模块和原版布局证据；SDK 构建次数为零，测试只在代码/门槛变化或实际失败修复后重跑。不能量化账号额度节省。

