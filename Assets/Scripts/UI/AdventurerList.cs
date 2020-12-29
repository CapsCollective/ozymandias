#pragma warning disable 0649
using System.Linq;
using Entities;
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

            foreach (Adventurer adventurer in Manager.Adventurers.List)
            {
                GameObject row = Instantiate(rowPrefab, transform);
                row.GetComponent<AdventurerRow>().Display(adventurer);
            }
        }
    }
}
