using System;
using Cards;
using DG.Tweening;
using Inputs;
using Managers;
using Requests;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace UI
{
    public class Book : MonoBehaviour
    {
        [Serializable]
        private class BookGroup
        {
            public CanvasGroup canvasGroup;
            public Button bookRibbon;
        }

        public enum BookPage
        {
            Settings = 0,
            Guide = 1,
            Reports = 2,
            Upgrades = 3,
        }

        const float DEFAULT_RIBBON_HEIGHT = 128;
        const float EXPANDED_RIBBON_HEIGHT = 180f;
        
        // Public fields
        public static Action OnOpened;

        private static readonly Vector3 ClosePos = new Vector3(0, -1000, 0);
        private static readonly Vector3 PunchScale = Vector3.one * 0.5f;
        
        [SerializeField] private Canvas canvas;

        [SerializeField] private Button
            closeButton,
            quitButton,
            clearSaveButton,
            introSettingsButton;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private GameObject clearSaveText, confirmClearText, finalClearText;
        [SerializeField] private float animateInDuration = .5f;
        [SerializeField] private float animateOutDuration = .75f;
        [SerializeField] private SerializedDictionary<BookPage, BookGroup> pages;
        private GameState _closeState; // The state the book will enter when 
        private bool _isOpen, _transitioning, _changingPage;
        private bool _fromGame; // TODO this state cache must be removed, please do not use it
        private CanvasGroup _closeButtonCanvas;

        private int _confirmDeleteStep;
        private int ConfirmingDelete
        {
            get => _confirmDeleteStep;
            set
            {
                _confirmDeleteStep = value;
                clearSaveText.SetActive(value == 0);
                confirmClearText.SetActive(value == 1);
                finalClearText.SetActive(value == 2);
            }
        }

        [SerializeField] private ExtendedDropdown resDropdown;
        
        private BookPage _page = BookPage.Settings;
        private BookPage Page
        {
            set
            {
                ConfirmingDelete = 0;
                // Enable or disable the quit to menu button to avoid the raycaster affecting
                // other pages in the book
                // TODO this needs a second menu state available for in-menu/game vs in-menu/intro
                bool enableQuit = _fromGame && !Tutorial.Tutorial.Active && value == BookPage.Settings;
                quitButton.gameObject.SetActive(enableQuit);
                quitButton.interactable = enableQuit;
                quitButton.enabled = enableQuit;
                
                bool enableClear = !_fromGame && !Tutorial.Tutorial.Active && value == BookPage.Settings;
                clearSaveButton.gameObject.SetActive(enableClear);
                clearSaveButton.interactable = enableClear;
                clearSaveButton.enabled = enableClear;

                if (value == BookPage.Settings)
                {
                    sfxSlider.navigation = new Navigation
                    {
                        selectOnDown = _fromGame ? quitButton : clearSaveButton,
                        selectOnUp = sfxSlider.navigation.selectOnUp,
                        mode = Navigation.Mode.Explicit
                    };
                }
                if (value == BookPage.Reports) CardsBookList.ScrollActive = true;

                if (_page == value) return;
                _changingPage = true;
                
                Manager.Jukebox.PlayPageTurn();

                pages[_page].canvasGroup.interactable = false;
                pages[_page].canvasGroup.blocksRaycasts = false;
                var rt = pages[_page].bookRibbon.transform as RectTransform;
                rt.DOSizeDelta(new Vector2(rt.sizeDelta.x, DEFAULT_RIBBON_HEIGHT), 0.15f);
                pages[_page].canvasGroup.GetComponent<UIController>()?.OnClose();
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
            introSettingsButton.onClick.AddListener(() =>
            {
                if (Manager.State.InIntro) Open();
            });
            closeButton.onClick.AddListener(Close);
            quitButton.onClick.AddListener(() =>
            {
                _closeState = GameState.ToIntro;
                Close();
            });
            
            clearSaveButton.onClick.AddListener(() =>
            {
                if (ConfirmingDelete == 2) Globals.ResetGameSave();
                else ConfirmingDelete++;
            });
            
            foreach ((BookPage page, BookGroup group) in pages)
            {
                group.bookRibbon.onClick.AddListener(() => Page = page);
            }

            var rt = pages[_page].bookRibbon.transform as RectTransform;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, EXPANDED_RIBBON_HEIGHT);

            BookButton.OnClicked += (toUnlocks) =>
            {
                if (toUnlocks) Page = BookPage.Upgrades;
                Open();
            };
            
            RequestDisplay.OnNotificationClicked += () =>
            {
                if (!Manager.State.InGame) return;
                Page = BookPage.Upgrades;
                Open();
            };

            Tutorial.Tutorial.ShowBook += () =>
            {
                Page = BookPage.Upgrades;
                Open();
            };

            Manager.Inputs.ToggleBook.performed += _ =>
            {
                if (Manager.Cards.SelectedCard != null)
                {
                    Manager.Cards.SelectCard(-1);
                }
                else if (Structures.Select.Instance.SelectedStructure != null) Structures.Select.Instance.SelectedStructure = null;
                else if (Manager.State.InGame || Manager.State.InIntro || (Manager.State.InMenu && _isOpen)) Toggle();
            };

            Manager.Inputs.Close.performed += _ =>
            {
                if (!_isOpen || _transitioning || Manager.Upgrades.BoxOpen || resDropdown.isOpen) return;
                Close();
            };

            // Position it as closed on start
            transform.localPosition = ClosePos;
        }

        public void Open(BookPage page)
        {
            Page = page;
            Open();
        }
        
        public void Open()
        {
            if (!Manager.State.InGame && !Manager.State.InIntro) return;
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
                    if (!Manager.Inputs.UsingController) _closeButtonCanvas.DOFade(1, 0.5f);
                    _transitioning = false;
                    _isOpen = true;
                    Manager.Jukebox.PlayBookThump();
                    pages[_page].canvasGroup.GetComponent<UIController>().OnOpen();
                    _changingPage = false;
                    if (_page == BookPage.Reports) CardsBookList.ScrollActive = true;
                    OnOpened?.Invoke();
                });
            Manager.Inputs.NavigateBookmark.performed += OnNavigateBookmark_performed;
        }

        public void Close()
        {
            ConfirmingDelete = 0;
            _transitioning = true;
            closeButton.gameObject.SetActive(false); 
            Manager.SelectUi(null);          
            transform.DOPunchScale(PunchScale, animateOutDuration, 0, 0);
            transform.DOLocalMove(ClosePos, animateOutDuration)
                .OnComplete(() =>
                {
                    canvas.enabled = false;
                    _transitioning = false;
                    _isOpen = false;
                    Manager.State.EnterState(_closeState);
                });
            pages[_page].canvasGroup.GetComponent<UIController>()?.OnClose();
            Manager.Inputs.NavigateBookmark.performed -= OnNavigateBookmark_performed;
        }

        private void OnNavigateBookmark_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (_changingPage) return;
            var val = -(int)obj.ReadValue<float>();
            var newPage = Mathf.Abs(((int)_page + val + 4) % 4);
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
