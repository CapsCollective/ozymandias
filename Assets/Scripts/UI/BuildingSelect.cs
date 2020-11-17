using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameManager;

namespace UI
{
    public class BuildingSelect : UiUpdater, IPointerEnterHandler, IPointerExitHandler
    {
        private const int Deselected = -1;
        
        public int position;
        public GameObject buildingPrefab;
        public TextMeshProUGUI title;
        public Image icon;
        public Image iconTexture;
        public TextMeshProUGUI cost;
        public Toggle toggle;
        
        [SerializeField] private Ease tweenEase;
        [SerializeField] private BuildingSelect[] siblingCards;
        [SerializeField] private Image cardBack;
        [SerializeField] private Texture2D unselectedBacking;
        [SerializeField] private Texture2D selectedBacking;

        private Vector3 _initialPosition;
        private RectTransform _rectTransform;
    
        private Sprite _unselectedBackingSprite;
        private Sprite _selectedBackingSprite;
    
        private bool _selected;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _initialPosition = _rectTransform.localPosition;
        
            _unselectedBackingSprite = Sprite.Create(
                unselectedBacking, new Rect(0, 0, unselectedBacking.width, unselectedBacking.height), 
                new Vector2(0.5f, 0.5f));
            _selectedBackingSprite = Sprite.Create(
                selectedBacking, new Rect(0, 0, selectedBacking.width, selectedBacking.height), 
                new Vector2(0.5f, 0.5f));
        }

        public override void UpdateUi()
        {
            BuildingStats building = buildingPrefab.GetComponent<BuildingStats>();
            Color colour = building.IconColour;
            title.text = building.name;
            title.color = colour;
            cost.text = building.ScaledCost.ToString();
            icon.sprite = building.icon;
            iconTexture.color = colour;
            bool active = building.ScaledCost <= Manager.Wealth;
            if (toggle.isOn && !active)
            {
                toggle.isOn = false;
                PlacementManager.Selected = Deselected;
            }
            bool interactable = IsInteractable(building);
            toggle.interactable = interactable;
            cardBack.color = interactable ? Color.white : new Color(0.8f, 0.8f, 0.8f, 1.0f);
        }
    
        private bool IsInteractable(BuildingStats building)
        {
            return building.ScaledCost <= Manager.Wealth;
        }

        public void ToggleSelect()
        {
            PlacementManager.Selected = toggle.isOn ? position : Deselected;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _rectTransform.DOLocalMove(_initialPosition + _rectTransform.transform.up * 60, 0.5f)
                .SetEase(tweenEase);
            _rectTransform.DOScale(new Vector3(1.1f, 1.1f), 0.5f).SetEase(tweenEase);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_selected) return;
            _rectTransform.DOLocalMove(_initialPosition, 0.5f).SetEase(tweenEase);
            _rectTransform.DOScale(Vector3.one, 0.5f).SetEase(tweenEase);
        }

        public void OnClicked()
        {
            if (!IsInteractable(buildingPrefab.GetComponent<BuildingStats>())) return;
            _selected = !_selected;
            OnPointerEnter(null);
            Array.ForEach(siblingCards, card => card.Deselect());
            cardBack.sprite = _selected ? _selectedBackingSprite : _unselectedBackingSprite;
        }
    
        private void Deselect()
        {
            _selected = false;
            OnPointerExit(null);
            cardBack.sprite = _unselectedBackingSprite;
        }
    }
}
