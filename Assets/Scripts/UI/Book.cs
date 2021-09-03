using DG.Tweening;
using Inputs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace UI
{
    public class Book : MonoBehaviour
    {
        private enum BookPage
        {
            Settings,
            Progress,
            Unlocks
        }

        private static readonly Vector3 ClosePos = new Vector3(0, -1000, 0);
        private static readonly Vector3 PunchScale = Vector3.one * 0.5f;
        
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup closeButtonCanvasGroup;
        [SerializeField] private Button 
            closeButton, 
            quitButton, 
            introSettingsButton, 
            openBookButton, 
            settingsRibbon,
            progressRibbon,
            unlocksRibbon;
        [SerializeField] private float animateInDuration = .5f;
        [SerializeField] private float animateOutDuration = .75f;
        [SerializeField] private SerializedDictionary<BookPage, CanvasGroup> pages;
        private GameState _closeState; // The state the book will enter when 
        private bool _isOpen, _transitioning;
        
        private BookPage _page = BookPage.Settings;
        private BookPage Page
        {
            set
            {
                 if (_page == value) return;
                
                 //TODO: Page turn sound effect
                
                pages[_page].interactable = false;
                pages[_page].blocksRaycasts = false;
                pages[_page].DOFade(0, 0.2f).OnComplete(() =>
                {
                    _page = value;
                    pages[_page].DOFade(1f, 0.2f);
                    pages[_page].interactable = true;
                    pages[_page].blocksRaycasts = true;
                });
            }
        }
        
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
            
            settingsRibbon.onClick.AddListener(() => Page = BookPage.Settings);
            unlocksRibbon.onClick.AddListener(() => Page = BookPage.Unlocks);
            progressRibbon.onClick.AddListener(() => Page = BookPage.Progress);

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
                    closeButtonCanvasGroup.alpha = 0;
                    closeButtonCanvasGroup.DOFade(1, 0.5f);
                    _transitioning = false;
                    _isOpen = true;
                });
        }

        private void Close()
        {
            _transitioning = true;
            Manager.State.EnterState(_closeState);
            closeButton.gameObject.SetActive(false);
            SelectUi(null);
            
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
