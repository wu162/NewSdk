
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

10016d2a <.text+0x15d2a>:
10016d2a:	56                   	push   esi
10016d2b:	8b f1                	mov    esi,ecx
10016d2d:	8b 46 04             	mov    eax,DWORD PTR [esi+0x4]
10016d30:	85 c0                	test   eax,eax
10016d32:	74 0e                	je     0x10016d42
10016d34:	50                   	push   eax
10016d35:	e8 50 b3 ff ff       	call   0x1001208a
10016d3a:	83 66 04 00          	and    DWORD PTR [esi+0x4],0x0
10016d3e:	83 66 08 00          	and    DWORD PTR [esi+0x8],0x0
10016d42:	8b 06                	mov    eax,DWORD PTR [esi]
10016d44:	83 f8 ff             	cmp    eax,0xffffffff
10016d47:	74 0a                	je     0x10016d53
10016d49:	50                   	push   eax
10016d4a:	ff 15 2c 70 09 10    	call   DWORD PTR ds:0x1009702c
10016d50:	83 0e ff             	or     DWORD PTR [esi],0xffffffff
10016d53:	33 c0                	xor    eax,eax
10016d55:	5e                   	pop    esi
10016d56:	c3                   	ret
10016d57:	83 39 ff             	cmp    DWORD PTR [ecx],0xffffffff
10016d5a:	74 05                	je     0x10016d61
10016d5c:	e9 c9 ff ff ff       	jmp    0x10016d2a
10016d61:	c3                   	ret
