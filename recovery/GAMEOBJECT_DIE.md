# 共享 DieMux 与七种 Die 模块

组件已验证：源码379bed90fede179cb9270a1566ce989fe48fb5df；GameObject run40019863862、基础run40019863876均success。每/Od、/O2模式130,596 fixtures、391,788三缓冲区逐字节比较、8,688拒绝、300不变性比较；平台0/1/2/99。两模式合计783,576次缓冲区比较。

SDK未运行，原路由保留，组件不冒充正式GameObject插件。此处只指共享DieModuleData契约的七个完整分派，不代表所有名称含Die/Death的模块（如InitiateRepairDie、SlowDeath等）都已完成。

| 模块 | TypeId | nativeSize | 全部新增根字段/子节点 |
|---|---:|---:|---|
| DieModuleData | 2655988594 | 52 | 共享inline DieMuxData@8；可通过已知容器节点携带该TypeId |
| DestroyDie | 259966977 | 52 | 继承共享契约；Token仍有自身继承层 |
| UpgradeDie | 1359837639 | 56 | UpgradeId@52强资源引用 |
| KeepObjectDie | 3491013058 | 60 | CollapsingTime@52 Time默认0s；StayOnRadar@56=false |
| CreateObjectDie | 996718440 | 72 | CreationList@52强引用；UpgradeRequired@56、UpgradeForbidden@64为inline8字节强引用列表 |
| CrushDie | 2448240860 | 76 | Total/BackEnd/FrontEndCrushSound@52/56/60；对应Percent@64/68/72默认100 |
| RefundDie | 3787260944 | 72 | UpgradeRequired@52强引用；BuildingRequired@56 inline字符串列表；RefundPercent@64=0；可空ObjectFilter@68（112字节） |

共享DieMuxData为inline44字节：VeterancyLevels@0（inline列表、String stride8）；ExemptStatus@8与RequiredStatus@12（可空28字节ObjectStatus）；DamageAmountRequired@16默认-1；Min/MaxKillerAngle@20/24默认1d/-1d；DeathTypes@28和DeathTypesForbidden@36各两word，原34位DeathType位集。

缺少DieMuxData时原Marshal不写默认值，inline44字节保持零；显式<DieMuxData/>才写-1及角度默认。Nullable status缺失与显式空位集保留分配和Token区别。字符串头长度不含结尾零字节，字符分配含零并4字节对齐；原Helper68660确认列表为8字节String，不能误用4字节String*。

分配次序：派生自身字段/列表/过滤器先于基类DieMux。CreateObject的UpgradeRequired/Forbidden是强资源引用并走suffix-index契约，不能复用声音选择器的弱Upgrade hash列表。Refund先BuildingRequired、后ObjectFilter、最后DieMux的等级字符串及Exempt/Required状态分配。

DestroyDie没有独立具体Token重载；原多态helper103932包含两记录（父DieModuleData+sentinel），必须读取此实际方法。不能把它直接alias父表，或将helper内初始对象转换误认为父Token绑定可空指针。GenerateDieFields.py派生此布局并检查原Marshal字段顺序；GameObjectDie.h经过完整--check。原blob固定6e40a9ed6c9f961d294d49768d911e6a6f354619，具体方法/原行号/body SHA256在evidence/gameobject-modules/die-source/manifest.json，按精确类型名索引。

扩展通用ModuleToken时保留既有语义：inline子结构使用自身offset（DieMux@8），inlineString列表按8字节bookmark，强引用列表4字节bookmark，ObjectFilter调用已验证专用Tokenize；nullable结构与nullable scalar保持分离。源方案报告不作为运行支持清单。

完整矩阵保留原88属性/38子节点、Body、ClientUpdate、声音播放、四选择器、六生命周期Update和AI等。新增七类型所有根字段、全部223项ObjectStatus及34项DeathType、ALL/未知/重复排除、空/多项列表、六容器分派、资源边界、ObjectFilter、顺序不变性、拒绝输入、独立缺失/显式空默认、角度、字符串长度与stride、两个nullable状态、折叠继承Token和sentinel断言。失败记录保留：首次为测试宿主变量重名，随后独立String长度断言误含终止符；最终锚点仅指修复后的完整矩阵成功。
