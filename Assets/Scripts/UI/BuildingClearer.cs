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

    [SerializeField] int _pixelsPerUnit;
    
    private Button _clearButton;
    private Cell _selected;
    private Vector3 _selectedSize = Vector3.zero;
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
        
        Vector3 buildingPosition = _selected.occupant.transform.position;

        _clearButton.transform.position = Vector3.Lerp(
            _clearButton.transform.position,
            _mainCamera.WorldToScreenPoint(buildingPosition) + 
            (Vector3.up * _pixelsPerUnit), 
            0.5f);
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
        
        Vector3 size = _selected.occupant.GetComponentInChildren<MeshRenderer>().bounds.size;

        Vector3 buildingPosition = _selected.occupant.transform.position;

        _clearButton.transform.position = _mainCamera.WorldToScreenPoint(buildingPosition) + (Vector3.up * _pixelsPerUnit);
        
        _clearButton.gameObject.SetActive(true);

        _selectedSize = size;
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
