# ArmorTemplate: XML to binary reconstruction

The supported source is `src/Native/ArmorTemplate.*`, shared `ScalarParsing.*`,
`src/Bridge/XmlNodeAdapter.h` and `src/Bridge/ArmorCompiler.cpp`.
The native serializer uses the already validated `Relo::Tracker`; the bridge
uses the shipped **Core** API types, but neither loads nor calls the original
Tokenizer or XmlCompiler. Original compiler calls exist only in oracle hosts.

`ArmorCompiler.ProcessInstance(InstanceDeclaration, TargetPlatform)` is a
single-type component entry point, not a complete `IAssetBuilderPlugin` DLL.
Other type IDs throw `NotSupportedException` explicitly. This stage does not
replace either file under `SDK/tools`, and does not claim a rebuilt MOD run.

## Original evidence

All tokens below are from the committed original Tokenizer inventory:

| Original method | Metadata token |
|---|---|
| ArmorListType Marshal | `0600002F` |
| ArmorTemplate Marshal / EndianCorrect | `06000030` / `06000031` |
| DamageType / Percentage marshaling | `0600030D` / `0600030E` |
| ArmorListType list Marshal / EndianCorrect | `0600030F` / `06000310` |
| ArmorTemplate root allocation | `060007D5` |
| Node constructor, default constructor | `06000801`, `06000804` |
| Node children / child / attribute / value | `06000805`–`06000808` |
| ArmorListType / ArmorTemplate Tokenize | `06000844` / `06000845` |
| ArmorListType list Tokenize | `06000A42` |
| Plugin HandleType for ArmorTemplate | `06000CE7` |

See `decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs` and `Plugin.cs` for
the actual pointer arithmetic. Struct size metadata alone does not contain
the recovered field layout.

## Serialized layout and ordering

| Root byte offset | Size | Meaning |
|---|---|---|
| 0 | 4 | Reserved zero word |
| 4 | 4 | Default percentage, fallback text `100` |
| 8 | 4 | DamageScalar percentage, fallback `100` |
| 12 | 4 | SideDamageScalar percentage, fallback `100` |
| 16 | 4 | RearDamageScalar percentage, fallback `100` |
| 20 | 4 | FlankedPenalty percentage, fallback `100%` |
| 24 | 4 | Number of Armor child entries |
| 28 | 4 | Relocated pointer to entries, zero for an empty list |

Each entry is `{uint32 Damage, float Percent}`. Missing entry attributes leave
zeroed memory unchanged; an unknown Damage string writes 38 (`ALL`), while
a missing Damage writes zero (`FORCE`). Matching is case sensitive and does
not trim whitespace. All 39 enum values follow the shipped DamageDef schema
order and are checked against the original DLL. Duplicate entries retain XML
order; no sorting or merging is introduced.

Percent parsing first converts the CRT double result to float, then multiplies
by the float constant 0.01 and rounds again. The percent sign is not required,
and trailing text is ignored by the numeric parser. .NET current culture must
not affect this conversion. Shared scalar parsers also preserve base-0 integer
parsing and the original Boolean first-character rule (`1`, `t`, `T`).
The legacy decimal lexer rejects hex-float/INF/NaN input accepted by modern
UCRT, preserves D exponents and signed zero, and leaves suffix text unparsed.
Unsigned overflow saturates magnitude before applying a negative sign, so
`-999999999999999999999999` produces 1, not modern UCRT's FFFFFFFF.

The main procedure table has seven 12-byte entries including its sentinel.
The Armor list has `n+1` entries (one count entry followed by `n` item tables),
with **no extra sentinel** in that list table. Each item table has three
entries, including its sentinel. Thus the token prefix is `96+48*n` bytes,
and the entire InstanceData is `128+56*n` bytes.

The emitted instance concatenates the token prefix and root/list data. Member
offsets include the full token-prefix length. Nonempty data relocations are
`{tokenSize+28, FFFFFFFF}`; an empty list has zero relocation bytes. Token-table
pointer fixups are already encoded in InstanceData; their relocation array is
not appended to the AssetBuffer. ImportsData is always empty on this original
Tokenizer HandleType path.

Win32 uses little endian; every other platform value follows the original big
endian branch. Scalar words and procedure hashes are byte swapped separately
from the pointer/member offsets handled by Tracker. Pointer fixups occur only
after tokenization, preserving host pointers while visiting children.

## XML adapter semantics

Attributes use an unqualified name and retain the difference between a missing
attribute and an existing empty value. `Child` searches the parent's current
namespace; `Children(name)` selects `ea:name` using BAB's namespace manager;
`Children(null)` selects all element children. These are distinct rules.
Navigators are cloned so attribute/child lookups leave the parent position
unchanged. Default child nodes preserve their fallback value.

The adapter uses managed ownership instead of the original native vtable,
gcroot and Release ABI. It does not reproduce the original object-memory ABI.
The serializer expects BAB's already prepared XML: schema validation,
inheritance and expression/reference preprocessing are Core responsibilities,
not implemented again here.

## Validation entry point

Run `./recovery/scripts/TestArmor.ps1` in the x86 Windows developer environment.
`ArmorOracle.cs` calls the actual original Plugin.ProcessInstance and compares
InstanceData, RelocationData and ImportsData byte for byte. Independent layout
assertions check sizes, offsets, list counts, empty imports and relocation
sentinels. `XmlNodeOracle.cs` directly calls original Node/List/Value methods
from each DLL in separate processes. `ScalarOracle.cs` tests shared parsers
against original global IL methods, including integer overflow and numeric
lexical edge cases. The reflected native objects and temporary BAB document
state are oracle-only scaffolding.

Committed results live under `evidence/armor`. Consult `STATUS.md` for the
latest validated commit, comparison counts and Actions run; source milestones
may precede their validation evidence commits.

Tokenizer has an IValue-based `unsigned short` overload; XmlCompiler only
instantiates the INode-based form. The scalar oracle explicitly reports that
coverage boundary instead of using a substitute for the missing function.

An application using the component can serialize a prepared node directly:

```csharp
var document = new System.Xml.XmlDocument();
document.LoadXml("<ArmorTemplate xmlns='uri:ea.com:eala:asset' id='Example' Default='100'><Armor Damage='CANNON' Percent='50'/></ArmorTemplate>");
var namespaces = new System.Xml.XmlNamespaceManager(document.NameTable);
namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
var buffer = Recovered.ArmorCompiler.CompileXml(document.DocumentElement, namespaces, false);
// buffer.InstanceData, buffer.RelocationData, buffer.ImportsData are ready.
```

Build the host for x86, reference the recovered component and the SDK Core
assembly, and use CLR4 legacy mixed-mode activation in the host configuration
when loading the old SDK dependencies. The Actions run publishes the optimized
component separately from the original compiler DLLs.

Remaining work starts with WeaponTemplate's layout, enums, optional blocks,
lists and polymorphic nuggets, then GameObject and XmlCompiler type families.
Full plugin assembly identity/metadata integration and replacement-DLL MOD
acceptance remain separate, uncompleted work.


Final validation: source `33e479960aeb3bfb5b42f1aaab9e8bcac214dbc6`,
[successful Windows run](https://github.com/wu162/NewSdk/actions/runs/39756201609).
ArmorTemplate passes 284 fixtures / 852 buffer comparisons per build mode;
Node passes 720 comparisons; scalars pass 368 per build mode. Both `/Od` and
`/O2` pass. The updated time/angle suite separately passes 41 float-bit checks.
The scope remains a complete single-type component and shared helpers, not
full recovery of both compiler DLLs.
