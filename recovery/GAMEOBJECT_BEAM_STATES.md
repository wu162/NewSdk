# LaserState / SweepingLaserState / StreamState（验证中）

三种完整模块；固定原blob6e40a9ed6c9f961d294d49768d911e6a6f354619。38方法精确签名/行号/SHA256见evidence/gameobject-modules/beam-state-source/manifest.json；GenerateBeamStateFields.py核对原blob、所有方法、字段/fallback、原PE ObjectStatus表和完整Token/强引用列表helper/sentinel，接入TestGameObject.ps1。未取得本批源码成功前不计入支持。

| 模块 | TypeId/native | 完整输入 |
|---|---|---|
| LaserState | 3887509261/44 | LaserId signed/base0@8默认0；OriginBoneName String@12；LaserEndParticleSystem强引用列表@20；LaserStartParticleSystem强引用列表@28；EndOffset可空Vector3@36；ObjectStatusValidation可空56字节@40 |
| SweepingLaserState | 1016285011/64 | Radius普通float@44默认10.0；SweepFXList强引用@48；VeteranSweepFXList强引用@52；SweepFXTimeout Time@56默认0s；SweepWeapon强引用@60；最后完整LaserState父类 |
| StreamState | 4277690261/12 | StreamId signed/base0@8默认0 |

粒子系统列表是每元素4字节的强引用，产生translations；Endian只交换count，不交换元素。Token列表helper为count key1820487795/type1564774360，element key2974916843/type0，bookmark每个4字节引用。

Laser依赖分配顺序：OriginBoneName内容、完整End列表、完整Start列表、可空EndOffset、可空validation；与XML次序无关。Sweeping先处理自身三个强引用（影响translations顺序），再处理完整父类；Token也保留额外LaserState父层。EndOffset Token直接指向Vector3数据，不生成递归Vector3子表。缺失指针null，显式空结构依次分配12/56字节。

完整差分覆盖三个类型全部属性、引用/字符串/数值/时间边界、六容器、0/1/2/27/28/29/65模块与双引用列表、空值、独立布局/强引用/分配/Token及sentinel、拒绝和次序不变性；SDK0次，原路由保留。此前LaserUpdate和LaserScatterUpdate与本组不同，本组单独计数。
