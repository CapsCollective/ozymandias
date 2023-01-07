using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using static Managers.GameManager;
using DG.Tweening;
using Cinemachine;

namespace UI {

    public class DebugUI : UIController
    {
        const float DEBUG_UPDATE_TIME = 0.1f;

        private Dictionary<string, object> settings = new Dictionary<string, object>();

        [SerializeField] private GameObject debugUI;
        [SerializeField] private GameObject grass;
        [SerializeField] private GameObject gameGUI;
        [SerializeField] private VolumeProfile postProcessProfile;
        [SerializeField] private VolumeProfile dofProcessProfile;
        [SerializeField] private UniversalRendererData rendererData;
        [SerializeField] private TMPro.TextMeshProUGUI settingsText;

        private CinemachineFreeLook camFreeLook;
        private bool toggleFPS = false;
        private float fps;
        private float timer;
        private int frames;
        private float[] frameCount;

        public void Start()
        {
            if (!Debug.isDebugBuild)
            {
                EnableDebugDisplay(false);
                return;
            }

            frameCount = new float[300];
            camFreeLook = Camera.main.GetComponent<CinemachineFreeLook>();
            Manager.Inputs.OnDebugToggle.performed += ToggleDebug;

            settings.Add("MainLightShadows", UnityGraphicsBullshit.MainLightCastShadows);
            settings.Add("ShadowDistance", UnityGraphicsBullshit.MaxShadowDistance);
            settings.Add("ShadowResolution", UnityGraphicsBullshit.MainLightShadowResolution);
            settings.Add("DrawDistance", camFreeLook.m_Lens.FarClipPlane);
            settings.Add("LODBias", QualitySettings.lodBias);
            settings.Add("Grass", grass.activeSelf);
            settings.Add("RenderScale", UnityGraphicsBullshit.RenderScale);
            settings.Add("SoftShadows", UnityGraphicsBullshit.SoftShadowsEnabled);
            settings.Add("FogStartDist", RenderSettings.fogEndDistance);
            UpdateText(false);

            EnableDebugDisplay(false);
        }

        private void EnableDebugDisplay(bool enable)
        {
            debugUI.SetActive(enable);
            settingsText.gameObject.SetActive(enable);
        }

        private void UpdateText(bool updateValues = true)
        {
            if (updateValues)
            {
                settings["MainLightShadows"] = UnityGraphicsBullshit.MainLightCastShadows;
                settings["ShadowDistance"] = UnityGraphicsBullshit.MaxShadowDistance;
                settings["ShadowResolution"] = UnityGraphicsBullshit.MainLightShadowResolution;
                settings["DrawDistance"] = camFreeLook.m_Lens.FarClipPlane;
                settings["LODBias"] = QualitySettings.lodBias;
                settings["Grass"] = grass.activeSelf;
                settings["RenderScale"] = UnityGraphicsBullshit.RenderScale;
                settings["SoftShadows"] = UnityGraphicsBullshit.SoftShadowsEnabled;
                settings["FogStartDist"] = RenderSettings.fogEndDistance;
            }

            StringBuilder sb = new StringBuilder("Settings");
            foreach(KeyValuePair<string, object> kvp in settings)
            {
                sb.AppendLine($"{kvp.Key}: {kvp.Value}");
            }
            settingsText.text = sb.ToString();
        }

        private void ToggleDebug(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            Debug.Log("Testing");
            debugUI.SetActive(!debugUI.activeSelf);
            if (debugUI.activeSelf) OnOpen();
            else OnClose();
            Manager.Inputs.TogglePlayerInput(!debugUI.activeSelf);
            settingsText.gameObject.SetActive(debugUI.activeSelf);
        }

        public void ToggleShadow(bool toggle)
        {
            UnityGraphicsBullshit.MainLightCastShadows = toggle;
            UpdateText();
        }

        public void ShadowDistance(float dist)
        {
            UnityGraphicsBullshit.MaxShadowDistance = dist;
            UpdateText();
        }

        public void SetShadowQuality(float q)
        {
            int pick = (int)q;
            UnityEngine.Rendering.Universal.ShadowResolution shadowResolution = UnityGraphicsBullshit.MainLightShadowResolution;
            switch (pick)
            {
                case 0:
                    shadowResolution = UnityEngine.Rendering.Universal.ShadowResolution._256;
                    break;
                case 1:
                    shadowResolution = UnityEngine.Rendering.Universal.ShadowResolution._512;
                    break;
                case 2:
                    shadowResolution = UnityEngine.Rendering.Universal.ShadowResolution._1024;
                    break;
                case 3:
                    shadowResolution = UnityEngine.Rendering.Universal.ShadowResolution._2048;
                    break;
                case 4:
                    shadowResolution = UnityEngine.Rendering.Universal.ShadowResolution._4096;
                    break;
            }
            UnityGraphicsBullshit.MainLightShadowResolution = shadowResolution;
            UpdateText();
        }

        public void ToggleFPS(bool t)
        {
            toggleFPS = t;
        }

        public void SetDrawDistance(float t)
        {
            camFreeLook.m_Lens.FarClipPlane = t;
            UpdateText();
        }

        public void SetLODBias(float b)
        {
            QualitySettings.lodBias = b;
            UpdateText();
        }

        public void ToggleGrass(bool toggle)
        {
            grass.SetActive(toggle);
            UpdateText();
        }

        public void ToggleClouds(bool toggle)
        {
            if(toggle)
                Shader.EnableKeyword("CLOUDS_ENABLED");
            else
                Shader.DisableKeyword("CLOUDS_ENABLED");
        }

        public void ToggleDOF(bool t)
        {
            dofProcessProfile.TryGet<DepthOfField>(out var dof);
            dof.active = t;
        }

        public void TogglePP(bool t)
        {
            rendererData.rendererFeatures[0].SetActive(t);
        }

        public void ToggleGUI(bool t)
        {
            gameGUI.SetActive(t);
        }

        public void SetRenderScale(float f)
        {
            UnityGraphicsBullshit.RenderScale = f;
            UpdateText();
        }

        public void ToggleSoftShadows(bool t)
        {
            UnityGraphicsBullshit.SoftShadowsEnabled = t;
            UpdateText();
        }

        public void SetFogDistance(float f)
        {
            RenderSettings.fogEndDistance = f;
            UpdateText();
        }
    }
}
