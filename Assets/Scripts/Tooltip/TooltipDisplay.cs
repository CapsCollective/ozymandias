using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Tooltip
{
    internal struct TooltipConfig
    {
        public string Title, Description;
        public Stat? Stat;
    }

    public class TooltipDisplay : MonoBehaviour
    {
        private CanvasGroup _cg;
        [SerializeField] private TextMeshProUGUI title, details, description;

        const float FadeDuration = 0.3f;

        private readonly Dictionary<TooltipType, TooltipConfig> Configs = new Dictionary<TooltipType, TooltipConfig>
        {
            {TooltipType.Brawler, new TooltipConfig {
                Title = "Brawlers",
                Description = "Brawlers are a staple of adventuring parties, the kind who prefer to beat their way " +
                              "out of a situation than talk or trick. They can be rowdy when left unattended, but " +
                              "give them plenty to fight, and maybe an audience to show off too and they’ll be a great help.",
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
                Description = "An odd and extravagant sort, performers avoid adventuring on their own, instead " +
                              "traveling with others, and spinning magic into their tales. However, they can be as " +
                              "critical as they are kind, so we don’t want to give them a reason to spread bad news about us.",
                Stat = Stat.Performer
            }},
            {TooltipType.Diviner, new TooltipConfig {
                Title = "Diviners",
                Description = "By the will of the gods themselves, Diviners channel holy magic to protect and heal " +
                              "others. They aren't all 'peace and love' types however, and will uphold their god's " +
                              "zealous force when confronted. A religious war is the last thing you want when " +
                              "creating a settlement.",
                Stat = Stat.Diviner
            }},
            {TooltipType.Arcanist, new TooltipConfig {
                Title = "Arcanists",
                Description = "Arcanists pluck from the very fabric of reality in weird, wonderful, and most " +
                              "importantly, profitable ways. They are eccentric scholars who enjoy isolation and the " +
                              "pursuit of arcane knowledge. It is recommended their magic is controlled to prevent " +
                              "*REDACTED*, and *SUPER REDACTED*.",
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
                Description = "The only thing adventurers love more than loot is food, so keep supply in plenty and " +
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
                Description = "The towns stability rises and falls each turn based on your defence against a growing " +
                              "threat. If it hits 0, that's game over.",
                Stat = Stat.Stability
            }},
            {TooltipType.Threat, new TooltipConfig {
                Title = "Threat",
                Description = "The ever growing dangers from the surrounding wilderness.",
                Stat = Stat.Threat
            }},
            {TooltipType.Defence, new TooltipConfig {
                Title = "Defence",
                Description = "The total protection your town has against Threat.",
                Stat = Stat.Defence
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
                    int spawnRate = Manager.Stats.RandomSpawnChance;
                    string spawnText = spawnRate == -1
                        ? "Adventurers will start to flee"
                        : HousingSpawnName(spawnRate) + " adventurer spawn chance"; 
                    details.text = $"{Manager.Stats.GetStat(Stat.Housing)} housing for {Manager.Adventurers.Available} total adventurers\n" +
                                   $"{FormattedBuildingString(Stat.Housing)}" +
                                   $"{FormattedUpgradeString(Stat.Housing)}" +
                                   $"{FormattedModifierString(Stat.Housing)}" +
                                   $"{HousingDescriptor(spawnRate)} ({spawnText})";
                    break;
                case Stat.Food:
                    details.text = $"{Manager.Stats.GetStat(Stat.Food)} food for {Manager.Adventurers.Available} total adventurers\n" +
                                   $"{FormattedBuildingString(Stat.Food)}" +
                                   $"{FormattedUpgradeString(Stat.Food)}" +
                                   $"{FormattedModifierString(Stat.Food)}" +
                                   $"{FoodDescriptor(Manager.Stats.FoodModifier)}";
                    break;
                case Stat.Defence:
                    details.text = $"{Manager.Stats.Defence} defence\n" +
                                   $"  ● +{Manager.Adventurers.Available} from total adventurers\n" +
                                   $"{FormattedBuildingString(Stat.Defence)}" +
                                   $"{FormattedModifierString(Stat.Defence)}";
                    break;
                case Stat.Threat:
                    details.text = $"{Manager.Stats.Threat} threat\n" +
                                   $"  ● +{Manager.Stats.BaseThreat} from events\n" +
                                   (Manager.Quests.RadiantQuestCellCount == 0 ? "" :$"  ● +{Manager.Quests.RadiantQuestCellCount} from enemy camps\n") +
                                   $"{FormattedModifierString(Stat.Threat)}\n";
                    break;
                case Stat.Stability:
                    int change = Manager.Stats.Defence - Manager.Stats.Threat;
                    details.text = $"{Manager.Stats.Stability}/100 town stability ({(change > 0 ? "+" : "") + change} next turn)";
                    break;
                case Stat.Spending:
                    int stat = Manager.Stats.GetStat(Stat.Spending);
                    details.text = $"{Manager.Stats.WealthPerTurn} wealth per turn\n" +
                                   $"{WealthPerAdventurer} per adventurer ({Manager.Adventurers.Available}) with {100 + stat}% spending\n" +
                                   $"  ● {100 + Manager.Stats.GetUpgradeMod(config.Stat.Value)}% base\n" +
                                   $"{FormattedBuildingString(Stat.Spending)}" +
                                   $"{FormattedUpgradeString(Stat.Spending)}" +
                                   $"{FormattedModifierString(Stat.Spending)}";
                    break; 
                default: // Stat for a guild
                    Guild guild = (Guild) config.Stat.Value;
                    string guildName = config.Stat.ToString().ToLower();
                    int total = Manager.Stats.GetStat(config.Stat.Value);
                    int count = Manager.Adventurers.GetCount(guild);
                    int spawnChance = Manager.Stats.SpawnChance(guild);
                    
                    details.text =
                        $"{total} satisfaction for {count} {guildName}s ({(total - count > 0 ? "+" : "")}{total - count})\n" +
                        $"{FormattedBuildingString(config.Stat.Value)}" +
                        $"{FormattedUpgradeString(config.Stat.Value)}" +
                        $"{FormattedFoodModifierString}" +
                        $"{FormattedModifierString(config.Stat.Value)}" +
                        $"{spawnChance}% {guildName} spawn chance per turn";
                    break;
            }
        }

        private string FormattedBuildingString(Stat stat)
        {
            int buildingMod = Manager.Structures.GetStat(stat) * (stat == Stat.Food || stat == Stat.Housing ? FoodHousingMultiplier : 1);
            return $"  ● {(buildingMod >= 0 ? "+" : "")}{buildingMod}{(stat == Stat.Spending ? "%" : "")} from buildings\n";
        }

        private string FormattedUpgradeString(Stat stat)
        {
            int upgradeMod = Manager.Stats.GetUpgradeMod(stat);
            return upgradeMod == 0 ? "" : $"  ● +{upgradeMod}{(stat == Stat.Spending ? "%" : "")} from upgrades\n";
        }

        // Formatted string for food modifiers (specifically for adventurer)
        private string FormattedFoodModifierString
        {
            get
            {
                int mod = Manager.Stats.FoodModifier;
                if (mod == 0) return "";
                bool isFoodInSurplus = Math.Sign(mod) == 1;
                string foodDescriptor = isFoodInSurplus ? "surplus" : "shortage";
                char foodSign = isFoodInSurplus ? '+' : '-';
                string textColor = isFoodInSurplus ? Colors.GreenText : Colors.RedText;

                return $"  ● <color={textColor}>{foodSign}{Math.Abs(mod)}</color> from food {foodDescriptor}\n";
            }
        }

        // Formatted string for listing all stat modifiers. 
        private string FormattedModifierString(Stat stat)
        {
            string formattedModifierString = "";

            foreach (var modifier in Manager.Stats.Modifiers[stat])
            {
                char sign = Math.Sign(modifier.amount) == 1 ? '+' : '-';
                string textColor = sign == '+' ? Colors.GreenText : Colors.RedText;
                string turnText = modifier.turnsLeft == 1 ? "turn" : "turns";
                formattedModifierString += 
                    $"  ● <color={textColor}>{sign}{Math.Abs(modifier.amount)}{(stat == Stat.Spending ? "%" : "")}</color> " +
                    $"from {modifier.reason} ({modifier.turnsLeft} {turnText} remaining)\n";
            }
            
            return formattedModifierString;
        }

        private string HousingDescriptor(int spawnRate)
        {
            return spawnRate switch
            {
                3 => "They're practically giving houses away",
                2 => "There is room to spare",
                1 => "There's just enough space",
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
