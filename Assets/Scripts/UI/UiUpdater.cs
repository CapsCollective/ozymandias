using UnityEngine;

namespace UI
{
    public abstract class UiUpdater : MonoBehaviour
    {

        private void Awake()
        {
            GameManager.OnUpdateUI += UpdateUi;
        }

        private void OnDestroy()
        {
            GameManager.OnUpdateUI -= UpdateUi;
        }

        public abstract void UpdateUi();
    }
}
