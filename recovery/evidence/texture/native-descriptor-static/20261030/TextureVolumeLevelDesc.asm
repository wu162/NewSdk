
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000d1ec <.text+0xc1ec>:
1000d1ec:	55                   	push   ebp
1000d1ed:	8b ec                	mov    ebp,esp
1000d1ef:	83 ec 0c             	sub    esp,0xc
1000d1f2:	56                   	push   esi
1000d1f3:	57                   	push   edi
1000d1f4:	8b 7d 08             	mov    edi,DWORD PTR [ebp+0x8]
1000d1f7:	8d 45 f8             	lea    eax,[ebp-0x8]
1000d1fa:	50                   	push   eax
1000d1fb:	8d 45 f4             	lea    eax,[ebp-0xc]
1000d1fe:	50                   	push   eax
1000d1ff:	8d 45 fc             	lea    eax,[ebp-0x4]
1000d202:	50                   	push   eax
1000d203:	8d 45 08             	lea    eax,[ebp+0x8]
1000d206:	50                   	push   eax
1000d207:	8d 45 0c             	lea    eax,[ebp+0xc]
1000d20a:	50                   	push   eax
1000d20b:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
1000d20e:	57                   	push   edi
1000d20f:	e8 73 fc ff ff       	call   0x1000ce87
1000d214:	8d 47 1c             	lea    eax,[edi+0x1c]
1000d217:	50                   	push   eax
1000d218:	e8 20 f6 ff ff       	call   0x1000c83d
1000d21d:	8b 75 10             	mov    esi,DWORD PTR [ebp+0x10]
1000d220:	57                   	push   edi
1000d221:	89 06                	mov    DWORD PTR [esi],eax
1000d223:	e8 60 f1 ff ff       	call   0x1000c388
1000d228:	83 66 08 00          	and    DWORD PTR [esi+0x8],0x0
1000d22c:	83 66 0c 00          	and    DWORD PTR [esi+0xc],0x0
1000d230:	89 46 04             	mov    DWORD PTR [esi+0x4],eax
1000d233:	8b 45 0c             	mov    eax,DWORD PTR [ebp+0xc]
1000d236:	89 46 10             	mov    DWORD PTR [esi+0x10],eax
1000d239:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1000d23c:	89 46 14             	mov    DWORD PTR [esi+0x14],eax
1000d23f:	8b 45 fc             	mov    eax,DWORD PTR [ebp-0x4]
1000d242:	5f                   	pop    edi
1000d243:	89 46 18             	mov    DWORD PTR [esi+0x18],eax
1000d246:	5e                   	pop    esi
1000d247:	c9                   	leave
1000d248:	c2 0c 00             	ret    0xc
