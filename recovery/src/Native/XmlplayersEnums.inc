const EnumNameValue enumBodyDamageType[]={
{"DAMAGED",1},
{"PRISTINE",0},
{"REALLY_DAMAGED",2},
{"RUBBLE",3},
};
const EnumNameValue enumPlayerTemplateType[]={
{"COMMENTATOR",4},
{"NONPLAYABLE",0},
{"NULL",5},
{"OBSERVER",2},
{"PLAYABLE",1},
{"RANDOM",3},
};
const EnumNameValue enumFactionType[]={
{"Allies",0},
{"Japan",2},
{"Neutral",3},
{"RANDOM",-1},
{"Soviet",1},
};
const EnumNameValue enumAIBeaconType[]={
{"Assault",0},
{"HoldObject",2},
{"HoldPosition",1},
{"Invalid",-1},
};
const EnumNameValue enumGameStatsMode[]={
{"CAMPAIGN_GAME",0},
{"LAN_GAME",2},
{"ONLINE_CLAN_1V1_GAME",6},
{"ONLINE_CLAN_2V2_GAME",7},
{"ONLINE_RANKED_1V1_GAME",4},
{"ONLINE_RANKED_2V2_GAME",5},
{"ONLINE_UNRANKED_GAME",3},
{"SKIRMISH_GAME",1},
};
const EnumNameValue enumAIBuildHeuristics[]={
{"BEST_UNIT_ACTUAL",2},
{"BEST_UNIT_EXPECTED",1},
{"BEST_UNIT_WEIGHTED",3},
{"BUILD_TEST",16},
{"CONSTANT",0},
{"FACTORY",8},
{"FEWEST_UNIT",4},
{"HARVESTER",12},
{"NEEDED_UNIT",7},
{"POWER",10},
{"RANDOM_UNIT",5},
{"REFINERY",11},
{"TECH",9},
{"UNIQUE",13},
{"UPGRADE",14},
{"UPGRADE_TECH",15},
{"UPGRADE_TEST",17},
{"WORST_UNIT",6},
};
unsigned PlayerEnum(const std::string& name,const std::string& value) {
if(name=="BodyDamageType"){for(const auto& e:enumBodyDamageType)if(value==e.name)return static_cast<unsigned>(e.value);return 1u;}
if(name=="PlayerTemplateType"){for(const auto& e:enumPlayerTemplateType)if(value==e.name)return static_cast<unsigned>(e.value);return 4u;}
if(name=="FactionType"){for(const auto& e:enumFactionType)if(value==e.name)return static_cast<unsigned>(e.value);return 0u;}
if(name=="AIBeaconType"){for(const auto& e:enumAIBeaconType)if(value==e.name)return static_cast<unsigned>(e.value);return 0u;}
if(name=="GameStatsMode"){for(const auto& e:enumGameStatsMode)if(value==e.name)return static_cast<unsigned>(e.value);return 0u;}
if(name=="AIBuildHeuristics"){for(const auto& e:enumAIBuildHeuristics)if(value==e.name)return static_cast<unsigned>(e.value);return 2u;}
throw std::invalid_argument("Unknown player enum: "+name);
}
