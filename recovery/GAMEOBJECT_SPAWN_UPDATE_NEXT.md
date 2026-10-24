# 下一完整能力组：Spawn/OCL Update（仍拒绝）

恢复前取证，尚未实现或验收。原Tokenizer blob6e40a9ed6c9f961d294d49768d911e6a6f354619的61个精确方法已在本地提取并逐项对照原完整blob，位于evidence/gameobject-modules/spawn-update-source；enum-status.json明确列出PE表待取证项，不得猜测数值或把部分表当完整证据。

| 完整组 | TypeId | native |
|---|---:|---:|
| SpawnBehavior | 227633887 | 92 |
| OCLUpdate | 3098903621 | 28 |
| OCLMonitorUpdate | 4047873639 | 40 |

先读manifest.signature/bodySha；不能靠文件编号或nativeSize推测语义。Spawn包含完整数量/时延/布尔、DamageTypesToPropagateToSlaves、Die和SpawnTemplate；OCLUpdate包含强引用OCL、Min/MaxDelay、CreateAtEdge、signed Amount；OCLMonitor包含Options、RequiredMasterObjectStatus及实际父/依赖契约。仍须完整核对Marshal、Token、endian和依赖，不以此摘要代替原方法。

下一步先取证DamageType、DeathType、OCLMonitorOption的原PE表，并核对所有非显式cast调用的typed helper是否需要手动加入证据（本轮HintObjectMapping endian就是这种情况）。可在下一次已准备好的相关代码批次中附带只读PE探针，在既有TestGameObject.ps1作业内输出/保存，不必修改带自动推送的云端工作流。既有InspectAssistEnums.py可作只读探针参考，但其当前只包含AssistanceHintType/WeaponSlotType。

三模块及全部必要依赖按整组实现和测试，统一/Od、/O2、平台0/1/2/99、三缓冲区逐字节差分、独立布局和拒绝测试；完整GameObject/SDK仍未完成，原路由保留。不要把原方法提取或PE探针成功计为模块支持。
