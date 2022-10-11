using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;

namespace UI
{
    public class Stability : UiUpdater
    {
        [SerializeField] private Sprite[] chevrons;
        [SerializeField] private RectTransform threatBar, defenceBadge, threatBadge;
        [SerializeField] private Image direction;
        [SerializeField] private TextMeshProUGUI defenceCount, threatCount;
        private const float BarLength = 560f;
        private const float Height = 25;
        private int _oldDefence, _oldThreat;
        private bool _running;
        
        protected override void UpdateUi()
        {
            if (Manager.State.InMenu) return;

            int defence = Manager.Stats.Defence;
            int threat = Manager.Stats.Threat;
            int change = defence - threat;
            
            if (_oldDefence != defence)
            {
                defenceCount.text = defence.ToString();
                int unavailable = Manager.Adventurers.Unavailable;
                if (unavailable != 0) defenceCount.text += $"/{defence + unavailable}";
                PunchBadge(defenceBadge);
                _oldDefence = defence;
            }

            if (_oldThreat != threat)
            {
                threatCount.text = threat.ToString();
                PunchBadge(threatBadge);
                _oldThreat = threat;
            }
            
            float width = BarLength * (100 - Mathf.Max(Manager.Stats.Stability, 0)) / 100f;
            threatBar.DOSizeDelta(new Vector2(width, Height), 0.5f);

            direction.enabled = change != 0 && Manager.Stats.Stability > 0;
            direction.rectTransform.DOAnchorPosX(Mathf.Clamp(-width-30f, -520,-120), 0.5f);
            direction.rectTransform.DORotate(new Vector3(0,0, change > 0 ? 90: -90), 0.5f);
            direction.sprite = chevrons[Mathf.Clamp(Mathf.Abs(change) / 5, 0, 2)];
        }
        
        private void PunchBadge(RectTransform badge)
        {
            if (_running) return; 
            _running = true;
            badge
                .DOPunchScale(new Vector3(0.2f,0.2f,0), 0.5f)
                .OnComplete(() => _running = false);
        }
    }
}
