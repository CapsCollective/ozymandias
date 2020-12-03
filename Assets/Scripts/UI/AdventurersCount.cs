using TMPro;
using static GameManager;

namespace UI
{
    public class AdventurersCount : UiUpdater
    {
        public TextMeshProUGUI adventurerText;
        public override void UpdateUi()
        {
            bool overCapacity = Manager.AvailableAdventurers - Manager.Accommodation > 0;
            adventurerText.text = "Adventurers: " + (overCapacity ? "<color=red>" : "") + Manager.AvailableAdventurers + (overCapacity ? "</color>" : "") + " / " + Manager.Accommodation;
        }
    }
}
