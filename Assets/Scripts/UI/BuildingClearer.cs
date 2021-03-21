using System;
using Controllers;
using UnityEngine;
using Utilities;
using UnityEngine.UI;
using Entities;
using static Managers.GameManager;

public class BuildingClearer : MonoBehaviour
{
    public Vector2 buttonOffset;
    
    private Button _clearButton;
    private Cell _selected;
    private Camera _mainCamera;

    void Start()
    {
        _clearButton = GetComponentInChildren<Button>(); 
        
        Click.OnLeftClick += LeftClick;
        Click.OnRightClick += RightClick;

        CameraMovement.OnCameraMove += Clear;

        _mainCamera = Camera.main;
        
        Clear();
    }

    private void Update()
    {
        if (_selected == null || !_clearButton.gameObject.activeSelf) return;

        MeshRenderer renderer = _selected.occupant.GetComponentInChildren<MeshRenderer>();
        Vector3 buildingPosition = _selected.occupant.transform.position;
        _clearButton.transform.position = Vector3.Lerp(_clearButton.transform.position,
            _mainCamera.WorldToScreenPoint(buildingPosition) + new Vector3(buttonOffset.x, buttonOffset.y, 0.0f), 0.5f);
    }

    void Clear()
    {
        _clearButton.gameObject.SetActive(false);
        _selected = null;
    }

    void LeftClick()
    {
        // If nothing is selected
        if (BuildingPlacement.Selected != -1 || _selected != null) return;
        
        _selected = Manager.Map.GetCellFromMouse();

        if (!_selected.occupant) return;
        Vector3 buildingPosition = _selected.occupant.transform.position;
        _clearButton.transform.position =
            _mainCamera.WorldToScreenPoint(buildingPosition) + new Vector3(buttonOffset.x, buttonOffset.y, 0.0f);
        _clearButton.gameObject.SetActive(true);
    }

    public void ClearBuilding()
    {
        // TODO: Work out what the exact mechanics are for removing things (how much does it cost?)
        Building occupant = _selected.occupant;
        if (_selected.occupant.indestructible) return;
        occupant.Clear();
        Manager.Buildings.Remove(occupant);
        Clear();
    }

    void RightClick()
    {
        Clear();
    }
}

internal class SerlializeFieldAttribute : Attribute
{
}
