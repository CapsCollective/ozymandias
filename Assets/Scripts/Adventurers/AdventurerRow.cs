using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using static Managers.GameManager;

namespace Adventurers
{
    public class AdventurerRow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText, guildText, daysText, locationText;

        public void Display(Adventurer adventurer)
        {
            nameText.text = adventurer.name;
            guildText.text = adventurer.guild.ToString();
            daysText.text = (Manager.Stats.TurnCounter - adventurer.turnJoined) + " Days";
            locationText.text = adventurer.assignedQuest ? adventurer.assignedQuest.Title : "In Town";
        }
    }
}
