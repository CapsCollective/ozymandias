using System;
using Controllers;
using UnityEngine;
using Utilities;
using UnityEngine.UI;
using Entities;
using static Managers.GameManager;

public class BuildingClearer : MonoBehaviour
{
    private Button _clearButton;
    private Cell _selected;
    private float _oldDistanceToCamera = 0.0f;
    private Camera _mainCamera;
    private Vector3 _oldCameraPosition = Vector3.zero;

    void Start()
    {
        _clearButton = GetComponentInChildren<Button>();
        
        Click.OnLeftClick += LeftClick;
        Click.OnRightClick += RightClick;

        Clear();

        CameraMovement.OnCameraMove += Clear;
        CameraMovement.OnCameraRotate += OnCameraRotate;

        _mainCamera = Camera.main;
        _oldCameraPosition = _mainCamera.transform.position;
    }

    private void Update()
    {
        if (_selected != null && _clearButton.gameObject.activeSelf)
        {
            Vector3 buildingPosition = _selected.occupant.transform.position;
            Vector3 smoothedPos = Vector3.Lerp(_clearButton.transform.position,
                _mainCamera.WorldToScreenPoint(buildingPosition) + (Vector3.up * 70.0f), 0.5f);
            _clearButton.transform.position = smoothedPos;
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
        if (BuildingPlacement.Selected == -1)
        {
            _selected = Manager.Map.GetCellFromMouse();

            if (_selected.occupant && _selected.occupant.type != BuildingType.Terrain)
            {
                Vector3 buildingPosition = _selected.occupant.transform.position;
                _clearButton.transform.position = _mainCamera.WorldToScreenPoint(buildingPosition) + (Vector3.up * 50.0f);
                _clearButton.gameObject.SetActive(true);
            }
        }
    }

    void OnCameraRotate()
    {

    }

    void RightClick()
    {
        Clear();
    }
}
