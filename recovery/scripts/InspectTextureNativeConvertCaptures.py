"""Independent complete capture model; only reads archived original bytes."""
import argparse,struct,hashlib,json,time,math
from pathlib import Path
F=lambda n:struct.unpack('<f',struct.pack('<f',n))[0]
ROOT=Path(__file__).resolve().parents[2]
D=(ROOT/'recovery/evidence/texture/native-convert-static/20261030/FormatDescriptors.bin').read_bytes()
def fmt(n):
 v=struct.unpack_from('<15I',D,(n&63)*60);return v[1]//8,v[2]//8,v[3:7],v[7:11],v[11:15]
def halfValue(h):
 value=math.ldexp(1+(h&1023)/1024,((h>>10)&31)-15) if h&0x7c00 else math.ldexp(h&1023,-24)
 return -value if h&0x8000 else value

def floatHalf(value):
 bits=struct.unpack('<I',struct.pack('<f',value))[0];magnitude=bits&0x7fffffff;sign=(bits>>16)&0x8000
 if magnitude>0x47ffefff:return sign|32767
 if magnitude<0x38800000:mantissa=((magnitude&0x7fffff)|0x800000)//(2**((113-(magnitude>>23))%32))
 else:mantissa=magnitude-0x38000000
 return sign|round(mantissa/8192)

def convert(pixel,sf,df,threshold):
 sb,sw,so,sm,ss=fmt(sf);db,dw,do,dm,ds=fmt(df);physical=[]
 sfFloat=sf&63 in (27,28,29,30,31,32,36,37,38,57);dfFloat=df&63 in (27,28,29,30,31,32,36,37,38,57)
 for c in range(4):
  maximum=sm[c]>>ss[c];n=(int.from_bytes(pixel[so[c]*sw:(so[c]+1)*sw],'little')&sm[c])>>ss[c]
  if sfFloat:
   value=halfValue(n) if maximum==65535 else struct.unpack('<f',struct.pack('<I',n))[0]
   physical.append(-math.inf if math.isnan(value) else 0.0 if value==0 else value);continue
  kind=(sf>>(9+2*c))&3;midpoint=(maximum>>1)+1
  if not maximum:physical.append(0);continue
  if kind==1:n^=midpoint
  factor=1 if sf&0x20000 else F(1/(midpoint-1 if kind else maximum))
  bias=F((-midpoint if kind else 0)*factor);value=F(n*factor+bias)
  physical.append(max(-1,value) if kind and not sf&0x20000 else value)
 physical.extend([0,1]);logical=[physical[(sf>>(18+3*c))&7] for c in range(4)];out=[0]*4
 for c in range(4):
  selector=(df>>(18+3*c))&7
  if selector<4:out[selector]=logical[c]
 result=bytearray(db)
 for c in range(4):
  maximum=dm[c]>>ds[c];kind=(df>>(9+2*c))&3;midpoint=(maximum>>1)+1;integer=bool(df&0x20000)
  if not maximum:continue
  if dfFloat:
   q=floatHalf(out[c]) if maximum==65535 else struct.unpack('<I',struct.pack('<f',0.0 if out[c]==0 else out[c]))[0]
   start=do[c]*dw;word=int.from_bytes(result[start:start+dw],'little')|((q<<ds[c])&dm[c]);result[start:start+dw]=word.to_bytes(dw,'little');continue
  lo=-(midpoint-1 if integer else 1) if kind else 0
  hi=(midpoint-1 if kind else maximum) if integer else 1
  value=min(hi,max(lo,out[c]));factor=1 if integer else midpoint-1 if kind else maximum;bias=midpoint if kind else 0
  if maximum==1:bias=F(bias+F(.5-threshold))
  scaled=F(value*factor+bias);q=int(F(scaled+(-.5 if scaled<0 else .5)))
  if kind==1:q^=midpoint
  start=do[c]*dw;word=int.from_bytes(result[start:start+dw],'little')|((q<<ds[c])&dm[c]);result[start:start+dw]=word.to_bytes(dw,'little')
 return result

