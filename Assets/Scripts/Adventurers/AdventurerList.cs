using UnityEngine;
using static GameState.GameManager;

namespace Adventurers
{
    // The UI List of all adventurers
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
