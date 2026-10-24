# GameObject 离场、对象清理与拖拽

状态：本轮最终完整组件矩阵已验收通过；精确源码/运行/计数见文末。本页不代表完整GameObject、完整替代DLL或SDK集成完成。

原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；50个精确原方法/wrapper及String/Time/引用/ObjectFilter/状态验证闭包和指纹见 `evidence/gameobject-modules/motion-cleanup-source/manifest.json`。`GenerateMotionCleanupFields.py` 再生metadata；逻辑独立于 `GameObjectMotionCleanupRuntime.h`。三个ID按全ModuleChoice去重，无新PE表。

| 完整类型 | TypeId | native | kind | Token父 |
|---|---:|---:|---:|---|
| RunOffMapBehavior | 2272972461 | 20 | 159 | Behavior |
| DestroyObjectsUpdate | 2082975044 | 124 | 160 | Update |
| GrapplingHookUpdate | 426756072 | 52 | 161 | Update |

三者Marshal直接ModuleData。RunOffMap：RequiresSpecificTrigger bool@16=true；RunOffMapWaypointName String8@8无默认；DieOnMap/FlyingOffMap bool@17/@18=false，padding@19不写。原schema先第一bool、再字符串、再后两bool；字符串缺省不分配、显式空分配NUL，endian只交换length和ModuleData两字。

DestroyObjects：Radius float@8=0.0，ObjectFilter为inline112@12；缺省node不Marshal但Token始终生成完整inline表(mode8)，有节点再应用Filter默认，endian递归整个Filter。native较大来自内联Filter，不是额外未知字段。

Grappling：PullRate float@8无默认；MinBounceDelay/MaxBounceDelay Time@12/@16默认0.75s/1.5s；BouncePercentage/CollidingPullRateFactor Percent@20/@24默认20%/0%；SingleFireWeapon/ContinuousFireWeapon/ContinuousFireWeaponBlocked强引用@28/@32/@36无默认；ContinuousFireWeaponDelay Time@40=0.5s；ObjectStatusValidation为nullable56@44，KeepAliveObjectFilter为nullable112@48。

Grappling固定根属性→状态验证→Filter的schema分配顺序，不按XML次序。验证结构ForbiddenStatus@0、RequiredStatus@28，Token生成完整nullable表(mode4)；Filter为mode7。endian交换Time/float/Percent并递归存在的验证和Filter；强引用、bool、指针不按普通word交换，最终引用由TrackerFinalize按平台写index+1，GameObject最终ImportsData为空。

Bridge kind161的KeepAliveObjectFilter走严格ReadDependencyFilter，Validation仅允许原两状态字段；RunOffMap无子节点，Destroy只接受ObjectFilter。高kind字段/节点校验提前return，重复/未知/foreign namespace/非法结构文本保持拒绝。测试 `GameObjectMotionCleanupCases.cs` 覆盖完整字段/父链、字符串、时间/百分比/强引用、验证/Filter缺省与空、schema顺序及拒绝，最终须/Od、/O2、四平台、三缓冲区逐字节、独立布局与不变性成功。


## 本轮最终整批验收

有效验收提交`284599bfd958c0f62d40ce1b72d27dd70a568bcc`；GameObject run **40094269582 success**，同提交基础run **40094269592 success**。/Od、/O2每模式**381140 fixtures、1143420三缓冲区逐字节比较、13884拒绝、1060不变性**；平台0/1/2/99，两模式合计**2286840**次缓冲区比较。实际两模式日志、validation.json、sourceCommit/runId及运行终态均已逐项核对。

本轮实际29新，164→193/322；129种其他已知Marshal继续明确拒绝，58 Token-only未知Marshal回退保留。本轮SDK0次，原路由保留。
