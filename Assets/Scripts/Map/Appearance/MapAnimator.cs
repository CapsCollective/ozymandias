using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MapAnimator : MonoBehaviour
{
    public Clear clear;

    public string floodTrigger = "Flood";
    public string drainTrigger = "Drain";
    public string effectOrigin = "_Origin";

    public bool flooded = false;

    private Animator _animator;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void LateUpdate()
    {
        UpdateEffectOrigin();

        if ((PlacementController.Selected != PlacementController.Deselected || clear.toggle.isOn) && !flooded)
            Flood();
        
        if ((PlacementController.Selected == PlacementController.Deselected && !clear.toggle.isOn) && flooded)
            Drain();

    }

    public void Drain()
    {
        flooded = false;
        _animator.SetTrigger(drainTrigger);
    }

    public void Flood()
    {
        flooded = true;
        _animator.SetTrigger(floodTrigger);
    }

    private void UpdateEffectOrigin()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        Physics.Raycast(ray, out RaycastHit hit);

        _meshRenderer.material.SetVector(effectOrigin, hit.point);
    }
}
