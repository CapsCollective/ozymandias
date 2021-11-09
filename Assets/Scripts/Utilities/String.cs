using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Utilities
{
    public static class String
    {
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

        public static string Pluralise(string word)
        {
            // Discard empty words
            if (word.Length <= 0) return word;
            
            // Lowercase the word for search purposes
            var searchWord = word.ToLower();
            
            // Check for non-pluralising word
            if(UncountableWords.FindIndex(s => s == searchWord) >= 0) return word;
            
            // Check for irregular plurals
            if (IrregularPluralRules.TryGetValue(searchWord, out var plural))
            {
                return word.Substring(0, 1) + plural.Substring(1);
            }

            // Regex check for regular plurals
            foreach (var kv in RegularPluralRules)
            {
                Regex findPattern = new Regex(kv.Key);

                if (!findPattern.IsMatch(searchWord)) continue;

                return Regex.Replace(word, kv.Key, kv.Value);
            }
            
            // Failed to pluralise word
            return word;
        }
    }
}