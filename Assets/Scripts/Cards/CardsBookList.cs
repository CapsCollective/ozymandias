using Buildings;
using UnityEngine;
using static Managers.GameManager;

namespace Cards
{
    public class CardsBookList : MonoBehaviour
    {
        [SerializeField] private GameObject cardDisplay; 
        
        private void Start()
        {
            foreach (GameObject building in Manager.Cards.StarterBuildings)
            {
                Instantiate(cardDisplay, transform)
                    .GetComponent<CardDisplay>()
                    .UpdateDetails(building.GetComponent<Building>());
            }
            foreach (GameObject building in Manager.Cards.UnlockableBuildings)
            {
                Instantiate(cardDisplay, transform)
                    .GetComponent<CardDisplay>()
                    .UpdateDetails(building.GetComponent<Building>());
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
