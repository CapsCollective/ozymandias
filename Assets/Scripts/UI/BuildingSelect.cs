using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class BuildingSelect : UiUpdater
{
    public const int Deselected = -1;
    public int position;
    public GameObject buildingPrefab;
    public TextMeshProUGUI title;
    public Image icon;
    public Image iconTexture;
    public TextMeshProUGUI cost;
    public Toggle toggle;
    public CanvasGroup canvasGroup;
    [SerializeField] private Ease tweenEase;

    private Vector3 _initialPosition;
    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _initialPosition = _rectTransform.localPosition;
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
        bool interactable = building.ScaledCost <= Manager.Wealth;
        toggle.interactable = interactable;
        canvasGroup.alpha = interactable ? 1 : 0.4f;
    }

    public void ToggleSelect()
    {
        PlacementManager.Selected = toggle.isOn ? position : Deselected;
        SelectTween(toggle.isOn);
    }

    private void SelectTween(bool selected)
    {
        if (selected)
        {
            _rectTransform.DOLocalMove(_initialPosition + _rectTransform.transform.up * 60, 0.5f)
                .SetEase(tweenEase);
            _rectTransform.DOScale(new Vector3(1.1f, 1.1f), 0.5f).SetEase(tweenEase);
        }
        else
        {
            _rectTransform.DOLocalMove(_initialPosition, 0.5f).SetEase(tweenEase);
            _rectTransform.DOScale(Vector3.one, 0.5f).SetEase(tweenEase);
        }
    }
}
