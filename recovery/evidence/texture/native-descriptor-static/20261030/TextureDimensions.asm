
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000c628 <.text+0xb628>:
1000c628:	55                   	push   ebp
1000c629:	8b ec                	mov    ebp,esp
1000c62b:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1000c62e:	8b 48 28             	mov    ecx,DWORD PTR [eax+0x28]
1000c631:	8b 50 30             	mov    edx,DWORD PTR [eax+0x30]
1000c634:	c1 e9 1e             	shr    ecx,0x1e
1000c637:	83 e1 02             	and    ecx,0x2
1000c63a:	c1 ea 09             	shr    edx,0x9
1000c63d:	41                   	inc    ecx
1000c63e:	83 e2 03             	and    edx,0x3
1000c641:	75 1e                	jne    0x1000c661
1000c643:	8b 40 24             	mov    eax,DWORD PTR [eax+0x24]
1000c646:	25 ff ff ff 00       	and    eax,0xffffff
1000c64b:	03 c1                	add    eax,ecx
1000c64d:	8b 4d 0c             	mov    ecx,DWORD PTR [ebp+0xc]
1000c650:	89 01                	mov    DWORD PTR [ecx],eax
1000c652:	8b 4d 10             	mov    ecx,DWORD PTR [ebp+0x10]
1000c655:	33 c0                	xor    eax,eax
1000c657:	40                   	inc    eax
1000c658:	89 01                	mov    DWORD PTR [ecx],eax
1000c65a:	8b 4d 14             	mov    ecx,DWORD PTR [ebp+0x14]
1000c65d:	89 01                	mov    DWORD PTR [ecx],eax
1000c65f:	eb 5b                	jmp    0x1000c6bc
1000c661:	83 fa 02             	cmp    edx,0x2
1000c664:	56                   	push   esi
1000c665:	8b 70 24             	mov    esi,DWORD PTR [eax+0x24]
1000c668:	57                   	push   edi
1000c669:	8b 7d 0c             	mov    edi,DWORD PTR [ebp+0xc]
1000c66c:	75 29                	jne    0x1000c697
1000c66e:	ba ff 07 00 00       	mov    edx,0x7ff
1000c673:	23 f2                	and    esi,edx
1000c675:	03 f1                	add    esi,ecx
1000c677:	89 37                	mov    DWORD PTR [edi],esi
1000c679:	8b 70 24             	mov    esi,DWORD PTR [eax+0x24]
1000c67c:	c1 ee 0b             	shr    esi,0xb
1000c67f:	23 f2                	and    esi,edx
1000c681:	8b 55 10             	mov    edx,DWORD PTR [ebp+0x10]
1000c684:	03 f1                	add    esi,ecx
1000c686:	89 32                	mov    DWORD PTR [edx],esi
1000c688:	8b 40 24             	mov    eax,DWORD PTR [eax+0x24]
1000c68b:	c1 e8 16             	shr    eax,0x16
1000c68e:	03 c1                	add    eax,ecx
1000c690:	8b 4d 14             	mov    ecx,DWORD PTR [ebp+0x14]
1000c693:	89 01                	mov    DWORD PTR [ecx],eax
1000c695:	eb 23                	jmp    0x1000c6ba
1000c697:	ba ff 1f 00 00       	mov    edx,0x1fff
1000c69c:	23 f2                	and    esi,edx
1000c69e:	03 f1                	add    esi,ecx
1000c6a0:	89 37                	mov    DWORD PTR [edi],esi
1000c6a2:	8b 40 24             	mov    eax,DWORD PTR [eax+0x24]
1000c6a5:	c1 e8 0d             	shr    eax,0xd
1000c6a8:	23 c2                	and    eax,edx
1000c6aa:	03 c1                	add    eax,ecx
1000c6ac:	8b 4d 10             	mov    ecx,DWORD PTR [ebp+0x10]
1000c6af:	89 01                	mov    DWORD PTR [ecx],eax
1000c6b1:	8b 45 14             	mov    eax,DWORD PTR [ebp+0x14]
1000c6b4:	c7 00 01 00 00 00    	mov    DWORD PTR [eax],0x1
1000c6ba:	5f                   	pop    edi
1000c6bb:	5e                   	pop    esi
1000c6bc:	5d                   	pop    ebp
1000c6bd:	c2 10 00             	ret    0x10
