using Managers;
using Structures;
using UnityEngine;
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
            foreach (Blueprint blueprint in Manager.Cards.All)
            {
                bool isUnlocked = Manager.Cards.IsUnlocked(blueprint);
                
                CardDisplay card = Instantiate(cardDisplayPrefab, transform).GetComponent<CardDisplay>();
                card.UpdateDetails(isUnlocked || blueprint.starter ? blueprint : null);
                
                // Update list on card unlock
                if(isUnlocked) Cards.OnUnlock += (unlocked) =>
                {
                    if (unlocked.type == blueprint.type) card.UpdateDetails(blueprint);
                };
            }
        }
    }
}
