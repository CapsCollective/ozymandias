using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;


public class FogColorChange : MonoBehaviour
{
    public MeshRenderer mr;
    private GameManager gm;
    public Transform blocker;
    
    [SerializeField] private float ratio;

    [SerializeField] private Color origEm;
    [SerializeField] private Color currentEm;

    [SerializeField] private Color origCol;
    [SerializeField] private Color currentCol;

    [SerializeField] private float origBlend;
    [SerializeField] private float currentBlend;

    [SerializeField] private float origBlocker;
    [SerializeField] private float currentBlocker;

    [SerializeField] private Color origFogCol;
    [SerializeField] private Color currentFogCol;

    [SerializeField] private float origFogDensity;
    [SerializeField] private float currentFogDensity;

    public Color deadEm;
    public Color deadCol;
    
    public float finalBlend;
    public float finalBlocker;
    public Color finalFogCol;
    public float finalFogDensity;

    // Start is called before the first frame update
    void Start()
    {
        origEm = mr.material.GetColor("_EmissionColor");
        origCol = mr.material.GetColor("_Color");
        origBlend = mr.material.GetFloat("_DistortionBlend");
        if (blocker)
        {
            origBlocker = blocker.localScale.x;
        }
        origFogCol = RenderSettings.fogColor;
        origFogDensity = RenderSettings.fogDensity;
    }

    // Update is called once per frame
    void Update()
    {
        SetColor();
    }

    public void SetColor()
    {
        ratio = ((float)gm.Threat / ((float)gm.Defense + (float)gm.Threat))-0.2f;
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
