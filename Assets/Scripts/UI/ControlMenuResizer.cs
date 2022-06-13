using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;

namespace UI
{
    public class ControlMenuResizer: MonoBehaviour
    {
        private VerticalLayoutGroup _layoutGroup;
        private void Start()
        {
            _layoutGroup = GetComponent<VerticalLayoutGroup>();
            
            Inputs.Inputs.OnControlChange += _ =>
            {
                _layoutGroup.spacing = Manager.Inputs.UsingController ? 15 : 50;
            };
            _layoutGroup.spacing = Manager.Inputs.UsingController ? 15 : 50;
        }
    }
}
