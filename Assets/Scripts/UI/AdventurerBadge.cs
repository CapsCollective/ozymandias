using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace UI
{
    public class AdventurerBadge : UiUpdater
    {
        [SerializeField] private Guild guild;
        [SerializeField] private TextMeshProUGUI count, countTicker;
        [SerializeField] private Image glow;
        private int _oldValue;
        private int _oldSatisfaction;
        private bool _running;
        
        protected override void UpdateUi()
        {
            if (Manager.State.InMenu) return;
            
            int value = Manager.Adventurers.GetCount(guild, true);
            int satisfaction = Manager.Stats.GetStat((Stat)guild);

            if (value != _oldValue)
            {
                TriggerTicker(value - _oldValue, value);
                _oldValue = value;
            }

            if (satisfaction != _oldSatisfaction)
            {
                PunchBadge();
                _oldSatisfaction = satisfaction;
            }
            
            Color color = satisfaction - value > 0 ? Colors.Green : Colors.Red;
            color.a = Math.Abs(satisfaction - value) / 5f;
            glow.color = color;
        }

        private void TriggerTicker(int amount, int total)
        {
            countTicker.text = (amount > 0 ? "+" : Colors.RedText) + amount + (amount > 0 ? "" : Colors.EndText);
            countTicker.enabled = true;
            countTicker.alpha = 1;
            countTicker.rectTransform.localPosition = new Vector3(0, -110, 0);
            countTicker.DOFade(0, 1f).SetDelay(1f);
            countTicker.rectTransform
                .DOLocalMove(new Vector3(0,-60,0), 1f)
                .SetDelay(1f)
                .OnComplete(() => count.text = total.ToString());
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
