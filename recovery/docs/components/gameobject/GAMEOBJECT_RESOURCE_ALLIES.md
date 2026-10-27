# GameObject 地形资源与盟友能量

状态：本轮最终完整组件矩阵已验收通过；精确源码/运行/计数见文末。本页不代表完整GameObject、完整替代DLL或SDK集成完成。

原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；49个精确原方法、Capture父层/StringList/ObjectFilter及引用helper指纹见 `evidence/gameobject-modules/resource-allies-source/manifest.json`。`GenerateResourceAlliesFields.py` 对照完整原方法再生metadata；逻辑独立于 `GameObjectResourceAlliesRuntime.h`。两TypeId按全ModuleChoice去重，无新PE表。

| 完整类型 | TypeId | native | kind | 父合同 |
|---|---:|---:|---:|---|
| DistributeEnergyToAlliesBehavior | 251140562 | 24 | 152 | Marshal/Token均Capture |
| TerrainResourceBehavior | 1510899067 | 44 | 153 | Marshal Module / Token Update |

Energy只增加ComradePercentage Percent@20，默认100.0→1.0，先本层再完整Capture20父层。父GrantMoney unsigned@8=0、SlavedCaptureObject弱GameObject ID@12无默认、GrantMoneyOneTime bool@16=false，padding@17–19不写；不是强引用。Token必须保留独立Capture父表，不能压成Update或Module。

Terrain：Radius float@8=0，MaxIncome signed@12=0，IncomeInterval unsigned@16=0xffffffff（不是Time）；Upgrade强引用@20无默认；UpgradeMustBePresent是StringList8@24无默认（不是bool、引用或typed-ID列表）；UpgradeBonusPercent Percent@32=100；ObjectFilter nullable112@36；HighPriority/Visible bool@40/@41默认false/true，末尾padding不写。

原属性顺序保留：先根属性与StringList数组/逐字符串分配，再Filter。StringList每项String8，完整列表Token使用mode6/stride8；Filter为nullable完整Token(mode7)。endian交换整数、float/Percent、StringList的每个length和count及存在的Filter，引用与指针不交换；内部强引用由TrackerFinalize按平台写index+1。GameObject最终ImportsData由原HandleType清为空，不能套用通用Tracker import列表格式。

Bridge kind153的ObjectFilter使用既有严格Filter读取；Energy无子节点，Terrain只允许该Filter。高kind属性/节点判断提前return，避免旧数组越界。测试 `GameObjectResourceAlliesCases.cs` 覆盖继承、weak/strong区分、signed/uint、StringList、schema顺序、完整Filter与拒绝，保留/Od、/O2、平台0/1/2/99、三缓冲区逐字节、独立布局及不变性。


## 本轮最终整批验收

有效验收提交`284599bfd958c0f62d40ce1b72d27dd70a568bcc`；GameObject run **40094269582 success**，同提交基础run **40094269592 success**。/Od、/O2每模式**381140 fixtures、1143420三缓冲区逐字节比较、13884拒绝、1060不变性**；平台0/1/2/99，两模式合计**2286840**次缓冲区比较。实际两模式日志、validation.json、sourceCommit/runId及运行终态均已逐项核对。

本轮实际29新，164→193/322；129种其他已知Marshal继续明确拒绝，58 Token-only未知Marshal回退保留。本轮SDK0次，原路由保留。
