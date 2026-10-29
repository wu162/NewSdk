#!/usr/bin/env python3
"""Read/disassemble fixed original DLL functions; never execute DLL or codec."""
import argparse,hashlib,json,struct,subprocess
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
EXPECTED='4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
FUNCTIONS=[
 ('DeviceCreateThunk',0x1c10,0x1c15),
 ('DeviceReleaseThunk',0x1ad0,0x1ad5),
 ('DeviceAllocateAligned',0xe044,0xe06e),
 ('DeviceAllocateZeroed',0xe06e,0xe097),
 ('DeviceCreate',0xe097,0xe0fe),
 ('DeviceInitialize',0x115fe,0x1165a),
 ('DeviceInitializeRenderer',0x1165a,0x1169a),
 ('DeviceRelease',0xdf8a,0xdfaf),
 ('DeviceDestroy',0x1169a,0x116ec),
 ('DeviceFreeAligned',0xdf76,0xdf8a)]
p=argparse.ArgumentParser(description=__doc__);p.add_argument('--root',type=Path,default=ROOT);p.add_argument('--out',type=Path,default=ROOT/'work/native-device/evidence');args=p.parse_args()
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
manifest['scopeBoundary']='Factory type!=2 (PC plugin uses type1), no bound resources or secondary allocations; type2 rendering and OOM equality not claimed'
manifest['constants']=[]
for name,rva in [('deviceValueAt2a80',0xb8308),('deviceValueAt2a84',0xb830c)]:
 value=struct.unpack_from('<I',raw,file_offset(rva))[0]
 manifest['constants'].append({'name':name,'originalRva':hex(rva),'value':hex(value)})
manifest['criticalSites']=[
 {'rva':'0xe097','evidence':'output cleared before allocation; six stdcall arguments; aligned zeroed object size0x5500/alignment128'},
 {'rva':'0x115fe','evidence':'refcount1, two fixed global values, actual creating thread ID, VirtualAlloc 0x12c0 MEM_COMMIT PAGE_READWRITE, return nonzero iff allocation succeeds'},
 {'rva':'0xe0ce','evidence':'only type2 enters rendering-state setup; other inputs/parameters ignored'},
 {'rva':'0xdf8a','evidence':'refcount!=1 wraps/decrements uint32 and returns; count1 runs destruction then frees hidden GlobalAlloc base'},
 {'rva':'0x1169a','evidence':'factory type!=2 has no bound-resource cleanup/current-renderer/secondary allocation; release VirtualFree command allocation'}]
(args.out/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n');print(json.dumps({'originalSha256':EXPECTED,'functions':len(FUNCTIONS),'output':str(args.out)}))
