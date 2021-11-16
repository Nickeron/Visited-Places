using System.Collections.Generic;

public static class Description
{
    private static string[] starters =
        {
        "That place was $.",
        "When I visited there it was $.",
        "I will never forget that place.",
        "Ahh, yes. A way to describe this place is $.",
        "Wait till you visit there. When I had been, it was $.",
        "I sometimes dream of being there. It was $.",
        "Hmm, $. That's what this place was."
    };

    private static string[] middlers =
        {
        "You could find $ there",
        "I could see $ around me",
        "It had $",
        "There were $ there.",
        "I saw $",
        "You could see $",
        "One can find $"
    };

    private static string[] enders =
        {
        "I may get there again one day. Who knows?",
        "Maybe things are different there now. I could come back and see for myself.",
        "The memories are vivid from over there. Like I am still there.",
        "Since everything in this world changes, that place must have changed too.",
        "Let's hope climate change has not deducted a lot from there.",
        "There is no way this place is still the same. I don't know. I guess a revisit is a must.",
        "I didn't have a bad time there. That's for sure. So why not plan a trip back soon?"
    };

    private static Dictionary<Population, string[]> crowdAdjectives = new()
    {
        { Population.Crowded, new string[] { "packed", "full", "brimming", "crammed", "crowded", "overflowing" } },
        { Population.Normal, new string[] { "normal", "average", "everyday", "like many others", "not too much, not too little", "your standard place" } },
        { Population.Empty, new string[] { "empty", "like a desert", "vacant", "bare", "desolate", "uninhabited" } },
    };
}
