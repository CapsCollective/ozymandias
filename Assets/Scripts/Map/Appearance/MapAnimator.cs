using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MapAnimator : MonoBehaviour
{
    public string floodTrigger = "Flood";
    public string drainTrigger = "Drain";
    public string effectOrigin = "_Origin";

    private Animator _animator;
    private MeshRenderer _meshRenderer;

    private int prevSelected = -1;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void LateUpdate()
    {
        UpdateEffectOrigin();

        if (PlacementController.Selected != prevSelected)
        {
            Debug.Log(PlacementController.Selected + " : " + prevSelected);

            if (PlacementController.Selected != PlacementController.Deselected)
                Fill();
            else
                Drain();
        }
        prevSelected = PlacementController.Selected;
    }

    public void Drain() { _animator.SetTrigger(drainTrigger); }

    public void Fill() { _animator.SetTrigger(floodTrigger); }

    private void UpdateEffectOrigin()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        Physics.Raycast(ray, out RaycastHit hit);

        _meshRenderer.material.SetVector(effectOrigin, hit.point);
    }
}
