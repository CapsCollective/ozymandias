using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace UI
{
    public class AdventurerBadge : UiUpdater
    {
        [SerializeField] private AdventurerCategory category;
        [SerializeField] private TextMeshProUGUI count;
        [SerializeField] private Image glow;
        
        protected override void UpdateUi()
        {
            count.text = Manager.Adventurers.GetCount(category).ToString();
            
            int satisfaction = Manager.GetSatisfaction(category);
            glow.color = Color.clear;
            
            if(satisfaction >= 5) glow.color = new Color(0, 0.7f,0);
            if(satisfaction <= -5) glow.color = new Color(0.8f, 0,0);
        }
    }
}
