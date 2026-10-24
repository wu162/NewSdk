# Projectile / Dynamics / Freefall 恢复（实现检查点，待完整矩阵）

四个模块及其依赖已实现，尚未验收，不计入已验证112种。固定原blob 6e40a9ed6c9f961d294d49768d911e6a6f354619；97个方法与PE枚举证据位于evidence/gameobject-modules/motion-source。GenerateMotionFields.py正式运行核对原blob、全部方法body SHA256、PE枚举、wrapper类型及分配大小、shape stride、enum fallback与Token sentinel。

| 模块 | TypeId | native大小 |
|---|---|---|
| ProjectileUpdate | 290244143 | 28 |
| DynamicsUpdate | 358254523 | 32 |
| FreefallUpdate | 4230098692 | 80 |
| EnableDynamicsCollide | 4235893941 | 12 |

Projectile Movement@24：SinglePolymorphic恰一个element才分派；四typed Movement大小16/28/24/4。未知TypeId分配4字节写0，有一条type0多态Token wrapper、无嵌套表。每种已知Movement保留完整ProjectileMoveBase父Token。Ground的angle/percent默认值复用既有原始解析；Bezier Path是强引用列表，Endian只交换count。

Dynamics Volume@28：可空76字节，全部9属性和Sphere/Capsule/Box/Cylinder/Triangle列表。shape stride为12/16/24/20/48，分配顺序固定按此五类；每个shape先derived标量，再Translation@0（可空12字节）、Rotation@4（可空16字节），变换按Translation再Rotation分配。Quaternion.w默认0，不是1。Token保留shape基类且直接bookmark变换pointee。

Freefall含完整60字节ModelCondition位集；Weight默认0.1。两个更新模块缺失的无fallback枚举保持分配时0，显式空/未知用原helper fallback，不能混同。

输入桥接验证所有已支持属性和嵌套结构；多态Movement按TypeId限制字段/Path，拒绝错误属性、外部namespace、未知结构及重复singleton。原始XML节点名不参与typed dispatch。未改变SDK原路由，未宣称完整GameObject或替代DLL。

测试MotionFamily覆盖四平台、全字段边界、PE枚举/未知fallback、位集、Movement四分支/未知/0或多个element、强引用Path、Volume五类shape/变换/list、0/1/2/27/28/29/65、所有module容器、独立native大小/默认/Token/分配与schema顺序不变性。最终验收仍需现有完整GameObject /Od、/O2三缓冲区差分及基础回归；结果回填本文件和RESUME.md。
