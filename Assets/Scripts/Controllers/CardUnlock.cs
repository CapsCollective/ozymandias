using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Entities;
using Managers;
using TMPro;
using UI;
using UnityEngine;
using static Managers.GameManager;

namespace Controllers
{
    public class CardUnlock : MonoBehaviour
    {
        [SerializeField] private BuildingCardDisplay cardDisplay;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private float animateInDuration = 2.0f;
        [SerializeField] private float animateOutDuration = 2.0f;

        private Vector3 _originalPos;
        private Canvas _canvas;
        private Stack<Building> _buildings = new Stack<Building>();
        private Building _displayBuilding;

        private void Start()
        {
            _canvas = GetComponent<Canvas>();
            _originalPos = cardDisplay.transform.localPosition;
            
            BuildingCards.OnUnlock += building =>
            {
                _buildings.Push(building);
            };

            Newspaper.OnClosed += OpenCard;

            Close();
        }

        private void OpenCard()
        {
            if (!_buildings.Any() || _displayBuilding) return;

            _displayBuilding = _buildings.Pop();
            cardDisplay.UpdateDetails(_displayBuilding);
            Open();
        }

        private void Open()
        {
            Manager.EnterMenu();
            _canvas.enabled = true;
            Jukebox.Instance.PlayScrunch();

            var cardTransform = cardDisplay.transform;
            cardTransform.localPosition = new Vector3(1000, 200, 0);
            cardTransform.localRotation = new Quaternion( 0.0f, 0.0f, 20.0f, 0.0f);
            
            cardDisplay.transform.DOLocalRotate(Vector3.zero, animateInDuration);
            cardDisplay.transform
                .DOLocalMove(_originalPos, animateInDuration)
                .OnComplete(() =>
                {
                    text.DOFade(1.0f, 0.5f);
                });
        }
        
        public void Close()
        {
            text.DOFade(0.0f, 0.5f);
            cardDisplay.transform.DOLocalMove(new Vector3(-500, -300, 0), animateOutDuration);
            cardDisplay.transform
                .DOLocalRotate(new Vector3(0, 0, 20), animateOutDuration)
                .OnComplete(() =>
                {
                    _displayBuilding = null;
                    if (_buildings.Any()) OpenCard();
                    else
                    {
                        _canvas.enabled = false;
                        Manager.ExitMenu();
                    }
                });
        }
    }
}
