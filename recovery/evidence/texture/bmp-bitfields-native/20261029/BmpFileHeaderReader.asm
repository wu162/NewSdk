
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

100167ad <.text+0x157ad>:
100167ad:	55                   	push   ebp
100167ae:	8b ec                	mov    ebp,esp
100167b0:	83 ec 14             	sub    esp,0x14
100167b3:	53                   	push   ebx
100167b4:	8b 5d 0c             	mov    ebx,DWORD PTR [ebp+0xc]
100167b7:	83 fb 0e             	cmp    ebx,0xe
100167ba:	89 4d fc             	mov    DWORD PTR [ebp-0x4],ecx
100167bd:	72 3b                	jb     0x100167fa
100167bf:	56                   	push   esi
100167c0:	8b 75 08             	mov    esi,DWORD PTR [ebp+0x8]
100167c3:	57                   	push   edi
100167c4:	8d 7d ec             	lea    edi,[ebp-0x14]
100167c7:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
100167c8:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
100167c9:	8d 45 ec             	lea    eax,[ebp-0x14]
100167cc:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
100167cd:	50                   	push   eax
100167ce:	50                   	push   eax
100167cf:	66 a5                	movs   WORD PTR es:[edi],WORD PTR ds:[esi]
100167d1:	e8 96 d6 ff ff       	call   0x10013e6c
100167d6:	66 81 7d ec 42 4d    	cmp    WORD PTR [ebp-0x14],0x4d42
100167dc:	5f                   	pop    edi
100167dd:	5e                   	pop    esi
100167de:	75 1a                	jne    0x100167fa
100167e0:	39 5d ee             	cmp    DWORD PTR [ebp-0x12],ebx
100167e3:	77 15                	ja     0x100167fa
100167e5:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
100167e8:	8b 4d fc             	mov    ecx,DWORD PTR [ebp-0x4]
100167eb:	83 c3 f2             	add    ebx,0xfffffff2
100167ee:	53                   	push   ebx
100167ef:	83 c0 0e             	add    eax,0xe
100167f2:	50                   	push   eax
100167f3:	e8 45 d7 ff ff       	call   0x10013f3d
100167f8:	eb 05                	jmp    0x100167ff
100167fa:	b8 05 40 00 80       	mov    eax,0x80004005
100167ff:	5b                   	pop    ebx
10016800:	c9                   	leave
10016801:	c2 08 00             	ret    0x8
