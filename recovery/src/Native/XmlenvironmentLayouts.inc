// Source-derived layouts: original Module blob2d2f471c8008b7c2e1f78e39752d0726da395cea.
std::map<std::string,Layout> MakeLayouts() {
    return {
        {"Vector2",{8,{{"x",0,Kind::Float,"0.0",""},{"y",4,Kind::Float,"0.0",""}},{}}},
        {"Vector3",{12,{{"x",0,Kind::Float,"0.0",""},{"y",4,Kind::Float,"0.0",""},{"z",8,Kind::Float,"0.0",""}},{}}},
        {"RGBColor",{12,{{"R",0,Kind::Float,nullptr,""},{"G",4,Kind::Float,nullptr,""},{"B",8,Kind::Float,nullptr,""}},{}}},
        {"PackedColor",{4,{},{}}},
        {"Random",{12,{{"Type",0,Kind::Enum,"UNIFORM","DistributionType"},{"Low",4,Kind::Float,nullptr,""},{"High",8,Kind::Float,nullptr,""}},{}}},
        {"Rain",{60,{{"RainTexture",0,Kind::Reference,nullptr,""},{"IsRaining",56,Kind::Bool,"false",""},{"NumRaindropsPerBox",4,Kind::Signed,"16000",""},{"RainBoxWidth",8,Kind::Float,"200",""},{"RainBoxLength",12,Kind::Float,"200",""},{"RainBoxHeight",16,Kind::Float,"200",""},{"MinWidth",20,Kind::Float,"0.5",""},{"MaxWidth",24,Kind::Float,"1.5",""},{"MinHeight",28,Kind::Float,"5.0",""},{"MaxHeight",32,Kind::Float,"15.0",""},{"MinSpeed",36,Kind::Float,"50.0",""},{"MaxSpeed",40,Kind::Float,"150.0",""},{"MinAlpha",44,Kind::Float,"0.1",""},{"MaxAlpha",48,Kind::Float,"0.5",""},{"WindStrength",52,Kind::Float,"1.0",""}},{}}},
        {"Lightning",{12,{{"LightningDuration",0,Kind::Signed,"30",""},{"LightningChance",4,Kind::Float,"0.01",""}},{{"LightningFactor","Random",8,ChildKind::Pointer}}}},
        {"Cloud",{8,{},{{"CloudTextureSize","Vector2",0,ChildKind::Pointer},{"CloudOffsetPerSecond","Vector2",4,ChildKind::Pointer}}}},
        {"WeatherData",{12,{{"id",0,Kind::Enum,nullptr,"WeatherType"},{"HasLightning",8,Kind::Bool,nullptr,""},{"Sound",4,Kind::Reference,nullptr,""}},{}}},
        {"Weather",{24,{},{{"Rain","Rain",4,ChildKind::Pointer},{"Lightning","Lightning",8,ChildKind::Pointer},{"Cloud","Cloud",12,ChildKind::Pointer},{"WeatherData","WeatherData",16,ChildKind::List}}}},
        {"WaterTransparency",{16,{{"TransparentWaterDepth",4,Kind::Float,"3.0",""},{"TransparentWaterMinOpacity",8,Kind::Float,"1.0",""},{"StandingWaterTexture",12,Kind::Hash,nullptr,""}},{}}},
        {"WaterSetting",{12,{{"TimeOfDay",4,Kind::Enum,nullptr,"TimeOfDayType"}},{{"DiffuseColor","PackedColor",8,ChildKind::Inline}}}},
        {"CloudEffect",{152,{{"CloudTexture",0,Kind::Reference,nullptr,""},{"DarkCloudTexture",4,Kind::Reference,nullptr,""},{"AlphaTexture",8,Kind::Reference,nullptr,""},{"PropagateSpeed",12,Kind::Float,nullptr,""},{"Angle",16,Kind::Signed,nullptr,""},{"DarkeningRate",20,Kind::Signed,nullptr,""},{"LighteningRate",24,Kind::Signed,nullptr,""},{"CloudScrollSpeed",28,Kind::Float,nullptr,""},{"DissipateTexture",32,Kind::Reference,nullptr,""},{"DissipateStartLevel",36,Kind::Float,nullptr,""},{"DissipateSpeed",40,Kind::Float,nullptr,""},{"DissipateRateScale",44,Kind::Float,nullptr,""},{"LightningShadows",148,Kind::Bool,nullptr,""},{"JitterLightningLightIntensity",149,Kind::Bool,nullptr,""},{"JitterLightningLightPosition",150,Kind::Bool,nullptr,""},{"LightningChance",48,Kind::Float,nullptr,""},{"LightningFrequency",52,Kind::Float,nullptr,""},{"LightningShadowIntensity",56,Kind::Float,nullptr,""},{"LightningFX",60,Kind::Hash,nullptr,""}},{{"DarkeningFactor","RGBColor",64,ChildKind::Inline},{"DarkeningFactorRain","RGBColor",76,ChildKind::Inline},{"LightningShadowColor","RGBColor",88,ChildKind::Inline},{"LightningLightPosition1","Vector2",100,ChildKind::Inline},{"LightningLightPosition2","Vector2",108,ChildKind::Inline},{"LightningLightPosition3","Vector2",116,ChildKind::Inline},{"LightningIntensity","Random",124,ChildKind::Inline},{"LightningDuration","Random",136,ChildKind::Inline}}}},
        {"Environment",{156,{},{{"CloudEffect","CloudEffect",4,ChildKind::Inline}}}},
        {"LightingInfo",{36,{},{{"Ambient","RGBColor",0,ChildKind::Inline},{"Diffuse","RGBColor",12,ChildKind::Inline},{"LightPos","Vector3",24,ChildKind::Inline}}}},
        {"LightingTime",{12,{{"TimeOfDay",0,Kind::Enum,nullptr,"TimeOfDayType"}},{{"Lighting","LightingInfo",4,ChildKind::List}}}},
        {"TerrainLighting",{8,{},{{"LightingTimeGroup","LightingTime",0,ChildKind::List}}}},
        {"LightingSettings",{48,{{"TimeOfDay",8,Kind::Enum,nullptr,"TimeOfDayType"},{"OccludedLuminanceScale",12,Kind::Float,"0.5",""},{"LoadAfter",4,Kind::Reference,nullptr,""}},{{"LightingMultiplierIfNoClouds","RGBColor",16,ChildKind::Inline},{"CurrentLightingSettings","LightingTime",28,ChildKind::Inline},{"TerrainLighting","TerrainLighting",40,ChildKind::Inline}}}},
        {"String",{8,{},{}}},
        {"WaterTextureList",{16,{{"Type",4,Kind::Enum,nullptr,"WaterTextureType"}},{{"Texture","String",8,ChildKind::List}}}},
    };
}
