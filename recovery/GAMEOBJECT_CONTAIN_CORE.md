# Contain核心完整8类（已通过Windows累计验收）

完整实现OpenContain148、Garrison160、HordeGarrison176、Contestable220、Tunnel184、TunnelTeleport184、Heal180、SlaughterHorde324。当前309/322已通过完整累计矩阵。

原证据contain-source覆盖整个13类家族197原方法/11张PE表，证据数量不等于实现数量。尚未实现Engineer、Infiltrator、Transport、HordeTransport、HordeContain；其拒绝测试保留。固定Tokenizer blob与PE哈希见GAMEOBJECT_OBJECT_CONTROL.md。

Open148完整字段、默认和父层；PassengerFilter nullable112、DieMux nullable44、PassengerData List8@112（每项128：String8、signed MaxPassengers、inline Filter112、bool124）、ModifierToGiveOnExit强引用List8@120、MemberTemplateStatusInfo List8@128（每项8：弱GameObject ID、单ObjectStatus枚举）。ObjectStatusWhileContaining默认CONTAINER_OCCUPIED；IgnoreDisabledBits默认UNDERPOWERED EMP；DoorOpenTime为uint。

Garrison自身InitialRoster inline8@148（弱ID、signed Count）后完整Open父层；Mobile156/Immune157无默认，Reset158=false。HordeGarrison ExitDelay160及nullable Vector3@164/168/172先于父层。Contestable两inline状态及EjectSpeed216=1；ForbiddenContainer默认UNDER_IRON_CURTAIN。Tunnel DeleteRemoved180先于弱TunnelMasterObject176再父层；TunnelTeleport原Token别名包装额外父表；Heal TimeForFullHeal176无默认；SlaughterHorde百分比176、inline状态180、FX208及inline Filter112@212先于HordeGarrison父层。

生成器核对完整原字段/default/Token/PE；测试包含全默认/padding、每字段与父层、所有枚举/未知值、nullable和列表边界0/1/2/27/28/29/65、nested字符串/过滤器/引用分配顺序、全Token链key/type/mode/offset、原DLL差分和三缓冲区不变性、拒绝测试。完整/Od/O2、四平台0/1/2/99累计矩阵；无真实SDK运行。

最终验收source9c138e8f182c4dac7c2732c42133e0a789925c01，run40145009538 success；/Od、/O2各1212528 fixtures、3637584三缓冲区比较、23056拒绝、3976不变性，平台0/1/2/99。基础40145009445 success。原失败记录保留；SDK0次。
