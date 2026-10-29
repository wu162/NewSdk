
SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1003c85b <.text+0x3b85b>:
1003c85b:	55                   	push   ebp
1003c85c:	8b ec                	mov    ebp,esp
1003c85e:	d9 45 54             	fld    DWORD PTR [ebp+0x54]
1003c861:	83 ec 10             	sub    esp,0x10
1003c864:	d9 45 0c             	fld    DWORD PTR [ebp+0xc]
1003c867:	53                   	push   ebx
1003c868:	de d9                	fcompp
1003c86a:	56                   	push   esi
1003c86b:	57                   	push   edi
1003c86c:	df e0                	fnstsw ax
1003c86e:	f6 c4 05             	test   ah,0x5
1003c871:	7a 05                	jp     0x1003c878
1003c873:	d9 45 54             	fld    DWORD PTR [ebp+0x54]
1003c876:	eb 03                	jmp    0x1003c87b
1003c878:	d9 45 0c             	fld    DWORD PTR [ebp+0xc]
1003c87b:	d9 5d f0             	fstp   DWORD PTR [ebp-0x10]
1003c87e:	d9 45 58             	fld    DWORD PTR [ebp+0x58]
1003c881:	d9 45 10             	fld    DWORD PTR [ebp+0x10]
1003c884:	de d9                	fcompp
1003c886:	df e0                	fnstsw ax
1003c888:	f6 c4 05             	test   ah,0x5
1003c88b:	7a 05                	jp     0x1003c892
1003c88d:	d9 45 58             	fld    DWORD PTR [ebp+0x58]
1003c890:	eb 03                	jmp    0x1003c895
1003c892:	d9 45 10             	fld    DWORD PTR [ebp+0x10]
1003c895:	d9 5d f4             	fstp   DWORD PTR [ebp-0xc]
1003c898:	d9 45 5c             	fld    DWORD PTR [ebp+0x5c]
1003c89b:	d9 45 14             	fld    DWORD PTR [ebp+0x14]
1003c89e:	de d9                	fcompp
1003c8a0:	df e0                	fnstsw ax
1003c8a2:	f6 c4 05             	test   ah,0x5
1003c8a5:	7a 05                	jp     0x1003c8ac
1003c8a7:	d9 45 5c             	fld    DWORD PTR [ebp+0x5c]
1003c8aa:	eb 03                	jmp    0x1003c8af
1003c8ac:	d9 45 14             	fld    DWORD PTR [ebp+0x14]
1003c8af:	d9 5d f8             	fstp   DWORD PTR [ebp-0x8]
1003c8b2:	d9 45 60             	fld    DWORD PTR [ebp+0x60]
1003c8b5:	d9 45 18             	fld    DWORD PTR [ebp+0x18]
1003c8b8:	de d9                	fcompp
1003c8ba:	df e0                	fnstsw ax
1003c8bc:	f6 c4 05             	test   ah,0x5
1003c8bf:	7a 05                	jp     0x1003c8c6
1003c8c1:	d9 45 60             	fld    DWORD PTR [ebp+0x60]
1003c8c4:	eb 03                	jmp    0x1003c8c9
1003c8c6:	d9 45 18             	fld    DWORD PTR [ebp+0x18]
1003c8c9:	d9 5d fc             	fstp   DWORD PTR [ebp-0x4]
1003c8cc:	d9 45 64             	fld    DWORD PTR [ebp+0x64]
1003c8cf:	d9 45 f0             	fld    DWORD PTR [ebp-0x10]
1003c8d2:	de d9                	fcompp
1003c8d4:	df e0                	fnstsw ax
1003c8d6:	f6 c4 41             	test   ah,0x41
1003c8d9:	75 05                	jne    0x1003c8e0
1003c8db:	d9 45 64             	fld    DWORD PTR [ebp+0x64]
1003c8de:	eb 03                	jmp    0x1003c8e3
1003c8e0:	d9 45 f0             	fld    DWORD PTR [ebp-0x10]
1003c8e3:	d9 5d 54             	fstp   DWORD PTR [ebp+0x54]
1003c8e6:	d9 45 68             	fld    DWORD PTR [ebp+0x68]
1003c8e9:	d9 45 f4             	fld    DWORD PTR [ebp-0xc]
1003c8ec:	de d9                	fcompp
1003c8ee:	df e0                	fnstsw ax
1003c8f0:	f6 c4 41             	test   ah,0x41
1003c8f3:	75 05                	jne    0x1003c8fa
1003c8f5:	d9 45 68             	fld    DWORD PTR [ebp+0x68]
1003c8f8:	eb 03                	jmp    0x1003c8fd
1003c8fa:	d9 45 f4             	fld    DWORD PTR [ebp-0xc]
1003c8fd:	d9 5d 58             	fstp   DWORD PTR [ebp+0x58]
1003c900:	d9 45 6c             	fld    DWORD PTR [ebp+0x6c]
1003c903:	d9 45 f8             	fld    DWORD PTR [ebp-0x8]
1003c906:	de d9                	fcompp
1003c908:	df e0                	fnstsw ax
1003c90a:	f6 c4 41             	test   ah,0x41
1003c90d:	75 05                	jne    0x1003c914
1003c90f:	d9 45 6c             	fld    DWORD PTR [ebp+0x6c]
1003c912:	eb 03                	jmp    0x1003c917
1003c914:	d9 45 f8             	fld    DWORD PTR [ebp-0x8]
1003c917:	d9 5d 5c             	fstp   DWORD PTR [ebp+0x5c]
1003c91a:	d9 45 70             	fld    DWORD PTR [ebp+0x70]
1003c91d:	d9 45 fc             	fld    DWORD PTR [ebp-0x4]
1003c920:	de d9                	fcompp
1003c922:	df e0                	fnstsw ax
1003c924:	f6 c4 41             	test   ah,0x41
1003c927:	75 05                	jne    0x1003c92e
1003c929:	d9 45 70             	fld    DWORD PTR [ebp+0x70]
1003c92c:	eb 03                	jmp    0x1003c931
1003c92e:	d9 45 fc             	fld    DWORD PTR [ebp-0x4]
1003c931:	d9 5d 60             	fstp   DWORD PTR [ebp+0x60]
1003c934:	83 cf ff             	or     edi,0xffffffff
1003c937:	d9 45 34             	fld    DWORD PTR [ebp+0x34]
1003c93a:	d8 4d 54             	fmul   DWORD PTR [ebp+0x54]
1003c93d:	d8 45 44             	fadd   DWORD PTR [ebp+0x44]
1003c940:	d9 5d 0c             	fstp   DWORD PTR [ebp+0xc]
1003c943:	d9 45 38             	fld    DWORD PTR [ebp+0x38]
1003c946:	d8 4d 58             	fmul   DWORD PTR [ebp+0x58]
1003c949:	d8 45 48             	fadd   DWORD PTR [ebp+0x48]
1003c94c:	d9 5d 10             	fstp   DWORD PTR [ebp+0x10]
1003c94f:	d9 45 3c             	fld    DWORD PTR [ebp+0x3c]
1003c952:	d8 4d 5c             	fmul   DWORD PTR [ebp+0x5c]
1003c955:	d8 45 4c             	fadd   DWORD PTR [ebp+0x4c]
1003c958:	d9 5d 14             	fstp   DWORD PTR [ebp+0x14]
1003c95b:	d9 45 40             	fld    DWORD PTR [ebp+0x40]
1003c95e:	d8 4d 60             	fmul   DWORD PTR [ebp+0x60]
1003c961:	d8 45 50             	fadd   DWORD PTR [ebp+0x50]
1003c964:	d9 5d 18             	fstp   DWORD PTR [ebp+0x18]
1003c967:	d9 05 08 80 09 10    	fld    DWORD PTR ds:0x10098008
1003c96d:	d9 55 60             	fst    DWORD PTR [ebp+0x60]
1003c970:	d9 55 5c             	fst    DWORD PTR [ebp+0x5c]
1003c973:	d9 55 58             	fst    DWORD PTR [ebp+0x58]
1003c976:	d9 5d 54             	fstp   DWORD PTR [ebp+0x54]
1003c979:	d9 05 08 13 0a 10    	fld    DWORD PTR ds:0x100a1308
1003c97f:	d9 55 70             	fst    DWORD PTR [ebp+0x70]
1003c982:	d9 55 6c             	fst    DWORD PTR [ebp+0x6c]
1003c985:	d9 55 68             	fst    DWORD PTR [ebp+0x68]
1003c988:	d9 5d 64             	fstp   DWORD PTR [ebp+0x64]
1003c98b:	d9 ee                	fldz
1003c98d:	d8 55 0c             	fcom   DWORD PTR [ebp+0xc]
1003c990:	df e0                	fnstsw ax
1003c992:	f6 c4 41             	test   ah,0x41
1003c995:	75 04                	jne    0x1003c99b
1003c997:	8b f7                	mov    esi,edi
1003c999:	eb 02                	jmp    0x1003c99d
1003c99b:	33 f6                	xor    esi,esi
1003c99d:	d8 55 10             	fcom   DWORD PTR [ebp+0x10]
1003c9a0:	df e0                	fnstsw ax
1003c9a2:	f6 c4 41             	test   ah,0x41
1003c9a5:	75 04                	jne    0x1003c9ab
1003c9a7:	8b d7                	mov    edx,edi
1003c9a9:	eb 02                	jmp    0x1003c9ad
1003c9ab:	33 d2                	xor    edx,edx
1003c9ad:	d8 55 14             	fcom   DWORD PTR [ebp+0x14]
1003c9b0:	df e0                	fnstsw ax
1003c9b2:	f6 c4 41             	test   ah,0x41
1003c9b5:	75 04                	jne    0x1003c9bb
1003c9b7:	8b cf                	mov    ecx,edi
1003c9b9:	eb 02                	jmp    0x1003c9bd
1003c9bb:	33 c9                	xor    ecx,ecx
1003c9bd:	d8 5d 18             	fcomp  DWORD PTR [ebp+0x18]
1003c9c0:	df e0                	fnstsw ax
1003c9c2:	f6 c4 41             	test   ah,0x41
1003c9c5:	75 04                	jne    0x1003c9cb
1003c9c7:	8b c7                	mov    eax,edi
1003c9c9:	eb 02                	jmp    0x1003c9cd
1003c9cb:	33 c0                	xor    eax,eax
1003c9cd:	8b 5d 64             	mov    ebx,DWORD PTR [ebp+0x64]
1003c9d0:	8b fe                	mov    edi,esi
1003c9d2:	f7 d7                	not    edi
1003c9d4:	23 7d 54             	and    edi,DWORD PTR [ebp+0x54]
1003c9d7:	23 de                	and    ebx,esi
1003c9d9:	8b f2                	mov    esi,edx
1003c9db:	0b fb                	or     edi,ebx
1003c9dd:	f7 d6                	not    esi
1003c9df:	23 75 58             	and    esi,DWORD PTR [ebp+0x58]
1003c9e2:	89 7d 34             	mov    DWORD PTR [ebp+0x34],edi
1003c9e5:	d9 45 34             	fld    DWORD PTR [ebp+0x34]
1003c9e8:	d8 45 0c             	fadd   DWORD PTR [ebp+0xc]
1003c9eb:	8b 7d 68             	mov    edi,DWORD PTR [ebp+0x68]
1003c9ee:	23 fa                	and    edi,edx
1003c9f0:	8b d1                	mov    edx,ecx
1003c9f2:	d9 5d 64             	fstp   DWORD PTR [ebp+0x64]
1003c9f5:	0b f7                	or     esi,edi
1003c9f7:	f7 d2                	not    edx
1003c9f9:	23 55 5c             	and    edx,DWORD PTR [ebp+0x5c]
1003c9fc:	89 75 38             	mov    DWORD PTR [ebp+0x38],esi
1003c9ff:	d9 45 38             	fld    DWORD PTR [ebp+0x38]
1003ca02:	d8 45 10             	fadd   DWORD PTR [ebp+0x10]
1003ca05:	8b 75 6c             	mov    esi,DWORD PTR [ebp+0x6c]
1003ca08:	23 f1                	and    esi,ecx
1003ca0a:	0b d6                	or     edx,esi
1003ca0c:	d9 5d 68             	fstp   DWORD PTR [ebp+0x68]
1003ca0f:	89 55 3c             	mov    DWORD PTR [ebp+0x3c],edx
1003ca12:	8b 55 70             	mov    edx,DWORD PTR [ebp+0x70]
1003ca15:	d9 45 3c             	fld    DWORD PTR [ebp+0x3c]
1003ca18:	d8 45 14             	fadd   DWORD PTR [ebp+0x14]
1003ca1b:	8b c8                	mov    ecx,eax
1003ca1d:	f7 d1                	not    ecx
1003ca1f:	23 4d 60             	and    ecx,DWORD PTR [ebp+0x60]
1003ca22:	23 d0                	and    edx,eax
1003ca24:	d9 5d 6c             	fstp   DWORD PTR [ebp+0x6c]
1003ca27:	0b ca                	or     ecx,edx
1003ca29:	89 4d 40             	mov    DWORD PTR [ebp+0x40],ecx
1003ca2c:	d9 45 40             	fld    DWORD PTR [ebp+0x40]
1003ca2f:	d8 45 18             	fadd   DWORD PTR [ebp+0x18]
1003ca32:	d9 5d 70             	fstp   DWORD PTR [ebp+0x70]
1003ca35:	d9 45 64             	fld    DWORD PTR [ebp+0x64]
1003ca38:	e8 f3 1d 01 00       	call   0x1004e830
1003ca3d:	89 45 60             	mov    DWORD PTR [ebp+0x60],eax
1003ca40:	db 45 60             	fild   DWORD PTR [ebp+0x60]
1003ca43:	d9 5d 34             	fstp   DWORD PTR [ebp+0x34]
1003ca46:	d9 45 68             	fld    DWORD PTR [ebp+0x68]
1003ca49:	e8 e2 1d 01 00       	call   0x1004e830
1003ca4e:	89 45 60             	mov    DWORD PTR [ebp+0x60],eax
1003ca51:	db 45 60             	fild   DWORD PTR [ebp+0x60]
1003ca54:	d9 5d 38             	fstp   DWORD PTR [ebp+0x38]
1003ca57:	d9 45 6c             	fld    DWORD PTR [ebp+0x6c]
1003ca5a:	e8 d1 1d 01 00       	call   0x1004e830
1003ca5f:	89 45 60             	mov    DWORD PTR [ebp+0x60],eax
1003ca62:	db 45 60             	fild   DWORD PTR [ebp+0x60]
1003ca65:	d9 5d 3c             	fstp   DWORD PTR [ebp+0x3c]
1003ca68:	d9 45 70             	fld    DWORD PTR [ebp+0x70]
1003ca6b:	e8 c0 1d 01 00       	call   0x1004e830
1003ca70:	89 45 60             	mov    DWORD PTR [ebp+0x60],eax
1003ca73:	db 45 60             	fild   DWORD PTR [ebp+0x60]
1003ca76:	8d 75 34             	lea    esi,[ebp+0x34]
1003ca79:	8d 7d 54             	lea    edi,[ebp+0x54]
1003ca7c:	d9 5d 40             	fstp   DWORD PTR [ebp+0x40]
1003ca7f:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ca80:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ca81:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ca82:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ca83:	33 ff                	xor    edi,edi
1003ca85:	8d 74 bd 54          	lea    esi,[ebp+edi*4+0x54]
1003ca89:	d9 06                	fld    DWORD PTR [esi]
1003ca8b:	e8 d6 1d 01 00       	call   0x1004e866
1003ca90:	47                   	inc    edi
1003ca91:	83 ff 04             	cmp    edi,0x4
1003ca94:	89 06                	mov    DWORD PTR [esi],eax
1003ca96:	72 ed                	jb     0x1003ca85
1003ca98:	8b 75 24             	mov    esi,DWORD PTR [ebp+0x24]
1003ca9b:	8b 45 54             	mov    eax,DWORD PTR [ebp+0x54]
1003ca9e:	23 45 24             	and    eax,DWORD PTR [ebp+0x24]
1003caa1:	8b 4d 58             	mov    ecx,DWORD PTR [ebp+0x58]
1003caa4:	23 4d 28             	and    ecx,DWORD PTR [ebp+0x28]
1003caa7:	8b 55 5c             	mov    edx,DWORD PTR [ebp+0x5c]
1003caaa:	23 55 2c             	and    edx,DWORD PTR [ebp+0x2c]
1003caad:	8b 7d 60             	mov    edi,DWORD PTR [ebp+0x60]
1003cab0:	23 7d 30             	and    edi,DWORD PTR [ebp+0x30]
1003cab3:	f7 d6                	not    esi
1003cab5:	23 75 0c             	and    esi,DWORD PTR [ebp+0xc]
1003cab8:	0b f0                	or     esi,eax
1003caba:	8b 45 28             	mov    eax,DWORD PTR [ebp+0x28]
1003cabd:	f7 d0                	not    eax
1003cabf:	23 45 10             	and    eax,DWORD PTR [ebp+0x10]
1003cac2:	0b c1                	or     eax,ecx
1003cac4:	8b 4d 2c             	mov    ecx,DWORD PTR [ebp+0x2c]
1003cac7:	33 45 78             	xor    eax,DWORD PTR [ebp+0x78]
1003caca:	f7 d1                	not    ecx
1003cacc:	23 4d 14             	and    ecx,DWORD PTR [ebp+0x14]
1003cacf:	89 45 58             	mov    DWORD PTR [ebp+0x58],eax
1003cad2:	0b ca                	or     ecx,edx
1003cad4:	8b 55 30             	mov    edx,DWORD PTR [ebp+0x30]
1003cad7:	33 4d 7c             	xor    ecx,DWORD PTR [ebp+0x7c]
1003cada:	f7 d2                	not    edx
1003cadc:	23 55 18             	and    edx,DWORD PTR [ebp+0x18]
1003cadf:	89 4d 5c             	mov    DWORD PTR [ebp+0x5c],ecx
1003cae2:	0b d7                	or     edx,edi
1003cae4:	8b 7d 74             	mov    edi,DWORD PTR [ebp+0x74]
1003cae7:	33 95 80 00 00 00    	xor    edx,DWORD PTR [ebp+0x80]
1003caed:	33 fe                	xor    edi,esi
1003caef:	8b 75 1c             	mov    esi,DWORD PTR [ebp+0x1c]
1003caf2:	8b 46 04             	mov    eax,DWORD PTR [esi+0x4]
1003caf5:	c1 e8 03             	shr    eax,0x3
1003caf8:	50                   	push   eax
1003caf9:	6a 00                	push   0x0
1003cafb:	ff 75 08             	push   DWORD PTR [ebp+0x8]
1003cafe:	89 7d 54             	mov    DWORD PTR [ebp+0x54],edi
1003cb01:	89 55 60             	mov    DWORD PTR [ebp+0x60],edx
1003cb04:	e8 01 1d 01 00       	call   0x1004e80a
1003cb09:	83 c4 0c             	add    esp,0xc
1003cb0c:	56                   	push   esi
1003cb0d:	83 ec 10             	sub    esp,0x10
1003cb10:	8b fc                	mov    edi,esp
1003cb12:	ff 75 08             	push   DWORD PTR [ebp+0x8]
1003cb15:	8d 75 54             	lea    esi,[ebp+0x54]
1003cb18:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003cb19:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003cb1a:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003cb1b:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003cb1c:	ff 55 20             	call   DWORD PTR [ebp+0x20]
1003cb1f:	5f                   	pop    edi
1003cb20:	5e                   	pop    esi
1003cb21:	5b                   	pop    ebx
1003cb22:	c9                   	leave
1003cb23:	c2 7c 00             	ret    0x7c
1003cb26:	55                   	push   ebp
1003cb27:	8b ec                	mov    ebp,esp
1003cb29:	83 ec 10             	sub    esp,0x10
1003cb2c:	d9 ee                	fldz
1003cb2e:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1003cb31:	8b 55 10             	mov    edx,DWORD PTR [ebp+0x10]
1003cb34:	d9 55 fc             	fst    DWORD PTR [ebp-0x4]
1003cb37:	56                   	push   esi
1003cb38:	d9 55 f8             	fst    DWORD PTR [ebp-0x8]
1003cb3b:	57                   	push   edi
1003cb3c:	d9 55 f4             	fst    DWORD PTR [ebp-0xc]
1003cb3f:	d9 5d f0             	fstp   DWORD PTR [ebp-0x10]
1003cb42:	8d 75 f0             	lea    esi,[ebp-0x10]
1003cb45:	8b f8                	mov    edi,eax
1003cb47:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003cb48:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003cb49:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003cb4a:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003cb4b:	33 f6                	xor    esi,esi
1003cb4d:	83 c2 1c             	add    edx,0x1c
1003cb50:	8b 3a                	mov    edi,DWORD PTR [edx]
1003cb52:	8b 4a 10             	mov    ecx,DWORD PTR [edx+0x10]
1003cb55:	d3 ef                	shr    edi,cl
1003cb57:	83 ff 01             	cmp    edi,0x1
1003cb5a:	75 10                	jne    0x1003cb6c
1003cb5c:	d9 45 0c             	fld    DWORD PTR [ebp+0xc]
1003cb5f:	d9 e8                	fld1
1003cb61:	de e1                	fsubrp st(1),st
1003cb63:	dc 25 98 80 09 10    	fsub   QWORD PTR ds:0x10098098
1003cb69:	d9 1c b0             	fstp   DWORD PTR [eax+esi*4]
1003cb6c:	46                   	inc    esi
1003cb6d:	83 c2 04             	add    edx,0x4
1003cb70:	83 fe 04             	cmp    esi,0x4
1003cb73:	72 db                	jb     0x1003cb50
1003cb75:	5f                   	pop    edi
1003cb76:	5e                   	pop    esi
1003cb77:	c9                   	leave
1003cb78:	c2 0c 00             	ret    0xc
