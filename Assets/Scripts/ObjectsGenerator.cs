using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class ObjectsGenerator : MonoBehaviour
{
    [Tooltip("Size of the area where, the generator won't be able to spawn objects, to avoid blocking the camera's view")]
    [Range(0, 3)]
    public int unspawnableAreaSize = 0;

    [Tooltip("How crowded do we want the planet to be?")]
    public Population population = Population.Normal;

    [Tooltip("The prefab that will ask for a decorative GameObject when in camera's view")]
    public GameObject spawner;

    GameObject[] _pool;
    Vector3[] _spawnablePositions;

    internal void PopulateWithSpawners(Vector3[] vertices)
    {
        _spawnablePositions = GetRandomElements(GetSpawnablePositions(vertices), (int) population).ToArray();

        foreach (Vector3 position in _spawnablePositions)
        {
            Instantiate(spawner, position, Quaternion.identity);
        }
        // Create a job to run in parallel through Unity's Multithreaded System
        //var spawnJob = new SpawnerCreationJob();

        //spawnJob.Schedule(vertices.Length, 1);

    }

    public IEnumerable<T> GetRandomElements<T>(IEnumerable<T> group, int elementsCount)
    {
        return group.OrderBy(arg => System.Guid.NewGuid()).Take(elementsCount);
    }

    IEnumerable<Vector3> GetSpawnablePositions(IEnumerable<Vector3> positions)
    {
        return positions.Where(position => IsPositionSafeToSpawn(position));
    }

    bool IsPositionSafeToSpawn(Vector3 pos)
    {        
        return !(IsInBounds(pos.x) && IsInBounds(pos.z));
    }

    bool IsInBounds(float number, int center = 10)
    {
        float upperBound = center + unspawnableAreaSize;
        float lowerBound = center - unspawnableAreaSize;
        return number < upperBound && number > lowerBound;
    }
}

public enum Population
{
    Empty = 50,
    Normal = 100,
    Crowded = 200,
}
