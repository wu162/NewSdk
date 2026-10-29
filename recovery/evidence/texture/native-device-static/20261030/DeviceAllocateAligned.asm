
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000e044 <.text+0xd044>:
1000e044:	8b 44 24 04          	mov    eax,DWORD PTR [esp+0x4]
1000e048:	56                   	push   esi
1000e049:	8b 74 24 0c          	mov    esi,DWORD PTR [esp+0xc]
1000e04d:	03 c6                	add    eax,esi
1000e04f:	50                   	push   eax
1000e050:	6a 40                	push   0x40
1000e052:	ff 15 20 70 09 10    	call   DWORD PTR ds:0x10097020
1000e058:	8b c8                	mov    ecx,eax
1000e05a:	85 c9                	test   ecx,ecx
1000e05c:	74 0c                	je     0x1000e06a
1000e05e:	8d 44 31 03          	lea    eax,[ecx+esi*1+0x3]
1000e062:	4e                   	dec    esi
1000e063:	f7 d6                	not    esi
1000e065:	23 c6                	and    eax,esi
1000e067:	89 48 fc             	mov    DWORD PTR [eax-0x4],ecx
1000e06a:	5e                   	pop    esi
1000e06b:	c2 08 00             	ret    0x8
