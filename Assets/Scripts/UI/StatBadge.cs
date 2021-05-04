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
        [SerializeField] private Image glow;
        
        protected override void UpdateUi()
        {
            int satisfaction = Manager.GetSatisfaction(stat);
            glow.color = Color.clear;
            
            if(satisfaction >= 10) glow.color = new Color(0, 0.7f,0);
            if(satisfaction <= -10) glow.color = new Color(0.8f, 0,0);
        }
    }
}
