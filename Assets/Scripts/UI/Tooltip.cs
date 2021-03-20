using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public enum TooltipType
    {
        Brawler,
        Outrider,
        Performer,
        Diviner,
        Arcanist,
        Housing,
        Food,
        Wealth,
        Stability,
        Newspaper,
        Progress,
        Quests,
        NextTurn
    }

    struct TooltipConfig
    {
        public string title, details, description;
        public bool isClass;
    }

    public class Tooltip : MonoBehaviour
    {
        private CanvasGroup _cg;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI details;
        [SerializeField] private TextMeshProUGUI description;

        const float FadeDuration = 0.3f;

        private readonly Dictionary<TooltipType, TooltipConfig> Configs = new Dictionary<TooltipType, TooltipConfig>
        {
            {TooltipType.Brawler, new TooltipConfig{
                title = "Brawlers",
                description = "Brawlers are a staple of adventuring parties, and the only ones who prefer to beat " +
                              "their way out of a situation than talk or trick. They can be rowdy when left " +
                              "unattended, but give them plenty to fight, and maybe an audience to show off too and " +
                              "they’ll be a great help.",
                isClass = false
            }},
            {TooltipType.Outrider, new TooltipConfig{
                title = "Outriders",
                description = "As shady as they are charming, an outrider thrives of exploring the unknown, spending " +
                              "their days scouting the forests, or lurking in the streets. Be warned, when left " +
                              "unchecked, thing's might start to go missing.",
                isClass = false
            }},
            {TooltipType.Performer, new TooltipConfig{
                title = "Performers",
                description = "An odd and extravagant sort, performers aren’t the type to adventurer on their own, " +
                              "often traveling with others, and spinning magic into their tales. However, they can " +
                              "be as critical as they are kind, so we don’t want to give them a reason to spread " +
                              "bad news about us.",
                isClass = false
            }},
            {TooltipType.Diviner, new TooltipConfig{
                title = "Diviners",
                description = "By the will of the gods themselves, Diviners channel holy magic to protect and heal " +
                              "others. They aren't all peace and love’ types however, as they can uphold their gods " +
                              "with zealous force when confronted. A religious war is the last thing you want when " +
                              "creating a settlement.",
                isClass = false
            }},
            {TooltipType.Arcanist, new TooltipConfig{
                title = "Arcanists",
                description = "Arcanists pluck from the very fabric of the Ethereal plane to distort reality in " +
                              "weird, wonderful, and most importantly profitable ways. They tend to be eccentric " +
                              "scholars who enjoy isolation as much as they enjoy pursuing arcane knowledge. It is " +
                              "recommended their magic is controlled and isolated due to *REDACTED* and " +
                              "*SUPER REDACTED*.",
                isClass = false
            }},
            {TooltipType.Housing, new TooltipConfig{
                title = "Housing",
                description = "Adventures need a place to stay and stash their loot in between adventurers. Plus if " +
                              "we have extra vacancies we might attract some passers by.",
                isClass = false
            }},
            {TooltipType.Food, new TooltipConfig{
                title = "Food",
                description = "he only thing adventurers love more than loot is food, so keep supply in plenty and " +
                              "you won't have any problems (more than the usual).",
                isClass = false
            }},
            {TooltipType.Wealth, new TooltipConfig{
                title = "Wealth",
                description = "Wealth is the currency you spend on performing in game actions, like building, " +
                              "clearing, and questing. Wealth gained per turn is based on your number of adventurers " +
                              "multiplied by your towns spending.",
                isClass = false
            }},
            {TooltipType.Stability, new TooltipConfig{
                title = "Town Stability",
                description = "The towns stability rises and falls by the constant struggle of the growing threat of " +
                              "the outside world, and the defense of the town. Build defensive buildings, complete " +
                              "quests, and most importantly attract more adventurers to keep this from running out, " +
                              "or else.",
                isClass = false
            }},
            {TooltipType.Newspaper, new TooltipConfig{
                title = "Newspaper",
                description = "Re-read the morning news.",
                isClass = false
            }},
            {TooltipType.Progress, new TooltipConfig{
                title = "Progress Report",
                description = "Check out your cities growth, and have a look at your achievements.",
                isClass = false
            }},
            {TooltipType.Quests, new TooltipConfig{
                title = "Quest Map",
                description = "Send out adventurers on quests for a variety of benefits. Be aware that they won't be " +
                              "around to Defend while questing.",
                isClass = false
            }},
            {TooltipType.NextTurn, new TooltipConfig{
                title = "Next Turn",
                description = "Jump forward to the next day, collect your income, get new a new set of building, and " +
                              "see what awaits.",
                isClass = false
            }}
        };
        
        private void Awake()
        {
            _cg = GetComponent<CanvasGroup>();
        }

        public void UpdateTooltip(TooltipType type)
        {
            TooltipConfig config = Configs[type];
            title.text = config.title;
            details.gameObject.SetActive(false);
            description.text = config.description;
        }

        public void Fade(float opacity)
        {
            _cg.DOFade(opacity, FadeDuration);
        }

        public bool IsVisible()
        {
            return _cg.alpha > 0;
        }
    }
}
