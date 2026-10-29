
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000d24b <.text+0xc24b>:
1000d24b:	55                   	push   ebp
1000d24c:	8b ec                	mov    ebp,esp
1000d24e:	83 ec 40             	sub    esp,0x40
1000d251:	83 65 c8 00          	and    DWORD PTR [ebp-0x38],0x0
1000d255:	53                   	push   ebx
1000d256:	56                   	push   esi
1000d257:	8b 75 08             	mov    esi,DWORD PTR [ebp+0x8]
1000d25a:	8b 46 30             	mov    eax,DWORD PTR [esi+0x30]
1000d25d:	8b 5e 1c             	mov    ebx,DWORD PTR [esi+0x1c]
1000d260:	c1 e8 09             	shr    eax,0x9
1000d263:	83 e0 03             	and    eax,0x3
1000d266:	89 45 ec             	mov    DWORD PTR [ebp-0x14],eax
1000d269:	8b 46 20             	mov    eax,DWORD PTR [esi+0x20]
1000d26c:	83 e0 3f             	and    eax,0x3f
1000d26f:	0f b6 04 45 30 82 09 	movzx  eax,BYTE PTR [eax*2+0x10098230]
1000d276:	10 
1000d277:	57                   	push   edi
1000d278:	8b 7e 28             	mov    edi,DWORD PTR [esi+0x28]
1000d27b:	89 45 dc             	mov    DWORD PTR [ebp-0x24],eax
1000d27e:	c1 ef 1f             	shr    edi,0x1f
1000d281:	8d 04 3f             	lea    eax,[edi+edi*1]
1000d284:	56                   	push   esi
1000d285:	c1 eb 1f             	shr    ebx,0x1f
1000d288:	89 45 e8             	mov    DWORD PTR [ebp-0x18],eax
1000d28b:	e8 f8 f0 ff ff       	call   0x1000c388
1000d290:	8d 45 e0             	lea    eax,[ebp-0x20]
1000d293:	50                   	push   eax
1000d294:	8d 45 d8             	lea    eax,[ebp-0x28]
1000d297:	50                   	push   eax
1000d298:	8d 45 d0             	lea    eax,[ebp-0x30]
1000d29b:	50                   	push   eax
1000d29c:	56                   	push   esi
1000d29d:	e8 86 f3 ff ff       	call   0x1000c628
1000d2a2:	8b 45 d0             	mov    eax,DWORD PTR [ebp-0x30]
1000d2a5:	2b 45 e8             	sub    eax,DWORD PTR [ebp-0x18]
1000d2a8:	89 45 cc             	mov    DWORD PTR [ebp-0x34],eax
1000d2ab:	48                   	dec    eax
1000d2ac:	50                   	push   eax
1000d2ad:	e8 fc f4 ff ff       	call   0x1000c7ae
1000d2b2:	8b cf                	mov    ecx,edi
1000d2b4:	2b c8                	sub    ecx,eax
1000d2b6:	33 c0                	xor    eax,eax
1000d2b8:	40                   	inc    eax
1000d2b9:	83 c1 20             	add    ecx,0x20
1000d2bc:	d3 e0                	shl    eax,cl
1000d2be:	89 45 f8             	mov    DWORD PTR [ebp-0x8],eax
1000d2c1:	8b 45 d8             	mov    eax,DWORD PTR [ebp-0x28]
1000d2c4:	2b 45 e8             	sub    eax,DWORD PTR [ebp-0x18]
1000d2c7:	89 45 f0             	mov    DWORD PTR [ebp-0x10],eax
1000d2ca:	48                   	dec    eax
1000d2cb:	50                   	push   eax
1000d2cc:	e8 dd f4 ff ff       	call   0x1000c7ae
1000d2d1:	33 d2                	xor    edx,edx
1000d2d3:	8b cf                	mov    ecx,edi
1000d2d5:	2b c8                	sub    ecx,eax
1000d2d7:	42                   	inc    edx
1000d2d8:	83 c1 20             	add    ecx,0x20
1000d2db:	8b c2                	mov    eax,edx
1000d2dd:	d3 e0                	shl    eax,cl
1000d2df:	83 7d ec 02          	cmp    DWORD PTR [ebp-0x14],0x2
1000d2e3:	89 45 08             	mov    DWORD PTR [ebp+0x8],eax
1000d2e6:	75 24                	jne    0x1000d30c
1000d2e8:	8b 45 e0             	mov    eax,DWORD PTR [ebp-0x20]
1000d2eb:	2b 45 e8             	sub    eax,DWORD PTR [ebp-0x18]
1000d2ee:	48                   	dec    eax
1000d2ef:	50                   	push   eax
1000d2f0:	e8 b9 f4 ff ff       	call   0x1000c7ae
1000d2f5:	8b cf                	mov    ecx,edi
1000d2f7:	2b c8                	sub    ecx,eax
1000d2f9:	33 c0                	xor    eax,eax
1000d2fb:	40                   	inc    eax
1000d2fc:	83 c1 20             	add    ecx,0x20
1000d2ff:	d3 e0                	shl    eax,cl
1000d301:	33 d2                	xor    edx,edx
1000d303:	42                   	inc    edx
1000d304:	89 45 fc             	mov    DWORD PTR [ebp-0x4],eax
1000d307:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1000d30a:	eb 03                	jmp    0x1000d30f
1000d30c:	89 55 fc             	mov    DWORD PTR [ebp-0x4],edx
1000d30f:	83 7d f8 10          	cmp    DWORD PTR [ebp-0x8],0x10
1000d313:	76 05                	jbe    0x1000d31a
1000d315:	83 f8 10             	cmp    eax,0x10
1000d318:	77 0d                	ja     0x1000d327
1000d31a:	66 f7 46 30 00 08    	test   WORD PTR [esi+0x30],0x800
1000d320:	74 05                	je     0x1000d327
1000d322:	89 55 d4             	mov    DWORD PTR [ebp-0x2c],edx
1000d325:	eb 04                	jmp    0x1000d32b
1000d327:	83 65 d4 00          	and    DWORD PTR [ebp-0x2c],0x0
1000d32b:	8b 7d 10             	mov    edi,DWORD PTR [ebp+0x10]
1000d32e:	85 ff                	test   edi,edi
1000d330:	74 0b                	je     0x1000d33d
1000d332:	83 7d d4 00          	cmp    DWORD PTR [ebp-0x2c],0x0
1000d336:	75 05                	jne    0x1000d33d
1000d338:	8b 46 30             	mov    eax,DWORD PTR [esi+0x30]
1000d33b:	eb 03                	jmp    0x1000d340
1000d33d:	8b 46 20             	mov    eax,DWORD PTR [esi+0x20]
1000d340:	c1 e8 0c             	shr    eax,0xc
1000d343:	c1 e0 0c             	shl    eax,0xc
1000d346:	85 ff                	test   edi,edi
1000d348:	89 45 e4             	mov    DWORD PTR [ebp-0x1c],eax
1000d34b:	0f 85 c6 00 00 00    	jne    0x1000d417
1000d351:	39 7d d4             	cmp    DWORD PTR [ebp-0x2c],edi
1000d354:	0f 85 bd 00 00 00    	jne    0x1000d417
1000d35a:	8b 45 d0             	mov    eax,DWORD PTR [ebp-0x30]
1000d35d:	8b 7e 30             	mov    edi,DWORD PTR [esi+0x30]
1000d360:	89 45 f8             	mov    DWORD PTR [ebp-0x8],eax
1000d363:	8b 45 d8             	mov    eax,DWORD PTR [ebp-0x28]
1000d366:	89 45 08             	mov    DWORD PTR [ebp+0x8],eax
1000d369:	8b 45 e0             	mov    eax,DWORD PTR [ebp-0x20]
1000d36c:	53                   	push   ebx
1000d36d:	89 45 fc             	mov    DWORD PTR [ebp-0x4],eax
1000d370:	8b 46 20             	mov    eax,DWORD PTR [esi+0x20]
1000d373:	c1 ef 09             	shr    edi,0x9
1000d376:	89 45 10             	mov    DWORD PTR [ebp+0x10],eax
1000d379:	83 65 10 3f          	and    DWORD PTR [ebp+0x10],0x3f
1000d37d:	83 e7 03             	and    edi,0x3
1000d380:	57                   	push   edi
1000d381:	ff 75 10             	push   DWORD PTR [ebp+0x10]
1000d384:	8d 45 fc             	lea    eax,[ebp-0x4]
1000d387:	ff 75 dc             	push   DWORD PTR [ebp-0x24]
1000d38a:	50                   	push   eax
1000d38b:	8d 45 08             	lea    eax,[ebp+0x8]
1000d38e:	50                   	push   eax
1000d38f:	8d 45 f8             	lea    eax,[ebp-0x8]
1000d392:	50                   	push   eax
1000d393:	e8 85 f5 ff ff       	call   0x1000c91d
1000d398:	8b 5e 1c             	mov    ebx,DWORD PTR [esi+0x1c]
1000d39b:	b8 00 00 00 80       	mov    eax,0x80000000
1000d3a0:	85 d8                	test   eax,ebx
1000d3a2:	75 44                	jne    0x1000d3e8
1000d3a4:	66 f7 46 20 00 04    	test   WORD PTR [esi+0x20],0x400
1000d3aa:	75 3c                	jne    0x1000d3e8
1000d3ac:	66 f7 46 30 00 08    	test   WORD PTR [esi+0x30],0x800
1000d3b2:	75 34                	jne    0x1000d3e8
1000d3b4:	83 ff 01             	cmp    edi,0x1
1000d3b7:	75 2f                	jne    0x1000d3e8
1000d3b9:	66 f7 46 2c c0 03    	test   WORD PTR [esi+0x2c],0x3c0
1000d3bf:	75 27                	jne    0x1000d3e8
1000d3c1:	85 46 28             	test   DWORD PTR [esi+0x28],eax
1000d3c4:	75 22                	jne    0x1000d3e8
1000d3c6:	8d 45 08             	lea    eax,[ebp+0x8]
1000d3c9:	50                   	push   eax
1000d3ca:	8d 45 10             	lea    eax,[ebp+0x10]
1000d3cd:	50                   	push   eax
1000d3ce:	ff 75 10             	push   DWORD PTR [ebp+0x10]
1000d3d1:	e8 ee f3 ff ff       	call   0x1000c7c4
1000d3d6:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1000d3d9:	8b 4d d8             	mov    ecx,DWORD PTR [ebp-0x28]
1000d3dc:	8d 4c 08 ff          	lea    ecx,[eax+ecx*1-0x1]
1000d3e0:	48                   	dec    eax
1000d3e1:	f7 d0                	not    eax
1000d3e3:	23 c8                	and    ecx,eax
1000d3e5:	89 4d 08             	mov    DWORD PTR [ebp+0x8],ecx
1000d3e8:	83 7d ec 00          	cmp    DWORD PTR [ebp-0x14],0x0
1000d3ec:	75 0f                	jne    0x1000d3fd
1000d3ee:	8b 7d f8             	mov    edi,DWORD PTR [ebp-0x8]
1000d3f1:	0f af 7d dc          	imul   edi,DWORD PTR [ebp-0x24]
1000d3f5:	c1 ef 03             	shr    edi,0x3
1000d3f8:	e9 fd 01 00 00       	jmp    0x1000d5fa
1000d3fd:	c1 eb 16             	shr    ebx,0x16
1000d400:	81 e3 ff 01 00 00    	and    ebx,0x1ff
1000d406:	0f af 5d dc          	imul   ebx,DWORD PTR [ebp-0x24]
1000d40a:	c1 e3 05             	shl    ebx,0x5
1000d40d:	c1 eb 03             	shr    ebx,0x3
1000d410:	8b fb                	mov    edi,ebx
1000d412:	e9 e3 01 00 00       	jmp    0x1000d5fa
1000d417:	8b 45 cc             	mov    eax,DWORD PTR [ebp-0x34]
1000d41a:	8b cf                	mov    ecx,edi
1000d41c:	d3 e8                	shr    eax,cl
1000d41e:	3b c2                	cmp    eax,edx
1000d420:	8b c8                	mov    ecx,eax
1000d422:	77 02                	ja     0x1000d426
1000d424:	8b ca                	mov    ecx,edx
1000d426:	8b 45 e8             	mov    eax,DWORD PTR [ebp-0x18]
1000d429:	8b 5d f0             	mov    ebx,DWORD PTR [ebp-0x10]
1000d42c:	03 c8                	add    ecx,eax
1000d42e:	89 4d d0             	mov    DWORD PTR [ebp-0x30],ecx
1000d431:	8b cf                	mov    ecx,edi
1000d433:	d3 eb                	shr    ebx,cl
1000d435:	3b da                	cmp    ebx,edx
1000d437:	77 02                	ja     0x1000d43b
1000d439:	8b da                	mov    ebx,edx
1000d43b:	03 d8                	add    ebx,eax
1000d43d:	83 7d ec 02          	cmp    DWORD PTR [ebp-0x14],0x2
1000d441:	89 5d d8             	mov    DWORD PTR [ebp-0x28],ebx
1000d444:	75 13                	jne    0x1000d459
1000d446:	8b 4d e0             	mov    ecx,DWORD PTR [ebp-0x20]
1000d449:	2b c8                	sub    ecx,eax
1000d44b:	8b d9                	mov    ebx,ecx
1000d44d:	8b cf                	mov    ecx,edi
1000d44f:	d3 eb                	shr    ebx,cl
1000d451:	3b da                	cmp    ebx,edx
1000d453:	76 02                	jbe    0x1000d457
1000d455:	8b d3                	mov    edx,ebx
1000d457:	03 d0                	add    edx,eax
1000d459:	56                   	push   esi
1000d45a:	89 55 e0             	mov    DWORD PTR [ebp-0x20],edx
1000d45d:	e8 26 ef ff ff       	call   0x1000c388
1000d462:	83 f8 12             	cmp    eax,0x12
1000d465:	74 0e                	je     0x1000d475
1000d467:	83 f8 13             	cmp    eax,0x13
1000d46a:	74 09                	je     0x1000d475
1000d46c:	c7 45 cc 01 00 00 00 	mov    DWORD PTR [ebp-0x34],0x1
1000d473:	eb 1f                	jmp    0x1000d494
1000d475:	8b 4e 20             	mov    ecx,DWORD PTR [esi+0x20]
1000d478:	8b 46 24             	mov    eax,DWORD PTR [esi+0x24]
1000d47b:	33 d2                	xor    edx,edx
1000d47d:	42                   	inc    edx
1000d47e:	c1 e9 0a             	shr    ecx,0xa
1000d481:	23 ca                	and    ecx,edx
1000d483:	03 c9                	add    ecx,ecx
1000d485:	d3 e2                	shl    edx,cl
1000d487:	c1 e8 1a             	shr    eax,0x1a
1000d48a:	03 c2                	add    eax,edx
1000d48c:	4a                   	dec    edx
1000d48d:	f7 d2                	not    edx
1000d48f:	23 c2                	and    eax,edx
1000d491:	89 45 cc             	mov    DWORD PTR [ebp-0x34],eax
1000d494:	ff 75 f8             	push   DWORD PTR [ebp-0x8]
1000d497:	e8 12 f3 ff ff       	call   0x1000c7ae
1000d49c:	6a 1f                	push   0x1f
1000d49e:	5b                   	pop    ebx
1000d49f:	ff 75 08             	push   DWORD PTR [ebp+0x8]
1000d4a2:	8b cb                	mov    ecx,ebx
1000d4a4:	2b c8                	sub    ecx,eax
1000d4a6:	89 4d f4             	mov    DWORD PTR [ebp-0xc],ecx
1000d4a9:	e8 00 f3 ff ff       	call   0x1000c7ae
1000d4ae:	ff 75 fc             	push   DWORD PTR [ebp-0x4]
1000d4b1:	2b d8                	sub    ebx,eax
1000d4b3:	89 5d ec             	mov    DWORD PTR [ebp-0x14],ebx
1000d4b6:	e8 f3 f2 ff ff       	call   0x1000c7ae
1000d4bb:	6a 1f                	push   0x1f
1000d4bd:	59                   	pop    ecx
1000d4be:	2b c8                	sub    ecx,eax
1000d4c0:	83 7d d4 00          	cmp    DWORD PTR [ebp-0x2c],0x0
1000d4c4:	89 4d f0             	mov    DWORD PTR [ebp-0x10],ecx
1000d4c7:	74 0d                	je     0x1000d4d6
1000d4c9:	ff 45 f4             	inc    DWORD PTR [ebp-0xc]
1000d4cc:	43                   	inc    ebx
1000d4cd:	41                   	inc    ecx
1000d4ce:	89 5d ec             	mov    DWORD PTR [ebp-0x14],ebx
1000d4d1:	89 4d f0             	mov    DWORD PTR [ebp-0x10],ecx
1000d4d4:	eb 01                	jmp    0x1000d4d7
1000d4d6:	4f                   	dec    edi
1000d4d7:	85 ff                	test   edi,edi
1000d4d9:	89 7d e8             	mov    DWORD PTR [ebp-0x18],edi
1000d4dc:	0f 8c 15 01 00 00    	jl     0x1000d5f7
1000d4e2:	8b 46 1c             	mov    eax,DWORD PTR [esi+0x1c]
1000d4e5:	8b 56 30             	mov    edx,DWORD PTR [esi+0x30]
1000d4e8:	c1 e8 1f             	shr    eax,0x1f
1000d4eb:	89 45 c4             	mov    DWORD PTR [ebp-0x3c],eax
1000d4ee:	8b 46 20             	mov    eax,DWORD PTR [esi+0x20]
1000d4f1:	83 e0 3f             	and    eax,0x3f
1000d4f4:	c1 ea 09             	shr    edx,0x9
1000d4f7:	83 e2 03             	and    edx,0x3
1000d4fa:	89 45 10             	mov    DWORD PTR [ebp+0x10],eax
1000d4fd:	89 55 d4             	mov    DWORD PTR [ebp-0x2c],edx
1000d500:	eb 03                	jmp    0x1000d505
1000d502:	8b 55 d4             	mov    edx,DWORD PTR [ebp-0x2c]
1000d505:	33 c0                	xor    eax,eax
1000d507:	39 45 f4             	cmp    DWORD PTR [ebp-0xc],eax
1000d50a:	76 03                	jbe    0x1000d50f
1000d50c:	ff 4d f4             	dec    DWORD PTR [ebp-0xc]
1000d50f:	39 45 ec             	cmp    DWORD PTR [ebp-0x14],eax
1000d512:	76 03                	jbe    0x1000d517
1000d514:	ff 4d ec             	dec    DWORD PTR [ebp-0x14]
1000d517:	39 45 f0             	cmp    DWORD PTR [ebp-0x10],eax
1000d51a:	76 03                	jbe    0x1000d51f
1000d51c:	ff 4d f0             	dec    DWORD PTR [ebp-0x10]
1000d51f:	8b 4d f4             	mov    ecx,DWORD PTR [ebp-0xc]
1000d522:	ff 75 c4             	push   DWORD PTR [ebp-0x3c]
1000d525:	33 c0                	xor    eax,eax
1000d527:	40                   	inc    eax
1000d528:	33 db                	xor    ebx,ebx
1000d52a:	43                   	inc    ebx
1000d52b:	d3 e3                	shl    ebx,cl
1000d52d:	8b 4d ec             	mov    ecx,DWORD PTR [ebp-0x14]
1000d530:	d3 e0                	shl    eax,cl
1000d532:	8b 4d f0             	mov    ecx,DWORD PTR [ebp-0x10]
1000d535:	52                   	push   edx
1000d536:	ff 75 10             	push   DWORD PTR [ebp+0x10]
1000d539:	89 45 c0             	mov    DWORD PTR [ebp-0x40],eax
1000d53c:	ff 75 dc             	push   DWORD PTR [ebp-0x24]
1000d53f:	89 45 08             	mov    DWORD PTR [ebp+0x8],eax
1000d542:	33 c0                	xor    eax,eax
1000d544:	40                   	inc    eax
1000d545:	d3 e0                	shl    eax,cl
1000d547:	89 5d f8             	mov    DWORD PTR [ebp-0x8],ebx
1000d54a:	89 45 fc             	mov    DWORD PTR [ebp-0x4],eax
1000d54d:	8d 45 fc             	lea    eax,[ebp-0x4]
1000d550:	50                   	push   eax
1000d551:	8d 45 08             	lea    eax,[ebp+0x8]
1000d554:	50                   	push   eax
1000d555:	8d 45 f8             	lea    eax,[ebp-0x8]
1000d558:	50                   	push   eax
1000d559:	e8 bf f3 ff ff       	call   0x1000c91d
1000d55e:	8b 7d f8             	mov    edi,DWORD PTR [ebp-0x8]
1000d561:	0f af 7d dc          	imul   edi,DWORD PTR [ebp-0x24]
1000d565:	c1 ef 03             	shr    edi,0x3
1000d568:	83 fb 10             	cmp    ebx,0x10
1000d56b:	76 06                	jbe    0x1000d573
1000d56d:	83 7d c0 10          	cmp    DWORD PTR [ebp-0x40],0x10
1000d571:	77 08                	ja     0x1000d57b
1000d573:	66 f7 46 30 00 08    	test   WORD PTR [esi+0x30],0x800
1000d579:	75 46                	jne    0x1000d5c1
1000d57b:	83 7d e8 00          	cmp    DWORD PTR [ebp-0x18],0x0
1000d57f:	7e 35                	jle    0x1000d5b6
1000d581:	83 7d d4 02          	cmp    DWORD PTR [ebp-0x2c],0x2
1000d585:	8b c7                	mov    eax,edi
1000d587:	74 14                	je     0x1000d59d
1000d589:	0f af 45 08          	imul   eax,DWORD PTR [ebp+0x8]
1000d58d:	05 ff 0f 00 00       	add    eax,0xfff
1000d592:	25 00 f0 ff ff       	and    eax,0xfffff000
1000d597:	0f af 45 fc          	imul   eax,DWORD PTR [ebp-0x4]
1000d59b:	eb 12                	jmp    0x1000d5af
1000d59d:	0f af 45 fc          	imul   eax,DWORD PTR [ebp-0x4]
1000d5a1:	0f af 45 08          	imul   eax,DWORD PTR [ebp+0x8]
1000d5a5:	05 ff 0f 00 00       	add    eax,0xfff
1000d5aa:	25 00 f0 ff ff       	and    eax,0xfffff000
1000d5af:	0f af 45 cc          	imul   eax,DWORD PTR [ebp-0x34]
1000d5b3:	01 45 e4             	add    DWORD PTR [ebp-0x1c],eax
1000d5b6:	ff 4d e8             	dec    DWORD PTR [ebp-0x18]
1000d5b9:	0f 89 43 ff ff ff    	jns    0x1000d502
1000d5bf:	eb 39                	jmp    0x1000d5fa
1000d5c1:	ff 75 10             	push   DWORD PTR [ebp+0x10]
1000d5c4:	8b 4d f0             	mov    ecx,DWORD PTR [ebp-0x10]
1000d5c7:	8b c7                	mov    eax,edi
1000d5c9:	0f af 45 08          	imul   eax,DWORD PTR [ebp+0x8]
1000d5cd:	50                   	push   eax
1000d5ce:	33 c0                	xor    eax,eax
1000d5d0:	40                   	inc    eax
1000d5d1:	d3 e0                	shl    eax,cl
1000d5d3:	8b 4d ec             	mov    ecx,DWORD PTR [ebp-0x14]
1000d5d6:	57                   	push   edi
1000d5d7:	50                   	push   eax
1000d5d8:	33 c0                	xor    eax,eax
1000d5da:	40                   	inc    eax
1000d5db:	d3 e0                	shl    eax,cl
1000d5dd:	8b 4d f4             	mov    ecx,DWORD PTR [ebp-0xc]
1000d5e0:	50                   	push   eax
1000d5e1:	33 c0                	xor    eax,eax
1000d5e3:	40                   	inc    eax
1000d5e4:	d3 e0                	shl    eax,cl
1000d5e6:	50                   	push   eax
1000d5e7:	ff 75 e8             	push   DWORD PTR [ebp-0x18]
1000d5ea:	e8 6b f8 ff ff       	call   0x1000ce5a
1000d5ef:	01 45 e4             	add    DWORD PTR [ebp-0x1c],eax
1000d5f2:	89 45 c8             	mov    DWORD PTR [ebp-0x38],eax
1000d5f5:	eb 03                	jmp    0x1000d5fa
1000d5f7:	8b 7d 18             	mov    edi,DWORD PTR [ebp+0x18]
1000d5fa:	8b c7                	mov    eax,edi
1000d5fc:	0f af 45 08          	imul   eax,DWORD PTR [ebp+0x8]
1000d600:	8b 5d 30             	mov    ebx,DWORD PTR [ebp+0x30]
1000d603:	8d 88 ff 0f 00 00    	lea    ecx,[eax+0xfff]
1000d609:	8b d1                	mov    edx,ecx
1000d60b:	89 4d 08             	mov    DWORD PTR [ebp+0x8],ecx
1000d60e:	b9 00 f0 ff ff       	mov    ecx,0xfffff000
1000d613:	23 d1                	and    edx,ecx
1000d615:	0f af 55 0c          	imul   edx,DWORD PTR [ebp+0xc]
1000d619:	03 55 e4             	add    edx,DWORD PTR [ebp-0x1c]
1000d61c:	89 13                	mov    DWORD PTR [ebx],edx
1000d61e:	8b 55 14             	mov    edx,DWORD PTR [ebp+0x14]
1000d621:	89 3a                	mov    DWORD PTR [edx],edi
1000d623:	8b 56 30             	mov    edx,DWORD PTR [esi+0x30]
1000d626:	5f                   	pop    edi
1000d627:	81 e2 00 06 00 00    	and    edx,0x600
1000d62d:	81 fa 00 04 00 00    	cmp    edx,0x400
1000d633:	5e                   	pop    esi
1000d634:	5b                   	pop    ebx
1000d635:	74 10                	je     0x1000d647
1000d637:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1000d63a:	23 c1                	and    eax,ecx
1000d63c:	8b 4d 18             	mov    ecx,DWORD PTR [ebp+0x18]
1000d63f:	89 01                	mov    DWORD PTR [ecx],eax
1000d641:	0f af 45 fc          	imul   eax,DWORD PTR [ebp-0x4]
1000d645:	eb 10                	jmp    0x1000d657
1000d647:	8b 55 18             	mov    edx,DWORD PTR [ebp+0x18]
1000d64a:	89 02                	mov    DWORD PTR [edx],eax
1000d64c:	0f af 45 fc          	imul   eax,DWORD PTR [ebp-0x4]
1000d650:	05 ff 0f 00 00       	add    eax,0xfff
1000d655:	23 c1                	and    eax,ecx
1000d657:	8b 4d 1c             	mov    ecx,DWORD PTR [ebp+0x1c]
1000d65a:	89 01                	mov    DWORD PTR [ecx],eax
1000d65c:	8b 45 24             	mov    eax,DWORD PTR [ebp+0x24]
1000d65f:	8b 4d d0             	mov    ecx,DWORD PTR [ebp-0x30]
1000d662:	89 08                	mov    DWORD PTR [eax],ecx
1000d664:	8b 45 28             	mov    eax,DWORD PTR [ebp+0x28]
1000d667:	8b 4d d8             	mov    ecx,DWORD PTR [ebp-0x28]
1000d66a:	89 08                	mov    DWORD PTR [eax],ecx
1000d66c:	8b 45 2c             	mov    eax,DWORD PTR [ebp+0x2c]
1000d66f:	8b 4d e0             	mov    ecx,DWORD PTR [ebp-0x20]
1000d672:	89 08                	mov    DWORD PTR [eax],ecx
1000d674:	8b 45 20             	mov    eax,DWORD PTR [ebp+0x20]
1000d677:	8b 4d c8             	mov    ecx,DWORD PTR [ebp-0x38]
1000d67a:	89 08                	mov    DWORD PTR [eax],ecx
1000d67c:	c9                   	leave
1000d67d:	c2 2c 00             	ret    0x2c
