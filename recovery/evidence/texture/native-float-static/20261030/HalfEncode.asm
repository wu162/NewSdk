
SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1003b057 <.text+0x3a057>:
1003b057:	55                   	push   ebp
1003b058:	8b ec                	mov    ebp,esp
1003b05a:	8b 55 1c             	mov    edx,DWORD PTR [ebp+0x1c]
1003b05d:	53                   	push   ebx
1003b05e:	56                   	push   esi
1003b05f:	57                   	push   edi
1003b060:	33 ff                	xor    edi,edi
1003b062:	83 c2 0c             	add    edx,0xc
1003b065:	d9 44 bd 0c          	fld    DWORD PTR [ebp+edi*4+0xc]
1003b069:	d9 5d 1c             	fstp   DWORD PTR [ebp+0x1c]
1003b06c:	8b 4d 1c             	mov    ecx,DWORD PTR [ebp+0x1c]
1003b06f:	8b c1                	mov    eax,ecx
1003b071:	c1 e9 10             	shr    ecx,0x10
1003b074:	25 ff ff ff 7f       	and    eax,0x7fffffff
1003b079:	81 e1 00 80 00 00    	and    ecx,0x8000
1003b07f:	3d ff ef ff 47       	cmp    eax,0x47ffefff
1003b084:	8b f1                	mov    esi,ecx
1003b086:	76 0a                	jbe    0x1003b092
1003b088:	66 81 ce ff 7f       	or     si,0x7fff
1003b08d:	0f b7 c6             	movzx  eax,si
1003b090:	eb 3b                	jmp    0x1003b0cd
1003b092:	3d 00 00 80 38       	cmp    eax,0x38800000
1003b097:	73 18                	jae    0x1003b0b1
1003b099:	8b d8                	mov    ebx,eax
1003b09b:	6a 71                	push   0x71
1003b09d:	c1 eb 17             	shr    ebx,0x17
1003b0a0:	25 ff ff 7f 00       	and    eax,0x7fffff
1003b0a5:	59                   	pop    ecx
1003b0a6:	2b cb                	sub    ecx,ebx
1003b0a8:	0d 00 00 80 00       	or     eax,0x800000
1003b0ad:	d3 e8                	shr    eax,cl
1003b0af:	eb 05                	jmp    0x1003b0b6
1003b0b1:	2d 00 00 00 38       	sub    eax,0x38000000
1003b0b6:	8b c8                	mov    ecx,eax
1003b0b8:	c1 e9 0d             	shr    ecx,0xd
1003b0bb:	83 e1 01             	and    ecx,0x1
1003b0be:	8d 84 01 ff 0f 00 00 	lea    eax,[ecx+eax*1+0xfff]
1003b0c5:	c1 e8 0d             	shr    eax,0xd
1003b0c8:	0b c6                	or     eax,esi
1003b0ca:	0f b7 c0             	movzx  eax,ax
1003b0cd:	8b 0a                	mov    ecx,DWORD PTR [edx]
1003b0cf:	8b 75 08             	mov    esi,DWORD PTR [ebp+0x8]
1003b0d2:	8d 34 4e             	lea    esi,[esi+ecx*2]
1003b0d5:	8b 4a 20             	mov    ecx,DWORD PTR [edx+0x20]
1003b0d8:	d3 e0                	shl    eax,cl
1003b0da:	83 c2 04             	add    edx,0x4
1003b0dd:	66 23 42 0c          	and    ax,WORD PTR [edx+0xc]
1003b0e1:	66 09 06             	or     WORD PTR [esi],ax
1003b0e4:	47                   	inc    edi
1003b0e5:	83 ff 04             	cmp    edi,0x4
1003b0e8:	0f 82 77 ff ff ff    	jb     0x1003b065
1003b0ee:	5f                   	pop    edi
1003b0ef:	5e                   	pop    esi
1003b0f0:	5b                   	pop    ebx
1003b0f1:	5d                   	pop    ebp
1003b0f2:	c2 18 00             	ret    0x18
1003b0f5:	55                   	push   ebp
1003b0f6:	8b ec                	mov    ebp,esp
1003b0f8:	8b 45 1c             	mov    eax,DWORD PTR [ebp+0x1c]
1003b0fb:	53                   	push   ebx
1003b0fc:	56                   	push   esi
1003b0fd:	57                   	push   edi
1003b0fe:	33 ff                	xor    edi,edi
1003b100:	83 c0 0c             	add    eax,0xc
1003b103:	d9 44 bd 0c          	fld    DWORD PTR [ebp+edi*4+0xc]
1003b107:	d9 5d 1c             	fstp   DWORD PTR [ebp+0x1c]
1003b10a:	8b 4d 1c             	mov    ecx,DWORD PTR [ebp+0x1c]
1003b10d:	8b d1                	mov    edx,ecx
1003b10f:	c1 e9 10             	shr    ecx,0x10
1003b112:	81 e2 ff ff ff 7f    	and    edx,0x7fffffff
1003b118:	81 e1 00 80 00 00    	and    ecx,0x8000
1003b11e:	81 fa ff ef ff 47    	cmp    edx,0x47ffefff
1003b124:	8b f1                	mov    esi,ecx
1003b126:	76 0a                	jbe    0x1003b132
1003b128:	66 81 ce ff 7f       	or     si,0x7fff
1003b12d:	0f b7 ce             	movzx  ecx,si
1003b130:	eb 3f                	jmp    0x1003b171
1003b132:	81 fa 00 00 80 38    	cmp    edx,0x38800000
1003b138:	73 1a                	jae    0x1003b154
1003b13a:	8b da                	mov    ebx,edx
1003b13c:	6a 71                	push   0x71
1003b13e:	c1 eb 17             	shr    ebx,0x17
1003b141:	81 e2 ff ff 7f 00    	and    edx,0x7fffff
1003b147:	59                   	pop    ecx
1003b148:	2b cb                	sub    ecx,ebx
1003b14a:	81 ca 00 00 80 00    	or     edx,0x800000
1003b150:	d3 ea                	shr    edx,cl
1003b152:	eb 06                	jmp    0x1003b15a
1003b154:	81 ea 00 00 00 38    	sub    edx,0x38000000
1003b15a:	8b ca                	mov    ecx,edx
1003b15c:	c1 e9 0d             	shr    ecx,0xd
1003b15f:	83 e1 01             	and    ecx,0x1
1003b162:	8d 8c 11 ff 0f 00 00 	lea    ecx,[ecx+edx*1+0xfff]
1003b169:	c1 e9 0d             	shr    ecx,0xd
1003b16c:	0b ce                	or     ecx,esi
1003b16e:	0f b7 c9             	movzx  ecx,cx
1003b171:	8b 10                	mov    edx,DWORD PTR [eax]
1003b173:	8b 75 08             	mov    esi,DWORD PTR [ebp+0x8]
1003b176:	8d 14 96             	lea    edx,[esi+edx*4]
1003b179:	0f b7 f1             	movzx  esi,cx
1003b17c:	8b 48 20             	mov    ecx,DWORD PTR [eax+0x20]
1003b17f:	d3 e6                	shl    esi,cl
1003b181:	83 c0 04             	add    eax,0x4
1003b184:	23 70 0c             	and    esi,DWORD PTR [eax+0xc]
1003b187:	09 32                	or     DWORD PTR [edx],esi
1003b189:	47                   	inc    edi
1003b18a:	83 ff 04             	cmp    edi,0x4
1003b18d:	0f 82 70 ff ff ff    	jb     0x1003b103
1003b193:	5f                   	pop    edi
1003b194:	5e                   	pop    esi
1003b195:	5b                   	pop    ebx
1003b196:	5d                   	pop    ebp
1003b197:	c2 18 00             	ret    0x18
