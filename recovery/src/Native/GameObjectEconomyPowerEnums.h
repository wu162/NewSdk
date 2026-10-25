// Original Tokenizer PE SHA256 ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603.
// Complete original name/value tables; bitset unknown index and single enum fallback stay distinct.
const EnumNameValue economyPowerTypeNames[] = {
    { "BACKUP_GENERATOR", 0u },
    { "INVALID", 1u },
};
const AIEnum economyPowerTypeEnum = { economyPowerTypeNames, 2u, 0, 0u };
const EnumNameValue economyPowerFlagsNames[] = {
    { "ACTIVE_WHEN_REPAIRING", 1u },
    { "NONE", 0u },
};
const AIEnum economyPowerFlagsEnum = { economyPowerFlagsNames, 2u, 1, 2u };
const EnumNameValue economyPowerCategoryNames[] = {
    { "AI_MONEY_LENDER", 22u },
    { "AUTO_DEPOSIT", 4u },
    { "BOUNTY", 6u },
    { "BUILD_WALLSPAN", 16u },
    { "CANCEL_BUILD", 9u },
    { "CAPTURE_BONUS", 3u },
    { "CASH_CHEAT", 7u },
    { "CASH_HACK", 26u },
    { "DEATH_REFUND", 11u },
    { "DEVASTATE", 8u },
    { "DOZER_CONSTRUCT", 17u },
    { "HARVESTER", 2u },
    { "INFILTRATOR", 27u },
    { "MONEY_CRATE", 10u },
    { "PILLAGE", 28u },
    { "PLAYERKILLED", 19u },
    { "PLAYER_CLONE", 24u },
    { "PLAYER_GIVE_MONEY", 23u },
    { "PRODUCTION", 15u },
    { "SCRIPTED_MONEY_ACTION", 20u },
    { "SELF_REPAIR", 18u },
    { "SELL_OBJECT", 5u },
    { "SHARED_DEPOSIT", 14u },
    { "SLAUGHTER", 12u },
    { "SPECIAL_POWER", 21u },
    { "STARTING_CASH", 1u },
    { "STEAL_MONEY", 29u },
    { "TERRAIN_RESOURCE", 13u },
    { "TRANSFER_ASSETS", 25u },
    { "UNKNOWN", 0u },
};
const AIEnum economyPowerCategoryEnum = { economyPowerCategoryNames, 30u, 22, 0u };
