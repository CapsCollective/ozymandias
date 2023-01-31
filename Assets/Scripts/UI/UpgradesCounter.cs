using Reports;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;

namespace UI
{
    public class UpgradesCounter : UiUpdater
    {
        [SerializeField] private Button upgradesButton, settingsButton, creditsButton;
        [SerializeField] private TextMeshProUGUI text;


        protected override void UpdateUi()
        {
            bool show = Manager.Achievements.Milestones[Milestone.TownsDestroyed] > 0;
            int purchasable = Manager.Upgrades.TotalPurchasable;
            text.text = purchasable.ToString();
            gameObject.SetActive(purchasable > 0);
            
            // Only show button after first game
            transform.parent.gameObject.SetActive(show);
            
            settingsButton.navigation = new Navigation
            {
                selectOnUp = settingsButton.navigation.selectOnUp,
                selectOnRight = settingsButton.navigation.selectOnRight,
                selectOnDown = show ? upgradesButton : creditsButton,
                mode = Navigation.Mode.Explicit
            };
            creditsButton.navigation = new Navigation
            {
                selectOnUp = show ? upgradesButton : settingsButton,
                selectOnRight = creditsButton.navigation.selectOnRight,
                selectOnDown = creditsButton.navigation.selectOnDown,
                mode = Navigation.Mode.Explicit
            };
        }
    }
}
