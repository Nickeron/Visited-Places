using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu]
public class DecorativeSet : ScriptableObject
{
    public string Name;
    public Gradient worldColor;
    public List<DecorativeGroup> decoratives;

    public Decors GetRandomDecors(System.Random rndg, int size = 5)
    {
        Decors newDecorSet = new();

        for (int i = 0; i < size; i++)
        {
            newDecorSet.Add(GetGroup(rndg).GetPiece(rndg));
        }
        return newDecorSet;
    }

    private DecorativeGroup GetGroup(System.Random rndg) => decoratives[rndg.Next(decoratives.Count)];
}
