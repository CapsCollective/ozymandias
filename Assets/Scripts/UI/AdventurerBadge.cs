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
        [SerializeField] private AdventurerType type;
        [SerializeField] private TextMeshProUGUI count, countTicker;
        [SerializeField] private Image glow;
        private int _oldValue;
        private int _oldSatisfaction;
        private bool _running;
        
        protected override void UpdateUi()
        {
            int value = Manager.Adventurers.GetCount(type);
            int satisfaction = Manager.Stats.GetStat((Stat)type);

            if (value > _oldValue)
            {
                TriggerTicker(value - _oldValue);
                _oldValue = value;
            }

            if (satisfaction != _oldSatisfaction)
            {
                PunchBadge();
                _oldSatisfaction = satisfaction;
            }
            
            count.text = value.ToString();
            
            glow.color = Color.clear;
            
            if (satisfaction - value >= 5) glow.color = new Color(0, 0.7f,0);
            if (satisfaction - value <= -5) glow.color = new Color(0.8f, 0,0);
        }

        private void TriggerTicker(int amount)
        {
            countTicker.text = "+" + amount;
            countTicker.enabled = true;
            countTicker.alpha = 1;
            countTicker.rectTransform.localPosition = new Vector3(0, -110, 0);
            countTicker.DOFade(0, 1f);
            countTicker.rectTransform.DOLocalMove(new Vector3(0,-60,0), 1f);
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
