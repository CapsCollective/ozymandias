using System;
using Controllers;
using UnityEngine;
using Utilities;
using UnityEngine.UI;
using Entities;
using static Managers.GameManager;

public class BuildingClearer : MonoBehaviour
{
    [SerializeField] private Button _clearButton;
    private Cell _selected;
    private Camera _mainCamera;

    void Start()
    {
        _clearButton = GetComponentInChildren<Button>();
        
        Click.OnLeftClick += LeftClick;
        Click.OnRightClick += RightClick;

        Clear();

        CameraMovement.OnCameraMove += Clear;

        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (_selected != null && _clearButton.gameObject.activeSelf)
        {
            Vector3 buildingPosition = _selected.occupant.transform.position;
            _clearButton.transform.position = Vector3.Lerp(_clearButton.transform.position,
                _mainCamera.WorldToScreenPoint(buildingPosition) + (Vector3.up * 70.0f), 0.5f);
        }
    }

    void Clear()
    {
        _clearButton.gameObject.SetActive(false);
        _selected = null;
    }

    void LeftClick()
    {
        // If nothing is selected
        if (BuildingPlacement.Selected == -1 && _selected == null)
        {
            _selected = Manager.Map.GetCellFromMouse();

            if (_selected.occupant && _selected.occupant.type != BuildingType.Terrain)
            {
                Vector3 buildingPosition = _selected.occupant.transform.position;
                _clearButton.transform.position = _mainCamera.WorldToScreenPoint(buildingPosition) + (Vector3.up * 70.0f);
                _clearButton.gameObject.SetActive(true);
            }
        }
    }

    public void ClearBuilding()
    {
        Building occupant = _selected.occupant;
        if (_selected.occupant.indestructible || occupant.type == BuildingType.GuildHall) return;
        occupant.Clear();
        Manager.Buildings.Remove(occupant);
        Clear();
    }

    void RightClick()
    {
        Clear();
    }
}