def audit(path):
 start=time.monotonic();data=path.read_bytes();assert data[:8] in (b'TXCNV001',b'TXCSG001',b'TXCFP001',b'TXCEN001',b'TXCFG001');signed=data[:8]==b'TXCSG001';floating=data[:8]==b'TXCFP001';endian=data[:8]==b'TXCEN001';flagged=data[:8]==b'TXCFG001';offset=8;counts={'cases':0,'success':0,'outOfMemory':0,'identity':0,'convertedPixels':0,'exhaustivePixels':0,'sourceBytes':0,'destinationBytes':0}
 ids=[2,3,4,5,6,7,8,9,10,14,15,16,17,24,25,26,41,42,43,45,46,47,48,50,54,55,56,62]
 S=lambda a,b,c,d:(a<<18)|(b<<21)|(c<<24)|(d<<27)
 if floating or endian or flagged:ids+=[27,28,29,30,31,32,36,37,38,57]
 maps=[S(0,1,2,3),S(2,1,0,5),S(0,0,0,5),S(4,5,1,2)]
 def unsignedParameters():
  for sf in ids:
   for df in ids:
    for mapping in range(4):
     for orientation in range(4):
      for region in range(4):
       for padding in (0,7):yield sf|maps[mapping],df|maps[(mapping+region)%4],[1,7,17,31][region],[1,3,5,2][region],padding,orientation,region,[0,.5,1][(mapping+orientation)%3],False
  for sf in [3,4,5,10,15,24,41,45,46,47]:
   for threshold in [0,.5,1]:
    yield sf|maps[0],6|maps[0],256,256,0,0,0,threshold,True
    yield sf|maps[1],3|maps[0],256,256,0,0,0,threshold,True
 def attrs(format,state):
  value=0x20000 if state in (1,3,5,7) else 0
  for c in range(4):
   kind=1 if state in (2,3) else 2 if state in (4,5) else c%3 if state==6 else (c+2)%3 if state==7 else 0
   if format==3 and c==3:kind=0
   value|=kind<<(9+c*2)
  return value
 def signedParameters():
  for sf in ids:
   for df in ids:
    for sourceState in range(8):
     for destinationState in range(8):
      for mapping in range(2):yield sf|attrs(sf,sourceState)|maps[(mapping+sourceState)%4],df|attrs(df,destinationState)|maps[(mapping+destinationState)%4],7 if mapping else 3,1 if mapping else 2,7 if mapping else 0,0,0,1 if mapping else .5,False
  for sf in [3,4,5,10,15,24,41,45,46,47]:
   for state in range(6):
    yield sf|attrs(sf,state)|maps[0],6|attrs(6,(state+2)%6)|maps[1],256,256,0,0,0,.5,True
    yield sf|attrs(sf,state)|maps[1],3|attrs(3,(state+3)%6)|maps[0],256,256,0,0,0,1,True
 def floatParameters():
  for sf in ids:
   for df in ids:
    for state in range(8):
     for mapping in range(4):yield sf|attrs(sf,state)|maps[mapping],df|attrs(df,(state+mapping*2)%8)|maps[(mapping+state)%4],[1,3,7,17][mapping],[1,2,3,5][mapping],7 if mapping%2 else 0,0,mapping,.5,0
  for sf in [27,28,29,30,31,32]:
   for df in [6,36]:
    for mapping in range(2):yield sf|maps[mapping],df|maps[0],256,256,0,0,0,.5,1
  for sf in [36,37,38,57]:
   for df in [27,6,36]:
    for mapping in range(2):yield sf|maps[mapping],df|maps[(mapping+1)%4],256,256,0,0,0,.5,2
  for mode in range(3,6):
   for df in [27,29]:yield 36|maps[0],df|maps[0],256,256,0,0,0,.5,mode
 def endianParameters():
  for sf in ids:
   for df in ids:
    for se in range(4):
     for de in range(4):
      for state in range(2):
       for mapping in range(2):
        for region in range(2):yield sf|(se<<6)|attrs(sf,6 if state else 0)|maps[mapping],df|(de<<6)|attrs(df,7 if state else 0)|maps[1-mapping],8,3,7 if mapping else 0,0,region,.5,0
  for sf in [24,27]:
   for se in range(4):
    for de in range(4):yield sf|(se<<6)|maps[0],6|(de<<6)|maps[1],256,256,0,0,0,.5,1
  for se in range(4):
   for de in range(4):
    for orientation in range(4):yield 6|(se<<6)|maps[0],50|(de<<6)|maps[1],4,2,7,orientation,0,.5,0
 def flagsParameters():
  flagSets=[0,1,2,3,4,8,12,15,18,0xffffffff,0x80000000,0x12345678]
  for sf in ids:
   for df in ids:
    for flags in flagSets:
     for mapping in range(2):
      state=(sf+df+flags)%8
      yield sf|attrs(sf,state)|maps[mapping]|(0x80 if mapping else 0),df|attrs(df,(state+3)%8)|maps[1-mapping]|(0 if mapping else 0x40),8,2,7 if mapping else 0,0,0,0 if flags==18 else .5,0,flags
  for sf in ids:
   for state in range(8):
    for flags in flagSets:
     for mapping in range(4):
      format=sf|attrs(sf,state)|maps[mapping]|(mapping<<6)
      yield format,format,8,2,7,0,0,0 if flags==18 else .5,0,flags
  for sf in [3,4,5,10,15,24,27,36]:
   for state in [0,2,3,4,5]:
    for flags in [1,18]:
     format=sf|attrs(sf,state)|maps[0]
     yield format,format,256,256,0,0,0,0 if flags==18 else .5,2 if sf==36 else 1,flags
 parameters=flagsParameters if flagged else endianParameters if endian else floatParameters if floating else signedParameters if signed else unsignedParameters
 for ordinal,parameter in enumerate(parameters()):
  sf,df,w,h,padding,orientation,region,threshold,exhaustive=parameter[:9];flags=parameter[9] if flagged else 0
  identity=sf==df and not (flags^(flags>>1))&5
  fields=struct.unpack_from('<13I' if flagged else '<12I',data,offset);offset+=52 if flagged else 48
  assert fields[:8]==(ordinal,sf,df,w,h,padding,orientation,region),(ordinal,'metadata')
  assert struct.unpack('<f',struct.pack('<I',fields[8]))[0]==threshold
  flagsField=fields[9] if flagged else 0;assert flagsField==flags
  hr,ns,nd=fields[10:] if flagged else fields[9:];sb=fmt(sf)[0];db=fmt(df)[0];sp=w*sb+padding;dp=w*db+padding
  assert ns==sp*h+64 and nd==dp*h+64
  source=data[offset:offset+ns];destination=data[offset+ns:offset+ns+nd];offset+=ns+nd
  expectedSource=bytearray(((i*13+ordinal*17)^(i>>3))&255 for i in range(ns));expected=bytearray((i*31+ordinal*19)&255 for i in range(nd))
  if exhaustive:
   if not (floating or flagged):
    for i in range(65536):struct.pack_into('<H',expectedSource,32+i*2,i)
   elif exhaustive==1:
    for i in range(65536):
     for c in range(sb//2):struct.pack_into('<H',expectedSource,32+i*sb+c*2,(i+c*7919)&65535)
   else:
    for i in range(65536):
     for c in range(sb//4):
      if exhaustive==2:
       n=(i+c*7919)&65535;bits=((n&32768)<<16)|(((n>>7)&255)<<23)|((n&127)*0x10204)
      else:
       half=i&32767;value=halfValue(half);nextValue=131072.0 if half==32767 else halfValue(half+1);midpoint=F((value+nextValue)*.5);bits=struct.unpack('<I',struct.pack('<f',midpoint))[0]+exhaustive-4;bits|=(i&32768)<<16
      struct.pack_into('<I',expectedSource,32+i*sb+c*4,bits)
   counts['exhaustivePixels']+=65536
  assert source==expectedSource,(ordinal,'complete source')
  expectedHr=0x8007000e if sf!=df and orientation else 0
  assert hr==expectedHr,(ordinal,'HRESULT',hex(hr),hex(expectedHr))
  if hr:counts['outOfMemory']+=1
  else:
   counts['success']+=1;left=top=x=y=0;right=w;bottom=h
   if region==1:left=4 if endian else int(w>1);top=1 if endian else int(h>1)
   elif region==2:x=int(w>1);y=int(h>1);right=w-1 if w>1 else 0;bottom=h-1 if h>1 else 0
   elif region==3:left=2 if w>2 else 0;x=left
   if identity:counts['identity']+=1
   cache={}
   def order(data,mode):
    if not mode:return data
    unit=2 if mode==1 else 4;mask=[0,1,3,2][mode]
    assert len(data)%unit==0
    # Independent complete-unit byte permutation; never invokes native code.
    return b''.join(bytes(data[base+(byte^mask)] for byte in range(unit)) for base in range(0,len(data),unit))
   for row in range(bottom-top):
    sr=(h-1-top-row) if orientation&1 else top+row;dr=(h-1-y-row) if orientation&2 else y+row
    sourceOffset=32+sr*sp+left*sb;destOffset=32+dr*dp+x*db
    window=source[sourceOffset:sourceOffset+(right-left)*sb]
    if not identity:window=order(window,(sf>>6)&3)
    output=bytearray()
    for col in range(right-left):
     pixel=window[col*sb:(col+1)*sb]
     if identity:stored=pixel
     else:
      if pixel not in cache:cache[pixel]=convert(pixel,sf,df,threshold)
      stored=cache[pixel];counts['convertedPixels']+=1
     output.extend(stored)
    if not identity:output=order(output,(df>>6)&3)
    expected[destOffset:destOffset+len(output)]=output

  if destination!=expected:
   first=next(i for i,(a,b) in enumerate(zip(destination,expected)) if a!=b);raise AssertionError((ordinal,'full destination',hex(sf),hex(df),first,destination[first],expected[first]))
  counts['cases']+=1;counts['sourceBytes']+=ns;counts['destinationBytes']+=nd
  if ordinal%20000==0:print('audited',ordinal,flush=True)
 assert offset==len(data) and counts['cases']==(49328 if flagged else 184928 if endian else 46262 if floating else 100472 if signed else 100412)
 return {'path':str(path),'sha256':hashlib.sha256(data).hexdigest(),'bytes':len(data),**counts,'seconds':round(time.monotonic()-start,3),'status':'passed','scope':'Every archived metadata/input/source/output/padding/guard byte and pixel checked with independent Python model from original descriptor metadata; no original/recovered DLL execution'}
if __name__=='__main__':
 p=argparse.ArgumentParser();p.add_argument('capture',type=Path);p.add_argument('--out',type=Path);a=p.parse_args();r=audit(a.capture);print(json.dumps(r,indent=2));
 if a.out:a.out.write_text(json.dumps(r,indent=2)+'\n')
