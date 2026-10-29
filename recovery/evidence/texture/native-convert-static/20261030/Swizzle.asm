
SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1003a22d <.text+0x3922d>:
1003a22d:	8b 4c 24 08          	mov    ecx,DWORD PTR [esp+0x8]
1003a231:	56                   	push   esi
1003a232:	8b c1                	mov    eax,ecx
1003a234:	6a 07                	push   0x7
1003a236:	5a                   	pop    edx
1003a237:	c1 f8 12             	sar    eax,0x12
1003a23a:	23 c2                	and    eax,edx
1003a23c:	8b 34 85 90 12 0a 10 	mov    esi,DWORD PTR [eax*4+0x100a1290]
1003a243:	8b 44 24 08          	mov    eax,DWORD PTR [esp+0x8]
1003a247:	89 30                	mov    DWORD PTR [eax],esi
1003a249:	8b f1                	mov    esi,ecx
1003a24b:	c1 fe 15             	sar    esi,0x15
1003a24e:	23 f2                	and    esi,edx
1003a250:	8b 34 b5 90 12 0a 10 	mov    esi,DWORD PTR [esi*4+0x100a1290]
1003a257:	89 70 04             	mov    DWORD PTR [eax+0x4],esi
1003a25a:	8b f1                	mov    esi,ecx
1003a25c:	c1 fe 18             	sar    esi,0x18
1003a25f:	c1 f9 1b             	sar    ecx,0x1b
1003a262:	23 f2                	and    esi,edx
1003a264:	8b 34 b5 90 12 0a 10 	mov    esi,DWORD PTR [esi*4+0x100a1290]
1003a26b:	23 ca                	and    ecx,edx
1003a26d:	8b 0c 8d 90 12 0a 10 	mov    ecx,DWORD PTR [ecx*4+0x100a1290]
1003a274:	89 70 08             	mov    DWORD PTR [eax+0x8],esi
1003a277:	89 48 0c             	mov    DWORD PTR [eax+0xc],ecx
1003a27a:	5e                   	pop    esi
1003a27b:	c2 08 00             	ret    0x8
1003a27e:	55                   	push   ebp
1003a27f:	8b ec                	mov    ebp,esp
1003a281:	83 ec 20             	sub    esp,0x20
1003a284:	56                   	push   esi
1003a285:	57                   	push   edi
1003a286:	b8 13 12 11 10       	mov    eax,0x10111213
1003a28b:	89 45 ec             	mov    DWORD PTR [ebp-0x14],eax
1003a28e:	89 45 e8             	mov    DWORD PTR [ebp-0x18],eax
1003a291:	89 45 e4             	mov    DWORD PTR [ebp-0x1c],eax
1003a294:	89 45 e0             	mov    DWORD PTR [ebp-0x20],eax
1003a297:	8b 45 0c             	mov    eax,DWORD PTR [ebp+0xc]
1003a29a:	8b d0                	mov    edx,eax
1003a29c:	c1 fa 12             	sar    edx,0x12
1003a29f:	6a 07                	push   0x7
1003a2a1:	59                   	pop    ecx
1003a2a2:	23 d1                	and    edx,ecx
1003a2a4:	c7 44 95 e0 03 02 01 	mov    DWORD PTR [ebp+edx*4-0x20],0x10203
1003a2ab:	00 
1003a2ac:	8b d0                	mov    edx,eax
1003a2ae:	c1 fa 15             	sar    edx,0x15
1003a2b1:	23 d1                	and    edx,ecx
1003a2b3:	c7 44 95 e0 07 06 05 	mov    DWORD PTR [ebp+edx*4-0x20],0x4050607
1003a2ba:	04 
1003a2bb:	8b d0                	mov    edx,eax
1003a2bd:	c1 fa 18             	sar    edx,0x18
1003a2c0:	c1 f8 1b             	sar    eax,0x1b
1003a2c3:	23 d1                	and    edx,ecx
1003a2c5:	c7 44 95 e0 0b 0a 09 	mov    DWORD PTR [ebp+edx*4-0x20],0x8090a0b
1003a2cc:	08 
1003a2cd:	23 c1                	and    eax,ecx
1003a2cf:	c7 44 85 e0 0f 0e 0d 	mov    DWORD PTR [ebp+eax*4-0x20],0xc0d0e0f
1003a2d6:	0c 
1003a2d7:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1003a2da:	8d 75 e0             	lea    esi,[ebp-0x20]
1003a2dd:	8b f8                	mov    edi,eax
1003a2df:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003a2e0:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003a2e1:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003a2e2:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003a2e3:	5f                   	pop    edi
1003a2e4:	5e                   	pop    esi
1003a2e5:	c9                   	leave
1003a2e6:	c2 08 00             	ret    0x8
