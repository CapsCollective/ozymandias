using Controllers;
using DG.Tweening;
using UnityEngine;
using Camera = UnityEngine.Camera;

namespace Environment
{
    public class MapAnimator : MonoBehaviour
    {
        private static readonly int Radius = Shader.PropertyToID("_Radius");
        private static readonly int Effect = Shader.PropertyToID("_Effect");
        private static readonly int Origin = Shader.PropertyToID("_Origin");

        private bool _flooded;
        private MeshRenderer _meshRenderer;
        private Camera _cam;
        private float _radius;
        private Color _effectColor;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _cam = Camera.main;

            _meshRenderer.material.SetFloat(Radius, 0);
            _meshRenderer.material.SetColor(Effect, new Color(0, 0.3f, 0, 0));
        }

        private void LateUpdate()
        {
            //TODO: Do we need to do this every frame, or only when it's updating?
            UpdateEffectOrigin();
        
            if (BuildingPlacement.Selected != BuildingPlacement.Deselected && !_flooded) Flood();
        
            if (BuildingPlacement.Selected == BuildingPlacement.Deselected && _flooded) Drain();
        }

        private void Drain()
        {
            _flooded = false;
            //_animator.SetTrigger(drainTrigger);
            DOTween.To(() => _radius, x => _radius = x, 0, 0.5f).OnUpdate(() => _meshRenderer.material.SetFloat(Radius, _radius));
            DOTween.To(() => _effectColor, x => _effectColor = x, new Color(0, 0.3f, 0, 0f), 0.5f).OnUpdate(() => _meshRenderer.material.SetColor(Effect, _effectColor));

        }

        private void Flood()
        {
            _flooded = true;
            //_animator.SetTrigger(floodTrigger);
            DOTween.To(() => _radius, x => _radius = x, 70, 0.5f).OnUpdate(() => _meshRenderer.material.SetFloat(Radius, _radius));
            DOTween.To(() => _effectColor, x => _effectColor = x, new Color(0, 0.3f, 0, 0.5f), 0.5f).OnUpdate(() => _meshRenderer.material.SetColor(Effect, _effectColor));
        }

        private void UpdateEffectOrigin()
        {
            Ray ray = _cam.ScreenPointToRay(new Vector3(InputManager.MousePosition.x, InputManager.MousePosition.y, _cam.nearClipPlane));
            Physics.Raycast(ray, out RaycastHit hit);

            _meshRenderer.material.SetVector(Origin, hit.point);
        }
    }
}
