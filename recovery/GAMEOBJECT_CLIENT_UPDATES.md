# ClientUpdate全部六种XSD类型

已实现且原DLL完整差分成功。源码76e1e34c12b4728ab194acffbde1a26bef52febd，run39966783977 success；后续矩阵包含全部本组。

| 类型 | TypeId | nativeSize |
|---|---:|---:|
| BeaconClientUpdate | 4183522625 | 16 |
| EvaAnnounceClientCreate | 3505323485 | 28 |
| EvaAnnounceSpecialPowerReadyClientUpdate | 2720065494 | 28 |
| LaserUpdate | 3399407677 | 32 |
| RadarMarkerClientUpdate | 2941371362 | 16 |
| LargeGroupAudioClientUpdate | 2552668823 | 28 |

完整25属性；LargeGroupAudio的Key为重复4字节、区分大小写FastHash，UnitWeight为ushort@24。LaserLifetime是普通float。全部继承Token、列表Token、endian和独立布局已验证，包含缺省、显式0、空与长列表、边界、四平台和两优化模式。每模式83,608 fixtures、250,824三缓冲区比较、8,384拒绝、268不变性比较。SDK未运行。
