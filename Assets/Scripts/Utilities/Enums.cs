public enum Metric
{
    Accommodation,
    Satisfaction,
    Effectiveness,
    Spending,
    Defense,
    Threat, // Mod changes per turn, not total
    Chaos,
    // Effectiveness Subcategories
    Weaponry,
    Magic,
    Equipment,
    Training,
    // Satisfaction Subcategories
    Food,
    Entertainment,
    Luxuries,
}

public enum BuildingType
{
    Terrain,
    Ruins,
    GuildHall,
    //Equipment
    GeneralStore, //Better name?
    Tailor,
    Apothecary,
    //Weaponry
    Blacksmith,
    Leatherworks,
    Armoury, //Doubles as Defense
    //Magic
    Alchemists,
    Enchanters,
    Jewellers, //Doubles as Luxury
    //Training: Class specific expensive buildings (quest unlocks?)
    Arena, //Doubles as Entertainment
    HuntingLodge, //Doubles as Food
    PerformanceHall, //Doubles as Entertainment
    Monastery, //Doubles as Accommodation
    Library, //Doubles as Magic
    //Food
    Farm,
    Bakery,
    Brewery,
    //Entertainment
    Tavern,
    Plaza,
    Bathhouse,
    //Luxury
    Herbalist,
    Cartographers,
    //Accommodation
    Inn,
    House,
    Barracks, // Doubles as housing
    //Defense
    GuardOutpost,
    //Misc
    Graveyard,
    Lake
}

public enum EventType
{
    Flavour,
    AdventurersJoin,
    Threat,
    Chaos,
    Endgame,
    Chain,
    Special,
    Advert,
    GameOver,
    AdventurersLeave,
    Blueprint,
    Radiant
}

public enum AdventurerCategory
{
    Brawler,
    Outrider,
    Performer,
    Diviner,
    Conduit
}
