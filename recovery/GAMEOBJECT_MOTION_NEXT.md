# 下一完整组：Projectile / Dynamics / Freefall（源码调查，尚未实现）

源码调查导出97方法：sourceCommit76ea42b0ab846d3f594887a72dfa1146aad28f58、run40051656143 success；具体Movement Marshal、typed dispatch、四typed wrapper均已核对纳入manifest。

这四种已知Marshal仍在明确拒绝清单，不能把原方法导出当运行支持。证据目录evidence/gameobject-modules/motion-source，固定原blob6e40a9ed6c9f961d294d49768d911e6a6f354619。manifest.json保存全部精确签名、原行号、方法body SHA256；enum-source.json从原PE读取，不按枚举名称猜值。

| 模块 | TypeId/native | 原Marshal及完整目标 |
|---|---|---|
| ProjectileUpdate | 290244143/28 | 003.txt：StateMachine scalar@8（无fallback）；MinDetonationHeight普通float@12；MinTargetDistance普通float@16；Flags一位ProjectileUpdateOptions@20；Movement为可空单一多态ProjectileMoveBase指针@24 |
| DynamicsUpdate | 358254523/32 | 009.txt：Motion enum@8；Collision enum@12；IntuitiveCollision enum@16默认COLLIDES_WITH_GROUND_ONLY；ContactTag enum@20默认NONE；DrawPriority enum@24默认REMOVABLE；Volume可空76字节DynamicsVolumeType@28 |
| FreefallUpdate | 4230098692/80 | 015.txt：ModelConditionsToSet完整456bit@8（60字节）；InitialVelocity float@68默认0；Acceleration float@72默认1；TerminalVelocity float@76默认15 |
| EnableDynamicsCollide | 4235893941/12 | 021.txt：Weight普通float@8默认0.1 |

## 不可漏掉的依赖

Projectile Movement调用MarshalSinglePolymorphic<ProjectileMoveBase*>。原SinglePolymorphic wrapper（按manifest精确签名定位）只在节点恰有一个element时调用真正Marshaler；0或多于1时不调用。它把target变量直接传给ProjectileMoveBase**重载，不能因为没有显式cast而漏读后续分派。ExportLifecycleMethods.py已扩展识别SinglePolymorphic/PolymorphicType调用，并为本组显式纳入所有ProjectileMoveBase**入口。已纳入四种Movement typed wrapper、真正ProjectileMoveBase**分派及具体方法；继续时仍核对最新manifest导出终态、完整dispatch及所有Movement实现，不能把wrapper当完整实现；需要检查未知TypeId/fallback、零/一/多个Movement元素、Tokenizer/Endian对应行为。

Dynamics Volume（按DynamicsVolumeType的精确Marshal签名定位）全部属性：Mass float默认0、Bounciness percent默认30%、Spinniness float默认2.0、StaticFriction默认0.6、KineticFriction默认0.5、AngularDrag默认0.1、LinearDrag默认0.1、ReportContacts bool默认false、ContactTag enum默认NONE；完整Sphere/Capsule/Box/Cylinder/Triangle五类列表。DynamicsShape基类（按DynamicsShapeType的精确Marshal签名定位）有Translation与Rotation（Vector3/Quaternion）；必须继续核对各derived布局、列表stride/分配顺序、缺失/显式空、父层Token及sentinel，不用已有Geometry形状替代这些不同类型。

枚举原helper fallback：DynamicsMotionType=1、DynamicsCollisionType=1、IntuitiveCollisionType=2、DynamicsContactTag=7、DynamicsDrawPriority=2、ProjectileStateMachineType=2；ProjectileUpdateOptions未知bit0。PE中的ModelConditionFlagType含457名称（最后未知456），实际位集456bit，复用既有MarshalModelCondition时核对相同约束。

## 建议整组实施与一次验收

先完成Dynamics Volume及五种shape/Quaternion、Projectile Movement完整分派和公共解析契约，再连贯实现四模块与所有依赖；不能只恢复Freefall/Weight两个标量而反复运行完整矩阵。新增类型保留拒绝直到有完整布局、Token和输入校验。完整后统一/Od、/O2、0/1/2/99三缓冲区逐字节差分、独立布局/枚举/位集/nullable/list/Token/sentinel、0/1/2/27/28/29/65、拒绝与不变性。

不自动触发SDK；原路由继续，正式GameObject集成门槛不降低。沿用当前recovery/compiler-source分支，普通推送，不强推。大文件fetch_file行范围在本环境返回空，因此用有界方法导出；后续复用signature/bodySha相同原证据，不重复下载全部源。

原Movement分派：2240593884=ProjectileMoveInStraightLine（16字节）；442194537=BalloonProjectileMoveToGround（4字节，仅TypeId）；2272689654=ProjectileMoveToGround（28字节）；3012512132=BezierSplineProjectile（24字节）。未知TypeId原版分配4字节并写0；所有这些属于下一组待实现范围。
