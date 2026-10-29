
SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1003b366 <.text+0x3a366>:
1003b366:	55                   	push   ebp
1003b367:	8b ec                	mov    ebp,esp
1003b369:	83 ec 70             	sub    esp,0x70
1003b36c:	83 38 01             	cmp    DWORD PTR [eax],0x1
1003b36f:	56                   	push   esi
1003b370:	57                   	push   edi
1003b371:	75 38                	jne    0x1003b3ab
1003b373:	b8 00 00 80 ff       	mov    eax,0xff800000
1003b378:	8d 7d b0             	lea    edi,[ebp-0x50]
1003b37b:	ab                   	stos   DWORD PTR es:[edi],eax
1003b37c:	ab                   	stos   DWORD PTR es:[edi],eax
1003b37d:	ab                   	stos   DWORD PTR es:[edi],eax
1003b37e:	ab                   	stos   DWORD PTR es:[edi],eax
1003b37f:	b8 00 00 80 3f       	mov    eax,0x3f800000
1003b384:	8d 7d c0             	lea    edi,[ebp-0x40]
1003b387:	ab                   	stos   DWORD PTR es:[edi],eax
1003b388:	ab                   	stos   DWORD PTR es:[edi],eax
1003b389:	ab                   	stos   DWORD PTR es:[edi],eax
1003b38a:	ab                   	stos   DWORD PTR es:[edi],eax
1003b38b:	33 c0                	xor    eax,eax
1003b38d:	8d 7d d0             	lea    edi,[ebp-0x30]
1003b390:	ab                   	stos   DWORD PTR es:[edi],eax
1003b391:	ab                   	stos   DWORD PTR es:[edi],eax
1003b392:	ab                   	stos   DWORD PTR es:[edi],eax
1003b393:	ab                   	stos   DWORD PTR es:[edi],eax
1003b394:	33 c0                	xor    eax,eax
1003b396:	8d 7d 90             	lea    edi,[ebp-0x70]
1003b399:	ab                   	stos   DWORD PTR es:[edi],eax
1003b39a:	ab                   	stos   DWORD PTR es:[edi],eax
1003b39b:	ab                   	stos   DWORD PTR es:[edi],eax
1003b39c:	ab                   	stos   DWORD PTR es:[edi],eax
1003b39d:	33 c0                	xor    eax,eax
1003b39f:	8d 7d a0             	lea    edi,[ebp-0x60]
1003b3a2:	ab                   	stos   DWORD PTR es:[edi],eax
1003b3a3:	ab                   	stos   DWORD PTR es:[edi],eax
1003b3a4:	ab                   	stos   DWORD PTR es:[edi],eax
1003b3a5:	ab                   	stos   DWORD PTR es:[edi],eax
1003b3a6:	e9 5d 01 00 00       	jmp    0x1003b508
1003b3ab:	8d 48 1c             	lea    ecx,[eax+0x1c]
1003b3ae:	83 c8 ff             	or     eax,0xffffffff
1003b3b1:	8d 7d 90             	lea    edi,[ebp-0x70]
1003b3b4:	ab                   	stos   DWORD PTR es:[edi],eax
1003b3b5:	ab                   	stos   DWORD PTR es:[edi],eax
1003b3b6:	ab                   	stos   DWORD PTR es:[edi],eax
1003b3b7:	89 4d fc             	mov    DWORD PTR [ebp-0x4],ecx
1003b3ba:	ab                   	stos   DWORD PTR es:[edi],eax
1003b3bb:	33 f6                	xor    esi,esi
1003b3bd:	eb 03                	jmp    0x1003b3c2
1003b3bf:	8b 4d fc             	mov    ecx,DWORD PTR [ebp-0x4]
1003b3c2:	8b 01                	mov    eax,DWORD PTR [ecx]
1003b3c4:	8b 49 10             	mov    ecx,DWORD PTR [ecx+0x10]
1003b3c7:	d3 e8                	shr    eax,cl
1003b3c9:	50                   	push   eax
1003b3ca:	e8 df 13 fd ff       	call   0x1000c7ae
1003b3cf:	6a 20                	push   0x20
1003b3d1:	5f                   	pop    edi
1003b3d2:	2b f8                	sub    edi,eax
1003b3d4:	75 18                	jne    0x1003b3ee
1003b3d6:	d9 ee                	fldz
1003b3d8:	83 64 35 a0 00       	and    DWORD PTR [ebp+esi*1-0x60],0x0
1003b3dd:	d9 54 35 d0          	fst    DWORD PTR [ebp+esi*1-0x30]
1003b3e1:	d9 54 35 c0          	fst    DWORD PTR [ebp+esi*1-0x40]
1003b3e5:	d9 5c 35 b0          	fstp   DWORD PTR [ebp+esi*1-0x50]
1003b3e9:	e9 dc 00 00 00       	jmp    0x1003b4ca
1003b3ee:	8b 45 0c             	mov    eax,DWORD PTR [ebp+0xc]
1003b3f1:	8b 04 06             	mov    eax,DWORD PTR [esi+eax*1]
1003b3f4:	33 d2                	xor    edx,edx
1003b3f6:	42                   	inc    edx
1003b3f7:	8d 4f ff             	lea    ecx,[edi-0x1]
1003b3fa:	d3 e2                	shl    edx,cl
1003b3fc:	8b c8                	mov    ecx,eax
1003b3fe:	49                   	dec    ecx
1003b3ff:	f7 d9                	neg    ecx
1003b401:	1b c9                	sbb    ecx,ecx
1003b403:	f7 d1                	not    ecx
1003b405:	23 ca                	and    ecx,edx
1003b407:	83 7d 08 00          	cmp    DWORD PTR [ebp+0x8],0x0
1003b40b:	89 4c 35 a0          	mov    DWORD PTR [ebp+esi*1-0x60],ecx
1003b40f:	0f 85 84 00 00 00    	jne    0x1003b499
1003b415:	85 c0                	test   eax,eax
1003b417:	74 3e                	je     0x1003b457
1003b419:	83 f8 03             	cmp    eax,0x3
1003b41c:	74 39                	je     0x1003b457
1003b41e:	8b c2                	mov    eax,edx
1003b420:	85 c0                	test   eax,eax
1003b422:	89 45 f8             	mov    DWORD PTR [ebp-0x8],eax
1003b425:	db 45 f8             	fild   DWORD PTR [ebp-0x8]
1003b428:	7d 06                	jge    0x1003b430
1003b42a:	d8 05 a0 80 09 10    	fadd   DWORD PTR ds:0x100980a0
1003b430:	4a                   	dec    edx
1003b431:	d9 e0                	fchs
1003b433:	85 d2                	test   edx,edx
1003b435:	d9 5c 35 d0          	fstp   DWORD PTR [ebp+esi*1-0x30]
1003b439:	89 55 f8             	mov    DWORD PTR [ebp-0x8],edx
1003b43c:	db 45 f8             	fild   DWORD PTR [ebp-0x8]
1003b43f:	7d 06                	jge    0x1003b447
1003b441:	d8 05 a0 80 09 10    	fadd   DWORD PTR ds:0x100980a0
1003b447:	d9 e8                	fld1
1003b449:	de f1                	fdivrp st(1),st
1003b44b:	d9 5c 35 c0          	fstp   DWORD PTR [ebp+esi*1-0x40]
1003b44f:	d9 05 a0 92 09 10    	fld    DWORD PTR ds:0x100992a0
1003b455:	eb 8e                	jmp    0x1003b3e5
1003b457:	d9 ee                	fldz
1003b459:	33 c0                	xor    eax,eax
1003b45b:	40                   	inc    eax
1003b45c:	d9 5c 35 d0          	fstp   DWORD PTR [ebp+esi*1-0x30]
1003b460:	33 d2                	xor    edx,edx
1003b462:	8b cf                	mov    ecx,edi
1003b464:	e8 f7 38 01 00       	call   0x1004ed60
1003b469:	83 c0 ff             	add    eax,0xffffffff
1003b46c:	83 d2 ff             	adc    edx,0xffffffff
1003b46f:	89 55 f4             	mov    DWORD PTR [ebp-0xc],edx
1003b472:	81 65 f4 00 00 00 80 	and    DWORD PTR [ebp-0xc],0x80000000
1003b479:	81 e2 ff ff ff 7f    	and    edx,0x7fffffff
1003b47f:	83 65 f0 00          	and    DWORD PTR [ebp-0x10],0x0
1003b483:	89 45 e8             	mov    DWORD PTR [ebp-0x18],eax
1003b486:	89 55 ec             	mov    DWORD PTR [ebp-0x14],edx
1003b489:	df 6d e8             	fild   QWORD PTR [ebp-0x18]
1003b48c:	df 6d f0             	fild   QWORD PTR [ebp-0x10]
1003b48f:	d9 e0                	fchs
1003b491:	de c1                	faddp  st(1),st
1003b493:	d9 e8                	fld1
1003b495:	de f1                	fdivrp st(1),st
1003b497:	eb 25                	jmp    0x1003b4be
1003b499:	85 c0                	test   eax,eax
1003b49b:	74 19                	je     0x1003b4b6
1003b49d:	83 f8 03             	cmp    eax,0x3
1003b4a0:	74 14                	je     0x1003b4b6
1003b4a2:	85 d2                	test   edx,edx
1003b4a4:	89 55 f8             	mov    DWORD PTR [ebp-0x8],edx
1003b4a7:	db 45 f8             	fild   DWORD PTR [ebp-0x8]
1003b4aa:	7d 06                	jge    0x1003b4b2
1003b4ac:	d8 05 a0 80 09 10    	fadd   DWORD PTR ds:0x100980a0
1003b4b2:	d9 e0                	fchs
1003b4b4:	eb 02                	jmp    0x1003b4b8
1003b4b6:	d9 ee                	fldz
1003b4b8:	d9 5c 35 d0          	fstp   DWORD PTR [ebp+esi*1-0x30]
1003b4bc:	d9 e8                	fld1
1003b4be:	d9 5c 35 c0          	fstp   DWORD PTR [ebp+esi*1-0x40]
1003b4c2:	c7 44 35 b0 00 00 80 	mov    DWORD PTR [ebp+esi*1-0x50],0xff800000
1003b4c9:	ff 
1003b4ca:	83 45 fc 04          	add    DWORD PTR [ebp-0x4],0x4
1003b4ce:	83 c6 04             	add    esi,0x4
1003b4d1:	83 fe 10             	cmp    esi,0x10
1003b4d4:	0f 82 e5 fe ff ff    	jb     0x1003b3bf
1003b4da:	d9 45 d0             	fld    DWORD PTR [ebp-0x30]
1003b4dd:	8d 75 e0             	lea    esi,[ebp-0x20]
1003b4e0:	d8 4d c0             	fmul   DWORD PTR [ebp-0x40]
1003b4e3:	8d 7d d0             	lea    edi,[ebp-0x30]
1003b4e6:	d9 5d e0             	fstp   DWORD PTR [ebp-0x20]
1003b4e9:	d9 45 d4             	fld    DWORD PTR [ebp-0x2c]
1003b4ec:	d8 4d c4             	fmul   DWORD PTR [ebp-0x3c]
1003b4ef:	d9 5d e4             	fstp   DWORD PTR [ebp-0x1c]
1003b4f2:	d9 45 d8             	fld    DWORD PTR [ebp-0x28]
1003b4f5:	d8 4d c8             	fmul   DWORD PTR [ebp-0x38]
1003b4f8:	d9 5d e8             	fstp   DWORD PTR [ebp-0x18]
1003b4fb:	d9 45 dc             	fld    DWORD PTR [ebp-0x24]
1003b4fe:	d8 4d cc             	fmul   DWORD PTR [ebp-0x34]
1003b501:	d9 5d ec             	fstp   DWORD PTR [ebp-0x14]
1003b504:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b505:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b506:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b507:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b508:	8b 7d 10             	mov    edi,DWORD PTR [ebp+0x10]
1003b50b:	8d 75 90             	lea    esi,[ebp-0x70]
1003b50e:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b50f:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b510:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b511:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b512:	8b 7d 14             	mov    edi,DWORD PTR [ebp+0x14]
1003b515:	8d 75 c0             	lea    esi,[ebp-0x40]
1003b518:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b519:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b51a:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b51b:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b51c:	8b 7d 18             	mov    edi,DWORD PTR [ebp+0x18]
1003b51f:	8d 75 d0             	lea    esi,[ebp-0x30]
1003b522:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b523:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b524:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b525:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b526:	8b 7d 1c             	mov    edi,DWORD PTR [ebp+0x1c]
1003b529:	8d 75 b0             	lea    esi,[ebp-0x50]
1003b52c:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b52d:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b52e:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b52f:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b530:	8b 7d 20             	mov    edi,DWORD PTR [ebp+0x20]
1003b533:	8d 75 a0             	lea    esi,[ebp-0x60]
1003b536:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b537:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b538:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b539:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003b53a:	5f                   	pop    edi
1003b53b:	5e                   	pop    esi
1003b53c:	c9                   	leave
1003b53d:	c2 1c 00             	ret    0x1c
