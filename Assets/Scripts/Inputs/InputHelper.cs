using Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Utilities;
using DG.Tweening;
using UI;
using static Managers.GameManager;

namespace Inputs
{
    // The sole purpose of this script is to help give access to PlayerInput
    public class InputHelper : MonoBehaviour
    {
        public static PlayerInput PlayerInput;
        public static EventSystem EventSystem;
        public static Action<GameObject> OnNewSelection;
        public static Action<bool> OnToggleCursor;
        public static Dictionary<GameObject, Vector2> CursorOffsetOverrides = new Dictionary<GameObject, Vector2>();

        [SerializeField] private RectTransform selectionHelper;
        private bool HelperActive
        {
            get => selectionHelper.gameObject.activeSelf;
            set => selectionHelper.gameObject.SetActive(value);
        }

        private GameObject worldSpaceCursor;
        private GameObject lastSelectedGameObject;
        private Dictionary<GameState, GameObject> previousSelections = new Dictionary<GameState, GameObject>();
        private Tween tween;

        // Start is called before the first frame update
        void Start()
        {
            EventSystem = FindObjectOfType<EventSystem>();
            PlayerInput = GetComponent<PlayerInput>();

            State.OnEnterState += StateChecks;
            Inputs.OnControlChange += OnControlChanged;
            OnNewSelection += NewSelection;
            UIController.OnUIOpen += (g, b) =>
            {
                if (!Manager.Inputs.UsingController) return;
                EventSystem.SetSelectedGameObject(g);
                HelperActive = b;
            };

            worldSpaceCursor = GetComponent<Cinemachine.CinemachineFreeLook>().m_Follow.GetChild(0).gameObject;
            Manager.Inputs.WorldSpaceCursor = worldSpaceCursor.transform;
            worldSpaceCursor.GetComponentInChildren<Renderer>().material.SetFloat("_Opacity", 0);
            HelperActive = false;

            OnToggleCursor += ToggleUICursor;
        }

        private void Update()
        {
            if (EventSystem.currentSelectedGameObject != lastSelectedGameObject)
            {
                OnNewSelection?.Invoke(EventSystem.currentSelectedGameObject);
            }
                
            lastSelectedGameObject = EventSystem.currentSelectedGameObject;
        }

        private void NewSelection(GameObject obj)
        {
            Debug.Log("Prev: " + (lastSelectedGameObject?.name ?? "null") + ", New: " + (obj?.name ?? "null"));
            if (!Manager.Inputs.UsingController || EventSystem.currentSelectedGameObject == null) return;
            
            previousSelections[Manager.State.Current] = obj;
            
            selectionHelper.SetParent(obj.transform);
            selectionHelper.anchoredPosition = CursorOffsetOverrides.ContainsKey(obj) ? CursorOffsetOverrides[obj] : Vector2.zero;
            selectionHelper.localScale = Vector3.one;
            selectionHelper.eulerAngles = Vector3.zero;
        }

        private void OnControlChanged(InputControlScheme obj)
        {
            bool isController = Manager.Inputs.UsingController;
            if (Manager.State.Current == GameState.InGame)
                ToggleWorldCursor(isController);
            if (Manager.State.Current == GameState.InMenu)
            {
                if (!isController) HelperActive = false;
                Cursor.visible = !isController;
            }
        }

        public void ResetSelection(GameState state)
        {
            previousSelections[state] = null;
        }

        // I'll fill this up with stuff when I can.
        private void StateChecks(GameState state)
        {
            if (Manager.Inputs.UsingController)
            {
                switch (state)
                {
                    case GameState.InCredits:
                        EventSystem.SetSelectedGameObject(null);
                        break;
                    case GameState.InIntro:
                        break;
                    case GameState.ToCredits:
                        EventSystem.SetSelectedGameObject(null);
                        break;
                    case GameState.Loading:
                        break;
                    case GameState.ToIntro:
                        ToggleWorldCursor(false);
                        break;
                    case GameState.ToGame:
                        EventSystem.SetSelectedGameObject(null);
                        HelperActive = false;
                        break;
                    case GameState.InGame:
                        ToggleWorldCursor(true); 
                        HelperActive = false;
                        break;
                    case GameState.NextTurn:
                        break;
                    case GameState.InMenu:
                        break;
                    case GameState.EndGame:
                        break;
                    case GameState.InDialogue:
                        break;
                }
            }
            else
            {
                EventSystem.SetSelectedGameObject(null);
            }
        }

        private void ToggleWorldCursor(bool toggle)
        {
            var renderer = worldSpaceCursor.GetComponentInChildren<Renderer>();
            float tweenTo = toggle ? 0.5f : 0.0f;
            float tweenFrom = toggle ? 0.0f : 0.5f;
            tween = DOTween.To(() => tweenFrom, y => tweenFrom = y, tweenTo, 0.25f).OnUpdate(() =>
            {
                renderer.material.SetFloat("_Opacity", tweenFrom);
            });
        }

        private void ToggleUICursor(bool toggle)
        {
            HelperActive = Manager.Inputs.UsingController && toggle;
        }

        private void OnDestroy()
        {
            tween.Kill();
        }
    }
}
