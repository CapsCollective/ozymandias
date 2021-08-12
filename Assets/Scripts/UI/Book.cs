using System;
using DG.Tweening;
using Inputs;
using UnityEngine;
using UnityEngine.UI;
using static GameState.GameManager;

namespace UI
{
    public class Book : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Button closeButton;
        [SerializeField] private float animateInDuration = .5f;
        [SerializeField] private float animateOutDuration = .75f;
        
        private void Awake()
        {
            closeButton.onClick.AddListener(Close);
            Close();
        }

        public void Open()
        {
            canvas.enabled = true;
            Manager.EnterMenu();
            Manager.Jukebox.PlayScrunch();
            transform.DOLocalMove(Vector3.zero, animateInDuration);
            transform.DOPunchScale(Vector3.one * 1.2f, animateInDuration, 0)
                .OnComplete(() => closeButton.gameObject.SetActive(true));
        }

        private void Close()
        {
            Manager.ExitMenu();
            closeButton.gameObject.SetActive(false);
            transform.DOLocalMove(new Vector3(0, -1000, 0), animateOutDuration);
            transform.DOPunchScale(Vector3.one * 1.2f, animateOutDuration, 0)
                .OnComplete(() => canvas.enabled = false);
            UIEventController.SelectUI(null);
        }
    }
}
