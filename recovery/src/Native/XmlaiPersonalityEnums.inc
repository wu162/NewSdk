// Include at namespace scope after EnumView, before XmlaiLayouts.inc.
// Exact original PE maps; same source blob as XmlaiPersonality.inc.
// Original enumMap source line 12366
static const EnumNameValue XmlaiPersonality_AIStrategicDirectiveEvent[] = {
    {"Begin",0},
    {"Cancel",2},
    {"Complete",1}
};
// Original enumMap source line 12350
static const EnumNameValue XmlaiPersonality_AIFeedbackWorthySkirmishGameEvent[] = {
    {"DefeatedByLocalPlayer",4},
    {"DefeatedLocalPlayer",3},
    {"FirstSeen",2},
    {"InTrouble",1},
    {"Lost",5},
    {"StartGame",0}
};
inline EnumView GetXmlaiPersonalityEnum(const std::string& name) {
    if(name=="AIStrategicDirectiveEvent") return EnumView{XmlaiPersonality_AIStrategicDirectiveEvent,sizeof(XmlaiPersonality_AIStrategicDirectiveEvent)/sizeof(EnumNameValue)};
    if(name=="AIFeedbackWorthySkirmishGameEvent") return EnumView{XmlaiPersonality_AIFeedbackWorthySkirmishGameEvent,sizeof(XmlaiPersonality_AIFeedbackWorthySkirmishGameEvent)/sizeof(EnumNameValue)};
    return EnumView{nullptr,0};
}
