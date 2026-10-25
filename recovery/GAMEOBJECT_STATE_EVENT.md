# GameObject 状态存储与LUA事件列表

状态：本轮最终完整组件矩阵已验收通过；精确源码/运行/计数见文末。本页不代表完整GameObject、完整替代DLL或SDK集成完成。

原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；23个精确原Marshal/Token/endian/wrapper、父层、Time/char helper与body指纹见 `evidence/gameobject-modules/state-event-source/manifest.json`。`GenerateStateEventFields.py` 再生独立metadata头；两种只需通用字段Marshal/endian，在 `GameObjectModuleDispatchRuntime.h` 直接分派。两ID按全ModuleChoice去重，无新PE表。

| 完整类型 | TypeId | native | kind | Token父 |
|---|---:|---:|---:|---|
| StoreCurrentStateUpdate | 3201389610 | 16 | 154 | Update |
| LUAEventListBehavior | 4201302198 | 16 | 155 | Behavior |

两者Marshal均直接ModuleData，不能按名称或相同native大小合并Token父层。

StoreCurrentState：SleepTime Time float@8默认0s，MaxTime Time float@12默认5s；使用完整原TimeTag单位解析，不是逻辑帧Duration。endian交换两float及ModuleData两字；两个成员Token加父条目后有终止项。

LUAEventList：EventListName为inline String<char>8@8（length@8、数据指针@12），无默认；缺省不分配，显式空分配一个NUL请求并按Tracker对齐为4字节。保留原字节和大小写，不计算hash。原char helper计算strlen，分配length+1再复制NUL。endian只交换length及ModuleData两字，不交换字符或指针。Token是字符串成员bookmark，不构造额外字符串字段表。

两种均无schema子节点；只接受本类型根属性及id/TypeId，未知、重复/外来namespace、结构文本和不适用子节点仍拒绝。高kind输入校验必须早return。测试 `GameObjectStateEventCases.cs` 验证Time边界、字符串缺省/空/NUL/对齐、完整父链和拒绝，最终须完整/Od、/O2、四平台、三缓冲区逐字节与独立布局。


## 本轮最终整批验收

有效验收提交`284599bfd958c0f62d40ce1b72d27dd70a568bcc`；GameObject run **40094269582 success**，同提交基础run **40094269592 success**。/Od、/O2每模式**381140 fixtures、1143420三缓冲区逐字节比较、13884拒绝、1060不变性**；平台0/1/2/99，两模式合计**2286840**次缓冲区比较。实际两模式日志、validation.json、sourceCommit/runId及运行终态均已逐项核对。

本轮实际29新，164→193/322；129种其他已知Marshal继续明确拒绝，58 Token-only未知Marshal回退保留。本轮SDK0次，原路由保留。
