using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;
using static Managers.State;

namespace UI
{
    public class Shade : MonoBehaviour
    {
        private const float TransitionSpeed = 0.5f;
        private Image _image;
        private bool _active;

        private void Start()
        {
            _image = GetComponent<Image>();
            gameObject.SetActive(false);
            OnEnterState += () => SetDisplay(Manager.State.InMenu);
        }

        private void SetDisplay(bool display)
        {
            // Check if we are entering a new active state
            if (display == _active) return;
            _active = display;
            
            // Display the shade if inactive
            if (_active) { gameObject.SetActive(true); }

            // Run transition animation
            _image.DOColor(new Color(0.0f, 0.0f, 0.0f, _active ? 0.5f : 0.0f), 1.0f - TransitionSpeed)
            .OnComplete(() => { 
                // Hide the shade if faded out
                if (!_active) { gameObject.SetActive(false); }
            });
        }
    }
}
