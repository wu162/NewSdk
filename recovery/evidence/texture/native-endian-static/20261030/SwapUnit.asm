
SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1003ed3d <.text+0x3dd3d>:
1003ed3d:	55                   	push   ebp
1003ed3e:	8b ec                	mov    ebp,esp
1003ed40:	51                   	push   ecx
1003ed41:	51                   	push   ecx
1003ed42:	53                   	push   ebx
1003ed43:	57                   	push   edi
1003ed44:	8b 7d 10             	mov    edi,DWORD PTR [ebp+0x10]
1003ed47:	81 ff 01 00 04 00    	cmp    edi,0x40001
1003ed4d:	75 33                	jne    0x1003ed82
1003ed4f:	8b 45 0c             	mov    eax,DWORD PTR [ebp+0xc]
1003ed52:	8b 00                	mov    eax,DWORD PTR [eax]
1003ed54:	8b c8                	mov    ecx,eax
1003ed56:	8b d0                	mov    edx,eax
1003ed58:	c1 ea 10             	shr    edx,0x10
1003ed5b:	81 e1 00 00 ff 00    	and    ecx,0xff0000
1003ed61:	0b ca                	or     ecx,edx
1003ed63:	8b d0                	mov    edx,eax
1003ed65:	81 e2 00 ff 00 00    	and    edx,0xff00
1003ed6b:	c1 e0 10             	shl    eax,0x10
1003ed6e:	0b d0                	or     edx,eax
1003ed70:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1003ed73:	c1 e9 08             	shr    ecx,0x8
1003ed76:	c1 e2 08             	shl    edx,0x8
1003ed79:	0b ca                	or     ecx,edx
1003ed7b:	89 08                	mov    DWORD PTR [eax],ecx
1003ed7d:	e9 ff 00 00 00       	jmp    0x1003ee81
1003ed82:	81 ff 01 00 02 00    	cmp    edi,0x20001
1003ed88:	75 15                	jne    0x1003ed9f
1003ed8a:	8b 45 0c             	mov    eax,DWORD PTR [ebp+0xc]
1003ed8d:	66 8b 00             	mov    ax,WORD PTR [eax]
1003ed90:	8b 4d 08             	mov    ecx,DWORD PTR [ebp+0x8]
1003ed93:	66 c1 c0 08          	rol    ax,0x8
1003ed97:	66 89 01             	mov    WORD PTR [ecx],ax
1003ed9a:	e9 e2 00 00 00       	jmp    0x1003ee81
1003ed9f:	81 ff 02 00 04 00    	cmp    edi,0x40002
1003eda5:	75 12                	jne    0x1003edb9
1003eda7:	8b 45 0c             	mov    eax,DWORD PTR [ebp+0xc]
1003edaa:	8b 00                	mov    eax,DWORD PTR [eax]
1003edac:	8b 4d 08             	mov    ecx,DWORD PTR [ebp+0x8]
1003edaf:	c1 c0 10             	rol    eax,0x10
1003edb2:	89 01                	mov    DWORD PTR [ecx],eax
1003edb4:	e9 c8 00 00 00       	jmp    0x1003ee81
1003edb9:	56                   	push   esi
1003edba:	8b f7                	mov    esi,edi
1003edbc:	c1 ee 10             	shr    esi,0x10
1003edbf:	81 e7 ff ff 00 00    	and    edi,0xffff
1003edc5:	0f 84 a2 00 00 00    	je     0x1003ee6d
1003edcb:	3b fe                	cmp    edi,esi
1003edcd:	0f 84 9a 00 00 00    	je     0x1003ee6d
1003edd3:	8d 47 ff             	lea    eax,[edi-0x1]
1003edd6:	50                   	push   eax
1003edd7:	e8 d2 d9 fc ff       	call   0x1000c7ae
1003eddc:	8b 55 08             	mov    edx,DWORD PTR [ebp+0x8]
1003eddf:	6a 20                	push   0x20
1003ede1:	59                   	pop    ecx
1003ede2:	2b c8                	sub    ecx,eax
1003ede4:	d3 ee                	shr    esi,cl
1003ede6:	3b 55 0c             	cmp    edx,DWORD PTR [ebp+0xc]
1003ede9:	8b c6                	mov    eax,esi
1003edeb:	74 34                	je     0x1003ee21
1003eded:	85 c0                	test   eax,eax
1003edef:	0f 86 8b 00 00 00    	jbe    0x1003ee80
1003edf5:	8d 70 ff             	lea    esi,[eax-0x1]
1003edf8:	0f af f7             	imul   esi,edi
1003edfb:	8b df                	mov    ebx,edi
1003edfd:	f7 db                	neg    ebx
1003edff:	03 75 0c             	add    esi,DWORD PTR [ebp+0xc]
1003ee02:	89 55 08             	mov    DWORD PTR [ebp+0x8],edx
1003ee05:	89 45 0c             	mov    DWORD PTR [ebp+0xc],eax
1003ee08:	57                   	push   edi
1003ee09:	56                   	push   esi
1003ee0a:	ff 75 08             	push   DWORD PTR [ebp+0x8]
1003ee0d:	e8 34 f9 00 00       	call   0x1004e746
1003ee12:	01 7d 08             	add    DWORD PTR [ebp+0x8],edi
1003ee15:	83 c4 0c             	add    esp,0xc
1003ee18:	03 f3                	add    esi,ebx
1003ee1a:	ff 4d 0c             	dec    DWORD PTR [ebp+0xc]
1003ee1d:	75 e9                	jne    0x1003ee08
1003ee1f:	eb 5f                	jmp    0x1003ee80
1003ee21:	8d 48 01             	lea    ecx,[eax+0x1]
1003ee24:	d1 e9                	shr    ecx,1
1003ee26:	74 58                	je     0x1003ee80
1003ee28:	89 7d 10             	mov    DWORD PTR [ebp+0x10],edi
1003ee2b:	f7 5d 10             	neg    DWORD PTR [ebp+0x10]
1003ee2e:	48                   	dec    eax
1003ee2f:	0f af c7             	imul   eax,edi
1003ee32:	03 45 0c             	add    eax,DWORD PTR [ebp+0xc]
1003ee35:	89 55 08             	mov    DWORD PTR [ebp+0x8],edx
1003ee38:	89 4d fc             	mov    DWORD PTR [ebp-0x4],ecx
1003ee3b:	85 ff                	test   edi,edi
1003ee3d:	76 21                	jbe    0x1003ee60
1003ee3f:	8b 55 08             	mov    edx,DWORD PTR [ebp+0x8]
1003ee42:	2b d0                	sub    edx,eax
1003ee44:	8b c8                	mov    ecx,eax
1003ee46:	89 55 f8             	mov    DWORD PTR [ebp-0x8],edx
1003ee49:	89 7d 0c             	mov    DWORD PTR [ebp+0xc],edi
1003ee4c:	8b 55 f8             	mov    edx,DWORD PTR [ebp-0x8]
1003ee4f:	8a 19                	mov    bl,BYTE PTR [ecx]
1003ee51:	8d 34 0a             	lea    esi,[edx+ecx*1]
1003ee54:	8a 16                	mov    dl,BYTE PTR [esi]
1003ee56:	88 1e                	mov    BYTE PTR [esi],bl
1003ee58:	88 11                	mov    BYTE PTR [ecx],dl
1003ee5a:	41                   	inc    ecx
1003ee5b:	ff 4d 0c             	dec    DWORD PTR [ebp+0xc]
1003ee5e:	75 ec                	jne    0x1003ee4c
1003ee60:	01 7d 08             	add    DWORD PTR [ebp+0x8],edi
1003ee63:	03 45 10             	add    eax,DWORD PTR [ebp+0x10]
1003ee66:	ff 4d fc             	dec    DWORD PTR [ebp-0x4]
1003ee69:	75 d0                	jne    0x1003ee3b
1003ee6b:	eb 13                	jmp    0x1003ee80
1003ee6d:	85 f6                	test   esi,esi
1003ee6f:	74 0f                	je     0x1003ee80
1003ee71:	56                   	push   esi
1003ee72:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
1003ee75:	ff 75 08             	push   DWORD PTR [ebp+0x8]
1003ee78:	e8 c9 f8 00 00       	call   0x1004e746
1003ee7d:	83 c4 0c             	add    esp,0xc
1003ee80:	5e                   	pop    esi
1003ee81:	5f                   	pop    edi
1003ee82:	5b                   	pop    ebx
1003ee83:	c9                   	leave
1003ee84:	c2 0c 00             	ret    0xc
