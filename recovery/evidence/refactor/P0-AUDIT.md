# P0 integration audit

Starting source: `87cf54e254dfb44a129da087316fd04806ed2b10`. Scope for this task is **P0–P3** (four phases, numbering starts at zero). The delivery branch is `refactor/modcompiler-p0-p3`; P4 native/router integration and P5 complete MOD execution are outside this task.

`source-inventory.json` freezes the initial production source paths and SHA-256 values. It is a source inventory, not proof that these implementations were loaded. `baseline/<run>-<attempt>/validation.json` separately identifies the tested deployment. No historical receipts are relabelled as validation of the new application.

## Identity and dependency boundaries

| Boundary | Initial behavior | Required integration treatment |
| --- | --- | --- |
| Core → Utility/Hash/Metrics | SDK assembly references in Core csproj | Foundation compiles restored sources together; do not copy old DLLs to satisfy these references |
| Core → MvpXml | Project reference and named embedded resources | Preserve resource names/bytes and EXSLT metadata repairs |
| Core → ExpressionEval | `ExpressionEvaluatorWrapper.LoadAssembly` loads old assembly, then reflects evaluator | Unified source build constructs evaluator directly; retain the separate legacy oracle path |
| Plugins | `PluginDescriptor.Initialize` uses Type.GetType/Activator | Explicit factories in unified production; supported route registration must remain auditable |
| Serialized configuration | `SerializedSectionHandler.Create` resolves assembly-qualified serializedType | Explicit local types/configuration for unified build; do not accidentally load original Core |
| Audio | Root `src/AudioCompiler`, original Core reference | Move after G0; update active script/project/workflow paths, preserve historical receipt anchors |
| Texture | x86 mixed mode; references original Core identity | Validate native writer and real A/B/C processing; stubs and Assembly.Load alone are insufficient |
| Expression grammar | antlr.runtime 2.7.5.2 | Explicit allowed external dependency, separate from restored business code |

Production excludes old GUIBuildOutput, NoScrollRichTextBox, resident BAB, Remote/Remoting and EALAModStudio. ShPath/ShMisc contain UI helper dependencies and need explicit treatment rather than indirectly retaining WinForms. Old projects remain historical/oracle entry points while the new path is validated.

## State and lifecycle findings

| Source owner | Actual state/calls | Integration requirement |
| --- | --- | --- |
| `Settings.Current` | Process-global mutable Settings; SettingsLoader clones configuration and resolves paths relative to DataRoot | Install effective per-stream settings in a scoped owner; restore in finally; SDK and MOD roots must remain independent |
| `DocumentProcessor` | Static missing references, document/stream stacks, timers and type timing dictionary; instance holds cache, schema and plugin registries | Creating another DocumentProcessor does not isolate all state. ResetTimers resets timings only. Prove failure/cancellation cleanup before claiming real multi-stream reuse |
| `PluginDescriptor` | Initialize creates plugin; ReInitialize reuses it; configObject is currently null | Preserve the actual lifecycle, do not optimize away reinitialization without evidence; ensure partial initialization is cleaned |
| `PluginRegistry` | Per-registry maps; ReInitialize invalidates type metadata map | Separate stream metadata and validate duplicate route IDs. AllPlugins is a map ValueCollection, so one plugin can occur repeatedly; version summation currently follows this topology |
| `OutputManager` | Per-output streams plus static `_LinkedBuffer` and `breakHandle` | Sequential stream execution initially; explicit output ownership, no concurrent mutable shared buffers |
| `StringHashBin` | Collision diagnostics depend on Settings.Current cache directory | Per-stream hash table and correct settings throughout lifetime |
| `Tracer` | Static dictionary, verbosity, TraceWrite callback and thread-local indentation | Scope event bridge and restore callbacks; filtering cannot erase compiler failure state |
| `MetricManager` | Static listeners, enabled/session state and descriptor map; OpenSession/CloseSession | Scope session ownership, close on every controlled exit; do not activate SQL listeners implicitly |
| Native/Bridge | Native layouts, relocation and buffer ownership cross managed boundary | P4 must validate actual ownership and final type identities; P2 injected executor tests are not evidence of native cleanup |

## Parameter and input semantics

The attached implementation plan is the source of the CLI contract. `/pc` means UsePrecompiled, `/csc` means CompressSessionCache; neither is a platform or cache master switch. `/sc` and `/bc` remain distinct. Win32 is the only new target. Unsupported options must fail before work rather than be silently discarded.

Content discovery preserves the old script meaning: base=`Data/Mod.xml`; apt=`Data/AptUI/*.xml`; maps=`Data/AdditionalMaps/MapMetaData_*.xml` (not SDK Global.xml). Only selected groups are scanned. A missing base entry is an error; empty optional collections need an explicit skipped state. Source/output/log roots must not overwrite inputs. Plan/session tests must distinguish a controlled injected executor from the future real engine.

## Gate policy

G0 uses fixed pre-refactor source and fixed MOD input, original SDK first and recovered deployment second, with identical absolute working paths. Four stream outputs and CustomData are compared. Restored Utility/Hash/Metrics are **not** silently credited to that old deployment.

G1 requires real cross-language types, resources and metadata signatures in the final EXE, with intermediate modules absent at runtime. G2 requires original/roundtrip/redirected real texture processing and cleanup. Failure is recorded as failure, not a justification to mark P1 complete. G3 foundation tests establish CLI/state/event behavior; full real MOD equivalence is P5 and cannot be inferred from test doubles.
