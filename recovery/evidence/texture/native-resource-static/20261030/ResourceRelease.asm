
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000c5d2 <.text+0xb5d2>:
1000c5d2:	56                   	push   esi
1000c5d3:	8b 74 24 08          	mov    esi,DWORD PTR [esp+0x8]
1000c5d7:	8d 46 04             	lea    eax,[esi+0x4]
1000c5da:	50                   	push   eax
1000c5db:	ff 15 9c 70 09 10    	call   DWORD PTR ds:0x1009709c
1000c5e1:	85 c0                	test   eax,eax
1000c5e3:	75 23                	jne    0x1000c608
1000c5e5:	8b 06                	mov    eax,DWORD PTR [esi]
1000c5e7:	8b c8                	mov    ecx,eax
1000c5e9:	80 e1 0f             	and    cl,0xf
1000c5ec:	80 f9 04             	cmp    cl,0x4
1000c5ef:	75 0f                	jne    0x1000c600
1000c5f1:	a9 00 00 00 40       	test   eax,0x40000000
1000c5f6:	74 08                	je     0x1000c600
1000c5f8:	ff 76 18             	push   DWORD PTR [esi+0x18]
1000c5fb:	e8 d2 ff ff ff       	call   0x1000c5d2
1000c600:	56                   	push   esi
1000c601:	e8 e6 fd ff ff       	call   0x1000c3ec
1000c606:	33 c0                	xor    eax,eax
1000c608:	5e                   	pop    esi
1000c609:	c2 04 00             	ret    0x4
