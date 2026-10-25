# 剩余模块清单：317/322已验收，剩余5类

最终source ffd4e0aa440dbb6a8742b283bf0b7c455a50115f，run40160094730 success；/Od与/O2完整累计、平台0/1/2/99、原DLL三缓冲区逐字节差分、拒绝与不变性均已通过。新增完整8类已验收，不含SDK集成；58 Token-only回退保留。

| TypeId | 类型 | native |
|---:|---|---:|
|1121390653|W3DSpotlightDrawModuleData|224|
|1322199627|W3DScriptedModelDrawModuleData|216|
|2323717438|W3DTankDrawModuleData|228|
|2615586235|W3DDynamicsDrawModuleData|252|
|2756454809|W3DTruckDrawModuleData|396|

这5类继续显式拒绝。完整下一组入口、原方法和结构大小见GAMEOBJECT_DRAW_MODEL_NEXT.md；原证据283方法/17张PE表不是实现数量。完整编译器、两替代DLL、正式SDK与完整mymod集成仍是独立验收门槛。
