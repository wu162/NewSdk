# GameObject Spawn/OCL Update 完整能力组

2026-10-24。完整能力组已通过原DLL差分。源码`3f0f0e017380dac81a210a0a8d4055da6d4de5ad`；GameObject run `40088179574 success`，基础 run `40088179650 success`。/Od、/O2每模式328144 fixtures、984432三缓冲区比较、12328拒绝、856不变性，平台0/1/2/99；两模式合计1968864缓冲区比较。validation.json、实际两模式日志、sourceCommit/runId及运行终态已核对。该首组当时155→158；本轮最终总数与拒绝范围见文末及RESUME.md。原路由保留，SDK未运行，不代表完整GameObject或完整替代DLL。

| 类型 | TypeId | native |
|---|---:|---:|
| SpawnBehavior | 227633887 | 92 |
| OCLUpdate | 3098903621 | 28 |
| OCLMonitorUpdate | 4047873639 | 40 |

原Tokenizer `-Module-.cs` blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；61个方法见`evidence/gameobject-modules/spawn-update-source/manifest.json`。逐方法signature/bodySha均已复核，含尾LF。`GenerateSpawnUpdateFields.py --check`核对原blob、61方法、完整属性/默认值顺序和Token表，并检查生成头一致性；本地`--exported-evidence --check`通过。原PE只读表证据由`InspectSpawnUpdateEnums.py`随现有测试作业输出，保留于`gameobject-root/spawn-update-enums.txt`。

Spawn完整继承Upgrade。signed SpawnNumberData@36/InitialBurst@44；三个Time字段@40/@56/@60；DamageType39双word@48，默认ALL全word置FFFFFFFF，未知index38。15个bool连续@76..90，padding@91不写。nullable DieMux44@64，完整Veterancy字符串列表/nullable ObjectStatus/角度/DeathType34双word；未知Death index33。SpawnTemplate为typed GameObject ID列表头@68（非AssetReference），每项4字节小写FastHash；分配顺序为Die、SpawnTemplate列表、Upgrade依赖，不能按XML子节点顺序分配。

OCLUpdate强ObjectCreationList引用@8，MinDelay/MaxDelay@12/@16使用原ceil到逻辑帧的Duration转换，Amount@20为signed默认-1，bool CreateAtEdge@24，其余padding保留零。OCLMonitor Options@8为inline位集，未知index1（mask2），RequiredMasterObjectStatus@12为完整inline28字节。

两个OCL模块的Marshal直接调用ModuleData；Token有独立Update→Contain→Behavior→Module父链，不能根据Marshal或C++继承简化。Spawn Token完整24项、OCL6项、Monitor3项，均保留sentinel；必要DieMux/Upgrade/动画helper和typed局部调用已核对，未发现提取清单遗漏。endian覆盖所有标量、双word位集、nullable结构和列表，指针仍由原Tracker公共实现重定位。

测试入口`GameObjectSpawnUpdateCases.cs`，保留原完整GameObject矩阵。新增全字段、Damage39/Death34全部已知词及独立bit断言、未知/ALL/负词、时间与signed边界、nullable缺省/显式空、列表0/1/2/27/28/29/65/129项、packed bool与padding、原schema分配顺序不变性、六容器、未知属性/节点/命名空间和非法重复拒绝。/Od、/O2、平台0/1/2/99及ProcessInstance三缓冲区逐字节比较不降低。

## 本轮最终整批验收

最终整批源码`922f44602683fc8a048d99fa2eb8ce8887ee0d72`；GameObject run **40089743442 success**，基础run **40089743457 success**。/Od、/O2每模式335812 fixtures、1007436三缓冲区逐字节比较、12596拒绝、892不变性，平台0/1/2/99；两模式合计2014872缓冲区比较。validation.json、实际两模式日志及sourceCommit/runId/终态已逐项核对。

9种实际新增模块与源码拆分均包含在该矩阵中，总数155→164/322，158种其他已知Marshal仍拒绝。SpawnCrate沿用既存kind103，仅强化回归，不重复计数。本轮SDK0次，原路由保留；组件成功不代表完整GameObject、正式插件或SDK集成完成。
