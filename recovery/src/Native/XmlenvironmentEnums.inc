    if(type=="WeatherType") {
        if(value=="CLOUDY")return 1u;
        if(value=="CLOUDYRAINY")return 3u;
        if(value=="INVALID")return 6u;
        if(value=="NORMAL")return 0u;
        if(value=="RAINY")return 2u;
        if(value=="SNOWY")return 5u;
        if(value=="SUNNY")return 4u;
        return 1u;
    }
    if(type=="TimeOfDayType") {
        if(value=="AFTERNOON")return 2u;
        if(value=="EVENING")return 3u;
        if(value=="INTERPOLATE")return 5u;
        if(value=="INVALID")return 0u;
        if(value=="MORNING")return 1u;
        if(value=="NIGHT")return 4u;
        return 2u;
    }
    if(type=="WaterTextureType") {
        if(value=="BUMP")return 1u;
        if(value=="FIRST")return 0u;
        if(value=="NOISE")return 4u;
        if(value=="NORMAL")return 2u;
        if(value=="RIVER")return 0u;
        if(value=="SKY")return 3u;
        if(value=="SPARKLE")return 5u;
        return 1u;
    }
    if(type=="DistributionType") {
        if(value=="CONSTANT")return 0u;
        if(value=="GAUSSIAN")return 2u;
        if(value=="HIGH_BIAS")return 5u;
        if(value=="LOW_BIAS")return 4u;
        if(value=="TRIANGULAR")return 3u;
        if(value=="UNIFORM")return 1u;
        return 0u;
    }
    if(type=="TerrainClassType") {
        if(value=="Cliff")return 3u;
        if(value=="Dirt")return 2u;
        if(value=="Grass")return 4u;
        if(value=="Misc")return 1u;
        if(value=="Mud")return 7u;
        if(value=="Road")return 6u;
        if(value=="Rock")return 5u;
        if(value=="Sand")return 8u;
        if(value=="Shrub")return 9u;
        if(value=="Snow")return 10u;
        if(value=="UNSPECIFIED")return 0u;
        return 3u;
    }


