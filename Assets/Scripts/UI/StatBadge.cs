using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace UI
{
    public class StatBadge : UiUpdater
    {
        [SerializeField] private Stat stat;
        [SerializeField] private TextMeshProUGUI count;
        [SerializeField] private Image glow;
        
        protected override void UpdateUi()
        {
            count.text = Manager.GetStat(stat).ToString();
            
            int satisfaction = Manager.GetSatisfaction(stat);
            glow.color = Color.clear;
            
            if(satisfaction >= 5) glow.color = Color.green;
            if(satisfaction <= -5) glow.color = Color.red;
        }
    }
}
