# 生命周期 Update：六种完整模块

组件已验证；SDK未运行，原路由保留。源码36bdb6c0bf42094010222fb7e5fa102644cce142，GameObject run40018019890、基础run40018019764均success。每/Od、/O2模式112,084 fixtures、336,252三缓冲区逐字节比较、8,576拒绝、292不变性比较；平台0/1/2/99。两模式合计672,504次缓冲区比较。后续共享Die组若通过，则仍在其完整矩阵内回归本组。

| 模块 | TypeId | 大小 | 全部根字段 |
|---|---:|---:|---|
| DeletionUpdate | 1111529555 | 16 | MinFrames@8=0、MaxFrames@12=1；unsigned |
| DieOnProducerDeathUpdate | 474181074 | 16 | SleepInterval@8=0.5s（Time float）；DieOnProducerDisabled@12=false |
| InvalidTargetsDeathUpdate | 1461180892 | 12 | InvalidStatus@8，可空28字节ObjectStatus位集 |
| HeightDieUpdate | 1964506706 | 20 | TargetHeight@8；InitialDelay@12=0；三bool@16/17/18均false |
| LoseHealthUpdate | 2974418213 | 24 | HealthTrigger@8=10%；DamagePerSecond@12=100；DeathType@16=NONE；ObjectStatusValidation指针@20 |
| LifetimeUpdate | 1814576242 | 44 | Min/MaxLifetime@8/12=0s；DeathType@16；CountdownSound/CountdownCompleteSound@20/24；TimeBeforeDeathThatCountdownCompletes@28=0s；TimeBetweenCountdownSounds@32=1s；可空int NumberOfCountdownSoundPlays@36；WaitForWakeUp/ScoreKill@40/41=false；IgnoreProjectileState@42 |

24根属性全部实现。ObjectStatusValidation为可空56字节结构：ForbiddenStatus@0与RequiredStatus@28各七word，查找顺序为Forbidden后Required。缺失结构保持null；显式空结构分配56字节。InvalidStatus缺失与显式空位集也保留指针差异。

Height InitialDelay原方法23162是atof→float→乘float 0.005→float→ceil→float→CLR x86 conv.u4；并非Time。复用此前根Duration已证明的int64范围/异常值低word行为，覆盖负数、临界毫秒、NaN/Inf与溢出。Lifetime和SleepInterval仍调用原Time契约。默认10%必须执行float(10)*float(0.01)，位模式0x3dcccccc；首次独立断言错误地写成0.1f，run40017749178失败，36b修订后完整通过，不把失败算作成功。

DeathType完整34项PE原表，scalar fallback33；大小写精确匹配。Nullable int为strtol(base0)，分配4字节，Token绑定pointee，Endian只交换非null值。ObjectStatus原223项及ALL/未知/重复排除语义复用已验收公共实现。

Token必须保留原链UpdateModuleData→ContainModuleData→BehaviorModuleData→ModuleData，不能按类名“修正”Contain这一层；生成器从原方法递归派生。原blob固定6e40a9ed6c9f961d294d49768d911e6a6f354619；方法、原行号和SHA256见evidence/gameobject-modules/lifecycle-source/manifest.json。字段查找/默认与原方法逐项assert，Death enum取原PE表；GenerateLifecycleFields.py --check集成进TestGameObject.ps1。

新增完整枚举与位集、每字段边界、可空输入、六容器分派、多模块列表、异常/拒绝、独立默认/偏移/分配/继承Token/sentinel测试。已有选择器、Body、ClientUpdate、AI和根子节点矩阵均保留。本组并不代表所有生命周期相关模块或完整GameObject已完成。
