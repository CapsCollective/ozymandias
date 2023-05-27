using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class Colors
    {
        public static bool ColorBlind { get; set; }
        
        public static readonly Color CardLight = new Color(0.94f, 0.93f, 0.86f);
        public static readonly Color CardDark = new Color(0.16f, 0.13f, 0.07f);

        private const byte GridActiveOpacity = 150;
        public static Color32 GridInactive = new Color32(20, 20, 20, 40);
        public static Color32 GridHighlighted = new Color32(180, 150, 0, GridActiveOpacity);
        public static Color32 GridActive => ColorBlind ? new Color32(15,100,200, GridActiveOpacity) : new Color32(0, 160, 15, GridActiveOpacity);
        public static Color32 GridInvalid = new Color32(150, 0, 0, GridActiveOpacity);
        
        public static Color Green => ColorBlind ? new Color32(15,100,200,255) : new Color32(0, 160, 15, 255);
        public static Color Red = new Color(0.9f, 0.1f, 0.1f);
        public static readonly Color CostActive = new Color(0.8f,0.6f,0.2f);
        public static readonly Color CostInactive = new Color(0.85f,0.85f,0.85f);

        private static string GreenHex => ColorBlind ? "#0029ADFF" : "#007000FF";
        private const string RedHex = "#820000FF";
        private const string LightRedHex = "#FF1111FF";

        private const string EndTextColor = "</color>";
        private static string TextColor(string colorHex) => $"<color={colorHex}>";
        private static string TextColor(Color color) => $"<color={ColorUtility.ToHtmlStringRGB(color)}>";

        /// <summary>
        /// Sets the text color green or red based on status.
        /// </summary>
        /// <param name="status">
        /// 1 or more ➔ Green,
        /// 0 ➔ None,
        /// -1 or less ➔ Red
        /// </param>
        public static string StatusColor(this string s, int status, bool lightRed = false) => 
            status == 0 ? s : TextColor(status > 0 ? GreenHex : lightRed ? LightRedHex : RedHex) + s + EndTextColor;
        public static string Color(this string s, string colorHex) => TextColor(colorHex) + s + EndTextColor;
        public static string Color(this string s, Color color) => TextColor(color) + s + EndTextColor;

        public static readonly Dictionary<Stat, Color> StatColours = new Dictionary<Stat, Color>
        {
            {Stat.Brawler, new Color(0.7f, 0.3f, 0.3f)},
            {Stat.Outrider, new Color(0.25f, 0.45f, 0.2f)},
            {Stat.Performer, new Color(0.30f, 0.6f, 0.6f)},
            {Stat.Diviner, new Color(0.75f, 0.6f, 0.3f)},
            {Stat.Arcanist, new Color(0.6f, 0.3f, 0.75f)},
            {Stat.Spending, new Color(0.8f, 0.6f, 0f)},
            {Stat.Food, new Color(0.5f, 0.65f, 0f)},
            {Stat.Housing, new Color(0.6f, 0.4f, 0f)},
            {Stat.Defence, new Color(0.25f, 0.35f, 1f)},
            {Stat.Threat, new Color(0.4f, 0f, 0f)}
        };
    }
}
