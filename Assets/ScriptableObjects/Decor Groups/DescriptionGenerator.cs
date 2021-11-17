using System;
using System.Collections.Generic;
using System.Linq;

public static class DescriptionGenerator
{
    private static string[] _starters =
        {
        "That place was $.",
        "When I visited there it was $.",
        "I will never forget that place.",
        "Ahh, yes. A way to describe this place is $.",
        "Wait till you visit there. When I had been, it was $.",
        "I sometimes dream of being there. It was $.",
        "Hmm, $. That's what this place was."
    };

    private static string[] _middlers =
        {
        "You could find $ there",
        "I could see $ around me",
        "It had $",
        "There were $ there.",
        "I saw $",
        "You could see $",
        "One can find $"
    };

    private static string[] _enders =
        {
        "I may get there again one day. Who knows?",
        "Maybe things are different there now. I could come back and see for myself.",
        "The memories are vivid from over there. Like I am still there.",
        "Since everything in this world changes, that place must have changed too.",
        "Let's hope climate change has not deducted a lot from there.",
        "There is no way this place is still the same. I don't know. I guess a revisit is a must.",
        "I didn't have a bad time there. That's for sure. So why not plan a trip back soon?"
    };

    private static Dictionary<Population, string[]> _crowdAdjectives = new()
    {
        { Population.Crowded, new string[] { "packed", "full", "brimming", "crammed", "crowded", "overflowing" } },
        { Population.Normal, new string[] { "normal", "average", "everyday", "like many others", "not too much, not too little", "your standard place" } },
        { Population.Empty, new string[] { "empty", "like a desert", "vacant", "bare", "desolate", "uninhabited" } },
    };

    public static string GetWorldDescription(Random rndg, DecorPiece[] decors, Population density)
    {        
        return GetRandomStarter(density, rndg) + GetRandomMiddler(decors, rndg) + _enders[rndg.Next(_enders.Length)];
    }

    private static string GetRandomStarter(Population density, Random rndg)
    {
        var denseAdjectives = _crowdAdjectives[density];
        string starter = _starters[rndg.Next(_starters.Length)];
        return ReplaceLastWith(starter, "&", denseAdjectives[rndg.Next(denseAdjectives.Length)]);
    }

    private static string GetRandomMiddler(DecorPiece[] decors, Random rndg)
    {
        string middler = _middlers[rndg.Next(_middlers.Length)];
        return ReplaceLastWith(middler, "&", GetDecorDescription(decors.Select(dec => dec.type).ToHashSet()));
    }

    private static string GetDecorDescription(HashSet<DecorativeType> decorTypesUsed)
    {
        switch (decorTypesUsed.Count)
        {
            case 0: return "absolutely nothing";
            case 1: return "just " + decorTypesUsed.First();
            default:
                string items = "";

                foreach (var type in decorTypesUsed)
                {
                    items += type.ToString() + ", ";
                }

                return ReplaceLastWith(RemoveLastOf(items));
        }
    }

    private static string ReplaceLastWith(string sentence, string separator = ", ", string replace = "and ")
    {
        int pos = sentence.LastIndexOf(separator);
        return RemoveLastOf(sentence, separator).Insert(pos, replace);
    }

    private static string RemoveLastOf(string sentence, string separator = ", ")
    {
        int pos = sentence.LastIndexOf(separator);
        return sentence.Remove(pos, separator.Length);
    }
}
