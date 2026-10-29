
SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1003a896 <.text+0x39896>:
1003a896:	53                   	push   ebx
1003a897:	56                   	push   esi
1003a898:	8b 74 24 14          	mov    esi,DWORD PTR [esp+0x14]
1003a89c:	33 db                	xor    ebx,ebx
1003a89e:	57                   	push   edi
1003a89f:	83 c6 1c             	add    esi,0x1c
1003a8a2:	b8 ff 03 00 00       	mov    eax,0x3ff
1003a8a7:	8b 4e f0             	mov    ecx,DWORD PTR [esi-0x10]
1003a8aa:	8b 54 24 14          	mov    edx,DWORD PTR [esp+0x14]
1003a8ae:	0f b7 14 4a          	movzx  edx,WORD PTR [edx+ecx*2]
1003a8b2:	23 16                	and    edx,DWORD PTR [esi]
1003a8b4:	8b 4e 10             	mov    ecx,DWORD PTR [esi+0x10]
1003a8b7:	d3 ea                	shr    edx,cl
1003a8b9:	0f b7 ca             	movzx  ecx,dx
1003a8bc:	8b f9                	mov    edi,ecx
1003a8be:	23 f8                	and    edi,eax
1003a8c0:	66 f7 c2 00 7c       	test   dx,0x7c00
1003a8c5:	74 0a                	je     0x1003a8d1
1003a8c7:	8b d1                	mov    edx,ecx
1003a8c9:	c1 ea 0a             	shr    edx,0xa
1003a8cc:	83 e2 1f             	and    edx,0x1f
1003a8cf:	eb 18                	jmp    0x1003a8e9
1003a8d1:	85 ff                	test   edi,edi
1003a8d3:	74 11                	je     0x1003a8e6
1003a8d5:	33 d2                	xor    edx,edx
1003a8d7:	42                   	inc    edx
1003a8d8:	03 ff                	add    edi,edi
1003a8da:	4a                   	dec    edx
1003a8db:	66 f7 c7 00 04       	test   di,0x400
1003a8e0:	74 f6                	je     0x1003a8d8
1003a8e2:	23 f8                	and    edi,eax
1003a8e4:	eb 03                	jmp    0x1003a8e9
1003a8e6:	6a 90                	push   0xffffff90
1003a8e8:	5a                   	pop    edx
1003a8e9:	81 e1 00 80 ff ff    	and    ecx,0xffff8000
1003a8ef:	c1 e1 03             	shl    ecx,0x3
1003a8f2:	0b cf                	or     ecx,edi
1003a8f4:	83 c2 70             	add    edx,0x70
1003a8f7:	c1 e2 17             	shl    edx,0x17
1003a8fa:	c1 e1 0d             	shl    ecx,0xd
1003a8fd:	0b ca                	or     ecx,edx
1003a8ff:	8b 54 24 10          	mov    edx,DWORD PTR [esp+0x10]
1003a903:	89 0c 9a             	mov    DWORD PTR [edx+ebx*4],ecx
1003a906:	43                   	inc    ebx
1003a907:	83 c6 04             	add    esi,0x4
1003a90a:	83 fb 04             	cmp    ebx,0x4
1003a90d:	72 98                	jb     0x1003a8a7
1003a90f:	5f                   	pop    edi
1003a910:	5e                   	pop    esi
1003a911:	8b c2                	mov    eax,edx
1003a913:	5b                   	pop    ebx
1003a914:	c2 0c 00             	ret    0xc
1003a917:	8b 54 24 0c          	mov    edx,DWORD PTR [esp+0xc]
1003a91b:	53                   	push   ebx
1003a91c:	56                   	push   esi
1003a91d:	33 db                	xor    ebx,ebx
1003a91f:	57                   	push   edi
1003a920:	83 c2 1c             	add    edx,0x1c
1003a923:	b8 ff 03 00 00       	mov    eax,0x3ff
1003a928:	8b 4a f0             	mov    ecx,DWORD PTR [edx-0x10]
1003a92b:	8b 74 24 14          	mov    esi,DWORD PTR [esp+0x14]
1003a92f:	8b 34 8e             	mov    esi,DWORD PTR [esi+ecx*4]
1003a932:	23 32                	and    esi,DWORD PTR [edx]
1003a934:	8b 4a 10             	mov    ecx,DWORD PTR [edx+0x10]
1003a937:	d3 ee                	shr    esi,cl
1003a939:	0f b7 fe             	movzx  edi,si
1003a93c:	0f b7 cf             	movzx  ecx,di
1003a93f:	8b f1                	mov    esi,ecx
1003a941:	23 f0                	and    esi,eax
1003a943:	66 f7 c7 00 7c       	test   di,0x7c00
1003a948:	74 0a                	je     0x1003a954
1003a94a:	8b f9                	mov    edi,ecx
1003a94c:	c1 ef 0a             	shr    edi,0xa
1003a94f:	83 e7 1f             	and    edi,0x1f
1003a952:	eb 18                	jmp    0x1003a96c
1003a954:	85 f6                	test   esi,esi
1003a956:	74 11                	je     0x1003a969
1003a958:	33 ff                	xor    edi,edi
1003a95a:	47                   	inc    edi
1003a95b:	03 f6                	add    esi,esi
1003a95d:	4f                   	dec    edi
1003a95e:	66 f7 c6 00 04       	test   si,0x400
1003a963:	74 f6                	je     0x1003a95b
1003a965:	23 f0                	and    esi,eax
1003a967:	eb 03                	jmp    0x1003a96c
1003a969:	6a 90                	push   0xffffff90
1003a96b:	5f                   	pop    edi
1003a96c:	81 e1 00 80 ff ff    	and    ecx,0xffff8000
1003a972:	c1 e1 03             	shl    ecx,0x3
1003a975:	0b ce                	or     ecx,esi
1003a977:	8b 74 24 10          	mov    esi,DWORD PTR [esp+0x10]
1003a97b:	83 c7 70             	add    edi,0x70
1003a97e:	c1 e1 0d             	shl    ecx,0xd
1003a981:	c1 e7 17             	shl    edi,0x17
1003a984:	0b cf                	or     ecx,edi
1003a986:	89 0c 9e             	mov    DWORD PTR [esi+ebx*4],ecx
1003a989:	43                   	inc    ebx
1003a98a:	83 c2 04             	add    edx,0x4
1003a98d:	83 fb 04             	cmp    ebx,0x4
1003a990:	72 96                	jb     0x1003a928
1003a992:	5f                   	pop    edi
1003a993:	8b c6                	mov    eax,esi
1003a995:	5e                   	pop    esi
1003a996:	5b                   	pop    ebx
1003a997:	c2 0c 00             	ret    0xc
