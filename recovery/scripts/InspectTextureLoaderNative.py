#!/usr/bin/env python3
"""Read/disassemble fixed original DLL functions; never execute DLL or codec."""
import argparse,hashlib,json,struct,subprocess
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
EXPECTED='4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
FUNCTIONS=[
 ('FileEntry',0x13c82,0x13ca7),
 ('FileReadWrapper',0x13b53,0x13bb8),
 ('Memory2DWrapper',0x13a9b,0x13ad8),
 ('TextureFromMemory',0x134b8,0x13a9b),
 ('FileRead',0x16bff,0x16d2a),
 ('FileClose',0x16d2a,0x16d62),
 ('CpuTextureCreate',0xdc78,0xdd41),
 ('CpuTextureHeader',0xd7ea,0xdabf),
 ('TextureAllocation',0xc9f6,0xccde),
 ('StorageAlign',0xc91d,0xc9f6),
 ('DefaultMipFilter',0x13173,0x134b8),
 ('Box2DDispatch',0x17825,0x179af),
 ('BoxFloatRows',0x179af,0x17b8a),
 ('Rgba8RowStore',0x1cfd0,0x1d391)]
p=argparse.ArgumentParser(description=__doc__);p.add_argument('--root',type=Path,default=ROOT);p.add_argument('--out',type=Path,default=ROOT/'work/native-loader/evidence');args=p.parse_args()
source=args.root/'SDK/tools/BinaryAssetBuilder.TextureCompiler.dll';raw=source.read_bytes();assert hashlib.sha256(raw).hexdigest()==EXPECTED
pe=struct.unpack_from('<I',raw,0x3c)[0];assert raw[pe:pe+4]==b'PE\0\0';sections=struct.unpack_from('<H',raw,pe+6)[0];sizeopt=struct.unpack_from('<H',raw,pe+20)[0];optional=pe+24;assert struct.unpack_from('<H',raw,optional)[0]==0x10b
base=struct.unpack_from('<I',raw,optional+28)[0];table=optional+sizeopt;section_rows=[]
for i in range(sections):
 position=table+i*40;vsize,va,rsize,rstart=struct.unpack_from('<IIII',raw,position+8);section_rows.append((va,vsize,rstart,rsize))
def file_offset(rva):
 for va,vsize,rstart,rsize in section_rows:
  if va<=rva<va+max(vsize,rsize):
   assert rva-va<rsize;return rstart+rva-va
 raise AssertionError('RVA has no file section')
args.out.mkdir(parents=True,exist_ok=True);manifest={'scope':'read-only fixed-function byte/disassembly extraction; not native execution or production acceptance','originalPath':source.relative_to(args.root).as_posix(),'originalSha256':EXPECTED,'imageBase':hex(base),'functions':[],'disassembler':subprocess.run(['objdump','--version'],capture_output=True,text=True,check=True).stdout.splitlines()[0]}
for name,start,end in FUNCTIONS:
 offset=file_offset(start);count=end-start;assert file_offset(end-1)==offset+count-1;body=raw[offset:offset+count];(args.out/(name+'.bin')).write_bytes(body)
 result=subprocess.run(['objdump','-d','-Mintel','--start-address='+hex(base+start),'--stop-address='+hex(base+end),str(source)],capture_output=True,text=True,check=True)
 (args.out/(name+'.asm')).write_text(result.stdout)
 manifest['functions'].append({'name':name,'rvaStart':hex(start),'rvaEndExclusive':hex(end),'vaStart':hex(base+start),'vaEndExclusive':hex(base+end),'fileOffset':hex(offset),'bytes':len(body),'sha256':hashlib.sha256(body).hexdigest(),'boundary':'fixed entry range to the next known function entry; body ends in return or tail jump','disassembly':name+'.asm','rawBytes':name+'.bin'})
manifest['scopeBoundary']='PC non-rendering file IO/default ordinary2D CPU texture creation and packed mip storage; read-only original evidence, no execution or production acceptance'
manifest['criticalSites']=[
 {'rva':'0x13c82','evidence':'default2D file call: dimensions/levels/format/filter default -1, usage0, pool1, output last'},
 {'rva':'0x16c9e','evidence':'CreateFileA GENERIC_READ FILE_SHARE_READ OPEN_EXISTING FILE_FLAG_RANDOM_ACCESS; file/read failure88760b59'},
 {'rva':'0xdc7e','evidence':'GlobalAlloc GMEM_ZEROINIT52-byte object; VirtualAlloc MEM_COMMIT/PAGE_READWRITE primary and optional secondary; exact cleanup on OOM'},
 {'rva':'0xd855','evidence':'packing2 becomes levels>1; original header stores stride in units32texels and dimensions-minus1'},
 {'rva':'0xc9f6','evidence':'ordinary2D oneface, power-of-two packed-tail storage; stop mip allocations when either exponent<=4; every allocation page rounded'},
 {'rva':'0x13268','evidence':'POT default mip filter5 BOX; optimized byte2D path requires both source axes>=2; collapsed axis uses three sequential normalized float32 adds/quarter, RGBA8 writer sets x87 toward-zero for scaled and half-biased stores'},
 {'managedToken':'0x06000017','evidence':'LoadImageA clears source format bits0x1c0 with fffffe3f, then XGCopySurface destination1a200006 flags12 threshold0; resource header endian tag does not imply a CPU file-upload byte swap'}]
(args.out/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n');print(json.dumps({'originalSha256':EXPECTED,'functions':len(FUNCTIONS),'output':str(args.out)}))
