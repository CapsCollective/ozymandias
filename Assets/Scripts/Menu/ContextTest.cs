using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextTest : MonoBehaviour
{
    [ContextMenu("ClearPlayerPref")]
    void ClearPlayerPref()
    {
        PlayerPrefs.DeleteAll();
    }
}
