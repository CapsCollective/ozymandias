using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;

namespace Inputs
{
    public class CentreButton : MonoBehaviour
    {
        public static Action OnWorldEdge;
        
        private const float InnerDistance = 35;
        private const float Text1Distance = 55;
        private const float Text2Distance = 70;
        private const float OuterDistance = 85;

        private bool _isCentering;
        private GameObject _dummyCursor;
        
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image buttonImage;

        [SerializeField] private GameObject dummyCursorNonScaling;
        [SerializeField] private GameObject dummyCursorScaling;

        private void Start()
        {
            var isMac = (Application.platform == RuntimePlatform.OSXPlayer || 
                         Application.platform == RuntimePlatform.OSXEditor);
            _dummyCursor = isMac ? dummyCursorNonScaling : dummyCursorScaling;
            button.onClick.AddListener(CenterCamera);
        }

        private void Update()
        {
            float townDist = -Manager.Camera.transform.position.z;

            bool isBeyondBounds = townDist > InnerDistance;
            button.gameObject.SetActive(isBeyondBounds);
            
            if (!isBeyondBounds) return;
            
            buttonText.text = GetReturnText(townDist);
            if (!_isCentering && townDist > OuterDistance) StartCoroutine(ManualCenter());
        }

        private static string GetReturnText(float distanceFromTown)
        {
            if (distanceFromTown < Text1Distance) return "Return to Town";
            if (distanceFromTown < Text2Distance) return "Please, Return to Town";
            return distanceFromTown < OuterDistance ? "There's nothing here" : "Fine, I'll do it myself";
        }

        private void CenterCamera()
        {
            Manager.Camera.MoveTo(Manager.Structures.TownCentre)
                .OnStart(() => { 
                    button.interactable = false;
                    buttonText.alpha = 0;
                })
                .OnComplete(() => { 
                    button.interactable = true; 
                    buttonText.alpha = 1;
                    _isCentering = false;
                });
        }

        private IEnumerator ManualCenter()
        {
            _isCentering = true;
            yield return new WaitForSeconds(1);
            _dummyCursor.SetActive(true);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, 
                Manager.Inputs.MousePosition, canvas.worldCamera, out var currentPos);
            currentPos = canvas.transform.TransformPoint(currentPos);

            var endPos = button.gameObject.transform.position;
            _dummyCursor.transform.position = currentPos;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            while (Vector2.Distance(currentPos, endPos) > 10f)
            {
                currentPos = Vector3.Lerp(currentPos, endPos, Time.deltaTime * 1.5f + 0.01f);
                _dummyCursor.transform.position = currentPos;
                yield return null;
            }
        
            OnWorldEdge?.Invoke();
            Manager.Jukebox.PlayClick();
            CenterCamera();
            _dummyCursor.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
