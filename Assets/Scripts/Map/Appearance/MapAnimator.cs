using Controllers;
using UnityEngine;
using Camera = UnityEngine.Camera;

[RequireComponent(typeof(Animator))]
public class MapAnimator : MonoBehaviour
{
    public Clear clear;

    public string floodTrigger = "Flood";
    public string drainTrigger = "Drain";
    public string effectOrigin = "_Origin";

    public bool flooded;

    private Animator _animator;
    private MeshRenderer _meshRenderer;
    private Camera _cam;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _cam = Camera.main;
    }

    private void LateUpdate()
    {
        UpdateEffectOrigin();

        var isClearing = clear && clear.toggle.isOn;

        if ((BuildingPlacement.Selected != BuildingPlacement.Deselected || isClearing) && !flooded)
            Flood();
        
        if ((BuildingPlacement.Selected == BuildingPlacement.Deselected && !isClearing) && flooded)
            Drain();

    }

    private void Drain()
    {
        flooded = false;
        _animator.SetTrigger(drainTrigger);
    }

    private void Flood()
    {
        flooded = true;
        _animator.SetTrigger(floodTrigger);
    }

    private void UpdateEffectOrigin()
    {
        Ray ray = _cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _cam.nearClipPlane));
        Physics.Raycast(ray, out RaycastHit hit);

        _meshRenderer.material.SetVector(effectOrigin, hit.point);
    }
}
