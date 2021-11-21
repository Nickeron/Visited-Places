using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Pool;

public class PropsGenerator : MonoBehaviour
{
    public System.Action ResetSpawnPoints;

    #region Private Fields
    [Tooltip("Size of the mesh's area, where the generator won't spawn objects, to avoid blocking the camera's view")]
    [SerializeField]
    [Range(1, 3)]
    private int unspawnableAreaSize = 1;

    [Tooltip("Prefab that will ask for a decorative GameObject when in camera's view")]
    [SerializeField]
    private GameObject spawnPointPrefab = null;

    [Tooltip("Transform to act as a parent for all spawned objects of this world")]
    [SerializeField]
    private Transform spawnParent = null;

    private ObjectPool<GameObject>[] _pools;
    private SpawnPoint[] _spawnPoints;

    private System.Random _rndg;
    #endregion Private Fields  

    /// <summary>
    /// Populates the mesh with GameObjects.
    /// Selects random vertices from the mesh and instantiates colliders at those positions.
    /// Uses coroutines to start and stop the process of creation without waiting 
    /// for the previous call to complete.
    /// </summary>
    /// <param name="seed">For the random generator to spit constant results.</param>
    /// <param name="vertices">Vertices of the generated mesh to be used as spawning positions.</param>
    /// <param name="decorPrefabs">GameObjects-Props to populate the place with.</param>
    /// <param name="frustumPlanes">Camera's Frustum Planes, to activate only the viewable spawining points.</param>
    /// <param name="density">Controls the amount of props this place will have.</param>
    internal void PopulateMeshWithProps(int seed, Vector3[] vertices, GameObject[] decorPrefabs, Plane[] frustumPlanes, Population density)
    {
        StopAllCoroutines();

        // The first time, instantiate the maximum number of spawners we will need
        if (_spawnPoints == null) CreateSpawnPoints();
        
        ResetSpawnPoints?.Invoke();

        _rndg = new System.Random(seed);

        // If we already have populated ObjectPools with prefabs let's clear them, before repopulating
        if (_pools != null) ClearPools();

        var randomElements = GetElements(GetSpawnablePositions(vertices), (int)density).ToArray();

        StartCoroutine(InitializePoolsRoutine(decorPrefabs, randomElements, frustumPlanes));
    }
    
    /// <summary>
    /// Creates the maximum amount of SpawnPoints that the place will use, and subscribes them to the ResetSpawnPoints action.
    /// So that they reset as Observers instead of using a for, for every one of them.
    /// </summary>
    private void CreateSpawnPoints()
    {
        _spawnPoints = new SpawnPoint[(int)Population.Crowded];

        for (int i = 0; i < (int) Population.Crowded; i++)
        {
            // Instantiate every spawn point with a random pool of objects to pull from, for variety
            _spawnPoints[i] = Instantiate(spawnPointPrefab, spawnParent, false).GetComponent<SpawnPoint>();
            ResetSpawnPoints += _spawnPoints[i].ResetProperties;
        }
    }

    /// <summary>
    /// A co-routine that initializes an ObjectPool for every given prop.
    /// </summary>
    /// <param name="decorPrefabs">Decoration Prefabs</param>
    /// <param name="spawnPositions">Passed on to PopulateSpawnPointsRoutine</param>
    /// <param name="frustumPlanes">Passed on to PopulateSpawnPointsRoutine</param>
    /// <returns></returns>
    private IEnumerator InitializePoolsRoutine(GameObject[] decorPrefabs, Vector3[] spawnPositions, Plane[] frustumPlanes)
    {
        _pools = new ObjectPool<GameObject>[decorPrefabs.Length];

        for (int i = 0; i < decorPrefabs.Length; i++)
        {
            _pools[i] = InitializePool(decorPrefabs[i]);
        }

        yield return PopulateSpawnPointsRoutine(spawnPositions, frustumPlanes);
    }

    /// <summary>
    /// Create an ObjectPool with the given GameObject.
    /// The Pool 
    /// OnCreation - Instantiates a new GameObject, under the spawnParent Transform.
    /// OnGet - Activates the decorative GameObject.
    /// OnRelease - Deactivates the decorative GameObject.
    /// OnDestroy - Destroys the decorative GameObject.
    /// </summary>
    /// <param name="prop">Decoration Prefab</param>
    /// <returns>new ObjectPool</returns>
    private ObjectPool<GameObject> InitializePool(GameObject prop)
    {
        return new ObjectPool<GameObject>(
                () => Instantiate(prop, spawnParent),
                decorative => decorative.SetActive(true),
                decorative => decorative.SetActive(false),
                decorative => Destroy(decorative));
    }

    private IEnumerator PopulateSpawnPointsRoutine(Vector3[] spawnPositions, Plane[] frustumPlanes)
    {
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            _spawnPoints[i].SetPositionAndPool(spawnPositions[i], GetPool(), frustumPlanes);
        }

        yield return null;
    }

    #region Helper Methods
    private void ClearPools()
    {
        foreach (var pool in _pools) pool.Dispose();
    }

    /// <summary>
    /// Randomly selects one of the available ObjectPools.
    /// </summary>
    /// <returns>An ObjectPool of props.</returns>
    private ObjectPool<GameObject> GetPool() => _pools[_rndg.Next(_pools.Count())];

    /// <summary>
    /// From a group of elements select randomly an amount of them and return them
    /// </summary>
    /// <typeparam name="T">Type of the IEnumerable: List, Array etc.</typeparam>
    /// <param name="group">Group of total elements</param>
    /// <param name="elementsCount">Amount of elements to select</param>
    /// <returns>IEnumerable group of randomly selected elements</returns>
    private IEnumerable<T> GetElements<T>(IEnumerable<T> group, int elementsCount) => group.OrderBy(arg => System.Guid.NewGuid()).Take(elementsCount);

    #region Spawn Safety
    /// <summary>
    /// From a group of vectors, select those that can be used to spawn props,
    /// without blocking the camera's view.
    /// </summary>
    /// <param name="positions">The group of vectors</param>
    /// <returns>IEnumerable group of vectors that are safe to spawn props on.</returns>
    private IEnumerable<Vector3> GetSpawnablePositions(IEnumerable<Vector3> positions) => positions.Where(position => IsPositionSafeToSpawn(position));

    private bool IsPositionSafeToSpawn(Vector3 pos) => !(IsInBounds(pos.x) && IsInBounds(pos.z));

    /// <summary>
    /// Checks if a coordinate belongs in the unspawnable area of coordinates.
    /// So if we spawn a prop there it would block the camera's view.
    /// </summary>
    /// <param name="coordinate">The coordinate to check. (x, y)</param>
    /// <param name="center">The position of the camera in the mesh system. On a 20X20 mesh it would be at (10, 10)</param>
    /// <returns>True if the coordinate is in the unspawnable area.</returns>
    private bool IsInBounds(float coordinate, int center = 10) => coordinate < center + unspawnableAreaSize && coordinate > center - unspawnableAreaSize;
    #endregion Spawn Safety
    #endregion Helper Methods
}
