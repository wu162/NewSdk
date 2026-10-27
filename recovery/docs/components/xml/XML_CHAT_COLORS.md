# OnlineChatColors完整组件

2026-10-25，`chatcolors`单根完整29色入口。最终Windows差分状态由本轮汇总与`evidence/xml-chatcolors/validation.json` sourceCommit/runId锚定；SDK未接入、未执行。

固定Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`、原DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。

原证据：`evidence/xml-source/parts/part-0064.cs.txt` 原行32387完整Marshal起点（跨part0065），`part-0065.cs.txt`原行32833 EndianCorrect；29个原NodeHolder getter/目标uint索引逐对核对，与原XSD AssetTypeOnlineChatColors.xsd一致；原StructLayout120，Plugin非Win32全部大端；TypeId1345252658、TypeHash1378748105。

根reserved@0、29个packedARGB颜色每4字节依次@4..116：Default、CurrentRoom、ChatRoom、Game、GameFull、GameCRCMismatch、PlayerNormal、PlayerBuddy、PlayerOwner、OfflinePlayerBuddy、PlayerSelf、PlayerIgnored、OfflinePlayerIgnored、ChatNormal、ChatEmote、ChatOwner、ChatOwnerEmote、ChatPrivate、ChatPrivateEmote、ChatPrivateOwner、ChatPrivateOwnerEmote、ChatBuddy、ChatSelf、AcceptTrue、AcceptFalse、MapSelected、MapUnselected、MessageOfTheDay、MessageOfTheDayHeading。全部内联，无动态分配/导入/重定位。

节点缺失为0；present-empty为0xff000000；R/G/B缺省0、A缺省255，各VC8 unsigned解析后截low8，打包`A<<24|R<<16|G<<8|B`，大端校正整个uint。节点顺序不影响字段位置，重复singleton显式拒绝，不把XSD required转化为原Marshal必填约束。

`TestXmlchatcolors.ps1` /Od与/O2 × 平台0/1/2/99；每原ProcessInstance显式Initialize平台，恢复ProcessInstance与CompileXml各三个数组完整比较。29节点×4通道完整整数/浮点前缀/溢出/符号/hex/空/垃圾/NUL/Unicode矩阵。独立ABI核120固定大小、缺失全0、reverseXML全29字段精确偏移和ARGB bits、每个present-empty只影响自身slot、unsigned截low8及无imports/relocations。每个颜色的未知/小写通道、重复节点、非法文本/子节点、foreign namespace、qualified通道，以及root边界和空声明/handle不匹配显式拒绝。没有SDK/Tokenizer/预处理路由变化。
