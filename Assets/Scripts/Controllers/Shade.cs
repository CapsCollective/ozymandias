using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class Shade : MonoBehaviour
    {
        // Constants
        private const float DefaultTransitionSpeed = 0.5f;
        
        // Public fields
        public static Action OnShadeOpened;

        // Instance field
        public static Shade Instance { get; private set; }
    
        // Private fields
        private Image _image;

        private void Awake()
        {
            Instance = this;
            _image = GetComponent<Image>();
            gameObject.SetActive(false);
        }

        public void SetDisplay(bool active, float transitionSpeed = DefaultTransitionSpeed)
        {
            // Invoke state change event
            if (active) OnShadeOpened?.Invoke();
            
            // Display the shade if inactive
            if (active) { gameObject.SetActive(true); }

            // Run transition animation
            _image.DOColor(
                new Color(0.0f, 0.0f, 0.0f, active ? 0.5f : 0.0f),
                1.0f - transitionSpeed).OnComplete(() => { 
                // Hide the shade if faded out
                if (!active) { gameObject.SetActive(false); }
            });
        }
    }
}
