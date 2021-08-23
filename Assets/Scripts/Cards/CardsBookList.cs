using Buildings;
using Managers;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Cards
{
    public class CardsBookList : MonoBehaviour
    {
        [SerializeField] private GameObject cardDisplayPrefab;

        private void Awake()
        {
            State.OnLoadingEnd += Display;
        }
        
        
        private void Display()
        {
            foreach (GameObject building in Manager.Cards.StarterBuildings)
            {
                Instantiate(cardDisplayPrefab, transform)
                    .GetComponent<CardDisplay>()
                    .UpdateDetails(building.GetComponent<Building>());
            }
            foreach (GameObject building in Manager.Cards.UnlockableBuildings)
            {
                CardDisplay card = Instantiate(cardDisplayPrefab, transform).GetComponent<CardDisplay>();
                card.gameObject.AddComponent<ScaleOnButtonHover>();
                bool isUnlocked = Manager.Cards.IsUnlocked(building);
                card.UpdateDetails(isUnlocked ?
                    building.GetComponent<Building>() :
                    null
                );

                if (!isUnlocked)
                {
                    // Update list on card unlock
                    Cards.OnUnlock += (unlocked) =>
                    {
                        if (unlocked.name == building.name)
                        {
                            card.UpdateDetails(building.GetComponent<Building>());
                        }
                    };
                }
            }
        }
    }
}
