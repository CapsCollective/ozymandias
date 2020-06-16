using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;


public class FogColorChange : MonoBehaviour
{
    public MeshRenderer mr;
    public Transform blocker;
    public Transform oceanBlocker;
    public SpriteRenderer waterBlockSR;
    
    private float ratio;
    private float oldRatio;
    private float newRatio;
    public float lerpTime = 1f;
    private float t = 0f;

    private Color origEm;
    private Color currentEm;

    private Color origCol;
    private Color currentCol;

    private float origBlend;
    private float currentBlend;

    private float origBlocker;
    private float currentBlocker;

    private float origOceanBlocker;
    private float currentOceanBlocker;

    private float origOceanAlpha =1f;
    private float currentOceanAlpha;
    public float finalOceanAlpha;



    private Color origFogCol;
    private Color currentFogCol;

    private float origFogFarDist;
    private float currentFogFarDist;

    private float origFogNearDist;
    private float currentFogNearDist;

    public Color deadEm;
    public Color deadCol;
    
    public float finalBlend;
    public float finalBlocker;
    public Color finalFogCol;
    public float finalFogNearDist;
    public float finalFogFarDist;

    private bool isSetup = false;

    private void Awake()
    {
        OnUpdateUI += UpdateFog;
    }

    private bool Setup()
    {
        oldRatio = Manager.ThreatLevel / 100f;
        origEm = mr.material.GetColor("_EmissionColor");
        origCol = mr.material.GetColor("_Color");
        origBlend = mr.material.GetFloat("_DistortionBlend");
        if (blocker)
        {
            origBlocker = blocker.localScale.x;
        }
        if (oceanBlocker)
        {
            origOceanBlocker = oceanBlocker.localScale.y;
        }
        origFogCol = RenderSettings.fogColor;
        origFogNearDist = RenderSettings.fogStartDistance;
        origFogFarDist = RenderSettings.fogEndDistance;

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSetup && Manager)
        {
            isSetup = Setup();
        }
    }

    public void UpdateFog()
    {
        if (isSetup)
        {
            newRatio = (Manager.ThreatLevel / 100f)-0.25f;
            StartCoroutine(ShiftingFog());
        }
        
    }

    private IEnumerator ShiftingFog()
    {
        while (t<lerpTime)
        {
            float updateRatio = Mathf.Lerp(oldRatio, newRatio, t / lerpTime);

            currentEm = Color.Lerp(origEm, deadEm, updateRatio);
            currentCol = Color.Lerp(origCol, deadCol, updateRatio);
            currentBlend = Mathf.Lerp(origBlend, finalBlend, updateRatio);
            currentBlocker = Mathf.Lerp(origBlocker, finalBlocker, updateRatio);
            currentOceanBlocker = Mathf.Lerp(origOceanBlocker, 0f, updateRatio);
            currentOceanAlpha = Mathf.Lerp(origOceanAlpha, finalOceanAlpha, updateRatio);
            currentFogCol = Color.Lerp(origFogCol, finalFogCol, updateRatio);
            currentFogNearDist = Mathf.Lerp(origFogNearDist, finalFogNearDist, updateRatio);
            currentFogFarDist = Mathf.Lerp(origFogFarDist, finalFogFarDist, updateRatio);

            mr.material.SetColor("_EmissionColor", currentEm);
            mr.material.SetColor("_Color", currentCol);
            mr.material.SetFloat("_DistortionBlend", currentBlend);
            blocker.localScale = new Vector3(currentBlocker, currentBlocker, currentBlocker);
            waterBlockSR.color = new Color(waterBlockSR.color.r, waterBlockSR.color.g, waterBlockSR.color.b, currentOceanAlpha);
            //oceanBlocker.localScale = new Vector3(oceanBlocker.localScale.x, currentOceanBlocker, oceanBlocker.localScale.z);
            RenderSettings.fogColor = currentFogCol;
            RenderSettings.fogStartDistance = currentFogNearDist;
            RenderSettings.fogEndDistance = currentFogFarDist;
            RenderSettings.ambientLight = currentFogCol;

            t += Time.deltaTime;
            yield return null;
        }
        t = 0f;
        oldRatio = newRatio;
    }


    //public void SetColor()
    //{
    //    ratio = Manager.ThreatLevel / 100f;

    //    currentEm = Color.Lerp(origEm, deadEm, ratio);
    //    currentCol = Color.Lerp(origCol, deadCol, ratio);
    //    currentBlend = Mathf.Lerp(origBlend, finalBlend, ratio);
    //    currentBlocker = Mathf.Lerp(origBlocker, finalBlocker, ratio);
    //    currentFogCol = Color.Lerp(origFogCol, finalFogCol, ratio);
    //    currentFogDensity = Mathf.Lerp(origFogDensity, finalFogDensity, ratio);

    //    mr.material.SetColor("_EmissionColor", currentEm);
    //    mr.material.SetColor("_Color", currentCol);
    //    mr.material.SetFloat("_DistortionBlend", currentBlend);
    //    blocker.localScale = new Vector3(currentBlocker, currentBlocker, currentBlocker);
    //    RenderSettings.fogColor = currentFogCol;
    //    RenderSettings.fogDensity = currentFogDensity;
    //    RenderSettings.ambientLight = currentFogCol;
    //}
}
