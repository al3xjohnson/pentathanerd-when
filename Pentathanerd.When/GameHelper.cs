using System;
using System.Collections.Concurrent;

namespace Pentathanerd.When
{
    internal class GameHelper
    {
        private readonly string _challengeText;

        public GameHelper(string challengeText)
        {
            _challengeText = challengeText;
        }

        public GameStats CalculateGameStats(ConcurrentDictionary<string, PlayerStats> connectedPlayers)
        {
            var retValue = new GameStats();
            var totalHits = 0;
            var totalMisses = 0;

            foreach (var connectedPlayer in connectedPlayers)
            {
                totalHits += connectedPlayer.Value.Hits;
                totalMisses += connectedPlayer.Value.Misses;
            }
            retValue.TotalHits = totalHits;

            var accuracy = GetAccuracy(totalHits, totalMisses);
            retValue.Accuracy = accuracy;

            var completionPercentage = GetCompletionPercentage(totalHits);
            retValue.CompletionPercentage = completionPercentage;

            var score = GetScore(accuracy, completionPercentage);
            retValue.Score = score;

            return retValue;
        }

        private static double GetAccuracy(int totalHits, int totalMisses)
        {
            var retValue = 0.0;
            double totalKeyPresses = totalHits + totalMisses;
            if (totalKeyPresses > 0)
            {
                var hitFraction = totalHits / totalKeyPresses;
                var hitPercentage = hitFraction * 100;
                retValue = Math.Round(hitPercentage, 2);
            }
            return retValue;
        }

        private double GetCompletionPercentage(double totalHits)
        {
            var retValue = 0.0;
            if (_challengeText.Length > 0)
            {
                var completionFraction = totalHits / _challengeText.Length;
                double completionPercentage = completionFraction * 100;
                retValue = Math.Round(completionPercentage, 2);
            }
            return retValue;
        }

        private static double GetScore(double accuracy, double completionPercentage)
        {
            var totalScorePercentage = (accuracy + completionPercentage) / 2;
            var score = (totalScorePercentage / 100) * GameConfiguration.AvailableScoreBeforeBonus;
            return Math.Round(score, 0);
        }

        public static ScreenLocation GetCurrentScreenLocation(ConcurrentDictionary<string, PlayerStats> connectedPlayers, string activePlayerConnectionId)
        {
            var retValue = ScreenLocation.Left;
            foreach (var connectedPlayer in connectedPlayers)
            {
                if (connectedPlayer.Key == activePlayerConnectionId)
                {
                    retValue = connectedPlayer.Value.ScreenLocation;
                }
            }
            return retValue;
        }

        public TimeSpan GetGameTime()
        {
            var charactersPerMillesecond = GameConfiguration.AverageCharactersPerMinute / 60.0 / 1000.0;
            var gameTime = _challengeText.Length / charactersPerMillesecond;

            return new TimeSpan(0, 0, 0, 0, Convert.ToInt32(gameTime));
        }
    }
}