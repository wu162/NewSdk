
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000c9f6 <.text+0xb9f6>:
1000c9f6:	55                   	push   ebp
1000c9f7:	8b ec                	mov    ebp,esp
1000c9f9:	33 c0                	xor    eax,eax
1000c9fb:	40                   	inc    eax
1000c9fc:	83 ec 28             	sub    esp,0x28
1000c9ff:	39 45 1c             	cmp    DWORD PTR [ebp+0x1c],eax
1000ca02:	75 78                	jne    0x1000ca7c
1000ca04:	39 45 10             	cmp    DWORD PTR [ebp+0x10],eax
1000ca07:	76 73                	jbe    0x1000ca7c
1000ca09:	89 45 d8             	mov    DWORD PTR [ebp-0x28],eax
1000ca0c:	89 45 f4             	mov    DWORD PTR [ebp-0xc],eax
1000ca0f:	83 65 ec 00          	and    DWORD PTR [ebp-0x14],0x0
1000ca13:	83 7d f4 00          	cmp    DWORD PTR [ebp-0xc],0x0
1000ca17:	75 03                	jne    0x1000ca1c
1000ca19:	8b 45 10             	mov    eax,DWORD PTR [ebp+0x10]
1000ca1c:	53                   	push   ebx
1000ca1d:	8b 5d 08             	mov    ebx,DWORD PTR [ebp+0x8]
1000ca20:	56                   	push   esi
1000ca21:	8b 75 28             	mov    esi,DWORD PTR [ebp+0x28]
1000ca24:	03 f6                	add    esi,esi
1000ca26:	57                   	push   edi
1000ca27:	8b 7d 0c             	mov    edi,DWORD PTR [ebp+0xc]
1000ca2a:	89 45 e8             	mov    DWORD PTR [ebp-0x18],eax
1000ca2d:	2b de                	sub    ebx,esi
1000ca2f:	8d 43 ff             	lea    eax,[ebx-0x1]
1000ca32:	50                   	push   eax
1000ca33:	2b fe                	sub    edi,esi
1000ca35:	e8 74 fd ff ff       	call   0x1000c7ae
1000ca3a:	8b c8                	mov    ecx,eax
1000ca3c:	8b 45 28             	mov    eax,DWORD PTR [ebp+0x28]
1000ca3f:	2b c1                	sub    eax,ecx
1000ca41:	83 c0 20             	add    eax,0x20
1000ca44:	89 45 e0             	mov    DWORD PTR [ebp-0x20],eax
1000ca47:	8d 47 ff             	lea    eax,[edi-0x1]
1000ca4a:	50                   	push   eax
1000ca4b:	e8 5e fd ff ff       	call   0x1000c7ae
1000ca50:	8b c8                	mov    ecx,eax
1000ca52:	8b 45 28             	mov    eax,DWORD PTR [ebp+0x28]
1000ca55:	2b c1                	sub    eax,ecx
1000ca57:	83 c0 20             	add    eax,0x20
1000ca5a:	83 7d 1c 02          	cmp    DWORD PTR [ebp+0x1c],0x2
1000ca5e:	89 45 dc             	mov    DWORD PTR [ebp-0x24],eax
1000ca61:	75 29                	jne    0x1000ca8c
1000ca63:	8b 45 e8             	mov    eax,DWORD PTR [ebp-0x18]
1000ca66:	2b c6                	sub    eax,esi
1000ca68:	89 45 e4             	mov    DWORD PTR [ebp-0x1c],eax
1000ca6b:	48                   	dec    eax
1000ca6c:	50                   	push   eax
1000ca6d:	e8 3c fd ff ff       	call   0x1000c7ae
1000ca72:	8b 75 28             	mov    esi,DWORD PTR [ebp+0x28]
1000ca75:	2b f0                	sub    esi,eax
1000ca77:	83 c6 20             	add    esi,0x20
1000ca7a:	eb 19                	jmp    0x1000ca95
1000ca7c:	83 65 d8 00          	and    DWORD PTR [ebp-0x28],0x0
1000ca80:	83 7d 1c 03          	cmp    DWORD PTR [ebp+0x1c],0x3
1000ca84:	74 86                	je     0x1000ca0c
1000ca86:	83 65 f4 00          	and    DWORD PTR [ebp-0xc],0x0
1000ca8a:	eb 83                	jmp    0x1000ca0f
1000ca8c:	c7 45 e4 01 00 00 00 	mov    DWORD PTR [ebp-0x1c],0x1
1000ca93:	33 f6                	xor    esi,esi
1000ca95:	8b 4d e0             	mov    ecx,DWORD PTR [ebp-0x20]
1000ca98:	33 d2                	xor    edx,edx
1000ca9a:	42                   	inc    edx
1000ca9b:	8b c2                	mov    eax,edx
1000ca9d:	d3 e0                	shl    eax,cl
1000ca9f:	8b 4d dc             	mov    ecx,DWORD PTR [ebp-0x24]
1000caa2:	d3 e2                	shl    edx,cl
1000caa4:	33 c9                	xor    ecx,ecx
1000caa6:	41                   	inc    ecx
1000caa7:	89 4d fc             	mov    DWORD PTR [ebp-0x4],ecx
1000caaa:	8b ce                	mov    ecx,esi
1000caac:	8b 75 fc             	mov    esi,DWORD PTR [ebp-0x4]
1000caaf:	d3 e6                	shl    esi,cl
1000cab1:	83 f8 10             	cmp    eax,0x10
1000cab4:	89 45 f0             	mov    DWORD PTR [ebp-0x10],eax
1000cab7:	89 55 f8             	mov    DWORD PTR [ebp-0x8],edx
1000caba:	89 75 fc             	mov    DWORD PTR [ebp-0x4],esi
1000cabd:	8b 75 30             	mov    esi,DWORD PTR [ebp+0x30]
1000cac0:	76 05                	jbe    0x1000cac7
1000cac2:	83 fa 10             	cmp    edx,0x10
1000cac5:	77 06                	ja     0x1000cacd
1000cac7:	83 7d 24 00          	cmp    DWORD PTR [ebp+0x24],0x0
1000cacb:	75 2c                	jne    0x1000caf9
1000cacd:	85 f6                	test   esi,esi
1000cacf:	75 05                	jne    0x1000cad6
1000cad1:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1000cad4:	eb 14                	jmp    0x1000caea
1000cad6:	8b 4d 18             	mov    ecx,DWORD PTR [ebp+0x18]
1000cad9:	0f b6 0c 4d 30 82 09 	movzx  ecx,BYTE PTR [ecx*2+0x10098230]
1000cae0:	10 
1000cae1:	8b c6                	mov    eax,esi
1000cae3:	c1 e0 03             	shl    eax,0x3
1000cae6:	33 d2                	xor    edx,edx
1000cae8:	f7 f1                	div    ecx
1000caea:	89 45 f0             	mov    DWORD PTR [ebp-0x10],eax
1000caed:	8b 45 0c             	mov    eax,DWORD PTR [ebp+0xc]
1000caf0:	89 45 f8             	mov    DWORD PTR [ebp-0x8],eax
1000caf3:	8b 45 e8             	mov    eax,DWORD PTR [ebp-0x18]
1000caf6:	89 45 fc             	mov    DWORD PTR [ebp-0x4],eax
1000caf9:	ff 75 20             	push   DWORD PTR [ebp+0x20]
1000cafc:	8b 45 18             	mov    eax,DWORD PTR [ebp+0x18]
1000caff:	ff 75 1c             	push   DWORD PTR [ebp+0x1c]
1000cb02:	50                   	push   eax
1000cb03:	0f b6 04 45 30 82 09 	movzx  eax,BYTE PTR [eax*2+0x10098230]
1000cb0a:	10 
1000cb0b:	50                   	push   eax
1000cb0c:	8d 45 fc             	lea    eax,[ebp-0x4]
1000cb0f:	50                   	push   eax
1000cb10:	8d 45 f8             	lea    eax,[ebp-0x8]
1000cb13:	50                   	push   eax
1000cb14:	8d 45 f0             	lea    eax,[ebp-0x10]
1000cb17:	50                   	push   eax
1000cb18:	e8 00 fe ff ff       	call   0x1000c91d
1000cb1d:	85 f6                	test   esi,esi
1000cb1f:	75 14                	jne    0x1000cb35
1000cb21:	8b 45 18             	mov    eax,DWORD PTR [ebp+0x18]
1000cb24:	0f b6 04 45 30 82 09 	movzx  eax,BYTE PTR [eax*2+0x10098230]
1000cb2b:	10 
1000cb2c:	8b 75 f0             	mov    esi,DWORD PTR [ebp-0x10]
1000cb2f:	0f af f0             	imul   esi,eax
1000cb32:	c1 ee 03             	shr    esi,0x3
1000cb35:	33 d2                	xor    edx,edx
1000cb37:	39 55 20             	cmp    DWORD PTR [ebp+0x20],edx
1000cb3a:	75 55                	jne    0x1000cb91
1000cb3c:	39 55 f4             	cmp    DWORD PTR [ebp-0xc],edx
1000cb3f:	75 50                	jne    0x1000cb91
1000cb41:	83 7d 1c 02          	cmp    DWORD PTR [ebp+0x1c],0x2
1000cb45:	74 66                	je     0x1000cbad
1000cb47:	83 7d 1c 01          	cmp    DWORD PTR [ebp+0x1c],0x1
1000cb4b:	75 37                	jne    0x1000cb84
1000cb4d:	83 7d 14 01          	cmp    DWORD PTR [ebp+0x14],0x1
1000cb51:	75 31                	jne    0x1000cb84
1000cb53:	39 55 24             	cmp    DWORD PTR [ebp+0x24],edx
1000cb56:	75 2c                	jne    0x1000cb84
1000cb58:	39 55 28             	cmp    DWORD PTR [ebp+0x28],edx
1000cb5b:	75 27                	jne    0x1000cb84
1000cb5d:	8d 45 1c             	lea    eax,[ebp+0x1c]
1000cb60:	50                   	push   eax
1000cb61:	8d 45 28             	lea    eax,[ebp+0x28]
1000cb64:	50                   	push   eax
1000cb65:	ff 75 18             	push   DWORD PTR [ebp+0x18]
1000cb68:	e8 57 fc ff ff       	call   0x1000c7c4
1000cb6d:	8b 4d 1c             	mov    ecx,DWORD PTR [ebp+0x1c]
1000cb70:	8b 45 0c             	mov    eax,DWORD PTR [ebp+0xc]
1000cb73:	8d 44 01 ff          	lea    eax,[ecx+eax*1-0x1]
1000cb77:	49                   	dec    ecx
1000cb78:	f7 d1                	not    ecx
1000cb7a:	23 c1                	and    eax,ecx
1000cb7c:	0f af c6             	imul   eax,esi
1000cb7f:	e9 3b 01 00 00       	jmp    0x1000ccbf
1000cb84:	0f af 75 fc          	imul   esi,DWORD PTR [ebp-0x4]
1000cb88:	0f af 75 f8          	imul   esi,DWORD PTR [ebp-0x8]
1000cb8c:	89 75 28             	mov    DWORD PTR [ebp+0x28],esi
1000cb8f:	eb 41                	jmp    0x1000cbd2
1000cb91:	83 7d 1c 02          	cmp    DWORD PTR [ebp+0x1c],0x2
1000cb95:	74 16                	je     0x1000cbad
1000cb97:	0f af 75 f8          	imul   esi,DWORD PTR [ebp-0x8]
1000cb9b:	81 c6 ff 0f 00 00    	add    esi,0xfff
1000cba1:	81 e6 00 f0 ff ff    	and    esi,0xfffff000
1000cba7:	0f af 75 fc          	imul   esi,DWORD PTR [ebp-0x4]
1000cbab:	eb 14                	jmp    0x1000cbc1
1000cbad:	0f af 75 fc          	imul   esi,DWORD PTR [ebp-0x4]
1000cbb1:	0f af 75 f8          	imul   esi,DWORD PTR [ebp-0x8]
1000cbb5:	81 c6 ff 0f 00 00    	add    esi,0xfff
1000cbbb:	81 e6 00 f0 ff ff    	and    esi,0xfffff000
1000cbc1:	39 55 f4             	cmp    DWORD PTR [ebp-0xc],edx
1000cbc4:	89 75 28             	mov    DWORD PTR [ebp+0x28],esi
1000cbc7:	74 09                	je     0x1000cbd2
1000cbc9:	8b c6                	mov    eax,esi
1000cbcb:	0f af 45 10          	imul   eax,DWORD PTR [ebp+0x10]
1000cbcf:	89 45 28             	mov    DWORD PTR [ebp+0x28],eax
1000cbd2:	83 7d 14 01          	cmp    DWORD PTR [ebp+0x14],0x1
1000cbd6:	77 09                	ja     0x1000cbe1
1000cbd8:	39 55 14             	cmp    DWORD PTR [ebp+0x14],edx
1000cbdb:	0f 85 db 00 00 00    	jne    0x1000ccbc
1000cbe1:	8b 4d e4             	mov    ecx,DWORD PTR [ebp-0x1c]
1000cbe4:	3b f9                	cmp    edi,ecx
1000cbe6:	8b c7                	mov    eax,edi
1000cbe8:	77 02                	ja     0x1000cbec
1000cbea:	8b c1                	mov    eax,ecx
1000cbec:	3b d8                	cmp    ebx,eax
1000cbee:	76 04                	jbe    0x1000cbf4
1000cbf0:	8b fb                	mov    edi,ebx
1000cbf2:	eb 06                	jmp    0x1000cbfa
1000cbf4:	3b f9                	cmp    edi,ecx
1000cbf6:	77 02                	ja     0x1000cbfa
1000cbf8:	8b f9                	mov    edi,ecx
1000cbfa:	39 55 14             	cmp    DWORD PTR [ebp+0x14],edx
1000cbfd:	75 0e                	jne    0x1000cc0d
1000cbff:	57                   	push   edi
1000cc00:	e8 a9 fb ff ff       	call   0x1000c7ae
1000cc05:	6a 20                	push   0x20
1000cc07:	59                   	pop    ecx
1000cc08:	2b c8                	sub    ecx,eax
1000cc0a:	89 4d 14             	mov    DWORD PTR [ebp+0x14],ecx
1000cc0d:	8b 45 e0             	mov    eax,DWORD PTR [ebp-0x20]
1000cc10:	8b 5d dc             	mov    ebx,DWORD PTR [ebp-0x24]
1000cc13:	8b 7d e8             	mov    edi,DWORD PTR [ebp-0x18]
1000cc16:	6a 0f                	push   0xf
1000cc18:	89 45 0c             	mov    DWORD PTR [ebp+0xc],eax
1000cc1b:	e8 8e fb ff ff       	call   0x1000c7ae
1000cc20:	8b 75 14             	mov    esi,DWORD PTR [ebp+0x14]
1000cc23:	c7 45 08 20 00 00 00 	mov    DWORD PTR [ebp+0x8],0x20
1000cc2a:	29 45 08             	sub    DWORD PTR [ebp+0x8],eax
1000cc2d:	eb 66                	jmp    0x1000cc95
1000cc2f:	83 7d 24 00          	cmp    DWORD PTR [ebp+0x24],0x0
1000cc33:	74 0c                	je     0x1000cc41
1000cc35:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1000cc38:	39 45 0c             	cmp    DWORD PTR [ebp+0xc],eax
1000cc3b:	76 5b                	jbe    0x1000cc98
1000cc3d:	3b d8                	cmp    ebx,eax
1000cc3f:	76 57                	jbe    0x1000cc98
1000cc41:	83 7d 0c 00          	cmp    DWORD PTR [ebp+0xc],0x0
1000cc45:	76 03                	jbe    0x1000cc4a
1000cc47:	ff 4d 0c             	dec    DWORD PTR [ebp+0xc]
1000cc4a:	85 db                	test   ebx,ebx
1000cc4c:	76 01                	jbe    0x1000cc4f
1000cc4e:	4b                   	dec    ebx
1000cc4f:	83 ff 01             	cmp    edi,0x1
1000cc52:	76 02                	jbe    0x1000cc56
1000cc54:	d1 ef                	shr    edi,1
1000cc56:	8b 4d 0c             	mov    ecx,DWORD PTR [ebp+0xc]
1000cc59:	ff 75 20             	push   DWORD PTR [ebp+0x20]
1000cc5c:	33 c0                	xor    eax,eax
1000cc5e:	ff 75 1c             	push   DWORD PTR [ebp+0x1c]
1000cc61:	40                   	inc    eax
1000cc62:	d3 e0                	shl    eax,cl
1000cc64:	8b cb                	mov    ecx,ebx
1000cc66:	89 7d 30             	mov    DWORD PTR [ebp+0x30],edi
1000cc69:	89 45 e0             	mov    DWORD PTR [ebp-0x20],eax
1000cc6c:	33 c0                	xor    eax,eax
1000cc6e:	40                   	inc    eax
1000cc6f:	d3 e0                	shl    eax,cl
1000cc71:	89 45 dc             	mov    DWORD PTR [ebp-0x24],eax
1000cc74:	8b 45 18             	mov    eax,DWORD PTR [ebp+0x18]
1000cc77:	50                   	push   eax
1000cc78:	0f b6 04 45 30 82 09 	movzx  eax,BYTE PTR [eax*2+0x10098230]
1000cc7f:	10 
1000cc80:	50                   	push   eax
1000cc81:	8d 45 30             	lea    eax,[ebp+0x30]
1000cc84:	50                   	push   eax
1000cc85:	8d 45 dc             	lea    eax,[ebp-0x24]
1000cc88:	50                   	push   eax
1000cc89:	8d 45 e0             	lea    eax,[ebp-0x20]
1000cc8c:	50                   	push   eax
1000cc8d:	e8 8b fc ff ff       	call   0x1000c91d
1000cc92:	01 45 ec             	add    DWORD PTR [ebp-0x14],eax
1000cc95:	4e                   	dec    esi
1000cc96:	75 97                	jne    0x1000cc2f
1000cc98:	83 7d f4 00          	cmp    DWORD PTR [ebp-0xc],0x0
1000cc9c:	74 1e                	je     0x1000ccbc
1000cc9e:	8b 45 d8             	mov    eax,DWORD PTR [ebp-0x28]
1000cca1:	8b 4d 10             	mov    ecx,DWORD PTR [ebp+0x10]
1000cca4:	f7 d8                	neg    eax
1000cca6:	1b c0                	sbb    eax,eax
1000cca8:	83 e0 03             	and    eax,0x3
1000ccab:	40                   	inc    eax
1000ccac:	8d 4c 08 ff          	lea    ecx,[eax+ecx*1-0x1]
1000ccb0:	48                   	dec    eax
1000ccb1:	f7 d0                	not    eax
1000ccb3:	23 c8                	and    ecx,eax
1000ccb5:	0f af 4d ec          	imul   ecx,DWORD PTR [ebp-0x14]
1000ccb9:	89 4d ec             	mov    DWORD PTR [ebp-0x14],ecx
1000ccbc:	8b 45 28             	mov    eax,DWORD PTR [ebp+0x28]
1000ccbf:	8b 4d 34             	mov    ecx,DWORD PTR [ebp+0x34]
1000ccc2:	8b 55 f0             	mov    edx,DWORD PTR [ebp-0x10]
1000ccc5:	89 11                	mov    DWORD PTR [ecx],edx
1000ccc7:	8b 4d 38             	mov    ecx,DWORD PTR [ebp+0x38]
1000ccca:	89 01                	mov    DWORD PTR [ecx],eax
1000cccc:	8b 45 3c             	mov    eax,DWORD PTR [ebp+0x3c]
1000cccf:	8b 4d ec             	mov    ecx,DWORD PTR [ebp-0x14]
1000ccd2:	5f                   	pop    edi
1000ccd3:	5e                   	pop    esi
1000ccd4:	89 08                	mov    DWORD PTR [eax],ecx
1000ccd6:	8b 45 14             	mov    eax,DWORD PTR [ebp+0x14]
1000ccd9:	5b                   	pop    ebx
1000ccda:	c9                   	leave
1000ccdb:	c2 38 00             	ret    0x38
