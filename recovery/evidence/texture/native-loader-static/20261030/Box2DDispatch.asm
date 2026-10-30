
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

10017825 <.text+0x16825>:
10017825:	55                   	push   ebp
10017826:	8b ec                	mov    ebp,esp
10017828:	83 ec 58             	sub    esp,0x58
1001782b:	56                   	push   esi
1001782c:	8b f1                	mov    esi,ecx
1001782e:	80 7e 08 05          	cmp    BYTE PTR [esi+0x8],0x5
10017832:	74 0a                	je     0x1001783e
10017834:	b8 05 40 00 80       	mov    eax,0x80004005
10017839:	e9 49 03 00 00       	jmp    0x10017b87
1001783e:	8b 06                	mov    eax,DWORD PTR [esi]
10017840:	8b 4e 04             	mov    ecx,DWORD PTR [esi+0x4]
10017843:	53                   	push   ebx
10017844:	8b 58 68             	mov    ebx,DWORD PTR [eax+0x68]
10017847:	8b d3                	mov    edx,ebx
10017849:	57                   	push   edi
1001784a:	8b 79 68             	mov    edi,DWORD PTR [ecx+0x68]
1001784d:	d1 ea                	shr    edx,1
1001784f:	3b fa                	cmp    edi,edx
10017851:	74 11                	je     0x10017864
10017853:	83 ff 01             	cmp    edi,0x1
10017856:	0f 85 24 03 00 00    	jne    0x10017b80
1001785c:	3b df                	cmp    ebx,edi
1001785e:	0f 85 1c 03 00 00    	jne    0x10017b80
10017864:	8b 78 6c             	mov    edi,DWORD PTR [eax+0x6c]
10017867:	8b 51 6c             	mov    edx,DWORD PTR [ecx+0x6c]
1001786a:	d1 ef                	shr    edi,1
1001786c:	3b d7                	cmp    edx,edi
1001786e:	74 12                	je     0x10017882
10017870:	83 fa 01             	cmp    edx,0x1
10017873:	0f 85 07 03 00 00    	jne    0x10017b80
10017879:	39 50 6c             	cmp    DWORD PTR [eax+0x6c],edx
1001787c:	0f 85 fe 02 00 00    	jne    0x10017b80
10017882:	33 d2                	xor    edx,edx
10017884:	42                   	inc    edx
10017885:	39 51 70             	cmp    DWORD PTR [ecx+0x70],edx
10017888:	0f 85 f2 02 00 00    	jne    0x10017b80
1001788e:	39 50 70             	cmp    DWORD PTR [eax+0x70],edx
10017891:	0f 85 e9 02 00 00    	jne    0x10017b80
10017897:	3b da                	cmp    ebx,edx
10017899:	76 10                	jbe    0x100178ab
1001789b:	84 da                	test   dl,bl
1001789d:	74 0c                	je     0x100178ab
1001789f:	83 60 68 fe          	and    DWORD PTR [eax+0x68],0xfffffffe
100178a3:	8b 06                	mov    eax,DWORD PTR [esi]
100178a5:	8b 48 78             	mov    ecx,DWORD PTR [eax+0x78]
100178a8:	29 48 74             	sub    DWORD PTR [eax+0x74],ecx
100178ab:	8b 06                	mov    eax,DWORD PTR [esi]
100178ad:	39 50 6c             	cmp    DWORD PTR [eax+0x6c],edx
100178b0:	76 04                	jbe    0x100178b6
100178b2:	83 60 6c fe          	and    DWORD PTR [eax+0x6c],0xfffffffe
100178b6:	f7 46 08 00 00 08 00 	test   DWORD PTR [esi+0x8],0x80000
100178bd:	0f 85 ec 00 00 00    	jne    0x100179af
100178c3:	8b 56 04             	mov    edx,DWORD PTR [esi+0x4]
100178c6:	8b 06                	mov    eax,DWORD PTR [esi]
100178c8:	8b 78 04             	mov    edi,DWORD PTR [eax+0x4]
100178cb:	8b 5a 04             	mov    ebx,DWORD PTR [edx+0x4]
100178ce:	b9 3f fe ff ff       	mov    ecx,0xfffffe3f
100178d3:	23 f9                	and    edi,ecx
100178d5:	23 d9                	and    ebx,ecx
100178d7:	3b fb                	cmp    edi,ebx
100178d9:	0f 85 d0 00 00 00    	jne    0x100179af
100178df:	83 78 68 02          	cmp    DWORD PTR [eax+0x68],0x2
100178e3:	0f 82 c6 00 00 00    	jb     0x100179af
100178e9:	83 78 6c 02          	cmp    DWORD PTR [eax+0x6c],0x2
100178ed:	0f 82 bc 00 00 00    	jb     0x100179af
100178f3:	83 7a 10 00          	cmp    DWORD PTR [edx+0x10],0x0
100178f7:	0f 85 b2 00 00 00    	jne    0x100179af
100178fd:	83 78 10 00          	cmp    DWORD PTR [eax+0x10],0x0
10017901:	0f 85 a8 00 00 00    	jne    0x100179af
10017907:	b8 02 00 00 28       	mov    eax,0x28000002
1001790c:	3b f8                	cmp    edi,eax
1001790e:	7f 59                	jg     0x10017969
10017910:	74 4e                	je     0x10017960
10017912:	81 ef 02 00 90 04    	sub    edi,0x4900002
10017918:	74 46                	je     0x10017960
1001791a:	81 ef 08 00 70 03    	sub    edi,0x3700008
10017920:	74 35                	je     0x10017957
10017922:	81 ef f9 ff 27 10    	sub    edi,0x1027fff9
10017928:	74 24                	je     0x1001794e
1001792a:	83 ef 03             	sub    edi,0x3
1001792d:	74 0d                	je     0x1001793c
1001792f:	83 ef 09             	sub    edi,0x9
10017932:	74 11                	je     0x10017945
10017934:	81 ef f7 ff f7 01    	sub    edi,0x1f7fff7
1001793a:	75 73                	jne    0x100179af
1001793c:	8b ce                	mov    ecx,esi
1001793e:	e8 59 f5 ff ff       	call   0x10016e9c
10017943:	eb 62                	jmp    0x100179a7
10017945:	8b ce                	mov    ecx,esi
10017947:	e8 e3 9d 02 00       	call   0x1004172f
1001794c:	eb 59                	jmp    0x100179a7
1001794e:	8b ce                	mov    ecx,esi
10017950:	e8 9b 9c 02 00       	call   0x100415f0
10017955:	eb 50                	jmp    0x100179a7
10017957:	8b ce                	mov    ecx,esi
10017959:	e8 f1 f8 ff ff       	call   0x1001724f
1001795e:	eb 47                	jmp    0x100179a7
10017960:	8b ce                	mov    ecx,esi
10017962:	e8 0b 9f 02 00       	call   0x10041872
10017967:	eb 3e                	jmp    0x100179a7
10017969:	81 ef 03 00 28 28    	sub    edi,0x28280003
1001796f:	74 2f                	je     0x100179a0
10017971:	4f                   	dec    edi
10017972:	74 23                	je     0x10017997
10017974:	4f                   	dec    edi
10017975:	4f                   	dec    edi
10017976:	74 0d                	je     0x10017985
10017978:	83 ef 09             	sub    edi,0x9
1001797b:	74 11                	je     0x1001798e
1001797d:	81 ef f7 ff f7 01    	sub    edi,0x1f7fff7
10017983:	75 2a                	jne    0x100179af
10017985:	8b ce                	mov    ecx,esi
10017987:	e8 4b f6 ff ff       	call   0x10016fd7
1001798c:	eb 19                	jmp    0x100179a7
1001798e:	8b ce                	mov    ecx,esi
10017990:	e8 67 9f 02 00       	call   0x100418fc
10017995:	eb 10                	jmp    0x100179a7
10017997:	8b ce                	mov    ecx,esi
10017999:	e8 5b f6 ff ff       	call   0x10016ff9
1001799e:	eb 07                	jmp    0x100179a7
100179a0:	8b ce                	mov    ecx,esi
100179a2:	e8 92 f7 ff ff       	call   0x10017139
100179a7:	85 c0                	test   eax,eax
100179a9:	0f 8d cd 01 00 00    	jge    0x10017b7c
