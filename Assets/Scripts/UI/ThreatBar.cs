#pragma warning disable 0649
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;

namespace UI
{
    public class ThreatBar : UiUpdater
    {
        private const int BarWidth = 510;
    
        [SerializeField] private RectTransform threatArea, nextTurnThreatArea, nextTurnDefenseArea;
        [SerializeField] private Image nextTurnThreatFill, nextTurnDefenseFill;

        private float _t;

        protected override void UpdateUi()
        {
            int nextTurn = Manager.ThreatLevel + Manager.ChangePerTurn;
            threatArea.sizeDelta = new Vector2(BarWidth * Manager.ThreatLevel / 100f, threatArea.sizeDelta.y);
            nextTurnThreatArea.sizeDelta = new Vector2(BarWidth * nextTurn / 100f, nextTurnThreatArea.sizeDelta.y);
            nextTurnDefenseArea.sizeDelta = new Vector2(BarWidth * (1 - nextTurn / 100f), nextTurnDefenseArea.sizeDelta.y);
        }

        public void Update()
        {
            _t += Time.deltaTime * 3;
            Color color = nextTurnThreatFill.color;
            color.a = Mathf.Lerp(0.1f, 0.4f, (Mathf.Sin(_t)+1)/2);
            nextTurnThreatFill.color = color;
            color = nextTurnDefenseFill.color;
            color.a = Mathf.Lerp(0.1f, 0.4f, (Mathf.Sin(_t)+1)/2);
            nextTurnDefenseFill.color = color;
        }
    }
}
