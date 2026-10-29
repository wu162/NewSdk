
SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1003b540 <.text+0x3a540>:
1003b540:	55                   	push   ebp
1003b541:	8d 6c 24 a8          	lea    ebp,[esp-0x58]
1003b545:	81 ec 88 00 00 00    	sub    esp,0x88
1003b54b:	83 38 01             	cmp    DWORD PTR [eax],0x1
1003b54e:	56                   	push   esi
1003b54f:	57                   	push   edi
1003b550:	75 44                	jne    0x1003b596
1003b552:	b8 00 00 80 7f       	mov    eax,0x7f800000
1003b557:	8d 7d 10             	lea    edi,[ebp+0x10]
1003b55a:	ab                   	stos   DWORD PTR es:[edi],eax
1003b55b:	ab                   	stos   DWORD PTR es:[edi],eax
1003b55c:	ab                   	stos   DWORD PTR es:[edi],eax
1003b55d:	ab                   	stos   DWORD PTR es:[edi],eax
1003b55e:	b8 00 00 80 ff       	mov    eax,0xff800000
1003b563:	8d 7d 00             	lea    edi,[ebp+0x0]
1003b566:	ab                   	stos   DWORD PTR es:[edi],eax
1003b567:	ab                   	stos   DWORD PTR es:[edi],eax
1003b568:	ab                   	stos   DWORD PTR es:[edi],eax
1003b569:	ab                   	stos   DWORD PTR es:[edi],eax
1003b56a:	33 c0                	xor    eax,eax
1003b56c:	8d 7d 20             	lea    edi,[ebp+0x20]
1003b56f:	ab                   	stos   DWORD PTR es:[edi],eax
1003b570:	ab                   	stos   DWORD PTR es:[edi],eax
1003b571:	ab                   	stos   DWORD PTR es:[edi],eax
1003b572:	ab                   	stos   DWORD PTR es:[edi],eax
1003b573:	b8 00 00 80 3f       	mov    eax,0x3f800000
1003b578:	8d 7d 30             	lea    edi,[ebp+0x30]
1003b57b:	ab                   	stos   DWORD PTR es:[edi],eax
1003b57c:	ab                   	stos   DWORD PTR es:[edi],eax
1003b57d:	ab                   	stos   DWORD PTR es:[edi],eax
1003b57e:	ab                   	stos   DWORD PTR es:[edi],eax
1003b57f:	33 c0                	xor    eax,eax
1003b581:	8d 7d d0             	lea    edi,[ebp-0x30]
1003b584:	ab                   	stos   DWORD PTR es:[edi],eax
1003b585:	ab                   	stos   DWORD PTR es:[edi],eax
1003b586:	ab                   	stos   DWORD PTR es:[edi],eax
1003b587:	ab                   	stos   DWORD PTR es:[edi],eax
1003b588:	33 c0                	xor    eax,eax
1003b58a:	8d 7d e0             	lea    edi,[ebp-0x20]
1003b58d:	ab                   	stos   DWORD PTR es:[edi],eax
1003b58e:	ab                   	stos   DWORD PTR es:[edi],eax
1003b58f:	ab                   	stos   DWORD PTR es:[edi],eax
1003b590:	ab                   	stos   DWORD PTR es:[edi],eax
1003b591:	e9 90 01 00 00       	jmp    0x1003b726
1003b596:	8d 48 1c             	lea    ecx,[eax+0x1c]
1003b599:	83 c8 ff             	or     eax,0xffffffff
1003b59c:	8d 7d d0             	lea    edi,[ebp-0x30]
1003b59f:	ab                   	stos   DWORD PTR es:[edi],eax
1003b5a0:	ab                   	stos   DWORD PTR es:[edi],eax
1003b5a1:	ab                   	stos   DWORD PTR es:[edi],eax
1003b5a2:	53                   	push   ebx
1003b5a3:	89 4d 50             	mov    DWORD PTR [ebp+0x50],ecx
1003b5a6:	ab                   	stos   DWORD PTR es:[edi],eax
1003b5a7:	33 f6                	xor    esi,esi
1003b5a9:	bb 00 00 00 80       	mov    ebx,0x80000000
1003b5ae:	eb 03                	jmp    0x1003b5b3
1003b5b0:	8b 4d 50             	mov    ecx,DWORD PTR [ebp+0x50]
1003b5b3:	8b 01                	mov    eax,DWORD PTR [ecx]
1003b5b5:	8b 49 10             	mov    ecx,DWORD PTR [ecx+0x10]
1003b5b8:	d3 e8                	shr    eax,cl
1003b5ba:	50                   	push   eax
1003b5bb:	e8 ee 11 fd ff       	call   0x1000c7ae
1003b5c0:	6a 20                	push   0x20
1003b5c2:	5f                   	pop    edi
1003b5c3:	2b f8                	sub    edi,eax
1003b5c5:	75 18                	jne    0x1003b5df
1003b5c7:	d9 ee                	fldz
1003b5c9:	83 64 35 e0 00       	and    DWORD PTR [ebp+esi*1-0x20],0x0
1003b5ce:	d9 54 35 00          	fst    DWORD PTR [ebp+esi*1+0x0]
1003b5d2:	d9 54 35 10          	fst    DWORD PTR [ebp+esi*1+0x10]
1003b5d6:	d9 54 35 30          	fst    DWORD PTR [ebp+esi*1+0x30]
1003b5da:	e9 32 01 00 00       	jmp    0x1003b711
1003b5df:	8b 45 64             	mov    eax,DWORD PTR [ebp+0x64]
1003b5e2:	8b 04 06             	mov    eax,DWORD PTR [esi+eax*1]
1003b5e5:	33 d2                	xor    edx,edx
1003b5e7:	42                   	inc    edx
1003b5e8:	8d 4f ff             	lea    ecx,[edi-0x1]
1003b5eb:	d3 e2                	shl    edx,cl
1003b5ed:	8b c8                	mov    ecx,eax
1003b5ef:	49                   	dec    ecx
1003b5f0:	f7 d9                	neg    ecx
1003b5f2:	1b c9                	sbb    ecx,ecx
1003b5f4:	f7 d1                	not    ecx
1003b5f6:	23 ca                	and    ecx,edx
1003b5f8:	83 7d 60 00          	cmp    DWORD PTR [ebp+0x60],0x0
1003b5fc:	89 4c 35 e0          	mov    DWORD PTR [ebp+esi*1-0x20],ecx
1003b600:	0f 85 89 00 00 00    	jne    0x1003b68f
1003b606:	85 c0                	test   eax,eax
1003b608:	74 45                	je     0x1003b64f
1003b60a:	83 f8 03             	cmp    eax,0x3
1003b60d:	74 40                	je     0x1003b64f
1003b60f:	d9 05 a0 92 09 10    	fld    DWORD PTR ds:0x100992a0
1003b615:	8d 42 ff             	lea    eax,[edx-0x1]
1003b618:	85 c0                	test   eax,eax
1003b61a:	d9 5c 35 00          	fstp   DWORD PTR [ebp+esi*1+0x0]
1003b61e:	d9 e8                	fld1
1003b620:	89 45 54             	mov    DWORD PTR [ebp+0x54],eax
1003b623:	d9 5c 35 10          	fstp   DWORD PTR [ebp+esi*1+0x10]
1003b627:	db 45 54             	fild   DWORD PTR [ebp+0x54]
1003b62a:	7d 06                	jge    0x1003b632
1003b62c:	d8 05 a0 80 09 10    	fadd   DWORD PTR ds:0x100980a0
1003b632:	89 55 54             	mov    DWORD PTR [ebp+0x54],edx
1003b635:	d9 5c 35 30          	fstp   DWORD PTR [ebp+esi*1+0x30]
1003b639:	db 45 54             	fild   DWORD PTR [ebp+0x54]
1003b63c:	85 d2                	test   edx,edx
1003b63e:	0f 8d cd 00 00 00    	jge    0x1003b711
1003b644:	d8 05 a0 80 09 10    	fadd   DWORD PTR ds:0x100980a0
1003b64a:	e9 c2 00 00 00       	jmp    0x1003b711
1003b64f:	d9 ee                	fldz
1003b651:	33 c0                	xor    eax,eax
1003b653:	d9 54 35 00          	fst    DWORD PTR [ebp+esi*1+0x0]
1003b657:	40                   	inc    eax
1003b658:	d9 e8                	fld1
1003b65a:	33 d2                	xor    edx,edx
1003b65c:	8b cf                	mov    ecx,edi
1003b65e:	d9 5c 35 10          	fstp   DWORD PTR [ebp+esi*1+0x10]
1003b662:	e8 f9 36 01 00       	call   0x1004ed60
1003b667:	83 c0 ff             	add    eax,0xffffffff
1003b66a:	83 d2 ff             	adc    edx,0xffffffff
1003b66d:	89 55 44             	mov    DWORD PTR [ebp+0x44],edx
1003b670:	21 5d 44             	and    DWORD PTR [ebp+0x44],ebx
1003b673:	81 e2 ff ff ff 7f    	and    edx,0x7fffffff
1003b679:	83 65 40 00          	and    DWORD PTR [ebp+0x40],0x0
1003b67d:	89 45 f0             	mov    DWORD PTR [ebp-0x10],eax
1003b680:	89 55 f4             	mov    DWORD PTR [ebp-0xc],edx
1003b683:	df 6d f0             	fild   QWORD PTR [ebp-0x10]
1003b686:	df 6d 40             	fild   QWORD PTR [ebp+0x40]
1003b689:	d9 e0                	fchs
1003b68b:	de c1                	faddp  st(1),st
1003b68d:	eb 7e                	jmp    0x1003b70d
1003b68f:	85 c0                	test   eax,eax
1003b691:	74 3c                	je     0x1003b6cf
1003b693:	83 f8 03             	cmp    eax,0x3
1003b696:	74 37                	je     0x1003b6cf
1003b698:	8d 42 ff             	lea    eax,[edx-0x1]
1003b69b:	85 c0                	test   eax,eax
1003b69d:	89 45 54             	mov    DWORD PTR [ebp+0x54],eax
1003b6a0:	db 45 54             	fild   DWORD PTR [ebp+0x54]
1003b6a3:	7d 06                	jge    0x1003b6ab
1003b6a5:	d8 05 a0 80 09 10    	fadd   DWORD PTR ds:0x100980a0
1003b6ab:	d9 5d 54             	fstp   DWORD PTR [ebp+0x54]
1003b6ae:	d9 45 54             	fld    DWORD PTR [ebp+0x54]
1003b6b1:	d9 e0                	fchs
1003b6b3:	d9 5c 35 00          	fstp   DWORD PTR [ebp+esi*1+0x0]
1003b6b7:	d9 45 54             	fld    DWORD PTR [ebp+0x54]
1003b6ba:	89 55 54             	mov    DWORD PTR [ebp+0x54],edx
1003b6bd:	d9 5c 35 10          	fstp   DWORD PTR [ebp+esi*1+0x10]
1003b6c1:	d9 e8                	fld1
1003b6c3:	d9 5c 35 30          	fstp   DWORD PTR [ebp+esi*1+0x30]
1003b6c7:	db 45 54             	fild   DWORD PTR [ebp+0x54]
1003b6ca:	e9 6d ff ff ff       	jmp    0x1003b63c
1003b6cf:	d9 ee                	fldz
1003b6d1:	33 c0                	xor    eax,eax
1003b6d3:	40                   	inc    eax
1003b6d4:	d9 54 35 00          	fst    DWORD PTR [ebp+esi*1+0x0]
1003b6d8:	33 d2                	xor    edx,edx
1003b6da:	8b cf                	mov    ecx,edi
1003b6dc:	e8 7f 36 01 00       	call   0x1004ed60
1003b6e1:	83 c0 ff             	add    eax,0xffffffff
1003b6e4:	83 d2 ff             	adc    edx,0xffffffff
1003b6e7:	89 55 4c             	mov    DWORD PTR [ebp+0x4c],edx
1003b6ea:	21 5d 4c             	and    DWORD PTR [ebp+0x4c],ebx
1003b6ed:	81 e2 ff ff ff 7f    	and    edx,0x7fffffff
1003b6f3:	83 65 48 00          	and    DWORD PTR [ebp+0x48],0x0
1003b6f7:	89 45 f8             	mov    DWORD PTR [ebp-0x8],eax
1003b6fa:	89 55 fc             	mov    DWORD PTR [ebp-0x4],edx
1003b6fd:	df 6d f8             	fild   QWORD PTR [ebp-0x8]
1003b700:	df 6d 48             	fild   QWORD PTR [ebp+0x48]
1003b703:	d9 e0                	fchs
1003b705:	de c1                	faddp  st(1),st
1003b707:	d9 5c 35 10          	fstp   DWORD PTR [ebp+esi*1+0x10]
1003b70b:	d9 e8                	fld1
1003b70d:	d9 5c 35 30          	fstp   DWORD PTR [ebp+esi*1+0x30]
1003b711:	83 45 50 04          	add    DWORD PTR [ebp+0x50],0x4
1003b715:	d9 5c 35 20          	fstp   DWORD PTR [ebp+esi*1+0x20]
1003b719:	83 c6 04             	add    esi,0x4
1003b71c:	83 fe 10             	cmp    esi,0x10
1003b71f:	0f 82 8b fe ff ff    	jb     0x1003b5b0
1003b725:	5b                   	pop    ebx
1003b726:	8b 7d 68             	mov    edi,DWORD PTR [ebp+0x68]
1003b729:	8d 75 d0             	lea    esi,[ebp-0x30]
1003b72c:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b72d:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b72e:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b72f:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b730:	8b 7d 6c             	mov    edi,DWORD PTR [ebp+0x6c]
1003b733:	8d 75 30             	lea    esi,[ebp+0x30]
1003b736:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b737:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b738:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b739:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b73a:	8b 7d 70             	mov    edi,DWORD PTR [ebp+0x70]
1003b73d:	8d 75 20             	lea    esi,[ebp+0x20]
1003b740:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b741:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b742:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b743:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b744:	8b 7d 74             	mov    edi,DWORD PTR [ebp+0x74]
1003b747:	8d 75 00             	lea    esi,[ebp+0x0]
1003b74a:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b74b:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b74c:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b74d:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b74e:	8b 7d 78             	mov    edi,DWORD PTR [ebp+0x78]
1003b751:	8d 75 10             	lea    esi,[ebp+0x10]
1003b754:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b755:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b756:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b757:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b758:	8b 7d 7c             	mov    edi,DWORD PTR [ebp+0x7c]
1003b75b:	8d 75 e0             	lea    esi,[ebp-0x20]
1003b75e:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b75f:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b760:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b761:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b762:	5f                   	pop    edi
1003b763:	5e                   	pop    esi
1003b764:	83 c5 58             	add    ebp,0x58
1003b767:	c9                   	leave
1003b768:	c2 20 00             	ret    0x20
