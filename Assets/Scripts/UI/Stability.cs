using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;
using DG.Tweening;

namespace UI
{
    public class Stability : UiUpdater
    {
        [SerializeField] private Image bar;
        private const float MaxWidth = 623f;
        private const float Height = 36;
        
        protected override void UpdateUi()
        {
            bar.rectTransform.DOSizeDelta(new Vector2(Mathf.Max(0,MaxWidth * Manager.Stability/100f), Height), 0.5f);
        }
    }
}
