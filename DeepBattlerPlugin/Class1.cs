using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Plugins;
using HearthDb.Enums;
using Hearthstone_Deck_Tracker.Enums;
using System.Windows.Controls;

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
        OpponentTurn,
        EndGame
    }

    public class DeepBattlerPlugin : IPlugin
    {
        private static readonly int[] BASE_TAVERN_UPGRADE_COSTS = { 5, 7, 8, 10, 10 };
        private int? _currentTavernUpgradeCost = null;

        public string Name => "DeepBattlerPlugin";
        public string Description => "Track BG state with simplified logic";
        public string ButtonText => "Do Nothing";
        public string Author => "Guanming";
        public Version Version => new Version(1, 0, 10);
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
        private readonly string _path = @"E:\DeepBattler\game_state.json";
        //private readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Agent", "game_state.json");
        private string _heroName = "Unknown Hero";
        private int _playerHeroId = 0;
        private bool _upgradedThisTurn = false;

        public void OnLoad()
        {
            GameEvents.OnGameStart.Add(OnGameStart);
            GameEvents.OnTurnStart.Add(OnTurnStart);
            GameEvents.OnGameEnd.Add(OnGameEnd);
        }

        public void OnUnload() { }
        public void OnButtonPress() { }

        private bool IsInRecruitmentPhase()
        {
            var playerEntity = Core.Game.PlayerEntity;
            if (playerEntity == null)
                return false;
            return Core.Game.Entities.Values.Any(e => e.GetTag(GameTag.IS_BACON_POOL_MINION) == 1);
        }

        public void OnUpdate()
        {
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
            }
            _lastTavernTier = 1;
            CurrentPhase = GamePhaseEnum.StartGame;
            _currentTavernUpgradeCost = null;
            UpdateGameState(true);
        }

        private void OnGameEnd()
        {
            CurrentPhase = GamePhaseEnum.EndGame;
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
            _upgradedThisTurn = false;
        }

        private void OnTurnStart(ActivePlayer player)
        {
            if (player == ActivePlayer.Player)
            {
                if (_currentTavernUpgradeCost == null)
                {
                    _currentTavernUpgradeCost = BASE_TAVERN_UPGRADE_COSTS[0];
                }
                else
                {
                    _currentTavernUpgradeCost = Math.Max(0, _currentTavernUpgradeCost.Value - 1);
                }
                _upgradedThisTurn = false;
                CurrentPhase = GamePhaseEnum.PlayerTurn;
                _isPlayerTurnStarted = true;
                _updateCounterSincePlayerTurn = 0;
                UpdateGameState();
            }
            var playerEntity = Core.Game.PlayerEntity;
            if (playerEntity == null)
                return;
            if (player == ActivePlayer.Opponent)
            {
                CurrentPhase = GamePhaseEnum.OpponentTurn;
                UpdateGameState();
            }
        }

        public void PlayerUpgradedTavern(int newTier)
        {
            _upgradedThisTurn = true;
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
                var json = "{";
                json += $"\"HeroName\":\"{EscapeJson(_heroName)}\",";
                json += $"\"HeroHealth\":{actualHealth},";
                json += $"\"Coins\":{availableCoins},";
                json += $"\"TavernUpgradeCost\":\"{tavernUpgradeCostStr}\",";
                json += $"\"HeroPowerDescription\":\"{EscapeJson(heroPowerDescription)}\",";
                json += $"\"CurrentTavernTier\":{tavernTier},";
                json += $"\"HeroPowerCost\":{heroPowerCost},";
                json += "\"Warband\":" + EntitiesToJson(currentWarband) + ",";
                json += "\"Hand\":" + EntitiesToJsonNoRace(currentHand) + ",";
                json += "\"TavernEntities\":" + EntitiesToJson(currentTavernEntities);
                json += "}";
                try { File.WriteAllText(_path, json); } catch (Exception) { }
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
            return (Core.Game.Player?.Hand?
                .Where(e =>
                    e.Card != null
                    && !string.IsNullOrEmpty(e.Card.Name)
                )
                .Select(e => new CardEntityInfo
                {
                    Name = e.Card.Name,
                    Description = CleanCardText(e.Card.Text),
                    IsSpell = e.GetTag(GameTag.CARDTYPE) == (int)CardType.BATTLEGROUND_SPELL
                })
                .ToArray()) ?? Array.Empty<CardEntityInfo>();
        }

        private CardEntityInfo[] ExtractTavernEntitiesWithDetails()
        {
            if (Core.Game.PlayerEntity == null)
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

        private string SpellToJson(CardEntityInfo entity)
        {
            return "{" +
                   $"\"Name\":\"{EscapeJson(entity.Name)}\"," +
                   $"\"Description\":\"{EscapeJson(entity.Description)}\"," +
                   $"\"Race\":\"Tavern Spell\"," +
                   $"\"Tier\":{entity.Tier}" +
                   "}";
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
