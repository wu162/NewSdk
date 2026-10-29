
SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1003a0b3 <.text+0x390b3>:
1003a0b3:	8b 54 24 0c          	mov    edx,DWORD PTR [esp+0xc]
1003a0b7:	8b 44 24 04          	mov    eax,DWORD PTR [esp+0x4]
1003a0bb:	56                   	push   esi
1003a0bc:	33 f6                	xor    esi,esi
1003a0be:	57                   	push   edi
1003a0bf:	83 c2 1c             	add    edx,0x1c
1003a0c2:	8b 4a f0             	mov    ecx,DWORD PTR [edx-0x10]
1003a0c5:	8b 7c 24 10          	mov    edi,DWORD PTR [esp+0x10]
1003a0c9:	8b 3c 8f             	mov    edi,DWORD PTR [edi+ecx*4]
1003a0cc:	23 3a                	and    edi,DWORD PTR [edx]
1003a0ce:	8b 4a 10             	mov    ecx,DWORD PTR [edx+0x10]
1003a0d1:	d3 ef                	shr    edi,cl
1003a0d3:	83 c2 04             	add    edx,0x4
1003a0d6:	89 3c b0             	mov    DWORD PTR [eax+esi*4],edi
1003a0d9:	46                   	inc    esi
1003a0da:	83 fe 04             	cmp    esi,0x4
1003a0dd:	72 e3                	jb     0x1003a0c2
1003a0df:	5f                   	pop    edi
1003a0e0:	5e                   	pop    esi
1003a0e1:	c2 0c 00             	ret    0xc
1003a0e4:	55                   	push   ebp
1003a0e5:	8b ec                	mov    ebp,esp
1003a0e7:	83 ec 50             	sub    esp,0x50
1003a0ea:	8b               	mov    eax,DWORD PTR [ebp+0x8]
