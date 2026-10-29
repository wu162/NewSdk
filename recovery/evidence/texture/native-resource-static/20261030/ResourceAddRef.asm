
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000c599 <.text+0xb599>:
1000c599:	56                   	push   esi
1000c59a:	8b 74 24 08          	mov    esi,DWORD PTR [esp+0x8]
1000c59e:	57                   	push   edi
1000c59f:	8d 46 04             	lea    eax,[esi+0x4]
1000c5a2:	50                   	push   eax
1000c5a3:	ff 15 a0 70 09 10    	call   DWORD PTR ds:0x100970a0
1000c5a9:	8b f8                	mov    edi,eax
1000c5ab:	83 ff 01             	cmp    edi,0x1
1000c5ae:	75 1b                	jne    0x1000c5cb
1000c5b0:	8b 06                	mov    eax,DWORD PTR [esi]
1000c5b2:	8b c8                	mov    ecx,eax
1000c5b4:	80 e1 0f             	and    cl,0xf
1000c5b7:	80 f9 04             	cmp    cl,0x4
1000c5ba:	75 0f                	jne    0x1000c5cb
1000c5bc:	a9 00 00 00 40       	test   eax,0x40000000
1000c5c1:	74 08                	je     0x1000c5cb
1000c5c3:	ff 76 18             	push   DWORD PTR [esi+0x18]
1000c5c6:	e8 ce ff ff ff       	call   0x1000c599
1000c5cb:	8b c7                	mov    eax,edi
1000c5cd:	5f                   	pop    edi
1000c5ce:	5e                   	pop    esi
1000c5cf:	c2 04 00             	ret    0x4
