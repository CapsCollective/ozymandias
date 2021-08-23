using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace GuildRequests
{
    public class RequestDisplay : UiUpdater
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI description, count;
        
        public Request Request { get; set; }
        
        protected override void UpdateUi()
        {
            if (Request == null)
            {
                description.text = "Nothings here yet!";
                count.text = "";
                slider.gameObject.SetActive(false);
            }
            else
            {
                description.text = Request.Description;
                count.text = Request.completed + "/" + Request.required;
                slider.gameObject.SetActive(true);
                slider.value = (float)Request.completed / Request.required;
            }
        }
    }
}
