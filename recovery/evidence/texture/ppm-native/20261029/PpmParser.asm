
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

10015240 <.text+0x14240>:
10015240:	55                   	push   ebp
10015241:	8b ec                	mov    ebp,esp
10015243:	83 ec 14             	sub    esp,0x14
10015246:	83 7d 0c 02          	cmp    DWORD PTR [ebp+0xc],0x2
1001524a:	56                   	push   esi
1001524b:	57                   	push   edi
1001524c:	8b f1                	mov    esi,ecx
1001524e:	73 0a                	jae    0x1001525a
10015250:	b8 05 40 00 80       	mov    eax,0x80004005
10015255:	e9 be 01 00 00       	jmp    0x10015418
1001525a:	53                   	push   ebx
1001525b:	8b 5d 08             	mov    ebx,DWORD PTR [ebp+0x8]
1001525e:	8a 03                	mov    al,BYTE PTR [ebx]
10015260:	3c 50                	cmp    al,0x50
10015262:	0f 85 aa 01 00 00    	jne    0x10015412
10015268:	80 7b 01 33          	cmp    BYTE PTR [ebx+0x1],0x33
1001526c:	75 09                	jne    0x10015277
1001526e:	c7 45 ec 01 00 00 00 	mov    DWORD PTR [ebp-0x14],0x1
10015275:	eb 16                	jmp    0x1001528d
10015277:	3c 50                	cmp    al,0x50
10015279:	0f 85 93 01 00 00    	jne    0x10015412
1001527f:	80 7b 01 36          	cmp    BYTE PTR [ebx+0x1],0x36
10015283:	0f 85 89 01 00 00    	jne    0x10015412
10015289:	83 65 ec 00          	and    DWORD PTR [ebp-0x14],0x0
1001528d:	83 c3 02             	add    ebx,0x2
10015290:	83 6d 0c 02          	sub    DWORD PTR [ebp+0xc],0x2
10015294:	c7 45 08 00 00 00 00 	mov    DWORD PTR [ebp+0x8],0x0
1001529b:	c7 45 f0 00 00 00 00 	mov    DWORD PTR [ebp-0x10],0x0
100152a2:	c7 45 fc ff 00 00 00 	mov    DWORD PTR [ebp-0x4],0xff
100152a9:	0f 84 63 01 00 00    	je     0x10015412
100152af:	8b 7d 0c             	mov    edi,DWORD PTR [ebp+0xc]
100152b2:	83 7d ec 00          	cmp    DWORD PTR [ebp-0x14],0x0
100152b6:	75 0a                	jne    0x100152c2
100152b8:	83 7d 08 03          	cmp    DWORD PTR [ebp+0x8],0x3
100152bc:	0f 84 5c 01 00 00    	je     0x1001541e
100152c2:	0f b6 03             	movzx  eax,BYTE PTR [ebx]
100152c5:	50                   	push   eax
100152c6:	e8 e3 94 03 00       	call   0x1004e7ae
100152cb:	85 c0                	test   eax,eax
100152cd:	59                   	pop    ecx
100152ce:	75 15                	jne    0x100152e5
100152d0:	80 3b 23             	cmp    BYTE PTR [ebx],0x23
100152d3:	75 19                	jne    0x100152ee
100152d5:	39 45 0c             	cmp    DWORD PTR [ebp+0xc],eax
100152d8:	74 0b                	je     0x100152e5
100152da:	80 3b 0a             	cmp    BYTE PTR [ebx],0xa
100152dd:	74 06                	je     0x100152e5
100152df:	43                   	inc    ebx
100152e0:	ff 4d 0c             	dec    DWORD PTR [ebp+0xc]
100152e3:	75 f5                	jne    0x100152da
100152e5:	43                   	inc    ebx
100152e6:	ff 4d 0c             	dec    DWORD PTR [ebp+0xc]
100152e9:	e9 1a 01 00 00       	jmp    0x10015408
100152ee:	33 c0                	xor    eax,eax
100152f0:	39 45 0c             	cmp    DWORD PTR [ebp+0xc],eax
100152f3:	89 45 f8             	mov    DWORD PTR [ebp-0x8],eax
100152f6:	74 39                	je     0x10015331
100152f8:	0f b6 03             	movzx  eax,BYTE PTR [ebx]
100152fb:	50                   	push   eax
100152fc:	e8 ad 94 03 00       	call   0x1004e7ae
10015301:	85 c0                	test   eax,eax
10015303:	59                   	pop    ecx
10015304:	75 28                	jne    0x1001532e
10015306:	0f b6 03             	movzx  eax,BYTE PTR [ebx]
10015309:	50                   	push   eax
1001530a:	e8 cd 95 03 00       	call   0x1004e8dc
1001530f:	85 c0                	test   eax,eax
10015311:	59                   	pop    ecx
10015312:	0f 84 fa 00 00 00    	je     0x10015412
10015318:	8b 45 f8             	mov    eax,DWORD PTR [ebp-0x8]
1001531b:	0f b6 0b             	movzx  ecx,BYTE PTR [ebx]
1001531e:	6b c0 0a             	imul   eax,eax,0xa
10015321:	8d 44 08 d0          	lea    eax,[eax+ecx*1-0x30]
10015325:	43                   	inc    ebx
10015326:	ff 4d 0c             	dec    DWORD PTR [ebp+0xc]
10015329:	89 45 f8             	mov    DWORD PTR [ebp-0x8],eax
1001532c:	75 ca                	jne    0x100152f8
1001532e:	8b 45 f8             	mov    eax,DWORD PTR [ebp-0x8]
10015331:	8b 4d 08             	mov    ecx,DWORD PTR [ebp+0x8]
10015334:	83 e9 00             	sub    ecx,0x0
10015337:	0f 84 c1 00 00 00    	je     0x100153fe
1001533d:	49                   	dec    ecx
1001533e:	74 6c                	je     0x100153ac
10015340:	49                   	dec    ecx
10015341:	74 64                	je     0x100153a7
10015343:	49                   	dec    ecx
10015344:	74 41                	je     0x10015387
10015346:	49                   	dec    ecx
10015347:	74 2c                	je     0x10015375
10015349:	49                   	dec    ecx
1001534a:	0f 85 b5 00 00 00    	jne    0x10015405
10015350:	69 c0 ff 00 00 00    	imul   eax,eax,0xff
10015356:	33 d2                	xor    edx,edx
10015358:	f7 75 fc             	div    DWORD PTR [ebp-0x4]
1001535b:	09 07                	or     DWORD PTR [edi],eax
1001535d:	83 c7 04             	add    edi,0x4
10015360:	3b 7d f4             	cmp    edi,DWORD PTR [ebp-0xc]
10015363:	0f 84 36 01 00 00    	je     0x1001549f
10015369:	c7 45 08 02 00 00 00 	mov    DWORD PTR [ebp+0x8],0x2
10015370:	e9 90 00 00 00       	jmp    0x10015405
10015375:	69 c0 ff 00 00 00    	imul   eax,eax,0xff
1001537b:	33 d2                	xor    edx,edx
1001537d:	f7 75 fc             	div    DWORD PTR [ebp-0x4]
10015380:	c1 e0 08             	shl    eax,0x8
10015383:	09 07                	or     DWORD PTR [edi],eax
10015385:	eb 7e                	jmp    0x10015405
10015387:	3b 7d f4             	cmp    edi,DWORD PTR [ebp-0xc]
1001538a:	0f 83 82 00 00 00    	jae    0x10015412
10015390:	69 c0 ff 00 00 00    	imul   eax,eax,0xff
10015396:	33 d2                	xor    edx,edx
10015398:	f7 75 fc             	div    DWORD PTR [ebp-0x4]
1001539b:	0d 00 ff ff ff       	or     eax,0xffffff00
100153a0:	c1 e0 10             	shl    eax,0x10
100153a3:	89 07                	mov    DWORD PTR [edi],eax
100153a5:	eb 5e                	jmp    0x10015405
100153a7:	89 45 fc             	mov    DWORD PTR [ebp-0x4],eax
100153aa:	eb 55                	jmp    0x10015401
100153ac:	85 c0                	test   eax,eax
100153ae:	74 62                	je     0x10015412
100153b0:	0f af 45 f0          	imul   eax,DWORD PTR [ebp-0x10]
100153b4:	c1 e0 02             	shl    eax,0x2
100153b7:	50                   	push   eax
100153b8:	89 45 f4             	mov    DWORD PTR [ebp-0xc],eax
100153bb:	e8 b4 cc ff ff       	call   0x10012074
100153c0:	85 c0                	test   eax,eax
100153c2:	89 46 04             	mov    DWORD PTR [esi+0x4],eax
100153c5:	0f 84 db 00 00 00    	je     0x100154a6
100153cb:	8b f8                	mov    edi,eax
100153cd:	8b 45 f4             	mov    eax,DWORD PTR [ebp-0xc]
100153d0:	03 c7                	add    eax,edi
100153d2:	89 45 f4             	mov    DWORD PTR [ebp-0xc],eax
100153d5:	8b 45 f0             	mov    eax,DWORD PTR [ebp-0x10]
100153d8:	33 c9                	xor    ecx,ecx
100153da:	8b d0                	mov    edx,eax
100153dc:	41                   	inc    ecx
100153dd:	c1 e2 02             	shl    edx,0x2
100153e0:	83 66 34 00          	and    DWORD PTR [esi+0x34],0x0
100153e4:	89 46 0c             	mov    DWORD PTR [esi+0xc],eax
100153e7:	8b 45 f8             	mov    eax,DWORD PTR [ebp-0x8]
100153ea:	89 4e 38             	mov    DWORD PTR [esi+0x38],ecx
100153ed:	c7 06 86 00 28 28    	mov    DWORD PTR [esi],0x28280086
100153f3:	89 56 30             	mov    DWORD PTR [esi+0x30],edx
100153f6:	89 46 10             	mov    DWORD PTR [esi+0x10],eax
100153f9:	89 4e 14             	mov    DWORD PTR [esi+0x14],ecx
100153fc:	eb 07                	jmp    0x10015405
100153fe:	89 45 f0             	mov    DWORD PTR [ebp-0x10],eax
10015401:	85 c0                	test   eax,eax
10015403:	74 0d                	je     0x10015412
10015405:	ff 45 08             	inc    DWORD PTR [ebp+0x8]
10015408:	83 7d 0c 00          	cmp    DWORD PTR [ebp+0xc],0x0
1001540c:	0f 85 a0 fe ff ff    	jne    0x100152b2
10015412:	b8 05 40 00 80       	mov    eax,0x80004005
10015417:	5b                   	pop    ebx
10015418:	5f                   	pop    edi
10015419:	5e                   	pop    esi
1001541a:	c9                   	leave
1001541b:	c2 08 00             	ret    0x8
1001541e:	8b 4d fc             	mov    ecx,DWORD PTR [ebp-0x4]
10015421:	81 f9 ff 00 00 00    	cmp    ecx,0xff
10015427:	77 e9                	ja     0x10015412
10015429:	83 7d 0c 01          	cmp    DWORD PTR [ebp+0xc],0x1
1001542d:	76 09                	jbe    0x10015438
1001542f:	80 3b 0d             	cmp    BYTE PTR [ebx],0xd
10015432:	75 04                	jne    0x10015438
10015434:	43                   	inc    ebx
10015435:	ff 4d 0c             	dec    DWORD PTR [ebp+0xc]
10015438:	43                   	inc    ebx
10015439:	ff 4d 0c             	dec    DWORD PTR [ebp+0xc]
1001543c:	74 4e                	je     0x1001548c
1001543e:	3b 7d f4             	cmp    edi,DWORD PTR [ebp-0xc]
10015441:	73 49                	jae    0x1001548c
10015443:	0f b6 03             	movzx  eax,BYTE PTR [ebx]
10015446:	69 c0 ff 00 00 00    	imul   eax,eax,0xff
1001544c:	33 d2                	xor    edx,edx
1001544e:	f7 f1                	div    ecx
10015450:	33 d2                	xor    edx,edx
10015452:	83 c3 03             	add    ebx,0x3
10015455:	8b f0                	mov    esi,eax
10015457:	0f b6 43 fe          	movzx  eax,BYTE PTR [ebx-0x2]
1001545b:	69 c0 ff 00 00 00    	imul   eax,eax,0xff
10015461:	f7 f1                	div    ecx
10015463:	81 ce 00 ff ff ff    	or     esi,0xffffff00
10015469:	c1 e6 08             	shl    esi,0x8
1001546c:	33 d2                	xor    edx,edx
1001546e:	0b f0                	or     esi,eax
10015470:	0f b6 43 ff          	movzx  eax,BYTE PTR [ebx-0x1]
10015474:	69 c0 ff 00 00 00    	imul   eax,eax,0xff
1001547a:	f7 f1                	div    ecx
1001547c:	c1 e6 08             	shl    esi,0x8
1001547f:	0b f0                	or     esi,eax
10015481:	89 37                	mov    DWORD PTR [edi],esi
10015483:	83 c7 04             	add    edi,0x4
10015486:	83 6d 0c 03          	sub    DWORD PTR [ebp+0xc],0x3
1001548a:	75 b2                	jne    0x1001543e
1001548c:	8b c7                	mov    eax,edi
1001548e:	2b 45 f4             	sub    eax,DWORD PTR [ebp-0xc]
10015491:	f7 d8                	neg    eax
10015493:	1b c0                	sbb    eax,eax
10015495:	25 05 40 00 80       	and    eax,0x80004005
1001549a:	e9 78 ff ff ff       	jmp    0x10015417
1001549f:	33 c0                	xor    eax,eax
100154a1:	e9 71 ff ff ff       	jmp    0x10015417
100154a6:	b8 0e 00 07 80       	mov    eax,0x8007000e
100154ab:	e9 67 ff ff ff       	jmp    0x10015417
