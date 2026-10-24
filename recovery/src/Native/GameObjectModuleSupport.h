// Included by GameObject.cpp; definitions remain in its namespace and original order.
unsigned GameObjectSoundSelectorKind(const std::string& typeId) {
    auto choice=FindModule(ParseUnsigned(typeId));return choice && choice->kind>=13 && choice->kind<=16?choice->kind:0;
}
bool GameObjectModuleAttributeSupported(const std::string& typeId,const std::string& attribute) {
    auto choice=FindModule(ParseUnsigned(typeId));if(!choice || !choice->kind) return false;
    if(choice->kind>=165 && choice->kind<=187) {
        for(const auto& fields:{specialPowerFieldSets[0],specialDerivedFieldSets[choice->kind-165]})
            for(unsigned i=0;i<fields.count;++i) if(attribute==fields.fields[i].name) return true;
        return false;
    }
    if(choice->kind>=162 && choice->kind<=164) {
        for(const auto& fields:{specialPowerFieldSets[0],specialPowerFieldSets[choice->kind-162]})
            for(unsigned i=0;i<fields.count;++i) if(attribute==fields.fields[i].name) return true;
        return false;
    }
    if(choice->kind>=159 && choice->kind<=161) {const auto& fields=motionCleanupFieldSets[choice->kind-159];for(unsigned i=0;i<fields.count;++i) if(attribute==fields.fields[i].name) return true;return false;}
    if(choice->kind>=156 && choice->kind<=158) {const auto& fields=modelConditionFieldSets[choice->kind-156];for(unsigned i=0;i<fields.count;++i) if(attribute==fields.fields[i].name) return true;return false;}
    if(choice->kind>=154 && choice->kind<=155) {const auto& fields=stateEventFieldSets[choice->kind-154];for(unsigned i=0;i<fields.count;++i) if(attribute==fields.fields[i].name) return true;return false;}
    if(choice->kind>=152 && choice->kind<=153) {
        const auto& fields=resourceAlliesFieldSets[choice->kind-152];for(unsigned i=0;i<fields.count;++i) if(attribute==fields.fields[i].name) return true;
        if(choice->kind==152) for(unsigned i=0;i<visionFieldSets[0].count;++i) if(attribute==visionFieldSets[0].fields[i].name) return true;
        return false;
    }
    if(choice->kind>=150 && choice->kind<=151) {const auto& fields=wallEncloseFieldSets[choice->kind-150];for(unsigned i=0;i<fields.count;++i) if(attribute==fields.fields[i].name) return true;return false;}
    if(choice->kind>=147 && choice->kind<=149) {const auto& fields=tiberiumFieldSets[choice->kind-147];for(unsigned i=0;i<fields.count;++i) if(attribute==fields.fields[i].name) return true;return false;}
    if(choice->kind>=145 && choice->kind<=146) {
        const auto& fields=stealthVisionFieldSets[choice->kind-145];for(unsigned i=0;i<fields.count;++i) if(attribute==fields.fields[i].name) return true;
        if(choice->kind==145) for(unsigned i=0;i<upgradeFieldSets[0].count;++i) if(attribute==upgradeFieldSets[0].fields[i].name) return true;
        if(choice->kind==146) for(const auto& name:stealthVisionVoiceNames) if(attribute==name) return true;
        return false;
    }
    if(choice->kind>=143 && choice->kind<=144) {const auto& fields=statusEffectsFieldSets[choice->kind-143];for(unsigned i=0;i<fields.count;++i) if(attribute==fields.fields[i].name) return true;return false;}
    if(choice->kind>=140 && choice->kind<=142) {const auto& fields=combatResponseFieldSets[choice->kind-140];for(unsigned i=0;i<fields.count;++i) if(attribute==fields.fields[i].name) return true;return false;}
    if(choice->kind>=137 && choice->kind<=139) {const auto& fields=structureWallFieldSets[choice->kind-137];for(unsigned i=0;i<fields.count;++i) if(attribute==fields.fields[i].name) return true;return false;}
    if(choice->kind>=133 && choice->kind<=136) {const auto& fields=economyPowerFieldSets[choice->kind-133];for(unsigned i=0;i<fields.count;++i) if(attribute==fields.fields[i].name) return true;return false;}
    if(choice->kind>=128 && choice->kind<=132) {const auto& fields=choice->kind<=129?queueParkingFieldSets[choice->kind-128]:buildingStructureFieldSets[choice->kind-130];for(unsigned i=0;i<fields.count;++i) if(attribute==fields.fields[i].name) return true;return false;}
    if(choice->kind==127) {const auto& fields=crateWatcherFieldSets[0];for(unsigned i=0;i<fields.count;++i) if(attribute==fields.fields[i].name) return true;return false;}
    if(choice->kind>=123 && choice->kind<=125) {
        const auto& own=spawnFieldSets[choice->kind-123];for(unsigned i=0;i<own.count;++i) if(attribute==own.fields[i].name) return true;
        if(choice->kind==123) for(unsigned i=0;i<upgradeFieldSets[0].count;++i) if(attribute==upgradeFieldSets[0].fields[i].name) return true;
        return false;
    }
    if(choice->kind>=119 && choice->kind<=122) {const auto& set=assistFieldSets[choice->kind-119];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind>=112 && choice->kind<=118) {
        unsigned group=choice->kind-112;const auto& own=economyFieldSets[group];for(unsigned i=0;i<own.count;++i) if(attribute==own.fields[i].name) return true;
        if(group==3 || group==4) for(unsigned i=0;i<economyFieldSets[0].count;++i) if(attribute==economyFieldSets[0].fields[i].name) return true;
        return false;
    }
    if(choice->kind>=104 && choice->kind<=111) {
        for(const auto& set:{aiUpdateFieldSets[choice->kind-104],aiUpdateFieldSets[0]}) for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;
        return false;
    }
    if(choice->kind>=96 && choice->kind<=103) {
        unsigned group=choice->kind-96;const auto& own=crateFieldSets[group];for(unsigned i=0;i<own.count;++i) if(attribute==own.fields[i].name) return true;
        if(group>0 && group<7) for(unsigned i=0;i<crateFieldSets[0].count;++i) if(attribute==crateFieldSets[0].fields[i].name) return true;return false;
    }
    if(choice->kind>=92 && choice->kind<=95) {
        const auto& own=healingFieldSets[choice->kind-92];for(unsigned i=0;i<own.count;++i) if(attribute==own.fields[i].name) return true;
        const auto& base=choice->kind==92?upgradeFieldSets[0]:healingFieldSets[4];if(choice->kind==92 || choice->kind==95) for(unsigned i=0;i<base.count;++i) if(attribute==base.fields[i].name) return true;return false;
    }
    if(choice->kind==91) return false;
    if(choice->kind>=87) {const auto& set=reactiveFieldSets[choice->kind-87];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;if(choice->kind==88 || choice->kind==90) for(unsigned i=0;i<upgradeFieldSets[0].count;++i) if(attribute==upgradeFieldSets[0].fields[i].name) return true;return false;}
    if(choice->kind>=84) {const auto& set=flightFieldSets[choice->kind-84];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;if(choice->kind==86) for(unsigned i=0;i<flightFieldSets[1].count;++i) if(attribute==flightFieldSets[1].fields[i].name) return true;return false;}
    if(choice->kind>=80) {const auto& set=motionFieldSets[choice->kind-80];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind>=77) {const auto& set=beamFieldSets[choice->kind-77];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;if(choice->kind==78) for(unsigned i=0;i<beamFieldSets[0].count;++i) if(attribute==beamFieldSets[0].fields[i].name) return true;return false;}
    if(choice->kind>=69) {const auto& set=weaponFieldSets[choice->kind-69];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind>=61) {const auto& set=visionFieldSets[choice->kind-61];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind==60) {for(unsigned i=0;i<receiverFieldSet.count;++i) if(attribute==receiverFieldSet.fields[i].name) return true;for(unsigned i=0;i<upgradeFieldSets[0].count;++i) if(attribute==upgradeFieldSets[0].fields[i].name) return true;return false;}
    if(choice->kind==59) {for(unsigned i=0;i<slavedFieldSet.count;++i) if(attribute==slavedFieldSet.fields[i].name) return true;return false;}
    if(choice->kind>=55) {const auto& set=createFieldSets[choice->kind-55];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind>=52) {const auto& set=productionFieldSets[choice->kind-52];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind>=30) {const auto& own=choice->kind>=41?remainingFieldSets[choice->kind-41]:upgradeFieldSets[choice->kind-30];for(unsigned i=0;i<own.count;++i) if(attribute==own.fields[i].name) return true;for(unsigned i=0;i<upgradeFieldSets[0].count;++i) if(attribute==upgradeFieldSets[0].fields[i].name) return true;return false;}
    if(choice->kind>=23) {const auto& set=dieFieldSets[choice->kind-23];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind>=17) {const auto& set=lifecycleFieldSets[choice->kind-17];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind>=13) return choice->kind==13 && (attribute=="Chance" || attribute=="VoicePriorityOverride" || attribute=="RerollOnEveryFrame");
    const auto& set=moduleFieldSets[choice->kind];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;
    if(choice->kind==2) for(const auto& f:ActiveBodyModuleDataModuleFields) if(attribute==f.name) return true;
    return false;
}
bool GameObjectModuleNestedAttributeSupported(const std::string& typeId,const std::string& name,const std::string& attribute) {
    auto choice=FindModule(ParseUnsigned(typeId));if(!choice) return false;
    if(choice->kind>=30 && choice->kind<=51 && name=="CustomAnimAndDuration") {const auto& set=upgradeFieldSets[11];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind>=23 && choice->kind<=29 && name=="DieMuxData") {const auto& set=dieFieldSets[7];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind==21 && name=="ObjectStatusValidation") return attribute=="ForbiddenStatus" || attribute=="RequiredStatus";
    unsigned fields=choice->kind==9 && name=="Sound"?11:choice->kind==10 && name=="ModelConditionSound"?12:0;
    const auto& set=moduleFieldSets[fields];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;
    return false;
}
bool GameObjectModuleNestedSupported(const std::string& typeId,const std::string& name) {
    auto choice=FindModule(ParseUnsigned(typeId));return choice && ((choice->kind>=1 && choice->kind<=2 && name=="DamageRangeModifier") || (choice->kind==8 && name=="Key") || (choice->kind==9 && name=="Sound") || (choice->kind==10 && name=="ModelConditionSound") || (choice->kind==21 && name=="ObjectStatusValidation") || (choice->kind>=23 && choice->kind<=29 && name=="DieMuxData") || (choice->kind==27 && (name=="UpgradeRequired" || name=="UpgradeForbidden")) || (choice->kind==29 && name=="ObjectFilter") || (choice->kind>=30 && choice->kind<=51 && (name=="CustomAnimAndDuration" || name=="TriggeredBy" || name=="ConflictsWith")));
}
unsigned GameObjectRecursiveModuleKind(const std::string& typeId) {auto c=FindModule(ParseUnsigned(typeId));return c && c->kind>=41?c->kind:0;}
unsigned GameObjectModuleNodeKind(unsigned kind,const std::string& path) {
    if(kind>=165 && kind<=187) {
        const auto& children=specialDerivedChildren[kind-165];
        if(children.filter) {
            if(path==children.filter) return 1;
            if(path==std::string(children.filter)+"/IncludeThing" || path==std::string(children.filter)+"/ExcludeThing") return 3;
        }
        if(children.strongList && path==children.strongList) return 3;
        return GameObjectModuleNodeKind(162,path);
    }
    if(kind>=162 && kind<=164) {
        if(path=="InitiateSound" || path=="AttributeModifierAffects" || path=="RequirementsFilterMP" || path=="RequirementsFilterStrategic") return 1;
        for(const char* filter:{"AttributeModifierAffects","RequirementsFilterMP","RequirementsFilterStrategic"})
            if(path==std::string(filter)+"/IncludeThing" || path==std::string(filter)+"/ExcludeThing") return 3;
        return 0;
    }
    if(kind>=159 && kind<=161) {
        if(kind==160) {if(path=="ObjectFilter") return 1;if(path=="ObjectFilter/IncludeThing" || path=="ObjectFilter/ExcludeThing") return 3;}
        if(kind==161) {if(path=="ObjectStatusValidation" || path=="KeepAliveObjectFilter") return 1;if(path=="KeepAliveObjectFilter/IncludeThing" || path=="KeepAliveObjectFilter/ExcludeThing") return 3;}
        return 0;
    }
    if(kind>=156 && kind<=158) return 0;
    if(kind>=154 && kind<=155) return 0;
    if(kind>=152 && kind<=153) {
        if(kind==153) {if(path=="ObjectFilter") return 1;if(path=="ObjectFilter/IncludeThing" || path=="ObjectFilter/ExcludeThing") return 3;}
        return 0;
    }
    if(kind>=150 && kind<=151) {
        if(kind==150 && path=="SegmentPattern") return 3;
        if(kind==151) {if(path=="Filter") return 1;if(path=="Filter/IncludeThing" || path=="Filter/ExcludeThing") return 3;}
        return 0;
    }
    if(kind>=147 && kind<=149) {
        if(kind==147 && path=="DieMuxData") return 1;
        if(kind==148 && path=="RadarColor") return 1;
        if(kind==149) {if(path=="DamageFilter" || path=="HealFilter") return 1;if(path=="DamageFilter/IncludeThing" || path=="DamageFilter/ExcludeThing" || path=="HealFilter/IncludeThing" || path=="HealFilter/ExcludeThing") return 3;}
        return 0;
    }
    if(kind>=145 && kind<=146) {
        if(kind==145) {if(path=="CustomAnimAndDuration") return 1;if(path=="TriggeredBy" || path=="ConflictsWith") return 3;}
        if(kind==146) {if(path=="RequiresNearbyObjectFilter") return 1;if(path=="RequiresNearbyObjectFilter/IncludeThing" || path=="RequiresNearbyObjectFilter/ExcludeThing") return 3;}
        return 0;
    }
    if(kind>=143 && kind<=144) return kind==143 && (path=="StartColor" || path=="EndColor")?1:0;
    if(kind>=140 && kind<=142) {
        if(kind==141) {if(path=="AcceptableFriends") return 1;if(path=="AcceptableFriends/IncludeThing" || path=="AcceptableFriends/ExcludeThing") return 3;}
        if(kind==142 && path=="ObjectStatusValidation") return 1;
        return 0;
    }
    if(kind>=137 && kind<=139) {
        if(kind==137) {if(path=="ConnectingObjectFilter") return 1;if(path=="ConnectingObjectFilter/IncludeThing" || path=="ConnectingObjectFilter/ExcludeThing") return 3;}
        if(kind==139 && (path=="RestrictedAreaShape" || path=="RestrictedAreaShape/Offset")) return 1;
        return 0;
    }
    if(kind>=133 && kind<=136) return kind==135 && (path=="InitialCaptureBonus" || path=="Deposit" || path=="ResourceCap")?2:0;
    if(kind>=128 && kind<=132) {
        if(kind==129) {if(path=="CanParkHereFilter" || path=="DefaultRallyPointOffset") return 1;if(path=="CanParkHereFilter/IncludeThing" || path=="CanParkHereFilter/ExcludeThing") return 3;}
        return 0;
    }
    if(kind==127) return 0;
    if(kind>=123 && kind<=125) {
        if(kind==123) {if(path=="Die" || path=="CustomAnimAndDuration") return 1;if(path=="SpawnTemplate" || path=="TriggeredBy" || path=="ConflictsWith") return 3;}
        return 0;
    }
    if(kind>=119 && kind<=122) {
        if(kind==119) {
            if(path=="HintObject") return 2;
            if(path=="PlacementRestriction" || path=="SuggestedOffset" || path=="HintObject/ObjectFilter" || path=="PlacementRestriction/RestrictedObjectFilter") return 1;
            if(path=="HintObject/ObjectFilter/IncludeThing" || path=="HintObject/ObjectFilter/ExcludeThing" || path=="PlacementRestriction/RestrictedObjectFilter/IncludeThing" || path=="PlacementRestriction/RestrictedObjectFilter/ExcludeThing") return 3;
        }
        if(kind==120) {if(path=="RequirementObjectFilter") return 1;if(path=="RequirementObjectFilter/IncludeThing" || path=="RequirementObjectFilter/ExcludeThing") return 3;}
        return 0;
    }
    if(kind>=112 && kind<=118) {
        if(kind==112 || kind==115 || kind==116) {
            if(path=="ForVoiceRetreatThisIsASafeHarborToObjectFilter") return 1;
            if(path=="ForVoiceRetreatThisIsASafeHarborToObjectFilter/IncludeThing" || path=="ForVoiceRetreatThisIsASafeHarborToObjectFilter/ExcludeThing") return 3;
        }
        if(kind==114 && path=="ObjectStatusValidation") return 1;
        if(kind==118) {if(path=="RotationSpeed" || path=="UnitSpacing" || path=="Filter") return 1;if(path=="Filter/IncludeThing" || path=="Filter/ExcludeThing") return 3;}
        return 0;
    }
    if(kind>=104 && kind<=111) {
        if(path=="UnitAITargetChooserData" || path=="UnitAITargetChooserData/ObjectVisionRange/Filter") return 1;
        if(path=="UnitAITargetChooserData/ObjectVisionRange") return 2;
        if(path=="UnitAITargetChooserData/ObjectVisionRange/Filter/IncludeThing" || path=="UnitAITargetChooserData/ObjectVisionRange/Filter/ExcludeThing") return 3;
        return 0;
    }
    if(kind>=96 && kind<=103) {
        if(kind==103) {if(path=="CrateList" || path=="BlockingUnits/IncludeThing" || path=="BlockingUnits/ExcludeThing") return 3;if(path=="BlockingUnits") return 1;}return 0;
    }
    if(kind>=92 && kind<=95) {
        if(kind==92) {if(path=="ObjectStatusValidation" || path=="CustomAnimAndDuration") return 1;if(path=="TriggeredBy" || path=="ConflictsWith") return 3;}
        if(kind==93 && (path=="PercentOfBuildCostToRebuildInfo" || path=="SelfAudio")) return 1;
        if(kind==94 && path=="Die") return 1;
        if(kind==95) {if(path=="ObjectFilter") return 1;if(path=="ObjectFilter/IncludeThing" || path=="ObjectFilter/ExcludeThing") return 3;}
        return 0;
    }
    if(kind==91) {
        for(const char* slot:{"WeaponSlotHardpoint","WeaponSlotTurret","WeaponSlotHierarchicalTurret","WeaponSlotContained"}) {
            std::string base=slot;if(path==base || path==base+"/Weapon") return 2;
            if(base=="WeaponSlotTurret" || base=="WeaponSlotHierarchicalTurret") {
                std::string settings=base+"/TurretSettings",chooser=settings+"/TurretAITargetChooserData",vision=chooser+"/ObjectVisionRange";
                if(path==settings || path==chooser || path==vision+"/Filter") return 1;
                if(path==vision) return 2;
                if(path==vision+"/Filter/IncludeThing" || path==vision+"/Filter/ExcludeThing") return 3;
            }
        }return 0;
    }
    if(kind>=87) {if(kind==87) return path=="Data"?2:0;if(kind==88 || kind==90) {if(path=="CustomAnimAndDuration") return 1;if(path=="TriggeredBy" || path=="ConflictsWith") return 3;}if(kind==90) {if(path=="WeaponOffset" || path=="DieMuxData") return 1;if(path=="WeaponFireProbability") return 2;}return 0;}
    if(kind>=84) return 0;
    if(kind>=80) {
        if(kind==80) return path=="Movement"?1:0;
        if(kind==81) {if(path=="Volume") return 1;for(const auto& shape:motionShapeNames) {std::string base=std::string("Volume/")+shape;if(path==base) return 2;if(path==base+"/Translation" || path==base+"/Rotation") return 1;}}
        return 0;
    }
    if(kind>=77) {if(kind==79) return 0;if(path=="EndOffset" || path=="ObjectStatusValidation") return 1;if(path=="LaserEndParticleSystem" || path=="LaserStartParticleSystem") return 3;return 0;}
    if(kind>=69) {if(WeaponValidationOffset(kind) && path=="ObjectStatusValidation") return 1;if(kind==70) {if(path=="FireWeaponNugget") return 2;if(path=="FireWeaponNugget/Offset") return 1;}if(kind==75) {if(path=="Color") return 1;if(path=="EffectInfo") return 2;}return 0;}
    if(kind>=61) {if(kind==67) {if(path=="BroadcastObjectFilter" || path=="ObjectStatusValidation") return 1;if(path=="BroadcastObjectFilter/IncludeThing" || path=="BroadcastObjectFilter/ExcludeThing") return 3;}return 0;}
    if(kind==60) {if(path=="ObjectFilter" || path=="CustomAnimAndDuration") return 1;if(path=="TriggeredBy" || path=="ConflictsWith" || path=="ObjectFilter/IncludeThing" || path=="ObjectFilter/ExcludeThing") return 3;return 0;}
    if(kind==59) return path=="GuardPosOffset"?1:0;
    if(kind>=55) {if(kind==56 && path=="ObjectFilter") return 1;if((kind==56 && path=="Upgrade") || (kind==58 && path=="SpecialPowerTemplate")) return 3;return 0;}
    if(kind>=52) {
        if(kind==52) {if(path=="QuantityModifier" || path=="ProductionModifier" || path=="GiveUpgradeToProduced/UpgradeTranslate") return 2;if(path=="GiveUpgradeToProduced" || path=="ProductionModifier/ModifierFilter") return 1;}
        if((kind==53 || kind==54) && (path=="UnitCreatePoint" || path=="NaturalRallyPoint")) return 1;
        return 0;
    }
    if(path=="CustomAnimAndDuration") return 1;
    if(path=="TriggeredBy" || path=="ConflictsWith") return 3;
    if(kind==41) {if(path=="AvailableSubStructure" || path=="StructureOffset") return 2;if(path=="AvailableSubStructure/UpgradeTrigger" || path=="AvailableSubStructure/ObjectTemplate") return 4;if(path=="StructureOffset/Offset") return 1;}
    if(kind==43 && path=="ReplacementTemplate") return 3;
    if(kind==44) {if(path=="Nugget" || path=="Nugget/UpgradeTexture") return 2;if(path=="Nugget/CustomAnimAndDuration") return 1;}
    if(kind==45 && path=="Offset") return 1;
    if(kind==46 && path=="UpgradeToRemove") return 3;
    if(kind==49 || kind==50) {if(path=="ObjectFilter") return 1;if(path=="ObjectFilter/IncludeThing" || path=="ObjectFilter/ExcludeThing") return 3;}
    if(kind==49 && path=="ApplyToUpgrade") return 3;
    return 0;
}
bool GameObjectMotionMoveAttribute(const std::string& typeId,const std::string& attribute) {
    int index=MotionMoveIndex(ParseUnsigned(typeId));if(index<0 || index==3) return false;const auto& set=motionFieldSets[4+index];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;
}
bool GameObjectMotionMovePaths(const std::string& typeId) {return ParseUnsigned(typeId)==3012512132u;}
bool GameObjectModuleNodeAttribute(unsigned kind,const std::string& path,const std::string& attribute) {
    if(kind>=165 && kind<=187) return GameObjectModuleNodeAttribute(162,path,attribute);
    if(kind>=162 && kind<=164) return path=="InitiateSound" && (attribute=="Sound" || attribute=="EvaEvent");
    if(kind>=159 && kind<=161) return kind==161 && path=="ObjectStatusValidation" && (attribute=="RequiredStatus" || attribute=="ForbiddenStatus");
    if(kind>=156 && kind<=158) return false;
    if(kind>=154 && kind<=155) return false;
    if(kind>=152 && kind<=153) return false;
    if(kind>=150 && kind<=151) return false;
    if(kind>=147 && kind<=149) {
        if(kind==147 && path=="DieMuxData") for(unsigned i=0;i<dieFieldSets[7].count;++i) if(attribute==dieFieldSets[7].fields[i].name) return true;
        if(kind==148 && path=="RadarColor") for(const auto& name:tiberiumColorNames) if(attribute==name) return true;
        return false;
    }
    if(kind>=145 && kind<=146) {
        if(kind==145 && path=="CustomAnimAndDuration") for(unsigned i=0;i<upgradeFieldSets[11].count;++i) if(attribute==upgradeFieldSets[11].fields[i].name) return true;
        return false;
    }
    if(kind>=143 && kind<=144) return kind==143 && (path=="StartColor" || path=="EndColor") && (attribute=="r" || attribute=="g" || attribute=="b");
    if(kind>=140 && kind<=142) return kind==142 && path=="ObjectStatusValidation" && (attribute=="RequiredStatus" || attribute=="ForbiddenStatus");
    if(kind>=137 && kind<=139) {
        if(kind!=139) return false;
        if(path=="RestrictedAreaShape/Offset") return attribute=="x" || attribute=="y" || attribute=="z";
        if(path=="RestrictedAreaShape") for(unsigned i=0;i<structureWallFieldSets[3].count;++i) if(attribute==structureWallFieldSets[3].fields[i].name) return true;
        return false;
    }
    if(kind>=133 && kind<=136) return kind==135 && (path=="InitialCaptureBonus" || path=="Deposit" || path=="ResourceCap") && (attribute=="Account" || attribute=="Amount");
    if(kind>=128 && kind<=132) return kind==129 && path=="DefaultRallyPointOffset" && (attribute=="x" || attribute=="y" || attribute=="z");
    if(kind==127) return false;
    if(kind>=123 && kind<=125) {
        if(kind!=123) return false;
        const ModuleFieldSet* set=path=="Die"?&dieFieldSets[7]:path=="CustomAnimAndDuration"?&upgradeFieldSets[11]:nullptr;
        if(set) for(unsigned i=0;i<set->count;++i) if(attribute==set->fields[i].name) return true;return false;
    }
    if(kind>=119 && kind<=122) {
        if(kind!=119) return false;unsigned fields=path=="HintObject"?4:path=="PlacementRestriction"?5:path=="SuggestedOffset"?6:99;
        if(fields==99) return false;const auto& set=assistFieldSets[fields];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;
    }
    if(kind>=112 && kind<=118) {
        if(kind==114 && path=="ObjectStatusValidation") return attribute=="RequiredStatus" || attribute=="ForbiddenStatus";
        if(kind==118 && (path=="RotationSpeed" || path=="UnitSpacing")) return attribute=="x" || attribute=="y" || attribute=="z";
        return false;
    }
    if(kind>=104 && kind<=111) {
        if(path=="UnitAITargetChooserData") {
            if(attribute=="TargetingCompareList") return true;
            for(unsigned i=0;i<weaponSetFieldSets[8].count;++i) if(attribute==weaponSetFieldSets[8].fields[i].name) return true;
        }
        if(path=="UnitAITargetChooserData/ObjectVisionRange") return attribute=="Range";
        return false;
    }
    if(kind>=96 && kind<=103) return false;
    if(kind>=92 && kind<=95) {
        const ModuleFieldSet* set=nullptr;
        if(kind==92) {if(path=="ObjectStatusValidation") set=&lifecycleFieldSets[6];if(path=="CustomAnimAndDuration") set=&upgradeFieldSets[11];}
        if(kind==93) {if(path=="PercentOfBuildCostToRebuildInfo") set=&healingFieldSets[5];if(path=="SelfAudio") set=&healingFieldSets[6];}
        if(kind==94 && path=="Die") set=&dieFieldSets[7];
        if(set) for(unsigned i=0;i<set->count;++i) if(attribute==set->fields[i].name) return true;return false;
    }
    if(kind==91) {
        unsigned fields=99;auto slash=path.find('/');auto slot=path.substr(0,slash);auto rest=slash==std::string::npos?"":path.substr(slash);
        if(rest.empty()) {
            if(attribute=="ParentID" && slot=="WeaponSlotHierarchicalTurret") return true;fields=1;
        }else if(rest=="/Weapon") fields=5;
        else if(rest=="/TurretSettings") fields=6;
        else if(rest=="/TurretSettings/TurretAITargetChooserData") {
            for(const auto& set:{weaponSetFieldSets[7],weaponSetFieldSets[8]}) for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;
        }else if(rest=="/TurretSettings/TurretAITargetChooserData/ObjectVisionRange") fields=9;
        if(fields==99) return false;const auto& set=weaponSetFieldSets[fields];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;
    }
    if(kind>=87) {const ModuleFieldSet* set=nullptr;if(kind==87 && path=="Data") set=&reactiveFieldSets[4];if((kind==88 || kind==90) && path=="CustomAnimAndDuration") set=&upgradeFieldSets[11];if(kind==90) {if(path=="WeaponOffset") set=&remainingFieldSets[15];if(path=="DieMuxData") set=&dieFieldSets[7];if(path=="WeaponFireProbability") set=&reactiveFieldSets[5];}if(set) for(unsigned i=0;i<set->count;++i) if(attribute==set->fields[i].name) return true;return false;}
    if(kind>=84) return false;
    if(kind>=80) {
        if(kind!=81) return false;unsigned fields=99;if(path=="Volume") fields=7;
        for(unsigned i=0;i<5;++i) {std::string base=std::string("Volume/")+motionShapeNames[i];if(path==base) fields=8+i;if(path==base+"/Translation") fields=13;if(path==base+"/Rotation") fields=14;}
        if(fields==99) return false;const auto& set=motionFieldSets[fields];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;
    }
    if(kind>=77) {if(path=="EndOffset") return attribute=="x" || attribute=="y" || attribute=="z";return path=="ObjectStatusValidation" && (attribute=="RequiredStatus" || attribute=="ForbiddenStatus");}
    if(kind>=69) {if(path=="ObjectStatusValidation") return attribute=="RequiredStatus" || attribute=="ForbiddenStatus";unsigned fields=path=="Color"?8:path=="EffectInfo"?9:path=="FireWeaponNugget"?10:path=="FireWeaponNugget/Offset"?11:99;if(fields==99) return false;const auto& set=weaponFieldSets[fields];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(kind>=61) return kind==67 && path=="ObjectStatusValidation" && (attribute=="RequiredStatus" || attribute=="ForbiddenStatus");
    if(kind==60) {if(path=="CustomAnimAndDuration") for(unsigned i=0;i<upgradeFieldSets[11].count;++i) if(attribute==upgradeFieldSets[11].fields[i].name) return true;return false;}
    if(kind==59) return path=="GuardPosOffset" && (attribute=="x" || attribute=="y" || attribute=="z");
    if(kind>=55) return false;
    if(kind>=52) {
        unsigned fields=path=="QuantityModifier"?3:path=="ProductionModifier"?4:path=="GiveUpgradeToProduced/UpgradeTranslate"?5:99;
        if(path=="UnitCreatePoint" || path=="NaturalRallyPoint") {for(unsigned i=0;i<remainingFieldSets[15].count;++i) if(attribute==remainingFieldSets[15].fields[i].name) return true;return false;}
        if(fields==99) return false;for(unsigned i=0;i<productionFieldSets[fields].count;++i) if(attribute==productionFieldSets[fields].fields[i].name) return true;return false;
    }
    if(path=="ObjectFilter") return false;
    unsigned index=path=="Nugget"?11:path=="Nugget/UpgradeTexture"?12:path=="StructureOffset"?13:(path=="Offset" || path=="StructureOffset/Offset")?15:99;
    if(path=="CustomAnimAndDuration" || path=="Nugget/CustomAnimAndDuration") {for(unsigned i=0;i<upgradeFieldSets[11].count;++i) if(attribute==upgradeFieldSets[11].fields[i].name) return true;return false;}
    if(index==99) return false;for(unsigned i=0;i<remainingFieldSets[index].count;++i) if(attribute==remainingFieldSets[index].fields[i].name) return true;return false;
}
bool GameObjectModuleChildSupported(const std::string& parent,const std::string& child) {
    for(const auto& entry:moduleChildNames) if(parent==entry.parent && child==entry.child) return true;
    return false;
}

