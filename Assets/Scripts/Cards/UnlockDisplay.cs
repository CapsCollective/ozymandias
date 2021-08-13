using System.Collections.Generic;
using System.Linq;
using Buildings;
using DG.Tweening;
using Events;
using TMPro;
using UnityEngine;
using static GameState.GameManager;

namespace Cards
{
    public class UnlockDisplay : MonoBehaviour
    {
        [SerializeField] private CardDisplay cardDisplay;
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
            
            Cards.OnUnlock += building =>
            {
                _buildings.Push(building);
            };

            Cards.OnDiscoverRuin += OpenCard;
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
            Manager.Jukebox.PlayScrunch();

            var cardTransform = cardDisplay.transform;
            cardTransform.localPosition = new Vector3(1000, 200, 0);
            cardTransform.localRotation = new Quaternion( 0.0f, 0.0f, 10.0f, 0.0f);
            
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
            cardDisplay.transform.DOLocalMove(new Vector3(-300, -500, 0), animateOutDuration);
            cardDisplay.transform.DOScale(Vector3.one * 0.4f, animateOutDuration);
            cardDisplay.transform
                .DOLocalRotate(new Vector3(0, 0, 20), animateOutDuration)
                .OnComplete(() =>
                {
                    cardDisplay.transform.localScale = Vector3.one;
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
