#!/usr/bin/env python3
"""Read/disassemble fixed original DLL functions; never execute DLL or codec."""
import argparse,hashlib,json,struct,subprocess
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
EXPECTED='4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
FUNCTIONS=[
 ('BmpFileHeaderReader',0x167ad,0x16804),
 ('DibParser',0x13f3d,0x149af),
 ('BitmapInfoHeaderCopy40',0x13e99,0x13ee8),
 ('XGGetTextureDesc',0x3df34,0x3df4a),
 ('XGGetTextureDescImplementation',0x3dc51,0x3df34),
 ('TextureLevelDescThunk',0xdb60,0xdb65),
 ('TextureLevelDescImplementation',0xd18d,0xd1ec),
 ('FormatFromTextureHeader',0xc83d,0xc871)]
p=argparse.ArgumentParser(description=__doc__);p.add_argument('--root',type=Path,default=ROOT);p.add_argument('--out',type=Path,default=ROOT/'work/bmp-decoder/native-evidence');args=p.parse_args()
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
manifest['criticalSites']=[
 {'rva':'0x13f61','evidence':'push 0xa/pop ecx; ebx=ebp-0x44; rep movs DWORD copies exactly 40 header bytes into local stack'},
 {'rva':'0x13f67','evidence':'eax=ebx; push eax twice; helper0x13e99 receives identical destination/source local header pointers'},
 {'rva':'0x13e99','evidence':'helper only self-copies fixed offsets0..0x24, ending with DWORD+0x24; no mask or appended DIB copy'},
 {'rva':'0x13fc1','evidence':'noncore branch validates DIB>=40, retaining ebx=ebp-0x44; no initialization of mask-adjacent locals'},
 {'rva':'0x14038','evidence':'compression3 requires size>=52, then loads masks from ebx+0x28/+0x2c/+0x30 and +0x34 for size>=56; these are beyond the copied header'},
 {'rva':'0x14074','evidence':'generic fallback format0x1a22aaa6 is set before matching masks to known formats'},
 {'rva':'0x3ddbc','evidence':'normal resource type3 path calls TextureLevelDescThunk0xdb60 then writes output width+4,height+8,depth+12,format+16'},
 {'rva':'0xd1b9','evidence':'TextureLevelDescImplementation calls FormatFromTextureHeader0xc83d and stores returned format into level descriptor offset0 at0xd1c2'},
 {'rva':'0x3dddd','evidence':'XGGetTextureDescImplementation unconditionally stores copied level format at output+0x10 on type3 path'}]
(args.out/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n');print(json.dumps({'originalSha256':EXPECTED,'functions':len(FUNCTIONS),'output':str(args.out)}))
