
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000e097 <.text+0xd097>:
1000e097:	56                   	push   esi
1000e098:	57                   	push   edi
1000e099:	8b 7c 24 20          	mov    edi,DWORD PTR [esp+0x20]
1000e09d:	83 27 00             	and    DWORD PTR [edi],0x0
1000e0a0:	68 80 00 00 00       	push   0x80
1000e0a5:	68 00 55 00 00       	push   0x5500
1000e0aa:	e8 bf ff ff ff       	call   0x1000e06e
1000e0af:	8b f0                	mov    esi,eax
1000e0b1:	85 f6                	test   esi,esi
1000e0b3:	75 07                	jne    0x1000e0bc
1000e0b5:	b8 0e 00 07 80       	mov    eax,0x8007000e
1000e0ba:	eb 3d                	jmp    0x1000e0f9
1000e0bc:	56                   	push   esi
1000e0bd:	e8 3c 35 00 00       	call   0x100115fe
1000e0c2:	85 c0                	test   eax,eax
1000e0c4:	75 08                	jne    0x1000e0ce
1000e0c6:	56                   	push   esi
1000e0c7:	e8 aa fe ff ff       	call   0x1000df76
1000e0cc:	eb e7                	jmp    0x1000e0b5
1000e0ce:	83 7c 24 10 02       	cmp    DWORD PTR [esp+0x10],0x2
1000e0d3:	75 20                	jne    0x1000e0f5
1000e0d5:	80 8e b0 2a 00 00 01 	or     BYTE PTR [esi+0x2ab0],0x1
1000e0dc:	8b ce                	mov    ecx,esi
1000e0de:	e8 1b 00 00 00       	call   0x1000e0fe
1000e0e3:	56                   	push   esi
1000e0e4:	e8 71 35 00 00       	call   0x1001165a
1000e0e9:	85 c0                	test   eax,eax
1000e0eb:	75 08                	jne    0x1000e0f5
1000e0ed:	56                   	push   esi
1000e0ee:	e8 97 fe ff ff       	call   0x1000df8a
1000e0f3:	eb c0                	jmp    0x1000e0b5
1000e0f5:	89 37                	mov    DWORD PTR [edi],esi
1000e0f7:	33 c0                	xor    eax,eax
1000e0f9:	5f                   	pop    edi
1000e0fa:	5e                   	pop    esi
1000e0fb:	c2 18 00             	ret    0x18
