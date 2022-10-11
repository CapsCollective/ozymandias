using Managers;
using UnityEngine;
using static Managers.GameManager;

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
            if (Globals.RestartingGame) return;
            OnUpdateUI -= UpdateUi;
        }

        protected abstract void UpdateUi();
    }
}
