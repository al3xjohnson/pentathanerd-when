using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNet.SignalR;

namespace Pentathanerd.When
{
    public class GamePlayHub : Hub
    {
        #region Constants
        private const int AverageCharactersPerMinute = 195;
        private const int AvailableScoreBeforeBonus = 1200;
        private const int LowerBoundPlayTimeInSeconds = 2;
        private const int UpperBoundPlayTimeInSeconds = 6;
        private const int KeyPressThresholdPerTurn = 5;
        private const string DefaultTeamName = "NoNameGang";
        #endregion

        #region Statics
        private static IHubContext _gamePlayHub;
        private static readonly List<string> _connectedUsers = new List<string>();
        private static ConcurrentDictionary<string, PlayerStats> _connectedPlayers = new ConcurrentDictionary<string, PlayerStats>();
        private static string _activePlayerConnectionId;
        private static string _teamName;

        private static string _challengeText;
        private static bool _gameIsActive;
        private static TimeSpan _gameTime;

        private static Timer ArrowTimer { get; set; }

        private static TimerExtension GameClock { get; set; }

        private static double TotalHits
        {
            get
            {
                double totalHits = 0;
                foreach (var connectedPlayer in _connectedPlayers)
                {
                    totalHits += connectedPlayer.Value.Hits;
                }
                return totalHits;
            }
        }

        private static double TotalMisses
        {
            get
            {
                double totalMisses = 0;
                foreach (var connectedPlayer in _connectedPlayers)
                {
                    totalMisses += connectedPlayer.Value.Misses;
                }
                return totalMisses;
            }
        }

        private static double HitPercentage
        {
            get
            {
                double retValue = 0;
                var totalHits = TotalHits;
                var totalKeyPresses = totalHits + TotalMisses;
                if (totalKeyPresses > 0)
                {
                    var hitFraction = totalHits / totalKeyPresses;
                    var hitPercentage = hitFraction * 100;
                    retValue = Math.Round(hitPercentage, 2);
                }

                return retValue > 0 ? retValue : 0;
            }
        }

        private static double CompletionPercentage
        {
            get
            {
                double retValue = 0;
                if (_challengeText.Length > 0)
                {
                    var completionFraction = TotalHits / _challengeText.Length;
                    var completionPercent = completionFraction * 100;
                    retValue = Math.Round(completionPercent, 2);
                }

                return retValue;
            }
        }

        private static double Score
        {
            get
            {
                var totalScorePercentage = (HitPercentage + CompletionPercentage) / 2;
                var score = (totalScorePercentage / 100) * AvailableScoreBeforeBonus;
                var roundedScore = Math.Round(score, 0);

                return roundedScore > 0 ? roundedScore : 0;
            }
        }

        private static ScreenLocation CurrentScreenLocation
        {
            get
            {
                var screenLocation = ScreenLocation.Left;
                foreach (var connectedPlayer in _connectedPlayers)
                {
                    if (connectedPlayer.Key == _activePlayerConnectionId)
                    {
                        screenLocation = connectedPlayer.Value.ScreenLocation;
                    }
                }
                return screenLocation;
            }
        }
        #endregion

        #region Hub overrides
        public override Task OnConnected()
        {
            InitializeGamePlayHubContext();

            _connectedUsers.Add(Context.ConnectionId);
            UpdateConnectedUsersCount();

            if (_connectedPlayers.Count < 2)
                Clients.Caller.showPlayerSelectionModal();

            if (_gameIsActive)
            {
                Clients.Caller.updateDisplayForLateComer(GameClock.SecondsLeft, _challengeText, TotalHits, CurrentScreenLocation.ToString().ToLower(), HitPercentage, CompletionPercentage, Score, _teamName);
            }

            return base.OnConnected();
        }

        private static void InitializeGamePlayHubContext()
        {
            _gamePlayHub = GlobalHost.ConnectionManager.GetHubContext<GamePlayHub>();
        }

