
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1001cfd0 <.text+0x1bfd0>:
1001cfd0:	55                   	push   ebp
1001cfd1:	8b ec                	mov    ebp,esp
1001cfd3:	83 ec 5c             	sub    esp,0x5c
1001cfd6:	53                   	push   ebx
1001cfd7:	56                   	push   esi
1001cfd8:	8b f1                	mov    esi,ecx
1001cfda:	83 7e 54 00          	cmp    DWORD PTR [esi+0x54],0x0
1001cfde:	57                   	push   edi
1001cfdf:	74 0b                	je     0x1001cfec
1001cfe1:	ff 75 10             	push   DWORD PTR [ebp+0x10]
1001cfe4:	e8 08 d4 ff ff       	call   0x1001a3f1
1001cfe9:	89 45 10             	mov    DWORD PTR [ebp+0x10],eax
1001cfec:	83 7e 10 00          	cmp    DWORD PTR [esi+0x10],0x0
1001cff0:	74 0d                	je     0x1001cfff
1001cff2:	ff 75 10             	push   DWORD PTR [ebp+0x10]
1001cff5:	8b ce                	mov    ecx,esi
1001cff7:	e8 14 df ff ff       	call   0x1001af10
1001cffc:	89 45 10             	mov    DWORD PTR [ebp+0x10],eax
1001cfff:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1001d002:	d9 ee                	fldz
1001d004:	8b 7e 68             	mov    edi,DWORD PTR [esi+0x68]
1001d007:	d9 55 b4             	fst    DWORD PTR [ebp-0x4c]
1001d00a:	8b 56 60             	mov    edx,DWORD PTR [esi+0x60]
1001d00d:	d9 55 b8             	fst    DWORD PTR [ebp-0x48]
1001d010:	8b 5e 64             	mov    ebx,DWORD PTR [esi+0x64]
1001d013:	0f af d0             	imul   edx,eax
1001d016:	d9 55 bc             	fst    DWORD PTR [ebp-0x44]
1001d019:	d9 5d c0             	fstp   DWORD PTR [ebp-0x40]
1001d01c:	89 45 08             	mov    DWORD PTR [ebp+0x8],eax
1001d01f:	83 65 08 01          	and    DWORD PTR [ebp+0x8],0x1
1001d023:	8d 4f ff             	lea    ecx,[edi-0x1]
1001d026:	0f af 4d 08          	imul   ecx,DWORD PTR [ebp+0x8]
1001d02a:	8d 0c 8a             	lea    ecx,[edx+ecx*4]
1001d02d:	8b 55 0c             	mov    edx,DWORD PTR [ebp+0xc]
1001d030:	0f af da             	imul   ebx,edx
1001d033:	03 cb                	add    ecx,ebx
1001d035:	03 4e 20             	add    ecx,DWORD PTR [esi+0x20]
1001d038:	83 e0 03             	and    eax,0x3
1001d03b:	83 e2 03             	and    edx,0x3
1001d03e:	83 7d 08 00          	cmp    DWORD PTR [ebp+0x8],0x0
1001d042:	89 4d e4             	mov    DWORD PTR [ebp-0x1c],ecx
1001d045:	8b 4e 34             	mov    ecx,DWORD PTR [esi+0x34]
1001d048:	8d 04 c2             	lea    eax,[edx+eax*8]
1001d04b:	8d 04 81             	lea    eax,[ecx+eax*4]
1001d04e:	89 45 dc             	mov    DWORD PTR [ebp-0x24],eax
1001d051:	75 07                	jne    0x1001d05a
1001d053:	33 db                	xor    ebx,ebx
1001d055:	33 ff                	xor    edi,edi
1001d057:	43                   	inc    ebx
1001d058:	eb 04                	jmp    0x1001d05e
1001d05a:	4f                   	dec    edi
1001d05b:	83 cb ff             	or     ebx,0xffffffff
1001d05e:	d9 7d f8             	fnstcw WORD PTR [ebp-0x8]
1001d061:	8b 45 f8             	mov    eax,DWORD PTR [ebp-0x8]
1001d064:	0d 00 0c 00 00       	or     eax,0xc00
1001d069:	89 45 e0             	mov    DWORD PTR [ebp-0x20],eax
1001d06c:	d9 6d e0             	fldcw  WORD PTR [ebp-0x20]
1001d06f:	8b 45 f8             	mov    eax,DWORD PTR [ebp-0x8]
1001d072:	89 46 7c             	mov    DWORD PTR [esi+0x7c],eax
1001d075:	83 7e 5c 00          	cmp    DWORD PTR [esi+0x5c],0x0
1001d079:	74 0a                	je     0x1001d085
1001d07b:	ff 75 10             	push   DWORD PTR [ebp+0x10]
1001d07e:	8b ce                	mov    ecx,esi
1001d080:	e8 7f e1 ff ff       	call   0x1001b204
1001d085:	83 65 08 00          	and    DWORD PTR [ebp+0x8],0x0
1001d089:	83 7e 68 00          	cmp    DWORD PTR [esi+0x68],0x0
1001d08d:	0f 86 fa 02 00 00    	jbe    0x1001d38d
1001d093:	8b 4d 10             	mov    ecx,DWORD PTR [ebp+0x10]
1001d096:	dd 05 00 80 09 10    	fld    QWORD PTR ds:0x10098000
1001d09c:	8b c3                	mov    eax,ebx
1001d09e:	c1 e0 02             	shl    eax,0x2
1001d0a1:	8b d7                	mov    edx,edi
1001d0a3:	89 45 d4             	mov    DWORD PTR [ebp-0x2c],eax
1001d0a6:	c1 e2 04             	shl    edx,0x4
1001d0a9:	8d 04 3b             	lea    eax,[ebx+edi*1]
1001d0ac:	89 5d 0c             	mov    DWORD PTR [ebp+0xc],ebx
1001d0af:	c1 65 0c 04          	shl    DWORD PTR [ebp+0xc],0x4
1001d0b3:	2b fb                	sub    edi,ebx
1001d0b5:	8d 4c 0a 08          	lea    ecx,[edx+ecx*1+0x8]
1001d0b9:	c1 e0 04             	shl    eax,0x4
1001d0bc:	c1 e7 04             	shl    edi,0x4
1001d0bf:	89 4d fc             	mov    DWORD PTR [ebp-0x4],ecx
1001d0c2:	8b 4d 08             	mov    ecx,DWORD PTR [ebp+0x8]
1001d0c5:	8b 5d dc             	mov    ebx,DWORD PTR [ebp-0x24]
1001d0c8:	83 e1 03             	and    ecx,0x3
1001d0cb:	d9 04 8b             	fld    DWORD PTR [ebx+ecx*4]
1001d0ce:	8b 4d fc             	mov    ecx,DWORD PTR [ebp-0x4]
1001d0d1:	d9 5d 10             	fstp   DWORD PTR [ebp+0x10]
1001d0d4:	d9 41 f8             	fld    DWORD PTR [ecx-0x8]
1001d0d7:	d8 45 b4             	fadd   DWORD PTR [ebp-0x4c]
1001d0da:	d8 c9                	fmul   st,st(1)
1001d0dc:	d9 5d a4             	fstp   DWORD PTR [ebp-0x5c]
1001d0df:	d9 41 fc             	fld    DWORD PTR [ecx-0x4]
1001d0e2:	d8 45 b8             	fadd   DWORD PTR [ebp-0x48]
1001d0e5:	d8 c9                	fmul   st,st(1)
1001d0e7:	d9 5d a8             	fstp   DWORD PTR [ebp-0x58]
1001d0ea:	d9 01                	fld    DWORD PTR [ecx]
1001d0ec:	d8 45 bc             	fadd   DWORD PTR [ebp-0x44]
1001d0ef:	d8 c9                	fmul   st,st(1)
1001d0f1:	d9 5d ac             	fstp   DWORD PTR [ebp-0x54]
1001d0f4:	d9 41 04             	fld    DWORD PTR [ecx+0x4]
1001d0f7:	d8 45 c0             	fadd   DWORD PTR [ebp-0x40]
1001d0fa:	de c9                	fmulp  st(1),st
1001d0fc:	d9 5d b0             	fstp   DWORD PTR [ebp-0x50]
1001d0ff:	d9 45 a4             	fld    DWORD PTR [ebp-0x5c]
1001d102:	d8 45 10             	fadd   DWORD PTR [ebp+0x10]
1001d105:	d9 5d d8             	fstp   DWORD PTR [ebp-0x28]
1001d108:	d9 45 d8             	fld    DWORD PTR [ebp-0x28]
1001d10b:	db 5d f4             	fistp  DWORD PTR [ebp-0xc]
1001d10e:	d9 45 a8             	fld    DWORD PTR [ebp-0x58]
1001d111:	8b 4d f4             	mov    ecx,DWORD PTR [ebp-0xc]
1001d114:	d8 45 10             	fadd   DWORD PTR [ebp+0x10]
1001d117:	89 4d c4             	mov    DWORD PTR [ebp-0x3c],ecx
1001d11a:	d9 5d d8             	fstp   DWORD PTR [ebp-0x28]
1001d11d:	d9 45 d8             	fld    DWORD PTR [ebp-0x28]
1001d120:	db 5d f0             	fistp  DWORD PTR [ebp-0x10]
1001d123:	d9 45 ac             	fld    DWORD PTR [ebp-0x54]
1001d126:	8b 4d f0             	mov    ecx,DWORD PTR [ebp-0x10]
1001d129:	d8 45 10             	fadd   DWORD PTR [ebp+0x10]
1001d12c:	89 4d c8             	mov    DWORD PTR [ebp-0x38],ecx
1001d12f:	d9 5d d8             	fstp   DWORD PTR [ebp-0x28]
1001d132:	d9 45 d8             	fld    DWORD PTR [ebp-0x28]
1001d135:	db 5d ec             	fistp  DWORD PTR [ebp-0x14]
1001d138:	d9 45 b0             	fld    DWORD PTR [ebp-0x50]
1001d13b:	8b 4d ec             	mov    ecx,DWORD PTR [ebp-0x14]
1001d13e:	d8 45 10             	fadd   DWORD PTR [ebp+0x10]
1001d141:	89 4d cc             	mov    DWORD PTR [ebp-0x34],ecx
1001d144:	d9 5d 10             	fstp   DWORD PTR [ebp+0x10]
1001d147:	d9 45 10             	fld    DWORD PTR [ebp+0x10]
1001d14a:	db 5d e8             	fistp  DWORD PTR [ebp-0x18]
1001d14d:	8b 4d e8             	mov    ecx,DWORD PTR [ebp-0x18]
1001d150:	83 7e 5c 00          	cmp    DWORD PTR [esi+0x5c],0x0
1001d154:	89 4d d0             	mov    DWORD PTR [ebp-0x30],ecx
1001d157:	0f 84 50 01 00 00    	je     0x1001d2ad
1001d15d:	d9 45 a4             	fld    DWORD PTR [ebp-0x5c]
1001d160:	8b 5e 5c             	mov    ebx,DWORD PTR [esi+0x5c]
1001d163:	da 65 c4             	fisub  DWORD PTR [ebp-0x3c]
1001d166:	8d 5c 1f 10          	lea    ebx,[edi+ebx*1+0x10]
1001d16a:	d9 5d 10             	fstp   DWORD PTR [ebp+0x10]
1001d16d:	d9 45 10             	fld    DWORD PTR [ebp+0x10]
1001d170:	dd 05 00 80 09 10    	fld    QWORD PTR ds:0x10098000
1001d176:	dc f9                	fdiv   st(1),st
1001d178:	d9 c9                	fxch   st(1)
1001d17a:	d9 5d b4             	fstp   DWORD PTR [ebp-0x4c]
1001d17d:	d9 45 b4             	fld    DWORD PTR [ebp-0x4c]
1001d180:	d9 c0                	fld    st(0)
1001d182:	dd 05 20 9c 09 10    	fld    QWORD PTR ds:0x10099c20
1001d188:	dc c9                	fmul   st(1),st
1001d18a:	d9 03                	fld    DWORD PTR [ebx]
1001d18c:	de c2                	faddp  st(2),st
1001d18e:	d9 c9                	fxch   st(1)
1001d190:	d9 1b                	fstp   DWORD PTR [ebx]
1001d192:	8b 5e 5c             	mov    ebx,DWORD PTR [esi+0x5c]
1001d195:	8d 5c 13 10          	lea    ebx,[ebx+edx*1+0x10]
1001d199:	d9 c1                	fld    st(1)
1001d19b:	dd 05 18 9c 09 10    	fld    QWORD PTR ds:0x10099c18
1001d1a1:	dc c9                	fmul   st(1),st
1001d1a3:	d9 03                	fld    DWORD PTR [ebx]
1001d1a5:	de c2                	faddp  st(2),st
1001d1a7:	d9 c9                	fxch   st(1)
1001d1a9:	d9 1b                	fstp   DWORD PTR [ebx]
1001d1ab:	8b 5e 5c             	mov    ebx,DWORD PTR [esi+0x5c]
1001d1ae:	8d 5c 18 10          	lea    ebx,[eax+ebx*1+0x10]
1001d1b2:	d9 c2                	fld    st(2)
1001d1b4:	dd 05 40 83 09 10    	fld    QWORD PTR ds:0x10098340
1001d1ba:	dc c9                	fmul   st(1),st
1001d1bc:	d9 03                	fld    DWORD PTR [ebx]
1001d1be:	de c2                	faddp  st(2),st
1001d1c0:	d9 c9                	fxch   st(1)
1001d1c2:	d9 1b                	fstp   DWORD PTR [ebx]
1001d1c4:	8b 5e 5c             	mov    ebx,DWORD PTR [esi+0x5c]
1001d1c7:	dd 05 10 9c 09 10    	fld    QWORD PTR ds:0x10099c10
1001d1cd:	8d 5c 1f 14          	lea    ebx,[edi+ebx*1+0x14]
1001d1d1:	dc cc                	fmul   st(4),st
1001d1d3:	d9 cc                	fxch   st(4)
1001d1d5:	d9 5d b4             	fstp   DWORD PTR [ebp-0x4c]
1001d1d8:	d9 45 a8             	fld    DWORD PTR [ebp-0x58]
1001d1db:	da 65 c8             	fisub  DWORD PTR [ebp-0x38]
1001d1de:	d9 5d 10             	fstp   DWORD PTR [ebp+0x10]
1001d1e1:	d9 45 10             	fld    DWORD PTR [ebp+0x10]
1001d1e4:	d8 f5                	fdiv   st,st(5)
1001d1e6:	d9 5d b8             	fstp   DWORD PTR [ebp-0x48]
1001d1e9:	d9 45 b8             	fld    DWORD PTR [ebp-0x48]
1001d1ec:	d9 c0                	fld    st(0)
1001d1ee:	d8 cc                	fmul   st,st(4)
1001d1f0:	d8 03                	fadd   DWORD PTR [ebx]
1001d1f2:	d9 1b                	fstp   DWORD PTR [ebx]
1001d1f4:	8b 5e 5c             	mov    ebx,DWORD PTR [esi+0x5c]
1001d1f7:	8d 5c 13 14          	lea    ebx,[ebx+edx*1+0x14]
1001d1fb:	d9 c0                	fld    st(0)
1001d1fd:	d8 cb                	fmul   st,st(3)
1001d1ff:	d8 03                	fadd   DWORD PTR [ebx]
1001d201:	d9 1b                	fstp   DWORD PTR [ebx]
1001d203:	8b 5e 5c             	mov    ebx,DWORD PTR [esi+0x5c]
1001d206:	8d 5c 18 14          	lea    ebx,[eax+ebx*1+0x14]
1001d20a:	d9 c0                	fld    st(0)
1001d20c:	d8 ca                	fmul   st,st(2)
1001d20e:	d8 03                	fadd   DWORD PTR [ebx]
1001d210:	d9 1b                	fstp   DWORD PTR [ebx]
1001d212:	8b 5e 5c             	mov    ebx,DWORD PTR [esi+0x5c]
1001d215:	8d 5c 1f 18          	lea    ebx,[edi+ebx*1+0x18]
1001d219:	d8 cc                	fmul   st,st(4)
1001d21b:	d9 5d b8             	fstp   DWORD PTR [ebp-0x48]
1001d21e:	d9 45 ac             	fld    DWORD PTR [ebp-0x54]
1001d221:	da 65 cc             	fisub  DWORD PTR [ebp-0x34]
1001d224:	d9 5d 10             	fstp   DWORD PTR [ebp+0x10]
1001d227:	d9 45 10             	fld    DWORD PTR [ebp+0x10]
1001d22a:	d8 f5                	fdiv   st,st(5)
1001d22c:	d9 5d bc             	fstp   DWORD PTR [ebp-0x44]
1001d22f:	d9 45 bc             	fld    DWORD PTR [ebp-0x44]
1001d232:	d9 c0                	fld    st(0)
1001d234:	d8 cc                	fmul   st,st(4)
1001d236:	d8 03                	fadd   DWORD PTR [ebx]
1001d238:	d9 1b                	fstp   DWORD PTR [ebx]
1001d23a:	8b 5e 5c             	mov    ebx,DWORD PTR [esi+0x5c]
1001d23d:	d9 c0                	fld    st(0)
1001d23f:	8d 5c 13 18          	lea    ebx,[ebx+edx*1+0x18]
1001d243:	d8 cb                	fmul   st,st(3)
1001d245:	d8 03                	fadd   DWORD PTR [ebx]
1001d247:	d9 1b                	fstp   DWORD PTR [ebx]
1001d249:	8b 5e 5c             	mov    ebx,DWORD PTR [esi+0x5c]
1001d24c:	8d 5c 18 18          	lea    ebx,[eax+ebx*1+0x18]
1001d250:	d9 c0                	fld    st(0)
1001d252:	d8 ca                	fmul   st,st(2)
1001d254:	d8 03                	fadd   DWORD PTR [ebx]
1001d256:	d9 1b                	fstp   DWORD PTR [ebx]
1001d258:	8b 5e 5c             	mov    ebx,DWORD PTR [esi+0x5c]
1001d25b:	8d 5c 1f 1c          	lea    ebx,[edi+ebx*1+0x1c]
1001d25f:	d8 cc                	fmul   st,st(4)
1001d261:	d9 5d bc             	fstp   DWORD PTR [ebp-0x44]
1001d264:	d9 45 b0             	fld    DWORD PTR [ebp-0x50]
1001d267:	da 65 d0             	fisub  DWORD PTR [ebp-0x30]
1001d26a:	d9 5d 10             	fstp   DWORD PTR [ebp+0x10]
1001d26d:	d9 45 10             	fld    DWORD PTR [ebp+0x10]
1001d270:	d8 f5                	fdiv   st,st(5)
1001d272:	d9 5d c0             	fstp   DWORD PTR [ebp-0x40]
1001d275:	d9 45 c0             	fld    DWORD PTR [ebp-0x40]
1001d278:	d9 c0                	fld    st(0)
1001d27a:	de cc                	fmulp  st(4),st
1001d27c:	d9 03                	fld    DWORD PTR [ebx]
1001d27e:	de c4                	faddp  st(4),st
1001d280:	d9 cb                	fxch   st(3)
1001d282:	d9 1b                	fstp   DWORD PTR [ebx]
1001d284:	8b 5e 5c             	mov    ebx,DWORD PTR [esi+0x5c]
1001d287:	8d 5c 13 1c          	lea    ebx,[ebx+edx*1+0x1c]
1001d28b:	d9 c2                	fld    st(2)
1001d28d:	de ca                	fmulp  st(2),st
1001d28f:	d9 03                	fld    DWORD PTR [ebx]
1001d291:	de c2                	faddp  st(2),st
1001d293:	d9 c9                	fxch   st(1)
1001d295:	d9 1b                	fstp   DWORD PTR [ebx]
1001d297:	8b 5e 5c             	mov    ebx,DWORD PTR [esi+0x5c]
1001d29a:	d9 c1                	fld    st(1)
1001d29c:	8d 5c 18 1c          	lea    ebx,[eax+ebx*1+0x1c]
1001d2a0:	de c9                	fmulp  st(1),st
1001d2a2:	d8 03                	fadd   DWORD PTR [ebx]
1001d2a4:	d9 1b                	fstp   DWORD PTR [ebx]
1001d2a6:	de c9                	fmulp  st(1),st
1001d2a8:	d9 5d c0             	fstp   DWORD PTR [ebp-0x40]
1001d2ab:	eb 06                	jmp    0x1001d2b3
1001d2ad:	dd 05 00 80 09 10    	fld    QWORD PTR ds:0x10098000
1001d2b3:	8b 5d c4             	mov    ebx,DWORD PTR [ebp-0x3c]
1001d2b6:	81 fb ff 00 00 00    	cmp    ebx,0xff
1001d2bc:	7c 05                	jl     0x1001d2c3
1001d2be:	bb ff 00 00 00       	mov    ebx,0xff
1001d2c3:	85 db                	test   ebx,ebx
1001d2c5:	7e 12                	jle    0x1001d2d9
1001d2c7:	81 7d c4 ff 00 00 00 	cmp    DWORD PTR [ebp-0x3c],0xff
1001d2ce:	7c 0d                	jl     0x1001d2dd
1001d2d0:	c7 45 c4 ff 00 00 00 	mov    DWORD PTR [ebp-0x3c],0xff
1001d2d7:	eb 04                	jmp    0x1001d2dd
1001d2d9:	83 65 c4 00          	and    DWORD PTR [ebp-0x3c],0x0
1001d2dd:	8b 5d c8             	mov    ebx,DWORD PTR [ebp-0x38]
1001d2e0:	81 fb ff 00 00 00    	cmp    ebx,0xff
1001d2e6:	7c 05                	jl     0x1001d2ed
1001d2e8:	bb ff 00 00 00       	mov    ebx,0xff
1001d2ed:	85 db                	test   ebx,ebx
1001d2ef:	7e 12                	jle    0x1001d303
1001d2f1:	81 7d c8 ff 00 00 00 	cmp    DWORD PTR [ebp-0x38],0xff
1001d2f8:	7c 0d                	jl     0x1001d307
1001d2fa:	c7 45 c8 ff 00 00 00 	mov    DWORD PTR [ebp-0x38],0xff
1001d301:	eb 04                	jmp    0x1001d307
1001d303:	83 65 c8 00          	and    DWORD PTR [ebp-0x38],0x0
1001d307:	8b 5d cc             	mov    ebx,DWORD PTR [ebp-0x34]
1001d30a:	81 fb ff 00 00 00    	cmp    ebx,0xff
1001d310:	7c 05                	jl     0x1001d317
1001d312:	bb ff 00 00 00       	mov    ebx,0xff
1001d317:	85 db                	test   ebx,ebx
1001d319:	7e 12                	jle    0x1001d32d
1001d31b:	81 7d cc ff 00 00 00 	cmp    DWORD PTR [ebp-0x34],0xff
1001d322:	7c 0d                	jl     0x1001d331
1001d324:	c7 45 cc ff 00 00 00 	mov    DWORD PTR [ebp-0x34],0xff
1001d32b:	eb 04                	jmp    0x1001d331
1001d32d:	83 65 cc 00          	and    DWORD PTR [ebp-0x34],0x0
1001d331:	81 f9 ff 00 00 00    	cmp    ecx,0xff
1001d337:	8b d9                	mov    ebx,ecx
1001d339:	7c 05                	jl     0x1001d340
1001d33b:	bb ff 00 00 00       	mov    ebx,0xff
1001d340:	85 db                	test   ebx,ebx
1001d342:	7e 0d                	jle    0x1001d351
1001d344:	bb ff 00 00 00       	mov    ebx,0xff
1001d349:	3b cb                	cmp    ecx,ebx
1001d34b:	7c 06                	jl     0x1001d353
1001d34d:	8b cb                	mov    ecx,ebx
1001d34f:	eb 02                	jmp    0x1001d353
1001d351:	33 c9                	xor    ecx,ecx
1001d353:	8b 5d e4             	mov    ebx,DWORD PTR [ebp-0x1c]
1001d356:	c1 e1 08             	shl    ecx,0x8
1001d359:	0b 4d cc             	or     ecx,DWORD PTR [ebp-0x34]
1001d35c:	c1 e1 08             	shl    ecx,0x8
1001d35f:	0b 4d c8             	or     ecx,DWORD PTR [ebp-0x38]
1001d362:	c1 e1 08             	shl    ecx,0x8
1001d365:	0b 4d c4             	or     ecx,DWORD PTR [ebp-0x3c]
1001d368:	89 0b                	mov    DWORD PTR [ebx],ecx
1001d36a:	8b 4d 0c             	mov    ecx,DWORD PTR [ebp+0xc]
1001d36d:	03 5d d4             	add    ebx,DWORD PTR [ebp-0x2c]
1001d370:	01 4d fc             	add    DWORD PTR [ebp-0x4],ecx
1001d373:	03 d1                	add    edx,ecx
1001d375:	03 f9                	add    edi,ecx
1001d377:	03 c1                	add    eax,ecx
1001d379:	ff 45 08             	inc    DWORD PTR [ebp+0x8]
1001d37c:	8b 4d 08             	mov    ecx,DWORD PTR [ebp+0x8]
1001d37f:	3b 4e 68             	cmp    ecx,DWORD PTR [esi+0x68]
1001d382:	89 5d e4             	mov    DWORD PTR [ebp-0x1c],ebx
1001d385:	0f 82 37 fd ff ff    	jb     0x1001d0c2
1001d38b:	dd d8                	fstp   st(0)
1001d38d:	8b 46 7c             	mov    eax,DWORD PTR [esi+0x7c]
1001d390:	89 45 10             	mov    DWORD PTR [ebp+0x10],eax
1001d393:	d9 6d 10             	fldcw  WORD PTR [ebp+0x10]
1001d396:	5f                   	pop    edi
1001d397:	5e                   	pop    esi
1001d398:	5b                   	pop    ebx
1001d399:	c9                   	leave
1001d39a:	c2 0c 00             	ret    0xc
