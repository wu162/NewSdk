#!/usr/bin/env python3
"""Read/disassemble fixed original DLL functions; never execute DLL or codec."""
import argparse,hashlib,json,struct,subprocess
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
EXPECTED='4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
FUNCTIONS=[
 ('ResourceAddRef',0xc599,0xc5d2),
 ('ResourceReleaseThunk',0x1940,0x1945),
 ('ResourceRelease',0xc5d2,0xc60c),
 ('ResourceDestroy',0xc3ec,0xc4d0),
 ('TextureUnlockThunk',0x1980,0x1994),
 ('TextureUnlock',0xc751,0xc76f),
 ('ResourceUnlock',0xc55c,0xc599)]
p=argparse.ArgumentParser(description=__doc__);p.add_argument('--root',type=Path,default=ROOT);p.add_argument('--out',type=Path,default=ROOT/'work/native-resource/evidence');args=p.parse_args()
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
manifest['scopeBoundary']='PC Win32 memory-resource lifetime, type0..8 except standalone GPU/context surfaces, parent views and UnlockRect; type9/GPU current-context/LockRect excluded'
manifest['criticalSites']=[
 {'rva':'0xc599','evidence':'InterlockedIncrement at object+4; result1 and type4/bit30 retains parent at+0x18 recursively'},
 {'rva':'0xc5d2','evidence':'InterlockedDecrement at object+4; result0 and type4/bit30 releases parent recursively then dispatches destruction'},
 {'rva':'0xc3ec','evidence':'type1 frees tagged+0x18; type2/7/8 frees+0x18; type3 frees masked+0x20/+0x30; type6 frees+0x20; all GlobalFree object'},
 {'rva':'0xc751','evidence':'secondary storage base is resource+0x30 with low12 bits cleared; level ignored'},
 {'rva':'0xc55c','evidence':'InterlockedExchangeAdd header,-256; only old lock nibble==1 sets dirty+0x14 toffff0000 and, when secondary base nonzero,+0x18'},
 {'rva':'0x198f','evidence':'public UnlockRect wrapper unconditionally returns0'}]
(args.out/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n');print(json.dumps({'originalSha256':EXPECTED,'functions':len(FUNCTIONS),'output':str(args.out)}))
