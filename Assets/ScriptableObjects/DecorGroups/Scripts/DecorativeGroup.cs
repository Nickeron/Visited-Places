using UnityEngine;

[CreateAssetMenu]
public class DecorativeGroup : ScriptableObject
{
    public string type;
    public GameObject[] props;

    public DecorPiece GetPiece(System.Random rndg) => new()
    {
        type = type,
        prefab = props[rndg.Next(props.Length)]
    };
}

public struct DecorPiece
{
    public string type;
    public GameObject prefab;
}
