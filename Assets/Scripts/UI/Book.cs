using DG.Tweening;
using Inputs;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace UI
{
    public class Book : MonoBehaviour
    {
        private static readonly Vector3 ClosePos = new Vector3(0, -1000, 0);
        private static readonly Vector3 PunchScale = Vector3.one * 0.5f;
        
        [SerializeField] private Canvas canvas;
        [SerializeField] private Button closeButton, quitButton, introSettingsButton, openBookButton;
        [SerializeField] private float animateInDuration = .5f;
        [SerializeField] private float animateOutDuration = .75f;
        private GameState _closeState; // The state the book will enter when 
        private bool _isOpen, _transitioning;
        private void Start()
        {
            introSettingsButton.onClick.AddListener(Open);
            openBookButton.onClick.AddListener(Open);
            closeButton.onClick.AddListener(Close);
            quitButton.onClick.AddListener(() =>
            {
                _closeState = GameState.ToIntro;
                Close();
            });

            Manager.Inputs.IA_ShowPause.performed += _ =>
            {
                if(Manager.State.InGame || Manager.State.InIntro || (Manager.State.InMenu && _isOpen)) Toggle();
            };
            
            transform.localPosition = ClosePos;
        }

        private void Open()
        {
            _transitioning = true;
            _closeState = Manager.State.Current;
            quitButton.gameObject.SetActive(Manager.State.InGame);
            Manager.State.EnterState(GameState.InMenu);
            //Manager.Jukebox.PlayScrunch(); TODO: book sound
            canvas.enabled = true;
            transform.DOPunchScale(PunchScale, animateInDuration, 0, 0);
            transform.DOLocalMove(Vector3.zero, animateInDuration)
                .OnComplete(() =>
                {
                    closeButton.gameObject.SetActive(true);
                    _transitioning = false;
                    _isOpen = true;
                });
        }

        private void Close()
        {
            _transitioning = true;
            Manager.State.EnterState(_closeState);
            closeButton.gameObject.SetActive(false);
            UIEventController.SelectUI(null);
            transform.DOPunchScale(PunchScale, animateOutDuration, 0, 0);
            transform.DOLocalMove(ClosePos, animateOutDuration)
                .OnComplete(() =>
                {
                    canvas.enabled = false;
                    _transitioning = false;
                    _isOpen = false;
                });
        }

        private void Toggle()
        {
            if (_transitioning) return;
            if (_isOpen) Close();
            else Open();
        }
    }
}
