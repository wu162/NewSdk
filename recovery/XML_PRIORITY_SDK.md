# XmlCompiler 十四优先类型：SDK 验收

更新：2026-10-24。实际源码 `aac2279c99abf4c55d7f4d682b3e68bbe3a77107`；[运行 40091218796](https://github.com/wu162/NewSdk/actions/runs/40091218796) completed/success。组件与联合链接同源码 [运行 40091218825](https://github.com/wu162/NewSdk/actions/runs/40091218825) completed/success。

## 已取得的结果

| 检查 | 结果 |
| --- | --- |
| 完整 CnC3Types.xsd | 18 个合法资产；移除 id、缺少 Physics、未知 FXList 属性三个负例均拒绝 |
| 原版插件 metadata | 14 类型 × 平台 0/1/2/99，Initialize/ReInitialize，共 800 项公开字段/属性及版本哈希比较；8 项未知类型拒绝 |
| Reduced | 18 个必需资产，14 类真实调用；原版重放 54 次三缓冲数组比较，隔离恢复版再比较 54 次 |
| FullMymod | 保留 shipped Mod.xml 全部 7 个 Includes，再追加同一 probe；20 次实际调用，原版和隔离恢复版各比较 60 次 |
| 额外实际覆盖 | AlliedWarFactoryCommandSet、mytankCommandSet 各调用一次；未用放宽比较忽略这些原 MOD 资产 |
| 产物与引用 | 两 profile 各自的 bin/manifest/imp/relo 四份产物逐字节一致；imports 均非空 |
| 原 DLL | 仓库原 Tokenizer/XmlCompiler 指纹不变；隔离重放目录物理无两 DLL，所有调用前后检查均未加载原编译器程序集 |

同一 /O2 联合 DLL 与插件只编译一次，两 profile 复用相同文件。SDK 平台为 Win32；/Od、/O2 及其他三平台的组件验证另有完整矩阵，不能把 Win32 SDK 结果称为其他平台 SDK 验收。

| Profile | mod.bin 字节 | manifest 字节 | imp 字节 | relo 字节 |
| --- | ---: | ---: | ---: | ---: |
| Reduced | 4660 | 2058 | 16 | 224 |
| FullMymod | 214204 | 6955 | 76 | 4876 |

本次联合 DLL SHA256 `48658316633586fa182cf4fffb82a80c76adc32ce19d262f2c362304eb12a44d`；插件 SHA256 `791ea21ef4e69490d97ee4b18e77a5256c13e411dc671f66422b1a5df7ace22c`。各输入、产物和 trace manifest 指纹在 validation/replay JSON，不能用其他提交的旧文件替代。

## 路由与执行

`src/Plugin/XmlPriorityPlugin.cs` 显式处理十四个优先根，直接调用恢复桥接，其他 TypeId 明确拒绝。准确 metadata 见 [XMLCOMPILER_METADATA.md](XMLCOMPILER_METADATA.md)。

`scripts/SetXmlPriorityPlugin.ps1` 在指定 SDK 副本切换 Original/Recovered；去除重复显式注册，保留原 #all 与其他专用处理器。两份 SDK 都使用相同的十四类显式注册拓扑，避免 PluginRegistry.AssetBuilderPluginsVersion 对显式类型逐项求和造成无关差异；selected types 均禁 build cache。重复切换与回切后逐项核对十四类唯一处理器。

Windows VS2022 x86 开发环境运行：

```powershell
./recovery/scripts/TestXmlPrioritySdk.ps1
```

该脚本新建临时 SDK、清除派生缓存、新输出目录，分别运行原版与恢复版 BAB。Reduced 保留必要引用并使用最小输入；FullMymod 保留全部原 Includes 和其余 Mod.xml 内容，逐项验证没有删除或替换。原版与恢复版在每个 profile 的 Mod.xml 指纹必须一致，probe 与所有必要依赖也相同。SDK 原目录不被改写。

`tests/XmlPrioritySdkOracle.cs` 有 schema、metadata、replay、isolated 模式。trace 记录 BAB 预处理后的 XML、type/id/platform 及三份缓冲；强制十八 probe、十四类型齐全，FullMymod 的其他同类型调用也必须逐一通过三数组和最终 mod.bin 包含检查。未知类型、重复/孤立 trace、身份或平台不符均失败。isolated 使用物理排除原两 DLL 的独立目录，禁止已加载的原编译器程序集。

## 证据入口

- `evidence/xml-priority-sdk/validation.json`：本轮 sourceCommit/runId、两 profile、四产物和同 DLL 指纹。
- 根目录 `schema.json`、`metadata.json` 及编译/对照日志。
- `Reduced/` 与 `FullMymod/`：各自的 validation、Original/Recovered 配置与输入、四产物、BAB stdout/stderr、trace、replay/isolated JSON 和日志。
- 工作流 artifact：`xml-priority-sdk-evidence`。触发入口是 `recovered-xml-priority-sdk.yml`，显式 `[sdk-xml-priority]` 或 workflow_dispatch；普通字段改动不自动重跑 SDK 批次，组件回归继续保留。

## 适用范围

十八个源资产在 `sdk/XmlPriorityAllProbe.xml`。除十二类默认/最小必需子树外，覆盖 FXList 双过滤器、Particle 新 Draw/Update/两个具体 Event、Lightning 两项 RandomSet、RenderObject 三项 ObjectSet；EventFX 引用同输入中的 FXList，实际走非空 imports。缺省由 BAB/XSD 补全并进入恢复代码。

FXList/Particle 在 XSD 不可继承；EventBase 与未知 base fallback 也没有合法具体元素可表达。没有为 SDK 人工填 TypeId 或伪造继承；这些底层能力由组件矩阵证明。十四类其余字段、枚举和边界的广覆盖来自组件差分，不能把十八样例称为全部字段的真实 MOD 覆盖。

完整 shipped mymod 指本提交仓库中的 Mod.xml 及七个既有 Includes，再追加十八 probe。其他类型仍由原 SDK 插件处理；这不是整个 SDK 在没有原 DLL 时的完整构建，也不是全部 301 根类型的总替代。

早期 40090297467 因 BAB 结构 TypeId 被拒绝失败；已从 Core.ValidateInstances 原代码核对并修复各组。两根 SDK 成功 40090655209、十四根 Reduced 成功 40090901840 属于前置里程碑；最终双 profile 只引用本页顶部源码/运行。
