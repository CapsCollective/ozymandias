using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class Colors
    {
        public static bool ColorBlind { get; set; }
        
        public static readonly Color CardLight = new Color(0.94f, 0.93f, 0.86f);
        public static readonly Color CardDark = new Color(0.16f, 0.13f, 0.07f);

        public static byte GridInactiveOpacity = 51;
        public static byte GridOpacity = 153;
        public static Color32 GridInactive => new Color32(51, 51, 51, GridInactiveOpacity);
        public static Color32 GridActive => ColorBlind ? new Color32(12,123,220, GridOpacity) : new Color32(0, 179, 26, GridOpacity);
        public static Color32 GridInvalid => ColorBlind ? new Color32(255,194,10, GridOpacity) : new Color32(230, 0, 0, GridOpacity);
        public static Color32 GridHighlighted => ColorBlind ? new Color32(235, 204, 52, GridOpacity) : new Color32(235, 204, 52, GridOpacity);
        
        public static Color Green => ColorBlind ? new Color32(12,123,220,255) : new Color(0,0.7f,0.1f);
        public static Color Red => ColorBlind ? new Color32(255,194,10,255) : new Color(1f, 0.1f, 0.1f);
        public static readonly Color CostActive = new Color(0.8f,0.6f,0.2f);
        public static readonly Color CostInactive = new Color(0.85f,0.85f,0.85f);

        private const string GreenHex = "#007000ff";
        private const string RedHex = "#820000ff";
        private const string LightRedHex = "#FF1111ff";

        private const string EndTextColor = "</color>";
        private static string TextColor(string colorHex) => $"<color={colorHex}>".Conditional(!ColorBlind);
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
            ColorBlind || status == 0 ? s : TextColor(status > 0 ? GreenHex : lightRed ? LightRedHex : RedHex) + s + EndTextColor;
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
