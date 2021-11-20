using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Decors : IEnumerable<DecorPiece>
{
    private readonly List<DecorPiece> _decorPieces = new();

    public void Add(DecorPiece prop) => _decorPieces.Add(prop);

    public IEnumerator<DecorPiece> GetEnumerator() => _decorPieces.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_decorPieces).GetEnumerator();

    public GameObject[] GetPrefabs() => _decorPieces.Select(piece => piece.prefab).ToArray();

    public HashSet<string> GetTypes() => _decorPieces.Select(piece => piece.type).ToHashSet();
}
