
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000c4d0 <.text+0xb4d0>:
1000c4d0:	55                   	push   ebp
1000c4d1:	8b ec                	mov    ebp,esp
1000c4d3:	8b 45 2c             	mov    eax,DWORD PTR [ebp+0x2c]
1000c4d6:	8b 4d 0c             	mov    ecx,DWORD PTR [ebp+0xc]
1000c4d9:	83 c8 02             	or     eax,0x2
1000c4dc:	0b 4d 10             	or     ecx,DWORD PTR [ebp+0x10]
1000c4df:	74 03                	je     0x1000c4e4
1000c4e1:	83 c8 02             	or     eax,0x2
1000c4e4:	a8 10                	test   al,0x10
1000c4e6:	53                   	push   ebx
1000c4e7:	8b 5d 08             	mov    ebx,DWORD PTR [ebp+0x8]
1000c4ea:	75 08                	jne    0x1000c4f4
1000c4ec:	f7 03 00 00 20 00    	test   DWORD PTR [ebx],0x200000
1000c4f2:	74 54                	je     0x1000c548
1000c4f4:	a8 01                	test   al,0x1
1000c4f6:	75 50                	jne    0x1000c548
1000c4f8:	83 7d 18 00          	cmp    DWORD PTR [ebp+0x18],0x0
1000c4fc:	56                   	push   esi
1000c4fd:	74 0c                	je     0x1000c50b
1000c4ff:	8b 4d 20             	mov    ecx,DWORD PTR [ebp+0x20]
1000c502:	85 c9                	test   ecx,ecx
1000c504:	74 05                	je     0x1000c50b
1000c506:	8d 73 18             	lea    esi,[ebx+0x18]
1000c509:	eb 06                	jmp    0x1000c511
1000c50b:	8b 4d 1c             	mov    ecx,DWORD PTR [ebp+0x1c]
1000c50e:	8d 73 14             	lea    esi,[ebx+0x14]
1000c511:	8b 45 24             	mov    eax,DWORD PTR [ebp+0x24]
1000c514:	8b 55 28             	mov    edx,DWORD PTR [ebp+0x28]
1000c517:	83 e1 80             	and    ecx,0xffffff80
1000c51a:	2b c1                	sub    eax,ecx
1000c51c:	57                   	push   edi
1000c51d:	8b 3e                	mov    edi,DWORD PTR [esi]
1000c51f:	8b c8                	mov    ecx,eax
1000c521:	8d 44 10 7f          	lea    eax,[eax+edx*1+0x7f]
1000c525:	8b d7                	mov    edx,edi
1000c527:	0f b7 ff             	movzx  edi,di
1000c52a:	c1 e8 07             	shr    eax,0x7
1000c52d:	c1 e9 07             	shr    ecx,0x7
1000c530:	c1 ea 10             	shr    edx,0x10
1000c533:	3b c7                	cmp    eax,edi
1000c535:	77 02                	ja     0x1000c539
1000c537:	8b c7                	mov    eax,edi
1000c539:	3b ca                	cmp    ecx,edx
1000c53b:	5f                   	pop    edi
1000c53c:	72 02                	jb     0x1000c540
1000c53e:	8b ca                	mov    ecx,edx
1000c540:	c1 e1 10             	shl    ecx,0x10
1000c543:	0b c8                	or     ecx,eax
1000c545:	89 0e                	mov    DWORD PTR [esi],ecx
1000c547:	5e                   	pop    esi
1000c548:	68 00 01 00 00       	push   0x100
1000c54d:	53                   	push   ebx
1000c54e:	ff 15 a4 70 09 10    	call   DWORD PTR ds:0x100970a4
1000c554:	8b 45 24             	mov    eax,DWORD PTR [ebp+0x24]
1000c557:	5b                   	pop    ebx
1000c558:	5d                   	pop    ebp
1000c559:	c2 2c 00             	ret    0x2c
