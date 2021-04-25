using System.Collections;
using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CentreButton : MonoBehaviour
    {
        private const float InnerDistance = 50;
        private const float OuterDistance = 160;
        
        private bool _isCentering;
        
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private CameraMovement cameraMovement;
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject dummyCursor;

        private void Update()
        {
            var cameraPos = cameraMovement.gameObject.transform.position;
            var townDist = Vector3.Magnitude(cameraPos);
            
            var isBeyondBounds = townDist > InnerDistance;
            button.gameObject.SetActive(isBeyondBounds);
            
            if (!isBeyondBounds) return;
            
            buttonText.text = GetReturnText(townDist);
            if (!_isCentering && townDist > OuterDistance) StartCoroutine(ManualCenter());
        }

        private static string GetReturnText(float distanceFromTown)
        {
            if (distanceFromTown < 80) return "Return to Town";
            if (distanceFromTown < 120) return "Please, Return to Town";
            return distanceFromTown < 160 ? "There's nothing here" : "Fine, I'll do it myself";
        }

        private IEnumerator ManualCenter()
        {
            _isCentering = true;
            yield return new WaitForSeconds(1);
            dummyCursor.SetActive(true);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, 
                Input.mousePosition, canvas.worldCamera, out var currentPos);
            currentPos = canvas.transform.TransformPoint(currentPos);

            var endPos = button.gameObject.transform.position;
            dummyCursor.transform.position = currentPos;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            while (Vector2.Distance(currentPos, endPos) > 10f)
            {
                currentPos = Vector3.Lerp(currentPos, endPos, Time.deltaTime * 1.5f + 0.01f);
                dummyCursor.transform.position = currentPos;
                yield return null;
            }
        
            Jukebox.Instance.PlayClick();
            cameraMovement.Center();
            dummyCursor.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _isCentering = false;
        }
    }
}