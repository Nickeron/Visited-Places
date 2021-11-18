using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Pool;

public class PropsGenerator : MonoBehaviour
{
    //[Tooltip("How crowded do we want the planet to be?")]
    //public Population population = Population.Normal;

    //[Tooltip("All the types of decorative objects, this world will use")]
    //public GameObject[] prefabs;

    // Size of the mesh's area, where the generator won't spawn objects, to avoid blocking the camera's view
    int unspawnableAreaSize = 1;

    [Tooltip("Prefab that will ask for a decorative GameObject when in camera's view")]
    [SerializeField]
    private GameObject spawnPointPrefab = null;

    [Tooltip("Transform to act as a parent for all spawned objects of this world")]
    [SerializeField]
    private Transform spawnParent = null;

    ObjectPool<GameObject>[] _pools;
    Vector3[] _spawnablePositions;

    System.Random _rndg;    

    internal void PopulateMeshWithProps(int seed, Vector3[] vertices, GameObject[] decorPrefabs, Population density)
    {
        StopAllCoroutines();        

        _rndg = new System.Random(seed);
        _spawnablePositions = GetRandomElements(GetSpawnablePositions(vertices), (int) density).ToArray();

        StartCoroutine(InitializePoolsRoutine(decorPrefabs));

        // TODO: Create a job to run in parallel through Unity's Multithreaded System
        //var spawnJob = new SpawnerCreationJob();

        //spawnJob.Schedule(vertices.Length, 1);
    }

    IEnumerator InitializePoolsRoutine(GameObject[] decorPrefabs)
    {
        _pools = new ObjectPool<GameObject>[decorPrefabs.Length];
        Debug.Log(_pools.Length);

        for (int poolIndex = 0; poolIndex < decorPrefabs.Length; poolIndex++)
        {
            int index = poolIndex;
            _pools[poolIndex] = new ObjectPool<GameObject>(
                () => Instantiate(decorPrefabs[index], spawnParent),
                decorative => decorative.SetActive(true),
                decorative => decorative.SetActive(false));
        }
        yield return PopulateSpawnPointsRoutine();
    }

    IEnumerator PopulateSpawnPointsRoutine()
    {
        foreach (Vector3 position in _spawnablePositions)
        {
            // Instantiate every spawner with a random pool of objects to pull from, so we have variety
            Instantiate(spawnPointPrefab, position, Quaternion.identity, spawnParent)
                .GetComponent<SpawnPoint>()
                .decorativesPool = GetRandomPool();            
        }
        yield return null;
    }

    public ObjectPool<GameObject> GetRandomPool()
    {
        return _pools[_rndg.Next(_pools.Count())];
    }

    public IEnumerable<T> GetRandomElements<T>(IEnumerable<T> group, int elementsCount)
    {
        return group.OrderBy(arg => System.Guid.NewGuid()).Take(elementsCount);
    }

    #region Spawn Safety
    IEnumerable<Vector3> GetSpawnablePositions(IEnumerable<Vector3> positions)
    {
        unspawnableAreaSize = _rndg.Next(3);
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
