namespace Utilities
{
    public enum Stat
    {
        Brawler,
        Outrider,
        Performer,
        Diviner,
        Arcanist,
        Housing,
        Food,
        Spending,
        Defence,
        Threat,
        Stability
    }

    public enum BuildingType
    {
        Terrain,
        Ruins,
        GuildHall,
        Blacksmith,
        Tavern,
        Herbalist,
        Shrine,
        Library,
        House,
        Inn,
        Farm,
        Watchtower,
        Markets,
        Tannery,
        FightingPit,
        Enchanter,
        Plaza,
        BathHouse,
        Monastery,
        Bakery,
        Brewery,
        HuntingLodge,
        Armoury,
        Barracks,
        Tailor,
        Apothecary,
        Jeweller,
        Cartographer,
        Quest
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

    public enum AdventurerType
    {
        Brawler,
        Outrider,
        Performer,
        Diviner,
        Arcanist
    }
    
    public enum TooltipType
    {
        Brawler,
        Outrider,
        Performer,
        Diviner,
        Arcanist,
        Housing,
        Food,
        Wealth,
        Stability,
        Newspaper,
        Progress,
        Quests,
        NextTurn,
        Threat,
        Defence
    }
    
    public enum HighlightState
    {
        Inactive,
        Valid,
        Invalid
    }
    
    public enum Direction
    {
        Left,
        Forward,
        Right,
        Back
    }
    
    public enum GameState
    {                      
        Loading,   // ┌────────────────────────────────┐
        ToIntro,   // │              ┌──►NextTurn───┐  │
        InIntro,   // │ ┌►ToGame─►InGame◄──►InMenu◄─┘  │
        ToGame,    // │ │                    ▲ │ │     │
        InGame,    // │ │   ┌────────────────┘ │ └──┐  │
        NextTurn,  // │ │   ▼      ┌─Loading   │    │  │
        InMenu,    // │ ├─InIntro◄─┴──ToIntro◄─┤─┐  │  │
        EndGame,   // │ └►ToCredits─►InCredits─┘ │  ▼  │
        ToCredits, // │                        GameEnd │
        InCredits  // └────────────────────────────────┘
    }

    public enum Achievement
    {
        Test = 0,
        
    }
    
    public enum Upgrade
    {
        Test = 0,
        
    }
}
