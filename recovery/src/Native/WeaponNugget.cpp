#include "WeaponNugget.h"
#include "ScalarParsing.h"
#include "DataInitializers.h"
#include "ResourceMarshaling.h"
#include "EnumBitset.h"
#include <algorithm>
#include <cstddef>
#include <cstring>
#include <stdexcept>
#ifdef _MANAGED
#pragma managed(push, off)
#endif
namespace Recovered { namespace Native {
namespace {
using U32 = std::uint32_t;
constexpr U32 DamageId = 3237297817u, DotId = 4022030886u, GrapplingId = 1093138400u;
constexpr U32 ContainedId = 2459653016u;
constexpr U32 SpawnId = 3266020104u, FireId = 2014632688u;
constexpr U32 LeechPercentId = 921679887u, LeechHealthId = 4012276286u, LineId = 4021248832u;
constexpr U32 ProjectileId=2278014412u, ScatterProjectileId=4131529406u, OclId=3322313791u, SeedId=1757028425u;
bool ReferenceFamily(U32 id) { return id==ProjectileId || id==ScatterProjectileId || id==OclId || id==SeedId; }
struct WordList { U32 count; U32* values; };
struct BaseNugget {
    U32 type, partition; U32* status; U32* model;
    float radius; ObjectFilter* special;
    WordList required, forbidden;
};
struct VeteranProjectile { U32 level, projectile; };
struct VeteranList { U32 count; VeteranProjectile* values; };
struct ProjectileNugget { BaseNugget base; U32 warhead, projectile, slot; float* offset; VeteranList veterans; int* barrel; };
struct ScatterProjectileNugget { BaseNugget base; float min, max; };
struct OclNugget { BaseNugget base; U32 ocl; unsigned char target, padding[3]; };
static_assert(sizeof(ProjectileNugget)==68 && sizeof(ScatterProjectileNugget)==48 && sizeof(OclNugget)==48 && sizeof(VeteranProjectile)==8, "Original reference family layouts");
struct ScalarInfo { float scalar; ObjectFilter filter; WordList required; };
struct ScalarList { U32 count; ScalarInfo* values; };
struct DamageNugget {
    BaseNugget base;
    float damage, taper, minRadius, arc, maxHeight, maxTerrain, flank, flanked, delay;
    U32 damageType, deathType, fxType, subType;
    float drainMultiplier, speed; U32 eva, revealer, invalidStatus[7];
    ScalarList scalars; unsigned char flags[8];
};
struct ContainedNugget { DamageNugget damage; U32 maxUnits, windowFx; ObjectFilter filter; };
struct SpawnNugget { DamageNugget damage; float probability; U32 model[15], objectId; };
struct FireNugget { DamageNugget damage; U32 weapon; ObjectFilter* filter; unsigned char onVictim, mustBeAlive, padding[2]; };
struct LineNugget { DamageNugget damage; float angle, width, leadIn, leadOut, overshoot; unsigned char dynamicLength, padding[3]; };
struct PercentNugget { DamageNugget damage; float percentage; };
struct DotNugget { DamageNugget damage; float interval, duration; unsigned char removeIfHealed, padding[3]; };
struct SeedNugget { OclNugget ocl; float amount, bonus; };
static_assert(sizeof(SeedNugget)==56, "Original SeedTiberium layout");
struct Procedure { U32 name, type; void* offset; };
struct TokenKey { U32 name, type; };
static_assert(sizeof(BaseNugget)==40 && sizeof(DamageNugget)==152 && sizeof(DotNugget)==164 && sizeof(PercentNugget)==156 && sizeof(LineNugget)==176 && sizeof(SpawnNugget)==220 && sizeof(FireNugget)==164 && sizeof(ContainedNugget)==272, "Original x86 nugget layouts");
static_assert(sizeof(ScalarInfo)==124 && offsetof(DamageNugget, scalars)==136 && offsetof(DamageNugget, flags)==144, "Original nested layouts");
const TokenKey WeaponEffectNuggetTokens[] = {
    { 430981792u, 602296504u },
    { 4166854928u, 2668471815u },
    { 3267660424u, 2814214430u },
    { 3196120606u, 1742454729u },
    { 1328662213u, 1683514611u },
    { 3457629061u, 3832456114u },
    { 2138150697u, 653971399u },
    { 725359606u, 653971399u },
    { 0u, 0u },
};
const TokenKey DamageNuggetTypeTokens[] = {
    { 1u, 3351432196u },
    { 2083775721u, 1683514611u },
    { 293792262u, 1683514611u },
    { 3963169641u, 1683514611u },
    { 480727292u, 1310124621u },
    { 217922344u, 2011428087u },
    { 103384912u, 1683514611u },
    { 580610775u, 1683514611u },
    { 2033101237u, 2011428087u },
    { 2189088468u, 1683514611u },
    { 1806152986u, 1683514611u },
    { 2328356458u, 2575722984u },
    { 1179152369u, 1179152369u },
    { 3322473782u, 3322473782u },
    { 799690338u, 799690338u },
    { 1656429799u, 1656429799u },
    { 1874552506u, 2011428087u },
    { 4084992252u, 1683514611u },
    { 1914840045u, 2011428087u },
    { 833111988u, 1683514611u },
    { 4203588415u, 3303818161u },
    { 771372620u, 3215691893u },
    { 1409035032u, 2011428087u },
    { 104611904u, 2011428087u },
    { 1170446220u, 2011428087u },
    { 558449986u, 2011428087u },
    { 3733354916u, 2814214430u },
    { 2442600653u, 3836143332u },
    { 0u, 0u },
};
const TokenKey ScalarInfoTokens[] = {
    { 1556264584u, 1683514611u },
    { 2239343654u, 3832456114u },
    { 2138150697u, 653971399u },
    { 0u, 0u },
};
const EnumNameValue PartitionManagerDistTestType[] = {
    { "CENTER_2D", 0 },
    { "CENTER_3D", 2 },
    { "CIRCLE", 1 },
    { "EXTENTS_3D", 4 },
    { "SPHERE", 3 },
};
const EnumNameValue DamageType[] = {
    { "ALL", 38 },
    { "AUTO_CANNON", 29 },
    { "BLAST", 10 },
    { "CANNON", 22 },
    { "CONCUSSIVE", 32 },
    { "CRUSH", 1 },
    { "EXPLOSIVE", 31 },
    { "FALLING", 9 },
    { "FIRE", 19 },
    { "FLAK", 33 },
    { "FLAME", 4 },
    { "FORCE", 0 },
    { "GRENADE", 23 },
    { "GUN", 21 },
    { "HEALING", 5 },
    { "IMPACT", 30 },
    { "KI", 36 },
    { "LASER", 17 },
    { "LOGICAL_FIRE", 15 },
    { "MAGIC", 13 },
    { "MELEE", 28 },
    { "MINE", 25 },
    { "PASSENGER", 12 },
    { "PENALTY", 8 },
    { "PIERCE", 3 },
    { "PLASMA", 18 },
    { "POISON", 16 },
    { "PRISM", 34 },
    { "RADIATION", 37 },
    { "REFLECTED", 11 },
    { "ROCKET", 20 },
    { "SLASH", 2 },
    { "SNIPER", 24 },
    { "STORM_SHIELD", 27 },
    { "TESLA", 35 },
    { "TIBERIUM", 26 },
    { "UNDEFINED", 14 },
    { "UNRESISTABLE", 6 },
    { "WATER", 7 },
};
const EnumNameValue DeathType[] = {
    { "ALL", 33 },
    { "BITTEN", 24 },
    { "BURNED", 3 },
    { "CATALYST", 19 },
    { "CRUSHED", 2 },
    { "DETONATED", 10 },
    { "ELECTROCUTED", 11 },
    { "EXPLODED", 4 },
    { "EXTRA_1", 26 },
    { "EXTRA_2", 27 },
    { "EXTRA_3", 28 },
    { "EXTRA_4", 29 },
    { "EXTRA_5", 30 },
    { "EXTRA_6", 31 },
    { "EXTRA_7", 32 },
    { "FADED", 17 },
    { "FLOODED", 7 },
    { "INTERNAL_DESTRUCTION", 21 },
    { "IRRADIATED", 22 },
    { "KNOCKBACK", 15 },
    { "LASERED", 9 },
    { "MAULED", 23 },
    { "NONE", 0 },
    { "NORMAL", 1 },
    { "POISONED", 5 },
    { "POISONED_BETA", 14 },
    { "PRODUCTION_CANCELLED", 25 },
    { "SHATTERED", 20 },
    { "SLAUGHTERED", 18 },
    { "SPLATTED", 13 },
    { "STABBED", 12 },
    { "SUICIDED", 8 },
    { "SUPERNATURAL", 16 },
    { "TOPPLED", 6 },
};
const EnumNameValue DamageFXType[] = {
    { "ALIEN_BOMB", 35 },
    { "ALIEN_BUZZER", 32 },
    { "ALIEN_CANNON", 27 },
    { "ALIEN_DISINTEGRATOR", 39 },
    { "ALIEN_GRENADE", 33 },
    { "ALIEN_ION", 36 },
    { "ALIEN_LASER", 30 },
    { "ALIEN_MACHINEGUN", 31 },
    { "ALIEN_MINE", 29 },
    { "ALIEN_PLASMADISK", 28 },
    { "ALIEN_RAILGUN", 38 },
    { "ALIEN_SONIC", 37 },
    { "ALIEN_TIOXIN", 34 },
    { "ALLIED_ATHENA", 74 },
    { "ALLIED_BOMB", 76 },
    { "ALLIED_BOMBLVL1", 77 },
    { "ALLIED_BOMBLVL2", 78 },
    { "ALLIED_BOMBLVL3", 79 },
    { "ALLIED_CANNON", 63 },
    { "ALLIED_CHAINGUN", 71 },
    { "ALLIED_DOG", 70 },
    { "ALLIED_FIGHTER", 72 },
    { "ALLIED_INFANTRYMISSILE", 66 },
    { "ALLIED_MISSILE", 65 },
    { "ALLIED_PAINTER", 69 },
    { "ALLIED_PARTICLE", 75 },
    { "ALLIED_PHALANX", 80 },
    { "ALLIED_PRESISIONBOMB", 82 },
    { "ALLIED_PRIMAGECANNON", 84 },
    { "ALLIED_PRIMAGECANNONVET1", 85 },
    { "ALLIED_PRIMAGECANNONVET2", 86 },
    { "ALLIED_PRISM", 83 },
    { "ALLIED_SAM", 67 },
    { "ALLIED_SCRAMBLER", 81 },
    { "ALLIED_SHOTGUN", 64 },
    { "ALLIED_SONIC", 73 },
    { "ALLIED_TANYA", 68 },
    { "CLUBBING", 110 },
    { "DEFAULT", -1 },
    { "GDI_BOMB", 8 },
    { "GDI_CANNON", 0 },
    { "GDI_FLAMETHROWER", 7 },
    { "GDI_GRENADE", 6 },
    { "GDI_ION", 11 },
    { "GDI_LASER", 3 },
    { "GDI_MACHINEGUN", 4 },
    { "GDI_MINE", 2 },
    { "GDI_MISSILE", 1 },
    { "GDI_RAILGUN", 10 },
    { "GDI_RIFLE", 5 },
    { "GDI_SHOCKWAVE", 12 },
    { "GDI_SONIC", 9 },
    { "GENERIC_FIRE", 108 },
    { "JAPAN_AIRCRAFT", 102 },
    { "JAPAN_AUTOCANNON", 96 },
    { "JAPAN_BEAM", 100 },
    { "JAPAN_BOMB", 97 },
    { "JAPAN_BONZAI", 103 },
    { "JAPAN_CANNON", 95 },
    { "JAPAN_FORCE", 88 },
    { "JAPAN_GUN", 89 },
    { "JAPAN_KAMIKAZE", 104 },
    { "JAPAN_MECHA", 91 },
    { "JAPAN_MECHAMISSILE", 93 },
    { "JAPAN_MECHARUSH", 92 },
    { "JAPAN_MISSILE", 90 },
    { "JAPAN_PLASMA", 99 },
    { "JAPAN_PSIONIC", 101 },
    { "JAPAN_ROCKET", 94 },
    { "JAPAN_RUSH", 98 },
    { "JAPAN_SHIRUKEN", 105 },
    { "JAPAN_TORPEDO", 87 },
    { "NEUTRAL_CANNON", 106 },
    { "NOD_BOMB", 21 },
    { "NOD_CANNON", 13 },
    { "NOD_FLAMETHROWER", 20 },
    { "NOD_GRENADE", 19 },
    { "NOD_HALLUCINE", 26 },
    { "NOD_ION", 24 },
    { "NOD_LASER", 16 },
    { "NOD_MACHINEGUN", 17 },
    { "NOD_MINE", 15 },
    { "NOD_MISSILE", 14 },
    { "NOD_RAILGUN", 23 },
    { "NOD_RIFLE", 18 },
    { "NOD_SEED", 25 },
    { "NOD_SONIC", 22 },
    { "POISON", 109 },
    { "RADIATION", 111 },
    { "REFLECTED", 107 },
    { "SOVIET_AK47", 45 },
    { "SOVIET_BEAR", 54 },
    { "SOVIET_BIGBOMB", 60 },
    { "SOVIET_BOMB", 55 },
    { "SOVIET_CANNON", 40 },
    { "SOVIET_DEBRIS", 59 },
    { "SOVIET_FLAK", 48 },
    { "SOVIET_FLAKVET", 49 },
    { "SOVIET_HARPOON", 42 },
    { "SOVIET_IRONCURTAIN", 62 },
    { "SOVIET_LASER", 44 },
    { "SOVIET_LEECH", 41 },
    { "SOVIET_MACHINEGUN", 46 },
    { "SOVIET_MAGNETICBOMB", 52 },
    { "SOVIET_MISSILE", 43 },
    { "SOVIET_MOLOTOV", 50 },
    { "SOVIET_ORBITAL", 56 },
    { "SOVIET_ORBITAL2", 57 },
    { "SOVIET_ORBITAL3", 58 },
    { "SOVIET_REPAIR", 53 },
    { "SOVIET_SNIPERRIFLE", 47 },
    { "SOVIET_TESLA", 51 },
    { "SOVIET_TORPEDO", 61 },
    { "UNDEFINED", 112 },
};
const EnumNameValue DamageSubType[] = {
    { "BECOME_UNDEAD", 1 },
    { "NORMAL", 0 },
    { "SELF", 2 },
    { "UNDEFINED", 3 },
};

std::string Text(const WeaponNuggetInput& input, const char* name, const char* fallback) {
    auto found=input.attributes.find(name); return found==input.attributes.end() ? std::string(fallback) : found->second;
}
template<std::size_t N> U32 Enum(const std::string& text, const EnumNameValue (&names)[N], U32 fallback) {
    for(const auto& entry:names) if(text==entry.name) return static_cast<U32>(entry.value);
    return fallback;
}
void Swap(void* ptr) { auto p=static_cast<unsigned char*>(ptr); std::swap(p[0],p[3]);std::swap(p[1],p[2]); }
void MarshalWords(const std::vector<std::string>& input, WordList& list, Relo::Tracker& data) {
    list.count=static_cast<U32>(input.size());
    if(list.count) {
        data.Push(reinterpret_cast<void**>(&list.values),4,list.count);
        for(U32 i=0;i<list.count;++i) list.values[i]=HashAssetId(input[i]);
        data.Pop();
    }
}
void TokenWords(WordList& list, void** table, Relo::Tracker& tokens) {
    auto entries=static_cast<Procedure*>(tokens.Push(table,12,list.count+1));
    entries[0].name=1820487795u;entries[0].type=1564774360u;
    tokens.BookmarkMemberOffset(&list.count,&entries[0].offset);
    for(U32 i=0;i<list.count;++i) {
        entries[i+1].name=2974916843u;
        tokens.BookmarkMemberOffset(list.values+i,&entries[i+1].offset);
    }
    tokens.Pop();
}
void EndianWords(WordList& list) {
    for(U32 i=0;i<list.count;++i) Swap(list.values+i);
    Swap(&list.count);
}
void SetKeys(Procedure* table, const TokenKey* keys, unsigned count) {
    for(unsigned i=0;i<count;++i) { table[i].name=keys[i].name;table[i].type=keys[i].type; }
}
void Member(void* object,unsigned offset,Procedure& entry,Relo::Tracker& tokens) {
    tokens.BookmarkMemberOffset(static_cast<unsigned char*>(object)+offset,&entry.offset);
}
void MarshalBase(const WeaponNuggetInput& input,BaseNugget& object,Relo::Tracker& data) {
    object.partition=Enum(Text(input,"PartitionFilterTestType","EXTENTS_3D"),PartitionManagerDistTestType,0);
    if(input.attributes.count("ForbiddenTargetObjectStatus")) {
        data.Push(reinterpret_cast<void**>(&object.status),28,1);
        MarshalObjectStatus(Text(input,"ForbiddenTargetObjectStatus",""),object.status);data.Pop();
    }
    if(input.attributes.count("ForbiddenTargetModelCondition")) {
        data.Push(reinterpret_cast<void**>(&object.model),60,1);
        MarshalModelCondition(Text(input,"ForbiddenTargetModelCondition",""),object.model);data.Pop();
    }
    object.radius=ParseFloat(Text(input,"Radius","0.0"));
    if(input.hasSpecialFilter) {
        data.Push(reinterpret_cast<void**>(&object.special),112,1);
        MarshalObjectFilter(input.specialFilter,object.special,data);data.Pop();
    }
    MarshalWords(input.requiredUpgrades,object.required,data);
    MarshalWords(input.forbiddenUpgrades,object.forbidden,data);
}
void TokenBase(BaseNugget& object,void** table,Relo::Tracker& tokens) {
    auto t=static_cast<Procedure*>(tokens.Push(table,12,9));SetKeys(t,WeaponEffectNuggetTokens,9);
    Member(&object,0,t[0],tokens);Member(&object,4,t[1],tokens);
    if(object.status) tokens.BookmarkMemberOffset(object.status,&t[2].offset);
    if(object.model) tokens.BookmarkMemberOffset(object.model,&t[3].offset);
    Member(&object,16,t[4],tokens);
    if(object.special) TokenizeObjectFilter(object.special,&t[5].offset,tokens);
    TokenWords(object.required,&t[6].offset,tokens);TokenWords(object.forbidden,&t[7].offset,tokens);tokens.Pop();
}
void EndianBase(BaseNugget& object) {
    Swap(&object.type);Swap(&object.partition);Swap(&object.radius);
    if(object.status) for(unsigned i=0;i<7;++i) Swap(object.status+i);
    if(object.model) for(unsigned i=0;i<15;++i) Swap(object.model+i);
    if(object.special) EndianObjectFilter(object.special);
    EndianWords(object.required);EndianWords(object.forbidden);
}
struct Field { const char* name;const char* fallback;unsigned offset;char kind; };
const Field damageFields[] = {
    {"Damage","0",40,'f'},{"DamageTaperOff","-1.0",44,'f'},
    {"MinRadius","0.0",48,'f'},{"DamageArc","360d",52,'a'},
    {"DamageArcInverted","false",144,'b'},{"DamageMaxHeight","-1",56,'f'},
    {"DamageMaxHeightAboveTerrain","-1",60,'f'},{"AcceptDamageAdd","true",145,'b'},
    {"FlankingBonus","0",64,'f'},{"FlankedScalar","1",68,'f'},{"DelayTimeSeconds","0s",72,'t'},
    {"DamageType","UNDEFINED",76,'e'},{"DeathType","NORMAL",80,'e'},
    {"DamageFXType","UNDEFINED",84,'e'},{"DamageSubType","NORMAL",88,'e'},
    {"OnlyKillOwnerWhenTriggered","false",146,'b'},{"DrainLifeMultiplier","1",92,'f'},
    {"DrainLife","false",147,'b'},{"DamageSpeed","0",96,'f'},
    {"UnderAttackOverrideEvaEvent","",100,'r'},{"VictimShroudRevealer",nullptr,104,'r'},
    {"NotifyOwnerOnVictimDeath","false",148,'b'},{"NotifyObserversOnPreDamageEffectPosition","false",149,'b'},
    {"ForceFXPositionToVictim","false",150,'b'},{"RadiusAffectsBridges","false",151,'b'}
};
const EnumNameValue WeaponSlotType[] = {
    {"NO_WEAPON",5},{"PRIMARY_WEAPON",0},{"QUATERNARY_WEAPON",3},{"QUINARY_WEAPON",4},{"SECONDARY_WEAPON",1},{"TERTIARY_WEAPON",2}
};
const EnumNameValue VeterancyLevel[] = {{"ELITE",2},{"HEROIC",3},{"INVALID",-1},{"REGULAR",0},{"VETERAN",1}};
void ValidateReference(const WeaponNuggetInput& input) {
    if(input.hasSpawnTemplate || input.hasTargetFilter || input.hasDamageFilter || !input.damageScalars.empty())
        throw std::invalid_argument("Damage-only child on reference nugget");
    if(input.typeId!=ProjectileId && (input.hasAttackOffset || input.hasBarrel || !input.veterans.empty()))
        throw std::invalid_argument("Projectile-only child on other nugget");
    for(const auto& attr:input.attributes) {
        bool known=attr.first=="PartitionFilterTestType" || attr.first=="Radius" || attr.first=="ForbiddenTargetObjectStatus" || attr.first=="ForbiddenTargetModelCondition";
        if(input.typeId==ProjectileId && (attr.first=="WarheadTemplate" || attr.first=="ProjectileTemplate" || attr.first=="WeaponLaunchBoneSlotOverride")) known=true;
        if(input.typeId==ScatterProjectileId && (attr.first=="ScatterMin" || attr.first=="ScatterMax")) known=true;
        if((input.typeId==OclId || input.typeId==SeedId) && (attr.first=="WeaponOCL" || attr.first=="TargetAsPrimaryObject")) known=true;
        if(input.typeId==SeedId && (attr.first=="FieldAmount" || attr.first=="SpawnedInFieldBonus")) known=true;
        if(!known) throw std::invalid_argument("Unsupported reference nugget attribute: "+attr.first);
    }
    for(const auto& attr:input.attackOffset) if(attr.first!="x" && attr.first!="y" && attr.first!="z") throw std::invalid_argument("Unsupported AttackOffset attribute");
    for(const auto& veteran:input.veterans) for(const auto& attr:veteran.attributes)
        if(attr.first!="VeterancyLevel" && attr.first!="ProjectileTemplate") throw std::invalid_argument("Unsupported VeterancyProjectiles attribute");
}
void MarshalReferenceNugget(const WeaponNuggetInput& src,BaseNugget* base,Relo::Tracker& data) {
    if(src.typeId==ScatterProjectileId) {
        auto object=reinterpret_cast<ScatterProjectileNugget*>(base);
        object->min=ParseFloat(Text(src,"ScatterMin","0"));object->max=ParseFloat(Text(src,"ScatterMax","0"));
    } else if(src.typeId==OclId || src.typeId==SeedId) {
        if(src.typeId==SeedId) {auto seed=reinterpret_cast<SeedNugget*>(base);seed->amount=ParseFloat(Text(src,"FieldAmount","0"));seed->bonus=ParseFloat(Text(src,"SpawnedInFieldBonus","0"));}
        auto object=reinterpret_cast<OclNugget*>(base);
        if(src.attributes.count("WeaponOCL")) MarshalReference(data,&object->ocl,Text(src,"WeaponOCL",""));
        object->target=ParseBoolean(Text(src,"TargetAsPrimaryObject","false"));
    } else {
        auto object=reinterpret_cast<ProjectileNugget*>(base);
        if(src.attributes.count("WarheadTemplate")) MarshalReference(data,&object->warhead,Text(src,"WarheadTemplate",""));
        if(src.attributes.count("ProjectileTemplate")) MarshalReference(data,&object->projectile,Text(src,"ProjectileTemplate",""));
        object->slot=Enum(Text(src,"WeaponLaunchBoneSlotOverride","NO_WEAPON"),WeaponSlotType,5);
        if(src.hasAttackOffset) {
            data.Push(reinterpret_cast<void**>(&object->offset),12,1);
            const char* names[]={"x","y","z"};
            for(unsigned j=0;j<3;++j) {auto it=src.attackOffset.find(names[j]);object->offset[j]=ParseFloat(it==src.attackOffset.end()?"0.0":it->second);}
            data.Pop();
        }
        object->veterans.count=static_cast<U32>(src.veterans.size());
        if(object->veterans.count) {
            data.Push(reinterpret_cast<void**>(&object->veterans.values),8,object->veterans.count);
            for(U32 j=0;j<object->veterans.count;++j) {
                const auto& attrs=src.veterans[j].attributes;auto& dst=object->veterans.values[j];
                auto level=attrs.find("VeterancyLevel");if(level!=attrs.end()) dst.level=Enum(level->second,VeterancyLevel,2);
                auto ref=attrs.find("ProjectileTemplate");if(ref!=attrs.end()) MarshalReference(data,&dst.projectile,ref->second);
            }
            data.Pop();
        }
        if(src.hasBarrel) {data.Push(reinterpret_cast<void**>(&object->barrel),4,1);*object->barrel=ParseSigned(src.barrel);data.Pop();}
    }
    MarshalBase(src,*base,data);base->type=src.typeId;
}
void TokenReferenceNugget(BaseNugget* base,void** table,Relo::Tracker& tokens,bool oclParent=false) {
    if(base->type==SeedId && !oclParent) {
        auto t=static_cast<Procedure*>(tokens.Push(table,12,4));
        t[0].name=1;t[0].type=OclId;TokenReferenceNugget(base,&t[0].offset,tokens,true);
        t[1].name=2531751936u;t[1].type=1683514611u;Member(base,48,t[1],tokens);
        t[2].name=644933238u;t[2].type=1683514611u;Member(base,52,t[2],tokens);tokens.Pop();return;
    }
    bool projectile=base->type==ProjectileId,scatter=base->type==ScatterProjectileId;
    auto t=static_cast<Procedure*>(tokens.Push(table,12,projectile?8:4));
    t[0].name=1;t[0].type=3351432196u;TokenBase(*base,&t[0].offset,tokens);
    if(!projectile) {
        t[1].name=scatter?1711669554u:3121636592u;t[1].type=scatter?1683514611u:1122298321u;
        t[2].name=scatter?3876912366u:2837807479u;t[2].type=scatter?1683514611u:2011428087u;
        Member(base,40,t[1],tokens);Member(base,44,t[2],tokens);
    } else {
        const U32 names[]={4053034122u,3441092437u,3557541540u,4234379302u,3738739198u,2647151577u};
        const U32 types[]={3494305058u,3215691893u,407422281u,691437687u,1099265352u,602296504u};
        for(unsigned j=0;j<6;++j) {t[j+1].name=names[j];t[j+1].type=types[j];}
        Member(base,40,t[1],tokens);Member(base,44,t[2],tokens);Member(base,48,t[3],tokens);
        auto object=reinterpret_cast<ProjectileNugget*>(base);
        if(object->offset) tokens.BookmarkMemberOffset(object->offset,&t[4].offset);
        auto entries=static_cast<Procedure*>(tokens.Push(&t[5].offset,12,object->veterans.count+1));
        entries[0].name=1820487795u;entries[0].type=1564774360u;Member(&object->veterans,0,entries[0],tokens);
        for(U32 j=0;j<object->veterans.count;++j) {
            entries[j+1].name=2974916843u;
            auto item=static_cast<Procedure*>(tokens.Push(&entries[j+1].offset,12,3));
            item[0].name=4221549935u;item[0].type=4221549935u;Member(object->veterans.values+j,0,item[0],tokens);
            item[1].name=3441092437u;item[1].type=3215691893u;Member(object->veterans.values+j,4,item[1],tokens);tokens.Pop();
        }
        tokens.Pop();
        if(object->barrel) tokens.BookmarkMemberOffset(object->barrel,&t[6].offset);
    }
    tokens.Pop();
}
void EndianReferenceNugget(BaseNugget* base) {
    if(base->type==SeedId) {auto seed=reinterpret_cast<SeedNugget*>(base);Swap(&seed->amount);Swap(&seed->bonus);}
    if(base->type==ScatterProjectileId) {auto object=reinterpret_cast<ScatterProjectileNugget*>(base);Swap(&object->min);Swap(&object->max);}
    if(base->type==ProjectileId) {
        auto object=reinterpret_cast<ProjectileNugget*>(base);Swap(&object->slot);
        if(object->offset) for(unsigned j=0;j<3;++j) Swap(object->offset+j);
        for(U32 j=0;j<object->veterans.count;++j) Swap(&object->veterans.values[j].level);
        Swap(&object->veterans.count);if(object->barrel) Swap(object->barrel);
    }
    EndianBase(*base);
}
void Validate(const WeaponNuggetInput& input) {
    if(ReferenceFamily(input.typeId)) {ValidateReference(input);return;}
    if(input.hasAttackOffset || input.hasBarrel || !input.veterans.empty()) throw std::invalid_argument("Projectile child on Damage family");
    if(input.typeId!=DamageId && input.typeId!=DotId && input.typeId!=GrapplingId && input.typeId!=LeechPercentId && input.typeId!=LeechHealthId && input.typeId!=LineId && input.typeId!=SpawnId && input.typeId!=FireId && input.typeId!=ContainedId)
        throw std::invalid_argument("Unrecovered WeaponEffectNugget TypeId");
    if(input.hasDamageFilter && input.typeId!=ContainedId) throw std::invalid_argument("DamageObjectFilter belongs to DamageContainedNugget");
    if(input.hasSpawnTemplate && input.typeId!=SpawnId) throw std::invalid_argument("SpawnTemplate belongs to DamageAndSpawnNugget");
    if(input.hasTargetFilter && input.typeId!=FireId) throw std::invalid_argument("Filter belongs to FireOnObjectsNugget");
    for(const auto& attr:input.attributes) {
        bool known=attr.first=="PartitionFilterTestType" || attr.first=="Radius" || attr.first=="ForbiddenTargetObjectStatus" || attr.first=="ForbiddenTargetModelCondition" || attr.first=="InvalidTargetStatus";
        for(const auto& field:damageFields) if(attr.first==field.name) known=true;
        if(input.typeId==DotId && (attr.first=="DamageInterval" || attr.first=="DamageDuration" || attr.first=="RemoveIfHealed")) known=true;
        if((input.typeId==LeechPercentId && attr.first=="PercentMaxHealthToTake") || (input.typeId==LeechHealthId && attr.first=="PercentDamageToHeal")) known=true;
        if(input.typeId==LineId && (attr.first=="OffsetAngle" || attr.first=="LineWidth" || attr.first=="LineLengthLeadIn" || attr.first=="LineLengthLeadOut" || attr.first=="UseDynamicLineLength" || attr.first=="OverShootDistance")) known=true;
        if(input.typeId==SpawnId && (attr.first=="SpawnProbability" || attr.first=="SpawnedModelConditionFlags")) known=true;
        if(input.typeId==FireId && (attr.first=="Weapon" || attr.first=="FireOnVictimObject" || attr.first=="VictimMustBeAlive")) known=true;
        if(input.typeId==ContainedId && (attr.first=="MaxUnitsToDamage" || attr.first=="WindowBlastFX")) known=true;
        if(!known) throw std::invalid_argument("Unsupported damage nugget attribute: "+attr.first);
    }
}
void MarshalDamage(const WeaponNuggetInput& input,DamageNugget& object,Relo::Tracker& data) {
    for(const auto& field:damageFields) {
        if(!field.fallback && !input.attributes.count(field.name)) continue;
        auto text=Text(input,field.name,field.fallback ? field.fallback : "");
        auto at=reinterpret_cast<unsigned char*>(&object)+field.offset;
        if(field.kind=='b') { *at=ParseBoolean(text);continue; }
        if(field.kind=='r') { MarshalReference(data,at,text);continue; }
        if(field.kind=='e') {
            U32 word=field.offset==76 ? Enum(text,DamageType,38) : field.offset==80 ? Enum(text,DeathType,33) : field.offset==84 ? Enum(text,DamageFXType,35) : Enum(text,DamageSubType,1);
            std::memcpy(at,&word,4);continue;
        }
        float value=field.kind=='a' ? InitializeAngle(text.c_str()) : field.kind=='t' ? InitializeTime(text.c_str()) : ParseFloat(text);
        std::memcpy(at,&value,4);
    }
    MarshalObjectStatus(Text(input,"InvalidTargetStatus","INSIDE_GARRISON UNATTACKABLE UNDER_IRON_CURTAIN"),object.invalidStatus);
    object.scalars.count=static_cast<U32>(input.damageScalars.size());
    if(object.scalars.count) {
        data.Push(reinterpret_cast<void**>(&object.scalars.values),124,object.scalars.count);
        for(U32 i=0;i<object.scalars.count;++i) {
            const auto& src=input.damageScalars[i];auto& dst=object.scalars.values[i];
            dst.scalar=ParsePercentage(src.scalar);
            if(src.hasFilter) MarshalObjectFilter(src.filter,&dst.filter,data);
            MarshalWords(src.requiredUpgrades,dst.required,data);
        }
        data.Pop();
    }
    // Original derived Marshal runs first; the common base allocates last.
    MarshalBase(input,object.base,data);
}
void TokenScalars(ScalarList& list,void** table,Relo::Tracker& tokens) {
    auto entries=static_cast<Procedure*>(tokens.Push(table,12,list.count+1));
    entries[0].name=1820487795u;entries[0].type=1564774360u;Member(&list,0,entries[0],tokens);
    for(U32 i=0;i<list.count;++i) {
        entries[i+1].name=2974916843u;
        auto t=static_cast<Procedure*>(tokens.Push(&entries[i+1].offset,12,4));SetKeys(t,ScalarInfoTokens,4);
        Member(list.values+i,0,t[0],tokens);
        TokenizeObjectFilter(&list.values[i].filter,&t[1].offset,tokens);
        TokenWords(list.values[i].required,&t[2].offset,tokens);tokens.Pop();
    }
    tokens.Pop();
}
void TokenDamage(DamageNugget& object,void** table,Relo::Tracker& tokens) {
    auto t=static_cast<Procedure*>(tokens.Push(table,12,29));SetKeys(t,DamageNuggetTypeTokens,29);
    TokenBase(object.base,&t[0].offset,tokens);
    const unsigned offsets[]={40,44,48,52,144,56,60,145,64,68,72,76,80,84,88,146,92,147,96,100,104,148,149,150,151,108};
    for(unsigned i=0;i<26;++i) Member(&object,offsets[i],t[i+1],tokens);
    TokenScalars(object.scalars,&t[27].offset,tokens);tokens.Pop();
}
void EndianDamage(DamageNugget& object) {
    // Reference slots at 100/104 are rewritten by Tracker rather than byte-swapped.
    for(unsigned at=40;at<100;at+=4) Swap(reinterpret_cast<unsigned char*>(&object)+at);
    for(unsigned i=0;i<7;++i) Swap(object.invalidStatus+i);
    for(U32 i=0;i<object.scalars.count;++i) {
        auto& scalar=object.scalars.values[i];Swap(&scalar.scalar);EndianObjectFilter(&scalar.filter);EndianWords(scalar.required);
    }
    Swap(&object.scalars.count);EndianBase(object.base);
}
}
void MarshalWeaponNuggets(const std::vector<WeaponNuggetInput>& input,WeaponNuggetList* list,Relo::Tracker& data) {
    for(const auto& value:input) Validate(value);
    list->count=static_cast<U32>(input.size());
    if(!list->count) return;
    data.Push(reinterpret_cast<void**>(&list->elements),4,list->count);
    for(U32 i=0;i<list->count;++i) {
        const auto& src=input[i];bool dot=src.typeId==DotId;bool percent=src.typeId==LeechPercentId || src.typeId==LeechHealthId;bool line=src.typeId==LineId;
        if(ReferenceFamily(src.typeId)) {
            auto base=static_cast<BaseNugget*>(data.Push(&list->elements[i],src.typeId==ProjectileId?68:src.typeId==SeedId?56:48,1));
            MarshalReferenceNugget(src,base,data);data.Pop();continue;
        }
        auto object=static_cast<DamageNugget*>(data.Push(&list->elements[i],src.typeId==ContainedId?272:src.typeId==SpawnId?220:src.typeId==FireId?164:dot?164:percent?156:line?176:152,1));
        if(src.typeId==ContainedId) {
            auto derived=reinterpret_cast<ContainedNugget*>(object);derived->maxUnits=static_cast<U32>(ParseSigned(Text(src,"MaxUnitsToDamage","100")));
            if(src.attributes.count("WindowBlastFX")) MarshalReference(data,&derived->windowFx,Text(src,"WindowBlastFX",""));
            if(src.hasDamageFilter) MarshalObjectFilter(src.damageFilter,&derived->filter,data);
        }
        if(src.typeId==SpawnId) {
            auto derived=reinterpret_cast<SpawnNugget*>(object);derived->probability=ParseFloat(Text(src,"SpawnProbability","1.0"));
            if(src.attributes.count("SpawnedModelConditionFlags")) MarshalModelCondition(Text(src,"SpawnedModelConditionFlags",""),derived->model);
            if(src.hasSpawnTemplate) derived->objectId=HashAssetId(src.spawnTemplate);
        }
        if(src.typeId==FireId) {
            auto derived=reinterpret_cast<FireNugget*>(object);
            if(src.attributes.count("Weapon")) MarshalReference(data,&derived->weapon,Text(src,"Weapon",""));
            derived->onVictim=ParseBoolean(Text(src,"FireOnVictimObject","false"));
            derived->mustBeAlive=ParseBoolean(Text(src,"VictimMustBeAlive","true"));
            if(src.hasTargetFilter) { data.Push(reinterpret_cast<void**>(&derived->filter),112,1);MarshalObjectFilter(src.targetFilter,derived->filter,data);data.Pop(); }
        }
        if(dot) {
            auto derived=reinterpret_cast<DotNugget*>(object);
            if(src.attributes.count("DamageInterval")) derived->interval=InitializeTime(Text(src,"DamageInterval","").c_str());
            if(src.attributes.count("DamageDuration")) derived->duration=InitializeTime(Text(src,"DamageDuration","").c_str());
            derived->removeIfHealed=ParseBoolean(Text(src,"RemoveIfHealed","true"));
        }
        if(line) {
            auto derived=reinterpret_cast<LineNugget*>(object);
            derived->angle=InitializeAngle(Text(src,"OffsetAngle","0d").c_str());
            derived->width=ParseFloat(Text(src,"LineWidth","0"));
            derived->leadIn=ParseFloat(Text(src,"LineLengthLeadIn","0"));
            derived->leadOut=ParseFloat(Text(src,"LineLengthLeadOut","0"));
            derived->overshoot=ParseFloat(Text(src,"OverShootDistance","0"));
            derived->dynamicLength=ParseBoolean(Text(src,"UseDynamicLineLength","false"));
        }
        if(percent) reinterpret_cast<PercentNugget*>(object)->percentage=ParsePercentage(Text(src,src.typeId==LeechPercentId?"PercentMaxHealthToTake":"PercentDamageToHeal","100%"));
        MarshalDamage(src,*object,data);object->base.type=src.typeId;data.Pop();
    }
    data.Pop();
}
void TokenizeWeaponNuggets(WeaponNuggetList* list,void** table,Relo::Tracker& tokens) {
    auto entries=static_cast<Procedure*>(tokens.Push(table,12,list->count+1));
    entries[0].name=1820487795u;entries[0].type=1564774360u;Member(list,0,entries[0],tokens);
    for(U32 i=0;i<list->count;++i) {
        entries[i+1].name=2974916843u;
        auto object=static_cast<DamageNugget*>(list->elements[i]);
        auto poly=static_cast<Procedure*>(tokens.Push(&entries[i+1].offset,12,1));poly->name=2;poly->type=object->base.type;
        if(ReferenceFamily(object->base.type)) TokenReferenceNugget(reinterpret_cast<BaseNugget*>(object),&poly->offset,tokens);
        else if(object->base.type==DamageId) TokenDamage(*object,&poly->offset,tokens);
        else {
            bool dot=object->base.type==DotId;bool percent=object->base.type==LeechPercentId || object->base.type==LeechHealthId;bool line=object->base.type==LineId;
            auto derived=static_cast<Procedure*>(tokens.Push(&poly->offset,12,object->base.type==ContainedId?5:object->base.type==SpawnId?5:object->base.type==FireId?6:dot?5:percent?3:line?8:2));
            derived[0].name=1;derived[0].type=DamageId;TokenDamage(*object,&derived[0].offset,tokens);
            if(object->base.type==ContainedId) {
                derived[1].name=1258282188u;derived[1].type=602296504u;Member(object,152,derived[1],tokens);
                derived[2].name=899645757u;derived[2].type=219975299u;Member(object,156,derived[2],tokens);
                derived[3].name=2805754577u;derived[3].type=3832456114u;
                TokenizeObjectFilter(&reinterpret_cast<ContainedNugget*>(object)->filter,&derived[3].offset,tokens);
            }
            if(object->base.type==SpawnId) {
                const U32 names[]={1998637738u,3234314496u,1528957511u};
                const U32 types[]={1683514611u,1742454729u,344905565u};
                const unsigned offsets[]={152,156,216};
                for(unsigned j=0;j<3;++j) {derived[j+1].name=names[j];derived[j+1].type=types[j];Member(object,offsets[j],derived[j+1],tokens);}
            }
            if(object->base.type==FireId) {
                const U32 names[]={63188358u,263143502u,3212784623u,2239343654u};
                const U32 types[]={3494305058u,2011428087u,2011428087u,3832456114u};
                for(unsigned j=0;j<4;++j) {derived[j+1].name=names[j];derived[j+1].type=types[j];}
                Member(object,152,derived[1],tokens);Member(object,160,derived[2],tokens);Member(object,161,derived[3],tokens);
                auto fire=reinterpret_cast<FireNugget*>(object);if(fire->filter) TokenizeObjectFilter(fire->filter,&derived[4].offset,tokens);
            }
            if(dot) {
                const U32 names[]={1244447716u,1828804499u,1329948426u};
                for(unsigned j=0;j<3;++j) { derived[j+1].name=names[j];derived[j+1].type=j==2?2011428087u:2575722984u;Member(object,152+4*j,derived[j+1],tokens); }
            }
            if(line) {
                const U32 names[]={4168427962u,1805140803u,1743148362u,564018997u,4070607831u,2533801744u};
                const unsigned offsets[]={152,156,160,164,172,168};
                for(unsigned j=0;j<6;++j) {
                    derived[j+1].name=names[j];derived[j+1].type=j==0?1310124621u:j==4?2011428087u:1683514611u;
                    Member(object,offsets[j],derived[j+1],tokens);
                }
            }
            if(percent) {
                derived[1].name=object->base.type==LeechPercentId?1407231226u:3760556672u;derived[1].type=1683514611u;
                Member(object,152,derived[1],tokens);
            }
            tokens.Pop();
        }
        tokens.Pop();
    }
    tokens.Pop();
}
void EndianWeaponNuggets(WeaponNuggetList* list) {
    for(U32 i=0;i<list->count;++i) {
        auto object=static_cast<DamageNugget*>(list->elements[i]);
        if(ReferenceFamily(object->base.type)) {EndianReferenceNugget(reinterpret_cast<BaseNugget*>(object));continue;}
        if(object->base.type==DotId) { auto dot=reinterpret_cast<DotNugget*>(object);Swap(&dot->interval);Swap(&dot->duration); }
        if(object->base.type==LeechPercentId || object->base.type==LeechHealthId) Swap(&reinterpret_cast<PercentNugget*>(object)->percentage);
        if(object->base.type==LineId) for(unsigned at=152;at<=168;at+=4) Swap(reinterpret_cast<unsigned char*>(object)+at);
        if(object->base.type==SpawnId) {
            auto spawn=reinterpret_cast<SpawnNugget*>(object);Swap(&spawn->probability);
            for(unsigned j=0;j<15;++j) Swap(spawn->model+j);Swap(&spawn->objectId);
        }
        if(object->base.type==FireId) {auto fire=reinterpret_cast<FireNugget*>(object);if(fire->filter) EndianObjectFilter(fire->filter);}
        if(object->base.type==ContainedId) {auto contained=reinterpret_cast<ContainedNugget*>(object);Swap(&contained->maxUnits);EndianObjectFilter(&contained->filter);}
        EndianDamage(*object);
    }
    Swap(&list->count);
}
}}
#ifdef _MANAGED
#pragma managed(pop)
#endif

