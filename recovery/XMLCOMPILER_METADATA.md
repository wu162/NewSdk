# Original XmlCompiler plugin metadata

Evidence: original Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`, methods at source lines 38771, 38776 and 38791. Core ExtendedTypeInformation blob `79159bf28b844f8728fba643f760cde3e795621e` has plain public fields and no constructor initializers.

`GetAllTypesHash()` returns 1424942948. `GetVersionNumber()` returns 4 for Win32 and unknown platforms, 7 for PlayStation3, 6 for Xbox360. For every listed root, `ProcessingHash = TypeHash ^ GetVersionNumber()`. HasCustomData is explicitly false; Tokenized and UseBuildCache remain their false field defaults. These are XmlCompiler values; Tokenizer metadata must not be substituted.

| TypeName | TypeId | TypeHash |
| --- | ---: | ---: |
| AIStrategicStateDefinition | 607123156 | 3594381428 |
| FXParticleSystemTemplate | 686292351 | 1465656250 |
| AITargetingHeuristic | 926814458 | 2460454737 |
| SpecialPowerTemplate | 2178440954 | 984303644 |
| FXList | 2254974584 | 2601721382 |
| AIMicroManagerData | 2423780215 | 1392447443 |
| AIBudgetStateDefinition | 2893598307 | 2583631670 |
| AIStateDefinition | 3239935619 | 50334453 |
| AttributeModifier | 3319822471 | 4177657499 |
| AIPersonalityDefinition | 3604279694 | 3409747243 |
| UpgradeTemplate | 3786401627 | 1334651241 |
| ObjectCreationList | 3899542881 | 578005457 |
| LogicCommandSet | 3959844197 | 2330506892 |
| LocomotorTemplate | 3972178387 | 3392338269 |

The current XmlPriority plugin explicitly registers all fourteen roots above. The original and recovered plugin metadata passed 800 field/property/hash comparisons on platforms 0/1/2/99 with Initialize/ReInitialize, plus 8 unknown-type rejection checks. SDK source `aac2279c99abf4c55d7f4d682b3e68bbe3a77107`, run 40091218796; see XML_PRIORITY_SDK.md for Reduced and FullMymod acceptance.
