using System.Linq;
using Managers;
using Structures;
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;

namespace Cards
{
    public class CardsBookList : MonoBehaviour
    {
        public static bool ScrollActive;
        
        [SerializeField] private GameObject cardDisplayPrefab;

        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private float speed;
        
        private void Awake()
        {
            State.OnEnterState += _ => ScrollActive = false;
            scrollRect.enabled = true;
            scrollbar.value = 0;
            
            // Update list on card unlock
            State.OnLoadingEnd += Display;
            Cards.OnUnlock += (_,_) => Display();
            State.OnNewGame += Display;
            Settings.OnToggleColorBlind += _ => Display();
        }

        private void Update()
        {
            if (!ScrollActive) return;
            scrollbar.value += Manager.Inputs.Scroll.ReadValue<Vector2>().y * speed * Time.deltaTime;
        }

        private void Display()
        {
            foreach (Transform child in transform) Destroy(child.gameObject);
            
            // Order starters first, then playable, then previously unlocked (but not playable), then locked
            var sorted = Manager.Cards.All.OrderBy(c =>
            {
                if (c.starter) return 0;
                if (Manager.Cards.IsPlayable(c)) return 1;
                if (Manager.Cards.IsUnlocked(c)) return 2;
                return 3;
            });
            
            foreach (Blueprint blueprint in sorted)
            {
                bool isUnlocked = Manager.Cards.IsUnlocked(blueprint);
                bool isPlayable = Manager.Cards.IsPlayable(blueprint);
                
                CardDisplay card = Instantiate(cardDisplayPrefab, transform).GetComponentInChildren<CardDisplay>();
                card.UpdateDetails(isUnlocked || blueprint.starter ? blueprint : null, isPlayable);
            }
        }
    }
}
