# 逐类型接入 SDK 的验证策略

更新：2026-10-22。用户明确要求：一种类型完整恢复后，通过 plugin 配置让 SDK 使用新 DLL 编译该类型，逐步替换原 Tokenizer / XmlCompiler。原 DLL 差分仍保留，真实 SDK 编译不再等所有类型恢复后才开始。

## 已确认的配置机制

入口是 SDK/tools/BinaryAssetBuilder.exe.config 的 assetbuilder/plugins。当前 Tokenizer 显式负责 GameObject、WeaponTemplate、ArmorTemplate；XmlCompiler 的 #all 处理其余类型。

原版 PluginRegistry 按类型建立 Dictionary，注册使用 Add：不能重复注册同一类型，不能靠顺序覆盖。迁移 ArmorTemplate 时，必须将它从原 Tokenizer 的 assetTypes 中移除，再交给新插件。迁移默认 XmlCompiler 的类型时，显式注册该类型，保留 #all 处理其余类型即可；已有显式注册时同样先移除冲突。

## 首次接入目标

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

下一轮约 25 分钟优先完成 ArmorTemplate 正式插件、配置分派和 SDK 原版/恢复版编译对照。之后继续 ModelCondition / ScatterRadiusVsType，再推进 Nuggets。遇到实际插件加载、元数据或预处理差异时记录具体阻塞，不通过原 DLL 转发绕过。
