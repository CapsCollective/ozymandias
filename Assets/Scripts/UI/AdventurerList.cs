using System.Linq;
using UnityEngine;
using static GameManager;

namespace UI
{
    public class AdventurerList : MonoBehaviour
    {
        public GameObject rowPrefab;

        public void Display()
        {
            foreach (Transform child in transform) Destroy(child.gameObject);

            foreach (Adventurer adventurer in Manager.adventurers.OrderByDescending(x => x.assignedQuest ? x.assignedQuest.title : "").ThenByDescending(x => x.turnJoined))
            {
                GameObject row = Instantiate(rowPrefab, transform);
                row.GetComponent<AdventurerRow>().Display(adventurer);
            }
        }
    }
}
