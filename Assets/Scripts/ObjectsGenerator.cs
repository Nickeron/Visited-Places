using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Pool;

public class ObjectsGenerator : MonoBehaviour
{
    [Tooltip("Size of the area where, the generator won't be able to spawn objects, to avoid blocking the camera's view")]
    [Range(0, 3)]
    public int unspawnableAreaSize = 0;

    [Tooltip("How crowded do we want the planet to be?")]
    public Population population = Population.Normal;

    [Tooltip("The prefab that will ask for a decorative GameObject when in camera's view")]
    public GameObject spawner;

    [Tooltip("A transform to act as a parent for all spawned objects of this world")]
    public Transform spawnParent;

    [Tooltip("All the types of decorative objects, this world will use")]
    public GameObject[] prefabs;

    ObjectPool<GameObject>[] _pools;
    Vector3[] _spawnablePositions;

    private void Awake()
    {
        InitializePools();
    }

    void InitializePools()
    {
        _pools = new ObjectPool<GameObject>[prefabs.Length];
        Debug.Log(_pools.Length);

        for (int poolIndex = 0; poolIndex < prefabs.Length; poolIndex++)
        {
            int index = poolIndex;
            _pools[poolIndex] = new ObjectPool<GameObject>(() =>
            {                
                return Instantiate(prefabs[index], spawnParent);
            }, OnGetDecorative, OnReleaseDecorative);
        }
    }

    private void OnGetDecorative(GameObject decorative)
    {
        decorative.SetActive(true);
        Debug.Log("Sending Decorative from pool");
    }
    
    private void OnReleaseDecorative(GameObject decorative)
    {
        decorative.SetActive(false);
        Debug.Log("Releasing Decorative in pool");
    }

    internal void PopulateWithSpawners(Vector3[] vertices)
    {
        _spawnablePositions = GetRandomElements(GetSpawnablePositions(vertices), (int) population).ToArray();

        foreach (Vector3 position in _spawnablePositions)
        {
            // Instantiate every spawner with a random pool of objects to pull from, so we have variety
            Instantiate(spawner, position, Quaternion.identity, spawnParent)
                .GetComponent<SpawnPoint>()
                .decorativesPool = GetRandomPool();
        }

        // TODO: Create a job to run in parallel through Unity's Multithreaded System
        //var spawnJob = new SpawnerCreationJob();

        //spawnJob.Schedule(vertices.Length, 1);
    }

    public ObjectPool<GameObject> GetRandomPool()
    {
        return _pools[UnityEngine.Random.Range(0, _pools.Count())];
    }

    public IEnumerable<T> GetRandomElements<T>(IEnumerable<T> group, int elementsCount)
    {
        return group.OrderBy(arg => System.Guid.NewGuid()).Take(elementsCount);
    }

    #region Spawn Safety
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
    #endregion Spawn Safety
}

public enum Population
{
    Empty = 50,
    Normal = 100,
    Crowded = 200,
}
