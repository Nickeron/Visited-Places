using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu]
public class DecorativeSet : ScriptableObject
{
    public string Name;
    public Gradient worldColor;
    public List<DecorativeGroup> decoratives;

    public DecorPiece[] GetRandomDecors(int arraySize, System.Random rndg)
    {
        DecorPiece[] newDecorSet = new DecorPiece[arraySize];

        for (int i = 0; i < arraySize; i++)
        {
            newDecorSet[i] = GetRandomDecorPiece(rndg);
        }
        return newDecorSet;
    }

    private DecorPiece GetRandomDecorPiece(System.Random rndg)
    {
        DecorativeGroup randomGroup = decoratives[rndg.Next(decoratives.Count)];

        return new DecorPiece
        {
            type = randomGroup.type,
            prefab = randomGroup.objects[rndg.Next(randomGroup.objects.Length)]
        };
    }
}

[System.Serializable]
public struct DecorativeGroup
{
    public DecorativeType type;
    public GameObject[] objects;
}

public struct DecorPiece
{
    public DecorativeType type;
    public GameObject prefab;
}

public enum DecorativeType
{
    trees,
    rocks,
    bushes,
    flowers,
    mushrooms,
    grass,
    plants,
    logs
}
