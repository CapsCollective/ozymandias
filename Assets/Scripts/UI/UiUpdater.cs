using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UiUpdater : MonoBehaviour
{

    private void Awake()
    {
        GameManager.OnUpdateUI += UpdateUi;
    }

    private void OnDestroy()
    {
        GameManager.OnUpdateUI -= UpdateUi;
    }

    public abstract void UpdateUi();
}
