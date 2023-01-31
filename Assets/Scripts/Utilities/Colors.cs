using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class Colors
    {
        public static bool ColorBlind { get; set; }
        
        public static readonly Color CardLight = new Color(0.94f, 0.93f, 0.86f);
        public static readonly Color CardDark = new Color(0.16f, 0.13f, 0.07f);

        public static Color GridInactive => new Color(0.2f, 0.2f, 0.2f, 0.2f);
        public static Color GridActive => ColorBlind ? new Color32(255,194,10,153) : new Color(0,0.7f,0.1f, 0.6f);
        public static Color GridInvalid => ColorBlind ? new Color32(12,123,220,153) : new Color(0.9f, 0, 0, 0.6f);
        
        public static Color Green => ColorBlind ? new Color32(255,194,10,255) : new Color(0,0.7f,0.1f);
        public static Color Red => ColorBlind ? new Color32(12,123,220,255) : new Color(0.9f, 0, 0);
        public static Color LightRed => ColorBlind ? new Color32(12,123,220,255) : new Color(1f, 0.1f, 0.1f);
        public static readonly Color CostActive = new Color(0.8f,0.6f,0.2f);
        public static readonly Color CostInactive = new Color(0.85f,0.85f,0.85f);

        public static string GreenText => ColorBlind ? "" : "<color=#007000ff>";
        public static string RedText => ColorBlind ? "" : "<color=#820000ff>";
        public static string EndText => ColorBlind ? "" :"</color>";

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
