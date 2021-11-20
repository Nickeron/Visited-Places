﻿using System;
using System.Collections.Generic;
using System.Linq;

public static class DescriptionGenerator
{
    private static string[] _starters =
        {
        "That place was $. ",
        "When I visited there it was $. ",
        "I will never forget that place. How $ it was. ",
        "Ahh, yes. A way to describe this place is $. ",
        "Wait till you visit there. When I had been, it was $. ",
        "I sometimes dream of being there. It was a $ place. ",
        "Hmm, $. That's what this place was. "
    };

    private static string[] _middlers =
        {
        "You could find $ there. ",
        "I could see $ around me. ",
        "It had $. ",
        "There were $ there. ",
        "I saw $. ",
        "You could see $. ",
        "One can find $. "
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

    private static string[] _namePrefix =
    {
        "En", "Ned", "Ic", "Hil", "Den", "Bor", "Sil", "It", "Am", "Eind", "Ut", "Cre", "Sw", "U", "Ru"
    };

    private static string[] _nameMidFix =
    {
        "g", "er", "e", "ver", "ti", "al", "ster", "hov", "ni", "zo", "te", "ed", "kr", "ma"
    };

    private static string[] _nameSuffix =
    {
        "land", "ce", "mark", "sum", "berg", "ia", "dam", "en", "recht", "ain", "sia", "nia"
    };

    private static Dictionary<Population, string[]> _crowdAdjectives = new()
    {
        { Population.Crowded, new string[] { "packed", "full", "brimming", "crammed", "crowded", "overflowing" } },
        { Population.Normal, new string[] { "normal", "average", "everyday", "usual", "not too much, not too little", "standard" } },
        { Population.Empty, new string[] { "empty", "deserted", "vacant", "bare", "desolate", "uninhabited" } },
    };

    public static Description GetWorldDescription(Random rndg, HashSet<string> decors, Population density)
    {
        return new Description
        {
            Title = GetRandomTitle(rndg),
            Body = GetRandomStarter(density, rndg) + GetRandomMiddler(decors, rndg) + GetRandom(_enders, rndg)
        };
    }

    private static string GetRandomTitle(Random rndg)
    {
        return GetRandom(_namePrefix, rndg) + GetRandom(_nameMidFix, rndg) + GetRandom(_nameSuffix, rndg);
    }

    private static string GetRandomStarter(Population density, Random rndg)
    {
        var denseAdjectives = _crowdAdjectives[density];

        return ReplaceLastWith(GetRandom(_starters, rndg), "$", GetRandom(denseAdjectives, rndg));
    }

    private static string GetRandomMiddler(HashSet<string> decors, Random rndg)
    {
        return ReplaceLastWith(GetRandom(_middlers, rndg), "$", GetDecorDescription(decors));
    }

    private static string GetDecorDescription(HashSet<string> decorTypesUsed)
    {
        switch (decorTypesUsed.Count)
        {
            case 0: return "absolutely nothing";
            case 1: return "just " + decorTypesUsed.First();
            default:
                string items = "";

                foreach (string type in decorTypesUsed)
                {
                    items += type + ", ";
                }

                return ReplaceLastWith(RemoveLastOf(items));
        }
    }

    static string GetRandom(string[] strings, Random randomGenerator)
    {
        return strings[randomGenerator.Next(strings.Length)];
    }

    private static string ReplaceLastWith(string sentence, string separator = ", ", string replace = " and ")
    {
        int pos = sentence.LastIndexOf(separator);
        return RemoveLastOf(sentence, separator).Insert(pos, replace);
    }

    private static string RemoveLastOf(string sentence, string separator = ", ")
    {
        int pos = sentence.LastIndexOf(separator);
        return pos > -1 ? sentence.Remove(pos, separator.Length): sentence;
    }
}