using System;
using Managers;
using Structures;
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;

namespace Cards
{
    public class CardsBookList : MonoBehaviour
    {
        public static bool ScrollActive = false;
        
        [SerializeField] private GameObject cardDisplayPrefab;

        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private float speed;
        
        private void Awake()
        {
            State.OnLoadingEnd += Display;
            State.OnEnterState += _ => ScrollActive = false;
        }

        private void Update()
        {
            if (!ScrollActive) return;
            scrollbar.value += Manager.Inputs.Scroll.ReadValue<Vector2>().y * speed * Time.deltaTime;
        }

        private void Display()
        {
            foreach (Blueprint blueprint in Manager.Cards.All)
            {
                bool isUnlocked = Manager.Cards.IsUnlocked(blueprint);
                
                CardDisplay card = Instantiate(cardDisplayPrefab, transform).GetComponentInChildren<CardDisplay>();
                card.UpdateDetails(isUnlocked || blueprint.starter ? blueprint : null);
                
                // Update list on card unlock
                if(!isUnlocked) Cards.OnUnlock += (unlocked) =>
                {
                    if (unlocked.type == blueprint.type) card.UpdateDetails(blueprint);
                };
            }
        }
    }
}
