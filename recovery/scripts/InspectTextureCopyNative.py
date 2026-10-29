#!/usr/bin/env python3
"""Read/disassemble fixed original DLL functions; never execute DLL or codec."""
import argparse,hashlib,json,struct,subprocess
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
EXPECTED='4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
FUNCTIONS=[
 ('XGCopySurface',0x3ccc0,0x3d969),
 ('XGCopySurfaceIdentityPath',0x3cdc4,0x3cef7),
 ('XGCopySurfaceBlockDimensions',0x3e615,0x3e645),
 ('FormatBlockDimensions',0xc7c4,0xc83d)]
p=argparse.ArgumentParser(description=__doc__);p.add_argument('--root',type=Path,default=ROOT);p.add_argument('--out',type=Path,default=ROOT/'work/native-copy/evidence');args=p.parse_args()
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
manifest['scopeBoundary']='Same full format, block-aligned region and compatible flag pairs identity row path; conversion/filter fallback not implemented or claimed'
metadata=raw[file_offset(0x97748):file_offset(0x97748)+64]
(args.out/'CopyFormatBits.bin').write_bytes(metadata)
manifest['formatBits']={'rva':'0x97748','bytes':64,'sha256':hashlib.sha256(metadata).hexdigest(),'bitsPerTexel':list(metadata)}
manifest['criticalSites']=[
 {'rva':'0x3cd59','evidence':'either low format0 or1 returns E_NOTIMPL'},
 {'rva':'0x3cdc4','evidence':'full format values equal and flags bits0==1/bits2==3 enter identity copy when all region block conditions hold'},
 {'rva':'0x3cdf4','evidence':'destination point/source rectangle origins aligned; unaligned right/bottom allowed only at supplied width/height'},
 {'rva':'0x3ce66','evidence':'ceil pixel extents to blocks; convert source/destination coordinates to block offsets; preserve signed row pitches'},
 {'rva':'0x3ceda','evidence':'memcpy each complete row, increment source/destination by supplied strides, return0; float argument unused on this path'}]
(args.out/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n');print(json.dumps({'originalSha256':EXPECTED,'functions':len(FUNCTIONS),'output':str(args.out)}))
