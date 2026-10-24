# BuildingBehavior / StructureUnpack / StructureSell

状态：原方法已核对，本组源码与差分测试尚未验收；不计入已支持模块数量。最终 sourceCommit、runId 和矩阵结果由本轮验收后补充。本页不代表完整 GameObject、插件或 SDK 集成完成。

原始依据：Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`。精确签名、原行号、包含尾 LF 的 body SHA256、字符串 literal 与 31 个模块/必要 helper 方法见 `evidence/gameobject-modules/building-structure-source/manifest.json` 及同目录编号文件。三个类型的 Marshal、Token、endian 和实际依赖已按固定原方法核对，无新枚举表需求。

## 完整类型和布局

| 类型 | TypeId | x86 native |
|---|---:|---:|
| BuildingBehaviorModuleData | 2170196321 | 40 |
| StructureUnpackUpdateModuleData | 2541716369 | 20 |
| StructureSellUpdateModuleData | 1318893585 | 16 |

三者 Marshal 均直接调用 ModuleData；Token 均保留 Update→Contain→Behavior→Module 独立父层，包括名字带 Behavior 的 BuildingBehavior。根结构 @0 为实际 TypeId、@4 为模块 id。

| BuildingBehavior 字段 | offset | 表示 | 默认 |
|---|---:|---|---|
| NightWindowObject | 8 | String<char>8 | 无 |
| FireWindowObject | 16 | String<char>8 | 无 |
| GlowWindowObject | 24 | String<char>8 | 无 |
| NightFireWindowObject | 32 | String<char>8 | 无 |

字符串为内联 length+指针，而非 GameObject 引用或 typed-ID。缺省不分配；显式空字符串仍分配一个 NUL 字节且 length 为 0。分配顺序固定为上表次序，不依赖 XML 属性顺序。

| StructureUnpack 字段 | offset | 表示 | 默认 |
|---|---:|---|---|
| UnpackTime | 8 | Time float | 0s |
| UnpackCompleteSound | 12 | nullable AssetReference<BaseAudioEventInfo> | 无 |
| OffsetHeightAboveWater | 16 | float | 5.0 |

UnpackCompleteSound 属性存在时先分配独立 4 字节值，包括显式空字符串；随后按原 helper 从反斜杠后的数值登记强引用。它不是内联强引用，也不是弱 typed-ID。Token 对存在的 4 字节值登记 bookmark，对缺省指针写空；不构造额外对象 Token 表。

| StructureSell 字段 | offset | 表示 | 默认 |
|---|---:|---|---|
| SellTime | 8 | Time float | 0s |
| ActiveModelCondition | 12 | 单值 ModelConditionFlagType | 无 |

ActiveModelCondition 使用完整 457 名称原表，未知/显式空回退 233、ALL 为 454；它不是模型状态位集。Time 使用原 TimeTag 单位解析，不是逻辑帧 Duration。“无”表示 getter 无 fallback，缺省保留零初始化。

## Token / endian 和验收边界

Building、Unpack、Sell 的有效 Token 条目分别为 5、4、3，另各有终止项；原 name/type hash、顺序及偏移完整记录于证据方法。

Building endian 交换四个字符串的 length 及 ModuleData 两字，不交换字符串字节或指针。Unpack 只交换 Time@8、Float@16 及 ModuleData 两字；可空强引用的指针和值均不交换，这是原方法行为。Sell 交换 Time@8、单枚举@12 及 ModuleData 两字。三者均无 schema 子节点，不能因公共 helper 可复用而放宽嵌套输入。

整组仍需完整 /Od、/O2、平台 0/1/2/99、三缓冲区逐字节差分、独立布局、字符串/引用空值边界及拒绝测试。组件通过后仍须与完整 GameObject、正式插件和 SDK 集成状态分开陈述。

