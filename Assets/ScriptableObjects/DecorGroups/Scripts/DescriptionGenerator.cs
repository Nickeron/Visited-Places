using System;
using System.Collections.Generic;
using System.Linq;

public static class DescriptionGenerator
{
    #region Strings
    private static string[] _starters =
    {
        "That place was $ and $. ",
        "When I visited there it was $, while $. ",
        "I will never forget that place. How $ it was as well as $. ",
        "Ahh, yes. A way to describe this place is $ and $. ",
        "Wait till you visit this $ place. When I had been, it was $. ",
        "When I dream of being in a $ place. I dream of a $ place like this. ",
        "Hmm, $. That's what this place was. And $, I may say. "
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
    #endregion Strings

    private static Random randomGenerator;

    /// <summary>
    /// Generates a description of a place, given the type of its decors, its density and terrainFlatness.
    /// </summary>
    /// <param name="rndg">For controlled results.</param>
    /// <param name="decors">The type of decorations used in the place.</param>
    /// <param name="density">How crowded this place is.</param>
    /// <param name="terrainFlatness">How bumpy or flat this place is.</param>
    /// <returns></returns>
    public static Description GetPlaceDescription(Random rndg, HashSet<string> decors, Population density, string terrainFlatness)
    {
        randomGenerator = rndg;
        return new Description
        {
            Title = GetTitle(),
            Body = GetStarter(density, terrainFlatness) + GetMiddler(decors) + GetRandom(_enders)
        };
    }

    /// <summary>
    /// Chooses a title of the place in random from 3 arrays of strings.
    /// </summary>
    /// <returns>A string for title of the place.</returns>
    private static string GetTitle()
    {
        return GetRandom(_namePrefix) + GetRandom(_nameMidFix) + GetRandom(_nameSuffix);
    }

    /// <summary>
    /// Creates a starting description sentence, based on the density of decorations and flatness of the terrain. 
    /// </summary>
    /// <param name="density">How crowded this place is.</param>
    /// <param name="terrainFlatness">How bumpy or flat this place is.</param>
    /// <returns>The first of the description</returns>
    private static string GetStarter(Population density, string terrainFlatness)
    {
        var denseAdjectives = _crowdAdjectives[density];

        string starter = GetRandom(_starters);
        return ReplaceLastWith(ReplaceLastWith(starter, "$", GetRandom(denseAdjectives)), "$", terrainFlatness);
    }

    /// <summary>
    /// Creates the middle description sentence, based on the type of decorations. 
    /// </summary>
    /// <param name="decors">The type of decorations used in the place.</param>
    /// <returns>The middle sentence of the description</returns>
    private static string GetMiddler(HashSet<string> decors)
    {
        return ReplaceLastWith(GetRandom(_middlers), "$", GetDecorDescription(decors));
    }

    /// <summary>
    /// Creates a description of the decorations used in the place. 
    /// </summary>
    /// <param name="decors">The type of decorations used in the place.</param>
    /// <returns>A part of the middle sentence</returns>
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

    /// <summary>
    /// Chooses a random string from the array provided.
    /// </summary>
    /// <param name="strings">to choose from</param>
    /// <returns>a random string</returns>
    static string GetRandom(string[] strings)
    {
        return strings[randomGenerator.Next(strings.Length)];
    }

    /// <summary>
    /// Replaces the last instance of a provided string in a sentence, with a new string.
    /// </summary>
    /// <param name="sentence">The whole string</param>
    /// <param name="changeablePart">The part to remove</param>
    /// <param name="newPart">The part to insert</param>
    /// <returns>The changed string</returns>
    private static string ReplaceLastWith(string sentence, string changeablePart = ", ", string newPart = " and ")
    {
        int pos = sentence.LastIndexOf(changeablePart);
        return RemoveLastOf(sentence, changeablePart).Insert(pos, newPart);
    }

    /// <summary>
    /// Removes the last instance of a provided string in a sentence.
    /// </summary>
    /// <param name="sentence">The whole string</param>
    /// <param name="changeablePart">The part to remove</param>
    /// <returns>The changed string</returns>
    private static string RemoveLastOf(string sentence, string changeablePart = ", ")
    {
        int pos = sentence.LastIndexOf(changeablePart);
        return pos > -1 ? sentence.Remove(pos, changeablePart.Length) : sentence;
    }
}
