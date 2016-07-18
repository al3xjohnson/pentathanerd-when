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
        private const double GameTimeInMinues = 3.5;
        private const int AvailableScoreBeforeBonus = 1200;
        private const int LowerBoundPlayTimeInSeconds = 3;
        private const int UpperBoundPlayTimeInSeconds = 10;
        private const int KeyPressThresholdPerTurn = 5;
        #endregion

        #region Statics
        private static IHubContext _gamePlayHub;
        private static ConcurrentDictionary<string, PlayerStats> _connectedPlayers = new ConcurrentDictionary<string, PlayerStats>();
        private static string _activePlayerConnectionId;

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
                var totalHits = TotalHits;
                var totalKeyPresses = totalHits + TotalMisses;
                var hitFraction = totalHits / totalKeyPresses;
                var hitPercentage = hitFraction * 100;
                var roundedHitPercentage = Math.Round(hitPercentage, 2);

                return roundedHitPercentage > 0 ? roundedHitPercentage : 0;
            }
        }

        private static double CompletionPercentage
        {
            get
            {
                var completionFraction = TotalHits / _challengeText.Length;
                var completionPercent = completionFraction * 100;

                return Math.Round(completionPercent, 2);
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

        private static TimeSpan GameTime
        {
            get
            {
                if (_gameTime == default(TimeSpan))
                {
                    var minutes = Convert.ToInt32(Math.Floor(GameTimeInMinues));
                    var seconds = Convert.ToInt32((GameTimeInMinues % minutes) * 60);

                    _gameTime = new TimeSpan(0, minutes, seconds);
                }
                return _gameTime;
            }
        }
        #endregion

        #region Hub overrides
        public override Task OnConnected()
        {
            InitializeGamePlayHubContext();

            if (_connectedPlayers.Count < 2)
                Clients.Caller.showPlayerSelectionModal();

            if (_gameIsActive)
            {
                Clients.Caller.updateDisplayForLateComer(GameClock.SecondsLeft, _challengeText, TotalHits, CurrentScreenLocation.ToString().ToLower(), HitPercentage, CompletionPercentage, Score);
            }

            return base.OnConnected();
        }

        private static void InitializeGamePlayHubContext()
        {
            _gamePlayHub = GlobalHost.ConnectionManager.GetHubContext<GamePlayHub>();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var connectionId = Context.ConnectionId;
            var connectedPlayer = _connectedPlayers.FirstOrDefault(x => x.Key == connectionId);

            if (connectedPlayer.Value == null)
                return base.OnDisconnected(stopCalled);

            PlayerStats player;
            if (connectedPlayer.Key != null)
                _connectedPlayers.TryRemove(connectedPlayer.Key, out player);

            if (_connectedPlayers.Count < 2)
            {
                EndGame(false);
            }

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
                    Clients.Client(connectedPlayer.Key).enableStartGameButton();
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

        public void StartGame()
        {
            if (!_connectedPlayers.Keys.Contains(Context.ConnectionId))
                return;

            if (_gameIsActive)
                return;

            _gameIsActive = true;
            InitializeGameClock();
            InitializeArrowTimer();
            SetStartingPlayer();
            LoadChallengeText();
            SetIntervalForCurrentPlayer();
            
            GameClock.Start();
            Clients.All.startGame(GameTime.TotalSeconds);
        }

        private static void InitializeGameClock()
        {
            GameClock = new TimerExtension(GameTime.TotalMilliseconds);
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

        private void LoadChallengeText()
        {
            _challengeText = ChallengeTextLibrary.GetRandomChallengeText();
            Clients.All.loadChallengeText(_challengeText);
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

            lock (connectedPlayer.Value)
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