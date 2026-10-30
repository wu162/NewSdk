
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

100134b8 <.text+0x124b8>:
100134b8:	55                   	push   ebp
100134b9:	8d 6c 24 cc          	lea    ebp,[esp-0x34]
100134bd:	81 ec a0 04 00 00    	sub    esp,0x4a0
100134c3:	56                   	push   esi
100134c4:	8b 75 70             	mov    esi,DWORD PTR [ebp+0x70]
100134c7:	57                   	push   edi
100134c8:	8d 4d d4             	lea    ecx,[ebp-0x2c]
100134cb:	e8 21 08 00 00       	call   0x10013cf1
100134d0:	33 ff                	xor    edi,edi
100134d2:	39 7d 3c             	cmp    DWORD PTR [ebp+0x3c],edi
100134d5:	89 7d 2c             	mov    DWORD PTR [ebp+0x2c],edi
100134d8:	89 7d 30             	mov    DWORD PTR [ebp+0x30],edi
100134db:	89 7d 70             	mov    DWORD PTR [ebp+0x70],edi
100134de:	75 16                	jne    0x100134f6
100134e0:	8d 4d d4             	lea    ecx,[ebp-0x2c]
100134e3:	e8 00 0a 00 00       	call   0x10013ee8
100134e8:	b8 6c 08 76 88       	mov    eax,0x8876086c
100134ed:	5f                   	pop    edi
100134ee:	5e                   	pop    esi
100134ef:	83 c5 34             	add    ebp,0x34
100134f2:	c9                   	leave
100134f3:	c2 44 00             	ret    0x44
100134f6:	39 7d 40             	cmp    DWORD PTR [ebp+0x40],edi
100134f9:	74 e5                	je     0x100134e0
100134fb:	39 7d 44             	cmp    DWORD PTR [ebp+0x44],edi
100134fe:	74 e0                	je     0x100134e0
10013500:	39 7d 7c             	cmp    DWORD PTR [ebp+0x7c],edi
10013503:	53                   	push   ebx
10013504:	75 12                	jne    0x10013518
10013506:	be 6c 08 76 88       	mov    esi,0x8876086c
1001350b:	8d 4d d4             	lea    ecx,[ebp-0x2c]
1001350e:	e8 d5 09 00 00       	call   0x10013ee8
10013513:	8b c6                	mov    eax,esi
10013515:	5b                   	pop    ebx
10013516:	eb d5                	jmp    0x100134ed
10013518:	3b f7                	cmp    esi,edi
1001351a:	75 09                	jne    0x10013525
1001351c:	83 7d 78 ff          	cmp    DWORD PTR [ebp+0x78],0xffffffff
10013520:	75 03                	jne    0x10013525
10013522:	8d 75 94             	lea    esi,[ebp-0x6c]
10013525:	33 db                	xor    ebx,ebx
10013527:	43                   	inc    ebx
10013528:	53                   	push   ebx
10013529:	56                   	push   esi
1001352a:	ff 75 44             	push   DWORD PTR [ebp+0x44]
1001352d:	8d 4d d4             	lea    ecx,[ebp-0x2c]
10013530:	ff 75 40             	push   DWORD PTR [ebp+0x40]
10013533:	e8 cc 32 00 00       	call   0x10016804
10013538:	3b c7                	cmp    eax,edi
1001353a:	7d 04                	jge    0x10013540
1001353c:	8b f0                	mov    esi,eax
1001353e:	eb cb                	jmp    0x1001350b
10013540:	83 c9 ff             	or     ecx,0xffffffff
10013543:	39 4d 78             	cmp    DWORD PTR [ebp+0x78],ecx
10013546:	75 06                	jne    0x1001354e
10013548:	8b 46 14             	mov    eax,DWORD PTR [esi+0x14]
1001354b:	89 45 78             	mov    DWORD PTR [ebp+0x78],eax
1001354e:	8b 45 20             	mov    eax,DWORD PTR [ebp+0x20]
10013551:	89 5d 40             	mov    DWORD PTR [ebp+0x40],ebx
10013554:	eb 06                	jmp    0x1001355c
10013556:	ff 45 40             	inc    DWORD PTR [ebp+0x40]
10013559:	8b 40 4c             	mov    eax,DWORD PTR [eax+0x4c]
1001355c:	3b c7                	cmp    eax,edi
1001355e:	75 f6                	jne    0x10013556
10013560:	83 7d 78 12          	cmp    DWORD PTR [ebp+0x78],0x12
10013564:	89 5d 44             	mov    DWORD PTR [ebp+0x44],ebx
10013567:	75 1e                	jne    0x10013587
10013569:	8b 45 24             	mov    eax,DWORD PTR [ebp+0x24]
1001356c:	3b c7                	cmp    eax,edi
1001356e:	74 10                	je     0x10013580
10013570:	8b 40 50             	mov    eax,DWORD PTR [eax+0x50]
10013573:	ff 45 44             	inc    DWORD PTR [ebp+0x44]
10013576:	3b c7                	cmp    eax,edi
10013578:	75 f6                	jne    0x10013570
1001357a:	83 7d 44 06          	cmp    DWORD PTR [ebp+0x44],0x6
1001357e:	74 07                	je     0x10013587
10013580:	be 05 40 00 80       	mov    esi,0x80004005
10013585:	eb 84                	jmp    0x1001350b
10013587:	83 7d 48 fe          	cmp    DWORD PTR [ebp+0x48],0xfffffffe
1001358b:	8b 45 e0             	mov    eax,DWORD PTR [ebp-0x20]
1001358e:	74 1f                	je     0x100135af
10013590:	3b c7                	cmp    eax,edi
10013592:	7c 1b                	jl     0x100135af
10013594:	39 7d 48             	cmp    DWORD PTR [ebp+0x48],edi
10013597:	74 05                	je     0x1001359e
10013599:	39 4d 48             	cmp    DWORD PTR [ebp+0x48],ecx
1001359c:	75 14                	jne    0x100135b2
1001359e:	3b c3                	cmp    eax,ebx
100135a0:	89 5d 48             	mov    DWORD PTR [ebp+0x48],ebx
100135a3:	76 0d                	jbe    0x100135b2
100135a5:	d1 65 48             	shl    DWORD PTR [ebp+0x48],1
100135a8:	39 45 48             	cmp    DWORD PTR [ebp+0x48],eax
100135ab:	72 f8                	jb     0x100135a5
100135ad:	eb 03                	jmp    0x100135b2
100135af:	89 45 48             	mov    DWORD PTR [ebp+0x48],eax
100135b2:	83 7d 4c fe          	cmp    DWORD PTR [ebp+0x4c],0xfffffffe
100135b6:	8b 45 e4             	mov    eax,DWORD PTR [ebp-0x1c]
100135b9:	74 1f                	je     0x100135da
100135bb:	3b c7                	cmp    eax,edi
100135bd:	7c 1b                	jl     0x100135da
100135bf:	39 7d 4c             	cmp    DWORD PTR [ebp+0x4c],edi
100135c2:	74 05                	je     0x100135c9
100135c4:	39 4d 4c             	cmp    DWORD PTR [ebp+0x4c],ecx
100135c7:	75 14                	jne    0x100135dd
100135c9:	3b c3                	cmp    eax,ebx
100135cb:	89 5d 4c             	mov    DWORD PTR [ebp+0x4c],ebx
100135ce:	76 0d                	jbe    0x100135dd
100135d0:	d1 65 4c             	shl    DWORD PTR [ebp+0x4c],1
100135d3:	39 45 4c             	cmp    DWORD PTR [ebp+0x4c],eax
100135d6:	72 f8                	jb     0x100135d0
100135d8:	eb 03                	jmp    0x100135dd
100135da:	89 45 4c             	mov    DWORD PTR [ebp+0x4c],eax
100135dd:	83 7d 50 fe          	cmp    DWORD PTR [ebp+0x50],0xfffffffe
100135e1:	8b 45 e8             	mov    eax,DWORD PTR [ebp-0x18]
100135e4:	74 1f                	je     0x10013605
100135e6:	3b c7                	cmp    eax,edi
100135e8:	7c 1b                	jl     0x10013605
100135ea:	39 7d 50             	cmp    DWORD PTR [ebp+0x50],edi
100135ed:	74 05                	je     0x100135f4
100135ef:	39 4d 50             	cmp    DWORD PTR [ebp+0x50],ecx
100135f2:	75 14                	jne    0x10013608
100135f4:	3b c3                	cmp    eax,ebx
100135f6:	89 5d 50             	mov    DWORD PTR [ebp+0x50],ebx
100135f9:	76 0d                	jbe    0x10013608
100135fb:	d1 65 50             	shl    DWORD PTR [ebp+0x50],1
100135fe:	39 45 50             	cmp    DWORD PTR [ebp+0x50],eax
10013601:	72 f8                	jb     0x100135fb
10013603:	eb 03                	jmp    0x10013608
10013605:	89 45 50             	mov    DWORD PTR [ebp+0x50],eax
10013608:	39 4d 64             	cmp    DWORD PTR [ebp+0x64],ecx
1001360b:	75 07                	jne    0x10013614
1001360d:	c7 45 64 04 00 08 00 	mov    DWORD PTR [ebp+0x64],0x80004
10013614:	39 4d 68             	cmp    DWORD PTR [ebp+0x68],ecx
10013617:	6a 05                	push   0x5
10013619:	5a                   	pop    edx
1001361a:	75 03                	jne    0x1001361f
1001361c:	89 55 68             	mov    DWORD PTR [ebp+0x68],edx
1001361f:	83 7d 78 12          	cmp    DWORD PTR [ebp+0x78],0x12
10013623:	75 0b                	jne    0x10013630
10013625:	b8 00 00 07 00       	mov    eax,0x70000
1001362a:	09 45 64             	or     DWORD PTR [ebp+0x64],eax
1001362d:	09 45 68             	or     DWORD PTR [ebp+0x68],eax
10013630:	80 7d 64 01          	cmp    BYTE PTR [ebp+0x64],0x1
10013634:	b8 ff 00 00 00       	mov    eax,0xff
10013639:	74 11                	je     0x1001364c
1001363b:	8b 4d 68             	mov    ecx,DWORD PTR [ebp+0x68]
1001363e:	23 c8                	and    ecx,eax
10013640:	83 f9 02             	cmp    ecx,0x2
10013643:	74 07                	je     0x1001364c
10013645:	3b ca                	cmp    ecx,edx
10013647:	89 7d 28             	mov    DWORD PTR [ebp+0x28],edi
1001364a:	75 03                	jne    0x1001364f
1001364c:	89 5d 28             	mov    DWORD PTR [ebp+0x28],ebx
1001364f:	39 7d dc             	cmp    DWORD PTR [ebp-0x24],edi
10013652:	74 0a                	je     0x1001365e
10013654:	be 01 40 00 80       	mov    esi,0x80004001
10013659:	e9 ad fe ff ff       	jmp    0x1001350b
1001365e:	68 00 04 00 00       	push   0x400
10013663:	50                   	push   eax
10013664:	8d 85 94 fb ff ff    	lea    eax,[ebp-0x46c]
1001366a:	50                   	push   eax
1001366b:	e8 9a b1 03 00       	call   0x1004e80a
10013670:	8b 45 5c             	mov    eax,DWORD PTR [ebp+0x5c]
10013673:	bb 3f fe ff ff       	mov    ebx,0xfffffe3f
10013678:	23 c3                	and    eax,ebx
1001367a:	83 c4 0c             	add    esp,0xc
1001367d:	3b c3                	cmp    eax,ebx
1001367f:	75 60                	jne    0x100136e1
10013681:	39 7d 6c             	cmp    DWORD PTR [ebp+0x6c],edi
10013684:	8b 75 d4             	mov    esi,DWORD PTR [ebp-0x2c]
10013687:	74 4f                	je     0x100136d8
10013689:	56                   	push   esi
1001368a:	e8 66 e9 ff ff       	call   0x10011ff5
1001368f:	8b 48 04             	mov    ecx,DWORD PTR [eax+0x4]
10013692:	3b cf                	cmp    ecx,edi
10013694:	74 0a                	je     0x100136a0
10013696:	83 f9 01             	cmp    ecx,0x1
10013699:	74 05                	je     0x100136a0
1001369b:	83 f9 02             	cmp    ecx,0x2
1001369e:	75 38                	jne    0x100136d8
100136a0:	39 78 10             	cmp    DWORD PTR [eax+0x10],edi
100136a3:	75 33                	jne    0x100136d8
100136a5:	6a 09                	push   0x9
100136a7:	59                   	pop    ecx
100136a8:	8b f0                	mov    esi,eax
100136aa:	8d 45 b0             	lea    eax,[ebp-0x50]
100136ad:	50                   	push   eax
100136ae:	ff 75 78             	push   DWORD PTR [ebp+0x78]
100136b1:	8d 7d b0             	lea    edi,[ebp-0x50]
100136b4:	ff 75 58             	push   DWORD PTR [ebp+0x58]
100136b7:	f3 a5                	rep movs DWORD PTR es:[edi],DWORD PTR ds:[esi]
100136b9:	83 4d b0 ff          	or     DWORD PTR [ebp-0x50],0xffffffff
100136bd:	6a 00                	push   0x0
100136bf:	c7 45 c0 01 00 00 00 	mov    DWORD PTR [ebp-0x40],0x1
100136c6:	e8 d5 e9 ff ff       	call   0x100120a0
100136cb:	8b f0                	mov    esi,eax
100136cd:	23 c3                	and    eax,ebx
100136cf:	33 ff                	xor    edi,edi
100136d1:	3b c3                	cmp    eax,ebx
100136d3:	75 03                	jne    0x100136d8
100136d5:	8b 75 d4             	mov    esi,DWORD PTR [ebp-0x2c]
100136d8:	56                   	push   esi
100136d9:	e8 3d e9 ff ff       	call   0x1001201b
100136de:	89 45 5c             	mov    DWORD PTR [ebp+0x5c],eax
100136e1:	39 7d 74             	cmp    DWORD PTR [ebp+0x74],edi
100136e4:	74 12                	je     0x100136f8
100136e6:	8b 7d 74             	mov    edi,DWORD PTR [ebp+0x74]
100136e9:	b9 00 01 00 00       	mov    ecx,0x100
100136ee:	8d b5 94 fb ff ff    	lea    esi,[ebp-0x46c]
100136f4:	f3 a5                	rep movs DWORD PTR es:[edi],DWORD PTR ds:[esi]
100136f6:	33 ff                	xor    edi,edi
100136f8:	ff 75 78             	push   DWORD PTR [ebp+0x78]
100136fb:	8d 45 5c             	lea    eax,[ebp+0x5c]
100136fe:	50                   	push   eax
100136ff:	ff 75 58             	push   DWORD PTR [ebp+0x58]
10013702:	8d 45 54             	lea    eax,[ebp+0x54]
10013705:	50                   	push   eax
10013706:	8d 45 50             	lea    eax,[ebp+0x50]
10013709:	50                   	push   eax
1001370a:	8d 45 4c             	lea    eax,[ebp+0x4c]
1001370d:	50                   	push   eax
1001370e:	8d 45 48             	lea    eax,[ebp+0x48]
10013711:	50                   	push   eax
10013712:	ff 75 3c             	push   DWORD PTR [ebp+0x3c]
10013715:	e8 d7 f5 ff ff       	call   0x10012cf1
1001371a:	8b f0                	mov    esi,eax
1001371c:	3b f7                	cmp    esi,edi
1001371e:	0f 8c e7 fd ff ff    	jl     0x1001350b
10013724:	8b 45 78             	mov    eax,DWORD PTR [ebp+0x78]
10013727:	83 e8 03             	sub    eax,0x3
1001372a:	bf f2 ff f8 7f       	mov    edi,0x7ff8fff2
1001372f:	bb 0e 00 07 80       	mov    ebx,0x8007000e
10013734:	74 30                	je     0x10013766
10013736:	83 e8 0e             	sub    eax,0xe
10013739:	74 18                	je     0x10013753
1001373b:	48                   	dec    eax
1001373c:	75 50                	jne    0x1001378e
1001373e:	6a 12                	push   0x12
10013740:	ff 75 60             	push   DWORD PTR [ebp+0x60]
10013743:	ff 75 5c             	push   DWORD PTR [ebp+0x5c]
10013746:	ff 75 58             	push   DWORD PTR [ebp+0x58]
10013749:	ff 75 54             	push   DWORD PTR [ebp+0x54]
1001374c:	6a 06                	push   0x6
1001374e:	ff 75 48             	push   DWORD PTR [ebp+0x48]
10013751:	eb 26                	jmp    0x10013779
10013753:	6a 11                	push   0x11
10013755:	ff 75 60             	push   DWORD PTR [ebp+0x60]
10013758:	ff 75 5c             	push   DWORD PTR [ebp+0x5c]
1001375b:	ff 75 58             	push   DWORD PTR [ebp+0x58]
1001375e:	ff 75 54             	push   DWORD PTR [ebp+0x54]
10013761:	ff 75 50             	push   DWORD PTR [ebp+0x50]
10013764:	eb 10                	jmp    0x10013776
10013766:	6a 03                	push   0x3
10013768:	ff 75 60             	push   DWORD PTR [ebp+0x60]
1001376b:	ff 75 5c             	push   DWORD PTR [ebp+0x5c]
1001376e:	ff 75 58             	push   DWORD PTR [ebp+0x58]
10013771:	ff 75 54             	push   DWORD PTR [ebp+0x54]
10013774:	6a 01                	push   0x1
10013776:	ff 75 4c             	push   DWORD PTR [ebp+0x4c]
10013779:	ff 75 48             	push   DWORD PTR [ebp+0x48]
1001377c:	e8 f7 a4 ff ff       	call   0x1000dc78
10013781:	8b f0                	mov    esi,eax
10013783:	89 75 70             	mov    DWORD PTR [ebp+0x70],esi
10013786:	f7 de                	neg    esi
10013788:	1b f6                	sbb    esi,esi
1001378a:	23 f7                	and    esi,edi
1001378c:	03 f3                	add    esi,ebx
1001378e:	33 ff                	xor    edi,edi
10013790:	3b f7                	cmp    esi,edi
10013792:	0f 8c b3 02 00 00    	jl     0x10013a4b
10013798:	8b 45 f4             	mov    eax,DWORD PTR [ebp-0xc]
1001379b:	89 45 5c             	mov    DWORD PTR [ebp+0x5c],eax
1001379e:	8b 45 f8             	mov    eax,DWORD PTR [ebp-0x8]
100137a1:	89 45 50             	mov    DWORD PTR [ebp+0x50],eax
100137a4:	8b 45 00             	mov    eax,DWORD PTR [ebp+0x0]
100137a7:	89 45 3c             	mov    DWORD PTR [ebp+0x3c],eax
100137aa:	8b 45 70             	mov    eax,DWORD PTR [ebp+0x70]
100137ad:	8b 40 30             	mov    eax,DWORD PTR [eax+0x30]
100137b0:	c1 e8 0b             	shr    eax,0xb
100137b3:	83 e0 01             	and    eax,0x1
100137b6:	39 7d 44             	cmp    DWORD PTR [ebp+0x44],edi
100137b9:	89 45 58             	mov    DWORD PTR [ebp+0x58],eax
100137bc:	8d 45 d4             	lea    eax,[ebp-0x2c]
100137bf:	89 45 60             	mov    DWORD PTR [ebp+0x60],eax
100137c2:	89 7d 4c             	mov    DWORD PTR [ebp+0x4c],edi
100137c5:	0f 86 2e 02 00 00    	jbe    0x100139f9
100137cb:	83 65 48 00          	and    DWORD PTR [ebp+0x48],0x0
100137cf:	83 7d 40 00          	cmp    DWORD PTR [ebp+0x40],0x0
100137d3:	8b 7d 60             	mov    edi,DWORD PTR [ebp+0x60]
100137d6:	0f 86 19 01 00 00    	jbe    0x100138f5
100137dc:	8b 45 48             	mov    eax,DWORD PTR [ebp+0x48]
100137df:	3b 45 54             	cmp    eax,DWORD PTR [ebp+0x54]
100137e2:	0f 83 0d 01 00 00    	jae    0x100138f5
100137e8:	8b 45 78             	mov    eax,DWORD PTR [ebp+0x78]
100137eb:	83 e8 03             	sub    eax,0x3
100137ee:	74 2a                	je     0x1001381a
100137f0:	83 e8 0e             	sub    eax,0xe
100137f3:	74 13                	je     0x10013808
100137f5:	48                   	dec    eax
100137f6:	75 3e                	jne    0x10013836
100137f8:	ff 75 48             	push   DWORD PTR [ebp+0x48]
100137fb:	ff 75 4c             	push   DWORD PTR [ebp+0x4c]
100137fe:	ff 75 70             	push   DWORD PTR [ebp+0x70]
10013801:	e8 5f a3 ff ff       	call   0x1000db65
10013806:	eb 1d                	jmp    0x10013825
10013808:	ff 75 48             	push   DWORD PTR [ebp+0x48]
1001380b:	ff 75 70             	push   DWORD PTR [ebp+0x70]
1001380e:	e8 eb a3 ff ff       	call   0x1000dbfe
10013813:	8b f0                	mov    esi,eax
10013815:	89 75 2c             	mov    DWORD PTR [ebp+0x2c],esi
10013818:	eb 10                	jmp    0x1001382a
1001381a:	ff 75 48             	push   DWORD PTR [ebp+0x48]
1001381d:	ff 75 70             	push   DWORD PTR [ebp+0x70]
10013820:	e8 ab a2 ff ff       	call   0x1000dad0
10013825:	8b f0                	mov    esi,eax
10013827:	89 75 30             	mov    DWORD PTR [ebp+0x30],esi
1001382a:	f7 de                	neg    esi
1001382c:	1b f6                	sbb    esi,esi
1001382e:	81 e6 f2 ff f8 7f    	and    esi,0x7ff8fff2
10013834:	03 f3                	add    esi,ebx
10013836:	85 f6                	test   esi,esi
10013838:	0f 8c f1 01 00 00    	jl     0x10013a2f
1001383e:	8b 45 78             	mov    eax,DWORD PTR [ebp+0x78]
10013841:	83 e8 03             	sub    eax,0x3
10013844:	74 08                	je     0x1001384e
10013846:	83 e8 0e             	sub    eax,0xe
10013849:	74 73                	je     0x100138be
1001384b:	48                   	dec    eax
1001384c:	75 31                	jne    0x1001387f
1001384e:	ff 75 6c             	push   DWORD PTR [ebp+0x6c]
10013851:	8d 47 18             	lea    eax,[edi+0x18]
10013854:	ff 75 64             	push   DWORD PTR [ebp+0x64]
10013857:	ff 75 50             	push   DWORD PTR [ebp+0x50]
1001385a:	ff 75 5c             	push   DWORD PTR [ebp+0x5c]
1001385d:	ff 75 58             	push   DWORD PTR [ebp+0x58]
10013860:	50                   	push   eax
10013861:	8d 85 94 fb ff ff    	lea    eax,[ebp-0x46c]
10013867:	50                   	push   eax
10013868:	ff 77 30             	push   DWORD PTR [edi+0x30]
1001386b:	ff 37                	push   DWORD PTR [edi]
1001386d:	ff 77 04             	push   DWORD PTR [edi+0x4]
10013870:	6a 00                	push   0x0
10013872:	ff 75 74             	push   DWORD PTR [ebp+0x74]
10013875:	ff 75 30             	push   DWORD PTR [ebp+0x30]
10013878:	e8 91 e8 ff ff       	call   0x1001210e
1001387d:	8b f0                	mov    esi,eax
1001387f:	85 f6                	test   esi,esi
10013881:	0f 8c a8 01 00 00    	jl     0x10013a2f
10013887:	83 7d 2c 00          	cmp    DWORD PTR [ebp+0x2c],0x0
1001388b:	74 0c                	je     0x10013899
1001388d:	ff 75 2c             	push   DWORD PTR [ebp+0x2c]
10013890:	e8 3d 8d ff ff       	call   0x1000c5d2
10013895:	83 65 2c 00          	and    DWORD PTR [ebp+0x2c],0x0
10013899:	83 7d 30 00          	cmp    DWORD PTR [ebp+0x30],0x0
1001389d:	74 0c                	je     0x100138ab
1001389f:	ff 75 30             	push   DWORD PTR [ebp+0x30]
100138a2:	e8 2b 8d ff ff       	call   0x1000c5d2
100138a7:	83 65 30 00          	and    DWORD PTR [ebp+0x30],0x0
100138ab:	ff 45 48             	inc    DWORD PTR [ebp+0x48]
100138ae:	8b 45 48             	mov    eax,DWORD PTR [ebp+0x48]
100138b1:	3b 45 40             	cmp    eax,DWORD PTR [ebp+0x40]
100138b4:	73 3f                	jae    0x100138f5
100138b6:	8b 7f 4c             	mov    edi,DWORD PTR [edi+0x4c]
100138b9:	e9 1e ff ff ff       	jmp    0x100137dc
100138be:	ff 75 6c             	push   DWORD PTR [ebp+0x6c]
100138c1:	8d 47 18             	lea    eax,[edi+0x18]
100138c4:	ff 75 64             	push   DWORD PTR [ebp+0x64]
100138c7:	ff 75 3c             	push   DWORD PTR [ebp+0x3c]
100138ca:	ff 75 50             	push   DWORD PTR [ebp+0x50]
100138cd:	ff 75 5c             	push   DWORD PTR [ebp+0x5c]
100138d0:	ff 75 58             	push   DWORD PTR [ebp+0x58]
100138d3:	50                   	push   eax
100138d4:	8d 85 94 fb ff ff    	lea    eax,[ebp-0x46c]
100138da:	50                   	push   eax
100138db:	ff 77 34             	push   DWORD PTR [edi+0x34]
100138de:	ff 77 30             	push   DWORD PTR [edi+0x30]
100138e1:	ff 37                	push   DWORD PTR [edi]
100138e3:	ff 77 04             	push   DWORD PTR [edi+0x4]
100138e6:	6a 00                	push   0x0
100138e8:	ff 75 74             	push   DWORD PTR [ebp+0x74]
100138eb:	ff 75 2c             	push   DWORD PTR [ebp+0x2c]
100138ee:	e8 ac ed ff ff       	call   0x1001269f
100138f3:	eb 88                	jmp    0x1001387d
100138f5:	83 7d 28 00          	cmp    DWORD PTR [ebp+0x28],0x0
100138f9:	0f 85 e2 00 00 00    	jne    0x100139e1
100138ff:	8b 45 54             	mov    eax,DWORD PTR [ebp+0x54]
10013902:	39 45 48             	cmp    DWORD PTR [ebp+0x48],eax
10013905:	0f 83 d6 00 00 00    	jae    0x100139e1
1001390b:	8b 45 78             	mov    eax,DWORD PTR [ebp+0x78]
1001390e:	83 e8 03             	sub    eax,0x3
10013911:	74 2a                	je     0x1001393d
10013913:	83 e8 0e             	sub    eax,0xe
10013916:	74 13                	je     0x1001392b
10013918:	48                   	dec    eax
10013919:	75 3e                	jne    0x10013959
1001391b:	ff 75 48             	push   DWORD PTR [ebp+0x48]
1001391e:	ff 75 4c             	push   DWORD PTR [ebp+0x4c]
10013921:	ff 75 70             	push   DWORD PTR [ebp+0x70]
10013924:	e8 3c a2 ff ff       	call   0x1000db65
10013929:	eb 1d                	jmp    0x10013948
1001392b:	ff 75 48             	push   DWORD PTR [ebp+0x48]
1001392e:	ff 75 70             	push   DWORD PTR [ebp+0x70]
10013931:	e8 c8 a2 ff ff       	call   0x1000dbfe
10013936:	8b f0                	mov    esi,eax
10013938:	89 75 2c             	mov    DWORD PTR [ebp+0x2c],esi
1001393b:	eb 10                	jmp    0x1001394d
1001393d:	ff 75 48             	push   DWORD PTR [ebp+0x48]
10013940:	ff 75 70             	push   DWORD PTR [ebp+0x70]
10013943:	e8 88 a1 ff ff       	call   0x1000dad0
10013948:	8b f0                	mov    esi,eax
1001394a:	89 75 30             	mov    DWORD PTR [ebp+0x30],esi
1001394d:	f7 de                	neg    esi
1001394f:	1b f6                	sbb    esi,esi
10013951:	81 e6 f2 ff f8 7f    	and    esi,0x7ff8fff2
10013957:	03 f3                	add    esi,ebx
10013959:	85 f6                	test   esi,esi
1001395b:	0f 8c ce 00 00 00    	jl     0x10013a2f
10013961:	8b 45 78             	mov    eax,DWORD PTR [ebp+0x78]
10013964:	83 e8 03             	sub    eax,0x3
10013967:	74 0c                	je     0x10013975
10013969:	83 e8 0e             	sub    eax,0xe
1001396c:	0f 84 ef 00 00 00    	je     0x10013a61
10013972:	48                   	dec    eax
10013973:	75 31                	jne    0x100139a6
10013975:	ff 75 6c             	push   DWORD PTR [ebp+0x6c]
10013978:	8d 47 18             	lea    eax,[edi+0x18]
1001397b:	ff 75 64             	push   DWORD PTR [ebp+0x64]
1001397e:	ff 75 50             	push   DWORD PTR [ebp+0x50]
10013981:	ff 75 5c             	push   DWORD PTR [ebp+0x5c]
10013984:	ff 75 58             	push   DWORD PTR [ebp+0x58]
10013987:	50                   	push   eax
10013988:	8d 85 94 fb ff ff    	lea    eax,[ebp-0x46c]
1001398e:	50                   	push   eax
1001398f:	ff 77 30             	push   DWORD PTR [edi+0x30]
10013992:	ff 37                	push   DWORD PTR [edi]
10013994:	ff 77 04             	push   DWORD PTR [edi+0x4]
10013997:	6a 00                	push   0x0
10013999:	ff 75 74             	push   DWORD PTR [ebp+0x74]
1001399c:	ff 75 30             	push   DWORD PTR [ebp+0x30]
1001399f:	e8 6a e7 ff ff       	call   0x1001210e
100139a4:	8b f0                	mov    esi,eax
100139a6:	85 f6                	test   esi,esi
100139a8:	0f 8c 81 00 00 00    	jl     0x10013a2f
100139ae:	83 7d 2c 00          	cmp    DWORD PTR [ebp+0x2c],0x0
100139b2:	74 0c                	je     0x100139c0
100139b4:	ff 75 2c             	push   DWORD PTR [ebp+0x2c]
100139b7:	e8 16 8c ff ff       	call   0x1000c5d2
100139bc:	83 65 2c 00          	and    DWORD PTR [ebp+0x2c],0x0
100139c0:	83 7d 30 00          	cmp    DWORD PTR [ebp+0x30],0x0
100139c4:	74 0c                	je     0x100139d2
100139c6:	ff 75 30             	push   DWORD PTR [ebp+0x30]
100139c9:	e8 04 8c ff ff       	call   0x1000c5d2
100139ce:	83 65 30 00          	and    DWORD PTR [ebp+0x30],0x0
100139d2:	ff 45 48             	inc    DWORD PTR [ebp+0x48]
100139d5:	8b 45 48             	mov    eax,DWORD PTR [ebp+0x48]
100139d8:	3b 45 54             	cmp    eax,DWORD PTR [ebp+0x54]
100139db:	0f 82 2a ff ff ff    	jb     0x1001390b
100139e1:	8b 45 60             	mov    eax,DWORD PTR [ebp+0x60]
100139e4:	8b 40 50             	mov    eax,DWORD PTR [eax+0x50]
100139e7:	ff 45 4c             	inc    DWORD PTR [ebp+0x4c]
100139ea:	89 45 60             	mov    DWORD PTR [ebp+0x60],eax
100139ed:	8b 45 4c             	mov    eax,DWORD PTR [ebp+0x4c]
100139f0:	3b 45 44             	cmp    eax,DWORD PTR [ebp+0x44]
100139f3:	0f 82 d2 fd ff ff    	jb     0x100137cb
100139f9:	83 7d 28 00          	cmp    DWORD PTR [ebp+0x28],0x0
100139fd:	74 22                	je     0x10013a21
100139ff:	8b 45 40             	mov    eax,DWORD PTR [ebp+0x40]
10013a02:	3b 45 54             	cmp    eax,DWORD PTR [ebp+0x54]
10013a05:	73 1a                	jae    0x10013a21
10013a07:	ff 75 68             	push   DWORD PTR [ebp+0x68]
10013a0a:	48                   	dec    eax
10013a0b:	50                   	push   eax
10013a0c:	8d 85 94 fb ff ff    	lea    eax,[ebp-0x46c]
10013a12:	50                   	push   eax
10013a13:	ff 75 70             	push   DWORD PTR [ebp+0x70]
10013a16:	e8 58 f7 ff ff       	call   0x10013173
10013a1b:	8b f0                	mov    esi,eax
10013a1d:	85 f6                	test   esi,esi
10013a1f:	7c 0e                	jl     0x10013a2f
10013a21:	8b 4d 70             	mov    ecx,DWORD PTR [ebp+0x70]
10013a24:	8b 45 7c             	mov    eax,DWORD PTR [ebp+0x7c]
10013a27:	83 65 70 00          	and    DWORD PTR [ebp+0x70],0x0
10013a2b:	89 08                	mov    DWORD PTR [eax],ecx
10013a2d:	33 f6                	xor    esi,esi
10013a2f:	33 ff                	xor    edi,edi
10013a31:	39 7d 2c             	cmp    DWORD PTR [ebp+0x2c],edi
10013a34:	74 08                	je     0x10013a3e
10013a36:	ff 75 2c             	push   DWORD PTR [ebp+0x2c]
10013a39:	e8 94 8b ff ff       	call   0x1000c5d2
10013a3e:	39 7d 30             	cmp    DWORD PTR [ebp+0x30],edi
10013a41:	74 08                	je     0x10013a4b
10013a43:	ff 75 30             	push   DWORD PTR [ebp+0x30]
10013a46:	e8 87 8b ff ff       	call   0x1000c5d2
10013a4b:	39 7d 70             	cmp    DWORD PTR [ebp+0x70],edi
10013a4e:	0f 84 b7 fa ff ff    	je     0x1001350b
10013a54:	ff 75 70             	push   DWORD PTR [ebp+0x70]
10013a57:	e8 76 8b ff ff       	call   0x1000c5d2
10013a5c:	e9 aa fa ff ff       	jmp    0x1001350b
10013a61:	ff 75 6c             	push   DWORD PTR [ebp+0x6c]
10013a64:	8d 47 18             	lea    eax,[edi+0x18]
10013a67:	ff 75 64             	push   DWORD PTR [ebp+0x64]
10013a6a:	ff 75 3c             	push   DWORD PTR [ebp+0x3c]
10013a6d:	ff 75 50             	push   DWORD PTR [ebp+0x50]
10013a70:	ff 75 5c             	push   DWORD PTR [ebp+0x5c]
10013a73:	ff 75 58             	push   DWORD PTR [ebp+0x58]
10013a76:	50                   	push   eax
10013a77:	8d 85 94 fb ff ff    	lea    eax,[ebp-0x46c]
10013a7d:	50                   	push   eax
10013a7e:	ff 77 34             	push   DWORD PTR [edi+0x34]
10013a81:	ff 77 30             	push   DWORD PTR [edi+0x30]
10013a84:	ff 37                	push   DWORD PTR [edi]
10013a86:	ff 77 04             	push   DWORD PTR [edi+0x4]
10013a89:	6a 00                	push   0x0
10013a8b:	ff 75 74             	push   DWORD PTR [ebp+0x74]
10013a8e:	ff 75 2c             	push   DWORD PTR [ebp+0x2c]
10013a91:	e8 09 ec ff ff       	call   0x1001269f
10013a96:	e9 09 ff ff ff       	jmp    0x100139a4
