
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000c91d <.text+0xb91d>:
1000c91d:	55                   	push   ebp
1000c91e:	8b ec                	mov    ebp,esp
1000c920:	51                   	push   ecx
1000c921:	51                   	push   ecx
1000c922:	53                   	push   ebx
1000c923:	56                   	push   esi
1000c924:	8b 75 1c             	mov    esi,DWORD PTR [ebp+0x1c]
1000c927:	f7 de                	neg    esi
1000c929:	1b f6                	sbb    esi,esi
1000c92b:	83 e6 1f             	and    esi,0x1f
1000c92e:	33 db                	xor    ebx,ebx
1000c930:	46                   	inc    esi
1000c931:	83 7d 1c 02          	cmp    DWORD PTR [ebp+0x1c],0x2
1000c935:	57                   	push   edi
1000c936:	0f 95 c3             	setne  bl
1000c939:	8d 45 18             	lea    eax,[ebp+0x18]
1000c93c:	50                   	push   eax
1000c93d:	8d 45 f8             	lea    eax,[ebp-0x8]
1000c940:	50                   	push   eax
1000c941:	ff 75 18             	push   DWORD PTR [ebp+0x18]
1000c944:	c7 45 fc 20 00 00 00 	mov    DWORD PTR [ebp-0x4],0x20
1000c94b:	4b                   	dec    ebx
1000c94c:	83 e3 03             	and    ebx,0x3
1000c94f:	43                   	inc    ebx
1000c950:	e8 6f fe ff ff       	call   0x1000c7c4
1000c955:	83 7d 20 00          	cmp    DWORD PTR [ebp+0x20],0x0
1000c959:	8b 7d 18             	mov    edi,DWORD PTR [ebp+0x18]
1000c95c:	8b 4d f8             	mov    ecx,DWORD PTR [ebp-0x8]
1000c95f:	75 23                	jne    0x1000c984
1000c961:	8b c1                	mov    eax,ecx
1000c963:	0f af c7             	imul   eax,edi
1000c966:	0f af 45 14          	imul   eax,DWORD PTR [ebp+0x14]
1000c96a:	c1 e8 03             	shr    eax,0x3
1000c96d:	89 45 18             	mov    DWORD PTR [ebp+0x18],eax
1000c970:	74 12                	je     0x1000c984
1000c972:	33 d2                	xor    edx,edx
1000c974:	b8 00 01 00 00       	mov    eax,0x100
1000c979:	f7 75 18             	div    DWORD PTR [ebp+0x18]
1000c97c:	83 f8 20             	cmp    eax,0x20
1000c97f:	72 03                	jb     0x1000c984
1000c981:	89 45 fc             	mov    DWORD PTR [ebp-0x4],eax
1000c984:	0f af 4d fc          	imul   ecx,DWORD PTR [ebp-0x4]
1000c988:	0f af fe             	imul   edi,esi
1000c98b:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1000c98e:	8b 30                	mov    esi,DWORD PTR [eax]
1000c990:	8d 74 0e ff          	lea    esi,[esi+ecx*1-0x1]
1000c994:	49                   	dec    ecx
1000c995:	f7 d1                	not    ecx
1000c997:	23 f1                	and    esi,ecx
1000c999:	8b 4d 0c             	mov    ecx,DWORD PTR [ebp+0xc]
1000c99c:	89 30                	mov    DWORD PTR [eax],esi
1000c99e:	8b 31                	mov    esi,DWORD PTR [ecx]
1000c9a0:	8b d7                	mov    edx,edi
1000c9a2:	8d 74 16 ff          	lea    esi,[esi+edx*1-0x1]
1000c9a6:	4a                   	dec    edx
1000c9a7:	f7 d2                	not    edx
1000c9a9:	23 f2                	and    esi,edx
1000c9ab:	89 31                	mov    DWORD PTR [ecx],esi
1000c9ad:	8b 75 10             	mov    esi,DWORD PTR [ebp+0x10]
1000c9b0:	8b 16                	mov    edx,DWORD PTR [esi]
1000c9b2:	8d 54 1a ff          	lea    edx,[edx+ebx*1-0x1]
1000c9b6:	4b                   	dec    ebx
1000c9b7:	f7 d3                	not    ebx
1000c9b9:	23 d3                	and    edx,ebx
1000c9bb:	89 16                	mov    DWORD PTR [esi],edx
1000c9bd:	8b 00                	mov    eax,DWORD PTR [eax]
1000c9bf:	0f af 45 14          	imul   eax,DWORD PTR [ebp+0x14]
1000c9c3:	c1 e8 03             	shr    eax,0x3
1000c9c6:	0f af 01             	imul   eax,DWORD PTR [ecx]
1000c9c9:	83 7d 1c 02          	cmp    DWORD PTR [ebp+0x1c],0x2
1000c9cd:	5f                   	pop    edi
1000c9ce:	5e                   	pop    esi
1000c9cf:	5b                   	pop    ebx
1000c9d0:	74 0f                	je     0x1000c9e1
1000c9d2:	05 ff 0f 00 00       	add    eax,0xfff
1000c9d7:	25 00 f0 ff ff       	and    eax,0xfffff000
1000c9dc:	0f af c2             	imul   eax,edx
1000c9df:	eb 11                	jmp    0x1000c9f2
1000c9e1:	0f af d0             	imul   edx,eax
1000c9e4:	81 c2 ff 0f 00 00    	add    edx,0xfff
1000c9ea:	81 e2 00 f0 ff ff    	and    edx,0xfffff000
1000c9f0:	8b c2                	mov    eax,edx
1000c9f2:	c9                   	leave
1000c9f3:	c2 1c 00             	ret    0x1c
