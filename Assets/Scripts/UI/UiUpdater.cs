using UnityEngine;
using static GameState.GameManager;

namespace UI
{
    public abstract class UiUpdater : MonoBehaviour
    {
        private void Awake()
        {
            OnUpdateUI += UpdateUi;
        }

        private void OnDestroy()
        {
            OnUpdateUI -= UpdateUi;
        }

        protected abstract void UpdateUi();
    }
}
