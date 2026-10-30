
SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1001431e <.text+0x1331e>:
1001431e:	0f b7 45 48          	movzx  eax,WORD PTR [ebp+0x48]
10014322:	83 f8 10             	cmp    eax,0x10
10014325:	89 45 40             	mov    DWORD PTR [ebp+0x40],eax
10014328:	74 17                	je     0x10014341
1001432a:	83 f8 18             	cmp    eax,0x18
1001432d:	74 09                	je     0x10014338
1001432f:	83 f8 20             	cmp    eax,0x20
10014332:	0f 85 68 06 00 00    	jne    0x100149a0
10014338:	c7 45 6c 86 00 28 28 	mov    DWORD PTR [ebp+0x6c],0x28280086
1001433f:	eb 07                	jmp    0x10014348
10014341:	c7 45 6c 43 00 28 28 	mov    DWORD PTR [ebp+0x6c],0x28280043
10014348:	8b 55 6c             	mov    edx,DWORD PTR [ebp+0x6c]
1001434b:	b8 3f fe ff ff       	mov    eax,0xfffffe3f
10014350:	23 d0                	and    edx,eax
10014352:	3b d0                	cmp    edx,eax
10014354:	0f 84 46 06 00 00    	je     0x100149a0
1001435a:	8b 7d 40             	mov    edi,DWORD PTR [ebp+0x40]
1001435d:	8b 4d 60             	mov    ecx,DWORD PTR [ebp+0x60]
10014360:	8b c7                	mov    eax,edi
10014362:	48                   	dec    eax
10014363:	74 27                	je     0x1001438c
10014365:	83 e8 03             	sub    eax,0x3
10014368:	74 1b                	je     0x10014385
1001436a:	83 e8 14             	sub    eax,0x14
1001436d:	74 0a                	je     0x10014379
1001436f:	c1 ef 03             	shr    edi,0x3
10014372:	0f af f9             	imul   edi,ecx
10014375:	8b c7                	mov    eax,edi
10014377:	eb 1b                	jmp    0x10014394
10014379:	8b f9                	mov    edi,ecx
1001437b:	8b c1                	mov    eax,ecx
1001437d:	c1 e7 02             	shl    edi,0x2
10014380:	6b c0 03             	imul   eax,eax,0x3
10014383:	eb 0f                	jmp    0x10014394
10014385:	8d 41 01             	lea    eax,[ecx+0x1]
10014388:	d1 e8                	shr    eax,1
1001438a:	eb 06                	jmp    0x10014392
1001438c:	8d 41 07             	lea    eax,[ecx+0x7]
1001438f:	c1 e8 03             	shr    eax,0x3
10014392:	8b f9                	mov    edi,ecx
10014394:	81 fa 26 aa 22 1a    	cmp    edx,0x1a22aa26
1001439a:	89 7d 4c             	mov    DWORD PTR [ebp+0x4c],edi
1001439d:	75 08                	jne    0x100143a7
1001439f:	8b f9                	mov    edi,ecx
100143a1:	c1 e7 04             	shl    edi,0x4
100143a4:	89 7d 4c             	mov    DWORD PTR [ebp+0x4c],edi
100143a7:	8d 48 03             	lea    ecx,[eax+0x3]
100143aa:	83 e1 fc             	and    ecx,0xfffffffc
100143ad:	33 f6                	xor    esi,esi
100143af:	39 75 44             	cmp    DWORD PTR [ebp+0x44],esi
100143b2:	89 4d 38             	mov    DWORD PTR [ebp+0x38],ecx
100143b5:	74 06                	je     0x100143bd
100143b7:	83 7d 44 03          	cmp    DWORD PTR [ebp+0x44],0x3
100143bb:	75 15                	jne    0x100143d2
100143bd:	8b 5d 5c             	mov    ebx,DWORD PTR [ebp+0x5c]
100143c0:	4b                   	dec    ebx
100143c1:	0f af d9             	imul   ebx,ecx
100143c4:	03 d8                	add    ebx,eax
100143c6:	03 5d 64             	add    ebx,DWORD PTR [ebp+0x64]
100143c9:	3b 5d 7c             	cmp    ebx,DWORD PTR [ebp+0x7c]
100143cc:	0f 87 ce 05 00 00    	ja     0x100149a0
100143d2:	39 75 44             	cmp    DWORD PTR [ebp+0x44],esi
100143d5:	75 4c                	jne    0x10014423
100143d7:	81 fa 06 00 28 28    	cmp    edx,0x28280006
100143dd:	75 44                	jne    0x10014423
100143df:	39 75 5c             	cmp    DWORD PTR [ebp+0x5c],esi
100143e2:	89 75 58             	mov    DWORD PTR [ebp+0x58],esi
100143e5:	76 3c                	jbe    0x10014423
100143e7:	8b 5d 78             	mov    ebx,DWORD PTR [ebp+0x78]
100143ea:	8b 45 64             	mov    eax,DWORD PTR [ebp+0x64]
100143ed:	8d 34 18             	lea    esi,[eax+ebx*1]
100143f0:	33 c0                	xor    eax,eax
100143f2:	39 45 60             	cmp    DWORD PTR [ebp+0x60],eax
100143f5:	76 1d                	jbe    0x10014414
100143f7:	f7 04 86 00 00 00 ff 	test   DWORD PTR [esi+eax*4],0xff000000
100143fe:	75 08                	jne    0x10014408
10014400:	40                   	inc    eax
10014401:	3b 45 60             	cmp    eax,DWORD PTR [ebp+0x60]
10014404:	72 f1                	jb     0x100143f7
10014406:	eb 07                	jmp    0x1001440f
10014408:	c7 45 6c 86 00 28 18 	mov    DWORD PTR [ebp+0x6c],0x18280086
1001440f:	3b 45 60             	cmp    eax,DWORD PTR [ebp+0x60]
10014412:	72 12                	jb     0x10014426
10014414:	ff 45 58             	inc    DWORD PTR [ebp+0x58]
10014417:	8b 45 58             	mov    eax,DWORD PTR [ebp+0x58]
1001441a:	03 f1                	add    esi,ecx
1001441c:	3b 45 5c             	cmp    eax,DWORD PTR [ebp+0x5c]
1001441f:	72 cf                	jb     0x100143f0
10014421:	eb 03                	jmp    0x10014426
10014423:	8b 5d 78             	mov    ebx,DWORD PTR [ebp+0x78]
10014426:	8b 75 3c             	mov    esi,DWORD PTR [ebp+0x3c]
10014429:	8b 4d 6c             	mov    ecx,DWORD PTR [ebp+0x6c]
1001442c:	8d 47 03             	lea    eax,[edi+0x3]
1001442f:	8b 7d 60             	mov    edi,DWORD PTR [ebp+0x60]
10014432:	33 d2                	xor    edx,edx
10014434:	83 e0 fc             	and    eax,0xfffffffc
10014437:	39 56 40             	cmp    DWORD PTR [esi+0x40],edx
1001443a:	89 7e 0c             	mov    DWORD PTR [esi+0xc],edi
1001443d:	8b 7d 5c             	mov    edi,DWORD PTR [ebp+0x5c]
10014440:	89 0e                	mov    DWORD PTR [esi],ecx
10014442:	89 46 30             	mov    DWORD PTR [esi+0x30],eax
10014445:	89 56 34             	mov    DWORD PTR [esi+0x34],edx
10014448:	89 7e 10             	mov    DWORD PTR [esi+0x10],edi
1001444b:	c7 46 14 01 00 00 00 	mov    DWORD PTR [esi+0x14],0x1
10014452:	0f 84 44 05 00 00    	je     0x1001499c
10014458:	39 55 44             	cmp    DWORD PTR [ebp+0x44],edx
1001445b:	74 06                	je     0x10014463
1001445d:	83 7d 44 03          	cmp    DWORD PTR [ebp+0x44],0x3
10014461:	75 31                	jne    0x10014494
10014463:	39 55 2c             	cmp    DWORD PTR [ebp+0x2c],edx
10014466:	7d 2c                	jge    0x10014494
10014468:	66 83 7d 48 08       	cmp    WORD PTR [ebp+0x48],0x8
1001446d:	72 25                	jb     0x10014494
1001446f:	66 83 7d 48 18       	cmp    WORD PTR [ebp+0x48],0x18
10014474:	74 1e                	je     0x10014494
10014476:	81 e1 3f fe ff ff    	and    ecx,0xfffffe3f
1001447c:	81 f9 26 aa 22 1a    	cmp    ecx,0x1a22aa26
10014482:	74 10                	je     0x10014494
10014484:	8b 45 64             	mov    eax,DWORD PTR [ebp+0x64]
10014487:	03 c3                	add    eax,ebx
10014489:	89 56 38             	mov    DWORD PTR [esi+0x38],edx
1001448c:	89 46 04             	mov    DWORD PTR [esi+0x4],eax
1001448f:	e9 08 05 00 00       	jmp    0x1001499c
10014494:	8b 7d 5c             	mov    edi,DWORD PTR [ebp+0x5c]
10014497:	0f af c7             	imul   eax,edi
1001449a:	50                   	push   eax
1001449b:	c7 46 38 01 00 00 00 	mov    DWORD PTR [esi+0x38],0x1
100144a2:	e8 cd db ff ff       	call   0x10012074
100144a7:	85 c0                	test   eax,eax
100144a9:	89 45 78             	mov    DWORD PTR [ebp+0x78],eax
100144ac:	89 46 04             	mov    DWORD PTR [esi+0x4],eax
100144af:	75 0a                	jne    0x100144bb
100144b1:	b8 0e 00 07 80       	mov    eax,0x8007000e
100144b6:	e9 ea 04 00 00       	jmp    0x100149a5
100144bb:	8b 45 64             	mov    eax,DWORD PTR [ebp+0x64]
100144be:	8b 76 30             	mov    esi,DWORD PTR [esi+0x30]
100144c1:	03 c3                	add    eax,ebx
100144c3:	89 45 50             	mov    DWORD PTR [ebp+0x50],eax
100144c6:	89 45 3c             	mov    DWORD PTR [ebp+0x3c],eax
100144c9:	8b 45 7c             	mov    eax,DWORD PTR [ebp+0x7c]
100144cc:	03 d8                	add    ebx,eax
100144ce:	83 7d 2c 00          	cmp    DWORD PTR [ebp+0x2c],0x0
100144d2:	89 5d 58             	mov    DWORD PTR [ebp+0x58],ebx
100144d5:	7d 08                	jge    0x100144df
100144d7:	8b 5d 78             	mov    ebx,DWORD PTR [ebp+0x78]
100144da:	89 75 64             	mov    DWORD PTR [ebp+0x64],esi
100144dd:	eb 10                	jmp    0x100144ef
100144df:	8d 5f ff             	lea    ebx,[edi-0x1]
100144e2:	0f af de             	imul   ebx,esi
100144e5:	8b c6                	mov    eax,esi
100144e7:	f7 d8                	neg    eax
100144e9:	03 5d 78             	add    ebx,DWORD PTR [ebp+0x78]
100144ec:	89 45 64             	mov    DWORD PTR [ebp+0x64],eax
100144ef:	8b 4d 78             	mov    ecx,DWORD PTR [ebp+0x78]
100144f2:	0f af f7             	imul   esi,edi
100144f5:	03 f1                	add    esi,ecx
100144f7:	83 7d 44 02          	cmp    DWORD PTR [ebp+0x44],0x2
100144fb:	8b d3                	mov    edx,ebx
100144fd:	89 75 7c             	mov    DWORD PTR [ebp+0x7c],esi
10014500:	89 55 5c             	mov    DWORD PTR [ebp+0x5c],edx
10014503:	0f 85 30 01 00 00    	jne    0x10014639
10014509:	3b d9                	cmp    ebx,ecx
1001450b:	0f 82 8b 04 00 00    	jb     0x1001499c
10014511:	8b 75 50             	mov    esi,DWORD PTR [ebp+0x50]
10014514:	3b 55 7c             	cmp    edx,DWORD PTR [ebp+0x7c]
10014517:	0f 83 7f 04 00 00    	jae    0x1001499c
1001451d:	3b 75 3c             	cmp    esi,DWORD PTR [ebp+0x3c]
10014520:	0f 82 7a 04 00 00    	jb     0x100149a0
10014526:	8d 7e 01             	lea    edi,[esi+0x1]
10014529:	3b 7d 58             	cmp    edi,DWORD PTR [ebp+0x58]
1001452c:	0f 83 6e 04 00 00    	jae    0x100149a0
10014532:	8a 06                	mov    al,BYTE PTR [esi]
10014534:	84 c0                	test   al,al
10014536:	0f 85 ac 00 00 00    	jne    0x100145e8
1001453c:	0f b6 07             	movzx  eax,BYTE PTR [edi]
1001453f:	8b c8                	mov    ecx,eax
10014541:	83 e9 00             	sub    ecx,0x0
10014544:	0f 84 94 00 00 00    	je     0x100145de
1001454a:	49                   	dec    ecx
1001454b:	0f 84 85 00 00 00    	je     0x100145d6
10014551:	49                   	dec    ecx
10014552:	74 62                	je     0x100145b6
10014554:	3b 5d 78             	cmp    ebx,DWORD PTR [ebp+0x78]
10014557:	0f 82 43 04 00 00    	jb     0x100149a0
1001455d:	8d 0c 18             	lea    ecx,[eax+ebx*1]
10014560:	3b 4d 7c             	cmp    ecx,DWORD PTR [ebp+0x7c]
10014563:	0f 87 37 04 00 00    	ja     0x100149a0
10014569:	80 3f 00             	cmp    BYTE PTR [edi],0x0
1001456c:	74 12                	je     0x10014580
1001456e:	8d 48 ff             	lea    ecx,[eax-0x1]
10014571:	d1 f9                	sar    ecx,1
10014573:	8d 4c 31 02          	lea    ecx,[ecx+esi*1+0x2]
10014577:	3b 4d 58             	cmp    ecx,DWORD PTR [ebp+0x58]
1001457a:	0f 83 20 04 00 00    	jae    0x100149a0
10014580:	33 c9                	xor    ecx,ecx
10014582:	85 c0                	test   eax,eax
10014584:	7e 21                	jle    0x100145a7
10014586:	8b c1                	mov    eax,ecx
10014588:	d1 f8                	sar    eax,1
1001458a:	f6 c1 01             	test   cl,0x1
1001458d:	0f b6 44 30 02       	movzx  eax,BYTE PTR [eax+esi*1+0x2]
10014592:	74 05                	je     0x10014599
10014594:	83 e0 0f             	and    eax,0xf
10014597:	eb 03                	jmp    0x1001459c
10014599:	c1 e8 04             	shr    eax,0x4
1001459c:	88 04 19             	mov    BYTE PTR [ecx+ebx*1],al
1001459f:	0f b6 07             	movzx  eax,BYTE PTR [edi]
100145a2:	41                   	inc    ecx
100145a3:	3b c8                	cmp    ecx,eax
100145a5:	7c df                	jl     0x10014586
100145a7:	0f b6 07             	movzx  eax,BYTE PTR [edi]
100145aa:	03 d8                	add    ebx,eax
100145ac:	d1 e8                	shr    eax,1
100145ae:	40                   	inc    eax
100145af:	83 e0 fe             	and    eax,0xfffffffe
100145b2:	03 f0                	add    esi,eax
100145b4:	eb 70                	jmp    0x10014626
100145b6:	8d 46 03             	lea    eax,[esi+0x3]
100145b9:	3b 45 58             	cmp    eax,DWORD PTR [ebp+0x58]
100145bc:	0f 83 de 03 00 00    	jae    0x100149a0
100145c2:	0f b6 00             	movzx  eax,BYTE PTR [eax]
100145c5:	0f af 45 64          	imul   eax,DWORD PTR [ebp+0x64]
100145c9:	83 c6 02             	add    esi,0x2
100145cc:	0f b6 0e             	movzx  ecx,BYTE PTR [esi]
100145cf:	03 cb                	add    ecx,ebx
100145d1:	8d 1c 01             	lea    ebx,[ecx+eax*1]
100145d4:	eb 50                	jmp    0x10014626
100145d6:	8b 45 7c             	mov    eax,DWORD PTR [ebp+0x7c]
100145d9:	89 45 5c             	mov    DWORD PTR [ebp+0x5c],eax
100145dc:	eb 48                	jmp    0x10014626
100145de:	03 55 64             	add    edx,DWORD PTR [ebp+0x64]
100145e1:	89 55 5c             	mov    DWORD PTR [ebp+0x5c],edx
100145e4:	8b da                	mov    ebx,edx
100145e6:	eb 3e                	jmp    0x10014626
100145e8:	3b 5d 78             	cmp    ebx,DWORD PTR [ebp+0x78]
100145eb:	0f 82 af 03 00 00    	jb     0x100149a0
100145f1:	0f b6 c0             	movzx  eax,al
100145f4:	8d 0c 18             	lea    ecx,[eax+ebx*1]
100145f7:	3b 4d 7c             	cmp    ecx,DWORD PTR [ebp+0x7c]
100145fa:	0f 87 a0 03 00 00    	ja     0x100149a0
10014600:	33 c9                	xor    ecx,ecx
10014602:	85 c0                	test   eax,eax
10014604:	7e 1b                	jle    0x10014621
10014606:	f6 c1 01             	test   cl,0x1
10014609:	0f b6 07             	movzx  eax,BYTE PTR [edi]
1001460c:	74 05                	je     0x10014613
1001460e:	83 e0 0f             	and    eax,0xf
10014611:	eb 03                	jmp    0x10014616
10014613:	c1 e8 04             	shr    eax,0x4
10014616:	88 04 19             	mov    BYTE PTR [ecx+ebx*1],al
10014619:	0f b6 06             	movzx  eax,BYTE PTR [esi]
1001461c:	41                   	inc    ecx
1001461d:	3b c8                	cmp    ecx,eax
1001461f:	7c e5                	jl     0x10014606
10014621:	0f b6 06             	movzx  eax,BYTE PTR [esi]
10014624:	03 d8                	add    ebx,eax
10014626:	8b 55 5c             	mov    edx,DWORD PTR [ebp+0x5c]
10014629:	46                   	inc    esi
1001462a:	46                   	inc    esi
1001462b:	3b 55 78             	cmp    edx,DWORD PTR [ebp+0x78]
1001462e:	0f 83 e0 fe ff ff    	jae    0x10014514
10014634:	e9 63 03 00 00       	jmp    0x1001499c
10014639:	83 7d 44 01          	cmp    DWORD PTR [ebp+0x44],0x1
1001463d:	0f 85 f4 00 00 00    	jne    0x10014737
10014643:	3b d9                	cmp    ebx,ecx
10014645:	0f 82 51 03 00 00    	jb     0x1001499c
1001464b:	8b 75 50             	mov    esi,DWORD PTR [ebp+0x50]
1001464e:	8b 45 5c             	mov    eax,DWORD PTR [ebp+0x5c]
10014651:	3b 45 7c             	cmp    eax,DWORD PTR [ebp+0x7c]
10014654:	0f 83 42 03 00 00    	jae    0x1001499c
1001465a:	3b 75 3c             	cmp    esi,DWORD PTR [ebp+0x3c]
1001465d:	0f 82 3d 03 00 00    	jb     0x100149a0
10014663:	8d 7e 01             	lea    edi,[esi+0x1]
10014666:	3b 7d 58             	cmp    edi,DWORD PTR [ebp+0x58]
10014669:	0f 83 31 03 00 00    	jae    0x100149a0
1001466f:	8a 06                	mov    al,BYTE PTR [esi]
10014671:	84 c0                	test   al,al
10014673:	0f 85 80 00 00 00    	jne    0x100146f9
10014679:	0f b6 07             	movzx  eax,BYTE PTR [edi]
1001467c:	8b c8                	mov    ecx,eax
1001467e:	83 e9 00             	sub    ecx,0x0
10014681:	74 6b                	je     0x100146ee
10014683:	49                   	dec    ecx
10014684:	74 60                	je     0x100146e6
10014686:	49                   	dec    ecx
10014687:	74 3d                	je     0x100146c6
10014689:	3b 5d 78             	cmp    ebx,DWORD PTR [ebp+0x78]
1001468c:	0f 82 0e 03 00 00    	jb     0x100149a0
10014692:	8d 0c 18             	lea    ecx,[eax+ebx*1]
10014695:	3b 4d 7c             	cmp    ecx,DWORD PTR [ebp+0x7c]
10014698:	0f 87 02 03 00 00    	ja     0x100149a0
1001469e:	8d 4c 30 02          	lea    ecx,[eax+esi*1+0x2]
100146a2:	3b 4d 58             	cmp    ecx,DWORD PTR [ebp+0x58]
100146a5:	0f 87 f5 02 00 00    	ja     0x100149a0
100146ab:	50                   	push   eax
100146ac:	8d 46 02             	lea    eax,[esi+0x2]
100146af:	50                   	push   eax
100146b0:	53                   	push   ebx
100146b1:	e8 90 a0 03 00       	call   0x1004e746
100146b6:	0f b6 07             	movzx  eax,BYTE PTR [edi]
100146b9:	03 d8                	add    ebx,eax
100146bb:	83 c4 0c             	add    esp,0xc
100146be:	40                   	inc    eax
100146bf:	83 e0 fe             	and    eax,0xfffffffe
100146c2:	03 f0                	add    esi,eax
100146c4:	eb 5e                	jmp    0x10014724
100146c6:	8d 46 03             	lea    eax,[esi+0x3]
100146c9:	3b 45 58             	cmp    eax,DWORD PTR [ebp+0x58]
100146cc:	0f 83 ce 02 00 00    	jae    0x100149a0
100146d2:	0f b6 00             	movzx  eax,BYTE PTR [eax]
100146d5:	0f af 45 64          	imul   eax,DWORD PTR [ebp+0x64]
100146d9:	83 c6 02             	add    esi,0x2
100146dc:	0f b6 0e             	movzx  ecx,BYTE PTR [esi]
100146df:	03 cb                	add    ecx,ebx
100146e1:	8d 1c 01             	lea    ebx,[ecx+eax*1]
100146e4:	eb 3e                	jmp    0x10014724
100146e6:	8b 45 7c             	mov    eax,DWORD PTR [ebp+0x7c]
100146e9:	89 45 5c             	mov    DWORD PTR [ebp+0x5c],eax
100146ec:	eb 36                	jmp    0x10014724
100146ee:	8b 5d 5c             	mov    ebx,DWORD PTR [ebp+0x5c]
100146f1:	03 5d 64             	add    ebx,DWORD PTR [ebp+0x64]
100146f4:	89 5d 5c             	mov    DWORD PTR [ebp+0x5c],ebx
100146f7:	eb 2b                	jmp    0x10014724
100146f9:	3b 5d 78             	cmp    ebx,DWORD PTR [ebp+0x78]
100146fc:	0f 82 9e 02 00 00    	jb     0x100149a0
10014702:	0f b6 c0             	movzx  eax,al
10014705:	8d 0c 18             	lea    ecx,[eax+ebx*1]
10014708:	3b 4d 7c             	cmp    ecx,DWORD PTR [ebp+0x7c]
1001470b:	0f 87 8f 02 00 00    	ja     0x100149a0
10014711:	50                   	push   eax
10014712:	0f b6 07             	movzx  eax,BYTE PTR [edi]
10014715:	50                   	push   eax
10014716:	53                   	push   ebx
10014717:	e8 ee a0 03 00       	call   0x1004e80a
1001471c:	0f b6 06             	movzx  eax,BYTE PTR [esi]
1001471f:	83 c4 0c             	add    esp,0xc
10014722:	03 d8                	add    ebx,eax
10014724:	8b 45 5c             	mov    eax,DWORD PTR [ebp+0x5c]
10014727:	46                   	inc    esi
10014728:	46                   	inc    esi
10014729:	3b 45 78             	cmp    eax,DWORD PTR [ebp+0x78]
1001472c:	0f 83 1c ff ff ff    	jae    0x1001464e
10014732:	e9 65 02 00 00       	jmp    0x1001499c
10014737:	66 83 7d 48 01       	cmp    WORD PTR [ebp+0x48],0x1
1001473c:	75 4a                	jne    0x10014788
1001473e:	3b d9                	cmp    ebx,ecx
10014740:	0f 82 56 02 00 00    	jb     0x1001499c
10014746:	8b 75 50             	mov    esi,DWORD PTR [ebp+0x50]
10014749:	3b 5d 7c             	cmp    ebx,DWORD PTR [ebp+0x7c]
1001474c:	0f 83 4a 02 00 00    	jae    0x1001499c
10014752:	33 c0                	xor    eax,eax
10014754:	39 45 60             	cmp    DWORD PTR [ebp+0x60],eax
10014757:	76 1f                	jbe    0x10014778
10014759:	8a d0                	mov    dl,al
1001475b:	80 e2 07             	and    dl,0x7
1001475e:	b1 07                	mov    cl,0x7
10014760:	2a ca                	sub    cl,dl
10014762:	8b d0                	mov    edx,eax
10014764:	c1 ea 03             	shr    edx,0x3
10014767:	8a 14 32             	mov    dl,BYTE PTR [edx+esi*1]
1001476a:	d2 ea                	shr    dl,cl
1001476c:	80 e2 01             	and    dl,0x1
1001476f:	88 14 18             	mov    BYTE PTR [eax+ebx*1],dl
10014772:	40                   	inc    eax
10014773:	3b 45 60             	cmp    eax,DWORD PTR [ebp+0x60]
10014776:	72 e1                	jb     0x10014759
10014778:	03 5d 64             	add    ebx,DWORD PTR [ebp+0x64]
1001477b:	03 75 38             	add    esi,DWORD PTR [ebp+0x38]
1001477e:	3b 5d 78             	cmp    ebx,DWORD PTR [ebp+0x78]
10014781:	73 c6                	jae    0x10014749
10014783:	e9 14 02 00 00       	jmp    0x1001499c
10014788:	66 83 7d 48 04       	cmp    WORD PTR [ebp+0x48],0x4
1001478d:	75 49                	jne    0x100147d8
1001478f:	3b d9                	cmp    ebx,ecx
10014791:	0f 82 05 02 00 00    	jb     0x1001499c
10014797:	8b 55 50             	mov    edx,DWORD PTR [ebp+0x50]
1001479a:	3b 5d 7c             	cmp    ebx,DWORD PTR [ebp+0x7c]
1001479d:	0f 83 f9 01 00 00    	jae    0x1001499c
100147a3:	33 c9                	xor    ecx,ecx
100147a5:	39 4d 60             	cmp    DWORD PTR [ebp+0x60],ecx
100147a8:	76 1e                	jbe    0x100147c8
100147aa:	8b c1                	mov    eax,ecx
100147ac:	d1 e8                	shr    eax,1
100147ae:	f6 c1 01             	test   cl,0x1
100147b1:	0f b6 04 10          	movzx  eax,BYTE PTR [eax+edx*1]
100147b5:	74 05                	je     0x100147bc
100147b7:	83 e0 0f             	and    eax,0xf
100147ba:	eb 03                	jmp    0x100147bf
100147bc:	c1 e8 04             	shr    eax,0x4
100147bf:	88 04 19             	mov    BYTE PTR [ecx+ebx*1],al
100147c2:	41                   	inc    ecx
100147c3:	3b 4d 60             	cmp    ecx,DWORD PTR [ebp+0x60]
100147c6:	72 e2                	jb     0x100147aa
100147c8:	03 5d 64             	add    ebx,DWORD PTR [ebp+0x64]
100147cb:	03 55 38             	add    edx,DWORD PTR [ebp+0x38]
100147ce:	3b 5d 78             	cmp    ebx,DWORD PTR [ebp+0x78]
100147d1:	73 c7                	jae    0x1001479a
100147d3:	e9 c4 01 00 00       	jmp    0x1001499c
100147d8:	66 83 7d 48 18       	cmp    WORD PTR [ebp+0x48],0x18
100147dd:	75 46                	jne    0x10014825
100147df:	3b d9                	cmp    ebx,ecx
100147e1:	0f 82 b5 01 00 00    	jb     0x1001499c
100147e7:	3b 5d 7c             	cmp    ebx,DWORD PTR [ebp+0x7c]
100147ea:	0f 83 ac 01 00 00    	jae    0x1001499c
100147f0:	83 7d 60 00          	cmp    DWORD PTR [ebp+0x60],0x0
100147f4:	76 1c                	jbe    0x10014812
100147f6:	8b 4d 50             	mov    ecx,DWORD PTR [ebp+0x50]
100147f9:	8b 55 60             	mov    edx,DWORD PTR [ebp+0x60]
100147fc:	8b c3                	mov    eax,ebx
100147fe:	8b f1                	mov    esi,ecx
10014800:	8b f8                	mov    edi,eax
10014802:	66 a5                	movs   WORD PTR es:[edi],WORD PTR ds:[esi]
10014804:	a4                   	movs   BYTE PTR es:[edi],BYTE PTR ds:[esi]
10014805:	c6 40 03 ff          	mov    BYTE PTR [eax+0x3],0xff
10014809:	83 c1 03             	add    ecx,0x3
1001480c:	83 c0 04             	add    eax,0x4
1001480f:	4a                   	dec    edx
10014810:	75 ec                	jne    0x100147fe
10014812:	03 5d 64             	add    ebx,DWORD PTR [ebp+0x64]
10014815:	8b 45 38             	mov    eax,DWORD PTR [ebp+0x38]
10014818:	01 45 50             	add    DWORD PTR [ebp+0x50],eax
1001481b:	3b 5d 78             	cmp    ebx,DWORD PTR [ebp+0x78]
1001481e:	73 c7                	jae    0x100147e7
10014820:	e9 77 01 00 00       	jmp    0x1001499c
10014825:	8b 45 6c             	mov    eax,DWORD PTR [ebp+0x6c]
10014828:	25 3f fe ff ff       	and    eax,0xfffffe3f
1001482d:	3d 26 aa 22 1a       	cmp    eax,0x1a22aa26
10014832:	74 31                	je     0x10014865
10014834:	3b d9                	cmp    ebx,ecx
10014836:	0f 82 60 01 00 00    	jb     0x1001499c
1001483c:	8b 75 50             	mov    esi,DWORD PTR [ebp+0x50]
1001483f:	3b 5d 7c             	cmp    ebx,DWORD PTR [ebp+0x7c]
10014842:	0f 83 54 01 00 00    	jae    0x1001499c
10014848:	ff 75 4c             	push   DWORD PTR [ebp+0x4c]
1001484b:	56                   	push   esi
1001484c:	53                   	push   ebx
1001484d:	e8 f4 9e 03 00       	call   0x1004e746
10014852:	03 5d 64             	add    ebx,DWORD PTR [ebp+0x64]
10014855:	03 75 38             	add    esi,DWORD PTR [ebp+0x38]
10014858:	83 c4 0c             	add    esp,0xc
1001485b:	3b 5d 78             	cmp    ebx,DWORD PTR [ebp+0x78]
1001485e:	73 df                	jae    0x1001483f
10014860:	e9 37 01 00 00       	jmp    0x1001499c
10014865:	8b 45 30             	mov    eax,DWORD PTR [ebp+0x30]
10014868:	d9 e8                	fld1
1001486a:	8b 7d 54             	mov    edi,DWORD PTR [ebp+0x54]
1001486d:	89 45 20             	mov    DWORD PTR [ebp+0x20],eax
10014870:	8b 45 34             	mov    eax,DWORD PTR [ebp+0x34]
10014873:	89 45 24             	mov    DWORD PTR [ebp+0x24],eax
10014876:	8b 45 68             	mov    eax,DWORD PTR [ebp+0x68]
10014879:	89 7d 1c             	mov    DWORD PTR [ebp+0x1c],edi
1001487c:	89 45 28             	mov    DWORD PTR [ebp+0x28],eax
1001487f:	33 c9                	xor    ecx,ecx
10014881:	8d 44 0d 1c          	lea    eax,[ebp+ecx*1+0x1c]
10014885:	8b 30                	mov    esi,DWORD PTR [eax]
10014887:	8d 54 0d ac          	lea    edx,[ebp+ecx*1-0x54]
1001488b:	83 22 00             	and    DWORD PTR [edx],0x0
1001488e:	85 f6                	test   esi,esi
10014890:	74 31                	je     0x100148c3
10014892:	f7 c6 01 00 00 00    	test   esi,0x1
10014898:	75 0f                	jne    0x100148a9
1001489a:	33 f6                	xor    esi,esi
1001489c:	d1 28                	shr    DWORD PTR [eax],1
1001489e:	46                   	inc    esi
1001489f:	f7 00 01 00 00 00    	test   DWORD PTR [eax],0x1
100148a5:	74 f5                	je     0x1001489c
100148a7:	89 32                	mov    DWORD PTR [edx],esi
100148a9:	8b 00                	mov    eax,DWORD PTR [eax]
100148ab:	85 c0                	test   eax,eax
100148ad:	89 45 3c             	mov    DWORD PTR [ebp+0x3c],eax
100148b0:	db 45 3c             	fild   DWORD PTR [ebp+0x3c]
100148b3:	7d 06                	jge    0x100148bb
100148b5:	d8 05 a0 80 09 10    	fadd   DWORD PTR ds:0x100980a0
100148bb:	d9 c1                	fld    st(1)
100148bd:	de f1                	fdivrp st(1),st
100148bf:	d9 5c 0d 0c          	fstp   DWORD PTR [ebp+ecx*1+0xc]
100148c3:	83 c1 04             	add    ecx,0x4
100148c6:	83 f9 10             	cmp    ecx,0x10
100148c9:	72 b6                	jb     0x10014881
100148cb:	85 ff                	test   edi,edi
100148cd:	75 21                	jne    0x100148f0
100148cf:	39 7d 30             	cmp    DWORD PTR [ebp+0x30],edi
100148d2:	75 1c                	jne    0x100148f0
100148d4:	39 7d 34             	cmp    DWORD PTR [ebp+0x34],edi
100148d7:	75 17                	jne    0x100148f0
100148d9:	d9 ee                	fldz
100148db:	33 c0                	xor    eax,eax
100148dd:	40                   	inc    eax
100148de:	d9 55 14             	fst    DWORD PTR [ebp+0x14]
100148e1:	d9 55 10             	fst    DWORD PTR [ebp+0x10]
100148e4:	89 45 24             	mov    DWORD PTR [ebp+0x24],eax
100148e7:	d9 5d 0c             	fstp   DWORD PTR [ebp+0xc]
100148ea:	89 45 20             	mov    DWORD PTR [ebp+0x20],eax
100148ed:	89 45 1c             	mov    DWORD PTR [ebp+0x1c],eax
100148f0:	3b 5d 78             	cmp    ebx,DWORD PTR [ebp+0x78]
100148f3:	0f 82 a1 00 00 00    	jb     0x1001499a
100148f9:	3b 5d 7c             	cmp    ebx,DWORD PTR [ebp+0x7c]
100148fc:	0f 83 98 00 00 00    	jae    0x1001499a
10014902:	8b 45 60             	mov    eax,DWORD PTR [ebp+0x60]
10014905:	85 c0                	test   eax,eax
10014907:	8b 55 50             	mov    edx,DWORD PTR [ebp+0x50]
1001490a:	89 5d 58             	mov    DWORD PTR [ebp+0x58],ebx
1001490d:	76 7d                	jbe    0x1001498c
1001490f:	89 45 34             	mov    DWORD PTR [ebp+0x34],eax
10014912:	83 7d 40 10          	cmp    DWORD PTR [ebp+0x40],0x10
10014916:	74 28                	je     0x10014940
10014918:	83 7d 40 18          	cmp    DWORD PTR [ebp+0x40],0x18
1001491c:	74 0d                	je     0x1001492b
1001491e:	83 7d 40 20          	cmp    DWORD PTR [ebp+0x40],0x20
10014922:	75 24                	jne    0x10014948
10014924:	8b 02                	mov    eax,DWORD PTR [edx]
10014926:	83 c2 04             	add    edx,0x4
10014929:	eb 1a                	jmp    0x10014945
1001492b:	0f b6 0a             	movzx  ecx,BYTE PTR [edx]
1001492e:	33 c0                	xor    eax,eax
10014930:	8a 62 02             	mov    ah,BYTE PTR [edx+0x2]
10014933:	8a 42 01             	mov    al,BYTE PTR [edx+0x1]
10014936:	c1 e0 08             	shl    eax,0x8
10014939:	0b c1                	or     eax,ecx
1001493b:	83 c2 03             	add    edx,0x3
1001493e:	eb 05                	jmp    0x10014945
10014940:	0f b7 02             	movzx  eax,WORD PTR [edx]
10014943:	42                   	inc    edx
10014944:	42                   	inc    edx
10014945:	89 45 4c             	mov    DWORD PTR [ebp+0x4c],eax
10014948:	33 c0                	xor    eax,eax
1001494a:	8b 74 05 1c          	mov    esi,DWORD PTR [ebp+eax*1+0x1c]
1001494e:	85 f6                	test   esi,esi
10014950:	74 21                	je     0x10014973
10014952:	8b 7d 4c             	mov    edi,DWORD PTR [ebp+0x4c]
10014955:	8b 4c 05 ac          	mov    ecx,DWORD PTR [ebp+eax*1-0x54]
10014959:	d3 ef                	shr    edi,cl
1001495b:	23 fe                	and    edi,esi
1001495d:	85 ff                	test   edi,edi
1001495f:	89 7d 30             	mov    DWORD PTR [ebp+0x30],edi
10014962:	db 45 30             	fild   DWORD PTR [ebp+0x30]
10014965:	7d 06                	jge    0x1001496d
10014967:	d8 05 a0 80 09 10    	fadd   DWORD PTR ds:0x100980a0
1001496d:	d8 4c 05 0c          	fmul   DWORD PTR [ebp+eax*1+0xc]
10014971:	eb 02                	jmp    0x10014975
10014973:	d9 c0                	fld    st(0)
10014975:	8b 4d 58             	mov    ecx,DWORD PTR [ebp+0x58]
10014978:	d9 1c 08             	fstp   DWORD PTR [eax+ecx*1]
1001497b:	83 c0 04             	add    eax,0x4
1001497e:	83 f8 10             	cmp    eax,0x10
10014981:	72 c7                	jb     0x1001494a
10014983:	83 45 58 10          	add    DWORD PTR [ebp+0x58],0x10
10014987:	ff 4d 34             	dec    DWORD PTR [ebp+0x34]
1001498a:	75 86                	jne    0x10014912
1001498c:	8b 45 38             	mov    eax,DWORD PTR [ebp+0x38]
1001498f:	03 5d 64             	add    ebx,DWORD PTR [ebp+0x64]
10014992:	01 45 50             	add    DWORD PTR [ebp+0x50],eax
10014995:	e9 56 ff ff ff       	jmp    0x100148f0
1001499a:	dd d8                	fstp   st(0)
1001499c:	33 c0                	xor    eax,eax
1001499e:	eb 05                	jmp    0x100149a5
100149a0:	b8 05 40 00 80       	mov    eax,0x80004005
100149a5:	5f                   	pop    edi
100149a6:	5e                   	pop    esi
100149a7:	5b                   	pop    ebx
100149a8:	83 c5 70             	add    ebp,0x70
100149ab:	c9                   	leave
