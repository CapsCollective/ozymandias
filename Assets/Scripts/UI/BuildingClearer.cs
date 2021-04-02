using System;
using System.Linq;
using Controllers;
using UnityEngine;
using Utilities;
using UnityEngine.UI;
using Entities;
using TMPro;
using static Managers.GameManager;

namespace UI
{
    public class BuildingClearer : MonoBehaviour
    { 
        [SerializeField] int pixelsPerUnit;
        
        private Button _clearButton;
        private Cell _selected;
        private int _selectedDestroyCost = 0;
        private Camera _mainCamera;
        private int _numberOfTerrainTilesDeleted = 0;
        private TextMeshProUGUI _nameText, _costText;

        private const float ScaleSteps = 1.025f;
        // Modifier for building refunds. Currently set to provide players with a 75% refund. 
        private const float BuildingRefundModifier = 0.25f;
    
        void Start()
        {
            _clearButton = GetComponentInChildren<Button>();

            TextMeshProUGUI[] textFields = transform.GetChild(0).GetComponentsInChildren<TextMeshProUGUI>();

            _nameText = textFields[0];
            _costText = textFields[1];

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
                (Vector3.up * pixelsPerUnit), 
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

            Vector3 buildingPosition = _selected.occupant.transform.position;
    
            _clearButton.transform.position = _mainCamera.WorldToScreenPoint(buildingPosition) + (Vector3.up * pixelsPerUnit);
            
            _clearButton.gameObject.SetActive(true);

            int destructionCost = 0;
            string name = _selected.occupant.name;

            if (_selected.occupant.type == BuildingType.Terrain)
            {
                var range = Enumerable.Range(1, _numberOfTerrainTilesDeleted);
                destructionCost = (int) range.Select(i => 1.0f / Math.Pow(ScaleSteps, i)).Sum();
                name = "Terrain";
            }
            else
            {
                destructionCost = Mathf.FloorToInt(_selected.occupant.baseCost * BuildingRefundModifier);
            }

            float opacity = Manager.Wealth >= destructionCost ? 255f : 166f;

            _clearButton.interactable = Manager.Wealth >= destructionCost;

            _costText.text = destructionCost.ToString();

            // Change opacity of the text.
            Color oldColor = _costText.color;
            _costText.color = new Color(oldColor.r, oldColor.g, oldColor.b, opacity);
            _nameText.color = new Color(oldColor.r, oldColor.g, oldColor.b, opacity);

            _nameText.text = name;

            _selectedDestroyCost = destructionCost;
        }
    
        public void ClearBuilding()
        {
            Building occupant = _selected.occupant;
            
            if (_selected.occupant.indestructible || !Manager.Spend(_selectedDestroyCost)) return;

            if (_selected.occupant.type == BuildingType.Terrain)
            {
                _numberOfTerrainTilesDeleted += Manager.Map.GetCells(_selected.occupant).Length;
            }

            occupant.Clear();
            Manager.Buildings.Remove(occupant);
            Clear();
        }
    
        void RightClick()
        {
            Clear();
        }
    }
}

