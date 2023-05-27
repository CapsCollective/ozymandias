using UI;
using Utilities;
using static Managers.GameManager;

namespace Upgrades
{
    public class FirstUpgradeTip : UiUpdater
    {
        protected override void UpdateUi()
        {
            gameObject.SetActive(Manager.Upgrades.GetLevel(UpgradeType.Discoveries) == 0);
        }
    }
}
