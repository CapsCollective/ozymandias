using UI;
using UnityEngine;
using UnityEngine.UI;

public class Stability : UiUpdater
{
    
    [Range(0, 100)] public int stability;
    
    [SerializeField] private RectTransform mask, fill;
    private void Update()
    {
        mask.localPosition = new Vector3(0,(stability - 100) * 1.45f, 0);
        fill.localPosition = new Vector3(0,(100 - stability) * 1.45f , 0);

    }

    protected override void UpdateUi()
    {
        
    }
}
