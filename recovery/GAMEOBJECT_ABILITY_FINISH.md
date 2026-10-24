# 完整对象控制与护盾组

2026-10-25实现，待本轮最终Windows完整差分验收，不据此增加已验收计数。

固定原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；原PE SHA256 `ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603`。`ability-finish-source/manifest.json`保存51完整原方法及原PE三表；来源只读取证run40133860981/source3170dcd10e3e77d1822379a31535fd06348a18f2。ExportAbilityFinishMethods.py重取并逐项核对，GenerateAbilityFinishFields.py核对literal/default/Token/父层及PE表。没有仅按名字猜实现。

- GiveUpgradeUpdate TypeId769868147/native256：FX引用240/244、FadeOutSpeed float248默认0.025、DeliverUpgrade bool252默认false；完整SpecialAbility240父层。
- ReplaceObjectUpdate TypeId1442440941/native260：ReplaceRadius float240、ReplaceFX引用244、Scatter bool256无默认；ReplaceObjectNugget List8@248，每项12。每项TargetObjectFilter nullable112指针0，ReplacementObject弱GameObject ID List8@4。列表、逐项过滤器和弱ID列表先于完整SpecialAbility父层分配。
- SpecialDisguiseUpdate TypeId2784675807/native264：TriggerInstantlyOnCreate bool260=false、OpacityTarget float240=0、DisguiseFX引用244、ScanRadiusForPropDisguise float248=0；TerrainData List8@252，每项8：TerrainClass单枚举0，无默认，未知3；ObjectTemplate弱GameObject ID4。TerrainClass原PE11个精确literal，Snow=10。完整SpecialAbility240父层。
- ShieldSphereUpdate TypeId1813100279/native264：**Marshal最后调用ModuleData，Token父表使用UpdateModuleData；不调用SpecialAbility240。** RadiusMax8=40、RadiusMin12=10；DamageTypesNotToAbsorb inline39位@16，未知bit38；ScanFrequency Time24=1s、Duration Time28=10s、MaxDamage float32=0；ObjectStatus inline223@36、ModelCondition inline456@64，默认空；InitiallyActive bool260=false；ShieldBoneName String8@124无默认；ShieldSizeMultiplier float132=1；Options bits1@136，唯一原PE拼写`ALLOW_ALLIES_PROJECTTILE_GOTHROUGH`，未知bit0。随后PositionOffset nullableVector12指针140（Token直接指向值）、ObjectFilter **inline112@144**（缺省节点保零，空节点执行默认）、IgnoreInsideToInsideCheck nullableFilter112指针256。顺序严格为String→Vector→inline Filter内部列表→nullable Filter及内部列表。

测试入口GameObjectAbilityFinishCases.cs：所有根属性、完整父层、默认/空节点、三种列表0/1/2/27/28/29/65边界、nested lists/filter/weak ID、独立原Token键与布局、分配顺序/引用值、枚举全部literal与未知回退、结构顺序不变性、非法属性/外来命名空间/未知及重复节点。原有全部回归、/Od、/O2、平台0/1/2/99与三缓冲区逐字节差分保持。组件验收与SDK集成分开；本轮不改插件路由，不覆盖原DLL。
