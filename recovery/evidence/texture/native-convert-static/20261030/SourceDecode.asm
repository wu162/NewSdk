
SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1003a99a <.text+0x3999a>:
1003a99a:	55                   	push   ebp
1003a99b:	8b ec                	mov    ebp,esp
1003a99d:	83 ec 20             	sub    esp,0x20
1003a9a0:	53                   	push   ebx
1003a9a1:	56                   	push   esi
1003a9a2:	57                   	push   edi
1003a9a3:	ff 75 10             	push   DWORD PTR [ebp+0x10]
1003a9a6:	8d 45 f0             	lea    eax,[ebp-0x10]
1003a9a9:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
1003a9ac:	50                   	push   eax
1003a9ad:	ff 55 14             	call   DWORD PTR [ebp+0x14]
1003a9b0:	8b f0                	mov    esi,eax
1003a9b2:	8b 45 58             	mov    eax,DWORD PTR [ebp+0x58]
1003a9b5:	8d 7d e0             	lea    edi,[ebp-0x20]
1003a9b8:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003a9b9:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003a9ba:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003a9bb:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003a9bc:	33 45 e0             	xor    eax,DWORD PTR [ebp-0x20]
1003a9bf:	8b 5d 08             	mov    ebx,DWORD PTR [ebp+0x8]
1003a9c2:	89 45 f0             	mov    DWORD PTR [ebp-0x10],eax
1003a9c5:	8b 45 5c             	mov    eax,DWORD PTR [ebp+0x5c]
1003a9c8:	33 45 e4             	xor    eax,DWORD PTR [ebp-0x1c]
1003a9cb:	8d 75 f0             	lea    esi,[ebp-0x10]
1003a9ce:	89 45 f4             	mov    DWORD PTR [ebp-0xc],eax
1003a9d1:	8b 45 60             	mov    eax,DWORD PTR [ebp+0x60]
1003a9d4:	33 45 e8             	xor    eax,DWORD PTR [ebp-0x18]
1003a9d7:	8b fb                	mov    edi,ebx
1003a9d9:	89 45 f8             	mov    DWORD PTR [ebp-0x8],eax
1003a9dc:	8b 45 64             	mov    eax,DWORD PTR [ebp+0x64]
1003a9df:	33 45 ec             	xor    eax,DWORD PTR [ebp-0x14]
1003a9e2:	83 ec 10             	sub    esp,0x10
1003a9e5:	89 45 fc             	mov    DWORD PTR [ebp-0x4],eax
1003a9e8:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003a9e9:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003a9ea:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003a9eb:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003a9ec:	8b fc                	mov    edi,esp
1003a9ee:	8d 75 f0             	lea    esi,[ebp-0x10]
1003a9f1:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003a9f2:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003a9f3:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003a9f4:	8d 45 58             	lea    eax,[ebp+0x58]
1003a9f7:	50                   	push   eax
1003a9f8:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003a9f9:	e8 21 f6 ff ff       	call   0x1003a01f
1003a9fe:	8b f0                	mov    esi,eax
1003aa00:	8d 7d f0             	lea    edi,[ebp-0x10]
1003aa03:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003aa04:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003aa05:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003aa06:	8b 45 18             	mov    eax,DWORD PTR [ebp+0x18]
1003aa09:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003aa0a:	8b 4d f0             	mov    ecx,DWORD PTR [ebp-0x10]
1003aa0d:	23 4d 18             	and    ecx,DWORD PTR [ebp+0x18]
1003aa10:	8b f3                	mov    esi,ebx
1003aa12:	8d 7d e0             	lea    edi,[ebp-0x20]
1003aa15:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003aa16:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003aa17:	f7 d0                	not    eax
1003aa19:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003aa1a:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003aa1b:	23 45 e0             	and    eax,DWORD PTR [ebp-0x20]
1003aa1e:	0b c1                	or     eax,ecx
1003aa20:	8b 4d f4             	mov    ecx,DWORD PTR [ebp-0xc]
1003aa23:	23 4d 1c             	and    ecx,DWORD PTR [ebp+0x1c]
1003aa26:	89 45 58             	mov    DWORD PTR [ebp+0x58],eax
1003aa29:	8b 45 1c             	mov    eax,DWORD PTR [ebp+0x1c]
1003aa2c:	f7 d0                	not    eax
1003aa2e:	23 45 e4             	and    eax,DWORD PTR [ebp-0x1c]
1003aa31:	0b c1                	or     eax,ecx
1003aa33:	8b 4d f8             	mov    ecx,DWORD PTR [ebp-0x8]
1003aa36:	23 4d 20             	and    ecx,DWORD PTR [ebp+0x20]
1003aa39:	89 45 5c             	mov    DWORD PTR [ebp+0x5c],eax
1003aa3c:	8b 45 20             	mov    eax,DWORD PTR [ebp+0x20]
1003aa3f:	f7 d0                	not    eax
1003aa41:	23 45 e8             	and    eax,DWORD PTR [ebp-0x18]
1003aa44:	0b c1                	or     eax,ecx
1003aa46:	8b 4d fc             	mov    ecx,DWORD PTR [ebp-0x4]
1003aa49:	89 45 60             	mov    DWORD PTR [ebp+0x60],eax
1003aa4c:	8b 45 24             	mov    eax,DWORD PTR [ebp+0x24]
1003aa4f:	f7 d0                	not    eax
1003aa51:	23 45 ec             	and    eax,DWORD PTR [ebp-0x14]
1003aa54:	23 4d 24             	and    ecx,DWORD PTR [ebp+0x24]
1003aa57:	d9 45 58             	fld    DWORD PTR [ebp+0x58]
1003aa5a:	0b c1                	or     eax,ecx
1003aa5c:	d8 4d 28             	fmul   DWORD PTR [ebp+0x28]
1003aa5f:	89 45 64             	mov    DWORD PTR [ebp+0x64],eax
1003aa62:	8d 75 18             	lea    esi,[ebp+0x18]
1003aa65:	8b fb                	mov    edi,ebx
1003aa67:	d8 45 38             	fadd   DWORD PTR [ebp+0x38]
1003aa6a:	d9 5d 18             	fstp   DWORD PTR [ebp+0x18]
1003aa6d:	d9 45 2c             	fld    DWORD PTR [ebp+0x2c]
1003aa70:	d8 4d 5c             	fmul   DWORD PTR [ebp+0x5c]
1003aa73:	d8 45 3c             	fadd   DWORD PTR [ebp+0x3c]
1003aa76:	d9 5d 1c             	fstp   DWORD PTR [ebp+0x1c]
1003aa79:	d9 45 60             	fld    DWORD PTR [ebp+0x60]
1003aa7c:	d8 4d 30             	fmul   DWORD PTR [ebp+0x30]
1003aa7f:	d8 45 40             	fadd   DWORD PTR [ebp+0x40]
1003aa82:	d9 5d 20             	fstp   DWORD PTR [ebp+0x20]
1003aa85:	d9 45 64             	fld    DWORD PTR [ebp+0x64]
1003aa88:	d8 4d 34             	fmul   DWORD PTR [ebp+0x34]
1003aa8b:	d8 45 44             	fadd   DWORD PTR [ebp+0x44]
1003aa8e:	d9 5d 24             	fstp   DWORD PTR [ebp+0x24]
1003aa91:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003aa92:	d9 45 18             	fld    DWORD PTR [ebp+0x18]
1003aa95:	d9 45 48             	fld    DWORD PTR [ebp+0x48]
1003aa98:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003aa99:	de d9                	fcompp
1003aa9b:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003aa9c:	df e0                	fnstsw ax
1003aa9e:	f6 c4 05             	test   ah,0x5
1003aaa1:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003aaa2:	7a 05                	jp     0x1003aaa9
1003aaa4:	d9 45 18             	fld    DWORD PTR [ebp+0x18]
1003aaa7:	eb 03                	jmp    0x1003aaac
1003aaa9:	d9 45 48             	fld    DWORD PTR [ebp+0x48]
1003aaac:	d9 5d 58             	fstp   DWORD PTR [ebp+0x58]
1003aaaf:	d9 45 1c             	fld    DWORD PTR [ebp+0x1c]
1003aab2:	d9 45 4c             	fld    DWORD PTR [ebp+0x4c]
1003aab5:	de d9                	fcompp
1003aab7:	df e0                	fnstsw ax
1003aab9:	f6 c4 05             	test   ah,0x5
1003aabc:	7a 05                	jp     0x1003aac3
1003aabe:	d9 45 1c             	fld    DWORD PTR [ebp+0x1c]
1003aac1:	eb 03                	jmp    0x1003aac6
1003aac3:	d9 45 4c             	fld    DWORD PTR [ebp+0x4c]
1003aac6:	d9 5d 5c             	fstp   DWORD PTR [ebp+0x5c]
1003aac9:	d9 45 20             	fld    DWORD PTR [ebp+0x20]
1003aacc:	d9 45 50             	fld    DWORD PTR [ebp+0x50]
1003aacf:	de d9                	fcompp
1003aad1:	df e0                	fnstsw ax
1003aad3:	f6 c4 05             	test   ah,0x5
1003aad6:	7a 05                	jp     0x1003aadd
1003aad8:	d9 45 20             	fld    DWORD PTR [ebp+0x20]
1003aadb:	eb 03                	jmp    0x1003aae0
1003aadd:	d9 45 50             	fld    DWORD PTR [ebp+0x50]
1003aae0:	d9 5d 60             	fstp   DWORD PTR [ebp+0x60]
1003aae3:	d9 45 24             	fld    DWORD PTR [ebp+0x24]
1003aae6:	d9 45 54             	fld    DWORD PTR [ebp+0x54]
1003aae9:	de d9                	fcompp
1003aaeb:	df e0                	fnstsw ax
1003aaed:	f6 c4 05             	test   ah,0x5
1003aaf0:	7a 05                	jp     0x1003aaf7
1003aaf2:	d9 45 24             	fld    DWORD PTR [ebp+0x24]
1003aaf5:	eb 03                	jmp    0x1003aafa
1003aaf7:	d9 45 54             	fld    DWORD PTR [ebp+0x54]
1003aafa:	d9 5d 64             	fstp   DWORD PTR [ebp+0x64]
1003aafd:	8b fb                	mov    edi,ebx
1003aaff:	8d 75 58             	lea    esi,[ebp+0x58]
1003ab02:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ab03:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ab04:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ab05:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ab06:	5f                   	pop    edi
1003ab07:	5e                   	pop    esi
1003ab08:	8b c3                	mov    eax,ebx
1003ab0a:	5b                   	pop    ebx
1003ab0b:	c9                   	leave
1003ab0c:	c2 60 00             	ret    0x60
1003ab0f:	55                   	push   ebp
1003ab10:	8b ec                	mov    ebp,esp
1003ab12:	83 ec 20             	sub    esp,0x20
1003ab15:	0f b7 45 0c          	movzx  eax,WORD PTR [ebp+0xc]
1003ab19:	56                   	push   esi
1003ab1a:	8b c8                	mov    ecx,eax
1003ab1c:	c1 e9 0b             	shr    ecx,0xb
1003ab1f:	89 4d f0             	mov    DWORD PTR [ebp-0x10],ecx
1003ab22:	57                   	push   edi
1003ab23:	8b c8                	mov    ecx,eax
1003ab25:	c1 e9 05             	shr    ecx,0x5
1003ab28:	83 ec 10             	sub    esp,0x10
1003ab2b:	83 e0 1f             	and    eax,0x1f
1003ab2e:	89 45 f8             	mov    DWORD PTR [ebp-0x8],eax
1003ab31:	83 e1 3f             	and    ecx,0x3f
1003ab34:	89 4d f4             	mov    DWORD PTR [ebp-0xc],ecx
1003ab37:	c7 45 fc 01 00 00 00 	mov    DWORD PTR [ebp-0x4],0x1
1003ab3e:	8b fc                	mov    edi,esp
1003ab40:	8d 75 f0             	lea    esi,[ebp-0x10]
1003ab43:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ab44:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ab45:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ab46:	8d 45 e0             	lea    eax,[ebp-0x20]
1003ab49:	50                   	push   eax
1003ab4a:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ab4b:	e8 cf f4 ff ff       	call   0x1003a01f
1003ab50:	8b f0                	mov    esi,eax
1003ab52:	8d 7d f0             	lea    edi,[ebp-0x10]
1003ab55:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ab56:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ab57:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ab58:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ab59:	d9 45 f0             	fld    DWORD PTR [ebp-0x10]
1003ab5c:	dd 05 88 9c 09 10    	fld    QWORD PTR ds:0x10099c88
1003ab62:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1003ab65:	dc c9                	fmul   st(1),st
1003ab67:	d9 c9                	fxch   st(1)
1003ab69:	8d 75 e0             	lea    esi,[ebp-0x20]
1003ab6c:	8b f8                	mov    edi,eax
1003ab6e:	d9 5d e0             	fstp   DWORD PTR [ebp-0x20]
1003ab71:	d9 45 f4             	fld    DWORD PTR [ebp-0xc]
1003ab74:	dc 0d 80 9c 09 10    	fmul   QWORD PTR ds:0x10099c80
1003ab7a:	d9 5d e4             	fstp   DWORD PTR [ebp-0x1c]
1003ab7d:	d8 4d f8             	fmul   DWORD PTR [ebp-0x8]
1003ab80:	d9 5d e8             	fstp   DWORD PTR [ebp-0x18]
1003ab83:	d9 45 fc             	fld    DWORD PTR [ebp-0x4]
1003ab86:	d9 5d ec             	fstp   DWORD PTR [ebp-0x14]
1003ab89:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ab8a:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ab8b:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ab8c:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003ab8d:	5f                   	pop    edi
1003ab8e:	5e                   	pop    esi
1003ab8f:	c9                   	leave
1003ab90:	c2 08 00             	ret    0x8
1003ab93:	55                   	push   ebp
1003ab94:	8b ec                	mov    ebp,esp
1003ab96:	83 ec 20             	sub    esp,0x20
1003ab99:	0f b7 4d 0c          	movzx  ecx,WORD PTR [ebp+0xc]
1003ab9d:	56                   	push   esi
1003ab9e:	57                   	push   edi
1003ab9f:	8b c1                	mov    eax,ecx
1003aba1:	0f b6 c9             	movzx  ecx,cl
1003aba4:	c1 e8 08             	shr    eax,0x8
1003aba7:	89 45 f0             	mov    DWORD PTR [ebp-0x10],eax
1003abaa:	89 45 f8             	mov    DWORD PTR [ebp-0x8],eax
1003abad:	83 ec 10             	sub    esp,0x10
1003abb0:	89 4d f4             	mov    DWORD PTR [ebp-0xc],ecx
1003abb3:	89 4d fc             	mov    DWORD PTR [ebp-0x4],ecx
1003abb6:	8b fc                	mov    edi,esp
1003abb8:	8d 75 f0             	lea    esi,[ebp-0x10]
1003abbb:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003abbc:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003abbd:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003abbe:	8d 45 e0             	lea    eax,[ebp-0x20]
1003abc1:	50                   	push   eax
1003abc2:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003abc3:	e8 57 f4 ff ff       	call   0x1003a01f
1003abc8:	8b f0                	mov    esi,eax
1003abca:	8d 7d f0             	lea    edi,[ebp-0x10]
1003abcd:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003abce:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003abcf:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003abd0:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003abd1:	d9 45 f0             	fld    DWORD PTR [ebp-0x10]
1003abd4:	dd 05 80 80 09 10    	fld    QWORD PTR ds:0x10098080
1003abda:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1003abdd:	dc c9                	fmul   st(1),st
1003abdf:	d9 c9                	fxch   st(1)
1003abe1:	8d 75 e0             	lea    esi,[ebp-0x20]
1003abe4:	8b f8                	mov    edi,eax
1003abe6:	d9 5d e0             	fstp   DWORD PTR [ebp-0x20]
1003abe9:	d9 45 f4             	fld    DWORD PTR [ebp-0xc]
1003abec:	d8 c9                	fmul   st,st(1)
1003abee:	d9 5d e4             	fstp   DWORD PTR [ebp-0x1c]
1003abf1:	d9 45 f8             	fld    DWORD PTR [ebp-0x8]
1003abf4:	d8 c9                	fmul   st,st(1)
1003abf6:	d9 5d e8             	fstp   DWORD PTR [ebp-0x18]
1003abf9:	d8 4d fc             	fmul   DWORD PTR [ebp-0x4]
1003abfc:	d9 5d ec             	fstp   DWORD PTR [ebp-0x14]
1003abff:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
