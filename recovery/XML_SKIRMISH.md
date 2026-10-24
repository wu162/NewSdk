# Game stats and skirmish settings

Two additional complete roots extend the players bridge; these remain pending unified Windows acceptance.

| Root | TypeId | Type hash | Root bytes |
| --- | --- | --- | --- |
| GameStatsModeData | 3249689110 | 4003247819 | 16 |
| SkirmishSettings | 402082255 | 3814569307 | 20 |

Evidence: original GameStatsModeData plugin wrapper (8676), Tracker (95887), and Marshal/Endian string-list helpers; SkirmishSettings Marshal (52750), BuildSpeed Marshal (52736), wrapper (27500), and Tracker (111721). Original plugin and Module anchors remain those in XML_PLAYERS.md.

GameStatsModeData promotes the restored nested 16-byte layout to its standalone root. SkirmishSettings stores LoadAfter at 4, RandomCrateSpawner weak hash at 8, and the BuildSpeed count/pointer at 12/16. Each BuildSpeed is one raw float. Original weak-hash/list/strong-reference processing allocates only the list; LoadAfter is the sole import. All public fields and dependencies are covered.

XmlplayersSkirmishCases.cs compares all three original-DLL buffers for defaults, all game-mode enums and fallback, map strings, references, weak hashes, float boundaries, Unicode/NUL, metadata, and lists through 257 entries. Independent ABI checks validate string pointers, enum payload, contiguous speed floats, and root import position. Unknown attributes, namespaces, children, and structured scalar leaves remain rejected. TestXmlplayers.ps1 retains /Od and /O2 and platforms 0/1/2/99; partial test inclusion is automatic.

XmlskirmishProbe.xml supplies SkirmishSettings for the full SDK XSD and dual-profile gates. CnC3Types.xsd does not register GameStatsModeData in the global AssetDeclaration choice, despite the original plugin exposing its standalone compiler. Its standalone form is therefore verified by the original-DLL component matrix; SDK coverage uses the existing GameStatsModeData child in MultiplayerSettings. The SDK probe does not alter the original schema to manufacture a standalone root. XmlplayersProbe.xml now supplies AIData's three required LowLodTree AssetId strings; these are strings rather than strong resource imports.
