using DG.Tweening;
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
        private int _oldSatisfaction;
        private bool _running;
        
        protected override void UpdateUi()
        {
            int satisfaction = Manager.GetSatisfaction(stat);
            glow.color = Color.clear;
            
            
            if (satisfaction != _oldSatisfaction)
            {
                PunchBadge();
                _oldSatisfaction = satisfaction;
            }
            
            if(satisfaction >= 10) glow.color = new Color(0, 0.7f,0);
            if(satisfaction <= -10) glow.color = new Color(0.8f, 0,0);
        }
        
        private void PunchBadge()
        {
            if (_running) return;
            _running = true;
            GetComponent<RectTransform>()
                .DOPunchScale(new Vector3(0.2f,0.2f,0), 0.5f)
                .OnComplete(() => _running = false);        }
    }
}
