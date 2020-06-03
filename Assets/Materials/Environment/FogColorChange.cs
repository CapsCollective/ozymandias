using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;


public class FogColorChange : MonoBehaviour
{
    public MeshRenderer mr;
    public Transform blocker;
    
    private float ratio;
    //private float oldRatio;
    //private float newRatio;
    //public float lerpTime=1f;

    private Color origEm;
    private Color currentEm;

    private Color origCol;
    private Color currentCol;

    private float origBlend;
    private float currentBlend;

    private float origBlocker;
    private float currentBlocker;

    private Color origFogCol;
    private Color currentFogCol;

    private float origFogDensity;
    private float currentFogDensity;

    public Color deadEm;
    public Color deadCol;
    
    public float finalBlend;
    public float finalBlocker;
    public Color finalFogCol;
    public float finalFogDensity;

    private bool isSetup = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private bool Setup()
    {
        //oldRatio = ((float)Manager.Threat / ((float)Manager.Defense + (float)Manager.Threat)) - 0.2f;
        origEm = mr.material.GetColor("_EmissionColor");
        origCol = mr.material.GetColor("_Color");
        origBlend = mr.material.GetFloat("_DistortionBlend");
        if (blocker)
        {
            origBlocker = blocker.localScale.x;
        }
        origFogCol = RenderSettings.fogColor;
        origFogDensity = RenderSettings.fogDensity;

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSetup && Manager)
        {
            isSetup = Setup();
        }
        else if(isSetup)
        {
            SetColor();
        }
        
    }

    //public void UpdateFog()
    //{
    //    newRatio = ((float)Manager.Threat / ((float)Manager.Defense + (float)Manager.Threat)) - 0.2f;
    //    StartCoroutine(ShiftingFog());
    //}

    //private IEnumerator ShiftingFog()
    //{
    //    float updateRatio = oldRatio;
    //    while (Mathf.Abs(newRatio-updateRatio) > 0.001)
    //    {
    //        currentEm = Color.Lerp(origEm, deadEm, updateRatio);
    //        currentCol = Color.Lerp(origCol, deadCol, updateRatio);
    //        currentBlend = Mathf.Lerp(origBlend, finalBlend, updateRatio);
    //        currentBlocker = Mathf.Lerp(origBlocker, finalBlocker, updateRatio);
    //        currentFogCol = Color.Lerp(origFogCol, finalFogCol, ratio);
    //        currentFogDensity = Mathf.Lerp(origFogDensity, finalFogDensity, updateRatio);

    //        mr.material.SetColor("_EmissionColor", currentEm);
    //        mr.material.SetColor("_Color", currentCol);
    //        mr.material.SetFloat("_DistortionBlend", currentBlend);
    //        blocker.localScale = new Vector3(currentBlocker, currentBlocker, currentBlocker);
    //        RenderSettings.fogColor = currentFogCol;
    //        RenderSettings.fogDensity = currentFogDensity;
    //        RenderSettings.ambientLight = currentFogCol;

    //        updateRatio += (ratio - oldRatio) * Time.deltaTime / lerpTime;
    //        yield return null;
    //    }


    //    oldRatio = ratio;
    //    yield return null;
    //}


    public void SetColor()
    {
        ratio = ((float)Manager.Threat / ((float)Manager.Defense + (float)Manager.Threat))-0.2f;

        currentEm = Color.Lerp(origEm, deadEm, ratio);
        currentCol = Color.Lerp(origCol, deadCol, ratio);
        currentBlend = Mathf.Lerp(origBlend, finalBlend, ratio);
        currentBlocker = Mathf.Lerp(origBlocker, finalBlocker, ratio);
        currentFogCol = Color.Lerp(origFogCol, finalFogCol, ratio);
        currentFogDensity = Mathf.Lerp(origFogDensity, finalFogDensity, ratio);

        mr.material.SetColor("_EmissionColor", currentEm);
        mr.material.SetColor("_Color", currentCol);
        mr.material.SetFloat("_DistortionBlend", currentBlend);
        blocker.localScale = new Vector3(currentBlocker, currentBlocker, currentBlocker);
        RenderSettings.fogColor = currentFogCol;
        RenderSettings.fogDensity = currentFogDensity;
        RenderSettings.ambientLight = currentFogCol;
    }
}