        private static void UpdateConnectedUsersCount()
        {
            _gamePlayHub.Clients.All.updateConnectedUsersCount(_connectedUsers.Count);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var connectionId = Context.ConnectionId;
            var connectedPlayer = _connectedPlayers.FirstOrDefault(x => x.Key == connectionId);

            if (connectedPlayer.Value != null)
            {

                PlayerStats player;
                if (connectedPlayer.Key != null)
                    _connectedPlayers.TryRemove(connectedPlayer.Key, out player);

                if (_connectedPlayers.Count < 2)
                {
                    EndGame(false);
                }
            }
            _connectedUsers.Remove(connectionId);
            UpdateConnectedUsersCount();

            return base.OnDisconnected(stopCalled);
        }
        #endregion

        private static void EndGame(bool earlyWinner)
        {
            ResetGameClient(earlyWinner);
            ResetGameValues();
        }

        private static void ResetGameClient(bool earlyWinner)
        {
            if (earlyWinner)
            {
                _gamePlayHub.Clients.All.endGameForEarlyWinner(GameClock.SecondsLeft);
            }
            else
            {
                _gamePlayHub.Clients.All.endGame(_gameIsActive);
            }

            foreach (var connectedPlayer in _connectedPlayers)
            {
                _gamePlayHub.Clients.Client(connectedPlayer.Key).enableResetGameButton();
            }
        }

        private static void ResetGameValues()
        {
            _gameIsActive = false;
            _activePlayerConnectionId = string.Empty;
            _challengeText = string.Empty;
            _teamName = string.Empty;

            StopArrowTimer();
            StopGameClock();
        }

        private static void StopArrowTimer()
        {
            if (ArrowTimer != null)
            {
                ArrowTimer.Stop();
                ArrowTimer.Dispose();
                ArrowTimer.Elapsed -= ArrowTimerOnElapsed;
            }
        }

        private static void StopGameClock()
        {
            if (GameClock != null)
            {
                GameClock.Stop();
                GameClock.Dispose();
                GameClock.Elapsed -= GameClockOnElapsed;
            }
        }

        public void RegisterPlayer(string location)
        {
            var screenLocation = ToScreenLocation(location);
            _connectedPlayers.TryAdd(Context.ConnectionId, new PlayerStats
            {
                ScreenLocation = screenLocation
            });

            SetScreenSelectedOnAllClients(location);
            Clients.Caller.showGameControls();

            if (_connectedPlayers.Count == 2)
            {
                foreach (var connectedPlayer in _connectedPlayers)
                {
                    if (connectedPlayer.Value.ScreenLocation == ScreenLocation.Left)
                    {
                        Clients.Client(connectedPlayer.Key).showTeamNameSelectionModal();
                    }
                }
            }
        }

        private static ScreenLocation ToScreenLocation(string location)
        {
            switch (location.ToLower())
            {
                case "left":
                    return ScreenLocation.Left;
                case "right":
                    return ScreenLocation.Right;
                default:
                    throw new ArgumentException("Not a valid screen location");
            }
        }

        public void SetSelectedScreens()
        {
            foreach (var connectedPlayer in _connectedPlayers)
            {
                SetScreenSelectedOnAllClients(connectedPlayer.Value.ScreenLocation.ToString());
            }
        }

        private void SetScreenSelectedOnAllClients(string location)
        {
            Clients.All.setScreenSelected(location.ToLower());
        }

        public void SetTeamName(string name)
        {
            if (!ConnectionIdBelongsToPlayer(Context.ConnectionId))
                return;

            if (string.IsNullOrEmpty(name))
                name = DefaultTeamName;

            _teamName = name;

            Clients.All.setTeamName(_teamName);

            foreach (var connectedPlayer in _connectedPlayers)
            {
                Clients.Client(connectedPlayer.Key).enableStartGameButton();
            }
        }

        private static bool ConnectionIdBelongsToPlayer(string connectionId)
        {
            var connectedPlayer = _connectedPlayers.FirstOrDefault(x => x.Key == connectionId);

            return connectedPlayer.Value != null;
        }

        public void StartGame()
        {
            if (!_connectedPlayers.Keys.Contains(Context.ConnectionId))
                return;

            if (_gameIsActive)
                return;

            _gameIsActive = true;
            LoadChallengeText();
            SetGameTime();
            InitializeGameClock();
            InitializeArrowTimer();
            SetStartingPlayer();
            SetIntervalForCurrentPlayer();
            
            GameClock.Start();
            Clients.All.startGame(_gameTime.TotalSeconds);
        }

