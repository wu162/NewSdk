# 逐类型接入 SDK 的验证策略

更新：2026-10-22。用户明确要求：一种类型完整恢复后，通过 plugin 配置让 SDK 使用新 DLL 编译该类型，逐步替换原 Tokenizer / XmlCompiler。原 DLL 差分仍保留，真实 SDK 编译不再等所有类型恢复后才开始。

## 已确认的配置机制

入口是 SDK/tools/BinaryAssetBuilder.exe.config 的 assetbuilder/plugins。当前 Tokenizer 显式负责 GameObject、WeaponTemplate、ArmorTemplate；XmlCompiler 的 #all 处理其余类型。

原版 PluginRegistry 按类型建立 Dictionary，注册使用 Add：不能重复注册同一类型，不能靠顺序覆盖。迁移 ArmorTemplate 时，必须将它从原 Tokenizer 的 assetTypes 中移除，再交给新插件。迁移默认 XmlCompiler 的类型时，显式注册该类型，保留 #all 处理其余类型即可；已有显式注册时同样先移除冲突。

## 首次接入目标（历史方案；当前已实现，见文末）

优先为已完整恢复的 ArmorTemplate 实现正式 IAssetBuilderPlugin 适配器，补齐初始化、版本、类型元数据和 ProcessInstance。现有 Recovered.Armor.dll 只是组件，尚不能直接填入 targetType 作为正式插件加载。

接入后的配置形态如下。Recovered.Compiler.Plugin / Recovered.Compiler 是计划中的适配器名称，尚未构建或验证，不能当成现成可启用配置：

```xml
<plugin assetTypes="GameObject,WeaponTemplate" enabled="true"
        targetType="BinaryAssetBuilder.Tokenizer.Plugin, BinaryAssetBuilder.Tokenizer" />
<plugin assetTypes="ArmorTemplate" enabled="true"
        targetType="Recovered.Compiler.Plugin, Recovered.Compiler" />
<plugin assetTypes="#all" enabled="true"
        targetType="BinaryAssetBuilder.XmlCompiler.Plugin, BinaryAssetBuilder.XmlCompiler" />
```

WeaponTemplate 尚缺 ScatterRadiusVsType 和 Nuggets，暂不切换整类 SDK 分派。新插件只声明已完成的类型，不在内部调用原编译器冒充恢复。

## 每种类型的验收

1. 完整恢复类型与依赖，原 DLL ProcessInstance 差分通过。
2. 在临时 SDK 副本放入新 DLL 和按类型配置，保留可运行的原版配置。
3. 使用全新输出、隔离缓存执行示例 MOD 基础数据编译，并加入明确包含目标类型及非默认字段的集成样例，避免示例没有触发该类型却误判成功。
4. 记录实际插件路由/调用，排除缓存命中或原插件回退；同一输入分别使用原版和恢复版配置编译。
5. 对比目标资产数据、重定位及引用，保留 bin/manifest/imp/relo 整体结果和差异。整体输出不一致时解释原因，不仅凭退出码或文件存在判定等价。
6. 提交可复现的插件构建、配置生成/切换工具、集成样例和验证证据。后续类型逐个加入；最终移除旧 DLL 依赖，执行完整替代两个 DLL 的 MOD 验收。

现有 /Od、/O2 多平台差分是回归层；按类型配置运行 SDK 是集成层；两个完整替代 DLL 是最终验收。分别报告实际通过范围。

下一轮约 35 分钟优先完成 ArmorTemplate 正式插件、配置分派和 SDK 原版/恢复版编译对照。之后继续 ModelCondition / ScatterRadiusVsType，再推进 Nuggets。遇到实际插件加载、元数据或预处理差异时记录具体阻塞，不通过原 DLL 转发绕过。

## mymod 验证源码与 XSD（用户补充，2026-10-22）

用户明确要求：为触发特定类型的真实 SDK 编译，可以且需要修改 mymod 源码；新增源码必须符合仓库 schema 规则。

