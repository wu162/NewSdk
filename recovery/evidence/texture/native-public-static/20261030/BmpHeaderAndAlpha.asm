
SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

10013ee8 <.text+0x12ee8>:
10013ee8:	56                   	push   esi
10013ee9:	8b f1                	mov    esi,ecx
10013eeb:	8b 46 04             	mov    eax,DWORD PTR [esi+0x4]
10013eee:	85 c0                	test   eax,eax
10013ef0:	74 0c                	je     0x10013efe
10013ef2:	83 7e 38 00          	cmp    DWORD PTR [esi+0x38],0x0
10013ef6:	74 06                	je     0x10013efe
10013ef8:	50                   	push   eax
10013ef9:	e8 8c e1 ff ff       	call   0x1001208a
10013efe:	8b 46 08             	mov    eax,DWORD PTR [esi+0x8]
10013f01:	85 c0                	test   eax,eax
10013f03:	74 0c                	je     0x10013f11
10013f05:	83 7e 3c 00          	cmp    DWORD PTR [esi+0x3c],0x0
10013f09:	74 06                	je     0x10013f11
10013f0b:	50                   	push   eax
10013f0c:	e8 79 e1 ff ff       	call   0x1001208a
10013f11:	57                   	push   edi
10013f12:	8b 7e 4c             	mov    edi,DWORD PTR [esi+0x4c]
10013f15:	85 ff                	test   edi,edi
10013f17:	74 0d                	je     0x10013f26
10013f19:	8b cf                	mov    ecx,edi
10013f1b:	e8 c8 ff ff ff       	call   0x10013ee8
10013f20:	57                   	push   edi
10013f21:	e8 64 e1 ff ff       	call   0x1001208a
10013f26:	8b 76 50             	mov    esi,DWORD PTR [esi+0x50]
10013f29:	85 f6                	test   esi,esi
10013f2b:	5f                   	pop    edi
10013f2c:	74 0d                	je     0x10013f3b
10013f2e:	8b ce                	mov    ecx,esi
10013f30:	e8 b3 ff ff ff       	call   0x10013ee8
10013f35:	56                   	push   esi
10013f36:	e8 4f e1 ff ff       	call   0x1001208a
10013f3b:	5e                   	pop    esi
10013f3c:	c3                   	ret
10013f3d:	55                   	push   ebp
10013f3e:	8d 6c 24 90          	lea    ebp,[esp-0x70]
10013f42:	81 ec c4 00 00 00    	sub    esp,0xc4
10013f48:	83 7d 7c 04          	cmp    DWORD PTR [ebp+0x7c],0x4
10013f4c:	89 4d 3c             	mov    DWORD PTR [ebp+0x3c],ecx
10013f4f:	73 0a                	jae    0x10013f5b
10013f51:	b8 05 40 00 80       	mov    eax,0x80004005
10013f56:	e9 4d 0a 00 00       	jmp    0x100149a8
10013f5b:	53                   	push   ebx
10013f5c:	56                   	push   esi
10013f5d:	8b 75 78             	mov    esi,DWORD PTR [ebp+0x78]
10013f60:	57                   	push   edi
10013f61:	6a 0a                	push   0xa
10013f63:	8d 5d bc             	lea    ebx,[ebp-0x44]
10013f66:	59                   	pop    ecx
10013f67:	8b c3                	mov    eax,ebx
10013f69:	50                   	push   eax
10013f6a:	8b fb                	mov    edi,ebx
10013f6c:	50                   	push   eax
10013f6d:	f3 a5                	rep movs DWORD PTR es:[edi],DWORD PTR ds:[esi]
10013f6f:	e8 25 ff ff ff       	call   0x10013e99
10013f74:	8b 45 bc             	mov    eax,DWORD PTR [ebp-0x44]
10013f77:	39 45 7c             	cmp    DWORD PTR [ebp+0x7c],eax
10013f7a:	0f 82 20 0a 00 00    	jb     0x100149a0
10013f80:	6a 0c                	push   0xc
10013f82:	5e                   	pop    esi
10013f83:	33 c9                	xor    ecx,ecx
10013f85:	3b c6                	cmp    eax,esi
10013f87:	75 38                	jne    0x10013fc1
10013f89:	0f bf 45 c0          	movsx  eax,WORD PTR [ebp-0x40]
10013f8d:	89 45 e8             	mov    DWORD PTR [ebp-0x18],eax
10013f90:	0f bf 45 c2          	movsx  eax,WORD PTR [ebp-0x3e]
10013f94:	89 45 ec             	mov    DWORD PTR [ebp-0x14],eax
10013f97:	66 8b 45 c4          	mov    ax,WORD PTR [ebp-0x3c]
10013f9b:	66 89 45 f0          	mov    WORD PTR [ebp-0x10],ax
10013f9f:	66 8b 45 c6          	mov    ax,WORD PTR [ebp-0x3a]
10013fa3:	89 75 e4             	mov    DWORD PTR [ebp-0x1c],esi
10013fa6:	66 89 45 f2          	mov    WORD PTR [ebp-0xe],ax
10013faa:	89 4d f4             	mov    DWORD PTR [ebp-0xc],ecx
10013fad:	89 4d f8             	mov    DWORD PTR [ebp-0x8],ecx
10013fb0:	89 4d fc             	mov    DWORD PTR [ebp-0x4],ecx
10013fb3:	89 4d 00             	mov    DWORD PTR [ebp+0x0],ecx
10013fb6:	89 4d 04             	mov    DWORD PTR [ebp+0x4],ecx
10013fb9:	89 4d 08             	mov    DWORD PTR [ebp+0x8],ecx
10013fbc:	8d 5d e4             	lea    ebx,[ebp-0x1c]
10013fbf:	eb 09                	jmp    0x10013fca
10013fc1:	83 f8 28             	cmp    eax,0x28
10013fc4:	0f 82 d6 09 00 00    	jb     0x100149a0
10013fca:	8b 43 04             	mov    eax,DWORD PTR [ebx+0x4]
10013fcd:	89 45 60             	mov    DWORD PTR [ebp+0x60],eax
10013fd0:	8b 43 08             	mov    eax,DWORD PTR [ebx+0x8]
10013fd3:	3b c1                	cmp    eax,ecx
10013fd5:	89 45 2c             	mov    DWORD PTR [ebp+0x2c],eax
10013fd8:	7f 02                	jg     0x10013fdc
10013fda:	f7 d8                	neg    eax
10013fdc:	8b 3b                	mov    edi,DWORD PTR [ebx]
10013fde:	33 d2                	xor    edx,edx
10013fe0:	3b fe                	cmp    edi,esi
10013fe2:	0f 95 c2             	setne  dl
10013fe5:	89 45 5c             	mov    DWORD PTR [ebp+0x5c],eax
10013fe8:	8b 43 20             	mov    eax,DWORD PTR [ebx+0x20]
10013feb:	83 c2 03             	add    edx,0x3
10013fee:	8b f2                	mov    esi,edx
10013ff0:	0f b7 53 0e          	movzx  edx,WORD PTR [ebx+0xe]
10013ff4:	66 83 fa 08          	cmp    dx,0x8
10013ff8:	89 55 48             	mov    DWORD PTR [ebp+0x48],edx
10013ffb:	77 0b                	ja     0x10014008
10013ffd:	3b c1                	cmp    eax,ecx
10013fff:	75 07                	jne    0x10014008
10014001:	33 c0                	xor    eax,eax
10014003:	40                   	inc    eax
10014004:	8a ca                	mov    cl,dl
10014006:	d3 e0                	shl    eax,cl
10014008:	0f af f0             	imul   esi,eax
1001400b:	03 f7                	add    esi,edi
1001400d:	3b 75 7c             	cmp    esi,DWORD PTR [ebp+0x7c]
10014010:	89 75 64             	mov    DWORD PTR [ebp+0x64],esi
10014013:	0f 87 87 09 00 00    	ja     0x100149a0
10014019:	66 83 7b 0c 01       	cmp    WORD PTR [ebx+0xc],0x1
1001401e:	0f 85 7c 09 00 00    	jne    0x100149a0
10014024:	8b 4b 10             	mov    ecx,DWORD PTR [ebx+0x10]
10014027:	83 f9 02             	cmp    ecx,0x2
1001402a:	89 4d 44             	mov    DWORD PTR [ebp+0x44],ecx
1001402d:	b8 ff 00 00 00       	mov    eax,0xff
10014032:	0f 86 e6 02 00 00    	jbe    0x1001431e
10014038:	83 f9 03             	cmp    ecx,0x3
1001403b:	0f 85 5f 09 00 00    	jne    0x100149a0
10014041:	83 ff 34             	cmp    edi,0x34
10014044:	0f 82 56 09 00 00    	jb     0x100149a0
1001404a:	83 ff 38             	cmp    edi,0x38
1001404d:	8b 4b 30             	mov    ecx,DWORD PTR [ebx+0x30]
10014050:	8b 53 2c             	mov    edx,DWORD PTR [ebx+0x2c]
10014053:	8b 73 28             	mov    esi,DWORD PTR [ebx+0x28]
10014056:	89 4d 34             	mov    DWORD PTR [ebp+0x34],ecx
10014059:	89 55 30             	mov    DWORD PTR [ebp+0x30],edx
1001405c:	89 75 54             	mov    DWORD PTR [ebp+0x54],esi
1001405f:	72 08                	jb     0x10014069
10014061:	8b 73 34             	mov    esi,DWORD PTR [ebx+0x34]
10014064:	89 75 68             	mov    DWORD PTR [ebp+0x68],esi
10014067:	eb 04                	jmp    0x1001406d
10014069:	83 65 68 00          	and    DWORD PTR [ebp+0x68],0x0
1001406d:	0f b7 75 48          	movzx  esi,WORD PTR [ebp+0x48]
10014071:	83 fe 10             	cmp    esi,0x10
10014074:	c7 45 6c a6 aa 22 1a 	mov    DWORD PTR [ebp+0x6c],0x1a22aaa6
1001407b:	89 75 40             	mov    DWORD PTR [ebp+0x40],esi
1001407e:	0f 84 a8 01 00 00    	je     0x1001422c
10014084:	83 fe 18             	cmp    esi,0x18
10014087:	0f 84 6f 01 00 00    	je     0x100141fc
1001408d:	83 fe 20             	cmp    esi,0x20
10014090:	0f 85 0a 09 00 00    	jne    0x100149a0
10014096:	3b c8                	cmp    ecx,eax
10014098:	75 3c                	jne    0x100140d6
1001409a:	81 fa 00 ff 00 00    	cmp    edx,0xff00
100140a0:	0f 85 a2 02 00 00    	jne    0x10014348
100140a6:	81 7d 54 00 00 ff 00 	cmp    DWORD PTR [ebp+0x54],0xff0000
100140ad:	0f 85 95 02 00 00    	jne    0x10014348
100140b3:	83 7d 68 00          	cmp    DWORD PTR [ebp+0x68],0x0
100140b7:	0f 84 7b 02 00 00    	je     0x10014338
100140bd:	81 7d 68 00 00 00 ff 	cmp    DWORD PTR [ebp+0x68],0xff000000
100140c4:	0f 85 7e 02 00 00    	jne    0x10014348
100140ca:	c7 45 6c 86 00 28 18 	mov    DWORD PTR [ebp+0x6c],0x18280086
100140d1:	e9 72 02 00 00       	jmp    0x10014348
100140d6:	bf 00 00 f0 3f       	mov    edi,0x3ff00000
100140db:	3b cf                	cmp    ecx,edi
100140dd:	75 32                	jne    0x10014111
100140df:	81 fa 00 fc 0f 00    	cmp    edx,0xffc00
100140e5:	0f 85 5d 02 00 00    	jne    0x10014348
100140eb:	81 7d 54 ff 03 00 00 	cmp    DWORD PTR [ebp+0x54],0x3ff
100140f2:	0f 85 50 02 00 00    	jne    0x10014348
100140f8:	81 7d 68 00 00 00 c0 	cmp    DWORD PTR [ebp+0x68],0xc0000000
100140ff:	0f 85 43 02 00 00    	jne    0x10014348
10014105:	c7 45 6c b6 00 20 1a 	mov    DWORD PTR [ebp+0x6c],0x1a2000b6
1001410c:	e9 37 02 00 00       	jmp    0x10014348
10014111:	be 00 00 ff 00       	mov    esi,0xff0000
10014116:	3b ce                	cmp    ecx,esi
10014118:	75 40                	jne    0x1001415a
1001411a:	81 fa 00 ff 00 00    	cmp    edx,0xff00
10014120:	0f 85 22 02 00 00    	jne    0x10014348
10014126:	39 45 54             	cmp    DWORD PTR [ebp+0x54],eax
10014129:	0f 85 19 02 00 00    	jne    0x10014348
1001412f:	81 7d 68 00 00 00 ff 	cmp    DWORD PTR [ebp+0x68],0xff000000
10014136:	75 0c                	jne    0x10014144
10014138:	c7 45 6c 86 00 20 1a 	mov    DWORD PTR [ebp+0x6c],0x1a200086
1001413f:	e9 04 02 00 00       	jmp    0x10014348
10014144:	83 7d 68 00          	cmp    DWORD PTR [ebp+0x68],0x0
10014148:	0f 85 fa 01 00 00    	jne    0x10014348
1001414e:	c7 45 6c 86 00 20 2a 	mov    DWORD PTR [ebp+0x6c],0x2a200086
10014155:	e9 ee 01 00 00       	jmp    0x10014348
1001415a:	85 c9                	test   ecx,ecx
1001415c:	75 2e                	jne    0x1001418c
1001415e:	81 fa 00 00 ff ff    	cmp    edx,0xffff0000
10014164:	0f 85 de 01 00 00    	jne    0x10014348
1001416a:	81 7d 54 ff ff 00 00 	cmp    DWORD PTR [ebp+0x54],0xffff
10014171:	0f 85 d1 01 00 00    	jne    0x10014348
10014177:	39 4d 68             	cmp    DWORD PTR [ebp+0x68],ecx
1001417a:	0f 85 c8 01 00 00    	jne    0x10014348
10014180:	c7 45 6c 99 00 20 2d 	mov    DWORD PTR [ebp+0x6c],0x2d200099
10014187:	e9 bc 01 00 00       	jmp    0x10014348
1001418c:	81 f9 ff 03 00 00    	cmp    ecx,0x3ff
10014192:	75 2e                	jne    0x100141c2
10014194:	81 fa 00 fc 0f 00    	cmp    edx,0xffc00
1001419a:	0f 85 a8 01 00 00    	jne    0x10014348
100141a0:	39 7d 54             	cmp    DWORD PTR [ebp+0x54],edi
100141a3:	0f 85 9f 01 00 00    	jne    0x10014348
100141a9:	81 7d 68 00 00 00 c0 	cmp    DWORD PTR [ebp+0x68],0xc0000000
100141b0:	0f 85 92 01 00 00    	jne    0x10014348
100141b6:	c7 45 6c b6 00 28 18 	mov    DWORD PTR [ebp+0x6c],0x182800b6
100141bd:	e9 86 01 00 00       	jmp    0x10014348
100141c2:	81 f9 00 ff 00 00    	cmp    ecx,0xff00
100141c8:	0f 85 7a 01 00 00    	jne    0x10014348
100141ce:	3b d6                	cmp    edx,esi
100141d0:	0f 85 72 01 00 00    	jne    0x10014348
100141d6:	81 7d 54 00 00 00 ff 	cmp    DWORD PTR [ebp+0x54],0xff000000
100141dd:	0f 85 65 01 00 00    	jne    0x10014348
100141e3:	83 7d 68 00          	cmp    DWORD PTR [ebp+0x68],0x0
100141e7:	0f 85 5b 01 00 00    	jne    0x10014348
100141ed:	ff 45 64             	inc    DWORD PTR [ebp+0x64]
100141f0:	c7 45 6c 86 00 28 28 	mov    DWORD PTR [ebp+0x6c],0x28280086
100141f7:	e9 4c 01 00 00       	jmp    0x10014348
100141fc:	3b c8                	cmp    ecx,eax
100141fe:	0f 85 44 01 00 00    	jne    0x10014348
10014204:	81 fa 00 ff 00 00    	cmp    edx,0xff00
1001420a:	0f 85 38 01 00 00    	jne    0x10014348
10014210:	81 7d 54 00 00 ff 00 	cmp    DWORD PTR [ebp+0x54],0xff0000
10014217:	0f 85 2b 01 00 00    	jne    0x10014348
1001421d:	83 7d 68 00          	cmp    DWORD PTR [ebp+0x68],0x0
10014221:	0f 85 21 01 00 00    	jne    0x10014348
10014227:	e9 0c 01 00 00       	jmp    0x10014338
1001422c:	3b c8                	cmp    ecx,eax
1001422e:	75 2a                	jne    0x1001425a
10014230:	3b d0                	cmp    edx,eax
10014232:	0f 85 10 01 00 00    	jne    0x10014348
10014238:	39 45 54             	cmp    DWORD PTR [ebp+0x54],eax
1001423b:	0f 85 07 01 00 00    	jne    0x10014348
10014241:	81 7d 68 00 ff 00 00 	cmp    DWORD PTR [ebp+0x68],0xff00
10014248:	0f 85 fa 00 00 00    	jne    0x10014348
1001424e:	c7 45 6c 4a 00 00 08 	mov    DWORD PTR [ebp+0x6c],0x800004a
10014255:	e9 ee 00 00 00       	jmp    0x10014348
1001425a:	83 f9 1f             	cmp    ecx,0x1f
1001425d:	75 67                	jne    0x100142c6
1001425f:	81 fa e0 07 00 00    	cmp    edx,0x7e0
10014265:	75 23                	jne    0x1001428a
10014267:	81 7d 54 00 f8 00 00 	cmp    DWORD PTR [ebp+0x54],0xf800
1001426e:	0f 85 d4 00 00 00    	jne    0x10014348
10014274:	83 7d 68 00          	cmp    DWORD PTR [ebp+0x68],0x0
10014278:	0f 85 ca 00 00 00    	jne    0x10014348
1001427e:	c7 45 6c 44 00 28 28 	mov    DWORD PTR [ebp+0x6c],0x28280044
10014285:	e9 be 00 00 00       	jmp    0x10014348
1001428a:	81 fa e0 03 00 00    	cmp    edx,0x3e0
10014290:	0f 85 b2 00 00 00    	jne    0x10014348
10014296:	81 7d 54 00 7c 00 00 	cmp    DWORD PTR [ebp+0x54],0x7c00
1001429d:	0f 85 a5 00 00 00    	jne    0x10014348
100142a3:	83 7d 68 00          	cmp    DWORD PTR [ebp+0x68],0x0
100142a7:	0f 84 94 00 00 00    	je     0x10014341
100142ad:	81 7d 68 00 80 00 00 	cmp    DWORD PTR [ebp+0x68],0x8000
100142b4:	0f 85 8e 00 00 00    	jne    0x10014348
100142ba:	c7 45 6c 43 00 28 18 	mov    DWORD PTR [ebp+0x6c],0x18280043
100142c1:	e9 82 00 00 00       	jmp    0x10014348
100142c6:	83 f9 0f             	cmp    ecx,0xf
100142c9:	75 32                	jne    0x100142fd
100142cb:	81 fa f0 00 00 00    	cmp    edx,0xf0
100142d1:	75 75                	jne    0x10014348
100142d3:	81 7d 54 00 0f 00 00 	cmp    DWORD PTR [ebp+0x54],0xf00
100142da:	75 6c                	jne    0x10014348
100142dc:	81 7d 68 00 f0 00 00 	cmp    DWORD PTR [ebp+0x68],0xf000
100142e3:	75 09                	jne    0x100142ee
100142e5:	c7 45 6c 4f 00 28 18 	mov    DWORD PTR [ebp+0x6c],0x1828004f
100142ec:	eb 5a                	jmp    0x10014348
100142ee:	83 7d 68 00          	cmp    DWORD PTR [ebp+0x68],0x0
100142f2:	75 54                	jne    0x10014348
100142f4:	c7 45 6c 4f 00 28 28 	mov    DWORD PTR [ebp+0x6c],0x2828004f
100142fb:	eb 4b                	jmp    0x10014348
100142fd:	b8 ff ff 00 00       	mov    eax,0xffff
10014302:	3b c8                	cmp    ecx,eax
10014304:	75 42                	jne    0x10014348
10014306:	3b d0                	cmp    edx,eax
10014308:	75 3e                	jne    0x10014348
1001430a:	39 45 54             	cmp    DWORD PTR [ebp+0x54],eax
1001430d:	75 39                	jne    0x10014348
1001430f:	83 7d 68 00          	cmp    DWORD PTR [ebp+0x68],0x0
10014313:	75 33                	jne    0x10014348
10014315:	c7 45 6c 58 00 00 28 	mov    DWORD PTR [ebp+0x6c],0x28000058
1001431c:	eb 2a                	jmp    0x10014348
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
1001444b:	c7                   	.byte 0xc7
1001444c:	46                   	inc    esi
1001444d:	14 01                	adc    al,0x1
	...
