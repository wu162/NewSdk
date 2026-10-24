# 下一完整模块：TechUpgradeReceiver（仅源码调查）

TypeId393460526，native180字节；尚未启用，仍在明确拒绝清单。不得把源码证据当运行支持。固定原blob6e40a9ed6c9f961d294d49768d911e6a6f354619；44方法精确签名/原行号/SHA256见evidence/gameobject-modules/tech-receiver-source/manifest.json，首次导出run40047754601 success。ReceiverDistanceType原PE名称/数值表见同目录enum-source.json；源及PE表导出run40048070974 success，已核对CIRCULAR=0、RECTANGULAR=1、未知0。这是独立类型，Token类型873624354不同于发送端DistanceCheckType的30012736，不能因名称/数值相同而复制其类型绑定。

## 已核对原Marshal39092（003.txt）

| 自身输入 | 偏移 | 原始语义 | 缺失默认 |
|---|---:|---|---|
| Radius | 36 | 普通float，atof | 保持0 |
| RadiusY | 40 | 可空普通float，允许负值 | null |
| DistanceType | 44 | ReceiverDistanceType scalar，2项、未知0，原helper58205（021.txt） | CIRCULAR |
| FXLineBoneName | 48 | inline8字节String头 | 保持0/null |
| RadarDrawModule | 56 | 弱TypedAssetId<ModuleData>，原helper67672（023.txt）先小写再FastHash | 保持0 |
| RadarRotateSpeed | 60 | Angle，原AngleTag Initialize | 0.1r |
| RadarRotateOffset | 64 | Angle | 0r |
| ObjectFilter子节点 | 68 | inline112字节ObjectFilter | 缺失保持全零，显式空才写filter默认 |

最后调用完整UpgradeModuleData基类（36字节），所以共同五bool、可空CustomAnimAndDuration、TriggeredBy/ConflictsWith弱列表都要保留。分配顺序为RadiusY、FXLineBoneName内容、inline filter的依赖，最后公共Upgrade的动画/弱列表；不能按内存偏移或XML次序重新排序。

## 原Token与Endian

完整005.txt记录9个非零Token加sentinel：Upgrade父层、Radius、RadiusY可空scalar pointee、独立ReceiverDistanceType(type873624354)、FXLineBoneName、RadarDrawModule、两Angle、inline filter@68。不要省略父层或沿用发送端ObjectFilter@56。004.txt为完整Endian，需逐方法继续核对所有scalar/nullable/inline filter/父层。

## 继续实施

1. 读取003/004/005及必要Helper，不重读整个大反编译；核对enum-source.json及角度/ModuleData弱ID helper，字段和Token生成器须按原类型/偏移绑定。
2. 复用已验收公共Upgrade/Angle/NullableFloat/String/ObjectFilter/弱ID实现；新增完整180字节分派和全部7自身属性、全部公共属性及子节点。
3. 同时覆盖全部枚举、Angle单位/数值边界、普通及可空float、大小写/Unicode弱ID、字符串与filter依赖分配次序、0/1/2/27/28/29/65模块/列表、六容器、缺失/显式空、独立布局/默认/Token/sentinel/拒绝。
4. 整组准备好后统一/Od、/O2、0/1/2/99三缓冲区原DLL逐字节验收；SDK按跨阶段降频策略，不自动触发。
