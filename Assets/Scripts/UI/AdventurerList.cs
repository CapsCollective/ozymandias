#pragma warning disable 0649
using System.Linq;
using UnityEngine;
using static Managers.GameManager;

namespace UI
{
    public class AdventurerList : MonoBehaviour
    {
        [SerializeField] private GameObject rowPrefab;

        public void Display()
        {
            foreach (Transform child in transform) Destroy(child.gameObject);

            foreach (Adventurer adventurer in Manager.Adventurers.OrderByDescending(x => x.assignedQuest ? x.assignedQuest.title : "").ThenByDescending(x => x.turnJoined))
            {
                GameObject row = Instantiate(rowPrefab, transform);
                row.GetComponent<AdventurerRow>().Display(adventurer);
            }
        }
    }
}
