# Public RA3 source fixtures in Upheaval

Input commit: `8d3fe79706c0ef1a2aa995a2fb669e74ebece444`. Validation run: [40181842895](https://github.com/wu162/NewSdk/actions/runs/40181842895). Status: running; no acceptance claim until the receipt is available.

## Sources and scope

- [CptGit/yuris-retaliation](https://github.com/CptGit/yuris-retaliation/tree/7f3fa9b83acffb62d36da5c9710dad78992b3feb/src/Retaliation/Data/Yuri/ChaosDrone): seven complete declaration files, nine assets across AttributeModifier, FXParticleSystemTemplate, FXList, WeaponTemplate, LogicCommand, LogicCommandSet and GameObject. This is the main new fixture group: self-centered gas weapon, damage/attribute/tint/Lua nuggets, GPU particle cylinder, tracked vehicle with laser draw, weapon turret, AI, inherited body and client audio modules.
- [valorad/RA3Afterdash](https://github.com/valorad/RA3Afterdash/tree/bc0adfd047471071159cb7f2c8d12441e40d5f2c/Data/Mod/Common/ObjectFilterAsset.xml): two complete ObjectFilterAsset declarations. Comparison against existing Upheaval shows these two definitions have identical semantics; retain them as explicitly named controls, not additional semantic coverage. The rest of Afterdash largely shares Upheaval definitions; its absent custom media are not imported. Full upstream GPL license is included alongside the fixtures.

`SDK/Mods/Upheaval/Data/Mod.xml` includes `Mod/PublicCases.xml`, which includes the eight fixture XML files. Prefix every imported root ID with PublicYuri_ or PublicAfterdash_ and update the complete local reference graph. Module identities retain upstream spelling. The manifest in PublicCases records upstream commits, original file SHA256, selected IDs and adaptations.

## Dependency closure

The local graph is:

- construction LogicCommand → ChaosDrone GameObject → ChaosDroneCommandSet;
- ChaosDrone → ChaosDroneGasAura WeaponTemplate;
- weapon → AttributeModifier_Hallucinate and FX_ChaosDroneGasRing;
- FX list → ChaosDroneGasRing particle system → stock FXDustCloud texture.

The vehicle's model, moving animation, armor, locomotor, destruction OCL/FX, portrait, button, track textures/terrain mapping and voice/movement sounds resolve through the existing Static/Global/Audio reference manifests. All eleven direct DATA/ART includes resolve to existing SDK files; HeadlightDraw2 XInclude also exists. Laser shader constants FXObeliskLaser and FXLaserOverlay are stock textures. No external DDS/W3X/audio payload is invented or copied.

A schema-directed static scan found 58 outgoing asset-reference occurrences with local or stock name matches, including xas:refType fields for armor, locomotor and animation; details are in `catalog/PublicModCaseDependencies.json`. ModuleData `id` annotations declare identities and are not counted as outgoing links. This name scan supplements the original SDK's authoritative schema, preprocessing, inheritance, include and link validation; it does not replace that validation or claim to be a generic dependency analyzer.

Adaptations: use SDK BaseVehicle through the upstream DATA include rather than importing Yuri's custom BaseVehicle override. Preserve BecomeDissident and ChaosDroneFunctions string fields, but do not import the separate Lua gameplay scripts: stages 3/4/5 do not exercise script execution. This is compiler validation of adapted source fixtures, not an assertion that the entire Yuri mod or its gameplay runs.

## Validation contract

The runner first executes all five original SDK BAB entries for Studio stages 3/4/5. It aborts immediately on nonzero exit, timeout, Error/Critical diagnostics or absent output. Only after this original baseline passes may recovered components be built and the recovered SDK run. Both modes receive identical Upheaval inputs, including the complete existing mod plus these fixtures, in separate clean SDK copies. The repository's production SDK DLLs remain untouched.

Require ten successful invocations and twenty identical bin/manifest/imp/relo pairs. Also require each of the ten new non-Weapon assets to appear in the actual recovered XML/GameObject plugin traces; Weapon has no trace sink and is covered by its registered recovered route and complete SDK output comparison. No unchanged chooser component matrix is rerun. Historical validation anchors remain historical.

Evidence includes sourceCommit, runId/attempt, originalBaselinePassed, each invocation/exit code, copied original/recovered configurations and DLL hashes, complete input hashes, output hashes and plugin-call coverage. Only actual terminal results can establish acceptance.
