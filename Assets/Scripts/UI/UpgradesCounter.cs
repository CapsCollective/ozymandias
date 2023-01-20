using TMPro;
using UnityEngine;
using static Managers.GameManager;

namespace UI
{
    public class UpgradesCounter : UiUpdater
    {
        [SerializeField] private TextMeshProUGUI text;


        protected override void UpdateUi()
        {
            int purchasable = Manager.Upgrades.TotalPurchasable;
            text.text = purchasable.ToString();
            gameObject.SetActive(purchasable > 0);
        }
    }
}
