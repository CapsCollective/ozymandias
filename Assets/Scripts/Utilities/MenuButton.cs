using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class MenuButton : MonoBehaviour
    {
        public Action OnClicked;
        
        private Button _button;
        private ScaleOnHover _scaler;
        private bool _interactable = true;

        public bool Interactable
        {
            set
            {
                _interactable = value;
                SetInteractable(value);
            }
        }

        private void Start()
        {
            _scaler = GetComponent<ScaleOnHover>();
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() => OnClicked?.Invoke());

            State.OnNextTurnBegin += () => SetInteractable(false);
            State.OnNextTurnEnd += () => SetInteractable(true);
        }

        private void SetInteractable(bool b)
        {
            _scaler.interactable = _button.interactable = _interactable && b;
        }
    }
}