        private void LoadChallengeText()
        {
            _challengeText = ChallengeTextLibrary.GetRandomChallengeText();
            Clients.All.loadChallengeText(_challengeText);
        }

        private static void SetGameTime()
        {
            const double charactersPerMillesecond = AverageCharactersPerMinute / 60.0 / 1000.0;
            var gameTime = _challengeText.Length / charactersPerMillesecond;

            _gameTime = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(gameTime));
        }

        private static void InitializeGameClock()
        {
            GameClock = new TimerExtension(_gameTime.TotalMilliseconds);
            GameClock.Elapsed += GameClockOnElapsed;
        }

        private static void GameClockOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            EndGame(false);
        }

        private static void InitializeArrowTimer()
        {
            ArrowTimer = new Timer();
            ArrowTimer.Elapsed += ArrowTimerOnElapsed;
        }

        private static void ArrowTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var nextPlayer = _connectedPlayers.FirstOrDefault(x => x.Key != _activePlayerConnectionId);
            if (nextPlayer.Value == null)
                return;

            nextPlayer.Value.KeysPressedThisTurn = 0;

            SetIntervalForCurrentPlayer();
            UpdatePlayersTurn(nextPlayer.Key);
        }

        private static void SetIntervalForCurrentPlayer()
        {
            var random = new Random();
            const int lowerBoundMilleseconds = LowerBoundPlayTimeInSeconds * 1000;
            const int upperBoundMilleseconds = UpperBoundPlayTimeInSeconds * 1000;
            var interval = random.Next(lowerBoundMilleseconds, upperBoundMilleseconds);

            if (ArrowTimer != null)
            {
                ArrowTimer.Stop();
                ArrowTimer.Interval = interval;
            }
        }

        private static void UpdatePlayersTurn(string nextPlayerConnectionId)
        {
            if (!string.IsNullOrEmpty(_activePlayerConnectionId))
            {
                _gamePlayHub.Clients.Client(_activePlayerConnectionId).setPlayersTurn(false);
                _gamePlayHub.Clients.Client(_activePlayerConnectionId).flashBackground(false);
            }

            _gamePlayHub.Clients.Client(nextPlayerConnectionId).setPlayersTurn(true);
            _gamePlayHub.Clients.Client(nextPlayerConnectionId).flashBackground(true);

            _activePlayerConnectionId = nextPlayerConnectionId;

            _gamePlayHub.Clients.All.flipArrow(CurrentScreenLocation.ToString().ToLower());
        }

        private static void SetStartingPlayer()
        {
            var random = new Random();
            var startingIndex = random.Next(0, 1);
            var connectionIds = new List<string>();

            foreach (var connectionId in _connectedPlayers.Keys)
            {
                connectionIds.Add(connectionId);
            }

            var activePlayer = (connectionIds.ToArray())[startingIndex];
            UpdatePlayersTurn(activePlayer);
        }

        public void ResetGame()
        {
            Clients.All.resetGame();

            foreach (var connectedPlayer in _connectedPlayers)
            {
                Clients.Client(connectedPlayer.Key).showPlayerSelectionModal();
            }
            _connectedPlayers = new ConcurrentDictionary<string, PlayerStats>();
        }

        public void RecordKeyStroke(bool correct, int iterator)
        {
            var connectionId = Context.ConnectionId;
            var connectedPlayer = _connectedPlayers.FirstOrDefault(x => x.Key == connectionId);

            if (connectedPlayer.Value == null)
                return;

            var player = connectedPlayer.Value;

            lock (player)
            {
                if (correct && _activePlayerConnectionId == connectionId)
                {
                    player.KeysPressedThisTurn++;
                    player.Hits++;
                }
                else if (_activePlayerConnectionId == connectionId)
                {
                    player.KeysPressedThisTurn++;
                    player.Misses++;
                }
                else
                {
                    player.Misses++;
                }
            }

            if (player.KeysPressedThisTurn == KeyPressThresholdPerTurn)
            {
                ArrowTimer.Start();
            }

            Clients.All.updateStats(HitPercentage, CompletionPercentage, Score);
            Clients.AllExcept(_activePlayerConnectionId).updateIteratorPosition(iterator);

            if (CompletionPercentage.Equals(100))
            {
                EndGame(true);
            }
        }
    }
}