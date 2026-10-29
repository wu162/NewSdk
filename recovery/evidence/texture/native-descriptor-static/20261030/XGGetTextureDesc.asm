
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1003dc51 <.text+0x3cc51>:
1003dc51:	55                   	push   ebp
1003dc52:	8b ec                	mov    ebp,esp
1003dc54:	83 ec 58             	sub    esp,0x58
1003dc57:	83 65 f8 00          	and    DWORD PTR [ebp-0x8],0x0
1003dc5b:	53                   	push   ebx
1003dc5c:	8b 5d 08             	mov    ebx,DWORD PTR [ebp+0x8]
1003dc5f:	57                   	push   edi
1003dc60:	8b f8                	mov    edi,eax
1003dc62:	57                   	push   edi
1003dc63:	89 7d fc             	mov    DWORD PTR [ebp-0x4],edi
1003dc66:	e8 1d e7 fc ff       	call   0x1000c388
1003dc6b:	89 06                	mov    DWORD PTR [esi],eax
1003dc6d:	83 e8 03             	sub    eax,0x3
1003dc70:	0f 84 46 01 00 00    	je     0x1003ddbc
1003dc76:	48                   	dec    eax
1003dc77:	0f 84 be 00 00 00    	je     0x1003dd3b
1003dc7d:	83 e8 0c             	sub    eax,0xc
1003dc80:	0f 84 8d 00 00 00    	je     0x1003dd13
1003dc86:	48                   	dec    eax
1003dc87:	74 5e                	je     0x1003dce7
1003dc89:	48                   	dec    eax
1003dc8a:	74 36                	je     0x1003dcc2
1003dc8c:	48                   	dec    eax
1003dc8d:	74 23                	je     0x1003dcb2
1003dc8f:	48                   	dec    eax
1003dc90:	0f 85 98 02 00 00    	jne    0x1003df2e
1003dc96:	8d 45 a8             	lea    eax,[ebp-0x58]
1003dc99:	50                   	push   eax
1003dc9a:	53                   	push   ebx
1003dc9b:	57                   	push   edi
1003dc9c:	e8 bf fe fc ff       	call   0x1000db60
1003dca1:	8b 45 c0             	mov    eax,DWORD PTR [ebp-0x40]
1003dca4:	89 46 04             	mov    DWORD PTR [esi+0x4],eax
1003dca7:	33 c0                	xor    eax,eax
1003dca9:	40                   	inc    eax
1003dcaa:	89 46 0c             	mov    DWORD PTR [esi+0xc],eax
1003dcad:	e9 25 01 00 00       	jmp    0x1003ddd7
1003dcb2:	8d 45 a8             	lea    eax,[ebp-0x58]
1003dcb5:	50                   	push   eax
1003dcb6:	53                   	push   ebx
1003dcb7:	57                   	push   edi
1003dcb8:	e8 a3 fe fc ff       	call   0x1000db60
1003dcbd:	e9 05 01 00 00       	jmp    0x1003ddc7
1003dcc2:	8d 45 a8             	lea    eax,[ebp-0x58]
1003dcc5:	50                   	push   eax
1003dcc6:	53                   	push   ebx
1003dcc7:	57                   	push   edi
1003dcc8:	e8 93 fe fc ff       	call   0x1000db60
1003dccd:	8b 45 c0             	mov    eax,DWORD PTR [ebp-0x40]
1003dcd0:	89 46 04             	mov    DWORD PTR [esi+0x4],eax
1003dcd3:	8b 45 c4             	mov    eax,DWORD PTR [ebp-0x3c]
1003dcd6:	89 46 08             	mov    DWORD PTR [esi+0x8],eax
1003dcd9:	8b 47 24             	mov    eax,DWORD PTR [edi+0x24]
1003dcdc:	c1 e8 1a             	shr    eax,0x1a
1003dcdf:	89 46 0c             	mov    DWORD PTR [esi+0xc],eax
1003dce2:	e9 f3 00 00 00       	jmp    0x1003ddda
1003dce7:	8d 45 c8             	lea    eax,[ebp-0x38]
1003dcea:	50                   	push   eax
1003dceb:	53                   	push   ebx
1003dcec:	57                   	push   edi
1003dced:	e8 07 ff fc ff       	call   0x1000dbf9
1003dcf2:	8b 45 d8             	mov    eax,DWORD PTR [ebp-0x28]
1003dcf5:	83 66 38 00          	and    DWORD PTR [esi+0x38],0x0
1003dcf9:	89 46 04             	mov    DWORD PTR [esi+0x4],eax
1003dcfc:	8b 45 dc             	mov    eax,DWORD PTR [ebp-0x24]
1003dcff:	89 46 08             	mov    DWORD PTR [esi+0x8],eax
1003dd02:	8b 45 e0             	mov    eax,DWORD PTR [ebp-0x20]
1003dd05:	89 46 0c             	mov    DWORD PTR [esi+0xc],eax
1003dd08:	8b 45 c8             	mov    eax,DWORD PTR [ebp-0x38]
1003dd0b:	89 46 10             	mov    DWORD PTR [esi+0x10],eax
1003dd0e:	e9 d3 00 00 00       	jmp    0x1003dde6
1003dd13:	8d 45 c8             	lea    eax,[ebp-0x38]
1003dd16:	50                   	push   eax
1003dd17:	57                   	push   edi
1003dd18:	e8 b8 00 fd ff       	call   0x1000ddd5
1003dd1d:	8b 45 d8             	mov    eax,DWORD PTR [ebp-0x28]
1003dd20:	83 66 38 00          	and    DWORD PTR [esi+0x38],0x0
1003dd24:	89 46 04             	mov    DWORD PTR [esi+0x4],eax
1003dd27:	8b 45 dc             	mov    eax,DWORD PTR [ebp-0x24]
1003dd2a:	89 46 08             	mov    DWORD PTR [esi+0x8],eax
1003dd2d:	8b 45 e0             	mov    eax,DWORD PTR [ebp-0x20]
1003dd30:	89 46 0c             	mov    DWORD PTR [esi+0xc],eax
1003dd33:	8b 45 c8             	mov    eax,DWORD PTR [ebp-0x38]
1003dd36:	89 46 10             	mov    DWORD PTR [esi+0x10],eax
1003dd39:	eb 30                	jmp    0x1003dd6b
1003dd3b:	8d 45 a8             	lea    eax,[ebp-0x58]
1003dd3e:	50                   	push   eax
1003dd3f:	57                   	push   edi
1003dd40:	e8 fc ff fc ff       	call   0x1000dd41
1003dd45:	8b 45 c4             	mov    eax,DWORD PTR [ebp-0x3c]
1003dd48:	8b 55 c0             	mov    edx,DWORD PTR [ebp-0x40]
1003dd4b:	8b 5d b8             	mov    ebx,DWORD PTR [ebp-0x48]
1003dd4e:	33 c9                	xor    ecx,ecx
1003dd50:	89 46 08             	mov    DWORD PTR [esi+0x8],eax
1003dd53:	8b 45 a8             	mov    eax,DWORD PTR [ebp-0x58]
1003dd56:	41                   	inc    ecx
1003dd57:	89 56 04             	mov    DWORD PTR [esi+0x4],edx
1003dd5a:	89 4e 0c             	mov    DWORD PTR [esi+0xc],ecx
1003dd5d:	89 46 10             	mov    DWORD PTR [esi+0x10],eax
1003dd60:	89 5e 38             	mov    DWORD PTR [esi+0x38],ebx
1003dd63:	f7 07 00 00 00 40    	test   DWORD PTR [edi],0x40000000
1003dd69:	74 0e                	je     0x1003dd79
1003dd6b:	8b 47 18             	mov    eax,DWORD PTR [edi+0x18]
1003dd6e:	8b 5f 1c             	mov    ebx,DWORD PTR [edi+0x1c]
1003dd71:	89 45 fc             	mov    DWORD PTR [ebp-0x4],eax
1003dd74:	83 e3 0f             	and    ebx,0xf
1003dd77:	eb 6d                	jmp    0x1003dde6
1003dd79:	83 e0 3f             	and    eax,0x3f
1003dd7c:	0f b6 80 48 77 09 10 	movzx  eax,BYTE PTR [eax+0x10097748]
1003dd83:	89 46 1c             	mov    DWORD PTR [esi+0x1c],eax
1003dd86:	8b 45 c4             	mov    eax,DWORD PTR [ebp-0x3c]
1003dd89:	89 46 24             	mov    DWORD PTR [esi+0x24],eax
1003dd8c:	8b 46 1c             	mov    eax,DWORD PTR [esi+0x1c]
1003dd8f:	c1 e8 03             	shr    eax,0x3
1003dd92:	89 56 20             	mov    DWORD PTR [esi+0x20],edx
1003dd95:	89 4e 28             	mov    DWORD PTR [esi+0x28],ecx
1003dd98:	89 46 2c             	mov    DWORD PTR [esi+0x2c],eax
1003dd9b:	8b 57 18             	mov    edx,DWORD PTR [edi+0x18]
1003dd9e:	81 e2 ff 3f 00 00    	and    edx,0x3fff
1003dda4:	0f af d0             	imul   edx,eax
1003dda7:	89 56 14             	mov    DWORD PTR [esi+0x14],edx
1003ddaa:	8b 47 2c             	mov    eax,DWORD PTR [edi+0x2c]
1003ddad:	83 66 30 00          	and    DWORD PTR [esi+0x30],0x0
1003ddb1:	89 46 18             	mov    DWORD PTR [esi+0x18],eax
1003ddb4:	89 4e 34             	mov    DWORD PTR [esi+0x34],ecx
1003ddb7:	e9 72 01 00 00       	jmp    0x1003df2e
1003ddbc:	8d 45 a8             	lea    eax,[ebp-0x58]
1003ddbf:	50                   	push   eax
1003ddc0:	53                   	push   ebx
1003ddc1:	57                   	push   edi
1003ddc2:	e8 99 fd fc ff       	call   0x1000db60
1003ddc7:	8b 45 c0             	mov    eax,DWORD PTR [ebp-0x40]
1003ddca:	89 46 04             	mov    DWORD PTR [esi+0x4],eax
1003ddcd:	8b 45 c4             	mov    eax,DWORD PTR [ebp-0x3c]
1003ddd0:	c7 46 0c 01 00 00 00 	mov    DWORD PTR [esi+0xc],0x1
1003ddd7:	89 46 08             	mov    DWORD PTR [esi+0x8],eax
1003ddda:	8b 45 a8             	mov    eax,DWORD PTR [ebp-0x58]
1003dddd:	89 46 10             	mov    DWORD PTR [esi+0x10],eax
1003dde0:	8b 45 b8             	mov    eax,DWORD PTR [ebp-0x48]
1003dde3:	89 46 38             	mov    DWORD PTR [esi+0x38],eax
1003dde6:	8b 46 10             	mov    eax,DWORD PTR [esi+0x10]
1003dde9:	83 e0 3f             	and    eax,0x3f
1003ddec:	0f b6 88 48 77 09 10 	movzx  ecx,BYTE PTR [eax+0x10097748]
1003ddf3:	83 66 34 00          	and    DWORD PTR [esi+0x34],0x0
1003ddf7:	89 4e 1c             	mov    DWORD PTR [esi+0x1c],ecx
1003ddfa:	8d 4d f4             	lea    ecx,[ebp-0xc]
1003ddfd:	51                   	push   ecx
1003ddfe:	8d 4d e8             	lea    ecx,[ebp-0x18]
1003de01:	51                   	push   ecx
1003de02:	50                   	push   eax
1003de03:	e8 bc e9 fc ff       	call   0x1000c7c4
1003de08:	8b 7d fc             	mov    edi,DWORD PTR [ebp-0x4]
1003de0b:	53                   	push   ebx
1003de0c:	8d 45 f8             	lea    eax,[ebp-0x8]
1003de0f:	50                   	push   eax
1003de10:	8d 45 fc             	lea    eax,[ebp-0x4]
1003de13:	50                   	push   eax
1003de14:	8d 45 f0             	lea    eax,[ebp-0x10]
1003de17:	50                   	push   eax
1003de18:	8d 45 ec             	lea    eax,[ebp-0x14]
1003de1b:	50                   	push   eax
1003de1c:	8b c7                	mov    eax,edi
1003de1e:	e8 64 fc ff ff       	call   0x1003da87
1003de23:	85 db                	test   ebx,ebx
1003de25:	8b 5d f4             	mov    ebx,DWORD PTR [ebp-0xc]
1003de28:	75 52                	jne    0x1003de7c
1003de2a:	8b 4f 30             	mov    ecx,DWORD PTR [edi+0x30]
1003de2d:	8b c1                	mov    eax,ecx
1003de2f:	25 00 06 00 00       	and    eax,0x600
1003de34:	3d 00 02 00 00       	cmp    eax,0x200
1003de39:	75 41                	jne    0x1003de7c
1003de3b:	b8 00 00 00 80       	mov    eax,0x80000000
1003de40:	85 47 1c             	test   DWORD PTR [edi+0x1c],eax
1003de43:	75 37                	jne    0x1003de7c
1003de45:	66 f7 47 20 00 04    	test   WORD PTR [edi+0x20],0x400
1003de4b:	75 2f                	jne    0x1003de7c
1003de4d:	f7 c1 00 f8 ff ff    	test   ecx,0xfffff800
1003de53:	75 27                	jne    0x1003de7c
1003de55:	85 47 28             	test   DWORD PTR [edi+0x28],eax
1003de58:	75 22                	jne    0x1003de7c
1003de5a:	8d 45 f0             	lea    eax,[ebp-0x10]
1003de5d:	50                   	push   eax
1003de5e:	8d 45 f4             	lea    eax,[ebp-0xc]
1003de61:	50                   	push   eax
1003de62:	8d 45 e4             	lea    eax,[ebp-0x1c]
1003de65:	50                   	push   eax
1003de66:	57                   	push   edi
1003de67:	e8 bc e7 fc ff       	call   0x1000c628
1003de6c:	8b 45 f4             	mov    eax,DWORD PTR [ebp-0xc]
1003de6f:	8d 4c 18 ff          	lea    ecx,[eax+ebx*1-0x1]
1003de73:	8d 43 ff             	lea    eax,[ebx-0x1]
1003de76:	f7 d0                	not    eax
1003de78:	23 c8                	and    ecx,eax
1003de7a:	eb 03                	jmp    0x1003de7f
1003de7c:	8b 4d f0             	mov    ecx,DWORD PTR [ebp-0x10]
1003de7f:	8b 46 1c             	mov    eax,DWORD PTR [esi+0x1c]
1003de82:	0f af 45 ec          	imul   eax,DWORD PTR [ebp-0x14]
1003de86:	c1 e8 03             	shr    eax,0x3
1003de89:	89 46 14             	mov    DWORD PTR [esi+0x14],eax
1003de8c:	0f af c1             	imul   eax,ecx
1003de8f:	83 7d fc 00          	cmp    DWORD PTR [ebp-0x4],0x0
1003de93:	89 46 18             	mov    DWORD PTR [esi+0x18],eax
1003de96:	75 11                	jne    0x1003dea9
1003de98:	83 7d 08 01          	cmp    DWORD PTR [ebp+0x8],0x1
1003de9c:	77 0b                	ja     0x1003dea9
1003de9e:	57                   	push   edi
1003de9f:	e8 e4 e4 fc ff       	call   0x1000c388
1003dea4:	83 f8 03             	cmp    eax,0x3
1003dea7:	74 1b                	je     0x1003dec4
1003dea9:	57                   	push   edi
1003deaa:	e8 d9 e4 fc ff       	call   0x1000c388
1003deaf:	83 f8 11             	cmp    eax,0x11
1003deb2:	74 10                	je     0x1003dec4
1003deb4:	8b 46 18             	mov    eax,DWORD PTR [esi+0x18]
1003deb7:	05 ff 0f 00 00       	add    eax,0xfff
1003debc:	25 00 f0 ff ff       	and    eax,0xfffff000
1003dec1:	89 46 18             	mov    DWORD PTR [esi+0x18],eax
1003dec4:	8b 47 28             	mov    eax,DWORD PTR [edi+0x28]
1003dec7:	c1 e0 0d             	shl    eax,0xd
1003deca:	c1 f8 1a             	sar    eax,0x1a
1003decd:	83 7d fc 00          	cmp    DWORD PTR [ebp-0x4],0x0
1003ded1:	89 46 30             	mov    DWORD PTR [esi+0x30],eax
1003ded4:	74 04                	je     0x1003deda
1003ded6:	83 4e 34 01          	or     DWORD PTR [esi+0x34],0x1
1003deda:	83 7d f8 00          	cmp    DWORD PTR [ebp-0x8],0x0
1003dede:	74 04                	je     0x1003dee4
1003dee0:	83 4e 34 02          	or     DWORD PTR [esi+0x34],0x2
1003dee4:	f7 47 28 00 00 00 80 	test   DWORD PTR [edi+0x28],0x80000000
1003deeb:	74 04                	je     0x1003def1
1003deed:	83 4e 34 08          	or     DWORD PTR [esi+0x34],0x8
1003def1:	8b 46 14             	mov    eax,DWORD PTR [esi+0x14]
1003def4:	8b 4d e8             	mov    ecx,DWORD PTR [ebp-0x18]
1003def7:	0f af c3             	imul   eax,ebx
1003defa:	89 46 14             	mov    DWORD PTR [esi+0x14],eax
1003defd:	8b 46 04             	mov    eax,DWORD PTR [esi+0x4]
1003df00:	8d 44 08 ff          	lea    eax,[eax+ecx*1-0x1]
1003df04:	33 d2                	xor    edx,edx
1003df06:	f7 f1                	div    ecx
1003df08:	33 d2                	xor    edx,edx
1003df0a:	89 46 20             	mov    DWORD PTR [esi+0x20],eax
1003df0d:	8b 46 08             	mov    eax,DWORD PTR [esi+0x8]
1003df10:	8d 44 03 ff          	lea    eax,[ebx+eax*1-0x1]
1003df14:	f7 f3                	div    ebx
1003df16:	89 46 24             	mov    DWORD PTR [esi+0x24],eax
1003df19:	8b 46 0c             	mov    eax,DWORD PTR [esi+0xc]
1003df1c:	89 46 28             	mov    DWORD PTR [esi+0x28],eax
1003df1f:	8b 46 1c             	mov    eax,DWORD PTR [esi+0x1c]
1003df22:	0f af c1             	imul   eax,ecx
1003df25:	0f af c3             	imul   eax,ebx
1003df28:	c1 e8 03             	shr    eax,0x3
1003df2b:	89 46 2c             	mov    DWORD PTR [esi+0x2c],eax
1003df2e:	5f                   	pop    edi
1003df2f:	5b                   	pop    ebx
1003df30:	c9                   	leave
1003df31:	c2 04 00             	ret    0x4
