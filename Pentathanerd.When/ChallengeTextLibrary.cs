using System;
using System.Collections.Generic;
using System.Linq;

namespace Pentathanerd.When
{
    internal static class ChallengeTextLibrary
    {
        private static int _lastChallengeTextIndex = -1;
        private static readonly List<string> _challengeText = new List<string>
        {
            @"A modern day warrior mean, mean stride. Today's Tom Sawyer mean, mean pride. Though his mind is not for rent, don't put him down as arrogant. He reserves the quiet defense riding out the day's events. The river. What you say about his company is what you say about society. Catch the mist, catch the myth. Catch the mystery, catch the drift. The world is, the world is love and life are deep maybe as his skies are wide. Today's Tom Sawyer he gets by on you and the space he invades he gets by on you. No, his mind is not for rent to any God or government. Always hopeful yet discontent. He knows changes aren't permanent, but change is. Exit the warrior today's Tom Sawyer he gets by on you and the energy you trade. He gets right on to the friction of the day."
        };

        private static readonly List<int> _usedChallengeTextIndex = new List<int>();

        public static string GetRandomChallengeText()
        {
            if (_challengeText.Count == 1)
                return _challengeText.First();

            var random = new Random();
            var i = random.Next(0, _challengeText.Count);

            if (_challengeText.Count == _usedChallengeTextIndex.Count)
                _usedChallengeTextIndex.Clear();

            while (i == _lastChallengeTextIndex || _usedChallengeTextIndex.Contains(i))
            {
                i = random.Next(0, _challengeText.Count);
            }

            _lastChallengeTextIndex = i;

            var challengeText = _challengeText.ToArray()[i];

            _usedChallengeTextIndex.Add(i);

            return challengeText;
        }
    }
}