- 样例工程为 `SDK/Mods/mymod`，实际 schema 目录为 `SDK/Schemas/xsd`，总入口 `CnC3Types.xsd`；例如 ArmorTemplate / WeaponTemplate 分别读取 `AssetTypeArmorTemplate.xsd` / `AssetTypeWeaponTemplate.xsd` 及其 include/import 依赖。
- 每接入一种类型，先读取该类型的 XSD、继承/共享定义和引用约束，再为 mymod 增加有独立 ID、明确非默认字段和必要依赖的合法 XML。不能仅照着反编译出的解析器推测可接受的 XML。
- 必须将新文件接入 mymod 实际基础数据入口的 Include/引用链；仅把文件放入目录不算触发编译。检查编译日志/新插件调用和产物，证明目标样例确实进入对应类型的 ProcessInstance。
- 先用原版插件配置验证同一份修改后的 mymod 可成功构建，再切换对应类型到恢复插件；两个配置必须使用相同的样例源码和隔离缓存/全新输出。
- 按真实入口执行 schema 校验，正确解析相对 include/import、命名空间、元素顺序、必填项、枚举和引用。保留校验/构建结果；不能为迁就样例放宽 XSD、关闭校验或把校验失败当作正常通过。
- 直接 ProcessInstance 差分中的畸形输入、未知枚举和溢出样例用于刻画底层行为；它们不自动成为合法 mymod 集成样例。集成样例须独立满足 XSD。
- 验证样例及入口修改应随源码和配置工具一并提交，便于后续重复运行；保留原有示例内容，使用清晰的验证文件/资产 ID。

下一轮 ArmorTemplate 接入验收必须同时包含上述 mymod/XSD 工作，不能只完成插件加载或运行未触发 ArmorTemplate 的旧示例。

## ArmorTemplate 当前接入状态 — 2026-10-23

已实现 `Recovered.Compiler.Plugin, Recovered.Compiler`，依赖恢复的 `Recovered.Armor.dll`，实际类型路由与上面的设计相同。配置切换脚本为 `scripts/SetArmorPlugin.ps1`，完整验证入口为 `scripts/TestArmorSdk.ps1`，使用与证据说明见 `ARMOR_SDK_INTEGRATION.md`。三个 mymod 验证资产包括默认、非默认和继承后的护甲；入口 Include 与 mytank ArmorSet 引用已提交。原版/恢复版 SDK 输出 bin/manifest/imp/relo 已逐字节一致，实际三次新插件调用及三缓冲区均已与原版核对。最终源码/run 以 `evidence/armor-sdk/validation.json` 为准。

ModelCondition / ScatterRadiusVsType 已继续完成差分验证；下一轮恢复 Nuggets。WeaponTemplate 完整前继续保留原版路由。新类型接入时复用此插件/切换/真实编译链，不能跳过合法 mymod 样例、SDK 预处理与实际产物比较。

## Nugget 阶段的实际 SDK 样例 — 2026-10-23

`RecoveryWeaponNuggetProbe.xml` 已加入 mymod 实际 Include 链，包含九种已恢复伤害类 Nugget 的合法 WeaponTemplate，DOT 必填字段和 ScalarInfo.Filter 均符合完整 XSD。`TestWeaponSdkProbe.ps1` 使用原版 Weapon 路由编译 mymod，要求九份恢复组件 InstanceData 实际出现在 mod.bin，并保存全部产物与证据。此项是合法源码/实际 SDK 产物核对，不能称为已切换恢复版 Weapon 插件。

同时，原有 Armor 正式插件对照随新增 mymod 样例运行，原版/恢复版 Armor 路由的完整 bin/manifest/imp/relo 仍逐字节一致。Nuggets 当前恢复 9/35 个显式分支，剩余 26 个明确拒绝；待 WeaponTemplate 完整后再按正式路由策略切换该顶层类型。
