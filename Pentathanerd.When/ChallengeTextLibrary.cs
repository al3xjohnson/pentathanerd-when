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
            //@"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam nec euismod mauris. Donec venenatis convallis urna, vitae finibus enim pretium pretium. Morbi diam lorem, tempor quis augue eu, semper faucibus nisl. Vivamus sed mollis magna. Etiam nisi urna, luctus sed mi ut, mollis laoreet est. Quisque in maximus quam. In malesuada arcu eget arcu semper sodales. Cras pellentesque iaculis ex at tempor. Aliquam quis nibh a odio tempus laoreet at id ante. Quisque eget ipsum nec elit convallis pretium et in justo. Sed rutrum ipsum vitae ante placerat, ac faucibus libero viverra. Nullam eget consequat purus. Sed non posuere leo. Suspendisse a hendrerit augue. Interdum et malesuada fames ac ante ipsum primis in faucibus. Nunc non mattis turpis. Vestibulum elementum nisi in ipsum tempor, quis blandit augue ultrices. Donec fermentum nibh velit, sit amet auctor lacus pulvinar sed. Vestibulum vel",
            //@"aliquet mi. Nam sit amet elementum leo. Praesent ultrices feugiat dolor quis ultrices. Quisque in arcu sit amet neque rutrum posuere quis ac justo. Ut placerat, magna vitae rutrum lacinia, quam diam cursus quam, ut auctor enim massa eu sapien. Aliquam erat volutpat. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Duis tincidunt purus nunc, ac hendrerit odio congue at. Phasellus in tristique elit. Integer mollis sem tempus sodales pharetra. Phasellus sollicitudin sodales condimentum. Aenean mi metus, pulvinar in mauris porttitor, congue efficitur eros. Donec posuere hendrerit tellus, a rhoncus purus maximus sit amet. Nunc nec risus et elit commodo gravida. In hac habitasse platea dictumst. Vivamus ut velit in ipsum mattis scelerisque. Duis enim lorem, accumsan id mollis id, ullamcorper a nibh. Duis tristique scelerisque velit, ut venenatis risus lobortis ut. Vestibulum eget eros eu",
            //@"risus porttitor sollicitudin. Nam iaculis urna non nunc eleifend, vel suscipit lectus finibus. Curabitur tempor convallis nisl, semper aliquam quam dapibus euismod. Maecenas sem velit, condimentum non auctor sed, mattis quis magna. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Ut ut erat vel massa interdum finibus. Etiam iaculis at est in sagittis. Quisque lacus tortor, pellentesque quis risus id, feugiat hendrerit augue. Pellentesque sit amet ex eu massa condimentum sagittis vitae vitae urna. Proin scelerisque nisi diam, nec egestas dolor molestie a. Sed odio ligula, rutrum non diam id, semper varius enim. In consectetur sit amet enim non iaculis. Etiam fringilla sit amet dolor vitae auctor. Curabitur congue diam vel fermentum egestas. Integer tempus dolor quis pellentesque volutpat. Maecenas ut venenatis libero. Vivamus sagittis mauris vel tortor pharetra, sed tristique purus tincidunt. Cras vitae",
            //@"commodo lacus, quis condimentum mi. Mauris varius efficitur sapien, a dapibus nibh porttitor non. Etiam elementum porttitor augue, sed elementum nisl venenatis ac. Morbi ultricies justo urna, et consectetur orci tincidunt at. Ut at felis eget felis sagittis fringilla eget non risus. Vestibulum sodales condimentum mattis. Donec in vulputate tortor. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam nec euismod mauris. Donec venenatis convallis urna, vitae finibus enim pretium pretium. Morbi diam lorem, tempor quis augue eu, semper faucibus nisl. Vivamus sed mollis magna. Etiam nisi urna, luctus sed mi ut, mollis laoreet est. Quisque in maximus quam. In malesuada arcu eget arcu semper sodales. Cras pellentesque iaculis ex at tempor. Aliquam quis nibh a odio tempus laoreet at id ante. Quisque eget ipsum nec elit convallis pretium et in justo. Sed rutrum ipsum vitae ante placerat, ac faucibus",
            //@"libero viverra. Nullam eget consequat purus. Sed non posuere leo. Suspendisse a hendrerit augue. Interdum et malesuada fames ac ante ipsum primis in faucibus. Nunc non mattis turpis. Vestibulum elementum nisi in ipsum tempor, quis blandit augue ultrices. Donec fermentum nibh velit, sit amet auctor lacus pulvinar sed. Vestibulum vel aliquet mi. Nam sit amet elementum leo. Praesent ultrices feugiat dolor quis ultrices. Quisque in arcu sit amet neque rutrum posuere quis ac justo. Ut placerat, magna vitae rutrum lacinia, quam diam cursus quam, ut auctor enim massa eu sapien. Aliquam erat volutpat. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Duis tincidunt purus nunc, ac hendrerit odio congue at. Phasellus in tristique elit. Integer mollis sem tempus sodales pharetra. Phasellus sollicitudin sodales condimentum. Aenean mi metus, pulvinar in mauris porttitor, congue efficitur eros. Donec posuere",
            //@"hendrerit tellus, a rhoncus purus maximus sit amet. Nunc nec risus et elit commodo gravida. In hac habitasse platea dictumst. Vivamus ut velit in ipsum mattis scelerisque. Duis enim lorem, accumsan id mollis id, ullamcorper a nibh. Duis tristique scelerisque velit, ut venenatis risus lobortis ut. Vestibulum eget eros eu risus porttitor sollicitudin. Nam iaculis urna non nunc eleifend, vel suscipit lectus finibus. Curabitur tempor convallis nisl, semper aliquam quam dapibus euismod. Maecenas sem velit, condimentum non auctor sed, mattis quis magna. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Ut ut erat vel massa interdum finibus. Etiam iaculis at est in sagittis. Quisque lacus tortor, pellentesque quis risus id, feugiat hendrerit augue. Pellentesque sit amet ex eu massa condimentum sagittis vitae vitae urna. Proin scelerisque nisi diam, nec egestas dolor molestie a. Sed odio"

            //@"Expenses as material breeding insisted building to in. Continual so distrusts pronounce by unwilling listening. Thing do taste on we manor. Him had wound use found hoped. Of distrusts immediate enjoyment curiosity do. Marianne numerous saw thoughts the humored. Behind sooner dining so window excuse he summer. Breakfast met certainty and fulfilled propriety led. Waited get either are wooded little her. Contrasted unreserved as Mr. particular collecting it everything as indulgence. Seems ask meant merry could put. Age old begin had boy noisy table front whole given. Its had resolving otherwise she contented therefore. Afford relied warmth out sir hearts sister use garden. Men day warmth formed admire former simple. Humanity declared vicinity continue supplied no an. He hastened am no property exercise of. Dissimilar comparison no terminated devonshire no literature on. Say most yet head room such just easy.",
            //@"Whether article spirits new her covered hastily sitting her. Money witty books nor son add. Chicken age had evening believe but proceed pretend Mrs. At missed advice my it no sister. Miss told ham dull knew see she spot near can. Spirit her entire her called. Meant balls it if up doubt small purse. Required his you put the outlived answered position. A pleasure exertion if believed provided to. All led out world these music while asked. Paid mind even sons does he door no. Attended overcame repeated it is perceive Marianne in. In am think on style child of. Servants moreover in sensible he it ye possible. Imagine was you removal raising gravity. Insatiable understood or expression dissimilar so sufficient. Its party every heard and event gay. Advice he indeed things adieus in number so uneasy. To many four fact in he fail.",
            //@"My hung it quit next do of. It fifteen charmed by private savings it Mr. Favorable cultivated alteration entreaties yet met sympathize. Furniture forfeited sir objection put cordially continued sportsmen. Apartments simplicity or understood do it we. Song such eyes had and off. Removed winding ask explain delight out few behaved lasting. Letters old hastily ham sending not sex chamber because present. Oh is indeed twenty entire figure. Occasional diminution announcing new now literature terminated. Really regard excuse off ten pulled. Lady am room head so lady four or eyes an. He do of consulted sometimes concluded Mr. A household behavior if pretended. Guest it he tears aware as. Make my no cold of need. He been past in by my hard. Warmly thrown oh he common future. Otherwise concealed favorite frankness on be at dashwoods defective at. Sympathize interested simplicity at do projecting increasing terminated.",
            //@"A modern day warrior mean, mean stride. Today's Tom Sawyer mean, mean pride. Though his mind is not for rent, don't put him down as arrogant. He reserves the quiet defense riding out the day's events. The river. What you say about his company is what you say about society. Catch the mist, catch the myth. Catch the mystery, catch the drift. The world is, the world is love and life are deep maybe as his skies are wide. Today's Tom Sawyer he gets by on you and the space he invades he gets by on you. No, his mind is not for rent to any God or government. Always hopeful yet discontent. He knows changes aren't permanent, but change is. Exit the warrior today's Tom Sawyer he gets by on you and the energy you trade. He gets right on to the friction of the day."
            @"12345678"
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