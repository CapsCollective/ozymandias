using UnityEngine;
using static Managers.GameManager;

namespace UI
{
    public class ControlPrompt: MonoBehaviour
    {
        [SerializeField] private bool invert;
        private void Start()
        {
            Inputs.Inputs.OnControlChange += _ =>
            {
                gameObject.SetActive(Manager.Inputs.UsingController != invert);
            };
            gameObject.SetActive(Manager.Inputs.UsingController != invert);
        }
    }
}
