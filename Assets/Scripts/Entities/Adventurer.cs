using System;
using Managers;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;
using static Managers.GameManager;

namespace Entities
{
    [Serializable]
    public class Adventurer : MonoBehaviour
    {
        private static string[] firstNames = { "Lydan", "Syrin", "Ptorik", "Joz", "Varog", "Gethrod", "Hezra", "Feron", "Ophni",
            "Colborn", "Fintis", "Gatlin", "Jinto", "Hagalbar", "Krinn", "Lenox", "Revvyn", "Hodus", "Dimian", "Paskel", 
            "Kontas", "Weston", "Azamarr", "Jather", "Tekren", "Jareth", "Adon", "Zaden", "Eune", "Graff", "Tez", "Jessop", 
            "Gunnar", "Pike", "Domnhar", "Baske", "Jerrick", "Mavrek", "Riordan", "Wulfe", "Straus", "Tyvrik", "Henndar", 
            "Favroe", "Whit", "Jaris", "Renham", "Kagran", "Lassrin", "Vadim", "Arlo", "Quintis", "Vale", "Caelan", 
            "Yorjan", "Khron", "Ishmael", "Jakrin", "Fangar", "Roux", "Baxar", "Hawke", "Gatlen", "Barak", "Nazim", 
            "Kadric", "Paquin", "Kent", "Moki", "Rankar", "Lothe", "Ryven", "Clawsen", "Pakker", "Embre", "Cassian", 
            "Verssek", "Dagfinn", "Ebraheim", "Nesso", "Eldermar", "Rivik", "Rourke", "Barton", "Hemm", "Sarkin", "Blaiz", 
            "Talon", "Agro", "Zagaroth", "Turrek", "Esdel", "Lustros", "Zenner", "Baashar", "Dagrod", "Gentar", "Feston", 
            "Syrana", "Resha", "Varin", "Wren", "Yuni", "Talis", "Kessa", "Magaltie", "Aeris", "Desmina", "Krynna", 
            "Asralyn", "Herra", "Pret", "Kory", "Afia", "Tessel", "Rhiannon", "Zara", "Jesi", "Belen", "Rei", "Ciscra", 
            "Temy", "Renalee", "Estyn", "Maarika", "Lynorr", "Tiv", "Annihya", "Semet", "Tamrin", "Antia", "Reslyn", 
            "Basak", "Vixra", "Pekka", "Xavia", "Beatha", "Yarri", "Liris", "Sonali", "Razra", "Soko", "Maeve", "Everen", 
            "Yelina", "Morwena", "Hagar", "Palra", "Elysa", "Sage", "Ketra", "Lynx", "Agama", "Thesra", "Tezani", "Ralia", 
            "Esmee", "Heron", "Naima", "Rydna", "Sparrow", "Baakshi", "Ibera", "Phlox", "Dessa", "Braithe", "Taewen", 
            "Larke", "Silene", "Phressa", "Esther", "Anika", "Rasy", "Harper", "Indie", "Vita", "Drusila", "Minha", 
            "Surane", "Lassona", "Merula", "Kye", "Jonna", "Lyla", "Zet", "Orett", "Naphtalia", "Turi", "Rhays", "Shike", 
            "Hartie", "Beela", "Leska", "Vemery", "Lunex", "Fidess", "Tisette", "Partha"
        };
    
        private static string[] lastNames = { "Bonebreaker", "Ashheart", "Marblecleaver", "Mountainkiller", "Seabreaker",
            "Clearmaul", "Lonesurge", "Honorbane", "Bouldergrain", "Claw", "Bluechief", "Hallowdrifter", "Netherbleeder",
            "Terrarock", "Sacredlight", "Rosetalon", "Barleyless", "Winterhunter", "Twoforest", "Deadshield", "Woodcloud",
            "Loneflaw", "Landsword", "Sheephunter", "Gazersea", "Landthunder", "Shieldgazer", "Whitesilver", "Starcloak",
            "Moorplains", "Tonguewulf", "Swordblade", "Mountainshade", "Maw", "Leafhiltsheep", "Oakborn", "Blackshadow", 
            "Powerbitter", "Bloodstar", "Rainshortear", "Goldcloak", "Mazegreen", "Mourneforge", "Cyanoaken", "Lynx", 
            "Eyeswyrm", "Starsteel", "Moorhalf", "Wyrmfire", "Halfwinter", "Breakerspear", "Glazeman", "Blurelf", 
            "Gazerbush", "Hamershine", "Commonseeker", "Roughwhirl", "Laughingsnout", "Orbstrike", "Ambersnarl", 
            "Crowstrike", "Runebraid", "Stillblade", "Hallowedsorrow", "Mildbreath", "Fogforge", "Albilon", "Ginerisey", 
            "Brichazac", "Lomadieu", "Bellevé", "Dudras", "Chanassard", "Ronchessac", "Chamillet", "Bougaitelet", 
            "Hallowswift", "Sacredpelt", "Rapidclaw", "Hazerider", "Shadegrove", "Coldsprinter", "Winddane", "Ashsorrow", 
            "Humblecut", "Ashbluff", "Marblemaw", "Boneflare", "Monsterbelly", "Truthbelly", "Sacredmore", "Dawnless", 
            "Crestbreeze", "Neredras", "Dumières", "Albimbert", "Cremeur", "Brichallard", "Béchalot", "Chabares", 
            "Chauveron", "Rocheveron", "Vernize", "Brightdoom", "Clanwillow", "Wheatglow", "Terrarock", "Laughingroar", 
            "Silverweaver", "Clearpunch", "Shieldtrap", "Foreswift", "Softgloom", "Treelash", "Grandsplitter", "Marblewing",
            "Sharpdoom", "Terraspear", "Rambumoux", "Lauregnory", "Chanalet", "Broffet", "Cardaithier", "Chauvelet", 
            "Astaseul", "Bizeveron", "Vernillard", "Croirral", "Wildforce", "Frozenscribe", "Warbelly", "Mournrock", 
            "Sagepunch", "Solidcut", "Peacescream", "Slateflayer", "Mistblood", "Winterwound", "Spiritscribe", "Irongrip", 
            "Plaingrove", "Keenstone", "Proudswift", "Marshrider", "Nicklegrain", "Masterfang", "Springbender", "Paleforce",
            "Strongblaze", "Silentbrace", "Dreamreaver", "Firecrusher", "Stoutspirit", "Whitemoon"
        };

        public Quest assignedQuest;

        public AdventurerType type;

        public bool isSpecial; // Works as a regular adventurer but is required for certain events so wont be removed

        public int turnJoined;

        public static string RandomName =>
            firstNames[Random.Range(0, firstNames.Length)] + " " + lastNames[Random.Range(0, lastNames.Length)];

        public static AdventurerType RandomType => Random.Range(0, 5) switch {
            0 => AdventurerType.Brawler,
            1 => AdventurerType.Outrider,
            2 => AdventurerType.Performer,
            3 => AdventurerType.Diviner,
            4 => AdventurerType.Arcanist,
            _ => AdventurerType.Brawler
        };

        public void Create(AdventurerType? aType = null)
        {
            name = RandomName;
            type = aType ?? RandomType;
            turnJoined = Manager.TurnCounter;
        }
    
        public AdventurerDetails Save()
        {
            return new AdventurerDetails
            {
                name = name,
                type = type,
                isSpecial = isSpecial,
                turnJoined = turnJoined
            };
        }

        public void Load(AdventurerDetails adventurer)
        {
            name = adventurer.name;
            type = adventurer.type;
            isSpecial = adventurer.isSpecial;
            turnJoined = adventurer.turnJoined;
        }
    }
}
