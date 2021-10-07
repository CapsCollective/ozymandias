using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utilities;

namespace UI
{
    public class HintText : MonoBehaviour
    {
        private static readonly List<string> Hints = new List<string> {
            "Right click while placing a building to rotate",
            "The game saves automatically each new turn",
            "Enemy camps don't grow while their quest is active",
            "The cost of a single health potion could feed a family of 5 for a year",
            "Farms produce all year round, it's magic, don't question it",
            "Remember to keep a little bit in savings for emergencies!"
        };

        private void Awake()
        {
            GetComponent<TextMeshProUGUI>().text = Hints.SelectRandom();
        }
    }
}
