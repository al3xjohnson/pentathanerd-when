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
        private static IHubContext GamePlayHubContext => GlobalHost.ConnectionManager.GetHubContext<GamePlayHub>();
        private static GameHelper _gameHelper;

        private static readonly List<string> _connectedUsers = new List<string>();
        private static ConcurrentDictionary<string, PlayerStats> _connectedPlayers = new ConcurrentDictionary<string, PlayerStats>();

        private static bool _gameIsActive;
        private static string _teamName;
        private static string _challengeText;
        private static string _activePlayerConnectionId;

        private static Timer ArrowTimer { get; set; }
        private static TimerExtension GameClock { get; set; }

        public override Task OnConnected()
        {
            _connectedUsers.Add(Context.ConnectionId);
            UpdateConnectedUsersCount();

            if (_connectedPlayers.Count < 2)
                Clients.Caller.showPlayerSelectionModal();

            if (_gameIsActive)
            {
                UpdateDisplayForLateComer();
            }

            return base.OnConnected();
        }

        private static void UpdateConnectedUsersCount()
        {
            GamePlayHubContext.Clients.All.updateConnectedUsersCount(_connectedUsers.Count);
        }

        private void UpdateDisplayForLateComer()
        {
            var gameStats = _gameHelper.CalculateGameStats(_connectedPlayers);
            var currentScreen = GameHelper.GetCurrentScreenLocation(_connectedPlayers, _activePlayerConnectionId);

            Clients.Caller.updateDisplayForLateComer(GameClock.SecondsLeft, _challengeText, gameStats.TotalHits, currentScreen.ToString().ToLower(), gameStats.Accuracy, gameStats.CompletionPercentage, gameStats.Score, _teamName);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var connectionId = Context.ConnectionId;
            var player = GetPlayer(connectionId);

            if (player != null)
            {
                PlayerStats removedPlayer;
                _connectedPlayers.TryRemove(player.ConnectionId, out removedPlayer);

                if (_connectedPlayers.Count < 2)
                {
                    Clients.All.disableStartGameButton();
                    EndGame(false);
                }
            }
            _connectedUsers.Remove(connectionId);
            UpdateConnectedUsersCount();

            return base.OnDisconnected(stopCalled);
        }

        private static PlayerStats GetPlayer(string connectionId)
        {
            var connectedPlayer = _connectedPlayers.FirstOrDefault(x => x.Key == connectionId);
            return connectedPlayer.Value;
        }

        private static void EndGame(bool earlyWinner)
        {
            ResetGameClient(earlyWinner);
            ResetGameValues();
        }

        private static void ResetGameClient(bool earlyWinner)
        {
            if (earlyWinner)
            {
                GamePlayHubContext.Clients.All.endGameForEarlyWinner(GameClock.SecondsLeft);
            }
            else
            {
                GamePlayHubContext.Clients.All.endGame(_gameIsActive);
            }

            foreach (var connectedPlayer in _connectedPlayers)
            {
                GamePlayHubContext.Clients.Client(connectedPlayer.Key).enableResetGameButton();
            }
        }

        private static void ResetGameValues()
        {
            _gameIsActive = false;
            _teamName = string.Empty;
            _challengeText = string.Empty;
            _activePlayerConnectionId = string.Empty;

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
            var connectionId = Context.ConnectionId;
            _connectedPlayers.TryAdd(connectionId, new PlayerStats
            {
                ConnectionId = connectionId,
                ScreenLocation = screenLocation
            });

            SetScreenSelectedOnAllClients(location);
            Clients.Caller.showGameControls();

            if (_connectedPlayers.Count == 2)
            {
                foreach (var connectedPlayer in _connectedPlayers)
                {
                    if (GameConfiguration.TeamNameSelectionEnabled)
                    {
                        if (connectedPlayer.Value.ScreenLocation == ScreenLocation.Left)
                        {
                            Clients.Client(connectedPlayer.Key).showTeamNameSelectionModal();
                        }
                    }
                    else
                    {
                        Clients.Client(connectedPlayer.Key).enableStartGameButton();
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

        private void SetScreenSelectedOnAllClients(string location)
        {
            Clients.All.setScreenSelected(location.ToLower());
        }

        public void SetSelectedScreens()
        {
            foreach (var connectedPlayer in _connectedPlayers)
            {
                SetScreenSelectedOnAllClients(connectedPlayer.Value.ScreenLocation.ToString());
            }
        }

        public void SetTeamName(string name)
        {
            var player = GetPlayer(Context.ConnectionId);
            if (player == default(PlayerStats))
                return;

            if (string.IsNullOrEmpty(name))
                name = GameConfiguration.DefaultTeamName;

            _teamName = name;

            Clients.All.setTeamName(_teamName);

            foreach (var connectedPlayer in _connectedPlayers)
            {
                Clients.Client(connectedPlayer.Key).enableStartGameButton();
            }
        }

        public void StartGame()
        {
            if (!_connectedPlayers.Keys.Contains(Context.ConnectionId))
                return;

            if (_gameIsActive)
                return;

            _gameIsActive = true;
            LoadChallengeText();
            _gameHelper = new GameHelper(_challengeText);
            InitializeGameClock();
            InitializeArrowTimer();
            SetStartingPlayer();
            SetIntervalForCurrentPlayer();
            
            GameClock.Start();
            Clients.All.startGame(GameClock.IntervalInSeconds);
        }

        private void LoadChallengeText()
        {
            _challengeText = ChallengeTextLibrary.GetRandomChallengeText();
            Clients.All.loadChallengeText(_challengeText);
        }

        private static void InitializeGameClock()
        {
            var gameTime = _gameHelper.GetGameTime();
            GameClock = new TimerExtension(gameTime.TotalMilliseconds);
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
            var lowerBoundMilleseconds = GameConfiguration.LowerBoundTurnTimeInSeconds * 1000;
            var upperBoundMilleseconds = GameConfiguration.UpperBoundTurnTimeInSeconds * 1000;
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
                GamePlayHubContext.Clients.Client(_activePlayerConnectionId).setPlayersTurn(false);
                GamePlayHubContext.Clients.Client(_activePlayerConnectionId).flashBackground(false);
            }

            GamePlayHubContext.Clients.Client(nextPlayerConnectionId).setPlayersTurn(true);
            GamePlayHubContext.Clients.Client(nextPlayerConnectionId).flashBackground(true);

            _activePlayerConnectionId = nextPlayerConnectionId;

            var currentScreenLocation = GameHelper.GetCurrentScreenLocation(_connectedPlayers, _activePlayerConnectionId);
            GamePlayHubContext.Clients.All.flipArrow(currentScreenLocation.ToString().ToLower());
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
            var player = GetPlayer(connectionId);

            if (player == null)
                return;

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

            if (player.KeysPressedThisTurn == GameConfiguration.KeyPressThresholdPerTurn)
            {
                ArrowTimer.Start();
            }

            var gameStats = _gameHelper.CalculateGameStats(_connectedPlayers);

            Clients.All.updateStats(gameStats.Accuracy, gameStats.CompletionPercentage, gameStats.Score);
            Clients.AllExcept(_activePlayerConnectionId).updateIteratorPosition(iterator);

            if (gameStats.CompletionPercentage.Equals(100))
            {
                EndGame(true);
            }
        }

        public static void HardReset()
        {
            EndGame(false);
            _connectedPlayers = new ConcurrentDictionary<string, PlayerStats>();
            GamePlayHubContext.Clients.All.resetGame();
            GamePlayHubContext.Clients.All.showPlayerSelectionModal();
        }
    }
}