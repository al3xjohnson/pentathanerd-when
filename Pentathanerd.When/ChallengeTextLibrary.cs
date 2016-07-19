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
            @"A modern day warrior mean, mean stride. Today's Tom Sawyer mean, mean pride. Though his mind is not for rent, don't put him down as arrogant. He reserves the quiet defense riding out the day's events. The river. What you say about his company is what you say about society. Catch the mist, catch the myth. Catch the mystery, catch the drift. The world is, the world is love and life are deep maybe as his skies are wide. Today's Tom Sawyer he gets by on you and the space he invades he gets by on you. No, his mind is not for rent to any God or government. Always hopeful yet discontent. He knows changes aren't permanent, but change is. Exit the warrior today's Tom Sawyer he gets by on you and the energy you trade. He gets right on to the friction of the day.",
            @"She was more like a beauty queen from a movie scene. I said don't mind, but what do you mean? I am the one who will dance on the floor in the round. She said I am the one, who will dance on the floor in the round. She told me her name was Billie Jean, as she caused a scene. Then every head turned with eyes that dreamed of being the one who will dance on the floor in the round. People always told me be careful of what you do and don't go around breaking young girls' hearts. And mother always told me be careful of who you love and be careful of what you do 'cause the lie becomes the truth. Billie Jean is not my lover she's just a girl who claims that I am the one, but the kid is not my son. She says I am the one, but the kid is not my son.",
            @"Early in the morning, risin' to the street. Light me up that cigarette and I strap shoes on my feet. Got to find a reason, a reason things went wrong. Got to find a reason why my money's all gone. I got a dalmatian, and I can still get high. I can play the guitar like a mother fucking riot. Well, life is too short, so love the one you got 'cause you might get run over or you might get shot. Never start no static I just get it off my chest. Never had to battle with no bulletproof vest. Take a small example, take a tip from me, take all of your money, give it all to charity. Love is what I got it's within my reach and the Sublime style's still straight from Long Beach. It all comes back to you, you'll finally get what you deserve. Try and test that you're bound to get served."
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