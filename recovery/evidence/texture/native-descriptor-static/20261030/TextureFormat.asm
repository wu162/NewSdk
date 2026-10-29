
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000c83d <.text+0xb83d>:
1000c83d:	8b 54 24 04          	mov    edx,DWORD PTR [esp+0x4]
1000c841:	8b 0a                	mov    ecx,DWORD PTR [edx]
1000c843:	8b 42 0c             	mov    eax,DWORD PTR [edx+0xc]
1000c846:	25 ff 1f 00 00       	and    eax,0x1fff
1000c84b:	56                   	push   esi
1000c84c:	8b f1                	mov    esi,ecx
1000c84e:	c1 e0 0a             	shl    eax,0xa
1000c851:	81 e6 fc 03 00 00    	and    esi,0x3fc
1000c857:	0b c6                	or     eax,esi
1000c859:	c1 e9 17             	shr    ecx,0x17
1000c85c:	c1 e0 07             	shl    eax,0x7
1000c85f:	81 e1 00 01 00 00    	and    ecx,0x100
1000c865:	0b c1                	or     eax,ecx
1000c867:	0f b6 4a 04          	movzx  ecx,BYTE PTR [edx+0x4]
1000c86b:	0b c1                	or     eax,ecx
1000c86d:	5e                   	pop    esi
1000c86e:	c2 04 00             	ret    0x4
