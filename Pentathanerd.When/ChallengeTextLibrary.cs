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
            @"Early in the morning, risin' to the street. Light me up that cigarette and I strap shoes on my feet. Got to find a reason, a reason things went wrong. Got to find a reason why my money's all gone. I got a dalmatian, and I can still get high. I can play the guitar like a mother fucking riot. Well, life is too short, so love the one you got 'cause you might get run over or you might get shot. Never start no static I just get it off my chest. Never had to battle with no bulletproof vest. Take a small example, take a tip from me, take all of your money, give it all to charity. Love is what I got it's within my reach and the Sublime style's still straight from Long Beach. It all comes back to you, you'll finally get what you deserve. Try and test that you're bound to get served.",
            @"Hey, hey. Bye, bye, bye. Bye, bye. Bye, bye. Oh, oh. I'm doin' this tonight, you're probably gonna start a fight. I know this can't be right. Hey, baby, come on. I loved you endlessly, when you weren't there for me. So now it's time to leave And make it alone. I know that I can't take no more, It ain't no lie. I wanna see you out that door baby, bye, bye, bye. Bye, bye Don't wanna be a fool for you, just another player in your game for two. You may hate me, but it ain't no lie, baby, bye, bye, bye. Bye, bye. Don't really wanna make it tough, I just wanna tell you that I had enough. It might sound crazy, but it ain't no lie, Baby, bye, bye, bye. Oh, oh. Just hit me with the truth now, girl, you're more than welcome to. So give me one good reason, Baby, come on",
            @"It was the best of times, it was the worst of times, it was the age of wisdom, it was the age of foolishness, it was the epoch of belief, it was the epoch of incredulity, it was the season of Light, it was the season of Darkness, it was the spring of hope, it was the winter of despair, we had everything before us, we had nothing before us, we were all going direct to Heaven, we were all going direct the other way— in short, the period was so far like the present period, that some of its noisiest authorities insisted on its being received, for good or for evil, in the superlative degree of comparison only. There were a king with a large jaw and a queen with a plain face, on the throne of England; there were a king with a large jaw and a queen with a fair face, on the throne of France.",
            @"If you really want to hear about it, the first thing you'll probably want to know is where I was born, an what my lousy childhood was like, and how my parents were occupied and all before they had me, and all that David Copperfield kind of crap, but I don't feel like going into it, if you want to know the truth. In the first place, that stuff bores me, and in the second place, my parents would have about two hemorrhages apiece if I told anything pretty personal about them. They're quite touchy about anything like that, especially my father. They're nice and all--I'm not saying that--but they're also touchy as hell. Besides, I'm not going to tell you my whole goddam autobiography or anything. I'll just tell you about this madman stuff that happened to me around last Christmas just before I got pretty run-down and had to come out here and take it easy. I mean that's all I told D.B. about, and he's my brother and all. He's in Hollywood.",
            @"A long time ago, in a galaxy far, far, away... A vast sea of stars serves as the backdrop for the main title. War drums echo through the heavens as a rollup slowly crawls into infinity. It is a period of civil war. Rebel spaceships, striking from a hidden base, have won their first victory against the evil Galactic Empire. During the battle, Rebel spies managed to steal secret plans to the Empire's ultimate weapon, the Death Star, an armored space station with enough power to destroy an entire planet. Pursued by the Empire's sinister agents, Princess Leia races home aboard her starship, custodian of the stolen plans that can save her people and restore freedom to the galaxy... The awesome yellow planet of Tatooine emerges from a total eclipse, her two moons glowing against the darkness. A tiny silver spacecraft, a Rebel Blockade Runner firing lasers from the back of the ship, races through space. It is pursed by a giant Imperial Stardestroyer.",
            @"Absolute quiet. SOUND bleeds in. Low level b.g. NOISES of Enterprise bridge, clicking of relays, minor electronic effects. We HEAR A FEMALE VOICE. SAAVIK'S VOICE. Captain's log. Stardate 8130.3, Starship Enterprise on training mission to Gamma Hydra. Section 14, coordinates 22/87/4. ApproachingNeutral Zone, all systems functioning. INT. ENTERPRISE BRIDGE - As the ANGLE WIDENS, we see the crew at stations; (screens and visual displays are in use): COMMANDER SULU at the helm, COMMANDER UHURA at the Comm Console, DR. BONES McCOY and SPOCK at his post. The Captain is new -- and unexpected. LT. SAAVIK is young and beautiful. She is half Vulcan and half Romulan. In appearance she is Vulcan with pointed ears, but her skin is fair and she has none of the expressionless facial immobility of a Vulcan. SULU Leaving Section Fourteen for Section Fifteen. SAAVIK Project parabolic course to avoid entering Neutral Zone. SULU Aye, Captain. UHURA (suddenly) Captain...",
            @"Jules: There’s this passage I got memorized. Ezekiel 25:17. 'The path of the righteous man is beset on all sides by the inequities of the selfish and the tyranny of evil men. Blessed is he who, in the name of charity and good will, shepherds the weak through the valley of darkness, for he is truly his brother's keeper and the finder of lost children. And I will strike down upon thee with great vengeance and furious anger those who attempt to poison and destroy my brothers. And you will know my name is The Lord when I lay my vengeance upon thee.’ I been saying that shit for years. And if you heard it, that meant your ass. I never gave much thought to what it meant. I just thought it was some cold-blooded shit to say to a motherfucker before I popped a cap in his ass. But I saw some shit this morning made me think twice. See, now I'm thinking, maybe it means you're the evil man, and I'm the righteous man, and Mr. 9 millimeter here, he's the shepherd protecting my righteous ass in the valley of darkness. Or it could mean you're the righteous man and I'm the shepherd and it's the world that's evil and selfish. I'd like that. But that shit ain't the truth. The truth is, you're the weak, and I'm the tyranny of evil men. But I'm trying, Ringo. I'm trying real hard to be the shepherd.",
            @"You certainly don't pal. 'Cause the good news is -- you're fired. The bad news is you've got, all you got, just one week to regain your jobs, starting tonight. Starting with tonights sit. Oh, have I got your attention now? Good. 'Cause we're adding a little something to this months sales contest. As you all know, first prize is a Cadillac Eldorado. Anyone want to see second prize? Second prize's a set of steak knives. Third prize is you're fired. You get the picture? You're laughing now? You got leads. Mitch and Murray paid good money. Get their names to sell them! You can't close the leads you're given, you can't close shit, you ARE shit, hit the bricks pal and beat it 'cause you are going out!!! 'The leads are weak.' Fucking leads are weak? You're weak. I've been in this business fifteen years. Moss: What's your name? Blake: FUCK YOU, that's my name!! You know why, Mister? 'Cause you drove a Hyundai to get here tonight, I drove a eighty thousand dollar BMW."
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

        private static double CalculateTime(string challengeText)
        {
            var retValue = 0;

            var wordsInChallengeText = challengeText.Split(' ').Length;


            return retValue;
        }
    }
}