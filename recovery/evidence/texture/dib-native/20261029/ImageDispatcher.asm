
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

10016804 <.text+0x15804>:
10016804:	55                   	push   ebp
10016805:	8b ec                	mov    ebp,esp
10016807:	83 ec 30             	sub    esp,0x30
1001680a:	53                   	push   ebx
1001680b:	56                   	push   esi
1001680c:	57                   	push   edi
1001680d:	33 ff                	xor    edi,edi
1001680f:	39 7d 08             	cmp    DWORD PTR [ebp+0x8],edi
10016812:	8b d9                	mov    ebx,ecx
10016814:	0f 84 79 02 00 00    	je     0x10016a93
1001681a:	39 7d 0c             	cmp    DWORD PTR [ebp+0xc],edi
1001681d:	0f 84 70 02 00 00    	je     0x10016a93
10016823:	8b 75 10             	mov    esi,DWORD PTR [ebp+0x10]
10016826:	3b f7                	cmp    esi,edi
10016828:	75 0c                	jne    0x10016836
1001682a:	39 7d 14             	cmp    DWORD PTR [ebp+0x14],edi
1001682d:	75 07                	jne    0x10016836
1001682f:	33 c0                	xor    eax,eax
10016831:	e9 62 02 00 00       	jmp    0x10016a98
10016836:	6a 04                	push   0x4
10016838:	58                   	pop    eax
10016839:	57                   	push   edi
1001683a:	50                   	push   eax
1001683b:	89 7d d0             	mov    DWORD PTR [ebp-0x30],edi
1001683e:	c7 45 d4 05 00 00 00 	mov    DWORD PTR [ebp-0x2c],0x5
10016845:	89 45 d8             	mov    DWORD PTR [ebp-0x28],eax
10016848:	c7 45 dc 01 00 00 00 	mov    DWORD PTR [ebp-0x24],0x1
1001684f:	c7 45 e0 03 00 00 00 	mov    DWORD PTR [ebp-0x20],0x3
10016856:	c7 45 e4 08 00 00 00 	mov    DWORD PTR [ebp-0x1c],0x8
1001685d:	c7 45 e8 07 00 00 00 	mov    DWORD PTR [ebp-0x18],0x7
10016864:	c7 45 ec 02 00 00 00 	mov    DWORD PTR [ebp-0x14],0x2
1001686b:	c7 45 f0 06 00 00 00 	mov    DWORD PTR [ebp-0x10],0x6
10016872:	e8 35 85 03 00       	call   0x1004edac
10016877:	50                   	push   eax
10016878:	e8 29 85 03 00       	call   0x1004eda6
1001687d:	83 c4 0c             	add    esp,0xc
10016880:	3b c7                	cmp    eax,edi
10016882:	89 45 f4             	mov    DWORD PTR [ebp-0xc],eax
10016885:	74 13                	je     0x1001689a
10016887:	8b f0                	mov    esi,eax
10016889:	6a 02                	push   0x2
1001688b:	bf 8c 93 09 10       	mov    edi,0x1009938c
10016890:	59                   	pop    ecx
10016891:	33 c0                	xor    eax,eax
10016893:	f3 a6                	repz cmps BYTE PTR ds:[esi],BYTE PTR es:[edi]
10016895:	8b 75 10             	mov    esi,DWORD PTR [ebp+0x10]
10016898:	74 0e                	je     0x100168a8
1001689a:	68 8c 93 09 10       	push   0x1009938c
1001689f:	6a 04                	push   0x4
100168a1:	e8 06 85 03 00       	call   0x1004edac
100168a6:	59                   	pop    ecx
100168a7:	59                   	pop    ecx
100168a8:	83 65 f8 00          	and    DWORD PTR [ebp-0x8],0x0
100168ac:	8b 45 14             	mov    eax,DWORD PTR [ebp+0x14]
100168af:	89 43 40             	mov    DWORD PTR [ebx+0x40],eax
100168b2:	8b 45 f8             	mov    eax,DWORD PTR [ebp-0x8]
100168b5:	8b 44 85 d0          	mov    eax,DWORD PTR [ebp+eax*4-0x30]
100168b9:	83 f8 08             	cmp    eax,0x8
100168bc:	c7 43 44 03 00 00 00 	mov    DWORD PTR [ebx+0x44],0x3
100168c3:	89 43 48             	mov    DWORD PTR [ebx+0x48],eax
100168c6:	0f 87 8f 00 00 00    	ja     0x1001695b
100168cc:	ff 24 85 a0 6a 01 10 	jmp    DWORD PTR [eax*4+0x10016aa0]
100168d3:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
100168d6:	8b cb                	mov    ecx,ebx
100168d8:	ff 75 08             	push   DWORD PTR [ebp+0x8]
100168db:	e8 cd fe ff ff       	call   0x100167ad
100168e0:	eb 76                	jmp    0x10016958
100168e2:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
100168e5:	8b cb                	mov    ecx,ebx
100168e7:	ff 75 08             	push   DWORD PTR [ebp+0x8]
100168ea:	e8 51 e9 ff ff       	call   0x10015240
100168ef:	eb 67                	jmp    0x10016958
100168f1:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
100168f4:	8b cb                	mov    ecx,ebx
100168f6:	ff 75 08             	push   DWORD PTR [ebp+0x8]
100168f9:	e8 e2 ef ff ff       	call   0x100158e0
100168fe:	eb 58                	jmp    0x10016958
10016900:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
10016903:	8b cb                	mov    ecx,ebx
10016905:	ff 75 08             	push   DWORD PTR [ebp+0x8]
10016908:	e8 a2 e0 ff ff       	call   0x100149af
1001690d:	eb 49                	jmp    0x10016958
1001690f:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
10016912:	8b cb                	mov    ecx,ebx
10016914:	ff 75 08             	push   DWORD PTR [ebp+0x8]
10016917:	e8 94 eb ff ff       	call   0x100154b0
1001691c:	eb 3a                	jmp    0x10016958
1001691e:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
10016921:	8b cb                	mov    ecx,ebx
10016923:	ff 75 08             	push   DWORD PTR [ebp+0x8]
10016926:	e8 4b f3 ff ff       	call   0x10015c76
1001692b:	eb 2b                	jmp    0x10016958
1001692d:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
10016930:	8b cb                	mov    ecx,ebx
10016932:	ff 75 08             	push   DWORD PTR [ebp+0x8]
10016935:	e8 2a f6 ff ff       	call   0x10015f64
1001693a:	eb 1c                	jmp    0x10016958
1001693c:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
1001693f:	8b cb                	mov    ecx,ebx
10016941:	ff 75 08             	push   DWORD PTR [ebp+0x8]
10016944:	e8 25 e3 ff ff       	call   0x10014c6e
10016949:	eb 0d                	jmp    0x10016958
1001694b:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
1001694e:	8b cb                	mov    ecx,ebx
10016950:	ff 75 08             	push   DWORD PTR [ebp+0x8]
10016953:	e8 e5 d5 ff ff       	call   0x10013f3d
10016958:	89 45 fc             	mov    DWORD PTR [ebp-0x4],eax
1001695b:	33 ff                	xor    edi,edi
1001695d:	39 7d fc             	cmp    DWORD PTR [ebp-0x4],edi
10016960:	7d 6f                	jge    0x100169d1
10016962:	8b 43 04             	mov    eax,DWORD PTR [ebx+0x4]
10016965:	3b c7                	cmp    eax,edi
10016967:	74 0b                	je     0x10016974
10016969:	39 7b 38             	cmp    DWORD PTR [ebx+0x38],edi
1001696c:	74 06                	je     0x10016974
1001696e:	50                   	push   eax
1001696f:	e8 16 b7 ff ff       	call   0x1001208a
10016974:	8b 43 08             	mov    eax,DWORD PTR [ebx+0x8]
10016977:	3b c7                	cmp    eax,edi
10016979:	74 0b                	je     0x10016986
1001697b:	39 7b 3c             	cmp    DWORD PTR [ebx+0x3c],edi
1001697e:	74 06                	je     0x10016986
10016980:	50                   	push   eax
10016981:	e8 04 b7 ff ff       	call   0x1001208a
10016986:	8b 7b 4c             	mov    edi,DWORD PTR [ebx+0x4c]
10016989:	85 ff                	test   edi,edi
1001698b:	74 0d                	je     0x1001699a
1001698d:	8b cf                	mov    ecx,edi
1001698f:	e8 54 d5 ff ff       	call   0x10013ee8
10016994:	57                   	push   edi
10016995:	e8 f0 b6 ff ff       	call   0x1001208a
1001699a:	8b 7b 50             	mov    edi,DWORD PTR [ebx+0x50]
1001699d:	33 c0                	xor    eax,eax
1001699f:	3b f8                	cmp    edi,eax
100169a1:	74 0f                	je     0x100169b2
100169a3:	8b cf                	mov    ecx,edi
100169a5:	e8 3e d5 ff ff       	call   0x10013ee8
100169aa:	57                   	push   edi
100169ab:	e8 da b6 ff ff       	call   0x1001208a
100169b0:	33 c0                	xor    eax,eax
100169b2:	ff 45 f8             	inc    DWORD PTR [ebp-0x8]
100169b5:	83 7d f8 09          	cmp    DWORD PTR [ebp-0x8],0x9
100169b9:	89 43 04             	mov    DWORD PTR [ebx+0x4],eax
100169bc:	89 43 08             	mov    DWORD PTR [ebx+0x8],eax
100169bf:	89 43 38             	mov    DWORD PTR [ebx+0x38],eax
100169c2:	89 43 3c             	mov    DWORD PTR [ebx+0x3c],eax
100169c5:	89 43 4c             	mov    DWORD PTR [ebx+0x4c],eax
100169c8:	89 43 50             	mov    DWORD PTR [ebx+0x50],eax
100169cb:	0f 82 db fe ff ff    	jb     0x100168ac
100169d1:	83 7d f4 00          	cmp    DWORD PTR [ebp-0xc],0x0
100169d5:	74 29                	je     0x10016a00
100169d7:	8b 75 f4             	mov    esi,DWORD PTR [ebp-0xc]
100169da:	6a 02                	push   0x2
100169dc:	bf 8c 93 09 10       	mov    edi,0x1009938c
100169e1:	59                   	pop    ecx
100169e2:	33 c0                	xor    eax,eax
100169e4:	f3 a6                	repz cmps BYTE PTR ds:[esi],BYTE PTR es:[edi]
100169e6:	74 0c                	je     0x100169f4
100169e8:	ff 75 f4             	push   DWORD PTR [ebp-0xc]
100169eb:	6a 04                	push   0x4
100169ed:	e8 ba 83 03 00       	call   0x1004edac
100169f2:	59                   	pop    ecx
100169f3:	59                   	pop    ecx
100169f4:	ff 75 f4             	push   DWORD PTR [ebp-0xc]
100169f7:	e8 5c 7d 03 00       	call   0x1004e758
100169fc:	8b 75 10             	mov    esi,DWORD PTR [ebp+0x10]
100169ff:	59                   	pop    ecx
10016a00:	83 7d f8 09          	cmp    DWORD PTR [ebp-0x8],0x9
10016a04:	75 0a                	jne    0x10016a10
10016a06:	b8 59 0b 76 88       	mov    eax,0x88760b59
10016a0b:	e9 88 00 00 00       	jmp    0x10016a98
10016a10:	85 f6                	test   esi,esi
10016a12:	74 49                	je     0x10016a5d
10016a14:	6a 07                	push   0x7
10016a16:	33 c0                	xor    eax,eax
10016a18:	59                   	pop    ecx
10016a19:	8b fe                	mov    edi,esi
10016a1b:	f3 ab                	rep stos DWORD PTR es:[edi],eax
10016a1d:	8b 43 0c             	mov    eax,DWORD PTR [ebx+0xc]
10016a20:	89 06                	mov    DWORD PTR [esi],eax
10016a22:	8b 43 10             	mov    eax,DWORD PTR [ebx+0x10]
10016a25:	89 46 04             	mov    DWORD PTR [esi+0x4],eax
10016a28:	8b 43 14             	mov    eax,DWORD PTR [ebx+0x14]
10016a2b:	89 46 08             	mov    DWORD PTR [esi+0x8],eax
10016a2e:	c7 46 0c 01 00 00 00 	mov    DWORD PTR [esi+0xc],0x1
10016a35:	ff 33                	push   DWORD PTR [ebx]
10016a37:	e8 df b5 ff ff       	call   0x1001201b
10016a3c:	89 46 10             	mov    DWORD PTR [esi+0x10],eax
10016a3f:	8b 43 44             	mov    eax,DWORD PTR [ebx+0x44]
10016a42:	89 46 14             	mov    DWORD PTR [esi+0x14],eax
10016a45:	8b 43 48             	mov    eax,DWORD PTR [ebx+0x48]
10016a48:	89 46 18             	mov    DWORD PTR [esi+0x18],eax
10016a4b:	8d 43 4c             	lea    eax,[ebx+0x4c]
10016a4e:	eb 08                	jmp    0x10016a58
10016a50:	ff 46 0c             	inc    DWORD PTR [esi+0xc]
10016a53:	8b 00                	mov    eax,DWORD PTR [eax]
10016a55:	83 c0 4c             	add    eax,0x4c
10016a58:	83 38 00             	cmp    DWORD PTR [eax],0x0
10016a5b:	75 f3                	jne    0x10016a50
10016a5d:	33 c9                	xor    ecx,ecx
10016a5f:	3b d9                	cmp    ebx,ecx
10016a61:	8b c3                	mov    eax,ebx
10016a63:	74 22                	je     0x10016a87
10016a65:	8b 50 0c             	mov    edx,DWORD PTR [eax+0xc]
10016a68:	89 50 20             	mov    DWORD PTR [eax+0x20],edx
10016a6b:	8b 50 10             	mov    edx,DWORD PTR [eax+0x10]
10016a6e:	89 50 24             	mov    DWORD PTR [eax+0x24],edx
10016a71:	8b 50 14             	mov    edx,DWORD PTR [eax+0x14]
10016a74:	89 48 18             	mov    DWORD PTR [eax+0x18],ecx
10016a77:	89 48 1c             	mov    DWORD PTR [eax+0x1c],ecx
10016a7a:	89 48 28             	mov    DWORD PTR [eax+0x28],ecx
10016a7d:	89 50 2c             	mov    DWORD PTR [eax+0x2c],edx
10016a80:	8b 40 4c             	mov    eax,DWORD PTR [eax+0x4c]
10016a83:	3b c1                	cmp    eax,ecx
10016a85:	75 de                	jne    0x10016a65
10016a87:	8b 5b 50             	mov    ebx,DWORD PTR [ebx+0x50]
10016a8a:	3b d9                	cmp    ebx,ecx
10016a8c:	75 d1                	jne    0x10016a5f
10016a8e:	e9 9c fd ff ff       	jmp    0x1001682f
10016a93:	b8 6c 08 76 88       	mov    eax,0x8876086c
10016a98:	5f                   	pop    edi
10016a99:	5e                   	pop    esi
10016a9a:	5b                   	pop    ebx
10016a9b:	c9                   	leave
10016a9c:	c2 10 00             	ret    0x10
