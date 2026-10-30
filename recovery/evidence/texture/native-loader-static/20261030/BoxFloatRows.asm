
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

100179af <.text+0x169af>:
100179af:	8b 46 04             	mov    eax,DWORD PTR [esi+0x4]
100179b2:	8b 40 68             	mov    eax,DWORD PTR [eax+0x68]
100179b5:	c1 e0 04             	shl    eax,0x4
100179b8:	50                   	push   eax
100179b9:	e8 b6 a6 ff ff       	call   0x10012074
100179be:	85 c0                	test   eax,eax
100179c0:	89 45 f0             	mov    DWORD PTR [ebp-0x10],eax
100179c3:	74 3d                	je     0x10017a02
100179c5:	8b 06                	mov    eax,DWORD PTR [esi]
100179c7:	83 78 6c 01          	cmp    DWORD PTR [eax+0x6c],0x1
100179cb:	8b 40 68             	mov    eax,DWORD PTR [eax+0x68]
100179ce:	75 10                	jne    0x100179e0
100179d0:	c1 e0 04             	shl    eax,0x4
100179d3:	50                   	push   eax
100179d4:	e8 9b a6 ff ff       	call   0x10012074
100179d9:	89 45 fc             	mov    DWORD PTR [ebp-0x4],eax
100179dc:	8b d8                	mov    ebx,eax
100179de:	eb 16                	jmp    0x100179f6
100179e0:	c1 e0 05             	shl    eax,0x5
100179e3:	50                   	push   eax
100179e4:	e8 8b a6 ff ff       	call   0x10012074
100179e9:	8b 0e                	mov    ecx,DWORD PTR [esi]
100179eb:	8b 59 68             	mov    ebx,DWORD PTR [ecx+0x68]
100179ee:	c1 e3 04             	shl    ebx,0x4
100179f1:	89 45 fc             	mov    DWORD PTR [ebp-0x4],eax
100179f4:	03 d8                	add    ebx,eax
100179f6:	85 c0                	test   eax,eax
100179f8:	75 12                	jne    0x10017a0c
100179fa:	ff 75 f0             	push   DWORD PTR [ebp-0x10]
100179fd:	e8 88 a6 ff ff       	call   0x1001208a
10017a02:	b8 0e 00 07 80       	mov    eax,0x8007000e
10017a07:	e9 79 01 00 00       	jmp    0x10017b85
10017a0c:	8b 0e                	mov    ecx,DWORD PTR [esi]
10017a0e:	83 79 68 01          	cmp    DWORD PTR [ecx+0x68],0x1
10017a12:	75 08                	jne    0x10017a1c
10017a14:	89 45 ec             	mov    DWORD PTR [ebp-0x14],eax
10017a17:	89 5d e8             	mov    DWORD PTR [ebp-0x18],ebx
10017a1a:	eb 0c                	jmp    0x10017a28
10017a1c:	8d 48 10             	lea    ecx,[eax+0x10]
10017a1f:	89 4d ec             	mov    DWORD PTR [ebp-0x14],ecx
10017a22:	8d 4b 10             	lea    ecx,[ebx+0x10]
10017a25:	89 4d e8             	mov    DWORD PTR [ebp-0x18],ecx
10017a28:	8b 4e 04             	mov    ecx,DWORD PTR [esi+0x4]
10017a2b:	83 65 f8 00          	and    DWORD PTR [ebp-0x8],0x0
10017a2f:	83 79 6c 00          	cmp    DWORD PTR [ecx+0x6c],0x0
10017a33:	0f 86 35 01 00 00    	jbe    0x10017b6e
10017a39:	ff 75 fc             	push   DWORD PTR [ebp-0x4]
10017a3c:	8b 7d f8             	mov    edi,DWORD PTR [ebp-0x8]
10017a3f:	8b 0e                	mov    ecx,DWORD PTR [esi]
10017a41:	8b 01                	mov    eax,DWORD PTR [ecx]
10017a43:	6a 00                	push   0x0
10017a45:	03 ff                	add    edi,edi
10017a47:	57                   	push   edi
10017a48:	ff 50 04             	call   DWORD PTR [eax+0x4]
10017a4b:	3b 5d fc             	cmp    ebx,DWORD PTR [ebp-0x4]
10017a4e:	74 0c                	je     0x10017a5c
10017a50:	8b 0e                	mov    ecx,DWORD PTR [esi]
10017a52:	8b 01                	mov    eax,DWORD PTR [ecx]
10017a54:	53                   	push   ebx
10017a55:	6a 00                	push   0x0
10017a57:	47                   	inc    edi
10017a58:	57                   	push   edi
10017a59:	ff 50 04             	call   DWORD PTR [eax+0x4]
10017a5c:	8b 46 04             	mov    eax,DWORD PTR [esi+0x4]
10017a5f:	83 65 f4 00          	and    DWORD PTR [ebp-0xc],0x0
10017a63:	83 78 68 00          	cmp    DWORD PTR [eax+0x68],0x0
10017a67:	0f 86 dc 00 00 00    	jbe    0x10017b49
10017a6d:	8b 7d f0             	mov    edi,DWORD PTR [ebp-0x10]
10017a70:	dd 05 88 80 09 10    	fld    QWORD PTR ds:0x10098088
10017a76:	83 c7 08             	add    edi,0x8
10017a79:	8b 45 f4             	mov    eax,DWORD PTR [ebp-0xc]
10017a7c:	8b 4d ec             	mov    ecx,DWORD PTR [ebp-0x14]
10017a7f:	8b 55 fc             	mov    edx,DWORD PTR [ebp-0x4]
10017a82:	03 c0                	add    eax,eax
10017a84:	c1 e0 04             	shl    eax,0x4
10017a87:	03 c8                	add    ecx,eax
10017a89:	d9 01                	fld    DWORD PTR [ecx]
10017a8b:	03 d0                	add    edx,eax
10017a8d:	d8 02                	fadd   DWORD PTR [edx]
10017a8f:	d9 5d d8             	fstp   DWORD PTR [ebp-0x28]
10017a92:	d9 41 04             	fld    DWORD PTR [ecx+0x4]
10017a95:	d8 42 04             	fadd   DWORD PTR [edx+0x4]
10017a98:	d9 5d dc             	fstp   DWORD PTR [ebp-0x24]
10017a9b:	d9 41 08             	fld    DWORD PTR [ecx+0x8]
10017a9e:	d8 42 08             	fadd   DWORD PTR [edx+0x8]
10017aa1:	d9 5d e0             	fstp   DWORD PTR [ebp-0x20]
10017aa4:	d9 41 0c             	fld    DWORD PTR [ecx+0xc]
10017aa7:	8d 0c 18             	lea    ecx,[eax+ebx*1]
10017aaa:	d8 42 0c             	fadd   DWORD PTR [edx+0xc]
10017aad:	d9 5d e4             	fstp   DWORD PTR [ebp-0x1c]
10017ab0:	d9 01                	fld    DWORD PTR [ecx]
10017ab2:	d8 45 d8             	fadd   DWORD PTR [ebp-0x28]
10017ab5:	d9 5d c8             	fstp   DWORD PTR [ebp-0x38]
10017ab8:	d9 41 04             	fld    DWORD PTR [ecx+0x4]
10017abb:	d8 45 dc             	fadd   DWORD PTR [ebp-0x24]
10017abe:	d9 5d cc             	fstp   DWORD PTR [ebp-0x34]
10017ac1:	d9 41 08             	fld    DWORD PTR [ecx+0x8]
10017ac4:	d8 45 e0             	fadd   DWORD PTR [ebp-0x20]
10017ac7:	d9 5d d0             	fstp   DWORD PTR [ebp-0x30]
10017aca:	d9 41 0c             	fld    DWORD PTR [ecx+0xc]
10017acd:	8b 4d e8             	mov    ecx,DWORD PTR [ebp-0x18]
10017ad0:	d8 45 e4             	fadd   DWORD PTR [ebp-0x1c]
10017ad3:	03 c1                	add    eax,ecx
10017ad5:	ff 45 f4             	inc    DWORD PTR [ebp-0xc]
10017ad8:	8b 4d f4             	mov    ecx,DWORD PTR [ebp-0xc]
10017adb:	d9 5d d4             	fstp   DWORD PTR [ebp-0x2c]
10017ade:	d9 00                	fld    DWORD PTR [eax]
10017ae0:	d8 45 c8             	fadd   DWORD PTR [ebp-0x38]
10017ae3:	d9 5d b8             	fstp   DWORD PTR [ebp-0x48]
10017ae6:	d9 40 04             	fld    DWORD PTR [eax+0x4]
10017ae9:	d8 45 cc             	fadd   DWORD PTR [ebp-0x34]
10017aec:	d9 5d bc             	fstp   DWORD PTR [ebp-0x44]
10017aef:	d9 40 08             	fld    DWORD PTR [eax+0x8]
10017af2:	d8 45 d0             	fadd   DWORD PTR [ebp-0x30]
10017af5:	d9 5d c0             	fstp   DWORD PTR [ebp-0x40]
10017af8:	d9 40 0c             	fld    DWORD PTR [eax+0xc]
10017afb:	d8 45 d4             	fadd   DWORD PTR [ebp-0x2c]
10017afe:	d9 5d c4             	fstp   DWORD PTR [ebp-0x3c]
10017b01:	d9 45 b8             	fld    DWORD PTR [ebp-0x48]
10017b04:	d8 c9                	fmul   st,st(1)
10017b06:	d9 5d a8             	fstp   DWORD PTR [ebp-0x58]
10017b09:	d9 45 bc             	fld    DWORD PTR [ebp-0x44]
10017b0c:	d8 c9                	fmul   st,st(1)
10017b0e:	d9 5d ac             	fstp   DWORD PTR [ebp-0x54]
10017b11:	d9 45 c0             	fld    DWORD PTR [ebp-0x40]
10017b14:	d8 c9                	fmul   st,st(1)
10017b16:	d9 5d b0             	fstp   DWORD PTR [ebp-0x50]
10017b19:	d9 45 c4             	fld    DWORD PTR [ebp-0x3c]
10017b1c:	d8 c9                	fmul   st,st(1)
10017b1e:	d9 5d b4             	fstp   DWORD PTR [ebp-0x4c]
10017b21:	d9 45 a8             	fld    DWORD PTR [ebp-0x58]
10017b24:	d9 5f f8             	fstp   DWORD PTR [edi-0x8]
10017b27:	d9 45 ac             	fld    DWORD PTR [ebp-0x54]
10017b2a:	d9 5f fc             	fstp   DWORD PTR [edi-0x4]
10017b2d:	d9 45 b0             	fld    DWORD PTR [ebp-0x50]
10017b30:	d9 1f                	fstp   DWORD PTR [edi]
10017b32:	83 c7 10             	add    edi,0x10
10017b35:	d9 45 b4             	fld    DWORD PTR [ebp-0x4c]
10017b38:	d9 5f f4             	fstp   DWORD PTR [edi-0xc]
10017b3b:	8b 46 04             	mov    eax,DWORD PTR [esi+0x4]
10017b3e:	3b 48 68             	cmp    ecx,DWORD PTR [eax+0x68]
10017b41:	0f 82 32 ff ff ff    	jb     0x10017a79
10017b47:	dd d8                	fstp   st(0)
10017b49:	ff 75 f0             	push   DWORD PTR [ebp-0x10]
10017b4c:	8b 4e 04             	mov    ecx,DWORD PTR [esi+0x4]
10017b4f:	8b 01                	mov    eax,DWORD PTR [ecx]
10017b51:	6a 00                	push   0x0
10017b53:	ff 75 f8             	push   DWORD PTR [ebp-0x8]
10017b56:	ff 50 08             	call   DWORD PTR [eax+0x8]
10017b59:	ff 45 f8             	inc    DWORD PTR [ebp-0x8]
10017b5c:	8b 46 04             	mov    eax,DWORD PTR [esi+0x4]
10017b5f:	8b 4d f8             	mov    ecx,DWORD PTR [ebp-0x8]
10017b62:	3b 48 6c             	cmp    ecx,DWORD PTR [eax+0x6c]
10017b65:	0f 82 ce fe ff ff    	jb     0x10017a39
10017b6b:	8b 45 fc             	mov    eax,DWORD PTR [ebp-0x4]
10017b6e:	50                   	push   eax
10017b6f:	e8 16 a5 ff ff       	call   0x1001208a
10017b74:	ff 75 f0             	push   DWORD PTR [ebp-0x10]
10017b77:	e8 0e a5 ff ff       	call   0x1001208a
10017b7c:	33 c0                	xor    eax,eax
10017b7e:	eb 05                	jmp    0x10017b85
10017b80:	b8 05 40 00 80       	mov    eax,0x80004005
10017b85:	5f                   	pop    edi
10017b86:	5b                   	pop    ebx
10017b87:	5e                   	pop    esi
10017b88:	c9                   	leave
10017b89:	c3                   	ret
