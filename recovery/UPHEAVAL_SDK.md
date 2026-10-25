# Upheaval 联合 SDK 验证

从 recovery/compiler-source ddf7ff6dac82b8f3e5331899d493e3847f46efb3 建立独立验证分支。Upheaval 原样取自 main bacfc69fc3e71c8116d1f22b933b8b6173170ecf，目录树 5551fc26a6f96b62613443e0d0fa2958c14cbce0。

仅执行 SDK/defaultscript.cs case3（3个 AptUI XML）、case4（MapMetaData_Global.xml）和 case5（Mod.xml）。全新输出目录，沿用默认 BAB 参数与完整输入、资源及 XSD。不执行后续 LoD、合并、修复、复制或打包。恢复组件在一次性 SDK 副本中联合启用，仓库 SDK/tools 保持原样。

构建 XML、GameObject、Weapon、Armor、W3X 插件和 Core、Remote、ExpressionEval、GameDataVerifier、EALABuild。原版与恢复版分别执行同一5入口，核对退出码、错误日志和20份 bin/manifest/imp/relo 哈希。EALABuild 在临时 SDK 中替换保存，但本次为提取 BAB 调用的无界面入口，不宣称运行 GUI 调度或每个已注册插件均有实际调用。实际覆盖以 trace 和输入为准；没有调用的组件不能宣称获得新增 SDK 验收。尚未还原的其他插件保留原版。

状态：待 Actions 验证，不能视为通过。日志、配置、DLL 指纹、输入 SHA256、产物及 validation.json 保存至 recovery/evidence/upheaval/<runId>-<attempt> 并上传 artifact。没有重复运行既有组件全量矩阵。
