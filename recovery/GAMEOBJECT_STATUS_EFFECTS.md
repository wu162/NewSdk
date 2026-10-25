# GameObject EMP与压制状态效果

状态：本轮最终完整组件矩阵已验收通过；精确源码/运行/计数见文末。本页不代表完整GameObject、完整替代DLL或SDK集成完成。

原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；28个精确原方法、wrapper、Color3f与必要helper指纹见 `evidence/gameobject-modules/status-effects-source/manifest.json`。`GenerateStatusEffectsFields.py` 再生metadata；逻辑位于独立 `GameObjectStatusEffectsRuntime.h`，无需新PE表。

| 完整类型 | TypeId | native | kind |
|---|---:|---:|---:|
| EMPUpdate | 3543149950 | 40 | 143 |
| SuppressionUpdate | 3170589623 | 56 | 144 |

按TypeId全头去重；二者Marshal直接ModuleData、Token父Update。

EMP：Lifetime/StartFadeDelay/DisabledDuration为Time@8/@12/@16，均0s；StartScale/TargetScaleMin/TargetScaleMax为float@20/@24/@28，均1；StartColor为nullable Color3f12@32，EndColor为nullable Color3f12@36。Color3f是小写r/g/b三个float@0/@4/@8，无默认，不做255换算或gamma转换，不是packed ColorTag。根属性后固定Start→End次序分配，不依赖XML节点顺序；缺省不分配，显式空节点分配12字节零。两个nullable Color都生成真实3字段嵌套Token表(mode4)，不能误用Vector的仅bookmark模式。endian交换根标量并递归交换存在颜色的3个float，不交换指针。

Suppression：UpdateDelay Time@8=1s；Suppressability float@12=0（不除100）；SuppressionDuration Time@16=3s；AttributeModifierSuppressed/AttributeModifierForceMove为强引用@20/@24无默认；IgnoreSuppressionObjectStatus为inline28字节位集@28无默认，复用完整223位Status合同/未知bit198/ALL完整7word。endian交换Time/float和7word，不交换强引用值；其最终平台字节由TrackerFinalize负责。

公共输入校验只接受EMP两个颜色节点及小写r/g/b，拒绝未知属性、重复/错误嵌套、外来namespace；Suppression无schema子节点。高kind guard提前返回，不能进入旧字段数组。

测试 `GameObjectStatusEffectsCases.cs` 验证完整字段、颜色缺省/显式空/大小写、schema顺序、Status位集与拒绝，保留既有矩阵。最终须/Od、/O2、平台0/1/2/99、三缓冲区逐字节差分和独立布局；不得以组件验收替代SDK或完整DLL。


## 本轮最终整批验收

有效验收提交`284599bfd958c0f62d40ce1b72d27dd70a568bcc`；GameObject run **40094269582 success**，同提交基础run **40094269592 success**。/Od、/O2每模式**381140 fixtures、1143420三缓冲区逐字节比较、13884拒绝、1060不变性**；平台0/1/2/99，两模式合计**2286840**次缓冲区比较。实际两模式日志、validation.json、sourceCommit/runId及运行终态均已逐项核对。

本轮实际29新，164→193/322；129种其他已知Marshal继续明确拒绝，58 Token-only未知Marshal回退保留。本轮SDK0次，原路由保留。
