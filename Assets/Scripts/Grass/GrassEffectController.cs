using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Grass
{
    public class GrassEffectController : MonoBehaviour
    {
        public enum GrassQualitySettings 
        { 
            Off = 0,
            Low = 1,
            Medium = 2,
            High = 3,
        };

        public static bool GrassOn;
        public static bool GrassNeedsUpdate = false;

        public GrassQualitySettings _grassQuality;
        [SerializeField] private VisualEffect _grassEffect;
        [SerializeField] private CustomRenderTexture _grassRT;
        [SerializeField] private Camera _grassCam;

        // Start is called before the first frame update
        void Start()
        {
            Managers.State.OnLoadingEnd += () => GrassNeedsUpdate = true;
            Structures.Structures.OnBuild += (s) => GrassNeedsUpdate = true;
            Structures.Structures.OnDestroy += (s) => GrassNeedsUpdate = true;
        }

        public void UpdateTexture(Structures.Structure s)
        {
            _grassCam.enabled = true;
            _grassRT.Update();
        }

        // Update is called once per frame
        void Update()
        {
            if (!GrassNeedsUpdate)
                _grassCam.enabled = false;

            if (GrassNeedsUpdate)
            {
                Debug.Log("Updating Grass");
                UpdateTexture(null);
                GrassNeedsUpdate = false;
            }
        }
    }
}
