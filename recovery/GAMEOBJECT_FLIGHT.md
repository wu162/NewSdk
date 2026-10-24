# Physics / Bezier / Missile（组件完整矩阵通过）

原blob 6e40a9ed6c9f961d294d49768d911e6a6f354619。flight-source证据49方法与PE枚举，导出源码2479c32473931bc1707ac7e0a372e0504b6ca727 / run40053432952 success。GenerateFlightFields.py核对原blob/全部方法SHA/PE、全属性默认/完整Token及sentinel，模板bookmark按指针元素大小换算偏移。

PhysicsBehavior 2042621387 / 104字节，27全属性；五个bool位于96–100，101–103填充；角速度普通float默认360，不是Angle；百分比除100，时间沿用原Time。

BezierProjectileBehavior 197077561 / 244字节，43全属性：18位Flags默认PING_PONG_SIDEWAYS_DRIFT、Type枚举；两段完整36字节KindOf位集@84/120（284位，未知149），signed BounceCount、unsigned GarrisonHitKillCount、FX/Weapon强引用、Emotion枚举（未知8）、普通float/percent/time及5Hz速度。FlightPathAdjustDistPerSecond@184复用原Velocity helper的float→double(float0.2)*double(value)→float契约。非nullable标量无fallback缺失保持0；MaxDistanceToTravel默认1000000。

MissileUpdate 2441348999 / 268字节，在完整Bezier父层上增加FuelLifetime@244、IgnitionDelay@248、两距离float@252/256、IgnitionFX强引用@260、DetonateOnNoFuel bool@264及末尾填充；保留Bezier父Token，不压平。Marshal自身字段后父层，Token父层在先。

新增ModuleValue::Bits只用于本组两段KindOf，通用bitset解析复用；Endian按9words交换，不把它当1word Options。桥接三类型无子结构，拒绝所有未知字段/嵌套节点。

FlightFamily覆盖每属性数值/引用/时间/布尔边界；每一个KindOf命名项、两mask及两个继承类型的独立位位置；全部Flags/Type/Emotion枚举；size/default/padding/parent/sentinel/5Hz；全部module容器及0/1/2/27/28/29/65；拒绝与属性/引用顺序不变性。完整矩阵已通过，116→119/322，203种已知Marshal仍拒绝。sourceCommit 3af11bee2540673c97a671dcf39876c7e687c7d8；GameObject run40068382383 success；基础run40068382487 success。每/Od、/O2：217200 fixtures、651600三缓冲区比较、10640拒绝、448不变性；平台0/1/2/99。终态与validation.json、两模式日志已核对。SDK原路由保留。
