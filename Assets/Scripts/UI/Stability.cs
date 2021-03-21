using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;
using DG.Tweening;

namespace UI
{
    public class Stability : UiUpdater
    {
        [SerializeField] private Image bar;
        
        protected override void UpdateUi()
        {
            if (Manager.Stability > 0)
            {
                bar.transform.DOScaleX(Manager.Stability/100f, 0.5f);
            }
            else
            {
                bar.transform.DOScaleX(0, 0.5f).OnComplete(() =>
                {
                    bar.color = Color.clear;
                });
            }
        }
    }
}
