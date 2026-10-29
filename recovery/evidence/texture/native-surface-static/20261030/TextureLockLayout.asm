
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000d680 <.text+0xc680>:
1000d680:	55                   	push   ebp
1000d681:	8b ec                	mov    ebp,esp
1000d683:	53                   	push   ebx
1000d684:	8b 5d 2c             	mov    ebx,DWORD PTR [ebp+0x2c]
1000d687:	56                   	push   esi
1000d688:	57                   	push   edi
1000d689:	8b 7d 20             	mov    edi,DWORD PTR [ebp+0x20]
1000d68c:	57                   	push   edi
1000d68d:	8b 75 08             	mov    esi,DWORD PTR [ebp+0x8]
1000d690:	8d 45 20             	lea    eax,[ebp+0x20]
1000d693:	50                   	push   eax
1000d694:	8d 45 28             	lea    eax,[ebp+0x28]
1000d697:	50                   	push   eax
1000d698:	8d 45 14             	lea    eax,[ebp+0x14]
1000d69b:	50                   	push   eax
1000d69c:	53                   	push   ebx
1000d69d:	8d 45 2c             	lea    eax,[ebp+0x2c]
1000d6a0:	50                   	push   eax
1000d6a1:	ff 75 28             	push   DWORD PTR [ebp+0x28]
1000d6a4:	ff 75 24             	push   DWORD PTR [ebp+0x24]
1000d6a7:	ff 75 18             	push   DWORD PTR [ebp+0x18]
1000d6aa:	ff 75 14             	push   DWORD PTR [ebp+0x14]
1000d6ad:	56                   	push   esi
1000d6ae:	e8 98 fb ff ff       	call   0x1000d24b
1000d6b3:	8d 45 20             	lea    eax,[ebp+0x20]
1000d6b6:	50                   	push   eax
1000d6b7:	8d 45 28             	lea    eax,[ebp+0x28]
1000d6ba:	50                   	push   eax
1000d6bb:	8b 46 20             	mov    eax,DWORD PTR [esi+0x20]
1000d6be:	83 e0 3f             	and    eax,0x3f
1000d6c1:	50                   	push   eax
1000d6c2:	e8 fd f0 ff ff       	call   0x1000c7c4
1000d6c7:	8b 45 24             	mov    eax,DWORD PTR [ebp+0x24]
1000d6ca:	8b 08                	mov    ecx,DWORD PTR [eax]
1000d6cc:	0f af 4d 20          	imul   ecx,DWORD PTR [ebp+0x20]
1000d6d0:	68 00 00 00 02       	push   0x2000000
1000d6d5:	ff 75 1c             	push   DWORD PTR [ebp+0x1c]
1000d6d8:	89 08                	mov    DWORD PTR [eax],ecx
1000d6da:	ff 75 2c             	push   DWORD PTR [ebp+0x2c]
1000d6dd:	8b 07                	mov    eax,DWORD PTR [edi]
1000d6df:	2b 03                	sub    eax,DWORD PTR [ebx]
1000d6e1:	8b 4e 30             	mov    ecx,DWORD PTR [esi+0x30]
1000d6e4:	50                   	push   eax
1000d6e5:	b8 00 f0 ff ff       	mov    eax,0xfffff000
1000d6ea:	23 c8                	and    ecx,eax
1000d6ec:	51                   	push   ecx
1000d6ed:	8b 4e 20             	mov    ecx,DWORD PTR [esi+0x20]
1000d6f0:	23 c8                	and    ecx,eax
1000d6f2:	51                   	push   ecx
1000d6f3:	ff 75 18             	push   DWORD PTR [ebp+0x18]
1000d6f6:	6a 0e                	push   0xe
1000d6f8:	ff 75 10             	push   DWORD PTR [ebp+0x10]
1000d6fb:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
1000d6fe:	56                   	push   esi
1000d6ff:	e8 cc ed ff ff       	call   0x1000c4d0
1000d704:	03 03                	add    eax,DWORD PTR [ebx]
1000d706:	89 07                	mov    DWORD PTR [edi],eax
1000d708:	5f                   	pop    edi
1000d709:	5e                   	pop    esi
1000d70a:	5b                   	pop    ebx
1000d70b:	5d                   	pop    ebp
1000d70c:	c2 28 00             	ret    0x28
