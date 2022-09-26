using System;
using System.Collections.Generic;
using DG.Tweening;
using Managers;
using TMPro;
using UnityEngine;
using Utilities;
using static Managers.GameManager;
using String = Utilities.String;
using UnityEngine.InputSystem;

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
        private Tween _tween;
        [SerializeField] private TextMeshProUGUI title, details, description;

        [SerializeField] private TooltipPlacement defaultTooltip;
        private TooltipPlacement _selectedTooltip;
        public bool NavigationActive { get; private set; }

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
                Description = "As shady as they are charming, an outrider thrives in exploring the unknown, spending " +
                              "their days scouting the forests, or lurking in the streets. Be warned, when left " +
                              "unchecked, things might start to go missing.",
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
                Description = "Adventures need a place to stay and stash their loot in between adventures. Plus if " +
                              "we have extra vacancies we might attract some passers by.",
                Stat = Stat.Housing
            }},
            {TooltipType.Food, new TooltipConfig {
                Title = "Food",
                Description = "The only thing adventurers love more than loot is food, so keep supply in plenty and " +
                              "you won't have any problems (any more than the usual).",
                Stat = Stat.Food
            }},
            {TooltipType.Wealth, new TooltipConfig {
                Title = "Wealth",
                Description = "Wealth is the currency you spend on performing in game actions, like building, " +
                              "clearing, and questing.",
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
                Description = "Lowers your town's stability, gained through events and enemy camps (which can be cleared via quests).",
                Stat = Stat.Threat
            }},
            {TooltipType.Defence, new TooltipConfig {
                Title = "Defence",
                Description = "Increases your town's stability, equals your total adventurers plus any bonuses from buildings or events.",
                Stat = Stat.Defence
            }},
            {TooltipType.Newspaper, new TooltipConfig {
                Title = "Newspaper",
                Description = "Re-read the morning news.",
            }},
            {TooltipType.Progress, new TooltipConfig {
                Title = "Town Ledger",
                Description = "Check your progress, purchase upgrades, and change settings.",
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
            }},
            {TooltipType.GuildTokens, new TooltipConfig {
                Title = "Guild Tokens",
                Description = "Earn tokens from each guild by completing their requests. Tokens can be used to purchase " +
                              "upgrades for your towns.",
            }},
        };
        
        private void Start()
        {
            NavigationActive = false;
            _selectedTooltip = defaultTooltip;
            _cg = GetComponent<CanvasGroup>();
            Manager.Inputs.ToggleTooltips.performed += ToggleTooltips;
            Manager.Inputs.Close.performed += _ => DeactivateTooltips(); 
            Inputs.Inputs.OnControlChange += _ => DeactivateTooltips();
            State.OnEnterState += _ => DeactivateTooltips();

            Manager.Inputs.NavigateTooltips.performed += NavigateTooltips;
        }

        private void DeactivateTooltips()
        {
            _selectedTooltip.OnPointerExit(null);
            NavigationActive = false;
        }

        private void ToggleTooltips(InputAction.CallbackContext obj)
        {
            if (!Manager.State.InGame) return;
            
            NavigationActive = !NavigationActive;
            if (NavigationActive) _selectedTooltip.OnPointerEnter(null);
            else _selectedTooltip.OnPointerExit(null);
        }

        private void NavigateTooltips(InputAction.CallbackContext obj)
        {
            if (!NavigationActive) return;

            Vector2 direction = Manager.Inputs.NavigateTooltips.ReadValue<Vector2>();
            _selectedTooltip.OnPointerExit(null);

            if (Mathf.RoundToInt(direction.y) == 1 && _selectedTooltip.navigationDirections.up)
             _selectedTooltip = _selectedTooltip.navigationDirections.up;
            else if (Mathf.RoundToInt(direction.y) == -1 && _selectedTooltip.navigationDirections.down)
             _selectedTooltip = _selectedTooltip.navigationDirections.down;
            else if (Mathf.RoundToInt(direction.x) == -1 && _selectedTooltip.navigationDirections.left)
             _selectedTooltip = _selectedTooltip.navigationDirections.left;
            else if (Mathf.RoundToInt(direction.x) == 1 && _selectedTooltip.navigationDirections.right)
             _selectedTooltip = _selectedTooltip.navigationDirections.right;
            
            _selectedTooltip.OnPointerEnter(null);
        }
        
        public void UpdateTooltip(TooltipType type)
        {
            TooltipConfig config = Configs[type];
            title.text = config.Title;
            description.text = config.Description;
            details.gameObject.SetActive(config.Stat != null);

            if (config.Stat == null) return;
            int adventurers = Manager.Adventurers.Available;
            int stat = Manager.Stats.GetStat((Stat)config.Stat);
            int diff = stat - adventurers;

            switch (config.Stat)
            {
                case Stat.Housing:
                    int spawnRate = Manager.Stats.RandomSpawnChance;
                    string spawnText = spawnRate == -1
                        ? "Adventurers will start to flee"
                        : HousingSpawnName(spawnRate) + " adventurer spawn chance"; 
                    details.text = $"{stat} housing for {adventurers} adventurers in town\n" +
                                   $"({(diff > 0 ? "+" : "") + diff} total)\n" +
                                   $"{FormattedBuildingString(Stat.Housing)}" +
                                   $"{FormattedUpgradeString(Stat.Housing)}" +
                                   $"{FormattedModifierString(Stat.Housing)}" +
                                   $"{HousingDescriptor(spawnRate)} ({spawnText})";
                    break;
                case Stat.Food:
                    details.text = $"{stat} food for {adventurers} adventurers in town\n"+
                                   $"({(diff > 0 ? "+" : "") + diff} total)\n" +
                                   $"{FormattedBuildingString(Stat.Food)}" +
                                   $"{FormattedUpgradeString(Stat.Food)}" +
                                   $"{FormattedModifierString(Stat.Food)}" +
                                   $"{FoodDescriptor(Manager.Stats.FoodModifier)}";
                    break;
                case Stat.Defence:
                    int unavailable = Manager.Adventurers.Unavailable;
                    details.text = $"{Manager.Stats.Defence} defence\n" +
                                   $"  ● +{adventurers}{(unavailable != 0 ? "/" + (adventurers + unavailable) : "")} from adventurers in town\n"+
                                   (unavailable != 0 ? $"     ({unavailable} out questing)\n" : "") + 
                                   $"{FormattedBuildingString(Stat.Defence)}" + 
                                   (Manager.Stats.MineStrikePenalty != 0 ? $"  ● {Manager.Stats.MineStrikePenalty} from the miners strike\n" : "") + 
                                   $"{FormattedModifierString(Stat.Defence)}";
                    break;
                case Stat.Threat:
                    details.text = $"{Manager.Stats.Threat} threat\n" +
                                   $"  ● +{Manager.Stats.BaseThreat} from events\n" +
                                   (Manager.Quests.RadiantQuestCellCount != 0 ? $"  ● +{Manager.Quests.RadiantQuestCellCount} from enemy camps\n" : "") +
                                   (Manager.Stats.ScarecrowThreat != 0 ? $"  ● +{Manager.Stats.ScarecrowThreat} from farms due to scarecrows\n" : "") +
                                   $"{FormattedModifierString(Stat.Threat)}\n";
                    break;
                case Stat.Stability:
                    int change = Manager.Stats.Defence - Manager.Stats.Threat;
                    details.text = $"{Manager.Stats.Stability}/100 town stability\n" +
                                   $"{(change > 0 ? "+" : "") + change} next turn ({Manager.Stats.Defence} defence - {Manager.Stats.Threat} threat)";
                    break;
                case Stat.Spending:
                    details.text = $"{Manager.Stats.WealthPerTurn} wealth per turn\n" +
                                   $"  ● +{(Manager.EventQueue.Flags[Flag.Cosmetics] ? 3 : WealthPerAdventurer) * adventurers} from {adventurers} adventurers in town" +
                                   $"{(Manager.EventQueue.Flags[Flag.Cosmetics] ? " (-2 spending per adventurers due to cosmetics)" : "")}\n" +
                                   $"  ● +{StartingSalary} starting salary\n" +
                                   $"{FormattedBuildingString(Stat.Spending)}" +
                                   $"{FormattedUpgradeString(Stat.Spending)}" +
                                   $"{FormattedModifierString(Stat.Spending)}";
                    break; 
                default: // Stat for a guild
                    Guild guild = (Guild) config.Stat.Value;
                    string guildName = config.Stat.ToString().ToLower();
                    int count = Manager.Adventurers.GetCount(guild, true);
                    int spawnChance = Manager.Stats.SpawnChance(guild);
                    
                    details.text =
                        $"{stat} satisfaction for {count} {String.Pluralise(guildName)} in town\n" +
                        $"({(stat - count > 0 ? "+" : "")}{stat - count} total)\n" +
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
            int buildingMod = Manager.Structures.GetStat(stat) * Manager.Stats.StatMultiplier(stat);
            return $"  ● {(buildingMod >= 0 ? "+" : "")}{buildingMod} from buildings\n";
        }

        private string FormattedUpgradeString(Stat stat)
        {
            int upgradeMod = Manager.Stats.GetUpgradeMod(stat);
            return upgradeMod == 0 ? "" : $"  ● +{upgradeMod} from upgrades\n";
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

                return $"  ● {textColor}{foodSign}{Math.Abs(mod)}{Colors.EndText} from food {foodDescriptor}\n";
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
                formattedModifierString += $"  ● {textColor}{sign}{Math.Abs(modifier.amount)}{Colors.EndText} " +
                                           $"from {modifier.reason} {(modifier.turnsLeft != -1 ? $"({modifier.turnsLeft} {turnText} remaining)": "")}\n";
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
            _tween = _cg.DOFade(opacity, FadeDuration);
        }

        public bool IsVisible()
        {
            return _cg.alpha > 0;
        }
    }
}
