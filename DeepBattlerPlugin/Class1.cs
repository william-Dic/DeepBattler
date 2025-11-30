using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;
using Hearthstone_Deck_Tracker.Plugins;
using HearthDb.Enums;
using Hearthstone_Deck_Tracker.Enums;
using System.Windows.Controls;
using System.Windows;

namespace DeepBattlerPlugin
{
    public class CardEntityInfo
    {
        public string Name { get; set; }
        public int? Atk { get; set; }
        public int? Health { get; set; }
        public string Description { get; set; }
        public string Race { get; set; }
        public bool IsSpell { get; set; }
        public int? Tier { get; set; }
    }

    public enum GamePhaseEnum
    {
        StartGame,
        PlayerTurn,
        EndGame
    }
    
    public class TurnHistory
    {
        public int TurnNumber { get; set; }
        public string Phase { get; set; }
        public string State { get; set; }  // Full game state at this turn
        public string ActionTaken { get; set; }  // Action suggested by LLM (will be filled by Python agent)
        public string BattleResult { get; set; }  // "Win", "Loss", "Tie", "None"
        public int HealthBeforeBattle { get; set; }
        public int HealthAfterBattle { get; set; }
        public int HealthChange { get; set; }
        public double Reward { get; set; }  // Calculated reward for GRPO training
    }

