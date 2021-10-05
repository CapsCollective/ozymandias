using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class Colors
    {
        public static readonly Color CardLight = new Color(0.94f, 0.93f, 0.86f);
        public static readonly Color CardDark = new Color(0.16f, 0.13f, 0.07f);
        public static readonly Color Green = new Color(0,0.7f,0.1f);
        public static readonly Color Red = new Color(0.9f,0,0);
        public static readonly Color CostActive = new Color(0.8f,0.6f,0.2f);
        public static readonly Color CostInactive = new Color(0.85f,0.85f,0.85f);
        public const string GreenText = "<color=#007000ff>";
        public const string RedText = "<color=#820000ff>";
        public const string EndText = "</color>";

        public static readonly Dictionary<Stat, Color> StatColours = new Dictionary<Stat, Color>
        {
            {Stat.Brawler, new Color(0.7f, 0.3f, 0.3f)},
            {Stat.Outrider, new Color(0.25f, 0.45f, 0.2f)},
            {Stat.Performer, new Color(0.30f, 0.6f, 0.6f)},
            {Stat.Diviner, new Color(0.75f, 0.6f, 0.3f)},
            {Stat.Arcanist, new Color(0.6f, 0.3f, 0.75f)},
            {Stat.Spending, new Color(0.8f, 0.6f, 0f)},
            {Stat.Defence, new Color(0.25f, 0.35f, 1f)},
            {Stat.Food, new Color(0.5f, 0.65f, 0f)},
            {Stat.Housing, new Color(0.6f, 0.4f, 0f)}
        };
    }
}
