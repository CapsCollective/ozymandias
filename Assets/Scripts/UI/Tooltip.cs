using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

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

    internal struct TooltipConfig
    {
        public string Title, Description;
        public Stat? Stat;
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
            {TooltipType.Brawler, new TooltipConfig {
                Title = "Brawlers",
                Description = "Brawlers are a staple of adventuring parties, and the only ones who prefer to beat " +
                              "their way out of a situation than talk or trick. They can be rowdy when left " +
                              "unattended, but give them plenty to fight, and maybe an audience to show off too and " +
                              "they’ll be a great help.",
                Stat = Stat.Brawler
            }},
            {TooltipType.Outrider, new TooltipConfig {
                Title = "Outriders",
                Description = "As shady as they are charming, an outrider thrives of exploring the unknown, spending " +
                              "their days scouting the forests, or lurking in the streets. Be warned, when left " +
                              "unchecked, thing's might start to go missing.",
                Stat = Stat.Outrider
            }},
            {TooltipType.Performer, new TooltipConfig {
                Title = "Performers",
                Description = "An odd and extravagant sort, performers aren’t the type to adventurer on their own, " +
                              "often traveling with others, and spinning magic into their tales. However, they can " +
                              "be as critical as they are kind, so we don’t want to give them a reason to spread " +
                              "bad news about us.",
                Stat = Stat.Performer
            }},
            {TooltipType.Diviner, new TooltipConfig {
                Title = "Diviners",
                Description = "By the will of the gods themselves, Diviners channel holy magic to protect and heal " +
                              "others. They aren't all peace and love’ types however, as they can uphold their gods " +
                              "with zealous force when confronted. A religious war is the last thing you want when " +
                              "creating a settlement.",
                Stat = Stat.Diviner
            }},
            {TooltipType.Arcanist, new TooltipConfig {
                Title = "Arcanists",
                Description = "Arcanists pluck from the very fabric of the Ethereal plane to distort reality in " +
                              "weird, wonderful, and most importantly profitable ways. They tend to be eccentric " +
                              "scholars who enjoy isolation as much as they enjoy pursuing arcane knowledge. It is " +
                              "recommended their magic is controlled and isolated due to *REDACTED* and " +
                              "*SUPER REDACTED*.",
                Stat = Stat.Arcanist
            }},
            {TooltipType.Housing, new TooltipConfig {
                Title = "Housing",
                Description = "Adventures need a place to stay and stash their loot in between adventurers. Plus if " +
                              "we have extra vacancies we might attract some passers by.",
                Stat = Stat.Housing
            }},
            {TooltipType.Food, new TooltipConfig {
                Title = "Food",
                Description = "he only thing adventurers love more than loot is food, so keep supply in plenty and " +
                              "you won't have any problems (more than the usual).",
                Stat = Stat.Food
            }},
            {TooltipType.Wealth, new TooltipConfig {
                Title = "Wealth",
                Description = "Wealth is the currency you spend on performing in game actions, like building, " +
                              "clearing, and questing. Wealth gained per turn is based on your number of adventurers " +
                              "multiplied by your towns spending.",
                Stat = Stat.Spending
            }},
            {TooltipType.Stability, new TooltipConfig {
                Title = "Town Stability",
                Description = "The towns stability rises and falls by the constant struggle of the growing threat of " +
                              "the outside world, and the defense of the town. Build defensive buildings, complete " +
                              "quests, and most importantly attract more adventurers to keep this from running out, " +
                              "or else.",
                Stat = Stat.Threat
            }},
            {TooltipType.Newspaper, new TooltipConfig {
                Title = "Newspaper",
                Description = "Re-read the morning news.",
            }},
            {TooltipType.Progress, new TooltipConfig {
                Title = "Progress Report",
                Description = "Check out your cities growth, and have a look at your achievements.",
            }},
            {TooltipType.Quests, new TooltipConfig {
                Title = "Quest Map",
                Description = "Send out adventurers on quests for a variety of benefits. Be aware that they won't be " +
                              "around to Defend while questing.",
            }},
            {TooltipType.NextTurn, new TooltipConfig {
                Title = "Next Turn",
                Description = "Jump forward to the next day, collect your income, get new a new set of building, and " +
                              "see what awaits.",
            }}
        };
        
        private void Awake()
        {
            _cg = GetComponent<CanvasGroup>();
        }

        public void UpdateTooltip(TooltipType type)
        {
            TooltipConfig config = Configs[type];
            title.text = config.Title;
            description.text = config.Description;
            details.gameObject.SetActive(config.Stat != null);
            
            switch (config.Stat)
            {
                case null: break;
                case Stat.Housing:
                    int spawnRate = Manager.RandomSpawnChance;
                    string descriptive = HousingDescriptor(spawnRate);
                    string spawnText = spawnRate == -1
                        ? "Adventurers will start to flee"
                        : HousingSpawnName(spawnRate) + " adventurer spawn chance"; 
                    details.text = $"{Manager.GetStat(Stat.Housing)} housing for {Manager.Adventurers.Available} total adventurers\n" +
                                   $"{descriptive} ({spawnText})";
                    break;
                case Stat.Food:
                    details.text = $"{Manager.GetStat(Stat.Food)} food for {Manager.Adventurers.Available} total adventurers\n" +
                                   $"{getFormattedModifierString(Stat.Food)}" +
                                   $"{FoodDescriptor(Manager.FoodModifier)}";
                    break;
                case Stat.Threat:
                    details.text = $"{Manager.Defense} defense against {Manager.Threat} threat";
                    break;
                case Stat.Spending:
                    details.text = $"{Manager.WealthPerTurn} wealth per turn from {Manager.Adventurers.Count} adventurers " +
                                   $"(5 wealth per adventurer) times {(100 + Manager.GetStat(Stat.Spending))/100f} from spending modifier.";
                    break; 
                default: // Stat for a class
                    AdventurerCategory category = (AdventurerCategory) config.Stat.Value;
                    string className = config.Stat.ToString();
                    int turnUntilSpawn = Manager.TurnsToSpawn(category) - Manager.SpawnCounters[category];
                    string spawnTurnText = turnUntilSpawn > 1 ? ("in " + turnUntilSpawn + " turns") : "next turn";

                    details.text =
                        $"{Manager.GetStat(config.Stat.Value)} {className} satisfaction for " +
                        $"{Manager.Adventurers.GetCount(category)} {className}s\n" +
                        $"{getFormattedFoodModifierString()}" +
                        $"{getFormattedModifierString(config.Stat.Value)}\n" +
                        $"New {className} will arrive {spawnTurnText} (+1 every {Manager.TurnsToSpawn(category)} turns)";
                    break;
            }
        }

        private string getFormattedFoodModifierString()
        {
            bool isFoodInSurplus = Math.Sign(Manager.FoodModifier) == 1;
            string foodDescriptor = isFoodInSurplus ? "surplus" : "shortage";
            char foodSign = isFoodInSurplus ? '+' : '-';
            string textColor = isFoodInSurplus ? getPositiveHexColor() : getNegativeHexColor();

            return $"  ● <color={textColor}>{foodSign}{Math.Abs(Manager.FoodModifier)}</color>" +
                   $" from food {foodDescriptor}\n";
        }

        private string getFormattedModifierString(Stat stat)
        {
            string formattedModifierString = "";

            foreach (var modifier in Manager.Modifiers[stat])
            {
                char sign = Math.Sign(modifier.amount) == 1 ? '+' : '-';
                string textColor = sign == '+' ? "#1bfc30" : "#FF0000";
                string turnText = modifier.turnsLeft == 1 ? "turn" : "turns";
                formattedModifierString += 
                    $"  ● <color={textColor}>{sign}{Math.Abs(modifier.amount)}</color> " +
                    $"from {modifier.reason} ({modifier.turnsLeft} {turnText} remaining)\n";
            }
            
            return formattedModifierString;
        }

        private string getPositiveHexColor()
        {
            return "#1bfc30";
        }
        
        private string getNegativeHexColor()
        {
            return "#FF0000";
        }

        private string HousingDescriptor(int spawnRate)
        {
            return spawnRate switch
            {
                3 => "They're practically giving houses away",
                2 => "There is room to spare",
                1 => "It's a bit cramped",
                0 => "There are adventurers on the street",
                -1 => "Adventurers are looking to get out of here",
                _ => ""
            };
        }
        
        private string FoodDescriptor(int foodMod)
        {
            return foodMod switch
            {
                2 => "There are daily feasts (+2 to all adventurers)",
                1 => "The adventurers are well fed (+1 to all adventurers)",
                0 => "The town is getting by (No modifiers)",
                -1 => "Rations have taken effect (-1 to all adventurers)",
                -2 => "People are starving (-2 to all adventurers)",
                _ => ""
            };
        }

        private string HousingSpawnName(int spawnRate)
        {
            return spawnRate switch
            {
                3 => "High",
                2 => "Medium",
                1 => "Low",
                0 => "No",
                _ => ""
            };
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
