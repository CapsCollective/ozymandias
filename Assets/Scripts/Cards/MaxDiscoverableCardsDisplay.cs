using TMPro;
using UI;
using Utilities;
using static Managers.GameManager;

namespace Cards
{
    public class MaxDiscoverableCardsDisplay : UiUpdater
    {
        private TextMeshProUGUI _text;

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        protected override void UpdateUi()
        {
            _text.text = "Max Cards Discoverable in Ruins: " + Manager.Upgrades.GetLevel(UpgradeType.Discoveries);
        }
    }
}
