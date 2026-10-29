
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000c55c <.text+0xb55c>:
1000c55c:	56                   	push   esi
1000c55d:	8b 74 24 08          	mov    esi,DWORD PTR [esp+0x8]
1000c561:	68 00 ff ff ff       	push   0xffffff00
1000c566:	56                   	push   esi
1000c567:	ff 15 a4 70 09 10    	call   DWORD PTR ds:0x100970a4
1000c56d:	25 00 0f 00 00       	and    eax,0xf00
1000c572:	3d 00 01 00 00       	cmp    eax,0x100
1000c577:	75 1c                	jne    0x1000c595
1000c579:	b8 00 00 ff ff       	mov    eax,0xffff0000
1000c57e:	39 46 14             	cmp    DWORD PTR [esi+0x14],eax
1000c581:	74 03                	je     0x1000c586
1000c583:	89 46 14             	mov    DWORD PTR [esi+0x14],eax
1000c586:	83 7c 24 10 00       	cmp    DWORD PTR [esp+0x10],0x0
1000c58b:	74 08                	je     0x1000c595
1000c58d:	39 46 18             	cmp    DWORD PTR [esi+0x18],eax
1000c590:	74 03                	je     0x1000c595
1000c592:	89 46 18             	mov    DWORD PTR [esi+0x18],eax
1000c595:	5e                   	pop    esi
1000c596:	c2 0c 00             	ret    0xc
