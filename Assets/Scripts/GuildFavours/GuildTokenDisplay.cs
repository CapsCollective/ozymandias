using TMPro;
using UI;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace GuildFavours
{
    public class GuildTokenDisplay : UiUpdater
    {
        [SerializeField] private Guild guild;
        [SerializeField] private TextMeshProUGUI count;
    
        protected override void UpdateUi()
        {
            count.text = Manager.Favours.GuildTokens[guild].ToString();
        }
    }
}
