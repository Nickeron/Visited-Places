using System;

using UnityEngine;
using UnityEngine.Pool;

public class WorldsManager : MonoBehaviour
{
    [Tooltip("A prefab to initialize for every world we want to create")]
    public GameObject WorldGeneratorPrefab;

    ObjectPool<GameObject> worlds;

    int index = 0;
    int starter = new System.Random().Next();
    int step = new System.Random().Next();

    private void Start()
    {
        index = starter;
    }

    void InitializePool()
    {
        worlds = new ObjectPool<GameObject>(CreateWorld, OnGetWorld, OnReleaseWorld);
    }    

    GameObject CreateWorld()
    {
        GameObject newWorld = Instantiate(WorldGeneratorPrefab, transform);
        // We name each world gameobject with its index, so that it can be used all around
        newWorld.name = index.ToString();

        newWorld.GetComponent<MeshGenerator>().InitializeWorld(index, WorldDataGenerator.GetRandomMeshData(index));
        return newWorld;
    }
    
    private void OnReleaseWorld(GameObject releasedWorld)
    {
        try
        {
            int worldIndex = int.Parse(releasedWorld.name);
            releasedWorld.SetActive(false);
        }
        catch (FormatException)
        {
            Debug.Log($"Unable to parse world: '{releasedWorld.name}'");
        }        
    }

    private void OnGetWorld(GameObject obj)
    {
        obj.SetActive(true);
    }
}
