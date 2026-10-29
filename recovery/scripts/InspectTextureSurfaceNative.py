#!/usr/bin/env python3
"""Read/disassemble fixed original DLL functions; never execute DLL or codec."""
import argparse,hashlib,json,struct,subprocess
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
EXPECTED='4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
FUNCTIONS=[
 ('TextureLockThunk',0x1950,0x1973),
 ('TextureLock',0xdb40,0xdb60),
 ('TextureLockRect',0xd70f,0xd776),
 ('TextureLockLayout',0xd680,0xd680+0x8f),
 ('TextureMipLayout',0xd24b,0xd680),
 ('TexturePackedOffset',0xcd64,0xce5a),
 ('TextureDimensions',0xc628,0xc6c0),
 ('TextureResourceType',0xc388,0xc3ec),
 ('TextureBlockDimensions',0xc7c4,0xc83d),
 ('TextureStorageAlignment',0xc91d,0xc9f6),
 ('TextureLockState',0xc4d0,0xc55c)]
p=argparse.ArgumentParser(description=__doc__);p.add_argument('--root',type=Path,default=ROOT);p.add_argument('--out',type=Path,default=ROOT/'work/native-surface/evidence');args=p.parse_args()
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
manifest['scopeBoundary']='PC type3 native texture layout and LockRect; fixed metadata, no image-output table or original backend calls'
metadata=raw[file_offset(0x98230):file_offset(0x98230)+128]
(args.out/'FormatBits.bin').write_bytes(metadata)
manifest['formatBits']={'rva':'0x98230','bytes':128,'sha256':hashlib.sha256(metadata).hexdigest(),'bitsPerTexel':list(metadata[::2])}
manifest['criticalSites']=[
 {'rva':'0xd24b','evidence':'full level/base/mip/packed/array address, pitch, allocation bounds, dimensions and packed offset'},
 {'rva':'0xcd64','evidence':'small-mip packing general offsets from powers, level and format blocks'},
 {'rva':'0xc91d','evidence':'format block and tile/linear alignment; dimension-dependent page layout'},
 {'rva':'0xc4d0','evidence':'dirty range merge from address-packedOffset and total size; InterlockedExchangeAdd header,+256'},
 {'rva':'0xd70f','evidence':'rect left*bits/8 and top*block-row pitch, right/bottom ignored; writes pitch and pointer'},
 {'rva':'0x196e','evidence':'public LockRect unconditionally returns0'}]
(args.out/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n');print(json.dumps({'originalSha256':EXPECTED,'functions':len(FUNCTIONS),'output':str(args.out)}))
