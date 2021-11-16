using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu]
public class DecorativeSet : ScriptableObject
{
    public string Name;
    public Gradient worldColor;
    public List<DecorativeGroup> decoratives;



    private DecorPiece[] GetRandomDecors(int variety)
    {
        DecorPiece[] newDecorSet = new DecorPiece[variety];

        for (int i = 0; i < variety; i++)
        {
            newDecorSet[i] = GetRandomDecorPiece();
        }
        return newDecorSet;
    }

    private string GetDescription(DecorativeGroup[] decorSet)
    {
        return "";
    }

    private DecorPiece GetRandomDecorPiece()
    {
        DecorativeGroup randomGroup = decoratives[Random.Range(0, decoratives.Count)];

        return new DecorPiece
        {
            type = randomGroup.type,
            prefab = randomGroup.objects[Random.Range(0, randomGroup.objects.Length)]
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
    Tree,
    Rock,
    Bush,
    Flower,
    Mushroom,
    Grass,
    Plant,
    DeadNature
}
