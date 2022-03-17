using DG.Tweening;
using Requests;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace UI
{
    public class Book : MonoBehaviour
    {
        [System.Serializable]
        private class BookGroup
        {
            public CanvasGroup canvasGroup;
            public Button bookRibbon;
        }

        private enum BookPage
        {
            Settings,
            Progress,
            Unlocks
        }

        const float DEFAULT_RIBBON_HEIGHT = 160f;
        const float EXPANDED_RIBBON_HEIGHT = 200f;

        private static readonly Vector3 ClosePos = new Vector3(0, -1000, 0);
        private static readonly Vector3 PunchScale = Vector3.one * 0.5f;
        
        [SerializeField] private Canvas canvas;
        [SerializeField] private Button 
            closeButton, 
            quitButton, 
            introSettingsButton,
            settingsRibbon,
            progressRibbon,
            unlocksRibbon;
        [SerializeField] private float animateInDuration = .5f;
        [SerializeField] private float animateOutDuration = .75f;
        [SerializeField] private SerializedDictionary<BookPage, BookGroup> pages;
        private GameState _closeState; // The state the book will enter when 
        private bool _isOpen, _transitioning, _changingPage;
        private bool _fromGame; // TODO this state cache must be removed, please do not use it
        private CanvasGroup _closeButtonCanvas;
        
        private BookPage _page = BookPage.Settings;
        private BookPage Page
        {
            set
            {
                // Enable or disable the quit to menu button to avoid the raycaster affecting
                // other pages in the book
                // TODO this needs a second menu state available for in-menu/game vs in-menu/intro
                _changingPage = true;
                var enableQuit = _fromGame && !Tutorial.Tutorial.Active && value == BookPage.Settings;
                quitButton.gameObject.SetActive(enableQuit);
                quitButton.interactable = enableQuit;
                quitButton.enabled = enableQuit;
                
                if (_page == value) return;
                
                Manager.Jukebox.PlayPageTurn();

                pages[_page].canvasGroup.interactable = false;
                pages[_page].canvasGroup.blocksRaycasts = false;
                var rt = pages[_page].bookRibbon.transform as RectTransform;
                rt.DOSizeDelta(new Vector2(rt.sizeDelta.x, DEFAULT_RIBBON_HEIGHT), 0.15f);
                pages[_page].canvasGroup.DOFade(0, 0.2f).OnComplete(() =>
                {
                    _page = value;
                    pages[_page].canvasGroup.DOFade(1f, 0.2f);
                    pages[_page].canvasGroup.interactable = true;
                    pages[_page].canvasGroup.blocksRaycasts = true;
                    pages[_page].canvasGroup.GetComponent<UIController>()?.OnOpen();
                    var rt = pages[_page].bookRibbon.transform as RectTransform;
                    rt.DOSizeDelta(new Vector2(rt.sizeDelta.x, EXPANDED_RIBBON_HEIGHT), 0.15f);
                    _changingPage = false;
                });
            }
        }
        
        private void Start()
        {
            _closeButtonCanvas = closeButton.GetComponent<CanvasGroup>();
            introSettingsButton.onClick.AddListener(Open);
            closeButton.onClick.AddListener(Close);
            quitButton.onClick.AddListener(() =>
            {
                _closeState = GameState.ToIntro;
                Close();
            });
            
            settingsRibbon.onClick.AddListener(() => Page = BookPage.Settings);
            unlocksRibbon.onClick.AddListener(() => Page = BookPage.Unlocks);
            progressRibbon.onClick.AddListener(() => Page = BookPage.Progress);

            BookButton.OnClicked += (toUnlocks) =>
            {
                if (toUnlocks) Page = BookPage.Unlocks;
                Open();
            };
            
            RequestDisplay.OnNotificationClicked += () =>
            {
                if (!Manager.State.InGame) return;
                Page = BookPage.Progress;
                Open();
            };

            Manager.Inputs.OnToggleBook.performed += _ =>
            {
                if (Manager.Cards.SelectedCard != null)
                {
                    Manager.Cards.SelectCard(-1);
                }
                else if (Structures.Select.Instance.SelectedStructure != null) Structures.Select.Instance.SelectedStructure = null;
                else if (Manager.State.InGame || Manager.State.InIntro || (Manager.State.InMenu && _isOpen)) Toggle();
            };

            transform.localPosition = ClosePos;
        }

        private void Open()
        {
            _transitioning = true;
            _closeState = Manager.State.Current;
            _fromGame = Manager.State.InGame;
            Page = _page; // Update the current page settings
            Manager.State.EnterState(GameState.InMenu);
            canvas.enabled = true;
            transform.DOPunchScale(PunchScale, animateInDuration, 0, 0);
            transform.DOLocalMove(Vector3.zero, animateInDuration)
                .OnComplete(() =>
                {
                    closeButton.gameObject.SetActive(true);
                    _closeButtonCanvas.alpha = 0;
                    _closeButtonCanvas.DOFade(1, 0.5f);
                    _transitioning = false;
                    _isOpen = true;
                    Manager.Jukebox.PlayBookThump();
                    pages[_page].canvasGroup.GetComponent<UIController>().OnOpen();
                    _changingPage = false;
                });
            Manager.Inputs.OnNavigateBookmark.performed += OnNavigateBookmark_performed;
        }


        private void Close()
        {
            _transitioning = true;
            closeButton.gameObject.SetActive(false);
            SelectUi(null);          
            transform.DOPunchScale(PunchScale, animateOutDuration, 0, 0);
            transform.DOLocalMove(ClosePos, animateOutDuration)
                .OnComplete(() =>
                {
                    canvas.enabled = false;
                    _transitioning = false;
                    _isOpen = false;
                    Manager.State.EnterState(_closeState);
                });

            Manager.Inputs.OnNavigateBookmark.performed -= OnNavigateBookmark_performed;
        }

        private void OnNavigateBookmark_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (_changingPage) return;
            var val = -(int)obj.ReadValue<float>();
            var newPage = Mathf.Abs(((int)_page + val + 3) % 3);
            Page = (BookPage)newPage;
        }

        private void Toggle()
        {
            if (_transitioning) return;
            if (_isOpen) Close();
            else Open();
        }
    }
}
