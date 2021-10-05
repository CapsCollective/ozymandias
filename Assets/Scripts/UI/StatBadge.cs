using System;
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
            int satisfaction = Manager.Stats.GetSatisfaction(stat);
            glow.color = Color.clear;
            
            
            if (satisfaction != _oldSatisfaction)
            {
                PunchBadge();
                _oldSatisfaction = satisfaction;
            }
            
            Color color = satisfaction > 0 ? Colors.Green : Colors.Red;
            color.a = (Math.Abs(satisfaction) / 10) / 2f; // Intentional drop of fraction to fix value to 0, 0.5, 1
            glow.color = color;
        }
        
        private void PunchBadge()
        {
            if (_running) return;
            _running = true;
            GetComponent<RectTransform>()
                .DOPunchScale(new Vector3(0.2f,0.2f,0), 0.5f)
                .OnComplete(() => _running = false);
        }
    }
}