    public class HeroChoiceInfo
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
    }

    public class DeepBattlerPlugin : IPlugin
    {
        private static readonly int[] BASE_TAVERN_UPGRADE_COSTS = { 5, 7, 8, 10, 10 };
        private int? _currentTavernUpgradeCost = null;

        public string Name => "DeepBattlerPlugin";
        public string Description => "Track BG state with simplified logic";
        public string ButtonText => "Do Nothing";
        public string Author => "Guanming";
        public Version Version => new Version(2, 1, 0);
        public MenuItem MenuItem => null;

        private bool _isPlayerTurnStarted = false;
        private int _updateCounterSincePlayerTurn = 0;
        private const int DelayBeforeFetchingTavern = 5;
        public GamePhaseEnum CurrentPhase { get; private set; } = GamePhaseEnum.StartGame;

        private int _lastHeroHealth;
        private int _lastCoins;
        private int _lastTavernTier = 1;
        private CardEntityInfo[] _lastWarband = Array.Empty<CardEntityInfo>();
        private CardEntityInfo[] _lastTavernEntities = Array.Empty<CardEntityInfo>();
        private CardEntityInfo[] _lastHand = Array.Empty<CardEntityInfo>();
        private readonly string _path = @"C:\Users\Guanming Wang\Desktop\DeepBattler\Agent\game_state.json";
        private readonly string _historyPath = @"C:\Users\Guanming Wang\Desktop\DeepBattler\Agent\game_history.json";
        private readonly string _resourcesRoot = @"C:\Users\Guanming Wang\Desktop\DeepBattler\Agent\resources";
        private readonly string _latestGameStatePath = @"C:\Users\Guanming Wang\Desktop\DeepBattler\Agent\real_time_caller\latest_game_state.json";
        //private readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Agent", "game_state.json");
        //private readonly string _historyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Agent", "game_history.json");
        //private readonly string _resourcesRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Agent", "resources");
        private AgentOutputWindow _agentOutputWindow;
        private string _heroName = "Unknown Hero";
        private int _playerHeroId = 0;
        private int _currentTurn = 0;
        private int _lastHealthBeforeBattle = 0;
        private string _lastBattleResult = "None"; // "Win", "Loss", "Tie", "None"
        private List<TurnHistory> _gameHistory = new List<TurnHistory>();
        private int? _finalRank = null;
        private string _currentGameFolder = "";

        public void OnLoad()
        {
            GameEvents.OnGameStart.Add(OnGameStart);
            GameEvents.OnTurnStart.Add(OnTurnStart);
            GameEvents.OnGameEnd.Add(OnGameEnd);
            GameEvents.OnInMenu.Add(OnInMenu);
            
            // Create and show agent output window
            try
            {
                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _agentOutputWindow = new AgentOutputWindow();
                        _agentOutputWindow.Show();
                    });
                }
                else
                {
                    // Fallback: create on a new thread with new Application
                    System.Threading.Thread thread = new System.Threading.Thread(() =>
                    {
                        var app = new Application();
                        app.Dispatcher.Invoke(() =>
                        {
                            _agentOutputWindow = new AgentOutputWindow();
                            _agentOutputWindow.Show();
                        });
                        app.Run();
                    });
                    thread.SetApartmentState(System.Threading.ApartmentState.STA);
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash plugin
                System.Diagnostics.Debug.WriteLine($"Error creating agent output window: {ex.Message}");
            }
        }

        public void OnUnload()
        {
            // Close agent output window
            try
            {
                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _agentOutputWindow?.Close();
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error closing agent output window: {ex.Message}");
            }
        }
        public void OnButtonPress() { }

        private bool IsInRecruitmentPhase()
        {
            var gameEntity = Core.Game?.GameEntity;
            if (gameEntity == null)
                return false;

            var step = (Step)gameEntity.GetTag(GameTag.STEP);
            var isRecruitmentStep =
                step == Step.MAIN_READY ||
                step == Step.MAIN_PRE_ACTION ||
                step == Step.MAIN_ACTION ||
                step == Step.MAIN_POST_ACTION;

            if (!isRecruitmentStep && CurrentPhase != GamePhaseEnum.StartGame)
                return false;

            var playerEntity = Core.Game.PlayerEntity;
            if (playerEntity == null)
                return false;
            return Core.Game.Entities.Values.Any(e => e.GetTag(GameTag.IS_BACON_POOL_MINION) == 1);
        }

        public void OnUpdate()
        {
            // Skip updates during hero selection phase
            if (IsHeroSelectionPhase())
                return;

            if (_heroName == "Unknown Hero" || _heroName.StartsWith("BaconPHhero"))
                UpdateHeroName();
            if (_isPlayerTurnStarted)
            {
                if (!IsInRecruitmentPhase())
                    return;
                _updateCounterSincePlayerTurn++;
                if (_updateCounterSincePlayerTurn >= DelayBeforeFetchingTavern)
                    UpdateGameState();
            }
            else
            {
                UpdateGameState();
            }
        }

        private void OnGameStart()
        {
            var playerHero = Core.Game.Player?.Hero;
            if (playerHero != null)
            {
                var heroCardId = playerHero.CardId;
                if (heroCardId != null && HearthDb.Cards.All.TryGetValue(heroCardId, out var foundHeroCard))
                    _heroName = foundHeroCard?.Name ?? "Unknown Hero";
                else
                    _heroName = "Unknown Hero";
                _playerHeroId = playerHero.Id;
                
                // Initialize health tracking
                var baseHealth = playerHero.GetTag(GameTag.HEALTH);
                var damage = playerHero.GetTag(GameTag.DAMAGE);
                _lastHealthBeforeBattle = baseHealth - damage;
            }
            _lastTavernTier = 1;
            _currentTurn = 0;
            _lastBattleResult = "None";
            _gameHistory = new List<TurnHistory>();
            _finalRank = null;
            InitializeResourcesFolder();
            CurrentPhase = GamePhaseEnum.StartGame;
            _currentTavernUpgradeCost = null;
            // Don't update game state at game start - wait until hero is selected
        }

        private void OnGameEnd()
        {
            CurrentPhase = GamePhaseEnum.EndGame;
            
            // Try to get final rank (if available in HDT API)
            // This might need adjustment based on actual HDT API
            var playerEntity = Core.Game.PlayerEntity;
            if (playerEntity != null)
            {
                // Try to get rank from game state if available
                // Note: This is a placeholder - actual implementation depends on HDT API
            }
            
            // Save final game history
            SaveGameHistory();
            
            UpdateGameState();
            ResetInternalState();
        }

        private void ResetInternalState()
        {
            _currentTavernUpgradeCost = null;
            _lastHeroHealth = 0;
            _lastCoins = 0;
            _lastTavernTier = 1;
            _lastWarband = Array.Empty<CardEntityInfo>();
            _lastTavernEntities = Array.Empty<CardEntityInfo>();
            _lastHand = Array.Empty<CardEntityInfo>();
            _heroName = "Unknown Hero";
            _playerHeroId = 0;
            _isPlayerTurnStarted = false;
            _currentTurn = 0;
            _lastHealthBeforeBattle = 0;
            _lastBattleResult = "None";
            _gameHistory = new List<TurnHistory>();
            _finalRank = null;
            _currentGameFolder = "";
        }
        
        private void OnInMenu()
        {
            // Reset battle result when returning to menu
            _lastBattleResult = "None";
        }
        
        private void CheckBattleResult()
        {
            var heroEntity = Core.Game.Player?.Hero;
            if (heroEntity == null || _lastHealthBeforeBattle == 0)
                return;
            
            var baseHealth = heroEntity.GetTag(GameTag.HEALTH);
            var damage = heroEntity.GetTag(GameTag.DAMAGE);
            var currentHealth = baseHealth - damage;
            
            // If health increased, likely a win (or tie with healing)
            // If health decreased, likely a loss
            // If health unchanged, likely a tie
            if (currentHealth > _lastHealthBeforeBattle)
            {
                _lastBattleResult = "Win";
            }
            else if (currentHealth < _lastHealthBeforeBattle)
            {
                _lastBattleResult = "Loss";
            }
            else
            {
                // Could be tie or first turn
                if (_currentTurn > 0)
                {
                    _lastBattleResult = "Tie";
                }
            }
        }
        
        private void RecordTurnHistory()
        {
            var heroEntity = Core.Game.Player?.Hero;
            if (heroEntity == null)
                return;
            
            var baseHealth = heroEntity.GetTag(GameTag.HEALTH);
            var damage = heroEntity.GetTag(GameTag.DAMAGE);
            var currentHealth = baseHealth - damage;
            
            // Get current game state as JSON string (we'll parse it as object)
            var currentStateJson = GetCurrentStateAsJson();
            
            // Calculate reward based on battle result and health change
            double reward = CalculateReward(_lastBattleResult, _lastHealthBeforeBattle, currentHealth);
            
            var turnHistory = new TurnHistory
            {
                TurnNumber = _currentTurn - 1, // Previous turn
                Phase = CurrentPhase.ToString(),
                State = currentStateJson,
                ActionTaken = "", // Will be filled by Python agent reading game_state.json
                BattleResult = _lastBattleResult,
                HealthBeforeBattle = _lastHealthBeforeBattle,
                HealthAfterBattle = currentHealth,
                HealthChange = currentHealth - _lastHealthBeforeBattle,
                Reward = reward
            };
            
            _gameHistory.Add(turnHistory);
            WriteTurnHistorySnapshot(turnHistory);
            SaveGameHistory(false);
        }
        
        private double CalculateReward(string battleResult, int healthBefore, int healthAfter)
        {
            double reward = 0.0;
            int healthChange = healthAfter - healthBefore;
            
            // Base reward from battle result
            switch (battleResult)
            {
                case "Win":
                    reward += 10.0;
                    break;
                case "Loss":
                    reward -= 5.0;
                    break;
                case "Tie":
                    reward += 2.0;
                    break;
            }
            
            // Health change penalty/bonus (losing health is bad, gaining is good)
            reward += healthChange * 0.5;
            
            // Survival bonus (staying alive is good)
            if (healthAfter > 0)
            {
                reward += 1.0;
            }
            
            return reward;
        }
        
        private string GetCurrentStateAsJson()
        {
            var heroEntity = Core.Game.Player?.Hero;
            var playerEntity = Core.Game.PlayerEntity;
            if (heroEntity == null || playerEntity == null)
                return "{}";

            // Get hero name directly from the game entity to ensure it's always up-to-date
            string currentHeroName = _heroName; // Default to cached value
            var heroCardId = heroEntity.CardId;
            if (heroCardId != null && HearthDb.Cards.All.TryGetValue(heroCardId, out var foundHeroCard))
            {
                if (!string.IsNullOrEmpty(foundHeroCard.Name) && !foundHeroCard.Name.StartsWith("BaconPHhero"))
                {
                    currentHeroName = foundHeroCard.Name;
                    // Update cached value for consistency
                    if (_heroName != currentHeroName)
                    {
                        _heroName = currentHeroName;
                    }
                }
            }

            var playerId = playerEntity.GetTag(GameTag.PLAYER_ID);
            var heroPowerEntity = Core.Game.Entities.Values.FirstOrDefault(e =>
                e.GetTag(GameTag.CONTROLLER) == playerId &&
                e.GetTag(GameTag.CARDTYPE) == (int)CardType.HERO_POWER);
            var baseHealth = heroEntity.GetTag(GameTag.HEALTH);
            var damage = heroEntity.GetTag(GameTag.DAMAGE);
            var actualHealth = baseHealth - damage;
            var coins = playerEntity.GetTag(GameTag.RESOURCES);
            var coinsUsed = playerEntity.GetTag(GameTag.RESOURCES_USED);
            var availableCoins = coins - coinsUsed;
            var tavernTier = playerEntity.GetTag(GameTag.PLAYER_TECH_LEVEL);
            var currentWarband = ExtractWarband();
            var currentHand = ExtractHand();
            var currentTavernEntities = ExtractTavernEntitiesWithDetails();
            var heroChoices = ExtractHeroChoices();
            var heroPowerDescription = CleanCardText(heroPowerEntity?.Card?.Text);
            var heroPowerCost = heroPowerEntity?.GetTag(GameTag.COST) ?? 0;
            var tavernUpgradeCostStr = "None";
            if (CurrentPhase == GamePhaseEnum.StartGame)
            {
                tavernUpgradeCostStr = "undefined";
            }
            else if (tavernTier < 6)
            {
                if (_currentTavernUpgradeCost == null && CurrentPhase == GamePhaseEnum.PlayerTurn)
                {
                    _currentTavernUpgradeCost = BASE_TAVERN_UPGRADE_COSTS[0];
                }
                if (_currentTavernUpgradeCost.HasValue)
                {
                    tavernUpgradeCostStr = _currentTavernUpgradeCost.Value.ToString();
                }
            }

            return BuildGameStateJson(
                _currentTurn,
                CurrentPhase,
                currentHeroName,
                actualHealth,
                availableCoins,
                tavernUpgradeCostStr,
                heroPowerDescription,
                heroPowerCost,
                tavernTier,
                currentWarband,
                currentHand,
                currentTavernEntities,
                heroChoices);
        }
        
        private void SaveGameHistory(bool isFinalSave = true)
        {
            if (_gameHistory.Count == 0)
                return;
            
            var historyJson = "{\n";
            historyJson += "  \"game_metadata\": {\n";
            historyJson += $"    \"hero_name\": \"{EscapeJson(_heroName)}\",\n";
            historyJson += $"    \"total_turns\": {_currentTurn},\n";
            historyJson += $"    \"final_rank\": {(isFinalSave ? (_finalRank?.ToString() ?? "null") : "null")},\n";
            historyJson += $"    \"status\": \"{(isFinalSave ? "complete" : "in_progress")}\"\n";
            historyJson += "  },\n";
            historyJson += "  \"turns\": [\n";
            
            var turnParts = new List<string>();
            foreach (var turn in _gameHistory)
            {
                var turnJson = "    {\n";
                turnJson += $"      \"turn_number\": {turn.TurnNumber},\n";
                turnJson += $"      \"phase\": \"{turn.Phase}\",\n";
                var stateStr = turn.State ?? "{}";
                turnJson += $"      \"state\": {stateStr},\n";
                turnJson += $"      \"action_taken\": \"{EscapeJson(turn.ActionTaken ?? "")}\",\n";
                turnJson += $"      \"battle_result\": \"{turn.BattleResult}\",\n";
                turnJson += $"      \"health_before_battle\": {turn.HealthBeforeBattle},\n";
                turnJson += $"      \"health_after_battle\": {turn.HealthAfterBattle},\n";
                turnJson += $"      \"health_change\": {turn.HealthChange},\n";
                turnJson += $"      \"reward\": {turn.Reward}\n";
                turnJson += "    }";
                turnParts.Add(turnJson);
            }
            
            historyJson += string.Join(",\n", turnParts);
            historyJson += "\n  ]\n";
            historyJson += "}";
            
            try { File.WriteAllText(_historyPath, historyJson); } catch (Exception) { }
        }

        private void OnTurnStart(ActivePlayer player)
        {
            if (player == ActivePlayer.Player)
            {
                // Check battle result from previous turn and record it
                if (_currentTurn > 0)
                {
                    CheckBattleResult();
                    RecordTurnHistory();
                }
                
                // Increment turn counter
                _currentTurn++;
                
                if (_currentTavernUpgradeCost == null)
                {
                    _currentTavernUpgradeCost = BASE_TAVERN_UPGRADE_COSTS[0];
                }
                else
                {
                    _currentTavernUpgradeCost = Math.Max(0, _currentTavernUpgradeCost.Value - 1);
                }
                CurrentPhase = GamePhaseEnum.PlayerTurn;
                _isPlayerTurnStarted = true;
                _updateCounterSincePlayerTurn = 0;
                
                // Store health before battle
                var heroEntity = Core.Game.Player?.Hero;
                if (heroEntity != null)
                {
                    var baseHealth = heroEntity.GetTag(GameTag.HEALTH);
                    var damage = heroEntity.GetTag(GameTag.DAMAGE);
                    _lastHealthBeforeBattle = baseHealth - damage;
                }
                
                UpdateGameState();
            }
            // In Battlegrounds, we only track player's turn, not opponent's turn
            // Opponent actions happen during combat which is automatic
        }

        public void PlayerUpgradedTavern(int newTier)
        {
            _lastTavernTier = newTier;
            if (newTier < 6)
            {
                _currentTavernUpgradeCost = BASE_TAVERN_UPGRADE_COSTS[newTier - 1];
            }
            else
            {
                _currentTavernUpgradeCost = null;
            }
            UpdateGameState(true);
        }

        private void UpdateHeroName()
        {
            var playerHero = Core.Game.Player?.Hero;
            if (playerHero != null)
            {
                var heroCardId = playerHero.CardId;
                if (heroCardId != null && HearthDb.Cards.All.TryGetValue(heroCardId, out var foundHeroCard))
                {
                    if (!string.IsNullOrEmpty(foundHeroCard.Name) && !foundHeroCard.Name.StartsWith("BaconPHhero"))
                    {
                        _heroName = foundHeroCard.Name;
                        UpdateGameState();
                    }
                }
            }
        }

        private void UpdateGameState(bool forceWrite = false)
        {
            var heroEntity = Core.Game.Player?.Hero;
            var playerEntity = Core.Game.PlayerEntity;
            if (heroEntity == null || playerEntity == null)
                return;

            if (!forceWrite && CurrentPhase != GamePhaseEnum.StartGame && !IsInRecruitmentPhase())
                return;
            
            // Update hero name if needed
            if (_heroName == "Unknown Hero" || _heroName.StartsWith("BaconPHhero"))
            {
                var heroCardId = heroEntity.CardId;
                if (heroCardId != null && HearthDb.Cards.All.TryGetValue(heroCardId, out var foundHeroCard))
                {
                    if (!string.IsNullOrEmpty(foundHeroCard.Name) && !foundHeroCard.Name.StartsWith("BaconPHhero"))
                    {
                        _heroName = foundHeroCard.Name;
                    }
                }
            }
            var playerId = playerEntity.GetTag(GameTag.PLAYER_ID);
            var heroPowerEntity = Core.Game.Entities.Values.FirstOrDefault(e =>
                e.GetTag(GameTag.CONTROLLER) == playerId &&
                e.GetTag(GameTag.CARDTYPE) == (int)CardType.HERO_POWER);
            var baseHealth = heroEntity.GetTag(GameTag.HEALTH);
            var damage = heroEntity.GetTag(GameTag.DAMAGE);
            var actualHealth = baseHealth - damage;
            var coins = playerEntity.GetTag(GameTag.RESOURCES);
            var coinsUsed = playerEntity.GetTag(GameTag.RESOURCES_USED);
            var availableCoins = coins - coinsUsed;
            var tavernTier = playerEntity.GetTag(GameTag.PLAYER_TECH_LEVEL);
            if (tavernTier > _lastTavernTier)
            {
                PlayerUpgradedTavern(tavernTier);
                return;
            }
            var currentWarband = ExtractWarband();
            var currentHand = ExtractHand();
            var currentTavernEntities = ExtractTavernEntitiesWithDetails();
            var heroChoices = ExtractHeroChoices();
            var heroPowerDescription = CleanCardText(heroPowerEntity?.Card?.Text);
            var heroPowerCost = heroPowerEntity?.GetTag(GameTag.COST) ?? 0;
            var tavernUpgradeCostStr = "None";
            if (CurrentPhase == GamePhaseEnum.StartGame)
            {
                tavernUpgradeCostStr = "undefined";
            }
            else if (tavernTier < 6)
            {
                if (_currentTavernUpgradeCost == null && CurrentPhase == GamePhaseEnum.PlayerTurn)
                {
                    _currentTavernUpgradeCost = BASE_TAVERN_UPGRADE_COSTS[0];
                }
                if (_currentTavernUpgradeCost.HasValue)
                {
                    tavernUpgradeCostStr = _currentTavernUpgradeCost.Value.ToString();
                }
            }
            bool changed = forceWrite
                       || actualHealth != _lastHeroHealth
                       || availableCoins != _lastCoins
                       || tavernTier != _lastTavernTier
                       || !EntitiesEqual(currentWarband, _lastWarband)
                       || !EntitiesEqual(currentTavernEntities, _lastTavernEntities)
                       || !EntitiesEqual(currentHand, _lastHand);
            if (changed)
            {
                _lastHeroHealth = actualHealth;
                _lastCoins = availableCoins;
                _lastWarband = currentWarband;
                _lastTavernEntities = currentTavernEntities;
                _lastHand = currentHand;
                var json = BuildGameStateJson(
                    _currentTurn,
                    CurrentPhase,
                    _heroName,
                    actualHealth,
                    availableCoins,
                    tavernUpgradeCostStr,
                    heroPowerDescription,
                    heroPowerCost,
                    tavernTier,
                    currentWarband,
                    currentHand,
                    currentTavernEntities,
                    heroChoices);
                try { File.WriteAllText(_path, json); } catch (Exception) { }
                WriteStateSnapshot(_currentTurn, json);
            }
        }

        private CardEntityInfo[] ExtractWarband()
        {
            return (Core.Game.Player?.Board?
                .Where(e => e.GetTag(GameTag.ZONE) == (int)Zone.PLAY &&
                            e.GetTag(GameTag.CARDTYPE) == (int)CardType.MINION &&
                            e.Card != null)
                .Select(e => new CardEntityInfo
                {
                    Name = e.Card.Name,
                    Atk = e.GetTag(GameTag.ATK),
                    Health = e.GetTag(GameTag.HEALTH) - e.GetTag(GameTag.DAMAGE),
                    Description = CleanCardText(e.Card.Text),
                    Race = GetRaceString(e.Card.Race),
                    IsSpell = false,
                    Tier = e.Card?.TechLevel
                })
                .ToArray()) ?? Array.Empty<CardEntityInfo>();
        }

        private CardEntityInfo[] ExtractHand()
        {
            var handEntities = Core.Game.Player?.Hand;
            if (handEntities == null)
                return Array.Empty<CardEntityInfo>();

            return handEntities
                .Where(e =>
                    e.Card != null
                    && !string.IsNullOrEmpty(e.Card.Name)
                    && e.GetTag(GameTag.CARDTYPE) != (int)CardType.HERO
                )
                .Select(e => new CardEntityInfo
                {
                    Name = e.Card.Name,
                    Description = CleanCardText(e.Card.Text),
                    IsSpell = e.GetTag(GameTag.CARDTYPE) == (int)CardType.BATTLEGROUND_SPELL
                })
                .ToArray();
        }

        private CardEntityInfo[] ExtractTavernEntitiesWithDetails()
        {
            if (Core.Game.PlayerEntity == null || !IsInRecruitmentPhase())
                return Array.Empty<CardEntityInfo>();
            var playerId = Core.Game.PlayerEntity.GetTag(GameTag.PLAYER_ID);
            var tavernMinions = Core.Game.Entities.Values
                .Where(e => e.GetTag(GameTag.ZONE) == (int)Zone.PLAY &&
                            e.Id != _playerHeroId &&
                            e.Card != null &&
                            !string.IsNullOrEmpty(e.Card.Name) &&
                            e.GetTag(GameTag.IS_BACON_POOL_MINION) == 1 &&
                            e.GetTag(GameTag.CONTROLLER) != playerId &&
                            e.GetTag(GameTag.CARDTYPE) == (int)CardType.MINION)
                .Select(e => new CardEntityInfo
                {
                    Name = e.Card.Name,
                    Atk = e.GetTag(GameTag.ATK),
                    Health = e.GetTag(GameTag.HEALTH) - e.GetTag(GameTag.DAMAGE),
                    Description = CleanCardText(e.Card.Text),
                    Race = GetRaceString(e.Card.Race),
                    IsSpell = false,
                    Tier = e.Card?.TechLevel
                });
            var tavernSpells = Core.Game.Entities.Values
                .Where(e => e.GetTag(GameTag.CARDTYPE) == (int)CardType.BATTLEGROUND_SPELL &&
                            e.GetTag(GameTag.ZONE) == (int)Zone.PLAY &&
                            e.Card != null &&
                            !string.IsNullOrEmpty(e.Card.Name))
                .Select(e => new CardEntityInfo
                {
                    Name = e.Card.Name,
                    Description = CleanCardText(e.Card.Text),
                    Race = "Tavern Spell",
                    IsSpell = true,
                    Tier = e.Card?.TechLevel
                });
            return tavernMinions.Concat(tavernSpells).ToArray();
        }

        private bool EntitiesEqual(CardEntityInfo[] arr1, CardEntityInfo[] arr2)
        {
            if (arr1.Length != arr2.Length)
                return false;
            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i].Name != arr2[i].Name ||
                    arr1[i].Description != arr2[i].Description ||
                    arr1[i].Race != arr2[i].Race ||
                    arr1[i].IsSpell != arr2[i].IsSpell ||
                    arr1[i].Tier != arr2[i].Tier)
                    return false;
                if (!arr1[i].IsSpell && (arr1[i].Atk != arr2[i].Atk || arr1[i].Health != arr2[i].Health))
                    return false;
            }
            return true;
        }

        private string EntitiesToJson(CardEntityInfo[] entities)
        {
            var parts = new List<string>();
            foreach (var e in entities)
            {
                if (e.IsSpell)
                    parts.Add(SpellToJson(e));
                else
                    parts.Add(MinionToJson(e));
            }
            return "[" + string.Join(",", parts) + "]";
        }

        private string BuildGameStateJson(
            int turnNumber,
            GamePhaseEnum phase,
            string heroName,
            int heroHealth,
            int availableGold,
            string tavernUpgradeCost,
            string heroPowerDescription,
            int heroPowerCost,
            int tavernTier,
            CardEntityInfo[] warband,
            CardEntityInfo[] hand,
            CardEntityInfo[] tavernEntities,
            HeroChoiceInfo[] heroChoices)
        {
            var json = "{\n";
            json += "  \"game_state\": {\n";
            json += $"    \"turn_number\": {turnNumber},\n";
            json += $"    \"phase\": \"{phase}\"\n";
            json += "  },\n";
            json += "  \"player_hero\": {\n";
            json += $"    \"name\": \"{EscapeJson(heroName)}\",\n";
            json += $"    \"current_health\": {heroHealth},\n";
            json += $"    \"hero_power\": {{\n";
            json += $"      \"description\": \"{EscapeJson(heroPowerDescription)}\",\n";
            json += $"      \"cost\": {heroPowerCost}\n";
            json += "    }\n";
            json += "  },\n";
            json += "  \"resources\": {\n";
            json += $"    \"available_gold\": {availableGold},\n";
            json += $"    \"tavern_tier\": {tavernTier},\n";
            json += $"    \"tavern_upgrade_cost\": \"{tavernUpgradeCost}\"\n";
            json += "  },\n";
            json += "  \"board_state\": {\n";
            json += $"    \"warband_size\": {warband.Length},\n";
            json += $"    \"warband\": {EntitiesToJsonFormatted(warband)},\n";
            json += $"    \"hand_size\": {hand.Length},\n";
            json += $"    \"hand\": {EntitiesToJsonNoRaceFormatted(hand)},\n";
            json += $"    \"tavern_available\": {tavernEntities.Length},\n";
            json += $"    \"tavern_entities\": {EntitiesToJsonFormatted(tavernEntities)}\n";
            json += "  }";

            if (heroChoices != null && heroChoices.Length > 0)
            {
                json += ",\n  \"hero_selection\": {\n";
                json += $"    \"available\": {HeroChoicesToJson(heroChoices)}\n";
                json += "  }\n";
            }
            else
            {
                json += "\n";
            }

            json += "}";
            return json;
        }

        private void InitializeResourcesFolder()
        {
            try
            {
                Directory.CreateDirectory(_resourcesRoot);
                _currentGameFolder = Path.Combine(_resourcesRoot, $"game_{DateTime.UtcNow:yyyyMMdd_HHmmss}");
                Directory.CreateDirectory(_currentGameFolder);
            }
            catch (Exception)
            {
                _currentGameFolder = _resourcesRoot;
            }
        }

        private string GetTurnFolderPath(int turnNumber)
        {
            var baseFolder = string.IsNullOrEmpty(_currentGameFolder) ? _resourcesRoot : _currentGameFolder;
            var folderName = $"turn_{Math.Max(turnNumber, 0):D3}";
            return Path.Combine(baseFolder, folderName);
        }

        private void WriteStateSnapshot(int turnNumber, string json)
        {
            try
            {
                var folder = GetTurnFolderPath(turnNumber);
                Directory.CreateDirectory(folder);
                File.WriteAllText(Path.Combine(folder, "game_state.json"), json);
                
                // Also write to latest_game_state.json for real-time caller
                try
                {
                    var latestDir = Path.GetDirectoryName(_latestGameStatePath);
                    if (!string.IsNullOrEmpty(latestDir))
                    {
                        Directory.CreateDirectory(latestDir);
                    }
                    // Use UTF8Encoding without BOM to avoid BOM issues in Python
                    var utf8NoBom = new System.Text.UTF8Encoding(false);
                    File.WriteAllText(_latestGameStatePath, json, utf8NoBom);
                }
                catch (Exception)
                {
                    // ignore latest_game_state.json write failures
                }
            }
            catch (Exception)
            {
                // ignore snapshot failures
            }
        }

        private void WriteTurnHistorySnapshot(TurnHistory turn)
        {
            try
            {
                var folder = GetTurnFolderPath(turn.TurnNumber);
                Directory.CreateDirectory(folder);
                var historyJson = BuildTurnHistoryJson(turn);
                File.WriteAllText(Path.Combine(folder, "game_history.json"), historyJson);
            }
            catch (Exception)
            {
                // ignore snapshot failures
            }
        }

        private string BuildTurnHistoryJson(TurnHistory turn)
        {
            var json = "{\n";
            json += $"  \"turn_number\": {turn.TurnNumber},\n";
            json += $"  \"phase\": \"{turn.Phase}\",\n";
            json += $"  \"state\": {turn.State ?? "{}"},\n";
            json += $"  \"action_taken\": \"{EscapeJson(turn.ActionTaken ?? "")}\",\n";
            json += $"  \"battle_result\": \"{turn.BattleResult}\",\n";
            json += $"  \"health_before_battle\": {turn.HealthBeforeBattle},\n";
            json += $"  \"health_after_battle\": {turn.HealthAfterBattle},\n";
            json += $"  \"health_change\": {turn.HealthChange},\n";
            json += $"  \"reward\": {turn.Reward}\n";
            json += "}";
            return json;
        }
        
        private string EntitiesToJsonFormatted(CardEntityInfo[] entities)
        {
            if (entities.Length == 0)
                return "[]";
            
            var parts = new List<string>();
            foreach (var e in entities)
            {
                if (e.IsSpell)
                    parts.Add("      " + SpellToJsonFormatted(e));
                else
                    parts.Add("      " + MinionToJsonFormatted(e));
            }
            return "[\n" + string.Join(",\n", parts) + "\n    ]";
        }

        private string MinionToJson(CardEntityInfo entity)
        {
            return "{" +
                   $"\"Name\":\"{EscapeJson(entity.Name)}\"," +
                   $"\"Atk\":{entity.Atk}," +
                   $"\"Health\":{entity.Health}," +
                   $"\"Description\":\"{EscapeJson(entity.Description)}\"," +
                   $"\"Race\":\"{EscapeJson(entity.Race)}\"," +
                   $"\"Tier\":{entity.Tier}" +
                   "}";
        }
        
        private string MinionToJsonFormatted(CardEntityInfo entity)
        {
            return "{\n" +
                   $"        \"name\": \"{EscapeJson(entity.Name)}\",\n" +
                   $"        \"attack\": {entity.Atk},\n" +
                   $"        \"health\": {entity.Health},\n" +
                   $"        \"description\": \"{EscapeJson(entity.Description)}\",\n" +
                   $"        \"race\": \"{EscapeJson(entity.Race)}\",\n" +
                   $"        \"tier\": {entity.Tier}\n" +
                   "      }";
        }

        private string SpellToJson(CardEntityInfo entity)
        {
            return "{" +
                   $"\"Name\":\"{EscapeJson(entity.Name)}\"," +
                   $"\"Description\":\"{EscapeJson(entity.Description)}\"," +
                   $"\"Race\":\"Tavern Spell\"," +
                   $"\"Tier\":{entity.Tier}" +
                   "}";
        }
        
        private string SpellToJsonFormatted(CardEntityInfo entity)
        {
            return "{\n" +
                   $"        \"name\": \"{EscapeJson(entity.Name)}\",\n" +
                   $"        \"type\": \"tavern_spell\",\n" +
                   $"        \"description\": \"{EscapeJson(entity.Description)}\",\n" +
                   $"        \"tier\": {entity.Tier}\n" +
                   "      }";
        }

        private string EntitiesToJsonNoRace(CardEntityInfo[] entities)
        {
            var parts = new List<string>();
            foreach (var e in entities)
            {
                var part = "{";
                part += $"\"Name\":\"{EscapeJson(e.Name)}\",";
                part += $"\"Description\":\"{EscapeJson(e.Description)}\"";
                part += "}";
                parts.Add(part);
            }
            return "[" + string.Join(",", parts) + "]";
        }
        
        private string EntitiesToJsonNoRaceFormatted(CardEntityInfo[] entities)
        {
            if (entities.Length == 0)
                return "[]";
            
            var parts = new List<string>();
            foreach (var e in entities)
            {
                var part = "      {\n";
                part += $"        \"name\": \"{EscapeJson(e.Name)}\",\n";
                part += $"        \"description\": \"{EscapeJson(e.Description)}\"\n";
                part += "      }";
                parts.Add(part);
            }
            return "[\n" + string.Join(",\n", parts) + "\n    ]";
        }

        private HeroChoiceInfo[] ExtractHeroChoices()
        {
            if (!IsHeroSelectionPhase())
                return Array.Empty<HeroChoiceInfo>();

            var playerEntity = Core.Game.PlayerEntity;
            if (playerEntity == null)
                return Array.Empty<HeroChoiceInfo>();

            var playerId = playerEntity.GetTag(GameTag.PLAYER_ID);
            
            // Get ALL heroes from HAND zone first (these are the selection candidates)
            var allHandHeroes = Core.Game.Player?.Hand?
                .Where(e =>
                    e.GetTag(GameTag.CARDTYPE) == (int)CardType.HERO &&
                    e.Card != null &&
                    !string.IsNullOrEmpty(e.Card.Name) &&
                    !IsPlaceholderHero(e))
                .ToList() ?? new List<Entity>();

            // If we have heroes in hand, try to identify which ones are selectable
            // In Battlegrounds, typically 4 heroes are shown, but player can only pick 2
            // The selectable ones might have specific tags or be in a specific order
            
            // Strategy: Get all heroes, but prioritize those that are NOT already selected
            // Check if hero is in PLAY zone (already selected) - exclude those
            var selectableHeroes = allHandHeroes
                .Where(e => 
                {
                    var zone = e.GetTag(GameTag.ZONE);
                    // Exclude heroes already in PLAY zone (selected)
                    return zone != (int)Zone.PLAY;
                })
                .ToList();

            // If we still have issues, get all heroes from hand and let the logic filter
            if (selectableHeroes.Count == 0 && allHandHeroes.Count > 0)
            {
                selectableHeroes = allHandHeroes;
            }

            // Also check SETASIDE for additional heroes that might be selectable
            // But be more careful - only get those controlled by player
            var setAsideHeroes = Core.Game.Entities.Values
                .Where(e =>
                    e.GetTag(GameTag.CARDTYPE) == (int)CardType.HERO &&
                    e.GetTag(GameTag.ZONE) == (int)Zone.SETASIDE &&
                    e.GetTag(GameTag.CONTROLLER) == playerId &&
                    e.Card != null &&
                    !string.IsNullOrEmpty(e.Card.Name) &&
                    !IsPlaceholderHero(e) &&
                    e.GetTag(GameTag.ZONE) != (int)Zone.PLAY)
                .ToList();

            // Combine and get unique heroes
            var allSelectableHeroes = selectableHeroes
                .Concat(setAsideHeroes)
                .GroupBy(e => e.Card.Name)
                .Select(g => g.First())
                .ToList();

            if (allSelectableHeroes.Count == 0)
                return Array.Empty<HeroChoiceInfo>();

            // Get hero descriptions from HearthDb if card.Text is empty
            var heroChoices = allSelectableHeroes
                .Select(entity =>
                {
                    var card = entity.Card;
                    var description = CleanCardText(card.Text);
                    
                    // If description is empty, try to get it from HearthDb
                    if (string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(card.Id))
                    {
                        if (HearthDb.Cards.All.TryGetValue(card.Id, out var dbCard))
                        {
                            description = CleanCardText(dbCard.Text);
                        }
                    }
                    
                    return new HeroChoiceInfo
                    {
                        Name = card.Name,
                        Description = description
                    };
                })
                .Where(choice => !string.IsNullOrEmpty(choice.Name) && 
                                 !choice.Name.StartsWith("BaconPHhero", StringComparison.OrdinalIgnoreCase) &&
                                 !choice.Name.Equals("BaconPHhero", StringComparison.OrdinalIgnoreCase))
                .OrderBy(choice => choice.Name)
                .ToArray(); // Return ALL heroes, not just 2

            return heroChoices;
        }
        
        private bool IsPlaceholderHero(Entity hero)
        {
            if (hero?.Card == null)
                return true;

            var cardId = hero.Card.Id;
            var cardName = hero.Card.Name;

            // Check if it's a placeholder by ID or name
            if (!string.IsNullOrEmpty(cardId) && 
                cardId.StartsWith("BaconPHhero", StringComparison.OrdinalIgnoreCase))
                return true;

            if (!string.IsNullOrEmpty(cardName) && 
                (cardName.StartsWith("BaconPHhero", StringComparison.OrdinalIgnoreCase) ||
                 cardName.Equals("BaconPHhero", StringComparison.OrdinalIgnoreCase)))
                return true;

            return false;
        }

        private bool IsHeroSelectionPhase()
        {
            // Check if we're in hero selection phase by looking for hero cards in hand
            // In Battlegrounds, hero selection cards appear in the player's hand
            var handHeroes = Core.Game.Player?.Hand?
                .Where(e =>
                    e.GetTag(GameTag.CARDTYPE) == (int)CardType.HERO &&
                    e.Card != null &&
                    !string.IsNullOrEmpty(e.Card.Name))
                .Count() ?? 0;
            
            // If there are hero cards in hand, we're in selection phase
            if (handHeroes > 0)
                return true;
            
            // Also check if current hero is a placeholder
            var hero = Core.Game.Player?.Hero;
            if (hero != null)
            {
                var heroCardId = hero.CardId;
                if (!string.IsNullOrEmpty(heroCardId) && 
                    heroCardId.StartsWith("BaconPHhero", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            
            return false;
        }

        private string EscapeJson(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }

        private string HeroChoicesToJson(IEnumerable<HeroChoiceInfo> choices)
        {
            var parts = choices?
                .Select(choice =>
                    "{ " +
                    $"\"name\": \"{EscapeJson(choice?.Name ?? "")}\", " +
                    $"\"description\": \"{EscapeJson(choice?.Description ?? "")}\" " +
                    "}")
                .ToArray() ?? Array.Empty<string>();
            return "[ " + string.Join(", ", parts) + " ]";
        }

        private string CleanCardText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";
            return text.Replace("\r", " ").Replace("\n", " ").Trim();
        }

        private string GetRaceString(string race)
        {
            return string.IsNullOrEmpty(race) ? "" : race;
        }
    }
}
