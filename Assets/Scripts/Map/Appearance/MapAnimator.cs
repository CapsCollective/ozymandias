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

    public AnimationClip drain;
    public AnimationClip flood;
    public AnimationClip entry;

    public bool flooded = false;

    private Animator _animator;
    private MeshRenderer _meshRenderer;

    private int prevSelected = -1;
    private bool prevClear = false;

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

        //if (PlacementController.Selected != prevSelected)
        //{
        //    Debug.Log(PlacementController.Selected + " : " + prevSelected);

        //    if (PlacementController.Selected != PlacementController.Deselected && _animator.GetCurrentAnimatorClipInfo(0)[0].clip != flood)
        //        Fill();
        //    else if(_animator.GetCurrentAnimatorClipInfo(0)[0].clip != drain || _animator.GetCurrentAnimatorClipInfo(0)[0].clip != entry)
        //        Drain();
        //}

        //if (clear.toggle.isOn != prevClear)
        //{
        //    if (clear.toggle.isOn && _animator.GetCurrentAnimatorClipInfo(0)[0].clip != flood)
        //        Fill();
        //    else if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip != drain || _animator.GetCurrentAnimatorClipInfo(0)[0].clip != entry)
        //        Drain();
        //}
        //prevSelected = PlacementController.Selected;
        //prevClear = clear.toggle.isOn;
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
