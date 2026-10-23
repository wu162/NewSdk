# 编译器恢复：当前续接入口

更新：2026-10-24（北京时间）。仓库wu162/NewSdk，分支recovery/compiler-source。先读AGENTS.md、EFFICIENCY.md及本文件。

## 已完成：四种声音／播报选择器

源码448bf2903742f7d43b2dc1e5938ef2fd4a2ca416；GameObject run40016394892、基础run40016394889均success。已核对远端validation.json及日志：每/Od、/O2模式105,348 fixtures、316,044三缓冲区比较、8,516拒绝、284不变性比较，平台0/1/2/99。两模式合计632,088次缓冲区逐字节比较。原矩阵完整保留。

已完成50/322已知Marshal类型；其余272明确拒绝，58 Token-only沿用原未知Marshal路径。根范围88属性、38类根子节点未变。未完成整个GameObject、两份完整替代DLL或正式GameObject插件。SDK本轮0次，原路由保留；component artifact为recovered-gameobject-component-x86。

详情GAMEOBJECT_SOUND_SELECTORS.md。四选择器全部属性、三Override、音频三种Voice列表和Sound/EVA列表、可空有符号优先级、弱Upgrade列表均完成；原方法在sound-selector-methods/，生成器GenerateSoundSelectorFields.py。注意AudioSoundEntry.Sound内联资源引用，SoundOrEvaEvent.Sound才可空。首次失败为测试节点名漏ClientBehavior后缀，最终修订完整通过。

此前九种Body、六ClientUpdate、两个声音播放模块、31公共模块及AI/根子节点继续在相同完整矩阵中；详见GAMEOBJECT_BODY.md、GAMEOBJECT_CLIENT_UPDATES.md、GAMEOBJECT_SOUND_PLAYBACK.md等，不重复全量加载历史STATUS。

## 下一完整组：生命周期Update

正在调查；尚未启用。DeletionUpdate、DieOnProducerDeathUpdate、InvalidTargetsDeathUpdate、HeightDieUpdate、LoseHealthUpdate、LifetimeUpdate。原方法和helper证据在evidence/gameobject-modules/lifecycle-source/manifest.json（68方法，固定原blob）。另含DestroyDie/DieMuxData，属于后续Die家族，不能因此声明已实现。
关键：UpdateModuleData Token实际继承ContainModuleData；LoseHealth有可空56字节ObjectStatusValidation，InvalidTargets有可空28字节ObjectStatus；Lifetime有可空有符号int；Height InitialDelay是ceil到逻辑帧的uint，与Time不等同。须继续逐方法核对enum表、默认、分配次序及Endian，不能仅看nativeSize。
普通组件组不触发SDK，完成整组代码和测试后统一完整矩阵；SDK累计到正式集成里程碑。

## 持续规则

- 仓库操作持续授权，只禁止强推；保留并发提交和Actions证据，普通快进更新现有分支，不新建无必要分支。
- 约45分钟有效实现阶段；每约10–15分钟检查并及时保存有意义成果，注明未验证/验证中/已验证。先完成一组则保存并继续下一依赖满足组，不空等凑时长。
- 默认单代理，按完整能力组连续调查/实现，统一/Od、/O2、四平台含99、三缓冲区逐字节、独立布局/Token/sentinel及拒绝矩阵。不得降低正确性。
- SDK跨阶段累计；原/恢复相同精简输入、合法XSD、实际调用与四产物比较、适用无原DLL重放门槛保留。组件成功不能冒充SDK集成。
- 命令行Git本环境无凭据，使用GitHub插件读写；大于1MB原反编译无法直接取回时，使用原blob核对的源方法导出工作流保存分段证据，不绕过原证据。无需重复完整Windows验证同一源码。
- 保留Weapon/Armor原已验收范围；Weapon source22c12f53c2f3c886cdb63d212ab10bc57cf715e0/run39896743313，Armor最近相关run39896743435。本轮未改其源码、SDK输入/路由，不复跑已完成SDK。正式SDK路由仍原版。
