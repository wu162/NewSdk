
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

10013173 <.text+0x12173>:
10013173:	55                   	push   ebp
10013174:	8b ec                	mov    ebp,esp
10013176:	83 ec 64             	sub    esp,0x64
10013179:	56                   	push   esi
1001317a:	8b 75 08             	mov    esi,DWORD PTR [ebp+0x8]
1001317d:	85 f6                	test   esi,esi
1001317f:	75 0a                	jne    0x1001318b
10013181:	b8 6c 08 76 88       	mov    eax,0x8876086c
10013186:	e9 28 03 00 00       	jmp    0x100134b3
1001318b:	53                   	push   ebx
1001318c:	56                   	push   esi
1001318d:	e8 f6 91 ff ff       	call   0x1000c388
10013192:	8b d8                	mov    ebx,eax
10013194:	83 e8 03             	sub    eax,0x3
10013197:	89 5d e8             	mov    DWORD PTR [ebp-0x18],ebx
1001319a:	74 12                	je     0x100131ae
1001319c:	83 e8 0e             	sub    eax,0xe
1001319f:	74 0d                	je     0x100131ae
100131a1:	48                   	dec    eax
100131a2:	74 0a                	je     0x100131ae
100131a4:	b8 6c 08 76 88       	mov    eax,0x8876086c
100131a9:	e9 04 03 00 00       	jmp    0x100134b2
100131ae:	83 7d 14 ff          	cmp    DWORD PTR [ebp+0x14],0xffffffff
100131b2:	57                   	push   edi
100131b3:	6a 05                	push   0x5
100131b5:	89 75 ec             	mov    DWORD PTR [ebp-0x14],esi
100131b8:	5f                   	pop    edi
100131b9:	75 3c                	jne    0x100131f7
100131bb:	8b c3                	mov    eax,ebx
100131bd:	83 e8 03             	sub    eax,0x3
100131c0:	74 7a                	je     0x1001323c
100131c2:	83 e8 0e             	sub    eax,0xe
100131c5:	74 4e                	je     0x10013215
100131c7:	48                   	dec    eax
100131c8:	0f 85 94 00 00 00    	jne    0x10013262
100131ce:	8d 45 9c             	lea    eax,[ebp-0x64]
100131d1:	50                   	push   eax
100131d2:	6a 00                	push   0x0
100131d4:	ff 75 ec             	push   DWORD PTR [ebp-0x14]
100131d7:	e8 84 a9 ff ff       	call   0x1000db60
100131dc:	8b 45 b4             	mov    eax,DWORD PTR [ebp-0x4c]
100131df:	8d 48 ff             	lea    ecx,[eax-0x1]
100131e2:	85 c8                	test   eax,ecx
100131e4:	75 0a                	jne    0x100131f0
100131e6:	8b 45 b8             	mov    eax,DWORD PTR [ebp-0x48]
100131e9:	8d 48 ff             	lea    ecx,[eax-0x1]
100131ec:	85 c8                	test   eax,ecx
100131ee:	74 78                	je     0x10013268
100131f0:	c7 45 14 04 00 08 00 	mov    DWORD PTR [ebp+0x14],0x80004
100131f7:	83 fb 12             	cmp    ebx,0x12
100131fa:	75 07                	jne    0x10013203
100131fc:	81 4d 14 00 00 07 00 	or     DWORD PTR [ebp+0x14],0x70000
10013203:	f7 45 14 00 00 40 00 	test   DWORD PTR [ebp+0x14],0x400000
1001320a:	74 61                	je     0x1001326d
1001320c:	81 4d 14 00 00 60 00 	or     DWORD PTR [ebp+0x14],0x600000
10013213:	eb 5f                	jmp    0x10013274
10013215:	8d 45 bc             	lea    eax,[ebp-0x44]
10013218:	50                   	push   eax
10013219:	6a 00                	push   0x0
1001321b:	ff 75 ec             	push   DWORD PTR [ebp-0x14]
1001321e:	e8 d6 a9 ff ff       	call   0x1000dbf9
10013223:	8b 45 cc             	mov    eax,DWORD PTR [ebp-0x34]
10013226:	8d 48 ff             	lea    ecx,[eax-0x1]
10013229:	85 c8                	test   eax,ecx
1001322b:	75 c3                	jne    0x100131f0
1001322d:	8b 45 d0             	mov    eax,DWORD PTR [ebp-0x30]
10013230:	8d 48 ff             	lea    ecx,[eax-0x1]
10013233:	85 c8                	test   eax,ecx
10013235:	75 b9                	jne    0x100131f0
10013237:	8b 45 d4             	mov    eax,DWORD PTR [ebp-0x2c]
1001323a:	eb ad                	jmp    0x100131e9
1001323c:	8d 45 9c             	lea    eax,[ebp-0x64]
1001323f:	50                   	push   eax
10013240:	6a 00                	push   0x0
10013242:	ff 75 ec             	push   DWORD PTR [ebp-0x14]
10013245:	e8 16 a9 ff ff       	call   0x1000db60
1001324a:	8b 45 b4             	mov    eax,DWORD PTR [ebp-0x4c]
1001324d:	8d 48 ff             	lea    ecx,[eax-0x1]
10013250:	85 c8                	test   eax,ecx
10013252:	75 0a                	jne    0x1001325e
10013254:	8b 45 b8             	mov    eax,DWORD PTR [ebp-0x48]
10013257:	8d 48 ff             	lea    ecx,[eax-0x1]
1001325a:	85 c8                	test   eax,ecx
1001325c:	74 0a                	je     0x10013268
1001325e:	83 65 14 00          	and    DWORD PTR [ebp+0x14],0x0
10013262:	83 7d 14 00          	cmp    DWORD PTR [ebp+0x14],0x0
10013266:	74 88                	je     0x100131f0
10013268:	89 7d 14             	mov    DWORD PTR [ebp+0x14],edi
1001326b:	eb 8a                	jmp    0x100131f7
1001326d:	81 65 14 ff ff 9f ff 	and    DWORD PTR [ebp+0x14],0xff9fffff
10013274:	56                   	push   esi
10013275:	e8 45 a8 ff ff       	call   0x1000dabf
1001327a:	83 7d 10 ff          	cmp    DWORD PTR [ebp+0x10],0xffffffff
1001327e:	89 45 dc             	mov    DWORD PTR [ebp-0x24],eax
10013281:	75 04                	jne    0x10013287
10013283:	83 65 10 00          	and    DWORD PTR [ebp+0x10],0x0
10013287:	39 45 10             	cmp    DWORD PTR [ebp+0x10],eax
1001328a:	72 0a                	jb     0x10013296
1001328c:	b8 6c 08 76 88       	mov    eax,0x8876086c
10013291:	e9 1b 02 00 00       	jmp    0x100134b1
10013296:	33 c9                	xor    ecx,ecx
10013298:	33 c0                	xor    eax,eax
1001329a:	83 fb 12             	cmp    ebx,0x12
1001329d:	0f 95 c0             	setne  al
100132a0:	89 4d 08             	mov    DWORD PTR [ebp+0x8],ecx
100132a3:	89 4d fc             	mov    DWORD PTR [ebp-0x4],ecx
100132a6:	89 4d f8             	mov    DWORD PTR [ebp-0x8],ecx
100132a9:	89 4d f4             	mov    DWORD PTR [ebp-0xc],ecx
100132ac:	48                   	dec    eax
100132ad:	23 c7                	and    eax,edi
100132af:	40                   	inc    eax
100132b0:	89 45 d8             	mov    DWORD PTR [ebp-0x28],eax
100132b3:	8b 45 14             	mov    eax,DWORD PTR [ebp+0x14]
100132b6:	25 ff 00 00 00       	and    eax,0xff
100132bb:	83 f8 02             	cmp    eax,0x2
100132be:	74 07                	je     0x100132c7
100132c0:	3b c7                	cmp    eax,edi
100132c2:	89 4d e4             	mov    DWORD PTR [ebp-0x1c],ecx
100132c5:	75 07                	jne    0x100132ce
100132c7:	c7 45 e4 01 00 00 00 	mov    DWORD PTR [ebp-0x1c],0x1
100132ce:	39 4d d8             	cmp    DWORD PTR [ebp-0x28],ecx
100132d1:	89 4d f0             	mov    DWORD PTR [ebp-0x10],ecx
100132d4:	0f 86 9b 01 00 00    	jbe    0x10013475
100132da:	8b 75 14             	mov    esi,DWORD PTR [ebp+0x14]
100132dd:	bb f2 ff f8 7f       	mov    ebx,0x7ff8fff2
100132e2:	bf 0e 00 07 80       	mov    edi,0x8007000e
100132e7:	8b 45 e8             	mov    eax,DWORD PTR [ebp-0x18]
100132ea:	83 e8 03             	sub    eax,0x3
100132ed:	74 2a                	je     0x10013319
100132ef:	83 e8 0e             	sub    eax,0xe
100132f2:	74 13                	je     0x10013307
100132f4:	48                   	dec    eax
100132f5:	75 3a                	jne    0x10013331
100132f7:	ff 75 10             	push   DWORD PTR [ebp+0x10]
100132fa:	ff 75 f0             	push   DWORD PTR [ebp-0x10]
100132fd:	ff 75 ec             	push   DWORD PTR [ebp-0x14]
10013300:	e8 60 a8 ff ff       	call   0x1000db65
10013305:	eb 1d                	jmp    0x10013324
10013307:	ff 75 10             	push   DWORD PTR [ebp+0x10]
1001330a:	ff 75 ec             	push   DWORD PTR [ebp-0x14]
1001330d:	e8 ec a8 ff ff       	call   0x1000dbfe
10013312:	8b f0                	mov    esi,eax
10013314:	89 75 fc             	mov    DWORD PTR [ebp-0x4],esi
10013317:	eb 10                	jmp    0x10013329
10013319:	ff 75 10             	push   DWORD PTR [ebp+0x10]
1001331c:	ff 75 ec             	push   DWORD PTR [ebp-0x14]
1001331f:	e8 ac a7 ff ff       	call   0x1000dad0
10013324:	8b f0                	mov    esi,eax
10013326:	89 75 08             	mov    DWORD PTR [ebp+0x8],esi
10013329:	f7 de                	neg    esi
1001332b:	1b f6                	sbb    esi,esi
1001332d:	23 f3                	and    esi,ebx
1001332f:	03 f7                	add    esi,edi
10013331:	85 f6                	test   esi,esi
10013333:	0f 8c 3e 01 00 00    	jl     0x10013477
10013339:	8b 4d 10             	mov    ecx,DWORD PTR [ebp+0x10]
1001333c:	e9 f4 00 00 00       	jmp    0x10013435
10013341:	8b 45 e8             	mov    eax,DWORD PTR [ebp-0x18]
10013344:	83 e8 03             	sub    eax,0x3
10013347:	74 26                	je     0x1001336f
10013349:	83 e8 0e             	sub    eax,0xe
1001334c:	74 11                	je     0x1001335f
1001334e:	48                   	dec    eax
1001334f:	75 34                	jne    0x10013385
10013351:	51                   	push   ecx
10013352:	ff 75 f0             	push   DWORD PTR [ebp-0x10]
10013355:	ff 75 ec             	push   DWORD PTR [ebp-0x14]
10013358:	e8 08 a8 ff ff       	call   0x1000db65
1001335d:	eb 19                	jmp    0x10013378
1001335f:	51                   	push   ecx
10013360:	ff 75 ec             	push   DWORD PTR [ebp-0x14]
10013363:	e8 96 a8 ff ff       	call   0x1000dbfe
10013368:	8b f0                	mov    esi,eax
1001336a:	89 75 f4             	mov    DWORD PTR [ebp-0xc],esi
1001336d:	eb 0e                	jmp    0x1001337d
1001336f:	51                   	push   ecx
10013370:	ff 75 ec             	push   DWORD PTR [ebp-0x14]
10013373:	e8 58 a7 ff ff       	call   0x1000dad0
10013378:	8b f0                	mov    esi,eax
1001337a:	89 75 f8             	mov    DWORD PTR [ebp-0x8],esi
1001337d:	f7 de                	neg    esi
1001337f:	1b f6                	sbb    esi,esi
10013381:	23 f3                	and    esi,ebx
10013383:	03 f7                	add    esi,edi
10013385:	85 f6                	test   esi,esi
10013387:	0f 8c ea 00 00 00    	jl     0x10013477
1001338d:	8b 45 e8             	mov    eax,DWORD PTR [ebp-0x18]
10013390:	83 e8 03             	sub    eax,0x3
10013393:	74 08                	je     0x1001339d
10013395:	83 e8 0e             	sub    eax,0xe
10013398:	74 5a                	je     0x100133f4
1001339a:	48                   	dec    eax
1001339b:	75 1b                	jne    0x100133b8
1001339d:	33 c0                	xor    eax,eax
1001339f:	50                   	push   eax
100133a0:	ff 75 14             	push   DWORD PTR [ebp+0x14]
100133a3:	50                   	push   eax
100133a4:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
100133a7:	ff 75 08             	push   DWORD PTR [ebp+0x8]
100133aa:	50                   	push   eax
100133ab:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
100133ae:	ff 75 f8             	push   DWORD PTR [ebp-0x8]
100133b1:	e8 33 fc ff ff       	call   0x10012fe9
100133b6:	8b f0                	mov    esi,eax
100133b8:	85 f6                	test   esi,esi
100133ba:	0f 8c b7 00 00 00    	jl     0x10013477
100133c0:	33 c0                	xor    eax,eax
100133c2:	39 45 e4             	cmp    DWORD PTR [ebp-0x1c],eax
100133c5:	74 48                	je     0x1001340f
100133c7:	39 45 08             	cmp    DWORD PTR [ebp+0x8],eax
100133ca:	74 08                	je     0x100133d4
100133cc:	ff 75 08             	push   DWORD PTR [ebp+0x8]
100133cf:	e8 fe 91 ff ff       	call   0x1000c5d2
100133d4:	83 7d fc 00          	cmp    DWORD PTR [ebp-0x4],0x0
100133d8:	74 08                	je     0x100133e2
100133da:	ff 75 fc             	push   DWORD PTR [ebp-0x4]
100133dd:	e8 f0 91 ff ff       	call   0x1000c5d2
100133e2:	8b 45 f8             	mov    eax,DWORD PTR [ebp-0x8]
100133e5:	83 65 f8 00          	and    DWORD PTR [ebp-0x8],0x0
100133e9:	89 45 08             	mov    DWORD PTR [ebp+0x8],eax
100133ec:	8b 45 f4             	mov    eax,DWORD PTR [ebp-0xc]
100133ef:	89 45 fc             	mov    DWORD PTR [ebp-0x4],eax
100133f2:	eb 3a                	jmp    0x1001342e
100133f4:	33 c0                	xor    eax,eax
100133f6:	50                   	push   eax
100133f7:	ff 75 14             	push   DWORD PTR [ebp+0x14]
100133fa:	50                   	push   eax
100133fb:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
100133fe:	ff 75 fc             	push   DWORD PTR [ebp-0x4]
10013401:	50                   	push   eax
10013402:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
10013405:	ff 75 f4             	push   DWORD PTR [ebp-0xc]
10013408:	e8 9e fc ff ff       	call   0x100130ab
1001340d:	eb a7                	jmp    0x100133b6
1001340f:	39 45 f8             	cmp    DWORD PTR [ebp-0x8],eax
10013412:	74 0c                	je     0x10013420
10013414:	ff 75 f8             	push   DWORD PTR [ebp-0x8]
10013417:	e8 b6 91 ff ff       	call   0x1000c5d2
1001341c:	83 65 f8 00          	and    DWORD PTR [ebp-0x8],0x0
10013420:	83 7d f4 00          	cmp    DWORD PTR [ebp-0xc],0x0
10013424:	74 0c                	je     0x10013432
10013426:	ff 75 f4             	push   DWORD PTR [ebp-0xc]
10013429:	e8 a4 91 ff ff       	call   0x1000c5d2
1001342e:	83 65 f4 00          	and    DWORD PTR [ebp-0xc],0x0
10013432:	8b 4d e0             	mov    ecx,DWORD PTR [ebp-0x20]
10013435:	41                   	inc    ecx
10013436:	3b 4d dc             	cmp    ecx,DWORD PTR [ebp-0x24]
10013439:	89 4d e0             	mov    DWORD PTR [ebp-0x20],ecx
1001343c:	0f 82 ff fe ff ff    	jb     0x10013341
10013442:	83 7d 08 00          	cmp    DWORD PTR [ebp+0x8],0x0
10013446:	74 0c                	je     0x10013454
10013448:	ff 75 08             	push   DWORD PTR [ebp+0x8]
1001344b:	e8 82 91 ff ff       	call   0x1000c5d2
10013450:	83 65 08 00          	and    DWORD PTR [ebp+0x8],0x0
10013454:	83 7d fc 00          	cmp    DWORD PTR [ebp-0x4],0x0
10013458:	74 0c                	je     0x10013466
1001345a:	ff 75 fc             	push   DWORD PTR [ebp-0x4]
1001345d:	e8 70 91 ff ff       	call   0x1000c5d2
10013462:	83 65 fc 00          	and    DWORD PTR [ebp-0x4],0x0
10013466:	ff 45 f0             	inc    DWORD PTR [ebp-0x10]
10013469:	8b 45 f0             	mov    eax,DWORD PTR [ebp-0x10]
1001346c:	3b 45 d8             	cmp    eax,DWORD PTR [ebp-0x28]
1001346f:	0f 82 72 fe ff ff    	jb     0x100132e7
10013475:	33 f6                	xor    esi,esi
10013477:	83 7d 08 00          	cmp    DWORD PTR [ebp+0x8],0x0
1001347b:	74 08                	je     0x10013485
1001347d:	ff 75 08             	push   DWORD PTR [ebp+0x8]
10013480:	e8 4d 91 ff ff       	call   0x1000c5d2
10013485:	83 7d fc 00          	cmp    DWORD PTR [ebp-0x4],0x0
10013489:	74 08                	je     0x10013493
1001348b:	ff 75 fc             	push   DWORD PTR [ebp-0x4]
1001348e:	e8 3f 91 ff ff       	call   0x1000c5d2
10013493:	83 7d f8 00          	cmp    DWORD PTR [ebp-0x8],0x0
10013497:	74 08                	je     0x100134a1
10013499:	ff 75 f8             	push   DWORD PTR [ebp-0x8]
1001349c:	e8 31 91 ff ff       	call   0x1000c5d2
100134a1:	83 7d f4 00          	cmp    DWORD PTR [ebp-0xc],0x0
100134a5:	74 08                	je     0x100134af
100134a7:	ff 75 f4             	push   DWORD PTR [ebp-0xc]
100134aa:	e8 23 91 ff ff       	call   0x1000c5d2
100134af:	8b c6                	mov    eax,esi
100134b1:	5f                   	pop    edi
100134b2:	5b                   	pop    ebx
100134b3:	5e                   	pop    esi
100134b4:	c9                   	leave
100134b5:	c2 10 00             	ret    0x10
