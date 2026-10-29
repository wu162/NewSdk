
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1001165a <.text+0x1065a>:
1001165a:	a1 04 83 0b 10       	mov    eax,ds:0x100b8304
1001165f:	83 38 00             	cmp    DWORD PTR [eax],0x0
10011662:	56                   	push   esi
10011663:	8b 74 24 08          	mov    esi,DWORD PTR [esp+0x8]
10011667:	75 02                	jne    0x1001166b
10011669:	89 30                	mov    DWORD PTR [eax],esi
1001166b:	8b ce                	mov    ecx,esi
1001166d:	e8 8c ca ff ff       	call   0x1000e0fe
10011672:	83 8e 40 4b 00 00 ff 	or     DWORD PTR [esi+0x4b40],0xffffffff
10011679:	83 8e 44 4b 00 00 ff 	or     DWORD PTR [esi+0x4b44],0xffffffff
10011680:	83 8e 70 31 00 00 ff 	or     DWORD PTR [esi+0x3170],0xffffffff
10011687:	56                   	push   esi
10011688:	e8 ef 07 00 00       	call   0x10011e7c
1001168d:	56                   	push   esi
1001168e:	e8 d5 fd ff ff       	call   0x10011468
10011693:	33 c0                	xor    eax,eax
10011695:	40                   	inc    eax
10011696:	5e                   	pop    esi
10011697:	c2 04 00             	ret    0x4
