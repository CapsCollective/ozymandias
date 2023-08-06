using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Utilities
{
    public static class String
    {
        #region Pluralise 
        private static readonly List<string> UncountableWords = new List<string> {
            "sheep", 
            "fish",
            "deer",
            "moose",
            "series",
            "species",
            "money",
            "rice",
            "information",
            "equipment",
        };
        
        private static readonly Dictionary<string, string> IrregularPluralRules = new Dictionary<string, string> {
            {"move", "moves"},
            {"foot", "feet"},
            {"goose", "geese"},
            {"sex", "sexes"},
            {"child", "children"},
            {"man", "men"},
            {"tooth", "teeth"},
            {"person", "people"},
        };
        
        private static readonly Dictionary<string, string> RegularPluralRules = new Dictionary<string, string> {
            {"(quiz)$", "$1zes"},
            {"^(ox)$", "$1en"},
            {"([m|l])ouse$", "$1ice"},
            {"(matr|vert|ind)ix|ex$", "$1ices"},
            {"(x|ch|ss|sh)$", "$1es"},
            {"([^aeiouy]|qu)y$", "$1ies"},
            {"(hive)$", "$1s"},
            {"(?:([^f])fe|([lr])f)$", "$1$2ves"},
            {"(shea|lea|loa|thie)f$", "$1ves"},
            {"sis$", "ses"},
            {"([ti])um$", "$1a"},
            {"(tomat|potat|ech|her|vet)o$", "$1oes"},
            {"(bu)s$", "$1ses"},
            {"(alias)$", "$1es"},
            {"(octop)us$", "$1i"},
            {"(ax|test)is$", "$1es"},
            {"(us)$", "$1es"},
            {"([^s]+)$", "$1s"},
        };
        
        private static readonly Dictionary<string, string> PluralCache = new Dictionary<string, string>();
        
        public static string Pluralise(this string word, int count)
        {
            return count == 1 ? word : Pluralise(word);
        }
        
        public static string Pluralise(this string word)
        {
            // Try find plural in cache
            if (PluralCache.ContainsKey(word))
            {
                return PluralCache[word];
            }

            // Discard empty words
            if (word.Length <= 0) return word;
            
            // Lowercase the word for search purposes
            var searchWord = word.ToLower();

            var plural = word;
            if(UncountableWords.FindIndex(s => s == searchWord) >= 0)
            {
                // Non-pluralising words
            }
            else if (IrregularPluralRules.TryGetValue(searchWord, out var pluralised))
            {
                // Irregular plurals
                plural = word[..1] + pluralised[1..];
            }
            else
            {
                // Regular plurals
                foreach (var kv in RegularPluralRules)
                {
                    Regex findPattern = new Regex(kv.Key);

                    if (!findPattern.IsMatch(searchWord)) continue;
                
                    plural = Regex.Replace(word, kv.Key, kv.Value);
                    break;
                }   
            }
            
            // Cache the found word
            PluralCache[word] = plural;
            return word;
        }
        #endregion
        
        public static string Conditional(this string phrase, bool display) => display ? phrase : "";

        private const string AlignCenter = "<align=\"center\">";
        private const string AlignEnd = "</align>";
        public static string Center(this string s) => AlignCenter + s + AlignEnd;

        private const string ItalicsStart = "<i>";
        private const string ItalicsEnd = "</i>";
        public static string Italics(this string s) => ItalicsStart + s + ItalicsEnd;
        
        public const string ListStart = "\n •<indent=20px>";
        public const string ListEnd = "</indent>";
        public static string ListItem(this string s) => ListStart + s + ListEnd;

        public static string WithSign(this int count) => (count > 0 ? "+" : "") + count;

        public static string GuildWithIcon(Guild guild) => $"{guild} (<sprite={(int)guild}>)";
        public static string GuildWithIcon(Guild guild, int count) => $"{guild.ToString().Pluralise(count)} (<sprite={(int)guild}>)";

        public static string StatWithIcon(Stat stat) => 
            $"{(stat == Stat.Spending ? "Wealth per turn" : stat)}{" Satisfaction".Conditional((int)stat < 5)} (<sprite={(int)stat}>)";

        public static string StatIcon(Stat stat) => $"<sprite={(int)stat}>";
    }
}
