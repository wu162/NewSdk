
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000c8d8 <.text+0xb8d8>:
1000c8d8:	8b 44 24 04          	mov    eax,DWORD PTR [esp+0x4]
1000c8dc:	53                   	push   ebx
1000c8dd:	56                   	push   esi
1000c8de:	8b 74 24 14          	mov    esi,DWORD PTR [esp+0x14]
1000c8e2:	8d 1c 36             	lea    ebx,[esi+esi*1]
1000c8e5:	2b c3                	sub    eax,ebx
1000c8e7:	57                   	push   edi
1000c8e8:	48                   	dec    eax
1000c8e9:	50                   	push   eax
1000c8ea:	e8 bf fe ff ff       	call   0x1000c7ae
1000c8ef:	8b fe                	mov    edi,esi
1000c8f1:	2b f8                	sub    edi,eax
1000c8f3:	8b 44 24 14          	mov    eax,DWORD PTR [esp+0x14]
1000c8f7:	2b c3                	sub    eax,ebx
1000c8f9:	83 c7 20             	add    edi,0x20
1000c8fc:	48                   	dec    eax
1000c8fd:	50                   	push   eax
1000c8fe:	e8 ab fe ff ff       	call   0x1000c7ae
1000c903:	2b f0                	sub    esi,eax
1000c905:	83 c6 20             	add    esi,0x20
1000c908:	3b fe                	cmp    edi,esi
1000c90a:	7c 02                	jl     0x1000c90e
1000c90c:	8b fe                	mov    edi,esi
1000c90e:	8d 47 fc             	lea    eax,[edi-0x4]
1000c911:	85 c0                	test   eax,eax
1000c913:	5f                   	pop    edi
1000c914:	5e                   	pop    esi
1000c915:	5b                   	pop    ebx
1000c916:	7f 02                	jg     0x1000c91a
1000c918:	33 c0                	xor    eax,eax
1000c91a:	c2 0c 00             	ret    0xc
