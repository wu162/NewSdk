
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000cd64 <.text+0xbd64>:
1000cd64:	55                   	push   ebp
1000cd65:	8b ec                	mov    ebp,esp
1000cd67:	83 ec 1c             	sub    esp,0x1c
1000cd6a:	83 65 e4 00          	and    DWORD PTR [ebp-0x1c],0x0
1000cd6e:	56                   	push   esi
1000cd6f:	57                   	push   edi
1000cd70:	33 c0                	xor    eax,eax
1000cd72:	8d 7d e8             	lea    edi,[ebp-0x18]
1000cd75:	ab                   	stos   DWORD PTR es:[edi],eax
1000cd76:	ab                   	stos   DWORD PTR es:[edi],eax
1000cd77:	8b 45 20             	mov    eax,DWORD PTR [ebp+0x20]
1000cd7a:	0f b6 0c 45 30 82 09 	movzx  ecx,BYTE PTR [eax*2+0x10098230]
1000cd81:	10 
1000cd82:	89 4d f8             	mov    DWORD PTR [ebp-0x8],ecx
1000cd85:	8d 4d fc             	lea    ecx,[ebp-0x4]
1000cd88:	51                   	push   ecx
1000cd89:	8d 4d 20             	lea    ecx,[ebp+0x20]
1000cd8c:	51                   	push   ecx
1000cd8d:	50                   	push   eax
1000cd8e:	e8 31 fa ff ff       	call   0x1000c7c4
1000cd93:	8b 45 0c             	mov    eax,DWORD PTR [ebp+0xc]
1000cd96:	48                   	dec    eax
1000cd97:	50                   	push   eax
1000cd98:	e8 11 fa ff ff       	call   0x1000c7ae
1000cd9d:	6a 20                	push   0x20
1000cd9f:	5e                   	pop    esi
1000cda0:	2b f0                	sub    esi,eax
1000cda2:	8b 45 10             	mov    eax,DWORD PTR [ebp+0x10]
1000cda5:	48                   	dec    eax
1000cda6:	50                   	push   eax
1000cda7:	e8 02 fa ff ff       	call   0x1000c7ae
1000cdac:	6a 20                	push   0x20
1000cdae:	5f                   	pop    edi
1000cdaf:	2b f8                	sub    edi,eax
1000cdb1:	8b 45 14             	mov    eax,DWORD PTR [ebp+0x14]
1000cdb4:	48                   	dec    eax
1000cdb5:	50                   	push   eax
1000cdb6:	e8 f3 f9 ff ff       	call   0x1000c7ae
1000cdbb:	6a 20                	push   0x20
1000cdbd:	5a                   	pop    edx
1000cdbe:	2b d0                	sub    edx,eax
1000cdc0:	33 c0                	xor    eax,eax
1000cdc2:	40                   	inc    eax
1000cdc3:	8b ce                	mov    ecx,esi
1000cdc5:	d3 e0                	shl    eax,cl
1000cdc7:	8b cf                	mov    ecx,edi
1000cdc9:	89 45 f0             	mov    DWORD PTR [ebp-0x10],eax
1000cdcc:	33 c0                	xor    eax,eax
1000cdce:	40                   	inc    eax
1000cdcf:	d3 e0                	shl    eax,cl
1000cdd1:	3b fe                	cmp    edi,esi
1000cdd3:	8b 7d 08             	mov    edi,DWORD PTR [ebp+0x8]
1000cdd6:	89 45 f4             	mov    DWORD PTR [ebp-0xc],eax
1000cdd9:	1b c0                	sbb    eax,eax
1000cddb:	f7 d8                	neg    eax
1000cddd:	83 ff 03             	cmp    edi,0x3
1000cde0:	73 4f                	jae    0x1000ce31
1000cde2:	6a 10                	push   0x10
1000cde4:	5a                   	pop    edx
1000cde5:	8b cf                	mov    ecx,edi
1000cde7:	d3 fa                	sar    edx,cl
1000cde9:	89 54 85 e4          	mov    DWORD PTR [ebp+eax*4-0x1c],edx
1000cded:	8b 4d ec             	mov    ecx,DWORD PTR [ebp-0x14]
1000cdf0:	8b 45 e4             	mov    eax,DWORD PTR [ebp-0x1c]
1000cdf3:	33 d2                	xor    edx,edx
1000cdf5:	f7 75 20             	div    DWORD PTR [ebp+0x20]
1000cdf8:	8b 55 24             	mov    edx,DWORD PTR [ebp+0x24]
1000cdfb:	8b 75 e8             	mov    esi,DWORD PTR [ebp-0x18]
1000cdfe:	5f                   	pop    edi
1000cdff:	89 02                	mov    DWORD PTR [edx],eax
1000ce01:	8b c6                	mov    eax,esi
1000ce03:	0f af 75 18          	imul   esi,DWORD PTR [ebp+0x18]
1000ce07:	33 d2                	xor    edx,edx
1000ce09:	f7 75 fc             	div    DWORD PTR [ebp-0x4]
1000ce0c:	8b 55 28             	mov    edx,DWORD PTR [ebp+0x28]
1000ce0f:	89 02                	mov    DWORD PTR [edx],eax
1000ce11:	8b 45 2c             	mov    eax,DWORD PTR [ebp+0x2c]
1000ce14:	89 08                	mov    DWORD PTR [eax],ecx
1000ce16:	0f af 4d 1c          	imul   ecx,DWORD PTR [ebp+0x1c]
1000ce1a:	8b 45 20             	mov    eax,DWORD PTR [ebp+0x20]
1000ce1d:	0f af 45 f8          	imul   eax,DWORD PTR [ebp-0x8]
1000ce21:	0f af 45 e4          	imul   eax,DWORD PTR [ebp-0x1c]
1000ce25:	c1 e8 03             	shr    eax,0x3
1000ce28:	03 c6                	add    eax,esi
1000ce2a:	03 c1                	add    eax,ecx
1000ce2c:	5e                   	pop    esi
1000ce2d:	c9                   	leave
1000ce2e:	c2 28 00             	ret    0x28
1000ce31:	83 f0 01             	xor    eax,0x1
1000ce34:	c1 e0 02             	shl    eax,0x2
1000ce37:	8b 74 05 f0          	mov    esi,DWORD PTR [ebp+eax*1-0x10]
1000ce3b:	8d 4f fe             	lea    ecx,[edi-0x2]
1000ce3e:	d3 ee                	shr    esi,cl
1000ce40:	83 fe 04             	cmp    esi,0x4
1000ce43:	89 74 05 e4          	mov    DWORD PTR [ebp+eax*1-0x1c],esi
1000ce47:	73 a4                	jae    0x1000cded
1000ce49:	2b d7                	sub    edx,edi
1000ce4b:	83 fa 01             	cmp    edx,0x1
1000ce4e:	77 03                	ja     0x1000ce53
1000ce50:	33 d2                	xor    edx,edx
1000ce52:	42                   	inc    edx
1000ce53:	8b ca                	mov    ecx,edx
1000ce55:	c1 e1 02             	shl    ecx,0x2
1000ce58:	eb 96                	jmp    0x1000cdf0
