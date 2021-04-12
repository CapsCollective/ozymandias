using System.Collections;
using TMPro;
using UnityEngine;
using static Managers.GameManager;

namespace UI
{
    public class WealthCounter : UiUpdater
    {
        [SerializeField] private TextMeshProUGUI wealth;
        [SerializeField] private TextMeshProUGUI wealthPerTurn;

        private int _wpt;
        private int _previousWealth = 0;
        private int _targetWealth = 0;

        // Update is called once per frame
        protected override void UpdateUi()
        {
            _wpt = Manager.WealthPerTurn;
            wealth.text = _targetWealth.ToString();
            wealthPerTurn.text = "+" + _wpt;
            if (_targetWealth == Manager.Wealth) return; // Don't double update
            _previousWealth = _targetWealth;
            _targetWealth = Manager.Wealth;
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
            wealth.text = Manager.Wealth.ToString();
            wealthPerTurn.text = "+" + Manager.WealthPerTurn;
        }
    }
}
