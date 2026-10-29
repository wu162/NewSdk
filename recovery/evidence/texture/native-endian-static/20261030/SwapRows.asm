
SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1003ee8c <.text+0x3de8c>:
1003ee8c:	55                   	push   ebp
1003ee8d:	8b ec                	mov    ebp,esp
1003ee8f:	8b 55 18             	mov    edx,DWORD PTR [ebp+0x18]
1003ee92:	85 d2                	test   edx,edx
1003ee94:	0f b7 45 10          	movzx  eax,WORD PTR [ebp+0x10]
1003ee98:	74 49                	je     0x1003eee3
1003ee9a:	85 c0                	test   eax,eax
1003ee9c:	56                   	push   esi
1003ee9d:	8b 75 08             	mov    esi,DWORD PTR [ebp+0x8]
1003eea0:	57                   	push   edi
1003eea1:	8b 7d 0c             	mov    edi,DWORD PTR [ebp+0xc]
1003eea4:	75 04                	jne    0x1003eeaa
1003eea6:	3b f7                	cmp    esi,edi
1003eea8:	74 37                	je     0x1003eee1
1003eeaa:	8b 4d 14             	mov    ecx,DWORD PTR [ebp+0x14]
1003eead:	8b c6                	mov    eax,esi
1003eeaf:	2b c7                	sub    eax,edi
1003eeb1:	33 c1                	xor    eax,ecx
1003eeb3:	85 c0                	test   eax,eax
1003eeb5:	53                   	push   ebx
1003eeb6:	78 0c                	js     0x1003eec4
1003eeb8:	8d 42 ff             	lea    eax,[edx-0x1]
1003eebb:	0f af c1             	imul   eax,ecx
1003eebe:	03 f0                	add    esi,eax
1003eec0:	03 f8                	add    edi,eax
1003eec2:	f7 d9                	neg    ecx
1003eec4:	85 d2                	test   edx,edx
1003eec6:	8b d9                	mov    ebx,ecx
1003eec8:	76 16                	jbe    0x1003eee0
1003eeca:	89 55 18             	mov    DWORD PTR [ebp+0x18],edx
1003eecd:	ff 75 10             	push   DWORD PTR [ebp+0x10]
1003eed0:	57                   	push   edi
1003eed1:	56                   	push   esi
1003eed2:	e8 66 fe ff ff       	call   0x1003ed3d
1003eed7:	03 f3                	add    esi,ebx
1003eed9:	03 fb                	add    edi,ebx
1003eedb:	ff 4d 18             	dec    DWORD PTR [ebp+0x18]
1003eede:	75 ed                	jne    0x1003eecd
1003eee0:	5b                   	pop    ebx
1003eee1:	5f                   	pop    edi
1003eee2:	5e                   	pop    esi
1003eee3:	5d                   	pop    ebp
1003eee4:	c2 14 00             	ret    0x14
