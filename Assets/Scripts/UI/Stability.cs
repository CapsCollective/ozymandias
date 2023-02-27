using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;

namespace UI
{
    public class Stability : UiUpdater
    {
        [SerializeField] private Sprite[] chevrons;
        [SerializeField] private RectTransform defenceBadge, threatBadge;
        [SerializeField] private Image direction;
        [SerializeField] private TextMeshProUGUI defenceCount, threatCount;
        private const float BarStart = 55f;
        private const float BarLength = 530f;
        private const float DirectionArrowStart = 115f;
        private const float DirectionArrowEnd = 525f;
        private int _oldDefence, _oldThreat;
        private float _linearOldStability, _linearStability;
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
            
            float width = BarLength * (Mathf.Max(Manager.Stats.Stability, 0)) / 100f;
            float target = Manager.Stats.Stability / 100.0f;
            float stability = _linearStability;
            DOTween.To(() => stability, x => stability = x, target, 0.5f).OnUpdate(() =>
            {
                Shader.SetGlobalFloat("Stability", stability);
            }).OnComplete(() =>
            {
                Shader.SetGlobalFloat("Stability", stability);
                _linearStability = stability;
            });

            Shader.SetGlobalFloat("StabilityIntensity", Mathf.Abs(change) / 15.0f);
            direction.enabled = change != 0 && Manager.Stats.Stability > 0;
            direction.rectTransform.DOAnchorPosX(Mathf.Clamp(BarStart + width, DirectionArrowStart, DirectionArrowEnd), 0.5f);
            direction.rectTransform.DORotate(new Vector3(0,0, change > 0 ? 90: -90), 0.5f);

            int chevronIndex = 0;
            if (Mathf.Abs(change) > 5) chevronIndex = 1;
            if (Mathf.Abs(change) > 15) chevronIndex = 2;
            direction.sprite = chevrons[chevronIndex];
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
