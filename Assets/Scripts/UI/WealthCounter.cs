using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace UI
{
    public class WealthCounter : UiUpdater
    {
        [SerializeField] private TextMeshProUGUI wealth;
        [SerializeField] private TextMeshProUGUI wealthPerTurn;
        [SerializeField] private RectTransform spendingTag;

        private int _wpt;
        private int _previousWealth = 0;
        private int _targetWealth = 0;
        private int _previousSpending = 0;
        private bool _running = false;

        protected override void UpdateUi()
        {
            if (_previousSpending != Manager.Stats.GetStat(Stat.Spending))
            {
                PunchBadge();
                _previousSpending = Manager.Stats.GetStat(Stat.Spending);
            }
            
            _wpt = Manager.Stats.WealthPerTurn;
            wealth.text = _targetWealth.ToString();
            wealthPerTurn.text = "+" + _wpt;
            if (_targetWealth == Manager.Stats.Wealth) return; // Don't double update
            _previousWealth = _targetWealth;
            _targetWealth = Manager.Stats.Wealth;
            StartCoroutine(Scale());
        }

        IEnumerator Scale()
        {
            for (float t = 0; t < 0.3f; t += Time.deltaTime)
            {
                int w = (int)Mathf.Lerp(_previousWealth, _targetWealth, t / 0.3f);
                wealth.text = w.ToString();
                yield return null;
            }
            wealth.text = Manager.Stats.Wealth.ToString();
            wealthPerTurn.text = "+" + Manager.Stats.WealthPerTurn;
        }
        
        private void PunchBadge()
        {
            if (_running) return; 
            _running = true;
            spendingTag
                .DOPunchScale(new Vector3(0.2f,0.2f,0), 0.5f)
                .OnComplete(() => _running = false);
        }
    }
}
