
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000d7ea <.text+0xc7ea>:
1000d7ea:	55                   	push   ebp
1000d7eb:	8b ec                	mov    ebp,esp
1000d7ed:	51                   	push   ecx
1000d7ee:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1000d7f1:	6a 03                	push   0x3
1000d7f3:	59                   	pop    ecx
1000d7f4:	2b c1                	sub    eax,ecx
1000d7f6:	0f 84 31 01 00 00    	je     0x1000d92d
1000d7fc:	83 e8 0e             	sub    eax,0xe
1000d7ff:	74 26                	je     0x1000d827
1000d801:	48                   	dec    eax
1000d802:	74 1b                	je     0x1000d81f
1000d804:	48                   	dec    eax
1000d805:	0f 84 22 01 00 00    	je     0x1000d92d
1000d80b:	48                   	dec    eax
1000d80c:	0f 85 22 01 00 00    	jne    0x1000d934
1000d812:	83 65 fc 00          	and    DWORD PTR [ebp-0x4],0x0
1000d816:	83 65 24 00          	and    DWORD PTR [ebp+0x24],0x0
1000d81a:	e9 1f 01 00 00       	jmp    0x1000d93e
1000d81f:	89 4d fc             	mov    DWORD PTR [ebp-0x4],ecx
1000d822:	e9 17 01 00 00       	jmp    0x1000d93e
1000d827:	c7 45 fc 02 00 00 00 	mov    DWORD PTR [ebp-0x4],0x2
1000d82e:	8b 45 14             	mov    eax,DWORD PTR [ebp+0x14]
1000d831:	83 7d 18 00          	cmp    DWORD PTR [ebp+0x18],0x0
1000d835:	53                   	push   ebx
1000d836:	56                   	push   esi
1000d837:	57                   	push   edi
1000d838:	8b 7d 10             	mov    edi,DWORD PTR [ebp+0x10]
1000d83b:	75 10                	jne    0x1000d84d
1000d83d:	ff 75 28             	push   DWORD PTR [ebp+0x28]
1000d840:	50                   	push   eax
1000d841:	57                   	push   edi
1000d842:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
1000d845:	e8 27 f0 ff ff       	call   0x1000c871
1000d84a:	89 45 18             	mov    DWORD PTR [ebp+0x18],eax
1000d84d:	8b 5d 24             	mov    ebx,DWORD PTR [ebp+0x24]
1000d850:	83 fb 02             	cmp    ebx,0x2
1000d853:	75 0a                	jne    0x1000d85f
1000d855:	33 c0                	xor    eax,eax
1000d857:	40                   	inc    eax
1000d858:	3b 45 18             	cmp    eax,DWORD PTR [ebp+0x18]
1000d85b:	1b db                	sbb    ebx,ebx
1000d85d:	f7 db                	neg    ebx
1000d85f:	8b 75 20             	mov    esi,DWORD PTR [ebp+0x20]
1000d862:	8d 45 30             	lea    eax,[ebp+0x30]
1000d865:	50                   	push   eax
1000d866:	8d 45 20             	lea    eax,[ebp+0x20]
1000d869:	50                   	push   eax
1000d86a:	8d 45 10             	lea    eax,[ebp+0x10]
1000d86d:	50                   	push   eax
1000d86e:	ff 75 30             	push   DWORD PTR [ebp+0x30]
1000d871:	8b c6                	mov    eax,esi
1000d873:	ff 75 2c             	push   DWORD PTR [ebp+0x2c]
1000d876:	c1 e8 08             	shr    eax,0x8
1000d879:	ff 75 28             	push   DWORD PTR [ebp+0x28]
1000d87c:	83 e0 01             	and    eax,0x1
1000d87f:	53                   	push   ebx
1000d880:	50                   	push   eax
1000d881:	ff 75 fc             	push   DWORD PTR [ebp-0x4]
1000d884:	89 75 24             	mov    DWORD PTR [ebp+0x24],esi
1000d887:	83 65 24 3f          	and    DWORD PTR [ebp+0x24],0x3f
1000d88b:	ff 75 24             	push   DWORD PTR [ebp+0x24]
1000d88e:	ff 75 18             	push   DWORD PTR [ebp+0x18]
1000d891:	ff 75 14             	push   DWORD PTR [ebp+0x14]
1000d894:	57                   	push   edi
1000d895:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
1000d898:	e8 59 f1 ff ff       	call   0x1000c9f6
1000d89d:	f6 45 1c 04          	test   BYTE PTR [ebp+0x1c],0x4
1000d8a1:	b9 03 00 10 00       	mov    ecx,0x100003
1000d8a6:	74 05                	je     0x1000d8ad
1000d8a8:	b9 03 00 30 00       	mov    ecx,0x300003
1000d8ad:	66 f7 45 1c 00 02    	test   WORD PTR [ebp+0x1c],0x200
1000d8b3:	74 06                	je     0x1000d8bb
1000d8b5:	81 c9 00 00 40 00    	or     ecx,0x400000
1000d8bb:	8b 45 34             	mov    eax,DWORD PTR [ebp+0x34]
1000d8be:	83 60 08 00          	and    DWORD PTR [eax+0x8],0x0
1000d8c2:	89 08                	mov    DWORD PTR [eax],ecx
1000d8c4:	b9 00 00 ff ff       	mov    ecx,0xffff0000
1000d8c9:	89 48 14             	mov    DWORD PTR [eax+0x14],ecx
1000d8cc:	89 48 18             	mov    DWORD PTR [eax+0x18],ecx
1000d8cf:	8b 4d fc             	mov    ecx,DWORD PTR [ebp-0x4]
1000d8d2:	8b d1                	mov    edx,ecx
1000d8d4:	c1 e2 09             	shl    edx,0x9
1000d8d7:	33 50 30             	xor    edx,DWORD PTR [eax+0x30]
1000d8da:	c7 40 04 01 00 00 00 	mov    DWORD PTR [eax+0x4],0x1
1000d8e1:	81 e2 00 06 00 00    	and    edx,0x600
1000d8e7:	31 50 30             	xor    DWORD PTR [eax+0x30],edx
1000d8ea:	83 e9 00             	sub    ecx,0x0
1000d8ed:	0f 84 82 00 00 00    	je     0x1000d975
1000d8f3:	49                   	dec    ecx
1000d8f4:	74 0a                	je     0x1000d900
1000d8f6:	49                   	dec    ecx
1000d8f7:	74 4d                	je     0x1000d946
1000d8f9:	49                   	dec    ecx
1000d8fa:	0f 85 8d 00 00 00    	jne    0x1000d98d
1000d900:	8b 55 28             	mov    edx,DWORD PTR [ebp+0x28]
1000d903:	8b 4d 0c             	mov    ecx,DWORD PTR [ebp+0xc]
1000d906:	8d 54 12 01          	lea    edx,[edx+edx*1+0x1]
1000d90a:	2b fa                	sub    edi,edx
1000d90c:	c1 e7 0d             	shl    edi,0xd
1000d90f:	2b ca                	sub    ecx,edx
1000d911:	81 e1 ff 1f 00 00    	and    ecx,0x1fff
1000d917:	81 e7 00 e0 ff 03    	and    edi,0x3ffe000
1000d91d:	0b f9                	or     edi,ecx
1000d91f:	8b 4d 14             	mov    ecx,DWORD PTR [ebp+0x14]
1000d922:	49                   	dec    ecx
1000d923:	c1 e1 1a             	shl    ecx,0x1a
1000d926:	0b f9                	or     edi,ecx
1000d928:	89 78 24             	mov    DWORD PTR [eax+0x24],edi
1000d92b:	eb 60                	jmp    0x1000d98d
1000d92d:	c7 45 fc 01 00 00 00 	mov    DWORD PTR [ebp-0x4],0x1
1000d934:	83 7d 08 11          	cmp    DWORD PTR [ebp+0x8],0x11
1000d938:	0f 84 f0 fe ff ff    	je     0x1000d82e
1000d93e:	33 c0                	xor    eax,eax
1000d940:	40                   	inc    eax
1000d941:	e9 eb fe ff ff       	jmp    0x1000d831
1000d946:	8b 55 28             	mov    edx,DWORD PTR [ebp+0x28]
1000d949:	8b 4d 14             	mov    ecx,DWORD PTR [ebp+0x14]
1000d94c:	8d 54 12 01          	lea    edx,[edx+edx*1+0x1]
1000d950:	2b ca                	sub    ecx,edx
1000d952:	2b fa                	sub    edi,edx
1000d954:	c1 e1 16             	shl    ecx,0x16
1000d957:	c1 e7 0b             	shl    edi,0xb
1000d95a:	81 e1 00 00 c0 ff    	and    ecx,0xffc00000
1000d960:	81 e7 00 f8 3f 00    	and    edi,0x3ff800
1000d966:	0b f9                	or     edi,ecx
1000d968:	8b 4d 0c             	mov    ecx,DWORD PTR [ebp+0xc]
1000d96b:	2b ca                	sub    ecx,edx
1000d96d:	81 e1 ff 07 00 00    	and    ecx,0x7ff
1000d973:	eb b1                	jmp    0x1000d926
1000d975:	8b 55 28             	mov    edx,DWORD PTR [ebp+0x28]
1000d978:	8b 4d 0c             	mov    ecx,DWORD PTR [ebp+0xc]
1000d97b:	8d 54 12 01          	lea    edx,[edx+edx*1+0x1]
1000d97f:	2b ca                	sub    ecx,edx
1000d981:	33 48 24             	xor    ecx,DWORD PTR [eax+0x24]
1000d984:	81 e1 ff ff ff 00    	and    ecx,0xffffff
1000d98a:	31 48 24             	xor    DWORD PTR [eax+0x24],ecx
1000d98d:	6a 03                	push   0x3
1000d98f:	59                   	pop    ecx
1000d990:	8b d6                	mov    edx,esi
1000d992:	c1 fa 0f             	sar    edx,0xf
1000d995:	23 d1                	and    edx,ecx
1000d997:	c1 e2 02             	shl    edx,0x2
1000d99a:	8b fe                	mov    edi,esi
1000d99c:	c1 ff 0d             	sar    edi,0xd
1000d99f:	23 f9                	and    edi,ecx
1000d9a1:	0b d7                	or     edx,edi
1000d9a3:	c1 e2 02             	shl    edx,0x2
1000d9a6:	8b fe                	mov    edi,esi
1000d9a8:	c1 ff 0b             	sar    edi,0xb
1000d9ab:	23 f9                	and    edi,ecx
1000d9ad:	0b d7                	or     edx,edi
1000d9af:	c1 e2 02             	shl    edx,0x2
1000d9b2:	8b fe                	mov    edi,esi
1000d9b4:	c1 ff 09             	sar    edi,0x9
1000d9b7:	23 f9                	and    edi,ecx
1000d9b9:	0b d7                	or     edx,edi
1000d9bb:	8b 7d 10             	mov    edi,DWORD PTR [ebp+0x10]
1000d9be:	81 e7 e0 3f 00 00    	and    edi,0x3fe0
1000d9c4:	c1 e2 02             	shl    edx,0x2
1000d9c7:	8b ce                	mov    ecx,esi
1000d9c9:	c1 f9 08             	sar    ecx,0x8
1000d9cc:	c1 e1 0e             	shl    ecx,0xe
1000d9cf:	0b cf                	or     ecx,edi
1000d9d1:	8b 78 1c             	mov    edi,DWORD PTR [eax+0x1c]
1000d9d4:	c1 e1 11             	shl    ecx,0x11
1000d9d7:	81 e7 00 fc 3f 00    	and    edi,0x3ffc00
1000d9dd:	0b cf                	or     ecx,edi
1000d9df:	81 e1 02 fc ff ff    	and    ecx,0xfffffc02
1000d9e5:	0b d1                	or     edx,ecx
1000d9e7:	8b 4d 18             	mov    ecx,DWORD PTR [ebp+0x18]
1000d9ea:	83 ca 02             	or     edx,0x2
1000d9ed:	49                   	dec    ecx
1000d9ee:	c1 e1 06             	shl    ecx,0x6
1000d9f1:	33 48 2c             	xor    ecx,DWORD PTR [eax+0x2c]
1000d9f4:	89 50 1c             	mov    DWORD PTR [eax+0x1c],edx
1000d9f7:	81 e1 c0 03 00 00    	and    ecx,0x3c0
1000d9fd:	31 48 2c             	xor    DWORD PTR [eax+0x2c],ecx
1000da00:	33 c9                	xor    ecx,ecx
1000da02:	83 7d 08 13          	cmp    DWORD PTR [ebp+0x8],0x13
1000da06:	8b d6                	mov    edx,esi
1000da08:	0f 94 c1             	sete   cl
1000da0b:	c1 fa 06             	sar    edx,0x6
1000da0e:	83 e2 03             	and    edx,0x3
1000da11:	c1 e3 0b             	shl    ebx,0xb
1000da14:	8b fe                	mov    edi,esi
1000da16:	c1 ff 1b             	sar    edi,0x1b
1000da19:	6a 07                	push   0x7
1000da1b:	83 e1 01             	and    ecx,0x1
1000da1e:	c1 e1 04             	shl    ecx,0x4
1000da21:	0b ca                	or     ecx,edx
1000da23:	8b 50 20             	mov    edx,DWORD PTR [eax+0x20]
1000da26:	83 e2 c0             	and    edx,0xffffffc0
1000da29:	0b 55 24             	or     edx,DWORD PTR [ebp+0x24]
1000da2c:	c1 e1 06             	shl    ecx,0x6
1000da2f:	81 e2 3f fb ff ff    	and    edx,0xfffffb3f
1000da35:	0b ca                	or     ecx,edx
1000da37:	8b 55 2c             	mov    edx,DWORD PTR [ebp+0x2c]
1000da3a:	89 48 20             	mov    DWORD PTR [eax+0x20],ecx
1000da3d:	8b 48 30             	mov    ecx,DWORD PTR [eax+0x30]
1000da40:	33 d9                	xor    ebx,ecx
1000da42:	81 e3 00 08 00 00    	and    ebx,0x800
1000da48:	33 d9                	xor    ebx,ecx
1000da4a:	c1 e2 03             	shl    edx,0x3
1000da4d:	89 58 30             	mov    DWORD PTR [eax+0x30],ebx
1000da50:	59                   	pop    ecx
1000da51:	23 f9                	and    edi,ecx
1000da53:	33 fa                	xor    edi,edx
1000da55:	8b 55 28             	mov    edx,DWORD PTR [ebp+0x28]
1000da58:	8b de                	mov    ebx,esi
1000da5a:	c1 fb 1b             	sar    ebx,0x1b
1000da5d:	c1 e2 15             	shl    edx,0x15
1000da60:	23 d9                	and    ebx,ecx
1000da62:	0b da                	or     ebx,edx
1000da64:	81 e7 f8 01 00 00    	and    edi,0x1f8
1000da6a:	33 fb                	xor    edi,ebx
1000da6c:	c1 e7 03             	shl    edi,0x3
1000da6f:	8b d6                	mov    edx,esi
1000da71:	c1 fa 18             	sar    edx,0x18
1000da74:	23 d1                	and    edx,ecx
1000da76:	0b fa                	or     edi,edx
1000da78:	8b d6                	mov    edx,esi
1000da7a:	c1 e7 03             	shl    edi,0x3
1000da7d:	c1 fa 15             	sar    edx,0x15
1000da80:	23 d1                	and    edx,ecx
1000da82:	0b fa                	or     edi,edx
1000da84:	8b d6                	mov    edx,esi
1000da86:	c1 e7 03             	shl    edi,0x3
1000da89:	c1 fa 12             	sar    edx,0x12
1000da8c:	23 d1                	and    edx,ecx
1000da8e:	8b 48 28             	mov    ecx,DWORD PTR [eax+0x28]
1000da91:	0b fa                	or     edi,edx
1000da93:	c1 fe 11             	sar    esi,0x11
1000da96:	03 ff                	add    edi,edi
1000da98:	83 e6 01             	and    esi,0x1
1000da9b:	0b fe                	or     edi,esi
1000da9d:	81 e1 00 00 f8 7f    	and    ecx,0x7ff80000
1000daa3:	0b f9                	or     edi,ecx
1000daa5:	8b 4d 20             	mov    ecx,DWORD PTR [ebp+0x20]
1000daa8:	89 78 28             	mov    DWORD PTR [eax+0x28],edi
1000daab:	8b 45 38             	mov    eax,DWORD PTR [ebp+0x38]
1000daae:	89 08                	mov    DWORD PTR [eax],ecx
1000dab0:	8b 45 3c             	mov    eax,DWORD PTR [ebp+0x3c]
1000dab3:	8b 4d 30             	mov    ecx,DWORD PTR [ebp+0x30]
1000dab6:	5f                   	pop    edi
1000dab7:	5e                   	pop    esi
1000dab8:	89 08                	mov    DWORD PTR [eax],ecx
1000daba:	5b                   	pop    ebx
1000dabb:	c9                   	leave
1000dabc:	c2 38 00             	ret    0x38
