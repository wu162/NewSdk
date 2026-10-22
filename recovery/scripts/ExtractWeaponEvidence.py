"""Read original PE enum maps without loading or executing either compiler DLL."""
import json, re, struct
from pathlib import Path
module = Path("recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs").read_text(encoding="utf-8-sig")
pe = Path("SDK/tools/BinaryAssetBuilder.Tokenizer.dll").read_bytes()
peoff = struct.unpack_from("<I", pe, 0x3c)[0]
sections = struct.unpack_from("<H", pe, peoff+6)[0]
optsize = struct.unpack_from("<H", pe, peoff+20)[0]
imagebase = struct.unpack_from("<I", pe, peoff+24+28)[0]
sectionbase = peoff+24+optsize
def string_at(va):
    rva = va-imagebase
    for i in range(sections):
        _, size, base, rawsize, raw = struct.unpack_from("<8sIIII", pe, sectionbase+40*i)
        if base <= rva < base+max(size,rawsize):
            start=raw+rva-base
            return pe[start:pe.index(b"\0",start)].decode("ascii")
    raise ValueError(hex(va))
wanted = ["WeaponPrefireType","WeaponReAcquireDetailType","WeaponReloadType","WeaponAffectsType","WeaponFlagsType","WeaponCollideType","WpnAntiT","VirtualDamageType","FXTriggerType","ObjectStatusType","ObjectFilterRule","ObjectFilterAlignment","ObjectFilterRelationship","KindOfType","ModelConditionFlagType","PartitionManagerDistTestType","DamageType","DeathType","DamageFXType","DamageSubType","WeaponSlotType","VeterancyLevel"]
result={}
for name in wanted:
    line = next(l for l in module.splitlines() if "internal static" in l and "enumMap" in l and "W4"+name+"_0040" in l and "FireWeaponFlagsType" not in l)
    data=bytes.fromhex(re.search(r"data\(([^)]+)\)",line).group(1))
    result[name]=[{"name":string_at(ptr),"value":value} for ptr,value in struct.iter_unpack("<Ii",data)]
out=Path("recovery/evidence/weapon"); out.mkdir(parents=True,exist_ok=True)
(out/"enums.json").write_text(json.dumps(result,indent=2)+"\n")

