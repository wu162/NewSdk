
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000c3ec <.text+0xb3ec>:
1000c3ec:	56                   	push   esi
1000c3ed:	8b 74 24 08          	mov    esi,DWORD PTR [esp+0x8]
1000c3f1:	8b 0e                	mov    ecx,DWORD PTR [esi]
1000c3f3:	8b c1                	mov    eax,ecx
1000c3f5:	83 e0 0f             	and    eax,0xf
1000c3f8:	48                   	dec    eax
1000c3f9:	83 f8 08             	cmp    eax,0x8
1000c3fc:	0f 87 9c 00 00 00    	ja     0x1000c49e
1000c402:	ff 24 85 ac c4 00 10 	jmp    DWORD PTR [eax*4+0x1000c4ac]
1000c409:	f7 c1 00 00 00 40    	test   ecx,0x40000000
1000c40f:	0f 85 89 00 00 00    	jne    0x1000c49e
1000c415:	85 c9                	test   ecx,ecx
1000c417:	79 1b                	jns    0x1000c434
1000c419:	8b 46 2c             	mov    eax,DWORD PTR [esi+0x2c]
1000c41c:	33 d2                	xor    edx,edx
1000c41e:	b9 00 14 00 00       	mov    ecx,0x1400
1000c423:	f7 f1                	div    ecx
1000c425:	50                   	push   eax
1000c426:	8b 46 1c             	mov    eax,DWORD PTR [esi+0x1c]
1000c429:	25 ff 0f 00 00       	and    eax,0xfff
1000c42e:	50                   	push   eax
1000c42f:	e8 71 21 00 00       	call   0x1000e5a5
1000c434:	f6 46 20 01          	test   BYTE PTR [esi+0x20],0x1
1000c438:	74 64                	je     0x1000c49e
1000c43a:	83 25 60 fb 0b 10 00 	and    DWORD PTR ds:0x100bfb60,0x0
1000c441:	eb 5b                	jmp    0x1000c49e
1000c443:	8b 46 20             	mov    eax,DWORD PTR [esi+0x20]
1000c446:	53                   	push   ebx
1000c447:	8b 1d a8 70 09 10    	mov    ebx,DWORD PTR ds:0x100970a8
1000c44d:	55                   	push   ebp
1000c44e:	57                   	push   edi
1000c44f:	8b 7e 30             	mov    edi,DWORD PTR [esi+0x30]
1000c452:	bd 00 80 00 00       	mov    ebp,0x8000
1000c457:	55                   	push   ebp
1000c458:	c1 e8 0c             	shr    eax,0xc
1000c45b:	c1 e0 0c             	shl    eax,0xc
1000c45e:	6a 00                	push   0x0
1000c460:	c1 ef 0c             	shr    edi,0xc
1000c463:	50                   	push   eax
1000c464:	c1 e7 0c             	shl    edi,0xc
1000c467:	ff d3                	call   ebx
1000c469:	55                   	push   ebp
1000c46a:	6a 00                	push   0x0
1000c46c:	57                   	push   edi
1000c46d:	ff d3                	call   ebx
1000c46f:	5f                   	pop    edi
1000c470:	5d                   	pop    ebp
1000c471:	5b                   	pop    ebx
1000c472:	eb 2a                	jmp    0x1000c49e
1000c474:	8b 46 18             	mov    eax,DWORD PTR [esi+0x18]
1000c477:	c1 e8 02             	shr    eax,0x2
1000c47a:	c1 e0 02             	shl    eax,0x2
1000c47d:	68 00 80 00 00       	push   0x8000
1000c482:	6a 00                	push   0x0
1000c484:	50                   	push   eax
1000c485:	ff 15 a8 70 09 10    	call   DWORD PTR ds:0x100970a8
1000c48b:	eb 11                	jmp    0x1000c49e
1000c48d:	8b 46 18             	mov    eax,DWORD PTR [esi+0x18]
1000c490:	eb eb                	jmp    0x1000c47d
1000c492:	8b 46 20             	mov    eax,DWORD PTR [esi+0x20]
1000c495:	eb e6                	jmp    0x1000c47d
1000c497:	8b ce                	mov    ecx,esi
1000c499:	e8 1a 20 00 00       	call   0x1000e4b8
1000c49e:	56                   	push   esi
1000c49f:	ff 15 28 70 09 10    	call   DWORD PTR ds:0x10097028
1000c4a5:	5e                   	pop    esi
1000c4a6:	c2 04 00             	ret    0x4
1000c4a9:	8d 49 00             	lea    ecx,[ecx+0x0]
1000c4ac:	74 c4                	je     0x1000c472
1000c4ae:	00 10                	add    BYTE PTR [eax],dl
1000c4b0:	8d                   	lea    eax,(bad)
1000c4b1:	c4 00                	les    eax,FWORD PTR [eax]
1000c4b3:	10 43 c4             	adc    BYTE PTR [ebx-0x3c],al
1000c4b6:	00 10                	add    BYTE PTR [eax],dl
1000c4b8:	09 c4                	or     esp,eax
1000c4ba:	00 10                	add    BYTE PTR [eax],dl
1000c4bc:	9e                   	sahf
1000c4bd:	c4 00                	les    eax,FWORD PTR [eax]
1000c4bf:	10 92 c4 00 10 8d    	adc    BYTE PTR [edx-0x72efff3c],dl
1000c4c5:	c4 00                	les    eax,FWORD PTR [eax]
1000c4c7:	10 8d c4 00 10 97    	adc    BYTE PTR [ebp-0x68efff3c],cl
1000c4cd:	c4 00                	les    eax,FWORD PTR [eax]
1000c4cf:	10                   	.byte 0x10
