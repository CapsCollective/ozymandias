using System;
using System.Collections.Generic;
using DG.Tweening;
using Managers;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
using static Managers.GameManager;
using String = Utilities.String;

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

        [SerializeField] private TooltipPlacement defaultTooltip;
        private TooltipPlacement _selectedTooltip;

        public static Action<bool> OnTooltipDisplay;
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
                Description = "Arcanists pluck from the fabric of reality in weird, wonderful, and most " +
                              "importantly, profitable ways. These eccentric scholars enjoy isolation and the " +
                              "pursuit of arcane knowledge. It is recommended their magic is controlled to prevent " +
                              "<b>REDACTED</b>, and <b>SUPER REDACTED</b>.",
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
            GameHud.OnTogglePhotoMode += _ => DeactivateTooltips();

            Manager.Inputs.NavigateTooltips.performed += NavigateTooltips;
        }

        private void DeactivateTooltips()
        {
            if(IsVisible()) _selectedTooltip.OnPointerExit(null);
            NavigationActive = false;
            OnTooltipDisplay?.Invoke(NavigationActive);
        }

        private void ToggleTooltips(InputAction.CallbackContext obj)
        {
            if (!Manager.State.InGame || Manager.GameHud.PhotoModeEnabled) return;
            Manager.Cards.SelectCard(-1);
            NavigationActive = !NavigationActive;
            OnTooltipDisplay?.Invoke(NavigationActive);

            if (NavigationActive) _selectedTooltip.OnPointerEnter(null);
            else _selectedTooltip.OnPointerExit(null);
        }

        private void NavigateTooltips(InputAction.CallbackContext obj)
        {
            if (!NavigationActive) return;
            
            Vector2 direction = obj.action.ReadValue<Vector2>();
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
                    int housingSpawnRate = Manager.Stats.HousingSpawnChance;
                    details.text =
                        $"{stat} {String.StatWithIcon(Stat.Housing)} for {adventurers} Adventurers in town" +
                        $"{FormattedBuildingString(Stat.Housing)}" +
                        $"{FormattedUpgradeString(Stat.Housing)}" +
                        $"{FormattedModifierString(Stat.Housing)}" +
                        ($"\n\n{diff.WithSign()} housing {Surplus(diff >= 0)}" + 
                         $"\n{HousingEffect(housingSpawnRate)}" + 
                         $"\n{HousingDescriptor(housingSpawnRate)}".Italics()
                        ).Center();
                    break;
                case Stat.Food:
                    int foodMod = Manager.Stats.FoodModifier;
                    details.text = 
                        $"{stat} {String.StatWithIcon(Stat.Food)} for {adventurers} Adventurers in town" +
                        FormattedBuildingString(Stat.Food) +
                        FormattedUpgradeString(Stat.Food) +
                        FormattedModifierString(Stat.Food) +
                        ($"\n\n{diff.WithSign()} food {Surplus(diff >= 0)}" +
                         $"\n{FoodEffect(foodMod)}" +
                         $"\n{FoodDescriptor(foodMod)}".Italics()
                        ).Center();
                    break;
                case Stat.Defence:
                    int unavailable = Manager.Adventurers.Unavailable;
                    details.text = 
                        $"{Manager.Stats.Defence} Total {String.StatWithIcon(Stat.Defence)}".Center() +
                        ($"+{adventurers}{("/" + (adventurers + unavailable)).Conditional(unavailable != 0)} from Adventurers in town" +
                        $"\n({unavailable} out questing)".Conditional(unavailable != 0)).ListItem() + 
                        FormattedBuildingString(Stat.Defence) + 
                        $"{Manager.Stats.MineStrikePenalty} from the miners strike"
                            .ListItem()
                            .Conditional(Manager.Stats.MineStrikePenalty != 0) + 
                        FormattedModifierString(Stat.Defence);
                    break;
                case Stat.Threat:
                    details.text = 
                        $"{Manager.Stats.Threat} Total {String.StatWithIcon(Stat.Threat)}".Center() +
                        $"+{Manager.Stats.BaseThreat} from events"
                            .ListItem() +
                        $"+{Manager.Quests.RadiantQuestCellCount} from enemy camps"
                            .ListItem()
                            .Conditional(Manager.Quests.RadiantQuestCellCount != 0) +
                        $"+{Manager.Stats.ScarecrowThreat} from farms due to scarecrows"
                            .ListItem()
                            .Conditional(Manager.Stats.ScarecrowThreat != 0 ) +
                        FormattedModifierString(Stat.Threat);
                    break;
                case Stat.Stability:
                    int change = Manager.Stats.Defence - Manager.Stats.Threat;
                    int turnsUntilDestruction = change >= 0 ? 0 : ((Manager.Stats.Stability - 1) / -change) + 1;
                    details.text = (
                        $"{Manager.Stats.Stability}/100 Town Stability" +
                        $"\n{change.WithSign()} next turn ({Manager.Stats.Defence} Defence - {Manager.Stats.Threat} Threat)" + 
                        $"\n{turnsUntilDestruction} {"turn".Pluralise(turnsUntilDestruction)} until town destruction".Conditional(change < 0)
                    ).Center();
                    break;
                case Stat.Spending:
                    details.text = 
                        $"{Manager.Stats.WealthPerTurn} {String.StatWithIcon(Stat.Spending)}".Center() +
                        ($"+{(Manager.EventQueue.Flags[Flag.Cosmetics] ? 0 : adventurers)}" +
                         $"/{Manager.Adventurers.Count}".Conditional(Manager.Adventurers.Unavailable > 0) +
                         " from Adventurers in town" +
                         "\n(No spending from adventurers due to Cosmetic Craze)".Conditional(Manager.EventQueue.Flags[Flag.Cosmetics])
                        ).ListItem() +
                        $"+{StartingSalary + Manager.Stats.GetUpgradeMod(Stat.Spending) * BaseStatMultiplier} Starting Salary".ListItem() +
                        FormattedBuildingString(Stat.Spending) +
                        FormattedModifierString(Stat.Spending);
                    break; 
                default: // Stat for a guild
                    Stat statType = config.Stat.Value;
                    Guild guild = (Guild) statType;
                    string guildName = config.Stat.ToString();
                    int count = Manager.Adventurers.GetCount(guild, true);
                    int spawnChance = Manager.Stats.SpawnChance(guild);
                    diff = stat - count;
                    details.text =
                        $"{stat} Satisfaction for {count} {guildName.Pluralise(count)} ({String.StatIcon(statType)}) in town" +
                        FormattedBuildingString(statType) +
                        FormattedUpgradeString(statType) +
                        FormattedFoodModifierString +
                        FormattedModifierString(statType) +
                        $"\n\n{diff.WithSign()} Satisfaction {Surplus(diff >= 0)}".Center() +
                        $"\n{Mathf.Abs(spawnChance)}%{"(max)".Conditional(spawnChance == Manager.Stats.MaxSpawnChance)} {guildName} {(spawnChance >= 0 ? "spawn" : "leave")} chance per turn".Center();
                    break;
            }
        }
        private string FormattedBuildingString(Stat stat)
        {
            int buildingMod = Manager.Structures.GetStat(stat) * Manager.Stats.StatMultiplier(stat);
            return $"{buildingMod.WithSign()} from Buildings".ListItem();
        }

        private string Surplus(bool isSurplus) => isSurplus ? "surplus" : "shortage";

        private string FormattedUpgradeString(Stat stat)
        {
            int upgradeMod = Manager.Stats.GetUpgradeMod(stat) * Manager.Stats.StatMultiplier(stat);
            return $"+{upgradeMod} from Upgrades".ListItem().Conditional(upgradeMod != 0);
        }

        // Formatted string for food modifiers (specifically for adventurer)
        private string FormattedFoodModifierString
        {
            get
            {
                int mod = Manager.Stats.FoodModifier;
                if (mod == 0) return "";
                bool isFoodInSurplus = mod > 0;
                return $"{mod.WithSign()} from {String.StatWithIcon(Stat.Food)} {Surplus(isFoodInSurplus)}".ListItem();
            }
        }

        // Formatted string for listing all stat modifiers. 
        private string FormattedModifierString(Stat stat)
        {
            string formattedModifierString = "";

            foreach (var modifier in Manager.Stats.Modifiers[stat])
            {
                formattedModifierString += (
                    $"{modifier.amount.WithSign()} {modifier.reason}" +
                    $"\n({modifier.turnsLeft} {"turn".Pluralise(modifier.turnsLeft)} remaining)".Conditional(modifier.turnsLeft != -1)
                ).ListItem();
            }
            
            return formattedModifierString;
        }

        private string FoodDescriptor(int foodMod)
        {
            return foodMod switch
            {
                >=  2 => "There are daily feasts",
                    1 => "The adventurers are well fed",
                    0 => "The town is getting by",
                   -1 => "Rations have taken effect",
                <= -2 => "People are starving",
            };
        }

        private string FoodEffect(int foodMod) => foodMod == 0 
            ? "No modifiers to satisfaction" 
            : foodMod.WithSign() + " to all adventurer satisfaction";

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

        private string HousingSpawnName(int spawnRate)
        {
            return spawnRate switch
            {
                3 => "Max",
                2 => "High",
                1 => "Low",
                0 => "No",
                _ => ""
            };
        }

        private string HousingEffect(int spawnRate)
        {
            return spawnRate == -1 
                ? "Adventurers will start to flee" 
                : HousingSpawnName(spawnRate) + " adventurer spawn chance";  
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
