using Controllers;
using UnityEngine;
using Camera = UnityEngine.Camera;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class MapAnimator : MonoBehaviour
{
    public string floodTrigger = "Flood";
    public string drainTrigger = "Drain";
    public string effectOrigin = "_Origin";

    public bool flooded;

    private Animator _animator;
    private MeshRenderer _meshRenderer;
    private Camera _cam;
    private float radius = 0;
    private Color effectColor;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _cam = Camera.main;

        _meshRenderer.material.SetFloat("_Radius", 0);
        _meshRenderer.material.SetColor("_Effect", new Color(0, 0.3f, 0, 0));
    }

    private void LateUpdate()
    {
        UpdateEffectOrigin();
        
        if (BuildingPlacement.Selected != BuildingPlacement.Deselected && !flooded)
            Flood();
        
        if (BuildingPlacement.Selected == BuildingPlacement.Deselected && flooded)
            Drain();

    }

    private void Drain()
    {
        flooded = false;
        //_animator.SetTrigger(drainTrigger);
        DOTween.To(() => radius, x => radius = x, 0, 0.5f).OnUpdate(() => _meshRenderer.material.SetFloat("_Radius", radius));
        DOTween.To(() => effectColor, x => effectColor = x, new Color(0, 0.3f, 0, 0f), 0.5f).OnUpdate(() => _meshRenderer.material.SetColor("_Effect", effectColor));

    }

    private void Flood()
    {
        flooded = true;
        //_animator.SetTrigger(floodTrigger);
        DOTween.To(() => radius, x => radius = x, 70, 0.5f).OnUpdate(() => _meshRenderer.material.SetFloat("_Radius", radius));
        DOTween.To(() => effectColor, x => effectColor = x, new Color(0, 0.3f, 0, 0.5f), 0.5f).OnUpdate(() => _meshRenderer.material.SetColor("_Effect", effectColor));
    }

    private void UpdateEffectOrigin()
    {
        Ray ray = _cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _cam.nearClipPlane));
        Physics.Raycast(ray, out RaycastHit hit);

        _meshRenderer.material.SetVector(effectOrigin, hit.point);
    }
}
