using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;
using DG.Tweening;

namespace UI
{
    public class Stability : UiUpdater
    {
        [SerializeField] private RectTransform mask, fill;
        [SerializeField] private Image chevron;
        [SerializeField] private List<Sprite> chevronSizes;
        
        private const float ChevronThreshold = 10;
        
        protected override void UpdateUi()
        {
            mask.DOLocalMoveY((Manager.Stability - 100) * 1.45f, 0.5f);
            fill.DOLocalMoveY((100 - Manager.Stability) * 1.45f, 0.5f);
            
            chevron.color = Manager.ChangePerTurn >= 0 ? new Color(0.37f, 0.73f, 0.19f) : new Color(0.82f, 0.17f, 0.14f);
            chevron.sprite = chevronSizes[Mathf.Min((int)Mathf.Ceil(Mathf.Abs(Manager.ChangePerTurn / ChevronThreshold)),3)];
        }
    }
}
